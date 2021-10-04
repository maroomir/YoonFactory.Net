using NumSharp;

namespace YoonFactory
{
    public static class YoonMatrixExtensions
    {
        public static NDArray ToNDArray(this YoonMatrix2X2Int pMatrix)
        {
            return new NDArray(pMatrix.Array.ToArray1D(), new Shape(2, 2));
        }

        public static NDArray ToNDArray(this YoonMatrix2X2Double pMatrix)
        {
            return new NDArray(pMatrix.Array.ToArray1D(), new Shape(2, 2));
        }

        public static NDArray ToNDArray(this YoonMatrix3X3Int pMatrix)
        {
            return new NDArray(pMatrix.Array.ToArray1D(), new Shape(3, 3));
        }

        public static NDArray ToNDArray(this YoonMatrix3X3Double pMatrix)
        {
            return new NDArray(pMatrix.Array.ToArray1D(), new Shape(3, 3));
        }

        public static NDArray ToNDArray(this YoonMatrix4X4Double pMatrix)
        {
            return new NDArray(pMatrix.Array.ToArray1D(), new Shape(4, 4));
        }
    }
}