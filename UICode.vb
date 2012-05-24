'********************************************************************************
'***
'*** Module     UtilCode
'*** Purpose    Utility class providing folder browsing
'***
'*** (c) Copyright 2007 Kofax Image Products.
'*** All rights reserved.
'***
'********************************************************************************
Imports System.IO
Imports System.Runtime.InteropServices
Imports System.Text

Module UtilCode

    '*************************************************
    ' SetComboValue
    '-------------------------------------------------
    ' Purpose:  This routine will set the passed in
    '           combo box to the passed in value.
    '           If the value does not exist in the
    '           combo box, it will raise an error.
    ' Inputs:   cboCombo    Reference to a combobox
    '           entryName   search string
    ' Ouputs:   None
    ' Returns:  True/False indicating if the string
    '           was found in the combo box.
    ' Notes:    None
    '*************************************************
    Function SetComboValue(ByVal cboCombo As System.Windows.Forms.ComboBox, ByVal entryName As String) As Boolean
        Dim I As Integer

        ' Search for passed in value, if it
        ' exists, set the listindex and exit
        For I = 0 To cboCombo.Items.Count - 1
            If entryName = VB6.GetItemString(cboCombo, I) Then
                cboCombo.SelectedIndex = I
                Return True
            End If
        Next I

        cboCombo.SelectedIndex = -1
        Return False
    End Function

    '*************************************************
    ' VerifyDirectoryName
    '-------------------------------------------------
    ' Purpose:  Check the passed in directory, and if
    '           it does not exist, allow the user the
    '           option to create it.
    ' Inputs:   DirName        directory name to verify
    '           strErrCaption  caption of the error message if 
    '                          an error occurs when creating the directory
    ' Outputs:  None
    ' Returns:  True/False if vlaid directory name
    ' Notes:    None
    '*************************************************
    Public Function VerifyDirectoryName(ByVal DirName As String, ByVal strErrCaption As String) As Boolean
        Dim CurrentDirectory As String = String.Empty

        If String.IsNullOrEmpty(DirName) Then
            Return False
        End If

        If Directory.Exists(DirName) Then
            Return True
        End If

        If DialogResult.Yes = MessageBox.Show( _
         String.Format(My.Resources.strAskToCreateDir, DirName), _
         My.Resources.strTitleDirNotExist, _
         MessageBoxButtons.YesNo, MessageBoxIcon.Question) Then
            Try
                Directory.CreateDirectory(DirName)
            Catch ex As Exception
                MessageBox.Show(ex.Message, strErrCaption, MessageBoxButtons.OK, MessageBoxIcon.Error)
                Return False
            End Try
            Return True
        Else
            Return False
        End If
    End Function

End Module