using YoonFactory.Image;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using OpenCvSharp;
using OpenCvSharp.Extensions;
using YoonFactory.Files;
using Point = OpenCvSharp.Point;
using Size = OpenCvSharp.Size;

namespace YoonFactory.CV
{
    public class CVImage : YoonImage
    {
        public CVImage() : base()
        {
            //
        }

        public Mat Matrix => Bitmap.ToMat();

        public CVImage(Mat pMatrix) : this()
        {
            Bitmap = pMatrix.ToBitmap();
        }

        public CVImage(YoonImage pImage)
        {
            FilePath = pImage.FilePath;
            Bitmap = pImage.CopyBitmap();
        }

        public new static List<CVImage> LoadImages(string strRoot)
        {
            if (!FileFactory.VerifyDirectory(strRoot)) return null;
            List<CVImage> pListImage = new List<CVImage>();
            foreach (string strFilePath in FileFactory.GetExtensionFilePaths(strRoot, ".bmp", ".jpg", ".jpeg", ".png"))
            {
                CVImage pImage = new CVImage();
                if (pImage.LoadImage(strFilePath))
                    pListImage.Add(pImage);
            }

            return pListImage;
        }

        public override bool LoadImage(string strPath)
        {
            FilePath = strPath;
            if (!IsFileExist()) return false;
            Bitmap = Cv2.ImRead(strPath).ToBitmap();
            if (Bitmap.PixelFormat == PixelFormat.Format24bppRgb)
            {
                Bitmap pBitmap = Bitmap.Clone(new Rectangle(0, 0, Bitmap.Width, Bitmap.Height),
                    PixelFormat.Format32bppArgb);
                Bitmap = pBitmap;
            }

            return true;
        }

        public bool LoadImage(string strPath, ImreadModes nMode)
        {
            FilePath = strPath;
            if (!IsFileExist()) return false;
            Bitmap = Cv2.ImRead(strPath, nMode).ToBitmap();
            if (Bitmap.PixelFormat == PixelFormat.Format24bppRgb)
            {
                Bitmap pBitmap = Bitmap.Clone(new Rectangle(0, 0, Bitmap.Width, Bitmap.Height),
                    PixelFormat.Format32bppArgb);
                Bitmap = pBitmap;
            }

            return true;
        }

        public override bool SaveImage(string strPath)
        {
            FilePath = strPath;
            try
            {
                Cv2.ImWrite(strPath, Matrix);
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }

            return false;
        }

        public override CVImage ToARGBImage(bool bAlphaMax = true)
        {
            switch (Matrix.Channels())
            {
                case 1:
                    return new CVImage(Matrix.CvtColor(ColorConversionCodes.GRAY2BGR)
                        .CvtColor(ColorConversionCodes.BGR2BGRA));
                case 3:
                    return new CVImage(Matrix.CvtColor(ColorConversionCodes.BGR2BGRA));
                case 4:
                    return this;
                default:
                    throw new FormatException("[YOONCV] Image Dimension is abnormal");
            }
        }

        public override YoonImage ToGrayImage()
        {
            switch (Matrix.Channels())
            {
                case 1:
                    return this;
                case 3:
                    return new CVImage(Matrix.CvtColor(ColorConversionCodes.BGR2GRAY));
                case 4:
                    return new CVImage(Matrix.CvtColor(ColorConversionCodes.BGRA2GRAY));
                default:
                    throw new FormatException("[YOONCV] Image Dimension is abnormal");
            }
        }

        public Mat CopyMatrix()
        {
            return Matrix.Clone();
        }

        public static void ShowImage(YoonImage pImage, string strTitle)
        {
            CVImage pCvImage = new CVImage(pImage);
            pCvImage.ShowImage(strTitle);
        }

        public void ShowImage(string strTitle)
        {
            Cv2.NamedWindow(strTitle, WindowFlags.AutoSize);
            Cv2.SetWindowProperty(strTitle, WindowPropertyFlags.Fullscreen, 0);
            Cv2.ImShow(strTitle, Matrix);
            Cv2.WaitKey(0);
            Cv2.DestroyAllWindows();
        }

        public void ShowHistogram(string strTitle, int nChannel)  // B : 0,  G : 1,  R : 2
        {
            if (Channel != 3)
                throw new FormatException("[YOONIMAGE ERROR] Bitmap format is not comportable");

            Mat pMatrix = Matrix;
            Mat pHistogramMatrix = new Mat();
            Mat pResultMatrix = Mat.Ones(new Size(256, Height), MatType.CV_8UC1);
            Cv2.CalcHist(new Mat[] { pMatrix }, new int[] { nChannel }, null, pHistogramMatrix, 1, new int[] { 256 },
                new Rangef[] { new Rangef(0, 256) });
            Cv2.Normalize(pHistogramMatrix, pHistogramMatrix, 0, 255, NormTypes.MinMax);
            for (int i = 0; i < pHistogramMatrix.Rows; i++)
            {
                Cv2.Line(pResultMatrix, new Point(i, pMatrix.Height), new Point(i, pMatrix.Height - pHistogramMatrix.Get<float>(i)),
                    Scalar.White);
            }
            Cv2.ImShow(strTitle, pResultMatrix);
            Cv2.WaitKey(0);
            Cv2.DestroyAllWindows();
        }

        public void ShowHistogram(string strTitle)
        {
            Mat pMatHistogram = new Mat();
            Mat pMatResult = Mat.Ones(new Size(256, Height), MatType.CV_8UC1);
            Mat pMatSource = ToGrayImage().Bitmap.ToMat();
            Cv2.CalcHist(new Mat[] { pMatSource }, new int[] { 0 }, null, pMatHistogram, 1, new int[] { 256 },
                new Rangef[] { new Rangef(0, 256) });
            Cv2.Normalize(pMatHistogram, pMatHistogram, 0, 255, NormTypes.MinMax);
            for (int i = 0; i < pMatHistogram.Rows; i++)
            {
                Cv2.Line(pMatResult, new Point(i, pMatSource.Height), new Point(i, pMatSource.Height - pMatHistogram.Get<float>(i)),
                    Scalar.White);
            }
            Cv2.ImShow(strTitle, pMatResult);
            Cv2.WaitKey(0);
            Cv2.DestroyAllWindows();
        }

        public static CVImage operator +(CVImage i1, CVImage i2)
        {
            return CVFactory.TwoImageProcess.Add(i1, i2);
        }

        public static CVImage operator -(CVImage i1, CVImage i2)
        {
            return CVFactory.TwoImageProcess.Subtract(i1, i2);
        }

        public static CVImage operator *(CVImage i1, CVImage i2)
        {
            return CVFactory.TwoImageProcess.Multiply(i1, i2);
        }

        public static CVImage operator /(CVImage i1, CVImage i2)
        {
            return CVFactory.TwoImageProcess.Divide(i1, i2);
        }
    }
}
