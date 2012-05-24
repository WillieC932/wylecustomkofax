'********************************************************************************
'***
'*** Class      ImageType
'*** Purpose    Adapter to provide access to properties in an image type returned
'***            by ReleaseSetupData using late binding because
'***            ReleaseSetupData.ImageTypes returns a value of type "Object"
'***
'*** (c) Copyright 2007 Kofax Image Products.
'*** All rights reserved.
'***
'********************************************************************************
Option Strict Off
Imports Kofax.CapLib4.Interop

Friend Class ImageType
    Private imageType As Object
    Public Sub New(ByVal imageType As Object)
        Me.imageType = imageType
    End Sub

    Public ReadOnly Property Description() As String
        Get
            Return imageType.Description
        End Get
    End Property

    Public ReadOnly Property MultiplePage() As Boolean
        Get
            Return imageType.MultiplePage
        End Get
    End Property

    Public ReadOnly Property Type() As CAP_IMAGE_FORMAT
        Get
            Return imageType.Type
        End Get
    End Property
End Class
