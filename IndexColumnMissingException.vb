'********************************************************************************
'***
'*** Class      IndexColumnMissingException
'*** Purpose    Definition of the exception thrown when an index field doesn't
'***            exist in the database because maybe the column has been deleted
'***
'*** (c) Copyright 2007 Kofax Image Products.
'*** All rights reserved.
'***
'********************************************************************************
Public Class IndexColumnMissingException
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
