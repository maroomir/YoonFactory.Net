using YoonFactory.Image;
using System;
using System.Threading.Tasks;
using OpenCvSharp;
using OpenCvSharp.Extensions;
using YoonFactory.Files;

namespace YoonFactory.CV
{
    public class CVImage : YoonImage
    {
        public Mat Matrix => Bitmap.ToMat();

        public CVImage() : base()
        {
            //
        }

        public CVImage(Mat pMatrix) : this()
        {
            Bitmap = pMatrix.ToBitmap();
        }

        public CVImage(YoonImage pImage)
        {
            FilePath = pImage.FilePath;
            Bitmap = pImage.CopyBitmap();
        }

        public override bool LoadImage(string strPath)
        {
            FilePath = strPath;
            if (!IsFileExist()) return false;
            Bitmap = Cv2.ImRead(strPath).ToBitmap();
            return true;
        }

        public bool LoadImage(string strPath, ImreadModes nMode)
        {
            FilePath = strPath;
            if (!IsFileExist()) return false;
            Bitmap = Cv2.ImRead(strPath, nMode).ToBitmap();
            return true;
        }

        public override bool SaveImage(string strPath)
        {
            FilePath = strPath;
            return Matrix.SaveImage(FilePath);
        }

        public Mat CopyMatrix()
        {
            return Matrix.Clone();
        }

        public override IYoonFile Clone()
        {
            CVImage pImage = new CVImage(Matrix) {FilePath = FilePath};
            return pImage;
        }

        public override YoonImage ToGrayImage()
        {
            return new CVImage(Matrix.CvtColor(ColorConversionCodes.BGR2GRAY));
        }

        public override YoonImage ToRGBImage()
        {
            return new CVImage(Matrix.CvtColor(ColorConversionCodes.GRAY2BGR));
        }

        public override YoonImage CropImage(YoonRect2N cropArea)
        {
            return new CVImage(Matrix.SubMat(cropArea.ToCVRect()));
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
            if (Plane != 3)
                throw new FormatException("[YOONIMAGE ERROR] Bitmap format is not comportable");

            Mat pHistogramMatrix = new Mat();
            Mat pResultMatrix = Mat.Ones(new Size(256, Height), MatType.CV_8UC1);
            Cv2.CalcHist(new Mat[] { Matrix }, new int[] { nChannel }, null, pHistogramMatrix, 1, new int[] { 256 },
                new Rangef[] { new Rangef(0, 256) });
            Cv2.Normalize(pHistogramMatrix, pHistogramMatrix, 0, 255, NormTypes.MinMax);
            for (int i = 0; i < pHistogramMatrix.Rows; i++)
            {
                Cv2.Line(pResultMatrix, new Point(i, Matrix.Height), new Point(i, Matrix.Height - pHistogramMatrix.Get<float>(i)),
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
            Mat pMatSource = (ToGrayImage() as CVImage).Matrix;
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
