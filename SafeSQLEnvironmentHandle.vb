'********************************************************************************
'***
'*** Class      SafeSQLEnvironmentHandle
'*** Purpose    This is a safe Win32 handle type for Odbc environment handles
'***
'*** (c) Copyright 2007-2008 Kofax Image Products.
'*** All rights reserved.
'***
'********************************************************************************
Imports Microsoft.Win32.SafeHandles
Imports System.Runtime.InteropServices

Friend Class SafeSQLEnvironmentHandle
    Inherits SafeHandleZeroOrMinusOneIsInvalid

    Private Const SQL_SUCCESS As Integer = 0
    Private Const SQL_SUCCESS_WITH_INFO As Short = 1

    <DllImport("odbc32")> _
    Private Shared Function SQLAllocEnv(ByRef hHandle As IntPtr) As Short
    End Function

    <DllImport("odbc32")> _
    Private Shared Function SQLFreeEnv(ByVal hHandle As IntPtr) As Short
    End Function

    Public Sub New()
        MyBase.New(True)
    End Sub

    Public Sub New(ByVal preexistingHandle As IntPtr, ByVal ownsHandle As Boolean)
        MyBase.New(ownsHandle)
        SetHandle(preexistingHandle)
    End Sub

    Public Shared Function SQLAllocEnv() As SafeSQLEnvironmentHandle
        Dim hEnvironment As IntPtr
        Dim rc As Integer = SQLAllocEnv(hEnvironment)
        If SQL_SUCCESS <> rc And SQL_SUCCESS_WITH_INFO <> rc Then
            Throw New OdbcEnvironmentException("Error allocating ODBC environment handle")
        End If
        Return New SafeSQLEnvironmentHandle(hEnvironment, True)
    End Function

    Protected Overrides Function ReleaseHandle() As Boolean
        Return SQL_SUCCESS = SQLFreeEnv(MyBase.handle)
    End Function
End Class
