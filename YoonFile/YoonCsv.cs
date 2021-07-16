using System.Collections.Generic;
using System.Text;
using System.IO;

namespace YoonFactory.Files
{
    public class YoonCsv : IYoonFile
    {
        #region IDisposable Support

        private bool _disposedValue = false;

        protected virtual void Dispose(bool disposing)
        {
            if (!_disposedValue)
            {
                if (disposing)
                {

                }

                _disposedValue = true;
            }
        }

        // 삭제 가능한 패턴을 올바르게 구현하기 위해 추가된 코드입니다.
        public void Dispose()
        {
            Dispose(true);
        }

        #endregion

        public string FilePath { get; private set; }

        public YoonCsv(string strPath)
        {
            FilePath = FileFactory.VerifyFileExtension(ref strPath, ".csv", true, true)
                ? strPath
                : Path.Combine(Directory.GetCurrentDirectory(), "YoonFactory", "YoonFactory.csv");
        }

        public void CopyFrom(IYoonFile pFile)
        {
            if (pFile is YoonCsv pCsv)
            {
                FilePath = pCsv.FilePath;
            }
        }

        public IYoonFile Clone()
        {
            return new YoonCsv(FilePath);
        }

        public bool IsFileExist()
        {
            return FileFactory.VerifyFilePath(FilePath, false);
        }

        public bool LoadFile()
        {
            return IsFileExist();
        }

        public bool SaveFile()
        {
            return IsFileExist();
        }

        public List<string[]> GetBlock()
        {
            List<string[]> pListResult = new List<string[]>();
            StreamReader reader = new StreamReader(FilePath);
            while (!reader.EndOfStream)
            {
                string strMessage = reader.ReadLine();
                string[] pArrayValue = strMessage.Split(',');
                pListResult.Add(pArrayValue);
            }

            reader.Close();
            return pListResult;
        }

        public bool SetBlock(List<string[]> pListValue)
        {
            FileStream fileStream = new FileStream(FilePath, FileMode.Create, FileAccess.Write);
            StreamWriter writer = new StreamWriter(fileStream, Encoding.Default);
            bool bResult = true;
            try
            {
                for (int iRow = 0; iRow < pListValue.Count; iRow++)
                {
                    string[] pArrayValue = pListValue[iRow];
                    string strData = "";
                    for (int iCol = 0; iCol < pArrayValue.Length; iCol++)
                    {
                        strData += pArrayValue[iCol];
                        if (iCol < pArrayValue.Length - 1)
                        {
                            strData += ",";
                        }
                    }

                    writer.WriteLine(strData);
                }
            }
            catch
            {
                bResult = false;
            }

            writer.Close();
            return bResult;
        }

        public bool SetLine(string[] pArrayValue)
        {
            FileStream fileStream = new FileStream(FilePath, FileMode.Create, FileAccess.Write);
            StreamWriter writer = new StreamWriter(fileStream, Encoding.Default);
            string strData = "";
            bool bResult = true;
            try
            {
                for (int iCol = 0; iCol < pArrayValue.Length; iCol++)
                {
                    strData += pArrayValue[iCol];
                    if (iCol < pArrayValue.Length - 1)
                    {
                        strData += ",";
                    }
                }

                writer.WriteLine(strData);
            }
            catch
            {
                bResult = false;
            }

            writer.Close();
            return bResult;
        }

        public bool SetLine(string strLine)
        {
            FileStream fileStream = new FileStream(FilePath, FileMode.Create, FileAccess.Write);
            StreamWriter writer = new StreamWriter(fileStream, Encoding.Default);
            if (strLine.Split(',').Length == 0) return false;

            bool bResult = true;
            try
            {
                writer.WriteLine(strLine);
            }
            catch
            {
                bResult = false;
            }

            writer.Close();
            return bResult;
        }
    }
}
