using System;

namespace YoonFactory.Align
{
    public static class AlignFactory
    {
        private const double INVALID_NUM = -1000.0;
        private const double TOLERANCE = 0.00001;

        public static void Align1D(AlignObject pObject, out double dX, out double dY, out double dTheta,
            double dScale = 1.0)
        {
            YoonVector2D pObjectVector = pObject.GetCurrentVector2D();
            YoonVector2D pOriginVector = pObject.GetOriginVector2D();
            CalculateAlign1D(pObjectVector, pOriginVector, pObject.GetCurrentTheta(), pObject.GetOriginTheta(),
                out dX, out dY, out dTheta, dScale);
        }

        public static void Align2D(eYoonDir2D nDir, AlignObject pLeftObject, AlignObject pRightObject, out double dX,
            out double dY, out double dTheta, double dScale = 1.0)
        {
            YoonVector2D pObjectVectorLeft = pLeftObject.GetCurrentVector2D();
            YoonVector2D pObjectVectorRight = pRightObject.GetCurrentVector2D();
            YoonVector2D pOriginVectorLeft = pLeftObject.GetOriginVector2D();
            YoonVector2D pOriginVectorRight = pRightObject.GetOriginVector2D();
            dTheta = CalculateTheta2D(pObjectVectorLeft, pObjectVectorRight, pOriginVectorLeft, pOriginVectorRight,
                dScale);
            YoonVector2D pResultVector = new YoonVector2D();
            if (nDir == pLeftObject.Direction)
                pResultVector = CalculateXY2D(pObjectVectorLeft, pOriginVectorLeft, dTheta, dScale);
            else if (nDir == pRightObject.Direction)
                pResultVector = CalculateXY2D(pObjectVectorRight, pOriginVectorRight, dTheta, dScale);
            else
            {
                pResultVector.X = INVALID_NUM;
                pResultVector.Y = INVALID_NUM;
            }

            dX = pResultVector.X;
            dY = pResultVector.Y;
        }

        private static void CalculateAlign1D(YoonVector2D pObjectVector, YoonVector2D pOriginVector, double dObjectTheta,
            double dOriginTheta, out double dX, out double dY, out double dTheta, double dScale = 1.0)
        {
            YoonVector2D pResultVector = pOriginVector - pObjectVector;
            dX = pResultVector.X * dScale;
            dY = pResultVector.Y * dScale;
            dTheta = (dOriginTheta - dObjectTheta) * dScale;
        }

        private static double CalculateTheta2D(YoonVector2D pObjectVectorLeft, YoonVector2D pObjectVectorRight,
            YoonVector2D pOriginVectorLeft, YoonVector2D pOriginVectorRight, double dScale = 1.0)
        {
            // Calculate the angle at which the "Y-axis" and "origin" by the rotation center
            double thetaObject = Math.Atan2(pOriginVectorRight.X - pOriginVectorLeft.X,
                pOriginVectorLeft.Y - pOriginVectorRight.Y);
            if (double.IsNaN(thetaObject)) return INVALID_NUM;

            // Calculate the angle at which the "Y-axis" and "mark" by the rotation center
            double thetaMark = Math.Atan2(pObjectVectorRight.X - pObjectVectorLeft.X,
                pObjectVectorLeft.Y - pObjectVectorRight.Y);
            if (double.IsNaN(thetaMark)) return INVALID_NUM;

            return (thetaObject - thetaMark) * dScale;
        }

        private static YoonVector2D CalculateXY2D(YoonVector2D pObjectVector, YoonVector2D pOriginVector, double dTheta,
            double dScale = 1.0)
        {
            if (Math.Abs(dTheta - INVALID_NUM) < TOLERANCE)
                return new YoonVector2D(INVALID_NUM, INVALID_NUM);

            // Calculate the align vector
            return new YoonVector2D()
            {
                X = (pOriginVector.X - (pObjectVector.X * Math.Cos(dTheta) - pObjectVector.Y * Math.Sin(dTheta))) *
                    dScale,
                Y = (pOriginVector.Y - (pObjectVector.X * Math.Sin(dTheta) + pObjectVector.Y * Math.Cos(dTheta))) *
                    dScale,
            };
        }
    }
}
