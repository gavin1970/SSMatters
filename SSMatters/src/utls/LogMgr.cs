using System;
using System.Diagnostics;

namespace TextLogManager
{
    internal class LogMgr
    {
        public void WriteLine(string message)
        {
            Debug.WriteLine(message);
        }
        public void WriteLine(Exception ex, string addNote = "")
        {
            string msg = addNote;
            if (!string.IsNullOrWhiteSpace(msg))
                msg += Environment.NewLine;
            msg += ex.Message;

            Debug.WriteLine(msg, addNote);
        }
    }
}
