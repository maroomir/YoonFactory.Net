using System.Collections.Generic;
using System.IO;
using System.IO.Compression;

namespace YoonFactory.Files
{
    public class YoonZip
    {
        private string _strZipPath;

        public YoonZip(string strPath)
        {
            _strZipPath = FileFactory.VerifyFileExtension(ref strPath, ".zip") ? strPath : "YoonZip.zip";
        }

        public string DirectoryPath { get; set; }

        public string ZipFilePath
        {
            get => _strZipPath;
            set
            {
                if (FileFactory.VerifyFileExtension(ref value, ".zip"))
                    _strZipPath = value;
            }
        }

        public void CompressZip()
        {
            if (!FileFactory.VerifyDirectory(DirectoryPath))
                return;

            List<string> fListFile = FileFactory.GetFilePaths(DirectoryPath);
            using (FileStream pStream = new FileStream(ZipFilePath, FileMode.Create, FileAccess.ReadWrite))
            {
                using (ZipArchive pArchive = new ZipArchive(pStream, ZipArchiveMode.Create))
                {
                    foreach (string strFile in fListFile)
                    {
                        string strPath = strFile.Substring(DirectoryPath.Length + 1);
                        pArchive.CreateEntryFromFile(strFile, strPath);
                    }
                }
            }
        }

        public void CompressZip(string strDirPath)
        {
            if (!FileFactory.VerifyDirectory(strDirPath))
                return;

            List<string> fListFile = FileFactory.GetFilePaths(strDirPath);
            using (FileStream pStream = new FileStream(ZipFilePath, FileMode.Create, FileAccess.ReadWrite))
            {
                using (ZipArchive pArchive = new ZipArchive(pStream, ZipArchiveMode.Create))
                {
                    foreach (string strFile in fListFile)
                    {
                        string strPath = strFile.Substring(strDirPath.Length + 1);
                        pArchive.CreateEntryFromFile(strFile, strPath);
                    }
                }
            }
        }

        public void ExtractZip()
        {
            if (!Directory.Exists(DirectoryPath))
            {
                Directory.CreateDirectory(DirectoryPath);
            }

            using (ZipArchive pArchive = ZipFile.OpenRead(ZipFilePath))
            {
                foreach (ZipArchiveEntry pArchiveEntry in pArchive.Entries)
                {
                    string strFilePath = Path.Combine(DirectoryPath, pArchiveEntry.FullName);
                    string strSubDir = Path.GetDirectoryName(strFilePath);
                    if (!Directory.Exists(strSubDir))
                    {
                        Directory.CreateDirectory(strSubDir);
                    }

                    pArchiveEntry.ExtractToFile(strFilePath);
                }
            }
        }

        public void ExtractZip(string strRoot)
        {
            if (!Directory.Exists(strRoot))
            {
                Directory.CreateDirectory(strRoot);
            }

            using (ZipArchive pArchive = ZipFile.OpenRead(ZipFilePath))
            {
                foreach (ZipArchiveEntry pArchiveEntry in pArchive.Entries)
                {
                    string strFilePath = Path.Combine(strRoot, pArchiveEntry.FullName);
                    string strSubDir = Path.GetDirectoryName(strFilePath);
                    if (!Directory.Exists(strSubDir))
                    {
                        Directory.CreateDirectory(strSubDir);
                    }

                    pArchiveEntry.ExtractToFile(strFilePath);
                }
            }
        }
    }
}
