using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using YoonFactory.Files.Ini;

namespace YoonFactory.Files
{
    /* Ini 파일을 관리하는 Class - 공용, @2020 */
    public class YoonIni : IYoonFile, IEnumerable<KeyValuePair<string, IniSection>>, IDictionary<string, IniSection>
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
                if (_pDicSections != null)
                    _pDicSections.Clear();
                _pDicSections = null;
                _disposedValue = true;
            }
        }

         ~YoonIni() {
           Dispose(false);
         }

         public void Dispose()
         {
             Dispose(true);
             GC.SuppressFinalize(this);
         }

         #endregion


        public static IEqualityComparer<string> DefaultComparer = new CaseInsensitiveStringComparer();

        class CaseInsensitiveStringComparer : IEqualityComparer<string>
        {
            public bool Equals(string x, string y)
            {
                return String.Compare(x, y, true) == 0;
            }

            public int GetHashCode(string obj)
            {
                return obj.ToLowerInvariant().GetHashCode();
            }
        }

        private Dictionary<string, IniSection> _pDicSections;
        public IEqualityComparer<string> StrComparer;
        public string FilePath { get; private set; }

        public bool SaveEmptySections;

        public YoonIni(string strPath) : this(strPath, DefaultComparer)
        {
            //
        }

        public YoonIni(string strPath, IEqualityComparer<string> stringComparer)
        {

            FilePath = FileFactory.VerifyFileExtension(ref strPath, ".ini", true, true)
                ? strPath
                : Path.Combine(Directory.GetCurrentDirectory(), "YoonFactory", "YoonFactory.ini");
            StrComparer = stringComparer;
            _pDicSections = new Dictionary<string, IniSection>(StrComparer);
        }

        public void CopyFrom(IYoonFile pFile)
        {
            if (pFile is not YoonIni pIni) return;
            FilePath = pIni.FilePath;
            StrComparer = pIni.StrComparer;
            _pDicSections = new Dictionary<string, IniSection>(StrComparer);
        }

        public IYoonFile Clone()
        {
            return new YoonIni(FilePath, StrComparer);
        }

        public bool IsFileExist()
        {
            return FileFactory.VerifyFilePath(FilePath, false);
        }

        public bool LoadFile()
        {
            string strPath = FilePath;
            if (!FileFactory.VerifyFileExtension(ref strPath, ".ini"))
                return false;

            using (FileStream fs = new FileStream(strPath, FileMode.Open, FileAccess.Read))
            {
                using (StreamReader sr = new StreamReader(fs))
                {
                    IniSection pSection = null;

                    while (!sr.EndOfStream)
                    {
                        string strLine = sr.ReadLine();

                        if (strLine == null) continue;
                        string strTrimStart = strLine.TrimStart();

                        if (strTrimStart.Length <= 0) continue;
                        if (strTrimStart[0] == '[')
                        {
                            int nSectionEnd = strTrimStart.IndexOf(']');
                            if (nSectionEnd <= 0) continue;
                            string strSectionName = strTrimStart.Substring(1, nSectionEnd - 1).Trim();
                            pSection = new IniSection(StrComparer);
                            _pDicSections[strSectionName] = pSection;
                        }
                        else if (pSection != null && strTrimStart[0] != ';')
                        {
                            string strKey;
                            IniValue pValue;

                            if (GetValue(strLine, out strKey, out pValue))
                            {
                                pSection[strKey] = pValue;
                            }
                        }
                    }
                }
            }

            return true;
        }

        public bool SaveFile()
        {
            return SaveFile(FileMode.Create);
        }

        public bool SaveFile(FileMode nMode)
        {
            string strPath = FilePath;
            if (!FileFactory.VerifyFileExtension(ref strPath, ".ini", true, true))
                return false;

            using (FileStream fs = new FileStream(strPath, nMode, FileAccess.Write))
            {
                using (StreamWriter pWriter = new StreamWriter(fs))
                {
                    foreach (var pSection in _pDicSections)
                    {
                        if (pSection.Value.Count > 0 || SaveEmptySections)
                        {
                            pWriter.WriteLine($"[{pSection.Key.Trim()}]");
                            foreach (var pPair in pSection.Value)
                            {
                                pWriter.WriteLine($"{pPair.Key}={pPair.Value}");
                            }

                            pWriter.WriteLine("");
                        }
                    }
                }
            }

            return true;
        }

        public bool SaveFile(Stream stream)
        {
            using (StreamWriter writer = new StreamWriter(stream))
            {
                foreach (var section in _pDicSections)
                {
                    if (section.Value.Count <= 0 && !SaveEmptySections) continue;
                    writer.WriteLine($"[{section.Key.Trim()}]");
                    foreach (var pPair in section.Value)
                    {
                        writer.WriteLine($"{pPair.Key}={pPair.Value}");
                    }

                    writer.WriteLine("");
                }
            }

            return true;
        }

        private bool GetValue(string strLine, out string strKey, out IniValue pValue)
        {
            int nAssignIndex = strLine.IndexOf('=');
            if (nAssignIndex <= 0)
            {
                strKey = null;
                pValue = null;
                return false;
            }

            strKey = strLine.Substring(0, nAssignIndex).Trim();
            string strValue = strLine.Substring(nAssignIndex + 1);
            pValue = new IniValue(strValue);
            return true;
        }

        public bool ContainsSection(string strSection)
        {
            return _pDicSections.ContainsKey(strSection);
        }

        public bool TryGetSection(string strSection, out IniSection pSection)
        {
            return _pDicSections.TryGetValue(strSection, out pSection);
        }

        bool IDictionary<string, IniSection>.TryGetValue(string strKey, out IniSection pSection)
        {
            return TryGetSection(strKey, out pSection);
        }

        public bool Remove(string strSection)
        {
            return _pDicSections.Remove(strSection);
        }

        public IniSection Add(string strSection, Dictionary<string, IniValue> pValue)
        {
            return Add(strSection, new IniSection(pValue, StrComparer));
        }

        public IniSection Add(string strSection, IniSection pSection)
        {
            if (!Equals(pSection.Comparer, StrComparer))
            {
                pSection = new IniSection(pSection, StrComparer);
            }

            _pDicSections.Add(strSection, pSection);
            return pSection;
        }

        public IniSection Add(string strSection)
        {
            IniSection value = new IniSection(StrComparer);
            _pDicSections.Add(strSection, value);
            return value;
        }

        void IDictionary<string, IniSection>.Add(string strKey, IniSection pSection)
        {
            Add(strKey, pSection);
        }

        bool IDictionary<string, IniSection>.ContainsKey(string strKey)
        {
            return ContainsSection(strKey);
        }

        public ICollection<string> Keys => _pDicSections.Keys;

        public ICollection<IniSection> Values => _pDicSections.Values;

        void ICollection<KeyValuePair<string, IniSection>>.Add(KeyValuePair<string, IniSection> item)
        {
            ((IDictionary<string, IniSection>) _pDicSections).Add(item);
        }

        public void Clear()
        {
            _pDicSections.Clear();
        }

        bool ICollection<KeyValuePair<string, IniSection>>.Contains(KeyValuePair<string, IniSection> item)
        {
            return ((IDictionary<string, IniSection>) _pDicSections).Contains(item);
        }

        void ICollection<KeyValuePair<string, IniSection>>.CopyTo(KeyValuePair<string, IniSection>[] array,
            int arrayIndex)
        {
            ((IDictionary<string, IniSection>) _pDicSections).CopyTo(array, arrayIndex);
        }

        public int Count => _pDicSections.Count;

        bool ICollection<KeyValuePair<string, IniSection>>.IsReadOnly =>
            ((IDictionary<string, IniSection>) _pDicSections).IsReadOnly;

        bool ICollection<KeyValuePair<string, IniSection>>.Remove(KeyValuePair<string, IniSection> item)
        {
            return ((IDictionary<string, IniSection>) _pDicSections).Remove(item);
        }

        public IEnumerator<KeyValuePair<string, IniSection>> GetEnumerator()
        {
            return _pDicSections.GetEnumerator();
        }

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        public IniSection this[string strSection]
        {
            get
            {
                if (_pDicSections.TryGetValue(strSection, out var pSection))
                {
                    return pSection;
                }

                pSection = new IniSection(StrComparer);
                _pDicSections[strSection] = pSection;
                return pSection;
            }
            set
            {
                IniSection pSection = value;
                if (!Equals(pSection.Comparer, StrComparer))
                {
                    pSection = new IniSection(pSection, StrComparer);
                }

                _pDicSections[strSection] = pSection;
            }
        }

        public string GetContents()
        {
            using (MemoryStream stream = new MemoryStream())
            {
                SaveFile(stream);
                stream.Flush();
                var builder = new StringBuilder(Encoding.UTF8.GetString(stream.ToArray()));
                return builder.ToString();
            }
        }
    }
}
