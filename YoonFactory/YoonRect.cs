using System;
using System.Collections.Generic;
using System.Xml.Serialization;

namespace YoonFactory
{
    /// <summary>
    /// 사각형 대응 변수 (기준좌표계 : Pixel과 동일/ 좌+상)
    /// </summary>
    public class YoonRect2N : IYoonRect, IYoonRect2D<int>, IEquatable<YoonRect2N>
    {
        public override bool Equals(object obj)
        {
            return Equals(obj as IYoonRect);
        }

        public bool Equals(IYoonRect pRect)
        {
            if (pRect is not YoonRect2N pRect2N) return false;
            return CenterPos.X == pRect2N.CenterPos.X &&
                   CenterPos.Y == pRect2N.CenterPos.Y &&
                   Width == pRect2N.Width &&
                   Height == pRect2N.Height;
        }

        public IYoonRect Clone()
        {
            return new YoonRect2N
            {
                CenterPos = this.CenterPos.Clone() as YoonVector2N, Width = this.Width, Height = this.Height
            };
        }

        public void CopyFrom(IYoonRect pRect)
        {
            if (pRect is not YoonRect2N pRect2N) return;
            CenterPos = pRect2N.CenterPos.Clone() as YoonVector2N;
            Width = pRect2N.Width;
            Height = pRect2N.Height;
        }

        [XmlAttribute] public IYoonVector2D<int> CenterPos { get; set; }
        [XmlAttribute] public int Width { get; set; }
        [XmlAttribute] public int Height { get; set; }

        public int Left => ((YoonVector2N) CenterPos).X - Width / 2;

        public int Top => ((YoonVector2N) CenterPos).Y - Height / 2;

        public int Right => ((YoonVector2N) CenterPos).X + Width / 2;

        public int Bottom => ((YoonVector2N) CenterPos).Y + Height / 2;

        public IYoonVector2D<int> TopLeft => new YoonVector2N(CenterPos.X - Width / 2, CenterPos.Y - Height / 2);

        public IYoonVector2D<int> TopRight => new YoonVector2N(CenterPos.X + Width / 2, CenterPos.Y - Height / 2);

        public IYoonVector2D<int> BottomLeft => new YoonVector2N(CenterPos.X - Width / 2, CenterPos.Y + Height / 2);

        public IYoonVector2D<int> BottomRight => new YoonVector2N(CenterPos.X + Width / 2, CenterPos.Y + Height / 2);

        public void SetVerifiedArea(int nMinX, int nMinY, int nMaxX, int nMaxY)
        {
            int nLeft = (Left > nMinX) ? Left : nMinX;
            int nRight = (Right <= nMaxX) ? Right : nMaxX;
            int nTop = (Top > nMinY) ? Top : nMinY;
            int nBottom = (Bottom <= nMaxY) ? Bottom : nMaxY;
            CenterPos.X = (nLeft + nRight) / 2;
            CenterPos.Y = (nTop + nBottom) / 2;
            Width = nRight - nLeft;
            Height = nBottom - nTop;
        }

        public void SetVerifiedArea(IYoonVector2D<int> pMinVector, IYoonVector2D<int> pMaxVector)
        {
            SetVerifiedArea(pMinVector.X, pMinVector.Y, pMaxVector.X, pMaxVector.Y);
        }

        public YoonRect2N()
        {
            CenterPos = new YoonVector2N {X = 0, Y = 0};
            Width = 0;
            Height = 0;
        }

        public YoonRect2N(YoonVector2N pVector, int nWidth, int nHeight)
        {
            CenterPos = new YoonVector2N {X = pVector.X, Y = pVector.Y};
            Width = nWidth;
            Height = nHeight;
        }

        public YoonRect2N(int nX, int nY, int nWidth, int nHeight)
        {
            CenterPos = new YoonVector2N {X = nX, Y = nY};
            Width = nWidth;
            Height = nHeight;
        }

        public YoonRect2N(eYoonDir2D nDir1, YoonVector2N nVector1, eYoonDir2D nDir2, YoonVector2N nVector2)
        {
            if (nDir1 == eYoonDir2D.TopLeft && nDir2 == eYoonDir2D.BottomRight)
            {
                CenterPos = (nVector1 + nVector2) / 2;
                Width = nVector2.X - nVector1.X;
                Height = nVector2.Y - nVector1.Y;
            }
            else if (nDir1 == eYoonDir2D.BottomRight && nDir2 == eYoonDir2D.TopLeft)
            {
                CenterPos = (nVector1 + nVector2) / 2;
                Width = nVector1.X - nVector2.X;
                Height = nVector1.Y - nVector2.Y;
            }

            if (nDir1 == eYoonDir2D.TopRight && nDir2 == eYoonDir2D.BottomLeft)
            {
                CenterPos = (nVector1 + nVector2) / 2;
                Width = nVector2.X - nVector1.X;
                Height = nVector1.Y - nVector2.Y;
            }
            else if (nDir1 == eYoonDir2D.BottomLeft && nDir2 == eYoonDir2D.TopRight)
            {
                CenterPos = (nVector1 + nVector2) / 2;
                Width = nVector1.X - nVector2.X;
                Height = nVector2.Y - nVector1.Y;
            }
            else
                throw new ArgumentException("[YOONCOMMON] Direction Argument is not correct");
        }

        public YoonRect2N(List<YoonVector2N> pList)
        {
            if (pList.Count < 2)
            {
                CenterPos = pList[0].Clone() as YoonVector2N;
                Width = 0;
                Height = 0;
            }
            else
            {
                int nInitLeft = (pList[0].X > pList[1].X) ? pList[1].X : pList[0].X;
                int nInitRight = (pList[0].X < pList[1].X) ? pList[1].X : pList[0].X;
                int nInitTop = (pList[0].Y > pList[1].Y) ? pList[1].Y : pList[0].Y;
                int nInitBottom = (pList[0].Y < pList[1].Y) ? pList[1].Y : pList[0].Y;
                YoonVector2N pPosTopLeft = new YoonVector2N(nInitLeft, nInitTop);
                YoonVector2N pPosBottomRight = new YoonVector2N(nInitRight, nInitBottom);
                for (int iParam = 2; iParam < pList.Count; iParam++)
                {
                    eYoonDir2D nDirDiffTopLeft = pPosTopLeft.DirectionTo(pList[iParam]);
                    eYoonDir2D nDirDiffBottomRight = pPosBottomRight.DirectionTo(pList[iParam]);
                    switch (nDirDiffTopLeft)
                    {
                        case eYoonDir2D.TopLeft:
                            pPosTopLeft.X = pList[iParam].X;
                            pPosTopLeft.Y = pList[iParam].Y;
                            break;
                        case eYoonDir2D.Top:
                            pPosTopLeft.Y = pList[iParam].Y;
                            break;
                        case eYoonDir2D.TopRight:
                            pPosTopLeft.Y = pList[iParam].Y;
                            break;
                        case eYoonDir2D.Left:
                            pPosTopLeft.X = pList[iParam].X;
                            break;
                        case eYoonDir2D.BottomLeft:
                            pPosTopLeft.X = pList[iParam].X;
                            break;
                        case eYoonDir2D.None:
                            break;
                        case eYoonDir2D.Center:
                            break;
                        case eYoonDir2D.Right:
                            break;
                        case eYoonDir2D.BottomRight:
                            break;
                        case eYoonDir2D.Bottom:
                            break;
                        default:
                            break;
                    }

                    switch (nDirDiffBottomRight)
                    {
                        case eYoonDir2D.TopRight:
                            pPosBottomRight.X = pList[iParam].X;
                            break;
                        case eYoonDir2D.Right:
                            pPosBottomRight.X = pList[iParam].X;
                            break;
                        case eYoonDir2D.BottomLeft:
                            pPosBottomRight.Y = pList[iParam].Y;
                            break;
                        case eYoonDir2D.Bottom:
                            pPosBottomRight.Y = pList[iParam].Y;
                            break;
                        case eYoonDir2D.BottomRight:
                            pPosBottomRight.X = pList[iParam].X;
                            pPosBottomRight.Y = pList[iParam].Y;
                            break;
                        case eYoonDir2D.None:
                            break;
                        case eYoonDir2D.Center:
                            break;
                        case eYoonDir2D.TopLeft:
                            break;
                        case eYoonDir2D.Top:
                            break;
                        case eYoonDir2D.Left:
                            break;
                        default:
                            break;
                    }
                }

                CenterPos = (pPosTopLeft + pPosBottomRight) / 2;
                Width = (pPosBottomRight - pPosTopLeft).X;
                Height = (pPosBottomRight - pPosTopLeft).Y;
            }
        }

        public YoonRect2N(params YoonVector2N[] pArgs)
        {
            if (pArgs.Length < 2)
            {
                CenterPos = pArgs[0].Clone() as YoonVector2N;
                Width = 0;
                Height = 0;
            }
            else
            {
                int nInitLeft = (pArgs[0].X > pArgs[1].X) ? pArgs[1].X : pArgs[0].X;
                int nInitRight = (pArgs[0].X < pArgs[1].X) ? pArgs[1].X : pArgs[0].X;
                int nInitTop = (pArgs[0].Y > pArgs[1].Y) ? pArgs[1].Y : pArgs[0].Y;
                int nInitBottom = (pArgs[0].Y < pArgs[1].Y) ? pArgs[1].Y : pArgs[0].Y;
                YoonVector2N pPosTopLeft = new YoonVector2N(nInitLeft, nInitTop);
                YoonVector2N pPosBottomRight = new YoonVector2N(nInitRight, nInitBottom);
                for (int iParam = 2; iParam < pArgs.Length; iParam++)
                {
                    eYoonDir2D nDirDiffTopLeft = pPosTopLeft.DirectionTo(pArgs[iParam]);
                    eYoonDir2D nDirDiffBottomRight = pPosBottomRight.DirectionTo(pArgs[iParam]);
                    switch (nDirDiffTopLeft)
                    {
                        case eYoonDir2D.TopLeft:
                            pPosTopLeft.X = pArgs[iParam].X;
                            pPosTopLeft.Y = pArgs[iParam].Y;
                            break;
                        case eYoonDir2D.Top:
                            pPosTopLeft.Y = pArgs[iParam].Y;
                            break;
                        case eYoonDir2D.TopRight:
                            pPosTopLeft.Y = pArgs[iParam].Y;
                            break;
                        case eYoonDir2D.Left:
                            pPosTopLeft.X = pArgs[iParam].X;
                            break;
                        case eYoonDir2D.BottomLeft:
                            pPosTopLeft.X = pArgs[iParam].X;
                            break;
                        case eYoonDir2D.None:
                            break;
                        case eYoonDir2D.Center:
                            break;
                        case eYoonDir2D.Right:
                            break;
                        case eYoonDir2D.BottomRight:
                            break;
                        case eYoonDir2D.Bottom:
                            break;
                        default:
                            break;
                    }

                    switch (nDirDiffBottomRight)
                    {
                        case eYoonDir2D.TopRight:
                            pPosBottomRight.X = pArgs[iParam].X;
                            break;
                        case eYoonDir2D.Right:
                            pPosBottomRight.X = pArgs[iParam].X;
                            break;
                        case eYoonDir2D.BottomLeft:
                            pPosBottomRight.Y = pArgs[iParam].Y;
                            break;
                        case eYoonDir2D.Bottom:
                            pPosBottomRight.Y = pArgs[iParam].Y;
                            break;
                        case eYoonDir2D.BottomRight:
                            pPosBottomRight.X = pArgs[iParam].X;
                            pPosBottomRight.Y = pArgs[iParam].Y;
                            break;
                        case eYoonDir2D.None:
                            break;
                        case eYoonDir2D.Center:
                            break;
                        case eYoonDir2D.TopLeft:
                            break;
                        case eYoonDir2D.Top:
                            break;
                        case eYoonDir2D.Left:
                            break;
                        default:
                            break;
                    }
                }

                CenterPos = (pPosTopLeft + pPosBottomRight) / 2;
                Width = (pPosBottomRight - pPosTopLeft).X;
                Height = (pPosBottomRight - pPosTopLeft).Y;
            }
        }

        public bool IsContain(IYoonVector pVector)
        {
            if (pVector is not YoonVector2D pVector2D) return false;
            return Left < pVector2D.X && pVector2D.X < Right &&
                   Top < pVector2D.Y && pVector2D.Y < Bottom;
        }

        public double Area()
        {
            return Width * Height;
        }

        public bool Equals(YoonRect2N other)
        {
            return other != null &&
                   EqualityComparer<IYoonVector2D<int>>.Default.Equals(CenterPos, other.CenterPos) &&
                   Width == other.Width &&
                   Height == other.Height &&
                   Left == other.Left &&
                   Top == other.Top &&
                   Right == other.Right &&
                   Bottom == other.Bottom &&
                   EqualityComparer<IYoonVector2D<int>>.Default.Equals(TopLeft, other.TopLeft) &&
                   EqualityComparer<IYoonVector2D<int>>.Default.Equals(TopRight, other.TopRight) &&
                   EqualityComparer<IYoonVector2D<int>>.Default.Equals(BottomLeft, other.BottomLeft) &&
                   EqualityComparer<IYoonVector2D<int>>.Default.Equals(BottomRight, other.BottomRight);
        }

        public override int GetHashCode()
        {
            int hashCode = 845742892;
            hashCode = hashCode * -1521134295 + EqualityComparer<IYoonVector2D<int>>.Default.GetHashCode(CenterPos);
            hashCode = hashCode * -1521134295 + Width.GetHashCode();
            hashCode = hashCode * -1521134295 + Height.GetHashCode();
            hashCode = hashCode * -1521134295 + Left.GetHashCode();
            hashCode = hashCode * -1521134295 + Top.GetHashCode();
            hashCode = hashCode * -1521134295 + Right.GetHashCode();
            hashCode = hashCode * -1521134295 + Bottom.GetHashCode();
            hashCode = hashCode * -1521134295 + EqualityComparer<IYoonVector2D<int>>.Default.GetHashCode(TopLeft);
            hashCode = hashCode * -1521134295 + EqualityComparer<IYoonVector2D<int>>.Default.GetHashCode(TopRight);
            hashCode = hashCode * -1521134295 + EqualityComparer<IYoonVector2D<int>>.Default.GetHashCode(BottomLeft);
            hashCode = hashCode * -1521134295 + EqualityComparer<IYoonVector2D<int>>.Default.GetHashCode(BottomRight);
            return hashCode;
        }

        IYoonFigure IYoonFigure.Clone()
        {
            return Clone();
        }

        public IYoonVector2D<int> GetPosition(eYoonDir2D nDir)
        {
            return nDir switch
            {
                eYoonDir2D.TopLeft => TopLeft,
                eYoonDir2D.Top => new YoonVector2N(CenterPos.X, Top),
                eYoonDir2D.TopRight => TopRight,
                eYoonDir2D.Left => new YoonVector2N(Left, CenterPos.Y),
                eYoonDir2D.Center => CenterPos,
                eYoonDir2D.Right => new YoonVector2N(Right, CenterPos.Y),
                eYoonDir2D.BottomLeft => BottomLeft,
                eYoonDir2D.Bottom => new YoonVector2N(CenterPos.X, Bottom),
                eYoonDir2D.BottomRight => BottomRight,
                _ => new YoonVector2N()
            };
        }

        public YoonRect2D ToRect2D()
        {
            return new YoonRect2D(CenterPos.X, CenterPos.Y, Width, Height);
        }

        public static YoonRect2N operator +(YoonRect2N pRectSource, YoonRect2N pRectObject)
        {
            int nTop = Math.Min(pRectSource.Top, pRectObject.Top);
            int nBottom = Math.Max(pRectSource.Bottom, pRectObject.Bottom);
            int nLeft = Math.Min(pRectSource.Left, pRectObject.Left);
            int nRight = Math.Max(pRectSource.Right, pRectObject.Right);
            return new YoonRect2N((nLeft + nRight) / 2, (nTop + nBottom) / 2, nRight - nLeft, nBottom - nTop);
        }

        public static bool operator ==(YoonRect2N pRectSource, YoonRect2N pRectObject)
        {
            return pRectSource?.Equals(pRectObject) == true;
        }

        public static bool operator !=(YoonRect2N pRectSource, YoonRect2N pRectObject)
        {
            return pRectSource?.Equals(pRectObject) == false;
        }
    }

    /// <summary>
    /// 사각형 대응 변수
    /// </summary>
    public class YoonRect2D : IYoonRect, IYoonRect2D<double>, IEquatable<YoonRect2D>
    {
        public override bool Equals(object obj)
        {
            return Equals(obj as IYoonRect);
        }

        public bool Equals(IYoonRect pRect)
        {
            if (pRect is not YoonRect2D pRect2D) return false;
            return CenterPos.X == pRect2D.CenterPos.X &&
                   CenterPos.Y == pRect2D.CenterPos.Y &&
                   Width == pRect2D.Width &&
                   Height == pRect2D.Height;
        }

        public IYoonRect Clone()
        {
            return new YoonRect2D
            {
                CenterPos = this.CenterPos.Clone() as YoonVector2D, Width = this.Width, Height = this.Height
            };
        }

        public void CopyFrom(IYoonRect pRect)
        {
            if (pRect is not YoonRect2D pRect2D) return;
            CenterPos = pRect2D.CenterPos.Clone() as YoonVector2D;
            Width = pRect2D.Width;
            Height = pRect2D.Height;
        }

        [XmlAttribute] public IYoonVector2D<double> CenterPos { get; set; }
        [XmlAttribute] public double Width { get; set; }
        [XmlAttribute] public double Height { get; set; }

        public double Left => CenterPos.X - Width / 2;

        public double Top => CenterPos.Y - Height / 2;

        public double Right => CenterPos.X + Width / 2;

        public double Bottom => CenterPos.Y + Height / 2;

        public IYoonVector2D<double> TopLeft => new YoonVector2D(CenterPos.X - Width / 2, CenterPos.Y - Height / 2);

        public IYoonVector2D<double> TopRight => new YoonVector2D(CenterPos.X + Width / 2, CenterPos.Y - Height / 2);

        public IYoonVector2D<double> BottomLeft => new YoonVector2D(CenterPos.X - Width / 2, CenterPos.Y + Height / 2);

        public IYoonVector2D<double> BottomRight => new YoonVector2D(CenterPos.X + Width / 2, CenterPos.Y + Height / 2);

        public void SetVerifiedArea(double dMinX, double dMinY, double dMaxX, double dMaxY)
        {
            double dLeft = (Left > dMinX) ? Left : dMinX;
            double dRight = (Right <= dMaxX) ? Right : dMaxX;
            double dTop = (Top > dMinY) ? Top : dMinY;
            double dBottom = (Bottom <= dMaxY) ? Bottom : dMaxY;
            CenterPos.X = (dLeft + dRight) / 2;
            CenterPos.Y = (dTop + dBottom) / 2;
            Width = dRight - dLeft;
            Height = dBottom - dTop;
        }

        public void SetVerifiedArea(IYoonVector2D<double> pMinVector, IYoonVector2D<double> pMaxVector)
        {
            SetVerifiedArea(pMinVector.X, pMinVector.Y, pMaxVector.X, pMaxVector.Y);
        }

        public YoonRect2D()
        {
            CenterPos = new YoonVector2D {X = 0, Y = 0};
            Width = 0;
            Height = 0;
        }

        public YoonRect2D(YoonVector2D pVector, double dWidth, double dHeight)
        {
            CenterPos = new YoonVector2D {X = pVector.X, Y = pVector.Y};
            Width = dWidth;
            Height = dHeight;
        }

        public YoonRect2D(double dX, double dY, double dWidth, double dHeight)
        {
            CenterPos = new YoonVector2D {X = dX, Y = dY};
            Width = dWidth;
            Height = dHeight;
        }

        public YoonRect2D(eYoonDir2D nDir1, YoonVector2D pVector1, eYoonDir2D nDir2, YoonVector2D pVector2)
        {
            switch (nDir1)
            {
                case eYoonDir2D.TopLeft when nDir2 == eYoonDir2D.BottomRight:
                    CenterPos = (pVector1 + pVector2) / 2;
                    Width = pVector2.X - pVector1.X;
                    Height = pVector2.Y - pVector1.Y;
                    break;
                case eYoonDir2D.BottomRight when nDir2 == eYoonDir2D.TopLeft:
                    CenterPos = (pVector1 + pVector2) / 2;
                    Width = pVector1.X - pVector2.X;
                    Height = pVector1.Y - pVector2.Y;
                    break;
            }

            switch (nDir1)
            {
                case eYoonDir2D.TopRight when nDir2 == eYoonDir2D.BottomLeft:
                    CenterPos = (pVector1 + pVector2) / 2;
                    Width = pVector2.X - pVector1.X;
                    Height = pVector1.Y - pVector2.Y;
                    break;
                case eYoonDir2D.BottomLeft when nDir2 == eYoonDir2D.TopRight:
                    CenterPos = (pVector1 + pVector2) / 2;
                    Width = pVector1.X - pVector2.X;
                    Height = pVector2.Y - pVector1.Y;
                    break;
                default:
                    throw new ArgumentException("[YOONCOMMON] Direction Argument is not correct");
            }
        }

        public YoonRect2D(List<YoonVector2D> pList)
        {
            if (pList.Count < 2)
            {
                CenterPos = pList[0].Clone() as YoonVector2D;
                Width = 0;
                Height = 0;
            }
            else
            {
                double dInitLeft = (pList[0].X > pList[1].X) ? pList[1].X : pList[0].X;
                double dInitRight = (pList[0].X < pList[1].X) ? pList[1].X : pList[0].X;
                double dInitTop = (pList[0].Y > pList[1].Y) ? pList[1].Y : pList[0].Y;
                double dInitBottom = (pList[0].Y < pList[1].Y) ? pList[1].Y : pList[0].Y;
                YoonVector2D pVectorTopLeft = new YoonVector2D(dInitLeft, dInitTop);
                YoonVector2D pVectorBottomRight = new YoonVector2D(dInitRight, dInitBottom);
                for (int iParam = 2; iParam < pList.Count; iParam++)
                {
                    eYoonDir2D nDirDiffTopLeft = pVectorTopLeft.DirectionTo(pList[iParam]);
                    eYoonDir2D nDirDiffBottomRight = pVectorBottomRight.DirectionTo(pList[iParam]);
                    switch (nDirDiffTopLeft)
                    {
                        case eYoonDir2D.TopLeft:
                            pVectorTopLeft.X = pList[iParam].X;
                            pVectorTopLeft.Y = pList[iParam].Y;
                            break;
                        case eYoonDir2D.Top:
                            pVectorTopLeft.Y = pList[iParam].Y;
                            break;
                        case eYoonDir2D.TopRight:
                            pVectorTopLeft.Y = pList[iParam].Y;
                            break;
                        case eYoonDir2D.Left:
                            pVectorTopLeft.X = pList[iParam].X;
                            break;
                        case eYoonDir2D.BottomLeft:
                            pVectorTopLeft.X = pList[iParam].X;
                            break;
                        case eYoonDir2D.None:
                            break;
                        case eYoonDir2D.Center:
                            break;
                        case eYoonDir2D.Right:
                            break;
                        case eYoonDir2D.BottomRight:
                            break;
                        case eYoonDir2D.Bottom:
                            break;
                        default:
                            break;
                    }

                    switch (nDirDiffBottomRight)
                    {
                        case eYoonDir2D.TopRight:
                            pVectorBottomRight.X = pList[iParam].X;
                            break;
                        case eYoonDir2D.Right:
                            pVectorBottomRight.X = pList[iParam].X;
                            break;
                        case eYoonDir2D.BottomLeft:
                            pVectorBottomRight.Y = pList[iParam].Y;
                            break;
                        case eYoonDir2D.Bottom:
                            pVectorBottomRight.Y = pList[iParam].Y;
                            break;
                        case eYoonDir2D.BottomRight:
                            pVectorBottomRight.X = pList[iParam].X;
                            pVectorBottomRight.Y = pList[iParam].Y;
                            break;
                        case eYoonDir2D.None:
                            break;
                        case eYoonDir2D.Center:
                            break;
                        case eYoonDir2D.TopLeft:
                            break;
                        case eYoonDir2D.Top:
                            break;
                        case eYoonDir2D.Left:
                            break;
                        default:
                            break;
                    }
                }

                CenterPos = (pVectorTopLeft + pVectorBottomRight) / 2;
                Width = (pVectorBottomRight - pVectorTopLeft).X;
                Height = (pVectorBottomRight - pVectorTopLeft).Y;
            }
        }

        public YoonRect2D(params YoonVector2D[] pArgs)
        {
            if (pArgs.Length < 2)
            {
                CenterPos = pArgs[0].Clone() as YoonVector2D;
                Width = 0;
                Height = 0;
            }
            else
            {
                double dInitLeft = (pArgs[0].X > pArgs[1].X) ? pArgs[1].X : pArgs[0].X;
                double dInitRight = (pArgs[0].X < pArgs[1].X) ? pArgs[1].X : pArgs[0].X;
                double dInitTop = (pArgs[0].Y > pArgs[1].Y) ? pArgs[1].Y : pArgs[0].Y;
                double dInitBottom = (pArgs[0].Y < pArgs[1].Y) ? pArgs[1].Y : pArgs[0].Y;
                YoonVector2D pVectorTopLeft = new YoonVector2D(dInitLeft, dInitTop);
                YoonVector2D pVectorBottomRight = new YoonVector2D(dInitRight, dInitBottom);
                for (int iParam = 2; iParam < pArgs.Length; iParam++)
                {
                    eYoonDir2D nDirDiffTopLeft = pVectorTopLeft.DirectionTo(pArgs[iParam]);
                    eYoonDir2D nDirDiffBottomRight = pVectorBottomRight.DirectionTo(pArgs[iParam]);
                    switch (nDirDiffTopLeft)
                    {
                        case eYoonDir2D.TopLeft:
                            pVectorTopLeft.X = pArgs[iParam].X;
                            pVectorTopLeft.Y = pArgs[iParam].Y;
                            break;
                        case eYoonDir2D.Top:
                            pVectorTopLeft.Y = pArgs[iParam].Y;
                            break;
                        case eYoonDir2D.TopRight:
                            pVectorTopLeft.Y = pArgs[iParam].Y;
                            break;
                        case eYoonDir2D.Left:
                            pVectorTopLeft.X = pArgs[iParam].X;
                            break;
                        case eYoonDir2D.BottomLeft:
                            pVectorTopLeft.X = pArgs[iParam].X;
                            break;
                        case eYoonDir2D.None:
                            break;
                        case eYoonDir2D.Center:
                            break;
                        case eYoonDir2D.Right:
                            break;
                        case eYoonDir2D.BottomRight:
                            break;
                        case eYoonDir2D.Bottom:
                            break;
                        default:
                            break;
                    }

                    switch (nDirDiffBottomRight)
                    {
                        case eYoonDir2D.TopRight:
                            pVectorBottomRight.X = pArgs[iParam].X;
                            break;
                        case eYoonDir2D.Right:
                            pVectorBottomRight.X = pArgs[iParam].X;
                            break;
                        case eYoonDir2D.BottomLeft:
                            pVectorBottomRight.Y = pArgs[iParam].Y;
                            break;
                        case eYoonDir2D.Bottom:
                            pVectorBottomRight.Y = pArgs[iParam].Y;
                            break;
                        case eYoonDir2D.BottomRight:
                            pVectorBottomRight.X = pArgs[iParam].X;
                            pVectorBottomRight.Y = pArgs[iParam].Y;
                            break;
                        case eYoonDir2D.None:
                            break;
                        case eYoonDir2D.Center:
                            break;
                        case eYoonDir2D.TopLeft:
                            break;
                        case eYoonDir2D.Top:
                            break;
                        case eYoonDir2D.Left:
                            break;
                        default:
                            break;
                    }
                }

                CenterPos = (pVectorTopLeft + pVectorBottomRight) / 2;
                Width = (pVectorBottomRight - pVectorTopLeft).X;
                Height = (pVectorBottomRight - pVectorTopLeft).Y;
            }
        }

        public bool IsContain(IYoonVector pVector)
        {
            if (pVector is not YoonVector2D pVector2D) return false;
            return Left < pVector2D.X && pVector2D.X < Right &&
                   Top < pVector2D.Y && pVector2D.Y < Bottom;
        }

        public double Area()
        {
            return Width * Height;
        }

        public bool Equals(YoonRect2D other)
        {
            return other != null &&
                   EqualityComparer<IYoonVector2D<double>>.Default.Equals(CenterPos, other.CenterPos) &&
                   Width == other.Width &&
                   Height == other.Height &&
                   Left == other.Left &&
                   Top == other.Top &&
                   Right == other.Right &&
                   Bottom == other.Bottom &&
                   EqualityComparer<IYoonVector2D<double>>.Default.Equals(TopLeft, other.TopLeft) &&
                   EqualityComparer<IYoonVector2D<double>>.Default.Equals(TopRight, other.TopRight) &&
                   EqualityComparer<IYoonVector2D<double>>.Default.Equals(BottomLeft, other.BottomLeft) &&
                   EqualityComparer<IYoonVector2D<double>>.Default.Equals(BottomRight, other.BottomRight);
        }

        public override int GetHashCode()
        {
            int hashCode = 845742892;
            hashCode = hashCode * -1521134295 + EqualityComparer<IYoonVector2D<double>>.Default.GetHashCode(CenterPos);
            hashCode = hashCode * -1521134295 + Width.GetHashCode();
            hashCode = hashCode * -1521134295 + Height.GetHashCode();
            hashCode = hashCode * -1521134295 + Left.GetHashCode();
            hashCode = hashCode * -1521134295 + Top.GetHashCode();
            hashCode = hashCode * -1521134295 + Right.GetHashCode();
            hashCode = hashCode * -1521134295 + Bottom.GetHashCode();
            hashCode = hashCode * -1521134295 + EqualityComparer<IYoonVector2D<double>>.Default.GetHashCode(TopLeft);
            hashCode = hashCode * -1521134295 + EqualityComparer<IYoonVector2D<double>>.Default.GetHashCode(TopRight);
            hashCode = hashCode * -1521134295 + EqualityComparer<IYoonVector2D<double>>.Default.GetHashCode(BottomLeft);
            hashCode = hashCode * -1521134295 +
                       EqualityComparer<IYoonVector2D<double>>.Default.GetHashCode(BottomRight);
            return hashCode;
        }

        IYoonFigure IYoonFigure.Clone()
        {
            return Clone();
        }

        public IYoonVector2D<double> GetPosition(eYoonDir2D nDir)
        {
            return nDir switch
            {
                eYoonDir2D.TopLeft => TopLeft,
                eYoonDir2D.Top => new YoonVector2D(CenterPos.X, Top),
                eYoonDir2D.TopRight => TopRight,
                eYoonDir2D.Left => new YoonVector2D(Left, CenterPos.Y),
                eYoonDir2D.Center => CenterPos,
                eYoonDir2D.Right => new YoonVector2D(Right, CenterPos.Y),
                eYoonDir2D.BottomLeft => BottomLeft,
                eYoonDir2D.Bottom => new YoonVector2D(CenterPos.X, Bottom),
                eYoonDir2D.BottomRight => BottomRight,
                _ => new YoonVector2D()
            };
        }

        public YoonRect2N ToRect2N()
        {
            return new YoonRect2N((int) CenterPos.X, (int) CenterPos.Y, (int) Width, (int) Height);
        }

        public static YoonRect2D operator +(YoonRect2D pRectSource, YoonRect2D pRectObject)
        {
            double nTop = Math.Min(pRectSource.Top, pRectObject.Top);
            double nBottom = Math.Max(pRectSource.Bottom, pRectObject.Bottom);
            double nLeft = Math.Min(pRectSource.Left, pRectObject.Left);
            double nRight = Math.Max(pRectSource.Right, pRectObject.Right);
            return new YoonRect2D((nLeft + nRight) / 2, (nTop + nBottom) / 2, nRight - nLeft, nBottom - nTop);
        }

        public static bool operator ==(YoonRect2D pRectSource, YoonRect2D pRectObject)
        {
            return pRectSource?.Equals(pRectObject) == true;
        }

        public static bool operator !=(YoonRect2D pRectSource, YoonRect2D pRectObject)
        {
            return pRectSource?.Equals(pRectObject) == false;
        }
    }
}
