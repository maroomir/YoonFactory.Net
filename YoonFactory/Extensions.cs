using System.Collections;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;

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

        public static string Log<K, V>(this Dictionary<K, V> pDic)
        {
            string strLog = "";
            foreach (K pKey in pDic.Keys)
            {
                strLog += pKey.ToString() + ":" + pDic[pKey].ToString();
                strLog += "/";
            }

            return strLog;
        }

    }
}