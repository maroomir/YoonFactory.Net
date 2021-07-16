using System;
using System.Collections.Generic;
using System.IO;

namespace YoonFactory.Files
{
    public static class FileFactory
    {
        public static bool VerifyDirectory(string strPath)
        {
            if (string.IsNullOrEmpty(strPath)) return false;
            if (!Directory.Exists(strPath))
            {
                Directory.CreateDirectory(strPath);
                if (Directory.Exists(strPath))
                    return true;
            }
            else
                return true;

            return false;
        }

        public static bool VerifyFilePath(string strPath, bool bCreateFile = true)
        {
            if (string.IsNullOrEmpty(strPath)) return false;
            FileInfo fi = new FileInfo(strPath);
            if (!VerifyDirectory(fi.DirectoryName))
                return false;

            try
            {
                if (!fi.Exists)
                {
                    if (!bCreateFile) return false;
                    FileStream fs = fi.Create();
                    fs.Close();
                    return true;
                }
                else
                    return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }

            return false;
        }

        /// <summary>
        /// VerifyFileExtension
        /// </summary>
        /// <param name="strPath"> File Path </param>
        /// <param name="strExt"> Extension string as like ".Ext" </param>
        /// <param name="bChangeExtension"> To change the extension as strExt </param>
        /// <returns></returns>
        public static bool VerifyFileExtension(ref string strPath, string strExt, bool bChangeExtension = false,
            bool bCreateFile = false)
        {
            if (!VerifyFilePath(strPath, bCreateFile)) return false;
            FileInfo fi = new FileInfo(strPath);
            if (fi.Extension != strExt)
            {
                if (!bChangeExtension) return false;
                string strFilePath = Path.Combine(fi.DirectoryName, Path.GetFileNameWithoutExtension(strPath) + strExt);
                if (!VerifyFilePath(strFilePath, bCreateFile)) return false;
                strPath = strFilePath;
            }

            return true;
        }

        public static bool IsFileExist(string strPath)
        {
            System.IO.FileInfo fi = new System.IO.FileInfo(strPath);
            return fi.Exists;
        }

        public static List<string> GetFileListInDir(string strRoot, List<string> pListFile)
        {
            if (pListFile == null)
                return null;
            FileAttributes fAttribute = File.GetAttributes(strRoot);
            // If root path is Directory
            if ((fAttribute & FileAttributes.Directory) == FileAttributes.Directory)
            {
                DirectoryInfo di = new DirectoryInfo(strRoot);
                // continues abstract subdirectory
                foreach (DirectoryInfo dir in di.GetDirectories())
                {
                    GetFileListInDir(dir.FullName, pListFile);
                }

                // abstract sub-files
                foreach (FileInfo fir in di.GetFiles())
                {
                    GetFileListInDir(fir.FullName, pListFile);
                }
            }
            else
            {
                FileInfo fi = new FileInfo(strRoot);
                pListFile.Add(fi.FullName);
            }

            return pListFile;
        }

        public static string GetTextFromFile(string strPath)
        {
            FileStream pStream = null;
            try
            {
                pStream = new FileStream(strPath, FileMode.Open);
            }
            catch
            {
                Console.WriteLine(string.Format("Error opening file ({0}) !", strPath));
                return string.Empty;
            }

            string strComplete = string.Empty;
            using (StreamReader sr = new StreamReader(pStream))
            {
                string strLine;
                while ((strLine = sr.ReadLine()) != null)
                    strComplete += strLine;
            }

            return strComplete;
        }

        public static List<string> GetTextLinesFromFile(string strPath)
        {
            FileStream fs = null;
            try
            {
                fs = new FileStream(strPath, FileMode.Open);
            }
            catch
            {
                Console.WriteLine($"Error opening file ({strPath}) !");
                return null;
            }

            List<string> pListTextComplete = new List<string>();
            using (StreamReader sr = new StreamReader(fs))
            {
                string strLine;
                while ((strLine = sr.ReadLine()) != null)
                    pListTextComplete.Add(strLine);
            }

            return pListTextComplete;
        }

        public static bool AppendTextToFile(string strPath, string strData)
        {
            FileStream fs = null;

            try
            {
                fs = new FileStream(strPath, FileMode.Append);
            }
            catch
            {
                Console.WriteLine($"Error opening file ({strPath}) !");
                return false;
            }

            StreamWriter sw = new StreamWriter(fs);
            sw.WriteLine(strData);
            sw.Close();
            return true;
        }

        public static bool DeleteFilePath(string strPath)
        {
            if (string.IsNullOrEmpty(strPath)) return false;
            FileInfo pFile = new FileInfo(strPath);
            if (!VerifyDirectory(pFile.DirectoryName))
                return false;

            try
            {
                if (pFile.Exists)
                {
                    pFile.Delete();
                    return true;
                }
                else
                    return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }

            return false;
        }

        public static void DeleteExtensionFilesInDirectory(string strRoot, string strExt)
        {
            if (!VerifyDirectory(strRoot)) return;
            DirectoryInfo dirInfo = new DirectoryInfo(strRoot);
            // File Clear in DirPath
            foreach (FileInfo file in dirInfo.GetFiles())
            {
                if (file.Extension == strExt)
                    file.Delete();
            }

            // Directory clear in DirPath
            foreach (DirectoryInfo dir in dirInfo.GetDirectories())
                DeleteExtensionFilesInDirectory(dir.FullName, strExt);
        }

        public static void DeleteIncludeSpecificInDirectory(string strRoot, string strSpecific,
            bool bCheckFileNameOnly = false)
        {
            if (!VerifyDirectory(strRoot)) return;
            DirectoryInfo dirInfo = new DirectoryInfo(strRoot);
            // File Clear in DirPath
            foreach (FileInfo file in dirInfo.GetFiles())
            {
                if (bCheckFileNameOnly)
                {
                    string strFileName = file.Name + "." + file.Extension;
                    if (strFileName.Contains(strSpecific))
                        file.Delete();
                }
                else
                {
                    if (file.FullName.Contains(strSpecific))
                        file.Delete();
                }
            }

            // Directory clear in DirPath
            foreach (DirectoryInfo dir in dirInfo.GetDirectories())
                DeleteExtensionFilesInDirectory(dir.FullName, strSpecific);
        }

        public static void DeleteOldFilesInDirectory(string strPath, int nDateSpan)
        {
            DeleteOldFilesInDirectory(strPath, DateTime.Now, nDateSpan);
        }

        public static void DeleteOldFilesInDirectory(string strPath, DateTime pDateStart, int nDateSpan)
        {
            if (!VerifyDirectory(strPath)) return;
            DirectoryInfo dirInfo = new DirectoryInfo(strPath);
            //// File Clear in DirPath
            foreach (FileInfo file in dirInfo.GetFiles())
            {
                TimeSpan pSpan = pDateStart - file.CreationTime;

                if ((int) pSpan.TotalDays >= nDateSpan)
                    file.Delete();
            }

            //// Directory clear in DirPath
            foreach (DirectoryInfo dir in dirInfo.GetDirectories())
                DeleteOldFilesInDirectory(dir.FullName, pDateStart, nDateSpan);
        }

        public static void DeleteAllFilesInDirectory(string strRoot)
        {
            if (!VerifyDirectory(strRoot)) return;
            DirectoryInfo pRootDir = new DirectoryInfo(strRoot);
            //// File Clear in DirPath
            foreach (FileInfo pFile in pRootDir.GetFiles())
            {
                pFile.Delete();
            }

            //// Directory clear in DirPath
            foreach (DirectoryInfo pDir in pRootDir.GetDirectories())
            {
                DeleteAllFilesInDirectory(pDir.FullName);
            }

            pRootDir.Delete();
        }
    }
}
