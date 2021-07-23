using System;
using System.Collections.Generic;
using System.Xml.Serialization;

namespace YoonFactory
{
    public class YoonRectAffine2D : IYoonRect2D<double>, IEquatable<YoonRectAffine2D>
    {
        public override bool Equals(object obj)
        {
            return Equals(obj as IYoonRect);
        }

        public bool Equals(IYoonRect pRect)
        {
            if (pRect is not YoonRectAffine2D pRectAffine2D) return false;
            return CenterPos.X == pRectAffine2D.CenterPos.X &&
                   CenterPos.Y == pRectAffine2D.CenterPos.Y &&
                   Width == pRectAffine2D.Width &&
                   Height == pRectAffine2D.Height &&
                   Rotation == pRectAffine2D.Rotation;
        }

        public IYoonRect Clone()
        {
            return new YoonRectAffine2D(0.0, 0.0, 0.0)
            {
                CenterPos = this.CenterPos.Clone() as YoonVector2D,
                Width = this.Width,
                Height = this.Height,
                Rotation = this.Rotation
            };
        }

        public void CopyFrom(IYoonRect pRect)
        {
            if (pRect is not YoonRectAffine2D pRectAffine2D) return;
            CenterPos = pRectAffine2D.CenterPos.Clone() as YoonVector2D;
            Width = pRectAffine2D.Width;
            Height = pRectAffine2D.Height;
            Rotation = pRectAffine2D.Rotation;
        }

        [XmlAttribute]
        public IYoonVector2D<double> CenterPos
        {
            get => _pCenter;
            set
            {
                _pCenter = value as YoonVector2D;
                InitRectOrigin((YoonVector2D) _pCenter, _dWidth, _dHeight);
            }
        }

        [XmlAttribute]
        public double Width
        {
            get => _dWidth;
            set
            {
                _dWidth = value;
                InitRectOrigin(_pCenter as YoonVector2D, _dWidth, _dHeight);
            }
        }

        [XmlAttribute]
        public double Height
        {
            get => _dHeight;
            set
            {
                _dHeight = value;
                InitRectOrigin(_pCenter as YoonVector2D, _dWidth, _dHeight);
            }
        }

        [XmlAttribute]
        public double Rotation
        {
            get => _dRotation;
            set
            {
                _dRotation = value;
                InitRectOrigin(_pCenter as YoonVector2D, _dWidth, _dHeight);

                _pCornerRotateTopLeft = _pCornerOriginTopLeft.GetRotateVector(_pCenter, _dRotation) as YoonVector2D;
                _pCornerRotateBottomLeft =
                    _pCornerOriginBottomLeft.GetRotateVector(_pCenter, _dRotation) as YoonVector2D;
                _pCornerRotateTopRight = _pCornerOriginTopRight.GetRotateVector(_pCenter, _dRotation) as YoonVector2D;
                _pCornerRotateBottomRight =
                    _pCornerOriginBottomRight.GetRotateVector(_pCenter, _dRotation) as YoonVector2D;
            }
        }

        private void InitRectOrigin(YoonVector2D vecCenter, double dWidth, double dHeight)
        {
            _pCornerOriginTopLeft.X = vecCenter.X - dWidth / 2;
            _pCornerOriginTopLeft.Y = vecCenter.Y - dHeight / 2;
            _pCornerOriginTopRight.X = vecCenter.X + dWidth / 2;
            _pCornerOriginTopRight.Y = vecCenter.Y - dHeight / 2;
            _pCornerOriginBottomLeft.X = vecCenter.X - dWidth / 2;
            _pCornerOriginBottomLeft.Y = vecCenter.Y + dHeight / 2;
            _pCornerOriginBottomRight.X = vecCenter.X + dWidth / 2;
            _pCornerOriginBottomRight.Y = vecCenter.Y + dHeight / 2;
        }

        private readonly YoonVector2D _pCornerOriginTopLeft = new YoonVector2D();
        private readonly YoonVector2D _pCornerOriginBottomLeft = new YoonVector2D();
        private readonly YoonVector2D _pCornerOriginTopRight = new YoonVector2D();
        private readonly YoonVector2D _pCornerOriginBottomRight = new YoonVector2D();
        private YoonVector2D _pCornerRotateTopLeft = null;
        private YoonVector2D _pCornerRotateBottomLeft = null;
        private YoonVector2D _pCornerRotateTopRight = null;
        private YoonVector2D _pCornerRotateBottomRight = null;
        private YoonVector2D _pCenter = new YoonVector2D(); // Set 내 InitOrigin과 충돌에 따른 StackOverflow 방지용
        private double _dWidth = 0.0; // Set 내 InitOrigin과 충돌에 따른 StackOverflow 방지용
        private double _dHeight = 0.0; // Set 내 InitOrigin과 충돌에 따른 StackOverflow 방지용
        private double _dRotation = 0.0; // Set 내 InitOrigin과 충돌에 따른 StackOverflow 방지용

        public double Left => CenterPos.X - Width / 2;

        public double Top => CenterPos.Y - Height / 2;

        public double Right => CenterPos.X + Width / 2;

        public double Bottom => CenterPos.Y + Height / 2;

        public IYoonVector2D<double> TopLeft => _pCornerRotateTopLeft;

        public IYoonVector2D<double> TopRight => _pCornerRotateTopRight;

        public IYoonVector2D<double> BottomLeft => _pCornerRotateBottomLeft;

        public IYoonVector2D<double> BottomRight => _pCornerRotateBottomRight;

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

        public YoonRectAffine2D()
        {
            CenterPos = new YoonVector2D {X = 0, Y = 0};
            Width = 0;
            Height = 0;
            Rotation = 0;
        }

        public YoonRectAffine2D(double dWidth, double dHeight, double dTheta)
        {
            CenterPos = new YoonVector2D {X = 0, Y = 0};
            Width = dWidth;
            Height = dHeight;

            Rotation = dTheta;
        }

        public YoonRectAffine2D(double dX, double dY, double dWidth, double dHeight, double dTheta)
        {
            CenterPos = new YoonVector2D {X = dX, Y = dY};
            Width = dWidth;
            Height = dHeight;

            Rotation = dTheta;
        }

        public YoonRectAffine2D(YoonVector2D pVector, double dWidth, double dHeight, double dTheta)
        {
            CenterPos = pVector.Clone() as YoonVector2D;
            Width = dWidth;
            Height = dHeight;
            Rotation = dTheta;
        }

        public bool IsContain(IYoonVector pVector)
        {
            if (pVector is not YoonVector2D pVector2D) return false;
            return _pCornerRotateBottomLeft.X < pVector2D.X && _pCornerRotateTopLeft.X < pVector2D.X &&
                   pVector2D.X < _pCornerRotateBottomRight.X && pVector2D.X < _pCornerRotateTopRight.X &&
                   _pCornerRotateTopLeft.Y < pVector2D.Y && _pCornerRotateTopRight.Y < pVector2D.Y &&
                   pVector2D.Y < _pCornerRotateBottomLeft.Y && pVector2D.Y < _pCornerRotateBottomRight.Y;
        }

        public double Area()
        {
            return Width * Height;
        }

        public bool Equals(YoonRectAffine2D other)
        {
            return other != null &&
                   EqualityComparer<IYoonVector2D<double>>.Default.Equals(CenterPos, other.CenterPos) &&
                   Width == other.Width &&
                   Height == other.Height &&
                   Rotation == other.Rotation &&
                   EqualityComparer<YoonVector2D>.Default.Equals(_pCornerOriginTopLeft, other._pCornerOriginTopLeft) &&
                   EqualityComparer<YoonVector2D>.Default.Equals(_pCornerOriginBottomLeft,
                       other._pCornerOriginBottomLeft) &&
                   EqualityComparer<YoonVector2D>.Default.Equals(_pCornerOriginTopRight,
                       other._pCornerOriginTopRight) &&
                   EqualityComparer<YoonVector2D>.Default.Equals(_pCornerOriginBottomRight,
                       other._pCornerOriginBottomRight) &&
                   EqualityComparer<YoonVector2D>.Default.Equals(_pCornerRotateTopLeft, other._pCornerRotateTopLeft) &&
                   EqualityComparer<YoonVector2D>.Default.Equals(_pCornerRotateBottomLeft,
                       other._pCornerRotateBottomLeft) &&
                   EqualityComparer<YoonVector2D>.Default.Equals(_pCornerRotateTopRight,
                       other._pCornerRotateTopRight) &&
                   EqualityComparer<YoonVector2D>.Default.Equals(_pCornerRotateBottomRight,
                       other._pCornerRotateBottomRight) &&
                   EqualityComparer<YoonVector2D>.Default.Equals(_pCenter, other._pCenter) &&
                   _dWidth == other._dWidth &&
                   _dHeight == other._dHeight &&
                   _dRotation == other._dRotation &&
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
            int hashCode = -408325479;
            hashCode = hashCode * -1521134295 + EqualityComparer<IYoonVector2D<double>>.Default.GetHashCode(CenterPos);
            hashCode = hashCode * -1521134295 + Width.GetHashCode();
            hashCode = hashCode * -1521134295 + Height.GetHashCode();
            hashCode = hashCode * -1521134295 + Rotation.GetHashCode();
            hashCode = hashCode * -1521134295 +
                       EqualityComparer<YoonVector2D>.Default.GetHashCode(_pCornerOriginTopLeft);
            hashCode = hashCode * -1521134295 +
                       EqualityComparer<YoonVector2D>.Default.GetHashCode(_pCornerOriginBottomLeft);
            hashCode = hashCode * -1521134295 +
                       EqualityComparer<YoonVector2D>.Default.GetHashCode(_pCornerOriginTopRight);
            hashCode = hashCode * -1521134295 +
                       EqualityComparer<YoonVector2D>.Default.GetHashCode(_pCornerOriginBottomRight);
            hashCode = hashCode * -1521134295 +
                       EqualityComparer<YoonVector2D>.Default.GetHashCode(_pCornerRotateTopLeft);
            hashCode = hashCode * -1521134295 +
                       EqualityComparer<YoonVector2D>.Default.GetHashCode(_pCornerRotateBottomLeft);
            hashCode = hashCode * -1521134295 +
                       EqualityComparer<YoonVector2D>.Default.GetHashCode(_pCornerRotateTopRight);
            hashCode = hashCode * -1521134295 +
                       EqualityComparer<YoonVector2D>.Default.GetHashCode(_pCornerRotateBottomRight);
            hashCode = hashCode * -1521134295 + EqualityComparer<YoonVector2D>.Default.GetHashCode(_pCenter);
            hashCode = hashCode * -1521134295 + _dWidth.GetHashCode();
            hashCode = hashCode * -1521134295 + _dHeight.GetHashCode();
            hashCode = hashCode * -1521134295 + _dRotation.GetHashCode();
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

        public static bool operator ==(YoonRectAffine2D r1, YoonRectAffine2D r2)
        {
            return r1?.Equals(r2) == true;
        }

        public static bool operator !=(YoonRectAffine2D r1, YoonRectAffine2D r2)
        {
            return r1?.Equals(r2) == false;
        }
    }
}
