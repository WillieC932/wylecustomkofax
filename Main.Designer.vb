<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> Partial Class frmSetup
#Region "Windows Form Designer generated code "
    'Required by the Windows Form Designer
	Private components As System.ComponentModel.IContainer
    Public WithEvents txtName As System.Windows.Forms.TextBox
	Public dlgHelpOpen As System.Windows.Forms.OpenFileDialog
	Public dlgHelpSave As System.Windows.Forms.SaveFileDialog
	Public dlgHelpFont As System.Windows.Forms.FontDialog
	Public dlgHelpColor As System.Windows.Forms.ColorDialog
	Public dlgHelpPrint As System.Windows.Forms.PrintDialog
	Public WithEvents cmdHelp As System.Windows.Forms.Button
	Public WithEvents cmdApply As System.Windows.Forms.Button
	Public WithEvents cmdCancel As System.Windows.Forms.Button
	Public WithEvents cmdOK As System.Windows.Forms.Button
	Public dlgDialogsOpen As System.Windows.Forms.OpenFileDialog
	Public WithEvents cmdSystemMDBBrowse As System.Windows.Forms.Button
	Public WithEvents txtSystemMDB As System.Windows.Forms.TextBox
	Public WithEvents txtConnString As System.Windows.Forms.TextBox
	Public WithEvents cmdConnBrowse As System.Windows.Forms.Button
	Public WithEvents txtUserName As System.Windows.Forms.TextBox
	Public WithEvents txtPassword As System.Windows.Forms.TextBox
	Public WithEvents lblSystemMDB As System.Windows.Forms.Label
    Public WithEvents cboDBTypes As System.Windows.Forms.ComboBox
    Public WithEvents databaseTabPage As System.Windows.Forms.TabPage
    Public WithEvents cboDocID As System.Windows.Forms.ComboBox
    Public WithEvents cboDTables As System.Windows.Forms.ComboBox
    Public WithEvents cboDocPath As System.Windows.Forms.ComboBox
    Public WithEvents linDocTable As System.Windows.Forms.Label
    Public WithEvents cboITables As System.Windows.Forms.ComboBox
    Public WithEvents chkReleaseKofaxPDF As System.Windows.Forms.CheckBox
    Public WithEvents txtKofaxPDFDir As System.Windows.Forms.TextBox
    Public WithEvents cmdPDFBrowse As System.Windows.Forms.Button
    Public WithEvents chkReleaseImageFiles As System.Windows.Forms.CheckBox
    Public WithEvents chkSkipFirstPage As System.Windows.Forms.CheckBox
    Public WithEvents cmdIFBrowse As System.Windows.Forms.Button
    Public WithEvents txtImageDir As System.Windows.Forms.TextBox
    Public WithEvents chkReleaseOCRFullText As System.Windows.Forms.CheckBox
    Public WithEvents cmdOCRBrowse As System.Windows.Forms.Button
    Public WithEvents txtOCRDir As System.Windows.Forms.TextBox
    Public WithEvents documentStorageTabPage As System.Windows.Forms.TabPage
    Public WithEvents cboImageType As System.Windows.Forms.ComboBox
    Public WithEvents imageFormatTabPage As System.Windows.Forms.TabPage
    Public WithEvents tabDatabase As System.Windows.Forms.TabControl
    Public WithEvents lblDocClassName As System.Windows.Forms.Label
    Public WithEvents lblBatchClassName As System.Windows.Forms.Label
    Public WithEvents fraTab As Microsoft.VisualBasic.Compatibility.VB6.PanelArray
    'NOTE: The following procedure is required by the Windows Form Designer
    'It can be modified using the Windows Form Designer.
    'Do not modify it using the code editor.
    <System.Diagnostics.DebuggerStepThrough()> Private Sub InitializeComponent()
        Me.components = New System.ComponentModel.Container
        Dim _fraTab_0 As System.Windows.Forms.Panel
        Dim fraDBControls As System.Windows.Forms.Panel
        Dim resources As System.ComponentModel.ComponentResourceManager = New System.ComponentModel.ComponentResourceManager(GetType(frmSetup))
        Dim lblUserName As System.Windows.Forms.Label
        Dim lblPassword As System.Windows.Forms.Label
        Dim linDBType As System.Windows.Forms.Label
        Dim lblDBType As System.Windows.Forms.Label
        Dim _fraTab_1 As System.Windows.Forms.Panel
        Dim fraDocVals As System.Windows.Forms.GroupBox
        Dim lblDocTable As System.Windows.Forms.Label
        Dim fraIndexVals As System.Windows.Forms.GroupBox
        Dim fraLinkBox As System.Windows.Forms.GroupBox
        Dim DataGridViewCellStyle1 As System.Windows.Forms.DataGridViewCellStyle = New System.Windows.Forms.DataGridViewCellStyle
        Dim DataGridViewCellStyle2 As System.Windows.Forms.DataGridViewCellStyle = New System.Windows.Forms.DataGridViewCellStyle
        Dim DataGridViewCellStyle3 As System.Windows.Forms.DataGridViewCellStyle = New System.Windows.Forms.DataGridViewCellStyle
        Dim lblIndexTable As System.Windows.Forms.Label
        Dim _fraTab_2 As System.Windows.Forms.Panel
        Dim fraPDFFiles As System.Windows.Forms.GroupBox
        Dim fraImageFiles As System.Windows.Forms.GroupBox
        Dim fraOCRFiles As System.Windows.Forms.GroupBox
        Dim _fraTab_3 As System.Windows.Forms.Panel
        Dim lblName As System.Windows.Forms.Label
        Dim lblDocClass As System.Windows.Forms.Label
        Dim lblBatchClass As System.Windows.Forms.Label
        Me.cmdSystemMDBBrowse = New System.Windows.Forms.Button
        Me.txtSystemMDB = New System.Windows.Forms.TextBox
        Me.txtConnString = New System.Windows.Forms.TextBox
        Me.cmdConnBrowse = New System.Windows.Forms.Button
        Me.txtUserName = New System.Windows.Forms.TextBox
        Me.txtPassword = New System.Windows.Forms.TextBox
        Me.lblSystemMDB = New System.Windows.Forms.Label
        Me.lblConnString = New System.Windows.Forms.Label
        Me.cboDBTypes = New System.Windows.Forms.ComboBox
        Me.cboDocID = New System.Windows.Forms.ComboBox
        Me.cboDTables = New System.Windows.Forms.ComboBox
        Me.cboDocPath = New System.Windows.Forms.ComboBox
        Me.lblDocID = New System.Windows.Forms.Label
        Me.linDocTable = New System.Windows.Forms.Label
        Me.lblDocPath = New System.Windows.Forms.Label
        Me.cboITables = New System.Windows.Forms.ComboBox
        Me.indexValuesDataGridView = New TWG.PdfByTitle.IndexValueDataGridView
        Me.chkReleaseKofaxPDF = New System.Windows.Forms.CheckBox
        Me.txtKofaxPDFDir = New System.Windows.Forms.TextBox
        Me.cmdPDFBrowse = New System.Windows.Forms.Button
        Me.lblPDFDir = New System.Windows.Forms.Label
        Me.chkReleaseImageFiles = New System.Windows.Forms.CheckBox
        Me.chkSkipFirstPage = New System.Windows.Forms.CheckBox
        Me.cmdIFBrowse = New System.Windows.Forms.Button
        Me.txtImageDir = New System.Windows.Forms.TextBox
        Me.lblImageDir = New System.Windows.Forms.Label
        Me.chkReleaseOCRFullText = New System.Windows.Forms.CheckBox
        Me.cmdOCRBrowse = New System.Windows.Forms.Button
        Me.txtOCRDir = New System.Windows.Forms.TextBox
        Me.lblOCRDir = New System.Windows.Forms.Label
        Me.fraReleaseImagesAs = New System.Windows.Forms.GroupBox
        Me.cboImageType = New System.Windows.Forms.ComboBox
        Me.lblImageType = New System.Windows.Forms.Label
        Me.txtName = New System.Windows.Forms.TextBox
        Me.dlgHelpOpen = New System.Windows.Forms.OpenFileDialog
        Me.dlgHelpSave = New System.Windows.Forms.SaveFileDialog
        Me.dlgHelpFont = New System.Windows.Forms.FontDialog
        Me.dlgHelpColor = New System.Windows.Forms.ColorDialog
        Me.dlgHelpPrint = New System.Windows.Forms.PrintDialog
        Me.cmdHelp = New System.Windows.Forms.Button
        Me.cmdApply = New System.Windows.Forms.Button
        Me.cmdCancel = New System.Windows.Forms.Button
        Me.cmdOK = New System.Windows.Forms.Button
        Me.dlgDialogsOpen = New System.Windows.Forms.OpenFileDialog
        Me.tabDatabase = New System.Windows.Forms.TabControl
        Me.databaseTabPage = New System.Windows.Forms.TabPage
        Me.tableSettingsTabPage = New System.Windows.Forms.TabPage
        Me.documentStorageTabPage = New System.Windows.Forms.TabPage
        Me.imageFormatTabPage = New System.Windows.Forms.TabPage
        Me.lblDocClassName = New System.Windows.Forms.Label
        Me.lblBatchClassName = New System.Windows.Forms.Label
        Me.fraTab = New Microsoft.VisualBasic.Compatibility.VB6.PanelArray(Me.components)
        _fraTab_0 = New System.Windows.Forms.Panel
        fraDBControls = New System.Windows.Forms.Panel
        lblUserName = New System.Windows.Forms.Label
        lblPassword = New System.Windows.Forms.Label
        linDBType = New System.Windows.Forms.Label
        lblDBType = New System.Windows.Forms.Label
        _fraTab_1 = New System.Windows.Forms.Panel
        fraDocVals = New System.Windows.Forms.GroupBox
        lblDocTable = New System.Windows.Forms.Label
        fraIndexVals = New System.Windows.Forms.GroupBox
        fraLinkBox = New System.Windows.Forms.GroupBox
        lblIndexTable = New System.Windows.Forms.Label
        _fraTab_2 = New System.Windows.Forms.Panel
        fraPDFFiles = New System.Windows.Forms.GroupBox
        fraImageFiles = New System.Windows.Forms.GroupBox
        fraOCRFiles = New System.Windows.Forms.GroupBox
        _fraTab_3 = New System.Windows.Forms.Panel
        lblName = New System.Windows.Forms.Label
        lblDocClass = New System.Windows.Forms.Label
        lblBatchClass = New System.Windows.Forms.Label
        _fraTab_0.SuspendLayout()
        fraDBControls.SuspendLayout()
        _fraTab_1.SuspendLayout()
        fraDocVals.SuspendLayout()
        fraIndexVals.SuspendLayout()
        fraLinkBox.SuspendLayout()
        CType(Me.indexValuesDataGridView, System.ComponentModel.ISupportInitialize).BeginInit()
        _fraTab_2.SuspendLayout()
        fraPDFFiles.SuspendLayout()
        fraImageFiles.SuspendLayout()
        fraOCRFiles.SuspendLayout()
        _fraTab_3.SuspendLayout()
        Me.fraReleaseImagesAs.SuspendLayout()
        Me.tabDatabase.SuspendLayout()
        Me.databaseTabPage.SuspendLayout()
        Me.tableSettingsTabPage.SuspendLayout()
        Me.documentStorageTabPage.SuspendLayout()
        Me.imageFormatTabPage.SuspendLayout()
        CType(Me.fraTab, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.SuspendLayout()
        '
        '_fraTab_0
        '
        _fraTab_0.Controls.Add(fraDBControls)
        _fraTab_0.Controls.Add(Me.cboDBTypes)
        _fraTab_0.Controls.Add(linDBType)
        _fraTab_0.Controls.Add(lblDBType)
        _fraTab_0.Cursor = System.Windows.Forms.Cursors.Default
        _fraTab_0.ForeColor = System.Drawing.SystemColors.ControlText
        Me.fraTab.SetIndex(_fraTab_0, CType(0, Short))
        resources.ApplyResources(_fraTab_0, "_fraTab_0")
        _fraTab_0.Name = "_fraTab_0"
        '
        'fraDBControls
        '
        fraDBControls.Controls.Add(Me.cmdSystemMDBBrowse)
        fraDBControls.Controls.Add(Me.txtSystemMDB)
        fraDBControls.Controls.Add(Me.txtConnString)
        fraDBControls.Controls.Add(Me.cmdConnBrowse)
        fraDBControls.Controls.Add(Me.txtUserName)
        fraDBControls.Controls.Add(Me.txtPassword)
        fraDBControls.Controls.Add(Me.lblSystemMDB)
        fraDBControls.Controls.Add(Me.lblConnString)
        fraDBControls.Controls.Add(lblUserName)
        fraDBControls.Controls.Add(lblPassword)
        fraDBControls.Cursor = System.Windows.Forms.Cursors.Default
        fraDBControls.ForeColor = System.Drawing.SystemColors.ControlText
        resources.ApplyResources(fraDBControls, "fraDBControls")
        fraDBControls.Name = "fraDBControls"
        '
        'cmdSystemMDBBrowse
        '
        Me.cmdSystemMDBBrowse.Cursor = System.Windows.Forms.Cursors.Default
        resources.ApplyResources(Me.cmdSystemMDBBrowse, "cmdSystemMDBBrowse")
        Me.cmdSystemMDBBrowse.Name = "cmdSystemMDBBrowse"
        Me.cmdSystemMDBBrowse.UseVisualStyleBackColor = True
        '
        'txtSystemMDB
        '
        Me.txtSystemMDB.AcceptsReturn = True
        Me.txtSystemMDB.Cursor = System.Windows.Forms.Cursors.IBeam
        resources.ApplyResources(Me.txtSystemMDB, "txtSystemMDB")
        Me.txtSystemMDB.Name = "txtSystemMDB"
        '
        'txtConnString
        '
        Me.txtConnString.AcceptsReturn = True
        Me.txtConnString.Cursor = System.Windows.Forms.Cursors.IBeam
        resources.ApplyResources(Me.txtConnString, "txtConnString")
        Me.txtConnString.Name = "txtConnString"
        '
        'cmdConnBrowse
        '
        Me.cmdConnBrowse.Cursor = System.Windows.Forms.Cursors.Default
        resources.ApplyResources(Me.cmdConnBrowse, "cmdConnBrowse")
        Me.cmdConnBrowse.Name = "cmdConnBrowse"
        Me.cmdConnBrowse.UseVisualStyleBackColor = True
        '
        'txtUserName
        '
        Me.txtUserName.AcceptsReturn = True
        Me.txtUserName.Cursor = System.Windows.Forms.Cursors.IBeam
        resources.ApplyResources(Me.txtUserName, "txtUserName")
        Me.txtUserName.Name = "txtUserName"
        '
        'txtPassword
        '
        Me.txtPassword.AcceptsReturn = True
        Me.txtPassword.Cursor = System.Windows.Forms.Cursors.IBeam
        resources.ApplyResources(Me.txtPassword, "txtPassword")
        Me.txtPassword.Name = "txtPassword"
        '
        'lblSystemMDB
        '
        Me.lblSystemMDB.Cursor = System.Windows.Forms.Cursors.Default
        resources.ApplyResources(Me.lblSystemMDB, "lblSystemMDB")
        Me.lblSystemMDB.Name = "lblSystemMDB"
        '
        'lblConnString
        '
        Me.lblConnString.Cursor = System.Windows.Forms.Cursors.Default
        resources.ApplyResources(Me.lblConnString, "lblConnString")
        Me.lblConnString.Name = "lblConnString"
        '
        'lblUserName
        '
        resources.ApplyResources(lblUserName, "lblUserName")
        lblUserName.Cursor = System.Windows.Forms.Cursors.Default
        lblUserName.Name = "lblUserName"
        '
        'lblPassword
        '
        resources.ApplyResources(lblPassword, "lblPassword")
        lblPassword.Cursor = System.Windows.Forms.Cursors.Default
        lblPassword.Name = "lblPassword"
        '
        'cboDBTypes
        '
        Me.cboDBTypes.BackColor = System.Drawing.SystemColors.Window
        Me.cboDBTypes.Cursor = System.Windows.Forms.Cursors.Default
        Me.cboDBTypes.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList
        Me.cboDBTypes.ForeColor = System.Drawing.SystemColors.WindowText
        Me.cboDBTypes.Items.AddRange(New Object() {resources.GetString("cboDBTypes.Items"), resources.GetString("cboDBTypes.Items1")})
        resources.ApplyResources(Me.cboDBTypes, "cboDBTypes")
        Me.cboDBTypes.Name = "cboDBTypes"
        '
        'linDBType
        '
        linDBType.BackColor = System.Drawing.SystemColors.InactiveCaption
        resources.ApplyResources(linDBType, "linDBType")
        linDBType.Name = "linDBType"
        '
        'lblDBType
        '
        lblDBType.Cursor = System.Windows.Forms.Cursors.Default
        resources.ApplyResources(lblDBType, "lblDBType")
        lblDBType.Name = "lblDBType"
        '
        '_fraTab_1
        '
        _fraTab_1.Controls.Add(fraDocVals)
        _fraTab_1.Controls.Add(fraIndexVals)
        _fraTab_1.Cursor = System.Windows.Forms.Cursors.Default
        Me.fraTab.SetIndex(_fraTab_1, CType(1, Short))
        resources.ApplyResources(_fraTab_1, "_fraTab_1")
        _fraTab_1.Name = "_fraTab_1"
        '
        'fraDocVals
        '
        fraDocVals.Controls.Add(Me.cboDocID)
        fraDocVals.Controls.Add(Me.cboDTables)
        fraDocVals.Controls.Add(Me.cboDocPath)
        fraDocVals.Controls.Add(Me.lblDocID)
        fraDocVals.Controls.Add(Me.linDocTable)
        fraDocVals.Controls.Add(lblDocTable)
        fraDocVals.Controls.Add(Me.lblDocPath)
        resources.ApplyResources(fraDocVals, "fraDocVals")
        fraDocVals.Name = "fraDocVals"
        fraDocVals.TabStop = False
        '
        'cboDocID
        '
        Me.cboDocID.BackColor = System.Drawing.SystemColors.Window
        Me.cboDocID.Cursor = System.Windows.Forms.Cursors.Default
        Me.cboDocID.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList
        resources.ApplyResources(Me.cboDocID, "cboDocID")
        Me.cboDocID.ForeColor = System.Drawing.SystemColors.WindowText
        Me.cboDocID.Name = "cboDocID"
        '
        'cboDTables
        '
        Me.cboDTables.BackColor = System.Drawing.SystemColors.Window
        Me.cboDTables.Cursor = System.Windows.Forms.Cursors.Default
        Me.cboDTables.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList
        Me.cboDTables.ForeColor = System.Drawing.SystemColors.WindowText
        resources.ApplyResources(Me.cboDTables, "cboDTables")
        Me.cboDTables.Name = "cboDTables"
        Me.cboDTables.Sorted = True
        '
        'cboDocPath
        '
        Me.cboDocPath.BackColor = System.Drawing.SystemColors.Window
        Me.cboDocPath.Cursor = System.Windows.Forms.Cursors.Default
        Me.cboDocPath.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList
        resources.ApplyResources(Me.cboDocPath, "cboDocPath")
        Me.cboDocPath.ForeColor = System.Drawing.SystemColors.WindowText
        Me.cboDocPath.Name = "cboDocPath"
        '
        'lblDocID
        '
        resources.ApplyResources(Me.lblDocID, "lblDocID")
        Me.lblDocID.Cursor = System.Windows.Forms.Cursors.Default
        Me.lblDocID.Name = "lblDocID"
        '
        'linDocTable
        '
        Me.linDocTable.BackColor = System.Drawing.SystemColors.InactiveCaption
        resources.ApplyResources(Me.linDocTable, "linDocTable")
        Me.linDocTable.Name = "linDocTable"
        '
        'lblDocTable
        '
        resources.ApplyResources(lblDocTable, "lblDocTable")
        lblDocTable.Cursor = System.Windows.Forms.Cursors.Default
        lblDocTable.Name = "lblDocTable"
        '
        'lblDocPath
        '
        resources.ApplyResources(Me.lblDocPath, "lblDocPath")
        Me.lblDocPath.Cursor = System.Windows.Forms.Cursors.Default
        Me.lblDocPath.Name = "lblDocPath"
        '
        'fraIndexVals
        '
        fraIndexVals.Controls.Add(Me.cboITables)
        fraIndexVals.Controls.Add(fraLinkBox)
        fraIndexVals.Controls.Add(lblIndexTable)
        resources.ApplyResources(fraIndexVals, "fraIndexVals")
        fraIndexVals.Name = "fraIndexVals"
        fraIndexVals.TabStop = False
        '
        'cboITables
        '
        Me.cboITables.BackColor = System.Drawing.SystemColors.Window
        Me.cboITables.Cursor = System.Windows.Forms.Cursors.Default
        Me.cboITables.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList
        Me.cboITables.ForeColor = System.Drawing.SystemColors.WindowText
        resources.ApplyResources(Me.cboITables, "cboITables")
        Me.cboITables.Name = "cboITables"
        Me.cboITables.Sorted = True
        '
        'fraLinkBox
        '
        fraLinkBox.Controls.Add(Me.indexValuesDataGridView)
        resources.ApplyResources(fraLinkBox, "fraLinkBox")
        fraLinkBox.Name = "fraLinkBox"
        fraLinkBox.TabStop = False
        '
        'indexValuesDataGridView
        '
        Me.indexValuesDataGridView.AllowUserToAddRows = False
        Me.indexValuesDataGridView.AllowUserToDeleteRows = False
        Me.indexValuesDataGridView.AllowUserToResizeRows = False
        Me.indexValuesDataGridView.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.Fill
        Me.indexValuesDataGridView.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D
        Me.indexValuesDataGridView.CellBorderStyle = System.Windows.Forms.DataGridViewCellBorderStyle.Sunken
        DataGridViewCellStyle1.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleCenter
        DataGridViewCellStyle1.BackColor = System.Drawing.SystemColors.Control
        DataGridViewCellStyle1.Font = New System.Drawing.Font("Microsoft Sans Serif", 8.0!)
        DataGridViewCellStyle1.ForeColor = System.Drawing.SystemColors.WindowText
        DataGridViewCellStyle1.SelectionBackColor = System.Drawing.SystemColors.Highlight
        DataGridViewCellStyle1.SelectionForeColor = System.Drawing.SystemColors.HighlightText
        DataGridViewCellStyle1.WrapMode = System.Windows.Forms.DataGridViewTriState.[True]
        Me.indexValuesDataGridView.ColumnHeadersDefaultCellStyle = DataGridViewCellStyle1
        Me.indexValuesDataGridView.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize
        DataGridViewCellStyle2.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft
        DataGridViewCellStyle2.BackColor = System.Drawing.SystemColors.Control
        DataGridViewCellStyle2.Font = New System.Drawing.Font("Microsoft Sans Serif", 8.0!)
        DataGridViewCellStyle2.ForeColor = System.Drawing.SystemColors.ControlText
        DataGridViewCellStyle2.SelectionBackColor = System.Drawing.SystemColors.Highlight
        DataGridViewCellStyle2.SelectionForeColor = System.Drawing.SystemColors.HighlightText
        DataGridViewCellStyle2.WrapMode = System.Windows.Forms.DataGridViewTriState.[False]
        Me.indexValuesDataGridView.DefaultCellStyle = DataGridViewCellStyle2
        resources.ApplyResources(Me.indexValuesDataGridView, "indexValuesDataGridView")
        Me.indexValuesDataGridView.EditMode = System.Windows.Forms.DataGridViewEditMode.EditOnEnter
        Me.indexValuesDataGridView.MultiSelect = False
        Me.indexValuesDataGridView.Name = "indexValuesDataGridView"
        Me.indexValuesDataGridView.RowHeadersVisible = False
        DataGridViewCellStyle3.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft
        Me.indexValuesDataGridView.RowsDefaultCellStyle = DataGridViewCellStyle3
        Me.indexValuesDataGridView.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect
        Me.indexValuesDataGridView.SetupData = Nothing
        Me.indexValuesDataGridView.StandardTab = True
        '
        'lblIndexTable
        '
        resources.ApplyResources(lblIndexTable, "lblIndexTable")
        lblIndexTable.Cursor = System.Windows.Forms.Cursors.Default
        lblIndexTable.Name = "lblIndexTable"
        '
        '_fraTab_2
        '
        _fraTab_2.Controls.Add(fraPDFFiles)
        _fraTab_2.Controls.Add(fraImageFiles)
        _fraTab_2.Controls.Add(fraOCRFiles)
        _fraTab_2.Cursor = System.Windows.Forms.Cursors.Default
        Me.fraTab.SetIndex(_fraTab_2, CType(2, Short))
        resources.ApplyResources(_fraTab_2, "_fraTab_2")
        _fraTab_2.Name = "_fraTab_2"
        '
        'fraPDFFiles
        '
        fraPDFFiles.Controls.Add(Me.chkReleaseKofaxPDF)
        fraPDFFiles.Controls.Add(Me.txtKofaxPDFDir)
        fraPDFFiles.Controls.Add(Me.cmdPDFBrowse)
        fraPDFFiles.Controls.Add(Me.lblPDFDir)
        resources.ApplyResources(fraPDFFiles, "fraPDFFiles")
        fraPDFFiles.Name = "fraPDFFiles"
        fraPDFFiles.TabStop = False
        '
        'chkReleaseKofaxPDF
        '
        resources.ApplyResources(Me.chkReleaseKofaxPDF, "chkReleaseKofaxPDF")
        Me.chkReleaseKofaxPDF.Cursor = System.Windows.Forms.Cursors.Default
        Me.chkReleaseKofaxPDF.Name = "chkReleaseKofaxPDF"
        Me.chkReleaseKofaxPDF.UseVisualStyleBackColor = True
        '
        'txtKofaxPDFDir
        '
        Me.txtKofaxPDFDir.AcceptsReturn = True
        Me.txtKofaxPDFDir.Cursor = System.Windows.Forms.Cursors.IBeam
        resources.ApplyResources(Me.txtKofaxPDFDir, "txtKofaxPDFDir")
        Me.txtKofaxPDFDir.Name = "txtKofaxPDFDir"
        '
        'cmdPDFBrowse
        '
        Me.cmdPDFBrowse.Cursor = System.Windows.Forms.Cursors.Default
        resources.ApplyResources(Me.cmdPDFBrowse, "cmdPDFBrowse")
        Me.cmdPDFBrowse.Name = "cmdPDFBrowse"
        Me.cmdPDFBrowse.UseVisualStyleBackColor = True
        '
        'lblPDFDir
        '
        resources.ApplyResources(Me.lblPDFDir, "lblPDFDir")
        Me.lblPDFDir.Cursor = System.Windows.Forms.Cursors.Default
        Me.lblPDFDir.Name = "lblPDFDir"
        '
        'fraImageFiles
        '
        fraImageFiles.Controls.Add(Me.chkReleaseImageFiles)
        fraImageFiles.Controls.Add(Me.chkSkipFirstPage)
        fraImageFiles.Controls.Add(Me.cmdIFBrowse)
        fraImageFiles.Controls.Add(Me.txtImageDir)
        fraImageFiles.Controls.Add(Me.lblImageDir)
        resources.ApplyResources(fraImageFiles, "fraImageFiles")
        fraImageFiles.Name = "fraImageFiles"
        fraImageFiles.TabStop = False
        '
        'chkReleaseImageFiles
        '
        resources.ApplyResources(Me.chkReleaseImageFiles, "chkReleaseImageFiles")
        Me.chkReleaseImageFiles.Cursor = System.Windows.Forms.Cursors.Default
        Me.chkReleaseImageFiles.Name = "chkReleaseImageFiles"
        Me.chkReleaseImageFiles.UseVisualStyleBackColor = True
        '
        'chkSkipFirstPage
        '
        resources.ApplyResources(Me.chkSkipFirstPage, "chkSkipFirstPage")
        Me.chkSkipFirstPage.Cursor = System.Windows.Forms.Cursors.Default
        Me.chkSkipFirstPage.Name = "chkSkipFirstPage"
        Me.chkSkipFirstPage.UseVisualStyleBackColor = True
        '
        'cmdIFBrowse
        '
        Me.cmdIFBrowse.Cursor = System.Windows.Forms.Cursors.Default
        resources.ApplyResources(Me.cmdIFBrowse, "cmdIFBrowse")
        Me.cmdIFBrowse.Name = "cmdIFBrowse"
        Me.cmdIFBrowse.UseVisualStyleBackColor = True
        '
        'txtImageDir
        '
        Me.txtImageDir.AcceptsReturn = True
        Me.txtImageDir.Cursor = System.Windows.Forms.Cursors.IBeam
        resources.ApplyResources(Me.txtImageDir, "txtImageDir")
        Me.txtImageDir.Name = "txtImageDir"
        '
        'lblImageDir
        '
        resources.ApplyResources(Me.lblImageDir, "lblImageDir")
        Me.lblImageDir.Cursor = System.Windows.Forms.Cursors.Default
        Me.lblImageDir.Name = "lblImageDir"
        '
        'fraOCRFiles
        '
        fraOCRFiles.Controls.Add(Me.chkReleaseOCRFullText)
        fraOCRFiles.Controls.Add(Me.cmdOCRBrowse)
        fraOCRFiles.Controls.Add(Me.txtOCRDir)
        fraOCRFiles.Controls.Add(Me.lblOCRDir)
        resources.ApplyResources(fraOCRFiles, "fraOCRFiles")
        fraOCRFiles.Name = "fraOCRFiles"
        fraOCRFiles.TabStop = False
        '
        'chkReleaseOCRFullText
        '
        resources.ApplyResources(Me.chkReleaseOCRFullText, "chkReleaseOCRFullText")
        Me.chkReleaseOCRFullText.Cursor = System.Windows.Forms.Cursors.Default
        Me.chkReleaseOCRFullText.Name = "chkReleaseOCRFullText"
        Me.chkReleaseOCRFullText.UseVisualStyleBackColor = True
        '
        'cmdOCRBrowse
        '
        Me.cmdOCRBrowse.Cursor = System.Windows.Forms.Cursors.Default
        resources.ApplyResources(Me.cmdOCRBrowse, "cmdOCRBrowse")
        Me.cmdOCRBrowse.Name = "cmdOCRBrowse"
        Me.cmdOCRBrowse.UseVisualStyleBackColor = True
        '
        'txtOCRDir
        '
        Me.txtOCRDir.AcceptsReturn = True
        Me.txtOCRDir.Cursor = System.Windows.Forms.Cursors.IBeam
        resources.ApplyResources(Me.txtOCRDir, "txtOCRDir")
        Me.txtOCRDir.Name = "txtOCRDir"
        '
        'lblOCRDir
        '
        resources.ApplyResources(Me.lblOCRDir, "lblOCRDir")
        Me.lblOCRDir.Cursor = System.Windows.Forms.Cursors.Default
        Me.lblOCRDir.Name = "lblOCRDir"
        '
        '_fraTab_3
        '
        _fraTab_3.Controls.Add(Me.fraReleaseImagesAs)
        _fraTab_3.Cursor = System.Windows.Forms.Cursors.Default
        Me.fraTab.SetIndex(_fraTab_3, CType(3, Short))
        resources.ApplyResources(_fraTab_3, "_fraTab_3")
        _fraTab_3.Name = "_fraTab_3"
        '
        'fraReleaseImagesAs
        '
        Me.fraReleaseImagesAs.Controls.Add(Me.cboImageType)
        Me.fraReleaseImagesAs.Controls.Add(Me.lblImageType)
        resources.ApplyResources(Me.fraReleaseImagesAs, "fraReleaseImagesAs")
        Me.fraReleaseImagesAs.Name = "fraReleaseImagesAs"
        Me.fraReleaseImagesAs.TabStop = False
        '
        'cboImageType
        '
        Me.cboImageType.Cursor = System.Windows.Forms.Cursors.Default
        Me.cboImageType.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList
        resources.ApplyResources(Me.cboImageType, "cboImageType")
        Me.cboImageType.Name = "cboImageType"
        Me.cboImageType.Sorted = True
        '
        'lblImageType
        '
        resources.ApplyResources(Me.lblImageType, "lblImageType")
        Me.lblImageType.Cursor = System.Windows.Forms.Cursors.Default
        Me.lblImageType.Name = "lblImageType"
        '
        'lblName
        '
        resources.ApplyResources(lblName, "lblName")
        lblName.Cursor = System.Windows.Forms.Cursors.Default
        lblName.Name = "lblName"
        '
        'lblDocClass
        '
        resources.ApplyResources(lblDocClass, "lblDocClass")
        lblDocClass.Cursor = System.Windows.Forms.Cursors.Default
        lblDocClass.Name = "lblDocClass"
        '
        'lblBatchClass
        '
        resources.ApplyResources(lblBatchClass, "lblBatchClass")
        lblBatchClass.Cursor = System.Windows.Forms.Cursors.Default
        lblBatchClass.Name = "lblBatchClass"
        '
        'txtName
        '
        Me.txtName.AcceptsReturn = True
        Me.txtName.Cursor = System.Windows.Forms.Cursors.IBeam
        resources.ApplyResources(Me.txtName, "txtName")
        Me.txtName.Name = "txtName"
        '
        'cmdHelp
        '
        Me.cmdHelp.Cursor = System.Windows.Forms.Cursors.Default
        resources.ApplyResources(Me.cmdHelp, "cmdHelp")
        Me.cmdHelp.Name = "cmdHelp"
        Me.cmdHelp.UseVisualStyleBackColor = True
        '
        'cmdApply
        '
        Me.cmdApply.Cursor = System.Windows.Forms.Cursors.Default
        resources.ApplyResources(Me.cmdApply, "cmdApply")
        Me.cmdApply.Name = "cmdApply"
        Me.cmdApply.UseVisualStyleBackColor = True
        '
        'cmdCancel
        '
        Me.cmdCancel.Cursor = System.Windows.Forms.Cursors.Default
        Me.cmdCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel
        resources.ApplyResources(Me.cmdCancel, "cmdCancel")
        Me.cmdCancel.Name = "cmdCancel"
        Me.cmdCancel.UseVisualStyleBackColor = True
        '
        'cmdOK
        '
        Me.cmdOK.Cursor = System.Windows.Forms.Cursors.Default
        Me.cmdOK.DialogResult = System.Windows.Forms.DialogResult.OK
        resources.ApplyResources(Me.cmdOK, "cmdOK")
        Me.cmdOK.Name = "cmdOK"
        Me.cmdOK.UseVisualStyleBackColor = True
        '
        'tabDatabase
        '
        Me.tabDatabase.Controls.Add(Me.databaseTabPage)
        Me.tabDatabase.Controls.Add(Me.tableSettingsTabPage)
        Me.tabDatabase.Controls.Add(Me.documentStorageTabPage)
        Me.tabDatabase.Controls.Add(Me.imageFormatTabPage)
        resources.ApplyResources(Me.tabDatabase, "tabDatabase")
        Me.tabDatabase.Name = "tabDatabase"
        Me.tabDatabase.SelectedIndex = 0
        '
        'databaseTabPage
        '
        Me.databaseTabPage.Controls.Add(_fraTab_0)
        resources.ApplyResources(Me.databaseTabPage, "databaseTabPage")
        Me.databaseTabPage.Name = "databaseTabPage"
        Me.databaseTabPage.UseVisualStyleBackColor = True
        '
        'tableSettingsTabPage
        '
        Me.tableSettingsTabPage.Controls.Add(_fraTab_1)
        resources.ApplyResources(Me.tableSettingsTabPage, "tableSettingsTabPage")
        Me.tableSettingsTabPage.Name = "tableSettingsTabPage"
        Me.tableSettingsTabPage.UseVisualStyleBackColor = True
        '
        'documentStorageTabPage
        '
        Me.documentStorageTabPage.Controls.Add(_fraTab_2)
        resources.ApplyResources(Me.documentStorageTabPage, "documentStorageTabPage")
        Me.documentStorageTabPage.Name = "documentStorageTabPage"
        Me.documentStorageTabPage.UseVisualStyleBackColor = True
        '
        'imageFormatTabPage
        '
        Me.imageFormatTabPage.Controls.Add(_fraTab_3)
        resources.ApplyResources(Me.imageFormatTabPage, "imageFormatTabPage")
        Me.imageFormatTabPage.Name = "imageFormatTabPage"
        Me.imageFormatTabPage.UseVisualStyleBackColor = True
        '
        'lblDocClassName
        '
        resources.ApplyResources(Me.lblDocClassName, "lblDocClassName")
        Me.lblDocClassName.BackColor = System.Drawing.SystemColors.Control
        Me.lblDocClassName.Cursor = System.Windows.Forms.Cursors.Default
        Me.lblDocClassName.ForeColor = System.Drawing.SystemColors.ControlText
        Me.lblDocClassName.Name = "lblDocClassName"
        '
        'lblBatchClassName
        '
        resources.ApplyResources(Me.lblBatchClassName, "lblBatchClassName")
        Me.lblBatchClassName.BackColor = System.Drawing.SystemColors.Control
        Me.lblBatchClassName.Cursor = System.Windows.Forms.Cursors.Default
        Me.lblBatchClassName.ForeColor = System.Drawing.SystemColors.ControlText
        Me.lblBatchClassName.Name = "lblBatchClassName"
        '
        'frmSetup
        '
        Me.AcceptButton = Me.cmdOK
        resources.ApplyResources(Me, "$this")
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.AutoValidate = System.Windows.Forms.AutoValidate.Disable
        Me.BackColor = System.Drawing.SystemColors.Control
        Me.CancelButton = Me.cmdCancel
        Me.Controls.Add(Me.txtName)
        Me.Controls.Add(Me.cmdHelp)
        Me.Controls.Add(Me.cmdApply)
        Me.Controls.Add(Me.cmdCancel)
        Me.Controls.Add(Me.cmdOK)
        Me.Controls.Add(Me.tabDatabase)
        Me.Controls.Add(lblName)
        Me.Controls.Add(Me.lblDocClassName)
        Me.Controls.Add(Me.lblBatchClassName)
        Me.Controls.Add(lblDocClass)
        Me.Controls.Add(lblBatchClass)
        Me.Cursor = System.Windows.Forms.Cursors.Default
        Me.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog
        Me.KeyPreview = True
        Me.MaximizeBox = False
        Me.MinimizeBox = False
        Me.Name = "frmSetup"
        Me.ShowInTaskbar = False
        _fraTab_0.ResumeLayout(False)
        fraDBControls.ResumeLayout(False)
        fraDBControls.PerformLayout()
        _fraTab_1.ResumeLayout(False)
        fraDocVals.ResumeLayout(False)
        fraDocVals.PerformLayout()
        fraIndexVals.ResumeLayout(False)
        fraIndexVals.PerformLayout()
        fraLinkBox.ResumeLayout(False)
        CType(Me.indexValuesDataGridView, System.ComponentModel.ISupportInitialize).EndInit()
        _fraTab_2.ResumeLayout(False)
        fraPDFFiles.ResumeLayout(False)
        fraPDFFiles.PerformLayout()
        fraImageFiles.ResumeLayout(False)
        fraImageFiles.PerformLayout()
        fraOCRFiles.ResumeLayout(False)
        fraOCRFiles.PerformLayout()
        _fraTab_3.ResumeLayout(False)
        Me.fraReleaseImagesAs.ResumeLayout(False)
        Me.fraReleaseImagesAs.PerformLayout()
        Me.tabDatabase.ResumeLayout(False)
        Me.databaseTabPage.ResumeLayout(False)
        Me.tableSettingsTabPage.ResumeLayout(False)
        Me.documentStorageTabPage.ResumeLayout(False)
        Me.imageFormatTabPage.ResumeLayout(False)
        CType(Me.fraTab, System.ComponentModel.ISupportInitialize).EndInit()
        Me.ResumeLayout(False)
        Me.PerformLayout()

    End Sub
    Friend WithEvents indexValuesDataGridView As IndexValueDataGridView
    Public WithEvents lblConnString As System.Windows.Forms.Label
    Public WithEvents tableSettingsTabPage As System.Windows.Forms.TabPage
    Public WithEvents lblPDFDir As System.Windows.Forms.Label
    Public WithEvents lblOCRDir As System.Windows.Forms.Label
    Public WithEvents lblDocID As System.Windows.Forms.Label
    Public WithEvents lblDocPath As System.Windows.Forms.Label
    Public WithEvents lblImageDir As System.Windows.Forms.Label
    Public WithEvents lblImageType As System.Windows.Forms.Label
    Public WithEvents fraReleaseImagesAs As System.Windows.Forms.GroupBox
#End Region 
End Class