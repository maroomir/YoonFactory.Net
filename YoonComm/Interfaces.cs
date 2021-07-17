using System;
using System.Text;

namespace YoonFactory.Comm
{
    public interface IYoonComm : IDisposable
    {
        event ShowMessageCallback OnShowMessageEvent;
        event RecieveDataCallback OnShowReceiveDataEvent;
        
        string RootDirectory { get; set; }
        string Port { get; set; }
        StringBuilder ReceiveMessage { get; }
        
        void CopyFrom(IYoonComm pComm);
        IYoonComm Clone();
        
        bool Open();
        void Close();
        bool Send(string strBuffer);
        bool Send(byte[] pBuffer);
        bool IsSend { get; }
        bool IsConnected { get; }

        void LoadParameter();
        void SaveParameter();
    }

    public interface IYoonTcpIp : IYoonComm
    {
        string Address { get; set; }
        bool IsRetryOpen { get; }
    }
}
