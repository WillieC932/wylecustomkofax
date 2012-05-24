'********************************************************************************
'***
'*** Class      Link
'*** Purpose    Definition of the Link type that contains Destination, Source,
'***            and SourceType
'***
'*** (c) Copyright 2007 Kofax Image Products.
'*** All rights reserved.
'***
'********************************************************************************
Imports Kofax.ReleaseLib
Imports SourceType = Kofax.ReleaseLib.KfxLinkSourceType

Friend Class Link
    Private m_destination As String
    Private m_sourceType As SourceType
    Private m_source As String

    Public Sub New(ByVal destination As String)
        m_destination = destination
        m_sourceType = KfxLinkSourceType.KFX_REL_UNDEFINED_LINK
        m_source = String.Empty
    End Sub

    Public Sub New(ByVal destination As String, ByVal sourceType As SourceType, ByVal source As String)
        m_destination = destination
        m_sourceType = sourceType
        m_source = source
    End Sub

    Public Property Destination() As String
        Get
            Return m_destination
        End Get
        Set(ByVal value As String)
            m_destination = value
        End Set
    End Property

    Public Property SourceType() As SourceType
        Get
            Return m_sourceType
        End Get
        Set(ByVal value As SourceType)
            m_sourceType = value
        End Set
    End Property

    Public Property Source() As String
        Get
            Return m_source
        End Get
        Set(ByVal value As String)
            m_source = value
        End Set
    End Property
End Class
