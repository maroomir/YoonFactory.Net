using System.Collections.Generic;

namespace YoonFactory
{
    public static class LogExtensions
    {
        public static string Log<TKey, TValue>(this Dictionary<TKey, TValue> pDic)
        {
            string strLog = "";
            foreach (TKey pKey in pDic.Keys)
            {
                strLog += $"{pKey.ToString()}:{pDic[pKey].ToString()}/";
            }

            return strLog;
        }

        public static string Log<T>(this List<T> pList)
        {
            string strLog = "";
            for (int i = 0; i < pList.Count; i++)
            {
                strLog += $"{i:D2}:{pList[i].ToString()}/";
                if (i > 99) break;
            }

            return strLog;
        }
    }
}