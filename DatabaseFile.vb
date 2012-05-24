'********************************************************************************
'***
'*** Class      DatabaseFile
'*** Purpose    Utility class for working with Odbc and ADO databases
'***
'*** (c) Copyright 2007 Kofax Image Products.
'*** All rights reserved.
'***
'********************************************************************************
Imports System.Data.Common
Imports System.Data.Odbc
Imports System.Data.OleDb
Imports System.Runtime.InteropServices
Imports System.Text.RegularExpressions

Friend Class DatabaseFile
    Implements IDisposable

	'==================
	' Local Variables
	'==================
    Private rsIndex As DbDataAdapter '*** ADO Index Table Recordset
    Private rsDoc As DbDataAdapter '*** ADO Document Table Recordset

    '===================
    ' Public Variables
    '===================
    Private dbFactory As DbProviderFactory
    Friend cnADO As DbConnection '*** ADO Connection Object
    Public Connected As Boolean

    ' -- Current DB connection settings --
    Public CurrentDBType As Integer
    Public ConnString As String
    Public ConnSysMDB As String
    Public ConnUser As String
    Public ConnPassword As String

    Public CurrentITable As String
    Public CurrentDTable As String

    Public ErrorLineNum As Short

    '*** Connection String to use for ADO Connection with Access Database
    Private Const adoJetProvider As String = "Provider=Microsoft.Jet.OLEDB.4.0;Persist Security Info=False;Data Source="
    Private Const adoJetPassword As String = ";Password="
    Private Const adoJetSystemDb As String = ";Jet OLEDB:System Database="

    '*************************************************
    ' ConnectToDatabase
    '-------------------------------------------------
    ' Purpose:  This routine will establish a
    '           connection to the passed in database.
    ' Inputs:   ConnType        Database Type
    '           DatabaseName    Could be the Access
    '                           database filename or
    '                           the DSN.
    '           UserName        username
    '           Password        password
    ' Outputs:  None
    ' Returns:  None
    ' Notes:    This routine will close the current
    '           database connection if one exists
    '*************************************************
    Public Sub ConnectToDatabase(ByVal ConnType As Integer, ByVal DatabaseName As String, ByVal SystemMDB As String, ByVal UserName As String, ByVal Password As String)
        Dim connectionString As String

        ' If we are already connected to a database
        ' with the same settings, just exit
        If Connected = True And CurrentDBType = ConnType And ConnString = DatabaseName And ConnSysMDB = SystemMDB And ConnUser = UserName And ConnPassword = Password Then
            Exit Sub
        End If

        ' If we are connected to a database,
        ' close the database before continuing
        CleanUpDBConnection()
        Connected = False

        ' Build the proper connection string.  If ODBC, the
        ' options line of the OpenDatabase method accepts the
        ' connection string.  If Access, just pass in the
        ' filename
        Select Case ConnType
            Case DBTYPE_ACCESS
                dbFactory = OleDbFactory.Instance
                cnADO = dbFactory.CreateConnection()
                'Add the string to specify to use s engine with the database path
                connectionString = adoJetProvider & DatabaseName
                If String.IsNullOrEmpty(UserName) Then
                    UserName = "Admin"
                End If
                connectionString = String.Concat(connectionString, ";User ID=", UserName)
                If Not String.IsNullOrEmpty(Password) Then
                    connectionString = String.Concat(connectionString, adoJetPassword, Password)
                End If
                If Not String.IsNullOrEmpty(SystemMDB) Then
                    connectionString = String.Concat(connectionString, adoJetSystemDb, SystemMDB)
                End If

            Case DBTYPE_ODBC
                dbFactory = OdbcFactory.Instance
                cnADO = dbFactory.CreateConnection()
                connectionString = "DSN=" & DatabaseName
                If Not String.IsNullOrEmpty(UserName) Then
                    connectionString = String.Concat(connectionString, "; uid=", UserName)
                End If
                If Not String.IsNullOrEmpty(Password) Then
                    connectionString = String.Concat(connectionString, "; pwd=", Password)
                End If

            Case Else
                ' Invalid Database type
                Throw New ArgumentException(My.Resources.strBadDbType, "ConnType")

        End Select

        'If there is a connection failure, the exception is caught and handled elsewhere
        cnADO.ConnectionString = connectionString

        Dim nMaxAttempts As Integer = 10
        For i As Integer = 0 To nMaxAttempts
            Try
                cnADO.Open()
                '*** If we get a successful open, exit the loop and continue.
                Exit For
            Catch ex As Exception
                If (TypeOf ex Is OleDbException) OrElse _
                   (TypeOf ex Is OdbcException) Then
                    '*** If we are at our last attempt, throw the exception to the caller,
                    '*** otherwise we will re-try again.
                    If i = nMaxAttempts Then
                        Throw
                    End If
                    '*** Pause half a second, to allow time for the cause of the lock prevention
                    '*** to resolve (and it may not be possible).
                    System.Threading.Thread.Sleep(500)
                Else
                    '*** If any other exception occured during database open, throw to caller.
                    Throw
                End If
            End Try
        Next

        ' We are connected
        Connected = True

        ' Remember the current DB settings
        CurrentDBType = ConnType
        ConnString = DatabaseName
        ConnSysMDB = SystemMDB
        ConnUser = UserName
        ConnPassword = Password
    End Sub

    '*************************************************
    ' SetupForDocuments
    '-------------------------------------------------
    ' Purpose:  This routine creates the connection to
    '           the database and builds the recordsets
    '           for the index and documents table.
    ' Inputs:   TheData     ReleaseData object
    ' Outputs:  None
    ' Returns:  None
    ' Notes:    None
    '*************************************************
    Sub SetupForDocuments(ByRef TheData As Kofax.ReleaseLib.ReleaseData, _
                          ByRef oCustomPropReader As CustomPropertiesReaderWriter)
        ' First establish the connection to the database,
        ' then create the record sets to the two tables
        ' we'll need to update
        With TheData

            ConnectToDatabase(oCustomPropReader.ConnectionType, .ConnectString, _
                              oCustomPropReader.SystemMdb, .UserName, .Password)

            '*** Open the recordset as a dynaset.  This should only pull the current records when needed.
            '*** We're only using a recordset to avoid database specific field formatting.
            '*** That is, we don't use the contents of the recordsets, we only use them
            '*** to do an AddNew. Therefore, optimize the recordset creation by using a SQL
            '*** statement that never return any rows by adding "WHERE 0=1"
            rsIndex = CreateDataAdapter(GetTableSelect(.TableName))

            '*** Open the recordset as a dynaset.  This should only pull the current records when needed.
            rsDoc = CreateDataAdapter(GetTableSelect(oCustomPropReader.DocTable))

        End With
    End Sub

    '*************************************************
    ' ReleaseIndexes
    '-------------------------------------------------
    ' Purpose:  This is the main that releases the
    '           document indexes to the database.
    ' Inputs:   The release data object
    ' Outputs:  None
    ' Returns:  None
    ' Notes:    None
    '*************************************************
    Sub ReleaseIndexes(ByRef TheData As Kofax.ReleaseLib.ReleaseData, ByVal strImageFilePath As String, _
                       ByRef customPropertiesReader As CustomPropertiesReaderWriter)
        Using transaction As DbTransaction = cnADO.BeginTransaction()
            Try
                rsIndex.SelectCommand.Transaction = transaction
                rsDoc.SelectCommand.Transaction = transaction

                ' Release DocPath and DocID to documents table
                ReleaseDocTable(TheData, strImageFilePath, customPropertiesReader)

                ' Release Data to Index Table
                ReleaseIndexTable(TheData)
                transaction.Commit()
            Catch
                Try
                    transaction.Rollback()
                Catch
                End Try
                Throw
            End Try
        End Using
    End Sub

    '*************************************************
    ' ReleaseIndexTable
    '-------------------------------------------------
    ' Purpose:  This routine will release all of the
    '           linked Ascent values to the back end
    '           database.
    ' Inputs:   The release data object
    ' Outputs:  None
    ' Returns:  None
    ' Notes:    None
    '*************************************************
    Sub ReleaseIndexTable(ByRef TheData As Kofax.ReleaseLib.ReleaseData)
        Dim I As Integer

        ' Loop through each value and add it to the table.
        Try
            ' Create a new record in the index table for this document.
            ' The table may be locked so retry if necessary.
            Dim table As New DataTable()
            rsIndex.Fill(table)
            Dim row As DataRow = table.NewRow()

            For I = 0 To TheData.Values.Count - 1
                If Left(TheData.Values(I + 1).Destination, 3) <> "PDF" Then
                    row(TheData.Values(I + 1).Destination) = TheData.Values(I + 1).Value
                End If
            Next

            ' Finally, update the record.
            ' The table may be locked so retry if necessary.
            table.Rows.Add(row)
            rsIndex.Update(table)
        Catch ex As InvalidCastException
            Dim sField As String

            ' A datatype conversion error occurred.
            ' Show the name of the Ascent index value.
            Select Case TheData.Values(I + 1).SourceType
                Case Kofax.ReleaseLib.KfxLinkSourceType.KFX_REL_TEXTCONSTANT
                    sField = String.Format("""{0}""", TheData.Values(I + 1).SourceName)
                Case Kofax.ReleaseLib.KfxLinkSourceType.KFX_REL_VARIABLE
                    sField = String.Format("{{{0}}}", TheData.Values(I + 1).SourceName)
                Case Kofax.ReleaseLib.KfxLinkSourceType.KFX_REL_INDEXFIELD
                    sField = TheData.Values(I + 1).SourceName
                Case Kofax.ReleaseLib.KfxLinkSourceType.KFX_REL_BATCHFIELD
                    sField = String.Format("{{${0}}}", TheData.Values(I + 1).SourceName)
                Case Kofax.ReleaseLib.KfxLinkSourceType.KFX_REL_DOCUMENTID
                    sField = TheData.Values(I + 1).SourceName
                Case Else
                    Throw
            End Select
            Throw New DataConvertException( _
                String.Format("{0} ({1}): {2} ({3} -> {4})", _
                    My.Resources.strIndexColumnError, TheData.Values(I + 1).Destination, _
                    Err.Description, sField, TheData.Values(I + 1).Destination), ex)
        Catch ex As ArgumentException
            ' An expected column was not found in the index table.
            ' Show a more meaningful error message.
            Throw New IndexColumnMissingException( _
                String.Format("{0} ({1}): {2}", My.Resources.strIndexColumnError, _
                TheData.Values(I + 1).Destination, _
                My.Resources.strIndexColumnMissing), ex)
        End Try
    End Sub

    '*************************************************
    ' ReleaseDocumentTable
    '-------------------------------------------------
    ' Purpose:  This routine releases the DocID and
    '           DocPath to the documents table.
    ' Inputs:   TheData     ReleaseData object
    '           PathName    optional release path
    ' Outputs:  None
    ' Returns:  None
    ' Notes:    None
    '*************************************************
    Sub ReleaseDocTable(ByRef TheData As Kofax.ReleaseLib.ReleaseData, ByVal PathName As String, _
                        ByRef customPropertiesReader As CustomPropertiesReaderWriter)
        Dim sPathName As String = String.Empty

        ' If we are given the optional PathName use it.
        If Not String.IsNullOrEmpty(PathName) Then
            sPathName = PathName
        End If

        ' Add new record to the documents table for this document.
        ' The table may be locked so retry if necessary.
        Dim strDocPath As String = customPropertiesReader.DocPath
        Dim strDocID As String = customPropertiesReader.DocId
        Dim strDocTable As String = customPropertiesReader.DocTable
        Try
            Dim table As New DataTable()
            rsDoc.Fill(table)
            Dim row As DataRow = table.NewRow()

            ' Add the document path and the Document ID
            Try
                row(strDocID) = TheData.UniqueDocumentID
            Catch ex As ArgumentException
                ' Always prefix the error message to indicate that an error
                ' occurred trying to set a column value in the Document Table
                Throw New DocumentColumnException( _
                    String.Format("{0} ({1}): {2}", _
                        My.Resources.strDocColumnError, _
                        strDocID, _
                        My.Resources.strDocColumnMissing), ex)
            End Try

            Try
                row(strDocPath) = sPathName
            Catch ex As ArgumentException
                ' Always prefix the error message to indicate that an error
                ' occurred trying to set a column value in the Document Table
                Throw New DocumentColumnException( _
                    String.Format("{0} ({1}): {2}", _
                        My.Resources.strDocColumnError, _
                        strDocPath, _
                        My.Resources.strDocColumnMissing), ex)
            End Try

            ' Update the table.
            ' The table may be locked so retry if necessary.
            table.Rows.Add(row)
            rsDoc.Update(table)
        Catch ex As OleDbException
            Dim oleDbError As OleDbError = ex.Errors(0)
            If oleDbError.SQLState = "3022" Then
                ' A duplicate index or primary key exists.  Change the message to indicate
                ' a record already exists for this document.
                Throw New DuplicateKeyException( _
                    String.Format("{0}: {1}", _
                        My.Resources.strDocAlreadyReleased, _
                        VB6.Format(TheData.UniqueDocumentID)), ex)
            ElseIf oleDbError.SQLState = "3073" Then
                ' Error indicates that the database or object is read only.
                ' However, this error is also seen when the table is lacking a
                ' unique index, so advise of this probability.
                Throw New ReadOnlyException( _
                    String.Format("{0} {1}", ex.Message, My.Resources.strReadOnlyError), ex)
            End If

            ' Always prefix the error message to indicate that an error
            ' occurred trying to update a record in the Document Table
            Throw New DocumentTableException( _
                String.Format("{0} ({1}): {2}", _
                    My.Resources.strDocTableError, _
                    strDocTable, _
                    ex.Message), ex)
        Catch ex As OdbcException
            Dim odbcError As OdbcError = ex.Errors(0)
            If odbcError.SQLState = "23000" Then
                ' A duplicate index or primary key exists.  Change the message to indicate
                ' a record already exists for this document.
                Throw New DuplicateKeyException( _
                    String.Format("{0}: {1}", _
                        My.Resources.strDocAlreadyReleased, _
                        VB6.Format(TheData.UniqueDocumentID)), ex)
            ElseIf odbcError.SQLState = "25000" Then
                ' Error indicates that the database or object is read only.
                ' However, this error is also seen when the table is lacking a
                ' unique index, so advise of this probability.
                Throw New ReadOnlyException( _
                    String.Format("{0} {1}", ex.Message, My.Resources.strReadOnlyError), ex)
            End If

            ' Always prefix the error message to indicate that an error
            ' occurred trying to update a record in the Document Table
            Throw New DocumentTableException( _
                String.Format("{0} ({1}): {2}", _
                    My.Resources.strDocTableError, _
                    strDocTable, _
                    ex.Message), ex)
        End Try
    End Sub

    '*************************************************
    ' CleanUpDBConnection
    '-------------------------------------------------
    ' Purpose:  This routine will close down all the
    '           connections to the database.  This
    '           should be done as the script exits.
    ' Inputs:   None
    ' Outputs:  None
    ' Returns:  None
    ' Notes:    None
    '*************************************************
    Sub CleanUpDBConnection()
        ' Close the database
        If Connected Then
            ' Close the recordsets
            If Not rsIndex Is Nothing Then
                rsIndex.Dispose()
                rsIndex = Nothing
            End If
            If Not rsDoc Is Nothing Then
                rsDoc.Dispose()
                rsDoc = Nothing
            End If

            ' Disconnect and cleanup
            If Not cnADO Is Nothing Then
                cnADO.Dispose()
                cnADO = Nothing
            End If

            CurrentDTable = String.Empty
            CurrentITable = String.Empty
            Connected = False

            dbFactory = Nothing
        End If
    End Sub

    '*************************************************
    ' SetupForValidation
    '-------------------------------------------------
    ' Purpose:  This routine creates the connection to
    '           the database and builds the recordsets
    '           for the index and documents table.
    ' Inputs:   SetupData     ReleaseSetupData object
    '*************************************************
    Public Sub SetupForValidation(ByRef setupData As Kofax.ReleaseLib.ReleaseSetupData)
        Using customPropertiesReader As New CustomPropertiesReaderWriter(setupData.CustomProperties)
            ' Establish the connection to the database
            ConnectToDatabase(customPropertiesReader.ConnectionType, setupData.ConnectString, customPropertiesReader.SystemMdb, setupData.UserName, setupData.Password)

            ' Create the record sets to the two tables
            rsIndex = CreateDataAdapter(GetTableSelect(setupData.TableName))
            rsDoc = CreateDataAdapter(GetTableSelect(customPropertiesReader.DocTable))
        End Using
    End Sub

    '*************************************************
    ' ValidateIndexTable
    '-------------------------------------------------
    ' Purpose:  This routine checks if each required
    '           column in the database has been linked
    '           to an Ascent value.
    ' Inputs:   The values collection
    ' Outputs:  None
    ' Returns:  True if all required fields have a
    '           link established to an Ascent value
    ' Notes:    None
    '*************************************************
    Public Function ValidateIndexTable(ByRef TheLinks As Kofax.ReleaseLib.Links) As Boolean
        Dim I As Integer

        ' Check all the columns in the index table
        Using reader As DbDataReader = _
            rsIndex.SelectCommand.ExecuteReader(CommandBehavior.SchemaOnly Or CommandBehavior.KeyInfo)
            Using schemaTable As DataTable = reader.GetSchemaTable()
                For Each schemaRow As DataRow In schemaTable.Rows
                    ' We are only concerned with the required columns
                    If Not CBool(schemaRow("AllowDBNull")) AndAlso Not CBool(schemaRow("IsAutoIncrement")) Then
                        ' Check if any link is established to this column
                        For I = 1 To TheLinks.Count
                            If TheLinks(I).Destination = CType(schemaRow("ColumnName"), String) Then
                                ' We found a link.  No need to keep looking.
                                GoTo Found
                            End If
                        Next
                        ' Was a link found?
                        Return False
Found:
                    End If
                Next

                ' Every required column had a link
                Return True
            End Using
        End Using
    End Function

    ''' <summary>
    ''' This routine delimits table names when they include a space
    ''' </summary>
    ''' <param name="strTableName">The table name</param>
    ''' <returns>SELECT statement for table</returns>
    ''' <remarks></remarks>
    Public Function GetTableSelect(ByVal strTableName As String) As String
        Return String.Concat("SELECT * FROM [", Regex.Replace(strTableName, "\.", "].["), "] WHERE 1=0")
    End Function

    ''' <summary>
    ''' This routine delimits column and table names when they include a space.
    ''' </summary>
    ''' <param name="strTableName">The table name</param>
    ''' <param name="strColumnName">The column name</param>
    ''' <returns>SELECT statement for a column in a table</returns>
    ''' <remarks></remarks>
    Public Function GetColumnSelect(ByVal strTableName As String, ByVal strColumnName As String) As String
        Return String.Concat("SELECT [", strColumnName, "] FROM [", Regex.Replace(strTableName, "\.", "].["), "] WHERE 1=0")
    End Function

    Private disposedValue As Boolean = False        ' To detect redundant calls

    ' IDisposable
    Protected Overridable Sub Dispose(ByVal disposing As Boolean)
        If Not Me.disposedValue Then
            If disposing Then
                CleanUpDBConnection()
            End If
        End If
        Me.disposedValue = True
    End Sub

    ''' <summary>
    ''' Utility function to create an ADO.NET command object
    ''' using the stored DbConnection
    ''' </summary>
    ''' <param name="commandText">The SQL text of the command</param>
    ''' <returns>An initialized DbCommand</returns>
    ''' <remarks></remarks>
    Friend Function CreateCommand(ByVal commandText As String) As DbCommand
        Dim command As DbCommand = dbFactory.CreateCommand()
        command.Connection = cnADO
        command.CommandText = commandText
        Return command
    End Function

    ''' <summary>
    ''' Utility function to create an ADO.NET data adapter
    ''' using the stored DbConnection
    ''' </summary>
    ''' <param name="commandText">The SQL SELECT command to use</param>
    ''' <returns>An initialized DbDataAdapter</returns>
    ''' <remarks></remarks>
    Friend Function CreateDataAdapter(ByVal commandText As String) As DbDataAdapter
        Dim dataAdapter As DbDataAdapter = dbFactory.CreateDataAdapter()
        dataAdapter.SelectCommand = CreateCommand(commandText)
        Dim commandBuilder As DbCommandBuilder = dbFactory.CreateCommandBuilder()
        '*** Access and SQL require brackets around names that have spaces or
        '*** match reserved words.
        commandBuilder.QuotePrefix = "["
        commandBuilder.QuoteSuffix = "]"
        commandBuilder.DataAdapter = dataAdapter
        Return dataAdapter
    End Function

#Region " IDisposable Support "
    ' This code added by Visual Basic to correctly implement the disposable pattern.
    Public Sub Dispose() Implements IDisposable.Dispose
        ' Do not change this code.  Put cleanup code in Dispose(ByVal disposing As Boolean) above.
        Dispose(True)
        GC.SuppressFinalize(Me)
    End Sub
#End Region
End Class