using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using YoonFactory;
using YoonFactory.Files;
using YoonFactory.Image;
using YoonFactory.Log;

namespace YoonSample.TestImage
{
    class Program
    {
        public static string _strRootDir = Path.Combine(Directory.GetCurrentDirectory(), @"../../../../Sample/Image"); 
        public static YoonConsoler _pClm = new YoonConsoler();
        
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
                    ProcessDrop();
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
            // Parsing
            _strRootDir = Path.Combine(_strRootDir, @"Align");
            Dictionary<string, YoonImage> pDicLeftImage = new Dictionary<string, YoonImage>();
            Dictionary<string, YoonImage> pDicRightImage = new Dictionary<string, YoonImage>();
            List<string> pListFilePath = FileFactory.GetExtensionFilePaths(_strRootDir, ".bmp");
            foreach (string strFilePath in pListFilePath)
            {
                FileInfo pFile = new FileInfo(strFilePath);
                string strKey = pFile.Directory.Name;
                if (strFilePath.Contains("Left.bmp"))
                    pDicLeftImage.Add(strKey, new YoonImage(strFilePath));
                else if (strFilePath.Contains("Right.bmp"))
                    pDicRightImage.Add(strKey, new YoonImage(strFilePath));
            }
        }

        static void ProcessDrop()
        {
            // Parsing
            _strRootDir = Path.Combine(_strRootDir, @"Drops");
            List<YoonImage> pListImage = YoonImage.LoadImages(_strRootDir);
            // Image Processing
            foreach (YoonImage pImage in pListImage)
            {
                YoonImage pResultImage = pImage.ToGrayImage();
                YoonDataset pDataset = pResultImage.FindBlobs(90, false);
                List<IYoonFigure> pListFigures = pDataset.Features;
                foreach (IYoonFigure pFigure in pListFigures)
                {
                    pResultImage.DrawFigure(pFigure, Color.Yellow);
                }
            }
        }
    }
}