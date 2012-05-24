'********************************************************************************
'***
'*** Class      OdbcFailedException
'*** Purpose    Exception thrown when populating the list of ODBC data sources
'***            which may occur if corrupt ODBC drivers are installed
'***
'*** (c) Copyright 2007 Kofax Image Products.
'*** All rights reserved.
'***
'********************************************************************************
Friend Class OdbcFailedException
    Inherits Exception

    Public Sub New()
        MyBase.New(My.Resources.strOdbcFailed)
    End Sub

    Public Sub New(ByVal message As String)
        MyBase.New(message)
    End Sub

    Public Sub New(ByVal message As String, ByVal innerException As Exception)
        MyBase.New(CStr(IIf(String.IsNullOrEmpty(message), My.Resources.strOdbcFailed, message)), innerException)
    End Sub
End Class
