'********************************************************************************
'***
'*** Class      KfxReleaseSetupScript
'*** Purpose    Definition of COM KfxReleaseSetupScript coclass used by Kofax
'***            Capture during export connector setup
'***
'*** (c) Copyright 2007 Kofax Image Products.
'*** All rights reserved.
'***
'********************************************************************************
Imports Kofax.CapLib4.Interop
Imports System.Data.Common
Imports System.Runtime.InteropServices

<ComVisible(True)> _
<Guid("E4F74A8C-7DA3-407f-A6E2-C06DC3277C9E")> _
<InterfaceType(ComInterfaceType.InterfaceIsIDispatch)> _
Public Interface IKfxReleaseSetupScript
    Property SetupData() As Kofax.ReleaseLib.ReleaseSetupData
    Function CloseScript() As Kofax.ReleaseLib.KfxReturnValue
    Function ActionEvent(ByRef intActionID As Kofax.ReleaseLib.KfxActionValue, ByRef strData1 As String, ByRef strData2 As String) As Kofax.ReleaseLib.KfxReturnValue
    Function OpenScript() As Kofax.ReleaseLib.KfxReturnValue
    Function RunUI() As Kofax.ReleaseLib.KfxReturnValue
End Interface

<ComVisible(True)> _
<ProgId("CustomDBRelPdfRename.kfxreleasesetupscript")> _
<Guid("96559C06-48F1-4bcd-BFEF-1A9143D7AA85")> _
<ClassInterface(ClassInterfaceType.None)> _
Public Class KfxReleaseSetupScript
    Implements IKfxReleaseSetupScript, IDisposable

    Private Const M_RSETUP As String = "Wyle Database Release Setup Script"

    ' ReleaseSetupData object set by the release
    ' setup controller.  This object is to be used during
    ' the document type setup process.  It will contain
    ' all of the information and interfaces you need to
    ' define a document type's release process.
    Private m_SetupData As Kofax.ReleaseLib.ReleaseSetupData

    ' When a link to a required database field is deleted,
    ' we want to show the UI with the form initialized as
    ' dirty so that validation is forced to occur.
    Dim bDirtyForm As Boolean

    Public Property SetupData() As Kofax.ReleaseLib.ReleaseSetupData Implements IKfxReleaseSetupScript.SetupData
        Get
            Return m_SetupData
        End Get
        Set(ByVal value As Kofax.ReleaseLib.ReleaseSetupData)
            m_SetupData = value
        End Set
    End Property

    '*************************************************
    ' CloseScript
    '-------------------------------------------------
    ' Purpose:  Script release point.  Perform any
    '           necessary cleanup such as releasing
    '           resources, etc.
    ' Inputs:   None
    ' Outputs:  None
    ' Returns:  One of the following:
    '             KFX_REL_SUCCESS, KFX_REL_ERROR,
    '             KFX_REL_FATALERROR, KFX_REL_REINIT
    '             KFX_REL_DOCCLASSERROR,
    ' Notes:    Called by Release Setup Controller
    '           once just before the script object
    '           is released.
    '*************************************************
    Public Function CloseScript() As Kofax.ReleaseLib.KfxReturnValue _
        Implements IKfxReleaseSetupScript.CloseScript

        Dispose()

        Return Kofax.ReleaseLib.KfxReturnValue.KFX_REL_SUCCESS
    End Function

    '*************************************************
    ' ActionEvent
    '-------------------------------------------------
    ' Purpose:  This method allows the setup script
    '           to respond to various events in the
    '           Administration module.  The script
    '           has the opportunity to make any
    '           necessary changes to the release
    '           settings in the ReleaseSetupData
    '           object or any other external data
    '           source.
    ' Inputs:   intActionID     ID of the event
    '           strData1        Action parameter 1
    '           strData2        Action parameter 2
    ' Outputs:  None
    ' Returns:  One of the following:
    '             KFX_REL_SUCCESS, KFX_REL_ERROR,
    '             KFX_REL_STOPPED, KFX_REL_UNSUPPORTED
    ' Notes:    Refer to the documentation for a list
    '           of actions and associated parameters.
    '*************************************************
    Public Function ActionEvent(ByRef intActionID As Kofax.ReleaseLib.KfxActionValue, ByRef strData1 As String, ByRef strData2 As String) _
        As Kofax.ReleaseLib.KfxReturnValue _
        Implements IKfxReleaseSetupScript.ActionEvent

        Dim strMsgHeader As String
        Dim nMaxLen As Integer
        Dim nBatchLen As Integer
        Dim nDocLen As Integer
        Dim nNameLen As Integer
        Static bShowUI As Boolean
        Static bRequired As Boolean

        ActionEvent = Kofax.ReleaseLib.KfxReturnValue.KFX_REL_SUCCESS

        If 0 = SetupData.New Then
            Select Case intActionID

                Case Kofax.ReleaseLib.KfxActionValue.KFX_REL_START
                    ' A new series of Action Events is
                    ' starting so initialize any flags
                    bShowUI = False
                    bRequired = False
                    bDirtyForm = False

                Case Kofax.ReleaseLib.KfxActionValue.KFX_REL_INDEXFIELD_DELETE
                    ' Delete any links to this Index Field. If a link to a
                    ' required database field is deleted, set the flags to
                    ' display the UI so the user can create a new link to it.
                    If RemoveTheLink(strData1, SetupData, Kofax.ReleaseLib.KfxLinkSourceType.KFX_REL_INDEXFIELD) = False Then
                        bShowUI = True
                        bRequired = True
                    End If

                Case Kofax.ReleaseLib.KfxActionValue.KFX_REL_INDEXFIELD_INSERT
                    ' Set the flag to display the UI so the user
                    ' can create a link to the new Index Field
                    bShowUI = True

                Case Kofax.ReleaseLib.KfxActionValue.KFX_REL_INDEXFIELD_RENAME
                    ' If this Index Field is used in a link,
                    ' change the link to reflect the new name.
                    Dim links As Kofax.ReleaseLib.Links = m_SetupData.Links
                    Using enumerator As New ComEnumerator(links.GetEnumerator)
                        While enumerator.MoveNext()
                            Dim oLink As Kofax.ReleaseLib.Link = CType(enumerator.Current, Kofax.ReleaseLib.Link)
                            Using New ComDisposer(oLink)
                                If oLink.Source = strData1 And oLink.SourceType = Kofax.ReleaseLib.KfxLinkSourceType.KFX_REL_INDEXFIELD Then
                                    oLink.Source = strData2
                                End If
                            End Using
                        End While
                    End Using

                Case Kofax.ReleaseLib.KfxActionValue.KFX_REL_BATCHFIELD_DELETE
                    ' Delete any links to this Batch Field. If a link to a
                    ' required database field is deleted, set the flags to
                    ' display the UI so the user can create a new link to it.
                    If RemoveTheLink(strData1, SetupData, Kofax.ReleaseLib.KfxLinkSourceType.KFX_REL_BATCHFIELD) = False Then
                        bShowUI = True
                        bRequired = True
                    End If

                Case Kofax.ReleaseLib.KfxActionValue.KFX_REL_BATCHFIELD_INSERT
                    ' Set the flag to display the UI so the user
                    ' can create a link to the new Batch Field
                    bShowUI = True

                Case Kofax.ReleaseLib.KfxActionValue.KFX_REL_BATCHFIELD_RENAME
                    ' If this Batch Field is used in a link,
                    ' change the link to reflect the new name.
                    Dim links As Kofax.ReleaseLib.Links = m_SetupData.Links
                    Using enumerator As New ComEnumerator(links.GetEnumerator)
                        While enumerator.MoveNext()
                            Dim oLink As Kofax.ReleaseLib.Link = CType(enumerator.Current, Kofax.ReleaseLib.Link)
                            Using New ComDisposer(oLink)
                                If oLink.Source = strData1 And oLink.SourceType = Kofax.ReleaseLib.KfxLinkSourceType.KFX_REL_BATCHFIELD Then
                                    oLink.Source = strData2
                                End If
                            End Using
                        End While
                    End Using

                Case Kofax.ReleaseLib.KfxActionValue.KFX_REL_RELEASESETUP_DELETE
                    ' Nothing to do

                Case Kofax.ReleaseLib.KfxActionValue.KFX_REL_IMPORT
                    ActionEvent = RunUI()

                Case Kofax.ReleaseLib.KfxActionValue.KFX_REL_BATCHCLASS_RENAME
                    ' Nothing to do

                Case Kofax.ReleaseLib.KfxActionValue.KFX_REL_DOCCLASS_RENAME
                    ' Nothing to do

                Case Kofax.ReleaseLib.KfxActionValue.KFX_REL_UPGRADE
                    ' The default export connectors do not
                    ' support the UPGRADE action at this time
                    ActionEvent = Kofax.ReleaseLib.KfxReturnValue.KFX_REL_UNSUPPORTED

                Case Kofax.ReleaseLib.KfxActionValue.KFX_REL_END
                    ' If a link to a required database field
                    ' was deleted, we set a flag that causes
                    ' the UI to require validation
                    If bRequired Then
                        bDirtyForm = True
                    End If

                    ' Check if the flag was set to display the
                    ' UI by any Action Events in the series.
                    If bShowUI = True Then
                        ActionEvent = RunUI()
                    End If

                    ' Clear the flags
                    bShowUI = False
                    bRequired = False
                    bDirtyForm = False

                Case Kofax.ReleaseLib.KfxActionValue.KFX_REL_PUBLISH_CHECK

                    ' Check if a link is established to
                    ' all required columns in the database
                    If ValidateDatabase() = Kofax.ReleaseLib.KfxReturnValue.KFX_REL_ERROR Then
                        ActionEvent = Kofax.ReleaseLib.KfxReturnValue.KFX_REL_ERROR
                        Exit Function
                    End If

                    ' Calculate string length for padding purpose
                    nBatchLen = Len(My.Resources.strBatchClass)
                    nDocLen = Len(My.Resources.strDocClass)
                    nNameLen = Len(My.Resources.strName)
                    nMaxLen = nBatchLen
                    nMaxLen = Math.Max(nDocLen, nMaxLen)
                    nMaxLen = Math.Max(nNameLen, nMaxLen)

                    ' Pad each string to the max length with spaces and append a tab character since padding
                    ' with spaces don't quite line up.  This ensures that all strings have equal length, and
                    ' works with localized strings.
                    strMsgHeader = My.Resources.strBatchClass & Space(nMaxLen - nBatchLen) & vbTab & SetupData.BatchClassName & vbCrLf & My.Resources.strDocClass & Space(nMaxLen - nDocLen) & vbTab & SetupData.DocClassName & vbCrLf & My.Resources.strName.Replace("&", "") & Space(nMaxLen - nNameLen) & vbTab & SetupData.Name & vbCrLf & vbCrLf

                    Select Case SetupData.ImageType
                        Case CAP_IMAGE_FORMAT.CAP_FORMAT_PDF
                            ' If PDF 2.01 is detected, return error.
                            MsgBox(strMsgHeader & My.Resources.strPdfPublishFailed, MsgBoxStyle.OkOnly Or MsgBoxStyle.Exclamation, My.Resources.strDataVerifyFail)
                            ActionEvent = Kofax.ReleaseLib.KfxReturnValue.KFX_REL_ERROR
                            Exit Function
                    End Select

                Case Else
                    ActionEvent = Kofax.ReleaseLib.KfxReturnValue.KFX_REL_UNSUPPORTED

            End Select

            ' Save our changes
            SetupData.Apply()
        End If
    End Function

    '*************************************************
    ' OpenScript
    '-------------------------------------------------
    ' Purpose:  Script initialization point.  Perform
    '           any necessary initialization such as
    '           logging in to a remote data source,
    '           allocating resources, etc.
    ' Inputs:   None
    ' Outputs:  None
    ' Returns:  One of the following:
    '             KFX_REL_SUCCESS, KFX_REL_ERROR,
    '             KFX_REL_FATALERROR, KFX_REL_REINIT
    '             KFX_REL_DOCCLASSERROR,
    ' Notes:    Called by the Release Controller
    '           once when the script object is loaded
    '           and before a call to RunUI() or
    '           ActionEvent() is made.
    '*************************************************
    Public Function OpenScript() As Kofax.ReleaseLib.KfxReturnValue Implements IKfxReleaseSetupScript.OpenScript
        bDirtyForm = False
        Return Kofax.ReleaseLib.KfxReturnValue.KFX_REL_SUCCESS
    End Function

    '*************************************************
    ' RunUI
    '-------------------------------------------------
    ' Purpose:  User interface display point.  This
    '           method is called by the Release Setup
    '           Controller to display the setup form
    '           specific to this script.
    ' Inputs:   None
    ' Outputs:  None
    ' Returns:  One of the following:
    '             KFX_REL_SUCCESS, KFX_REL_ERROR,
    '             or KFX_REL_STOPPED
    ' Notes:    Called by Release Setup Controller
    '           when the Administration module asks
    '           to run the script and whenever a
    '           Batch Field or Index Field is inserted.
    '*************************************************
    Public Function RunUI() As Kofax.ReleaseLib.KfxReturnValue Implements IKfxReleaseSetupScript.RunUI
        Using oForm As New frmSetup
            oForm.ShowForm(SetupData, bDirtyForm)
            Return Kofax.ReleaseLib.KfxReturnValue.KFX_REL_SUCCESS
        End Using
    End Function

    '*************************************************
    ' RemoveTheLink
    '-------------------------------------------------
    ' Purpose:  Deletes links from the export connector
    '           when the associated Index Field or
    '           Batch Field is removed from the
    '           Document Class or Batch Class
    '           respectively in the Administration
    '           module.
    ' Returns:  False if any link to a required field
    '           in the database is deleted; else True
    ' Inputs:   strData1    the link source name
    '           SetupData   the SetupData object
    '           nLinkType   the link source type
    ' Outputs:  None
    ' Notes:    Called by ActionEvent() when the
    '           action specified is either
    '           KFX_REL_INDEXFIELD_DELETE or
    '           KFX_REL_BATCHFIELD_DELETE.
    '*************************************************
    Private Function RemoveTheLink(ByRef strData1 As String, ByRef SetupData As Kofax.ReleaseLib.ReleaseSetupData, ByRef nLinkType As Kofax.ReleaseLib.KfxLinkSourceType) As Boolean
        ' Remove all links to the deleted field
        Dim links As Kofax.ReleaseLib.Links = SetupData.Links
        Using enumerator As New ComEnumerator(links.GetEnumerator)
            While enumerator.MoveNext()
                Dim oLink As Kofax.ReleaseLib.Link = CType(enumerator.Current, Kofax.ReleaseLib.Link)
                Using New ComDisposer(oLink)
                    If oLink.Source = strData1 And oLink.SourceType = nLinkType Then
                        ' Return FALSE if ANY of the deleted links were required.
                        ' DO NOT assign the return value of RemoveLinkOK directly
                        ' because only the last link would be evaluated.
                        If RemoveLinkOK(SetupData, oLink) = False Then
                            Return False
                        End If
                    End If
                End Using
            End While
        End Using

        Return True
    End Function

    '*************************************************
    ' RemoveLinkOK
    '-------------------------------------------------
    ' Purpose:  Does the actual removal of the link
    '           when the associated Index Field or
    '           Batch Field is removed from the
    '           Document Class or Batch Class
    '           respectively in the Administration
    '           module.
    ' Inputs:   SetupData   the SetupData object
    '           oLink       the link object to remove
    ' Outputs:  None
    ' Returns:  True if the database field is not
    '           required; False if it is required
    ' Notes:    Called by RemoveTheLink
    '*************************************************
    Private Function RemoveLinkOK(ByVal SetupData As Kofax.ReleaseLib.ReleaseSetupData, ByVal oLink As Kofax.ReleaseLib.Link) As Boolean
        Using oDB As New DatabaseFile
            If Left(oLink.Destination, 3) <> "PDF" Then
                ' Always remove the link first
                SetupData.Links.Remove(oLink.Destination)

                ' Determine if the database field is required
                oDB.SetupForValidation(SetupData)

                Using command As DbCommand = _
                    oDB.CreateCommand(oDB.GetColumnSelect(SetupData.TableName, oLink.Destination))
                    Using reader As DbDataReader = command.ExecuteReader(CommandBehavior.SchemaOnly Or CommandBehavior.KeyInfo)
                        Using schemaTable As DataTable = reader.GetSchemaTable()

                            ' If the column can be null, then no user interaction is required. Remove is OK.
                            Return CBool(schemaTable.Rows(0)("AllowDBNull"))
                        End Using
                    End Using
                End Using
            Else
                ' Can't delete PDF links, so just set it to UNDEFINED.
                oLink.SourceType = Kofax.ReleaseLib.KfxLinkSourceType.KFX_REL_UNDEFINED_LINK
                oLink.Source = String.Empty
                Return True
            End If
        End Using
    End Function

    '*************************************************
    ' ValidateDatabase
    '-------------------------------------------------
    ' Purpose:  Determines if all required columns in
    '           the database have a link established.
    ' Inputs:   None
    ' Outputs:  None
    ' Returns:  KFX_REL_SUCCESS if all required
    '           columns are linked; else KFX_REL_ERROR
    ' Notes:    None
    '*************************************************
    Private Function ValidateDatabase() As Kofax.ReleaseLib.KfxReturnValue
        Dim result As Kofax.ReleaseLib.KfxReturnValue = Kofax.ReleaseLib.KfxReturnValue.KFX_REL_ERROR

        Using oDB As New DatabaseFile
            Try
                ' Open the database
                oDB.SetupForValidation(SetupData)

                Dim links As Kofax.ReleaseLib.Links = SetupData.Links
                ' Check if each required field in the Index table
                ' has a link established to an Ascent Capture value
                If oDB.ValidateIndexTable(links) = True Then
                    result = Kofax.ReleaseLib.KfxReturnValue.KFX_REL_SUCCESS
                Else
                    If m_SetupData IsNot Nothing Then
                        m_SetupData.LogError(-1, 0, 0, My.Resources.strLinkToRequiredFieldMissing, M_RSETUP, 0)
                    End If
                End If
            Catch ex As Exception
                ' To match 8.0 behavior log exceptions and return error.
                result = Kofax.ReleaseLib.KfxReturnValue.KFX_REL_ERROR
                If m_SetupData IsNot Nothing Then
                    m_SetupData.LogError(-1, 0, 0, ex.Message, M_RSETUP, 0)
                End If
            End Try
        End Using

        Return result
    End Function

    Public Sub Dispose() Implements IDisposable.Dispose
        If m_SetupData IsNot Nothing Then
            Marshal.FinalReleaseComObject(m_SetupData.BatchFields)
            Marshal.FinalReleaseComObject(m_SetupData.CustomProperties)
            Marshal.FinalReleaseComObject(m_SetupData.IndexFields)
            Marshal.FinalReleaseComObject(m_SetupData.Links)
            Marshal.FinalReleaseComObject(m_SetupData)
            m_SetupData = Nothing
        End If
    End Sub
End Class
