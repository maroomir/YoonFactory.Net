using System;
using System.Text;
using System.IO.Ports;
using System.Threading;

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
                    Thread.Sleep(100);
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

        public StringBuilder ReceiveMessage { get; private set; }
        
        public YoonSerial()
        {
            //
        }

        public YoonSerial(string strPort)
        {
            Port = strPort;
        }
        
        public void CopyFrom(IYoonComm pComm)
        {
            if (pComm is not YoonSerial) return;
            Port = pComm.Port;
        }

        public IYoonComm Clone()
        {
            Close();
            return new YoonSerial(Port);
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


        public bool Send(string strBuffer)
        {
            if (!_pSerial.IsOpen) return false;

            try
            {
                _pSerial.Write(strBuffer);
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }

            return false;
        }

        public bool Send(byte[] pBuffer)
        {
            if (!_pSerial.IsOpen) return false;

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

        public string Receive(int nWaitTime)
        {
            if (_pSerial.IsOpen == false) return "";

            int nReceiveSize = _pSerial.BytesToRead;
            byte[] pBufferIncoming = new byte[nReceiveSize];
            _pSerial.ReadTimeout = nWaitTime;
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
