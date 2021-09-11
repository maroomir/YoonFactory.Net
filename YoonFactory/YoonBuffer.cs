using System;
using System.ComponentModel.DataAnnotations;
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
            int nLength = nWidth * nHeight;
            Rows = nHeight;
            Cols = nWidth;
            if (nLength <= 0)
                throw new ArgumentOutOfRangeException("[YOONBUFFER EXCEPTION] Abnormal buffer length");
            _pBuffer = new byte[nLength];
        }

        public YoonBuffer2D(IntPtr ptrAddress, int nWidth, int nHeight)
        {
            int nLength = nWidth * nHeight;
            Rows = nHeight;
            Cols = nWidth;
            SetBuffer(ptrAddress, nLength);
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
            if (_pBuffer == null || _pBuffer.Length == 0)
                throw new OutOfMemoryException("[YOONBUFFER EXCEPTION] Abnormal main buffer size");
            if (pArea.Width <= 0 || pArea.Height <= 0 || pArea.Left <= 0 || pArea.Top <= 0)
                throw new ArgumentException("[YOONBUFFER EXCEPTION] Abnormal Area size");
            if (pArea.Right > Cols || pArea.Bottom > Rows)
                throw new ArgumentOutOfRangeException("[YOONBUFFER EXCEPTION] Abnormal area size");

            int nLength = pArea.Width * pArea.Height;
            byte[] pResultBuffer = new byte[nLength];
            for (int iY = 0; iY < pArea.Height; iY++)
            {
                int nY = pArea.Top + iY;
                for (int iX = 0; iX < pArea.Width; iX++)
                {
                    int nX = pArea.Left + iX;
                    pResultBuffer[iY * pArea.Width + iX] = _pBuffer[nY * Cols + nX];
                }
            }

            return pResultBuffer;
        }

        public byte[] CopyBuffer(YoonVector2N pStartVector, YoonVector2N pEndVector)
        {
            YoonRect2N pRect = new YoonRect2N(pStartVector, pEndVector);
            return CopyBuffer(pRect);
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

        public bool SetBuffer(byte[] pBuffer, YoonRect2N pArea)
        {
            if (_pBuffer == null || _pBuffer.Length == 0)
                throw new OutOfMemoryException("[YOONBUFFER EXCEPTION] Abnormal main buffer size");
            if (pArea.Width <= 0 || pArea.Height <= 0 || pArea.Left <= 0 || pArea.Top <= 0)
                throw new ArgumentException("[YOONBUFFER EXCEPTION] Abnormal Area size");
            if (pArea.Right > Cols || pArea.Bottom > Rows)
                throw new ArgumentOutOfRangeException("[YOONBUFFER EXCEPTION] Abnormal area size");
            if (pArea.Width * pArea.Height != pBuffer.Length)
                throw new ArgumentOutOfRangeException("[YOONBUFFER EXCEPTION] Abnormal buffer length");

            try
            {
                for (int iY = 0; iY < pArea.Height; iY++)
                {
                    int nY = pArea.Top + iY;
                    for (int iX = 0; iX < pArea.Width; iX++)
                    {
                        int nX = pArea.Left + iX;
                        _pBuffer[nY * Cols + nX] = pBuffer[iY * pArea.Width + iX];
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("[YOONBUFFER EXCEPTION] Buffer copy exception");
                Console.WriteLine(ex.ToString());
                return false;
            }

            return true;
        }

        public bool SetBuffer(byte[] pBuffer, YoonVector2N pStartVector, YoonVector2N pEndVector)
        {
            YoonRect2N pRect = new YoonRect2N(pStartVector, pEndVector);
            return SetBuffer(pBuffer, pRect);
        }

        public byte GetValue(int nRow, int nCol)
        {
            if (_pBuffer == null || Rows <= nRow || Cols <= nCol)
                throw new OutOfMemoryException("[YOONBUFFER EXCEPTION] Abnormal main buffer size");
            return _pBuffer[nRow * Cols + nCol];
        }

        public bool SetValue(byte value, int nRow, int nCol)
        {
            if (_pBuffer == null || Rows <= nRow || Cols <= nCol)
                throw new OutOfMemoryException("[YOONBUFFER EXCEPTION] Abnormal main buffer size");
            _pBuffer[nRow * Cols + nCol] = value;
            return true;
        }

        public bool Equals(IYoonBuffer pBuffer)
        {
            if (_pBuffer.Length != pBuffer.Length)
                return false;
            if (pBuffer is not YoonBuffer2D pBuffer2D)
                return false;
            for (int i = 0; i < _pBuffer.Length; i++)
            {
                if (_pBuffer[i] != pBuffer2D._pBuffer[i])
                    return false;
            }

            return pBuffer2D.Rows == Rows
                   && pBuffer2D.Cols == Cols;
        }

        public void CopyFrom(IYoonBuffer pBuffer)
        {
            if (pBuffer is not YoonBuffer2D pBuffer2D) return;
            if (pBuffer2D._pBuffer == null || pBuffer2D.Length == 0) return;
            if (_pBuffer == null || _pBuffer.Length != pBuffer2D.Length)
                _pBuffer = new byte[pBuffer2D.Length];
            Rows = pBuffer2D.Rows;
            Cols = pBuffer2D.Cols;
            Array.Copy(pBuffer2D._pBuffer, _pBuffer, pBuffer2D.Length);
        }

        public IYoonBuffer Clone()
        {
            return new YoonBuffer2D(_pBuffer, Cols, Rows);
        }

        public bool Equals(YoonBuffer2D other)
        {
            if (ReferenceEquals(null, other)) return false;
            if (ReferenceEquals(this, other)) return true;
            return _disposedValue == other._disposedValue && Equals(_pBuffer, other._pBuffer) && Rows == other.Rows &&
                   Cols == other.Cols;
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

        public static bool operator ==(YoonBuffer2D pBufferSource, YoonBuffer2D pBufferObject)
        {
            Debug.Assert(pBufferSource != null, nameof(pBufferSource) + " != null");
            return pBufferSource.Equals((IYoonBuffer) pBufferObject);
        }

        public static bool operator !=(YoonBuffer2D pBufferSource, YoonBuffer2D pBufferObject)
        {
            return !(pBufferSource == pBufferObject);
        }
    }
    
    public class YoonBuffer3D : IYoonBuffer3D<byte>, IEquatable<YoonBuffer3D>
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

        ~YoonBuffer3D()
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
        public int Depth { get; private set; } = 0;

        public YoonBuffer3D(int nWidth, int nHeight, int nDepth)
        {
            int nLength = nWidth * nHeight * nDepth;
            Rows = nHeight;
            Cols = nWidth;
            Depth = nDepth;
            if (nLength <= 0)
                throw new ArgumentOutOfRangeException("[YOONBUFFER EXCEPTION] Abnormal buffer length");
            _pBuffer = new byte[nLength];
        }

        public YoonBuffer3D(IntPtr ptrAddress, int nWidth, int nHeight, int nDepth)
        {
            int nLength = nWidth * nHeight * nDepth;
            Rows = nHeight;
            Cols = nWidth;
            Depth = nDepth;
            SetBuffer(ptrAddress, nLength);
        }

        public YoonBuffer3D(byte[] pBuffer, int nWidth, int nHeight, int nDepth)
        {
            Rows = nHeight;
            Cols = nWidth;
            Depth = nDepth;
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

        public YoonBuffer2D ToBuffer2D(int nPlane)
        {
            if (_pBuffer == null || _pBuffer.Length == 0)
                throw new OutOfMemoryException("[YOONBUFFER EXCEPTION] Abnormal main buffer size");
            if (nPlane >= Depth || nPlane < 0)
                throw new ArgumentException("[YOONBUFFER EXCEPTION] Abnormal Depth size");
            return new YoonBuffer2D(CopyBuffer(nPlane), Rows, Cols);
        }

        public byte[] CopyBuffer(int nPlane)
        {
            if (_pBuffer == null || _pBuffer.Length == 0)
                throw new OutOfMemoryException("[YOONBUFFER EXCEPTION] Abnormal main buffer size");
            if (nPlane >= Depth || nPlane < 0)
                throw new ArgumentException("[YOONBUFFER EXCEPTION] Abnormal Depth size");

            int nLength = Rows * Cols;
            byte[] pResultBuffer = new byte[nLength];
            Array.Copy(_pBuffer, nPlane * nLength, pResultBuffer, 0, nLength);
            return pResultBuffer;
        }

        public byte[] CopyBuffer(YoonRect2N pArea)
        {
            if (_pBuffer == null || _pBuffer.Length == 0)
                throw new OutOfMemoryException("[YOONBUFFER EXCEPTION] Abnormal main buffer size");
            if (pArea.Width <= 0 || pArea.Height <= 0 || pArea.Left <= 0 || pArea.Top <= 0)
                throw new ArgumentException("[YOONBUFFER EXCEPTION] Abnormal Area size");
            if (pArea.Right > Cols || pArea.Bottom > Rows)
                throw new ArgumentOutOfRangeException("[YOONBUFFER EXCEPTION] Abnormal area size");

            int nTotalSize = Rows * Cols;
            int nAreaSize = pArea.Width * pArea.Height;
            byte[] pResultBuffer = new byte[nAreaSize * Depth];
            for (int iZ = 0; iZ < Depth; iZ++)
            {
                for (int iY = 0; iY < pArea.Height; iY++)
                {
                    int nY = pArea.Top + iY;
                    for (int iX = 0; iX < pArea.Width; iX++)
                    {
                        int nX = pArea.Left + iX;
                        pResultBuffer[iZ * nAreaSize + iY * pArea.Width + iX] =
                            _pBuffer[iZ * nTotalSize + nY * Cols + nX];
                    }
                }
            }

            return pResultBuffer;
        }

        public byte[] CopyBuffer(YoonRect2N pArea, int nPlane)
        {
            if (_pBuffer == null || _pBuffer.Length == 0)
                throw new OutOfMemoryException("[YOONBUFFER EXCEPTION] Abnormal main buffer size");
            if (pArea.Width <= 0 || pArea.Height <= 0 || pArea.Left <= 0 || pArea.Top <= 0)
                throw new ArgumentException("[YOONBUFFER EXCEPTION] Abnormal Area size");
            if (pArea.Right > Cols || pArea.Bottom > Rows)
                throw new ArgumentOutOfRangeException("[YOONBUFFER EXCEPTION] Abnormal area size");
            if (nPlane >= Depth || nPlane < 0)
                throw new ArgumentException("[YOONBUFFER EXCEPTION] Abnormal Depth size");

            YoonBuffer2D pBuffer2D = ToBuffer2D(nPlane);
            return pBuffer2D.CopyBuffer(pArea);
        }

        public byte[] CopyBuffer(YoonVector2N pStartVector, YoonVector2N pEndVector)
        {
            YoonRect2N pRect = new YoonRect2N(pStartVector, pEndVector);
            return CopyBuffer(pRect);
        }

        public byte[] CopyBuffer(YoonVector2N pStartVector, YoonVector2N pEndVector, int nPlane)
        {
            YoonRect2N pRect = new YoonRect2N(pStartVector, pEndVector);
            return CopyBuffer(pRect, nPlane);
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
        
        public bool SetBuffer(byte[] pBuffer, int nPlane)
        {
            throw new NotImplementedException();
        }

        public bool SetBuffer(byte[] pBuffer, YoonRect2N pArea, int nPlane)
        {
            throw new NotImplementedException();
        }

        public bool SetBuffer(byte[] pBuffer, YoonVector2N pStartVector, YoonVector2N pEndVector, int nPlane)
        {
            throw new NotImplementedException();
        }

        public byte GetValue(int nX, int nY, int nPlane)
        {
            throw new NotImplementedException();
        }

        public bool SetValue(byte value, int nX, int nY, int nPlane)
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

        public bool Equals(YoonBuffer3D other)
        {
            if (ReferenceEquals(null, other)) return false;
            if (ReferenceEquals(this, other)) return true;
            return _disposedValue == other._disposedValue && Equals(_pBuffer, other._pBuffer) && Rows == other.Rows && Cols == other.Cols && Depth == other.Depth;
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
            return Equals((YoonBuffer3D) obj);
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(_disposedValue, _pBuffer, Rows, Cols, Depth);
        }
    }
}