using System.Xml.Serialization;
using System;
using System.Collections.Generic;

namespace YoonFactory
{
    public class YoonLine2N : IYoonLine, IYoonLine2D<int>, IEquatable<YoonLine2N>
    {
        [XmlAnyAttribute] public IYoonVector2D<int> StartPos { get; private set; } = new YoonVector2N(0, 0);
        [XmlAnyAttribute] public IYoonVector2D<int> EndPos { get; private set; } = new YoonVector2N(0, 0);

        public IYoonVector2D<int> CenterPos =>
            new YoonVector2N((StartPos.X + EndPos.X) / 2, (StartPos.Y + EndPos.Y) / 2);

        public double Length => Math.Sqrt(Math.Pow(StartPos.X - EndPos.X, 2.0) + Math.Pow(StartPos.Y - EndPos.Y, 2.0));

        IYoonRect2D<int> IYoonLine2D<int>.Area => new YoonRect2N((YoonVector2N) StartPos, (YoonVector2N) EndPos);

        private readonly double _dSlope = 0.0;
        private readonly double _dIntercept = 0.0;

        public override bool Equals(object obj)
        {
            return Equals(obj as IYoonLine);
        }

        public bool Equals(IYoonLine pLine)
        {
            if (pLine is YoonLine2N pLine2N)
            {
                if (StartPos.X == pLine2N.StartPos.X &&
                    StartPos.Y == pLine2N.StartPos.Y &&
                    StartPos.W == pLine2N.StartPos.W &&
                    EndPos.X == pLine2N.EndPos.X &&
                    EndPos.Y == pLine2N.EndPos.Y &&
                    EndPos.W == pLine2N.EndPos.W)
                    return true;
            }

            return false;
        }

        public IYoonLine Clone()
        {
            return new YoonLine2N
            {
                StartPos = StartPos.Clone() as YoonVector2N, EndPos = EndPos.Clone() as YoonVector2N
            };
        }

        public void CopyFrom(IYoonLine pLine)
        {
            if (pLine is not YoonLine2N pLine2N) return;
            StartPos = pLine2N.StartPos.Clone() as YoonVector2N;
            EndPos = pLine2N.EndPos.Clone() as YoonVector2N;
        }

        public YoonLine2N()
        {
            //
        }

        public YoonLine2N(int nStartX, int nStartY, int nEndX, int nEndY)
        {
            StartPos = new YoonVector2N(nStartX, nStartY);
            EndPos = new YoonVector2N(nEndX, nEndY);
            _dSlope = (nEndY - nStartY) / (nEndX - nStartX);
            _dIntercept = nStartY - _dSlope * nStartX;
        }

        public YoonLine2N(double dSlope, double dIntercept)
        {
            _dSlope = dSlope;
            _dIntercept = dIntercept;
            StartPos = new YoonVector2N(-1, Y(1));
            EndPos = new YoonVector2N(1, Y(1));
        }

        public YoonLine2N(params YoonVector2N[] pArgs)
        {
            if (pArgs.Length < 2 && pArgs.Length >= 10) return;
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

            if (!MathFactory.LeastSquare(ref _dSlope, ref _dIntercept, pArgs.Length, pX, pY))
                return;
            StartPos = new YoonVector2N(nMinX, Y(nMinX));
            EndPos = new YoonVector2N(nMaxX, Y(nMaxX));
            //// Slope, Intercept Recalculating - Only use Integer Form
            _dSlope = (EndPos.Y - StartPos.Y) / (EndPos.X - StartPos.X);
            _dIntercept = StartPos.Y - _dSlope * StartPos.X;
        }

        public int X(int nY)
        {
            return (int) ((nY - _dIntercept) / _dSlope);
        }

        public int Y(int nX)
        {
            return (int) (nX * _dSlope + _dIntercept);
        }

        public bool IsContain(IYoonVector pVector)
        {
            return pVector switch
            {
                YoonVector2N pVector2N => pVector2N.Y == pVector2N.X * _dSlope + _dIntercept,
                YoonVector2D pVector2D => pVector2D.Y == pVector2D.X * _dSlope + _dIntercept,
                _ => false
            };
        }

        public double Distance(IYoonVector pVector)
        {
            return pVector switch
            {
                YoonVector2N pVector2N => Math.Abs(_dSlope * pVector2N.X - pVector2N.Y + _dIntercept) /
                                          Math.Sqrt(_dSlope * _dSlope + 1),
                YoonVector2D pVector2D => Math.Abs(_dSlope * pVector2D.X - pVector2D.Y + _dIntercept) /
                                          Math.Sqrt(_dSlope * _dSlope + 1),
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
                   _dSlope == other._dSlope &&
                   _dIntercept == other._dIntercept;
        }

        public override int GetHashCode()
        {
            int hashCode = 1569030335;
            hashCode = hashCode * -1521134295 + EqualityComparer<IYoonVector2D<int>>.Default.GetHashCode(StartPos);
            hashCode = hashCode * -1521134295 + EqualityComparer<IYoonVector2D<int>>.Default.GetHashCode(EndPos);
            hashCode = hashCode * -1521134295 + EqualityComparer<IYoonVector2D<int>>.Default.GetHashCode(CenterPos);
            hashCode = hashCode * -1521134295 + Length.GetHashCode();
            hashCode = hashCode * -1521134295 + _dSlope.GetHashCode();
            hashCode = hashCode * -1521134295 + _dIntercept.GetHashCode();
            return hashCode;
        }

        IYoonFigure IYoonFigure.Clone()
        {
            return Clone();
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
        [XmlAnyAttribute] public IYoonVector2D<double> StartPos { get; private set; } = new YoonVector2D(0, 0);
        [XmlAnyAttribute] public IYoonVector2D<double> EndPos { get; private set; } = new YoonVector2D(0, 0);

        public IYoonVector2D<double> CenterPos =>
            new YoonVector2D((StartPos.X + EndPos.X) / 2, (StartPos.Y + EndPos.Y) / 2);

        public double Length => Math.Sqrt(Math.Pow(StartPos.X - EndPos.X, 2.0) + Math.Pow(StartPos.Y - EndPos.Y, 2.0));

        public IYoonRect2D<double> Area => new YoonRect2D((YoonVector2D) StartPos, (YoonVector2D) EndPos);

        private readonly double _dSlope = 0.0;
        private readonly double _dIntercept = 0.0;

        public override bool Equals(object obj)
        {
            return Equals(obj as IYoonLine);
        }

        public bool Equals(IYoonLine pLine)
        {
            if (pLine is not YoonLine2D pLine2D) return false;
            return StartPos.X == pLine2D.StartPos.X &&
                   StartPos.Y == pLine2D.StartPos.Y &&
                   StartPos.W == pLine2D.StartPos.W &&
                   EndPos.X == pLine2D.EndPos.X &&
                   EndPos.Y == pLine2D.EndPos.Y &&
                   EndPos.W == pLine2D.EndPos.W;
        }

        public IYoonLine Clone()
        {
            YoonLine2D pLine = new YoonLine2D();
            pLine.StartPos = StartPos.Clone() as YoonVector2D;
            pLine.EndPos = EndPos.Clone() as YoonVector2D;
            return pLine;
        }

        public void CopyFrom(IYoonLine pLine)
        {
            if (pLine is not YoonLine2D pLine2D) return;
            StartPos = pLine2D.StartPos.Clone() as YoonVector2D;
            EndPos = pLine2D.EndPos.Clone() as YoonVector2D;
        }

        public YoonLine2D()
        {
            //
        }

        public YoonLine2D(double dStartX, double dStartY, double dEndX, double dEndY)
        {
            StartPos = new YoonVector2D(dStartX, dStartY);
            EndPos = new YoonVector2D(dEndX, dEndY);
            _dSlope = (dEndY - dStartY) / (dEndX - dStartX);
            _dIntercept = dStartY - _dSlope * dStartX;
        }

        public YoonLine2D(double dSlope, double dIntercept)
        {
            _dSlope = dSlope;
            _dIntercept = dIntercept;
            StartPos = new YoonVector2D(-1, Y(1));
            EndPos = new YoonVector2D(1, Y(1));
        }

        public YoonLine2D(params YoonVector2D[] pArgs)
        {
            if (pArgs.Length < 2 && pArgs.Length >= 10) return;
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

            if (!MathFactory.LeastSquare(ref _dSlope, ref _dIntercept, pArgs.Length, pX, pY))
                return;
            StartPos = new YoonVector2D(dMinX, Y(dMinX));
            EndPos = new YoonVector2D(dMaxX, Y(dMaxX));
        }

        public double X(double dY)
        {
            return (dY - _dIntercept) / _dSlope;
        }

        public double Y(double dX)
        {
            return dX * _dSlope + _dIntercept;
        }

        public bool IsContain(IYoonVector pVector)
        {
            return pVector switch
            {
                YoonVector2N pVector2N => pVector2N.Y == pVector2N.X * _dSlope + _dIntercept,
                YoonVector2D pVector2D => pVector2D.Y == pVector2D.X * _dSlope + _dIntercept,
                _ => false
            };
        }

        public double Distance(IYoonVector pVector)
        {
            return pVector switch
            {
                YoonVector2N pVector2N => Math.Abs(_dSlope * pVector2N.X - pVector2N.Y + _dIntercept) /
                                          Math.Sqrt(_dSlope * _dSlope + 1),
                YoonVector2D pVector2D => Math.Abs(_dSlope * pVector2D.X - pVector2D.Y + _dIntercept) /
                                          Math.Sqrt(_dSlope * _dSlope + 1),
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
                   _dSlope == other._dSlope &&
                   _dIntercept == other._dIntercept;
        }

        public override int GetHashCode()
        {
            int hashCode = 1569030335;
            hashCode = hashCode * -1521134295 + EqualityComparer<IYoonVector2D<double>>.Default.GetHashCode(StartPos);
            hashCode = hashCode * -1521134295 + EqualityComparer<IYoonVector2D<double>>.Default.GetHashCode(EndPos);
            hashCode = hashCode * -1521134295 + EqualityComparer<IYoonVector2D<double>>.Default.GetHashCode(CenterPos);
            hashCode = hashCode * -1521134295 + Length.GetHashCode();
            hashCode = hashCode * -1521134295 + _dSlope.GetHashCode();
            hashCode = hashCode * -1521134295 + _dIntercept.GetHashCode();
            return hashCode;
        }

        IYoonFigure IYoonFigure.Clone()
        {
            return Clone();
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
