using System;
using System.Text;

namespace YoonFactory.Comm
{
    public interface IYoonComm : IDisposable
    {
        event ShowMessageCallback OnShowMessageEvent;
        event RecieveDataCallback OnShowReceiveDataEvent;
        
        string RootDirectory { get; set; }
        string Address { get; set; }
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
        bool IsRetryOpen { get; }

        void LoadParameter();
        void SaveParameter();
        void OnRetryThreadStart();
        void OnRetryThreadStop();
    }
}
