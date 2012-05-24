'********************************************************************************
'***
'*** Class      IndexValueTextBox
'*** Purpose    Defines a TextBox control that allows the Enter key to fire
'***            a KeyDown event
'***
'*** (c) Copyright 2008 Kofax Image Products.
'*** All rights reserved.
'***
'********************************************************************************
Public Class IndexValueTextBox
    Inherits TextBox

    Protected Overrides Function ProcessCmdKey(ByRef msg As System.Windows.Forms.Message, ByVal keyData As System.Windows.Forms.Keys) As Boolean
        If msg.Msg = 256 And (keyData = Keys.Enter And keyData = Keys.Return) Then
            OnKeyDown(New KeyEventArgs(keyData))
            Return True
        Else
            Return MyBase.ProcessCmdKey(msg, keyData)
        End If
    End Function
End Class
