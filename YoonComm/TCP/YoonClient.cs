using System;
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
                    if (m_clientSocket != null)
                    {
                        m_clientSocket.Disconnect(false);
                        m_clientSocket.Dispose();
                    }
                }
                // Close RetryConnect Thread
                OnRetryThreadStop();
                // Refund memory to main function
                m_threadRetryConnect = null;
                m_clientSocket = null;
                m_sendHandler = null;
                m_receiveHandler = null;
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
            m_sendHandler = new AsyncCallback(OnSendEvent);
            m_receiveHandler = new AsyncCallback(OnReceiveEvent);
            // Initialize message parameter
            sbReceiveMessage = new StringBuilder(string.Empty);
        }

        public YoonClient(string strParamDirectory)
        {
            // Initialize the delegate to use async works
            m_sendHandler = new AsyncCallback(OnSendEvent);
            m_receiveHandler = new AsyncCallback(OnReceiveEvent);
            // Initialize message parameter
            sbReceiveMessage = new StringBuilder(string.Empty);

            RootDirectory = strParamDirectory;
            LoadParam();
        }

        protected class AsyncObject
        {
            public byte[] Buffer;
            public Socket WorkingSocket;

            public AsyncObject(Int32 bufferSize)
            {
                this.Buffer = new Byte[bufferSize];
            }
        }

        public event ShowMessageCallback OnShowMessageEvent;
        public event RecieveDataCallback OnShowReceiveDataEvent;
        public bool IsRetryOpen { get; private set; } = false;
        public bool IsSend { get; private set; } = false;
        public StringBuilder sbReceiveMessage { get; private set; } = null;
        public string RootDirectory { get; set; } = Path.Combine(Directory.GetCurrentDirectory(), "YoonFactory");
        public string Address
        {
            get => Param.IP;
            set
            {
                if (TCPFactory.VerifyIPAddress(value))
                    Param.IP = value;
            }
        }
        public string Port
        {
            get => Param.Port;
            set
            {
                if (TCPFactory.VerifyPort(value))
                    Param.Port = value;
            }
        }

        private const int BUFFER_SIZE = 4096;

        private Socket m_clientSocket = null;
        private AsyncCallback m_receiveHandler;
        private AsyncCallback m_sendHandler;

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
        public void SetParam(string strIP, string strPort, string strRetryConnect, string strTimeout, string strRetryCount, string strElapsedTime)
        {
            Param.IP = strIP;
            Param.Port = strPort;
            Param.RetryConnect = strRetryConnect;
            Param.Timeout = strTimeout;
            Param.RetryCount = strRetryCount;
            Param.ElapsedTime = strElapsedTime;
        }

        public void SetParam(string strIP, int nPort, bool bRetryConnect, int nTimeout, int nRetryCount, int nElapsedTime)
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
                if (m_clientSocket == null) return false;
                return m_clientSocket.Connected;
            }
        }

        public bool Open()
        {
            return Connect();
        }

        public bool Connect()
        {
            if (m_clientSocket != null)
            {
                if (m_clientSocket.Connected == true)
                    return true;
            }

            try
            {
                m_clientSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.IP);

                //IPHostEntry ipHostInfo = Dns.Resolve(Param.fIP);
                if (!IsRetryOpen)
                    OnShowMessageEvent(this, new MessageArgs(eYoonStatus.Info, string.Format("Connection Attempt : {0}/{1}", Param.IP, Param.Port)));
                IPAddress ipAddress = IPAddress.Parse(Param.IP);
                IAsyncResult asyncResult = m_clientSocket.BeginConnect(new IPEndPoint(ipAddress, int.Parse(Param.Port)), null, null);
                if (asyncResult.AsyncWaitHandle.WaitOne(100, false))
                {
                    m_clientSocket.EndConnect(asyncResult);
                }
                else
                {
                    IsRetryOpen = true;
                    if (!IsRetryOpen)
                        OnShowMessageEvent(this, new MessageArgs(eYoonStatus.Error, "Connection Failure : Client Connecting delay"));
                    if (m_clientSocket != null)
                    {
                        m_clientSocket.Close();
                        m_clientSocket = null;
                    }
                    return false;
                }
                //m_socket.Connect(m_strIP, int.Parse(m_strPort));
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());

                if (!IsRetryOpen)
                    OnShowMessageEvent(this, new MessageArgs(eYoonStatus.Error, "Connection Failure : Socket Error"));
                if (m_clientSocket != null)
                {
                    m_clientSocket.Close();
                    m_clientSocket = null;
                }
                return false;
            }

            //// 4096의 크기를 갖는 Byte 배열을 가진 AsyncObject Class를 생성한다.
            AsyncObject ao = new AsyncObject(BUFFER_SIZE);

            //// 작업 중인 소켓을 저장하기 위해 SocketClient를 할당한다.
            ao.WorkingSocket = m_clientSocket;

            try
            {
                //// 비동기적으로 들어오는 자료를 수신하기 위해 BeginReceive 메서드 사용!
                m_clientSocket.BeginReceive(ao.Buffer, 0, ao.Buffer.Length, SocketFlags.None, m_receiveHandler, ao);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
                OnShowMessageEvent(this, new MessageArgs(eYoonStatus.Error, "Receive Waiting Failure : Socket Error"));
            }


            if (m_clientSocket.Connected == true)
            {
                IsRetryOpen = false;
                OnShowMessageEvent(this, new MessageArgs(eYoonStatus.Info, "Connection Success"));
                SaveParam();
            }
            return m_clientSocket.Connected;
        }

        public bool Connect(string strIp, string strPort)
        {
            if (m_clientSocket != null)
            {
                if (m_clientSocket.Connected == true)
                    return true;
            }

            try
            {
                m_clientSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.IP);

                Param.IP = strIp;
                Param.Port = strPort;

                if (!IsRetryOpen)
                    OnShowMessageEvent(this, new MessageArgs(eYoonStatus.Info, string.Format("Connection Attempt : {0}/{1}", Param.IP, Param.Port)));
                //IPHostEntry ipHostInfo = Dns.Resolve(Param.fIP);
                IPAddress ipAddress = IPAddress.Parse(Param.IP);
                IAsyncResult asyncResult = m_clientSocket.BeginConnect(new IPEndPoint(ipAddress, int.Parse(Param.Port)), null, null);
                if (asyncResult.AsyncWaitHandle.WaitOne(100, false))
                {
                    m_clientSocket.EndConnect(asyncResult);
                }
                else
                {
                    IsRetryOpen = true;
                    if (!IsRetryOpen)
                        OnShowMessageEvent(this, new MessageArgs(eYoonStatus.Error, "Connection Failure : Client Connecting delay"));
                    if (m_clientSocket != null)
                    {
                        m_clientSocket.Close();
                        m_clientSocket = null;
                    }
                    return false;
                }
                //m_ClientSocket.Connect(m_strIP, int.Parse(m_strPort));
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());

                if (!IsRetryOpen)
                    OnShowMessageEvent(this, new MessageArgs(eYoonStatus.Error, "Connection Failure : Socket Error"));
                if (m_clientSocket != null)
                {
                    m_clientSocket.Close();
                    m_clientSocket = null;
                }
                return false;
            }

            //// 4096 바이트의 크기를 갖는 바이트 배열을 가진 AsyncObject 클래스를 생성한다.
            AsyncObject ao = new AsyncObject(BUFFER_SIZE);

            //// 작업 중인 소켓을 저장하기 위해 socketClient를 할당한다.
            ao.WorkingSocket = m_clientSocket;

            try
            {
                //// 비동기적으로 들어오는 자료를 수신하기 위해 BeginReceive 메서드 사용!
                m_clientSocket.BeginReceive(ao.Buffer, 0, ao.Buffer.Length, SocketFlags.None, m_receiveHandler, ao);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
                OnShowMessageEvent(this, new MessageArgs(eYoonStatus.Error, "Receive Waiting Failure : Socket Error"));
            }

            if (m_clientSocket.Connected == true)
            {
                IsRetryOpen = false;
                OnShowMessageEvent(this, new MessageArgs(eYoonStatus.Info, "Connection Success"));
                SaveParam();
            }
            return m_clientSocket.Connected;
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

            if (m_clientSocket == null)
                return;

            m_clientSocket.Close();
            m_clientSocket = null;
        }

        Thread m_threadRetryConnect = null;
        Stopwatch m_StopWatch = new Stopwatch();
        /// <summary>
        /// 재연결을 시도하기 위한 Thread를 작성 및 구동한다.
        /// </summary>
        public void OnRetryThreadStart()
        {
            if (Param.RetryConnect == Boolean.FalseString)
                return;

            m_threadRetryConnect = new Thread(new ThreadStart(ProcessRetry));
            m_threadRetryConnect.Name = "Retry Connect";
            m_threadRetryConnect.Start();
        }

        public void OnRetryThreadStop()
        {
            if (m_threadRetryConnect == null) return;

            if (m_threadRetryConnect.IsAlive)
                m_threadRetryConnect.Abort();
        }

        /// <summary>
        /// 재연결 시도 Thread에 들어가는 Core 함수다.
        /// </summary>
        private void ProcessRetry()
        {
            m_StopWatch.Stop();
            m_StopWatch.Reset();
            m_StopWatch.Start();

            OnShowMessageEvent(this, new MessageArgs(eYoonStatus.Info, "Connection Retry Start"));
            int nCount = Convert.ToInt32(Param.RetryCount);
            int nTimeOut = Convert.ToInt32(Param.Timeout);

            for (int iRetry = 0; iRetry < nCount; iRetry++)
            {
                //// Error : Timeout
                if (m_StopWatch.ElapsedMilliseconds >= nTimeOut)
                    break;

                //// Error : IsRetryConnect is false suddenly
                if (!IsRetryOpen)
                    break;

                ////  Success to connect
                if (m_clientSocket != null)
                {
                    if (m_clientSocket.Connected == true)
                    {
                        OnShowMessageEvent(this, new MessageArgs(eYoonStatus.Info, "Connection Retry Success"));
                        IsRetryOpen = false;
                        break;
                    }
                }
                //m_clientSocket.Connect(m_strIP, m_nPort);
                Connect();
            }
            m_StopWatch.Stop();
            m_StopWatch.Reset();

            if (m_clientSocket == null)
            {
                OnShowMessageEvent(this, new MessageArgs(eYoonStatus.Error, "Connection Retry Failure : Connection Socket Empty"));
                return;
            }
            if (m_clientSocket.Connected == false)
                OnShowMessageEvent(this, new MessageArgs(eYoonStatus.Error, "Connection Retry Failure : Connection Fail"));
        }

        /// <summary>
        ///  Buffer 상의 내용을 보낸다.
        /// </summary>
        /// <param name="strBuff">전송할 내용</param>
        public bool Send(string strBuff)
        {
            if (m_clientSocket == null)
                return false;
            if (m_clientSocket.Connected == false)
            {
                Disconnect();
                OnShowMessageEvent(this, new MessageArgs(eYoonStatus.Error, "Send Failure : Connection Fail"));
                IsRetryOpen = true;
                OnRetryThreadStart();
                return false;
            }
            IsSend = false;

            AsyncObject ao = new AsyncObject(1);

            //// 문자열을 Byte 배열(ASCII)로 변환해서 Buffer에 삽입한다.
            ao.Buffer = Encoding.ASCII.GetBytes(strBuff);
            ao.WorkingSocket = m_clientSocket;

            try
            {
                m_clientSocket.BeginSend(ao.Buffer, 0, ao.Buffer.Length, SocketFlags.None, m_sendHandler, ao);
                OnShowMessageEvent(this, new MessageArgs(eYoonStatus.Send, string.Format("Send Message To String : " + strBuff)));
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
                OnShowMessageEvent(this, new MessageArgs(eYoonStatus.Error, "Send Failure : Socket Error"));
            }
            return false;
        }

        /// <summary>
        /// Data의 내용을 보낸다.
        /// </summary>
        /// <param name="data">전송할 내용</param>
        public bool Send(byte[] data)
        {
            if (m_clientSocket == null)
                return false;
            if (m_clientSocket.Connected == false)
            {
                Disconnect();
                OnShowMessageEvent(this, new MessageArgs(eYoonStatus.Error, "Send Failure : Connection Fail"));
                IsRetryOpen = true;
                OnRetryThreadStart();
                return false;
            }
            IsSend = false;

            AsyncObject ao = new AsyncObject(1);

            string strBuff = Encoding.ASCII.GetString(data);
            //// 문자열을 Byte 배열(ASCII)로 변환해서 Buffer에 삽입한다.
            ao.Buffer = Encoding.ASCII.GetBytes(strBuff);
            ao.WorkingSocket = m_clientSocket;

            try
            {
                m_clientSocket.BeginSend(ao.Buffer, 0, ao.Buffer.Length, SocketFlags.None, m_sendHandler, ao);
                //strBuff.Replace("\0", "");
                //strBuff = "[S] " + strBuff;
                OnShowMessageEvent(this, new MessageArgs(eYoonStatus.Send, string.Format("Send Message To String : " + strBuff)));
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
                OnShowMessageEvent(this, new MessageArgs(eYoonStatus.Error, "Send Failure : Socket Error"));
            }
            return false;
        }

        private void OnSendEvent(IAsyncResult ar)
        {
            //// 넘겨진 추가 정보를 가져옵니다.
            AsyncObject ao = (AsyncObject)ar.AsyncState;

            //// 보낸 바이트 수를 저장할 변수 선언
            Int32 sentBytes;

            try
            {
                //// 자료를 전송하고, 전송한 바이트를 가져옵니다.
                sentBytes = ao.WorkingSocket.EndSend(ar);
                if (!ao.WorkingSocket.Connected)
                {
                    //// 예외가 발생하면 예외 정보 출력 후 함수를 종료한다.
                    OnShowMessageEvent(this, new MessageArgs(eYoonStatus.Error, string.Format("Send Failure : Socket Disconnect")));
                    return;
                }
                IsSend = true;
            }
            catch (Exception ex)
            {
                //// 예외가 발생하면 예외 정보 출력 후 함수를 종료한다.
                Console.WriteLine(ex.ToString());
                OnShowMessageEvent(this, new MessageArgs(eYoonStatus.Error, "Send Failure : Socket Error"));
                return;
            }

            if (sentBytes > 0)
            {
                //// 여기도 마찬가지로 보낸 바이트 수 만큼 배열 선언 후 복사한다.
                Byte[] msgByte = new Byte[sentBytes];
                Array.Copy(ao.Buffer, msgByte, sentBytes);

                OnShowMessageEvent(this, new MessageArgs(eYoonStatus.Info, string.Format("Send Success : {0}", Encoding.ASCII.GetString(msgByte))));
            }
        }

        private void OnReceiveEvent(IAsyncResult ar)
        {
            if (m_clientSocket == null) return;

            try
            {
                //// 동기화된 State Object에서 Socket과 State Object를 검색한다.
                AsyncObject ao = (AsyncObject)ar.AsyncState;
                if (!ao.WorkingSocket.Connected)
                {
                    //// 예외가 발생하면 예외 정보 출력 후 함수를 종료한다.
                    OnShowMessageEvent(this, new MessageArgs(eYoonStatus.Error, string.Format("Receive Failure : Socket Disconnect")));
                    return;
                }

                //// Remote Device에서 Data를 읽어온다.
                int bytesRead = ao.WorkingSocket.EndReceive(ar);
                if (bytesRead > 0)
                {
                    ////// 더 많은 Data가 있을 수 있으므로 현재까지의 Data를 저장한다.
                    sbReceiveMessage.Append(Encoding.ASCII.GetString(ao.Buffer, 0, bytesRead));
                    //// 자료 처리가 끝났으면 이제 다시 데이터를 수신받기 위해서 수신 대기를 해야 합니다.
                    //// BeginReceive 메서드를 이용해 비동기적으로 작업을 대기했다면,
                    //// 반드시 대리자 함수에서 EndReceive 메서드를 이용해 비동기 작업이 끝났다고 알려줘야 합니다!
                    ao.WorkingSocket.BeginReceive(ao.Buffer, 0, ao.Buffer.Length, 0, m_receiveHandler, ao);

                    byte[] buffer = new byte[bytesRead];
                    System.Buffer.BlockCopy(ao.Buffer, 0, buffer, 0, buffer.Length);
                    OnShowReceiveDataEvent(this, new BufferArgs(buffer));
                    OnShowMessageEvent(this, new MessageArgs(eYoonStatus.Info, string.Format("Receive Sucess : {0}", Encoding.ASCII.GetString(buffer))));
                    //strRecv = Encoding.ASCII.GetString(state.buffer, 0, bytesRead);
                }
                else // 서버와 연결이 끊겼을 경우
                {
                    //// 모든 Data가 도착했으므로, 응답(CallBack) 한다.
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
