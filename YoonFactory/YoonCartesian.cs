using System.Xml.Serialization;

namespace YoonFactory
{
    public class YoonCartesianN : IYoonCartesian<int>
    {
        public IYoonCartesian Clone()
        {
            return new YoonCartesianN
            {
                X = X,
                Y = Y,
                Z = Z,
                RX = RX,
                RY = RY,
                RZ = RZ
            };
        }

        public void CopyFrom(IYoonCartesian pCart)
        {
            if (pCart is YoonCartesianN pCartInt)
            {
                X = pCartInt.X;
                Y = pCartInt.Y;
                Z = pCartInt.Z;
                RX = pCartInt.RX;
                RY = pCartInt.RY;
                RZ = pCartInt.RZ;
            }
        }

        [XmlIgnore] public int X { get; set; }
        [XmlAttribute] public int Y { get; set; }
        [XmlAttribute] public int Z { get; set; }
        [XmlIgnore] public int RX { get; set; }
        [XmlAttribute] public int RY { get; set; }
        [XmlAttribute] public int RZ { get; set; }

        public int[] Array
        {
            get => new int[6] {X, Y, Z, RX, RY, RZ};
            set
            {
                X = value[0];
                Y = value[1];
                Z = value[2];
                RX = value[3];
                RY = value[4];
                RZ = value[5];
            }
        }

        public YoonCartesianN()
        {
            Unit();
        }

        public YoonCartesianN(IYoonCartesian c)
        {
            CopyFrom(c);
        }

        public YoonCartesianN(int nX, int nY, int nZ, int nRX, int nRY, int nRZ)
        {
            X = nX;
            Y = nY;
            Z = nZ;
            RX = nRX;
            RY = nRY;
            RZ = nRZ;
        }

        public void Zero()
        {
            X = Y = Z = RX = RY = RZ = 0;
        }

        public void Unit()
        {
            X = Y = Z = 1;
            RX = RY = RZ = 0;
        }

        public static YoonCartesianN operator *(YoonCartesianN pCart, int nNum)
        {
            return new YoonCartesianN(pCart.X * nNum, pCart.Y * nNum, pCart.Z * nNum, pCart.RX, pCart.RY, pCart.RZ);
        }

        public static YoonCartesianN operator +(YoonCartesianN pCartSource, YoonCartesianN pCartObject)
        {
            return new YoonCartesianN(pCartSource.X + pCartObject.X, pCartSource.Y + pCartObject.Y,
                pCartSource.Z + pCartObject.Z, pCartSource.RX + pCartObject.RX, pCartSource.RY + pCartObject.RY,
                pCartSource.RZ + pCartObject.RZ);
        }

        public static YoonCartesianN operator +(YoonCartesianN pCart, YoonVector2N pVector)
        {
            return new YoonCartesianN(pCart.X + pVector.X, pCart.Y + pVector.Y, pCart.Z, pCart.RX, pCart.RY, pCart.RZ);
        }

        public static YoonCartesianN operator -(YoonCartesianN pCartSource, YoonCartesianN pCartObject)
        {
            return new YoonCartesianN(pCartSource.X - pCartObject.X, pCartSource.Y - pCartObject.Y,
                pCartSource.Z - pCartObject.Z, pCartSource.RX - pCartObject.RX, pCartSource.RY - pCartObject.RY,
                pCartSource.RZ - pCartObject.RZ);
        }

        public static YoonCartesianN operator -(YoonCartesianN pCart, YoonVector2N pVector)
        {
            return new YoonCartesianN(pCart.X - pVector.X, pCart.Y - pVector.Y, pCart.Z, pCart.RX, pCart.RY, pCart.RZ);
        }

        public static YoonCartesianN operator /(YoonCartesianN pCart, int nNum)
        {
            return new YoonCartesianN(pCart.X / nNum, pCart.Y / nNum, pCart.Z / nNum, pCart.RX, pCart.RY, pCart.RZ);
        }
    }

    public class YoonCartesianD : IYoonCartesian<double>
    {
        public IYoonCartesian Clone()
        {
            return new YoonCartesianD
            {
                X = X,
                Y = Y,
                Z = Z,
                RX = RX,
                RY = RY,
                RZ = RZ
            };
        }

        public void CopyFrom(IYoonCartesian pCart)
        {
            if (pCart is YoonCartesianD pCartDouble)
            {
                X = pCartDouble.X;
                Y = pCartDouble.Y;
                Z = pCartDouble.Z;
                RX = pCartDouble.RX;
                RY = pCartDouble.RY;
                RZ = pCartDouble.RZ;
            }
        }

        [XmlIgnore] public double X { get; set; }
        [XmlAttribute] public double Y { get; set; }
        [XmlAttribute] public double Z { get; set; }
        [XmlIgnore] public double RX { get; set; }
        [XmlAttribute] public double RY { get; set; }
        [XmlAttribute] public double RZ { get; set; }

        public double[] Array
        {
            get => new double[6] {X, Y, Z, RX, RY, RZ};
            set
            {
                X = value[0];
                Y = value[1];
                Z = value[2];
                RX = value[3];
                RY = value[4];
                RZ = value[5];
            }
        }

        public YoonCartesianD()
        {
            Unit();
        }

        public YoonCartesianD(IYoonCartesian pCart)
        {
            CopyFrom(pCart);
        }

        public YoonCartesianD(double dX, double dY, double dZ, double dRX, double dRY, double dRZ)
        {
            X = dX;
            Y = dY;
            Z = dZ;
            RX = dRX;
            RY = dRY;
            RZ = dRZ;
        }

        public void Zero()
        {
            X = Y = Z = RX = RY = RZ = 0;
        }

        public void Unit()
        {
            X = Y = Z = 1;
            RX = RY = RZ = 0;
        }

        public static YoonCartesianD operator *(YoonCartesianD pCart, double dNum)
        {
            return new YoonCartesianD(pCart.X * dNum, pCart.Y * dNum, pCart.Z * dNum, pCart.RX, pCart.RY, pCart.RZ);
        }

        public static YoonCartesianD operator +(YoonCartesianD pCartSource, YoonCartesianD pCartObject)
        {
            return new YoonCartesianD(pCartSource.X + pCartObject.X, pCartSource.Y + pCartObject.Y,
                pCartSource.Z + pCartObject.Z, pCartSource.RX + pCartObject.RX, pCartSource.RY + pCartObject.RY,
                pCartSource.RZ + pCartObject.RZ);
        }

        public static YoonCartesianD operator +(YoonCartesianD pCart, YoonVector2D pVector)
        {
            return new YoonCartesianD(pCart.X + pVector.X, pCart.Y + pVector.Y, pCart.Z, pCart.RX, pCart.RY, pCart.RZ);
        }

        public static YoonCartesianD operator -(YoonCartesianD pCartSource, YoonCartesianD pCartObject)
        {
            return new YoonCartesianD(pCartSource.X - pCartObject.X, pCartSource.Y - pCartObject.Y,
                pCartSource.Z - pCartObject.Z, pCartSource.RX - pCartObject.RX, pCartSource.RY - pCartObject.RY,
                pCartSource.RZ - pCartObject.RZ);
        }

        public static YoonCartesianD operator -(YoonCartesianD pCart, YoonVector2D pVector)
        {
            return new YoonCartesianD(pCart.X - pVector.X, pCart.Y - pVector.Y, pCart.Z, pCart.RX, pCart.RY, pCart.RZ);
        }

        public static YoonCartesianD operator /(YoonCartesianD pCart, double dNum)
        {
            return new YoonCartesianD(pCart.X / dNum, pCart.Y / dNum, pCart.Z / dNum, pCart.RX, pCart.RY, pCart.RZ);
        }
    }
}
