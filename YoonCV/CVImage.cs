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
        #region IDisposable Support
        private bool _disposedValue = false;

        protected override void Dispose(bool disposing)
        {
            if (!_disposedValue)
            {
                if (disposing)
                {
                    base.Bitmap.Dispose();
                    Matrix.Dispose();
                }
                base.Bitmap = null;
                Matrix = null;

                _disposedValue = true;
            }
        }

        ~CVImage()
        {
            Dispose(false);
            GC.SuppressFinalize(this);
        }
        #endregion

        public Mat Matrix { get; private set; } = null;

        public CVImage() : base()
        {
            Matrix = BitmapConverter.ToMat(base.Bitmap);
        }

        public CVImage(Mat pMatrix) : this()
        {
            Task pTask = Task.Factory.StartNew(() =>base.Bitmap = pMatrix.ToBitmap());
            Matrix = pMatrix.Clone();
            pTask.Wait();
        }

        public CVImage(YoonImage pImage)
        {
            Task pTask = Task.Factory.StartNew(() => Matrix = pImage.Bitmap.ToMat());
            Bitmap = pImage.CopyBitmap();
            pTask.Wait();
        }

        public override bool LoadImage(string strPath)
        {
            FilePath = strPath;
            if (!IsFileExist()) return false;
            Matrix = Cv2.ImRead(strPath);
            Bitmap = Matrix.ToBitmap();
            return true;
        }

        public bool LoadImage(string strPath, ImreadModes nMode)
        {
            FilePath = strPath;
            if (!IsFileExist()) return false;
            Matrix = Cv2.ImRead(strPath, nMode);
            Bitmap = Matrix.ToBitmap();
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
