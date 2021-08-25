using System;

namespace YoonFactory
{
    public class YoonBuffer1D : IYoonBuffer<byte>, IEquatable<YoonBuffer1D>
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

        public IntPtr ScanAddress()
        {
            throw new NotImplementedException();
        }

        public bool Print(IntPtr pAddress)
        {
            throw new NotImplementedException();
        }

        public byte[] Scan()
        {
            throw new NotImplementedException();
        }

        public bool Print(byte[] pBuffer)
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