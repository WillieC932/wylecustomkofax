'********************************************************************************
'***
'*** Class      ComDisposer
'*** Purpose    Implements IDisposable for COM objects
'***
'*** (c) Copyright 2007 Kofax Image Products.
'*** All rights reserved.
'***
'********************************************************************************
Imports System.Runtime.InteropServices

Friend Class ComDisposer
    Implements IDisposable

    Private comObject As Object

    Public Sub New(ByVal comObject As Object)
        Me.comObject = comObject
    End Sub

    Public Sub Dispose() Implements IDisposable.Dispose
        If comObject IsNot Nothing Then
            Marshal.FinalReleaseComObject(comObject)
            comObject = Nothing
        End If
    End Sub
End Class
