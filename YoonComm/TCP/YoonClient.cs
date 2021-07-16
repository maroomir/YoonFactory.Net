﻿using System;
using System.Text;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.Diagnostics;
using YoonFactory.Files;

namespace YoonFactory.Comm.TCP
{
    public class YoonClient : IYoonTcpIp
    {

        #region IDisposable Support

        private bool _disposedValue = false;

        protected virtual void Dispose(bool disposing)
        {
            if (!_disposedValue)
            {
                if (disposing)
                {
                    if (_pClientSocket != null)
                    {
                        _pClientSocket.Disconnect(false);
                        _pClientSocket.Dispose();
                    }
                }

                // Close RetryConnect Thread
                OnRetryThreadStop();
                // Refund memory to main function
                _pThreadRetryConnect = null;
                _pClientSocket = null;
                _pSendHandler = null;
                _pReceiveHandler = null;
                _disposedValue = true;
            }
        }

        ~YoonClient()
        {
            Dispose(false);
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        #endregion

        public YoonClient()
        {
            // Initialize the delegate to use async works
            _pSendHandler = new AsyncCallback(OnSendEvent);
            _pReceiveHandler = new AsyncCallback(OnReceiveEvent);
            // Initialize message parameter
            ReceiveMessage = new StringBuilder(string.Empty);
        }

        public YoonClient(string strParamDirectory)
        {
            // Initialize the delegate to use async works
            _pSendHandler = new AsyncCallback(OnSendEvent);
            _pReceiveHandler = new AsyncCallback(OnReceiveEvent);
            // Initialize message parameter
            ReceiveMessage = new StringBuilder(string.Empty);

            RootDirectory = strParamDirectory;
            LoadParam();
        }

        protected class AsyncObject
        {
            public byte[] Buffer;
            public Socket WorkingSocket;

            public AsyncObject(int nBufferSize)
            {
                this.Buffer = new Byte[nBufferSize];
            }
        }

        public event ShowMessageCallback OnShowMessageEvent;
        public event RecieveDataCallback OnShowReceiveDataEvent;
        public bool IsRetryOpen { get; private set; } = false;
        public bool IsSend { get; private set; } = false;
        public StringBuilder ReceiveMessage { get; private set; } = null;
        public string RootDirectory { get; set; } = Path.Combine(Directory.GetCurrentDirectory(), "YoonFactory");

        public string Address
        {
            get => Param.IP;
            set
            {
                if (CommunicationFactory.VerifyIPAddress(value))
                    Param.IP = value;
            }
        }

        public string Port
        {
            get => Param.Port;
            set
            {
                if (CommunicationFactory.VerifyPort(value))
                    Param.Port = value;
            }
        }

        private const int BUFFER_SIZE = 4096;

        private Socket _pClientSocket = null;
        private AsyncCallback _pReceiveHandler;
        private AsyncCallback _pSendHandler;

        private struct Param
        {
            public static string IP = "127.0.0.1";
            public static string Port = "1234";
            public static string RetryConnect = "true";
            public static string Timeout = "10000";
            public static string RetryCount = "10";
            public static string ElapsedTime = "5000";
        }

        public void CopyFrom(IYoonComm pComm)
        {
            if (pComm is not YoonClient pClient) return;
            Disconnect();
            if (pClient.IsConnected)
                pClient.Disconnect();

            LoadParam();
            Address = pClient.Address;
            Port = pClient.Port;
        }

        public IYoonComm Clone()
        {
            Disconnect();

            YoonClient pClient = new YoonClient();
            pClient.LoadParam();
            pClient.Address = Address;
            pClient.Port = Port;
            return pClient;
        }

        public void SetParam(string strIP, string strPort, string strRetryConnect, string strTimeout,
            string strRetryCount, string strElapsedTime)
        {
            Param.IP = strIP;
            Param.Port = strPort;
            Param.RetryConnect = strRetryConnect;
            Param.Timeout = strTimeout;
            Param.RetryCount = strRetryCount;
            Param.ElapsedTime = strElapsedTime;
        }

        public void SetParam(string strIP, int nPort, bool bRetryConnect, int nTimeout, int nRetryCount,
            int nElapsedTime)
        {
            Param.IP = strIP;
            Param.Port = nPort.ToString();
            Param.RetryConnect = bRetryConnect.ToString();
            Param.Timeout = nTimeout.ToString();
            Param.RetryCount = nRetryCount.ToString();
            Param.ElapsedTime = nElapsedTime.ToString();
        }

        public void LoadParam()
        {
            string strFilePath = Path.Combine(RootDirectory, "IPClient.ini");
            using (YoonIni ic = new YoonIni(strFilePath))
            {
                ic.LoadFile();
                Param.IP = ic["Client"]["IP"].ToString("127.0.0.1");
                Param.Port = ic["Client"]["Port"].ToString("1234");
                Param.RetryConnect = ic["Client"]["RetryConnect"].ToString("true");
                Param.RetryCount = ic["Client"]["RetryCount"].ToString("100");
                Param.Timeout = ic["Client"]["TimeOut"].ToString("10000");
                Param.ElapsedTime = ic["Client"]["ElapsedTime"].ToString("5000");
            }
        }

        public void SaveParam()
        {
            string strFilePath = Path.Combine(RootDirectory, "IPClient.ini");
            using (YoonIni ic = new YoonIni(strFilePath))
            {
                ic["Client"]["IP"] = Param.IP;
                ic["Client"]["Port"] = Param.Port;
                ic["Client"]["RetryConnect"] = Param.RetryConnect;
                ic["Client"]["RetryCount"] = Param.RetryCount;
                ic["Client"]["TimeOut"] = Param.Timeout;
                ic["Client"]["ElapsedTime"] = Param.ElapsedTime;
                ic.SaveFile();
            }
        }

        public bool IsConnected
        {
            get
            {
                if (_pClientSocket == null) return false;
                return _pClientSocket.Connected;
            }
        }

        public bool Open()
        {
            return Connect();
        }

        public bool Connect()
        {
            if (_pClientSocket is {Connected: true}) return true;

            try
            {
                _pClientSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.IP);

                //IPHostEntry ipHostInfo = Dns.Resolve(Param.fIP);
                if (!IsRetryOpen)
                    OnShowMessageEvent(this,
                        new MessageArgs(eYoonStatus.Info,
                            string.Format("Connection Attempt : {0}/{1}", Param.IP, Param.Port)));
                IPAddress pIPAddress = IPAddress.Parse(Param.IP);
                IAsyncResult pResult =
                    _pClientSocket.BeginConnect(new IPEndPoint(pIPAddress, int.Parse(Param.Port)), null, null);
                if (pResult.AsyncWaitHandle.WaitOne(100, false))
                {
                    _pClientSocket.EndConnect(pResult);
                }
                else
                {
                    IsRetryOpen = true;
                    if (!IsRetryOpen)
                        OnShowMessageEvent(this,
                            new MessageArgs(eYoonStatus.Error, "Connection Failure : Client Connecting delay"));
                    if (_pClientSocket == null) return false;
                    _pClientSocket.Close();
                    _pClientSocket = null;

                    return false;
                }
                //m_socket.Connect(m_strIP, int.Parse(m_strPort));
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());

                if (!IsRetryOpen)
                    OnShowMessageEvent(this, new MessageArgs(eYoonStatus.Error, "Connection Failure : Socket Error"));
                if (_pClientSocket == null) return false;
                _pClientSocket.Close();
                _pClientSocket = null;
                return false;
            }

            // Save the working socket in the 4,096 size socket object
            AsyncObject ao = new AsyncObject(BUFFER_SIZE) {WorkingSocket = _pClientSocket};

            try
            {
                // Receive the incoming data asynchronously
                _pClientSocket.BeginReceive(ao.Buffer, 0, ao.Buffer.Length, SocketFlags.None, _pReceiveHandler, ao);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
                OnShowMessageEvent(this, new MessageArgs(eYoonStatus.Error, "Receive Waiting Failure : Socket Error"));
            }


            if (_pClientSocket.Connected != true) return _pClientSocket.Connected;
            IsRetryOpen = false;
            OnShowMessageEvent(this, new MessageArgs(eYoonStatus.Info, "Connection Success"));
            SaveParam();

            return _pClientSocket.Connected;
        }

        public bool Connect(string strIp, string strPort)
        {
            if (_pClientSocket is {Connected: true}) return true;

            try
            {
                _pClientSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.IP);

                Param.IP = strIp;
                Param.Port = strPort;

                if (!IsRetryOpen)
                    OnShowMessageEvent(this,
                        new MessageArgs(eYoonStatus.Info,
                            string.Format("Connection Attempt : {0}/{1}", Param.IP, Param.Port)));
                //IPHostEntry ipHostInfo = Dns.Resolve(Param.fIP);
                IPAddress ipAddress = IPAddress.Parse(Param.IP);
                IAsyncResult asyncResult =
                    _pClientSocket.BeginConnect(new IPEndPoint(ipAddress, int.Parse(Param.Port)), null, null);
                if (asyncResult.AsyncWaitHandle.WaitOne(100, false))
                {
                    _pClientSocket.EndConnect(asyncResult);
                }
                else
                {
                    IsRetryOpen = true;
                    if (!IsRetryOpen)
                        OnShowMessageEvent(this,
                            new MessageArgs(eYoonStatus.Error, "Connection Failure : Client Connecting delay"));
                    if (_pClientSocket == null) return false;
                    _pClientSocket.Close();
                    _pClientSocket = null;

                    return false;
                }
                //m_ClientSocket.Connect(m_strIP, int.Parse(m_strPort));
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());

                if (!IsRetryOpen)
                    OnShowMessageEvent(this, new MessageArgs(eYoonStatus.Error, "Connection Failure : Socket Error"));
                if (_pClientSocket == null) return false;
                _pClientSocket.Close();
                _pClientSocket = null;

                return false;
            }

            // Save the working socket in the 4,096 size socket object
            AsyncObject pObject = new AsyncObject(BUFFER_SIZE) {WorkingSocket = _pClientSocket};

            try
            {
                // Receive the incoming data asynchronously
                _pClientSocket.BeginReceive(pObject.Buffer, 0, pObject.Buffer.Length, SocketFlags.None, _pReceiveHandler, pObject);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
                OnShowMessageEvent(this, new MessageArgs(eYoonStatus.Error, "Receive Waiting Failure : Socket Error"));
            }

            if (_pClientSocket.Connected != true) return _pClientSocket.Connected;
            IsRetryOpen = false;
            OnShowMessageEvent(this, new MessageArgs(eYoonStatus.Info, "Connection Success"));
            SaveParam();

            return _pClientSocket.Connected;
        }

        public void Close()
        {
            Disconnect();
        }

        public void Disconnect()
        {
            if (IsRetryOpen)
            {
                IsRetryOpen = false;
                Thread.Sleep(100);
            }

            OnShowMessageEvent(this, new MessageArgs(eYoonStatus.Info, "Close Connection"));

            if (_pClientSocket == null)
                return;

            _pClientSocket.Close();
            _pClientSocket = null;
        }

        private Thread _pThreadRetryConnect = null;
        private Stopwatch _pStopWatch = new Stopwatch();

        /// <summary>
        /// Start the retry thread
        /// </summary>
        public void OnRetryThreadStart()
        {
            if (Param.RetryConnect == Boolean.FalseString)
                return;

            _pThreadRetryConnect = new Thread(new ThreadStart(ProcessRetry)) {Name = "Retry Connect"};
            _pThreadRetryConnect.Start();
        }

        public void OnRetryThreadStop()
        {
            if (_pThreadRetryConnect == null) return;

            if (_pThreadRetryConnect.IsAlive)
                _pThreadRetryConnect.Interrupt();
            _pThreadRetryConnect = null;
        }

        private void ProcessRetry()
        {
            _pStopWatch.Stop();
            _pStopWatch.Reset();
            _pStopWatch.Start();

            OnShowMessageEvent(this, new MessageArgs(eYoonStatus.Info, "Connection Retry Start"));
            int nCount = Convert.ToInt32(Param.RetryCount);
            int nTimeOut = Convert.ToInt32(Param.Timeout);

            for (int iRetry = 0; iRetry < nCount; iRetry++)
            {
                //// Error : Timeout
                if (_pStopWatch.ElapsedMilliseconds >= nTimeOut)
                    break;

                //// Error : IsRetryConnect is false suddenly
                if (!IsRetryOpen)
                    break;

                ////  Success to connect
                if (_pClientSocket != null)
                {
                    if (_pClientSocket.Connected == true)
                    {
                        OnShowMessageEvent(this, new MessageArgs(eYoonStatus.Info, "Connection Retry Success"));
                        IsRetryOpen = false;
                        break;
                    }
                }

                //m_clientSocket.Connect(m_strIP, m_nPort);
                Connect();
            }

            _pStopWatch.Stop();
            _pStopWatch.Reset();

            if (_pClientSocket == null)
            {
                OnShowMessageEvent(this,
                    new MessageArgs(eYoonStatus.Error, "Connection Retry Failure : Connection Socket Empty"));
                return;
            }

            if (_pClientSocket.Connected == false)
                OnShowMessageEvent(this,
                    new MessageArgs(eYoonStatus.Error, "Connection Retry Failure : Connection Fail"));
        }

        public bool Send(string strBuffer)
        {
            if (_pClientSocket == null)
                return false;
            if (_pClientSocket.Connected == false)
            {
                Disconnect();
                OnShowMessageEvent(this, new MessageArgs(eYoonStatus.Error, "Send Failure : Connection Fail"));
                IsRetryOpen = true;
                OnRetryThreadStart();
                return false;
            }

            IsSend = false;

            // Convert the ASCII to byte buffer
            AsyncObject pObject = new AsyncObject(1)
            {
                Buffer = Encoding.ASCII.GetBytes(strBuffer), WorkingSocket = _pClientSocket
            };

            try
            {
                _pClientSocket.BeginSend(pObject.Buffer, 0, pObject.Buffer.Length, SocketFlags.None, _pSendHandler,
                    pObject);
                OnShowMessageEvent(this,
                    new MessageArgs(eYoonStatus.Send, string.Format("Send Message To String : " + strBuffer)));
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
                OnShowMessageEvent(this, new MessageArgs(eYoonStatus.Error, "Send Failure : Socket Error"));
            }

            return false;
        }

        public bool Send(byte[] pBuffer)
        {
            if (_pClientSocket == null)
                return false;
            if (_pClientSocket.Connected == false)
            {
                Disconnect();
                OnShowMessageEvent(this, new MessageArgs(eYoonStatus.Error, "Send Failure : Connection Fail"));
                IsRetryOpen = true;
                OnRetryThreadStart();
                return false;
            }

            IsSend = false;

            AsyncObject pObject = new AsyncObject(1);

            string strBuff = Encoding.ASCII.GetString(pBuffer);
            // Convert the byte buffer to ASCII
            pObject.Buffer = Encoding.ASCII.GetBytes(strBuff);
            pObject.WorkingSocket = _pClientSocket;

            try
            {
                _pClientSocket.BeginSend(pObject.Buffer, 0, pObject.Buffer.Length, SocketFlags.None, _pSendHandler,
                    pObject);
                //strBuff.Replace("\0", "");
                //strBuff = "[S] " + strBuff;
                OnShowMessageEvent(this,
                    new MessageArgs(eYoonStatus.Send, string.Format("Send Message To String : " + strBuff)));
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
                OnShowMessageEvent(this, new MessageArgs(eYoonStatus.Error, "Send Failure : Socket Error"));
            }

            return false;
        }

        private void OnSendEvent(IAsyncResult pResult)
        {
            AsyncObject pObject = (AsyncObject) pResult.AsyncState;
            int nLengthSend;
            try
            {
                // Send the data and take the information
                nLengthSend = pObject.WorkingSocket.EndSend(pResult);
                if (!pObject.WorkingSocket.Connected)
                {
                    OnShowMessageEvent(this,
                        new MessageArgs(eYoonStatus.Error, string.Format("Send Failure : Socket Disconnect")));
                    return;
                }

                IsSend = true;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
                OnShowMessageEvent(this, new MessageArgs(eYoonStatus.Error, "Send Failure : Socket Error"));
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
            if (_pClientSocket == null) return;

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
                int nLengthRead = pObject.WorkingSocket.EndReceive(pResult);
                if (nLengthRead > 0)
                {
                    // Save the data up to now for being ready more data
                    ReceiveMessage.Append(Encoding.ASCII.GetString(pObject.Buffer, 0, nLengthRead));
                    // wait the receive
                    pObject.WorkingSocket.BeginReceive(pObject.Buffer, 0, pObject.Buffer.Length, 0, _pReceiveHandler,
                        pObject);

                    byte[] buffer = new byte[nLengthRead];
                    Buffer.BlockCopy(pObject.Buffer, 0, buffer, 0, buffer.Length);
                    OnShowReceiveDataEvent(this, new BufferArgs(buffer));
                    OnShowMessageEvent(this, new MessageArgs(eYoonStatus.Info,
                        $"Receive Success : {Encoding.ASCII.GetString(buffer)}"));
                    //strRecv = Encoding.ASCII.GetString(state.buffer, 0, bytesRead);
                }
                else // Timeout
                {
                    //if (state.sb.Length > 1)
                    //{
                    //        strRecv = state.sb.ToString();
                    //        ReceiveBufferEvent(strRecv);
                    //}
                    Disconnect();
                    OnShowMessageEvent(this, new MessageArgs(eYoonStatus.Error, "Receive Failure : Disconnection"));
                    IsRetryOpen = true;
                    OnRetryThreadStart();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());

                OnShowMessageEvent(this, new MessageArgs(eYoonStatus.Error, "Receive Failure : Socket Error"));
                Disconnect();
                IsRetryOpen = true;
                OnRetryThreadStart();
            }
        }
    }
}
