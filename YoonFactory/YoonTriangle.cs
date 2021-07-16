using System.Xml.Serialization;

namespace YoonFactory
{
    public class YoonTriangle2N : IYoonTriangle2D<int>
    {
        public IYoonTriangle Clone()
        {
            return new YoonTriangle2N {X = this.X, Y = this.Y, Height = this.Height};
        }

        public void CopyFrom(IYoonTriangle pTriangle)
        {
            if (pTriangle is not YoonTriangle2N pTriangle2N) return;
            X = pTriangle2N.X;
            Y = pTriangle2N.Y;
            Height = pTriangle2N.Height;
        }

        [XmlAttribute] public int X { get; set; }
        [XmlAttribute] public int Y { get; set; }
        [XmlAttribute] public int Height { get; set; }

        public YoonTriangle2N()
        {
            X = 0;
            Y = 0;
            Height = 0;
        }

        public YoonTriangle2N(int nX, int nY, int nHeight)
        {
            X = nX;
            Y = nY;
            Height = nHeight;
        }

        public int Area()
        {
            return X * Y / 2;
        }

        IYoonFigure IYoonFigure.Clone()
        {
            return Clone();
        }
    }

    public class YoonTriangle2D : IYoonTriangle, IYoonTriangle2D<double>
    {
        public IYoonTriangle Clone()
        {
            return new YoonTriangle2D {X = this.X, Y = this.Y, Height = this.Height};
        }

        public void CopyFrom(IYoonTriangle pTriangle)
        {
            if (pTriangle is not YoonTriangle2D pTriangle2D) return;
            X = pTriangle2D.X;
            Y = pTriangle2D.Y;
            Height = pTriangle2D.Height;
        }

        [XmlAttribute] public double X { get; set; }
        [XmlAttribute] public double Y { get; set; }
        [XmlAttribute] public double Height { get; set; }

        public YoonTriangle2D()
        {
            X = 0;
            Y = 0;
            Height = 0;
        }

        public YoonTriangle2D(double dX, double dY, double dHeight)
        {
            X = dX;
            Y = dY;
            Height = dHeight;
        }

        public double Area()
        {
            return 0.5 * X * Y;
        }

        IYoonFigure IYoonFigure.Clone()
        {
            return Clone();
        }
    }
}
