using System;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace YoonFactory
{
    public class YoonBuffer1D : IYoonBuffer1D<byte>, IEquatable<YoonBuffer1D>
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

                _pBuffer = null;
                _disposedValue = true;
            }
        }

        ~YoonBuffer1D()
        {
            Dispose(false);
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        #endregion

        private byte[] _pBuffer = null;

        public int Length => _pBuffer?.Length ?? 0;

        public YoonBuffer1D(int nLength)
        {
            if (nLength <= 0)
                throw new ArgumentOutOfRangeException("[YOONBUFFER EXCEPTION] Abnormal buffer length");
            _pBuffer = new byte[nLength];
        }

        public YoonBuffer1D(IntPtr ptrAddress, int nLength)
        {
            SetBuffer(ptrAddress, nLength);
        }

        public YoonBuffer1D(byte[] pBuffer)
        {
            SetBuffer(pBuffer);
        }

        public IntPtr GetAddress()
        {
            Debug.Assert(_pBuffer != null, "_pBuffer != null");
            IntPtr pAddress = Marshal.AllocHGlobal(_pBuffer.Length * sizeof(byte));
            Marshal.Copy(_pBuffer, 0, pAddress, _pBuffer.Length);
            return pAddress;
        }

        public byte[] GetBuffer()
        {
            return _pBuffer;
        }

        public byte[] CopyBuffer()
        {
            Debug.Assert(_pBuffer != null, "_pBuffer != null");
            byte[] pResultBuffer = new byte[_pBuffer.Length];
            Array.Copy(_pBuffer, pResultBuffer, _pBuffer.Length);
            return pResultBuffer;
        }

        public byte[] CopyBuffer(int nStart, int nEnd)
        {
            int nLength = Math.Abs(nEnd - nStart);
            nStart = (nStart < nEnd) ? nStart : nEnd;
            nEnd = nStart + nLength;
            byte[] pResultBuffer = new byte[nLength];
            Array.Copy(_pBuffer, nStart, pResultBuffer, nEnd, nLength);
            return pResultBuffer;
        }

        public bool SetBuffer(IntPtr ptrAddress, int nLength)
        {
            if (ptrAddress == IntPtr.Zero)
                throw new ArgumentNullException("[YOONBUFFER EXCEPTION] Address is null");
            if (nLength <= 0)
                throw new ArgumentOutOfRangeException("[YOONBUFFER EXCEPTION] Abnormal buffer length");
            try
            {
                _pBuffer ??= new byte[nLength];
                Marshal.Copy(ptrAddress, _pBuffer, 0, nLength);
            }
            catch (Exception ex)
            {
                Console.WriteLine("[YOONBUFFER EXCEPTION] Marshal copy exception");
                Console.Write(ex.ToString());
                return false;
            }

            return true;
        }

        public bool SetBuffer(byte[] pBuffer)
        {
            if (pBuffer is not {Length: > 0})
                throw new ArgumentException("[YOONBUFFER EXCEPTION] Abnormal buffer length");
            try
            {
                _pBuffer ??= new byte[pBuffer.Length];
                Array.Copy(pBuffer, _pBuffer, pBuffer.Length);
            }
            catch (Exception ex)
            {
                Console.WriteLine("[YOONBUFFER EXCEPTION] Marshal copy exception");
                Console.WriteLine(ex.ToString());
                return false;
            }

            return true;
        }

        public bool SetBuffer(byte[] pBuffer, int nStart, int nEnd)
        {
            int nLength = Math.Abs(nEnd - nStart);
            nStart = (nStart < nEnd) ? nStart : nEnd;
            nEnd = nStart + nLength;
            if (_pBuffer == null || _pBuffer.Length < nEnd)
                throw new OutOfMemoryException("[YOONBUFFER EXCEPTION] Abnormal main buffer size");
            if (pBuffer == null || pBuffer.Length != nLength)
                throw new ArgumentException("[YOONBUFFER EXCEPTION] Abnormal buffer length");
            try
            {
                for (int i = nStart; i < nEnd; i++)
                    _pBuffer[i] = pBuffer[i - nStart];
            }
            catch (Exception ex)
            {
                Console.WriteLine("[YOONBUFFER EXCEPTION] Buffer copy exception");
                Console.WriteLine(ex.ToString());
                return false;
            }

            return true;
        }

        public byte GetValue(int nPos)
        {
            if (_pBuffer == null || _pBuffer.Length <= nPos)
                throw new OutOfMemoryException("[YOONBUFFER EXCEPTION] Abnormal main buffer size");
            return _pBuffer[nPos];
        }

        public bool SetValue(byte value, int nPos)
        {
            if (_pBuffer == null || _pBuffer.Length <= nPos)
                throw new OutOfMemoryException("[YOONBUFFER EXCEPTION] Abnormal main buffer size");
            _pBuffer[nPos] = value;
            return true;
        }

        public bool Equals(IYoonBuffer pBuffer)
        {
            if (_pBuffer.Length != pBuffer.Length)
                return false;
            if (pBuffer is not YoonBuffer1D pBuffer1D)
                return false;
            for (int i = 0; i < _pBuffer.Length; i++)
            {
                if (_pBuffer[i] != pBuffer1D._pBuffer[i])
                    return false;
            }

            return true;
        }

        public void CopyFrom(IYoonBuffer pBuffer)
        {
            throw new NotImplementedException();
        }

        public IYoonBuffer Clone()
        {
            throw new NotImplementedException();
        }

        public bool Equals(YoonBuffer1D other)
        {
            if (ReferenceEquals(null, other)) return false;
            if (ReferenceEquals(this, other)) return true;
            return _disposedValue == other._disposedValue && Equals(_pBuffer, other._pBuffer);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
            return Equals((YoonBuffer1D) obj);
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(_disposedValue, _pBuffer);
        }

        public static bool operator ==(YoonBuffer1D pBufferSource, YoonBuffer1D pBufferObject)
        {
            return false;
        }

        public static bool operator !=(YoonBuffer1D pBufferSource, YoonBuffer1D pBufferObject)
        {
            return !(pBufferSource == pBufferObject);
        }
    }
}