using System;
using System.IO;
using System.Xml.Serialization;

namespace YoonFactory.Files
{
    public class YoonXml : IYoonFile
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
                _pDocument = null;
            }
        }

        ~YoonXml()
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

        public YoonXml(string strPath)
        {
            FilePath = !FileFactory.VerifyFileExtension(ref strPath, ".xml", false, false)
                ? strPath
                : Path.Combine(Directory.GetCurrentDirectory(), "YoonXml.xml");
        }

        public void CopyFrom(IYoonFile pFile)
        {
            if (pFile is YoonXml pXml)
            {
                FilePath = pXml.FilePath;
            }
        }

        public IYoonFile Clone()
        {
            return new YoonXml(FilePath);
        }

        public bool IsFileExist()
        {
            return FileFactory.VerifyFilePath(FilePath, false);
        }

        public bool LoadFile()
        {
            if (_pDocument == null || _pTypeDocument == null)
                throw new NullReferenceException("Document has Null Reference");
            return LoadFile(out _pDocument, _pTypeDocument);
        }

        public bool LoadFile(out object pParamData, Type pType)
        {
            StreamReader pReader;

            string strPath = FilePath;
            if (!FileFactory.VerifyFileExtension(ref strPath, ".xml"))
            {
                Console.WriteLine("Load XML File Failure : " + FilePath + "\n");
                pParamData = null;
                return false;
            }

            try
            {
                pReader = new StreamReader(strPath);
            }
            catch (Exception ex)
            {
                Console.WriteLine("LoadXML_Critical : " + ex.ToString() + "\n");
                pParamData = null;
                return false;
            }

            try
            {
                XmlSerializer pSerializer = new XmlSerializer(pType);
                pParamData = pSerializer.Deserialize(pReader);
                pReader.Close(); // IOException : Sharing violation on path 오류 수정
            }
            catch (Exception ex)
            {
                Console.WriteLine("LoadXML_Critical : " + ex.ToString() + "\n");
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
                Console.WriteLine("SaveXML_Critical : " + ex.ToString() + "\n");
                return false;
            }

            try
            {
                XmlSerializer pSerializer = new XmlSerializer(pType);
                pSerializer.Serialize(pWriter, pParamData);
                pWriter.Close(); // IOException : Sharing violation on path 오류 수정
            }
            catch (Exception ex)
            {
                Console.WriteLine("SaveXML_Critical : " + ex.ToString() + "\n");
                pWriter.Close();
                return false;
            }

            return true;
        }
    }
}