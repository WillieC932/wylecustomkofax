'****************************************************************************
'*	(c) Copyright Kofax Image Products, 2004-2008. All rights reserved.
'*	Unauthorized use, duplication or distribution is strictly prohibited.
'*****************************************************************************
'*
'*	File:		Main.vb
'*
'*	Purpose:	Dialog for the Adobe Acrobat settings for export connectors
'*
'****************************************************************************/
Imports Kofax.CapLib4.Interop
Imports Kofax.ReleaseLib
Imports System.Collections.Generic
Imports System.ComponentModel
Imports System.Data.Common
Imports System.Data.Odbc
Imports System.IO
Imports System.Runtime.InteropServices
Imports System.Text.RegularExpressions
Imports SourceType = Kofax.ReleaseLib.KfxLinkSourceType
Imports VB = Microsoft.VisualBasic

Friend Class frmSetup
    Inherits System.Windows.Forms.Form

    ' Do not internationalize this string.  This is the
    ' default username the JET database engine uses to
    ' log into an Access database.  We want to display
    ' this to the user so they realize the username
    ' by which they are connecting to the database.
    Private Const DEFAULT_USERNAME As String = "Admin"

    '======================
    ' Object Declarations
    '======================
    ' Remember to release these in the Form_Unload event
    Private m_setupData As Kofax.ReleaseLib.ReleaseSetupData
    Private oDB As New DatabaseFile
    
    '=======================
    ' Form Level Variables
    '=======================
    Private fDirty As Boolean
    Private links As New List(Of Link)
    Private errorProvider As New ErrorProvider(components)
    Private indexValuesSelectedRow As DataGridViewRow
    Private m_bSuppressTextChangeEvent As Boolean = False

    '===================
    ' Global Variables
    '===================
    Public gNewLinkType As SourceType
    Public gNewLinkData As String

    Public Sub New()
        MyBase.New()
        'This call is required by the Windows Form Designer.
        InitializeComponent()
    End Sub

    '*************************************************
    ' Dirty [Let Property]
    '-------------------------------------------------
    ' Purpose:  The dirty property will set the
    '           current status of the data.  If
    '           the data is dirty, the Apply
    '           button is enabled.
    ' Inputs:   NewStatus   Boolean indicating if
    '                       data is dirty (TRUE)
    '                       or clean (FALSE)
    ' Outputs:  None
    ' Returns:  None
    ' Notes:    None
    '*************************************************

    '*************************************************
    ' Dirty [Get Property]
    '-------------------------------------------------
    ' Purpose:  The dirty property will return
    '           the current status of the data.
    ' Inputs:   None
    ' Outputs:  TRUE if the data is dirty
    '           FALSE if the data is clean
    ' Returns:  None
    ' Notes:    None
    '*************************************************
    Public Property Dirty() As Boolean
        Get
            Return fDirty
        End Get
        Set(ByVal Value As Boolean)
            fDirty = Value
            cmdApply.Enabled = fDirty
        End Set
    End Property

    '*************************************************
    ' AddString
    '-------------------------------------------------
    ' Purpose:  Formats a specified string to be
    '           appended on a new line of another
    '           string.  If more than 10 strings
    '           have been appended, the function
    '           substitutes the phrase "And More"
    '           for the specified string.
    ' Inputs:   nCount  the number of strings that
    '                   have been appended.
    '           sField  the string to append
    ' Outputs:  None
    ' Returns:  The formatted string
    ' Notes:    This function is used solely by the
    '           data verification routines to list
    '           the Index Fields and Batch Fields
    '           that were not used as document
    '           Index Values.
    '*************************************************
    Function AddString(ByRef nCount As Integer, ByRef sField As String) As String
        nCount = nCount + 1
        If nCount < 10 Then
            Return vbCrLf & vbTab & "- " & sField
        ElseIf nCount = 10 Then
            Return vbCrLf & vbTab & My.Resources.strAndMore
        Else
            Return String.Empty
        End If
    End Function

    '*************************************************
    ' BreakAllLinks
    '-------------------------------------------------
    ' Purpose:  This routine will clear all linking
    '           information from the list of links,
    '           disconnect from the DB, and clear
    '           the info on the table settings tab.
    ' Inputs:   None
    ' Outputs:  None
    ' Returns:  None
    ' Notes:    None
    '*************************************************
    Function BreakAllLinks() As Boolean
        If oDB.Connected Then
            If DialogResult.Yes = _
                MessageBox.Show(My.Resources.strBreakAllDbLinks, My.Resources.strTitleDatabase, MessageBoxButtons.YesNo, MessageBoxIcon.Exclamation) Then

                ' If there are links, break them all
                For Each link As Link In links
                    link.Source = String.Empty
                    link.SourceType = SourceType.KFX_REL_UNDEFINED_LINK
                Next

                ' Disconnect from the database
                oDB.CleanUpDBConnection()

                ' Clear the settings on the Table tab
                ClearTableTab()
                Return True
            Else
                Return False
            End If
        Else
            Return True
        End If
    End Function

    '*************************************************
    ' BuildLinkList
    '-------------------------------------------------
    ' Purpose:  This routine builds the list of links
    '           from column names in the specified
    '           database table.
    ' Inputs:   TableName    Database table name
    ' Outputs:  None
    ' Returns:  None
    ' Notes:    Must be connected to the database
    '           before calling this function.
    '*************************************************
    Sub BuildLinkList(ByRef TableName As String)
        Using command As DbCommand = oDB.CreateCommand(oDB.GetTableSelect(TableName))
            Using reader As DbDataReader = _
                command.ExecuteReader(CommandBehavior.SchemaOnly Or CommandBehavior.KeyInfo)
                Using schemaTable As DataTable = reader.GetSchemaTable()
                    '*** Loop through the fields to add them to link collection
                    indexValuesSelectedRow = Nothing
                    links = New List(Of Link)

                    If Not schemaTable Is Nothing Then
                        For Each schemaRow As DataRow In schemaTable.Rows
                            If Not CBool(schemaRow("IsAutoIncrement")) Then
                                links.Add(New Link(CStr(schemaRow("ColumnName"))))
                            End If
                        Next
                    Else
                        MsgBox(String.Format(My.Resources.strNotValidTable, TableName), _
                            MsgBoxStyle.OkOnly Or MsgBoxStyle.Exclamation, _
                            My.Resources.strTitleDatabase)
                        oDB.CurrentITable = String.Empty
                        Return
                    End If
                End Using
            End Using
        End Using

        If links.Count = 0 Then
            ' Tell the user and reject the table.
            MsgBox(My.Resources.strNotValidIndexTable, _
                MsgBoxStyle.OkOnly Or MsgBoxStyle.Exclamation, _
                My.Resources.strTitleDatabase)
            oDB.CurrentITable = String.Empty
        Else
            indexValuesDataGridView.AutoGenerateColumns = False
            indexValuesDataGridView.DataSource = links
            indexValuesDataGridView.ClearSelection()

            oDB.CurrentITable = TableName
        End If
    End Sub

    '*************************************************
    ' cboDBTypes_Click
    '-------------------------------------------------
    ' Purpose:  This routine handles the event of the
    '           user changing the database type.  The
    '           user is prompted to back out of making
    '           the change.  If the user chooses to
    '           continue with the change, all of the
    '           Index Value links to the database are
    '           broken.
    '*************************************************
    Private Sub cboDBTypes_SelectedIndexChanged(ByVal eventSender As System.Object, ByVal eventArgs As System.EventArgs) Handles cboDBTypes.SelectedIndexChanged
        With cboDBTypes
            ' Check if the user selected a different database type
            If oDB.CurrentDBType <> cboDBTypes.SelectedIndex + 1 Then
                ' If the current database settings have been used to
                ' connect with the database, warn the user that by
                ' changing this setting they will lose all of their
                ' current settings.
                If BreakAllLinks() Then
                    Me.Dirty = True
                    SetDBTypeUI(cboDBTypes.SelectedIndex + 1)
                    oDB.CurrentDBType = cboDBTypes.SelectedIndex + 1
                Else
                    ' Otherwise, set the combo box back to
                    ' it original setting
                    .SelectedIndex = oDB.CurrentDBType - 1
                End If
            End If
        End With
    End Sub

    '*************************************************
    ' cboDocPath_Change
    '-------------------------------------------------
    ' Purpose:  The user selected a different column
    '           in the database to store the path to
    '           the image file.  Mark the data dirty.
    '*************************************************
    Private Sub cboDocPath_TextChanged(ByVal eventSender As System.Object, ByVal eventArgs As System.EventArgs) _
        Handles cboDocPath.TextChanged, cboDocID.TextChanged

        Dirty = True
    End Sub

    '*************************************************
    ' cboDTables_Click
    '-------------------------------------------------
    ' Purpose:  The user selected a different table
    '           from the database to store document
    '           information.  Prompt the user to back
    '           out the change.  If the user chooses
    '           to continue, the DocID and DocPath
    '           combo boxes are cleared and refilled
    '           with the columns in the new table.
    ' Inputs:   None
    ' Outputs:  None
    ' Returns:  None
    ' Notes:    This event can be re-entrant.  If the
    '           user chooses to cancel the change, we
    '           programatically reset the combo box
    '           value which retriggers this event.
    '           We therefore use a local static flag
    '           to ignore this subsequent call.
    '*************************************************
    Private Sub cboDTables_SelectedIndexChanged(ByVal eventSender As System.Object, ByVal eventArgs As System.EventArgs) Handles cboDTables.SelectedIndexChanged
        Dim Results As Integer

        With cboDTables
            ' Only display the message box if the Document Table
            ' listbox has been changed AND either the DocID or DocPath
            ' drop-down listboxes have a selection.  Otherwise, just do
            ' the change.
            If oDB.CurrentDTable <> VB6.GetItemString(cboDTables, .SelectedIndex) And oDB.CurrentDTable <> "" And (cboDocID.SelectedIndex <> -1 Or cboDocPath.SelectedIndex <> -1) Then
                ' Warn the user that any current settings will be lost.
                Results = MsgBox(My.Resources.strBreakDocIdPath, _
                    MsgBoxStyle.YesNo Or MsgBoxStyle.Question, My.Resources.strTitleDatabase)
            Else
                Results = MsgBoxResult.Yes
            End If

            ' Only reload the DocID and DocPath dropdown listboxes if
            ' the user has OKed it AND the Document Table was actually
            ' changed.  If the user cancelled a change, set the listbox
            ' back to the previous setting.  Otherwise, just exit.
            If Results = MsgBoxResult.Yes And oDB.CurrentDTable <> VB6.GetItemString(cboDTables, .SelectedIndex) Then
                Try
                    Me.Dirty = True
                    FillDocLists(VB6.GetItemString(cboDTables, .SelectedIndex))
                Catch e As Exception
                    MsgBox(e.Message, _
                        MsgBoxStyle.OkOnly Or MsgBoxStyle.Exclamation, _
                        My.Resources.strTitleDatabase)
                    SetComboValue(cboDTables, oDB.CurrentDTable)
                End Try
            ElseIf Results = MsgBoxResult.No Then
                SetComboValue(cboDTables, oDB.CurrentDTable)
            End If
        End With
    End Sub

    '*************************************************
    ' cboImageType_Click
    '-------------------------------------------------
    ' Purpose:  The user selected a different image
    '           format.  Mark the data dirty.  If
    '           they chose PDF, enable the controls
    '           to define the PDF settings.
    ' Inputs:   None
    ' Outputs:  None
    ' Returns:  None
    ' Notes:    None
    '*************************************************
    Private Sub cboImageType_SelectedIndexChanged( _
        ByVal eventSender As System.Object, _
        ByVal eventArgs As System.EventArgs) _
        Handles cboImageType.SelectedIndexChanged

        Dim imageType As ImageTypeComboBoxItem = CType(cboImageType.SelectedItem, ImageTypeComboBoxItem)
        If imageType.Type <> m_setupData.ImageType Then
            Dirty = True
        End If

    End Sub

    '*************************************************
    ' cboITables_Click
    '-------------------------------------------------
    ' Purpose:  The user selected a different table
    '           from the database to store index
    '           information.  Prompt the user to back
    '           out the change.  If the user chooses
    '           to continue, all of the Index Value
    '           links are broken and the link control
    '           is repopulated with the columns from
    '           the new table.
    ' Inputs:   None
    ' Outputs:  None
    ' Returns:  None
    ' Notes:    This event can be re-entrant.  If the
    '           user chooses to cancel the change, we
    '           programatically reset the combo box
    '           value which retriggers this event.
    '           We therefore use a local static flag
    '           to ignore this subsequent call.
    '*************************************************
    Private Sub cboITables_SelectedIndexChanged(ByVal eventSender As System.Object, ByVal eventArgs As System.EventArgs) Handles cboITables.SelectedIndexChanged
        Dim Results As Integer

        With cboITables
            ' Display the message box if a new table was selected AND if there
            ' is at least one entry linked.  Otherwise, just make the switch.
            If VB6.GetItemString(cboITables, .SelectedIndex) <> oDB.CurrentITable And oDB.CurrentITable <> "" And LinksExist() Then
                Results = MsgBox(My.Resources.strBreakIndexLinks, _
                    MsgBoxStyle.YesNo Or MsgBoxStyle.Question, My.Resources.strTitleDatabase)
            Else
                Results = MsgBoxResult.Yes
            End If

            ' Reload the link box if it's OK AND a new table was selected.
            ' If the user cancelled the table change, set the table back to
            ' it's previous setting.  Otherwise, just exit.
            If Results = MsgBoxResult.Yes And VB6.GetItemString(cboITables, .SelectedIndex) <> oDB.CurrentITable Then
                Try
                    Dirty = True
                    BuildLinkList(VB6.GetItemString(cboITables, .SelectedIndex))
                Catch e As Exception
                    MsgBox(e.Message, _
                        MsgBoxStyle.OkOnly Or MsgBoxStyle.Exclamation, _
                        My.Resources.strTitleDatabase)
                    SetComboValue(cboITables, oDB.CurrentITable)
                End Try
            ElseIf Results = MsgBoxResult.No Then
                SetComboValue(cboITables, oDB.CurrentITable)
            End If
        End With
    End Sub

    '*************************************************
    ' chkReleaseOCRFullText_Click
    '-------------------------------------------------
    ' Purpose:  The user toggled whether or not to
    '           release OCR Full Text of each
    '           document.  It enables/disables
    '           controls that are used for OCR Full
    '           Text setup.  Mark the data dirty.
    ' Inputs:   None
    ' Outputs:  None
    ' Returns:  None
    ' Notes:    None
    '*************************************************
    Private Sub chkReleaseOCRFullText_CheckStateChanged(ByVal eventSender As System.Object, ByVal eventArgs As System.EventArgs) Handles chkReleaseOCRFullText.CheckStateChanged
        lblOCRDir.Enabled = chkReleaseOCRFullText.Checked
        txtOCRDir.Enabled = chkReleaseOCRFullText.Checked
        cmdOCRBrowse.Enabled = chkReleaseOCRFullText.Checked
        Dirty = True
    End Sub

    '*************************************************
    ' chkReleaseKofaxPDF_Click
    '-------------------------------------------------
    ' Purpose:  The user toggled whether or not to
    '           release Kofax PDF files of each
    '           document.  It enables/disables
    '           various controls that are used to
    '           to release images.
    '           Mark the data dirty.
    '*************************************************
    Private Sub chkReleaseKofaxPDF_CheckStateChanged(ByVal eventSender As System.Object, ByVal eventArgs As System.EventArgs) Handles chkReleaseKofaxPDF.CheckStateChanged
        lblPDFDir.Enabled = chkReleaseKofaxPDF.Checked
        txtKofaxPDFDir.Enabled = chkReleaseKofaxPDF.Checked
        cmdPDFBrowse.Enabled = chkReleaseKofaxPDF.Checked
        Dirty = True
    End Sub

    '*************************************************
    ' chkSkipFirstPage_Click
    '-------------------------------------------------
    ' Purpose:  The user toggled whether or not to
    '           release the first page of each
    '           document.  Mark the data dirty.
    '*************************************************
    Private Sub chkSkipFirstPage_CheckStateChanged(ByVal eventSender As System.Object, ByVal eventArgs As System.EventArgs) Handles chkSkipFirstPage.CheckStateChanged
        Dirty = True
    End Sub

    '*************************************************
    ' ClearTableTab
    '-------------------------------------------------
    ' Purpose:  This routine will clear the table
    '           settings tab of any current settings.
    '           This includes the link box.
    ' Inputs:   None
    ' Outputs:  None
    ' Returns:  None
    ' Notes:    None
    '*************************************************
    Sub ClearTableTab()
        ' Clear the Index Table frame by setting
        ' the table combo to no selection, clearing
        ' any link table info and making the scroll
        ' bar disappear
        cboITables.Items.Clear()

        ' Clear the Documents Table frame by clearing
        ' all of the combo boxes
        cboDTables.Items.Clear()
        cboDocID.Items.Clear()
        cboDocID.Enabled = False
        lblDocID.Enabled = False
        cboDocPath.Items.Clear()
        cboDocPath.Enabled = False
        lblDocPath.Enabled = False
    End Sub

    '*************************************************
    ' cmdApply_Click
    '-------------------------------------------------
    ' Purpose:  Verify the settings.  If there are
    '           no errors, save the changes and
    '           allow the user to continue editting.
    ' Inputs:   None
    ' Outputs:  None
    ' Returns:  None
    ' Notes:    If the settings are validated and
    '           saved, the data is marked clean.
    '*************************************************
    Private Sub cmdApply_Click(ByVal eventSender As System.Object, ByVal eventArgs As System.EventArgs) Handles cmdApply.Click
        SaveReleaseSettings()
    End Sub

    '*************************************************
    ' cmdConnBrowse_Click
    '-------------------------------------------------
    ' Purpose:  This routine will display either the
    '           File Open common dialog or an ODBC
    '           data source selection dialog depending
    '           upon the database type.
    ' Inputs:   None
    ' Outputs:  None
    ' Returns:  None
    ' Notes:    None
    '*************************************************
    Private Sub cmdConnBrowse_Click(ByVal eventSender As System.Object, ByVal eventArgs As System.EventArgs) Handles cmdConnBrowse.Click
        Try
            Select Case oDB.CurrentDBType

                ' If selecting an Access database,
                ' simply use the File Open common dialog.
                Case DBTYPE_ACCESS
                    Dim oOpenFileDialog As New OpenFileDialog()
                    oOpenFileDialog.CheckFileExists = True
                    oOpenFileDialog.CheckPathExists = True
                    oOpenFileDialog.ShowReadOnly = False

                    ' Set the filter to *.MDB
                    oOpenFileDialog.Filter = String.Format( _
                        "{0} (*.mdb)|*.mdb|{1} (*.*)|*.*", _
                        My.Resources.strAccessDatabases, _
                        My.Resources.strAllFiles)

                    ' Initialize the dialog to the file and path specified in the text box.
                    ' If nothing is specified and no previous default directory has been
                    ' set, default to C:\
                    If Not String.IsNullOrEmpty(txtConnString.Text) Then
                        If txtConnString.Text.EndsWith(".mdb") Then
                            oOpenFileDialog.FileName = txtConnString.Text
                            If Not String.IsNullOrEmpty(Path.GetDirectoryName(txtConnString.Text)) Then
                                oOpenFileDialog.InitialDirectory = Path.GetDirectoryName(txtConnString.Text)
                            End If
                        Else
                            oOpenFileDialog.FileName = String.Empty
                            oOpenFileDialog.InitialDirectory = txtConnString.Text
                        End If
                    Else
                        oOpenFileDialog.FileName = String.Empty
                        If String.IsNullOrEmpty(oOpenFileDialog.InitialDirectory) Then
                            oOpenFileDialog.InitialDirectory = Directory.GetDirectoryRoot(Environment.SystemDirectory)
                        End If
                    End If

                    If DialogResult.OK = oOpenFileDialog.ShowDialog() Then
                        txtConnString.Text = oOpenFileDialog.FileName
                        Dirty = True
                    End If

                    ' If selecting an ODBC data source,
                    ' display a list of machine data sources.
                Case DBTYPE_ODBC
                    ' Load the dialog with a list of valid ODBC machine data sources.
                    Using oForm As New frmODBC
                        ' If a data source was selected from the dialog, get the
                        ' connection string from the temporary database connection.
                        ' Then parse the data source name, user name, and password.
                        If DialogResult.OK = oForm.ShowDialog(Me) Then
                            Dirty = True
                            txtConnString.Text = GetDsnValue(oForm.ConnectionString, "dsn")
                            txtUserName.Text = GetDsnValue(oForm.ConnectionString, "uid")
                            txtPassword.Text = GetDsnValue(oForm.ConnectionString, "pwd")
                        End If
                    End Using
            End Select
        Catch ex As OdbcFailedException
            MessageBox.Show(My.Resources.strOdbcFailed, My.Resources.strTitleReleaseSetupError, MessageBoxButtons.OK, MessageBoxIcon.Exclamation)
        End Try
    End Sub

    ''' <summary>
    ''' Return the value of a DSN attribute by name
    ''' </summary>
    ''' <param name="connectionString">The full connection string to search</param>
    ''' <param name="name">The DSN attribute to get the value for</param>
    ''' <returns>The value associated with the DSN attribute</returns>
    ''' <remarks></remarks>
    Private Function GetDsnValue(ByVal connectionString As String, ByVal name As String) As String
        Dim match As Match
        match = Regex.Match( _
            connectionString, _
            String.Format("\b{0}.*?=(?<{0}>[^;]*)", name), _
            RegexOptions.IgnoreCase)
        Return match.Groups(name).Value
    End Function

    '*************************************************
    ' cmdHelp_Click
    '-------------------------------------------------
    ' Purpose:  Display the help topic for the tab
    '           that is currently displayed.
    ' Inputs:   None
    ' Outputs:  None
    ' Returns:  None
    ' Notes:    Each tab on the SSTab control has
    '           a unique Help Context ID.  We add
    '           the tab index to the first ID to
    '           display the appropriate help info.
    '           If additional tabs are added to the
    '           SSTab control, their Help Context IDs
    '           must be kept sequential.
    '           This export connector uses a proprietary
    '           COM object for its help system.
    '*************************************************
    Private Sub cmdHelp_Click(ByVal eventSender As System.Object, ByVal eventArgs As System.EventArgs) Handles cmdHelp.Click
        Help.Show(tabDatabase.SelectedIndex + TABS_FIRST_HELPID)
    End Sub

    '*************************************************
    ' cmdIFBrowse_Click
    '-------------------------------------------------
    ' Purpose:  Initialize and display the dialog
    '           allowing the user to browse for the
    '           directory where images will be
    '           stored during Release.  Mark the
    '           data dirty if the user selects a
    '           directory.
    ' Inputs:   None
    ' Outputs:  None
    ' Returns:  None
    ' Notes:    We store the Help Context ID in the
    '           dialog's Tag property since it is
    '           used for multiple purposes.
    '*************************************************
    Private Sub cmdIFBrowse_Click(ByVal eventSender As System.Object, ByVal eventArgs As System.EventArgs) Handles cmdIFBrowse.Click
        Dim dialog As New FolderBrowserDialog()
        dialog.Description = My.Resources.strTitleSelectImgDir
        dialog.SelectedPath = txtImageDir.Text
        If DialogResult.OK = dialog.ShowDialog() Then
            txtImageDir.Text = dialog.SelectedPath
            Me.Dirty = True
        End If
    End Sub

    '*************************************************
    ' cmdOCRBrowse_Click
    '-------------------------------------------------
    ' Purpose:  Initialize and display the dialog
    '           allowing the user to browse for the
    '           directory where OCR Full Text files
    '           will be stored during Release. Mark
    '           the data dirty if the user selects
    '           a directory.
    ' Inputs:   None
    ' Outputs:  None
    ' Returns:  None
    ' Notes:    We store the Help Context ID in the
    '           dialog's Tag property since it is
    '           used for multiple purposes.
    '*************************************************
    Private Sub cmdOCRBrowse_Click(ByVal eventSender As System.Object, ByVal eventArgs As System.EventArgs) Handles cmdOCRBrowse.Click
        Dim dialog As New FolderBrowserDialog()
        dialog.Description = My.Resources.strTitleSelectOcrDir
        dialog.SelectedPath = txtOCRDir.Text
        If DialogResult.OK = dialog.ShowDialog() Then
            txtOCRDir.Text = dialog.SelectedPath
            Me.Dirty = True
        End If
    End Sub

    '*************************************************
    ' cmdOK_Click
    '-------------------------------------------------
    ' Purpose:  Validates the settings if they are
    '           dirty and saves them before exiting
    '           from the Release Setup script.
    ' Inputs:   None
    ' Outputs:  None
    ' Returns:  None
    ' Note:     The first time the Release Setup
    '           script is run for a Doc Class, it
    '           is not marked Dirty initially.
    '           Therefore the OK button still needs
    '           to validate the settings because the
    '           defaults are not complete.  However,
    '           once the data has been changed and
    '           validated, the fVerified flag is set
    '           True.  This means that OK only needs
    '           to verify settings when the form is
    '           Dirty from that point forward.
    '*************************************************
    Private Sub cmdOK_Click(ByVal eventSender As System.Object, ByVal eventArgs As System.EventArgs) Handles cmdOK.Click
        If Not SaveReleaseSettings() Then
            DialogResult = DialogResult.None
        End If
    End Sub

    '*************************************************
    ' cmdSystemMDBBrowse Click
    '-------------------------------------------------
    ' Purpose:  This routine displays the File Open
    '           common dialog so the user may look
    '           for the system mda or mdw file.
    ' Inputs:   None
    ' Outputs:  None
    ' Returns:  None
    ' Notes:    None
    '*************************************************
    Private Sub cmdSystemMDBBrowse_Click(ByVal eventSender As System.Object, ByVal eventArgs As System.EventArgs) Handles cmdSystemMDBBrowse.Click
        Dim oOpenFileDialog As New OpenFileDialog()
        oOpenFileDialog.CheckFileExists = True
        oOpenFileDialog.CheckPathExists = True
        oOpenFileDialog.ShowReadOnly = False

        ' Set the filter to *.MDA or *.MDW
        oOpenFileDialog.Filter = My.Resources.strWorkgroupFiles & " (*.mda;*.mdw)|*.mda;*.mdw|" & My.Resources.strAllFiles & " (*.*)|*.*"

        ' Initialize the dialog to the file and path specified in the text box.
        ' If nothing is specified and no previous default directory has been
        ' set, default to C:\
        If txtSystemMDB.Text <> "" Then
            If LCase(VB.Right(txtSystemMDB.Text, 4)) = ".mda" Or LCase(VB.Right(txtSystemMDB.Text, 4)) = ".mdw" Then
                oOpenFileDialog.FileName = txtSystemMDB.Text
                If Path.GetDirectoryName(txtSystemMDB.Text) <> "" Then
                    oOpenFileDialog.InitialDirectory = Path.GetDirectoryName(txtSystemMDB.Text)
                End If
            Else
                oOpenFileDialog.FileName = ""
                oOpenFileDialog.InitialDirectory = txtSystemMDB.Text
            End If
        Else
            oOpenFileDialog.FileName = String.Empty
            If String.IsNullOrEmpty(oOpenFileDialog.InitialDirectory) Then
                oOpenFileDialog.InitialDirectory = Directory.GetDirectoryRoot(Environment.SystemDirectory)
            End If
        End If

        If DialogResult.OK = oOpenFileDialog.ShowDialog() Then
            txtSystemMDB.Text = oOpenFileDialog.FileName
            Dirty = True
        End If
    End Sub

    ''' <summary>
    ''' Determines if Adobe Acrobat settings should be enabled
    ''' </summary>
    ''' <returns>True if the image type selected in the combo box is
    ''' JPEG, Multi Page TIFF, PCX or Single Page TIFF</returns>
    ''' <remarks></remarks>
    Private Function IsPdfImageType() As Boolean
        Dim imageType As ImageTypeComboBoxItem = CType(cboImageType.SelectedItem, ImageTypeComboBoxItem)
        If imageType Is Nothing Then
            Return False
        Else
            Dim nSelectedImageType As Integer = imageType.Type
            Return nSelectedImageType = CAP_IMAGE_FORMAT.CAP_FORMAT_PDF_JPEG Or _
                nSelectedImageType = CAP_IMAGE_FORMAT.CAP_FORMAT_PDF_MULTI Or _
                nSelectedImageType = CAP_IMAGE_FORMAT.CAP_FORMAT_PDF_PCX Or _
                nSelectedImageType = CAP_IMAGE_FORMAT.CAP_FORMAT_PDF_SINGLE
        End If
    End Function

    '*************************************************
    ' Fill Document Link's Lists
    '-------------------------------------------------
    ' Purpose:  This routine will fill the DocID and
    '           DocPath combo boxes with all valid
    '           column names in the specified table.
    ' Inputs:   TableName    Database table name
    ' Outputs:  None
    ' Returns:  None
    ' Notes:    A valid database connection must have
    '           been made before calling this routine
    '*************************************************
    Sub FillDocLists(ByRef TableName As String)
        ' Loop thru the fields in the table for valid
        ' field types for the Document ID or Path.
        ' Document ID may be numeric or text.
        ' Document Path must be text.
        Using command As DbCommand = _
            oDB.CreateCommand(oDB.GetTableSelect(TableName))
            Using reader As DbDataReader = _
                command.ExecuteReader(CommandBehavior.SchemaOnly Or CommandBehavior.KeyInfo)
                Using table As DataTable = reader.GetSchemaTable()
                    ' Clear the current contents of the combo boxes
                    cboDocID.Items.Clear()
                    cboDocID.Enabled = True
                    lblDocID.Enabled = True
                    cboDocPath.Items.Clear()
                    cboDocPath.Enabled = True
                    lblDocPath.Enabled = True

                    If Not table Is Nothing Then
                        For Each column As DataRow In table.Rows
                            Select Case column("DataType").ToString()
                                ' The following types are valid for the Document ID only.
                                Case "System.Int16", "System.Int32", "System.Int64"
                                    ' AutoIncrField (i.e. Auto Number) is not valid, so we skip these.
                                    If Not CBool(column("IsAutoIncrement")) Then
                                        cboDocID.Items.Add(column("ColumnName"))
                                    End If

                                    ' The following types are valid for both Document ID and Document Path.
                                Case "System.String"
                                    cboDocPath.Items.Add(column("ColumnName"))
                                    cboDocID.Items.Add(column("ColumnName"))
                            End Select
                        Next
                    Else
                        MsgBox(String.Format(My.Resources.strNotValidTable, TableName), _
                            MsgBoxStyle.OkOnly Or MsgBoxStyle.Exclamation, _
                            My.Resources.strTitleDatabase)
                        oDB.CurrentDTable = String.Empty
                        Return
                    End If
                End Using
            End Using
        End Using

        ' We don't have a valid table if either combo box is empty.
        If cboDocID.Items.Count = 0 Then
            ' Tell the user and reject the table.
            MsgBox(My.Resources.strNotADocTable & vbCr & _
                My.Resources.strNoValidDocId, _
                MsgBoxStyle.OkOnly Or MsgBoxStyle.Exclamation, _
                My.Resources.strTitleDatabase)
            oDB.CurrentDTable = String.Empty
            cboDTables.SelectedIndex = -1
        ElseIf cboDocPath.Items.Count = 0 Then
            ' Tell the user and reject the table.
            MsgBox(My.Resources.strNotADocTable & vbCr & _
                My.Resources.strNoValidDocPath, _
                MsgBoxStyle.OkOnly Or MsgBoxStyle.Exclamation, _
                My.Resources.strTitleDatabase)
            oDB.CurrentDTable = String.Empty
            cboDTables.SelectedIndex = -1
        Else
            ' Keep the table.
            oDB.CurrentDTable = TableName
        End If
    End Sub

    '*************************************************
    ' FillTableCombos
    '-------------------------------------------------
    ' Purpose:  This routine will fill the Index and
    '           Document Table combos with the tables
    '           defined for the current database.
    ' Inputs:   None
    ' Outputs:  None
    ' Returns:  None
    ' Notes:    None
    '*************************************************
    Sub FillTableCombos()
        ' Clear any values from the last table selection
        cboITables.Items.Clear()
        cboDTables.Items.Clear()
        cboDocID.Items.Clear()
        cboDocID.Enabled = False
        lblDocID.Enabled = False
        cboDocPath.Items.Clear()
        cboDocPath.Enabled = False
        lblDocPath.Enabled = False

        ' Loop through each table definition and add
        ' non-system tables to the combos.
        Using table As DataTable = oDB.cnADO.GetSchema("Tables")
            For Each row As DataRow In table.Rows
                '*** Load only non system tables
                Dim strTableType As String = CStr(row("TABLE_TYPE"))
                Dim strTableName As String = CStr(row("TABLE_NAME"))
                Dim oTableSchema As Object
                If table.Columns.Contains("TABLE_SCHEMA") Then
                    oTableSchema = row("TABLE_SCHEMA")
                Else
                    oTableSchema = DBNull.Value
                End If
                If Not strTableType = "SYSTEM TABLE" AndAlso _
                    Not strTableType = "SYSTEM VIEW" AndAlso _
                    Not strTableType = "ACCESS TABLE" AndAlso _
                    Not strTableName.Contains("=") Then

                    '*** Check to see if the table schema is blank.  If it is then just return the tablename
                    If Not Convert.IsDBNull(oTableSchema) Then
                        Dim strFullTableName As String = String.Format("{0}.{1}", oTableSchema, strTableName)
                        cboITables.Items.Add(strFullTableName)
                        cboDTables.Items.Add(strFullTableName)
                    Else
                        cboITables.Items.Add(strTableName)
                        cboDTables.Items.Add(strTableName)
                    End If
                End If
            Next
        End Using
    End Sub

    '*************************************************
    ' FillUIWithDefaults
    '-------------------------------------------------
    ' Purpose:  If the user is editing the release
    '           settings for the first time on this
    '           Batch Class/Doc Class instance,
    '           fill the user interface with
    '           default values rather than reading
    '           the settings in the data object.
    ' Inputs:   None
    ' Outputs:  None
    ' Returns:  None
    ' Notes:    None
    '*************************************************
    Private Sub FillUIWithDefaults()
        ' -- Database Tab --

        ' When starting a new release queue, there
        ' should be no connection to start.
        cboDBTypes.SelectedIndex = DBTYPE_ACCESS - 1
        txtConnString.Clear()
        txtSystemMDB.Clear()
        txtUserName.Text = DEFAULT_USERNAME
        txtPassword.Clear()

        ' -- Table Settings Tab --
        InitializeIndexValueColumns()

        ' -- Document Storage Tab --
        ' The directory text boxes are cleared
        ' and skip first page is disabled.
        chkReleaseImageFiles.CheckState = System.Windows.Forms.CheckState.Checked
        txtImageDir.Clear()

        ' --- Disable OCR Full text ---
        chkReleaseOCRFullText.CheckState = System.Windows.Forms.CheckState.Unchecked
        lblOCRDir.Enabled = False
        txtOCRDir.Clear()
        txtOCRDir.Enabled = False
        cmdOCRBrowse.Enabled = False

        ' --- Disable Kofax PDF ---
        chkReleaseKofaxPDF.CheckState = System.Windows.Forms.CheckState.Unchecked
        lblPDFDir.Enabled = False
        txtKofaxPDFDir.Clear()
        txtKofaxPDFDir.Enabled = False
        cmdPDFBrowse.Enabled = False

        chkSkipFirstPage.CheckState = System.Windows.Forms.CheckState.Unchecked

        ' --- Image Format Tab ---

        '*** Defaults to multi-page TIFF release
        SetImageType(CAP_IMAGE_FORMAT.CAP_FORMAT_MTIFF_G4)
    End Sub

    '*************************************************
    ' FillUIWithImageType
    '-------------------------------------------------
    ' Purpose:  This routine gets all image types
    '           from the SetupData object and fills
    '           the combo box with the description
    '           and ID
    ' Inputs:   oSetupData      ReleaseSetupData object
    '           bMultiPageOnly  flag to list only
    '                           multipage image formats
    ' Outputs:  None
    ' Returns:  None
    ' Notes:    None
    '*************************************************
    Private Sub FillUIWithImageType(ByRef oSetupData As Kofax.ReleaseLib.ReleaseSetupData, ByRef bMultiPageOnly As Boolean)
        Dim releaseSetupData As New ReleaseSetupData(oSetupData)
        Dim imageType As ImageType

        With cboImageType
            ' Start with an empty combo box
            .Items.Clear()
            ' Get each item and add it
            For Each oImageTypeObject As Object In releaseSetupData.ImageTypes
                imageType = New ImageType(oImageTypeObject)
                If Not bMultiPageOnly Or imageType.MultiplePage Then
                    .Items.Add(New ImageTypeComboBoxItem(imageType))
                End If
            Next

        End With

        ' Set default
        cboImageType.SelectedIndex = 0
    End Sub

    '*************************************************
    ' FillUIWithSettings
    '-------------------------------------------------
    ' Purpose:  This routine will fill the user
    '           interface with the current release
    '           settings for this Batch Class/Doc
    '           Class combination from SetupData.
    ' Inputs:   None
    ' Outputs:  None
    ' Returns:  None
    ' Notes:    None
    '*************************************************
    Private Sub FillUIWithSettings()
        Using customPropertiesReader As New CustomPropertiesReaderWriter(m_setupData.CustomProperties)
            chkReleaseImageFiles.Checked = Not customPropertiesReader.DisableImageExport
            chkReleaseOCRFullText.Checked = Not customPropertiesReader.DisableTextExport
            chkReleaseKofaxPDF.Checked = customPropertiesReader.EnableKofaxPDFExport

            Try
                ' -- Release Destination Name --
                txtName.Text = m_setupData.Name

                ' -- Database Tab --
                cboDBTypes.SelectedIndex = customPropertiesReader.ConnectionType - 1
                txtConnString.Text = m_setupData.ConnectString
                txtSystemMDB.Text = customPropertiesReader.SystemMdb
                txtUserName.Text = m_setupData.UserName
                txtPassword.Text = m_setupData.Password

                ' -- Connect To Database --

                ' Set a flag to indicate we are trying to connect to the database.
                ' This flag will be used if an error occurs to give a more specific message.
                oDB.ConnectToDatabase( _
                    customPropertiesReader.ConnectionType, _
                    m_setupData.ConnectString, _
                    customPropertiesReader.SystemMdb, _
                    m_setupData.UserName, m_setupData.Password)

                ' -- Table Settings Tab --
                InitializeIndexValueColumns()
                FillTableCombos()
                SetComboValue(cboITables, m_setupData.TableName)
                SetComboValue(cboDTables, customPropertiesReader.DocTable)
                InitLinks(m_setupData.Links)
                SetComboValue(cboDocID, customPropertiesReader.DocId)
                SetComboValue(cboDocPath, customPropertiesReader.DocPath)

                ' -- Document Storage Tab --
                txtImageDir.Text = m_setupData.ImageFilePath
                txtOCRDir.Text = m_setupData.TextFilePath
                txtKofaxPDFDir.Text = m_setupData.KofaxPDFPath
                chkSkipFirstPage.Checked = m_setupData.SkipFirstPage <> 0

                ' -- Image Format Tab --

                ' Selected the saved image selection otherwise use default multi-page TIFF image format
                If CBool(chkReleaseImageFiles.CheckState) Then
                    If UnsupportedPDFDected(m_setupData.ImageType) Then
                        SetImageType(CAP_IMAGE_FORMAT.CAP_FORMAT_MTIFF_G4) ' Use default image format
                    Else
                        SetImageType(m_setupData.ImageType) ' Select saved PDF image format
                    End If
                Else
                    SetImageType(CAP_IMAGE_FORMAT.CAP_FORMAT_MTIFF_G4) ' Use default image format
                End If
            Catch ex As Exception
                Dim MsgText As String

                ' Set the error message which will be displayed.
                ' The user will be prompted to use the default
                ' settings.  If the error occurred while connecting
                ' to the database, this will also be indicated.
                MsgText = My.Resources.strUseDefault
                If Not oDB.Connected Then
                    MsgText = My.Resources.strNotConnectedToDb & vbCrLf & MsgText
                End If

                If DialogResult.Yes = MessageBox.Show(MsgText, My.Resources.strTitleReleaseSetupError, MessageBoxButtons.YesNo, MessageBoxIcon.Exclamation) Then
                    ' The user wants to revert to the defaults.
                    FillUIWithDefaults()
                Else
                    Throw New DatabaseConnectionException(Nothing, ex)
                End If
            End Try
        End Using
    End Sub

    Private Sub InitializeIndexValueColumns()
        '*** Set up index value table columns if not already done
        If indexValuesDataGridView.Columns.Count = 0 Then
            indexValuesDataGridView.Columns.Add("Destination", My.Resources.strDatabaseColumns)
            indexValuesDataGridView.Columns(0).DataPropertyName = "Destination"
            indexValuesDataGridView.Columns(0).ReadOnly = True

            indexValuesDataGridView.Columns.Add(New DataGridViewIndexValueColumn())
            indexValuesDataGridView.Columns(1).DataPropertyName = "Source"

            indexValuesDataGridView.Columns.Add("SourceType", String.Empty)
            indexValuesDataGridView.Columns(2).DataPropertyName = "SourceType"
            indexValuesDataGridView.Columns(2).Visible = False
        End If
    End Sub

    '*************************************************
    ' FindColumnName
    '-------------------------------------------------
    ' Purpose:  This routine scans the column names
    '           in the link list for the passed in
    '           value.
    ' Inputs:   ColumnName      search string
    ' Outputs:  None
    ' Returns:  Index into the link array where the
    '           column name was found or -1 if the
    '           column name was not found.
    ' Notes:    None
    '*************************************************
    Function FindColumnName(ByRef ColumnName As String) As Link
        ' Look for the requested column name
        For Each link As Link In links
            If link.Destination = ColumnName Then
                ' We found it so exit
                Return link
            End If
        Next

        Return Nothing
    End Function

    '*************************************************
    ' FindSourceLink
    '-------------------------------------------------
    ' Purpose:  This routine scans the source and the
    '           source types in the link list for the
    '           passed in values.
    ' Inputs:   sName       Source name search string
    '           IndexType   Source type value
    ' Outputs:  None
    ' Returns:  Index into the link array where the
    '           index source was found or -1 if the
    '           index source was not found.
    ' Notes:    None
    '*************************************************
    Private Function FindSourceLink(ByRef sName As String, ByRef nIndexType As Kofax.ReleaseLib.KfxLinkSourceType) As Link
        ' Search through the list
        For Each link As Link In links
            If sName = link.Source And nIndexType = link.SourceType Then
                ' We found it so exit
                Return link
            End If
        Next

        ' We did not find it
        Return Nothing
    End Function
    '*************************************************
    ' cmdPDFBrowse_Click
    '-------------------------------------------------
    ' Purpose:  Initialize and display the dialog
    '           allowing the user to browse for the
    '           directory where Kofax PDF files
    '           will be stored during Release. Mark
    '           the data dirty if the user selects
    '           a directory.
    ' Inputs:   None
    ' Outputs:  None
    ' Returns:  None
    ' Notes:    We store the Help Context ID in the
    '           dialog's Tag property since it is
    '           used for multiple purposes.
    '*************************************************
    Private Sub cmdPDFBrowse_Click(ByVal eventSender As System.Object, ByVal eventArgs As System.EventArgs) Handles cmdPDFBrowse.Click
        Dim dialog As New FolderBrowserDialog()
        dialog.Description = My.Resources.strTitleSelectPdfDir
        dialog.SelectedPath = txtKofaxPDFDir.Text
        If DialogResult.OK = dialog.ShowDialog() Then
            txtKofaxPDFDir.Text = dialog.SelectedPath
            Me.Dirty = True
        End If
    End Sub

    '*************************************************
    ' Form_QueryUnload
    '-------------------------------------------------
    ' Purpose:  This event is called first whenever
    '           the form is about to unload. When the
    '           user clicks OK or Cancel we start to
    '           unload the form.  In this event, we
    '           simply validate that all changes are
    '           saved and hide the form.  The form is
    '           actually unloaded by the ReleaseSetup
    '           class.  That time, the form is not
    '           visible and we allow it to unload.
    ' Inputs:   None
    ' Outputs:  Cancel      flag to abort Unload event
    '           UnloadMode  cause of the Unload event
    ' Returns:  None
    ' Notes:    None
    '*************************************************
    Protected Overrides Sub OnFormClosing(ByVal e As System.Windows.Forms.FormClosingEventArgs)
        MyBase.OnFormClosing(e)

        ' If the form is visible then we only validate
        ' that the data is saved and then hide the form.
        ' We do not allow it to unload yet.

        ' Check the form status and if changes have been made,
        ' allow the user to save.  Otherwise just exit.
        If Me.Dirty = True Then
            Dim eResult As DialogResult
            eResult = MessageBox.Show(My.Resources.strSaveSettings, My.Resources.strTitleSaveSettings, MessageBoxButtons.YesNoCancel, MessageBoxIcon.Question)
            If DialogResult.Yes = eResult Then
                ' Try and save
                If Not SaveReleaseSettings() Then
                    e.Cancel = True
                End If
            ElseIf DialogResult.Cancel = eResult Then
                ' Go back to the form
                e.Cancel = True
            End If
        End If
    End Sub

    '*************************************************
    ' InitLinks
    '-------------------------------------------------
    ' Purpose:  This routine will initialize the
    '           link list with the entries from
    '           the Links collection.
    ' Inputs:   LinkList    Links collection
    '*************************************************
    Sub InitLinks(ByRef linkList As Kofax.ReleaseLib.Links)
        ' Loop through all of the values in the links table
        Using enumerator As New ComEnumerator(linkList.GetEnumerator())
            While enumerator.MoveNext()
                Dim link As Kofax.ReleaseLib.Link = CType(enumerator.Current, Kofax.ReleaseLib.Link)
                ' PDF links have destination like "PDF_****". Ignore PDF links here.
                If VB.Left(link.Destination, 3) <> "PDF" Then
                    Dim columnLink As Link = FindColumnName(link.Destination)
                    If Not columnLink Is Nothing Then
                        columnLink.SourceType = CType(link.SourceType, SourceType)
                        columnLink.Source = link.Source
                    Else
                        ' If we can't find the destination value in the
                        ' list of table columns, ask the user whether to
                        ' remove it from the list of links or quit
                        If MsgBox( _
                            String.Format("{0}: ""{1}"".{2}{3}", _
                                My.Resources.strIndexColumnMissing, _
                                link.Destination, vbCrLf, _
                                My.Resources.strRemoveLinkOrQuit), _
                            MsgBoxStyle.Exclamation Or MsgBoxStyle.OkCancel, _
                            My.Resources.strTitleReleaseSetupError) = MsgBoxResult.Ok Then

                            linkList.Remove(link.Destination)
                        End If
                    End If
                End If
            End While
        End Using
    End Sub

    '*************************************************
    ' LinksExist
    '-------------------------------------------------
    ' Purpose:  This routine will search all of the
    '           link entries to see if any links
    '           exist.
    ' Returns:  True/False
    '*************************************************
    Function LinksExist() As Boolean
        ' Loop through each entry and look for
        ' any source type value other than NO_LINK
        For Each link As Link In links
            If link.SourceType <> SourceType.KFX_REL_UNDEFINED_LINK Then
                Return True
            End If
        Next

        Return False
    End Function

    '*****************************************************
    '*** Routine: LoadFormSettings
    '*** Purpose: Load all the settings while the wait
    '***          dialog is shown
    '*** Inputs:
    '*** Returns:
    '*****************************************************
    Public Sub LoadFormSettings()
        ' Populate the combo with all supported image types
        FillUIWithImageType(m_setupData, False)

        ' Display the Batch Class and Document Class names
        lblBatchClassName.Text = m_setupData.BatchClassName
        lblDocClassName.Text = m_setupData.DocClassName

        ' If there is no currently existing data, load
        ' the UI with defaults, otherwise load the
        ' current settings.
        If m_setupData.New <> 0 Then
            FillUIWithDefaults()
        Else
            FillUIWithSettings()
        End If
    End Sub

    '*************************************************
    ' SaveReleaseSettings
    '-------------------------------------------------
    ' Purpose:  This routine will save the setup data
    '           to the Ascent database through the
    '           SetupData properties and collections.
    ' Inputs:   None
    ' Outputs:  None
    ' Returns:  True/False
    ' Notes:    None
    '*************************************************
    Function SaveReleaseSettings() As Boolean
        If Not ValidateChildren(ValidationConstraints.None) Then
            Return False
        End If

        Dim imageType As ImageTypeComboBoxItem = CType(cboImageType.SelectedItem, ImageTypeComboBoxItem)

        Try
            ' Change to a Wait cursor because this may take
            ' a while.  Remember to change it back at all
            ' possible exit points.
            Me.Cursor = System.Windows.Forms.Cursors.WaitCursor

            ' Clear all entries from the custom properties collection
            m_setupData.CustomProperties.RemoveAll()

            ' -- Release Destination Name --
            m_setupData.Name = txtName.Text

            Using customPropertiesWriter As New CustomPropertiesReaderWriter(m_setupData.CustomProperties)
                ' -- Database Tab --
                customPropertiesWriter.ConnectionType = cboDBTypes.SelectedIndex + 1
                customPropertiesWriter.SystemMdb = txtSystemMDB.Text
                m_setupData.ConnectString = txtConnString.Text
                m_setupData.UserName = txtUserName.Text
                m_setupData.Password = txtPassword.Text

                ' -- Table Settings Tab --
                m_setupData.TableName = VB6.GetItemString(cboITables, cboITables.SelectedIndex)

                ' Save links at the end
                customPropertiesWriter.DocTable = VB6.GetItemString(cboDTables, cboDTables.SelectedIndex)
                customPropertiesWriter.DocId = VB6.GetItemString(cboDocID, cboDocID.SelectedIndex)
                customPropertiesWriter.DocPath = VB6.GetItemString(cboDocPath, cboDocPath.SelectedIndex)

                ' -- Links Collection --

                ' Note: This routine deletes all the existing index links,
                ' so the PDF links have to be saved after calling this one.
                If Not SaveTheLinks() Then
                    ' Restore previous settings to the ReleaseSetupData object
                    m_setupData.Refresh(1)
                    Return False
                End If

                ' -- Document Storage Tab --
                With m_setupData
                    .ImageFilePath = txtImageDir.Text
                    .TextFilePath = txtOCRDir.Text
                    .KofaxPDFPath = txtKofaxPDFDir.Text
                    .SkipFirstPage = CInt(IIf(chkSkipFirstPage.Checked, 1, 0))
                End With

                customPropertiesWriter.DisableImageExport = Not chkReleaseImageFiles.Checked
                customPropertiesWriter.DisableTextExport = Not chkReleaseOCRFullText.Checked
                m_setupData.KofaxPDFReleaseScriptEnabled = chkReleaseKofaxPDF.Checked
            End Using

            ' -- Image Format Tab --
            m_setupData.ImageType = imageType.Type

            '*** Save and clean up
            m_setupData.Apply()
            Dirty = False

            Return True
        Catch ex As Exception
            ' Restore previous settings to the ReleaseSetupData object
            m_setupData.Refresh(1)
            Throw
        Finally
            ' Change the cursor back
            Me.Cursor = System.Windows.Forms.Cursors.Default
        End Try
    End Function

    '*************************************************
    ' SaveTheLinks
    '-------------------------------------------------
    ' Purpose:  This routine will save the links to
    '           the links collection.
    ' Inputs:   None
    ' Outputs:  None
    ' Returns:  True/False
    ' Notes:    None
    '*************************************************
    Function SaveTheLinks() As Boolean
        Dim oLinks As Kofax.ReleaseLib.Links

        Try
            oLinks = m_setupData.Links

            ' Clear all the current links and reload them
            oLinks.RemoveAll()

            ' Add each link one at a time to the collection
            For Each link As Link In links
                If link.SourceType <> SourceType.KFX_REL_UNDEFINED_LINK Then
                    oLinks.Add( _
                        link.Source, _
                        CType(link.SourceType, KfxLinkSourceType), _
                        link.Destination)
                End If
            Next

            Return True
        Catch ex As Exception
            ' Set the focus to the bad link
            tabDatabase.SelectedTab = tableSettingsTabPage
            MsgBox(ex.Message, MsgBoxStyle.OkOnly Or MsgBoxStyle.Exclamation, My.Resources.strDataVerifyFail)
            Return False
        End Try
    End Function

    '*********************************************
    ' SetDBTypeUI
    '---------------------------------------------
    ' Purpose:  This routine changes the Database
    '           tab UI to fit the selected dbtype
    ' Inputs:   DBType      Database type value
    ' Outputs:  None
    ' Returns:  None
    ' Notes:    None
    '*********************************************
    Sub SetDBTypeUI(ByRef DBType As Integer)
        ' Clear all databoxes
        txtConnString.Clear()
        txtSystemMDB.Clear()
        txtUserName.Clear()
        txtPassword.Clear()

        ' Depending upon the database type, change
        ' the connection string caption and any other
        ' UI settings required.
        Select Case DBType
            Case DBTYPE_ACCESS
                lblConnString.Text = My.Resources.strAccessDb
                txtSystemMDB.Enabled = True
                lblSystemMDB.Enabled = True
                cmdSystemMDBBrowse.Enabled = True
                txtUserName.Text = DEFAULT_USERNAME

            Case DBTYPE_ODBC
                lblConnString.Text = My.Resources.strDsn
                txtSystemMDB.Enabled = False
                lblSystemMDB.Enabled = False
                cmdSystemMDBBrowse.Enabled = False
        End Select
    End Sub

    '*************************************************
    ' SetImageType
    '-------------------------------------------------
    ' Purpose:  This routine sets the image type
    '           combo box index to the image type
    '           passed in.  If the image type isn't
    '           found, a msgbox is shown and the
    '           first image type is selected.
    ' Inputs:   nType   selected image type
    ' Outputs:  None
    ' Returns:  None
    ' Notes:    None
    '*************************************************
    Private Sub SetImageType(ByRef nType As Integer)
        Dim imageType As ImageTypeComboBoxItem

        If UnsupportedPDFDected(nType) Then
            nType = CAP_IMAGE_FORMAT.CAP_FORMAT_MTIFF_G4
        End If

        For I As Integer = 0 To cboImageType.Items.Count - 1
            imageType = CType(cboImageType.Items(I), ImageTypeComboBoxItem)
            If imageType.Type = nType Then
                cboImageType.SelectedIndex = I
                Exit Sub
            End If
        Next

        MsgBox(String.Format("{0} {1}", My.Resources.strMissingImageType, nType.ToString()), MsgBoxStyle.OkOnly)
        cboImageType.SelectedIndex = 0
    End Sub

    '*************************************************
    ' ShowForm
    '-------------------------------------------------
    ' Purpose:  This is the entry point. It loads
    '           the user interface from the resource
    '           file, creates the Index Values popup
    '           menu, & loads any previous settings.
    ' Inputs:   oSetupData  the setup data object
    ' Outputs:  None
    ' Returns:  True/False
    ' Notes:    None
    '*************************************************
    Public Sub ShowForm(ByRef oSetupData As Kofax.ReleaseLib.ReleaseSetupData, ByRef bDirty As Boolean)
        m_setupData = oSetupData
        indexValuesDataGridView.SetupData = m_setupData

        Try
            LoadFormSettings()
            Dirty = bDirty
            ShowDialog()
        Catch ex As DatabaseConnectionException
            ' If we can't connect to the database and the user
            ' decided not to use default settings we just bail out
        End Try
    End Sub

    '*************************************************
    ' pdfImageFormat3_Change
    '-------------------------------------------------
    ' Purpose:  Some setting on the PDF3 control (tab)
    '           has changed.  Mark the form dirty.
    ' Inputs:   None
    ' Outputs:  None
    ' Returns:  None
    ' Notes:    None
    '*************************************************
    Private Sub pdfImageFormat3_Change()
        Me.Dirty = True
    End Sub

    '*************************************************
    ' pdfImageFormat3_Error
    '-------------------------------------------------
    ' Purpose:  PDF3 Control is logging an error.
    ' Inputs:   ErrNum      Error number
    '           ErrMsg      Error message text
    '           SourceFile  source code module where
    '                       error occurred
    '           LineNo      line number where error
    '                       occurred
    '           ReRaise     flag to throw an error
    '           Display     flag to display MsgBox
    ' Outputs:  None
    ' Returns:  None
    ' Notes:    None
    '*************************************************
    Private Sub pdfImageFormat3_Error(ByRef ErrNum As Integer, ByRef ErrMsg As String, ByRef SourceFile As String, ByRef LineNo As Short, ByRef ReRaise As Boolean, ByRef Display As Boolean)
        Dim strMessage As String = String.Format("{0}{1}(#{2:d})", ErrMsg, vbCrLf, ErrNum)
        If Display Then
            MessageBox.Show(ErrMsg)
        End If
        If ReRaise Then
            Throw New Exception(ErrMsg)
        End If
    End Sub

    '*************************************************
    ' txtConnString_Change
    '-------------------------------------------------
    ' Purpose:  If the connection is already made to
    '           the database, we warn the user that
    '           changing the connect string will lose
    '           all database settings.  Give them a
    '           chance to undo the change.
    ' Inputs:   None
    ' Outputs:  None
    ' Returns:  None
    ' Notes:    This event can be re-entrant.  If the
    '           user chooses to cancel the change, we
    '           programatically reset the text box
    '           value which retriggers this event.
    '           We therefore use a local static flag
    '           to ignore this subsequent call.
    '*************************************************
    Private Sub txtConnString_TextChanged(ByVal eventSender As System.Object, ByVal eventArgs As System.EventArgs) Handles txtConnString.TextChanged
        ' If we are currently connected to a database and the
        ' user changes the connection string, allow the user
        ' to back out of the change before breaking all the links.
        SmartTextHandle(txtConnString, oDB.ConnString)
    End Sub

    '*************************************************
    ' SmartTextHandle
    '-------------------------------------------------
    ' Purpose:  Intelligently handles text events that may change
    '           the text of the control that fired the original event.
    '*************************************************
    Private Sub SmartTextHandle(ByRef oTextBox As TextBox, ByVal strOriginalValue As String)
        '*** The m_bSuppressTextChangeEvent flag is used to determine if we want to show the
        '*** prompt about breaking links.
        If (Not m_bSuppressTextChangeEvent) AndAlso BreakAllLinks() Then
            Dirty = True
        Else
            '*** If we are going to change the text back, supress the event handling so the prompt
            '*** is not displayed (because the text is being reverted - this change we want).
            m_bSuppressTextChangeEvent = True
            oTextBox.Text = strOriginalValue
            m_bSuppressTextChangeEvent = False
        End If
    End Sub
    '*************************************************
    ' txtImageDir_Change
    '-------------------------------------------------
    ' Purpose:  Mark the form dirty when the Image
    '           release directory changes.
    ' Inputs:   None
    ' Outputs:  None
    ' Returns:  None
    ' Notes:    None
    '*************************************************
    Private Sub txtImageDir_TextChanged(ByVal eventSender As System.Object, ByVal eventArgs As System.EventArgs) Handles txtImageDir.TextChanged
        Dirty = True
    End Sub

    '*************************************************
    ' txtName_Change
    '-------------------------------------------------
    ' Purpose:  Mark the form dirty when the release
    '           destination name changes.
    '*************************************************
    Private Sub txtName_TextChanged(ByVal eventSender As System.Object, ByVal eventArgs As System.EventArgs) _
        Handles txtName.TextChanged, txtOCRDir.TextChanged, txtKofaxPDFDir.TextChanged

        Dirty = True
    End Sub

    '*************************************************
    ' txtPassword_Change
    '-------------------------------------------------
    ' Purpose:  If the connection is already made to
    '           the database, we warn the user that
    '           changing the password will lose all
    '           database settings.  Give them a
    '           chance to undo the change.
    ' Notes:    This event can be re-entrant.  If the
    '           user chooses to cancel the change, we
    '           programatically reset the text box
    '           value which retriggers this event.
    '           We therefore use a local static flag
    '           to ignore this subsequent call.
    '*************************************************
    Private Sub txtPassword_TextChanged(ByVal eventSender As System.Object, ByVal eventArgs As System.EventArgs) Handles txtPassword.TextChanged
        ' Only ask if we are currently connected to a database
        SmartTextHandle(txtPassword, oDB.ConnPassword)
    End Sub

    '*************************************************
    ' txtSystemMDB_Change
    '-------------------------------------------------
    ' Purpose:  We place the cursor at the end of the
    '           current system MDB file in case it is
    '           longer than the textbox can display.
    '*************************************************
    Private Sub txtSystemMDB_TextChanged(ByVal eventSender As System.Object, ByVal eventArgs As System.EventArgs) Handles txtSystemMDB.TextChanged
        ' If we are currently connected to a database and the
        ' user changes the workgroup file, allow the user
        ' to back out of the change before breaking all the links.
        SmartTextHandle(txtSystemMDB, oDB.ConnSysMDB)
    End Sub

    '*************************************************
    ' txtUserName_Change
    '-------------------------------------------------
    ' Purpose:  If the connection is already made to
    '           the database, we warn the user that
    '           changing the user name will lose all
    '           database settings.  Give them a
    '           chance to undo the change.
    ' Inputs:   None
    ' Outputs:  None
    ' Returns:  None
    ' Notes:    This event can be re-entrant.  If the
    '           user chooses to cancel the change, we
    '           programatically reset the text box
    '           value which retriggers this event.
    '           We therefore use a local static flag
    '           to ignore this subsequent call.
    '*************************************************
    Private Sub txtUserName_TextChanged(ByVal eventSender As System.Object, ByVal eventArgs As System.EventArgs) Handles txtUserName.TextChanged
        SmartTextHandle(txtUserName, oDB.ConnUser)
    End Sub

    Private Sub indexValuesDataGridView_CellEnter(ByVal sender As System.Object, ByVal e As System.Windows.Forms.DataGridViewCellEventArgs) Handles indexValuesDataGridView.CellEnter
        'BeginInvoke is used to move to current cell to the value column
        'rather than directly setting the CurrentCell property because if you did
        'the CellEnter event would be raised recursively
        If 1 <> e.ColumnIndex Then
            BeginInvoke(CType(AddressOf MoveToIndexValueColumn, MethodInvoker))
        End If
    End Sub

    Private Sub MoveToIndexValueColumn()
        If indexValuesDataGridView.CurrentCellAddress.Y >= 0 Then
        indexValuesDataGridView.CurrentCell = indexValuesDataGridView.Item(1, indexValuesDataGridView.CurrentCellAddress.Y)
        End If
    End Sub

    Private Sub indexValuesDataGridView_CurrentCellDirtyStateChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles indexValuesDataGridView.CurrentCellDirtyStateChanged
        Dirty = True
    End Sub

    Protected Overrides Sub OnHelpRequested(ByVal hevent As System.Windows.Forms.HelpEventArgs)
        MyBase.OnHelpRequested(hevent)

        If Not hevent.Handled Then
			Help.Show(tabDatabase.SelectedIndex + TABS_FIRST_HELPID)
			hevent.Handled = True
        End If
    End Sub

    ''' <summary>
    ''' Validates all controls and shows a message box for only the first
    ''' control that failed validation and sets the focus to this control
    ''' </summary>
    ''' <param name="validationConstraints">See Form.ValidationChildren in MSDN</param>
    ''' <returns>See Form.ValidationChildren in MSDN</returns>
    ''' <remarks>See Form.ValidationChildren in MSDN</remarks>
    Public Overrides Function ValidateChildren(ByVal validationConstraints As System.Windows.Forms.ValidationConstraints) As Boolean
        errorProvider.Clear()
        Dim result As Boolean = MyBase.ValidateChildren(validationConstraints)
        ShowValidationResults(Me.Controls)
        Return result
    End Function

    ''' <summary>
    ''' Looks through all controls and if any failed validation shows a message box
    ''' and sets focus to the control
    ''' </summary>
    ''' <param name="controls">The controls to look for errors in</param>
    ''' <returns>True if controls passed validation otherwise false</returns>
    ''' <remarks></remarks>
    Private Function ShowValidationResults(ByVal controls As Control.ControlCollection) As Boolean
        Dim invalidControl As Control = Nothing
        FindFirstInvalidControl(controls, invalidControl)
        If invalidControl IsNot Nothing Then
            Dim message As String = errorProvider.GetError(invalidControl)
            SelectTabPage(invalidControl)
            MessageBox.Show(message, My.Resources.strDataVerifyFail, MessageBoxButtons.OK, MessageBoxIcon.Exclamation)
            invalidControl.Focus()
            Return False
        End If
        Return True
    End Function

    ''' <summary>
    ''' Get the control that failed validation lowest in the tab order
    ''' </summary>
    ''' <param name="controls">The controls to check for failed validation</param>
    ''' <param name="invalidControl">The lowest control in tab order that failed validation</param>
    ''' <remarks>Uses recursion to search all controls</remarks>
    Private Sub FindFirstInvalidControl(ByVal controls As Control.ControlCollection, ByRef invalidControl As Control)
        For Each control As Control In controls
            If Not String.IsNullOrEmpty(errorProvider.GetError(control)) And _
                (invalidControl Is Nothing OrElse control.TabIndex < invalidControl.TabIndex) Then

                invalidControl = control
            ElseIf control.Controls IsNot Nothing Then
                FindFirstInvalidControl(control.Controls, invalidControl)
            End If
        Next
    End Sub

    ''' <summary>
    ''' Selects the tab containing the control passed in
    ''' </summary>
    ''' <param name="control">The control to select the tab page for</param>
    ''' <remarks></remarks>
    Private Sub SelectTabPage(ByVal control As Control)
        If control.Parent IsNot Nothing Then
            If TypeOf control.Parent Is TabPage Then
                tabDatabase.SelectedTab = CType(control.Parent, TabPage)
            Else
                SelectTabPage(control.Parent)
            End If
        End If
    End Sub

    Private Sub txtName_Validating(ByVal sender As System.Object, ByVal e As System.ComponentModel.CancelEventArgs) Handles txtName.Validating
        ' Make sure that the destination name is
        ' not longer then 32 characters.
        If txtName.Text.Length > 32 Then
            errorProvider.SetError(txtName, My.Resources.strBadDestination)
            e.Cancel = True
        End If
    End Sub

    Private Sub txtConnString_Validating(ByVal sender As System.Object, ByVal e As System.ComponentModel.CancelEventArgs) Handles txtConnString.Validating
        ' Check if a database has been specified
        If oDB.CurrentDBType = DBTYPE_ACCESS AndAlso Not File.Exists(txtConnString.Text) Then
            errorProvider.SetError(txtConnString, My.Resources.strBadAccessName)
            e.Cancel = True
            Return
        End If

        If oDB.CurrentDBType = DBTYPE_ODBC AndAlso String.IsNullOrEmpty(txtConnString.Text) Then
            errorProvider.SetError(txtConnString, My.Resources.strBadConnectionString)
            e.Cancel = True
            Return
        End If

        If oDB.Connected = False Then
            ' If we are not currently connected to the database,
            ' try and connect with the current settings.  If the
            ' connection fails, send the user back to the
            ' database tab.
            ClearTableTab() ' Clear any current settings
            Try
                oDB.ConnectToDatabase(oDB.CurrentDBType, txtConnString.Text, txtSystemMDB.Text, txtUserName.Text, txtPassword.Text)
                FillTableCombos() ' Once connected, read in the
            Catch ex As OdbcException
                errorProvider.SetError(txtConnString, ex.Errors(0).Message)
                e.Cancel = True
            Catch ex As DbException
                errorProvider.SetError(txtConnString, ex.Message)
                e.Cancel = True
            End Try
        End If
    End Sub

    Private Sub txtSystemMDB_Validating(ByVal sender As System.Object, ByVal e As System.ComponentModel.CancelEventArgs) Handles txtSystemMDB.Validating
        ' Check to see if the workgroup file name is valid
        If Not String.IsNullOrEmpty(txtSystemMDB.Text) And txtSystemMDB.Visible Then
            If Not File.Exists(txtSystemMDB.Text) Then
                errorProvider.SetError(txtSystemMDB, My.Resources.strBadWorkgroupName)
                e.Cancel = True
            End If
        End If
    End Sub

    Private Sub txtSystemMDB_Leave(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles txtSystemMDB.Leave
        txtSystemMDB.Text = txtSystemMDB.Text.Trim()
    End Sub

    Private Sub txtImageDir_Leave(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles txtImageDir.Leave
        txtImageDir.Text = txtImageDir.Text.Trim()
    End Sub

    Private Sub txtOCRDir_Leave(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles txtOCRDir.Leave
        txtOCRDir.Text = txtOCRDir.Text.Trim()
    End Sub

    Private Sub txtKofaxPDFDir_Leave(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles txtKofaxPDFDir.Leave
        txtKofaxPDFDir.Text = txtKofaxPDFDir.Text.Trim()
    End Sub

    Private Sub txtImageDir_Validating(ByVal sender As System.Object, ByVal e As System.ComponentModel.CancelEventArgs) Handles txtImageDir.Validating
        ' Verify image settings if chkReleaseImageFiles is set
        If chkReleaseImageFiles.Checked Then
            If String.IsNullOrEmpty(txtImageDir.Text) Then
                errorProvider.SetError(txtImageDir, My.Resources.strNoImageDirectory)
                e.Cancel = True
                Return
            End If

            If Not VerifyDirectoryName(txtImageDir.Text, Me.Text) Then
                errorProvider.SetError(txtImageDir, txtImageDir.Text & vbCrLf & My.Resources.strBadImageDirectory)
                e.Cancel = True
                Return
            End If
        End If
    End Sub

    Private Sub cboITables_Validating(ByVal sender As System.Object, ByVal e As System.ComponentModel.CancelEventArgs) Handles cboITables.Validating
        ' Make sure that a selection has been made
        ' for the Index Table.
        If cboITables.SelectedIndex = -1 Then
            errorProvider.SetError(cboITables, My.Resources.strNoIndexTable)
            e.Cancel = True
            Return
        End If

        ' Check to make sure that the Index Table selection
        ' is not the same as the Document Table selection.
        If cboITables.SelectedIndex = cboDTables.SelectedIndex Then
            errorProvider.SetError(cboITables, My.Resources.strIndexSameAsDoc)
            e.Cancel = True
            Return
        End If

        If _
                FindSourceLink( _
                    My.Resources.strDocumentId, _
                    Kofax.ReleaseLib.KfxLinkSourceType.KFX_REL_DOCUMENTID) Is Nothing And _
                FindSourceLink( _
                    My.Resources.strDocumentId, _
                    Kofax.ReleaseLib.KfxLinkSourceType.KFX_REL_VARIABLE) Is Nothing Then
            errorProvider.SetError(cboITables, My.Resources.strMissingDocIdLink)
            e.Cancel = True
            Return
        End If

        Dim sFieldMissing As String = String.Empty
        Dim nFieldMissingCnt As Integer
        Try
            Dim tableName As String = VB6.GetItemString(cboITables, cboITables.SelectedIndex)
            Using command As DbCommand = oDB.CreateCommand(oDB.GetTableSelect(tableName))
                Using reader As DbDataReader = command.ExecuteReader(CommandBehavior.SchemaOnly Or CommandBehavior.KeyInfo)
                    Using schemaTable As DataTable = reader.GetSchemaTable()
                        ' Verify that all required fields in the database table are linked.
                        For Each schemaRow As DataRow In schemaTable.Rows
                            Dim link As Link = FindColumnName(CStr(schemaRow("ColumnName")))
                            If Not link Is Nothing Then
                                ' Test each required field for a link
                                If Not CBool(schemaRow("AllowDBNull")) Then
                                    If link.SourceType = SourceType.KFX_REL_UNDEFINED_LINK Then
                                        sFieldMissing = sFieldMissing & AddString(nFieldMissingCnt, CStr(schemaRow("ColumnName")))
                                    End If
                                End If
                            End If
                        Next
                    End Using
                End Using
            End Using
        Catch oEx As Exception
            errorProvider.SetError(cboITables, oEx.Message)
            e.Cancel = True
            Return
        End Try

        ' Required fields are not allowed to be missing.
        If Not String.IsNullOrEmpty(sFieldMissing) Then
            errorProvider.SetError(cboITables, My.Resources.strLinkToRequiredFieldMissing & sFieldMissing)
            e.Cancel = True
            Return
        End If

        ' Now check the Index Fields for any not assigned
        Dim sMissingLink As String = String.Empty
        Dim nMissingLinkCnt As Integer
        Using enumerator As New ComEnumerator(m_setupData.IndexFields.GetEnumerator())
            While enumerator.MoveNext()
                Dim oIndexField As Kofax.ReleaseLib.IndexField = CType(enumerator.Current, Kofax.ReleaseLib.IndexField)
                ' Check only document index field, and skip folder index field.
                If Len(oIndexField.FolderClassName) = 0 Then
                    If _
                        FindSourceLink( _
                            oIndexField.Name, _
                            Kofax.ReleaseLib.KfxLinkSourceType.KFX_REL_INDEXFIELD) Is Nothing Then
                        sMissingLink = sMissingLink & AddString(nMissingLinkCnt, oIndexField.Name)
                    End If
                End If
            End While
        End Using

        ' Simply report the unused Index Fields to the user.
        If Not String.IsNullOrEmpty(sMissingLink) Then
            If DialogResult.Cancel = _
                MessageBox.Show(My.Resources.strNotAllIndexFieldsUsed & sMissingLink, _
                    My.Resources.strDataVerify, _
                    MessageBoxButtons.OKCancel, _
                    MessageBoxIcon.Information) Then

                SelectTabPage(cboITables)
                e.Cancel = True
                Return
            End If
        End If

        ' Now check the Batch Fields for any not assigned
        sMissingLink = String.Empty
        nMissingLinkCnt = 0
        Using enumerator As New ComEnumerator(m_setupData.BatchFields.GetEnumerator())
            While enumerator.MoveNext()
                Dim oBatchField As Kofax.ReleaseLib.BatchField = CType(enumerator.Current, Kofax.ReleaseLib.BatchField)
                If FindSourceLink( _
                    oBatchField.Name, _
                    Kofax.ReleaseLib.KfxLinkSourceType.KFX_REL_BATCHFIELD) Is Nothing Then
                    sMissingLink = sMissingLink & AddString(nMissingLinkCnt, oBatchField.Name)
                End If
            End While
        End Using

        ' Simply report the unused Batch Fields to the user.
        If Not String.IsNullOrEmpty(sMissingLink) Then
            If DialogResult.Cancel = _
                MessageBox.Show(My.Resources.strNotAllBatchFieldsUsed & sMissingLink, _
                    My.Resources.strDataVerify, _
                    MessageBoxButtons.OKCancel, _
                    MessageBoxIcon.Information) Then

                SelectTabPage(cboITables)
                e.Cancel = True
                Return
            End If
        End If
    End Sub

    Private Sub cboDTables_Validating(ByVal sender As System.Object, ByVal e As System.ComponentModel.CancelEventArgs) Handles cboDTables.Validating
        ' Make sure that a selection has been made
        ' for the Document Table
        If cboDTables.SelectedIndex = -1 Then
            errorProvider.SetError(cboDTables, My.Resources.strNoDocTable)
            e.Cancel = True
        End If
    End Sub

    Private Sub cboDocID_Validating(ByVal sender As System.Object, ByVal e As System.ComponentModel.CancelEventArgs) Handles cboDocID.Validating
        ' Make sure that a selection has been made for the
        ' Document ID.  Don't need to check the type as
        ' it's done when the combo is filled.
        If cboDocID.Enabled And cboDocID.SelectedIndex = -1 Then
            errorProvider.SetError(cboDocID, My.Resources.strNoDocIdLink)
            e.Cancel = True
        End If
    End Sub

    Private Sub cboDocPath_Validating(ByVal sender As System.Object, ByVal e As System.ComponentModel.CancelEventArgs) Handles cboDocPath.Validating
        ' Make sure that a selection has been made for
        ' the document path.  Don't need to check the type
        ' as it's done when the combo box is filled.
        If cboDocPath.Enabled And cboDocPath.SelectedIndex = -1 Then
            errorProvider.SetError(cboDocPath, My.Resources.strNoDocPathLink)
            e.Cancel = True
        End If
    End Sub

    Private Sub txtKofaxPDFDir_Validating(ByVal sender As System.Object, ByVal e As System.ComponentModel.CancelEventArgs) Handles txtKofaxPDFDir.Validating
        ' Validate Kofax PDF release directory if checked
        If chkReleaseKofaxPDF.Checked Then
            ' Validate the Kofax PDF release directory if it is specified.
            ' If it doesn't exist, allow the user to create it.
            If Not String.IsNullOrEmpty(txtKofaxPDFDir.Text) Then
                If Not VerifyDirectoryName(txtKofaxPDFDir.Text, Me.Text) Then
                    errorProvider.SetError(txtKofaxPDFDir, txtKofaxPDFDir.Text & vbCrLf & My.Resources.strBadKfxPdfDirectory)
                    e.Cancel = True
                End If
            ElseIf m_setupData.KofaxPDFDocClassEnabled <> 0 Then
                ' The Kofax PDF directory is not required even if they
                ' have an Kofax PDF Generation queue spec'ed for the batch class
                ' but we warn the user if a directory is not specified.
                If DialogResult.OK = _
                    MessageBox.Show(My.Resources.strNoKfxPdfDirectory & vbCrLf, My.Resources.strDataVerify, MessageBoxButtons.OKCancel, MessageBoxIcon.Information) Then
                    tabDatabase.SelectedTab = documentStorageTabPage
                    txtKofaxPDFDir.Focus()
                    e.Cancel = True
                End If
            End If
        End If
    End Sub

    Private Sub txtOCRDir_Validating(ByVal sender As System.Object, ByVal e As System.ComponentModel.CancelEventArgs) Handles txtOCRDir.Validating
        ' Validate the OCR release directory if checked
        If chkReleaseOCRFullText.Checked Then
            ' Validate the OCR release directory if it is specified.
            ' If it doesn't exist, allow the user to create it.
            If Not String.IsNullOrEmpty(txtOCRDir.Text) Then
                If Not VerifyDirectoryName(txtOCRDir.Text, Me.Text) Then
                    errorProvider.SetError(txtOCRDir, txtOCRDir.Text & vbCrLf & My.Resources.strBadOcrDirectory)
                    e.Cancel = True
                End If
            ElseIf m_setupData.TextFileEnabled <> 0 Then
                ' The OCR directory is not required even if they
                ' have an OCR queue spec'ed for the Batch Class
                ' but we warn the user if a directory is not specified.
                If DialogResult.Cancel = _
                    MessageBox.Show(My.Resources.strNoOcrDirectory & vbCrLf & My.Resources.strOcrFilesDiscarded, _
                        My.Resources.strDataVerify, MessageBoxButtons.OKCancel, MessageBoxIcon.Information) Then

                    tabDatabase.SelectedTab = documentStorageTabPage
                    txtOCRDir.Focus()
                    e.Cancel = True
                End If
            End If
        End If
    End Sub

    '*************************************************
    ' chkReleaseImageFiles_Click
    '-------------------------------------------------
    ' Purpose:  The user toggled whether or not to
    '           release image files of each
    '           document.  It enables/disables
    '           various controls that are used to
    '           to release images.
    '           Mark the data dirty.
    '*************************************************
    Private Sub chkReleaseImageFiles_CheckedChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles chkReleaseImageFiles.CheckedChanged
        lblImageDir.Enabled = chkReleaseImageFiles.Checked
        txtImageDir.Enabled = chkReleaseImageFiles.Checked
        cmdIFBrowse.Enabled = chkReleaseImageFiles.Checked
        chkSkipFirstPage.Enabled = chkReleaseImageFiles.Checked
        fraReleaseImagesAs.Enabled = chkReleaseImageFiles.Checked
        lblImageType.Enabled = chkReleaseImageFiles.Checked
        cboImageType.Enabled = chkReleaseImageFiles.Checked
        Dirty = True
    End Sub

    Private Sub tabDatabase_Deselecting(ByVal sender As Object, ByVal e As System.Windows.Forms.TabControlCancelEventArgs) Handles tabDatabase.Deselecting
        If e.TabPage Is databaseTabPage AndAlso Not ValidateChildren(ValidationConstraints.Visible) Then
            e.Cancel = True
        End If
    End Sub

    'Form overrides dispose to clean up the component list.
    Protected Overloads Overrides Sub Dispose(ByVal Disposing As Boolean)
        If Disposing Then
            If Not components Is Nothing Then
                components.Dispose()
            End If

            oDB.Dispose()
        End If
        MyBase.Dispose(Disposing)
    End Sub

	Private Sub indexValuesDataGridView_Enter(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles indexValuesDataGridView.Enter
		'*** Put the selection back to the selection when the control last had the focus.
        If indexValuesSelectedRow IsNot Nothing Then
            indexValuesSelectedRow.Selected = True
        End If
    End Sub

    Private Sub indexValuesDataGridView_CellLeave(ByVal sender As System.Object, ByVal e As System.Windows.Forms.DataGridViewCellEventArgs) Handles indexValuesDataGridView.CellLeave
        indexValuesDataGridView.PerformValidation()
	End Sub

	Private Sub indexValuesDataGridView_Leave(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles indexValuesDataGridView.Leave
        indexValuesSelectedRow = indexValuesDataGridView.CurrentRow
        indexValuesDataGridView.ClearSelection()
    End Sub

    Private Sub tabDatabase_SelectedIndexChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles tabDatabase.SelectedIndexChanged
        indexValuesDataGridView.CurrentCell = Nothing
    End Sub
End Class
