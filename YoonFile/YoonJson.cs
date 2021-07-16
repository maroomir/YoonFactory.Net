using System;
using System.IO;
using Newtonsoft.Json;

namespace YoonFactory.Files
{
    public class YoonJson : IYoonFile
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
                _pDocument = null;
                _disposedValue = true;
            }
        }

        ~YoonJson()
        {
            Dispose(false);
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        #endregion

        private object _pDocument;
        private Type _pTypeDocument;

        public string FilePath { get; private set; }

        public object Document
        {
            get => _pDocument;
            set
            {
                _pTypeDocument = Document.GetType();
                _pDocument = value;
            }
        }

        public YoonJson(string strPath)
        {
            FilePath = !FileFactory.VerifyFileExtension(ref strPath, ".json", false, false)
                ? strPath
                : Path.Combine(Directory.GetCurrentDirectory(), "YoonJSON.json");
        }

        public void CopyFrom(IYoonFile pFile)
        {
            if (pFile is YoonJson pJson)
            {
                FilePath = pJson.FilePath;
            }
        }

        public IYoonFile Clone()
        {
            return new YoonJson(FilePath);
        }

        public bool IsFileExist()
        {
            return FileFactory.IsFileExist(FilePath);
        }

        public bool LoadFile()
        {
            if (_pDocument == null || _pTypeDocument == null)
                throw new NullReferenceException("Document has Null Reference");
            return LoadFile(out _pDocument, _pTypeDocument);
        }

        public bool LoadFile(out object pParamData, Type pType)
        {
            string strPath = FilePath;
            if (!FileFactory.VerifyFileExtension(ref strPath, ".xml"))
            {
                Console.WriteLine("Load Json File Failure : " + FilePath + "\n");
                pParamData = null;
                return false;
            }

            StreamReader pReader;

            try
            {
                pReader = new StreamReader(strPath);
            }
            catch (Exception ex)
            {
                Console.WriteLine("LoadJson_Critical : " + ex.ToString() + "\n");
                pParamData = null;
                return false;
            }

            try
            {
                JsonSerializer pSerializer = new JsonSerializer();
                pParamData = pSerializer.Deserialize(pReader, pType);
                pReader.Close();
            }
            catch (Exception ex)
            {
                Console.WriteLine("LoadJson_Critical : " + ex.ToString() + "\n");
                pParamData = null;
                pReader.Close();
                return false;
            }

            return true;
        }

        public bool SaveFile()
        {
            if (_pDocument == null || _pTypeDocument == null)
                throw new NullReferenceException("Document has Null Reference");
            return SaveFile(_pDocument, _pTypeDocument);
        }

        public bool SaveFile(object pParamData, Type pType)
        {
            string strPath = FilePath;
            if (!FileFactory.VerifyFileExtension(ref strPath, ".xml", false, true))
            {
                Console.WriteLine("Create XML File Failure : " + FilePath + "\n");
                return false;
            }

            StreamWriter pWriter;

            try
            {
                pWriter = new StreamWriter(strPath);
            }
            catch (Exception ex)
            {
                Console.WriteLine("SaveJson_Critical : " + ex.ToString() + "\n");
                return false;
            }

            try
            {
                JsonSerializer pSerializer = new JsonSerializer();
                pSerializer.Serialize(pWriter, pParamData);
                pWriter.Close(); // IOException : Sharing violation on path 오류 수정
            }
            catch (Exception ex)
            {
                Console.WriteLine("SaveJson_Critical : " + ex.ToString() + "\n");
                pWriter.Close();
                return false;
            }

            return true;
        }
    }
}