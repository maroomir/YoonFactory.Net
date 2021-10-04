using System;
using System.Collections.Generic;

namespace YoonFactory
{
    public static class ArrayExtensions
    {
        public static T[] Slice<T>(this T[] pSource, int nStart, int nEnd)
        {
            if (!typeof(T).IsPrimitive)
                throw new InvalidOperationException("[YOONFACTORY] Not supported for manage types");
            if (pSource == null)
                throw new ArgumentNullException("[YOONFACTORY] Array is null");
            if (nEnd < 0)
                nEnd = pSource.Length + nEnd;
            int nLength = nEnd - nStart;
            T[] pResult = new T[nLength];
            for (int i = 0; i < nLength; i++)
                pResult[i] = pSource[i + nStart];
            return pResult;
        }

        public static T[] GetRow<T>(this T[,] pSource, int nRow)
        {
            if (!typeof(T).IsPrimitive)
                throw new InvalidOperationException("[YOONFACTORY] Not supported for manage types");
            if (pSource == null)
                throw new ArgumentNullException("[YOONFACTORY] Array is null");
            if (nRow < 0 || nRow > pSource.GetUpperBound(0))
                throw new ArgumentOutOfRangeException("[YOONFACTORY] Row size is the out of source range");
            int nCols = pSource.GetUpperBound(1) + 1;
            T[] pResult = new T[nCols];
            for (int i = 0; i < nCols; i++)
            {
                pResult[i] = pSource[nRow, i];
            }

            return pResult;
        }

        public static T[] GetColumn<T>(this T[,] pSource, int nCol)
        {
            if (!typeof(T).IsPrimitive)
                throw new InvalidOperationException("[YOONFACTORY] Not supported for manage types");
            if (pSource == null)
                throw new ArgumentNullException("[YOONFACTORY] Array is null");
            if (nCol < 0 || nCol > pSource.GetUpperBound(1))
                throw new ArgumentOutOfRangeException("[YOONFACTORY] Row size is the out of source range");
            int nRows = pSource.GetUpperBound(0) + 1;
            T[] pResult = new T[nRows];
            for (int i = 0; i < nRows; i++)
            {
                pResult[i] = pSource[i, nCol];
            }

            return pResult;
        }

        public static T[] ToArray1D<T>(this T[,] pSource)
        {
            T[] pResult = new T[pSource.Length];
            for (int j = 0; j < pSource.GetLength(0); j++)
            {
                for (int i = 0; i < pSource.GetLength(1); i++)
                {
                    pResult[j * pSource.GetLength(1) + i] = pSource[i, j];
                }
            }

            return pResult;
        }
    }
}