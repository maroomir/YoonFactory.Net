using System;

namespace YoonFactory.Files
{
    public interface IYoonFile : IDisposable
    {
        string FilePath { get; }

        void CopyFrom(IYoonFile pFile);
        IYoonFile Clone();
        bool IsFileExist();
        bool LoadFile();
        bool SaveFile();
    }
}
