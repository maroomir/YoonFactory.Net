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
        public string StringData { get; set; }

        public byte[] ArrayData { get; set; }

        public BufferArgs(string data)
        {
            this.StringData = data;
            this.ArrayData = Encoding.ASCII.GetBytes(data);
        }

        public BufferArgs(byte[] data)
        {
            this.StringData = Encoding.ASCII.GetString(data);
            this.ArrayData = data;
        }
    }

    public delegate void ShowMessageCallback(object sender, MessageArgs e);
    public delegate void RecieveDataCallback(object sender, BufferArgs e);
}
