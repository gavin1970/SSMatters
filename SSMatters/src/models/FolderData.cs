using System;
using System.Collections.Generic;
using System.Drawing;

namespace SSMatters.src.models
{
    internal class FolderData
    {
        public FolderData() { }
        public FolderData(string fullPath) 
        { 
            FullPath = fullPath;
        }
        /// <summary>
        /// Current full path folder name
        /// </summary>
        public string FullPath { get; set; } = string.Empty;
        /// <summary>
        /// Current folder name
        /// </summary>
        public string DirectoryName { get; set; } = string.Empty;
        /// <summary>
        /// Date folder was created.
        /// </summary>
        public DateTime CreatedDate { get; set; } = DateTime.MinValue;
        /// <summary>
        /// Date folder was last updated.
        /// </summary>
        public DateTime ModifiedDate { get; set; } = DateTime.MinValue;
        /// <summary>
        /// Total byte file size for this folder only.
        /// </summary>
        public double TotalByteSize { get; set; } = 0;
        /// <summary>
        /// Total byte file size for this folder and all sub folders.
        /// </summary>
        public double TotalFileByteSize { get; set; } = 0;
        /// <summary>
        /// File count for this folder and all sub sub folders.
        /// </summary>
        public Int64 TotalFileCount { get; set; } = 0;
        /// <summary>
        /// File listing for this folder only.
        /// </summary>
        public List<FileData> Files { get; set; } = new List<FileData>();
        /// <summary>
        /// Directory count for this folder and all sub sub folders.
        /// </summary>
        public Int64 TotalDirectoryCount { get; set; } = 0;
        /// <summary>
        /// Folder listing under this folder only.
        /// </summary>
        public List<FolderData> Directories { get; set; } = new List<FolderData>();
        /// <summary>
        /// Any issues will show up here.
        /// </summary>
        public string Info { get; set; }
        /// <summary>
        /// Error free
        /// </summary>
        public bool Valid { get; set; } = true;
        /// <summary>
        /// Location and Size
        /// </summary>
        public RectangleF Rectangle { get; set; }
    }
}
