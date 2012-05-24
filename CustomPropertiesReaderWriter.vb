'********************************************************************************
'***
'*** Class      CustomPropertiesReaderWriter
'*** Purpose    Facade for accessing an export connector's custom properties
'***
'*** (c) Copyright 2007 Kofax Image Products.
'*** All rights reserved.
'***
'********************************************************************************

Imports Kofax.ReleaseLib
Imports System.Diagnostics
Imports System.Runtime.InteropServices

Friend Class CustomPropertiesReaderWriter
    Implements IDisposable

    Private customProperties As ICustomProperties2

    '=============================
    ' Keys for Custom Properties
    '=============================
    Private Const KEY_ADOBE_DELETE_HUNG As String = "Adobe Delete Hung"
    Private Const KEY_ADOBE_WAIT_FOR_STATUS As String = "Adobe Wait For Status"
    Private Const KEY_ASCIIFILE As String = "ASCII File Name"
    Private Const KEY_CONTYPE As String = "Connection Type"
    Private Const KEY_DISABLE_IMAGE_EXPORT As String = "DisableImageExport"
    Private Const KEY_DISABLE_TEXT_EXPORT As String = "DisableTextExport"
    Private Const KEY_DOCID As String = "Document ID Column"
    Private Const KEY_DOCPATH As String = "Document Path Column"
    Private Const KEY_DOCTABLE As String = "Documents Table"
    Private Const KEY_ENABLE_KOFAX_PDF_EXPORT As String = "EnableKofaxPDFExport"
    Private Const KEY_SYSTEMMDB As String = "System MDB"

    Public Sub New(ByVal customProperties As CustomProperties)
        Debug.Assert(customProperties IsNot Nothing)
        Me.customProperties = CType(customProperties, ICustomProperties2)
    End Sub

    Public Property AdobeDeleteHung() As Boolean
        Get
            Dim customProperty As CustomProperty
            customProperty = customProperties.TryGetValue(KEY_ADOBE_DELETE_HUNG)
            Dim bValue As Boolean = False
            If customProperty IsNot Nothing Then
                Using New ComDisposer(customProperty)
                    bValue = ConvertToBooleanValue(customProperty.Value)
                End Using
            End If
            Return bValue
        End Get
        Set(ByVal value As Boolean)
            customProperties.Add(KEY_ADOBE_DELETE_HUNG, value.ToString())
        End Set
    End Property

    Public Property AdobeWaitForStatus() As Boolean
        Get
            Dim customProperty As CustomProperty
            customProperty = customProperties.TryGetValue(KEY_ADOBE_WAIT_FOR_STATUS)
            Dim bValue As Boolean = False
            If customProperty IsNot Nothing Then
                Using New ComDisposer(customProperty)
                    bValue = ConvertToBooleanValue(customProperty.Value)
                End Using
            End If
            Return bValue
        End Get
        Set(ByVal value As Boolean)
            customProperties.Add(KEY_ADOBE_WAIT_FOR_STATUS, value.ToString())
        End Set
    End Property

    Public Property AsciiFile() As String
        Get
            Return customProperties.Item(KEY_ASCIIFILE).Value
        End Get
        Set(ByVal value As String)
            customProperties.Add(KEY_ASCIIFILE, value)
        End Set
    End Property

    Public Property ConnectionType() As Integer
        Get
            Return Integer.Parse(customProperties.Item(KEY_CONTYPE).Value)
        End Get
        Set(ByVal value As Integer)
            customProperties.Add(KEY_CONTYPE, value.ToString())
        End Set
    End Property

    Public Property DisableImageExport() As Boolean
        Get
            Dim customProperty As CustomProperty
            customProperty = customProperties.TryGetValue(KEY_DISABLE_IMAGE_EXPORT)
            Dim bValue As Boolean = False
            If customProperty IsNot Nothing Then
                Using New ComDisposer(customProperty)
                    bValue = ConvertToBooleanValue(customProperty.Value)
                End Using
            End If
            Return bValue
        End Get
        Set(ByVal value As Boolean)
            customProperties.Add(KEY_DISABLE_IMAGE_EXPORT, value.ToString())
        End Set
    End Property

    Public Property DisableTextExport() As Boolean
        Get
            Dim customProperty As CustomProperty
            customProperty = customProperties.TryGetValue(KEY_DISABLE_TEXT_EXPORT)
            Dim bValue As Boolean = False
            If customProperty IsNot Nothing Then
                Using New ComDisposer(customProperty)
                    bValue = ConvertToBooleanValue(customProperty.Value)
                End Using
            End If
            Return bValue
        End Get
        Set(ByVal value As Boolean)
            customProperties.Add(KEY_DISABLE_TEXT_EXPORT, value.ToString())
        End Set
    End Property

    Public Property DocId() As String
        Get
            Return customProperties.Item(KEY_DOCID).Value
        End Get
        Set(ByVal value As String)
            customProperties.Add(KEY_DOCID, value)
        End Set
    End Property

    Public Property DocPath() As String
        Get
            Return customProperties.Item(KEY_DOCPATH).Value
        End Get
        Set(ByVal value As String)
            customProperties.Add(KEY_DOCPATH, value)
        End Set
    End Property

    Public Property DocTable() As String
        Get
            Return customProperties.Item(KEY_DOCTABLE).Value
        End Get
        Set(ByVal value As String)
            customProperties.Add(KEY_DOCTABLE, value)
        End Set
    End Property

    Public Property EnableKofaxPDFExport() As Boolean
        Get
            Dim customProperty As CustomProperty
            customProperty = customProperties.TryGetValue(KEY_ENABLE_KOFAX_PDF_EXPORT)
            Dim bValue As Boolean = False
            If customProperty IsNot Nothing Then
                Using New ComDisposer(customProperty)
                    bValue = ConvertToBooleanValue(customProperty.Value)
                End Using
            End If
            Return bValue
        End Get
        Set(ByVal value As Boolean)
            customProperties.Add(KEY_ENABLE_KOFAX_PDF_EXPORT, CStr(value))
        End Set
    End Property

    Public Property SystemMdb() As String
        Get
            Return customProperties.Item(KEY_SYSTEMMDB).Value
        End Get
        Set(ByVal value As String)
            customProperties.Add(KEY_SYSTEMMDB, value)
        End Set
    End Property

    ''' <summary>
    ''' Convert property string value to boolean.
    ''' </summary>
    ''' <param name="strValue"></param>
    ''' <returns>boolean value if can convert; otherwise false</returns>
    ''' <remarks></remarks>
    Private Function ConvertToBooleanValue(ByVal strValue As String) As Boolean

        Dim bValue As Boolean = False

        If Not String.IsNullOrEmpty(strValue) Then
            If Not Boolean.TryParse(strValue, bValue) Then
                '*** For backward compatible, in case the value 
                '*** was stored with localized string, such as
                '*** 'Wahr' (in German) instead of 'True'
                bValue = String.Equals( _
                            strValue, _
                            My.Resources.TXT_LocalizedValueTrue, _
                            StringComparison.OrdinalIgnoreCase)
            End If
        End If

        Return bValue
    End Function

    Public Sub Dispose() Implements IDisposable.Dispose
        If customProperties IsNot Nothing Then
            Marshal.ReleaseComObject(customProperties)
            customProperties = Nothing
        End If
    End Sub

End Class
