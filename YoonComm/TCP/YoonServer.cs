using System;
using System.Text;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.Diagnostics;
using System.Collections.Generic;
using YoonFactory.Files;

namespace YoonFactory.Comm.TCP
{
    public class YoonServer : IYoonTcpIp, IDisposable
    {
        #region IDisposable Support

        private bool _disposedValue = false;

        protected virtual void Dispose(bool disposing)
        {
            if (!_disposedValue)
            {
                // Save Last setting
                SaveParam();
                SaveTarget();

                if (disposing)
                {
                    if (_pServerSocket != null)
                    {
                        _pServerSocket.Close();
                        _pServerSocket.Dispose();
                        _pServerSocket = null;
                    }

                    if (_pConnectedClientSocket != null)
                    {
                        _pConnectedClientSocket.Disconnect(false);
                        _pConnectedClientSocket.Dispose();
                        _pConnectedClientSocket = null;
                    }
                }

                // Close RetryConnect Thread
                OnRetryThreadStop();
                // Refund memory to main function
                _pThreadRetryListen = null;

                _pAcceptHandler = null;
                _pReceiveHandler = null;
                _pSendHandler = null;

                _disposedValue = true;
            }
        }

        public YoonServer()
        {
            ReceiveMessage = new StringBuilder(string.Empty);

            _pAcceptHandler = new AsyncCallback(OnAcceptClientEvent);
            _pReceiveHandler = new AsyncCallback(OnReceiveEvent);
            _pSendHandler = new AsyncCallback(OnSendEvent);
        }

        public YoonServer(string strParamDirectory)
        {
            RootDirectory = strParamDirectory;
            LoadParam();
            LoadTarget();

            ReceiveMessage = new StringBuilder(string.Empty);

            _pAcceptHandler = new AsyncCallback(OnAcceptClientEvent);
            _pReceiveHandler = new AsyncCallback(OnReceiveEvent);
            _pSendHandler = new AsyncCallback(OnSendEvent);
        }

        ~YoonServer()
        {
            Dispose(false);
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        #endregion

        protected class AsyncObject
        {
            public byte[] Buffer;
            public Socket WorkingSocket;

            public AsyncObject(int nBufferSize)
            {
                Buffer = new byte[nBufferSize];
            }
        }

        public event ShowMessageCallback OnShowMessageEvent;
        public event RecieveDataCallback OnShowReceiveDataEvent;
        public bool IsRetryOpen { get; private set; } = false;
        public bool IsSend { get; private set; } = false;
        public StringBuilder ReceiveMessage { get; private set; } = null;
        public string Address { get; set; } = string.Empty;
        public string RootDirectory { get; set; } = Path.Combine(Directory.GetCurrentDirectory(), "YoonFactory");

        public string Port
        {
            get => Param.Port;
            set
            {
                if (CommunicationFactory.VerifyPort(value))
                    Param.Port = value;
            }
        }

        public void CopyFrom(IYoonComm pComm)
        {
            if (pComm is not YoonServer pServer) return;
            Close();
            if (pServer.IsConnected)
                pServer.Close();

            LoadParam();
            Port = pServer.Port;
        }

        public IYoonComm Clone()
        {
            Close();

            YoonServer pServer = new YoonServer();
            pServer.LoadParam();
            pServer.Port = Port;
            return pServer;
        }

        private const int BUFFER_SIZE = 4096;

        private Socket _pServerSocket = null;
        private Socket _pConnectedClientSocket = null;
        private AsyncCallback _pAcceptHandler;
        private AsyncCallback _pReceiveHandler;
        private AsyncCallback _pSendHandler;
        private List<string> _pListClientIP = null;

        private struct Param
        {
            public static string Port = "1234";
            public static string Backlog = "5";
            public static string RetryCount = "10";
            public static string RetryListen = "true";
            public static string Timeout = "10000";
        }

        public void SetParam(string strPort, string strBacklog, string strRetryListen, string strTimeout,
            string strRetryCount)
        {
            Param.Port = strPort;
            Param.Backlog = strBacklog;
            Param.RetryCount = strRetryCount;
            Param.RetryListen = strRetryListen;
            Param.Timeout = strTimeout;
        }

        public void SetParam(int strPort, int nBacklog, bool bRetryListen, int nTimeout, int nRetryCount)
        {
            Param.Port = strPort.ToString();
            Param.Backlog = nBacklog.ToString();
            Param.RetryListen = bRetryListen.ToString();
            Param.Timeout = nTimeout.ToString();
            Param.RetryCount = nRetryCount.ToString();
        }

        public void LoadParam()
        {
            string strParamFilePath = Path.Combine(RootDirectory, "IPServer.ini");
            using (YoonIni ic = new YoonIni(strParamFilePath))
            {
                ic.LoadFile();
                Param.Port = ic["Server"]["Port"].ToString("1234");
                Param.Backlog = ic["Server"]["Backlog"].ToString("5");
                Param.RetryListen = ic["Server"]["RetryListen"].ToString("true");
                Param.RetryCount = ic["Server"]["RetryCount"].ToString("10");
                Param.Timeout = ic["Server"]["TimeOut"].ToString("10000");
            }
        }

        public void SaveParam()
        {
            string strParamFilePath = Path.Combine(RootDirectory, "IPServer.ini");
            using (YoonIni ic = new YoonIni(strParamFilePath))
            {
                ic["Server"]["Port"] = Param.Port;
                ic["Server"]["Backlog"] = Param.Backlog;
                ic["Server"]["RetryListen"] = Param.RetryListen;
                ic["Server"]["RetryCount"] = Param.RetryCount;
                ic["Server"]["TimeOut"] = Param.Timeout;
                ic.SaveFile();
            }
        }

        public void LoadTarget()
        {
            try
            {
                string strTargetPath = Path.Combine(RootDirectory, "IPTarget.xml");
                YoonXml xc = new YoonXml(strTargetPath);
                object pTargetParam;
                if (xc.LoadFile(out pTargetParam, typeof(List<string>)))
                    _pListClientIP = (List<string>) pTargetParam;
                else if (_pListClientIP == null)
                {
                    _pListClientIP = new List<string>();
                }

                _pListClientIP.Clear();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }
        }

        public void SaveTarget()
        {
            if (_pListClientIP == null)
            {
                _pListClientIP = new List<string>();
                _pListClientIP.Clear();
            }

            try
            {
                string strTargetPath = Path.Combine(RootDirectory, "IPTarget.xml");
                YoonXml xc = new YoonXml(strTargetPath);
                xc.SaveFile(_pListClientIP, typeof(List<string>));
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }
        }

        public bool IsBound => _pServerSocket is {IsBound: true};

        public bool IsConnected => _pConnectedClientSocket is {Connected: true};

        public bool Open()
        {
            return Listen();
        }

        public bool Listen()
        {
            if (_pServerSocket is {IsBound: true}) return true;

            try
            {
                _pServerSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.IP);
                if (!IsRetryOpen)
                    OnShowMessageEvent(this,
                        new MessageArgs(eYoonStatus.Info, string.Format("Listen Port : {0}", Param.Port)));
                // Binding port and Listening per backlogging
                _pServerSocket.Bind(new IPEndPoint(IPAddress.Any, int.Parse(Param.Port)));
                _pServerSocket.Listen(int.Parse(Param.Backlog));
                // Associate the connection request
                IAsyncResult pResult = _pServerSocket.BeginAccept(_pAcceptHandler, null);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());

                if (!IsRetryOpen)
                    OnShowMessageEvent(this,
                        new MessageArgs(eYoonStatus.Error, string.Format("Listening Failure : Socket Error")));
                _pServerSocket?.Close();
                _pServerSocket = null;
                return false;
            }

            if (_pServerSocket.IsBound == true)
            {
                OnShowMessageEvent(this, new MessageArgs(eYoonStatus.Info, string.Format("Listen Success")));
                SaveParam();
                IsRetryOpen = false;
            }
            else
            {
                if (!IsRetryOpen)
                    OnShowMessageEvent(this,
                        new MessageArgs(eYoonStatus.Error, string.Format("Listen Failure : Bound Fail")));
                IsRetryOpen = true;
            }

            return _pServerSocket.IsBound;
        }

        public bool Listen(string strPort)
        {
            if (_pServerSocket is {IsBound: true}) return true;

            try
            {
                _pServerSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.IP);

                Param.Port = strPort;

                if (!IsRetryOpen)
                    OnShowMessageEvent(this,
                        new MessageArgs(eYoonStatus.Info, string.Format("Listen Port : {0}", Param.Port)));
                // Binding Port and Listening per backlogging
                _pServerSocket.Bind(new IPEndPoint(IPAddress.Any, int.Parse(Param.Port)));
                _pServerSocket.Listen(int.Parse(Param.Backlog));
                // Associate the connection request
                IAsyncResult asyncResult = _pServerSocket.BeginAccept(_pAcceptHandler, null);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());

                if (!IsRetryOpen)
                    OnShowMessageEvent(this,
                        new MessageArgs(eYoonStatus.Error, string.Format("Listening Failure : Socket Error")));
                if (_pServerSocket != null)
                {
                    _pServerSocket.Close();
                    _pServerSocket = null;
                }

                return false;
            }

            if (_pServerSocket.IsBound == true)
            {
                IsRetryOpen = false;
                OnShowMessageEvent(this, new MessageArgs(eYoonStatus.Info, string.Format("Listen Success")));
                SaveParam();
            }
            else
            {
                if (!IsRetryOpen)
                    OnShowMessageEvent(this,
                        new MessageArgs(eYoonStatus.Error, string.Format("Listen Failure : Bound Fail")));
                IsRetryOpen = true;
            }

            return true;
        }

        public void Close()
        {
            if (IsRetryOpen)
            {
                IsRetryOpen = false;
                Thread.Sleep(100);
            }

            OnShowMessageEvent(this, new MessageArgs(eYoonStatus.Info, string.Format("Close Listen")));

            if (_pServerSocket == null)
                return;

            _pServerSocket.Close();
            _pServerSocket = null;
        }

        public void OnAcceptClientEvent(IAsyncResult ar)
        {
            if (_pServerSocket is not {IsBound: true}) return;

            Socket pClientSocket;
            try
            {
                // Accept the connection request
                pClientSocket = _pServerSocket.EndAccept(ar);
                // Get Client IP Address and Save to IPTarget.xml
                string strAddressFull = pClientSocket.RemoteEndPoint.ToString();
                bool bDuplicatedAddress = false;
                foreach (string strIP in _pListClientIP)
                {
                    if (strIP == strAddressFull)
                        bDuplicatedAddress = true;
                }

                if (!bDuplicatedAddress)
                    _pListClientIP.Add(strAddressFull);
                OnShowMessageEvent(this, new MessageArgs(eYoonStatus.Info,
                    $"Acception Success To Client : {strAddressFull}"));
                SaveTarget();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());

                OnShowMessageEvent(this, new MessageArgs(eYoonStatus.Error, string.Format("Acceptation Failure")));
                return;
            }

            // Save the working socket in the 4,096 size socket object
            AsyncObject ao = new AsyncObject(BUFFER_SIZE) {WorkingSocket = pClientSocket};
            // Save the client socket
            _pConnectedClientSocket = pClientSocket;
            try
            {
                // Receive the incoming data asynchronously
                pClientSocket.BeginReceive(ao.Buffer, 0, ao.Buffer.Length, SocketFlags.None, _pReceiveHandler, ao);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());

                OnShowMessageEvent(this,
                    new MessageArgs(eYoonStatus.Error, string.Format("Receive Waiting Failure : Socket Error")));
                return;
            }
        }

        private Thread _pThreadRetryListen = null;
        private Stopwatch _pStopWatch = new Stopwatch();

        public void OnRetryThreadStart()
        {
            if (Param.RetryListen == bool.FalseString)
                return;
            _pThreadRetryListen = new Thread(new ThreadStart(ProcessRetry)) {Name = "Retry Listen"};
            _pThreadRetryListen.Start();
        }

        public void OnRetryThreadStop()
        {
            if (_pThreadRetryListen == null) return;

            if (_pThreadRetryListen.IsAlive)
                _pThreadRetryListen.Interrupt();
            _pThreadRetryListen = null;
        }

        private void ProcessRetry()
        {
            _pStopWatch.Stop();
            _pStopWatch.Reset();
            _pStopWatch.Start();

            OnShowMessageEvent(this, new MessageArgs(eYoonStatus.Info, string.Format("Listen Retry Start")));
            int nCount = Convert.ToInt32(Param.RetryCount);
            int nTimeOut = Convert.ToInt32(Param.Timeout);

            for (int iRetry = 0; iRetry < nCount; iRetry++)
            {
                //// Error : Timeout
                if (_pStopWatch.ElapsedMilliseconds >= nTimeOut)
                    break;

                //// Error : Retry Listen is false suddenly
                if (!IsRetryOpen)
                    break;

                ////  Success to connect
                if (_pServerSocket is {IsBound: true})
                {
                    OnShowMessageEvent(this, new MessageArgs(eYoonStatus.Info, string.Format("Listen Retry Success")));
                    IsRetryOpen = false;
                    break;
                }

                Listen();
            }

            _pStopWatch.Stop();
            _pStopWatch.Reset();

            if (_pServerSocket == null)
            {
                OnShowMessageEvent(this,
                    new MessageArgs(eYoonStatus.Error, "Listen Retry Failure : Listen Socket Empty"));
                return;
            }

            if (_pServerSocket.IsBound == false)
            {
                OnShowMessageEvent(this,
                    new MessageArgs(eYoonStatus.Error, string.Format("Listen Retry Failure : Connection Fail")));
            }
        }

        public bool Send(string strBuffer)
        {
            if (_pServerSocket == null || _pConnectedClientSocket == null)
                return false;
            if (_pConnectedClientSocket.Connected == false)
            {
                OnShowMessageEvent(this, new MessageArgs(eYoonStatus.Error, "Send Failure : Connection Fail"));
                return false;
            }

            IsSend = false;
            AsyncObject pObject = new AsyncObject(1);

            // Convert the byte buffer to ASCII
            pObject.Buffer = Encoding.ASCII.GetBytes(strBuffer);
            pObject.WorkingSocket = _pConnectedClientSocket;

            try
            {
                _pConnectedClientSocket.BeginSend(pObject.Buffer, 0, pObject.Buffer.Length, SocketFlags.None,
                    _pSendHandler, pObject);
                OnShowMessageEvent(this,
                    new MessageArgs(eYoonStatus.Info, string.Format("Send Message To String : " + strBuffer)));
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
                OnShowMessageEvent(this, new MessageArgs(eYoonStatus.Error, "Send Failure : Client Socket Error"));
            }

            return false;
        }

        public bool Send(byte[] pBuffer)
        {
            if (_pServerSocket == null || _pConnectedClientSocket == null)
                return false;
            if (_pConnectedClientSocket.Connected == false)
            {
                OnShowMessageEvent(this, new MessageArgs(eYoonStatus.Error, "Send Failure : Connection Fail"));
                return false;
            }

            IsSend = false;

            AsyncObject pObject = new AsyncObject(1);

            string strBuffer = Encoding.ASCII.GetString(pBuffer);
            // Convert the byte buffer to ASCII
            pObject.Buffer = Encoding.ASCII.GetBytes(strBuffer);
            pObject.WorkingSocket = _pConnectedClientSocket;

            try
            {
                _pConnectedClientSocket.BeginSend(pObject.Buffer, 0, pObject.Buffer.Length, SocketFlags.None,
                    _pSendHandler, pObject);
                //strBuff.Replace("\0", "");
                //strBuff = "[S] " + strBuff;
                OnShowMessageEvent(this,
                    new MessageArgs(eYoonStatus.Info, string.Format("Send Message To String : " + strBuffer)));
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
                OnShowMessageEvent(this, new MessageArgs(eYoonStatus.Error, "Send Failure : Client Socket Error"));
            }

            return false;
        }

        private void OnSendEvent(IAsyncResult ar)
        {
            if (_pServerSocket == null || _pConnectedClientSocket == null) return;

            AsyncObject pObject = (AsyncObject) ar.AsyncState;
            if (!pObject.WorkingSocket.Connected)
            {
                OnShowMessageEvent(this,
                    new MessageArgs(eYoonStatus.Error, string.Format("Send Failure : Socket Disconnect")));
                return;
            }

            int nLengthSend;

            try
            {
                // Send the data and take the information
                nLengthSend = pObject.WorkingSocket.EndSend(ar);
                IsSend = true;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());

                OnShowMessageEvent(this,
                    new MessageArgs(eYoonStatus.Error, string.Format("Send Failure : Socket Error")));
                return;
            }

            if (nLengthSend <= 0) return;
            // Declare an array of bytes sent
            byte[] msgByte = new byte[nLengthSend];
            Array.Copy(pObject.Buffer, msgByte, nLengthSend);

            OnShowMessageEvent(this, new MessageArgs(eYoonStatus.Info,
                $"Send Success : {Encoding.ASCII.GetString(msgByte)}"));
        }

        private void OnReceiveEvent(IAsyncResult pResult)
        {
            if (_pServerSocket == null || _pConnectedClientSocket == null) return;

            try
            {
                // Search the socket and object in synchronized states
                AsyncObject pObject = (AsyncObject) pResult.AsyncState;
                if (!pObject.WorkingSocket.Connected)
                {
                    OnShowMessageEvent(this,
                        new MessageArgs(eYoonStatus.Error, string.Format("Receive Failure : Socket Disconnect")));
                    return;
                }

                // Read the data from device
                int bytesRead = pObject.WorkingSocket.EndReceive(pResult);
                if (bytesRead > 0)
                {
                    // Save the data up to now for being ready more data
                    ReceiveMessage.Append(Encoding.ASCII.GetString(pObject.Buffer, 0, bytesRead));
                    // wait the receive
                    pObject.WorkingSocket.BeginReceive(pObject.Buffer, 0, pObject.Buffer.Length, 0, _pReceiveHandler,
                        pObject);

                    byte[] buffer = new byte[bytesRead];
                    Buffer.BlockCopy(pObject.Buffer, 0, buffer, 0, buffer.Length);
                    OnShowReceiveDataEvent(this, new BufferArgs(buffer));
                    OnShowMessageEvent(this,
                        new MessageArgs(eYoonStatus.Info,
                            string.Format("Receive Sucess : {0}", Encoding.ASCII.GetString(buffer))));
                    //strRecv = Encoding.ASCII.GetString(state.buffer, 0, bytesRead);
                }
                else // Timeout
                {
                    //if (state.sb.Length > 1)
                    //{
                    //        strRecv = state.sb.ToString();
                    //        ReceiveBufferEvent(strRecv);
                    //}
                    OnShowMessageEvent(this, new MessageArgs(eYoonStatus.Error, "Receive Failure : Connection Fail"));
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());

                OnShowMessageEvent(this, new MessageArgs(eYoonStatus.Error, "Receive Failure: Socket Error"));
            }
        }
    }
}
