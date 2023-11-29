using System;
using System.Drawing;
using System.IO;

namespace SSMatters.src.models
{
    internal class FileData
    {
        public FileData() { }
        public FileData(FileInfo fileInfo) 
        { 
            try
            {
                FullName = fileInfo.FullName;
                FileName = fileInfo.Name;
                if (fileInfo.Exists)
                {
                    TotalByteSize = fileInfo.Length;
                    CreatedDate = fileInfo.CreationTime;
                    ModifiedDate = fileInfo.LastWriteTime;
                }
                else
                    Info = "Could not find file.  Possible permission during access.";
            }
            catch (IOException ex)
            {
                Info = ex.Message;
            }
        }
        /// <summary>
        /// Current full path to file 
        /// </summary>
        public string FullName { get; set; } = string.Empty;
        /// <summary>
        /// Current file name
        /// </summary>
        public string FileName { get; set; } = string.Empty;
        /// <summary>
        /// Total byte file size for this file only.
        /// </summary>
        public double TotalByteSize { get; set; }
        /// <summary>
        /// Created date of file
        /// </summary>
        public DateTime CreatedDate { get; set; }
        /// <summary>
        /// Modified Date of file
        /// </summary>
        public DateTime ModifiedDate { get; set; }
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
        /// <summary>
        /// Icon assocated to file or EXE file icon.
        /// </summary>
        public Icon IconAssociation { get; set; }
        /// <summary>
        /// Prevent icon lookup more than once.
        /// </summary>
        public bool IconLookedUp { get; set; } = false;
    }
}
