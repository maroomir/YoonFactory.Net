using System;
using System.Text;
namespace YoonFactory.Comm
{
    public class MessageArgs : EventArgs
    {
        public eYoonStatus Status { get; set; }
        public string Message { get; set; }

        public MessageArgs(eYoonStatus nAssort, string message)
        {
            this.Status = nAssort;
            this.Message = message;
        }
    }

    public class BufferArgs : EventArgs
    {
        public eYoonBufferMode Mode { get; private set; }
        public string StringData { get; private set; }
        public byte[] ArrayData { get; private set; }

        public BufferArgs(string data)
        {
            Mode = eYoonBufferMode.String;
            StringData = data;
            ArrayData = Encoding.ASCII.GetBytes(data);
        }

        public BufferArgs(byte[] data)
        {
            Mode = eYoonBufferMode.ByteArray;
            StringData = Encoding.ASCII.GetString(data);
            ArrayData = data;
        }
    }

    public delegate void ShowMessageCallback(object sender, MessageArgs e);
    public delegate void RecieveDataCallback(object sender, BufferArgs e);
}
