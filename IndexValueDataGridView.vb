'********************************************************************************
'***
'*** Class      IndexValueDataGridView
'*** Purpose    Defines a DataGridView that contains a reference to
'***            ReleaseSetupData and a work around to what looks like
'***            a bug with DataGridViews and COM Interop where validation
'***            doesn't occur so we have to force Validating events to
'***            fire
'***
'*** (c) Copyright 2008 Kofax Image Products.
'*** All rights reserved.
'***
'********************************************************************************
Imports System.ComponentModel

Public Class IndexValueDataGridView
    Inherits DataGridView

    Private m_setupData As Kofax.ReleaseLib.ReleaseSetupData

    Public Property SetupData() As Kofax.ReleaseLib.ReleaseSetupData
        Get
            Return m_setupData
        End Get
        Set(ByVal value As Kofax.ReleaseLib.ReleaseSetupData)
            m_setupData = value
        End Set
    End Property

    ''' <summary>
    ''' Causes the DataGridView to fire the Validating event
    ''' </summary>
    ''' <remarks>
    ''' This is a workaround to a problem where validating events
    ''' aren't getting fired when the DataGridView loses focus when
    ''' used with COM Interop (or something else we're doing)
    ''' </remarks>
    Public Sub PerformValidation()
        OnValidating(New CancelEventArgs())
    End Sub
End Class
