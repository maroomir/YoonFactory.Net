using System;
using System.Drawing;
using System.Runtime.InteropServices;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using OpenCvSharp;
using YoonFactory.Files;
using Point = OpenCvSharp.Point;

namespace YoonFactory.CV
{
    public static class CVFactory
    {
        public static YoonObject FindTemplate(this CVImage pSourceImage, CVImage pTemplateImage, double dScore = 0.7) => TemplateMatch.FindTemplate(pTemplateImage, pSourceImage, dScore);
        public static CVImage Add(this CVImage pSourceImage, CVImage pObjectImage) => TwoImageProcess.Add(pSourceImage, pObjectImage);
        public static CVImage Subtract(this CVImage pSourceImage, CVImage pObjectImage) => TwoImageProcess.Subtract(pSourceImage, pObjectImage);
        public static CVImage Multiply(this CVImage pSourceImage, CVImage pObjectImage) => TwoImageProcess.Multiply(pSourceImage, pObjectImage);
        public static CVImage Divide(this CVImage pSourceImage, CVImage pObjectImage) => TwoImageProcess.Divide(pSourceImage, pObjectImage);
        public static CVImage Max(this CVImage pSourceImage, CVImage pObjectImage) => TwoImageProcess.Max(pSourceImage, pObjectImage);
        public static CVImage Min(this CVImage pSourceImage, CVImage pObjectImage) => TwoImageProcess.Min(pSourceImage, pObjectImage);
        public static CVImage AbsDiff(this CVImage pSourceImage, CVImage pObjectImage) => TwoImageProcess.AbsDiff(pSourceImage, pObjectImage);
        public static CVImage Sobel(this CVImage pSourceImage, int nOrderX = 1, int nOrderY = 0) => Filter.Sobel(pSourceImage, nOrderX, nOrderY);
        public static CVImage Scharr(this CVImage pSourceImage, int nOrderX = 1, int nOrderY = 0) => Filter.Scharr(pSourceImage, nOrderX, nOrderY);
        public static CVImage Laplacian(this CVImage pSourceImage) => Filter.Laplacian(pSourceImage);
        public static CVImage Gaussian(this CVImage pSourceImage, int nSizeX = 3, int nSizeY = 3) => Filter.Gaussian(pSourceImage, nSizeX, nSizeY);
        public static CVImage Canny(this CVImage pSourceImage, double dThresholdMin = 100, double dThresholdMax = 200) => Filter.Canny(pSourceImage, dThresholdMin, dThresholdMax);
        public static CVImage FillFlood(this CVImage pSourceImage, YoonVector2N pVector, byte nThreshold, bool bWhite) => Fill.FillFlood(pSourceImage, pVector, nThreshold, bWhite);
        public static CVImage FillFlood(this CVImage pSourceImage, YoonVector2N pVector, byte nThreshold, Color pFillColor) => Fill.FillFlood(pSourceImage, pVector, nThreshold, pFillColor);

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
                Rect pRectResult = FindTemplate(pTemplateImage.Matrix, pSourceImage.Matrix, out var dMatchScore, dScore, TemplateMatchModes.CCoeffNormed);
                return new YoonObject(0, pRectResult.ToYoonRect(), pTemplateImage.CropImage(pRectResult.ToYoonRect()), dMatchScore, (int)(dMatchScore * pRectResult.Width * pRectResult.Height));
            }

            public static Rect FindTemplate(Mat pTemplateMatrix, Mat pSourceMatrix, out double dMatchScore, double dThresholdScore, TemplateMatchModes nMode = TemplateMatchModes.CCoeffNormed)
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
                    throw new InvalidOperationException("[YOONCV EXCEPTION] Process Failed : Matching score is too low");
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

            public static Mat Add(Mat pSourceMatrix, Mat pObjectMatrix)
            {
                Mat pResultMatrix = new Mat();
                Cv2.Add(pSourceMatrix, pObjectMatrix, pResultMatrix);
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
                    pListSumImage.Add(pPyrLaplacianObject[i].SubMat(0, nObjectHeight, nObjectWidth / 2 + 1, nObjectWidth));
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

            public static CVImage FillFlood(CVImage pSourceImage, YoonVector2N pVector, byte nThreshold, Color pFillColor)
            {
                return new CVImage(FillFlood(pSourceImage.Matrix, pVector, nThreshold, pFillColor));
            }

            public static Mat FillFlood(Mat pSourceMatrix, YoonVector2N pVector, byte nThreshold, bool isWhite)
            {
                Mat pResultMatrix = pSourceMatrix.Clone();
                if (isWhite)
                    Cv2.FloodFill(pResultMatrix, pVector.ToCVPoint(), Scalar.All(255), out _, Scalar.All(nThreshold), Scalar.All(nThreshold), FloodFillFlags.Link8);
                else
                    Cv2.FloodFill(pResultMatrix, pVector.ToCVPoint(), Scalar.All(0), out _, Scalar.All(nThreshold), Scalar.All(nThreshold), FloodFillFlags.Link8);
                return pResultMatrix;
            }

            public static Mat FillFlood(Mat pSourceMatrix, YoonVector2N pVector, byte nThreshold, Color pFillColor)
            {
                Mat pResultMatrix = pSourceMatrix.Clone();
                Cv2.FloodFill(pResultMatrix, pVector.ToCVPoint(), pFillColor.ToScalar(), out _, Scalar.All(nThreshold), Scalar.All(nThreshold), FloodFillFlags.Link8);
                return pResultMatrix;
            }
        }

        public static class ColorDetection
        {
            public static CVImage DetectHSV(CVImage pSourceImage, byte nHue, byte nSaturation, byte nValue)
            {
                if(pSourceImage.Plane != 3)
                    throw new FormatException("[YOONIMAGE EXCEPTION] Image arguments is not 8bit format");
                return new CVImage(DetectHSV(pSourceImage.Matrix, nHue, nSaturation, nValue));
            }

            public static Mat DetectHSV(Mat pSourceMatrix, byte nHue, byte nSaturation, byte nValue, byte nThreshold = 10)
            {
                Mat pHsvMatrix = new Mat(pSourceMatrix.Size(), MatType.CV_8UC3);
                Mat pMaskMatrix = new Mat(pSourceMatrix.Size(), MatType.CV_8UC3);
                Mat pResultMatrix = new Mat(pSourceMatrix.Size(), MatType.CV_8UC3);
                Cv2.CvtColor(pSourceMatrix, pHsvMatrix, ColorConversionCodes.BGR2HSV);
                byte nHueLow = (byte)Math.Max(nHue - nThreshold / 2, 0);
                byte nHueHigh = (byte)Math.Min(nHue + nThreshold / 2, 255);
                byte nSaturationLow = (byte)Math.Max(nSaturation - nThreshold / 2, 0);
                byte nSaturationHigh = (byte)Math.Min(nSaturation + nThreshold / 2, 255);
                byte nValueLow = (byte)Math.Max(nValue - nThreshold / 2, 0);
                byte nValueHigh = (byte)Math.Min(nValue + nThreshold / 2, 255);
                Cv2.InRange(pHsvMatrix, new Scalar(nHueLow, nSaturationLow, nValueLow), new Scalar(nHueHigh, nSaturationHigh, nValueHigh), pMaskMatrix);
                Cv2.BitwiseAnd(pHsvMatrix, pHsvMatrix, pResultMatrix, pMaskMatrix);
                Cv2.CvtColor(pResultMatrix, pResultMatrix, ColorConversionCodes.HSV2BGR);
                return pResultMatrix;
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
                return new CVImage(Resize(pSourceImage.Matrix, (int)(dRatio * pSourceImage.Width), (int)(dRatio * pSourceImage.Height)));
            }

            public static CVImage Resize(CVImage pSourceImage, double dRatioX, double dRatioY)
            {
                return new CVImage(Resize(pSourceImage.Matrix, (int)(dRatioX * pSourceImage.Width), (int)(dRatioY * pSourceImage.Height)));
            }

            public static Mat Resize(Mat pSourceMatrix, int nDestWidth, int nDestHeight)
            {
                return pSourceMatrix.Resize(new OpenCvSharp.Size(nDestWidth, nDestHeight));
            }
        }
    }
}
