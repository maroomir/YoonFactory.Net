using System;
using System.Diagnostics;
using System.Threading;
using YoonFactory.Comm;
using YoonFactory.Comm.Serial;
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
            Console.Write("Typing the module (TCPServer, TCPClient, Serial) >> ");
            string strSelectModule = Console.ReadLine();
            switch (strSelectModule)
            {
                case "TCPServer":
                    _pClm.Write("Start TCPServer Module");
                    ProcessServer();
                    break;
                case "TCPClient":
                    _pClm.Write("Start TCPClient Module");
                    ProcessClient();
                    break;
                case "Serial":
                    _pClm.Write("Start Serial Module");
                    ProcessSerial();
                    break;
                default:
                    _pClm.Write("Interrupt Module");
                    break;
            }
        }

        static void ProcessServer()
        {
            _pCommModule = new YoonServer();
            string strPort = "";
            while (!CommunicationFactory.VerifyTCPPort(strPort))
            {
                Console.Write("Port >> ");
                strPort = Console.ReadLine();
            }

            _pCommModule.Port = strPort;
            _pClm.Write($"Ready to {strPort} Port");
            _pCommModule.OnShowMessageEvent += (sender, args) => _pClm.Write(args.Message);
            _pCommModule.OnShowReceiveDataEvent += (sender, args) => Console.WriteLine(args.StringData);
            if (!_pCommModule.Open())
                _pCommModule.OnRetryThreadStart();
            while (true)
            {
                Console.Write("Send >> ");
                string strSendMessage = Console.ReadLine();
                if (strSendMessage?.ToLower() == "break")
                    break;
                if (_pCommModule.Send(strSendMessage))
                    _pClm.Write($"Send the Message : {strSendMessage}");
                Thread.Sleep(1000);
            }
        }


        static void ProcessClient()
        {
            _pCommModule = new YoonClient();
            string strAddress = "";
            string strPort = "";
            while (!CommunicationFactory.VerifyIPAddress(strAddress))
            {
                Console.Write("IP >> ");
                strAddress = Console.ReadLine();
            }

            while (!CommunicationFactory.VerifyTCPPort(strPort))
            {
                Console.Write("Port >> ");
                strPort = Console.ReadLine();
            }

            _pCommModule.Address = strAddress;
            _pCommModule.Port = strPort;
            _pClm.Write($"Ready to {strAddress} : {strPort}");
            _pCommModule.OnShowMessageEvent += (sender, args) => _pClm.Write(args.Message);
            _pCommModule.OnShowReceiveDataEvent += (sender, args) => Console.WriteLine(args.StringData);
            if (!_pCommModule.Open())
                _pCommModule.OnRetryThreadStart();
            while (true)
            {
                Console.Write("Send >> ");
                string strSendMessage = Console.ReadLine();
                if (strSendMessage?.ToLower() == "break")
                    break;
                if (_pCommModule.Send(strSendMessage))
                    _pClm.Write($"Send the Message : {strSendMessage}");
                Thread.Sleep(1000);
            }
        }

        static void ProcessSerial()
        {
            _pCommModule = new YoonSerial();
            string strPort = "";
            while (!CommunicationFactory.VerifySerialPort(strPort))
            {
                Console.Write("Port >> ");
                strPort = Console.ReadLine();
            }

            _pCommModule.Port = strPort;
            _pClm.Write($"Ready to {strPort} Port");
            _pCommModule.OnShowMessageEvent += (sender, args) => _pClm.Write(args.Message);
            _pCommModule.OnShowReceiveDataEvent += (sender, args) => Console.WriteLine(args.StringData);
            if (!_pCommModule.Open())
                _pCommModule.OnRetryThreadStart();
            while (true)
            {
                Console.Write("Send >> ");
                string strSendMessage = Console.ReadLine();
                if (strSendMessage?.ToLower() == "break")
                    break;
                if (_pCommModule.Send(strSendMessage))
                    _pClm.Write($"Send the Message : {strSendMessage}");
                Thread.Sleep(1000);
            }
        }
    }
}