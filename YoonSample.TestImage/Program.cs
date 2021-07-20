using System;
using YoonFactory.Image;
using YoonFactory.Log;

namespace YoonSample.TestImage
{
    class Program
    {
        public static YoonConsoler _pClm = new YoonConsoler();
        public static YoonImage _pImage = null;
        
        static void Main(string[] args)
        {
            Console.Write("Select the processing mode (Align, Drops, Glass)");
            string strSelectionModule = Console.ReadLine();
            switch (strSelectionModule.ToLower())
            {
                case "align":
                    _pClm.Write("Start Align Process");
                    ProcessAlign();
                    break;
                case "drops":
                    _pClm.Write("Start Drops Inspection");
                    break;
                case "glass":
                    _pClm.Write("Start Glass Detector");
                    break;
                default:
                    break;
            }
        }

        static void ProcessAlign()
        {
            _pImage = new YoonImage();
        }
    }
}