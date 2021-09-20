using System;
using System.Drawing;
using System.Runtime.InteropServices;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using OpenCvSharp;
using OpenCvSharp.Features2D;
using OpenCvSharp.XFeatures2D;
using YoonFactory.Files;
using YoonFactory.Image;
using Point = OpenCvSharp.Point;

namespace YoonFactory.CV
{
    public static class CVFactory
    {
        public const int MAX_FEATURES = 10000;
        
        public static YoonObject FindTemplate(this CVImage pSourceImage, CVImage pTemplateImage, double dScore = 0.7) =>
            TemplateMatch.FindTemplate(pTemplateImage, pSourceImage, dScore);

        public static CVImage Add(this CVImage pSourceImage, CVImage pObjectImage) =>
            TwoImageProcess.Add(pSourceImage, pObjectImage);

        public static CVImage Subtract(this CVImage pSourceImage, CVImage pObjectImage) =>
            TwoImageProcess.Subtract(pSourceImage, pObjectImage);

        public static CVImage Multiply(this CVImage pSourceImage, CVImage pObjectImage) =>
            TwoImageProcess.Multiply(pSourceImage, pObjectImage);

        public static CVImage Divide(this CVImage pSourceImage, CVImage pObjectImage) =>
            TwoImageProcess.Divide(pSourceImage, pObjectImage);

        public static CVImage Max(this CVImage pSourceImage, CVImage pObjectImage) =>
            TwoImageProcess.Max(pSourceImage, pObjectImage);

        public static CVImage Min(this CVImage pSourceImage, CVImage pObjectImage) =>
            TwoImageProcess.Min(pSourceImage, pObjectImage);

        public static CVImage AbsDiff(this CVImage pSourceImage, CVImage pObjectImage) =>
            TwoImageProcess.AbsDiff(pSourceImage, pObjectImage);

        public static CVImage Binary(this CVImage pSourceImage, double dThreshold) =>
            Filter.Binary(pSourceImage, dThreshold);

        public static CVImage Sobel(this CVImage pSourceImage, int nOrderX = 1, int nOrderY = 0) =>
            Filter.Sobel(pSourceImage, nOrderX, nOrderY);

        public static CVImage Scharr(this CVImage pSourceImage, int nOrderX = 1, int nOrderY = 0) =>
            Filter.Scharr(pSourceImage, nOrderX, nOrderY);

        public static CVImage Laplacian(this CVImage pSourceImage) => Filter.Laplacian(pSourceImage);

        public static CVImage Gaussian(this CVImage pSourceImage, int nSizeX = 3, int nSizeY = 3) =>
            Filter.Gaussian(pSourceImage, nSizeX, nSizeY);

        public static CVImage
            Canny(this CVImage pSourceImage, double dThresholdMin = 100, double dThresholdMax = 200) =>
            Filter.Canny(pSourceImage, dThresholdMin, dThresholdMax);

        public static CVImage
            FillFlood(this CVImage pSourceImage, YoonVector2N pVector, byte nThreshold, bool bWhite) =>
            Fill.FillFlood(pSourceImage, pVector, nThreshold, bWhite);

        public static CVImage FillFlood(this CVImage pSourceImage, YoonVector2N pVector, byte nThreshold,
            Color pFillColor) => Fill.FillFlood(pSourceImage, pVector, nThreshold, pFillColor);

        public static YoonDataset FindLines(this CVImage pSourceImage, double dThresholdMin, double dThresholdMax,
            int nThresholdHough = 150, int nMaxCount = 30) =>
            LineMatch.FindLines(pSourceImage, dThresholdMin, dThresholdMax, nThresholdHough, nMaxCount);
        
        public static YoonDataset FindSiftFeature(this CVImage pSourceImage, int nOctaves = 3, double dContrastThresh = 0.4,
            double dEdgeThresh = 10.0, double dFilterSigma = 2.0) => FeatureDetector.FindSiftFeature(pSourceImage, nOctaves,
            dContrastThresh, dEdgeThresh, dFilterSigma);

        public static YoonDataset FindSurfFeature(this CVImage pSourceImage, double dMetricThresh = 1000.0, int nOctaves = 3,
            int nScaleLevel = 4) => FeatureDetector.FindSurfFeature(pSourceImage, dMetricThresh, nOctaves, nScaleLevel);

        public static YoonDataset FindHarrisFeature(this CVImage pSourceImage, int nThresh = 100, int nBlockSize = 2,
            int nApertureSize = 3, double dKSize = 0.04)
            => FeatureDetector.FindHarrisFeature(pSourceImage, nThresh, nBlockSize, nApertureSize, dKSize);

        public static CVImage Resize(this CVImage pSourceImage, double dRatio) =>
            Transform.Resize(pSourceImage, dRatio);

        public static CVImage Resize(this CVImage pSourceImage, int nDestWidth, int nDestHeight) =>
            Transform.Resize(pSourceImage, nDestWidth, nDestHeight);
        
        public static class VideoProcessor
        {
            public static List<CVImage> GetLocalFrames(string strFileName, int nStep = 1)
            {
                List<CVImage> pListFrame = new List<CVImage>();
                using (VideoCapture pVideo = new VideoCapture(strFileName))
                {
                    using (Mat pFrame = new Mat())
                    {
                        int iCount = 0;
                        while (pVideo.PosFrames != pVideo.FrameCount)
                        {
                            if (!pVideo.Read(pFrame))
                                Cv2.WaitKey();
                            if (pFrame.Size().Width > 0 && pFrame.Size().Height > 0)
                                iCount++;
                            if (iCount == nStep)
                            {
                                pListFrame.Add(new CVImage(pFrame));
                                iCount = 0;
                            }

                            if (Cv2.WaitKey(1) >= 0)
                                break;
                        }
                    }

                    pVideo.Release();
                }

                return pListFrame;
            }

            public static List<CVImage> GetRTSPFrames(string strAddress, int nStep = 1)
            {
                List<CVImage> pListFrame = new List<CVImage>();
                using (VideoCapture pVideo = new VideoCapture())
                {
                    pVideo.Open(strAddress);
                    using (Mat pFrame = new Mat())
                    {
                        int iCount = 0;
                        while (true)
                        {
                            if (!pVideo.Read(pFrame))
                                Cv2.WaitKey();
                            if (pFrame.Size().Width > 0 && pFrame.Size().Height > 0)
                                iCount++;
                            if (iCount == nStep)
                            {
                                pListFrame.Add(new CVImage(pFrame));
                                iCount = 0;
                            }

                            if (Cv2.WaitKey(1) >= 0)
                                break;
                        }
                    }
                }

                return pListFrame;
            }

            public static async Task ReceiveLocalFrameAsync(string strFileName, RecieveFrameCallback pCallback)
            {
                if (!FileFactory.VerifyFileExtensions(strFileName, "mp4", "avi"))
                    return;
                // Execute asynchronous to separate from the main process
                await Task.Run(new Action(() =>
                {
                    using (VideoCapture pVideo = new VideoCapture(strFileName))
                    {
                        using (Mat pFrame = new Mat())
                        {
                            while (pVideo.PosFrames != pVideo.FrameCount)
                            {
                                if (!pVideo.Read(pFrame))
                                    Cv2.WaitKey();
                                if (pFrame.Size().Width > 0 && pFrame.Size().Height > 0)
                                    pCallback?.Invoke(pVideo, new FrameArgs(pFrame));
                                if (Cv2.WaitKey(1) >= 0)
                                    break;
                            }
                        }

                        pVideo.Release();
                    }
                }));
            }

            public static async Task ReceiveRTSPFrameAsync(string strAddress, RecieveFrameCallback pCallback)
            {
                // Execute asynchronous to separate from the main process
                await Task.Run(new Action(() =>
                {
                    using (VideoCapture pVideo = new VideoCapture())
                    {
                        pVideo.Open(strAddress);
                        using (Mat pFrame = new Mat())
                        {
                            while (true)
                            {
                                if (!pVideo.Read(pFrame))
                                    Cv2.WaitKey();
                                if (pFrame.Size().Width > 0 && pFrame.Size().Height > 0)
                                    pCallback?.Invoke(pVideo, new FrameArgs(pFrame));
                                if (Cv2.WaitKey(1) >= 0)
                                    break;
                            }
                        }

                        pVideo.Release();
                    }
                }));
            }
        }
        
        public static class Converter
        {
            public static Mat ToGrayMatrix(IntPtr pBufferAddress, int nWidth, int nHeight)
            {
                if (pBufferAddress == IntPtr.Zero) return null;
                byte[] pBuffer = new byte[nWidth * nHeight];
                Marshal.Copy(pBufferAddress, pBuffer, 0, nWidth * nHeight);
                return Mat.FromImageData(pBuffer, ImreadModes.Grayscale);
            }

            public static Mat ToGrayMatrix(byte[] pBuffer, int nWidth, int nHeight)
            {
                if (pBuffer == null || pBuffer.Length != nWidth * nHeight) return null;
                return Mat.FromImageData(pBuffer, ImreadModes.Grayscale);
            }

            public static Mat ToColorMatrix(IntPtr pBufferAddress, int nWidth, int nHeight)
            {
                if (pBufferAddress == IntPtr.Zero) return null;
                byte[] pBuffer = new byte[nWidth * nHeight * 3];
                Marshal.Copy(pBufferAddress, pBuffer, 0, nWidth * nHeight);
                return Mat.FromImageData(pBuffer, ImreadModes.Color);
            }

            public static Mat ToColorMatrix(byte[] pBuffer, int nWidth, int nHeight)
            {
                if (pBuffer == null || pBuffer.Length != nWidth * nHeight * 3) return null;
                return Mat.FromImageData(pBuffer, ImreadModes.Color);
            }
        }

        public static class TemplateMatch
        {
            public static YoonObject FindTemplate(CVImage pTemplateImage, CVImage pSourceImage, double dScore = 0.7)
            {
                Rect pRectResult = FindTemplate(pTemplateImage.Matrix, pSourceImage.Matrix, out var dMatchScore, dScore,
                    TemplateMatchModes.CCoeffNormed);
                return new YoonObject(0, pRectResult.ToYoonRect(), dMatchScore,
                    (int) (dMatchScore * pRectResult.Width * pRectResult.Height));
            }

            public static Rect FindTemplate(Mat pTemplateMatrix, Mat pSourceMatrix, out double dMatchScore,
                double dThresholdScore, TemplateMatchModes nMode = TemplateMatchModes.CCoeffNormed)
            {
                Mat pResultImage = new Mat();
                Cv2.MatchTemplate(pSourceMatrix, pTemplateMatrix, pResultImage, nMode);
                Cv2.MinMaxLoc(pResultImage, out _, out double dMaxVal, out _, out Point pMaxPos);
                if (dMaxVal > dThresholdScore)
                {
                    dMatchScore = dMaxVal;
                    return new Rect(pMaxPos, pTemplateMatrix.Size());
                }
                else
                    throw new InvalidOperationException(
                        "[YOONCV EXCEPTION] Process Failed : Matching score is too low");
            }
        }

        public static class LineMatch
        {
            public static YoonDataset FindLines(CVImage pSourceImage, double dThresholdMin, double dThresholdMax,
                int nThresholdHough = 150, int nMaxCount = 30)
            {
                if (pSourceImage.Channel != 1)
                    throw new FormatException("[YOONCV EXCEPTION] Image arguments is not 8bit format");

                List<YoonLine2N> pListLine =
                    FindLines(pSourceImage.Matrix, dThresholdMin, dThresholdMax, nThresholdHough, nMaxCount);
                YoonDataset pResultDataset = new YoonDataset();
                for (int i = 0; i < pListLine.Count; i++)
                {
                    pResultDataset.Add(new YoonObject(i, pListLine[i].Clone() as YoonLine2N, pListLine[i].CenterPos));
                }

                return pResultDataset;
            }

            public static List<YoonLine2N> FindLines(Mat pSourceMatrix, double dThresholdMin, double dThresholdMax,
                int nThresholdHough, int nMaxCount = 30)
            {
                Mat pResultMatrix = Filter.Binary(pSourceMatrix, (dThresholdMin + dThresholdMax) / 2);
                pResultMatrix = Filter.Canny(pResultMatrix, dThresholdMin, dThresholdMax);
                LineSegmentPolar[] pLineStorage = pResultMatrix.HoughLines(1, Cv2.PI / 180, nThresholdHough, 0, 0);
                List<YoonLine2N> pListLine = new List<YoonLine2N>();
                for (int iLine = 0; iLine < Math.Min(nMaxCount, pLineStorage.Length); iLine++)
                {
                    float dDistance = pLineStorage[iLine].Rho; // Distance as the zero position
                    float dTheta = pLineStorage[iLine].Theta; // Angle of the perpendicular line 
                    double dX0 = dDistance * Math.Cos(dTheta); // Intersection position with perpendicular line 
                    double dY0 = dDistance * Math.Sin(dTheta); // Intersection position with perpendicular line
                    YoonVector2N pVector1 = new YoonVector2N(Convert.ToInt32(dX0 - pSourceMatrix.Width * Math.Sin(dTheta)),
                        Convert.ToInt32(dY0 + pSourceMatrix.Height * Math.Cos(dTheta)));
                    YoonVector2N pVector2 = new YoonVector2N(Convert.ToInt32(dX0 + pSourceMatrix.Width * Math.Sin(dTheta)),
                        Convert.ToInt32(dY0 - pSourceMatrix.Height * Math.Cos(dTheta)));
                    pListLine.Add(new YoonLine2N(pVector1, pVector2));
                }

                return pListLine;
            }
        }
        
        public static class TwoImageProcess
        {
            public static CVImage Add(CVImage pSourceImage, CVImage pObjectImage)
            {
                if (pSourceImage.Width != pObjectImage.Width || pSourceImage.Height != pObjectImage.Height)
                    throw new ArgumentOutOfRangeException("[YOONCV EXCEPTION] Image size is not same");
                return new CVImage(Add(pSourceImage.Matrix, pObjectImage.Matrix));
            }

            public static CVImage Add(CVImage pSourceImage, CVImage pObjectImage, double dSourceWeight)
            {
                if (dSourceWeight is > 1 or < 0)
                    throw new ArgumentOutOfRangeException("[YOONCV EXCEPTION] Image size is not same");
                return new CVImage(Add(pSourceImage.Matrix, dSourceWeight, pObjectImage.Matrix, 1 - dSourceWeight));
            }

            public static Mat Add(Mat pSourceMatrix, Mat pObjectMatrix)
            {
                Mat pResultMatrix = new Mat();
                Cv2.Add(pSourceMatrix, pObjectMatrix, pResultMatrix);
                return pResultMatrix;
            }

            public static Mat Add(Mat pSourceMatrix, double dSourceWeight, Mat pObjectMatrix, double dObjectWeight)
            {
                if (dSourceWeight == 0 && dObjectWeight == 0)
                    throw new ArgumentException("[YOONCV EXCEPTION] Weight value is abnormal");
                if (pSourceMatrix.Width != pObjectMatrix.Width || pSourceMatrix.Height != pObjectMatrix.Height)
                    throw new ArgumentOutOfRangeException("[YOONCV EXCEPTION] Image size is not same");
                dSourceWeight = dSourceWeight / (dSourceWeight + dObjectWeight);
                dObjectWeight = dObjectWeight / (dSourceWeight + dObjectWeight);
                Mat pResultMatrix = new Mat();
                Cv2.AddWeighted(pSourceMatrix, dSourceWeight, pObjectMatrix, dObjectWeight, 0, pResultMatrix);
                return pResultMatrix;
            }

            public static CVImage Subtract(CVImage pSourceImage, CVImage pObjectImage)
            {
                if (pSourceImage.Width != pObjectImage.Width || pSourceImage.Height != pObjectImage.Height)
                    throw new ArgumentOutOfRangeException("[YOONCV EXCEPTION] Image size is not same");
                return new CVImage(Subtract(pSourceImage.Matrix, pObjectImage.Matrix));
            }

            public static Mat Subtract(Mat pSourceMatrix, Mat pObjectMatrix)
            {
                Mat pResultMatrix = new Mat();
                Cv2.Subtract(pSourceMatrix, pObjectMatrix, pResultMatrix);
                return pResultMatrix;
            }

            public static CVImage Multiply(CVImage pSourceImage, CVImage pObjectImage)
            {
                if (pSourceImage.Width != pObjectImage.Width || pSourceImage.Height != pObjectImage.Height)
                    throw new ArgumentOutOfRangeException("[YOONCV EXCEPTION] Image size is not same");
                return new CVImage(Multiply(pSourceImage.Matrix, pObjectImage.Matrix));
            }

            public static Mat Multiply(Mat pSourceMatrix, Mat pObjectMatrix)
            {
                Mat pResultMatrix = new Mat();
                Cv2.Multiply(pSourceMatrix, pObjectMatrix, pResultMatrix);
                return pResultMatrix;
            }

            public static CVImage Divide(CVImage pSourceImage, CVImage pObjectImage)
            {
                if (pSourceImage.Width != pObjectImage.Width || pSourceImage.Height != pObjectImage.Height)
                    throw new ArgumentOutOfRangeException("[YOONCV EXCEPTION] Image size is not same");
                return new CVImage(Divide(pSourceImage.Matrix, pObjectImage.Matrix));
            }

            public static Mat Divide(Mat pSourceMatrix, Mat pObjectMatrix)
            {
                Mat pResultMatrix = new Mat();
                Cv2.Divide(pSourceMatrix, pObjectMatrix, pResultMatrix);
                return pResultMatrix;
            }

            public static CVImage Max(CVImage pSourceImage, CVImage pObjectImage)
            {
                if (pSourceImage.Width != pObjectImage.Width || pSourceImage.Height != pObjectImage.Height)
                    throw new ArgumentOutOfRangeException("[YOONCV EXCEPTION] Image size is not same");
                return new CVImage(Max(pSourceImage.Matrix, pObjectImage.Matrix));
            }

            public static Mat Max(Mat pSourceMatrix, Mat pObjectMatrix)
            {
                Mat pResultMatrix = new Mat();
                Cv2.Max(pSourceMatrix, pObjectMatrix, pResultMatrix);
                return pResultMatrix;
            }

            public static CVImage Min(CVImage pSourceImage, CVImage pObjectImage)
            {
                if (pSourceImage.Width != pObjectImage.Width || pSourceImage.Height != pObjectImage.Height)
                    throw new ArgumentOutOfRangeException("[YOONCV EXCEPTION] Image size is not same");
                return new CVImage(Min(pSourceImage.Matrix, pObjectImage.Matrix));
            }

            public static Mat Min(Mat pSourceMatrix, Mat pObjectMatrix)
            {
                Mat pResultMatrix = new Mat();
                Cv2.Max(pSourceMatrix, pObjectMatrix, pResultMatrix);
                return pResultMatrix;
            }

            public static CVImage AbsDiff(CVImage pSourceImage, CVImage pObjectImage)
            {
                if (pSourceImage.Width != pObjectImage.Width || pSourceImage.Height != pObjectImage.Height)
                    throw new ArgumentOutOfRangeException("[YOONCV EXCEPTION] Image size is not same");
                return new CVImage(AbsDiff(pSourceImage.Matrix, pObjectImage.Matrix));
            }

            public static Mat AbsDiff(Mat pSourceMatrix, Mat pObjectMatrix)
            {
                Mat pResultMatrix = new Mat();
                Cv2.Absdiff(pSourceMatrix, pObjectMatrix, pResultMatrix);
                return pResultMatrix;
            }

            public static CVImage Blending(CVImage pSourceImage, CVImage pObjectImage)
            {
                return new CVImage(Blending(pSourceImage.Matrix, pObjectImage.Matrix));
            }

            public static Mat Blending(Mat pSourceMatrix, Mat pObjectMatrix, int nDepth = 5)
            {
                Mat pPipelineMatrix = pSourceMatrix.Clone();
                // Construct the Gaussian Pyramid
                List<Mat> pPyrGaussianSource = new List<Mat>();
                List<Mat> pPyrGaussianObject = new List<Mat>();
                pPyrGaussianSource.Add(pSourceMatrix);
                pPyrGaussianObject.Add(pObjectMatrix);
                for (int i = 0; i < nDepth; i++)
                {
                    Cv2.PyrDown(pPyrGaussianSource[i], pPipelineMatrix);
                    pPyrGaussianSource.Add(pPipelineMatrix.Clone());
                    Cv2.PyrDown(pPyrGaussianObject[i], pPipelineMatrix);
                    pPyrGaussianObject.Add(pPipelineMatrix.Clone());
                }

                // Construct the Laplacian Pyramid
                List<Mat> pPyrLaplacianSource = new List<Mat>();
                List<Mat> pPyrLaplacianObject = new List<Mat>();
                pPyrLaplacianSource.Add(pPyrGaussianSource.Last());
                pPyrLaplacianObject.Add(pPyrLaplacianObject.Last());
                for (int i = nDepth - 1; i >= 0; i--)
                {
                    int nDestWidth = pPyrGaussianSource[i - 1].Width;
                    int nDestHeight = pPyrGaussianSource[i - 1].Height;
                    Cv2.PyrUp(pPyrLaplacianSource[i], pPipelineMatrix, new OpenCvSharp.Size(nDestWidth, nDestHeight));
                    Cv2.Subtract(pPyrGaussianSource[i - 1], pPipelineMatrix.Clone(), pPipelineMatrix);
                    pPyrLaplacianSource.Add(pPipelineMatrix.Clone());

                    nDestWidth = pPyrGaussianObject[i - 1].Width;
                    nDestHeight = pPyrGaussianObject[i - 1].Height;
                    Cv2.PyrUp(pPyrLaplacianObject[i], pPipelineMatrix, new OpenCvSharp.Size(nDestWidth, nDestHeight));
                    Cv2.Subtract(pPyrGaussianObject[i - 1], pPipelineMatrix.Clone(), pPipelineMatrix);
                    pPyrLaplacianObject.Add(pPipelineMatrix.Clone());
                }

                // Blend the image
                List<Mat> pListBlending = new List<Mat>();
                for (int i = 0; i < nDepth; i++)
                {
                    List<Mat> pListSumImage = new List<Mat>();
                    int nSourceWidth = pPyrLaplacianSource[i].Width;
                    int nSourceHeight = pPyrLaplacianSource[i].Height;
                    int nObjectWidth = pPyrLaplacianObject[i].Width;
                    int nObjectHeight = pPyrLaplacianObject[i].Height;
                    pListSumImage.Add(pPyrLaplacianSource[i].SubMat(0, nSourceHeight, 0, nSourceWidth / 2));
                    pListSumImage.Add(pPyrLaplacianObject[i]
                        .SubMat(0, nObjectHeight, nObjectWidth / 2 + 1, nObjectWidth));
                    Cv2.HConcat(pListSumImage, pPipelineMatrix);
                    pListSumImage.Clear();

                    pListBlending.Add(pPipelineMatrix.Clone());
                }

                // Upscale Image
                for (int i = nDepth - 1; i >= 0; i--)
                {
                    int nDestWidth = pListBlending[i - 1].Width;
                    int nDestHeight = pListBlending[i - 1].Height;
                    Cv2.PyrUp(pListBlending[i], pPipelineMatrix, new OpenCvSharp.Size(nDestWidth, nDestHeight));
                }

                return pPipelineMatrix.Clone();
            }
        }

        public static class Filter
        {
            public static CVImage Binary(CVImage pSourceImage, double dThreshold)
            {
                return new CVImage(Binary(pSourceImage.Matrix, dThreshold));
            }

            public static Mat Binary(Mat pSourceMatrix, double dThreshold)
            {
                Mat pResultMatrix = new Mat(pSourceMatrix.Size(), MatType.CV_8U);
                Cv2.Threshold(pSourceMatrix, pResultMatrix, dThreshold, 255, ThresholdTypes.Binary);
                return pResultMatrix;
            }

            public static CVImage Sobel(CVImage pSourceImage, int nOrderX = 1, int nOrderY = 0)
            {
                return new CVImage(Sobel(pSourceImage.Matrix, nOrderX, nOrderY));
            }

            public static Mat Sobel(Mat pSourceMatrix, int nOrderX, int nOrderY)
            {
                Mat pResultMatrix = new Mat();
                Cv2.Sobel(pSourceMatrix, pResultMatrix, MatType.CV_32F, nOrderX, nOrderY);
                pResultMatrix.ConvertTo(pResultMatrix, MatType.CV_8UC1); // Downscale
                return pResultMatrix;
            }

            public static CVImage Scharr(CVImage pSourceImage, int nOrderX = 1, int nOrderY = 0)
            {
                return new CVImage(Scharr(pSourceImage.Matrix, nOrderX, nOrderY));
            }

            public static Mat Scharr(Mat pSourceMatrix, int nOrderX, int nOrderY)
            {
                Mat pResultMatrix = new Mat();
                Cv2.Scharr(pSourceMatrix, pResultMatrix, MatType.CV_32F, nOrderX, nOrderY);
                pResultMatrix.ConvertTo(pResultMatrix, MatType.CV_8UC1); // Downscale
                return pResultMatrix;
            }

            public static CVImage Laplacian(CVImage pSourceImage)
            {
                return new CVImage(Laplacian(pSourceImage.Matrix));
            }

            public static Mat Laplacian(Mat pSourceMatrix)
            {
                Mat pResultMatrix = new Mat();
                Cv2.Laplacian(pSourceMatrix, pResultMatrix, MatType.CV_32F);
                pResultMatrix.ConvertTo(pResultMatrix, MatType.CV_8UC1); // Downscale
                return pResultMatrix;
            }

            public static CVImage Gaussian(CVImage pSourceImage, int nSizeX = 3, int nSizeY = 3)
            {
                return new CVImage(Gaussian(pSourceImage.Matrix, nSizeX, nSizeY));
            }

            public static Mat Gaussian(Mat pSourceMatrix, int nSizeX, int nSizeY)
            {
                Mat pResultMatrix = new Mat();
                Cv2.GaussianBlur(pSourceMatrix, pResultMatrix, new OpenCvSharp.Size(nSizeX, nSizeY), 1);
                return pResultMatrix;
            }

            public static CVImage Canny(CVImage pSourceImage, double dThresholdMin = 100, double dThresholdMax = 200)
            {
                return new CVImage(Canny(pSourceImage.Matrix, dThresholdMin, dThresholdMax));
            }

            public static Mat Canny(Mat pSourceMatrix, double dThresholdMin, double dThresholdMax)
            {
                Mat pResultMatrix = new Mat();
                Cv2.Canny(pSourceMatrix, pResultMatrix, dThresholdMin, dThresholdMax);
                return pResultMatrix;
            }
        }

        public static class Fill
        {
            public static CVImage FillFlood(CVImage pSourceImage, YoonVector2N pVector, byte nThreshold, bool bWhite)
            {
                return new CVImage(FillFlood(pSourceImage.Matrix, pVector, nThreshold, bWhite));
            }

            public static CVImage FillFlood(CVImage pSourceImage, YoonVector2N pVector, byte nThreshold,
                Color pFillColor)
            {
                return new CVImage(FillFlood(pSourceImage.Matrix, pVector, nThreshold, pFillColor));
            }

            public static Mat FillFlood(Mat pSourceMatrix, YoonVector2N pVector, byte nThreshold, bool isWhite)
            {
                Mat pResultMatrix = pSourceMatrix.Clone();
                if (isWhite)
                    Cv2.FloodFill(pResultMatrix, pVector.ToCVPoint(), Scalar.All(255), out _, Scalar.All(nThreshold),
                        Scalar.All(nThreshold), FloodFillFlags.Link8);
                else
                    Cv2.FloodFill(pResultMatrix, pVector.ToCVPoint(), Scalar.All(0), out _, Scalar.All(nThreshold),
                        Scalar.All(nThreshold), FloodFillFlags.Link8);
                return pResultMatrix;
            }

            public static Mat FillFlood(Mat pSourceMatrix, YoonVector2N pVector, byte nThreshold, Color pFillColor)
            {
                Mat pResultMatrix = pSourceMatrix.Clone();
                Cv2.FloodFill(pResultMatrix, pVector.ToCVPoint(), pFillColor.ToScalar(), out _, Scalar.All(nThreshold),
                    Scalar.All(nThreshold), FloodFillFlags.Link8);
                return pResultMatrix;
            }
        }

        public static class ColorDetector
        {
            public static CVImage DetectHSV(CVImage pSourceImage, byte nHue, byte nSaturation, byte nValue)
            {
                if (pSourceImage.Channel != 3)
                    throw new FormatException("[YOONIMAGE EXCEPTION] Image arguments is not 24bit format");
                return new CVImage(DetectHSV(pSourceImage.Matrix, nHue, nSaturation, nValue));
            }

            public static Mat DetectHSV(Mat pSourceMatrix, byte nHue, byte nSaturation, byte nValue,
                byte nThreshold = 10)
            {
                Mat pHsvMatrix = new Mat(pSourceMatrix.Size(), MatType.CV_8UC3);
                Mat pMaskMatrix = new Mat(pSourceMatrix.Size(), MatType.CV_8UC3);
                Mat pResultMatrix = new Mat(pSourceMatrix.Size(), MatType.CV_8UC3);
                Cv2.CvtColor(pSourceMatrix, pHsvMatrix, ColorConversionCodes.BGR2HSV);
                byte nHueLow = (byte) Math.Max(nHue - nThreshold / 2, 0);
                byte nHueHigh = (byte) Math.Min(nHue + nThreshold / 2, 255);
                byte nSaturationLow = (byte) Math.Max(nSaturation - nThreshold / 2, 0);
                byte nSaturationHigh = (byte) Math.Min(nSaturation + nThreshold / 2, 255);
                byte nValueLow = (byte) Math.Max(nValue - nThreshold / 2, 0);
                byte nValueHigh = (byte) Math.Min(nValue + nThreshold / 2, 255);
                Cv2.InRange(pHsvMatrix, new Scalar(nHueLow, nSaturationLow, nValueLow),
                    new Scalar(nHueHigh, nSaturationHigh, nValueHigh), pMaskMatrix);
                Cv2.BitwiseAnd(pHsvMatrix, pHsvMatrix, pResultMatrix, pMaskMatrix);
                Cv2.CvtColor(pResultMatrix, pResultMatrix, ColorConversionCodes.HSV2BGR);
                return pResultMatrix;
            }
        }

        public static class FeatureDetector
        {
            public static YoonDataset FindSiftFeature(CVImage pSourceImage, int nOctaves = 3,
                double dContrastThresh = 0.4, double dEdgeThresh = 10.0, double dFilterSigma = 2.0)
            {
                return FindSiftFeature(pSourceImage.Matrix, nOctaves, dContrastThresh, dEdgeThresh, dFilterSigma);
            }

            public static YoonDataset FindSiftFeature(Mat pSourceMatrix, int nOctaves, double dContrastThresh,
                double dEdgeThresh, double dFilterSigma)
            {
                SIFT pDetector = SIFT.Create(MAX_FEATURES, nOctaves, dContrastThresh, dEdgeThresh, dFilterSigma);
                KeyPoint[] pFeatures = pDetector.Detect(pSourceMatrix);
                YoonDataset pDataset = new YoonDataset();
                for (int iLabel = 0; iLabel < pFeatures.Length; iLabel++)
                {
                    KeyPoint pFeature = pFeatures[iLabel];
                    YoonVector2N pFeaturePos = pFeature.Pt.ToPoint().ToYoonVector();
                    int nSize = (int) pFeature.Size;
                    YoonRect2N pFeatureArea = new YoonRect2N(pFeaturePos.X, pFeaturePos.Y, nSize, nSize);
                    pDataset.Add(new YoonObject(iLabel, pFeatureArea, pFeaturePos));
                }

                return pDataset;
            }

            public static YoonDataset FindSurfFeature(CVImage pSourceImage, double dMetricThresh = 1000.0,
                int nOctaves = 3, int nScaleLevel = 4)
            {
                return FindSurfFeature(pSourceImage.Matrix, dMetricThresh, nOctaves, nScaleLevel);
            }

            public static YoonDataset FindSurfFeature(Mat pSourceMatrix, double dMetricThresh, int nOctaves,
                int nScaleLevel)
            {
                SURF pDetector = SURF.Create(dMetricThresh, nOctaves, nScaleLevel);
                KeyPoint[] pFeatures = pDetector.Detect(pSourceMatrix);
                YoonDataset pDataset = new YoonDataset();
                for (int iLabel = 0; iLabel < pFeatures.Length; iLabel++)
                {
                    KeyPoint pFeature = pFeatures[iLabel];
                    YoonVector2N pFeaturePos = pFeature.Pt.ToPoint().ToYoonVector();
                    int nSize = (int) pFeature.Size;
                    YoonRect2N pFeatureArea = new YoonRect2N(pFeaturePos.X, pFeaturePos.Y, nSize, nSize);
                    pDataset.Add(new YoonObject(iLabel, pFeatureArea, pFeaturePos));
                }

                return pDataset;
            }

            public static YoonDataset FindHarrisFeature(CVImage pSourceImage, int nThresh = 100, int nBlockSize = 2,
                int nApertureSize = 3, double dKSize = 0.04)
            {
                return FindHarrisFeature(pSourceImage.Matrix, nThresh, nBlockSize, nApertureSize, dKSize);
            }

            public static YoonDataset FindHarrisFeature(Mat pSourceMatrix, int nThresh, int nBlockSize,
                int nApertureSize, double dKSize)
            {
                Mat pPipelineMatrix = new Mat(pSourceMatrix.Size(), MatType.CV_32FC1);
                Mat pNormalizeMatrix = new Mat();
                Cv2.CornerHarris(pSourceMatrix, pPipelineMatrix, nBlockSize, nApertureSize, dKSize,
                    BorderTypes.Default);
                Cv2.Normalize(pPipelineMatrix, pNormalizeMatrix, 0, 255, NormTypes.MinMax);
                YoonDataset pDataset = new YoonDataset();
                int nLabel = 0;
                for (int iY = 0; iY < pNormalizeMatrix.Rows; iY++)
                {
                    for (int iX = 0; iX < pNormalizeMatrix.Cols; iX++)
                    {
                        if (pNormalizeMatrix.At<float>(iY, iX) > nThresh)
                        {
                            YoonRect2N pFeatureArea = new YoonRect2N(iX, iY, 5, 5);
                            YoonVector2N pFeaturePos = new YoonVector2N(iX, iY);
                            pDataset.Add(new YoonObject(nLabel++, pFeatureArea, pFeaturePos));
                        }
                    }
                }

                return pDataset;
            }
        }

        public static class FeatureMatch
        {
            public static void SiftMatching(CVImage pSourceImage, CVImage pObjectImage,
                out YoonDataset pSourceDataset, out YoonDataset pObjectDataset,
                int nOctaves = 3, double dContrastThresh = 0.4, double dEdgeThresh = 10.0, double dFilterSigma = 2.0)
            {
                SiftMatching(pSourceImage.Matrix, pObjectImage.Matrix, out pSourceDataset, out pObjectDataset,
                    nOctaves, dContrastThresh, dEdgeThresh, dFilterSigma);
            }

            public static void SiftMatching(Mat pSourceMatrix, Mat pObjectMatrix,
                out YoonDataset pSourceDataset, out YoonDataset pObjectDataset,
                int nOctaves, double dContrastThresh, double dEdgeThresh, double dFilterSigma)
            {
                SIFT pDetector = SIFT.Create(MAX_FEATURES, nOctaves, dContrastThresh, dEdgeThresh, dFilterSigma);
                BFMatcher pMatcher = new BFMatcher();
                pSourceDataset = new YoonDataset();
                pObjectDataset = new YoonDataset();
                // SIFT Detection
                Mat pSourceDescriptor = new Mat();
                Mat pObjectDescriptor = new Mat();
                pDetector.DetectAndCompute(pSourceMatrix, null, out KeyPoint[] pSourceFeatures, pSourceDescriptor);
                pDetector.DetectAndCompute(pObjectMatrix, null, out KeyPoint[] pObjectFeatures, pObjectDescriptor);
                // Matching the feature each others
                DMatch[][] pMatchArray = pMatcher.KnnMatch(pSourceDescriptor, pObjectDescriptor, 2);
                int nLabelNo = 0;
                for (int i = 0; i < pMatchArray.Length; i++)
                {
                    if (pMatchArray[i][0].Distance < pMatchArray[i][1].Distance * 0.3)
                    {
                        DMatch pMatchResult = pMatchArray[i][0];
                        // Input the source data-set
                        int iSource = pMatchResult.QueryIdx;
                        int nSourceSize = (int) pSourceFeatures[iSource].Size;
                        YoonVector2N pSourcePos = pSourceFeatures[iSource].Pt.ToPoint().ToYoonVector();
                        YoonRect2N pSourceFigure = new YoonRect2N(pSourcePos.X, pSourcePos.Y, nSourceSize, nSourceSize);
                        pSourceDataset.Add(new YoonObject(nLabelNo, pSourceFigure, pSourcePos));
                        // Input the object data-set
                        int iObject = pMatchResult.TrainIdx;
                        int nObjectSize = (int) pObjectFeatures[iObject].Size;
                        YoonVector2N pObjectPos = pObjectFeatures[iObject].Pt.ToPoint().ToYoonVector();
                        YoonRect2N pObjectFigure = new YoonRect2N(pObjectPos.X, pObjectPos.Y, nObjectSize, nObjectSize);
                        pObjectDataset.Add(new YoonObject(nLabelNo, pObjectFigure, pObjectPos));
                        nLabelNo++;
                    }
                }

                // Release memories
                pSourceDescriptor.Dispose();
                pObjectDescriptor.Dispose();
                pMatcher.Dispose();
                pDetector.Dispose();
                GC.Collect(0, GCCollectionMode.Default);
                GC.WaitForFullGCComplete();
            }
        }

        public static class Transform
        {
            public static CVImage FlipX(CVImage pSourceImage) => new CVImage(FlipX(pSourceImage.Matrix));
            public static CVImage FlipY(CVImage pSourceImage) => new CVImage(FlipY(pSourceImage.Matrix));
            public static CVImage FlipXY(CVImage pSourceImage) => new CVImage(FlipXY(pSourceImage.Matrix));
            public static Mat FlipX(Mat pSourceMatrix) => pSourceMatrix.Flip(FlipMode.X);
            public static Mat FlipY(Mat pSourceMatrix) => pSourceMatrix.Flip(FlipMode.Y);
            public static Mat FlipXY(Mat pSourceMatrix) => pSourceMatrix.Flip(FlipMode.XY);

            public static CVImage Flip(CVImage pSourceImage, eYoonDir2DMode nMode)
            {
                switch (nMode)
                {
                    case eYoonDir2DMode.AxisX:
                        return FlipX(pSourceImage);
                    case eYoonDir2DMode.AxisY:
                        return FlipY(pSourceImage);
                    case eYoonDir2DMode.Fixed:
                        return pSourceImage.Clone() as CVImage;
                    default:
                        return FlipXY(pSourceImage);
                }
            }

            public static CVImage Resize(CVImage pSourceImage, double dRatio)
            {
                return new CVImage(Resize(pSourceImage.Matrix, (int) (dRatio * pSourceImage.Width),
                    (int) (dRatio * pSourceImage.Height)));
            }

            public static CVImage Resize(CVImage pSourceImage, int nDestWidth, int nDestHeight)
            {
                return new CVImage(Resize(pSourceImage.Matrix, nDestWidth, nDestHeight));
            }

            public static Mat Resize(Mat pSourceMatrix, int nDestWidth, int nDestHeight)
            {
                return pSourceMatrix.Resize(new OpenCvSharp.Size(nDestWidth, nDestHeight));
            }
        }
    }
}
