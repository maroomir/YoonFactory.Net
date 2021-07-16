using System;
using System.Text;
using System.IO.Ports;

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

        private SerialPort _pSerial = new SerialPort();

        public string Port { get; set; }

        public StringBuilder sbReceiveMessage { get; }
        
        public YoonSerial()
        {
            //
        }

        public void CopyFrom(IYoonComm pComm)
        {
            throw new NotImplementedException();
        }

        public IYoonComm Clone()
        {
            throw new NotImplementedException();
        }

        public bool Send(string strBuffer)
        {
            throw new NotImplementedException();
        }

        public bool Send(byte[] pBuffer)
        {
            throw new NotImplementedException();
        }

        public bool Open()
        {
            // Set-up the parameter of serial communication
            _pSerial ??= new SerialPort();
            _pSerial.PortName = Port;
            _pSerial.BaudRate = 115200;
            _pSerial.DataBits = 8;
            _pSerial.Parity = Parity.None;
            _pSerial.StopBits = StopBits.One;
            _pSerial.ReadTimeout = 100;
            _pSerial.WriteTimeout = 100;
            // Open the port for serial communication
            try
            {
                _pSerial.Open();
            }
            catch
            {
                Console.Write("Port Open Error!");
                return false;
            }

            if (_pSerial.IsOpen)
                Console.Write("Port Open Success : " + _pSerial.PortName);
            else
                Console.Write("Port Open Fail : " + _pSerial.PortName);
            return true;
        }

        /// <summary>
        /// Open the port to use serial
        /// </summary>
        /// <param name="strPortName">Port Name with HEAD (ex. COM1)</param>
        /// <returns></returns>
        public bool Open(string strPortName)
        {
            // Return false if the port name is invalid
            if (strPortName == "") return false;
            int nHeadLength = strPortName.IndexOf("COM", StringComparison.Ordinal);
            if (nHeadLength <= 0)
            {
                Console.Write("Invalid Port Name : " + strPortName);
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
            if (!_pSerial.IsOpen) return;
            _pSerial.Close();
            _pSerial.Dispose();
            _pSerial = null;
        }
        /// <summary>
        /// Write the data in the port
        /// </summary>
        /// <param name="data"></param>
        public void WritePort(string data)
        {
            if (_pSerial.IsOpen == false) return;

            try
            {
                _pSerial.Write(data);
            }
            catch(Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }
        public void WritePort(char[] data, int length)
        {
            if (_pSerial.IsOpen == false) return;

            try
            {
                _pSerial.Write(data, 0, length);
            }
            catch(Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }
        /// <summary>
        /// Port에서 Data를 읽어온다.
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        public string ReadPort(int waitTime)
        {
            if (_pSerial.IsOpen == false) return "";

            int recieveSize = _pSerial.BytesToRead;
            byte[] incommBuffer = new byte[recieveSize];
            string resultData = "";
            _pSerial.ReadTimeout = waitTime;
            try
            {
                if (recieveSize != 0)
                {
                    resultData = "";
                    _pSerial.Read(incommBuffer, 0, recieveSize);
                    for (int i = 0; i < recieveSize; i++)
                    {
                        resultData += Convert.ToChar(incommBuffer[i]);
                    }
                }
            }
            catch(Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
            return resultData;
        }

        /// <summary>
        /// 문자열을 ASCII 문자열로 변환한다.
        /// </summary>
        /// <param name="text"></param>
        /// <returns></returns>
        public string StringToHex(string text)
        {
            string resultHex = "";
            byte[] arrayByteStr = Encoding.ASCII.GetBytes(text);
            foreach (byte byteStr in arrayByteStr)
            {
                resultHex += string.Format("{0:x2}", byteStr);
            }
            return resultHex;
        }
        /// <summary>
        /// 10진수를 ASCII 문자열로 변환한다.
        /// </summary>
        /// <param name="data"></param>
        /// <param name="length"></param>
        /// <returns></returns>
        public byte[] DecToHex(int data, int length)
        {
            return Encoding.ASCII.GetBytes(data.ToString().ToCharArray(), 0, length);
        }
    }
}
