'********************************************************************************
'***
'*** Class      DataConvertException
'*** Purpose    Definition of the DataConvertException thrown when an index
'***            table is released and the field value can't be converted
'***            correctly
'***
'*** (c) Copyright 2007 Kofax Image Products.
'*** All rights reserved.
'***
'********************************************************************************
Friend Class DataConvertException
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
