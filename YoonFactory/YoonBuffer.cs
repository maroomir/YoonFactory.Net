using System;
using System.Diagnostics;
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
            nEnd = nStart + nEnd;
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
                Marshal.Copy(ptrAddress, _pBuffer, 0, nLength);
            }
            catch (Exception ex)
            {
                Console.WriteLine("[YOONBUFFER EXCEPTION] Marshal copy is not active");
                Console.Write(ex.ToString());
                return false;
            }

            return true;
        }

        public bool SetBuffer(byte[] pBuffer)
        {
            if (pBuffer is not {Length: > 0})
                throw new ArgumentException("[YOONBUFFER EXCEPTION] Abnormal buffer exception");
            try
            {
                Array.Copy(pBuffer, _pBuffer, pBuffer.Length);
            }
            catch (Exception ex)
            {
                Console.WriteLine("[YOONBUFFER EXCEPTION] Marshal copy is not active");
                Console.WriteLine(ex.ToString());
                return false;
            }

            return true;
        }

        public bool SetBuffer(byte[] pBuffer, int nStart, int nEnd)
        {
            throw new NotImplementedException();
        }

        public byte GetValue(int nPos)
        {
            throw new NotImplementedException();
        }

        public bool SetValue(byte value, int nPos)
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