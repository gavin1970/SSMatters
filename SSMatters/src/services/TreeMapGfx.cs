using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Windows.Forms;
using SSMatters.src.models;
using SSMatters.src.utls;

namespace SSMatters.src.services
{
    internal class TreeMapGfx
    {
        //private struct FileAssoc
        //{
        //    public string ext;
        //    public Icon ico;
        //}

        #region Private Vars
        private const int maxFileFolderDisplay = 255;
        private const string folderAssoc = @".\icons";
        #region Static Vars
        private static readonly object threadProtect = new object();
        //private static List<FileAssoc> fileAssocs = new List<FileAssoc>();
        #endregion
        #endregion

        #region Public Properties
        public STreeMapOptions Options { get; set; } = new STreeMapOptions();
        #endregion

        #region Public Methods
        public TreeMapGfx(STreeMapOptions options = null) 
        {
            if(options != null)
                Options = options;

            if (!Directory.Exists(folderAssoc))
                Directory.CreateDirectory(folderAssoc);
        }
        public int DrawView(Graphics gfx, FolderData folderData, Size maxSize, bool dynamic)
        {
            int retVal;

            switch (Options.MapType)
            {
                case STreeMapType.File:
                    //if (dynamic)
                        retVal = DynamicDrawFiles(gfx, folderData, maxSize);
                    //else
                    //    retVal = StaticDrawFiles(gfx, folderData, maxSize);
                    break;
                case STreeMapType.Folder:
                    //if (dynamic)
                        retVal = DynamicDrawFolders(gfx, folderData, maxSize);
                    //else
                    //    retVal = StaticDrawFolders(gfx, folderData, maxSize);
                    break;
                default:
                    retVal = 0;
                    break;
            }

            return retVal;
        }
        #endregion

        #region Private Methods
        private Brush SpanColor(Brush color, double perc)
        {
            double f = Math.Round(perc * 255,0);
            Color clr = Color.FromArgb((int)f, ((SolidBrush)color).Color);
            return new SolidBrush(clr);
        }
        private int DrawBoxes(Graphics gfx, RectangleF[] rectangles, string[] labels, bool isFolder)
        {
            gfx.Clear(SystemColors.ControlDarkDark);
            int cnt = 1;

            Brush bg = isFolder ? Options.FolderBGColor : Options.FileBGColor;
            Brush fgl = isFolder ? Options.FolderFGColorLight : Options.FileFGColorLight;
            Brush fgd = isFolder ? Options.FolderFGColorDark : Options.FileFGColorDark;
            Pen pen = isFolder ? Options.FolderPen : Options.FilePen;
            Font fnt = isFolder ? Options.FolderFont : Options.FileFont;

            // Draw and label each rectangle
            for (int i = 0; i < rectangles.Length; i++)
            {
                //default
                double perc = .5;
                if (rectangles.Length > 1)
                {
                    //setup fade percentage based on total file count.
                    perc = 1 - (((double)cnt) / ((double)rectangles.Length));
                    if (perc < .02)
                        perc = .02;
                }

                //file.Rectangle = fRects[fLoc++];
                Brush bgColor = SpanColor(bg, perc);
                Brush fgColor = perc <= .5 ? fgl : fgd;

                //draw background as white, so fades look better.
                gfx.FillRectangle(Brushes.White, rectangles[i]);
                //draw faded background on previous white draw
                gfx.FillRectangle(bgColor, rectangles[i]);
                gfx.DrawRectangle(pen, rectangles[i].X, rectangles[i].Y, rectangles[i].Width, rectangles[i].Height);
                gfx.DrawString(labels[i], fnt, fgColor, rectangles[i]);
                cnt++;
            }

            return rectangles.Length;
        }
        private int DrawEmpty(Graphics gfx, RectangleF borders, bool isFolder)
        {
            string type = isFolder ? "folders" : "files";
            Font fnt = isFolder ? Options.FolderFont : Options.FileFont;
            fnt = new Font(fnt.FontFamily, fnt.Size + 10, fnt.Style, fnt.Unit);
            StringFormat stringFormat = new StringFormat();
            stringFormat.Alignment = StringAlignment.Center;
            stringFormat.LineAlignment = StringAlignment.Center;
            gfx.DrawString($"No {type} exists", fnt, Brushes.Black, borders, stringFormat);
            return 0;
        }
        private int DynamicDrawFiles(Graphics gfx, FolderData folderData, Size maxSize)
        {
            int retVal = 0;
            double minPercSize = .03;
            RectangleF emptyRect = new RectangleF(0.0F, 0.0F, (float)maxSize.Width, (float)maxSize.Height);
            RectangleF[] rectangles = null;
            FileData[] allFileData;

            lock (threadProtect)
            {
                //lets sort the array for larger to smaller in over all byte size
                folderData.Files.Sort(delegate (FileData x, FileData y)
                {
                    return y.TotalByteSize.CompareTo(x.TotalByteSize);
                });

                //since there are folders that have thousands of files and sub folders, these boxes are
                //too small to see or click on.  Therefore, I'm limiting these to the set amount (maxFileFolderDisplay).
                allFileData = folderData.Files.Take(maxFileFolderDisplay).ToArray();
            }

            // Create sample data. Data must be sorted large to small.
            double[] fAllSizes = allFileData
                    .Where(w => w.TotalByteSize > 0)
                    .Select(d => d.TotalByteSize)
                    .ToArray();

            //fAllSizes could have numbers much larger than then next or
            //the smallest, that smaller boxes will not show up.  We need to get
            //percentages of size over all and shrink these numbers.
            double total = fAllSizes.Sum();
            int sizeLoc = 0;

            //change up sizes for percentage, smaller numbers
            foreach(double size in fAllSizes)
            {
                double boxPerc = Math.Round((size / total) * 100, 2);
                //if below a min, normalize the box to the same size.
                if (boxPerc < (minPercSize * 100))
                    boxPerc = (minPercSize * 100);

                fAllSizes[sizeLoc++] = boxPerc;  //get percentage
            }

            // Create an array of labels in the same order as the sorted data.
            string[] labels = allFileData
                    .Where(w => w.TotalByteSize > 0)
                    .Select(d => $"{d.FileName}\n{Common.FormatByteSize(d.TotalByteSize)}")
                    .ToArray();

            //TODO: Treemap class has an issue when there is only 1 folder.  Something I've not dug into yet.
            //My if check resolves this for now.
            if (fAllSizes.Length > 1)
            {
                //lets build some rectangles
                rectangles = TreeMapRecs.GetRectangles(fAllSizes, maxSize.Width, maxSize.Height);
                int cnt = 0;
                foreach (FileData fd in allFileData)
                {
                    //zero size was fillered out in sort above.
                    if (fd.TotalByteSize > 0)
                        fd.Rectangle = rectangles[cnt++];
                }
            }
            else if (fAllSizes.Length == 1)
            {
                rectangles = new RectangleF[] { emptyRect };
                if (allFileData.Length == 1)
                    allFileData[0].Rectangle = rectangles[0];
                else
                {
                    try
                    {
                        throw new WarningException($"One or more files in this folder have a size of 0 bytes.  Those files will not be displayed here.");
                    } 
                    catch(WarningException ex)
                    {
                        // Get stack trace for the exception with source file information
                        var st = new StackTrace(ex, true);
                        // Get the top stack frame
                        var frame = st.GetFrame(0);
                        // Get the code file from the stack frame
                        var fileName = frame.GetFileName();
                        // Get the line number from the stack frame
                        var line = frame.GetFileLineNumber();
                        // Lets display it.
                        MessageBox.Show($"{ex.Message}\n\nFile: {fileName}\nLine #: {line}", "Notice", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                }
            }

            //make sure we have the same amount of recs and labels.  If not, something went wrong.
            if (rectangles?.Length == labels?.Length && rectangles?.Length > 0)
                retVal = DrawBoxes(gfx, rectangles, labels, false);
            else if (rectangles?.Length == 0 || labels?.Length == 0)
                retVal = DrawEmpty(gfx, emptyRect, false);
            else if (rectangles?.Length > 0)
            {
                try
                {
                    throw new Exception($"This should never occur, but lets take a look if it does.\nrectangles?.Length did not equal labels?.Length'\nrectangles: {rectangles?.Length} != labels: {labels?.Length}");
                }
                catch (Exception ex)
                {
                    // Get stack trace for the exception with source file information
                    var st = new StackTrace(ex, true);
                    // Get the top stack frame
                    var frame = st.GetFrame(0);
                    // Get the code file from the stack frame
                    var fileName = frame.GetFileName();
                    // Get the line number from the stack frame
                    var line = frame.GetFileLineNumber();
                    // Lets display it.
                    MessageBox.Show($"{ex.Message}\n\nFile: {fileName}\nLine #: {line}", "Wait, What?!!", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }

            return retVal;
        }
        private int DynamicDrawFolders(Graphics gfx, FolderData folderData, Size maxSize)
        {
            int retVal = 0;
            double minPercSize = .03;
            RectangleF emptyRect = new RectangleF(0.0F, 0.0F, (float)maxSize.Width, (float)maxSize.Height);
            RectangleF[] rectangles = null;
            FolderData[] subFolderData;

            lock (threadProtect)
            {
                folderData.Directories.Sort(delegate (FolderData x, FolderData y)
                {
                    return y.TotalFileByteSize.CompareTo(x.TotalFileByteSize);
                });

                subFolderData = folderData.Directories.Take(maxFileFolderDisplay).ToArray();
            }

            // Create sample data. Data must be sorted large to small.
            double[] fAllSizes = subFolderData
                    .Where(w => w.TotalFileByteSize > 0)
                    .Select(d => d.TotalFileByteSize)
                    .ToArray();

            //because fAllSizes could have numbers much larger than then next or
            //the smallest, that smaller boxes will not show up.  We need to get
            //percentages of size over all and scrink these numbers.
            double total = fAllSizes.Sum();
            int sizeLoc = 0;

            foreach (double size in fAllSizes)
            {
                double boxPerc = Math.Round((size / total) * 100, 2);
                if (boxPerc < (minPercSize * 100))
                    boxPerc = (minPercSize * 100);

                fAllSizes[sizeLoc++] = boxPerc;  //get percentage
            }

            // Create an array of labels in the same order as the sorted data.
            string[] labels = subFolderData
                    .Where(w => w.TotalFileByteSize > 0)
                    .Select(d => $"{d.DirectoryName}\n{Common.FormatByteSize(d.TotalFileByteSize)}")
                    .ToArray();

            if (fAllSizes.Length > 1)
            {
                rectangles = TreeMapRecs.GetRectangles(fAllSizes, maxSize.Width, maxSize.Height);

                int cnt = 0;
                foreach (FolderData fd in subFolderData)
                {
                    //zero size was fillered out in sort above.
                    if (fd.TotalFileByteSize > 0)
                        fd.Rectangle = rectangles[cnt++];
                }
            }
            else if (fAllSizes.Length == 1)
            {
                rectangles = new RectangleF[] { emptyRect };
                if (subFolderData.Length == 1)
                    subFolderData[0].Rectangle = rectangles[0];
                else
                {
                    foreach (var dir in subFolderData)
                    {
                        if (dir.DirectoryName.StartsWith(labels[0].Split('\n')[0]))
                        {
                            dir.Rectangle = rectangles[0];
                            break;
                        }
                    }
                }
            }

            if (rectangles?.Length == labels?.Length && rectangles?.Length > 0)
                retVal = DrawBoxes(gfx, rectangles, labels, true);
            else if (rectangles?.Length == 0 || labels?.Length == 0)
                retVal = DrawEmpty(gfx, emptyRect, true);
            else if (rectangles?.Length > 0)
            {
                try
                {
                    throw new Exception($"This should never occur, but lets take a look if it does.\nrectangles?.Length did not equal labels?.Length'\nrectangles: {rectangles?.Length} != labels: {labels?.Length}");
                }
                catch (Exception ex)
                {
                    // Get stack trace for the exception with source file information
                    var st = new StackTrace(ex, true);
                    // Get the top stack frame
                    var frame = st.GetFrame(0);
                    // Get the code file from the stack frame
                    var fileName = frame.GetFileName();
                    // Get the line number from the stack frame
                    var line = frame.GetFileLineNumber();
                    // Lets display it.
                    MessageBox.Show($"{ex.Message}\n\nFile: {fileName}\nLine #: {line}", "Wait, What?!!", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }

            return retVal;
        }
        #endregion
    }
}
