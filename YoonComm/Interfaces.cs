using System;
using System.Text;

namespace YoonFactory.Comm
{
    public interface IYoonComm : IDisposable
    {
        string Port { get; set; }
        StringBuilder sbReceiveMessage { get; }
        
        void CopyFrom(IYoonComm pComm);
        IYoonComm Clone();
        
        bool Open();
        void Close();
        bool Send(string strBuffer);
        bool Send(byte[] pBuffer);
    }

    public interface IYoonTcpIp : IYoonComm
    {
        event ShowMessageCallback OnShowMessageEvent;
        event RecieveDataCallback OnShowReceiveDataEvent;

        string RootDirectory { get; set; }
        string Address { get; set; }
        bool IsRetryOpen { get; }
        bool IsSend { get; }
        bool IsConnected { get; }

        void LoadParam();
        void SaveParam();
    }
}
