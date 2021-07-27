using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Threading.Tasks;
using YoonFactory;
using YoonFactory.CV;
using YoonFactory.Files;
using YoonFactory.Image;
using YoonFactory.Log;

namespace YoonSample.TestImage
{
    class Program
    {
        public static string _strRootDir = Path.Combine(FileFactory.GetParantsRoot(Directory.GetCurrentDirectory(), 4),
            @"Sample", @"Image");

        public static YoonConsoler _pClm = new YoonConsoler();

        static void Main(string[] args)
        {
            Console.Write("Select the processing mode (Align, Drops, Glass) >> ");
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
            _pClm.Write("Image Load Completed");
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
            _pClm.Write("Image Load Completed");
            // Image Processing
            List<YoonImage> pListResult = new List<YoonImage>();
            object pLocker = new object();
            Stopwatch pTimer = new Stopwatch();
            pTimer.Reset();
            pTimer.Start();
            //Parallel.For(0, pListImage.Count, i =>  // Use the parallel processing under the 30 counts
            for (int i=0; i<pListImage.Count; i++)
            {
                YoonImage pResultImage = pListImage[i].ToGrayImage();
                YoonRect2N pScanArea = new YoonRect2N(pListImage[i].CenterPos, pListImage[i].Width - 100, pListImage[i].Height - 200);
                YoonDataset pDataset = pResultImage.FindBlobs(pScanArea, 90, false);
                for (int iObject = 0; iObject < pDataset.Count; iObject++)
                {
                    pResultImage.DrawRect(pScanArea, Color.Chartreuse);
                    YoonVector2N pVectorFeature = (YoonVector2N) pDataset[iObject].ReferencePosition;
                    pResultImage.DrawFigure(pDataset[iObject].Feature, Color.Yellow);
                    pResultImage.DrawCross(pVectorFeature, Color.Yellow);
                }

                lock (pLocker)
                {
                    _pClm.Write($"Image {i:D} processing completed");
                    pListResult.Add(pResultImage);
                }
            }
            //);
            pTimer.Stop();
            _pClm.Write($"Image Processing Completed [{pTimer.ElapsedMilliseconds / pListResult.Count:F1}ms/img]");
            // Show Image
            foreach (YoonImage pImage in pListResult)
                CVImage.ShowImage(pImage, pImage.FilePath);
        }
    }
}