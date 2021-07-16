using System;
using System.Collections.Generic;
using System.Linq;

namespace YoonFactory.Files.Ini
{
    public struct IniValue
    {
        private static bool TryParseInt(string strText, out int nValue)
        {
            if (int.TryParse(strText,
                System.Globalization.NumberStyles.Integer,
                System.Globalization.CultureInfo.InvariantCulture,
                out int nResult))
            {
                nValue = nResult;
                return true;
            }

            nValue = 0;
            return false;
        }

        private static bool TryParseDouble(string strText, out double dValue)
        {
            if (double.TryParse(strText,
                System.Globalization.NumberStyles.Float,
                System.Globalization.CultureInfo.InvariantCulture,
                out var dResult))
            {
                dValue = dResult;
                return true;
            }

            dValue = Double.NaN;
            return false;
        }

        private static bool TryParseEnum<T>(string strText, out T value) where T : struct, IFormattable
        {
            if (Enum.TryParse(strText, out T pResult))
            {
                value = pResult;
                return true;
            }

            value = default(T);
            return false;
        }

        private static bool TryParse<T>(string strText, out T value) where T : IConvertible
        {
            try
            {
                value = (T) Convert.ChangeType(strText, typeof(T));
                if (value == null)
                    return false;
                else
                    return true;
            }
            catch
            {
                value = default(T);
                return false;
            }
        }

        public string Value;

        public IniValue(object value)
        {
            if (value is IFormattable formattable)
            {
                Value = formattable.ToString(null, System.Globalization.CultureInfo.InvariantCulture);
            }
            else
            {
                Value = value?.ToString();
            }
        }

        public IniValue(string strValue)
        {
            Value = strValue;
        }

        public bool ToBool(bool bInvalid = false)
        {
            return TryConvertBool(out bool bResult) ? bResult : bInvalid;
        }

        public bool TryConvertBool(out bool bResult)
        {
            if (Value == null)
            {
                bResult = default(bool);
                return false;
            }

            string strBool = Value.Trim().ToLowerInvariant();
            switch (strBool)
            {
                case "true":
                    bResult = true;
                    return true;
                case "false":
                    bResult = false;
                    return true;
                default:
                    bResult = default(bool);
                    return false;
            }
        }

        public int ToInt(int nValue = 0)
        {
            return TryConvertInt(out int nResult) ? nResult : nValue;
        }

        public bool TryConvertInt(out int nResult)
        {
            if (Value != null) return TryParseInt(Value.Trim(), out nResult);
            nResult = default(int);
            return false;
        }

        public double ToDouble(double dValue = 0)
        {
            return TryConvertDouble(out var dResult) ? dResult : dValue;
        }

        public bool TryConvertDouble(out double result)
        {
            if (Value != null) return TryParseDouble(Value.Trim(), out result);
            result = default(double);
            return false;
        }

        public T ToEnum<T>(T value) where T : struct, IFormattable
        {
            return TryConvertEnum(out T result) ? result : value;
        }

        public bool TryConvertEnum<T>(out T result) where T : struct, IFormattable
        {
            if (Value != null) return TryParseEnum<T>(Value.Trim(), out result);
            result = default(T);
            return false;
        }

        public T To<T>(T value) where T : IConvertible
        {
            return TryConvert(out T result) ? result : value;
        }

        public bool TryConvert<T>(out T result) where T : IConvertible
        {
            if (Value == null)
            {
                result = default(T);
                return false;
            }

            if (TryParse<T>(Value.Trim(), out result))
            {
                return true;
            }

            return false;
        }

        public string GetString()
        {
            return GetString(true, false);
        }

        public string GetString(bool bPreserveWhiteSpace)
        {
            return GetString(true, bPreserveWhiteSpace);
        }

        public string GetString(bool bAllowOuterQuotes, bool bPreserveWhiteSpace)
        {
            if (Value == null)
            {
                return "";
            }

            string strTrimmed = Value.Trim();
            if (bAllowOuterQuotes && strTrimmed.Length >= 2 && strTrimmed[0] == '"' && strTrimmed[^1] == '"')
            {
                string strInner = strTrimmed.Substring(1, strTrimmed.Length - 2);
                return bPreserveWhiteSpace ? strInner : strInner.Trim();
            }

            return bPreserveWhiteSpace ? Value : Value.Trim();
        }

        public override string ToString()
        {
            return Value;
        }

        public string ToString(string strValue)
        {
            if (string.IsNullOrEmpty(Value))
            {
                return strValue;
            }

            return Value;
        }

        public static implicit operator IniValue(byte value)
        {
            return new IniValue(value);
        }

        public static implicit operator IniValue(short value)
        {
            return new IniValue(value);
        }

        public static implicit operator IniValue(int value)
        {
            return new IniValue(value);
        }

        public static implicit operator IniValue(sbyte value)
        {
            return new IniValue(value);
        }

        public static implicit operator IniValue(ushort value)
        {
            return new IniValue(value);
        }

        public static implicit operator IniValue(uint value)
        {
            return new IniValue(value);
        }

        public static implicit operator IniValue(float value)
        {
            return new IniValue(value);
        }

        public static implicit operator IniValue(double value)
        {
            return new IniValue(value);
        }

        public static implicit operator IniValue(bool value)
        {
            return new IniValue(value);
        }

        public static implicit operator IniValue(string value)
        {
            return new IniValue(value);
        }

        public static IniValue Default { get; } = new IniValue();
    }

    public sealed class IniSection : IYoonSection<string, IniValue>
    {
        #region IDisposable Support

        ~IniSection()
        {
            this.Dispose(false);
        }

        private bool disposed;

        public void Dispose()
        {
            this.Dispose(true);
            GC.SuppressFinalize(this);
        }

        private void Dispose(bool disposing)
        {
            if (this.disposed) return;
            if (disposing)
            {
                //
            }

            Clear();
            _pDicIniValue = null;
            _pListKeyOrdered = null;
            this.disposed = true;
        }

        #endregion

        public string RootDirectory { get; set; }

        private Dictionary<string, IniValue> _pDicIniValue;
        private List<string> _pListKeyOrdered;

        public IEqualityComparer<string> Comparer => _pDicIniValue.Comparer;

        public IniValue this[int nIndex]
        {
            get
            {
                if (_pListKeyOrdered == null)
                {
                    throw new InvalidOperationException(
                        "Cannot index IniSection using integer key: section was not ordered.");
                }

                if (nIndex < 0 || nIndex >= _pListKeyOrdered.Count)
                {
                    throw new IndexOutOfRangeException("Index must be within the bounds." + Environment.NewLine +
                                                       "Parameter name: index");
                }

                return _pDicIniValue[_pListKeyOrdered[nIndex]];
            }
            set
            {
                if (_pListKeyOrdered == null)
                {
                    throw new InvalidOperationException(
                        "Cannot index IniSection using integer key: section was not ordered.");
                }

                if (nIndex < 0 || nIndex >= _pListKeyOrdered.Count)
                {
                    throw new IndexOutOfRangeException("Index must be within the bounds." + Environment.NewLine +
                                                       "Parameter name: index");
                }

                string strKey = _pListKeyOrdered[nIndex];
                _pDicIniValue[strKey] = value;
            }
        }

        public IniValue this[string strName]
        {
            get => _pDicIniValue.TryGetValue(strName, out var pValue) ? pValue : IniValue.Default;
            set
            {
                if (_pListKeyOrdered != null && !_pListKeyOrdered.Contains(strName, Comparer))
                {
                    _pListKeyOrdered.Add(strName);
                }

                _pDicIniValue[strName] = value;
            }
        }

        public IniSection()
            : this(YoonIni.DefaultComparer)
        {
        }

        public IniSection(IEqualityComparer<string> pStringComparer)
        {
            _pDicIniValue = new Dictionary<string, IniValue>(pStringComparer);
            _pListKeyOrdered = new List<string>();
        }

        public IniSection(Dictionary<string, IniValue> pDic)
            : this(pDic, YoonIni.DefaultComparer)
        {
        }

        public IniSection(Dictionary<string, IniValue> pDic, IEqualityComparer<string> pStringComparer)
        {
            _pDicIniValue = new Dictionary<string, IniValue>(pDic, pStringComparer);
            _pListKeyOrdered = new List<string>(pDic.Keys);
        }

        public IniSection(IniSection pSection)
            : this(pSection, YoonIni.DefaultComparer)
        {
        }

        public IniSection(IniSection pSection, IEqualityComparer<string> pStringComparer)
        {
            _pDicIniValue = new Dictionary<string, IniValue>(pSection._pDicIniValue, pStringComparer);
            _pListKeyOrdered = new List<string>(pSection._pListKeyOrdered);
        }

        public void Clear()
        {
            _pDicIniValue?.Clear();
            _pListKeyOrdered?.Clear();
        }

        public string KeyOf(int nIndex)
        {
            if (_pListKeyOrdered == null)
            {
                throw new InvalidOperationException("Cannot call KeyOf(int) on IniSection: section was not ordered.");
            }

            if (nIndex < 0 || nIndex > _pListKeyOrdered.Count)
            {
                throw new IndexOutOfRangeException("Index must be within the bounds." + Environment.NewLine +
                                                   "Parameter name: nIndex");
            }

            return _pListKeyOrdered[nIndex];
        }

        public int IndexOf(string strKey)
        {
            if (_pListKeyOrdered == null)
            {
                throw new InvalidOperationException(
                    "Cannot call IndexOf(string) on IniSection: section was not ordered.");
            }

            return IndexOf(strKey, 0, _pListKeyOrdered.Count);
        }

        public int IndexOf(string strKey, int nIndex)
        {
            if (_pListKeyOrdered == null)
            {
                throw new InvalidOperationException(
                    "Cannot call IndexOf(string, int) on IniSection: section was not ordered.");
            }

            return IndexOf(strKey, nIndex, _pListKeyOrdered.Count - nIndex);
        }

        public int IndexOf(string strKey, int nIndex, int nCount)
        {
            if (_pListKeyOrdered == null)
            {
                throw new InvalidOperationException(
                    "Cannot call IndexOf(string, int, int) on IniSection: section was not ordered.");
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
                if (Comparer.Equals(_pListKeyOrdered[i], strKey))
                {
                    return i;
                }
            }

            return -1;
        }

        public int LastIndexOf(string strKey)
        {
            if (_pListKeyOrdered == null)
            {
                throw new InvalidOperationException(
                    "Cannot call LastIndexOf(string) on IniSection: section was not ordered.");
            }

            return LastIndexOf(strKey, 0, _pListKeyOrdered.Count);
        }

        public int LastIndexOf(string strKey, int nIndex)
        {
            if (_pListKeyOrdered == null)
            {
                throw new InvalidOperationException(
                    "Cannot call LastIndexOf(string, int) on IniSection: section was not ordered.");
            }

            return LastIndexOf(strKey, nIndex, _pListKeyOrdered.Count - nIndex);
        }

        public int LastIndexOf(string strKey, int nIndex, int nCount)
        {
            if (_pListKeyOrdered == null)
            {
                throw new InvalidOperationException(
                    "Cannot call LastIndexOf(string, int, int) on IniSection: section was not ordered.");
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

            int end = nIndex + nCount;
            for (int i = end - 1; i >= nIndex; i--)
            {
                if (Comparer.Equals(_pListKeyOrdered[i], strKey))
                {
                    return i;
                }
            }

            return -1;
        }

        public void Insert(int nIndex, string strKey, IniValue pValue)
        {
            if (_pListKeyOrdered == null)
            {
                throw new InvalidOperationException(
                    "Cannot call Insert(int, string, IniValue) on IniSection: section was not ordered.");
            }

            if (nIndex < 0 || nIndex > _pListKeyOrdered.Count)
            {
                throw new IndexOutOfRangeException("Index must be within the bounds." + Environment.NewLine +
                                                   "Parameter name: nIndex");
            }

            _pDicIniValue.Add(strKey, pValue);
            _pListKeyOrdered.Insert(nIndex, strKey);
        }

        public void InsertRange(int nIndex, IEnumerable<KeyValuePair<string, IniValue>> pCollection)
        {
            if (_pListKeyOrdered == null)
            {
                throw new InvalidOperationException(
                    "Cannot call InsertRange(int, IEnumerable<KeyValuePair<string, IniValue>>) on IniSection: section was not ordered.");
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
                    "Cannot call RemoveAt(int) on IniSection: section was not ordered.");
            }

            if (nIndex < 0 || nIndex > _pListKeyOrdered.Count)
            {
                throw new IndexOutOfRangeException("Index must be within the bounds." + Environment.NewLine +
                                                   "Parameter name: nIndex");
            }

            string strKey = _pListKeyOrdered[nIndex];
            _pListKeyOrdered.RemoveAt(nIndex);
            _pDicIniValue.Remove(strKey);
        }

        public void RemoveRange(int nIndex, int nCount)
        {
            if (_pListKeyOrdered == null)
            {
                throw new InvalidOperationException(
                    "Cannot call RemoveRange(int, int) on IniSection: section was not ordered.");
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
                throw new InvalidOperationException("Cannot call Reverse() on IniSection: section was not ordered.");
            }

            _pListKeyOrdered.Reverse();
        }

        public void Reverse(int nIndex, int nCount)
        {
            if (_pListKeyOrdered == null)
            {
                throw new InvalidOperationException(
                    "Cannot call Reverse(int, int) on IniSection: section was not ordered.");
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

        public ICollection<IniValue> GetOrderedValues()
        {
            if (_pListKeyOrdered == null)
            {
                throw new InvalidOperationException(
                    "Cannot call GetOrderedValues() on IniSection: section was not ordered.");
            }

            var list = new List<IniValue>();
            for (int i = 0; i < _pListKeyOrdered.Count; i++)
            {
                list.Add(_pDicIniValue[_pListKeyOrdered[i]]);
            }

            return list;
        }

        public void Add(string strKey, IniValue pValue)
        {
            _pDicIniValue.Add(strKey, pValue);
            _pListKeyOrdered?.Add(strKey);
        }

        public bool ContainsKey(string strKey)
        {
            return _pDicIniValue.ContainsKey(strKey);
        }

        /// <summary>
        /// Returns this IniSection's collection of keys. If the IniSection is ordered, the keys will be returned in order.
        /// </summary>
        public ICollection<string> Keys =>
            (_pListKeyOrdered != null) ? (ICollection<string>) _pListKeyOrdered : _pDicIniValue.Keys;

        public bool Remove(string strKey)
        {
            bool bResult = _pDicIniValue.Remove(strKey);
            if (_pListKeyOrdered == null || !bResult) return bResult;
            for (int i = 0; i < _pListKeyOrdered.Count; i++)
            {
                if (!Comparer.Equals(_pListKeyOrdered[i], strKey)) continue;
                _pListKeyOrdered.RemoveAt(i);
                break;
            }

            return bResult;
        }

        public bool TryGetValue(string strKey, out IniValue pValue)
        {
            return _pDicIniValue.TryGetValue(strKey, out pValue);
        }

        /// <summary>
        /// Returns the values in this IniSection. These values are always out of order. To get ordered values from an IniSection call GetOrderedValues instead.
        /// </summary>
        public ICollection<IniValue> Values => _pDicIniValue.Values;

        void ICollection<KeyValuePair<string, IniValue>>.Add(KeyValuePair<string, IniValue> pCollection)
        {
            ((IDictionary<string, IniValue>) _pDicIniValue).Add(pCollection);
            _pListKeyOrdered?.Add(pCollection.Key);
        }

        bool ICollection<KeyValuePair<string, IniValue>>.Contains(KeyValuePair<string, IniValue> pCollection)
        {
            return ((IDictionary<string, IniValue>) _pDicIniValue).Contains(pCollection);
        }

        void ICollection<KeyValuePair<string, IniValue>>.CopyTo(KeyValuePair<string, IniValue>[] pArray,
            int nIndexArray)
        {
            ((IDictionary<string, IniValue>) _pDicIniValue).CopyTo(pArray, nIndexArray);
        }

        public int Count => _pDicIniValue.Count;

        bool ICollection<KeyValuePair<string, IniValue>>.IsReadOnly =>
            ((IDictionary<string, IniValue>) _pDicIniValue).IsReadOnly;

        bool ICollection<KeyValuePair<string, IniValue>>.Remove(KeyValuePair<string, IniValue> pCollection)
        {
            var bResult = ((IDictionary<string, IniValue>) _pDicIniValue).Remove(pCollection);
            if (_pListKeyOrdered == null || !bResult) return bResult;
            for (int i = 0; i < _pListKeyOrdered.Count; i++)
            {
                if (!Comparer.Equals(_pListKeyOrdered[i], pCollection.Key)) continue;
                _pListKeyOrdered.RemoveAt(i);
                break;
            }

            return bResult;
        }

        public IEnumerator<KeyValuePair<string, IniValue>> GetEnumerator()
        {
            return _pListKeyOrdered != null ? GetOrderedEnumerator() : _pDicIniValue.GetEnumerator();
        }

        private IEnumerator<KeyValuePair<string, IniValue>> GetOrderedEnumerator()
        {
            foreach (string strKey in _pListKeyOrdered)
            {
                yield return new KeyValuePair<string, IniValue>(strKey, _pDicIniValue[strKey]);
            }
        }

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        public static implicit operator IniSection(Dictionary<string, IniValue> pDic)
        {
            return new IniSection(pDic);
        }

        public static explicit operator Dictionary<string, IniValue>(IniSection pSection)
        {
            return pSection._pDicIniValue;
        }
    }
}