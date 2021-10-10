using System.Xml.Serialization;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using static System.Double;
using static System.Int32;

namespace YoonFactory
{
    public class YoonLine2N : IYoonLine, IYoonLine2D<int>, IEquatable<YoonLine2N>
    {
        private const int INVALID_NUM = -65536;

        [XmlAnyAttribute] public IYoonVector2D<int> StartPos { get; private set; } = new YoonVector2N(0, 0);
        [XmlAnyAttribute] public IYoonVector2D<int> EndPos { get; private set; } = new YoonVector2N(0, 0);

        public int PropertiesCount => 4;  // START X, START Y, END X, END Y
        
        public IYoonVector2D<int> CenterPos =>
            new YoonVector2N((StartPos.X + EndPos.X) / 2, (StartPos.Y + EndPos.Y) / 2);

        public double Length => Math.Sqrt(Math.Pow(StartPos.X - EndPos.X, 2.0) + Math.Pow(StartPos.Y - EndPos.Y, 2.0));

        public double Slope { get; private set; } = 0.0;
        
        public double Constant { get; private set; } = 0.0;
        
        IYoonRect2D<int> IYoonLine2D<int>.Area => new YoonRect2N((YoonVector2N) StartPos, (YoonVector2N) EndPos);

        public void FromArgs(params string[] pArgs)
        {
            if (pArgs.Length != PropertiesCount) return;
            int nStartX = int.Parse(pArgs[0]);
            int nStartY = int.Parse(pArgs[1]);
            int nEndX = int.Parse(pArgs[2]);
            int nEndY = int.Parse(pArgs[3]);
            StartPos = new YoonVector2N(nStartX, nStartY);
            EndPos = new YoonVector2N(nEndX, nEndY);
            Slope = (nEndY - nStartY) / (nEndX == nStartX ? 0.00001 : nEndX - nStartX);
            Constant = nStartY - Slope * nStartX;
        }

        public override bool Equals(object obj)
        {
            return Equals(obj as IYoonLine);
        }

        public bool Equals(IYoonLine pLine)
        {
            if (pLine is YoonLine2N pLine2N)
            {
                if (Slope == pLine2N.Slope &&
                    Constant == pLine2N.Constant)
                    return true;
            }

            return false;
        }

        public IYoonLine Clone()
        {
            return new YoonLine2N
            {
                StartPos = StartPos.Clone() as YoonVector2N, EndPos = EndPos.Clone() as YoonVector2N, Slope = Slope,
                Constant = Constant
            };
        }

        public void CopyFrom(IYoonLine pLine)
        {
            if (pLine is not YoonLine2N pLine2N) return;
            StartPos = pLine2N.StartPos.Clone() as YoonVector2N;
            EndPos = pLine2N.EndPos.Clone() as YoonVector2N;
            Slope = pLine2N.Slope;
            Constant = pLine2N.Constant;
        }

        public YoonLine2N()
        {
            //
        }

        public YoonLine2N(int nStartX, int nStartY, int nEndX, int nEndY)
        {
            StartPos = new YoonVector2N(nStartX, nStartY);
            EndPos = new YoonVector2N(nEndX, nEndY);
            Slope = (nEndY - nStartY) / (nEndX == nStartX ? 0.00001 : nEndX - nStartX);
            Constant = nStartY - Slope * nStartX;
        }

        public YoonLine2N(double slope, double constant)
        {
            Slope = slope;
            Constant = constant;
            StartPos = new YoonVector2N(-1, Y(-1));
            EndPos = new YoonVector2N(1, Y(1));
        }

        public YoonLine2N(List<YoonVector2N> pList, eYoonDir2D nDirArrange = eYoonDir2D.None)
        {
            if (pList == null || pList.Count < 2)
            {
                Debug.WriteLine("[YOONCOMMON] Input array is abnormal");
                StartPos = new YoonVector2N(INVALID_NUM, INVALID_NUM);
                EndPos = new YoonVector2N(INVALID_NUM, INVALID_NUM);
                return;
            }

            if (pList.Count > 10)
            {
                List<YoonVector2N> pListSorted = new List<YoonVector2N>(pList);
                pList.Clear();
                MathFactory.SortVector(ref pListSorted, nDirArrange);
                int nStart = (pListSorted.Count - 10) / 2;
                for (int i = 0; i < 10; i++)
                    pList.Add(pListSorted[i + nStart]);
                pListSorted.Clear();
            }

            int[] pX = new int[pList.Count];
            int[] pY = new int[pList.Count];
            int nMinX = 65535;
            int nMaxX = -65535;
            for (int iVector = 0; iVector < pList.Count; iVector++)
            {
                pX[iVector] = pList[iVector].X;
                pY[iVector] = pList[iVector].Y;
                if (nMinX > pX[iVector]) nMinX = pX[iVector];
                if (nMaxX < pX[iVector]) nMaxX = pX[iVector];
            }

            double dSlope = 0.0;
            double dConstant = 0.0;
            if (!MathFactory.LeastSquare(ref dSlope, ref dConstant, pList.Count, pX, pY))
                return;
            Slope = dSlope;
            Constant = dConstant;
            StartPos = new YoonVector2N(nMinX, Y(nMinX));
            EndPos = new YoonVector2N(nMaxX, Y(nMaxX));
        }

        public YoonLine2N(params YoonVector2N[] pArgs)
        {
            if (pArgs.Length < 2 && pArgs.Length >= 10)
            {
                Debug.WriteLine("[YOONCOMMON] Input array is abnormal");
                StartPos = new YoonVector2N(INVALID_NUM, INVALID_NUM);
                EndPos = new YoonVector2N(INVALID_NUM, INVALID_NUM);
                return;
            }

            int[] pX = new int[pArgs.Length];
            int[] pY = new int[pArgs.Length];
            int nMinX = 65535;
            int nMaxX = -65535;
            for (int iVector = 0; iVector < pArgs.Length; iVector++)
            {
                pX[iVector] = pArgs[iVector].X;
                pY[iVector] = pArgs[iVector].Y;
                if (nMinX > pX[iVector]) nMinX = pX[iVector];
                if (nMaxX < pX[iVector]) nMaxX = pX[iVector];
            }

            double dSlope = 0.0;
            double dConstant = 0.0;
            if (!MathFactory.LeastSquare(ref dSlope, ref dConstant, pArgs.Length, pX, pY))
                return;
            Slope = dSlope;
            Constant = dConstant;
            StartPos = new YoonVector2N(nMinX, Y(nMinX));
            EndPos = new YoonVector2N(nMaxX, Y(nMaxX));
        }

        public int X(int nY)
        {
            return (int) ((nY - Constant) / Slope);
        }

        public int Y(int nX)
        {
            return (int) (nX * Slope + Constant);
        }

        public IYoonVector2D<int> Intersection(IYoonLine pLine)
        {
            YoonVector2N pResultVector = new YoonVector2N(INVALID_NUM, INVALID_NUM);
            if (pLine is not YoonLine2N pLine2N) return pResultVector;
            // Check the invalid line
            if ((YoonVector2N) StartPos == new YoonVector2N(INVALID_NUM, INVALID_NUM) ||
                (YoonVector2N) EndPos == new YoonVector2N(INVALID_NUM, INVALID_NUM) ||
                (YoonVector2N) pLine2N.StartPos == new YoonVector2N(INVALID_NUM, INVALID_NUM) ||
                (YoonVector2N) pLine2N.EndPos == new YoonVector2N(INVALID_NUM, INVALID_NUM))
                return pResultVector;
            // Two lines parallel
            if (Slope == pLine2N.Slope) return pResultVector;
            pResultVector.X = (int) (-(Constant - pLine.Constant) / (Slope - pLine.Slope));
            pResultVector.Y = Y(pResultVector.X);
            return pResultVector;
        }

        public void Fit(IYoonRect2D<int> pRect)
        {
            int nStartX = Math.Min(pRect.Left, pRect.Right);
            int nStartY = Math.Min(pRect.Top, pRect.Bottom);
            int nEndX = Math.Max(pRect.Left, pRect.Right);
            int nEndY = Math.Max(pRect.Top, pRect.Bottom);
            StartPos.X = X(nStartY) > nStartX ? X(nStartY) : nStartX;
            StartPos.Y = Y(nStartX) > nStartY ? Y(nStartX) : nStartY;
            EndPos.X = X(nEndY) > nEndX ? nEndX : X(nEndY);
            EndPos.Y = Y(nEndX) > nEndY ? nEndY : Y(nEndX);
        }

        public void Fit(int width, int height)
        {
            StartPos.X = X(0) > 0 ? X(0) : 0;
            StartPos.Y = Y(0) > 0 ? Y(0) : 0;
            EndPos.X = X(height) > width ? width : X(height);
            EndPos.Y = Y(width) > height ? height : Y(width);
        }

        public bool IsContain(IYoonVector pVector)
        {
            return pVector switch
            {
                YoonVector2N pVector2N => pVector2N.Y == pVector2N.X * Slope + Constant,
                YoonVector2D pVector2D => pVector2D.Y == pVector2D.X * Slope + Constant,
                _ => false
            };
        }

        public double Distance(IYoonVector pVector)
        {
            return pVector switch
            {
                YoonVector2N pVector2N => Math.Abs(Slope * pVector2N.X - pVector2N.Y + Constant) /
                                          Math.Sqrt(Slope * Slope + 1),
                YoonVector2D pVector2D => Math.Abs(Slope * pVector2D.X - pVector2D.Y + Constant) /
                                          Math.Sqrt(Slope * Slope + 1),
                _ => throw new ArgumentException("[YOONCOMMON] Vector argument format is not correct")
            };
        }

        public bool Equals(YoonLine2N other)
        {
            return other != null &&
                   EqualityComparer<IYoonVector2D<int>>.Default.Equals(StartPos, other.StartPos) &&
                   EqualityComparer<IYoonVector2D<int>>.Default.Equals(EndPos, other.EndPos) &&
                   EqualityComparer<IYoonVector2D<int>>.Default.Equals(CenterPos, other.CenterPos) &&
                   Length == other.Length &&
                   Slope == other.Slope &&
                   Constant == other.Constant;
        }

        public override int GetHashCode()
        {
            int hashCode = 1569030335;
            hashCode = hashCode * -1521134295 + EqualityComparer<IYoonVector2D<int>>.Default.GetHashCode(StartPos);
            hashCode = hashCode * -1521134295 + EqualityComparer<IYoonVector2D<int>>.Default.GetHashCode(EndPos);
            hashCode = hashCode * -1521134295 + EqualityComparer<IYoonVector2D<int>>.Default.GetHashCode(CenterPos);
            hashCode = hashCode * -1521134295 + Length.GetHashCode();
            hashCode = hashCode * -1521134295 + Slope.GetHashCode();
            hashCode = hashCode * -1521134295 + Constant.GetHashCode();
            return hashCode;
        }

        IYoonFigure IYoonFigure.Clone()
        {
            return Clone();
        }

        public YoonLine2D ToLine2D()
        {
            return new YoonLine2D(StartPos.X, StartPos.Y, EndPos.X, EndPos.Y);
        }
        
        public static YoonLine2D[] ToLine2Ns(YoonLine2N[] pLines)
        {
            YoonLine2D[] pResultLine = new YoonLine2D[pLines.Length];
            for (int iLine = 0; iLine < pLines.Length; iLine++)
            {
                pResultLine[iLine] = pLines[iLine].ToLine2D();
            }

            return pResultLine;
        }

        public static bool operator ==(YoonLine2N l1, YoonLine2N l2)
        {
            return l1?.Equals(l2) == true;
        }

        public static bool operator !=(YoonLine2N l1, YoonLine2N l2)
        {
            return l1?.Equals(l2) == false;
        }
    }

    public class YoonLine2D : IYoonLine, IYoonLine2D<double>, IEquatable<YoonLine2D>
    {
        private const double INVALID_NUM = -65536.00;
        
        [XmlAnyAttribute] public IYoonVector2D<double> StartPos { get; private set; } = new YoonVector2D(0, 0);
        [XmlAnyAttribute] public IYoonVector2D<double> EndPos { get; private set; } = new YoonVector2D(0, 0);

        public int PropertiesCount => 4;  // START X, START Y, END X, END Y
        
        public IYoonVector2D<double> CenterPos =>
            new YoonVector2D((StartPos.X + EndPos.X) / 2, (StartPos.Y + EndPos.Y) / 2);

        public double Length => Math.Sqrt(Math.Pow(StartPos.X - EndPos.X, 2.0) + Math.Pow(StartPos.Y - EndPos.Y, 2.0));

        public double Slope { get; private set; } = 0.0;
        
        public double Constant { get; private set; } = 0.0;
        
        public IYoonRect2D<double> Area => new YoonRect2D((YoonVector2D) StartPos, (YoonVector2D) EndPos);

        public void FromArgs(params string[] pArgs)
        {
            if (pArgs.Length != PropertiesCount) return;
            double dStartX = double.Parse(pArgs[0]);
            double dStartY = double.Parse(pArgs[1]);
            double dEndX = double.Parse(pArgs[2]);
            double dEndY = double.Parse(pArgs[3]);
            StartPos = new YoonVector2D(dStartX, dStartY);
            EndPos = new YoonVector2D(dEndX, dEndY);
            Slope = (dEndY - dStartY) / (dEndX == dStartX ? 0.00001 : dEndX - dStartX);
            Constant = dStartY - Slope * dStartX;
        }
        
        public override bool Equals(object obj)
        {
            return Equals(obj as IYoonLine);
        }

        public bool Equals(IYoonLine pLine)
        {
            if (pLine is not YoonLine2D pLine2D) return false;
            return Slope == pLine.Slope &&
                   Constant == pLine.Constant;
        }

        public IYoonLine Clone()
        {
            YoonLine2D pLine = new YoonLine2D
            {
                StartPos = StartPos.Clone() as YoonVector2D, EndPos = EndPos.Clone() as YoonVector2D, Slope = Slope,
                Constant = Constant
            };
            return pLine;
        }

        public void CopyFrom(IYoonLine pLine)
        {
            if (pLine is not YoonLine2D pLine2D) return;
            StartPos = pLine2D.StartPos.Clone() as YoonVector2D;
            EndPos = pLine2D.EndPos.Clone() as YoonVector2D;
            Slope = pLine2D.Slope;
            Constant = pLine2D.Constant;
        }

        public YoonLine2D()
        {
            //
        }

        public YoonLine2D(double dStartX, double dStartY, double dEndX, double dEndY)
        {
            StartPos = new YoonVector2D(dStartX, dStartY);
            EndPos = new YoonVector2D(dEndX, dEndY);
            Slope = (dEndY - dStartY) / (dEndX == dStartX ? 0.00001 : dEndX - dStartX);
            Constant = dStartY - Slope * dStartX;
        }
        
        public YoonLine2D(double slope, double constant)
        {
            Slope = slope;
            Constant = constant;
            StartPos = new YoonVector2D(0, Y(0));
            EndPos = new YoonVector2D(100, Y(100));
        }
        
        public YoonLine2D(double dA, double dB, double dC) // AX + BY + C
        {
            // AX + BY + C => Y = -A/B X - C/B
            Slope = - dA / dB;
            Constant = - dC / dB;
            StartPos = new YoonVector2D(0, Y(0));
            EndPos = new YoonVector2D(100, Y(100));
        }
        
        public YoonLine2D(List<YoonVector2D> pList, eYoonDir2D nDirArrange = eYoonDir2D.None)
        {
            if (pList == null || pList.Count < 2)
            {
                Debug.WriteLine("[YOONCOMMON] Input array is abnormal");
                StartPos = new YoonVector2D(INVALID_NUM, INVALID_NUM);
                EndPos = new YoonVector2D(INVALID_NUM, INVALID_NUM);
                return;
            }
            if (pList.Count > 10)
            {
                List<YoonVector2D> pListSorted = new List<YoonVector2D>(pList);
                pList.Clear();
                MathFactory.SortVector(ref pListSorted, nDirArrange);
                int nStart = (pListSorted.Count - 10) / 2;
                for (int i = 0; i < 10; i++)
                    pList.Add(pListSorted[i + nStart]);
                pListSorted.Clear();
            }
            double[] pX = new double[pList.Count];
            double[] pY = new double[pList.Count];
            double dMinX = 65535;
            double dMaxX = -65535;
            for (int iVector = 0; iVector < pList.Count; iVector++)
            {
                pX[iVector] = pList[iVector].X;
                pY[iVector] = pList[iVector].Y;
                if (dMinX > pX[iVector]) dMinX = pX[iVector];
                if (dMaxX < pX[iVector]) dMaxX = pX[iVector];
            }

            double dSlope = 0.0;
            double dIntercept = 0.0;
            if (!MathFactory.LeastSquare(ref dSlope, ref dIntercept, pList.Count, pX, pY))
                return;
            Slope = dSlope;
            Constant = dIntercept;
            StartPos = new YoonVector2D(dMinX, Y(dMinX));
            EndPos = new YoonVector2D(dMaxX, Y(dMaxX));
        }

        public YoonLine2D(params YoonVector2D[] pArgs)
        {
            if (pArgs.Length < 2 && pArgs.Length >= 10)
            {
                Debug.WriteLine("[YOONCOMMON] Input array is abnormal");
                StartPos = new YoonVector2D(INVALID_NUM, INVALID_NUM);
                EndPos = new YoonVector2D(INVALID_NUM, INVALID_NUM);
                return;
            }

            double[] pX = new double[pArgs.Length];
            double[] pY = new double[pArgs.Length];
            double dMinX = 65535;
            double dMaxX = -65535;
            for (int iVector = 0; iVector < pArgs.Length; iVector++)
            {
                pX[iVector] = pArgs[iVector].X;
                pY[iVector] = pArgs[iVector].Y;
                if (dMinX > pX[iVector]) dMinX = pX[iVector];
                if (dMaxX < pX[iVector]) dMaxX = pX[iVector];
            }

            double dSlope = 0.0;
            double dIntercept = 0.0;
            if (!MathFactory.LeastSquare(ref dSlope, ref dIntercept, pArgs.Length, pX, pY))
                return;
            Slope = dSlope;
            Constant = dIntercept;
            StartPos = new YoonVector2D(dMinX, Y(dMinX));
            EndPos = new YoonVector2D(dMaxX, Y(dMaxX));
        }

        public double X(double dY)
        {
            return (dY - Constant) / Slope;
        }

        public double Y(double dX)
        {
            return dX * Slope + Constant;
        }

        public IYoonVector2D<double> Intersection(IYoonLine pLine)
        {
            YoonVector2D pResultVector = new YoonVector2D(INVALID_NUM, INVALID_NUM);
            if (pLine is not YoonLine2D pLine2D) return pResultVector;
            // Check the invalid line
            if ((YoonVector2D)StartPos == new YoonVector2D(INVALID_NUM, INVALID_NUM) ||
                (YoonVector2D)EndPos == new YoonVector2D(INVALID_NUM, INVALID_NUM) ||
                (YoonVector2D)pLine2D.StartPos == new YoonVector2D(INVALID_NUM, INVALID_NUM) ||
                (YoonVector2D)pLine2D.EndPos == new YoonVector2D(INVALID_NUM, INVALID_NUM))
                return pResultVector;
            // Two lines parallel
            if (Slope == pLine2D.Slope) return pResultVector;
            pResultVector.X = (int) (-(Constant - pLine.Constant) / (Slope - pLine.Slope));
            pResultVector.Y = Y(pResultVector.X);
            return pResultVector;
        }


        public void Fit(IYoonRect2D<double> pRect)
        {
            double nStartX = Math.Min(pRect.Left, pRect.Right);
            double nStartY = Math.Min(pRect.Top, pRect.Bottom);
            double nEndX = Math.Max(pRect.Left, pRect.Right);
            double nEndY = Math.Max(pRect.Top, pRect.Bottom);
            StartPos.X = X(nStartY) > nStartX ? X(nStartY) : nStartX;
            StartPos.Y = Y(nStartX) > nStartY ? Y(nStartX) : nStartY;
            EndPos.X = X(nEndY) > nEndX ? nEndX : X(nEndY);
            EndPos.Y = Y(nEndX) > nEndY ? nEndY : Y(nEndX);
        }

        public void Fit(double width, double height)
        {
            StartPos.X = X(0) > 0 ? X(0) : 0;
            StartPos.Y = Y(0) > 0 ? Y(0) : 0;
            EndPos.X = X(height) > width ? width : X(height);
            EndPos.Y = Y(width) > height ? height : Y(width);
        }

        public bool IsContain(IYoonVector pVector)
        {
            return pVector switch
            {
                YoonVector2N pVector2N => pVector2N.Y == pVector2N.X * Slope + Constant,
                YoonVector2D pVector2D => pVector2D.Y == pVector2D.X * Slope + Constant,
                _ => false
            };
        }

        public double Distance(IYoonVector pVector)
        {
            return pVector switch
            {
                YoonVector2N pVector2N => Math.Abs(Slope * pVector2N.X - pVector2N.Y + Constant) /
                                          Math.Sqrt(Slope * Slope + 1),
                YoonVector2D pVector2D => Math.Abs(Slope * pVector2D.X - pVector2D.Y + Constant) /
                                          Math.Sqrt(Slope * Slope + 1),
                _ => throw new ArgumentException("[YOONCOMMON] Vector argument format is not correct")
            };
        }

        public bool Equals(YoonLine2D other)
        {
            return other != null &&
                   EqualityComparer<IYoonVector2D<double>>.Default.Equals(StartPos, other.StartPos) &&
                   EqualityComparer<IYoonVector2D<double>>.Default.Equals(EndPos, other.EndPos) &&
                   EqualityComparer<IYoonVector2D<double>>.Default.Equals(CenterPos, other.CenterPos) &&
                   Length == other.Length &&
                   Slope == other.Slope &&
                   Constant == other.Constant;
        }

        public override int GetHashCode()
        {
            int hashCode = 1569030335;
            hashCode = hashCode * -1521134295 + EqualityComparer<IYoonVector2D<double>>.Default.GetHashCode(StartPos);
            hashCode = hashCode * -1521134295 + EqualityComparer<IYoonVector2D<double>>.Default.GetHashCode(EndPos);
            hashCode = hashCode * -1521134295 + EqualityComparer<IYoonVector2D<double>>.Default.GetHashCode(CenterPos);
            hashCode = hashCode * -1521134295 + Length.GetHashCode();
            hashCode = hashCode * -1521134295 + Slope.GetHashCode();
            hashCode = hashCode * -1521134295 + Constant.GetHashCode();
            return hashCode;
        }

        IYoonFigure IYoonFigure.Clone()
        {
            return Clone();
        }

        public YoonLine2N ToLine2N()
        {
            return new YoonLine2N((int) StartPos.X, (int) StartPos.Y, (int) EndPos.X, (int) EndPos.Y);
        }

        public static YoonLine2N[] ToLine2Ns(YoonLine2D[] pLines)
        {
            YoonLine2N[] pResultLine = new YoonLine2N[pLines.Length];
            for (int iLine = 0; iLine < pLines.Length; iLine++)
            {
                pResultLine[iLine] = pLines[iLine].ToLine2N();
            }

            return pResultLine;
        }

        public static bool operator ==(YoonLine2D l1, YoonLine2D l2)
        {
            return l1?.Equals(l2) == true;
        }

        public static bool operator !=(YoonLine2D l1, YoonLine2D l2)
        {
            return l1?.Equals(l2) == false;
        }
    }
}
