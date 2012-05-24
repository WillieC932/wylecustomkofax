'********************************************************************************
'***
'*** Class      DatabaseConnectionException
'*** Purpose    Exception thrown when the database can't be opened
'***
'*** (c) Copyright 2007-2008 Kofax Image Products.
'*** All rights reserved.
'***
'********************************************************************************
Public Class DatabaseConnectionException
    Inherits Exception

    Public Sub New()
        MyBase.New(My.Resources.strNotConnectedToDb)
    End Sub

    Public Sub New(ByVal message As String)
        MyBase.New(message)
    End Sub

    Public Sub New(ByVal message As String, ByVal innerException As Exception)
        MyBase.New(CStr(IIf(String.IsNullOrEmpty(message), My.Resources.strNotConnectedToDb, message)), innerException)
    End Sub
End Class
