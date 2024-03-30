using SSMatters.src.models;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using TextLogManager;

namespace SSMatters.src.services
{
    internal enum Find_Type
    {
        Add,
        Rename,
        Delete,
        FindOnly
    }
    internal class SysIODetails
    {
        #region Readonly variables
        public static readonly List<DriveInfo> DriveList = DriveInfo.GetDrives().ToList();
        private readonly LogMgr logger = new LogMgr();
        #endregion

        #region Private Properties
        private bool ForceStop { get; set; } = true;
        #endregion

        #region Private Mehtods
        public double LoadFiles(ref FolderData thisFolder, FileInfo[] allFiles, bool isFind)
        {
            if (string.IsNullOrWhiteSpace(thisFolder.FullPath))
                return 0;

            thisFolder.TotalByteSize = allFiles.Sum(file => file.Length);
            foreach (var file in allFiles)
            {
                if (this.ForceStop)
                    break;

                FileData fileInfoData = new FileData();
                try
                {
                    fileInfoData = new FileData(file);
                }
                catch (Exception ex)
                {
                    if(ex.Message.IndexOf("denied")>-1)
                        logger.WriteLine(ex.Message);
                    else
                        logger.WriteLine(ex, $"While calling FileData() on: '{file}'.");

                    this.LastError = ex;
                    fileInfoData = new FileData
                    {
                        FullName = file.FullName,
                        Info = ex.Message
                    };
                }
                finally
                {
                    if (!thisFolder.Files.Contains(fileInfoData))
                    {
                        if (isFind)
                        {
                            //double check, can have dups if not validated because of file size updates, etc.
                            //this process slows down the add, which is why it's setup only for FindFileData()->Add calls.
                            if (thisFolder.Files.Find(f => f.FullName == file.FullName) == null)
                                thisFolder.Files.Add(fileInfoData);
                        }
                        else
                            thisFolder.Files.Add(fileInfoData);
                    }
                }
            }
            return thisFolder.TotalByteSize;
        }
        public long LoadFolder(ref FolderData thisFolder, bool isFind)
        {
            if (string.IsNullOrWhiteSpace(thisFolder.FullPath))
                return 0;

            ScanStatus.DirectoryCount++;
            var dirInfo = new DirectoryInfo(thisFolder.FullPath);
            thisFolder.DirectoryName = dirInfo.Name;
            thisFolder.CreatedDate = dirInfo.CreationTime;
            thisFolder.ModifiedDate = dirInfo.LastWriteTime;
            FileInfo[] allFiles;

            try
            {
                //this will thow an exception if access denied
                IEnumerable<FileInfo> files = dirInfo.EnumerateFiles();
                //files received, but some files, not may be accessible,
                //lets pull files with size of 0 or better.
                allFiles = files?.Where(f => (f?.Length >= 0)).ToArray();
            }
            catch (Exception ex)
            {
                if (ex.Message.IndexOf("denied") > -1)
                    logger.WriteLine(ex);
                else
                    logger.WriteLine(ex, $"While calling EnumerateFiles() for '{thisFolder.DirectoryName}'.");

                this.LastError = ex;
                thisFolder.Info = ex.Message;
                thisFolder.Valid = false;
                return 0;
            }

            thisFolder.TotalByteSize = LoadFiles(ref thisFolder, allFiles, isFind);
            thisFolder.TotalFileByteSize = thisFolder.TotalByteSize;
            thisFolder.TotalFileCount = allFiles.Count();

            ScanStatus.FileCount += allFiles.Count();
            ScanStatus.TotalFileSize += thisFolder.TotalByteSize;

            var dirListing = dirInfo.EnumerateDirectories();

            foreach (var dir in dirListing)
            {
                if (this.ForceStop)
                    break;

                var localFolderData = new FolderData(dir.FullName);
                thisFolder.TotalDirectoryCount++;
                thisFolder.TotalDirectoryCount += LoadFolder(ref localFolderData, isFind);
                thisFolder.TotalFileByteSize += localFolderData.TotalFileByteSize;
                thisFolder.TotalFileCount += localFolderData.TotalFileCount;

                if (!thisFolder.Directories.Contains(localFolderData))
                {
                    if (isFind)
                    {
                        //double check, can have dups if not validated because of file size updates, etc.
                        //this process slows down the add, which is why it's setup only for FindFileData()->Add calls.
                        if (thisFolder.Directories.Find(d => d.FullPath == dir.FullName) == null)
                            thisFolder.Directories.Add(localFolderData);
                    }
                    else
                        thisFolder.Directories.Add(localFolderData);
                }
            }

            return thisFolder.TotalDirectoryCount;
        }
        private FolderData FindFolderData(string startFolder, FolderData folderData, Find_Type fType = Find_Type.FindOnly)
        {
            if (string.IsNullOrWhiteSpace(folderData.FullPath))
                return null;

            if (folderData.FullPath == startFolder || ForceStop)
                return folderData;

            foreach (FolderData f in folderData.Directories)
            {
                if (f.FullPath.ToLower().Equals(startFolder.ToLower()))
                {
                    if (fType == Find_Type.Delete)
                    {
                        bool success = folderData.Directories.Remove(f);
                        folderData.TotalByteSize = folderData.Files.Sum(d => d.TotalByteSize);
                        folderData.TotalFileByteSize = folderData.Directories.Sum(d => d.TotalFileByteSize) + folderData.TotalByteSize - f.TotalFileByteSize;
                        Debug.WriteLine($"{f.DirectoryName} was removed: {success}");
                    }
                    return f;
                }

                if (fType == Find_Type.Add)
                {
                    if (Path.GetDirectoryName(startFolder) == f.FullPath || startFolder == f.FullPath)
                    {
                        try
                        {
                            FolderData fd = f;
                            //load files and folders.
                            f.Directories.Clear();
                            LoadFolder(ref fd, true);
                            folderData.TotalByteSize = folderData.Files.Sum(d => d.TotalByteSize);
                            folderData.TotalFileByteSize = folderData.Directories.Sum(d => d.TotalFileByteSize) + folderData.TotalByteSize;
                            return f;
                        }
                        catch { }

                    }
                }

                //drill into sub folders.
                FolderData subF = FindFolderData(startFolder, f, fType);
                if (subF != null)
                {
                    if (fType == Find_Type.Delete)
                    {
                        //might of been deleted from the above "FindFolderData" call, so lets check.
                        if (f.Directories.Contains(subF))
                        {
                            bool success = f.Directories.Remove(subF);
                            folderData.TotalByteSize = folderData.Files.Sum(d => d.TotalByteSize);
                            folderData.TotalFileByteSize = folderData.Directories.Sum(d => d.TotalFileByteSize) + folderData.TotalByteSize - subF.TotalFileByteSize;
                            Debug.WriteLine($"{subF.DirectoryName} was removed: {success}");
                        }
                    }
                    return subF;
                }
            }

            return null;
        }
        private FolderData FindFileData(string findFile, FolderData folderData, Find_Type fType = Find_Type.FindOnly)
        {
            if (string.IsNullOrWhiteSpace(folderData.FullPath))
                return null;

            if (folderData.Files.Find(e => e.FullName == findFile) != null || ForceStop)
                return folderData;

            foreach (FolderData f in folderData.Directories)
            {
                FileData file = folderData.Files.Find(e => e.FullName == findFile);
                if (file != null)
                {
                    if (fType == Find_Type.Delete)
                    {
                        bool success = folderData.Files.Remove(file);
                        folderData.TotalByteSize = folderData.Files.Sum(d => d.TotalByteSize) - file.TotalByteSize;
                        folderData.TotalFileByteSize = folderData.Directories.Sum(d => d.TotalFileByteSize) + folderData.TotalByteSize;
                        Debug.WriteLine($"{file.FileName} was removed: {success}");
                    }
                    return f;
                }

                if(fType == Find_Type.Add)
                {
                    if(Path.GetDirectoryName(findFile) == f.FullPath)
                    {
                        try
                        {
                            FileInfo[] allFiles;
                            FolderData fd = f;

                            var dirInfo = new DirectoryInfo(f.FullPath);
                            //this will thow an exception if access denied
                            IEnumerable<FileInfo> files = dirInfo.EnumerateFiles();
                            //files received, but some files, not may be accessible,
                            //lets pull files with size of 0 or better.
                            allFiles = files?.Where(fs => (fs?.Length >= 0)).ToArray();
                            //load files.
                            f.Files.Clear();
                            LoadFiles(ref fd, allFiles, true);
                            folderData.TotalByteSize = folderData.Files.Sum(d => d.TotalByteSize);
                            folderData.TotalFileByteSize = folderData.Directories.Sum(d => d.TotalFileByteSize) + folderData.TotalByteSize;
                            //f.Files.AddRange(fd.Files);
                            return f;
                        }
                        catch { }
                    }
                }

                //drill into sub folders.
                FolderData subF = FindFileData(findFile, f, fType);
                if (subF != null)
                {
                    FileData subFile = subF.Files.Find(e => e.FullName == findFile);
                    if (fType == Find_Type.Delete && subFile != null)
                    {
                        bool success = subF.Files.Remove(subFile);
                        folderData.TotalByteSize = folderData.Files.Sum(d => d.TotalByteSize);
                        folderData.TotalFileByteSize = folderData.Directories.Sum(d => d.TotalFileByteSize) + folderData.TotalByteSize;
                        Debug.WriteLine($"{subFile.FileName} was removed: {success}");
                    }
                    return subF;
                }
            }

            return null;
        }
        #endregion

        #region Public Properties
        public FolderData CurrentScanFolderDetails = new FolderData();
        public FolderData OrgScanFolderDetails { get; set; } = new FolderData();
        public Exception LastError { get; private set; }
        public bool IsScanning { get; private set; }
        public ScanTotal ScanStatus { get; private set; } = new ScanTotal();
        #endregion

        #region Publc Mehtods
        public List<string> SearchList(FolderData root, string searchFor, List<string> found = null)
        {
            if(found == null)
                found = new List<string>();

            foreach (var file in root.Files)
            {
                if (file.FileName.Contains(searchFor))
                    found.Add(file.FullName);
            }

            foreach (var dir in root.Directories)
            {
                if (dir.DirectoryName.Contains(searchFor))
                    found.Add(dir.FullPath);

                found = SearchList(dir, searchFor, found);
            }

            return found;
        }
        public void StopScan()
        {
            this.ForceStop = true;
        }
        public bool OpenFile(string filePath)
        {
            bool retVal = true;
            try { Process p = Process.Start(filePath); } catch { retVal = false; }
            return retVal;
        }
        public bool FindExisting(string startFolder, out FolderData scanFolderDetails, bool searchOnly = false)
        {
            bool retVal;

            if (startFolder.IndexOf("\\") == -1 && startFolder.Length == 2)
                startFolder += "\\";        //drive
            
            scanFolderDetails = FindFolderData(startFolder, OrgScanFolderDetails);

            if (scanFolderDetails != null)
            {
                if(!searchOnly)
                    CurrentScanFolderDetails = scanFolderDetails;

                logger.WriteLine($"Ending directory scan of: '{startFolder}' with {scanFolderDetails.TotalDirectoryCount} folder and {scanFolderDetails.TotalFileCount} files.");
                retVal = true;
            }
            else
            {
                logger.WriteLine($"Ending directory scan of: '{startFolder}' was not found.");
                retVal = false;
            }

            return retVal;
        }
        public bool StartScan(string startFolder, bool startOver = false)
        {
            bool retVal = false;
            this.IsScanning = false; //incase something is stuck in true.
            string msg = string.Empty;

            if (startOver)
            {
                CurrentScanFolderDetails = new FolderData();  //reset
                OrgScanFolderDetails = new FolderData();
            }

            var scanFolderDetails = new FolderData();  //reset
            ScanStatus = new ScanTotal();

            startFolder = startFolder?.Trim();

            if (!string.IsNullOrEmpty(startFolder) && startFolder.Substring(1, 1) == ":")
            {
                DriveInfo di = DriveList.Find(d => d.Name.Substring(0, 2).ToLower() == startFolder.Substring(0, 2).ToLower());
                if (di == null)
                {
                    msg = $"Directory '{startFolder}' is missing a drive.";
                    LastError = new Exception(msg);
                    logger.WriteLine(LastError);
                    return false;
                }
            }
            else
            {
                msg = $"'{startFolder}' drive missing from path.";
                LastError = new Exception(msg);
                logger.WriteLine(LastError);
                return false;
            }

            this.ForceStop = false;
            this.IsScanning = true;

            if (!startOver && OrgScanFolderDetails.Valid)
            {
                retVal = FindExisting(startFolder, out scanFolderDetails);
            }
            else
            {
                try
                {
                    //scanFolderDetails.FullPath = startFolder;
                    CurrentScanFolderDetails.FullPath = startFolder;
                    logger.WriteLine($"Starting directory scan: '{startFolder}'.");

                    LoadFolder(ref CurrentScanFolderDetails, false);

                    //CurrentScanFolderDetails = OrgScanFolderDetails;
                    OrgScanFolderDetails = CurrentScanFolderDetails;

                    logger.WriteLine($"Ending directory scan of: '{startFolder}' with {CurrentScanFolderDetails.TotalDirectoryCount} folder and {CurrentScanFolderDetails.TotalFileCount} files.");

                    //clear scan status
                    ScanStatus = new ScanTotal();
                    retVal = true;
                }
                catch (Exception ex)
                {
                    logger.WriteLine(ex, $"While running scans on root: '{CurrentScanFolderDetails.FullPath}'.");
                    scanFolderDetails.Info = ex.Message;
                    scanFolderDetails.Valid = false;
                    this.LastError = ex;
                }
            }

            this.IsScanning = false;
            return retVal;
        }
        public bool DeleteFile(string fullFilePath)
        {
            bool retVal = true;
            if (OrgScanFolderDetails == null)
                return retVal;

            FolderData fd = FindFileData(fullFilePath, OrgScanFolderDetails, Find_Type.Delete);
            retVal = fd != null;

            return retVal;
        }
        public bool DeleteFolder(string fullFolderPath)
        {
            bool retVal = true;
            if (OrgScanFolderDetails == null)
                return retVal;

            FolderData fd = FindFolderData(fullFolderPath, OrgScanFolderDetails, Find_Type.Delete);
            retVal = fd != null;

            return retVal;
        }
        public bool AddFile(string fullFilePath)
        {
            bool retVal = true;
            if (OrgScanFolderDetails == null)
                return retVal;

            FolderData fd = FindFileData(fullFilePath, OrgScanFolderDetails, Find_Type.Add);
            retVal = fd != null;

            return retVal;
        }
        public bool AddFolder(string fullFolderPath)
        {
            bool retVal = true;
            if (OrgScanFolderDetails == null)
                return retVal;

            FolderData fd = FindFolderData(fullFolderPath, OrgScanFolderDetails, Find_Type.Add);
            retVal = fd != null;

            return retVal;
        }
        public object GetPathByPointer(Point point, bool isFolder)
        {
            if (isFolder)
            {
                FolderData fd = CurrentScanFolderDetails.Directories
                    .Find(e => e.Rectangle
                    .IntersectsWith(new Rectangle(point, new Size(1, 1))));

                if (fd != null)
                    return fd;

            }
            else
            {
                FileData fid = CurrentScanFolderDetails.Files
                    .Find(e => e.Rectangle
                    .IntersectsWith(new Rectangle(point, new Size(1, 1))));

                if (fid != null)
                    return fid;
            }

            return null;
        }
        #endregion
    }
}
