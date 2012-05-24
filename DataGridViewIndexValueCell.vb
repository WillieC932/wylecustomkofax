'********************************************************************************
'***
'*** Class      DataGridViewIndexValueCell
'*** Purpose    Definition of the custom DataGridViewCell for index field values
'***
'*** (c) Copyright 2007 Kofax Image Products.
'*** All rights reserved.
'***
'********************************************************************************
Imports Kofax.ReleaseLib
Imports System.Windows.Forms
Imports System.Collections.Generic
Imports System.ComponentModel
Imports System.Text.RegularExpressions
Imports SourceType = Kofax.ReleaseLib.KfxLinkSourceType

Friend Class DataGridViewIndexValueCell
    Inherits DataGridViewTextBoxCell

    Public Overrides ReadOnly Property EditType() As System.Type
        Get
            Return GetType(DataGridViewIndexValueEditingControl)
        End Get
    End Property

    Protected Overrides Function GetFormattedValue(ByVal value As Object, ByVal rowIndex As Integer, ByRef cellStyle As System.Windows.Forms.DataGridViewCellStyle, ByVal valueTypeConverter As System.ComponentModel.TypeConverter, ByVal formattedValueTypeConverter As System.ComponentModel.TypeConverter, ByVal context As System.Windows.Forms.DataGridViewDataErrorContexts) As Object
        Dim formattedValue As String = CStr(GetValue(rowIndex))

        Dim sourceType As SourceType = CType(DataGridView(2, rowIndex).Value, SourceType)
        Select Case sourceType
            Case KfxLinkSourceType.KFX_REL_UNDEFINED_LINK
                cellStyle.Font = VB6.FontChangeBold(cellStyle.Font, False)
                formattedValue = String.Empty
            Case GetSourceType(KfxLinkSourceType.KFX_REL_TEXTCONSTANT)
                cellStyle.Font = VB6.FontChangeBold(cellStyle.Font, False)
                formattedValue = String.Format("""{0}""", formattedValue)
            Case GetSourceType(KfxLinkSourceType.KFX_REL_VARIABLE)
                cellStyle.Font = VB6.FontChangeBold(cellStyle.Font, True)
                formattedValue = String.Format("{{{0}}}", formattedValue)
            Case GetSourceType(KfxLinkSourceType.KFX_REL_INDEXFIELD)
                cellStyle.Font = VB6.FontChangeBold(cellStyle.Font, True)
            Case GetSourceType(KfxLinkSourceType.KFX_REL_BATCHFIELD)
                cellStyle.Font = VB6.FontChangeBold(cellStyle.Font, True)
                formattedValue = String.Format("{{${0}}}", formattedValue)
            Case GetSourceType(KfxLinkSourceType.KFX_REL_DOCUMENTID)
                cellStyle.Font = VB6.FontChangeBold(cellStyle.Font, True)
                formattedValue = String.Format("{{{0}}}", formattedValue)
        End Select

        Return formattedValue
    End Function

    Protected Overrides Function GetValue(ByVal rowIndex As Integer) As Object
        Dim links As List(Of Link) = CType(DataGridView.DataSource, List(Of Link))
        Return links(rowIndex).Source
    End Function

    ' We have to un-format values here
    Protected Overrides Function SetValue(ByVal rowIndex As Integer, ByVal value As Object) As Boolean
        If value IsNot Nothing Then
            Dim sourceType As SourceType = CType(DataGridView(2, rowIndex).Value, SourceType)
            Select Case sourceType
                Case GetSourceType(KfxLinkSourceType.KFX_REL_TEXTCONSTANT)
                    value = Regex.Replace(CStr(value), "^""|""$", "")
                Case GetSourceType(KfxLinkSourceType.KFX_REL_VARIABLE)
                    value = Regex.Replace(CStr(value), "^{|}$", "")
                Case GetSourceType(KfxLinkSourceType.KFX_REL_BATCHFIELD)
                    value = Regex.Replace(CStr(value), "^{\$|}$", "")
                Case GetSourceType(KfxLinkSourceType.KFX_REL_DOCUMENTID)
                    value = Regex.Replace(CStr(value), "^{|}$", "")
            End Select
        End If

        Return MyBase.SetValue(rowIndex, value)
    End Function

    Private Function GetSourceType(ByVal sourceType As KfxLinkSourceType) As SourceType
        Return CType(sourceType, SourceType)
    End Function
End Class
