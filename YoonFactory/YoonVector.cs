using System;
using System.Collections.Generic;
using System.Xml.Serialization;

namespace YoonFactory
{
    /// <summary>
    /// Homogeneous coordinate
    /// </summary>
    public class YoonVector2N : IYoonVector2D<int>, IEquatable<YoonVector2N>
    {
        public int Count { get; } = 3;

        public eYoonDir2D Direction
        {
            get
            {
                return X switch
                {
                    0 when Y == 0 => eYoonDir2D.Center,
                    0 when Y > 0 => eYoonDir2D.Top,
                    0 when Y < 0 => eYoonDir2D.Bottom,
                    > 0 when Y == 0 => eYoonDir2D.Right,
                    < 0 when Y == 0 => eYoonDir2D.Left,
                    > 0 when Y > 0 => eYoonDir2D.TopRight,
                    < 0 when Y > 0 => eYoonDir2D.TopLeft,
                    < 0 when Y < 0 => eYoonDir2D.BottomLeft,
                    > 0 when Y < 0 => eYoonDir2D.BottomRight,
                    _ => eYoonDir2D.None
                };
            }
            set
            {
                switch(value)
                {
                    case eYoonDir2D.Center:
                        X = Y = 0;
                        break;
                    case eYoonDir2D.Top:
                        X = 0;
                        Y = 1;
                        break;
                    case eYoonDir2D.Bottom:
                        X = 0;
                        Y = -1;
                        break;
                    case eYoonDir2D.Right:
                        X = 1;
                        Y = 0;
                        break;
                    case eYoonDir2D.Left:
                        X = -1;
                        Y = 0;
                        break;
                    case eYoonDir2D.TopRight:
                        X = 1;
                        Y = 1;
                        break;
                    case eYoonDir2D.TopLeft:
                        X = -1;
                        Y = 1;
                        break;
                    case eYoonDir2D.BottomLeft:
                        X = -1;
                        Y = -1;
                        break;
                    case eYoonDir2D.BottomRight:
                        X = 1;
                        Y = -1;
                        break;
                    case eYoonDir2D.None:
                        break;
                    default:
                        break;
                }
            }
        }

        public void CopyFrom(IYoonVector v)
        {
            if (v is not YoonVector2N vec) return;
            X = vec.X;
            Y = vec.Y;
            W = 1;
        }

        public IYoonVector Clone()
        {
            return new YoonVector2N {X = this.X, Y = this.Y, W = this.W};
        }

        public override bool Equals(object obj)
        {
            return Equals(obj as IYoonVector);
        }

        public bool Equals(IYoonVector pVector)
        {
            if (pVector is not YoonVector2N pVector2N) return false;
            return X == pVector2N.X &&
                   Y == pVector2N.Y &&
                   W == pVector2N.W;
        }

        [XmlIgnore]
        private static double DELTA = 0.0000000000001;
        [XmlIgnore]
        public int W
        {
            get => Array[2];

            set => Array[2] = value;
        }
        [XmlAttribute]
        public int X
        {
            get => Array[0];

            set => Array[0] = value;
        }
        [XmlAttribute]
        public int Y
        {
            get => Array[1];

            set => Array[1] = value;
        }
        public IYoonCartesian<int> Cartesian
        {
            get => new YoonCartesianN(X, Y, 0, 0, 0, 0);
            set
            {
                try
                {
                    X = value.X;
                    Y = value.Y;
                }
                catch
                {
                    //
                }
            }
        }
        public int[] Array { get; set; } = new int[3];

        public YoonVector2N()
        {
            X = 0;
            Y = 0;
            W = 1;
        }
        public YoonVector2N(IYoonVector pVector)
        {
            CopyFrom(pVector);
        }
        public YoonVector2N(int nX, int nY)
        {
            X = nX;
            Y = nY;
            W = 1;
        }
        public YoonVector2N(eYoonDir2D nDir)
        {
            Direction = nDir;
            W = 1;
        }

        public void Zero()
        {
            X = 0;
            Y = 0;
            W = 1;
        }
        public double Length()
        {
            return Math.Sqrt(X * X + Y * Y);
        }

        public IYoonVector Unit()
        {
            double dLength = this.Length();
            if (!(dLength > DELTA)) return this;
            dLength = 1.0 / dLength;
            X *= (int)dLength;
            Y *= (int)dLength;
            return this;
        }

        public double Distance(IYoonVector pVector)
        {
            if (pVector is YoonVector2N pVector2N)
            {
                double dx = this.X - pVector2N.X;
                double dy = this.Y - pVector2N.Y;
                return Math.Sqrt(dx * dx + dy * dy);
            }
            else
                return 0.0;
        }

        public eYoonDir2D DirectionTo(IYoonVector pVector)
        {
            if (pVector is YoonVector2N pVector2N)
            {
                YoonVector2N pVecDiff = this - pVector2N;
                return pVecDiff.Direction;
            }
            else
                return eYoonDir2D.None;
        }

        public double Angle2D(IYoonVector pVector)
        {
            if (pVector is YoonVector2N pVector2N)
            {
                return Math.Atan2(pVector2N.Y - Y, pVector2N.X - X);
            }
            else
                return 0.0;
        }

        public IYoonVector GetScaleVector(int nScaleX, int nScaleY)
        {
            YoonMatrix2N pMatrix = new YoonMatrix2N();
            pMatrix.SetScaleUnit(nScaleX, nScaleY);
            return pMatrix * this;
        }

        public IYoonVector GetNextVector(int nMoveX, int nMoveY)
        {
            YoonMatrix2N pMatrix = new YoonMatrix2N();
            pMatrix.SetMovementUnit(nMoveX, nMoveY);
            return pMatrix * this;
        }

        public IYoonVector GetNextVector(IYoonVector pVector)
        {
            if (pVector is not YoonVector2N pVector2N) return this;
            YoonMatrix2N pMatrix = new YoonMatrix2N();
            pMatrix.SetMovementUnit(pVector2N);
            return pMatrix * this;
        }

        public IYoonVector GetNextVector(eYoonDir2D nDir)
        {
            return GetNextVector(new YoonVector2N(nDir));
        }

        public IYoonVector GetRotateVector(double dAngle)
        {
            YoonMatrix2N pMatrix = new YoonMatrix2N();
            pMatrix.SetRotateUnit(dAngle);
            return pMatrix * this;
        }

        public IYoonVector GetRotateVector(IYoonVector pVectorCenter, double dAngle)
        {
            if (pVectorCenter is not YoonVector2N pVector) return this;
            YoonVector2N pResultVector = new YoonVector2N(this);
            pResultVector.Move(-pVector);
            pResultVector.Rotate(dAngle);
            pResultVector.Move(pVector);
            return pResultVector;
        }

        public void Scale(int nScaleX, int nScaleY)
        {
            YoonMatrix2N pMatrix = new YoonMatrix2N();
            pMatrix.SetScaleUnit(nScaleX, nScaleY);
            YoonVector2N pVector = pMatrix * this;
            this.X = pVector.X;
            this.Y = pVector.Y;
        }

        public void Move(int nMoveX, int nMoveY)
        {
            YoonMatrix2N pMatrix = new YoonMatrix2N();
            pMatrix.SetMovementUnit(nMoveX, nMoveY);
            YoonVector2N pVector = pMatrix * this;
            this.X = pVector.X;
            this.Y = pVector.Y;
        }

        public void Move(IYoonVector pVector)
        {
            if (pVector is not YoonVector2N pVector2N) return;
            YoonMatrix2N pMatrix = new YoonMatrix2N();
            pMatrix.SetMovementUnit(pVector2N);
            YoonVector2N pResultVector = pMatrix * this;
            this.X = pResultVector.X;
            this.Y = pResultVector.Y;
        }

        public void Move(eYoonDir2D nDir)
        {
            Move(new YoonVector2N(nDir));
        }

        public void Rotate(double dAngle)
        {
            YoonMatrix2N pMatrix = new YoonMatrix2N();
            pMatrix.SetRotateUnit(dAngle);
            YoonVector2N pVector = pMatrix * this;
            this.X = pVector.X;
            this.Y = pVector.Y;
        }

        public void Rotate(IYoonVector pVectorCenter, double dAngle)
        {
            if (pVectorCenter is not YoonVector2N pVector) return;
            Move(-pVector);
            Rotate(dAngle);
            Move(pVector);
        }

        public bool Equals(YoonVector2N other)
        {
            return other != null &&
                   Count == other.Count &&
                   Direction == other.Direction &&
                   W == other.W &&
                   X == other.X &&
                   Y == other.Y &&
                   EqualityComparer<IYoonCartesian<int>>.Default.Equals(Cartesian, other.Cartesian) &&
                   EqualityComparer<int[]>.Default.Equals(Array, other.Array);
        }

        public override int GetHashCode()
        {
            int hashCode = 1178670866;
            hashCode = hashCode * -1521134295 + Count.GetHashCode();
            hashCode = hashCode * -1521134295 + Direction.GetHashCode();
            hashCode = hashCode * -1521134295 + W.GetHashCode();
            hashCode = hashCode * -1521134295 + X.GetHashCode();
            hashCode = hashCode * -1521134295 + Y.GetHashCode();
            hashCode = hashCode * -1521134295 + EqualityComparer<IYoonCartesian<int>>.Default.GetHashCode(Cartesian);
            hashCode = hashCode * -1521134295 + EqualityComparer<int[]>.Default.GetHashCode(Array);
            return hashCode;
        }

        IYoonFigure IYoonFigure.Clone()
        {
            return Clone();
        }

        public YoonVector2D ToVector2D()
        {
            return new YoonVector2D(X, Y);
        }

        public static YoonVector2N operator *(YoonMatrix3X3Int pMatrix, YoonVector2N pVector)
        {
            YoonVector2N pResultVector = new YoonVector2N();
            for (int i = 0; i < pVector.Count; i++)
            {
                pResultVector.Array[i] = 0;
                for (int k = 0; k < pMatrix.Length; k++)
                    pResultVector.Array[i] += pMatrix.Array[i, k] * pVector.Array[k];
            }
            return pResultVector;
        }
        public static YoonVector2N operator *(YoonVector2N pVector, int nNum)
        {
            return new YoonVector2N(pVector.X * nNum, pVector.Y * nNum);
        }
        public static YoonVector2N operator +(YoonVector2N pVectorSource, YoonVector2N pVectorObject)
        {
            return new YoonVector2N(pVectorSource.X + pVectorObject.X, pVectorSource.Y + pVectorObject.Y);
        }
        public static YoonVector2N operator +(YoonVector2N pVector, eYoonDir2D nDir)
        {
            YoonVector2N pResultVector = new YoonVector2N(nDir);
            return new YoonVector2N(pVector.X + pResultVector.X, pVector.Y + pResultVector.Y);
        }
        public static YoonVector2N operator -(YoonVector2N pVectorSource, YoonVector2N pVectorObject)
        {
            return new YoonVector2N(pVectorSource.X - pVectorObject.X, pVectorSource.Y - pVectorObject.Y);
        }
        public static YoonVector2N operator -(YoonVector2N pVector, eYoonDir2D nDir)
        {
            YoonVector2N pResultVector = new YoonVector2N(nDir);
            return new YoonVector2N(pVector.X - pResultVector.X, pVector.Y - pResultVector.Y);
        }
        public static YoonVector2N operator -(YoonVector2N pVector)
        {
            return new YoonVector2N(-pVector.X, -pVector.Y);
        }
        public static YoonVector2N operator /(YoonVector2N pVector, int nNum)
        {
            return new YoonVector2N(pVector.X / nNum, pVector.Y / nNum);
        }
        public static int operator *(YoonVector2N pVectorSource, YoonVector2N pVectorObject) // dot product
        {
            return pVectorSource.X * pVectorObject.X + pVectorSource.Y * pVectorObject.Y;
        }
        public static bool operator ==(YoonVector2N pVectorSource, YoonVector2N pVectorObject)
        {
            return pVectorSource?.Equals(pVectorObject) == true;
        }
        public static bool operator !=(YoonVector2N pVectorSource, YoonVector2N pVectorObject)
        {
            return pVectorSource?.Equals(pVectorObject) == false;
        }
    }

    /// <summary>
    /// Homogeneous coordinate
    /// </summary>
    public class YoonVector2D : IYoonVector2D<double>, IEquatable<YoonVector2D>
    {
        public int Count { get; } = 3;

        public eYoonDir2D Direction
        {
            get
            {
                return X switch
                {
                    0 when Y == 0 => eYoonDir2D.Center,
                    0 when Y > 0 => eYoonDir2D.Top,
                    0 when Y < 0 => eYoonDir2D.Bottom,
                    > 0 when Y == 0 => eYoonDir2D.Right,
                    < 0 when Y == 0 => eYoonDir2D.Left,
                    > 0 when Y > 0 => eYoonDir2D.TopRight,
                    < 0 when Y > 0 => eYoonDir2D.TopLeft,
                    < 0 when Y < 0 => eYoonDir2D.BottomLeft,
                    > 0 when Y < 0 => eYoonDir2D.BottomRight,
                    _ => eYoonDir2D.None
                };
            }
            set
            {
                switch (value)
                {
                    case eYoonDir2D.Center:
                        X = Y = 0;
                        break;
                    case eYoonDir2D.Top:
                        X = 0;
                        Y = 1;
                        break;
                    case eYoonDir2D.Bottom:
                        X = 0;
                        Y = -1;
                        break;
                    case eYoonDir2D.Right:
                        X = 1;
                        Y = 0;
                        break;
                    case eYoonDir2D.Left:
                        X = -1;
                        Y = 0;
                        break;
                    case eYoonDir2D.TopRight:
                        X = 1;
                        Y = 1;
                        break;
                    case eYoonDir2D.TopLeft:
                        X = -1;
                        Y = 1;
                        break;
                    case eYoonDir2D.BottomLeft:
                        X = -1;
                        Y = -1;
                        break;
                    case eYoonDir2D.BottomRight:
                        X = 1;
                        Y = -1;
                        break;
                    case eYoonDir2D.None:
                        break;
                    default:
                        break;
                }
            }
        }

        public override bool Equals(object obj)
        {
            return Equals(obj as IYoonVector);
        }

        public bool Equals(IYoonVector pVector)
        {
            if (pVector is not YoonVector2D pVector2D) return false;
            return X == pVector2D.X &&
                   Y == pVector2D.Y &&
                   W == pVector2D.W;
        }

        public void CopyFrom(IYoonVector pVector)
        {
            if (pVector is not YoonVector2D pVector2D) return;
            X = pVector2D.X;
            Y = pVector2D.Y;
            W = 1;
        }

        public IYoonVector Clone()
        {
            return new YoonVector2D {X = this.X, Y = this.Y, W = this.W};
        }
        [XmlIgnore]
        private static double DELTA = 0.0000000000001;
        [XmlIgnore]
        public double W
        {
            get => Array[2];

            set => Array[2] = value;
        }
        [XmlAttribute]
        public double X
        {
            get => Array[0];

            set => Array[0] = value;
        }
        [XmlAttribute]
        public double Y
        {
            get => Array[1];

            set => Array[1] = value;
        }
        public IYoonCartesian<double> Cartesian
        {
            get => new YoonCartesianD(X, Y, 0, 0, 0, 0);
            set
            {
                try
                {
                    X = value.X;
                    Y = value.Y;
                }
                catch
                {
                    //
                }
            }
        }
        public double[] Array { get; set; } = new double[3];

        public YoonVector2D()
        {
            X = 0;
            Y = 0;
            W = 1;
        }
        public YoonVector2D(IYoonVector p)
        {
            CopyFrom(p);
        }
        public YoonVector2D(double dx, double dy)
        {
            X = dx;
            Y = dy;
            W = 1;
        }
        public YoonVector2D(eYoonDir2D nDir)
        {
            Direction = nDir;
            W = 1;
        }
        public void Zero()
        {
            X = 0;
            Y = 0;
            W = 1;
        }

        public double Length()
        {
            return Math.Sqrt(X * X + Y * Y);
        }

        public IYoonVector Unit()
        {
            double len = this.Length();
            if (!(len > DELTA)) return this;
            len = 1.0 / len;
            X *= len;
            Y *= len;
            return this;
        }

        public double Distance(IYoonVector p)
        {
            if (p is YoonVector2D vec)
            {
                double dx = this.X - vec.X;
                double dy = this.Y - vec.Y;
                return Math.Sqrt(dx * dx + dy * dy);
            }
            else
                return 0.0;
        }

        public eYoonDir2D DirectionTo(IYoonVector pPos)
        {
            if (pPos is YoonVector2D vec)
            {
                YoonVector2D pVecDiff = this - vec;
                return pVecDiff.Direction;
            }
            else
                return eYoonDir2D.None;
        }

        public double Angle2D(IYoonVector pPosObject)
        {
            if (pPosObject is YoonVector2D vec)
            {
                return Math.Atan2(vec.Y - Y, vec.X - X);
            }
            else
                return 0.0;
        }

        public IYoonVector GetScaleVector(double nScaleX, double nScaleY)
        {
            YoonMatrix2D pMatrix = new YoonMatrix2D();
            pMatrix.SetScaleUnit(nScaleX, nScaleY);
            return pMatrix * this;
        }

        public IYoonVector GetNextVector(double nMoveX, double nMoveY)
        {
            YoonMatrix2D pMatrix = new YoonMatrix2D();
            pMatrix.SetMovementUnit(nMoveX, nMoveY);
            return pMatrix * this;
        }

        public IYoonVector GetNextVector(IYoonVector pVector)
        {
            if (pVector is not YoonVector2D pVector2D) return this;
            YoonMatrix2D pMatrix = new YoonMatrix2D();
            pMatrix.SetMovementUnit(pVector2D);
            return pMatrix * this;
        }

        public IYoonVector GetNextVector(eYoonDir2D nDir)
        {
            return GetNextVector(new YoonVector2D(nDir));
        }

        public IYoonVector GetRotateVector(double dAngle)
        {
            YoonMatrix2D pMatrix = new YoonMatrix2D();
            pMatrix.SetRotateUnit(dAngle);
            return pMatrix * this;
        }

        public IYoonVector GetRotateVector(IYoonVector pVectorCenter, double dAngle)
        {
            if (pVectorCenter is YoonVector2D pVecCenter)
            {
                YoonVector2D pVecResult = new YoonVector2D(this);
                pVecResult.Move(-pVecCenter);
                pVecResult.Rotate(dAngle);
                pVecResult.Move(pVecCenter);
                return pVecResult;
            }
            return this;
        }

        public void Scale(double nScaleX, double nScaleY)
        {
            YoonMatrix2D pMatrix = new YoonMatrix2D();
            pMatrix.SetScaleUnit(nScaleX, nScaleY);
            YoonVector2D v = pMatrix * this;
            this.X = v.X;
            this.Y = v.Y;
        }

        public void Move(double nMoveX, double nMoveY)
        {
            YoonMatrix2D pMatrix = new YoonMatrix2D();
            pMatrix.SetMovementUnit(nMoveX, nMoveY);
            YoonVector2D v = pMatrix * this;
            this.X = v.X;
            this.Y = v.Y;
        }

        public void Move(IYoonVector pVector)
        {
            if (pVector is not YoonVector2D pVector2D) return;
            YoonMatrix2D pMatrix = new YoonMatrix2D();
            pMatrix.SetMovementUnit(pVector2D);
            YoonVector2D v = pMatrix * this;
            this.X = v.X;
            this.Y = v.Y;
        }

        public void Move(eYoonDir2D nDir)
        {
            Move(new YoonVector2D(nDir));
        }

        public void Rotate(double dAngle)
        {
            YoonMatrix2D pMatrix = new YoonMatrix2D();
            pMatrix.SetRotateUnit(dAngle);
            YoonVector2D v = pMatrix * this;
            this.X = v.X;
            this.Y = v.Y;
        }

        public void Rotate(IYoonVector pVectorCenter, double dAngle)
        {
            if (pVectorCenter is not YoonVector2D pVecCenter) return;
            Move(-pVecCenter);
            Rotate(dAngle);
            Move(pVecCenter);
        }

        public bool Equals(YoonVector2D other)
        {
            return other != null &&
                   Count == other.Count &&
                   Direction == other.Direction &&
                   W == other.W &&
                   X == other.X &&
                   Y == other.Y &&
                   EqualityComparer<IYoonCartesian<double>>.Default.Equals(Cartesian, other.Cartesian) &&
                   EqualityComparer<double[]>.Default.Equals(Array, other.Array);
        }

        public override int GetHashCode()
        {
            int hashCode = 1178670866;
            hashCode = hashCode * -1521134295 + Count.GetHashCode();
            hashCode = hashCode * -1521134295 + Direction.GetHashCode();
            hashCode = hashCode * -1521134295 + W.GetHashCode();
            hashCode = hashCode * -1521134295 + X.GetHashCode();
            hashCode = hashCode * -1521134295 + Y.GetHashCode();
            hashCode = hashCode * -1521134295 + EqualityComparer<IYoonCartesian<double>>.Default.GetHashCode(Cartesian);
            hashCode = hashCode * -1521134295 + EqualityComparer<double[]>.Default.GetHashCode(Array);
            return hashCode;
        }

        IYoonFigure IYoonFigure.Clone()
        {
            return Clone();
        }

        public YoonVector2N ToVector2N()
        {
            return new YoonVector2N((int)X, (int)Y);
        }

        public static YoonVector2D operator *(YoonMatrix3X3Double m, YoonVector2D v)
        {
            YoonVector2D pVector = new YoonVector2D();
            for (int i = 0; i < v.Count; i++)
            {
                pVector.Array[i] = 0;
                for (int k = 0; k < m.Length; k++)
                    pVector.Array[i] += m.Array[i, k] * v.Array[k];
            }
            return pVector;
        }
        public static YoonVector2D operator *(YoonVector2D v, double a)
        {
            return new YoonVector2D(v.X * a, v.Y * a);
        }
        public static YoonVector2D operator +(YoonVector2D v1, YoonVector2D v2)
        {
            return new YoonVector2D(v1.X + v2.X, v1.Y + v2.Y);
        }
        public static YoonVector2D operator +(YoonVector2D v, YoonCartesianD c)
        {
            return new YoonVector2D(v.X + c.X, v.Y + c.Y);
        }
        public static YoonVector2D operator +(YoonVector2D v, eYoonDir2D d)
        {
            YoonVector2D pVector = new YoonVector2D(d);
            return new YoonVector2D(v.X + pVector.X, v.Y + pVector.Y);
        }
        public static YoonVector2D operator -(YoonVector2D v1, YoonVector2D v2)
        {
            return new YoonVector2D(v1.X - v2.X, v1.Y - v2.Y);
        }
        public static YoonVector2D operator -(YoonVector2D v)
        {
            return new YoonVector2D(-v.X, -v.Y);
        }
        public static YoonVector2D operator -(YoonVector2D v, YoonCartesianD c)
        {
            return new YoonVector2D(v.X - c.X, v.Y - c.Y);
        }
        public static YoonVector2D operator -(YoonVector2D v, eYoonDir2D d)
        {
            YoonVector2D pVector = new YoonVector2D(d);
            return new YoonVector2D(v.X - pVector.X, v.Y - pVector.Y);
        }
        public static YoonVector2D operator /(YoonVector2D v, double a)
        {
            return new YoonVector2D(v.X / a, v.Y / a);
        }
        public static double operator *(YoonVector2D v1, YoonVector2D v2) // dot product
        {
            return v1.X * v2.X + v1.Y * v2.Y;
        }
        public static bool operator ==(YoonVector2D v1, YoonVector2D v2)
        {
            return v1?.Equals(v2) == true;
        }
        public static bool operator !=(YoonVector2D v1, YoonVector2D v2)
        {
            return v1?.Equals(v2) == false;
        }
    }

    /// <summary>
    /// Homogenous Vector
    /// </summary>
    public class YoonVector3D : IYoonVector, IYoonVector3D<double>, IEquatable<YoonVector3D>
    {
        public int Count { get; } = 4;

        public override bool Equals(object obj)
        {
            return Equals(obj as IYoonVector);
        }

        public bool Equals(IYoonVector pVector)
        {
            if (pVector is not YoonVector3D pVector3D) return false;
            return X == pVector3D.X &&
                   Y == pVector3D.Y &&
                   W == pVector3D.W;
        }

        public void CopyFrom(IYoonVector pVector)
        {
            if (pVector is not YoonVector3D pVector3D) return;
            X = pVector3D.X;
            Y = pVector3D.Y;
            W = 1;
        }

        public IYoonVector Clone()
        {
            YoonVector3D pVector = new YoonVector3D {X = this.X, Y = this.Y, Z = this.Z, W = this.W};
            return pVector;
        }

        [XmlIgnore]
        private static double DELTA = 0.0000000000001;
        [XmlIgnore]
        public double W
        {
            get => Array[3];

            set => Array[3] = value;
        }
        [XmlAttribute]
        public double X
        {
            get => Array[0];

            set => Array[0] = value;
        }
        [XmlAttribute]
        public double Y
        {
            get => Array[1];

            set => Array[1] = value;
        }
        [XmlAttribute]
        public double Z
        {
            get => Array[2];

            set => Array[2] = value;
        }

        public IYoonCartesian<double> Cartesian
        {
            get => new YoonCartesianD(X, Y, Z, 0, 0, 0);
            set
            {
                try
                {
                    X = value.X;
                    Y = value.Y;
                    Z = value.Z;
                }
                catch
                {
                    //
                }
            }
        }

        public double[] Array { get; set; } = new double[4];

        public YoonVector3D()
        {
            X = 0;
            Y = 0;
            Z = 0;
            W = 1;
        }

        public YoonVector3D(IYoonVector p)
        {
            CopyFrom(p);
        }

        public YoonVector3D(double dX, double dY, double dZ)
        {
            X = dX;
            Y = dY;
            Z = dZ;
            W = 1;
        }

        public void Zero()
        {
            X = 0;
            Y = 0;
            Z = 0;
            W = 1;
        }

        public double Length()
        {
            return Math.Sqrt(X * X + Y * Y + Z * Z);
        }

        public IYoonVector Unit()
        {
            double dLength = this.Length();
            if (!(dLength > DELTA)) return this;
            dLength = 1.0 / dLength;
            X *= dLength;
            Y *= dLength;
            Z *= dLength;
            return this;
        }

        public double Distance(IYoonVector pVector)
        {
            if (pVector is YoonVector3D pVector3D)
            {
                double dx = this.X - pVector3D.X;
                double dy = this.Y - pVector3D.Y;
                double dz = this.Z - pVector3D.Z;
                return Math.Sqrt(dx * dx + dy * dy);
            }
            else
                return 0.0;
        }

        public double AngleX(IYoonVector pVector)
        {
            if (pVector is YoonVector3D pVector3D)
            {
                return Math.Atan2(pVector3D.Y - Y, pVector3D.Z - Z);
            }
            else
                return 0.0;
        }

        public double AngleY(IYoonVector pVector)
        {
            if (pVector is YoonVector3D pVector3D)
            {
                return Math.Atan2(pVector3D.X - X, pVector3D.Z - Z);
            }
            else
                return 0.0;
        }

        public double AngleZ(IYoonVector pVector)
        {
            if (pVector is YoonVector3D pVector3D)
            {
                return Math.Atan2(pVector3D.Y - Y, pVector3D.X - X);
            }
            else
                return 0.0;
        }

        public IYoonVector GetScaleVector(double dScaleX, double dScaleY, double dScaleZ)
        {
            YoonMatrix3D pMatrix = new YoonMatrix3D();
            pMatrix.SetScaleUnit(dScaleX, dScaleY, dScaleZ);
            return pMatrix * this;
        }

        public IYoonVector GetNextVector(double dMoveX, double dMoveY, double dMoveZ)
        {
            YoonMatrix3D pMatrix = new YoonMatrix3D();
            pMatrix.SetMovementUnit(dMoveX, dMoveY, dMoveZ);
            return pMatrix * this;
        }

        public IYoonVector GetNextVector(IYoonVector pVector)
        {
            if (pVector is YoonVector3D pVector3D)
            {
                YoonMatrix3D pMatrix = new YoonMatrix3D();
                pMatrix.SetMovementUnit(pVector3D);
                return pMatrix * this;
            }
            return this;
        }

        public void Scale(double dScaleX, double dScaleY, double dScaleZ)
        {
            YoonMatrix3D pMatrix = new YoonMatrix3D();
            pMatrix.SetScaleUnit(dScaleX, dScaleY, dScaleZ);
            YoonVector3D pVector = pMatrix * this;
            this.X = pVector.X;
            this.Y = pVector.Y;
            this.Z = pVector.Z;
        }

        public void Move(double dX, double dY, double dZ)
        {
            YoonMatrix3D pMatrix = new YoonMatrix3D();
            pMatrix.SetMovementUnit(dX, dY, dZ);
            YoonVector3D pVector = pMatrix * this;
            this.X = pVector.X;
            this.Y = pVector.Y;
            this.Z = pVector.Z;
        }

        public void Move(IYoonVector pVector)
        {
            if (pVector is not YoonVector3D pVector3D) return;
            YoonMatrix3D pMatrix = new YoonMatrix3D();
            pMatrix.SetMovementUnit(pVector3D);
            YoonVector3D pResultVector = pMatrix * this;
            this.X = pResultVector.X;
            this.Y = pResultVector.Y;
            this.Z = pResultVector.Z;
        }

        public void RotateX(double dAngle)
        {
            YoonMatrix3D pMatrix = new YoonMatrix3D();
            pMatrix.SetRotateXUnit(dAngle);
            YoonVector3D pVector = pMatrix * this;
            this.X = pVector.X;
            this.Y = pVector.Y;
            this.Z = pVector.Z;
        }

        public void RotateY(double dAngle)
        {
            YoonMatrix3D pMatrix = new YoonMatrix3D();
            pMatrix.SetRotateYUnit(dAngle);
            YoonVector3D pVector = pMatrix * this;
            this.X = pVector.X;
            this.Y = pVector.Y;
            this.Z = pVector.Z;
        }

        public void RotateZ(double dAngle)
        {
            YoonMatrix3D pMatrix = new YoonMatrix3D();
            pMatrix.SetRotateZUnit(dAngle);
            YoonVector3D pVector3D = pMatrix * this;
            this.X = pVector3D.X;
            this.Y = pVector3D.Y;
            this.Z = pVector3D.Z;
        }

        public bool Equals(YoonVector3D other)
        {
            return other != null &&
                   Count == other.Count &&
                   W == other.W &&
                   X == other.X &&
                   Y == other.Y &&
                   Z == other.Z &&
                   EqualityComparer<IYoonCartesian<double>>.Default.Equals(Cartesian, other.Cartesian) &&
                   EqualityComparer<double[]>.Default.Equals(Array, other.Array);
        }

        public override int GetHashCode()
        {
            int hashCode = -366330435;
            hashCode = hashCode * -1521134295 + Count.GetHashCode();
            hashCode = hashCode * -1521134295 + W.GetHashCode();
            hashCode = hashCode * -1521134295 + X.GetHashCode();
            hashCode = hashCode * -1521134295 + Y.GetHashCode();
            hashCode = hashCode * -1521134295 + Z.GetHashCode();
            hashCode = hashCode * -1521134295 + EqualityComparer<IYoonCartesian<double>>.Default.GetHashCode(Cartesian);
            hashCode = hashCode * -1521134295 + EqualityComparer<double[]>.Default.GetHashCode(Array);
            return hashCode;
        }

        IYoonFigure IYoonFigure.Clone()
        {
            return Clone();
        }

        public static YoonVector3D operator *(YoonMatrix4X4Double pMatrix, YoonVector3D pVector)
        {
            YoonVector3D pResultVector = new YoonVector3D();
            for (int i = 0; i < pVector.Count; i++)
            {
                pResultVector.Array[i] = 0;
                for (int k = 0; k < pMatrix.Length; k++)
                    pResultVector.Array[i] += pMatrix.Array[i, k] * pVector.Array[k];
            }
            return pResultVector;
        }
        public static YoonVector3D operator *(YoonVector3D pVector, double dAngle)
        {
            return new YoonVector3D(pVector.X * dAngle, pVector.Y * dAngle, pVector.Z * dAngle);
        }
        public static YoonVector3D operator +(YoonVector3D pVectorSource, YoonVector3D pVectorObject)
        {
            return new YoonVector3D(pVectorSource.X + pVectorObject.X, pVectorSource.Y + pVectorObject.Y, pVectorSource.Z + pVectorObject.Z);
        }
        public static YoonVector3D operator +(YoonVector3D pVector, YoonCartesianD pCart)
        {
            return new YoonVector3D(pVector.X + pCart.X, pVector.Y + pCart.Y, pVector.Z + pCart.Z);
        }
        public static YoonVector3D operator -(YoonVector3D pVectorSource, YoonVector3D pVectorObject)
        {
            return new YoonVector3D(pVectorSource.X - pVectorObject.X, pVectorSource.Y - pVectorObject.Y, pVectorSource.Z - pVectorObject.Z);
        }
        public static YoonVector3D operator -(YoonVector3D pVector)
        {
            return new YoonVector3D(-pVector.X, -pVector.Y, -pVector.Z);
        }
        public static YoonVector3D operator -(YoonVector3D pVector, YoonCartesianD pCart)
        {
            return new YoonVector3D(pVector.X - pCart.X, pVector.Y - pCart.Y, pVector.Z - pCart.Z);
        }
        public static YoonVector3D operator /(YoonVector3D pVector, double dAngle)
        {
            return new YoonVector3D(pVector.X / dAngle, pVector.Y / dAngle, pVector.Z / dAngle);
        }
        public static double operator *(YoonVector3D pVectorSource, YoonVector3D pVectorObject) // dot product
        {
            return pVectorSource.X * pVectorObject.X + pVectorSource.Y * pVectorObject.Y + pVectorSource.Z * pVectorObject.Z;
        }
        public static bool operator ==(YoonVector3D pVectorSource, YoonVector3D pVectorObject)
        {
            return pVectorSource?.Equals(pVectorObject) == true;
        }
        public static bool operator !=(YoonVector3D pVectorSource, YoonVector3D pVectorObject)
        {
            return pVectorSource?.Equals(pVectorObject) == false;
        }
    }

}
