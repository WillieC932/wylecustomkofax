Friend Class OdbcEnvironmentException
    Inherits Exception

    Public Sub New()
        Me.New(Nothing, Nothing)
    End Sub

    Public Sub New(ByVal message As String)
        Me.New(message, Nothing)
    End Sub

    Public Sub New(ByVal message As String, ByVal innerException As Exception)
        MyBase.New(CStr(IIf(message Is Nothing, My.Resources.strOdbcEnvironment, message)), innerException)
    End Sub
End Class
