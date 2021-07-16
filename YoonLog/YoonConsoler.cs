using System;
using System.Threading.Tasks;
using System.IO;
using YoonFactory.Files;

#pragma warning disable

namespace YoonFactory.Log
{
    public class YoonConsoler : IYoonLog, IDisposable
    {
        #region IDisposable Support

        private bool _disposedValue = false;

        protected virtual void Dispose(bool disposing)
        {
            if (!_disposedValue)
            {
                if (disposing)
                {
                    //
                }

                _disposedValue = true;
            }
        }

        public void Dispose()
        {
            Dispose(true);
        }

        #endregion

        public string RootDirectory { get; set; } =
            Path.Combine(Directory.GetCurrentDirectory(), "YoonFactory", "CLMLog");

        public event LogProcessCallback OnProcessLogEvent; // Only use to unify the interface

        private readonly int _nDirectoryFileExistDays = 1; // DO NOT USE 0
        private const int MAX_DAY_SPAN = 30;

        public YoonConsoler()
        {
            //
        }

        public YoonConsoler(int nDays)
        {
            _nDirectoryFileExistDays = nDays switch
            {
                > MAX_DAY_SPAN => MAX_DAY_SPAN,
                < 0 => 0,
                _ => nDays
            };
        }

        public YoonConsoler(string strPath, int nDays) : this(nDays)
        {
            RootDirectory = strPath;
        }

        /// <summary>
        /// Write a log on the console
        /// </summary>
        /// <param name="strMessage"> Message </param>
        public void Write(string strMessage, bool bSave = true)
        {
            DateTime pNow = DateTime.Now;
            string nowTime = string.Format("{0:yyyy-MM-dd HH:mm:ss:fff}", pNow);
            string strLine = "[" + nowTime + "] " + strMessage;
            Console.WriteLine(strLine);
#if DEBUG
            if (RootDirectory != string.Empty && bSave)
                WriteConsoleLog(strLine);
#endif
        }

        private void WriteConsoleLog(string data)
        {
            Task.Factory.StartNew(new Action(DeleteDirectoryFile));

            string strPath = CreateDirectoryFile();
            if (!FileFactory.VerifyFileExtension(ref strPath, ".txt"))
            {
                Console.WriteLine("LogManagement WriteConsoleLog: path fail");
                return;
            }

            StreamWriter pWriter;
            try
            {
                pWriter = new StreamWriter(strPath, true);
            }
            catch (Exception ex)
            {
                Console.WriteLine("LogManagement WriteConsoleLog Exception: {0}", ex.ToString());
                return;
            }

            try
            {
                pWriter.WriteLine(data);
                pWriter.Close();
            }
            catch (System.Exception ex)
            {
                Console.WriteLine("LogManagement WriteConsoleLog Exception: {0}", ex.ToString());
                pWriter.Close();
            }
        }

        private string CreateDirectoryFile()
        {
            string strDirPath = string.Empty;
            DateTime now = DateTime.Now;

            try
            {
                strDirPath = Path.Combine(RootDirectory, now.Year.ToString(), now.Month.ToString(), now.Day.ToString(),
                    now.Hour.ToString() + ".txt");

                FileFactory.VerifyFilePath(strDirPath);
            }
            catch (System.Exception ex)
            {
                Console.WriteLine("LogManagement CreateDirectoryFile Exception: {0}", ex.ToString());
            }

            return strDirPath;
        }

        private void DeleteDirectoryFile()
        {
            try
            {
                DateTime pNow = DateTime.Now;
                string strMonthPath = Path.Combine(RootDirectory, pNow.Year.ToString(), pNow.Month.ToString());
                FileFactory.DeleteOldFilesInDirectory(strMonthPath, _nDirectoryFileExistDays);
                DateTime pPreMonth = pNow.AddMonths(-1); // Pre-Month
                strMonthPath = Path.Combine(RootDirectory, pPreMonth.Year.ToString(), pPreMonth.Month.ToString());
                FileFactory.DeleteOldFilesInDirectory(strMonthPath, _nDirectoryFileExistDays);
            }
            catch (System.Exception ex)
            {
                Console.WriteLine("LogManagement DeleteDirectoryFile Exception: {0}", ex.ToString());
            }
        }
    }
}