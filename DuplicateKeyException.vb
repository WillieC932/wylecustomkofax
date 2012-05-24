'********************************************************************************
'***
'*** Class      DuplicateKeyException
'*** Purpose    Definition of the exception thrown whe the database rejects
'***            a document due to key violations
'***
'*** (c) Copyright 2007 Kofax Image Products.
'*** All rights reserved.
'***
'********************************************************************************
Public Class DuplicateKeyException
    Inherits Exception

    Public Sub New()
        MyBase.New()
    End Sub

    Public Sub New(ByVal message As String)
        MyBase.New(message)
    End Sub

    Public Sub New(ByVal message As String, ByVal innerException As Exception)
        MyBase.New(message, innerException)
    End Sub
End Class
