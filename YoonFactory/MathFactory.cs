using System;
using System.Collections.Generic;

namespace YoonFactory
{
    public static class MathFactory
    {
        private static readonly double TOLERANCE = 0.00001;

        public static double GetCorrelationCoefficient(byte[] pBufferSource, byte[] pBufferObject, int nWidth,
            int nHeight)
        {
            // Find the average of the source
            int nSize = nWidth * nHeight;
            int nSumSource = 0;
            for (int j = 0; j < nHeight; j++)
            {
                for (int i = 0; i < nWidth; i++)
                {
                    int nValue = pBufferSource[j * nWidth + i];
                    nSumSource += nValue;
                }
            }

            int nAverageSource = nSumSource / nSize;
            // Find the average of the object
            int nSumObject = 0;
            for (int j = 0; j < nHeight; j++)
            {
                for (int i = 0; i < nWidth; i++)
                {
                    int nValue = pBufferObject[j * nWidth + i];
                    nSumObject += nValue;
                }
            }

            int nAverageObject = nSumObject / nSize;
            // Get collection coefficient
            int nDiffX = 0;
            int nDiffY = 0;
            int nDiffXY = 0;
            int nDiffSum = 0;
            for (int j = 0; j < nHeight; j++)
            {
                for (int i = 0; i < nWidth; i++)
                {
                    int nSource = pBufferSource[j * nWidth + i];
                    int nObject = pBufferObject[j * nWidth + i];
                    int nDiffSource = nSource - nAverageSource;
                    int nDiffObject = nObject - nAverageObject;
                    nDiffX += nDiffSource * nDiffSource;
                    nDiffY += nDiffObject * nDiffObject;
                    nDiffXY += nDiffSource * nDiffObject;
                    nDiffSum += Math.Abs(nDiffSource - nDiffObject);
                }
            }

            double dA = Math.Sqrt(nDiffX * nDiffY);
            double dB = (double) nDiffXY;
            if (Math.Abs(dA) < 1)
            {
                dA = 1.0;
            }

            return dB * 100.0 / dA;
        }

        public static double GetCorrelationCoefficient(int[] pBufferSource, int[] pBufferObject, int nWidth,
            int nHeight)
        {
            // Find the average of the source
            int nSize = nWidth * nHeight;
            int nSumSource = 0;
            for (int j = 0; j < nHeight; j++)
            {
                for (int i = 0; i < nWidth; i++)
                {
                    int nSource = pBufferSource[j * nWidth + i];
                    nSumSource += nSource;
                }
            }

            int nAverageSource = nSumSource / nSize;
            // Find the average of the object
            int nSumObject = 0;
            for (int j = 0; j < nHeight; j++)
            {
                for (int i = 0; i < nWidth; i++)
                {
                    int nObject = pBufferObject[j * nWidth + i];
                    nSumObject += nObject;
                }
            }

            int nAverageObject = nSumObject / nSize;
            // Get correlation coefficient
            int nDiffX = 0;
            int nDiffY = 0;
            int nDiffXY = 0;
            int nDiffSum = 0;
            for (int j = 0; j < nHeight; j++)
            {
                for (int i = 0; i < nWidth; i++)
                {
                    int nSource = pBufferSource[j * nWidth + i];
                    int nObject = pBufferObject[j * nWidth + i];
                    int nDiffSource = nSource - nAverageSource;
                    int nDiffObject = nObject - nAverageObject;
                    nDiffX += nDiffSource * nDiffSource;
                    nDiffY += nDiffObject * nDiffObject;
                    nDiffXY += nDiffSource * nDiffObject;
                    nDiffSum += Math.Abs(nDiffSource - nDiffObject);
                }
            }

            double dA = Math.Sqrt(nDiffX * nDiffY);
            double dB = nDiffXY;
            if (Math.Abs(dA) < 1)
            {
                dA = 1.0;
            }

            return dB * 100.0 / dA;
        }

        //  Least Square  (Y = (Slope)X + (intercept))
        public static bool LeastSquare(ref double dSlope, ref double dIntercept, int number, double[] pX, double[] pY)
        {
            //  Check the consistency of data
            for (int i = 0; i < number; i++)
            {
                if (!(pX[i] > 10000) && !(pY[i] > 10000)) continue;
                dSlope = 0;
                dIntercept = 0;
                return false;
            }

            switch (number)
            {
                case < 2:
                case > 10:
                    dSlope = 0;
                    dIntercept = 0;
                    return false;
                case 2:
                    double dDiffX = pX[1] - pX[0];
                    double dDiffY = pY[1] - pY[0];
                    if (Math.Abs(dDiffX) < 0.0001)
                        dDiffX = 0.0001;
                    dIntercept = dDiffY / dDiffX;
                    dSlope = pY[0] - dIntercept * pX[0];
                    break;
                default:
                    double dSumX1 = 0.00;
                    double dSumX2 = 0.00;
                    double dSumXY = 0.00;
                    double dSumY = 0.00;
                    for (int i = 0; i < number; i++)
                    {
                        dSumX1 += pX[i];
                        dSumX2 += pX[i] * pX[i];
                        dSumXY += pX[i] * pY[i];
                        dSumY += pY[i];
                    }

                    double dC = dSumX1 * dSumX1 - (double) number * dSumX2;
                    if (dC == 0.0)
                        return false;
                    double dB = (dSumX1 * dSumY - dSumXY * (double) number) / dC;
                    double dA = (dSumY - dB * dSumX1) / number;
                    dSlope = dA;
                    dIntercept = dB;
                    break;
            }

            return true;
        }

        public static bool LeastSquare(ref double dSlope, ref double dIntercept, int number, int[] pX, int[] pY)
        {
            //  Check the consistency of data
            for (int i = 0; i < number; i++)
            {
                if (pX[i] <= 10000 && pY[i] <= 10000) continue;
                dSlope = 0.0;
                dIntercept = 0.0;
                return false;
            }

            switch (number)
            {
                case < 2:
                case > 10:
                    dSlope = 0;
                    dIntercept = 0;
                    return false;
                case 2:
                {
                    double dDiffX = pX[1] - pX[0];
                    double dDiffY = pY[1] - pY[0];
                    if (Math.Abs(dDiffX) < 0.0001)
                        dDiffX = 0.0001;
                    dIntercept = dDiffY / dDiffX;
                    dSlope = pY[0] - dIntercept * pX[0];
                    break;
                }
                default:
                {
                    double dSumX1 = 0.00;
                    double dSumX2 = 0.00;
                    double dSumXY = 0.00;
                    double dSumY = 0.00;
                    for (int i = 0; i < number; i++)
                    {
                        dSumX1 += pX[i];
                        dSumX2 += pX[i] * pX[i];
                        dSumXY += pX[i] * pY[i];
                        dSumY += pY[i];
                    }

                    double dC = dSumX1 * dSumX1 - (double) number * dSumX2;
                    if (dC == 0.0)
                        return false;
                    double dB = (dSumX1 * dSumY - dSumXY * (double) number) / dC;
                    double dA = (dSumY - dB * dSumX1) / number;
                    dSlope = dA;
                    dIntercept = dB;
                    break;
                }
            }

            return true;
        }

        //  Get (N-1) order function with (N) points
        //  A[0] + A[1]X + A[2]X^2 ...
        public static bool Lagrange(ref double[] pA, ref double[] pX, ref double[] pY, int nCountPoint)
        {
            for (int i = 0; i < nCountPoint; i++)
                pA[i] = 0.0;
            for (int i = 0; i < nCountPoint; i++)
            {
                if (pY[i] == 0.0) continue;
                double dCoefficient = pY[i];
                for (int j = 0; j < nCountPoint; j++)
                {
                    if (i == j) continue;
                    if (Math.Abs(pX[i] - pX[j]) < TOLERANCE) return false;
                    dCoefficient /= pX[i] - pX[j];
                }

                double dTemp = pX[i];
                pX[i] = 0.0;
                for (int j = 0; j < nCountPoint; j++)
                {
                    double dSign = (j % 2 != 0) ? -1.0 : 1.0;
                    pA[nCountPoint - j - 1] += dSign * dCoefficient * Combine(pX, pY, nCountPoint, j);
                }

                pX[i] = dTemp;
            }

            return true;
        }

        // Find a circle passing through three points
        public static bool Get3PointToCircle(YoonVector2N[] pArrayVector, out double dCenterX, out double dCenterY,
            out double dRadius)
        {
            YoonVector2N pVectorCenter1 = new YoonVector2N();
            YoonVector2N pVectorCenter2 = new YoonVector2N();
            pVectorCenter1.X = (pArrayVector[0].X + pArrayVector[1].X) / 2;
            pVectorCenter1.Y = (pArrayVector[0].Y + pArrayVector[1].Y) / 2;
            pVectorCenter2.X = (pArrayVector[0].X + pArrayVector[2].X) / 2;
            pVectorCenter2.Y = (pArrayVector[0].Y + pArrayVector[2].Y) / 2;
            double dDiffX1 = pArrayVector[1].X - pArrayVector[0].X;
            double dDiffX2 = pArrayVector[2].X - pArrayVector[0].X;
            double dDiffY1 = pArrayVector[0].Y - pArrayVector[1].Y;
            double dDiffY2 = pArrayVector[0].Y - pArrayVector[2].Y;
            double dIncline1 = 0.0;
            double dConstant1 = 0.0;
            dCenterX = 0.0;
            dCenterY = 0.0;
            dRadius = 0.0;
            if (dDiffY1 != 0)
            {
                // Find the perpendicular bisector  (Y = a1X + b)
                dIncline1 = dDiffX1 / dDiffY1;
                dConstant1 = pVectorCenter1.Y - dIncline1 * pVectorCenter1.X;
                if (dDiffY2 != 0)
                {
                    // Find the perpendicular bisector   (Y = a2X + b)
                    double dIncline2 = dDiffX2 / dDiffY2;
                    double dConstant2 = pVectorCenter2.Y - dIncline2 * pVectorCenter2.X;
                    if (Math.Abs(dIncline1 - dIncline2) > TOLERANCE)
                        // Find the intersection point
                        dCenterX = (dConstant1 - dConstant2) / (dIncline2 - dIncline1);
                    else
                        return false;
                }
                else if (dDiffX2 == 0)
                    return false;
                else
                    dCenterX = pVectorCenter2.X;
            }
            else if (dDiffY2 != 0 && dDiffX1 != 0)
            {
                dIncline1 = dDiffX2 / dDiffY2;
                dConstant1 = pVectorCenter2.Y - dIncline1 * pVectorCenter2.X;
                dCenterX = pVectorCenter1.X;
            }
            else
                return false;

            // Find the intersection and radius
            dCenterY = dIncline1 * dCenterX + dConstant1;
            dRadius = Math.Sqrt((pArrayVector[0].X - dCenterX) * (pArrayVector[0].X - dCenterX) +
                                (pArrayVector[0].Y - dCenterY) * (pArrayVector[0].Y - dCenterY));
            return true;
        }

        public static bool Get3PointToCircle(YoonVector2D[] pPoint, ref double dCenterX, ref double dCenterY,
            ref double dRadius)
        {
            YoonVector2D pVectorCenter1 = new YoonVector2D();
            YoonVector2D pVectorCenter2 = new YoonVector2D();
            pVectorCenter1.X = (pPoint[0].X + pPoint[1].X) / 2;
            pVectorCenter1.Y = (pPoint[0].Y + pPoint[1].Y) / 2;
            pVectorCenter2.X = (pPoint[0].X + pPoint[2].X) / 2;
            pVectorCenter2.Y = (pPoint[0].Y + pPoint[2].Y) / 2;
            double dDiffX1 = pPoint[1].X - pPoint[0].X;
            double dDiffX2 = pPoint[2].X - pPoint[0].X;
            double dDiffY1 = pPoint[0].Y - pPoint[1].Y;
            double dDiffY2 = pPoint[0].Y - pPoint[2].Y;
            double dIncline1 = 0.0;
            double dConstant1 = 0.0;
            dCenterX = 0.0;
            dCenterY = 0.0;
            dRadius = 0.0;
            if (dDiffY1 != 0)
            {
                // Find the perpendicular bisector  (Y = a1X + b)
                dIncline1 = dDiffX1 / dDiffY1;
                dConstant1 = pVectorCenter1.Y - dIncline1 * pVectorCenter1.X;
                if (dDiffY2 != 0)
                {
                    // Find the perpendicular bisector   (Y = a2X + b)
                    double dIncline2 = dDiffX2 / dDiffY2;
                    double dConstant2 = pVectorCenter2.Y - dIncline2 * pVectorCenter2.X;
                    if (Math.Abs(dIncline1 - dIncline2) > TOLERANCE)
                        // Find the intersection point
                        dCenterX = (dConstant1 - dConstant2) / (dIncline2 - dIncline1);
                    else
                        return false;
                }
                else if (dDiffX2 == 0)
                    return false;
                else
                    dCenterX = pVectorCenter2.X;
            }
            else if (dDiffY2 != 0 && dDiffX1 != 0)
            {
                dIncline1 = dDiffX2 / dDiffY2;
                dConstant1 = pVectorCenter2.Y - dIncline1 * pVectorCenter2.X;
                dCenterX = pVectorCenter1.X;
            }
            else
                return false;

            // Find the intersection and radius
            dCenterY = dIncline1 * dCenterX + dConstant1;
            dRadius = Math.Sqrt((pPoint[0].X - dCenterX) * (pPoint[0].X - dCenterX) +
                                (pPoint[0].Y - dCenterY) * (pPoint[0].Y - dCenterY));
            return true;
        }

        public static double Combine(double[] pX, double[] pY, int nNumber, int nRoot)
        {
            double dResult = 1.0;
            if (nNumber <= 0 || nRoot < 0 || nNumber < nRoot)
                return 0.0;
            ////  nCr
            if (nNumber == nRoot)
            {
                for (int i = 0; i < nNumber; i++)
                    dResult *= pX[i];
                return dResult;
            }

            ////  nC0 = 1
            if (nRoot == 0)
                return 1.0;
            ////  Find the combine
            dResult = pX[nNumber - 1] * Combine(pX, pY, nNumber - 1, nRoot - 1) + Combine(pX, pY, nNumber - 1, nRoot);
            return dResult;
        }

        // Histogram Peak
        public static List<int> GetHistogramPeak(int[] pHistogram, int size, int nDiffParam)
        {
            List<int> pListPeakPos = new List<int>();
            int nMin = 0;
            int nDiffCurr = 0;
            int nDiffPrev = 0;
            for (int i = 1; i < size; i++)
            {
                nDiffCurr = pHistogram[i] - pHistogram[i - 1];
                if (i == 1)
                {
                    nDiffPrev = nDiffCurr;
                    continue;
                }

                // Trend to increase
                if (nDiffCurr >= 0)
                {
                    // Raise from the lower limit
                    if (nDiffPrev < 0)
                    {
                        nMin = pHistogram[i];
                    }
                }
                // Trend to decrease
                else
                {
                    // Fail from the upper limit
                    if (nDiffPrev >= 0)
                    {
                        int nDiffTemp = pHistogram[i] - nMin;
                        // Length from upper limit to lower limit
                        if (nDiffTemp >= nDiffParam)
                        {
                            pListPeakPos.Add(i - 1);
                        }
                    }
                }

                nDiffPrev = nDiffCurr;
            }

            // Calculate the reference to find the center point
            for (int iPeak = 0; iPeak < pListPeakPos.Count; iPeak++)
            {
                int nX0 = pListPeakPos[iPeak];
                if (nX0 < 0 || nX0 >= size)
                    continue;
                // Find the 5 Level lower part
                int nRefer = pHistogram[nX0] - 5;
                // Scan to left
                int nX1 = nX0;
                for (int i = nX0; i >= 0; i--)
                {
                    if (pHistogram[i] <= nRefer)
                    {
                        nX1 = i;
                        break;
                    }
                }

                // Scan to right
                int nX2 = nX0;
                for (int i = nX0; i < size; i++)
                {
                    if (pHistogram[i] <= nRefer)
                    {
                        nX2 = i;
                        break;
                    }
                }

                pListPeakPos[iPeak] = (nX1 + nX2) / 2;
            }

            return pListPeakPos;
        }
    }
}