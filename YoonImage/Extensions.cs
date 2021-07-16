using System.Drawing;

namespace YoonFactory
{
    public static class YoonFigureExtension
    {
        public static Point ToPoint(this IYoonVector pVector)
        {
            return pVector switch
            {
                YoonVector2N pVec2N => new Point(pVec2N.X, pVec2N.Y),
                YoonVector2D pVec2D => new Point((int) pVec2D.X, (int) pVec2D.Y),
                _ => new Point()
            };
        }

        public static PointF ToPointF(this IYoonVector pVector)
        {
            return pVector switch
            {
                YoonVector2N pVec2N => new PointF(pVec2N.X, pVec2N.Y),
                YoonVector2D pVec2D => new PointF((float) pVec2D.X, (float) pVec2D.Y),
                _ => new PointF()
            };
        }

        public static YoonVector2N ToYoonVector(this Point pPos)
        {
            return new YoonVector2N(pPos.X, pPos.Y);
        }

        public static YoonVector2D ToYoonVector(this PointF pPos)
        {
            return new YoonVector2D(pPos.X, pPos.Y);
        }

        public static Rectangle ToRectangle(this IYoonRect pRect)
        {
            return pRect switch
            {
                YoonRect2N pRect2N => new Rectangle(pRect2N.TopLeft.ToPoint(), new Size(pRect2N.Width, pRect2N.Height)),
                YoonRect2D pRect2D => new Rectangle(pRect2D.TopLeft.ToPoint(),
                    new Size((int) pRect2D.Width, (int) pRect2D.Height)),
                _ => new Rectangle()
            };
        }

        public static RectangleF ToRectangleF(this IYoonRect pRect)
        {
            return pRect switch
            {
                YoonRect2N pRect2N => new RectangleF(pRect2N.TopLeft.ToPointF(),
                    new Size(pRect2N.Width, pRect2N.Height)),
                YoonRect2D pRect2D => new RectangleF(pRect2D.TopLeft.ToPointF(),
                    new SizeF((float) pRect2D.Width, (float) pRect2D.Height)),
                _ => new Rectangle()
            };
        }

        public static YoonRect2N ToYoonRect(this Rectangle pRect)
        {
            YoonVector2N pPosCenter = new YoonVector2N(pRect.X + pRect.Width / 2, pRect.Y + pRect.Height / 2);
            return new YoonRect2N(pPosCenter, pRect.Width, pRect.Height);
        }

        public static YoonRect2D ToYoonRect(this RectangleF pRect)
        {
            YoonVector2D pPosCenter = new YoonVector2D(pRect.X + pRect.Width / 2, pRect.Y + pRect.Height / 2);
            return new YoonRect2D(pPosCenter, pRect.Width, pRect.Height);
        }
    }
}
