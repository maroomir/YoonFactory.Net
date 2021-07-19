using System;
using System.Drawing;

namespace YoonFactory.Log
{
    public class LogArgs : EventArgs
    {
        public string Message;
    }

    public class LogDisplayArgs : LogArgs
    {
        public Color BackColor;

        public LogDisplayArgs(Color pColor, string strMessage)
        {
            BackColor = pColor;
            Message = strMessage;
        }
    }

    public delegate void LogProcessCallback(object sender, LogArgs e);

}