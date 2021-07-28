using System;
using System.Diagnostics;
using System.Text;
using System.IO;
using System.IO.Ports;
using System.Threading;
using YoonFactory.Files;

namespace YoonFactory.Comm.Serial
{
    public class YoonSerial : IYoonComm
    {

        #region IDisposable Support

        private bool _disposedValue = false;

        protected virtual void Dispose(bool disposing)
        {
            if (!_disposedValue)
            {
                if (disposing)
                {
                    Close();
                    _pSerial.Dispose();
                }

                OnRetryThreadStop();
                if (_pThreadReceive != null)
                {
                    _pThreadReceive.Interrupt();
                    Thread.Sleep(1000);
                    _pThreadReceive = null;
                }

                _disposedValue = true;
            }
        }

        ~YoonSerial()
        {
            Dispose(false);
        }

        public void Dispose()
        {
            Dispose(true);
        }

        #endregion

        public YoonSerial()
        {
            // Initialize message parameter
            ReceiveMessage = new StringBuilder(string.Empty);
        }

        public YoonSerial(string strParamDirectory)
        {
            // Initialize message parameter
            ReceiveMessage = new StringBuilder(string.Empty);

            RootDirectory = strParamDirectory;
            LoadParameter();
        }

        public event ShowMessageCallback OnShowMessageEvent;
        public event RecieveDataCallback OnShowReceiveDataEvent;
        public bool IsSend { get; private set; } = false;
        public bool IsRetryOpen { get; private set; } = false;
        public StringBuilder ReceiveMessage { get; private set; }
        public string RootDirectory { get; set; } = Path.Combine(Directory.GetCurrentDirectory(), "YoonFactory");
        public string Address { get; set; } = string.Empty;

        public string Port
        {
            get => Parameter.Port;
            set
            {
                if (CommunicationFactory.VerifySerialPort(value))
                    Parameter.Port = value;
            }
        }

        public bool IsConnected => _pSerial is {IsOpen: true};

        private SerialPort _pSerial = new SerialPort();

        private struct Parameter
        {
            public static string Port = "COM1";
            public static string BaudRate = "115200";
            public static string DataBits = "8";
            public static string Parity = "None";
            public static string StopBits = "One";
            public static string RetryOpen = "true";
            public static string RetryCount = "10";
            public static string ReadTimeout = "100";
            public static string WriteTimeout = "100";
            public static string RetryTimeout = "10000";
        }

        public void CopyFrom(IYoonComm pComm)
        {
            if (pComm is not YoonSerial pSerial) return;
            Close();
            if (pSerial.IsConnected)
                pSerial.Close();

            RootDirectory = pSerial.RootDirectory;
            LoadParameter();
            Port = pComm.Port;
        }

        public IYoonComm Clone()
        {
            Close();

            YoonSerial pSerial = new YoonSerial();
            pSerial.RootDirectory = RootDirectory;
            pSerial.LoadParameter();
            pSerial.Port = Port;
            return pSerial;
        }

        public void SetParameter(string strPort, string strBaudRate, string strDataBits, string strParity,
            string strStopBits, string strRetryOpen, string strRetryCount, string strReadTimeout,
            string strWriteTimeout, string strRetryTimeout)
        {
            Parameter.Port = strPort;
            Parameter.BaudRate = strBaudRate;
            Parameter.DataBits = strDataBits;
            Parameter.Parity = strParity;
            Parameter.StopBits = strStopBits;
            Parameter.RetryOpen = strRetryOpen;
            Parameter.RetryCount = strRetryCount;
            Parameter.ReadTimeout = strReadTimeout;
            Parameter.WriteTimeout = strWriteTimeout;
            Parameter.RetryTimeout = strRetryTimeout;
        }

        public void SetParameter(string strPort, int nBaudRate, int nDataBits, string strParity, string strStopBits,
            bool bRetryConnect, int nRetryCount, int nReadTimeout, int nWriteTimeout, int nRetryTimeout)
        {
            Parameter.Port = strPort;
            Parameter.BaudRate = nBaudRate.ToString();
            Parameter.DataBits = nDataBits.ToString();
            Parameter.Parity = strParity;
            Parameter.StopBits = strStopBits;
            Parameter.RetryOpen = bRetryConnect.ToString();
            Parameter.RetryCount = nRetryCount.ToString();
            Parameter.ReadTimeout = nReadTimeout.ToString();
            Parameter.WriteTimeout = nWriteTimeout.ToString();
            Parameter.RetryTimeout = nRetryTimeout.ToString();
        }

        public void LoadParameter()
        {
            string strFilePath = Path.Combine(RootDirectory, "Serial.ini");
            YoonIni pIni = new YoonIni(strFilePath);
            pIni.LoadFile();
            Parameter.Port = pIni["Serial"]["Port"].ToString("COM1");
            Parameter.BaudRate = pIni["Serial"]["BaudRate"].ToString("115200");
            Parameter.DataBits = pIni["Serial"]["DataBits"].ToString("8");
            Parameter.Parity = pIni["Serial"]["Parity"].ToString("None");
            Parameter.StopBits = pIni["Serial"]["StopBits"].ToString("One");
            Parameter.RetryOpen = pIni["Serial"]["RetryOpen"].ToString("true");
            Parameter.RetryCount = pIni["Serial"]["RetryCount"].ToString("100");
            Parameter.ReadTimeout = pIni["Serial"]["ReadTimeout"].ToString("100");
            Parameter.WriteTimeout = pIni["Serial"]["WriteTimeout"].ToString("100");
            Parameter.RetryTimeout = pIni["Serial"]["RetryTimeout"].ToString("10000");
            pIni.Dispose();
        }

        public void SaveParameter()
        {
            string strFilePath = Path.Combine(RootDirectory, "Serial.ini");
            YoonIni pIni = new YoonIni(strFilePath);
            pIni["Serial"]["Port"] = Parameter.Port;
            pIni["Serial"]["BaudRate"] = Parameter.BaudRate;
            pIni["Serial"]["DataBits"] = Parameter.DataBits;
            pIni["Serial"]["Parity"] = Parameter.Parity;
            pIni["Serial"]["StopBits"] = Parameter.StopBits;
            pIni["Serial"]["RetryOpen"] = Parameter.RetryOpen;
            pIni["Serial"]["RetryCount"] = Parameter.RetryCount;
            pIni["Serial"]["ReadTimeout"] = Parameter.ReadTimeout;
            pIni["Serial"]["WriteTimeout"] = Parameter.WriteTimeout;
            pIni["Serial"]["RetryTimeout"] = Parameter.RetryTimeout;
            pIni.SaveFile();
            pIni.Dispose();
        }

        private Thread _pThreadReceive = null;

        public bool Open()
        {
            try
            {
                _pSerial ??= new SerialPort();
                // Set-up the parameter of serial communication
                _pSerial.PortName = Parameter.Port;
                _pSerial.BaudRate = Convert.ToInt32(Parameter.BaudRate);
                _pSerial.DataBits = Convert.ToInt32(Parameter.DataBits);
                _pSerial.Parity = (Parity) Enum.Parse(typeof(Parity), Parameter.Parity);
                _pSerial.StopBits = (StopBits) Enum.Parse(typeof(Parity), Parameter.StopBits);
                _pSerial.ReadTimeout = Convert.ToInt32(Parameter.ReadTimeout);
                _pSerial.WriteTimeout = Convert.ToInt32(Parameter.WriteTimeout);
                // Open the port for serial communication
                _pSerial.Open();
            }
            catch
            {
                OnShowMessageEvent?.Invoke(this, new MessageArgs(eYoonStatus.Error, "Port Open Error!"));
                return false;
            }

            if (_pSerial.IsOpen)
            {
                OnShowMessageEvent?.Invoke(this,
                    new MessageArgs(eYoonStatus.Conform, "Port Open Success : " + _pSerial.PortName));
                _pThreadReceive = new Thread(ProcessReceive);
                _pThreadReceive.Start();
            }
            else
            {
                OnShowMessageEvent?.Invoke(this,
                    new MessageArgs(eYoonStatus.Error, "Port Open Fail : " + _pSerial.PortName));
            }

            return _pSerial.IsOpen;
        }

        /// <summary>
        /// Open the port to use serial
        /// </summary>
        /// <param name="strPortName">Port Name with HEAD (ex. COM1)</param>
        /// <returns></returns>
        public bool Open(string strPortName)
        {
            // Return false if the port name is invalid
            if (!CommunicationFactory.VerifySerialPort(strPortName))
            {
                OnShowMessageEvent?.Invoke(this,
                    new MessageArgs(eYoonStatus.Error, "Invalid Port Name : " + strPortName));
                return false;
            }

            Port = strPortName;
            return Open();
        }

        /// <summary>
        /// Close the serial communication
        /// </summary>
        public void Close()
        {
            if (_pSerial == null) return;
            _pSerial.Close();
            _pSerial = null;
        }

        private Thread _pThreadRetry = null;
        private readonly Stopwatch _pStopWatch = new Stopwatch();

        public void OnRetryThreadStart()
        {
            if (Parameter.RetryOpen == bool.FalseString)
                return;

            _pThreadRetry = new Thread(ProcessRetry);
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

            OnShowMessageEvent?.Invoke(this, new MessageArgs(eYoonStatus.Info, string.Format("Open Retry Start")));
            int nCount = Convert.ToInt32(Parameter.RetryCount);
            int nTimeOut = Convert.ToInt32(Parameter.RetryTimeout);

            for (int iRetry = 0; iRetry < nCount; iRetry++)
            {
                // Error : Timeout
                if (_pStopWatch.ElapsedMilliseconds >= nTimeOut)
                    break;

                // Error : Retry Open is false suddenly
                if (!IsRetryOpen)
                    break;

                // success to open
                if (_pSerial is {IsOpen: true})
                {
                    OnShowMessageEvent?.Invoke(this, new MessageArgs(eYoonStatus.Info, "Open Retry Success"));
                    IsRetryOpen = false;
                    break;
                }

                Open();
            }

            _pStopWatch.Stop();
            _pStopWatch.Reset();

            if (_pSerial == null)
            {
                OnShowMessageEvent?.Invoke(this,
                    new MessageArgs(eYoonStatus.Error, "Open Retry Failure : Abnormal Serial"));
                return;
            }

            if (_pSerial.IsOpen == false)
            {
                OnShowMessageEvent?.Invoke(this,
                    new MessageArgs(eYoonStatus.Error, "Open Retry Failure : Connection Fail"));
            }
        }

        public bool Send(byte[] pBuffer)
        {
            if (_pThreadReceive is not {IsAlive: true})
                return false;
            _bPauseReceive = true;
            Thread.Sleep(1000);
            IsSend = OnSendEvent(this, new BufferArgs(pBuffer));
            _bPauseReceive = false;
            return IsSend;
        }

        public bool Send(string strBuffer)
        {
            if (_pThreadReceive is not {IsAlive: true})
                return false;

            _bPauseReceive = true;
            Thread.Sleep(1000);
            IsSend = OnSendEvent(this, new BufferArgs(strBuffer));
            _bPauseReceive = false;
            return IsSend;
        }

        private bool OnSendEvent(object sender, BufferArgs e)
        {
            if (_pSerial is not {IsOpen: true}) return false;
            try
            {
                switch (e.Mode)
                {
                    case eYoonBufferMode.String:
                        _pSerial.Write(e.StringData);
                        break;
                    case eYoonBufferMode.ByteArray:
                        _pSerial.Write(e.ArrayData, 0, e.ArrayData.Length);
                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }

                return true;
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.ToString());
                OnShowMessageEvent?.Invoke(this, new MessageArgs(eYoonStatus.Error, "Send Message Fail"));
            }

            return false;
        }

        private bool _bPauseReceive = false;

        private void ProcessReceive()
        {
            while (true)
            {
                if (_pSerial is not {IsOpen: true}) return;

                if (_bPauseReceive) continue;

                if (_pSerial.BytesToRead == 0) continue;
                byte[] pBufferIncoming = new byte[_pSerial.BytesToRead];
                string strReceiveMessage = "";
                try
                {
                    _pSerial.Read(pBufferIncoming, 0, _pSerial.BytesToRead);
                    for (int i = 0; i < _pSerial.BytesToRead; i++)
                    {
                        strReceiveMessage += Convert.ToChar(pBufferIncoming[i]);
                    }

                    ReceiveMessage.Append(strReceiveMessage);
                    OnShowReceiveDataEvent?.Invoke(this, new BufferArgs(strReceiveMessage));
                }
                catch (Exception ex)
                {
                    Debug.WriteLine(ex.ToString());
                }
            }
        }
    }
}
