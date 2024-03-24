using Microsoft.WindowsAPICodePack.Dialogs;
using SSMatters.src.models;
using SSMatters.src.services;
using SSMatters.src.utls;
using SwitchHelper;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Windows.Forms;

namespace SSMatters
{
    public partial class FrmMain : Form
    {
        [Flags]
        public enum CAPTURE_TYPE
        {
            Created = 0x1,
            Deleted = 0x2,
            Changed = 0x4,
            Renamed = 0x8,
            Error = 0xA,
            Duplicate = 0xB,
            Backedup = 0xC,
            Latest = 0xD,
        }

        private struct Queued_Data
        {
            public CAPTURE_TYPE CType;
            public string OldPath;
            public string NewPath;
        }

        delegate void StringBoolDelegate(string s, bool b);
        delegate void StringDelegate(string s);
        delegate void NoParamsDelegate();

        #region Private Readonly Properties
        private TreeMapGfx TDirMap { get; } = new TreeMapGfx(new STreeMapOptions() { MapType = STreeMapType.Folder });
        private TreeMapGfx TFileMap { get; } = new TreeMapGfx(new STreeMapOptions() { MapType = STreeMapType.File });
        #endregion

        #region Private Properties
        private object LastMOObject { get; set; } = null;
        private DateTime LblHoverDate { get; set; } = DateTime.MinValue;
        private string CurrentMouseOver { get; set; } = string.Empty;
        private Thread ThreadScan { get; set; } = new Thread(() => { });  //default empty thread.
        private Thread QueWatchThread { get; set; } = new Thread(() => { });
        #endregion

        #region Static Properties
        private static readonly object DupBlockerLock = new object();
        private static SysIODetails LoadData { get; set; } = new SysIODetails();
        private static bool AutoIOMonitor { get; set; }
        private static FileSystemWatcher FileWatcher { get; set; } = null;
        private static Queue FileQueue { get; set; } = new Queue();
        private static Dictionary<string, DateTime> DupBlocker { get; set; } = new Dictionary<string, DateTime>();
        #endregion

        #region Constructor
        public FrmMain()
        {
            InitializeComponent();
            this.Text = $"{About.AppTitle} v{About.ProductVersion.MajorMinor}";
        }
        #endregion

        #region Private Methods
        private void StartIOMonitor(string startPath)
        {
            //reset
            StopIOMonitor();
            //setup auto monitor flag
            AutoIOMonitor = true;
            //clear arrays
            DupBlocker = new Dictionary<string, DateTime>();
            FileQueue = new Queue();

            //starting new watcher
            FileWatcher = new FileSystemWatcher(startPath)
            {
                NotifyFilter = NotifyFilters.CreationTime
                                 | NotifyFilters.DirectoryName
                                 | NotifyFilters.FileName
                                 | NotifyFilters.LastWrite
                                 | NotifyFilters.Size
            };

            FileWatcher.Changed += OnChanged;
            FileWatcher.Created += OnCreated;
            FileWatcher.Deleted += OnDeleted;
            FileWatcher.Renamed += OnRenamed;

            FileWatcher.Filter = "*.*";
            FileWatcher.IncludeSubdirectories = true;
            FileWatcher.EnableRaisingEvents = true;
        }
        private void StopIOMonitor()
        {
            //stop auto monitor flag
            AutoIOMonitor = false;

            if (FileWatcher != null && !string.IsNullOrWhiteSpace(FileWatcher.Path))
            {
                FileWatcher.Dispose();
                Thread.Sleep(500);
            }

            if (LoadData.IsScanning)
            {
                //reset screen.
                FolderTreeMapPanel.Invalidate();
                FileTreeMapPanel.Invalidate();
            }
        }

        private void SetFile(string oldPath, CAPTURE_TYPE cType, string newPath = null)
        {
            lock (DupBlockerLock)
            {
                if (DupBlocker.ContainsKey(oldPath))
                {
                    if (DupBlocker.TryGetValue(oldPath, out DateTime dt))
                    {
                        if (dt > DateTime.UtcNow)
                            return;
                        else
                            DupBlocker.Remove(oldPath);
                    }
                }
                else
                    DupBlocker.Add(oldPath, DateTime.UtcNow.AddSeconds(10));

                //ignoring all rename directory changes.  That could get very large very quick.
                if (Directory.Exists(oldPath) && cType == CAPTURE_TYPE.Renamed)
                    return;

                bool changeMade = false;
                //lets add it to the queue
                Queued_Data _queuedData = new Queued_Data() { CType = cType, OldPath = oldPath, NewPath = newPath };
                FileQueue.Enqueue(_queuedData);

                if (!QueWatchThread.IsAlive)
                {
                    Debug.WriteLine($"Thread Not Alivt");
                    QueWatchThread = new Thread(() =>
                    {
                        Debug.WriteLine($"While Loop: {FileQueue.Count} && {AutoIOMonitor}");
                        while (FileQueue.Count != 0 && AutoIOMonitor)
                        {
                            if (!CkbAutoIOMonitor.Checked)
                                break;

                            Debug.WriteLine($"In While Loop: {FileQueue.Count}");
                            var scanFolderDetails = new FolderData();  //reset
                            Queued_Data queuedData = (Queued_Data)FileQueue.Dequeue();

                            string sPath = queuedData.OldPath;
                            string prevPath = sPath;

                            Debug.WriteLine($"Path set: {sPath}");

                            while (!Directory.Exists(sPath))
                            {
                                prevPath = sPath;
                                sPath = Path.GetDirectoryName(prevPath);
                            }

                            switch (queuedData.CType)
                            {
                                case CAPTURE_TYPE.Created:
                                    Debug.WriteLine($"In Loop: Created: {queuedData.OldPath}");
                                    if (Directory.Exists(queuedData.OldPath) || File.Exists(queuedData.OldPath))
                                    {
                                        if (LoadData.FindExisting(Path.GetDirectoryName(prevPath), out scanFolderDetails, true))
                                        {
                                            LoadData.LoadFolder(ref scanFolderDetails, true);
                                            changeMade = true;
                                        }
                                    }
                                    break;
                                case CAPTURE_TYPE.Deleted:
                                    Debug.WriteLine($"In Loop: Deleted: {sPath}");
                                    if (LoadData.FindExisting(sPath, out scanFolderDetails, true))
                                    {
                                        FileData fileData = scanFolderDetails.Files.Find(f => f.FullName == prevPath);
                                        if (fileData != null)
                                        {
                                            scanFolderDetails.Files.Remove(fileData);
                                            scanFolderDetails.TotalFileCount -= 1;
                                            changeMade = true;
                                        }
                                        else
                                        {
                                            FolderData folderData = scanFolderDetails.Directories.Find(f => f.FullPath == prevPath);
                                            if (folderData != null)
                                            {
                                                scanFolderDetails.Directories.Remove(folderData);
                                                scanFolderDetails.TotalFileCount -= folderData.TotalFileCount;
                                                scanFolderDetails.TotalDirectoryCount -= (folderData.TotalDirectoryCount + 1);
                                                changeMade = true;
                                            }
                                        }
                                    }
                                    break;
                                    //case CAPTURE_TYPE.Renamed:
                                    //    if (Directory.Exists(queuedData.OldPath))
                                    //        LoadData.RenameFolder(queuedData.OldPath);
                                    //    else
                                    //        LoadData.RenameFile(queuedData.OldPath);
                                    //    break;
                            }

                            if (DupBlocker.ContainsKey(queuedData.OldPath))
                                DupBlocker.Remove(queuedData.OldPath);

                            //wait 3 seconds to see if more files are added.
                            if (FileQueue.Count == 0)
                                Thread.Sleep(3000);
                        }

                        //clean up older files that might still exist.
                        var remItems = DupBlocker.Where(w => w.Value < DateTime.UtcNow).ToList();
                        foreach (var remItem in remItems)
                            DupBlocker.Remove(remItem.Key);

                        if (changeMade)
                            RefreshView();
                        return;
                    });
                    QueWatchThread.Start();
                }
            }
        }
        private void SetChecked(string cntrlName, bool isChecked)
        {
            if (InvokeRequired)
            {
                var d = new StringBoolDelegate(SetChecked);
                if (!Disposing && !IsDisposed)
                {
                    try { Invoke(d, cntrlName, isChecked); }
                    catch (ObjectDisposedException ex) { Debug.WriteLine(ex.Message); }
                    catch { }
                }
            }
            else if (!Disposing && !IsDisposed)
            {
                Control ctn = FindControlByName(this, cntrlName);
                if (ctn != null && ctn.GetType().Name == "CheckBox")
                    ((CheckBox)ctn).Checked = isChecked;
            }
        }
        private void SetEnable(string cntrlName, bool enable)
        {
            if (InvokeRequired)
            {
                var d = new StringBoolDelegate(SetEnable);
                if (!Disposing && !IsDisposed)
                {
                    try { Invoke(d, cntrlName, enable); }
                    catch (ObjectDisposedException ex) { Debug.WriteLine(ex.Message); }
                    catch { }
                }
            }
            else if (!Disposing && !IsDisposed)
            {
                Control ctn = FindControlByName(this, cntrlName);
                if (ctn != null)
                    ctn.Enabled = enable;
            }
        }
        private void SetVisible(string cntrlName, bool visible)
        {
            if (InvokeRequired)
            {
                var d = new StringBoolDelegate(SetVisible);
                if (!Disposing && !IsDisposed)
                {
                    try { Invoke(d, cntrlName, visible); }
                    catch (ObjectDisposedException ex) { Debug.WriteLine(ex.Message); }
                    catch { }
                }
            }
            else if (!Disposing && !IsDisposed)
            {
                Control ctn = FindControlByName(this, cntrlName);
                if (ctn != null)
                    ctn.Visible = visible;
            }
        }
        private void SetStatus(string status)
        {
            if (InvokeRequired)
            {
                var d = new StringDelegate(SetStatus);
                if (!Disposing && !IsDisposed)
                {
                    try { Invoke(d, status); }
                    catch (ObjectDisposedException ex) { Debug.WriteLine(ex.Message); }
                    catch { }
                }
            }
            else if (!Disposing && !IsDisposed)
            {
                ToolStripItem tsi = FindToolStripItemByName(MainStatusStrip, "ToolStatus");
                if (tsi != null)
                    tsi.Text = status;
                else
                    this.ToolStatus.Text = status;

                if (status.IndexOf("Scan completed in") > -1)
                    RefreshView();

                SetButtonStatus();
            }
        }
        private void RefreshView()
        {
            if (InvokeRequired)
            {
                var d = new NoParamsDelegate(RefreshView);
                if (!Disposing && !IsDisposed)
                {
                    try { Invoke(d); }
                    catch (ObjectDisposedException ex) { Debug.WriteLine(ex.Message); }
                    catch { }
                }
            }
            else if (!Disposing && !IsDisposed)
            {
                this.FileTreeMapPanel.Invalidate();
                this.FolderTreeMapPanel.Invalidate();
            }
        }
        private void StartScan(string path, bool startOver)
        {
            if ((!ThreadScan.IsAlive) && !string.IsNullOrWhiteSpace(path))
            {
                this.LblCurrentPath.Text = path;

                var dirInfo = new DirectoryInfo(path);
                if (dirInfo.Exists)
                {
                    ThreadScan = new Thread(() =>
                    {
                        DateTime dtStart = DateTime.Now;
                        bool success = LoadData.StartScan(path, startOver);
                        SetVisible("ButtonStopScan", false);
                        SetVisible("ButtonStartScan", true);
                        TimeSpan tsLength = DateTime.Now.Subtract(dtStart);

                        if (!success)
                        {
                            string msg = LoadData.LastError != null ?
                                            $"Scan failed - {LoadData.LastError.Message}" :
                                            "Unknown Exception Occured";

                            SetStatus(msg);
                            MessageBox.Show(msg, About.AppTitle, MessageBoxButtons.OK, MessageBoxIcon.Error);
                        }
                        else
                        {
                            SetStatus($"Scan completed in {Math.Round(tsLength.TotalSeconds,2)}s.\nFiles: {LoadData.CurrentScanFolderDetails.TotalFileCount:n0}, Folders: {LoadData.CurrentScanFolderDetails.TotalDirectoryCount:n0}\nTotal Byte Size: {LoadData.CurrentScanFolderDetails.TotalFileByteSize:n0}, Total Size: {Common.FormatByteSize(LoadData.CurrentScanFolderDetails.TotalFileByteSize)}");
                        }

                        return;
                    });

                    if (startOver)
                    {
                        LoadData.OrgScanFolderDetails = new FolderData();
                        LoadData.CurrentScanFolderDetails = new FolderData();
                    }

                    SetVisible("ButtonStartScan", false);
                    SetVisible("ButtonStopScan", true);
                    SetStatus("Scan started...");
                    ThreadScan.Start();
                }
                else
                {
                    MessageBox.Show($"{path} does not exist.", "oops", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }
        private void StopScan()
        {
            LoadData.StopScan();

            while (LoadData.IsScanning)
                Thread.Sleep(0);    //cycle cpu
        }
        private void GoBack()
        {
            if (LoadData.CurrentScanFolderDetails.FullPath != LoadData.OrgScanFolderDetails.FullPath)
            {
                string backPath = LoadData.CurrentScanFolderDetails.FullPath.Substring(0, LoadData.CurrentScanFolderDetails.FullPath.LastIndexOf("\\"));
                StartScan(backPath, false);
            }

            SetButtonStatus();
        }
        private void SetButtonStatus()
        {
            if (LoadData.CurrentScanFolderDetails.FullPath == LoadData.OrgScanFolderDetails.FullPath)
            {
                BtnBackFolder.Enabled = false;
                BtnRoot.Enabled = false;
            }
            else
            {
                BtnRoot.Enabled = true;
                BtnBackFolder.Enabled = true;
            }
        }
        private bool CleanPath(out string path)
        {
            bool retVal = false;

            path = string.Empty;
            path = this.CmbStartingDriveOrPath.Text.Trim();

            //clean up
            if (path.IndexOf("/") > -1)
                path = path.Replace("/", @"\");

            //if less than 3, exit.
            if (path.Length < 3)
            {
                //be nice, add slash if X:
                if (path.Length == 2 && path.Substring(1, 1) == ":")
                {
                    path += @"\";
                }
                else
                {
                    MessageBox.Show(@"Drive letter with colon and slash is required.  e.g. 'C:\'", About.AppTitle, MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return retVal;
                }
            }

            //return only drive letter.
            string driveLetter = path.Substring(0, 3);

            //check to see if drive exists.
            if (DriveInfo.GetDrives().ToList().Find(d => d.Name.ToLower() == driveLetter.ToLower()) == null)
            {
                MessageBox.Show($"Drive '{driveLetter}' does not exist.", About.AppTitle, MessageBoxButtons.OK, MessageBoxIcon.Error);
                return retVal;
            }

            if (path.Length > 3 && path.EndsWith(@"\"))
                path = path.Substring(0, path.Length - 1);

            //validate directory exists.
            if (path.Length > 3 && !Directory.Exists(path))
            {
                MessageBox.Show($"Directory '{path}' does not exist.", About.AppTitle, MessageBoxButtons.OK, MessageBoxIcon.Error);
                return retVal;
            }
            else
                retVal = true;

            this.CmbStartingDriveOrPath.Text = path;

            return retVal;
        }
        private void ShowHoverInfo(string info, PointF loc, bool isFolder)
        {
            if (loc == PointF.Empty)
            {
                LastMOObject = null;
                this.LblFolderHoverInfo.Hide();
                this.LblFileHoverInfo.Hide();
                if(isFolder)
                    this.FolderTreeMapPanel.Cursor = Cursors.Default;
                else
                    this.FileTreeMapPanel.Cursor = Cursors.Default;
            }
            else
            {
                if (isFolder)
                {
                    this.LblFolderHoverInfo.MaximumSize = Size.Empty;   //set default

                    if (!string.IsNullOrWhiteSpace(info))
                        this.LblFolderHoverInfo.Text = info;

                    this.LblFolderHoverInfo.Location = new Point((int)loc.X, (int)loc.Y);
                    if (this.LblFolderHoverInfo.Width > this.FolderTreeMapPanel.Width)
                    {
                        this.LblFolderHoverInfo.Left = 0;
                        this.LblFolderHoverInfo.MaximumSize = this.FolderTreeMapPanel.Size; //forces size within panel
                    }
                    else if (this.LblFolderHoverInfo.Right > this.FolderTreeMapPanel.Width)
                        this.LblFolderHoverInfo.Left = this.FolderTreeMapPanel.Width - this.LblFolderHoverInfo.Width;

                    if (this.LblFolderHoverInfo.Top < this.FolderTreeMapPanel.Top)
                        this.LblFolderHoverInfo.Top = this.FolderTreeMapPanel.Top;
                    else if (this.LblFolderHoverInfo.Bottom > this.FolderTreeMapPanel.Bottom)
                        this.LblFolderHoverInfo.Top = this.FolderTreeMapPanel.Height - this.LblFolderHoverInfo.Height;
                }
                else
                {
                    this.LblFileHoverInfo.MaximumSize = Size.Empty;   //set default

                    loc.X += this.FileTreeMapPanel.Left;
                    if (!string.IsNullOrWhiteSpace(info))
                        this.LblFileHoverInfo.Text = info;

                    this.LblFileHoverInfo.Location = new Point((int)loc.X, (int)loc.Y);
                    if (this.LblFileHoverInfo.Width > this.FileTreeMapPanel.Width)
                    {
                        this.LblFileHoverInfo.Left = 0;
                        this.LblFileHoverInfo.MaximumSize = this.FileTreeMapPanel.Size; //forces size within panel
                    }
                    else if (this.LblFileHoverInfo.Right > this.FileTreeMapPanel.Width)
                        this.LblFileHoverInfo.Left = this.FileTreeMapPanel.Width - this.LblFileHoverInfo.Width;

                    if (this.LblFileHoverInfo.Top < this.FileTreeMapPanel.Top)
                        this.LblFileHoverInfo.Top = this.FileTreeMapPanel.Top;
                    else if (this.LblFileHoverInfo.Bottom > this.FileTreeMapPanel.Bottom)
                        this.LblFileHoverInfo.Top = this.FileTreeMapPanel.Height - this.LblFileHoverInfo.Height;
                }

                this.LblFolderHoverInfo.Visible = isFolder;
                this.LblFileHoverInfo.Visible = !isFolder;

                this.LblHoverDate = DateTime.Now.AddSeconds(10);
            }
        }
        /// <summary>
        /// Only for controls, not ToolStripItems
        /// </summary>
        /// <param name="parent"></param>
        /// <param name="cntrlName"></param>
        /// <returns></returns>
        private Control FindControlByName(Control parent, string cntrlName)
        {
            foreach (Control ctrl in parent.Controls)
            {
                if (ctrl.Name == cntrlName)
                    return ctrl;
            }

            foreach (Control ctrl in parent.Controls)
            {
                Control retVal = FindControlByName(ctrl, cntrlName);
                if (retVal != null)
                    return retVal;
            }

            return null;
        }
        /// <summary>
        /// Only for controls, not ToolStripItems
        /// </summary>
        /// <param name="parent"></param>
        /// <param name="cntrlName"></param>
        /// <returns></returns>
        private ToolStripItem FindToolStripItemByName(ToolStrip parent, string tsiName)
        {
            ToolStripItem retVal = parent.Items[tsiName];
            if (retVal != null)
                return retVal;

            foreach (ToolStripItem tsi in parent.Items)
            {
                if (tsi.Name == tsiName)
                    return tsi;
            }

            foreach (ToolStrip ctrl in parent.Items)
            {
                retVal = FindToolStripItemByName(ctrl, tsiName);
                if (retVal != null)
                    return retVal;
            }

            return null;
        }
        #endregion

        #region Private API Event Methods
        private void OnChanged(object sender, FileSystemEventArgs e)
        {
            if (e.ChangeType != WatcherChangeTypes.Changed)
                return;

            Debug.WriteLine($"CHANGED: {e.FullPath}");
            SetFile(e.FullPath, CAPTURE_TYPE.Changed);
        }
        private void OnCreated(object sender, FileSystemEventArgs e)
        {
            if (e.ChangeType != WatcherChangeTypes.Created)
                return;

            Debug.WriteLine($"CREATED: {e.FullPath}");
            SetFile(e.FullPath, CAPTURE_TYPE.Created);
        }
        private void OnDeleted(object sender, FileSystemEventArgs e)
        {
            if (e.ChangeType != WatcherChangeTypes.Deleted)
                return;

            Debug.WriteLine($"DELETED: {e.FullPath}");
            SetFile(e.FullPath, CAPTURE_TYPE.Deleted);
        }
        private void OnRenamed(object sender, RenamedEventArgs e)
        {
            if (e.ChangeType != WatcherChangeTypes.Renamed)
                return;

            Debug.WriteLine($"RENAMED: {Path.GetFileName(e.OldFullPath)}\t->\t{Path.GetFileName(e.FullPath)}");
            SetFile(e.OldFullPath, CAPTURE_TYPE.Renamed, e.FullPath);
        }
        #endregion

        #region Private Control Events
        private void FrmMain_Load(object sender, EventArgs e)
        {
            this.CmbStartingDriveOrPath.Items.Clear();

            if (this.CmbStartingDriveOrPath.Items.Count == 0)
            {
                foreach(DriveInfo drive in DriveInfo.GetDrives())
                    this.CmbStartingDriveOrPath.Items.Add($"{drive.Name}");
            }

            this.CmbStartingDriveOrPath.Text = this.CmbStartingDriveOrPath.Items[0].ToString();
        }
        private void FrmMain_Resize(object sender, EventArgs e)
        {
            RefreshView();
        }
        private void FrmMain_FormClosing(object sender, FormClosingEventArgs e)
        {
            StopIOMonitor();
            StopScan();
        }
        private void FolderTreeMapPanel_Paint(object sender, PaintEventArgs e)
        {
            Graphics gfx = e.Graphics;
            gfx.Clear(Color.Gray);

            if (!CkbAutoIOMonitor.Checked && LoadData.IsScanning)
                return;

            if (LoadData.CurrentScanFolderDetails.TotalFileByteSize <= 0)
                return;

            TDirMap.DrawView(gfx, LoadData.CurrentScanFolderDetails, this.FolderTreeMapPanel.Size, true);
        }
        private void FileTreeMapPanel_Paint(object sender, PaintEventArgs e)
        {
            Graphics gfx = e.Graphics;
            gfx.Clear(Color.Gray);

            if (!CkbAutoIOMonitor.Checked && LoadData.IsScanning)
                return;

            if (LoadData.CurrentScanFolderDetails.TotalFileByteSize <= 0)
                return;

            TFileMap.DrawView(gfx, LoadData.CurrentScanFolderDetails, this.FileTreeMapPanel.Size, true);
        }
        private void FileFldrSplitContainer_SplitterMoved(object sender, SplitterEventArgs e)
        {
            RefreshView();
        }
        private void ButtonStartScan_Click(object sender, EventArgs e)
        {
            bool wasChecked = CkbAutoIOMonitor.Checked;

            SetEnable("CkbAutoIOMonitor", false);
            SetChecked("CkbAutoIOMonitor", false);

            StopIOMonitor();
            StopScan();

            if (!CleanPath(out string path))
                return;

            //reset all data
            LoadData = new SysIODetails();
            //start the scan process.
            StartScan(path, true);

            //reset screen.
            FolderTreeMapPanel.Invalidate();
            FileTreeMapPanel.Invalidate();

            //setup thread to monitor scanning.
            Thread th = new Thread(() => {
                Thread.Sleep(500);

                while (LoadData.IsScanning)
                {
                    //display info in status bar.
                    SetStatus($"Scan in progres...\nFiles: {LoadData.ScanStatus.FileCount:n0}, Folders: {LoadData.ScanStatus.DirectoryCount:n0}\nTotal Byte Size: {LoadData.ScanStatus.TotalFileSize:n0}, Total Size: {Common.FormatByteSize(LoadData.ScanStatus.TotalFileSize)}");
                    Thread.Sleep(500);
                }

                if (this.CkbAutoIOMonitor.Checked)
                    StartIOMonitor(path);

                SetEnable("CkbAutoIOMonitor", true);
                SetChecked("CkbAutoIOMonitor", wasChecked);
                return;
            });

            th.Start();
        }
        private void ButtonStopScan_Click(object sender, EventArgs e)
        {
            StopScan();
        }
        private MouseEventArgs BuildNewMouseEvent(object sender, MouseEventArgs e)
        {
            Point p;
            //setup as switch case for future possibilities.
            switch (sender.GetType().Name.ToLower())
            {
                case "label":
                default:
                    p = new Point(((Label)sender).Left + e.Location.X, ((Label)sender).Top + e.Location.Y);
                    break;
            }
            return new MouseEventArgs(e.Button, e.Clicks, p.X, p.Y, e.Delta);
        }
        private void LblHoverInfo_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            Label lbl = sender as Label;
            if (lbl.Name.Contains("Folder"))
                FolderTreeMapPanel_MouseDoubleClick(this.FolderTreeMapPanel, BuildNewMouseEvent(sender, e));
            else
                FileTreeMapPanel_MouseDoubleClick(this.FileTreeMapPanel, BuildNewMouseEvent(sender, e));
        }
        private void LblFolderHoverInfo_MouseMove(object sender, MouseEventArgs e)
        {
            TreeMapPanel_MouseMove(this.FolderTreeMapPanel, BuildNewMouseEvent(sender, e));
        }
        private void LblFileHoverInfo_MouseMove(object sender, MouseEventArgs e)
        {
            TreeMapPanel_MouseMove(this.FileTreeMapPanel, BuildNewMouseEvent(sender, e));
        }
        private void TreeMapPanel_MouseMove(object sender, MouseEventArgs e)
        {
            Point spacing = new Point(10, 30);
            bool isFolder = (sender as Panel).Name == "FolderTreeMapPanel";
            StringBuilder sb = new StringBuilder();
            PointF point;

            var newLastMOObject = LoadData.GetPathByPointer(e.Location, isFolder);

            if (isFolder)
            {
                if (newLastMOObject != null && newLastMOObject != LastMOObject)
                {

                    this.FolderTreeMapPanel.Cursor = Cursors.Hand;
                    LastMOObject = newLastMOObject;
                    FolderData folder = LastMOObject as FolderData;
                    this.CurrentMouseOver = folder.FullPath;

                    point = new PointF(folder.Rectangle.Location.X + spacing.X, folder.Rectangle.Location.Y + spacing.Y);

                    if (folder.Valid)
                    {
                        sb.AppendLine($"Directory: {folder.DirectoryName}");
                        sb.AppendLine($"Created: {folder.CreatedDate}");
                        sb.AppendLine($"Modified: {folder.ModifiedDate}");
                        sb.AppendLine($"Size: {folder.TotalFileByteSize:n0}b ({Common.FormatByteSize(folder.TotalFileByteSize)})");
                        sb.AppendLine($"Files: {folder.TotalFileCount:n0}");
                        sb.AppendLine($"Folders: {folder.TotalDirectoryCount:n0}");
                    }
                    else
                        sb.AppendLine($"Error: {folder.Info}");

                    ShowHoverInfo(sb.ToString(), point, true);
                    this.Cursor = Cursors.Hand;
                }
                else if (newLastMOObject == null)
                {
                    ShowHoverInfo(string.Empty, PointF.Empty, isFolder);
                    //this.Cursor = Cursors.Default;
                }
            }
            else 
            {
                if (newLastMOObject != null && newLastMOObject != LastMOObject)
                {
                    this.FileTreeMapPanel.Cursor = Cursors.Hand;
                    LastMOObject = newLastMOObject;
                    FileData file = LastMOObject as FileData;
                    this.CurrentMouseOver = file.FullName;

                    point = new PointF(file.Rectangle.Location.X + spacing.X, file.Rectangle.Location.Y + spacing.Y);

                    if (file.Valid)
                    {
                        sb.AppendLine($"File: {file.FileName}");
                        sb.AppendLine($"Created: {file.CreatedDate}");
                        sb.AppendLine($"Modified: {file.ModifiedDate}");
                        sb.AppendLine($"Size: {file.TotalByteSize:n0}b ({Common.FormatByteSize(file.TotalByteSize)})");
                    }
                    else
                        sb.AppendLine($"Error: {file.Info}");

                    ShowHoverInfo(sb.ToString(), point, false);
                }
                else if (newLastMOObject == null)
                {
                    ShowHoverInfo(string.Empty, PointF.Empty, isFolder);
                    //this.Cursor = Cursors.Default;
                }
            }
        }
        private void FileTreeMapPanel_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            if (ButtonStartScan.Enabled && !string.IsNullOrWhiteSpace(this.CurrentMouseOver) && File.Exists(this.CurrentMouseOver))
            {
                if (!LoadData.OpenFile(this.CurrentMouseOver))
                    MessageBox.Show($"Failed to execute {this.CurrentMouseOver}.", About.AppTitle, MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        private void FolderTreeMapPanel_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            if (ButtonStartScan.Enabled && !string.IsNullOrWhiteSpace(this.CurrentMouseOver) && Directory.Exists(this.CurrentMouseOver))
            {
                StartScan(this.CurrentMouseOver, false);
                SetButtonStatus();
            }
        }
        private void BtnRoot_Click(object sender, EventArgs e)
        {
            StartScan(LoadData.OrgScanFolderDetails.FullPath, false);
            SetButtonStatus();
        }
        private void BtnBackFolder_Click(object sender, EventArgs e)
        {
            GoBack();
        }
        private void WatchTimer_Tick(object sender, EventArgs e)
        {
            if (LblHoverDate < DateTime.Now && (this.LblFileHoverInfo.Visible || this.LblFolderHoverInfo.Visible))
            {
                ShowHoverInfo(string.Empty, PointF.Empty, false);
                ShowHoverInfo(string.Empty, PointF.Empty, true);
            }
        }
        private void FileFolderContextMenuStrip_Opening(object sender, CancelEventArgs e)
        {
            if (LastMOObject == null || LastMOObject?.GetType()?.Name == null)
                e.Cancel = true;
        }
        private void CopyInformationToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (LastMOObject == null || LastMOObject?.GetType()?.Name == null)
                return;

            StringBuilder sb = new StringBuilder();
            bool isFolder = LastMOObject.GetType().Name == "FolderData";

            if(isFolder)
            {
                FolderData folder = LastMOObject as FolderData;
                if (folder.Valid)
                {
                    sb.AppendLine($"Name: {folder.DirectoryName}");
                    sb.AppendLine($"Path: {folder.FullPath}");
                    sb.AppendLine($"Created: {folder.CreatedDate}");
                    sb.AppendLine($"Modified: {folder.ModifiedDate}");
                    sb.AppendLine($"Size: {folder.TotalFileByteSize:n0}b ({Common.FormatByteSize(folder.TotalFileByteSize)})");
                    sb.AppendLine($"Files: {folder.TotalFileCount}");
                    sb.AppendLine($"Folders: {folder.TotalDirectoryCount}");
                }
                else
                    sb.AppendLine($"Error: {folder.Info}");

                Clipboard.Clear();
                Clipboard.SetText(sb.ToString());
                SetStatus($"{folder.DirectoryName} info has been copied to clipboard.");
            }
            else
            {
                FileData file = LastMOObject as FileData;
                if (file.Valid)
                {
                    sb.AppendLine($"Name: {file.FileName}");
                    sb.AppendLine($"Path: {file.FullName}");
                    sb.AppendLine($"Created: {file.CreatedDate}");
                    sb.AppendLine($"Modified: {file.ModifiedDate}");
                    sb.AppendLine($"Size: {file.TotalByteSize:n0}b ({Common.FormatByteSize(file.TotalByteSize)})");
                }
                else
                    sb.AppendLine($"Error: {file.Info}");

                Clipboard.Clear();
                Clipboard.SetText(sb.ToString());
                SetStatus($"{file.FileName} info has been copied to clipboard.");
            }
        }
        private void OpenLocationToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (LastMOObject == null)
                return;

            bool isFolder = LastMOObject.GetType().Name == "FolderData";
            if (isFolder)
            {
                FolderData folder = LastMOObject as FolderData;
                Process.Start(folder.FullPath);
            }
            else
            {
                FileData file = LastMOObject as FileData;
                Process.Start(Path.GetDirectoryName(file.FullName));
            }
        }
        private void DeleteToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (LastMOObject == null)
                return;

            string resMsg, resTitle;
            bool isFolder = LastMOObject.GetType().Name == "FolderData";
            if (isFolder)
            {
                FolderData folder = LastMOObject as FolderData;
                if (MessageBox.Show($"Are you sure you want to delete\n'{folder.FullPath}'", "Careful", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.No)
                    return;

                try
                {
                    Directory.Delete(folder.FullPath, true);
                    if(LoadData.DeleteFolder(folder.FullPath))
                    {
                        string dir = folder.FullPath.Substring(0, folder.FullPath.LastIndexOf('\\'));
                        StartScan(dir, false);
                    }
                    resTitle = "Success";
                    resMsg = $"Succeefully deleted, '{folder.FullPath}'";
                }
                catch (IOException ex)
                {
                    resTitle = "IO Exception";
                    resMsg = ex.Message;
                }
                catch (AccessViolationException ex)
                {
                    resTitle = "Access Violation";
                    resMsg = ex.Message;
                }
                catch (Exception ex)
                {
                    resTitle = "General Exception";
                    resMsg = ex.Message;
                }
            }
            else
            {
                FileData file = LastMOObject as FileData;
                if (MessageBox.Show($"Are you sure you want to delete\n'{file.FullName}'", "Careful", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.No)
                    return;

                try
                {
                    //File.Delete(file.FullName);
                    if (LoadData.DeleteFile(file.FullName))
                    {
                        string dir = Path.GetDirectoryName(file.FullName);
                        StartScan(dir, false);
                    }
                    resTitle = "Success";
                    resMsg = $"Succeefully deleted, '{file.FullName}'";
                }
                catch(IOException ex)
                {
                    resTitle = "IO Exception";
                    resMsg = ex.Message;
                }
                catch (AccessViolationException ex)
                {
                    resTitle = "Access Violation";
                    resMsg = ex.Message;
                }
                catch (Exception ex)
                {
                    resTitle = "General Exception";
                    resMsg = ex.Message;
                }
            }

            MessageBox.Show(resMsg, resTitle, MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
        private void Default_Cursor(object sender, MouseEventArgs e)
        {
            this.Cursor = Cursors.Default;
        }
        private void BtnPickFolder_Click(object sender, EventArgs e)
        {
            string retVal = this.CmbStartingDriveOrPath.Text.Trim();
            if (Common.FolderPicker(ref retVal, "Pick a root search folder.") == CommonFileDialogResult.Ok)
                this.CmbStartingDriveOrPath.Text = retVal;
        }
        private void ExitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.Close();
        }
        private void AboutToolStripMenuItem_Click(object sender, EventArgs e)
        {
#if DEBUG
            string buildType = "(DEBUG)";
#else
            string buildType = "(RELEASE)";
#endif 
            MessageBox.Show($"{About.AppTitle} {buildType}\nProduct Version: {About.ProductVersion.Full}\nFile Version: {About.FileVersion.Full}", About.AppTitle, MessageBoxButtons.OK, MessageBoxIcon.Information);
        }
        private void CkbAutoIOMonitor_CheckedChanged(object sender, EventArgs e)
        {
            if (this.CkbAutoIOMonitor.Checked)
            {
                if (!CleanPath(out string path))
                    return;

                StartIOMonitor(path);
            }
            else
                StopIOMonitor();
        }
        private void BackToolStripMenuItem_Click(object sender, EventArgs e)
        {
            GoBack();
        }
        private void MenuActionsBack_Click(object sender, EventArgs e)
        {
            GoBack();
        }
        #endregion
    }
}
