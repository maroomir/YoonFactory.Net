using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenCvSharp;
using System.Drawing;

namespace YoonFactory.CV
{
    public static class CvColorExtension
    {
        public static Scalar ToScalar(this Color pColor)
        {
            return Scalar.FromRgb(pColor.R, pColor.G, pColor.B);
        }
    }

    public static class YoonFigureExtension
    {
        public static OpenCvSharp.Point ToCVPoint(this IYoonVector pVector)
        {
            return pVector switch
            {
                YoonVector2N pVec2N => new OpenCvSharp.Point(pVec2N.X, pVec2N.Y),
                YoonVector2D pVec2D => new OpenCvSharp.Point(pVec2D.X, pVec2D.Y),
                _ => new OpenCvSharp.Point()
            };
        }

        public static Point2d ToCVPoint2D(this IYoonVector pVector)
        {
            return pVector switch
            {
                YoonVector2N pVec2N => new Point2d(pVec2N.X, pVec2N.Y),
                YoonVector2D pVec2D => new Point2d(pVec2D.X, pVec2D.Y),
                _ => new Point2d()
            };
        }

        public static Point3d ToCVPoint3D(this YoonVector3D pVector)
        {
            return new Point3d(pVector.X, pVector.Y, pVector.Z);
        }

        public static Vec2i ToCVVector(this YoonVector2N pVector)
        {
            return new Vec2i(pVector.X, pVector.Y);
        }

        public static Vec2d ToCVVector(this YoonVector2D pVector)
        {
            return new Vec2d(pVector.X, pVector.Y);
        }

        public static Vec3d ToCVVector(this YoonVector3D pVector)
        {
            return new Vec3d(pVector.X, pVector.Y, pVector.Z);
        }

        public static YoonVector2N ToYoonVector(this OpenCvSharp.Point pPos)
        {
            return new YoonVector2N(pPos.X, pPos.Y);
        }

        public static YoonVector2D ToYoonVector(this Point2d pPos)
        {
            return new YoonVector2D(pPos.X, pPos.Y);
        }

        public static YoonVector3D ToYoonVector(this Point3d pPos)
        {
            return new YoonVector3D(pPos.X, pPos.Y, pPos.Z);
        }

        public static YoonVector2N ToYoonVector(this Vec2i pVec)
        {
            return new YoonVector2N(pVec.Item0, pVec.Item1);
        }

        public static YoonVector2D ToYoonVector(this Vec2d pVec)
        {
            return new YoonVector2D(pVec.Item0, pVec.Item1);
        }

        public static YoonVector3D ToYoonVector(this Vec3d pVec)
        {
            return new YoonVector3D(pVec.Item0, pVec.Item1, pVec.Item2);
        }

        public static Rect ToCVRect(this IYoonRect pRect)
        {
            return pRect switch
            {
                YoonRect2N pRect2N => new Rect(pRect2N.TopLeft.ToCVPoint(),
                    new OpenCvSharp.Size(pRect2N.Width, pRect2N.Height)),
                YoonRect2D pRect2D => new Rect(pRect2D.TopLeft.ToCVPoint(),
                    new OpenCvSharp.Size(pRect2D.Width, pRect2D.Height)),
                _ => new Rect()
            };
        }

        public static Rect2d ToCVRect2D(this IYoonRect pRect)
        {
            return pRect switch
            {
                YoonRect2N pRect2N => new Rect2d(pRect2N.TopLeft.ToCVPoint2D(),
                    new Size2d(pRect2N.Width, pRect2N.Height)),
                YoonRect2D pRect2D => new Rect2d(pRect2D.TopLeft.ToCVPoint2D(),
                    new Size2d(pRect2D.Width, pRect2D.Height)),
                _ => new Rect2d()
            };
        }

        public static YoonRect2N ToYoonRect(this Rect pRect)
        {
            YoonVector2N pPosCenter = new YoonVector2N(pRect.X + pRect.Width / 2, pRect.Y + pRect.Height / 2);
            return new YoonRect2N(pPosCenter, pRect.Width, pRect.Height);
        }

        public static YoonRect2D ToYoonRect(this Rect2d pRect)
        {
            YoonVector2D pPosCenter = new YoonVector2D(pRect.X + pRect.Width / 2, pRect.Y + pRect.Height / 2);
            return new YoonRect2D(pPosCenter, pRect.Width, pRect.Height);
        }
    }
}
