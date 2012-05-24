'********************************************************************************
'***
'*** Module     ODBCAPI
'*** Purpose    Declaration of odbc32 APIs
'***
'*** (c) Copyright 2007 Kofax Image Products.
'*** All rights reserved.
'***
'********************************************************************************
Imports System.Runtime.InteropServices
Imports System.Text

Module ODBCAPI
    '=========================================
    ' Function declarations for the ODBC API
    '=========================================
    <DllImport("odbc32", CharSet:=CharSet.Unicode)> _
    Public Function SQLDataSourcesW(ByVal hEnv As SafeSQLEnvironmentHandle, ByVal fDirection As Short, ByVal szDSN As StringBuilder, ByVal cbDSNMax As Short, ByRef pcbDSN As Short, ByVal szDescription As StringBuilder, ByVal cbDescriptionMax As Short, ByRef pcbDescription As Short) As Short
    End Function

    '=================================
    ' Constants used by the ODBC API
    '=================================
    ' -- Function Return Codes --
    Public Const SQL_SUCCESS As Short = 0
    Public Const SQL_SUCCESS_WITH_INFO As Short = 1

    ' -- Data Source Direction --
    Public Const SQL_FETCH_NEXT As Short = 1
    Public Const SQL_FETCH_FIRST As Short = 2
End Module