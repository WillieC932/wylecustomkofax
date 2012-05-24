'********************************************************************************
'***
'*** Class      ImageTypeComboBoxItem
'*** Purpose    Used to hold image types in a combo box
'***
'*** (c) Copyright 2007 Kofax Image Products.
'*** All rights reserved.
'***
'********************************************************************************
Imports Kofax.CapLib4.Interop

Friend Class ImageTypeComboBoxItem
    Private m_description As String
    Private m_type As CAP_IMAGE_FORMAT

    Public Sub New(ByVal imageType As ImageType)
        m_description = imageType.Description
        m_type = imageType.Type
    End Sub

    Public Sub New(ByVal description As String, ByVal type As CAP_IMAGE_FORMAT)
        m_description = description
        m_type = type
    End Sub

    Public Overrides Function ToString() As String
        Return m_description
    End Function

    Public ReadOnly Property Description() As String
        Get
            Return m_description
        End Get
    End Property

    Public ReadOnly Property Type() As CAP_IMAGE_FORMAT
        Get
            Return m_type
        End Get
    End Property
End Class
