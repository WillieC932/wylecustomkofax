'********************************************************************************
'***
'*** Class      DataGridViewIndexValueEditingControl
'*** Purpose    Implementation of IDataGridViewEditingControl used to edit
'***            index value cells
'***
'*** (c) Copyright 2007 Kofax Image Products.
'*** All rights reserved.
'***
'********************************************************************************
Imports System.Drawing
Imports System.Windows.Forms
Imports SourceType = Kofax.ReleaseLib.KfxLinkSourceType

Friend Class DataGridViewIndexValueEditingControl
    Inherits Panel
    Implements IDataGridViewEditingControl

    Private m_link As Link
    Private WithEvents m_button As New Button()
    Private WithEvents m_textBox As New IndexValueTextBox()
    Private m_dataGridView As IndexValueDataGridView
    Private WithEvents m_contextMenuStrip As System.Windows.Forms.ContextMenuStrip
    Private components As System.ComponentModel.IContainer
    Private WithEvents unlinkFieldMenuItem As System.Windows.Forms.ToolStripMenuItem
    Private WithEvents toolStripSeparator As System.Windows.Forms.ToolStripSeparator
    Private WithEvents documentIdMenuItem As System.Windows.Forms.ToolStripMenuItem
    Private WithEvents documentIndexFieldsMenuItem As System.Windows.Forms.ToolStripMenuItem
    Private WithEvents folderIndexFieldsMenuItem As System.Windows.Forms.ToolStripMenuItem
    Private WithEvents batchFieldsMenuItem As System.Windows.Forms.ToolStripMenuItem
    Private WithEvents kofaxCaptureValuesMenuItem As System.Windows.Forms.ToolStripMenuItem
    Private WithEvents textConstantMenuItem As System.Windows.Forms.ToolStripMenuItem
    Private m_valueChanged As Boolean
    Private m_rowIndex As Integer
	Private m_cell As DataGridViewIndexValueCell
	Private Const TEXTCONSTANT_MAX_SIZE As Integer = 254


    Public Sub New()
        InitializeComponent()

        m_button.Dock = DockStyle.Right
        m_button.Image = My.Resources.IndexValueButtonImage

        m_textBox.Dock = DockStyle.Fill
        m_textBox.ReadOnly = True
		m_textBox.MaxLength = TEXTCONSTANT_MAX_SIZE

        Controls.Add(m_textBox)
        Controls.Add(m_button)
    End Sub

	''' <summary>
	''' Allow the space bar to open the menu to edit the index value, or handle up and down arrow keys
	''' </summary>
	''' <param name="e">The event args</param>
	''' <remarks>Delegates to the PreviewKeyDown handler for the textbox</remarks>
	Protected Overrides Sub OnPreviewKeyDown(ByVal e As System.Windows.Forms.PreviewKeyDownEventArgs)
		MyBase.OnPreviewKeyDown(e)

        If e.KeyData = Keys.Space Or e.KeyData = Keys.Apps Then
            ShowContextMenu()
        End If
    End Sub

    Private Sub TextBox_KeyDown(ByVal sender As Object, ByVal e As KeyEventArgs) Handles m_textBox.KeyDown
        If Not m_textBox.ReadOnly And (e.KeyData = Keys.Enter Or e.KeyData = Keys.Return) Then
            m_dataGridView.PerformValidation()
            m_dataGridView.BeginEdit(True)
            e.Handled = True
        End If
    End Sub

    Private Sub TextBox_Leave(ByVal sender As Object, ByVal e As EventArgs) Handles m_textBox.Leave
        If Not m_textBox.ReadOnly Then
            m_textBox.ReadOnly = True
            m_link.Source = m_textBox.Text
            If String.IsNullOrEmpty(m_link.Source) Then
                m_link.SourceType = SourceType.KFX_REL_UNDEFINED_LINK
            End If
            m_textBox.BackColor = m_dataGridView.DefaultCellStyle.SelectionBackColor
            m_textBox.ForeColor = m_dataGridView.DefaultCellStyle.SelectionForeColor
            EditingControlValueChanged = True
        End If
    End Sub

    Private Sub IndexValueClicked(ByVal sender As Object, ByVal e As EventArgs) Handles m_button.Click
        ShowContextMenu()
    End Sub

    Private Sub ShowContextMenu()
        Dim bShowUnlinkMenuItem As Boolean = m_link.SourceType <> SourceType.KFX_REL_UNDEFINED_LINK
        m_contextMenuStrip.Items(unlinkFieldMenuItem.Name).Visible = bShowUnlinkMenuItem
        m_contextMenuStrip.Items(toolStripSeparator.Name).Visible = bShowUnlinkMenuItem

        ' Build Kofax Capture Values menu
        Dim releaseSetupData As New ReleaseSetupData(m_dataGridView.SetupData)
        If kofaxCaptureValuesMenuItem.DropDownItems.Count = 0 Then
            ' Add the Ascent Capture Values to the menu
            For Each strBatchVar As String In releaseSetupData.BatchVariableNames
                kofaxCaptureValuesMenuItem.DropDownItems.Add( _
                 strBatchVar, Nothing, AddressOf KofaxCaptureValuesMenuItem_Click)
            Next
        End If
        kofaxCaptureValuesMenuItem.Visible = releaseSetupData.BatchVariableNames.Count > 0

        ' Build Batch Fields menu
        If batchFieldsMenuItem.DropDownItems.Count = 0 Then
            ' Add the batch fields to the menu
            ' Populate each element with the data
            Using enumerator As New ComEnumerator(m_dataGridView.SetupData.BatchFields.GetEnumerator())
                While enumerator.MoveNext()
                    Dim currField As Kofax.ReleaseLib.BatchField = CType(enumerator.Current, Kofax.ReleaseLib.BatchField)
                    batchFieldsMenuItem.DropDownItems.Add( _
                     currField.Name, Nothing, AddressOf BatchFieldsMenuItem_Click)
                End While
            End Using
        End If

        ' If there are no batch fields remove the menu option
        batchFieldsMenuItem.Visible = batchFieldsMenuItem.DropDownItems.Count > 0

        ' Build Document Index Fields menu
        If documentIndexFieldsMenuItem.DropDownItems.Count = 0 Then
            Using enumerator As New ComEnumerator(m_dataGridView.SetupData.IndexFields.GetEnumerator())
                While enumerator.MoveNext()
                    Dim currField As Kofax.ReleaseLib.IndexField = CType(enumerator.Current, Kofax.ReleaseLib.IndexField)
                    ' Only add document index fields and skip folder index fields
                    If Len(currField.FolderClassName) = 0 Then
                        documentIndexFieldsMenuItem.DropDownItems.Add( _
                         currField.Name, Nothing, AddressOf DocumentIndexFieldsMenuItem_Click)
                    End If
                End While
            End Using
        End If

        ' If there are no index fields hide the submenu
        documentIndexFieldsMenuItem.Visible = documentIndexFieldsMenuItem.DropDownItems.Count > 0

        ' Build Folder Index Fields menu
        Dim oFolderClassSubMenu As ToolStripMenuItem = Nothing
        Dim strFolderClassName As String = String.Empty

        If folderIndexFieldsMenuItem.DropDownItems.Count = 0 Then
            Using enumerator As New ComEnumerator(m_dataGridView.SetupData.IndexFields.GetEnumerator())
                While enumerator.MoveNext()
                    Dim oIndexField As Kofax.ReleaseLib.IndexField = CType(enumerator.Current, Kofax.ReleaseLib.IndexField)
                    If Len(oIndexField.FolderClassName) > 0 Then
                        ' Create a new folder class sub menu if the folder class name is changed
                        If strFolderClassName <> oIndexField.FolderClassName Then
                            ' Create a new folder class sub menu
                            oFolderClassSubMenu = New ToolStripMenuItem(oIndexField.FolderClassName)
                            folderIndexFieldsMenuItem.DropDownItems.Add(oFolderClassSubMenu)

                            ' Remember the last folder class name
                            strFolderClassName = oIndexField.FolderClassName
                        End If

                        ' Append a new folder index field menu item
                        oFolderClassSubMenu.DropDownItems.Add( _
                         oIndexField.FolderIndexFieldName, _
                         Nothing, _
                         AddressOf FolderIndexFieldsMenuItem_Click).Tag = oIndexField.Name
                    End If
                End While
            End Using
        End If
        folderIndexFieldsMenuItem.Visible = folderIndexFieldsMenuItem.DropDownItems.Count > 0

        m_contextMenuStrip.Show(Me, 0, Height)
    End Sub

    Public Sub ApplyCellStyleToEditingControl(ByVal dataGridViewCellStyle As System.Windows.Forms.DataGridViewCellStyle) Implements System.Windows.Forms.IDataGridViewEditingControl.ApplyCellStyleToEditingControl
        m_textBox.Font = dataGridViewCellStyle.Font
    End Sub

    Public Property EditingControlDataGridView() As System.Windows.Forms.DataGridView Implements System.Windows.Forms.IDataGridViewEditingControl.EditingControlDataGridView
        Get
            Return m_dataGridView
        End Get
        Set(ByVal value As System.Windows.Forms.DataGridView)
            m_dataGridView = CType(value, IndexValueDataGridView)
            m_button.Size = New Size(m_dataGridView(0, 0).Size.Height, m_dataGridView(0, 0).Size.Height)
            m_textBox.BackColor = m_dataGridView.DefaultCellStyle.SelectionBackColor
            m_textBox.ForeColor = m_dataGridView.DefaultCellStyle.SelectionForeColor
        End Set
    End Property

    Public Property EditingControlFormattedValue() As Object Implements System.Windows.Forms.IDataGridViewEditingControl.EditingControlFormattedValue
        Get
            Return m_textBox.Text
        End Get
        Set(ByVal value As Object)
            m_textBox.Text = CType(value, String)
        End Set
    End Property

    Public Property EditingControlRowIndex() As Integer Implements System.Windows.Forms.IDataGridViewEditingControl.EditingControlRowIndex
        Get
            Return m_rowIndex
        End Get
        Set(ByVal value As Integer)
            m_rowIndex = value
            m_link = CType(m_dataGridView.Rows(m_rowIndex).DataBoundItem, Link)
            m_cell = CType(m_dataGridView.Item(1, m_rowIndex), DataGridViewIndexValueCell)
            m_textBox.Text = CStr(m_cell.FormattedValue)
        End Set
    End Property

    Public Property EditingControlValueChanged() As Boolean Implements System.Windows.Forms.IDataGridViewEditingControl.EditingControlValueChanged
        Get
            Return m_valueChanged
        End Get
        Set(ByVal value As Boolean)
            m_valueChanged = value
            m_dataGridView.NotifyCurrentCellDirty(m_valueChanged)
        End Set
    End Property

    Public Function EditingControlWantsInputKey(ByVal keyData As System.Windows.Forms.Keys, ByVal dataGridViewWantsInputKey As Boolean) As Boolean Implements System.Windows.Forms.IDataGridViewEditingControl.EditingControlWantsInputKey
        If m_textBox.ReadOnly Then
            ' If not in edit mode, treat left and right
            ' arrow keys like up and down arrow keys
            Select Case keyData
                Case Keys.Left
                    SendKeys.Send("{UP}")
                    Return True

                Case Keys.Right
                    SendKeys.Send("{DOWN}")
                    Return True
            End Select
        Else
            ' If in edit mode, allow up and down
            ' keys to leave the row,
            ' otherwise let the TextBox control
            ' handle them
            Select Case keyData
                Case Keys.Up, Keys.Down
                    Return False

                Case Else
                    Return True
            End Select
        End If
        Return False
    End Function

    Public ReadOnly Property EditingPanelCursor() As System.Windows.Forms.Cursor Implements System.Windows.Forms.IDataGridViewEditingControl.EditingPanelCursor
        Get
            Return Cursor
        End Get
    End Property

    Public Function GetEditingControlFormattedValue(ByVal context As System.Windows.Forms.DataGridViewDataErrorContexts) As Object Implements System.Windows.Forms.IDataGridViewEditingControl.GetEditingControlFormattedValue
        Return EditingControlFormattedValue
    End Function

    Public Sub PrepareEditingControlForEdit(ByVal selectAll As Boolean) Implements System.Windows.Forms.IDataGridViewEditingControl.PrepareEditingControlForEdit
    End Sub

    Public ReadOnly Property RepositionEditingControlOnValueChange() As Boolean Implements System.Windows.Forms.IDataGridViewEditingControl.RepositionEditingControlOnValueChange
        Get
            Return False
        End Get
    End Property

    Private Sub InitializeComponent()
        Me.components = New System.ComponentModel.Container
        Me.m_contextMenuStrip = New System.Windows.Forms.ContextMenuStrip(Me.components)
        Me.unlinkFieldMenuItem = New System.Windows.Forms.ToolStripMenuItem
        Me.toolStripSeparator = New System.Windows.Forms.ToolStripSeparator
        Me.documentIdMenuItem = New System.Windows.Forms.ToolStripMenuItem
        Me.documentIndexFieldsMenuItem = New System.Windows.Forms.ToolStripMenuItem
        Me.folderIndexFieldsMenuItem = New System.Windows.Forms.ToolStripMenuItem
        Me.batchFieldsMenuItem = New System.Windows.Forms.ToolStripMenuItem
        Me.kofaxCaptureValuesMenuItem = New System.Windows.Forms.ToolStripMenuItem
        Me.textConstantMenuItem = New System.Windows.Forms.ToolStripMenuItem
        Me.m_contextMenuStrip.SuspendLayout()
        Me.SuspendLayout()
        '
        'm_contextMenuStrip
        '
        Me.m_contextMenuStrip.Items.AddRange(New System.Windows.Forms.ToolStripItem() {Me.unlinkFieldMenuItem, Me.toolStripSeparator, Me.documentIdMenuItem, Me.documentIndexFieldsMenuItem, Me.folderIndexFieldsMenuItem, Me.batchFieldsMenuItem, Me.kofaxCaptureValuesMenuItem, Me.textConstantMenuItem})
        Me.m_contextMenuStrip.Name = "m_contextMenuStrip"
        Me.m_contextMenuStrip.RenderMode = System.Windows.Forms.ToolStripRenderMode.System
        Me.m_contextMenuStrip.Size = New System.Drawing.Size(184, 164)
        '
        'unlinkFieldMenuItem
        '
        Me.unlinkFieldMenuItem.Name = "unlinkFieldMenuItem"
        Me.unlinkFieldMenuItem.Size = New System.Drawing.Size(183, 22)
        Me.unlinkFieldMenuItem.Text = My.Resources.strTextUnlinkField
        '
        'toolStripSeparator
        '
        Me.toolStripSeparator.Name = "toolStripSeparator"
        Me.toolStripSeparator.Size = New System.Drawing.Size(180, 6)
        '
        'documentIdMenuItem
        '
        Me.documentIdMenuItem.Name = "documentIdMenuItem"
        Me.documentIdMenuItem.Size = New System.Drawing.Size(183, 22)
        Me.documentIdMenuItem.Text = My.Resources.strDocumentId
        '
        'documentIndexFieldsMenuItem
        '
        Me.documentIndexFieldsMenuItem.Name = "documentIndexFieldsMenuItem"
        Me.documentIndexFieldsMenuItem.Size = New System.Drawing.Size(183, 22)
        Me.documentIndexFieldsMenuItem.Text = My.Resources.strTextDocIndexField
        '
        'folderIndexFieldsMenuItem
        '
        Me.folderIndexFieldsMenuItem.Name = "folderIndexFieldsMenuItem"
        Me.folderIndexFieldsMenuItem.Size = New System.Drawing.Size(183, 22)
        Me.folderIndexFieldsMenuItem.Text = My.Resources.strTextFolderIndexField
        '
        'batchFieldsMenuItem
        '
        Me.batchFieldsMenuItem.Name = "batchFieldsMenuItem"
        Me.batchFieldsMenuItem.Size = New System.Drawing.Size(183, 22)
        Me.batchFieldsMenuItem.Text = My.Resources.strTextBatchFields
        '
        'kofaxCaptureValuesMenuItem
        '
        Me.kofaxCaptureValuesMenuItem.Name = "kofaxCaptureValuesMenuItem"
        Me.kofaxCaptureValuesMenuItem.Size = New System.Drawing.Size(183, 22)
        Me.kofaxCaptureValuesMenuItem.Text = My.Resources.strTextKCValues
        '
        'textConstantMenuItem
        '
        Me.textConstantMenuItem.Name = "textConstantMenuItem"
        Me.textConstantMenuItem.Size = New System.Drawing.Size(183, 22)
        Me.textConstantMenuItem.Text = My.Resources.strTextConst
        Me.m_contextMenuStrip.ResumeLayout(False)
        Me.ResumeLayout(False)

    End Sub

    Private Sub unlinkFieldMenuItem_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles unlinkFieldMenuItem.Click
        m_link.Source = String.Empty
        m_link.SourceType = SourceType.KFX_REL_UNDEFINED_LINK
        m_textBox.Text = CStr(m_cell.FormattedValue)
        EditingControlValueChanged = True
    End Sub

    Private Sub textConstantMenuItem_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles textConstantMenuItem.Click
        If SourceType.KFX_REL_TEXTCONSTANT <> m_link.SourceType Then
            m_link.Source = String.Empty
            m_link.SourceType = SourceType.KFX_REL_TEXTCONSTANT
            m_textBox.Text = m_link.Source
            EditingControlValueChanged = True
        End If
        m_textBox.Text = m_link.Source
        m_textBox.ReadOnly = False
        m_textBox.BackColor = m_cell.Style.BackColor
        m_textBox.ForeColor = m_cell.Style.ForeColor
        m_textBox.Focus()
    End Sub

    Private Sub documentIdMenuItem_Click(ByVal sender As Object, ByVal e As EventArgs) Handles documentIdMenuItem.Click
        m_link.Source = documentIdMenuItem.Text.Replace("&", "")
        m_link.SourceType = SourceType.KFX_REL_DOCUMENTID
        m_textBox.Text = CStr(m_cell.FormattedValue)
        EditingControlValueChanged = True
    End Sub

    Private Sub KofaxCaptureValuesMenuItem_Click(ByVal sender As Object, ByVal e As EventArgs)
        Dim oItem As ToolStripItem = CType(sender, ToolStripItem)
        m_link.Source = oItem.Text
        m_link.SourceType = SourceType.KFX_REL_VARIABLE
        m_textBox.Text = CStr(m_cell.FormattedValue)
        EditingControlValueChanged = True
    End Sub

    Private Sub BatchFieldsMenuItem_Click(ByVal sender As Object, ByVal eventArgs As EventArgs)
        Dim oItem As ToolStripItem = CType(sender, ToolStripItem)
        m_link.Source = oItem.Text
        m_link.SourceType = SourceType.KFX_REL_BATCHFIELD
        m_textBox.Text = CStr(m_cell.FormattedValue)
        EditingControlValueChanged = True
    End Sub

    Private Sub DocumentIndexFieldsMenuItem_Click(ByVal sender As Object, ByVal eventArgs As EventArgs)
        Dim oItem As ToolStripItem = CType(sender, ToolStripItem)
        m_link.Source = oItem.Text
        m_link.SourceType = SourceType.KFX_REL_INDEXFIELD
        m_textBox.Text = CStr(m_cell.FormattedValue)
        EditingControlValueChanged = True
    End Sub

    Private Sub FolderIndexFieldsMenuItem_Click(ByVal sender As Object, ByVal eventArgs As EventArgs)
        Dim oItem As ToolStripItem = CType(sender, ToolStripItem)
        m_link.Source = CStr(oItem.Tag)
        m_link.SourceType = SourceType.KFX_REL_INDEXFIELD
        m_textBox.Text = CStr(m_cell.FormattedValue)
        EditingControlValueChanged = True
    End Sub
End Class
