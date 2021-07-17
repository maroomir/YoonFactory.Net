using System;
using System.Text;

namespace YoonFactory.Comm
{
    public interface IYoonComm : IDisposable
    {
        event ShowMessageCallback OnShowMessageEvent;
        event RecieveDataCallback OnShowReceiveDataEvent;
        
        string Port { get; set; }
        StringBuilder ReceiveMessage { get; }
        
        void CopyFrom(IYoonComm pComm);
        IYoonComm Clone();
        
        bool Open();
        void Close();
        bool Send(string strBuffer);
        bool Send(byte[] pBuffer);
    }

    public interface IYoonTcpIp : IYoonComm
    {
        string RootDirectory { get; set; }
        string Address { get; set; }
        bool IsRetryOpen { get; }
        bool IsSend { get; }
        bool IsConnected { get; }

        void LoadParameter();
        void SaveParameter();
    }
}
