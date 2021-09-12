namespace YoonFactory
{
    public static class DefaultExtensions
    {
        public static T[] Slice<T>(this T[] pSource, int nStart, int nEnd)
        {
            if (nEnd < 0)
                nEnd = pSource.Length + nEnd;
            int nLength = nEnd - nStart;
            T[] pResult = new T[nLength];
            for (int i = 0; i < nLength; i++)
                pResult[i] = pSource[i + nStart];
            return pResult;
        }
    }
}