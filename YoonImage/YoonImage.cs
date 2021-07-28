using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using YoonFactory.Files;
using Color = System.Drawing.Color;
using FileFactory = YoonFactory.Files.FileFactory;
using Point = System.Drawing.Point;
using Rectangle = System.Drawing.Rectangle;

namespace YoonFactory.Image
{
    public class YoonImage : IYoonFile, IEquatable<YoonImage>
    {
        #region IDisposable Support

        private bool _disposedValue = false;

        protected virtual void Dispose(bool disposing)
        {
            if (!_disposedValue)
            {
                if (disposing)
                {
                    Bitmap.Dispose();
                }

                Bitmap = null;
                _disposedValue = true;
            }
        }

        ~YoonImage()
        {
            Dispose(false);
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        #endregion

        public Bitmap Bitmap { get; protected set; } = null;
        protected const int DEFAULT_WIDTH = 640;
        protected const int DEFAULT_HEIGHT = 480;

        public string FilePath { get; set; }

        public PixelFormat Format => Bitmap.PixelFormat;

        public int Plane
        {
            get
            {
                return Bitmap.PixelFormat switch
                {
                    PixelFormat.Format8bppIndexed => 1,
                    PixelFormat.Format16bppArgb1555 => 2,
                    PixelFormat.Format16bppGrayScale => 2,
                    PixelFormat.Format16bppRgb555 => 2,
                    PixelFormat.Format16bppRgb565 => 2,
                    PixelFormat.Format1bppIndexed => 2,
                    PixelFormat.Format24bppRgb => 3,
                    PixelFormat.Format32bppArgb => 4,
                    PixelFormat.Format32bppPArgb => 4,
                    PixelFormat.Format32bppRgb => 4,
                    _ => 0
                };
            }
        }

        private PixelFormat GetDefaultFormat(int nPlane)
        {
            return nPlane switch
            {
                1 => PixelFormat.Format8bppIndexed,
                2 => PixelFormat.Format16bppGrayScale,
                3 => PixelFormat.Format24bppRgb,
                4 => PixelFormat.Format32bppArgb,
                _ => PixelFormat.DontCare
            };
        }

        public int Stride => Plane * Bitmap.Width;

        public int Width => Bitmap.Width;

        public int Height => Bitmap.Height;

        public YoonVector2N CenterPos => new YoonVector2N(Width / 2, Height / 2);

        public YoonRect2N Area => new YoonRect2N(CenterPos, Width, Height);

        public YoonImage()
            : this(DEFAULT_WIDTH, DEFAULT_HEIGHT, PixelFormat.Format8bppIndexed)
        {
        }

        public YoonImage(string strImagePath)
        {
            LoadImage(strImagePath);
        }

        public YoonImage(int nWidth, int nHeight, int nPlane)
        {
            if (nWidth <= 0 || nHeight <= 0)
            {
                nWidth = DEFAULT_WIDTH;
                nHeight = DEFAULT_HEIGHT;
            }

            Bitmap = new Bitmap(nWidth, nHeight, GetDefaultFormat(nPlane));
            ColorPalette pPallete = Bitmap.Palette;
            switch (Bitmap.PixelFormat)
            {
                case PixelFormat.Format8bppIndexed:
                    Parallel.For(0, 256, i => { pPallete.Entries[i] = Color.FromArgb(i, i, i); });
                    Bitmap.Palette = pPallete;
                    break;
                default:
                    break;
            }
        }

        public YoonImage(int nWidth, int nHeight, PixelFormat nFormat)
        {
            if (nWidth <= 0 || nHeight <= 0)
            {
                nWidth = DEFAULT_WIDTH;
                nHeight = DEFAULT_HEIGHT;
            }

            Bitmap = new Bitmap(nWidth, nHeight, nFormat);
            ColorPalette pPallete = Bitmap.Palette;
            switch (Bitmap.PixelFormat)
            {
                case PixelFormat.Format8bppIndexed:
                    Parallel.For(0, 256, i => { pPallete.Entries[i] = Color.FromArgb(i, i, i); });
                    Bitmap.Palette = pPallete;
                    break;
                default:
                    break;
            }
        }

        public YoonImage(IntPtr ptrAddress, int nWidth, int nHeight, PixelFormat nFormat,
            eYoonRGBMode nMode = eYoonRGBMode.None)
            : this(nWidth, nHeight, nFormat)
        {
            switch (nFormat)
            {
                case PixelFormat.Format8bppIndexed:
                    SetGrayImage(ptrAddress);
                    break;
                case PixelFormat.Format24bppRgb:
                    switch (nMode)
                    {
                        case eYoonRGBMode.None:
                            SetRGBImage(ptrAddress);
                            break;
                        case eYoonRGBMode.Mixed:
                            SetRGBImageWithColorMixed(ptrAddress);
                            break;
                        case eYoonRGBMode.Parallel:
                            SetRGBImageWithColorParallel(ptrAddress);
                            break;
                        default:
                            break;
                    }

                    break;
                case PixelFormat.Format32bppArgb:
                    switch (nMode)
                    {
                        case eYoonRGBMode.None:
                            SetARGBImage(ptrAddress);
                            break;
                        case eYoonRGBMode.Mixed:
                            SetARGBImageWithColorMixed(ptrAddress);
                            break;
                        case eYoonRGBMode.Parallel:
                            SetARGBImageWithColorParallel(ptrAddress);
                            break;
                        default:
                            break;
                    }

                    break;
                default:
                    break;
            }
        }

        public YoonImage(byte[] pBuffer, int nWidth, int nHeight, PixelFormat nFormat,
            eYoonRGBMode nMode = eYoonRGBMode.None)
            : this(nWidth, nHeight, nFormat)
        {
            switch (nFormat)
            {
                case PixelFormat.Format8bppIndexed:
                    SetGrayImage(pBuffer);
                    break;
                case PixelFormat.Format24bppRgb:
                    switch (nMode)
                    {
                        case eYoonRGBMode.None:
                            SetRGBImage(pBuffer);
                            break;
                        case eYoonRGBMode.Mixed:
                            SetRGBImageWithColorMixed(pBuffer);
                            break;
                        case eYoonRGBMode.Parallel:
                            SetRGBImageWithColorParallel(pBuffer);
                            break;
                        default:
                            break;
                    }

                    break;
                case PixelFormat.Format32bppArgb:
                    switch (nMode)
                    {
                        case eYoonRGBMode.Mixed:
                            SetARGBImageWithColorMixed(pBuffer);
                            break;
                        case eYoonRGBMode.Parallel:
                            SetARGBImageWithColorParallel(pBuffer);
                            break;
                        default:
                            break;
                    }

                    break;
                default:
                    break;
            }
        }

        public YoonImage(int[] pBuffer, int nWidth, int nHeight, PixelFormat nFormat)
            : this(nWidth, nHeight, nFormat)
        {
            switch (nFormat)
            {
                case PixelFormat.Format32bppArgb:
                    SetARGBImage(pBuffer);
                    break;
                default:
                    break;
            }
        }

        public YoonImage(IntPtr ptrAddress, int nWidth, int nHeight, int nPlane, eYoonRGBMode nMode = eYoonRGBMode.None)
            : this(nWidth, nHeight, nPlane)
        {
            switch (nPlane)
            {
                case 1:
                    SetGrayImage(ptrAddress);
                    break;
                case 3:
                    switch (nMode)
                    {
                        case eYoonRGBMode.None:
                            SetRGBImage(ptrAddress);
                            break;
                        case eYoonRGBMode.Mixed:
                            SetRGBImageWithColorMixed(ptrAddress);
                            break;
                        case eYoonRGBMode.Parallel:
                            SetRGBImageWithColorParallel(ptrAddress);
                            break;
                        default:
                            break;
                    }

                    break;
                case 4:
                    switch (nMode)
                    {
                        case eYoonRGBMode.None:
                            SetARGBImage(ptrAddress);
                            break;
                        case eYoonRGBMode.Mixed:
                            SetARGBImageWithColorMixed(ptrAddress);
                            break;
                        case eYoonRGBMode.Parallel:
                            SetARGBImageWithColorParallel(ptrAddress);
                            break;
                        default:
                            break;
                    }

                    break;
                default:
                    break;
            }
        }

        public YoonImage(byte[] pBuffer, int nWidth, int nHeight, int nPlane, eYoonRGBMode nMode = eYoonRGBMode.None)
            : this(nWidth, nHeight, nPlane)
        {
            switch (nPlane)
            {
                case 1:
                    SetGrayImage(pBuffer);
                    break;
                case 3:
                    switch (nMode)
                    {
                        case eYoonRGBMode.None:
                            SetRGBImage(pBuffer);
                            break;
                        case eYoonRGBMode.Mixed:
                            SetRGBImageWithColorMixed(pBuffer);
                            break;
                        case eYoonRGBMode.Parallel:
                            SetRGBImageWithColorParallel(pBuffer);
                            break;
                        default:
                            break;
                    }

                    break;
                case 4:
                    switch (nMode)
                    {
                        case eYoonRGBMode.Mixed:
                            SetARGBImageWithColorMixed(pBuffer);
                            break;
                        case eYoonRGBMode.Parallel:
                            SetARGBImageWithColorParallel(pBuffer);
                            break;
                        default:
                            break;
                    }

                    break;
                default:
                    break;
            }
        }

        public YoonImage(int[] pBuffer, int nWidth, int nHeight, int nPlane)
            : this(nWidth, nHeight, nPlane)
        {
            switch (nPlane)
            {
                case 4:
                    SetARGBImage(pBuffer);
                    break;
                default:
                    break;
            }
        }

        public YoonImage(Bitmap pBitmap)
        {
            Debug.Assert(pBitmap != null, nameof(pBitmap) + " != null");
            Bitmap = (Bitmap) pBitmap.Clone();
        }

        public void CopyFrom(IYoonFile pFile)
        {
            if (pFile is YoonImage pImage)
            {
                FilePath = pImage.FilePath;
                Bitmap = pImage.CopyBitmap();
            }
        }

        public virtual IYoonFile Clone()
        {
            return new YoonImage(Bitmap) {FilePath = FilePath};
        }

        public override bool Equals(object obj)
        {
            return Equals(obj as YoonImage);
        }

        public bool IsFileExist()
        {
            return FileFactory.VerifyFilePath(FilePath, false);
        }

        public bool LoadFile()
        {
            return LoadImage(FilePath);
        }

        public bool SaveFile()
        {
            return SaveImage(FilePath);
        }

        public void SetVerifiedArea(ref YoonRect2N pArea)
        {
            if (pArea.Left < 0)
                pArea.CenterPos.X = Bitmap.Width / 2;
            if (pArea.Top < 0)
                pArea.CenterPos.Y = Bitmap.Height / 2;
            if (pArea.Right > Bitmap.Width)
                pArea.Width = Bitmap.Width - pArea.Left;
            if (pArea.Bottom > Bitmap.Height)
                pArea.Height = Bitmap.Height - pArea.Top;
        }

        public bool IsVerifiedArea(YoonRect2N pArea)
        {
            return (pArea.Left >= 0 && pArea.Top >= 0 && pArea.Right <= Bitmap.Width && pArea.Bottom <= Bitmap.Height);
        }

        public static List<YoonImage> LoadImages(string strRoot)
        {
            if (!FileFactory.VerifyDirectory(strRoot)) return null;
            List<YoonImage> pListImage = new List<YoonImage>();
            foreach (string strFilePath in FileFactory.GetExtensionFilePaths(strRoot, ".bmp", ".jpg", ".png"))
                pListImage.Add(new YoonImage(strFilePath));
            return pListImage;
        }

        public static int SaveImages(string strRoot, List<YoonImage> pListImage)
        {
            if (FileFactory.VerifyDirectory(strRoot)) return -1;
            int nCountSave = 0;
            for (int iCount = 0; iCount < pListImage.Count; iCount++)
                if (pListImage[iCount]?.SaveImage(Path.Combine(strRoot, $"{iCount}.bmp")) == true)
                    nCountSave++;

            return nCountSave;
        }

        public virtual bool LoadImage(string strPath)
        {
            FilePath = strPath;
            if (!IsFileExist()) return false;
            try
            {
                if (FileFactory.VerifyFileExtensions(strPath, ".bmp", ".jpg", ".png"))
                {
                    Bitmap = (Bitmap) System.Drawing.Image.FromFile(FilePath);
                    return true;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }

            return false;
        }

        public virtual bool SaveImage(string strPath)
        {
            try
            {
                if (FileFactory.VerifyFileExtensions(strPath, ".bmp", ".jpg", ".png"))
                {
                    FilePath = strPath;
                    Bitmap.Save(strPath);
                    return true;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }

            return false;
        }
        
        public Bitmap CopyBitmap()
        {
            if (Bitmap.Width <= 0 || Bitmap.Height <= 0)
                throw new IndexOutOfRangeException("[YOONIMAGE ERROR] Bitmap size is not normal");

            YoonImage pResult = new YoonImage(Bitmap.Width, Bitmap.Height, Bitmap.PixelFormat);
            {
                switch (Plane)
                {
                    case 1:
                        pResult.SetGrayImage(this.GetGrayBuffer());
                        break;
                    case 3:
                        pResult.SetRGBImage(this.GetRGBBuffer());
                        break;
                    case 4:
                        pResult.SetARGBImage(this.GetARGBBuffer());
                        break;
                    default:
                        break;
                }
            }
            return pResult.Bitmap;
        }

        public virtual YoonImage CropImage(YoonRect2N pCropArea)
        {
            YoonImage pImageResult = new YoonImage(pCropArea.Width, pCropArea.Height, PixelFormat.Format8bppIndexed);
            for (int iY = 0; iY < pCropArea.Height; iY++)
            {
                int nY = pCropArea.Top + iY;
                if (nY >= Bitmap.Height) continue;
                byte[] pByte = new byte[pCropArea.Width];
                for (int iX = 0; iX < pCropArea.Width; iX++)
                {
                    int nX = pCropArea.Left + iX;
                    if (nX >= Bitmap.Width) continue;
                    pByte[iX] = Math.Max((byte) 0, Math.Min(GetGrayPixel(iX, iY), (byte) 255));
                }

                pImageResult.SetGrayLine(pByte, iY);
            }

            return pImageResult;
        }

        public virtual YoonImage ToGrayImage()
        {
            if (Plane == 1)
                return this;
            byte[] pByte = new byte[Width * Height];
            switch (Format)
            {
                case PixelFormat.Format24bppRgb:
                    byte[,] pBufferRGB = Scan24bitPlaneBuffer(new Rectangle(Point.Empty, Bitmap.Size));
                    for (int j = 0; j < Bitmap.Height; j++)
                    {
                        for (int i = 0; i < Bitmap.Width; i++)
                        {
                            // ITU-RBT.709, YPrPb
                            pByte[j * Bitmap.Width + i] = (byte) (0.299f * pBufferRGB[j * Bitmap.Width + i, 0] +
                                                                  0.587f * pBufferRGB[j * Bitmap.Width + i, 1] +
                                                                  0.114f * pBufferRGB[j * Bitmap.Width + i, 2]); 
                        }
                    }

                    break;
                case PixelFormat.Format32bppArgb:
                    byte[,] pBufferARGB = Scan32bitPlaneBuffer(new Rectangle(Point.Empty, Bitmap.Size));
                    for (int j = 0; j < Bitmap.Height; j++)
                    {
                        for (int i = 0; i < Bitmap.Width; i++)
                        { 
                            // ITU-RBT.709, YPrPb
                            pByte[j * Bitmap.Width + i] = (byte) (0.299f * pBufferARGB[j * Bitmap.Width + i, 1] +
                                                                  0.587f * pBufferARGB[j * Bitmap.Width + i, 2] +
                                                                  0.114f * pBufferARGB[j * Bitmap.Width + i, 3]);
                        }
                    }

                    break;
                default:
                    throw new FormatException("[YOONIMAGE ERROR] Bitmap format is not comportable");
            }

            return new YoonImage(pByte, Width, Height, PixelFormat.Format8bppIndexed);
        }

        public virtual YoonImage ToRGBImage()
        {
            if (Plane != 1)
                throw new FormatException("[YOONIMAGE ERROR] Bitmap format is not comportable");
            byte[] pBuffer = GetGrayBuffer();
            YoonImage pImageResult = new YoonImage(Width, Height, PixelFormat.Format24bppRgb);
            return pImageResult.SetRGBImageWithPlane(pBuffer, pBuffer, pBuffer)
                ? pImageResult
                : new YoonImage(Width, Height, PixelFormat.Format24bppRgb);
        }
        
        public virtual YoonImage ToARGBImage()
        {
            if (Plane != 1)
                throw new FormatException("[YOONIMAGE ERROR] Bitmap format is not comportable");
            byte[] pBuffer = GetGrayBuffer();
            YoonImage pImageResult = new YoonImage(Width, Height, PixelFormat.Format32bppArgb);
            return pImageResult.SetARGBImageWithPlane(pBuffer, pBuffer, pBuffer)
                ? pImageResult
                : new YoonImage(Width, Height, PixelFormat.Format32bppArgb);
        }

        private async Task ConvertGrey()
        {
            await Task.Run(() =>
            {
                if (Plane == 1) return;
                byte[] pByte = new byte[Width * Height];
                switch (Format)
                {
                    case PixelFormat.Format24bppRgb:
                        byte[,] pBufferRGB = Scan24bitPlaneBuffer(new Rectangle(Point.Empty, Bitmap.Size));
                        for (int j = 0; j < Bitmap.Height; j++)
                        {
                            for (int i = 0; i < Bitmap.Width; i++)
                            {
                                // ITU-RBT.709, YPrPb
                                pByte[j * Bitmap.Width + i] = (byte) (0.299f * pBufferRGB[j * Bitmap.Width + i, 0] +
                                                                      0.587f * pBufferRGB[j * Bitmap.Width + i, 1] +
                                                                      0.114f * pBufferRGB[j * Bitmap.Width + i, 2]);
                            }
                        }

                        break;
                    case PixelFormat.Format32bppArgb:
                        byte[,] pBufferARGB = Scan32bitPlaneBuffer(new Rectangle(Point.Empty, Bitmap.Size));
                        for (int j = 0; j < Bitmap.Height; j++)
                        {
                            for (int i = 0; i < Bitmap.Width; i++)
                            {
                                // ITU-RBT.709, YPrPb
                                pByte[j * Bitmap.Width + i] = (byte) (0.299f * pBufferARGB[j * Bitmap.Width + i, 1] +
                                                                      0.587f * pBufferARGB[j * Bitmap.Width + i, 2] +
                                                                      0.114f * pBufferARGB[j * Bitmap.Width + i, 3]);
                            }
                        }

                        break;
                    default:
                        throw new FormatException("[YOONIMAGE ERROR] Bitmap format is not comportable");
                }

                Bitmap = new Bitmap(Width, Height, PixelFormat.Format8bppIndexed);
                SetGrayImage(pByte);
            });
        }

        private async Task ConvertRGB()
        {
            await Task.Run(() =>
            {
                if (Plane != 1) return;
                byte[] pBuffer = GetGrayBuffer();
                Bitmap = new Bitmap(Width, Height, PixelFormat.Format24bppRgb);
                SetRGBImageWithPlane(pBuffer, pBuffer, pBuffer);
            });
        }

        private async Task ConvertARGB()
        {
            await Task.Run(() =>
            {
                if (Plane != 1) return;
                byte[] pBuffer = GetGrayBuffer();
                Bitmap = new Bitmap(Width, Height, PixelFormat.Format32bppArgb);
                SetARGBImageWithPlane(pBuffer, pBuffer, pBuffer);
            });
        }

        public void FillTriangle(int nX, int nY, int nSize, eYoonDir2D nDir, Color pColor, double dZoom)
        {
            Task pTaskConvert = ConvertARGB();

            PointF[] pPoint = new PointF[3];
            pPoint[0].X = (float) (nX * dZoom);
            pPoint[0].Y = (float) (nY * dZoom);
            switch (nDir)
            {
                case eYoonDir2D.Top:
                    pPoint[1].X = pPoint[0].X + (float) (nSize / 2 * dZoom);
                    pPoint[1].Y = pPoint[0].Y + (float) (nSize * dZoom);
                    pPoint[2].X = pPoint[0].X - (float) (nSize / 2 * dZoom);
                    pPoint[2].Y = pPoint[0].Y + (float) (nSize * dZoom);
                    break;
                case eYoonDir2D.Bottom:
                    pPoint[1].X = pPoint[0].X - (float) (nSize / 2 * dZoom);
                    pPoint[1].Y = pPoint[0].Y - (float) (nSize * dZoom);
                    pPoint[2].X = pPoint[0].X + (float) (nSize / 2 * dZoom);
                    pPoint[2].Y = pPoint[0].Y - (float) (nSize * dZoom);
                    break;
                case eYoonDir2D.Left:
                    pPoint[1].X = pPoint[0].X + (float) (nSize * dZoom);
                    pPoint[1].Y = pPoint[0].Y - (float) (nSize / 2 * dZoom);
                    pPoint[2].X = pPoint[0].X + (float) (nSize * dZoom);
                    pPoint[2].Y = pPoint[0].Y + (float) (nSize / 2 * dZoom);
                    break;
                case eYoonDir2D.Right:
                    pPoint[1].X = pPoint[0].X - (float) (nSize * dZoom);
                    pPoint[1].Y = pPoint[0].Y + (float) (nSize / 2 * dZoom);
                    pPoint[2].X = pPoint[0].X - (float) (nSize * dZoom);
                    pPoint[2].Y = pPoint[0].Y - (float) (nSize / 2 * dZoom);
                    break;
            }

            pTaskConvert.Wait();
            using (Graphics graph = Graphics.FromImage(Bitmap))
            {
                SolidBrush brush = new SolidBrush(pColor);
                graph.FillPolygon(brush, pPoint);
            }
        }

        public void FillRect(YoonRect2N pRect, Color pColor, double dRatio = 1.0)
        {
            Task pTaskConvert = ConvertARGB();
            
            float startX = (float) (pRect.CenterPos.X - pRect.Width / 2) * (float) dRatio;
            float startY = (float) (pRect.CenterPos.Y - pRect.Height / 2) * (float) dRatio;
            pTaskConvert.Wait();
            using (Graphics graph = Graphics.FromImage(Bitmap))
            {
                SolidBrush brush = new SolidBrush(pColor);
                graph.FillRectangle(brush, startX, startY, (float) pRect.Width, (float) pRect.Height);
            }
        }

        public void FillRect(YoonRect2D pRect, Color pColor, double dRatio = 1.0)
        {
            Task pTaskConvert = ConvertARGB();
            
            float startX = (float) (pRect.CenterPos.X - pRect.Width / 2) * (float) dRatio;
            float startY = (float) (pRect.CenterPos.Y - pRect.Height / 2) * (float) dRatio;
            pTaskConvert.Wait();
            using (Graphics graph = Graphics.FromImage(Bitmap))
            {
                SolidBrush brush = new SolidBrush(pColor);
                graph.FillRectangle(brush, startX, startY, (float) pRect.Width, (float) pRect.Height);
            }
        }

        public void FillRect(int nCenterX, int nCenterY, int nWidth, int nHeight, Color pColor, double dZoom)
        {
            Task pTaskConvert = ConvertARGB();
            
            float startX = (float) (nCenterX - nWidth / 2) * (float) dZoom;
            float startY = (float) (nCenterY - nHeight / 2) * (float) dZoom;
            pTaskConvert.Wait();
            using (Graphics graph = Graphics.FromImage(Bitmap))
            {
                SolidBrush brush = new SolidBrush(pColor);
                graph.FillRectangle(brush, startX, startY, (float) nWidth, (float) nHeight);
            }
        }

        public void FillPolygon(YoonVector2N[] pArrayPoint, Color pColor, double dRatio = 1.0)
        {
            Task pTaskConvert = ConvertARGB();
            
            PointF[] pArrayDraw = new PointF[pArrayPoint.Length];
            for (int iPoint = 0; iPoint < pArrayPoint.Length; iPoint++)
            {
                pArrayDraw[iPoint].X = (float) pArrayPoint[iPoint].X * (float) dRatio;
                pArrayDraw[iPoint].Y = (float) pArrayPoint[iPoint].Y * (float) dRatio;
            }

            pTaskConvert.Wait();

            using (Graphics graph = Graphics.FromImage(Bitmap))
            {
                SolidBrush brush = new SolidBrush(pColor);
                graph.FillPolygon(brush, pArrayDraw);
            }
        }

        public void FillCanvas(Color pColor)
        {
            Task pTaskConvert = ConvertARGB();
            
            Rectangle pRectCanvas = new Rectangle(0, 0, Bitmap.Width, Bitmap.Height);
            pTaskConvert.Wait();
            using (Graphics graph = Graphics.FromImage(Bitmap))
            {
                SolidBrush brush = new SolidBrush(pColor);
                Region pRegion = new Region(pRectCanvas);
                graph.FillRegion(brush, pRegion);
            }
        }

        public void DrawFigure(IYoonFigure pFigure, Color pColor, int nPenWidth = 1, double dZoom = 1.0)
        {
            switch (pFigure)
            {
                case YoonRect2N pRect2N:
                    DrawRect(pRect2N, pColor, nPenWidth, dZoom);
                    break;
                case YoonRect2D pRect2D:
                    DrawRect(pRect2D.ToRect2N(), pColor, nPenWidth, dZoom);
                    break;
                case YoonLine2N pLine2N:
                    DrawLine(pLine2N, pColor, nPenWidth, dZoom);
                    break;
                case YoonLine2D pLine2D:
                    DrawLine(pLine2D.ToLine2N(), pColor, nPenWidth, dZoom);
                    break;
                case YoonRectAffine2D pRectAffine2D:
                    DrawPolygon(pColor, nPenWidth, dZoom,
                        (YoonVector2D) pRectAffine2D.GetPosition(eYoonDir2D.TopLeft),
                        (YoonVector2D) pRectAffine2D.GetPosition(eYoonDir2D.TopRight),
                        (YoonVector2D) pRectAffine2D.GetPosition(eYoonDir2D.BottomRight),
                        (YoonVector2D) pRectAffine2D.GetPosition(eYoonDir2D.BottomLeft));
                    break;
            }
        }

        public void DrawTriangle(int nX, int nY, int nSize, eYoonDir2D nDir, Color pColor, int nPenWidth = 1, double dZoom = 1.0)
        {
            Task pTaskConvert = ConvertARGB();
            
            PointF[] pIYoonVector = new PointF[3];
            pIYoonVector[0].X = (float) (nX * dZoom);
            pIYoonVector[0].Y = (float) (nY * dZoom);
            switch (nDir)
            {
                case eYoonDir2D.Top:
                    pIYoonVector[1].X = pIYoonVector[0].X + (float) (nSize / 2 * dZoom);
                    pIYoonVector[1].Y = pIYoonVector[0].Y + (float) (nSize * dZoom);
                    pIYoonVector[2].X = pIYoonVector[0].X - (float) (nSize / 2 * dZoom);
                    pIYoonVector[2].Y = pIYoonVector[0].Y + (float) (nSize * dZoom);
                    break;
                case eYoonDir2D.Bottom:
                    pIYoonVector[1].X = pIYoonVector[0].X - (float) (nSize / 2 * dZoom);
                    pIYoonVector[1].Y = pIYoonVector[0].Y - (float) (nSize * dZoom);
                    pIYoonVector[2].X = pIYoonVector[0].X + (float) (nSize / 2 * dZoom);
                    pIYoonVector[2].Y = pIYoonVector[0].Y - (float) (nSize * dZoom);
                    break;
                case eYoonDir2D.Left:
                    pIYoonVector[1].X = pIYoonVector[0].X + (float) (nSize * dZoom);
                    pIYoonVector[1].Y = pIYoonVector[0].Y - (float) (nSize / 2 * dZoom);
                    pIYoonVector[2].X = pIYoonVector[0].X + (float) (nSize * dZoom);
                    pIYoonVector[2].Y = pIYoonVector[0].Y + (float) (nSize / 2 * dZoom);
                    break;
                case eYoonDir2D.Right:
                    pIYoonVector[1].X = pIYoonVector[0].X - (float) (nSize * dZoom);
                    pIYoonVector[1].Y = pIYoonVector[0].Y + (float) (nSize / 2 * dZoom);
                    pIYoonVector[2].X = pIYoonVector[0].X - (float) (nSize * dZoom);
                    pIYoonVector[2].Y = pIYoonVector[0].Y - (float) (nSize / 2 * dZoom);
                    break;
            }

            pTaskConvert.Wait();
            using (Graphics graph = Graphics.FromImage(Bitmap))
            {
                Pen pen = new Pen(pColor, (float) nPenWidth);
                graph.DrawLine(pen, pIYoonVector[0], pIYoonVector[1]);
                graph.DrawLine(pen, pIYoonVector[1], pIYoonVector[2]);
                graph.DrawLine(pen, pIYoonVector[2], pIYoonVector[0]);
            }
        }

        public void DrawRect(YoonRect2N pRect, Color pColor, int nPenWidth = 1, double dRatio = 1.0)
        {
            if (pRect.Right <= pRect.Left || pRect.Bottom <= pRect.Top)
                return;
            DrawLine((YoonVector2N) pRect.TopLeft, (YoonVector2N) pRect.TopRight, pColor, nPenWidth, dRatio);
            DrawLine((YoonVector2N) pRect.TopRight, (YoonVector2N) pRect.BottomRight, pColor, nPenWidth, dRatio);
            DrawLine((YoonVector2N) pRect.BottomLeft, (YoonVector2N) pRect.BottomRight, pColor, nPenWidth, dRatio);
            DrawLine((YoonVector2N) pRect.TopLeft, (YoonVector2N) pRect.BottomLeft, pColor, nPenWidth, dRatio);
        }

        public void DrawRect(int nCenterX, int nCenterY, int nWidth, int nHeight, Color pColor, int nPenWidth = 1,
            double dRatio = 1.0)
        {
            if (nWidth <= 0 || nHeight <= 0)
                return;
            DrawLine(nCenterX - nWidth / 2, nCenterY - nHeight / 2, nCenterX + nWidth / 2, nCenterY - nHeight / 2,
                pColor, nPenWidth, dRatio);
            DrawLine(nCenterX + nWidth / 2, nCenterY - nHeight / 2, nCenterX + nWidth / 2, nCenterY + nHeight / 2,
                pColor, nPenWidth, dRatio);
            DrawLine(nCenterX - nWidth / 2, nCenterY + nHeight / 2, nCenterX + nWidth / 2, nCenterY + nHeight / 2,
                pColor, nPenWidth, dRatio);
            DrawLine(nCenterX - nWidth / 2, nCenterY - nHeight / 2, nCenterX - nWidth / 2, nCenterY + nHeight / 2,
                pColor, nPenWidth, dRatio);
        }

        public void DrawLine(YoonLine2N pLine, Color pColor, int nPenWidth = 1, double dRatio = 1.0)
        {
            int nX1 = Math.Max(pLine.X(0), 0);
            int nY1 = pLine.Y(nX1);
            int nX2 = Math.Min(pLine.X(Width), Height);
            int nY2 = pLine.Y(nX2);
            DrawLine(nX1, nY1, nX2, nY2, pColor, nPenWidth, dRatio);
        }
        
        public void DrawLine(YoonVector2N pVector1, YoonVector2N pVector2, Color pColor, int nPenWidth = 1,
            double dRatio = 1.0)
        {
            Task pTaskConvert = ConvertARGB();
            
            double dDeltaX1 = pVector1.X * dRatio;
            double dDeltaY1 = pVector1.Y * dRatio;
            double dDeltaX2 = pVector2.X * dRatio;
            double dDeltaY2 = pVector2.Y * dRatio;
            pTaskConvert.Wait();
            using (Graphics graph = Graphics.FromImage(Bitmap))
            {
                Pen pen = new Pen(pColor, nPenWidth);
                graph.DrawLine(pen, new PointF((float) Math.Round(dDeltaX1), (float) Math.Round(dDeltaY1)),
                    new PointF((float) Math.Round(dDeltaX2), (float) Math.Round(dDeltaY2)));
            }
        }

        public void DrawLine(int nX1, int nY1, int nX2, int nY2, Color pColor, int nPenWidth = 1, double dZoom = 1.0)
        {
            Task pTaskConvert = ConvertARGB();
            
            double dDeltaX1 = (double) nX1 * dZoom;
            double dDeltaY1 = (double) nY1 * dZoom;
            double dDeltaX2 = (double) nX2 * dZoom;
            double dDeltaY2 = (double) nY2 * dZoom;
            pTaskConvert.Wait();
            using (Graphics graph = Graphics.FromImage(Bitmap))
            {
                Pen pPen = new Pen(pColor, nPenWidth);
                graph.DrawLine(pPen, new PointF((float) Math.Round(dDeltaX1), (float) Math.Round(dDeltaY1)),
                    new PointF((float) Math.Round(dDeltaX2), (float) Math.Round(dDeltaY2)));
            }
        }

        public void DrawText(YoonVector2N pVector, Color pColor, string strText, int nFontSize = 8, double dRatio = 1.0)
        {
            Task pTaskConvert = ConvertARGB();
            
            float dDeltaX = (float) (pVector.X * dRatio);
            float dDeltaY = (float) (pVector.Y * dRatio);
            float dSize = (float) nFontSize;
            if (dSize < 10) dSize = 10;
            pTaskConvert.Wait();
            using (Graphics graph = Graphics.FromImage(Bitmap))
            {
                Brush brush = new SolidBrush(pColor);
                FontFamily fontFamily = new FontFamily("Tahoma");
                Font font = new Font(fontFamily, dSize, FontStyle.Regular, GraphicsUnit.Pixel);
                graph.DrawString(strText, font, brush, dDeltaX, dDeltaY);
            }
        }

        public void DrawCross(YoonVector2N pVector, Color pColor, int nSize = 10, int nPenWidth = 1, double dRatio = 1.0)
        {
            Task pTaskConvert = ConvertARGB();
            
            float dDeltaX = (float) (pVector.X * dRatio);
            float dDeltaY = (float) (pVector.Y * dRatio);
            float dX1 = dDeltaX - nSize;
            float dX2 = dDeltaX + nSize;
            float dY1 = dDeltaY - nSize;
            float dY2 = dDeltaY + nSize;
            pTaskConvert.Wait();
            using (Graphics graph = Graphics.FromImage(Bitmap))
            {
                Pen pPen = new Pen(pColor, (float) nPenWidth);
                graph.DrawLine(pPen, new PointF(dX1, dDeltaY), new PointF(dX2, dDeltaY));
                graph.DrawLine(pPen, new PointF(dDeltaX, dY1), new PointF(dDeltaX, dY2));
            }
        }

        public void DrawEllipse(YoonRect2N pRect, Color pColor, int nPenWidth = 1, double dRatio = 1.0)
        {
            Task pTaskConvert = ConvertARGB();
            
            int nX1 = (int) Math.Round(pRect.Left * dRatio);
            int nY1 = (int) Math.Round(pRect.Top * dRatio);
            int nX2 = (int) Math.Round(pRect.Right * dRatio);
            int nY2 = (int) Math.Round(pRect.Bottom * dRatio);
            pTaskConvert.Wait();
            using (Graphics graph = Graphics.FromImage(Bitmap))
            {
                Pen pPen = new Pen(pColor, (float) nPenWidth);
                graph.DrawEllipse(pPen, nX1, nY1, (nX2 - nX1), (nY2 - nY1));
            }
        }

        public void DrawPolygon(YoonVector2N[] pArrayPoint, Color pColor, int nPenWidth = 1, double dRatio = 1.0)
        {
            Task pTaskConvert = ConvertARGB();
            
            PointF[] pArrayDraw = new PointF[pArrayPoint.Length];
            for (int iPoint = 0; iPoint < pArrayPoint.Length; iPoint++)
            {
                pArrayDraw[iPoint].X = pArrayPoint[iPoint].X * (float) dRatio;
                pArrayDraw[iPoint].Y = pArrayPoint[iPoint].Y * (float) dRatio;
            }

            pTaskConvert.Wait();
            using (Graphics graph = Graphics.FromImage(Bitmap))
            {
                Pen pPen = new Pen(pColor, (float) nPenWidth);
                graph.DrawPolygon(pPen, pArrayDraw);
            }
        }
        
        public void DrawPolygon(Color pColor, int nPenWidth = 1, double dRatio = 1.0, params YoonVector2N[] pArgs)
        {
            Task pTaskConvert = ConvertARGB();
            
            PointF[] pArrayDraw = new PointF[pArgs.Length];
            for (int iPoint = 0; iPoint < pArgs.Length; iPoint++)
            {
                pArrayDraw[iPoint].X = pArgs[iPoint].X * (float) dRatio;
                pArrayDraw[iPoint].Y = pArgs[iPoint].Y * (float) dRatio;
            }

            pTaskConvert.Wait();
            using (Graphics graph = Graphics.FromImage(Bitmap))
            {
                Pen pPen = new Pen(pColor, (float) nPenWidth);
                graph.DrawPolygon(pPen, pArrayDraw);
            }
        }
        
        public void DrawPolygon(Color pColor, int nPenWidth = 1, double dRatio = 1.0, params YoonVector2D[] pArgs)
        {
            Task pTaskConvert = ConvertARGB();
            
            PointF[] pArrayDraw = new PointF[pArgs.Length];
            for (int iPoint = 0; iPoint < pArgs.Length; iPoint++)
            {
                pArrayDraw[iPoint].X = (float) pArgs[iPoint].X * (float) dRatio;
                pArrayDraw[iPoint].Y = (float) pArgs[iPoint].Y * (float) dRatio;
            }

            pTaskConvert.Wait();
            using (Graphics graph = Graphics.FromImage(Bitmap))
            {
                Pen pPen = new Pen(pColor, (float) nPenWidth);
                graph.DrawPolygon(pPen, pArrayDraw);
            }
        }
        
        public bool SetOffset(byte offset)
        {
            if (Bitmap.Width <= 0 || Bitmap.Height <= 0)
                throw new IndexOutOfRangeException("[YOONIMAGE ERROR] Bitmap size is not normal");
            byte[] pByte;
            bool bResult = true;
            for (int iY = 0; iY < Bitmap.Height; iY++)
            {
                pByte = new byte[Bitmap.Width];
                for (int iX = 0; iX < Bitmap.Width; iX++)
                {
                    pByte[iX] = (byte) Math.Max(0, Math.Min(GetGrayPixel(iX, iY) + offset, 255));
                }

                if (!SetGrayLine(pByte, iY))
                    bResult = false;
            }

            return bResult;
        }

        public int[] GetGrayHistogram()
        {
            if (Bitmap.Width <= 0 || Bitmap.Height <= 0)
                throw new IndexOutOfRangeException("[YOONIMAGE ERROR] Bitmap size is not normal");
            if (Bitmap.PixelFormat != PixelFormat.Format8bppIndexed)
                throw new FormatException("[YOONIMAGE EXCEPTION] Pixel format isnot correct");
            int[] pHistogram = new int[256];
            Array.Clear(pHistogram, 0, pHistogram.Length);
            for (int iY = 0; iY < Bitmap.Height; iY++)
            {
                for (int iX = 0; iX < Bitmap.Width; iX++)
                {
                    byte nValue = GetGrayPixel(iX, iY);
                    if (nValue > 255 || nValue < 0)
                        continue;
                    pHistogram[nValue]++;
                }
            }

            return pHistogram;
        }

        public int[] GetRedHistogram()
        {
            if (Bitmap.Width <= 0 || Bitmap.Height <= 0)
                throw new IndexOutOfRangeException("[YOONIMAGE ERROR] Bitmap size is not normal");
            if (Bitmap.PixelFormat != PixelFormat.Format32bppArgb && Bitmap.PixelFormat != PixelFormat.Format24bppRgb)
                throw new FormatException("[YOONIMAGE EXCEPTION] Pixel format isnot correct");
            int[] pHistogram = new int[256];
            Array.Clear(pHistogram, 0, pHistogram.Length);
            for (int iY = 0; iY < Bitmap.Height; iY++)
            {
                for (int iX = 0; iX < Bitmap.Width; iX++)
                {
                    byte nValue = GetRedPixel(iX, iY);
                    if (nValue > 255 || nValue < 0)
                        continue;
                    pHistogram[nValue]++;
                }
            }

            return pHistogram;
        }

        public int[] GetBlueHistogram()
        {
            if (Bitmap.Width <= 0 || Bitmap.Height <= 0)
                throw new IndexOutOfRangeException("[YOONIMAGE ERROR] Bitmap size is not normal");
            if (Bitmap.PixelFormat != PixelFormat.Format32bppArgb && Bitmap.PixelFormat != PixelFormat.Format24bppRgb)
                throw new FormatException("[YOONIMAGE EXCEPTION] Pixel format isnot correct");
            int[] pHistogram = new int[256];
            Array.Clear(pHistogram, 0, pHistogram.Length);
            for (int iY = 0; iY < Bitmap.Height; iY++)
            {
                for (int iX = 0; iX < Bitmap.Width; iX++)
                {
                    byte value = GetBluePixel(iX, iY);
                    if (value > 255 || value < 0)
                        continue;
                    pHistogram[value]++;
                }
            }

            return pHistogram;
        }

        public int[] GetGreenHistogram()
        {
            if (Bitmap.Width <= 0 || Bitmap.Height <= 0)
                throw new IndexOutOfRangeException("[YOONIMAGE ERROR] Bitmap size is not normal");
            if (Bitmap.PixelFormat != PixelFormat.Format32bppArgb && Bitmap.PixelFormat != PixelFormat.Format24bppRgb)
                throw new FormatException("[YOONIMAGE EXCEPTION] Pixel format isnot correct");
            int[] pHistogram = new int[256];
            Array.Clear(pHistogram, 0, pHistogram.Length);
            for (int iY = 0; iY < Bitmap.Height; iY++)
            {
                for (int iX = 0; iX < Bitmap.Width; iX++)
                {
                    byte nValue = GetGreenPixel(iX, iY);
                    if (nValue > 255 || nValue < 0)
                        continue;
                    pHistogram[nValue]++;
                }
            }

            return pHistogram;
        }

        public byte[] GetGrayBuffer()
        {
            return Scan8bitBuffer(new Rectangle(Point.Empty, Bitmap.Size));
        }

        public byte[] GetGrayLine(int nPixelY)
        {
            return Scan8bitBuffer(new Rectangle(0, nPixelY, Bitmap.Width, 1));
        }

        public byte GetGrayPixel(int nPixelX, int nPixelY)
        {
            return Scan8bitBuffer(new Rectangle(nPixelX, nPixelY, 1, 1))[0];
        }

        public byte GetGrayPixel(YoonVector2N pVec)
        {
            return Scan8bitBuffer(new Rectangle(pVec.X, pVec.Y, 1, 1))[0];
        }

        public byte[] GetRGBBuffer()
        {
            return Scan24bitBuffer(new Rectangle(Point.Empty, Bitmap.Size));
        }

        public int[] GetARGBBuffer()
        {
            return Scan32bitBuffer(new Rectangle(Point.Empty, Bitmap.Size));
        }

        private byte[]
            GetPlaneBuffer(int nPlane) // RGB Plane 0 : R, 1 : G, 2 : B / ARGB Plane 0 : A, 1 : R, 2 : G, 3 : B
        {
            switch (Bitmap.PixelFormat)
            {
                case PixelFormat.Format24bppRgb:
                    return Scan24bitPlaneBuffer(new Rectangle(Point.Empty, Bitmap.Size), nPlane);
                case PixelFormat.Format32bppArgb:
                    return Scan32bitPlaneBuffer(new Rectangle(Point.Empty, Bitmap.Size), nPlane);
                default:
                    throw new FormatException("[YOONIMAGE EXCEPTION] Pixel format isnot correct");
            }
        }

        private byte GetPlanePixel(int nPlane, int nPixelX, int nPixelY)
        {
            switch (Bitmap.PixelFormat)
            {
                case PixelFormat.Format24bppRgb:
                    return Scan24bitPlaneBuffer(new Rectangle(nPixelX, nPixelY, 1, 1), nPlane)[0];
                case PixelFormat.Format32bppArgb:
                    return Scan32bitPlaneBuffer(new Rectangle(nPixelX, nPixelY, 1, 1), nPlane)[0];
                default:
                    throw new FormatException("[YOONIMAGE EXCEPTION] Pixel format isnot correct");
            }
        }

        public byte[] GetRedBuffer()
        {
            switch (Bitmap.PixelFormat)
            {
                case PixelFormat.Format24bppRgb:
                    return GetPlaneBuffer(0); // 0 : Red
                case PixelFormat.Format32bppArgb:
                    return GetPlaneBuffer(1); // 1 : Red
                default:
                    throw new FormatException("[YOONIMAGE EXCEPTION] Pixel format isnot correct");
            }
        }

        public byte GetRedPixel(int nPixelX, int nPixelY)
        {
            switch (Bitmap.PixelFormat)
            {
                case PixelFormat.Format24bppRgb:
                    return GetPlanePixel(0, nPixelX, nPixelY); // 0 : Red
                case PixelFormat.Format32bppArgb:
                    return GetPlanePixel(1, nPixelX, nPixelY); // 1 : Red
                default:
                    throw new FormatException("[YOONIMAGE EXCEPTION] Pixel format isnot correct");
            }
        }

        public byte[] GetGreenBuffer()
        {
            switch (Bitmap.PixelFormat)
            {
                case PixelFormat.Format24bppRgb:
                    return GetPlaneBuffer(1); // 1 : Green
                case PixelFormat.Format32bppArgb:
                    return GetPlaneBuffer(2); // 2 : Green
                default:
                    throw new FormatException("[YOONIMAGE EXCEPTION] Pixel format isnot correct");
            }
        }

        public byte GetGreenPixel(int nPixelX, int nPixelY)
        {
            switch (Bitmap.PixelFormat)
            {
                case PixelFormat.Format24bppRgb:
                    return GetPlanePixel(1, nPixelX, nPixelY); // 1 : Green
                case PixelFormat.Format32bppArgb:
                    return GetPlanePixel(2, nPixelX, nPixelY); // 2 : Green
                default:
                    throw new FormatException("[YOONIMAGE EXCEPTION] Pixel format isnot correct");
            }
        }

        public byte[] GetBlueBuffer()
        {
            switch (Bitmap.PixelFormat)
            {
                case PixelFormat.Format24bppRgb:
                    return GetPlaneBuffer(2); // 2 : Blue
                case PixelFormat.Format32bppArgb:
                    return GetPlaneBuffer(3); // 3 : Blue
                default:
                    throw new FormatException("[YOONIMAGE EXCEPTION] Pixel format isnot correct");
            }
        }

        public byte GetBluePixel(int nPixelX, int nPixelY)
        {
            switch (Bitmap.PixelFormat)
            {
                case PixelFormat.Format24bppRgb:
                    return GetPlanePixel(2, nPixelX, nPixelY); // 2 : Blue
                case PixelFormat.Format32bppArgb:
                    return GetPlanePixel(3, nPixelX, nPixelY); // 3 : Blue
                default:
                    throw new FormatException("[YOONIMAGE EXCEPTION] Pixel format isnot correct");
            }
        }

        public byte[] GetRGBLine(int nPixelY)
        {
            return Scan24bitBuffer(new Rectangle(0, nPixelY, Bitmap.Width, 1));
        }

        public byte[] GetRGBPixel(int nPixelX, int nPixelY)
        {
            return Scan24bitBuffer(new Rectangle(nPixelX, nPixelY, 1, 1));
        }

        public byte[] GetRGBPixel(YoonVector2N pVec)
        {
            return Scan24bitBuffer(new Rectangle(pVec.X, pVec.Y, 1, 1));
        }

        public int[] GetARGBLine(int nPixelY)
        {
            return Scan32bitBuffer(new Rectangle(0, nPixelY, Bitmap.Width, 1));
        }

        public int GetARGBPixel(int nPixelX, int nPixelY)
        {
            return Scan32bitBuffer(new Rectangle(nPixelX, nPixelY, 1, 1))[0];
        }

        public int GetARGBPixel(YoonVector2N pVec)
        {
            return Scan32bitBuffer(new Rectangle(pVec.X, pVec.Y, 1, 1))[0];
        }

        public bool SetGrayImage(IntPtr pBufferAddress)
        {
            if (pBufferAddress == IntPtr.Zero)
                throw new ArgumentNullException("[YOONIMAGE EXCEPTION] Address is null");
            if (Bitmap.Width <= 0 || Bitmap.Height <= 0)
                throw new IndexOutOfRangeException("[YOONIMAGE ERROR] Bitmap size is not normal");

            byte[] pBuffer = new byte[Bitmap.Width * Bitmap.Height];
            Marshal.Copy(pBufferAddress, pBuffer, 0, Bitmap.Width * Bitmap.Height);
            return SetGrayImage(pBuffer);
        }

        public bool SetGrayImage(byte[] pBuffer)
        {
            return Print8bitBuffer(pBuffer, new Rectangle(Point.Empty, Bitmap.Size));
        }

        public bool SetGrayLine(byte[] pBuffer, int nPixelY)
        {
            return Print8bitBuffer(pBuffer, new Rectangle(0, nPixelY, Bitmap.Width, 1));
        }

        public bool SetGrayPixel(byte nLevel, int nPixelX, int nPixelY)
        {
            return Print8bitBuffer(new byte[1] {nLevel}, new Rectangle(nPixelX, nPixelY, 1, 1));
        }

        public bool SetGrayPixel(byte nLevel, YoonVector2N pVec)
        {
            return Print8bitBuffer(new byte[1] {nLevel}, new Rectangle(pVec.X, pVec.Y, 1, 1));
        }

        public bool SetRGBImage(IntPtr pBufferAddress)
        {
            if (pBufferAddress == IntPtr.Zero)
                throw new ArgumentNullException("[YOONIMAGE EXCEPTION] Address is null");
            if (Bitmap.Width <= 0 || Bitmap.Height <= 0)
                throw new IndexOutOfRangeException("[YOONIMAGE ERROR] Bitmap size is not normal");

            byte[] pBuffer = new byte[Bitmap.Width * Bitmap.Height * 3];
            Marshal.Copy(pBufferAddress, pBuffer, 0, Bitmap.Width * Bitmap.Height);
            return SetRGBImage(pBuffer);
        }

        public bool SetRGBImage(byte[] pBuffer)
        {
            return Print24bitBuffer(pBuffer, new Rectangle(Point.Empty, Bitmap.Size));
        }

        public bool SetRGBLine(byte[] pBuffer, int nPixelY)
        {
            return Print24bitBuffer(pBuffer, new Rectangle(0, nPixelY, Bitmap.Width, 1));
        }

        public bool SetRGBPixel(byte[] pLevel, int nPixelX, int nPixelY)
        {
            return Print24bitBuffer(pLevel, new Rectangle(nPixelX, nPixelY, 1, 1));
        }

        public bool SetRGBPixel(byte[] pLevel, YoonVector2N pVec)
        {
            return Print24bitBuffer(pLevel, new Rectangle(pVec.X, pVec.Y, 1, 1));
        }

        public bool SetRGBImageWithColorMixed(IntPtr pBufferAddress, bool bRGBOrder = true)
        {
            if (pBufferAddress == IntPtr.Zero)
                throw new ArgumentNullException("[YOONIMAGE EXCEPTION] Address is null");
            if (Bitmap.Width <= 0 || Bitmap.Height <= 0 || Bitmap.PixelFormat != PixelFormat.Format24bppRgb)
                throw new IndexOutOfRangeException("[YOONIMAGE ERROR] Bitmap property is not normal");
            byte[] pBuffer = new byte[Bitmap.Width * Bitmap.Height * 3];
            Marshal.Copy(pBufferAddress, pBuffer, 0, Bitmap.Width * Bitmap.Height * 3);
            return SetRGBImageWithColorMixed(pBuffer, bRGBOrder);
        }

        public bool SetRGBImageWithColorMixed(byte[] pBuffer, bool bRGBOrder = true)
        {
            if (Bitmap.PixelFormat != PixelFormat.Format24bppRgb)
                throw new FormatException("[YOONIMAGE EXCEPTION] Pixel format isnot correct");
            if (pBuffer.Length != Bitmap.Width * Bitmap.Height * 3)
                throw new ArgumentOutOfRangeException("[YOONIMAGE ERROR] Buffer property is out of range");
            int nRed, nGreen, nBlue;
            if (bRGBOrder)
            {
                nRed = 0;
                nGreen = 1;
                nBlue = 2;
            }
            else
            {
                nRed = 2;
                nGreen = 1;
                nBlue = 0;
            }

            bool bResult = true;
            for (int iY = 0; iY < Bitmap.Height; iY++) // Exception for nHeight-1
            {
                byte[] pPixel = new byte[Bitmap.Width * 3];
                for (int iX = 0; iX < Bitmap.Width; iX++)
                {
                    pPixel[iX * 3 + 0] = pBuffer[iY * Bitmap.Width * 3 + iX * 3 + nBlue]; // Blue
                    pPixel[iX * 3 + 1] = pBuffer[iY * Bitmap.Width * 3 + iX * 3 + nGreen]; // Green
                    pPixel[iX * 3 + 2] = pBuffer[iY * Bitmap.Width * 3 + iX * 3 + nRed]; // Red
                }

                if (!SetRGBLine(pPixel, iY))
                    bResult = false;
            }

            return bResult;
        }

        public bool SetRGBImageWithColorParallel(IntPtr pBufferAddress, bool bRGBOrder = true)
        {
            if (pBufferAddress == IntPtr.Zero)
                throw new ArgumentNullException("[YOONIMAGE EXCEPTION] Address is null");
            if (Bitmap.Width <= 0 || Bitmap.Height <= 0 || Bitmap.PixelFormat != PixelFormat.Format24bppRgb)
                throw new IndexOutOfRangeException("[YOONIMAGE ERROR] Bitmap property is not normal");
            byte[] pBuffer = new byte[Bitmap.Width * Bitmap.Height * 3];
            Marshal.Copy(pBufferAddress, pBuffer, 0, Bitmap.Width * Bitmap.Height * 3);
            return SetRGBImageWithColorParallel(pBuffer, bRGBOrder);
        }

        public bool SetRGBImageWithColorParallel(byte[] pBuffer, bool bRGBOrder = true)
        {
            if (Bitmap.PixelFormat != PixelFormat.Format24bppRgb)
                throw new FormatException("[YOONIMAGE EXCEPTION] Pixel format isnot correct");
            if (pBuffer.Length != Bitmap.Width * Bitmap.Height * 3)
                throw new ArgumentOutOfRangeException("[YOONIMAGE ERROR] Buffer property is out of range");
            int nRed, nGreen, nBlue;
            if (bRGBOrder)
            {
                nRed = 0;
                nGreen = 1;
                nBlue = 2;
            }
            else
            {
                nRed = 2;
                nGreen = 1;
                nBlue = 0;
            }

            byte[] pPixel;
            bool bResult = true;
            for (int iPlane = 0; iPlane < 3; iPlane++)
            {
                pPixel = new byte[Bitmap.Width * 3];
                for (int iY = 0; iY < Bitmap.Height; iY++) // Exception for nHeight-1
                {
                    for (int iX = 0; iX < Bitmap.Width; iX++)
                    {
                        pPixel[iX * 3 + 0] =
                            pBuffer[(nBlue * Bitmap.Width * Bitmap.Height) + iY * Bitmap.Width + iX]; // Blue
                        pPixel[iX * 3 + 1] =
                            pBuffer[(nGreen * Bitmap.Width * Bitmap.Height) + iY * Bitmap.Width + iX]; // Green
                        pPixel[iX * 3 + 2] =
                            pBuffer[(nRed * Bitmap.Width * Bitmap.Height) + iY * Bitmap.Width + iX]; // Red
                    }

                    if (!SetRGBLine(pPixel, iY))
                        bResult = false;
                }
            }

            return bResult;
        }

        public bool SetRGBImageWithPlane(byte[] pRed, byte[] pGreen, byte[] pBlue)
        {
            if (Bitmap.PixelFormat != PixelFormat.Format24bppRgb)
                throw new FormatException("[YOONIMAGE EXCEPTION] Pixel format isnot correct");
            if (pRed == null || pRed.Length != Bitmap.Width * Bitmap.Height ||
                pGreen == null || pGreen.Length != Bitmap.Width * Bitmap.Height ||
                pBlue == null || pBlue.Length != Bitmap.Width * Bitmap.Height)
                throw new ArgumentOutOfRangeException("[YOONIMAGE ERROR] Buffer property is out of range");
            byte[] pPixel;
            bool bResult = true;
            for (int iY = 0; iY < Bitmap.Height; iY++)
            {
                pPixel = new byte[Bitmap.Width * 3];
                for (int iX = 0; iX < Bitmap.Width; iX++)
                {
                    pPixel[iX * 3 + 0] = pBlue[iY * Bitmap.Width + iX];
                    pPixel[iX * 3 + 1] = pGreen[iY * Bitmap.Width + iX];
                    pPixel[iX * 3 + 2] = pRed[iY * Bitmap.Width + iX];
                }

                if (!SetRGBLine(pPixel, iY))
                    bResult = false;
            }

            return bResult;
        }

        public bool SetARGBImage(IntPtr pBufferAddress)
        {
            if (pBufferAddress == IntPtr.Zero)
                throw new ArgumentNullException("[YOONIMAGE EXCEPTION] Address is null");
            if (Bitmap.Width <= 0 || Bitmap.Height <= 0)
                throw new IndexOutOfRangeException("[YOONIMAGE ERROR] Bitmap size is not normal");

            int[] pBuffer = new int[Bitmap.Width * Bitmap.Height];
            Marshal.Copy(pBufferAddress, pBuffer, 0, Bitmap.Width * Bitmap.Height);
            return SetARGBImage(pBuffer);
        }

        public bool SetARGBImage(int[] pBuffer)
        {
            return Print32bitBuffer(pBuffer, new Rectangle(Point.Empty, Bitmap.Size));
        }

        public bool SetARGBLine(int[] pBuffer, int nPixelY)
        {
            return Print32bitBuffer(pBuffer, new Rectangle(0, nPixelY, Bitmap.Width, 1));
        }

        public bool SetARGBPixel(int nLevel, int nPixelX, int nPixelY)
        {
            return Print32bitBuffer(new int[1] {nLevel}, new Rectangle(nPixelX, nPixelY, 1, 1));
        }

        public bool SetARGBPixel(int nLevel, YoonVector2N pVector)
        {
            return Print32bitBuffer(new int[1] {nLevel}, new Rectangle(pVector.X, pVector.Y, 1, 1));
        }

        public bool SetARGBImageWithColorMixed(IntPtr pBufferAddress, bool bRGBOrder = true)
        {
            if (pBufferAddress == IntPtr.Zero)
                throw new ArgumentNullException("[YOONIMAGE EXCEPTION] Address is null");
            if (Bitmap.Width <= 0 || Bitmap.Height <= 0 || Bitmap.PixelFormat != PixelFormat.Format32bppArgb)
                throw new IndexOutOfRangeException("[YOONIMAGE ERROR] Bitmap property is not normal");
            byte[] pBuffer = new byte[Bitmap.Width * Bitmap.Height * 4];
            Marshal.Copy(pBufferAddress, pBuffer, 0, Bitmap.Width * Bitmap.Height * 4);
            return SetARGBImageWithColorMixed(pBuffer, bRGBOrder);
        }

        public bool SetARGBImageWithColorMixed(byte[] pBuffer, bool bRGBOrder = true)
        {
            if (Bitmap.PixelFormat != PixelFormat.Format32bppArgb)
                throw new FormatException("[YOONIMAGE EXCEPTION] Pixel format isnot correct");
            if (pBuffer.Length != Bitmap.Width * Bitmap.Height * 3)
                throw new ArgumentOutOfRangeException("[YOONIMAGE ERROR] Buffer property is out of range");
            int nRed, nGreen, nBlue;
            int nAlpha = 3;
            if (bRGBOrder)
            {
                nRed = 0;
                nGreen = 1;
                nBlue = 2;
            } // Alpha = 3
            else
            {
                nRed = 2;
                nGreen = 1;
                nBlue = 0;
            } // Alpha = 3

            int[] pPixel;
            bool bResult = true;
            for (int iY = 0; iY < Bitmap.Height; iY++) // Exception for nHeight-1
            {
                pPixel = new int[Bitmap.Width];
                for (int iX = 0; iX < Bitmap.Width; iX++)
                {
                    byte[] pBytePixel = new byte[4];
                    pBytePixel[0] = pBuffer[iY * Bitmap.Width * 3 + iX * 3 + nBlue]; // Blue
                    pBytePixel[1] = pBuffer[iY * Bitmap.Width * 3 + iX * 3 + nGreen]; // Green
                    pBytePixel[2] = pBuffer[iY * Bitmap.Width * 3 + iX * 3 + nRed]; // Red
                    pBytePixel[3] = pBuffer[iY * Bitmap.Width * 3 + iX * 3 + nAlpha]; // Alpha = Max (0xFF)
                    pPixel[iX] = BitConverter.ToInt32(pBytePixel, 0);
                }

                if (!SetARGBLine(pPixel, iY))
                    bResult = false;
            }

            return bResult;
        }

        public bool SetARGBImageWithColorParallel(IntPtr pBufferAddress, bool bRGBOrder = true)
        {
            if (pBufferAddress == IntPtr.Zero)
                throw new ArgumentNullException("[YOONIMAGE EXCEPTION] Address is null");
            if (Bitmap.Width <= 0 || Bitmap.Height <= 0 || Bitmap.PixelFormat != PixelFormat.Format32bppArgb)
                throw new IndexOutOfRangeException("[YOONIMAGE ERROR] Bitmap property is not normal");
            byte[] pBuffer = new byte[Bitmap.Width * Bitmap.Height * 4];
            Marshal.Copy(pBufferAddress, pBuffer, 0, Bitmap.Width * Bitmap.Height * 4);
            return SetARGBImageWithColorParallel(pBuffer, bRGBOrder);
        }

        public bool SetARGBImageWithColorParallel(byte[] pBuffer, bool bRGBOrder = true)
        {
            if (Bitmap.PixelFormat != PixelFormat.Format32bppArgb)
                throw new FormatException("[YOONIMAGE EXCEPTION] Pixel format isnot correct");
            if (pBuffer.Length != Bitmap.Width * Bitmap.Height * 3)
                throw new ArgumentOutOfRangeException("[YOONIMAGE ERROR] Buffer property is out of range");
            int nRed, nGreen, nBlue;
            int nAlpha = 3;
            if (bRGBOrder)
            {
                nRed = 0;
                nGreen = 1;
                nBlue = 2;
            } // Alpha = 3
            else
            {
                nRed = 2;
                nGreen = 1;
                nBlue = 0;
            } // Alpha = 3

            int[] pPixel;
            bool bResult = true;
            for (int iPlane = 0; iPlane < 3; iPlane++)
            {
                pPixel = new int[Bitmap.Width];
                for (int iY = 0; iY < Bitmap.Height; iY++) // Exception for nHeight-1
                {
                    for (int iX = 0; iX < Bitmap.Width; iX++)
                    {
                        byte[] pBytePixel = new byte[4];
                        pBytePixel[0] =
                            pBuffer[(nBlue * Bitmap.Width * Bitmap.Height) + iY * Bitmap.Width + iX]; // Blue
                        pBytePixel[1] =
                            pBuffer[(nGreen * Bitmap.Width * Bitmap.Height) + iY * Bitmap.Width + iX]; // Green
                        pBytePixel[2] = pBuffer[(nRed * Bitmap.Width * Bitmap.Height) + iY * Bitmap.Width + iX]; // Red
                        pBytePixel[3] = pBuffer[(nAlpha * Bitmap.Width * Bitmap.Height) + iY * Bitmap.Width + iX];
                        ; // Alpha = Max (0xFF)
                        pPixel[iX] = BitConverter.ToInt32(pBytePixel, 0);
                    }

                    if (!SetARGBLine(pPixel, iY))
                        bResult = false;
                }
            }

            return bResult;
        }

        public bool SetARGBImageWithPlane(byte[] pRed, byte[] pGreen, byte[] pBlue)
        {
            if (Bitmap.PixelFormat != PixelFormat.Format32bppArgb)
                throw new FormatException("[YOONIMAGE EXCEPTION] Pixel format isnot correct");
            if (pRed == null || pRed.Length != Bitmap.Width * Bitmap.Height ||
                pGreen == null || pGreen.Length != Bitmap.Width * Bitmap.Height ||
                pBlue == null || pBlue.Length != Bitmap.Width * Bitmap.Height)
                throw new ArgumentOutOfRangeException("[YOONIMAGE ERROR] Buffer property is out of range");
            int[] pPixel;
            bool bResult = true;
            for (int iY = 0; iY < Bitmap.Height; iY++)
            {
                pPixel = new int[Bitmap.Width];
                for (int iX = 0; iX < Bitmap.Width; iX++)
                {
                    byte[] pBytePixel = new byte[4];
                    pBytePixel[0] = pBlue[iY * Bitmap.Width + iX];
                    pBytePixel[1] = pGreen[iY * Bitmap.Width + iX];
                    pBytePixel[2] = pRed[iY * Bitmap.Width + iX];
                    pBytePixel[3] = (byte) 0xFF; // Alpha = Max (0xFF)
                    pPixel[iX] = BitConverter.ToInt32(pBytePixel, 0);
                }

                if (!SetARGBLine(pPixel, iY))
                    bResult = false;
            }

            return bResult;
        }

        private byte[] Scan8bitBuffer(Rectangle pRect)
        {
            if (Format != PixelFormat.Format8bppIndexed)
                throw new FormatException("[YOONIMAGE EXCEPTION] Bitmap isnot Indexed format");
            if (pRect.X > Bitmap.Width || pRect.Y > Bitmap.Height)
                throw new ArgumentOutOfRangeException("[YOONIMAGE ERROR] Rect property is out of range");
            byte[] pBuffer = new byte[pRect.Width * pRect.Height];
            try
            {
                BitmapData pImageData = Bitmap.LockBits(pRect, ImageLockMode.ReadOnly, Bitmap.PixelFormat);
                Marshal.Copy(pImageData.Scan0, pBuffer, 0, pBuffer.Length);
                Bitmap.UnlockBits(pImageData);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
                return null;
            }

            return pBuffer;
        }

        private byte[] Scan24bitBuffer(Rectangle pRect)
        {
            if (Format != PixelFormat.Format24bppRgb)
                throw new FormatException("[YOONIMAGE EXCEPTION] Bitmap isnot RGB format");
            if (pRect.X > Bitmap.Width || pRect.Y > Bitmap.Height)
                throw new ArgumentOutOfRangeException("[YOONIMAGE ERROR] Rect property is out of range");
            byte[] pBuffer = new byte[pRect.Width * pRect.Height * Plane];
            try
            {
                BitmapData pImageData = Bitmap.LockBits(pRect, ImageLockMode.ReadOnly, Bitmap.PixelFormat);
                Marshal.Copy(pImageData.Scan0, pBuffer, 0, pBuffer.Length);
                Bitmap.UnlockBits(pImageData);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
                return null;
            }

            return pBuffer;
        }

        private int[] Scan32bitBuffer(Rectangle pRect)
        {
            if (Format != PixelFormat.Format32bppArgb)
                throw new FormatException("[YOONIMAGE EXCEPTION] Bitmap isnot RGB format");
            if (pRect.X > Bitmap.Width || pRect.Y > Bitmap.Height)
                throw new ArgumentOutOfRangeException("[YOONIMAGE ERROR] Rect property is out of range");
            int[] pBuffer = new int[pRect.Width * pRect.Height];
            try
            {
                BitmapData pImageData = Bitmap.LockBits(pRect, ImageLockMode.ReadOnly, Bitmap.PixelFormat);
                Marshal.Copy(pImageData.Scan0, pBuffer, 0, pBuffer.Length);
                Bitmap.UnlockBits(pImageData);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
                return null;
            }

            return pBuffer;
        }

        private byte[,] Scan24bitPlaneBuffer(Rectangle pRect)
        {
            if (Format != PixelFormat.Format24bppRgb)
                throw new FormatException("[YOONIMAGE EXCEPTION] Bitmap isnot RGB format");
            if (pRect.X > Bitmap.Width || pRect.Y > Bitmap.Height)
                throw new ArgumentOutOfRangeException("[YOONIMAGE ERROR] Rect property is out of range");
            byte[,] pPixelPlanes = new byte[pRect.Width * pRect.Height, Plane];
            try
            {
                BitmapData pImageData = Bitmap.LockBits(pRect, ImageLockMode.ReadOnly, Bitmap.PixelFormat);
                Parallel.For(0, pRect.Height, j =>
                {
                    for (int i = 0; i < pRect.Width; i++)
                    {
                        byte[] pBytePixel = new byte[3]; // {R, G, B}
                        Marshal.Copy(pImageData.Scan0 + (j * pRect.Width * 3 + i * 3), pBytePixel, 0,
                            pBytePixel.Length);
                        pPixelPlanes[j * pRect.Width + i, 0] = pBytePixel[0]; // R
                        pPixelPlanes[j * pRect.Width + i, 1] = pBytePixel[1]; // G
                        pPixelPlanes[j * pRect.Width + i, 2] = pBytePixel[2]; // B
                    }
                });
                Bitmap.UnlockBits(pImageData);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
                return null;
            }

            return pPixelPlanes;
        }

        private byte[] Scan24bitPlaneBuffer(Rectangle pRect, int nPlane)
        {
            if (Format != PixelFormat.Format24bppRgb)
                throw new FormatException("[YOONIMAGE EXCEPTION] Bitmap isnot RGB format");
            if (nPlane < 0 || nPlane >= Plane)
                throw new ArgumentException("[YOONIMAGE EXCEPTION] Plane isnot adjust");
            if (pRect.X > Bitmap.Width || pRect.Y > Bitmap.Height)
                throw new ArgumentOutOfRangeException("[YOONIMAGE ERROR] Rect property is out of range");
            byte[] pPixelPlanes = new byte[pRect.Width * pRect.Height];
            try
            {
                BitmapData pImageData = Bitmap.LockBits(pRect, ImageLockMode.ReadOnly, Bitmap.PixelFormat);
                Parallel.For(0, pRect.Height, j =>
                {
                    for (int i = 0; i < pRect.Width; i++)
                    {
                        byte[] pBytePixel = new byte[3]; // {R, G, B}
                        Marshal.Copy(pImageData.Scan0 + (j * pRect.Width * 3 + i * 3), pBytePixel, 0,
                            pBytePixel.Length);
                        pPixelPlanes[j * pRect.Width + i] = pBytePixel[nPlane];
                    }
                });
                Bitmap.UnlockBits(pImageData);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
                return null;
            }

            return pPixelPlanes;
        }

        private byte[,] Scan32bitPlaneBuffer(Rectangle pRect)
        {
            if (Format != PixelFormat.Format32bppArgb)
                throw new FormatException("[YOONIMAGE EXCEPTION] Bitmap isnot RGB format");
            if (pRect.X > Bitmap.Width || pRect.Y > Bitmap.Height)
                throw new ArgumentOutOfRangeException("[YOONIMAGE ERROR] Rect property is out of range");
            byte[,] pPixelPlanes = new byte[pRect.Width * pRect.Height, Plane];
            try
            {
                BitmapData pImageData = Bitmap.LockBits(pRect, ImageLockMode.ReadOnly, Bitmap.PixelFormat);
                Parallel.For(0, pRect.Height, j =>
                {
                    for (int i = 0; i < pRect.Width; i++)
                    {
                        byte[] pBytePixel = new byte[4]; // {A, R, G, B}
                        Marshal.Copy(pImageData.Scan0 + (j * pRect.Width * 4 + i * 4), pBytePixel, 0,
                            pBytePixel.Length);
                        pPixelPlanes[j * pRect.Width + i, 0] = pBytePixel[0]; // A
                        pPixelPlanes[j * pRect.Width + i, 1] = pBytePixel[1]; // R
                        pPixelPlanes[j * pRect.Width + i, 2] = pBytePixel[2]; // G
                        pPixelPlanes[j * pRect.Width + i, 3] = pBytePixel[3]; // B
                    }
                });
                Bitmap.UnlockBits(pImageData);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
                return null;
            }

            return pPixelPlanes;
        }

        private byte[] Scan32bitPlaneBuffer(Rectangle pRect, int nPlane)
        {
            if (Format != PixelFormat.Format32bppArgb)
                throw new FormatException("[YOONIMAGE EXCEPTION] Bitmap isnot RGB format");
            if (nPlane < 0 || nPlane >= Plane)
                throw new ArgumentException("[YOONIMAGE EXCEPTION] Plane isnot adjust");
            if (pRect.X > Bitmap.Width || pRect.Y > Bitmap.Height)
                throw new ArgumentOutOfRangeException("[YOONIMAGE ERROR] Rect property is out of range");
            byte[] pPixelPlanes = new byte[pRect.Width * pRect.Height];
            try
            {
                BitmapData pImageData = Bitmap.LockBits(pRect, ImageLockMode.ReadOnly, Bitmap.PixelFormat);
                Parallel.For(0, pRect.Height, j =>
                {
                    for (int i = 0; i < pRect.Width; i++)
                    {
                        byte[] pBytePixel = new byte[4]; // {A, R, G, B}
                        Marshal.Copy(pImageData.Scan0 + (j * pRect.Width * 4 + i * 4), pBytePixel, 0,
                            pBytePixel.Length);
                        pPixelPlanes[j * pRect.Width + i] = pBytePixel[nPlane];
                    }
                });
                Bitmap.UnlockBits(pImageData);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
                return null;
            }

            return pPixelPlanes;
        }

        private bool Print8bitBuffer(byte[] pBuffer, Rectangle pRect)
        {
            if (Plane != 1)
                throw new FormatException("[YOONIMAGE EXCEPTION] Bitmap isnot Indexed format");
            if (Bitmap.Width <= 0 || Bitmap.Height <= 0)
                throw new IndexOutOfRangeException("[YOONIMAGE ERROR] Bitmap size is not normal");
            if (pRect.X > Bitmap.Width || pRect.Y > Bitmap.Height)
                throw new ArgumentOutOfRangeException("[YOONIMAGE ERROR] Rect property is out of range");
            if (pBuffer.Length != pRect.Width * pRect.Height)
                throw new ArgumentException("[YOONIMAGE ERROR] Rect or Buffer property is abnormal");
            try
            {
                BitmapData pImageData = Bitmap.LockBits(pRect, ImageLockMode.WriteOnly, Bitmap.PixelFormat);
                Marshal.Copy(pBuffer, 0, pImageData.Scan0, pBuffer.Length);
                Bitmap.UnlockBits(pImageData);
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }

            return false;
        }

        private bool Print24bitBuffer(byte[] pBuffer, Rectangle pRect)
        {
            if (Plane != 3)
                throw new FormatException("[YOONIMAGE EXCEPTION] Bitmap isnot RGB format");
            if (Bitmap.Width <= 0 || Bitmap.Height <= 0)
                throw new IndexOutOfRangeException("[YOONIMAGE ERROR] Bitmap size is not normal");
            if (pRect.X > Bitmap.Width || pRect.Y > Bitmap.Height)
                throw new ArgumentOutOfRangeException("[YOONIMAGE ERROR] Rect property is out of range");
            if (pBuffer.Length != pRect.Width * pRect.Height * Plane)
                throw new ArgumentException("[YOONIMAGE ERROR] Rect or Buffer property is abnormal");
            try
            {
                BitmapData pImageData = Bitmap.LockBits(pRect, ImageLockMode.WriteOnly, Bitmap.PixelFormat);
                Marshal.Copy(pBuffer, 0, pImageData.Scan0, pBuffer.Length);
                Bitmap.UnlockBits(pImageData);
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }

            return false;
        }

        private bool Print32bitBuffer(int[] pBuffer, Rectangle pRect)
        {
            if (Plane != 4)
                throw new FormatException("[YOONIMAGE EXCEPTION] Bitmap isnot RGB format");
            if (Bitmap.Width <= 0 || Bitmap.Height <= 0)
                throw new IndexOutOfRangeException("[YOONIMAGE ERROR] Bitmap size is not normal");
            if (pRect.X > Bitmap.Width || pRect.Y > Bitmap.Height)
                throw new ArgumentOutOfRangeException("[YOONIMAGE ERROR] Rect property is out of range");
            if (pBuffer.Length != pRect.Width * pRect.Height)
                throw new ArgumentException("[YOONIMAGE ERROR] Rect or Buffer property is abnormal");
            try
            {
                BitmapData pImageData = Bitmap.LockBits(pRect, ImageLockMode.WriteOnly, PixelFormat.Format32bppArgb);
                Marshal.Copy(pBuffer, 0, pImageData.Scan0, pBuffer.Length);
                Bitmap.UnlockBits(pImageData);
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }

            return false;
        }

        public bool Equals(YoonImage other)
        {
            return other != null &&
                   _disposedValue == other._disposedValue &&
                   EqualityComparer<Bitmap>.Default.Equals(Bitmap, other.Bitmap) &&
                   FilePath == other.FilePath &&
                   Format == other.Format &&
                   Plane == other.Plane &&
                   Stride == other.Stride &&
                   Width == other.Width &&
                   Height == other.Height;
        }

        public override int GetHashCode()
        {
            int hashCode = 1198763934;
            hashCode = hashCode * -1521134295 + _disposedValue.GetHashCode();
            hashCode = hashCode * -1521134295 + EqualityComparer<Bitmap>.Default.GetHashCode(Bitmap);
            hashCode = hashCode * -1521134295 + EqualityComparer<string>.Default.GetHashCode(FilePath);
            hashCode = hashCode * -1521134295 + Format.GetHashCode();
            hashCode = hashCode * -1521134295 + Plane.GetHashCode();
            hashCode = hashCode * -1521134295 + Stride.GetHashCode();
            hashCode = hashCode * -1521134295 + Width.GetHashCode();
            hashCode = hashCode * -1521134295 + Height.GetHashCode();
            return hashCode;
        }

        public static YoonImage operator +(YoonImage pImageSource, YoonImage pImageObject)
        {
            return ImageFactory.Add(pImageSource, pImageObject);
        }

        public static YoonImage operator -(YoonImage pImageSource, YoonImage pImageObject)
        {
            return ImageFactory.Subtract(pImageSource, pImageObject);
        }

        public static bool operator ==(YoonImage pImageSource, YoonImage pImageObject)
        {
            return pImageSource?.Equals(pImageObject) == true;
        }

        public static bool operator !=(YoonImage pImageSource, YoonImage pImageObject)
        {
            return pImageSource?.Equals(pImageObject) == false;
        }
    }
}