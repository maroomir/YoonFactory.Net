using System;
using System.Collections.Generic;
using System.Linq;

namespace YoonFactory.Param
{
    public class YoonContainer<T> : IYoonContainer, IYoonContainer<T, YoonParameter> where T : IConvertible
    {
        #region IDisposable Support

        ~YoonContainer()
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
            _pDicParam = null;
            _pListKeyOrdered = null;
            this.disposed = true;
        }

        #endregion

        public static IEqualityComparer<T> DefaultComparer = new CaseInsensitiveComparer();

        class CaseInsensitiveComparer : IEqualityComparer<T>
        {
            public bool Equals(T x, T y)
            {
                return x.Equals(y);
            }

            public int GetHashCode(T obj)
            {
                return obj.GetHashCode();
            }
        }

        public string FilesDirectory { get; set; }

        protected Dictionary<T, YoonParameter> _pDicParam;
        protected List<T> _pListKeyOrdered;

        public IEqualityComparer<T> Comparer => _pDicParam.Comparer;

        public YoonParameter this[int nIndex]
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

                return _pDicParam[_pListKeyOrdered[nIndex]];
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
                _pDicParam[key] = value;
            }
        }

        public YoonParameter this[T pKey]
        {
            get => _pDicParam.TryGetValue(pKey, out YoonParameter pParam) ? pParam : new YoonParameter();
            set
            {
                if (_pListKeyOrdered != null && !_pListKeyOrdered.Contains(pKey, Comparer))
                {
                    _pListKeyOrdered.Add(pKey);
                }

                _pDicParam[pKey] = value;
            }
        }

        public YoonContainer()
        {
            _pDicParam = new Dictionary<T, YoonParameter>(DefaultComparer);
            _pListKeyOrdered = new List<T>();
        }

        public YoonContainer(Dictionary<T, YoonParameter> pDic)
        {
            _pDicParam = new Dictionary<T, YoonParameter>(pDic, DefaultComparer);
            _pListKeyOrdered = new List<T>(pDic.Keys);
        }

        public YoonContainer(YoonContainer<T> pContainer)
        {
            _pDicParam = new Dictionary<T, YoonParameter>(pContainer._pDicParam, DefaultComparer);
            _pListKeyOrdered = new List<T>(pContainer._pListKeyOrdered);
        }

        public void CopyFrom(IYoonContainer pContainer)
        {
            if (pContainer is not YoonContainer<T> pParamContainer) return;
            Clear();
            foreach (T pKey in pParamContainer.Keys)
            {
                Add(pKey, pParamContainer[pKey]);
            }
        }

        IYoonContainer IYoonContainer.Clone()
        {
            return new YoonContainer<T>(this);
        }

        public IYoonContainer<T, YoonParameter> Clone()
        {
            return new YoonContainer<T>(this);
        }

        public void Clear()
        {
            _pDicParam?.Clear();
            _pListKeyOrdered?.Clear();
        }

        public bool LoadValue(T pKey)
        {
            if (FilesDirectory == string.Empty || pKey == null)
                return false;

            if (!_pDicParam.ContainsKey(pKey))
                Add(pKey, new YoonParameter());
            _pDicParam[pKey].RootDirectory = FilesDirectory;
            return _pDicParam[pKey].LoadParameter(pKey.ToString());
        }

        public bool SaveValue(T pKey)
        {
            if (FilesDirectory == string.Empty || pKey == null || !_pDicParam.ContainsKey(pKey))
                return false;

            _pDicParam[pKey].RootDirectory = FilesDirectory;
            return _pDicParam[pKey].SaveParameter(pKey.ToString());
        }

        public T KeyOf(int nIndex)
        {
            if (_pListKeyOrdered == null)
            {
                throw new InvalidOperationException(
                    "Cannot call KeyOf(int) on ResultContainer: section was not ordered.");
            }

            if (nIndex < 0 || nIndex > _pListKeyOrdered.Count)
            {
                throw new IndexOutOfRangeException("Index must be within the bounds." + Environment.NewLine +
                                                   "Parameter name: nIndex");
            }

            return _pListKeyOrdered[nIndex];
        }

        public int IndexOf(T pKey)
        {
            if (_pListKeyOrdered == null)
            {
                throw new InvalidOperationException(
                    "Cannot call IndexOf(T) on ParameterContainer: section was not ordered.");
            }

            return IndexOf(pKey, 0, _pListKeyOrdered.Count);
        }

        public int IndexOf(T pKey, int nIndex)
        {
            if (_pListKeyOrdered == null)
            {
                throw new InvalidOperationException(
                    "Cannot call IndexOf(T, int) on ParameterContainer: section was not ordered.");
            }

            return IndexOf(pKey, nIndex, _pListKeyOrdered.Count - nIndex);
        }

        public int IndexOf(T pKey, int nIndex, int nCount)
        {
            if (_pListKeyOrdered == null)
            {
                throw new InvalidOperationException(
                    "Cannot call IndexOf(T, int, int) on ParameterContainer: section was not ordered.");
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

            int nEnd = nIndex + nCount;
            for (int i = nIndex; i < nEnd; i++)
            {
                if (Comparer.Equals(_pListKeyOrdered[i], pKey))
                {
                    return i;
                }
            }

            return -1;
        }

        public int LastIndexOf(T strKey)
        {
            if (_pListKeyOrdered == null)
            {
                throw new InvalidOperationException(
                    "Cannot call LastIndexOf(T) on ParameterContainer: section was not ordered.");
            }

            return LastIndexOf(strKey, 0, _pListKeyOrdered.Count);
        }

        public int LastIndexOf(T strKey, int nIndex)
        {
            if (_pListKeyOrdered == null)
            {
                throw new InvalidOperationException(
                    "Cannot call LastIndexOf(T, int) on ParameterContainer: section was not ordered.");
            }

            return LastIndexOf(strKey, nIndex, _pListKeyOrdered.Count - nIndex);
        }

        public int LastIndexOf(T pKey, int nIndex, int nCount)
        {
            if (_pListKeyOrdered == null)
            {
                throw new InvalidOperationException(
                    "Cannot call LastIndexOf(T, int, int) on ParameterContainer: section was not ordered.");
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

            int nEnd = nIndex + nCount;
            for (int i = nEnd - 1; i >= nIndex; i--)
            {
                if (Comparer.Equals(_pListKeyOrdered[i], pKey))
                {
                    return i;
                }
            }

            return -1;
        }

        public void Insert(int nIndex, T pKey, YoonParameter pValue)
        {
            if (_pListKeyOrdered == null)
            {
                throw new InvalidOperationException(
                    "Cannot call Insert(int, T, YoonParameter) on ParameterContainer: section was not ordered.");
            }

            if (nIndex < 0 || nIndex > _pListKeyOrdered.Count)
            {
                throw new IndexOutOfRangeException("Index must be within the bounds." + Environment.NewLine +
                                                   "Parameter name: nIndex");
            }

            _pDicParam.Add(pKey, pValue);
            _pListKeyOrdered.Insert(nIndex, pKey);
        }

        public void InsertRange(int nIndex, IEnumerable<KeyValuePair<T, YoonParameter>> pCollection)
        {
            if (_pListKeyOrdered == null)
            {
                throw new InvalidOperationException(
                    "Cannot call InsertRange(int, IEnumerable<KeyValuePair<T, YoonParameter>>) on ParameterContainer: section was not ordered.");
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
                throw new InvalidOperationException(
                    "Cannot call RemoveAt(int) on ParameterContainer: section was not ordered.");
            }

            if (nIndex < 0 || nIndex > _pListKeyOrdered.Count)
            {
                throw new IndexOutOfRangeException("Index must be within the bounds." + Environment.NewLine +
                                                   "Parameter name: nIndex");
            }

            var key = _pListKeyOrdered[nIndex];
            _pListKeyOrdered.RemoveAt(nIndex);
            _pDicParam.Remove(key);
        }

        public void RemoveRange(int nIndex, int nCount)
        {
            if (_pListKeyOrdered == null)
            {
                throw new InvalidOperationException(
                    "Cannot call RemoveRange(int, int) on ParameterContainer: section was not ordered.");
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
                throw new InvalidOperationException(
                    "Cannot call Reverse() on ParameterContainer: section was not ordered.");
            }

            _pListKeyOrdered.Reverse();
        }

        public void Reverse(int nIndex, int nCount)
        {
            if (_pListKeyOrdered == null)
            {
                throw new InvalidOperationException(
                    "Cannot call Reverse(int, int) on ParameterContainer: section was not ordered.");
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

        public ICollection<YoonParameter> GetOrderedValues()
        {
            if (_pListKeyOrdered == null)
            {
                throw new InvalidOperationException(
                    "Cannot call GetOrderedValues() on ParameterContainer: section was not ordered.");
            }

            List<YoonParameter> pList = new List<YoonParameter>();
            for (int i = 0; i < _pListKeyOrdered.Count; i++)
            {
                pList.Add(_pDicParam[_pListKeyOrdered[i]]);
            }

            return pList;
        }

        public void Add(T pKey, YoonParameter pValue)
        {
            _pDicParam.Add(pKey, pValue);
            _pListKeyOrdered?.Add(pKey);
        }

        public bool ContainsKey(T pKey)
        {
            return _pDicParam.ContainsKey(pKey);
        }

        public ICollection<T> Keys => (_pListKeyOrdered != null) ? (ICollection<T>) _pListKeyOrdered : _pDicParam.Keys;

        public ICollection<YoonParameter> Values => _pDicParam.Values;

        public bool Remove(T pKey)
        {
            bool bResult = _pDicParam.Remove(pKey);
            if (_pListKeyOrdered == null || !bResult) return bResult;
            for (int i = 0; i < _pListKeyOrdered.Count; i++)
            {
                if (!Comparer.Equals(_pListKeyOrdered[i], pKey)) continue;
                _pListKeyOrdered.RemoveAt(i);
                break;
            }

            return bResult;
        }

        public bool TryGetValue(T pKey, out YoonParameter pValue)
        {
            return _pDicParam.TryGetValue(pKey, out pValue);
        }

        public int Count => _pDicParam.Count;

        void ICollection<KeyValuePair<T, YoonParameter>>.Add(KeyValuePair<T, YoonParameter> pCollection)
        {
            ((IDictionary<T, YoonParameter>) _pDicParam).Add(pCollection);
            _pListKeyOrdered?.Add(pCollection.Key);
        }

        bool ICollection<KeyValuePair<T, YoonParameter>>.Contains(KeyValuePair<T, YoonParameter> pCollection)
        {
            return ((IDictionary<T, YoonParameter>) _pDicParam).Contains(pCollection);
        }

        void ICollection<KeyValuePair<T, YoonParameter>>.CopyTo(KeyValuePair<T, YoonParameter>[] pArray,
            int nIndexArray)
        {
            ((IDictionary<T, YoonParameter>) _pDicParam).CopyTo(pArray, nIndexArray);
        }

        bool ICollection<KeyValuePair<T, YoonParameter>>.IsReadOnly =>
            ((IDictionary<T, YoonParameter>) _pDicParam).IsReadOnly;

        bool ICollection<KeyValuePair<T, YoonParameter>>.Remove(KeyValuePair<T, YoonParameter> pCollection)
        {
            bool bResult = ((IDictionary<T, YoonParameter>) _pDicParam).Remove(pCollection);
            if (_pListKeyOrdered == null || !bResult) return bResult;
            for (int i = 0; i < _pListKeyOrdered.Count; i++)
            {
                if (Comparer.Equals(_pListKeyOrdered[i], pCollection.Key))
                {
                    _pListKeyOrdered.RemoveAt(i);
                    break;
                }
            }

            return bResult;
        }

        public IEnumerator<KeyValuePair<T, YoonParameter>> GetEnumerator()
        {
            return _pListKeyOrdered != null ? GetOrderedEnumerator() : _pDicParam.GetEnumerator();
        }

        private IEnumerator<KeyValuePair<T, YoonParameter>> GetOrderedEnumerator()
        {
            foreach (var key in _pListKeyOrdered)
            {
                yield return new KeyValuePair<T, YoonParameter>(key, _pDicParam[key]);
            }
        }

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        public static implicit operator YoonContainer<T>(Dictionary<T, YoonParameter> pDic)
        {
            return new YoonContainer<T>(pDic);
        }

        public static explicit operator Dictionary<T, YoonParameter>(YoonContainer<T> pContainer)
        {
            return pContainer._pDicParam;
        }
    }
}
