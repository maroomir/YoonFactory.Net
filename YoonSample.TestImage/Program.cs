using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Printing;
using System.IO;
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
            Console.WriteLine("Select the processing mode = ");
            Console.Write("Align, CVAlign, Drops, Glass, CVGlass, Sift, Surf >> ");
            string strSelectionModule = Console.ReadLine();
            switch (strSelectionModule.ToLower())
            {
                case "align":
                    _pClm.Write("Start Align Process");
                    ProcessAlign();
                    break;
                case "cvalign":
                    _pClm.Write("Start CVAlign Process");
                    ProcessCVAlign();
                    break;
                case "drops":
                    _pClm.Write("Start Drops Inspection");
                    ProcessDrop();
                    break;
                case "glass":
                    _pClm.Write("Start Glass Detector");
                    ProcessGlass();
                    break;
                case "cvglass":
                    _pClm.Write("Start CVGlass Detector");
                    ProcessCVGlass();
                    break;
                case "sift":
                    _pClm.Write("Start SIFT Feature Detector");
                    ProcessFeature("SIFT");
                    break;
                case "surf":
                    _pClm.Write("Start SIFT Feature Detector");
                    ProcessFeature("SURF");
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
            Stopwatch pTimer = new Stopwatch();
            foreach (string strKey in pDicLeftImage.Keys)
            {
                if (strKey == "Origin") continue;
                pTimer.Reset();
                pTimer.Start();
                AlignObject pObjectLeft =
                    new AlignObject(pDicLeftImage[strKey].FindPattern(pMarkImage, 10), pOriginLeft);
                AlignObject pObjectRight =
                    new AlignObject(pDicRightImage[strKey].FindPattern(pMarkImage, 10), pOriginRight);
                _pClm.Write($"Tact-time : {pTimer.ElapsedMilliseconds:F1}ms");
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

        static void ProcessCVAlign()
        {
            // Parsing
            _strRootDir = Path.Combine(_strRootDir, @"Align");
            Dictionary<string, CVImage> pDicLeftImage = new Dictionary<string, CVImage>();
            Dictionary<string, CVImage> pDicRightImage = new Dictionary<string, CVImage>();
            List<string> pListFilePath = FileFactory.GetExtensionFilePaths(_strRootDir, ".bmp");
            _pClm.Write("Image Load Completed");
            foreach (string strFilePath in pListFilePath)
            {
                FileInfo pFile = new FileInfo(strFilePath);
                string strKey = pFile.Directory.Name;
                if (strFilePath.Contains("Left.bmp"))
                    pDicLeftImage.Add(strKey, new CVImage(new YoonImage(strFilePath)));
                else if (strFilePath.Contains("Right.bmp"))
                    pDicRightImage.Add(strKey, new CVImage(new YoonImage(strFilePath)));
            }

            _pClm.Write("Image Parsing Completed");
            // Load Mark
            CVImage pMarkImage = new CVImage(new YoonImage(Path.Combine(_strRootDir, @"mark.jpg")).ToGrayImage());
            pMarkImage.ShowImage("MARK");
            _pClm.Write("Load Mark Completed");
            // Set origin
            AlignObject pOriginLeft = new AlignObject(pDicLeftImage["Origin"].FindTemplate(pMarkImage, 0.7));
            AlignObject pOriginRight = new AlignObject(pDicRightImage["Origin"].FindTemplate(pMarkImage, 0.7));
            pOriginLeft.SetOrigin(eYoonDir2D.Left);
            pOriginRight.SetOrigin(eYoonDir2D.Right);
            CVImage pResultLeft, pResultRight;
            using (pResultLeft = pDicLeftImage["Origin"].Clone() as CVImage)
            {
                pResultLeft.DrawFigure(pOriginLeft.OriginFeature, Color.Chartreuse);
                pResultLeft.DrawCross((YoonVector2N) pOriginLeft.OriginPosition, Color.Chartreuse);
                pResultLeft.ShowImage("Origin LEFT");
            }

            using (pResultRight = pDicRightImage["Origin"].Clone() as CVImage)
            {
                pResultRight.DrawFigure(pOriginRight.OriginFeature, Color.Chartreuse);
                pResultRight.DrawCross((YoonVector2N) pOriginRight.OriginPosition, Color.Chartreuse);
                pResultRight.ShowImage("Origin RIGHT");
            }

            _pClm.Write("Set Origin To Parsing");
            // Image Processing
            Stopwatch pTimer = new Stopwatch();
            foreach (string strKey in pDicLeftImage.Keys)
            {
                if (strKey == "Origin") continue;
                pTimer.Reset();
                pTimer.Start();
                AlignObject pObjectLeft =
                    new AlignObject(pDicLeftImage[strKey].FindTemplate(pMarkImage, 0.7), pOriginLeft);
                AlignObject pObjectRight =
                    new AlignObject(pDicRightImage[strKey].FindTemplate(pMarkImage, 0.7), pOriginRight);
                pTimer.Stop();
                _pClm.Write($"Tact-time : {pTimer.ElapsedMilliseconds:F1}ms");
                _pClm.Write($"{strKey} Align Start");
                AlignFactory.Align2D(eYoonDir2D.Left, pObjectLeft, pObjectRight, out double dX, out double dY,
                    out double dTheta);
                _pClm.Write($"[Left] X : {dX:F4}, Y : {dY:F4}, Theta : {dTheta:F4}");
                AlignFactory.Align2D(eYoonDir2D.Right, pObjectLeft, pObjectRight, out dX, out dY, out dTheta);
                _pClm.Write($"[Right] X : {dX:F4}, Y : {dY:F4}, Theta : {dTheta:F4}");
                using (pResultLeft = pDicLeftImage[strKey].Clone() as CVImage)
                {
                    pResultLeft.DrawFigure(pObjectLeft.Feature, Color.Yellow);
                    pResultLeft.DrawCross((YoonVector2N) pObjectLeft.Position, Color.Yellow);
                    pResultLeft.DrawCross((YoonVector2N) pObjectLeft.OriginPosition, Color.Chartreuse);
                    pResultLeft.ShowImage(strKey + " LEFT");
                }

                using (pResultRight = pDicRightImage[strKey].Clone() as CVImage)
                {
                    pResultRight.DrawFigure(pObjectRight.Feature, Color.Yellow);
                    pResultRight.DrawCross((YoonVector2N) pObjectRight.Position, Color.Yellow);
                    pResultRight.DrawCross((YoonVector2N) pOriginRight.OriginPosition, Color.Chartreuse);
                    pResultRight.ShowImage(strKey + " RIGHT");
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
                CVImage.ShowImage(pImage, pImage.FileName);
        }

        static void ProcessGlass()
        {
            // Parsing
            _strRootDir = Path.Combine(_strRootDir, @"Glass");
            List<YoonImage> pListImage = YoonImage.LoadImages(_strRootDir);
            _pClm.Write("Image Load Completed");
            // Image Processing
            List<YoonImage> pListResult = new List<YoonImage>();
            object pLocker = new object();
            Stopwatch pTimer = new Stopwatch();
            pTimer.Reset();
            pTimer.Start();
            for (int i = 0; i < pListImage.Count; i++)
            {
                YoonImage pResultImage = pListImage[i].ToGrayImage();
                YoonObject pLeftObject = pResultImage.FindLine(eYoonDir2D.Right, 100, true);
                YoonObject pRightObject = pResultImage.FindLine(eYoonDir2D.Left, 100, true);
                YoonObject pTopObject = pResultImage.FindLine(eYoonDir2D.Top, 100, true);
                YoonObject pBottomObject = pResultImage.FindLine(eYoonDir2D.Bottom, 100, true);
                YoonRect2N pRectObject = new YoonRect2N((YoonLine2N) pTopObject.Feature,
                    (YoonLine2N) pLeftObject.Feature, (YoonLine2N) pBottomObject.Feature,
                    (YoonLine2N) pRightObject.Feature);
                pResultImage.DrawFigure(pLeftObject.Feature, Color.Yellow, 3);
                pResultImage.DrawFigure(pRightObject.Feature, Color.Yellow, 3);
                pResultImage.DrawFigure(pTopObject.Feature, Color.Yellow, 3);
                pResultImage.DrawFigure(pBottomObject.Feature, Color.Yellow, 3);
                pResultImage.DrawRect(pRectObject, Color.Aqua, 2);
                pListResult.Add(pResultImage);
            }

            pTimer.Stop();
            _pClm.Write($"Image Processing Completed [{pTimer.ElapsedMilliseconds / pListResult.Count:F1}ms/img]");
            foreach (YoonImage pImage in pListResult)
                CVImage.ShowImage(pImage, pImage.FilePath);
        }

        static void ProcessCVGlass()
        {
            // Parsing
            _strRootDir = Path.Combine(_strRootDir, @"Glass");
            List<YoonImage> pListImage = YoonImage.LoadImages(_strRootDir);
            _pClm.Write("Image Load Completed");
            // Image Processing
            List<CVImage> pListResult = new List<CVImage>();
            object pLocker = new object();
            Stopwatch pTimer = new Stopwatch();
            pTimer.Reset();
            pTimer.Start();
            for (int i = 0; i < pListImage.Count; i++)
            {
                CVImage pResultImage = new CVImage(pListImage[i].ToGrayImage());
                YoonDataset pResultDataset = pResultImage.FindLines(80, 200, nMaxCount:30);
                _pClm.Write($"Sample {i:D} : Find {pResultDataset.Count:D} objects");
                foreach (YoonObject pObject in pResultDataset)
                    pResultImage.DrawFigure(pObject.Feature, Color.Yellow, 3);
                pListResult.Add(pResultImage);
            }

            pTimer.Stop();
            _pClm.Write($"Image Processing Completed [{pTimer.ElapsedMilliseconds / pListResult.Count:F1}ms/img]");
            foreach (CVImage pImage in pListResult)
                pImage.ShowImage(pImage.FileName);
        }

        static void ProcessFeature(string strProcess)
        {
            // Parsing
            _strRootDir = Path.Combine(_strRootDir, @"Feature");
            List<YoonImage> pListImage = YoonImage.LoadImages(_strRootDir);
            _pClm.Write("Image Load Completed");
            // Insert Parameter
            Dictionary<string, string> pDicArgs = new Dictionary<string, string>();
            switch (strProcess)
            {
                case "SURF":
                    Console.Write("MetricThreshold (default : 1000.0) >> ");
                    pDicArgs.Add("MetricThreshold" ,Console.ReadLine());
                    Console.Write("NumOctaves (default : 3) >> ");
                    pDicArgs.Add("NumOctaves", Console.ReadLine());
                    Console.Write("NumScaleLevels (default : 4) >> ");
                    pDicArgs.Add("NumScaleLevels",Console.ReadLine());
                    break;
                case "SIFT":
                    Console.Write("NumOctaves (default : 3) >> ");
                    pDicArgs.Add("NumOctaves", Console.ReadLine());
                    Console.Write("PeakThresh (default : 0) >> ");
                    pDicArgs.Add("PeakThresh", Console.ReadLine());
                    Console.Write("EdgeThresh (default : 10) >> ");
                    pDicArgs.Add("EdgeThresh", Console.ReadLine());
                    Console.Write("Magnif (default : 3) >> ");
                    pDicArgs.Add("Magnif", Console.ReadLine());
                    Console.Write("WindowSize (default : 2) >> ");
                    pDicArgs.Add("WindowSize", Console.ReadLine());
                    break;
                default:
                    return;
            }
            _pClm.Write(strProcess + " Parameter Input Completed");
            // Image Processing
            List<CVImage> pListResult = new List<CVImage>();
            Stopwatch pTimer = new Stopwatch();
            foreach (YoonImage pProcessImage in pListImage)
            {
                CVImage pResultImage = new CVImage(pProcessImage.ToGrayImage());
                pResultImage.FilePath = pProcessImage.FilePath;
                YoonDataset pResultDataset = new YoonDataset();
                int nOctaves, nScaleLevel;
                double dMetricThresh, dContrashThresh, dEdgeThresh, dFilter;
                switch (strProcess)
                {
                    case "SURF":
                        dMetricThresh = double.Parse(pDicArgs["MetricThreshold"]);
                        nOctaves = int.Parse(pDicArgs["NumOctaves"]);
                        nScaleLevel = int.Parse(pDicArgs["NumScaleLevels"]);
                        pTimer.Reset();
                        pTimer.Start();
                        pResultDataset = pResultImage.Surf(dMetricThresh, nOctaves, nScaleLevel);
                        pTimer.Stop();
                        break;
                    case "SIFT":
                        nOctaves = int.Parse(pDicArgs["NumOctaves"]);
                        dContrashThresh = double.Parse(pDicArgs["PeakThresh"]);
                        dEdgeThresh = double.Parse(pDicArgs["EdgeThresh"]);
                        dFilter = double.Parse(pDicArgs["WindowSize"]);
                        pTimer.Reset();
                        pTimer.Start();
                        pResultDataset = pResultImage.Sift(nOctaves, dContrashThresh, dEdgeThresh, dFilter);
                        pTimer.Stop();
                        break;
                    default:
                        return;
                }

                _pClm.Write(
                    $"Sample {pProcessImage.FileName} : Find {pResultDataset.Count:D} objects, {pTimer.ElapsedMilliseconds:F2} ms");
                foreach (YoonObject pObject in pResultDataset)
                    pResultImage.DrawCross(pObject.Position as YoonVector2N, Color.Aqua, 3, 1);
                pListResult.Add(pResultImage);
            }
            
            foreach (CVImage pShowImage in pListResult)
                pShowImage.ShowImage(pShowImage.FileName);
        }
    }
}