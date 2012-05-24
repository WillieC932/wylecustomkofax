'********************************************************************************
'***
'*** Module: Help.vb
'*** Purpose: Adapter for using KHelpFinder
'***
'*** (c) Copyright 2008 Kofax Image Products.
'*** All rights reserved.
'********************************************************************************
Imports Kofax.KHelpFinder
Imports System.IO
Imports System.Reflection
Imports System.Runtime.InteropServices

Module Help
    Public Sub Show(ByVal contextId As Integer)
        Dim chmHlp As New KChmHlp()
        chmHlp.ShowHelp(Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly.Location), "MikeDBRel.ini"), contextId)
        Marshal.FinalReleaseComObject(chmHlp)
    End Sub

    Public ReadOnly Property ServicePack() As String
        Get
            Dim chmHlp As New KChmHlp()
            Dim value As String = chmHlp.ServicePack
            Marshal.FinalReleaseComObject(chmHlp)
            Return value
        End Get
    End Property
End Module
