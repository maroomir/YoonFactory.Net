namespace YoonFactory.Log
{
    public interface IYoonLog
    {
        string RootDirectory { get; set; }

        event LogProcessCallback OnProcessLogEvent;
        void Write(string strMessage, bool bSave);
    }

}