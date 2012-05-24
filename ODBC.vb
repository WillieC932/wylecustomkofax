'********************************************************************************
'***
'*** Class      frmODBC
'*** Purpose    Definition of the form for selecting an ODBC data source
'***
'*** (c) Copyright 2007 Kofax Image Products.
'*** All rights reserved.
'***
'********************************************************************************
Imports System.Data.Odbc
Imports System.Runtime.InteropServices
Imports System.Text
Imports VB = Microsoft.VisualBasic

Friend Class frmODBC
    Inherits System.Windows.Forms.Form

    Private m_strConnectionString As String

    Public ReadOnly Property ConnectionString() As String
        Get
            Return m_strConnectionString
        End Get
    End Property

    '*************************************************
    ' cmdHelp_Click
    '-------------------------------------------------
    ' Purpose:  Display the online help for the ODBC
    '           browse dialog.
    ' Inputs:   None
    ' Outputs:  None
    ' Returns:  None
    ' Notes:    None
    '*************************************************
    Private Sub cmdHelp_Click(ByVal eventSender As System.Object, ByVal eventArgs As System.EventArgs) Handles cmdHelp.Click
        Help.Show(ODBC_BROWSER_HELPID)
    End Sub

    '*************************************************
    ' cmdOK_Click
    '-------------------------------------------------
    ' Purpose:  Attempt to connect to the selected
    '           DSN.  If successful, hide the dialog.
    '*************************************************
    Private Sub cmdOK_Click(ByVal eventSender As System.Object, ByVal eventArgs As System.EventArgs) Handles cmdOK.Click
        ' Make sure the user selected a DSN from the list
        If lstDSN.SelectedIndex <> -1 Then
            ' Attempt to open the selected DSN.
            Dim oDB As New ADODB.Connection
            Try
                oDB.Properties("Prompt").Value = ADODB.ConnectPromptEnum.adPromptComplete
                oDB.Open(String.Concat("Persist Security Info=True;DSN=", lstDSN.SelectedItem.ToString()))
                m_strConnectionString = oDB.ConnectionString
                DialogResult = Windows.Forms.DialogResult.OK
            Catch ex As COMException
                ' The user cancelled out of the chance to log into the data source
                If ex.ErrorCode = &H80040E4E Then
                    DialogResult = Windows.Forms.DialogResult.None
                Else
                    DialogResult = Windows.Forms.DialogResult.None
                    MessageBox.Show(ex.Message, Me.Text, MessageBoxButtons.OK, MessageBoxIcon.Exclamation)
                End If
            Finally
                Marshal.FinalReleaseComObject(oDB)
            End Try
        End If
    End Sub

    '*************************************************
    ' OnLoad
    '-------------------------------------------------
    ' Purpose:  Load the resource strings for the UI
    '           controls, initialize the items that
    '           are set after a valid DSN connection,
    '           and populate the listbox with valid
    '           machine data sources.
    ' Inputs:   None
    ' Outputs:  None
    ' Returns:  None
    ' Notes:    None
    '*************************************************
    Protected Overrides Sub OnLoad(ByVal e As System.EventArgs)
        MyBase.OnLoad(e)

        ' Initialize the dialog
        ' Load the listbox with valid machine data sources
        PopulateList()
    End Sub

    '*************************************************
    ' PopulateList
    '-------------------------------------------------
    ' Purpose:  Use the ODBC API calls to get a list
    '           of all machine (User and System) data
    '           sources.  Populate the listbox with
    '           the DSNs.
    ' Inputs:   None
    ' Outputs:  None
    ' Returns:  True/False indicating if the listbox
    '           was populated successfully.
    ' Notes:    None
    '*************************************************
    Public Sub PopulateList()
        Dim intRetVal As Short ' Return value from ODBC API functions.
        Dim intDSN As Short ' Length of DSN name.
        Dim intDescription As Short ' Length of Description.

        Try
            ' Clear the list box before we populate it.
            lstDSN.Items.Clear()

            ' Allocate the strings used to retrieve information from the API.
            Dim strDSN As New StringBuilder(256)
            Dim strDescription As New StringBuilder(256)

            ' Allocate the ODBC environment and get a valid environment handle.
            Using hEnv As SafeSQLEnvironmentHandle = SafeSQLEnvironmentHandle.SQLAllocEnv()
                ' Get the first ODBC data source in the list.
                intRetVal = SQLDataSourcesW(hEnv, SQL_FETCH_FIRST, strDSN, CShort(strDSN.Capacity), intDSN, strDescription, CShort(strDescription.Capacity), intDescription)

                ' Iterate through all of the data sources and add them to the list.
                While intRetVal = SQL_SUCCESS Or intRetVal = SQL_SUCCESS_WITH_INFO
                    ' Add the DSN to the list box.
                    lstDSN.Items.Add(strDSN.ToString())

                    ' Get the next data source.
                    intRetVal = SQLDataSourcesW(hEnv, SQL_FETCH_NEXT, strDSN, CShort(strDSN.Capacity), intDSN, strDescription, CShort(strDescription.Capacity), intDescription)
                End While
            End Using

            '*** Set selection in ODBC listbox
            If lstDSN.Items.Count > 0 Then
                lstDSN.SelectedIndex = 0
            End If

            '*** Enaable OK if button was selected.
            If lstDSN.SelectedIndex <> -1 Then
                cmdOK.Enabled = True
            End If
        Catch ex As OdbcEnvironmentException
            MessageBox.Show(My.Resources.strOdbcEnvironment, My.Resources.strTitleReleaseSetupError, MessageBoxButtons.OK, MessageBoxIcon.Exclamation)
            Return
        Catch ex As Exception
            Throw New OdbcFailedException(Nothing, ex)
        End Try
    End Sub

    '*************************************************
    ' lstDSN_Click
    '-------------------------------------------------
    ' Purpose:  Clicking a DSN in the listbox
    '           indicates the user is selecting an
    '           item.  If a DSN was selected enable
    '           the OK button.
    '*************************************************
    Private Sub lstDSN_SelectedIndexChanged(ByVal eventSender As System.Object, ByVal eventArgs As System.EventArgs) Handles lstDSN.SelectedIndexChanged
        If lstDSN.SelectedIndex <> -1 Then
            cmdOK.Enabled = True
        End If
    End Sub

    '*************************************************
    ' lstDSN_DblClick
    '-------------------------------------------------
    ' Purpose:  Double-clicking a DSN in the listbox
    '           indicates the user is selecting that
    '           item.  If a DSN was selected, force a
    '           simulated click of the OK button.
    '*************************************************
    Private Sub lstDSN_DoubleClick(ByVal eventSender As System.Object, ByVal eventArgs As System.EventArgs) Handles lstDSN.DoubleClick
        ' Double-clicking an item in the listbox automatically
        ' acts like the user clicked OK after selecting an item
        If lstDSN.SelectedIndex <> -1 Then
            cmdOK_Click(cmdOK, New System.EventArgs())
        End If
    End Sub

    Protected Overrides Sub OnHelpRequested(ByVal hevent As System.Windows.Forms.HelpEventArgs)
        MyBase.OnHelpRequested(hevent)

        If Not hevent.Handled Then
            Help.Show(ODBC_BROWSER_HELPID)
            hevent.Handled = True
        End If
    End Sub
End Class