'********************************************************************************
'***
'*** Module     ResConstants
'*** Purpose    Definition of some contants
'***
'*** (c) Copyright 2007 Kofax Image Products.
'*** All rights reserved.
'***
'********************************************************************************
Imports Kofax.ReleaseLib

Module ResConstants

    '===================
    ' Help Context IDs
    '===================
    Public Const TABS_FIRST_HELPID As Integer = &H26221
    Public Const ODBC_BROWSER_HELPID As Integer = &H26232
    Public Const ADOBE_ACROBAT_HELPID As Integer = &H26213

    '=====================
    ' Database Constants
    '=====================

    ' -- Database Type Constants --
    Public Const DBTYPE_ACCESS As Short = 1
    Public Const DBTYPE_ODBC As Short = 2
End Module