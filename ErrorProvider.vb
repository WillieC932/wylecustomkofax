' This is a proxy for the full featured .NET ErrorProvider
' with the idea that one day we'll use it, but to avoid
' changing the UI for this project but follow the same
' coding patterns used for the .NET ErrorProvider I put
' this together.
Imports System.Collections.Generic
Imports System.ComponentModel

Class ErrorProvider
    Private items As New Dictionary(Of Control, String)

    Public Sub New(ByVal container As IContainer)
        ' container not used, but this constructor matches
        ' the .NET ErrorProvider one
    End Sub

    Public Sub Clear()
        items.Clear()
    End Sub

    Public Sub SetError(ByVal control As Control, ByVal value As String)
        items(control) = value
    End Sub

    Public Function GetError(ByVal control As Control) As String
        If items.ContainsKey(control) Then
            Return items(control)
        Else
            Return String.Empty
        End If
    End Function
End Class
