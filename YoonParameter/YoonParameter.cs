using System;
using YoonFactory.Files;
using System.IO;

namespace YoonFactory.Param
{
    public class YoonParameter
    {
        public IYoonParameter Parameter { get; private set; } = null;
        public Type ParameterType { get; private set; } = null;

        public string RootDirectory { get; set; }

        public YoonParameter()
        {
            Parameter = null;
            ParameterType = typeof(object);
            RootDirectory = Path.Combine(Directory.GetCurrentDirectory(), "YoonFactory");
        }

        public YoonParameter(IYoonParameter pParam, Type pType)
        {
            Parameter = pParam.Clone();
            ParameterType = pType;
            RootDirectory = Path.Combine(Directory.GetCurrentDirectory(), "YoonFactory");
        }

        public void SetParameter(IYoonParameter pParam, Type pType)
        {
            if (ParameterType != null && ParameterType != pType) return;
            Parameter = pParam.Clone();
            ParameterType = pType;
        }

        public bool Equals(YoonParameter pParam)
        {
            return pParam.Parameter.Equals(Parameter) && pParam.ParameterType == ParameterType &&
                   pParam.RootDirectory == RootDirectory;
        }

        public bool SaveParameter()
        {
            return SaveParameter(ParameterType.FullName);
        }

        public bool SaveParameter(string strFileName)
        {
            if (RootDirectory == string.Empty || Parameter == null) return false;

            string strFilePath = Path.Combine(RootDirectory, $@"{strFileName}.xml");
            YoonXml pXml = new YoonXml(strFilePath);
            return pXml.SaveFile(Parameter, ParameterType);
        }

        public bool LoadParameter(bool bSaveIfFalse = false)
        {
            return LoadParameter(ParameterType.FullName, bSaveIfFalse);
        }

        public bool LoadParameter(string strFileName, bool bSaveIfFalse = false)
        {
            if (RootDirectory == string.Empty || Parameter == null) return false;

            string strFilePath = Path.Combine(RootDirectory, $@"{strFileName}.xml");
            IYoonParameter pParamBk = Parameter.Clone();
            YoonXml pXml = new YoonXml(strFilePath);
            if (!pXml.LoadFile(out object pParam, ParameterType)) return false;
            Parameter = pParam as IYoonParameter;
            if (Parameter != null)
                return true;
            Parameter = pParamBk;
            if (bSaveIfFalse) SaveParameter();
            return false;
        }
    }
}
