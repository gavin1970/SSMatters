using System.Windows.Forms;

namespace SSMatters
{
    partial class FrmMain
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code
        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FrmMain));
            this.MainWinMenuStrip = new System.Windows.Forms.MenuStrip();
            this.MenuFile = new System.Windows.Forms.ToolStripMenuItem();
            this.MenuActions = new System.Windows.Forms.ToolStripMenuItem();
            this.LblStartingDriveOrPath = new System.Windows.Forms.ToolStripMenuItem();
            this.CmbStartingDriveOrPath = new System.Windows.Forms.ToolStripComboBox();
            this.MainStatusStrip = new System.Windows.Forms.StatusStrip();
            this.ToolStatus = new System.Windows.Forms.ToolStripStatusLabel();
            this.ButtonStartScan = new System.Windows.Forms.Button();
            this.PanelTopMenuStartStopButtons = new System.Windows.Forms.Panel();
            this.panel3 = new System.Windows.Forms.Panel();
            this.CkbAutoIOMonitor = new System.Windows.Forms.CheckBox();
            this.ButtonStopScan = new System.Windows.Forms.Button();
            this.BtnPickFolder = new System.Windows.Forms.Button();
            this.FolderTreeMapPanel = new System.Windows.Forms.Panel();
            this.FileFolderContextMenuStrip = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.toolStripSeparator1 = new System.Windows.Forms.ToolStripSeparator();
            this.LblFolderHoverInfo = new System.Windows.Forms.Label();
            this.LblCurrentPath = new System.Windows.Forms.Label();
            this.PanelLocationRootButton = new System.Windows.Forms.Panel();
            this.BtnBackFolder = new System.Windows.Forms.Button();
            this.label1 = new System.Windows.Forms.Label();
            this.BtnRoot = new System.Windows.Forms.Button();
            this.FileFldrSplitContainer = new System.Windows.Forms.SplitContainer();
            this.FileTreeMapPanel = new System.Windows.Forms.Panel();
            this.LblFileHoverInfo = new System.Windows.Forms.Label();
            this.WatchTimer = new System.Windows.Forms.Timer(this.components);
            this.PanelSearchResults = new System.Windows.Forms.Panel();
            this.PanelFoldersAndFiles = new System.Windows.Forms.Panel();
            this.BackToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.CopyInformationToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.OpenLocationToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.DeleteToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.FindToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.AboutToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.ExitToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.MenuActionsBack = new System.Windows.Forms.ToolStripMenuItem();
            this.FoundCountLabel = new System.Windows.Forms.Label();
            this.MainWinMenuStrip.SuspendLayout();
            this.MainStatusStrip.SuspendLayout();
            this.PanelTopMenuStartStopButtons.SuspendLayout();
            this.panel3.SuspendLayout();
            this.FolderTreeMapPanel.SuspendLayout();
            this.FileFolderContextMenuStrip.SuspendLayout();
            this.PanelLocationRootButton.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.FileFldrSplitContainer)).BeginInit();
            this.FileFldrSplitContainer.Panel1.SuspendLayout();
            this.FileFldrSplitContainer.Panel2.SuspendLayout();
            this.FileFldrSplitContainer.SuspendLayout();
            this.FileTreeMapPanel.SuspendLayout();
            this.PanelSearchResults.SuspendLayout();
            this.PanelFoldersAndFiles.SuspendLayout();
            this.SuspendLayout();
            // 
            // MainWinMenuStrip
            // 
            this.MainWinMenuStrip.BackColor = System.Drawing.SystemColors.MenuBar;
            this.MainWinMenuStrip.Dock = System.Windows.Forms.DockStyle.None;
            this.MainWinMenuStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.MenuFile,
            this.MenuActions,
            this.LblStartingDriveOrPath,
            this.CmbStartingDriveOrPath});
            this.MainWinMenuStrip.Location = new System.Drawing.Point(0, 2);
            this.MainWinMenuStrip.Name = "MainWinMenuStrip";
            this.MainWinMenuStrip.Size = new System.Drawing.Size(440, 27);
            this.MainWinMenuStrip.TabIndex = 0;
            this.MainWinMenuStrip.Text = "menuStrip1";
            // 
            // MenuFile
            // 
            this.MenuFile.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.FindToolStripMenuItem,
            this.AboutToolStripMenuItem,
            this.ExitToolStripMenuItem});
            this.MenuFile.Name = "MenuFile";
            this.MenuFile.Size = new System.Drawing.Size(37, 23);
            this.MenuFile.Text = "&File";
            // 
            // MenuActions
            // 
            this.MenuActions.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.MenuActionsBack});
            this.MenuActions.Name = "MenuActions";
            this.MenuActions.Size = new System.Drawing.Size(59, 23);
            this.MenuActions.Text = "&Actions";
            // 
            // LblStartingDriveOrPath
            // 
            this.LblStartingDriveOrPath.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            this.LblStartingDriveOrPath.Name = "LblStartingDriveOrPath";
            this.LblStartingDriveOrPath.Size = new System.Drawing.Size(134, 23);
            this.LblStartingDriveOrPath.Text = "Starting Drive or Path:";
            this.LblStartingDriveOrPath.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // CmbStartingDriveOrPath
            // 
            this.CmbStartingDriveOrPath.Items.AddRange(new object[] {
            "C:\\",
            "C:\\Java_Tools\\apache-maven-3.8.6"});
            this.CmbStartingDriveOrPath.Name = "CmbStartingDriveOrPath";
            this.CmbStartingDriveOrPath.Size = new System.Drawing.Size(200, 23);
            // 
            // MainStatusStrip
            // 
            this.MainStatusStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.ToolStatus});
            this.MainStatusStrip.Location = new System.Drawing.Point(0, 625);
            this.MainStatusStrip.Name = "MainStatusStrip";
            this.MainStatusStrip.Size = new System.Drawing.Size(970, 22);
            this.MainStatusStrip.TabIndex = 1;
            this.MainStatusStrip.Text = "statusStrip1";
            this.MainStatusStrip.MouseMove += new System.Windows.Forms.MouseEventHandler(this.Default_Cursor);
            // 
            // ToolStatus
            // 
            this.ToolStatus.Name = "ToolStatus";
            this.ToolStatus.Size = new System.Drawing.Size(955, 17);
            this.ToolStatus.Spring = true;
            this.ToolStatus.Text = "Ready...";
            this.ToolStatus.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // ButtonStartScan
            // 
            this.ButtonStartScan.BackColor = System.Drawing.SystemColors.ButtonFace;
            this.ButtonStartScan.Dock = System.Windows.Forms.DockStyle.Right;
            this.ButtonStartScan.Location = new System.Drawing.Point(166, 5);
            this.ButtonStartScan.Name = "ButtonStartScan";
            this.ButtonStartScan.Size = new System.Drawing.Size(54, 23);
            this.ButtonStartScan.TabIndex = 2;
            this.ButtonStartScan.Text = "&Scan";
            this.ButtonStartScan.UseVisualStyleBackColor = false;
            this.ButtonStartScan.Click += new System.EventHandler(this.ButtonStartScan_Click);
            // 
            // PanelTopMenuStartStopButtons
            // 
            this.PanelTopMenuStartStopButtons.BackColor = System.Drawing.SystemColors.Control;
            this.PanelTopMenuStartStopButtons.Controls.Add(this.panel3);
            this.PanelTopMenuStartStopButtons.Controls.Add(this.MainWinMenuStrip);
            this.PanelTopMenuStartStopButtons.Controls.Add(this.BtnPickFolder);
            this.PanelTopMenuStartStopButtons.Dock = System.Windows.Forms.DockStyle.Top;
            this.PanelTopMenuStartStopButtons.Location = new System.Drawing.Point(0, 0);
            this.PanelTopMenuStartStopButtons.Name = "PanelTopMenuStartStopButtons";
            this.PanelTopMenuStartStopButtons.Padding = new System.Windows.Forms.Padding(0, 0, 0, 5);
            this.PanelTopMenuStartStopButtons.Size = new System.Drawing.Size(970, 38);
            this.PanelTopMenuStartStopButtons.TabIndex = 3;
            this.PanelTopMenuStartStopButtons.MouseMove += new System.Windows.Forms.MouseEventHandler(this.Default_Cursor);
            // 
            // panel3
            // 
            this.panel3.Controls.Add(this.CkbAutoIOMonitor);
            this.panel3.Controls.Add(this.ButtonStopScan);
            this.panel3.Controls.Add(this.ButtonStartScan);
            this.panel3.Dock = System.Windows.Forms.DockStyle.Right;
            this.panel3.Location = new System.Drawing.Point(745, 0);
            this.panel3.Name = "panel3";
            this.panel3.Padding = new System.Windows.Forms.Padding(5);
            this.panel3.Size = new System.Drawing.Size(225, 33);
            this.panel3.TabIndex = 6;
            // 
            // CkbAutoIOMonitor
            // 
            this.CkbAutoIOMonitor.AutoSize = true;
            this.CkbAutoIOMonitor.Location = new System.Drawing.Point(20, 8);
            this.CkbAutoIOMonitor.Name = "CkbAutoIOMonitor";
            this.CkbAutoIOMonitor.Size = new System.Drawing.Size(86, 17);
            this.CkbAutoIOMonitor.TabIndex = 5;
            this.CkbAutoIOMonitor.Text = "&Auto Update";
            this.CkbAutoIOMonitor.UseVisualStyleBackColor = true;
            this.CkbAutoIOMonitor.CheckedChanged += new System.EventHandler(this.CkbAutoIOMonitor_CheckedChanged);
            // 
            // ButtonStopScan
            // 
            this.ButtonStopScan.BackColor = System.Drawing.SystemColors.ButtonFace;
            this.ButtonStopScan.Dock = System.Windows.Forms.DockStyle.Right;
            this.ButtonStopScan.Location = new System.Drawing.Point(112, 5);
            this.ButtonStopScan.Name = "ButtonStopScan";
            this.ButtonStopScan.Size = new System.Drawing.Size(54, 23);
            this.ButtonStopScan.TabIndex = 3;
            this.ButtonStopScan.Text = "&Stop";
            this.ButtonStopScan.UseVisualStyleBackColor = false;
            this.ButtonStopScan.Visible = false;
            this.ButtonStopScan.Click += new System.EventHandler(this.ButtonStopScan_Click);
            // 
            // BtnPickFolder
            // 
            this.BtnPickFolder.BackColor = System.Drawing.SystemColors.ActiveBorder;
            this.BtnPickFolder.Location = new System.Drawing.Point(381, 1);
            this.BtnPickFolder.Name = "BtnPickFolder";
            this.BtnPickFolder.Size = new System.Drawing.Size(31, 28);
            this.BtnPickFolder.TabIndex = 4;
            this.BtnPickFolder.Text = "...";
            this.BtnPickFolder.UseVisualStyleBackColor = false;
            this.BtnPickFolder.Click += new System.EventHandler(this.BtnPickFolder_Click);
            // 
            // FolderTreeMapPanel
            // 
            this.FolderTreeMapPanel.ContextMenuStrip = this.FileFolderContextMenuStrip;
            this.FolderTreeMapPanel.Controls.Add(this.LblFolderHoverInfo);
            this.FolderTreeMapPanel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.FolderTreeMapPanel.Location = new System.Drawing.Point(0, 0);
            this.FolderTreeMapPanel.Name = "FolderTreeMapPanel";
            this.FolderTreeMapPanel.Size = new System.Drawing.Size(471, 485);
            this.FolderTreeMapPanel.TabIndex = 4;
            this.FolderTreeMapPanel.Paint += new System.Windows.Forms.PaintEventHandler(this.FolderTreeMapPanel_Paint);
            this.FolderTreeMapPanel.MouseDoubleClick += new System.Windows.Forms.MouseEventHandler(this.FolderTreeMapPanel_MouseDoubleClick);
            this.FolderTreeMapPanel.MouseMove += new System.Windows.Forms.MouseEventHandler(this.TreeMapPanel_MouseMove);
            // 
            // FileFolderContextMenuStrip
            // 
            this.FileFolderContextMenuStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.BackToolStripMenuItem,
            this.toolStripSeparator1,
            this.CopyInformationToolStripMenuItem,
            this.OpenLocationToolStripMenuItem,
            this.DeleteToolStripMenuItem});
            this.FileFolderContextMenuStrip.Name = "FileFolderContextMenuStrip";
            this.FileFolderContextMenuStrip.Size = new System.Drawing.Size(169, 98);
            this.FileFolderContextMenuStrip.Opening += new System.ComponentModel.CancelEventHandler(this.FileFolderContextMenuStrip_Opening);
            // 
            // toolStripSeparator1
            // 
            this.toolStripSeparator1.Name = "toolStripSeparator1";
            this.toolStripSeparator1.Size = new System.Drawing.Size(165, 6);
            // 
            // LblFolderHoverInfo
            // 
            this.LblFolderHoverInfo.AutoSize = true;
            this.LblFolderHoverInfo.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(255)))), ((int)(((byte)(192)))));
            this.LblFolderHoverInfo.Location = new System.Drawing.Point(82, 82);
            this.LblFolderHoverInfo.Name = "LblFolderHoverInfo";
            this.LblFolderHoverInfo.Size = new System.Drawing.Size(86, 13);
            this.LblFolderHoverInfo.TabIndex = 0;
            this.LblFolderHoverInfo.Text = "Hover Text Here";
            this.LblFolderHoverInfo.Visible = false;
            this.LblFolderHoverInfo.MouseDoubleClick += new System.Windows.Forms.MouseEventHandler(this.LblHoverInfo_MouseDoubleClick);
            this.LblFolderHoverInfo.MouseMove += new System.Windows.Forms.MouseEventHandler(this.LblFolderHoverInfo_MouseMove);
            // 
            // LblCurrentPath
            // 
            this.LblCurrentPath.Dock = System.Windows.Forms.DockStyle.Fill;
            this.LblCurrentPath.Location = new System.Drawing.Point(0, 0);
            this.LblCurrentPath.Name = "LblCurrentPath";
            this.LblCurrentPath.Size = new System.Drawing.Size(894, 22);
            this.LblCurrentPath.TabIndex = 5;
            this.LblCurrentPath.Text = "......";
            this.LblCurrentPath.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.LblCurrentPath.MouseMove += new System.Windows.Forms.MouseEventHandler(this.Default_Cursor);
            // 
            // PanelLocationRootButton
            // 
            this.PanelLocationRootButton.Controls.Add(this.LblCurrentPath);
            this.PanelLocationRootButton.Controls.Add(this.BtnBackFolder);
            this.PanelLocationRootButton.Controls.Add(this.label1);
            this.PanelLocationRootButton.Controls.Add(this.BtnRoot);
            this.PanelLocationRootButton.Dock = System.Windows.Forms.DockStyle.Top;
            this.PanelLocationRootButton.Location = new System.Drawing.Point(0, 38);
            this.PanelLocationRootButton.Name = "PanelLocationRootButton";
            this.PanelLocationRootButton.Size = new System.Drawing.Size(970, 22);
            this.PanelLocationRootButton.TabIndex = 5;
            // 
            // BtnBackFolder
            // 
            this.BtnBackFolder.Dock = System.Windows.Forms.DockStyle.Right;
            this.BtnBackFolder.Enabled = false;
            this.BtnBackFolder.Location = new System.Drawing.Point(894, 0);
            this.BtnBackFolder.Name = "BtnBackFolder";
            this.BtnBackFolder.Size = new System.Drawing.Size(28, 22);
            this.BtnBackFolder.TabIndex = 7;
            this.BtnBackFolder.Text = "<<";
            this.BtnBackFolder.UseVisualStyleBackColor = true;
            this.BtnBackFolder.Click += new System.EventHandler(this.BtnBackFolder_Click);
            // 
            // label1
            // 
            this.label1.Dock = System.Windows.Forms.DockStyle.Right;
            this.label1.Location = new System.Drawing.Point(922, 0);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(5, 22);
            this.label1.TabIndex = 8;
            // 
            // BtnRoot
            // 
            this.BtnRoot.Dock = System.Windows.Forms.DockStyle.Right;
            this.BtnRoot.Enabled = false;
            this.BtnRoot.Location = new System.Drawing.Point(927, 0);
            this.BtnRoot.Name = "BtnRoot";
            this.BtnRoot.Size = new System.Drawing.Size(43, 22);
            this.BtnRoot.TabIndex = 6;
            this.BtnRoot.Text = "Root";
            this.BtnRoot.UseVisualStyleBackColor = true;
            this.BtnRoot.Click += new System.EventHandler(this.BtnRoot_Click);
            // 
            // FileFldrSplitContainer
            // 
            this.FileFldrSplitContainer.Cursor = System.Windows.Forms.Cursors.SizeWE;
            this.FileFldrSplitContainer.Dock = System.Windows.Forms.DockStyle.Fill;
            this.FileFldrSplitContainer.Location = new System.Drawing.Point(5, 5);
            this.FileFldrSplitContainer.Name = "FileFldrSplitContainer";
            // 
            // FileFldrSplitContainer.Panel1
            // 
            this.FileFldrSplitContainer.Panel1.Controls.Add(this.FolderTreeMapPanel);
            // 
            // FileFldrSplitContainer.Panel2
            // 
            this.FileFldrSplitContainer.Panel2.Controls.Add(this.FileTreeMapPanel);
            this.FileFldrSplitContainer.Size = new System.Drawing.Size(960, 485);
            this.FileFldrSplitContainer.SplitterDistance = 471;
            this.FileFldrSplitContainer.SplitterWidth = 10;
            this.FileFldrSplitContainer.TabIndex = 6;
            this.FileFldrSplitContainer.SplitterMoved += new System.Windows.Forms.SplitterEventHandler(this.FileFldrSplitContainer_SplitterMoved);
            // 
            // FileTreeMapPanel
            // 
            this.FileTreeMapPanel.ContextMenuStrip = this.FileFolderContextMenuStrip;
            this.FileTreeMapPanel.Controls.Add(this.LblFileHoverInfo);
            this.FileTreeMapPanel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.FileTreeMapPanel.Location = new System.Drawing.Point(0, 0);
            this.FileTreeMapPanel.Name = "FileTreeMapPanel";
            this.FileTreeMapPanel.Size = new System.Drawing.Size(479, 485);
            this.FileTreeMapPanel.TabIndex = 0;
            this.FileTreeMapPanel.Paint += new System.Windows.Forms.PaintEventHandler(this.FileTreeMapPanel_Paint);
            this.FileTreeMapPanel.MouseDoubleClick += new System.Windows.Forms.MouseEventHandler(this.FileTreeMapPanel_MouseDoubleClick);
            this.FileTreeMapPanel.MouseMove += new System.Windows.Forms.MouseEventHandler(this.TreeMapPanel_MouseMove);
            // 
            // LblFileHoverInfo
            // 
            this.LblFileHoverInfo.AutoSize = true;
            this.LblFileHoverInfo.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(255)))), ((int)(((byte)(192)))));
            this.LblFileHoverInfo.Location = new System.Drawing.Point(97, 92);
            this.LblFileHoverInfo.Name = "LblFileHoverInfo";
            this.LblFileHoverInfo.Size = new System.Drawing.Size(86, 13);
            this.LblFileHoverInfo.TabIndex = 1;
            this.LblFileHoverInfo.Text = "Hover Text Here";
            this.LblFileHoverInfo.Visible = false;
            this.LblFileHoverInfo.MouseDoubleClick += new System.Windows.Forms.MouseEventHandler(this.LblHoverInfo_MouseDoubleClick);
            this.LblFileHoverInfo.MouseMove += new System.Windows.Forms.MouseEventHandler(this.LblFileHoverInfo_MouseMove);
            // 
            // WatchTimer
            // 
            this.WatchTimer.Enabled = true;
            this.WatchTimer.Interval = 300;
            this.WatchTimer.Tick += new System.EventHandler(this.WatchTimer_Tick);
            // 
            // PanelSearchResults
            // 
            this.PanelSearchResults.Controls.Add(this.FoundCountLabel);
            this.PanelSearchResults.Dock = System.Windows.Forms.DockStyle.Top;
            this.PanelSearchResults.Location = new System.Drawing.Point(0, 60);
            this.PanelSearchResults.Name = "PanelSearchResults";
            this.PanelSearchResults.Size = new System.Drawing.Size(970, 70);
            this.PanelSearchResults.TabIndex = 7;
            this.PanelSearchResults.Visible = false;
            // 
            // PanelFoldersAndFiles
            // 
            this.PanelFoldersAndFiles.Controls.Add(this.FileFldrSplitContainer);
            this.PanelFoldersAndFiles.Dock = System.Windows.Forms.DockStyle.Fill;
            this.PanelFoldersAndFiles.Location = new System.Drawing.Point(0, 130);
            this.PanelFoldersAndFiles.Name = "PanelFoldersAndFiles";
            this.PanelFoldersAndFiles.Padding = new System.Windows.Forms.Padding(5);
            this.PanelFoldersAndFiles.Size = new System.Drawing.Size(970, 495);
            this.PanelFoldersAndFiles.TabIndex = 8;
            // 
            // BackToolStripMenuItem
            // 
            this.BackToolStripMenuItem.Image = global::SSMatters.Properties.Resources.dot_left_arrow_live;
            this.BackToolStripMenuItem.Name = "BackToolStripMenuItem";
            this.BackToolStripMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Alt | System.Windows.Forms.Keys.B)));
            this.BackToolStripMenuItem.Size = new System.Drawing.Size(168, 22);
            this.BackToolStripMenuItem.Text = "&Back";
            this.BackToolStripMenuItem.Click += new System.EventHandler(this.BackToolStripMenuItem_Click);
            // 
            // CopyInformationToolStripMenuItem
            // 
            this.CopyInformationToolStripMenuItem.Image = global::SSMatters.Properties.Resources.copy_to_clipboard;
            this.CopyInformationToolStripMenuItem.Name = "CopyInformationToolStripMenuItem";
            this.CopyInformationToolStripMenuItem.Size = new System.Drawing.Size(168, 22);
            this.CopyInformationToolStripMenuItem.Text = "&Copy Information";
            this.CopyInformationToolStripMenuItem.Click += new System.EventHandler(this.CopyInformationToolStripMenuItem_Click);
            // 
            // OpenLocationToolStripMenuItem
            // 
            this.OpenLocationToolStripMenuItem.Image = global::SSMatters.Properties.Resources.folder;
            this.OpenLocationToolStripMenuItem.Name = "OpenLocationToolStripMenuItem";
            this.OpenLocationToolStripMenuItem.Size = new System.Drawing.Size(168, 22);
            this.OpenLocationToolStripMenuItem.Text = "&Open Location";
            this.OpenLocationToolStripMenuItem.Click += new System.EventHandler(this.OpenLocationToolStripMenuItem_Click);
            // 
            // DeleteToolStripMenuItem
            // 
            this.DeleteToolStripMenuItem.Image = global::SSMatters.Properties.Resources.delete;
            this.DeleteToolStripMenuItem.Name = "DeleteToolStripMenuItem";
            this.DeleteToolStripMenuItem.Size = new System.Drawing.Size(168, 22);
            this.DeleteToolStripMenuItem.Text = "&Delete";
            this.DeleteToolStripMenuItem.Click += new System.EventHandler(this.DeleteToolStripMenuItem_Click);
            // 
            // FindToolStripMenuItem
            // 
            this.FindToolStripMenuItem.Image = global::SSMatters.Properties.Resources.Search;
            this.FindToolStripMenuItem.Name = "FindToolStripMenuItem";
            this.FindToolStripMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.F)));
            this.FindToolStripMenuItem.Size = new System.Drawing.Size(180, 22);
            this.FindToolStripMenuItem.Text = "&Find";
            this.FindToolStripMenuItem.Click += new System.EventHandler(this.FindToolStripMenuItem_Click);
            // 
            // AboutToolStripMenuItem
            // 
            this.AboutToolStripMenuItem.Image = global::SSMatters.Properties.Resources.chizl_default_icon;
            this.AboutToolStripMenuItem.Name = "AboutToolStripMenuItem";
            this.AboutToolStripMenuItem.ShortcutKeys = System.Windows.Forms.Keys.F1;
            this.AboutToolStripMenuItem.Size = new System.Drawing.Size(180, 22);
            this.AboutToolStripMenuItem.Text = "&About";
            this.AboutToolStripMenuItem.Click += new System.EventHandler(this.AboutToolStripMenuItem_Click);
            // 
            // ExitToolStripMenuItem
            // 
            this.ExitToolStripMenuItem.Image = global::SSMatters.Properties.Resources.power_off;
            this.ExitToolStripMenuItem.Name = "ExitToolStripMenuItem";
            this.ExitToolStripMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.X)));
            this.ExitToolStripMenuItem.Size = new System.Drawing.Size(180, 22);
            this.ExitToolStripMenuItem.Text = "E&xit";
            this.ExitToolStripMenuItem.Click += new System.EventHandler(this.ExitToolStripMenuItem_Click);
            // 
            // MenuActionsBack
            // 
            this.MenuActionsBack.Image = global::SSMatters.Properties.Resources.dot_left_arrow_live;
            this.MenuActionsBack.Name = "MenuActionsBack";
            this.MenuActionsBack.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Alt | System.Windows.Forms.Keys.B)));
            this.MenuActionsBack.Size = new System.Drawing.Size(180, 22);
            this.MenuActionsBack.Text = "&Back";
            this.MenuActionsBack.Click += new System.EventHandler(this.MenuActionsBack_Click);
            // 
            // FoundCountLabel
            // 
            this.FoundCountLabel.AutoSize = true;
            this.FoundCountLabel.Location = new System.Drawing.Point(12, 3);
            this.FoundCountLabel.Name = "FoundCountLabel";
            this.FoundCountLabel.Size = new System.Drawing.Size(13, 13);
            this.FoundCountLabel.TabIndex = 0;
            this.FoundCountLabel.Text = "?";
            this.FoundCountLabel.Click += new System.EventHandler(this.FoundCountLabel_Click);
            // 
            // FrmMain
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(970, 647);
            this.Controls.Add(this.PanelFoldersAndFiles);
            this.Controls.Add(this.PanelSearchResults);
            this.Controls.Add(this.PanelLocationRootButton);
            this.Controls.Add(this.PanelTopMenuStartStopButtons);
            this.Controls.Add(this.MainStatusStrip);
            this.DoubleBuffered = true;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MainMenuStrip = this.MainWinMenuStrip;
            this.Name = "FrmMain";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Form1";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.FrmMain_FormClosing);
            this.Load += new System.EventHandler(this.FrmMain_Load);
            this.Resize += new System.EventHandler(this.FrmMain_Resize);
            this.MainWinMenuStrip.ResumeLayout(false);
            this.MainWinMenuStrip.PerformLayout();
            this.MainStatusStrip.ResumeLayout(false);
            this.MainStatusStrip.PerformLayout();
            this.PanelTopMenuStartStopButtons.ResumeLayout(false);
            this.PanelTopMenuStartStopButtons.PerformLayout();
            this.panel3.ResumeLayout(false);
            this.panel3.PerformLayout();
            this.FolderTreeMapPanel.ResumeLayout(false);
            this.FolderTreeMapPanel.PerformLayout();
            this.FileFolderContextMenuStrip.ResumeLayout(false);
            this.PanelLocationRootButton.ResumeLayout(false);
            this.FileFldrSplitContainer.Panel1.ResumeLayout(false);
            this.FileFldrSplitContainer.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.FileFldrSplitContainer)).EndInit();
            this.FileFldrSplitContainer.ResumeLayout(false);
            this.FileTreeMapPanel.ResumeLayout(false);
            this.FileTreeMapPanel.PerformLayout();
            this.PanelSearchResults.ResumeLayout(false);
            this.PanelSearchResults.PerformLayout();
            this.PanelFoldersAndFiles.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }
        #endregion

        private System.Windows.Forms.MenuStrip MainWinMenuStrip;
        private System.Windows.Forms.StatusStrip MainStatusStrip;
        private System.Windows.Forms.ToolStripStatusLabel ToolStatus;
        private System.Windows.Forms.ToolStripMenuItem MenuFile;
        private System.Windows.Forms.ToolStripMenuItem ExitToolStripMenuItem;
        private System.Windows.Forms.Button ButtonStartScan;
        private System.Windows.Forms.Panel PanelTopMenuStartStopButtons;
        private System.Windows.Forms.Button ButtonStopScan;
        private System.Windows.Forms.Panel FolderTreeMapPanel;
        private System.Windows.Forms.ToolStripMenuItem LblStartingDriveOrPath;
        private System.Windows.Forms.ToolStripComboBox CmbStartingDriveOrPath;
        private System.Windows.Forms.Button BtnPickFolder;
        private System.Windows.Forms.Label LblCurrentPath;
        private System.Windows.Forms.Panel PanelLocationRootButton;
        private System.Windows.Forms.Button BtnBackFolder;
        private System.Windows.Forms.Button BtnRoot;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label LblFolderHoverInfo;
        private System.Windows.Forms.SplitContainer FileFldrSplitContainer;
        private System.Windows.Forms.Panel FileTreeMapPanel;
        private System.Windows.Forms.Label LblFileHoverInfo;
        private System.Windows.Forms.Timer WatchTimer;
        private System.Windows.Forms.ContextMenuStrip FileFolderContextMenuStrip;
        private System.Windows.Forms.ToolStripMenuItem OpenLocationToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem DeleteToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem CopyInformationToolStripMenuItem;
        private ToolStripMenuItem AboutToolStripMenuItem;
        private CheckBox CkbAutoIOMonitor;
        private ToolStripMenuItem BackToolStripMenuItem;
        private ToolStripSeparator toolStripSeparator1;
        private ToolStripMenuItem MenuActions;
        private ToolStripMenuItem MenuActionsBack;
        private Panel panel3;
        private Panel PanelSearchResults;
        private Panel PanelFoldersAndFiles;
        private ToolStripMenuItem FindToolStripMenuItem;
        private Label FoundCountLabel;
    }
}

