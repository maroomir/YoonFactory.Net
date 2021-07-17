using System;
using YoonFactory.Comm;
using YoonFactory.Comm.TCP;
using YoonFactory.Log;

namespace YoonSample.ConsoleComm
{
    class Program
    {
        private static YoonConsoler _pClm = new YoonConsoler();
        private static IYoonComm _pCommModule = null;
        
        static void Main(string[] args)
        {
            _pClm.Write("Typing the module (TCPServer, TCPClient, Serial) >> ");
            string strSelectModule = Console.ReadLine();
            switch (strSelectModule)
            {
                case "TCPServer":
                    ConnectServer();
                    break;
                case "TCPClient":
                    break;
                case "Serial":
                    break;
                default:
                    _pClm.Write("Interrupt : Please check the typing keys");
                    break;
            }
        }

        static void ConnectServer()
        {
            _pCommModule = new YoonServer();
            string strPort = "";
            while (!CommunicationFactory.VerifyTCPPort(strPort))
            {
                _pClm.Write("Port >> ");
                strPort = Console.ReadLine();
            }
            _pCommModule.Open();
        }
    }
}