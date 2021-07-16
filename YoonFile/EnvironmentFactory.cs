using System;
using System.IO;

namespace YoonFactory.Files
{
    /* 환경 파일을 관리하는 Class */
    public static class EnvironmentFactory
    {
        public enum eTypeEnvironment
        {
            Windows,
            MacOS,
            Ubuntu,
        }

        public static string GetHomeFolder(eTypeEnvironment fOS)
        {
            switch (fOS)
            {
                case eTypeEnvironment.Windows:
                    return Path.Combine(Environment.GetEnvironmentVariable("HOMEPATH"));
                case eTypeEnvironment.MacOS:
                    return Path.Combine(Environment.GetEnvironmentVariable("HOME"));
                case eTypeEnvironment.Ubuntu:
                    return Path.Combine(Environment.GetEnvironmentVariable("HOME"));
                default:
                    break;
            }
            return string.Empty;
        }

        public static string GetRootFolder(eTypeEnvironment fOS)
        {
            switch (fOS)
            {
                case eTypeEnvironment.Windows:
                    return Path.Combine(Environment.GetEnvironmentVariable("SystemRoot"));
                case eTypeEnvironment.MacOS:
                    return Path.Combine("/Library");
                case eTypeEnvironment.Ubuntu:
                    return Path.Combine("usr", "share");
                default:
                    break;
            }
            return string.Empty;
        }

        public static string GetFontFolder(eTypeEnvironment fOS)
        {
            switch (fOS)
            {
                case eTypeEnvironment.Windows:
                    return Path.Combine(GetRootFolder(fOS), "Fonts");
                case eTypeEnvironment.MacOS:
                    return Path.Combine(GetRootFolder(fOS), "Fonts");
                case eTypeEnvironment.Ubuntu:
                    return Path.Combine(GetRootFolder(fOS), "Fonts");
            }
            return string.Empty;
        }
    }

}
