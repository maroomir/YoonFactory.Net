using System;

namespace YoonFactory.Comm.TCP
{
    public static class TCPFactory
    {
        public static int[] GetIPAddressArray(string strIPAddress)
        {
            const int MAX_IPV4_NUM = 4;
            int[] arrayNumIP = { -1, -1, -1, -1 };

            if (!VerifyIPAddress(strIPAddress))
                return arrayNumIP;

            try
            {
                string[] strIPCookedDivide = strIPAddress.Split('.');
                if (strIPCookedDivide.Length != MAX_IPV4_NUM) return arrayNumIP;
                for (int iValue = 0; iValue < MAX_IPV4_NUM; iValue++)
                {
                    arrayNumIP[iValue] = Convert.ToInt32(strIPCookedDivide[iValue]);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("IP Address Invalid : Address / " + strIPAddress);
                Console.WriteLine(ex.ToString());
            }
            return arrayNumIP;
        }

        public static int GetIPAddressNum(int nOrder, string strIPAddress)
        {
            const int MAX_IPV4_NUM = 4;
            if (nOrder >= 0 && nOrder < MAX_IPV4_NUM)
                return GetIPAddressArray(strIPAddress)[nOrder];
            return -1;
        }

        public static bool VerifyIPAddress(string strIPAddress)
        {
            const int MAX_IPV4_NUM = 4;
            try
            {
                string[] strIPCookedDivide = strIPAddress.Split('.');
                if (strIPCookedDivide.Length != MAX_IPV4_NUM) return false;
                for (int iValue = 0; iValue < MAX_IPV4_NUM; iValue++)
                {
                    int nIPNum = Convert.ToInt32(strIPCookedDivide[iValue]);
                    if (nIPNum < 0 || nIPNum > 255) return false;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("IP Address Invalid : Address / " + strIPAddress);
                Console.WriteLine(ex.ToString());
                return false;
            }
            return true;
        }

        public static bool VerifyPort(string strPort)
        {
            try
            {
                int nNumPort = Convert.ToInt32(strPort);
                if (nNumPort < 0 || nNumPort > 65536) return false;
            }
            catch (Exception ex)
            {
                Console.WriteLine("Port Num Invalid : Port No / " + strPort);
                Console.WriteLine(ex.ToString());
                return false;
            }
            return true;
        }
    }

}