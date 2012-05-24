'/****************************************************************************
'*	(c) Copyright Kofax Image Products, 2006. All rights reserved.
'*	Unauthorized use, duplication or distribution is strictly prohibited.
'*****************************************************************************
'*
'*	File:		ComEnumerator.vb
'*
'*	Purpose:	Implements IDisposable for COM objects
'*
'****************************************************************************/
Imports System.Collections
Imports System.Runtime.InteropServices

Friend Class ComEnumerator
    Implements IEnumerator, IDisposable

    Private enumerator As IEnumerator

    Public Sub New(ByVal enumerator As IEnumerator)
        Me.enumerator = enumerator
    End Sub

    Public ReadOnly Property Current() As Object Implements System.Collections.IEnumerator.Current
        Get
            Return enumerator.Current
        End Get
    End Property

    Public Function MoveNext() As Boolean Implements System.Collections.IEnumerator.MoveNext
        Return enumerator.MoveNext()
    End Function

    Public Sub Reset() Implements System.Collections.IEnumerator.Reset
        enumerator.Reset()
    End Sub

    ' This code added by Visual Basic to correctly implement the disposable pattern.
    Public Sub Dispose() Implements IDisposable.Dispose
        Dim adapter As ICustomAdapter = CType(enumerator, ICustomAdapter)
        Marshal.FinalReleaseComObject(adapter.GetUnderlyingObject())
    End Sub
End Class
