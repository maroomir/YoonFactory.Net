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
    public class YoonServer : IYoonComm
    {
        #region IDisposable Support

        private bool _disposedValue = false;

        protected virtual void Dispose(bool disposing)
        {
            if (!_disposedValue)
            {
                // Save Last setting
                SaveParameter();
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
                _pThreadRetry = null;

                _pAcceptHandler = null;
                _pReceiveHandler = null;
                _pSendHandler = null;

                _disposedValue = true;
            }
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
            LoadParameter();
            LoadTarget();

            ReceiveMessage = new StringBuilder(string.Empty);

            _pAcceptHandler = new AsyncCallback(OnAcceptClientEvent);
            _pReceiveHandler = new AsyncCallback(OnReceiveEvent);
            _pSendHandler = new AsyncCallback(OnSendEvent);
        }

        private class AsyncObject
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
            get => Parameter.Port;
            set
            {
                if (CommunicationFactory.VerifyTCPPort(value))
                    Parameter.Port = value;
            }
        }

        public void CopyFrom(IYoonComm pComm)
        {
            if (pComm is not YoonServer pServer) return;
            Close();
            if (pServer.IsConnected)
                pServer.Close();

            LoadParameter();
            Port = pServer.Port;
        }

        public IYoonComm Clone()
        {
            Close();

            YoonServer pServer = new YoonServer();
            pServer.LoadParameter();
            pServer.Port = Port;
            return pServer;
        }

        private const int BUFFER_SIZE = 4096;

        private Socket _pServerSocket = null;
        private Socket _pConnectedClientSocket = null;
        private AsyncCallback _pAcceptHandler;
        private AsyncCallback _pReceiveHandler;
        private AsyncCallback _pSendHandler;
        private List<string> _pListClientIp = null;

        private struct Parameter
        {
            public static string Port = "1234";
            public static string Backlog = "5";
            public static string RetryCount = "10";
            public static string RetryListen = "true";
            public static string Timeout = "10000";
        }

        public void SetParameter(string strPort, string strBacklog, string strRetryListen, string strTimeout,
            string strRetryCount)
        {
            Parameter.Port = strPort;
            Parameter.Backlog = strBacklog;
            Parameter.RetryCount = strRetryCount;
            Parameter.RetryListen = strRetryListen;
            Parameter.Timeout = strTimeout;
        }

        public void SetParameter(int strPort, int nBacklog, bool bRetryListen, int nTimeout, int nRetryCount)
        {
            Parameter.Port = strPort.ToString();
            Parameter.Backlog = nBacklog.ToString();
            Parameter.RetryListen = bRetryListen.ToString();
            Parameter.Timeout = nTimeout.ToString();
            Parameter.RetryCount = nRetryCount.ToString();
        }

        public void LoadParameter()
        {
            string strParamFilePath = Path.Combine(RootDirectory, "IPServer.ini");
            YoonIni pIni = new YoonIni(strParamFilePath);
            pIni.LoadFile();
            Parameter.Port = pIni["Server"]["Port"].ToString("1234");
            Parameter.Backlog = pIni["Server"]["Backlog"].ToString("5");
            Parameter.RetryListen = pIni["Server"]["RetryListen"].ToString("true");
            Parameter.RetryCount = pIni["Server"]["RetryCount"].ToString("10");
            Parameter.Timeout = pIni["Server"]["TimeOut"].ToString("10000");
            pIni.Dispose();
        }

        public void SaveParameter()
        {
            string strParamFilePath = Path.Combine(RootDirectory, "IPServer.ini");
            YoonIni pIni = new YoonIni(strParamFilePath);
            pIni["Server"]["Port"] = Parameter.Port;
            pIni["Server"]["Backlog"] = Parameter.Backlog;
            pIni["Server"]["RetryListen"] = Parameter.RetryListen;
            pIni["Server"]["RetryCount"] = Parameter.RetryCount;
            pIni["Server"]["TimeOut"] = Parameter.Timeout;
            pIni.SaveFile();
            pIni.Dispose();
        }

        public void LoadTarget()
        {
            try
            {
                string strTargetPath = Path.Combine(RootDirectory, "IPTarget.xml");
                YoonXml pXml = new YoonXml(strTargetPath);
                if (pXml.LoadFile(out object pTargetParam, typeof(List<string>)))
                    _pListClientIp = (List<string>) pTargetParam;
                else if (_pListClientIp == null)
                {
                    _pListClientIp = new List<string>();
                }

                _pListClientIp.Clear();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }
        }

        public void SaveTarget()
        {
            if (_pListClientIp == null)
            {
                _pListClientIp = new List<string>();
                _pListClientIp.Clear();
            }

            try
            {
                string strTargetPath = Path.Combine(RootDirectory, "IPTarget.xml");
                YoonXml pXml = new YoonXml(strTargetPath);
                pXml.SaveFile(_pListClientIp, typeof(List<string>));
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
                    OnShowMessageEvent?.Invoke(this,
                        new MessageArgs(eYoonStatus.Info, $"Listen Port : {Parameter.Port}"));
                // Binding port and Listening per backlogging
                _pServerSocket.Bind(new IPEndPoint(IPAddress.Any, int.Parse(Parameter.Port)));
                _pServerSocket.Listen(int.Parse(Parameter.Backlog));
                // Associate the connection request
                IAsyncResult pResult = _pServerSocket.BeginAccept(_pAcceptHandler, null);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());

                if (!IsRetryOpen)
                    OnShowMessageEvent?.Invoke(this,
                        new MessageArgs(eYoonStatus.Error, "Listening Failure : Socket Error"));
                _pServerSocket?.Close();
                _pServerSocket = null;
                return false;
            }

            if (_pServerSocket.IsBound == true)
            {
                OnShowMessageEvent?.Invoke(this, new MessageArgs(eYoonStatus.Info, "Listen Success"));
                SaveParameter();
                IsRetryOpen = false;
            }
            else
            {
                if (!IsRetryOpen)
                    OnShowMessageEvent?.Invoke(this,
                        new MessageArgs(eYoonStatus.Error, "Listen Failure : Bound Fail"));
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

                Parameter.Port = strPort;

                if (!IsRetryOpen)
                    OnShowMessageEvent?.Invoke(this,
                        new MessageArgs(eYoonStatus.Info, $"Listen Port : {Parameter.Port}"));
                // Binding Port and Listening per backlogging
                _pServerSocket.Bind(new IPEndPoint(IPAddress.Any, int.Parse(Parameter.Port)));
                _pServerSocket.Listen(int.Parse(Parameter.Backlog));
                // Associate the connection request
                IAsyncResult asyncResult = _pServerSocket.BeginAccept(_pAcceptHandler, null);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());

                if (!IsRetryOpen)
                    OnShowMessageEvent?.Invoke(this,
                        new MessageArgs(eYoonStatus.Error, "Listening Failure : Socket Error"));
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
                OnShowMessageEvent?.Invoke(this, new MessageArgs(eYoonStatus.Info, "Listen Success"));
                SaveParameter();
            }
            else
            {
                if (!IsRetryOpen)
                    OnShowMessageEvent?.Invoke(this,
                        new MessageArgs(eYoonStatus.Error, "Listen Failure : Bound Fail"));
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

            OnShowMessageEvent?.Invoke(this, new MessageArgs(eYoonStatus.Info, "Close Listen"));

            if (_pServerSocket == null)
                return;

            _pServerSocket.Close();
            _pServerSocket = null;
        }

        private void OnAcceptClientEvent(IAsyncResult pResult)
        {
            if (_pServerSocket is not {IsBound: true}) return;

            Socket pClientSocket;
            try
            {
                // Accept the connection request
                pClientSocket = _pServerSocket.EndAccept(pResult);
                // Get Client IP Address and Save to IPTarget.xml
                string strAddressFull = pClientSocket.RemoteEndPoint?.ToString();
                bool bDuplicatedAddress = false;
                foreach (string strIp in _pListClientIp)
                {
                    if (strIp == strAddressFull)
                        bDuplicatedAddress = true;
                }

                if (!bDuplicatedAddress)
                    _pListClientIp.Add(strAddressFull);
                OnShowMessageEvent?.Invoke(this, new MessageArgs(eYoonStatus.Info,
                    $"Success To Accept Client : {strAddressFull}"));
                SaveTarget();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());

                OnShowMessageEvent?.Invoke(this, new MessageArgs(eYoonStatus.Error, string.Format("Acceptation Failure")));
                return;
            }

            // Save the working socket in the 4,096 size socket object
            AsyncObject pObject = new AsyncObject(BUFFER_SIZE) {WorkingSocket = pClientSocket};
            // Save the client socket
            _pConnectedClientSocket = pClientSocket;
            try
            {
                // Receive the incoming data asynchronously
                pClientSocket.BeginReceive(pObject.Buffer, 0, pObject.Buffer.Length, SocketFlags.None, _pReceiveHandler, pObject);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());

                OnShowMessageEvent?.Invoke(this,
                    new MessageArgs(eYoonStatus.Error, string.Format("Receive Waiting Failure : Socket Error")));
                return;
            }
        }

        private Thread _pThreadRetry = null;
        private Stopwatch _pStopWatch = new Stopwatch();

        public void OnRetryThreadStart()
        {
            if (_pThreadRetry != null || Parameter.RetryListen == bool.FalseString)
                return;
            _pThreadRetry = new Thread(ProcessRetry) {Name = "Retry Listen"};
            _pThreadRetry.Start();
        }

        public void OnRetryThreadStop()
        {
            if (_pThreadRetry == null) return;

            if (_pThreadRetry.IsAlive)
            {
                _pThreadRetry.Interrupt();
                Thread.Sleep(100);
            }

            _pThreadRetry = null;
        }

        private void ProcessRetry()
        {
            _pStopWatch.Stop();
            _pStopWatch.Reset();
            _pStopWatch.Start();

            IsRetryOpen = true;

            OnShowMessageEvent?.Invoke(this, new MessageArgs(eYoonStatus.Info, "Listen Retry Start"));
            int nCount = Convert.ToInt32(Parameter.RetryCount);
            int nTimeOut = Convert.ToInt32(Parameter.Timeout);

            for (int iRetry = 0; iRetry < nCount; iRetry++)
            {
                // Error : Timeout
                if (_pStopWatch.ElapsedMilliseconds >= nTimeOut)
                    break;

                // Error : Retry Listen is false suddenly
                if (!IsRetryOpen)
                    break;

                //  Success to connect
                if (_pServerSocket is {IsBound: true})
                {
                    OnShowMessageEvent?.Invoke(this, new MessageArgs(eYoonStatus.Info, "Listen Retry Success"));
                    IsRetryOpen = false;
                    break;
                }

                Listen();
            }

            _pStopWatch.Stop();
            _pStopWatch.Reset();

            if (_pServerSocket == null)
            {
                OnShowMessageEvent?.Invoke(this,
                    new MessageArgs(eYoonStatus.Error, "Listen Retry Failure : Listen Socket Empty"));
                return;
            }

            if (_pServerSocket.IsBound == false)
            {
                OnShowMessageEvent?.Invoke(this,
                    new MessageArgs(eYoonStatus.Error, "Listen Retry Failure : Connection Fail"));
            }
        }

        public bool Send(string strBuffer)
        {
            if (_pServerSocket == null || _pConnectedClientSocket == null)
                return false;
            if (_pConnectedClientSocket.Connected == false)
            {
                OnShowMessageEvent?.Invoke(this, new MessageArgs(eYoonStatus.Error, "Send Failure : Connection Fail"));
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
                OnShowMessageEvent?.Invoke(this,
                    new MessageArgs(eYoonStatus.Info, "Send Message To String : " + strBuffer));
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
                OnShowMessageEvent?.Invoke(this, new MessageArgs(eYoonStatus.Error, "Send Failure : Client Socket Error"));
            }

            return false;
        }

        public bool Send(byte[] pBuffer)
        {
            if (_pServerSocket == null || _pConnectedClientSocket == null)
                return false;
            if (_pConnectedClientSocket.Connected == false)
            {
                OnShowMessageEvent?.Invoke(this, new MessageArgs(eYoonStatus.Error, "Send Failure : Connection Fail"));
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
                OnShowMessageEvent?.Invoke(this,
                    new MessageArgs(eYoonStatus.Info, "Send Message To String : " + strBuffer));
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
                OnShowMessageEvent?.Invoke(this, new MessageArgs(eYoonStatus.Error, "Send Failure : Client Socket Error"));
            }

            return false;
        }

        private void OnSendEvent(IAsyncResult pResult)
        {
            if (_pServerSocket == null || _pConnectedClientSocket == null) return;

            AsyncObject pObject = (AsyncObject) pResult.AsyncState;
            if (!pObject.WorkingSocket.Connected)
            {
                OnShowMessageEvent?.Invoke(this,
                    new MessageArgs(eYoonStatus.Error, "Send Failure : Socket Disconnect"));
                return;
            }

            int nLengthSend;

            try
            {
                // Send the data and take the information
                nLengthSend = pObject.WorkingSocket.EndSend(pResult);
                IsSend = true;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());

                OnShowMessageEvent?.Invoke(this,
                    new MessageArgs(eYoonStatus.Error, "Send Failure : Socket Error"));
                return;
            }

            if (nLengthSend <= 0) return;
            // Declare an array of bytes sent
            byte[] msgByte = new byte[nLengthSend];
            Array.Copy(pObject.Buffer, msgByte, nLengthSend);

            OnShowMessageEvent?.Invoke(this, new MessageArgs(eYoonStatus.Info,
                $"Send Success : {Encoding.ASCII.GetString(msgByte)}"));
        }

        private void OnReceiveEvent(IAsyncResult pResult)
        {
            if (_pServerSocket == null || _pConnectedClientSocket == null) return;

            try
            {
                // Search the socket and object in synchronized states
                AsyncObject pObject = (AsyncObject) pResult.AsyncState;
                Debug.Assert(pObject != null, nameof(pObject) + " != null");
                if (!pObject.WorkingSocket.Connected)
                {
                    OnShowMessageEvent?.Invoke(this,
                        new MessageArgs(eYoonStatus.Error, "Receive Failure : Socket Disconnect"));
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
                    OnShowReceiveDataEvent?.Invoke(this, new BufferArgs(buffer));
                    OnShowMessageEvent?.Invoke(this,
                        new MessageArgs(eYoonStatus.Info, $"Receive Success : {Encoding.ASCII.GetString(buffer)}"));
                }
                else // Timeout
                {
                    OnShowMessageEvent?.Invoke(this, new MessageArgs(eYoonStatus.Error, "Receive Failure : Connection Fail"));
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());

                OnShowMessageEvent?.Invoke(this, new MessageArgs(eYoonStatus.Error, "Receive Failure: Socket Error"));
            }
        }
    }
}
