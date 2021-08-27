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
            if (pBuffer is not YoonBuffer1D pBuffer1D) return;
            if (pBuffer1D._pBuffer == null || pBuffer1D.Length == 0) return;
            if (_pBuffer == null || _pBuffer.Length != pBuffer1D.Length)
                _pBuffer = new byte[pBuffer1D.Length];
            Array.Copy(pBuffer1D._pBuffer, _pBuffer, pBuffer1D.Length);
        }

        public IYoonBuffer Clone()
        {
            return new YoonBuffer1D(_pBuffer);
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
            Debug.Assert(pBufferSource != null, nameof(pBufferSource) + " != null");
            return pBufferSource.Equals((IYoonBuffer) pBufferObject);
        }

        public static bool operator !=(YoonBuffer1D pBufferSource, YoonBuffer1D pBufferObject)
        {
            return !(pBufferSource == pBufferObject);
        }
    }

    public class YoonBuffer2D : IYoonBuffer2D<byte>, IEquatable<YoonBuffer2D>
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

        ~YoonBuffer2D()
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
        public int Rows { get; private set; } = 0;
        public int Cols { get; private set; } = 0;

        public YoonBuffer2D(int nWidth, int nHeight)
        {
            Rows = nHeight;
            Cols = nWidth;
            if (nWidth * nHeight <= 0)
                throw new ArgumentOutOfRangeException("[YOONBUFFER EXCEPTION] Abnormal buffer length");
            _pBuffer = new byte[nWidth * nHeight];
        }

        public YoonBuffer2D(IntPtr ptrAddress, int nWidth, int nHeight)
        {
            Rows = nHeight;
            Cols = nWidth;
            SetBuffer(ptrAddress, nWidth * nHeight);
        }

        public YoonBuffer2D(byte[] pBuffer, int nWidth, int nHeight)
        {
            Rows = nHeight;
            Cols = nWidth;
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
        
        public byte[] CopyBuffer(YoonRect2N pArea)
        {
            throw new NotImplementedException();
        }

        public byte[] CopyBuffer(YoonVector2N pStartVector, YoonVector2N pEndVector)
        {
            throw new NotImplementedException();
        }

        public bool SetBuffer(byte[] pBuffer)
        {
            throw new NotImplementedException();
        }

        public bool SetBuffer(IntPtr ptrAddress, int nLength)
        {
            throw new NotImplementedException();
        }

        public bool SetBuffer(byte[] pBuffer, YoonRect2N pArea)
        {
            throw new NotImplementedException();
        }

        public bool SetBuffer(byte[] pBuffer, YoonVector2N pStartVector, YoonVector2N pEndVector)
        {
            throw new NotImplementedException();
        }

        public byte GetValue(int nRow, int nCol)
        {
            throw new NotImplementedException();
        }

        public bool SetValue(byte value, int nRow, int nCol)
        {
            throw new NotImplementedException();
        }
        
        public bool Equals(IYoonBuffer pBuffer)
        {
            throw new NotImplementedException();
        }

        public void CopyFrom(IYoonBuffer pBuffer)
        {
            throw new NotImplementedException();
        }

        public IYoonBuffer Clone()
        {
            throw new NotImplementedException();
        }

        public bool Equals(YoonBuffer2D other)
        {
            if (ReferenceEquals(null, other)) return false;
            if (ReferenceEquals(this, other)) return true;
            return _disposedValue == other._disposedValue && Equals(_pBuffer, other._pBuffer) && Rows == other.Rows && Cols == other.Cols;
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
            return Equals((YoonBuffer2D) obj);
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(_disposedValue, _pBuffer, Rows, Cols);
        }
    }
}