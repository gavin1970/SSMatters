using Microsoft.WindowsAPICodePack.Dialogs;
using System;

namespace SSMatters.src.utls
{
    internal class Common
    {
        static readonly string[] suffixes = { "Bytes", "KB", "MB", "GB", "TB", "PB" };
        public static string FormatByteSize(double bytes)
        {
            int idx = 0;
            double num= bytes;

            while (num > 1024)
            {
                num /= 1024;
                idx++;
            }
            return string.Format("{0:n2} {1}", num, suffixes[idx]);
        }
        public static CommonFileDialogResult FolderPicker(ref string folder, string dlgName)
        {
            CommonFileDialogResult retVal = CommonFileDialogResult.Cancel;
            using (CommonOpenFileDialog fbd = new CommonOpenFileDialog(dlgName))
            {
                fbd.Multiselect = false;
                fbd.IsFolderPicker = true;
                fbd.InitialDirectory = folder;
                
                retVal = fbd.ShowDialog();

                if (retVal == CommonFileDialogResult.Ok)
                    folder = fbd.FileName;
            }

            return retVal;
        }

    }
}
