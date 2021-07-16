using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using System.Collections;
using YoonFactory.Files;

namespace YoonFactory.Param
{
    public class YoonTemplate<T> : YoonContainer<T>, IYoonTemplate where T : IConvertible
    {
        public int No { get; set; }
        public string Name { get; set; }
        public string RootDirectory { get; set; }

        public override string ToString()
        {
            return $"{No:D2}_{Name}";
        }

        public YoonTemplate()
        {
            No = 0;
            Name = "Default";
            RootDirectory = Path.Combine(System.IO.Directory.GetCurrentDirectory(), "YoonFactory");
            _pDicParam = new Dictionary<T, YoonParameter>(DefaultComparer);
            _pListKeyOrdered = new List<T>();
        }

        public void CopyFrom(IYoonTemplate pTemplate)
        {
            if (pTemplate is not YoonTemplate<T> pTempOrigin) return;
            Clear();

            No = pTempOrigin.No;
            Name = pTempOrigin.Name;
            RootDirectory = pTempOrigin.RootDirectory;
            foreach (T pKey in pTempOrigin.Keys)
            {
                Add(pKey, pTempOrigin[pKey]);
            }
        }

        public new IYoonTemplate Clone()
        {
            YoonTemplate<T> pTemplate = new YoonTemplate<T>();
            {
                pTemplate.No = No;
                pTemplate.Name = Name;
                pTemplate.RootDirectory = RootDirectory;
                pTemplate._pDicParam = new Dictionary<T, YoonParameter>(_pDicParam, DefaultComparer);
            }
            return pTemplate;
        }

        public bool LoadTemplate()
        {
            if (RootDirectory == string.Empty || _pDicParam == null)
                return false;

            string strIniFilePath = Path.Combine(RootDirectory, @"YoonTemplate.ini");
            FilesDirectory = Path.Combine(RootDirectory, ToString());
            bool bResult = true;
            using (YoonIni pIni = new YoonIni(strIniFilePath))
            {
                pIni.LoadFile();
                No = pIni["HEAD"]["No"].ToInt(No);
                Name = pIni["HEAD"]["Name"].ToString(Name);
                int nCount = pIni["HEAD"]["Count"].ToInt(0);
                for (int iParam = 0; iParam < nCount; iParam++)
                {
                    T pKey = pIni["KEY"][iParam.ToString()].To(default(T));
                    if (!LoadValue(pKey))
                        bResult = false;
                }
            }

            return bResult;
        }

        public bool SaveTemplate()
        {
            if (RootDirectory == string.Empty || _pDicParam == null)
                return false;

            string strIniFilePath = Path.Combine(RootDirectory, @"YoonTemplate.ini");
            FilesDirectory = Path.Combine(RootDirectory, ToString());
            bool bResult = true;
            using (YoonIni pIni = new YoonIni(strIniFilePath))
            {
                int iParam = 0;
                pIni["HEAD"]["No"] = No;
                pIni["HEAD"]["Name"] = Name;
                pIni["HEAD"]["Count"] = Count;
                foreach (T key in Keys)
                {
                    pIni["KEY"][(iParam++).ToString()] = key.ToString();
                    if (!SaveValue(key))
                        bResult = false;
                }

                pIni.SaveFile();
            }

            return bResult;
        }
    }

    public class YoonTemplate<TKey, TValue> : IYoonTemplate, IYoonSection<TKey, TValue>
        where TKey : IConvertible where TValue : IYoonTemplate
    {
        #region IDisposable Support

        ~YoonTemplate()
        {
            this.Dispose(false);
        }

        private bool disposed;

        public void Dispose()
        {
            this.Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (this.disposed) return;
            if (disposing)
            {
                ////  .Net Framework에 의해 관리되는 리소스를 여기서 정리합니다.
            }

            //// .NET Framework에 의하여 관리되지 않는 외부 리소스들을 여기서 정리합니다.
            Clear();
            _pDicTemplate = null;
            _pListKeyOrdered = null;
            this.disposed = true;
        }

        #endregion

        public static IEqualityComparer<TKey> DefaultComparer = new CaseInsensitiveComparer();

        class CaseInsensitiveComparer : IEqualityComparer<TKey>
        {
            public bool Equals(TKey x, TKey y)
            {
                return x.Equals(y);
            }

            public int GetHashCode(TKey obj)
            {
                return obj.GetHashCode();
            }
        }

        public int No { get; set; }
        public string Name { get; set; }
        public string RootDirectory { get; set; }
        public bool IsMarkNumber { get; set; } = true;

        public override string ToString()
        {
            return (IsMarkNumber) ? Name : string.Format("{0:D2}_{1}", No, Name);
        }

        protected Dictionary<TKey, TValue> _pDicTemplate;
        protected List<TKey> _pListKeyOrdered;

        public IEqualityComparer<TKey> Comparer => _pDicTemplate.Comparer;

        public TValue this[int nIndex]
        {
            get
            {
                if (_pListKeyOrdered == null)
                {
                    throw new InvalidOperationException(
                        "Cannot index ParameterContainer using integer key: section was not ordered.");
                }

                if (nIndex < 0 || nIndex >= _pListKeyOrdered.Count)
                {
                    throw new IndexOutOfRangeException("Index must be within the bounds." + Environment.NewLine +
                                                       "Parameter name: index");
                }

                return _pDicTemplate[_pListKeyOrdered[nIndex]];
            }
            set
            {
                if (_pListKeyOrdered == null)
                {
                    throw new InvalidOperationException(
                        "Cannot index ParameterContainer using integer key: section was not ordered.");
                }

                if (nIndex < 0 || nIndex >= _pListKeyOrdered.Count)
                {
                    throw new IndexOutOfRangeException("Index must be within the bounds." + Environment.NewLine +
                                                       "Parameter name: index");
                }

                var key = _pListKeyOrdered[nIndex];
                _pDicTemplate[key] = value;
            }
        }

        public TValue this[TKey pKey]
        {
            get => _pDicTemplate.TryGetValue(pKey, out var param) ? param : default(TValue);
            set
            {
                if (_pListKeyOrdered != null && !_pListKeyOrdered.Contains(pKey, Comparer))
                {
                    _pListKeyOrdered.Add(pKey);
                }

                _pDicTemplate[pKey] = value;
            }
        }

        public YoonTemplate(string strName)
        {
            No = 0;
            Name = strName;
            IsMarkNumber = false;
            RootDirectory = Path.Combine(System.IO.Directory.GetCurrentDirectory(), "YoonFactory");
            this._pDicTemplate = new Dictionary<TKey, TValue>(DefaultComparer);
            this._pListKeyOrdered = new List<TKey>();
        }

        public YoonTemplate(int nNo, string strName)
        {
            No = nNo;
            Name = strName;
            IsMarkNumber = true;
            RootDirectory = Path.Combine(System.IO.Directory.GetCurrentDirectory(), "YoonFactory");
            this._pDicTemplate = new Dictionary<TKey, TValue>(DefaultComparer);
            this._pListKeyOrdered = new List<TKey>();
        }

        public YoonTemplate(YoonTemplate<TKey, TValue> pTemplate)
        {
            No = pTemplate.No;
            Name = pTemplate.Name;
            IsMarkNumber = pTemplate.IsMarkNumber;
            RootDirectory = pTemplate.RootDirectory;
            this._pDicTemplate = new Dictionary<TKey, TValue>(pTemplate._pDicTemplate, DefaultComparer);
            this._pListKeyOrdered = new List<TKey>();
        }

        public void CopyFrom(IYoonTemplate pTemplate)
        {
            if (pTemplate is not YoonTemplate<TKey, TValue> pTemplateOrigin) return;
            Clear();

            No = pTemplateOrigin.No;
            Name = pTemplateOrigin.Name;
            IsMarkNumber = pTemplateOrigin.IsMarkNumber;
            RootDirectory = pTemplateOrigin.RootDirectory;
            foreach (TKey pKey in pTemplateOrigin.Keys)
            {
                Add(pKey, pTemplateOrigin[pKey]);
            }
        }

        public IYoonTemplate Clone()
        {
            return new YoonTemplate<TKey, TValue>(this);
        }

        public void Clear()
        {
            _pDicTemplate?.Clear();
            _pListKeyOrdered?.Clear();
        }

        public bool LoadTemplate()
        {
            if (RootDirectory == string.Empty || _pDicTemplate == null)
            {
                throw new InvalidOperationException(
                    "LoadTemplate() on Template<TKey, TValue> : Template Info isnot contained");
            }

            bool bResult = true;
            string strIniFilePath = Path.Combine(RootDirectory, $@"{ToString()}Template.ini");
            using (YoonIni pIni = new YoonIni(strIniFilePath))
            {
                pIni.LoadFile();
                if (No != pIni["HEAD"]["No"].ToInt(No))
                {
                    throw new FileLoadException(
                        "LoadTemplate() on Template<TKey, TValue> : Template No isnot matching");
                }

                if (Name != pIni["HEAD"]["Name"].ToString(Name))
                {
                    throw new FileLoadException(
                        "LoadTemplate() on Template<TKey, TValue> : Template Name isnot Matching");
                }

                int nCount = pIni["HEAD"]["Count"].ToInt(0);
                for (int iParam = 0; iParam < nCount; iParam++)
                {
                    TKey key = pIni["KEY"][iParam.ToString()].To(default(TKey));
                    if (!_pDicTemplate.ContainsKey(key))
                        Add(key, default(TValue));
                    _pDicTemplate[key].No = iParam;
                    _pDicTemplate[key].Name = key.ToString();
                    _pDicTemplate[key].RootDirectory = Path.Combine(RootDirectory, ToString());
                    if (!_pDicTemplate[key].LoadTemplate())
                        bResult = false;
                }
            }

            return bResult;
        }

        public bool SaveTemplate()
        {
            if (RootDirectory == string.Empty || _pDicTemplate == null)
            {
                throw new InvalidOperationException(
                    "SaveTemplate() on Template<TKey, TValue> : Template Info isnot contained");
            }

            bool bResult = true;
            string strIniFilePath = Path.Combine(RootDirectory, string.Format(@"{0}Template.ini", ToString()));
            using (YoonIni pIni = new YoonIni(strIniFilePath))
            {
                int iParam = 0;
                pIni["HEAD"]["No"] = No;
                pIni["HEAD"]["Name"] = Name;
                pIni["HEAD"]["Count"] = Count;
                foreach (TKey key in _pDicTemplate.Keys)
                {
                    pIni["KEY"][iParam.ToString()] = key.ToString();
                    _pDicTemplate[key].No = iParam;
                    _pDicTemplate[key].Name = key.ToString();
                    _pDicTemplate[key].RootDirectory = Path.Combine(RootDirectory, ToString());
                    if (!_pDicTemplate[key].SaveTemplate())
                        bResult = false;
                    iParam++;
                }

                pIni.SaveFile();
            }

            return bResult;
        }

        public TKey KeyOf(int nIndex)
        {
            if (_pListKeyOrdered == null)
            {
                throw new InvalidOperationException("Cannot call KeyOf(int) on Template: section was not ordered.");
            }

            if (nIndex < 0 || nIndex > _pListKeyOrdered.Count)
            {
                throw new IndexOutOfRangeException("Index must be within the bounds." + Environment.NewLine +
                                                   "Parameter name: nIndex");
            }

            return _pListKeyOrdered[nIndex];
        }

        public int IndexOf(TKey pKey)
        {
            if (_pListKeyOrdered == null)
            {
                throw new InvalidOperationException("Cannot call IndexOf(TKey) on Template: section was not ordered.");
            }

            return IndexOf(pKey, 0, _pListKeyOrdered.Count);
        }

        public int IndexOf(TKey pKey, int nIndex)
        {
            if (_pListKeyOrdered == null)
            {
                throw new InvalidOperationException(
                    "Cannot call IndexOf(TKey, int) on Template: section was not ordered.");
            }

            return IndexOf(pKey, nIndex, _pListKeyOrdered.Count - nIndex);
        }

        public int IndexOf(TKey pKey, int nIndex, int nCount)
        {
            if (_pListKeyOrdered == null)
            {
                throw new InvalidOperationException(
                    "Cannot call IndexOf(TKey, int, int) on Template: section was not ordered.");
            }

            if (nIndex < 0 || nIndex > _pListKeyOrdered.Count)
            {
                throw new IndexOutOfRangeException("Index must be within the bounds." + Environment.NewLine +
                                                   "Parameter name: nIndex");
            }

            if (nCount < 0)
            {
                throw new IndexOutOfRangeException("Count cannot be less than zero." + Environment.NewLine +
                                                   "Parameter name: nCount");
            }

            if (nIndex + nCount > _pListKeyOrdered.Count)
            {
                throw new ArgumentException(
                    "Index and count were out of bounds for the array or count is greater than the number of elements from index to the end of the source collection.");
            }

            var nEnd = nIndex + nCount;
            for (int i = nIndex; i < nEnd; i++)
            {
                if (Comparer.Equals(_pListKeyOrdered[i], pKey))
                {
                    return i;
                }
            }

            return -1;
        }

        public int LastIndexOf(TKey pKey)
        {
            if (_pListKeyOrdered == null)
            {
                throw new InvalidOperationException(
                    "Cannot call LastIndexOf(TKey) on Template: section was not ordered.");
            }

            return LastIndexOf(pKey, 0, _pListKeyOrdered.Count);
        }

        public int LastIndexOf(TKey pKey, int nIndex)
        {
            if (_pListKeyOrdered == null)
            {
                throw new InvalidOperationException(
                    "Cannot call LastIndexOf(T, int) on Template: section was not ordered.");
            }

            return LastIndexOf(pKey, nIndex, _pListKeyOrdered.Count - nIndex);
        }

        public int LastIndexOf(TKey pKey, int nIndex, int nCount)
        {
            if (_pListKeyOrdered == null)
            {
                throw new InvalidOperationException(
                    "Cannot call LastIndexOf(T, int, int) on Template: section was not ordered.");
            }

            if (nIndex < 0 || nIndex > _pListKeyOrdered.Count)
            {
                throw new IndexOutOfRangeException("Index must be within the bounds." + Environment.NewLine +
                                                   "Parameter name: nIndex");
            }

            if (nCount < 0)
            {
                throw new IndexOutOfRangeException("Count cannot be less than zero." + Environment.NewLine +
                                                   "Parameter name : nCount");
            }

            if (nIndex + nCount > _pListKeyOrdered.Count)
            {
                throw new ArgumentException(
                    "Index and Count were out of bounds for the array or count is greater than the number of elements from index to the end of the source collection.");
            }

            var end = nIndex + nCount;
            for (int i = end - 1; i >= nIndex; i--)
            {
                if (Comparer.Equals(_pListKeyOrdered[i], pKey))
                {
                    return i;
                }
            }

            return -1;
        }

        public void Insert(int nIndex, TKey pKey, TValue pValue)
        {
            if (_pListKeyOrdered == null)
            {
                throw new InvalidOperationException(
                    "Cannot call Insert(int, TKey, TValue) on Template: section was not ordered.");
            }

            if (nIndex < 0 || nIndex > _pListKeyOrdered.Count)
            {
                throw new IndexOutOfRangeException("Index must be within the bounds." + Environment.NewLine +
                                                   "Parameter name: nIndex");
            }

            _pDicTemplate.Add(pKey, pValue);
            _pListKeyOrdered.Insert(nIndex, pKey);
        }

        public void InsertRange(int nIndex, IEnumerable<KeyValuePair<TKey, TValue>> pCollection)
        {
            if (_pListKeyOrdered == null)
            {
                throw new InvalidOperationException(
                    "Cannot call InsertRange(int, IEnumerable<KeyValuePair<TTKey, TValue>>) on Template: section was not ordered.");
            }

            if (pCollection == null)
            {
                throw new ArgumentNullException("Value cannot be null." + Environment.NewLine +
                                                "Parameter name: pCollection");
            }

            if (nIndex < 0 || nIndex > _pListKeyOrdered.Count)
            {
                throw new IndexOutOfRangeException("Index must be within the bounds." + Environment.NewLine +
                                                   "Parameter name: nIndex");
            }

            foreach (var pPair in pCollection)
            {
                Insert(nIndex, pPair.Key, pPair.Value);
                nIndex++;
            }
        }

        public void RemoveAt(int nIndex)
        {
            if (_pListKeyOrdered == null)
            {
                throw new InvalidOperationException("Cannot call RemoveAt(int) on Template: section was not ordered.");
            }

            if (nIndex < 0 || nIndex > _pListKeyOrdered.Count)
            {
                throw new IndexOutOfRangeException("Index must be within the bounds." + Environment.NewLine +
                                                   "Parameter name: nIndex");
            }

            var key = _pListKeyOrdered[nIndex];
            _pListKeyOrdered.RemoveAt(nIndex);
            _pDicTemplate.Remove(key);
        }

        public void RemoveRange(int nIndex, int nCount)
        {
            if (_pListKeyOrdered == null)
            {
                throw new InvalidOperationException(
                    "Cannot call RemoveRange(int, int) on Template: section was not ordered.");
            }

            if (nIndex < 0 || nIndex > _pListKeyOrdered.Count)
            {
                throw new IndexOutOfRangeException("Index must be within the bounds." + Environment.NewLine +
                                                   "Parameter name: nIndex");
            }

            if (nCount < 0)
            {
                throw new IndexOutOfRangeException("Count cannot be less than zero." + Environment.NewLine +
                                                   "Parameter name: nCount");
            }

            if (nIndex + nCount > _pListKeyOrdered.Count)
            {
                throw new ArgumentException(
                    "Index and count were out of bounds for the array or count is greater than the number of elements from index to the end of the source collection.");
            }

            for (int i = 0; i < nCount; i++)
            {
                RemoveAt(nIndex);
            }
        }

        public void Reverse()
        {
            if (_pListKeyOrdered == null)
            {
                throw new InvalidOperationException("Cannot call Reverse() on Template: section was not ordered.");
            }

            _pListKeyOrdered.Reverse();
        }

        public void Reverse(int nIndex, int nCount)
        {
            if (_pListKeyOrdered == null)
            {
                throw new InvalidOperationException(
                    "Cannot call Reverse(int, int) on Template: section was not ordered.");
            }

            if (nIndex < 0 || nIndex > _pListKeyOrdered.Count)
            {
                throw new IndexOutOfRangeException("Index must be within the bounds." + Environment.NewLine +
                                                   "Parameter name: nIndex");
            }

            if (nCount < 0)
            {
                throw new IndexOutOfRangeException("Count cannot be less than zero." + Environment.NewLine +
                                                   "Parameter name: nCount");
            }

            if (nIndex + nCount > _pListKeyOrdered.Count)
            {
                throw new ArgumentException(
                    "Index and count were out of bounds for the array or count is greater than the number of elements from index to the end of the source collection.");
            }

            _pListKeyOrdered.Reverse(nIndex, nCount);
        }

        public ICollection<TValue> GetOrderedValues()
        {
            if (_pListKeyOrdered == null)
            {
                throw new InvalidOperationException(
                    "Cannot call GetOrderedValues() on Template: section was not ordered.");
            }

            var list = new List<TValue>();
            for (int i = 0; i < _pListKeyOrdered.Count; i++)
            {
                list.Add(_pDicTemplate[_pListKeyOrdered[i]]);
            }

            return list;
        }

        public void Add(TKey pKey, TValue pValue)
        {
            _pDicTemplate.Add(pKey, pValue);
            _pListKeyOrdered?.Add(pKey);
        }

        public bool ContainsKey(TKey pKey)
        {
            return _pDicTemplate.ContainsKey(pKey);
        }

        public ICollection<TKey> Keys =>
            (_pListKeyOrdered != null) ? (ICollection<TKey>) _pListKeyOrdered : _pDicTemplate.Keys;

        public ICollection<TValue> Values => _pDicTemplate.Values;

        public bool Remove(TKey pKey)
        {
            var ret = _pDicTemplate.Remove(pKey);
            if (_pListKeyOrdered != null && ret)
            {
                for (int i = 0; i < _pListKeyOrdered.Count; i++)
                {
                    if (Comparer.Equals(_pListKeyOrdered[i], pKey))
                    {
                        _pListKeyOrdered.RemoveAt(i);
                        break;
                    }
                }
            }

            return ret;
        }

        public bool TryGetValue(TKey pKey, out TValue pValue)
        {
            return _pDicTemplate.TryGetValue(pKey, out pValue);
        }

        public int Count => _pDicTemplate.Count;

        void ICollection<KeyValuePair<TKey, TValue>>.Add(KeyValuePair<TKey, TValue> pCollection)
        {
            ((IDictionary<TKey, TValue>) _pDicTemplate).Add(pCollection);
            _pListKeyOrdered?.Add(pCollection.Key);
        }

        bool ICollection<KeyValuePair<TKey, TValue>>.Contains(KeyValuePair<TKey, TValue> pCollection)
        {
            return ((IDictionary<TKey, TValue>) _pDicTemplate).Contains(pCollection);
        }

        void ICollection<KeyValuePair<TKey, TValue>>.CopyTo(KeyValuePair<TKey, TValue>[] pArray, int nIndexArray)
        {
            ((IDictionary<TKey, TValue>) _pDicTemplate).CopyTo(pArray, nIndexArray);
        }

        bool ICollection<KeyValuePair<TKey, TValue>>.IsReadOnly =>
            ((IDictionary<TKey, TValue>) _pDicTemplate).IsReadOnly;

        bool ICollection<KeyValuePair<TKey, TValue>>.Remove(KeyValuePair<TKey, TValue> pCollection)
        {
            var ret = ((IDictionary<TKey, TValue>) _pDicTemplate).Remove(pCollection);
            if (_pListKeyOrdered != null && ret)
            {
                for (int i = 0; i < _pListKeyOrdered.Count; i++)
                {
                    if (Comparer.Equals(_pListKeyOrdered[i], pCollection.Key))
                    {
                        _pListKeyOrdered.RemoveAt(i);
                        break;
                    }
                }
            }

            return ret;
        }

        public IEnumerator<KeyValuePair<TKey, TValue>> GetEnumerator()
        {
            if (_pListKeyOrdered != null)
            {
                return GetOrderedEnumerator();
            }
            else
            {
                return _pDicTemplate.GetEnumerator();
            }
        }

        private IEnumerator<KeyValuePair<TKey, TValue>> GetOrderedEnumerator()
        {
            foreach (var key in _pListKeyOrdered)
            {
                yield return new KeyValuePair<TKey, TValue>(key, _pDicTemplate[key]);
            }
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        public static implicit operator YoonTemplate<TKey, TValue>(Dictionary<TKey, TValue> pDic)
        {
            return new YoonTemplate<TKey, TValue>(pDic);
        }

        public static explicit operator Dictionary<TKey, TValue>(YoonTemplate<TKey, TValue> pContainer)
        {
            return pContainer._pDicTemplate;
        }
    }
}
