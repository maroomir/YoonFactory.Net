using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Threading.Tasks;
using YoonFactory;
using YoonFactory.Align;
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

            _pClm.Write("Image Parsing Completed");
            // Load Mark
            YoonImage pMarkImage = new YoonImage(Path.Combine(_strRootDir, @"mark.jpg")).ToGrayImage();
            CVImage.ShowImage(pMarkImage, "MARK");
            _pClm.Write("Load Mark Completed");
            // Set origin
            AlignObject pOriginLeft = new AlignObject(pDicLeftImage["Origin"].FindPattern(pMarkImage, 10));
            AlignObject pOriginRight = new AlignObject(pDicRightImage["Origin"].FindPattern(pMarkImage, 10));
            pOriginLeft.SetOrigin(eYoonDir2D.Left);
            pOriginRight.SetOrigin(eYoonDir2D.Right);
            YoonImage pResultLeft, pResultRight;
            using (pResultLeft = pDicLeftImage["Origin"].Clone() as YoonImage)
            {
                pResultLeft.DrawFigure(pOriginLeft.OriginFeature, Color.Chartreuse);
                pResultLeft.DrawCross((YoonVector2N) pOriginLeft.OriginPosition, Color.Chartreuse);
                CVImage.ShowImage(pResultLeft, "Origin LEFT");
            }

            using (pResultRight = pDicRightImage["Origin"].Clone() as YoonImage)
            {
                pResultRight.DrawFigure(pOriginRight.OriginFeature, Color.Chartreuse);
                pResultRight.DrawCross((YoonVector2N) pOriginRight.OriginPosition, Color.Chartreuse);
                CVImage.ShowImage(pResultRight, "Origin RIGHT");
            }

            _pClm.Write("Set Origin To Parsing");
            // Image Processing
            foreach (string strKey in pDicLeftImage.Keys)
            {
                if (strKey == "Origin") continue;
                AlignObject pObjectLeft =
                    new AlignObject(pDicLeftImage[strKey].FindPattern(pMarkImage, 10), pOriginLeft);
                AlignObject pObjectRight =
                    new AlignObject(pDicRightImage[strKey].FindPattern(pMarkImage, 10), pOriginRight);
                _pClm.Write($"{strKey} Align Start");
                AlignFactory.Align2D(eYoonDir2D.Left, pObjectLeft, pObjectRight, out double dX, out double dY,
                    out double dTheta);
                _pClm.Write($"[Left] X : {dX:F4}, Y : {dY:F4}, Theta : {dTheta:F4}");
                AlignFactory.Align2D(eYoonDir2D.Right, pObjectLeft, pObjectRight, out dX, out dY, out dTheta);
                _pClm.Write($"[Right] X : {dX:F4}, Y : {dY:F4}, Theta : {dTheta:F4}");
                using (pResultLeft = pDicLeftImage[strKey].Clone() as YoonImage)
                {
                    pResultLeft.DrawFigure(pObjectLeft.Feature, Color.Yellow);
                    pResultLeft.DrawCross((YoonVector2N) pObjectLeft.Position, Color.Yellow);
                    pResultLeft.DrawCross((YoonVector2N) pObjectLeft.OriginPosition, Color.Chartreuse);
                    CVImage.ShowImage(pResultLeft, strKey + " LEFT");
                }

                using (pResultRight = pDicRightImage[strKey].Clone() as YoonImage)
                {
                    pResultRight.DrawFigure(pObjectRight.Feature, Color.Yellow);
                    pResultRight.DrawCross((YoonVector2N) pObjectRight.Position, Color.Yellow);
                    pResultRight.DrawCross((YoonVector2N) pOriginRight.OriginPosition, Color.Chartreuse);
                    CVImage.ShowImage(pResultRight, strKey + " RIGHT");
                }
            }
            _pClm.Write("Finish The Align Process");
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
                    YoonVector2N pVectorFeature = (YoonVector2N) pDataset[iObject].Position;
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