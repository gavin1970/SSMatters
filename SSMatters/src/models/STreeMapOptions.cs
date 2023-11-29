using System.Drawing;

namespace SSMatters.src.models
{
    internal enum STreeMapType
    {
        Folder,
        File
    }
    internal class STreeMapOptions
    {
        public STreeMapType MapType { get; set; } = STreeMapType.Folder;
        public Size MaxWindowSize { get; set; } = Size.Empty;
        public int BorderSize { get; set; } = 1;
        public Brush BorderColor { get; set; } = Brushes.DarkGray;
        public Brush FolderFGColorDark { get; set; } = Brushes.Wheat;
        public Brush FolderFGColorLight { get; set; } = Brushes.Black;
        public Brush FolderBGColor { get; set; } = Brushes.DarkBlue; //
        public Font FolderFont { get; set; } = new Font("Verdana", 10, FontStyle.Bold, GraphicsUnit.Pixel);
        public Pen FolderPen { get { return new Pen(BorderColor, BorderSize); } }
        public Brush FileFGColorDark { get; set; } = Brushes.Wheat;
        public Brush FileFGColorLight { get; set; } = Brushes.Black;
        public Brush FileBGColor { get; set; } = Brushes.DarkGreen;   //
        public Pen FilePen { get { return new Pen(BorderColor, BorderSize); } }
        public Font FileFont { get; set; } = new Font("Verdana", 10, FontStyle.Bold, GraphicsUnit.Pixel);
        public Brush ErrorFGColor { get; set; } = Brushes.White;
        public Brush ErrorBGColor { get; set; } = Brushes.Red;
    }
}
