using System;
using System.Drawing;
using System.Threading.Tasks;
using System.IO;
using YoonFactory.Files;

namespace YoonFactory.Log
{
    public class YoonDisplayer : IYoonLog, IDisposable
    {
        #region IDisposable Support

        private bool _disposedValue = false; // 중복 호출을 검색하려면

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
            Path.Combine(Directory.GetCurrentDirectory(), "YoonFactory", "DLMLog");

        public event LogProcessCallback OnProcessLogEvent;
        private readonly int _nDirectoryFileExistDays = 1; // DO NOT USE 0
        private static readonly object _logLock = new object();
        private const int MAX_DAY_SPAN = 30;

        public YoonDisplayer()
        {
            //
        }

        public YoonDisplayer(int nDays)
        {
            _nDirectoryFileExistDays = nDays switch
            {
                > MAX_DAY_SPAN => MAX_DAY_SPAN,
                < 0 => 0,
                _ => nDays
            };
        }

        public YoonDisplayer(string strPath, int nDays) : this(nDays)
        {
            RootDirectory = strPath;
        }

        public void Write(string strMessage, bool bSave = true)
        {
            Write(eYoonStatus.Info, strMessage, bSave);
        }

        /// <summary>
        /// Write a log on the general form
        /// </summary>
        /// <param name="index"></param>
        /// <param name="strMessage"></param>
        public void Write(eYoonStatus index, string strMessage, bool bSave = true)
        {
            string strLine = DisplayMessage(index, strMessage);
#if DEBUG
            if (RootDirectory != string.Empty && bSave)
                WriteDisplayLog(strLine);
#endif
        }

        private string DisplayMessage(eYoonStatus index, string strMessage)
        {
            lock (_logLock)
            {
                DateTime pNow = DateTime.Now;
                string nowTime = $"{pNow:yyyy-MM-dd HH:mm:ss:fff}"; //현재 시간 가져오기
                string strMessageLine = "";
                switch (index)
                {
                    case eYoonStatus.Normal:
                        strMessageLine = "[" + nowTime + "] " + strMessage + "\n";
                        OnProcessLogEvent(this, new LogDisplayerArgs(Color.White, strMessageLine));
                        break;
                    case eYoonStatus.Conform:
                        strMessageLine = "[" + nowTime + "]  CONFIRM : " + strMessage + "\n";
                        OnProcessLogEvent(this, new LogDisplayerArgs(Color.Khaki, strMessageLine));
                        break;
                    case eYoonStatus.Send:
                        strMessageLine = "[" + nowTime + "]  SEND : " + strMessage + "\n";
                        OnProcessLogEvent(this, new LogDisplayerArgs(Color.Lavender, strMessageLine));
                        break;
                    case eYoonStatus.Receive:
                        strMessageLine = "[" + nowTime + "]  RECEIVE : " + strMessage + "\n";
                        OnProcessLogEvent(this, new LogDisplayerArgs(Color.Lavender, strMessageLine));
                        break;
                    case eYoonStatus.User:
                        strMessageLine = "[" + nowTime + "]  USER : " + strMessage + "\n";
                        OnProcessLogEvent(this, new LogDisplayerArgs(Color.Lime, strMessageLine));
                        break;
                    case eYoonStatus.Inspect:
                        strMessageLine = "[" + nowTime + "]  INSPECT : " + strMessage + "\n";
                        OnProcessLogEvent(this, new LogDisplayerArgs(Color.Lime, strMessageLine));
                        break;
                    case eYoonStatus.Error:
                        strMessageLine = "[" + nowTime + "]  ERROR : " + strMessage + "\n";
                        OnProcessLogEvent(this, new LogDisplayerArgs(Color.Salmon, strMessageLine));
                        break;
                    case eYoonStatus.Info:
                        strMessageLine = "[" + nowTime + "]  INFO : " + strMessage + "\n";
                        OnProcessLogEvent(this, new LogDisplayerArgs(Color.Khaki, strMessageLine));
                        break;
                }

                return strMessageLine;
            }
        }

        private void WriteDisplayLog(string data)
        {
            Task.Factory.StartNew(new System.Action(DeleteDirectoryFile));

            string strPath = CreateDirectoryFile();
            if (!FileFactory.VerifyFileExtension(ref strPath, ".txt"))
            {
                Console.WriteLine("LogManagement WriteDisplayLog: path fail");
                return;
            }

            StreamWriter pWriter;
            try
            {
                pWriter = new StreamWriter(strPath, true);
            }
            catch (Exception ex)
            {
                Console.WriteLine("LogManagement WriteDisplayLog Exception: {0}", ex.ToString());
                return;
            }

            try
            {
                pWriter.WriteLine(data);
                pWriter.Close();
            }
            catch (System.Exception ex)
            {
                Console.WriteLine("LogManagement WriteDisplayLog Exception: {0}", ex.ToString());
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
