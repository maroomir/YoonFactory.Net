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
            FileInfo pFile = new FileInfo(strPath);
            if (!VerifyDirectory(pFile.DirectoryName))
                return false;

            try
            {
                if (!pFile.Exists)
                {
                    if (!bCreateFile) return false;
                    FileStream pStream = pFile.Create();
                    pStream.Close();
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

        public static bool VerifyFileExtensions(string strPath, params string[] pArgs)
        {
            if (!VerifyFilePath(strPath, false)) return false;
            bool bResult = false;
            foreach (string strExt in pArgs)
            {
                bResult = VerifyFileExtension(ref strPath, strExt, false, false);
                if (bResult) break;
            }

            return bResult;
        }

        public static bool VerifyFileExtension(ref string strPath, string strExt, bool bChangeExtension = false,
            bool bCreateFile = false)
        {
            if (!VerifyFilePath(strPath, bCreateFile)) return false;
            FileInfo pFile = new FileInfo(strPath);
            if (pFile.Extension == strExt) return true;
            if (!bChangeExtension) return false;
            string strFilePath = Path.Combine(pFile.DirectoryName!, Path.GetFileNameWithoutExtension(strPath) + strExt);
            if (!VerifyFilePath(strFilePath, bCreateFile)) return false;
            strPath = strFilePath;

            return true;
        }

        public static bool IsFileExist(string strPath)
        {
            FileInfo pFile = new FileInfo(strPath);
            return pFile.Exists;
        }

        public static List<string> GetFilePaths(string strRoot)
        {
            List<string> pListFile = new List<string>();
            FileAttributes fAttribute = File.GetAttributes(strRoot);
            // If root path is Directory
            if ((fAttribute & FileAttributes.Directory) == FileAttributes.Directory)
            {
                DirectoryInfo pRootDir = new DirectoryInfo(strRoot);
                // continues abstract subdirectory
                foreach (DirectoryInfo pDir in pRootDir.GetDirectories())
                {
                    pListFile.AddRange(GetFilePaths(pDir.FullName));
                }

                // abstract sub-files
                foreach (FileInfo pFile in pRootDir.GetFiles())
                {
                    pListFile.AddRange(GetFilePaths(pFile.FullName));
                }
            }
            else
            {
                FileInfo pFile = new FileInfo(strRoot);
                pListFile.Add(pFile.FullName);
            }

            return pListFile;
        }

        public static List<string> GetExtensionFilePaths(string strRoot, string strExt)
        {
            // TODO : Construct the File Paths
            List<string> pListFile = new List<string>();
            FileAttributes fAttribute = File.GetAttributes(strRoot);
            // If root path is Directory
            if ((fAttribute & FileAttributes.Directory) == FileAttributes.Directory)
            {
                DirectoryInfo pRootDir = new DirectoryInfo(strRoot);
                // continues abstract subdirectory
                foreach (DirectoryInfo pDir in pRootDir.GetDirectories())
                {
                    pListFile.AddRange(GetExtensionFilePaths(pDir.FullName, strExt));
                }

                // abstract sub-files
                foreach (FileInfo pFile in pRootDir.GetFiles())
                {
                    pListFile.AddRange(GetExtensionFilePaths(pFile.FullName, strExt));
                }
            }
            else
            {
                FileInfo pFile = new FileInfo(strRoot);
                if (pFile.Extension == strExt)
                    pListFile.Add(pFile.FullName);
            }

            return pListFile;
        }

        public static List<string> GetExtensionFilePaths(string strRoot, params string[] pArgs)
        {
            // TODO : Construct the File Paths
            List<string> pListFile = new List<string>();
            FileAttributes fAttribute = File.GetAttributes(strRoot);
            // If root path is Directory
            if ((fAttribute & FileAttributes.Directory) == FileAttributes.Directory)
            {
                DirectoryInfo pRootDir = new DirectoryInfo(strRoot);
                // continues abstract subdirectory
                foreach (DirectoryInfo pDir in pRootDir.GetDirectories())
                {
                    pListFile.AddRange(GetExtensionFilePaths(pDir.FullName, pArgs));
                }

                // abstract sub-files
                foreach (FileInfo pFile in pRootDir.GetFiles())
                {
                    pListFile.AddRange(GetExtensionFilePaths(pFile.FullName, pArgs));
                }
            }
            else
            {
                FileInfo pFile = new FileInfo(strRoot);
                foreach (string strExt in pArgs)
                    if (pFile.Extension == strExt)
                        pListFile.Add(pFile.FullName);
            }

            return pListFile;
        }

        public static string GetTextFromFile(string strPath)
        {
            FileStream pStream;
            try
            {
                pStream = new FileStream(strPath, FileMode.Open);
            }
            catch
            {
                Console.WriteLine($"Error opening file ({strPath}) !");
                return string.Empty;
            }

            string strComplete = string.Empty;
            using (StreamReader pReader = new StreamReader(pStream))
            {
                string strLine;
                while ((strLine = pReader.ReadLine()) != null)
                    strComplete += strLine;
            }

            return strComplete;
        }

        public static List<string> GetTextLinesFromFile(string strPath)
        {
            FileStream pStream;
            try
            {
                pStream = new FileStream(strPath, FileMode.Open);
            }
            catch
            {
                Console.WriteLine($"Error opening file ({strPath}) !");
                return null;
            }

            List<string> pListTextComplete = new List<string>();
            using (StreamReader pReader = new StreamReader(pStream))
            {
                string strLine;
                while ((strLine = pReader.ReadLine()) != null)
                    pListTextComplete.Add(strLine);
            }

            return pListTextComplete;
        }

        public static bool AppendTextToFile(string strPath, string strData)
        {
            FileStream pStream = null;

            try
            {
                pStream = new FileStream(strPath, FileMode.Append);
            }
            catch
            {
                Console.WriteLine($"Error opening file ({strPath}) !");
                return false;
            }

            StreamWriter pWriter = new StreamWriter(pStream);
            pWriter.WriteLine(strData);
            pWriter.Close();
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
                    pFile.Delete();

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
            DirectoryInfo pRootDir = new DirectoryInfo(strRoot);
            // File Clear in DirPath
            foreach (FileInfo pFile in pRootDir.GetFiles())
            {
                if (pFile.Extension == strExt)
                    pFile.Delete();
            }

            // Directory clear in DirPath
            foreach (DirectoryInfo pDir in pRootDir.GetDirectories())
                DeleteExtensionFilesInDirectory(pDir.FullName, strExt);
        }

        public static void DeleteIncludeSpecificInDirectory(string strRoot, string strSpecific,
            bool bCheckFileNameOnly = false)
        {
            if (!VerifyDirectory(strRoot)) return;
            DirectoryInfo pRootDir = new DirectoryInfo(strRoot);
            // File Clear in DirPath
            foreach (FileInfo pFile in pRootDir.GetFiles())
            {
                if (bCheckFileNameOnly)
                {
                    string strFileName = pFile.Name + "." + pFile.Extension;
                    if (strFileName.Contains(strSpecific))
                        pFile.Delete();
                }
                else
                {
                    if (pFile.FullName.Contains(strSpecific))
                        pFile.Delete();
                }
            }

            // Directory clear in DirPath
            foreach (DirectoryInfo pDir in pRootDir.GetDirectories())
                DeleteExtensionFilesInDirectory(pDir.FullName, strSpecific);
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
