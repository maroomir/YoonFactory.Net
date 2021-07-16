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
        private bool disposedValue = false; // 중복 호출을 검색하려면

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                // Save Last setting
                SaveParam();
                SaveTarget();

                if (disposing)
                {
                    // TODO: 관리되는 상태(관리되는 개체)를 삭제합니다.
                    if (m_serverSocket != null)
                    {
                        m_serverSocket.Close();
                        m_serverSocket.Dispose();
                        m_serverSocket = null;
                    }
                    if (m_connectedClientSocket != null)
                    {
                        m_connectedClientSocket.Disconnect(false);
                        m_connectedClientSocket.Dispose();
                        m_connectedClientSocket = null;
                    }
                }

                // TODO: 관리되지 않는 리소스(관리되지 않는 개체)를 해제하고 아래의 종료자를 재정의합니다.
                // TODO: 큰 필드를 null로 설정합니다.

                // Close RetryConnect Thread
                OnRetryThreadStop();
                // Refund memory to main function
                m_threadRetryListen = null;

                m_acceptHandler = null;
                m_receiveHandler = null;
                m_sendHandler = null;

                disposedValue = true;
            }
        }

        // TODO: 위의 Dispose(bool disposing)에 관리되지 않는 리소스를 해제하는 코드가 포함되어 있는 경우에만 종료자를 재정의합니다.
        public YoonServer()
        {
            // 관리 가능한 변수를 초기화 합니다.
            sbReceiveMessage = new StringBuilder(string.Empty);

            // 비동기 작업에 사용될 대리자를 초기화합니다.
            m_acceptHandler = new AsyncCallback(OnAcceptClientEvent);
            m_receiveHandler = new AsyncCallback(OnReceiveEvent);
            m_sendHandler = new AsyncCallback(OnSendEvent);
        }

        public YoonServer(string strParamDirectory)
        {
            RootDirectory = strParamDirectory;
            LoadParam();
            LoadTarget();

            // 관리 가능한 변수를 초기화 합니다.
            sbReceiveMessage = new StringBuilder(string.Empty);

            // 비동기 작업에 사용될 대리자를 초기화합니다.
            m_acceptHandler = new AsyncCallback(OnAcceptClientEvent);
            m_receiveHandler = new AsyncCallback(OnReceiveEvent);
            m_sendHandler = new AsyncCallback(OnSendEvent);
        }

        ~YoonServer()
        {
            Dispose(false);
        }

        // 삭제 가능한 패턴을 올바르게 구현하기 위해 추가된 코드입니다.
        public void Dispose()
        {
            // 이 코드를 변경하지 마세요. 위의 Dispose(bool disposing)에 정리 코드를 입력하세요.
            Dispose(true);
            // TODO: 위의 종료자가 재정의된 경우 다음 코드 줄의 주석 처리를 제거합니다.
            GC.SuppressFinalize(this);
        }
        #endregion

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
        public string Address { get; set; } = string.Empty;
        public string RootDirectory { get; set; } = Path.Combine(Directory.GetCurrentDirectory(), "YoonFactory");
        public string Port
        {
            get => Param.Port;
            set
            {
                if (TCPFactory.VerifyPort(value))
                    Param.Port = value;
            }
        }

        public void CopyFrom(IYoonComm pComm)
        {
            if (pComm is YoonServer pServer)
            {
                Close();
                if (pServer.IsConnected)
                    pServer.Close();

                LoadParam();
                Port = pServer.Port;
            }
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

        private Socket m_serverSocket = null;
        private Socket m_connectedClientSocket = null;
        private AsyncCallback m_acceptHandler;
        private AsyncCallback m_receiveHandler;
        private AsyncCallback m_sendHandler;
        private List<string> m_listClientIP = null;

        private struct Param
        {
            public static string Port = "1234";
            public static string Backlog = "5";
            public static string RetryCount = "10";
            public static string RetryListen = "true";
            public static string Timeout = "10000";
        }

        /// <summary>
        /// Parameter를 설정한다.
        /// </summary>
        /// <param name="port">Port Num  (ex. 5000)</param>
        /// <param name="backlog"> Backlog (ex. 5)</param>
        /// <param name="retryListen">재시도 여부  (ex.  true)</param>
        /// <param name="timeout">Time Out 예상 시간  (ex. 5000)</param>
        /// <param name="retryCount">재시도 횟수  (ex. 1)</param>
        public void SetParam(string port, string backlog, string retryListen, string timeout, string retryCount)
        {
            Param.Port = port;
            Param.Backlog = backlog;
            Param.RetryCount = retryCount;
            Param.RetryListen = retryListen;
            Param.Timeout = timeout;
        }

        public void SetParam(int port, int backlog, bool IsRetryListen, int timeout, int retryCount)
        {
            Param.Port = port.ToString();
            Param.Backlog = backlog.ToString();
            Param.RetryListen = IsRetryListen.ToString();
            Param.Timeout = timeout.ToString();
            Param.RetryCount = retryCount.ToString();
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
                    m_listClientIP = (List<string>)pTargetParam;
                else if (m_listClientIP == null)
                {
                    m_listClientIP = new List<string>();
                }
                m_listClientIP.Clear();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }
        }

        public void SaveTarget()
        {
            if (m_listClientIP == null)
            {
                m_listClientIP = new List<string>();
                m_listClientIP.Clear();
            }
            try
            {
                string strTargetPath = Path.Combine(RootDirectory, "IPTarget.xml");
                YoonXml xc = new YoonXml(strTargetPath);
                xc.SaveFile(m_listClientIP, typeof(List<string>));
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }
        }

        public bool IsBound
        {
            get
            {
                if (m_serverSocket == null) return false;
                return m_serverSocket.IsBound;
            }
        }

        public bool IsConnected
        {
            get
            {
                if (m_connectedClientSocket == null) return false;
                return m_connectedClientSocket.Connected;
            }
        }

        public bool Open()
        {
            return Listen();
        }

        public bool Listen()
        {
            if (m_serverSocket != null)
            {
                if (m_serverSocket.IsBound == true)
                    return true;
            }

            try
            {
                m_serverSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.IP);
                if (!IsRetryOpen)
                    OnShowMessageEvent(this, new MessageArgs(eYoonStatus.Info, string.Format("Listen Port : {0}", Param.Port)));
                //// Binding port and Listening per backlogging
                m_serverSocket.Bind(new IPEndPoint(IPAddress.Any, int.Parse(Param.Port)));
                m_serverSocket.Listen(int.Parse(Param.Backlog));
                //// Associate the connection request
                IAsyncResult asyncResult = m_serverSocket.BeginAccept(m_acceptHandler, null);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());

                if (!IsRetryOpen)
                    OnShowMessageEvent(this, new MessageArgs(eYoonStatus.Error, string.Format("Listening Failure : Socket Error")));
                if (m_serverSocket != null)
                {
                    m_serverSocket.Close();
                    m_serverSocket = null;
                }
                return false;
            }

            if (m_serverSocket.IsBound == true)
            {
                OnShowMessageEvent(this, new MessageArgs(eYoonStatus.Info, string.Format("Listen Success")));
                SaveParam();
                IsRetryOpen = false;
            }
            else
            {
                if (!IsRetryOpen)
                    OnShowMessageEvent(this, new MessageArgs(eYoonStatus.Error, string.Format("Listen Failure : Bound Fail")));
                IsRetryOpen = true;
            }
            return m_serverSocket.IsBound;
        }

        public bool Listen(string strPort)
        {
            if (m_serverSocket != null)
            {
                if (m_serverSocket.IsBound == true)
                    return true;
            }

            try
            {
                m_serverSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.IP);

                Param.Port = strPort;

                if (!IsRetryOpen)
                    OnShowMessageEvent(this, new MessageArgs(eYoonStatus.Info, string.Format("Listen Port : {0}", Param.Port)));
                //// Binding Port and Listening per backlogging
                m_serverSocket.Bind(new IPEndPoint(IPAddress.Any, int.Parse(Param.Port)));
                m_serverSocket.Listen(int.Parse(Param.Backlog));
                //// Associate the connection request
                IAsyncResult asyncResult = m_serverSocket.BeginAccept(m_acceptHandler, null);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());

                if (!IsRetryOpen)
                    OnShowMessageEvent(this, new MessageArgs(eYoonStatus.Error, string.Format("Listening Failure : Socket Error")));
                if (m_serverSocket != null)
                {
                    m_serverSocket.Close();
                    m_serverSocket = null;
                }
                return false;
            }

            if (m_serverSocket.IsBound == true)
            {
                IsRetryOpen = false;
                OnShowMessageEvent(this, new MessageArgs(eYoonStatus.Info, string.Format("Listen Success")));
                SaveParam();
            }
            else
            {
                if (!IsRetryOpen)
                    OnShowMessageEvent(this, new MessageArgs(eYoonStatus.Error, string.Format("Listen Failure : Bound Fail")));
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

            if (m_serverSocket == null)
                return;

            m_serverSocket.Close();
            m_serverSocket = null;
        }

        public void OnAcceptClientEvent(IAsyncResult ar)
        {
            if (m_serverSocket == null || !m_serverSocket.IsBound) return;

            Socket clientSocket;
            try
            {
                //// 클라이언트의 연결 요청을 수락합니다.
                clientSocket = m_serverSocket.EndAccept(ar);
                //// Get Client IP Address and Save to IPTarget.xml
                string strAddressFull = clientSocket.RemoteEndPoint.ToString();
                bool bDuplicatedAddress = false;
                foreach (string strBuff in m_listClientIP)
                {
                    if (strBuff == strAddressFull)
                        bDuplicatedAddress = true;
                }
                if (!bDuplicatedAddress)
                    m_listClientIP.Add(strAddressFull);
                OnShowMessageEvent(this, new MessageArgs(eYoonStatus.Info, string.Format("Acception Success To Client : {0}", strAddressFull)));
                SaveTarget();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());

                OnShowMessageEvent(this, new MessageArgs(eYoonStatus.Error, string.Format("Acceptation Failure")));
                return;
            }

            //// 4096의 크기를 갖는 Byte 배열을 가진 AsyncObject Class를 생성한다.
            AsyncObject ao = new AsyncObject(BUFFER_SIZE);
            //// 클라이언트 소켓 저장
            m_connectedClientSocket = clientSocket;
            //// 작업 중인 소켓을 저장하기 위해 SocketClient를 할당한다.
            ao.WorkingSocket = clientSocket;
            try
            {
                //// 비동기적으로 들어오는 자료를 수신하기 위해 BeginReceive 메서드를 사용한다.
                clientSocket.BeginReceive(ao.Buffer, 0, ao.Buffer.Length, SocketFlags.None, m_receiveHandler, ao);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());

                OnShowMessageEvent(this, new MessageArgs(eYoonStatus.Error, string.Format("Receive Waiting Failure : Socket Error")));
                return;
            }
        }

        Thread m_threadRetryListen = null;
        Stopwatch m_StopWatch = new Stopwatch();
        /// <summary>
        /// 재연결을 시도하기 위한 Thread를 작성 및 구동한다.
        /// </summary>
        public void OnRetryThreadStart()
        {
            if (Param.RetryListen == Boolean.FalseString)
                return;
            m_threadRetryListen = new Thread(new ThreadStart(ProcessRetry));
            m_threadRetryListen.Name = "Retry Listen";
            m_threadRetryListen.Start();
        }

        public void OnRetryThreadStop()
        {
            if (m_threadRetryListen == null) return;

            if (m_threadRetryListen.IsAlive)
                m_threadRetryListen.Abort();
        }

        /// <summary>
        /// 재연결 시도 Thread에 들어가는 Core 함수다.
        /// </summary>
        private void ProcessRetry()
        {
            m_StopWatch.Stop();
            m_StopWatch.Reset();
            m_StopWatch.Start();

            OnShowMessageEvent(this, new MessageArgs(eYoonStatus.Info, string.Format("Listen Retry Start")));
            int nCount = Convert.ToInt32(Param.RetryCount);
            int nTimeOut = Convert.ToInt32(Param.Timeout);

            for (int iRetry = 0; iRetry < nCount; iRetry++)
            {
                //// Error : Timeout
                if (m_StopWatch.ElapsedMilliseconds >= nTimeOut)
                    break;

                //// Error : Retry Listen is false suddenly
                if (!IsRetryOpen)
                    break;

                ////  Success to connect
                if (m_serverSocket != null)
                {
                    if (m_serverSocket.IsBound == true)
                    {
                        OnShowMessageEvent(this, new MessageArgs(eYoonStatus.Info, string.Format("Listen Retry Success")));
                        IsRetryOpen = false;
                        break;
                    }
                }
                Listen();
            }
            m_StopWatch.Stop();
            m_StopWatch.Reset();

            if (m_serverSocket == null)
            {
                OnShowMessageEvent(this, new MessageArgs(eYoonStatus.Error, "Listen Retry Failure : Listen Socket Empty"));
                return;
            }
            if (m_serverSocket.IsBound == false)
            {
                OnShowMessageEvent(this, new MessageArgs(eYoonStatus.Error, string.Format("Listen Retry Failure : Connection Fail")));
            }
        }

        /// <summary>
        ///  Buffer 상의 내용을 보낸다.
        /// </summary>
        /// <param name="strBuffer">전송할 내용</param>
        public bool Send(string strBuffer)
        {
            if (m_serverSocket == null || m_connectedClientSocket == null)
                return false;
            if (m_connectedClientSocket.Connected == false)
            {
                OnShowMessageEvent(this, new MessageArgs(eYoonStatus.Error, "Send Failure : Connection Fail"));
                return false;
            }
            IsSend = false;
            // 추가 정보를 넘기기 위한 변수 선언
            // 크기를 설정하는게 의미가 없습니다.
            // 왜냐하면 바로 밑의 코드에서 문자열을 유니코드 형으로 변환한 바이트 배열을 반환하기 때문에
            // 최소한의 크기르 배열을 초기화합니다.
            AsyncObject ao = new AsyncObject(1);

            //// 문자열을 Byte 배열(ASCII)로 변환해서 Buffer에 삽입한다.
            ao.Buffer = Encoding.ASCII.GetBytes(strBuffer);
            ao.WorkingSocket = m_connectedClientSocket;

            try
            {
                m_connectedClientSocket.BeginSend(ao.Buffer, 0, ao.Buffer.Length, SocketFlags.None, m_sendHandler, ao);
                OnShowMessageEvent(this, new MessageArgs(eYoonStatus.Info, string.Format("Send Message To String : " + strBuffer)));
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
                OnShowMessageEvent(this, new MessageArgs(eYoonStatus.Error, "Send Failure : Client Socket Error"));
            }
            return false;
        }

        /// <summary>
        /// Data의 내용을 보낸다.
        /// </summary>
        /// <param name="pBuffer">전송할 내용</param>
        public bool Send(byte[] pBuffer)
        {
            if (m_serverSocket == null || m_connectedClientSocket == null)
                return false;
            if (m_connectedClientSocket.Connected == false)
            {
                OnShowMessageEvent(this, new MessageArgs(eYoonStatus.Error, "Send Failure : Connection Fail"));
                return false;
            }
            IsSend = false;

            AsyncObject ao = new AsyncObject(1);

            string strBuff = Encoding.ASCII.GetString(pBuffer);
            //// 문자열을 Byte 배열(ASCII)로 변환해서 Buffer에 삽입한다.
            ao.Buffer = Encoding.ASCII.GetBytes(strBuff);
            ao.WorkingSocket = m_connectedClientSocket;

            try
            {
                m_connectedClientSocket.BeginSend(ao.Buffer, 0, ao.Buffer.Length, SocketFlags.None, m_sendHandler, ao);
                //strBuff.Replace("\0", "");
                //strBuff = "[S] " + strBuff;
                OnShowMessageEvent(this, new MessageArgs(eYoonStatus.Info, string.Format("Send Message To String : " + strBuff)));
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
            if (m_serverSocket == null || m_connectedClientSocket == null) return;

            //// 넘겨진 추가 정보를 가져옵니다.
            AsyncObject ao = (AsyncObject)ar.AsyncState;
            if (!ao.WorkingSocket.Connected)
            {
                //// 예외가 발생하면 예외 정보 출력 후 함수를 종료한다.
                OnShowMessageEvent(this, new MessageArgs(eYoonStatus.Error, string.Format("Send Failure : Socket Disconnect")));
                return;
            }

            //// 보낸 바이트 수를 저장할 변수 선언
            Int32 sentBytes;

            try
            {
                //// 자료를 전송하고, 전송한 바이트를 가져옵니다.
                sentBytes = ao.WorkingSocket.EndSend(ar);
                IsSend = true;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());

                //// 예외가 발생하면 예외 정보 출력 후 함수를 종료한다.
                OnShowMessageEvent(this, new MessageArgs(eYoonStatus.Error, string.Format("Send Failure : Socket Error")));
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
            if (m_serverSocket == null || m_connectedClientSocket == null) return;

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
