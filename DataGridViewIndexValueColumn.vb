'********************************************************************************
'***
'*** Class      DataGridViewIndexValueColumn
'*** Purpose    Definition of the custom DataGridViewColumn for index value types
'***
'*** (c) Copyright 2007 Kofax Image Products.
'*** All rights reserved.
'***
'********************************************************************************
Imports System.Windows.Forms

Friend Class DataGridViewIndexValueColumn
    Inherits DataGridViewColumn

    Public Sub New()
        MyBase.New(New DataGridViewIndexValueCell())
        Me.Name = "Source"
        Me.HeaderText = My.Resources.strIndexValue
        DefaultCellStyle.BackColor = SystemColors.Window
        DefaultCellStyle.ForeColor = SystemColors.WindowText
    End Sub
End Class
