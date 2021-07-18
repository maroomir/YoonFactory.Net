using System;
using System.Text;
using System.IO;
using System.IO.Ports;
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
                _disposedValue = true;
            }
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
            public static string ReadTimeout = "100";
            public static string WriteTimeout = "100";
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
            string strStopBits, string strReadTimeout, string strWriteTimeout)
        {
            Parameter.Port = strPort;
            Parameter.BaudRate = strBaudRate;
            Parameter.DataBits = strDataBits;
            Parameter.Parity = strParity;
            Parameter.StopBits = strStopBits;
            Parameter.ReadTimeout = strReadTimeout;
            Parameter.WriteTimeout = strWriteTimeout;
        }

        public void SetParameter(string strPort, int nBaudRate, int nDataBits, string strParity, string strStopBits,
            int nReadTimeout, int nWriteTimeout)
        {
            Parameter.Port = strPort;
            Parameter.BaudRate = nBaudRate.ToString();
            Parameter.DataBits = nDataBits.ToString();
            Parameter.Parity = strParity;
            Parameter.StopBits = strStopBits;
            Parameter.ReadTimeout = nReadTimeout.ToString();
            Parameter.WriteTimeout = nWriteTimeout.ToString();
        }

        public void LoadParameter()
        {
            string strFilePath = Path.Combine(RootDirectory, "Serial.ini");
            using (YoonIni pIni = new YoonIni(strFilePath))
            {
                pIni.LoadFile();
                Parameter.Port = pIni["Serial"]["Port"].ToString("COM1");
                Parameter.BaudRate = pIni["Serial"]["BaudRate"].ToString("115200");
                Parameter.DataBits = pIni["Serial"]["DataBits"].ToString("8");
                Parameter.Parity = pIni["Serial"]["Parity"].ToString("None");
                Parameter.StopBits = pIni["Serial"]["StopBits"].ToString("One");
                Parameter.ReadTimeout = pIni["Serial"]["ReadTimeout"].ToString("100");
                Parameter.WriteTimeout = pIni["Serial"]["WriteTimeout"].ToString("100");
            }
        }

        public void SaveParameter()
        {
            string strFilePath = Path.Combine(RootDirectory, "Serial.ini");
            using (YoonIni pIni = new YoonIni(strFilePath))
            {
                pIni["Serial"]["Port"] = Parameter.Port;
                pIni["Serial"]["BaudRate"] = Parameter.BaudRate;
                pIni["Serial"]["DataBits"] = Parameter.DataBits;
                pIni["Serial"]["Parity"] = Parameter.Parity;
                pIni["Serial"]["StopBits"] = Parameter.StopBits;
                pIni["Serial"]["ReadTimeout"] = Parameter.ReadTimeout;
                pIni["Serial"]["WriteTimeout"] = Parameter.WriteTimeout;
                pIni.SaveFile();
            }
        }

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

            OnShowMessageEvent?.Invoke(this,
                _pSerial.IsOpen
                    ? new MessageArgs(eYoonStatus.Conform, "Port Open Success : " + _pSerial.PortName)
                    : new MessageArgs(eYoonStatus.Error, "Port Open Fail : " + _pSerial.PortName));
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
                OnShowMessageEvent?.Invoke(this, new MessageArgs(eYoonStatus.Error, "Invalid Port Name : " + strPortName));
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

        public bool Send(byte[] pBuffer)
        {
            throw new NotImplementedException();
        }

        public bool Send(string strBuffer)
        {
            throw new NotImplementedException();
        }

        private bool OnSendEvent(string strBuffer)
        {
            if (!_pSerial.IsOpen) return false;
            IsSend = false;
            try
            {
                _pSerial.Write(strBuffer);
                IsSend = true;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }

            return IsSend;
        }

        private bool OnSendEvent(byte[] pBuffer)
        {
            if (!_pSerial.IsOpen) return false;
            IsSend = false;
            try
            {
                _pSerial.Write(pBuffer, 0, pBuffer.Length);
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }

            return false;
        }

        private string OnReceiveEvent()
        {
            if (_pSerial.IsOpen == false) return "";

            int nReceiveSize = _pSerial.BytesToRead;
            byte[] pBufferIncoming = new byte[nReceiveSize];
            string strReceiveMessage = "";
            try
            {
                if (nReceiveSize != 0)
                {
                    _pSerial.Read(pBufferIncoming, 0, nReceiveSize);
                    for (int i = 0; i < nReceiveSize; i++)
                    {
                        strReceiveMessage += Convert.ToChar(pBufferIncoming[i]);
                    }

                    ReceiveMessage = new StringBuilder(strReceiveMessage);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }

            return strReceiveMessage;
        }
    }
}
