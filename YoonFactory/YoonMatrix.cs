using System;
using System.Collections.Generic;

namespace YoonFactory
{
    public class YoonMatrix2X2Int : IYoonMatrix, IEquatable<YoonMatrix2X2Int>
    {
        public int Length { get; } = 2;

        public int matrix_11
        {
            get => Array[0, 0];

            set => Array[0, 0] = value;
        }

        public int matrix_12
        {
            get => Array[0, 1];

            set => Array[0, 1] = value;
        }

        public int matrix_21
        {
            get => Array[1, 0];

            set => Array[1, 0] = value;
        }

        public int matrix_22
        {
            get => Array[1, 1];

            set => Array[1, 1] = value;
        }

        public YoonMatrix2X2Int()
        {
            matrix_11 = matrix_22 = 1;
            matrix_12 = matrix_21 = 0;
        }

        public YoonMatrix2X2Int(IYoonMatrix m)
        {
            this.CopyFrom(m);
        }

        public int[,] Array { get; set; } = new int[2, 2];

        public int Determinant => matrix_11 * matrix_22 - matrix_12 * matrix_21;

        public IYoonMatrix Clone()
        {
            return new YoonMatrix2X2Int
            {
                matrix_11 = matrix_11, matrix_12 = matrix_12, matrix_21 = matrix_21, matrix_22 = matrix_22
            };
        }

        public void CopyFrom(IYoonMatrix pMatrix)
        {
            if (pMatrix is YoonMatrix2X2Int pMatrixInt)
            {
                matrix_11 = pMatrixInt.matrix_11;
                matrix_12 = pMatrixInt.matrix_12;
                matrix_21 = pMatrixInt.matrix_21;
                matrix_22 = pMatrixInt.matrix_22;
            }
        }

        public IYoonMatrix Inverse()
        {
            if (Determinant == 0) return Unit();

            YoonMatrix2X2Int pMatrix = new YoonMatrix2X2Int(this);
            matrix_11 = pMatrix.matrix_22 / pMatrix.Determinant;
            matrix_12 = -pMatrix.matrix_12 / pMatrix.Determinant;
            matrix_21 = -pMatrix.matrix_21 / pMatrix.Determinant;
            matrix_22 = pMatrix.matrix_11 / pMatrix.Determinant;
            return this;
        }

        public IYoonMatrix Transpose()
        {
            YoonMatrix2X2Int pMatrix = new YoonMatrix2X2Int(this);
            matrix_12 = pMatrix.matrix_21;
            matrix_21 = pMatrix.matrix_12;
            return this;
        }

        public IYoonMatrix Unit()
        {
            matrix_11 = matrix_22 = 1;
            matrix_12 = matrix_21 = 0;
            return this;
        }

        public override bool Equals(object obj)
        {
            return Equals(obj as YoonMatrix2X2Int);
        }

        public bool Equals(YoonMatrix2X2Int other)
        {
            return other != null &&
                   Length == other.Length &&
                   matrix_11 == other.matrix_11 &&
                   matrix_12 == other.matrix_12 &&
                   matrix_21 == other.matrix_21 &&
                   matrix_22 == other.matrix_22 &&
                   EqualityComparer<int[,]>.Default.Equals(Array, other.Array) &&
                   Determinant == other.Determinant;
        }

        public override int GetHashCode()
        {
            int hashCode = 915136901;
            hashCode = hashCode * -1521134295 + Length.GetHashCode();
            hashCode = hashCode * -1521134295 + matrix_11.GetHashCode();
            hashCode = hashCode * -1521134295 + matrix_12.GetHashCode();
            hashCode = hashCode * -1521134295 + matrix_21.GetHashCode();
            hashCode = hashCode * -1521134295 + matrix_22.GetHashCode();
            hashCode = hashCode * -1521134295 + EqualityComparer<int[,]>.Default.GetHashCode(Array);
            hashCode = hashCode * -1521134295 + Determinant.GetHashCode();
            return hashCode;
        }

        public static YoonMatrix2X2Int operator *(int nNum, YoonMatrix2X2Int pMatrix)
        {
            YoonMatrix2X2Int pResultMatrix = new YoonMatrix2X2Int();
            for (int i = 0; i < pResultMatrix.Length; i++)
            {
                for (int j = 0; j < pResultMatrix.Length; j++)
                {
                    pResultMatrix.Array[i, j] = nNum * pMatrix.Array[i, j];
                }
            }
            return pResultMatrix;
        }

        public static YoonMatrix2X2Int operator /(YoonMatrix2X2Int pMatrix, int nNum)
        {
            YoonMatrix2X2Int pResultMatrix = new YoonMatrix2X2Int();
            for (int i = 0; i < pResultMatrix.Length; i++)
            {
                for (int j = 0; j < pResultMatrix.Length; j++)
                {
                    pResultMatrix.Array[i, j] = pMatrix.Array[i, j] / nNum;
                }
            }
            return pResultMatrix;
        }

        public static YoonMatrix2X2Int operator *(YoonMatrix2X2Int nNum, YoonMatrix2X2Int pMatrix)
        {
            YoonMatrix2X2Int pResultMatrix = new YoonMatrix2X2Int();
            for (int i = 0; i < pResultMatrix.Length; i++)
            {
                for (int j = 0; j < pResultMatrix.Length; j++)
                {
                    pResultMatrix.Array[i, j] = 0;
                    for (int kValue = 0; kValue < pResultMatrix.Length; kValue++)
                        pResultMatrix.Array[i, j] += (nNum.Array[i, kValue] * pMatrix.Array[kValue, j]);
                }
            }
            return pResultMatrix;
        }

        public static bool operator ==(YoonMatrix2X2Int pMatrixSource, YoonMatrix2X2Int pMatrixObject)
        {
            for (int i = 0; i < pMatrixSource?.Length; i++)
            {
                for (int j = 0; j < pMatrixSource?.Length; j++)
                {
                    if (pMatrixSource?.Array[i, j] != pMatrixObject?.Array[i, j])
                        return false;
                }
            }
            return true;
        }

        public static bool operator !=(YoonMatrix2X2Int pMatrixSource, YoonMatrix2X2Int pMatrixObject)
        {
            for (int i = 0; i < pMatrixSource?.Length; i++)
            {
                for (int j = 0; j < pMatrixSource?.Length; j++)
                {
                    if (pMatrixSource?.Array[i, j] != pMatrixObject?.Array[i, j])
                        return true;
                }
            }
            return false;
        }
    }

    public class YoonMatrix2X2Double : IYoonMatrix, IEquatable<YoonMatrix2X2Double>
    {
        public int Length { get; } = 2;

        public double matrix_11
        {
            get => Array[0, 0];

            set => Array[0, 0] = value;
        }

        public double matrix_12
        {
            get => Array[0, 1];

            set => Array[0, 1] = value;
        }

        public double matrix_21
        {
            get => Array[1, 0];

            set => Array[1, 0] = value;
        }

        public double matrix_22
        {
            get => Array[1, 1];

            set => Array[1, 1] = value;
        }

        public YoonMatrix2X2Double()
        {
            matrix_11 = matrix_22 = 1;
            matrix_12 = matrix_21 = 0;
        }

        public YoonMatrix2X2Double(IYoonMatrix m)
        {
            this.CopyFrom(m);
        }

        public double[,] Array { get; set; } = new double[2, 2];

        public double Determinant => matrix_11 * matrix_22 - matrix_12 * matrix_21;

        public IYoonMatrix Clone()
        {
            return new YoonMatrix2X2Double
            {
                matrix_11 = matrix_11, matrix_12 = matrix_12, matrix_21 = matrix_21, matrix_22 = matrix_22
            };
        }

        public void CopyFrom(IYoonMatrix pMatrix)
        {
            if (pMatrix is YoonMatrix2X2Double pMatrixDouble)
            {
                matrix_11 = pMatrixDouble.matrix_11;
                matrix_12 = pMatrixDouble.matrix_12;
                matrix_21 = pMatrixDouble.matrix_21;
                matrix_22 = pMatrixDouble.matrix_22;
            }
        }

        public IYoonMatrix Inverse()
        {
            if (Determinant == 0) return Unit();
            YoonMatrix2X2Double m = new YoonMatrix2X2Double(this);
            matrix_11 = m.matrix_22 / m.Determinant;
            matrix_12 = -m.matrix_12 / m.Determinant;
            matrix_21 = -m.matrix_21 / m.Determinant;
            matrix_22 = m.matrix_11 / m.Determinant;
            return this;
        }

        public IYoonMatrix Transpose()
        {
            YoonMatrix2X2Double m = new YoonMatrix2X2Double(this);
            matrix_12 = m.matrix_21;
            matrix_21 = m.matrix_12;
            return this;
        }

        public IYoonMatrix Unit()
        {
            matrix_11 = matrix_22 = 1;
            matrix_12 = matrix_21 = 0;
            return this;
        }

        public override bool Equals(object obj)
        {
            return Equals(obj as YoonMatrix2X2Double);
        }

        public bool Equals(YoonMatrix2X2Double other)
        {
            return other != null &&
                   Length == other.Length &&
                   matrix_11 == other.matrix_11 &&
                   matrix_12 == other.matrix_12 &&
                   matrix_21 == other.matrix_21 &&
                   matrix_22 == other.matrix_22 &&
                   EqualityComparer<double[,]>.Default.Equals(Array, other.Array) &&
                   Determinant == other.Determinant;
        }

        public override int GetHashCode()
        {
            int hashCode = 915136901;
            hashCode = hashCode * -1521134295 + Length.GetHashCode();
            hashCode = hashCode * -1521134295 + matrix_11.GetHashCode();
            hashCode = hashCode * -1521134295 + matrix_12.GetHashCode();
            hashCode = hashCode * -1521134295 + matrix_21.GetHashCode();
            hashCode = hashCode * -1521134295 + matrix_22.GetHashCode();
            hashCode = hashCode * -1521134295 + EqualityComparer<double[,]>.Default.GetHashCode(Array);
            hashCode = hashCode * -1521134295 + Determinant.GetHashCode();
            return hashCode;
        }

        public static YoonMatrix2X2Double operator *(double dNum, YoonMatrix2X2Double pMatrix)
        {
            YoonMatrix2X2Double pResultMatrix = new YoonMatrix2X2Double();
            for (int i = 0; i < pResultMatrix.Length; i++)
            {
                for (int j = 0; j < pResultMatrix.Length; j++)
                {
                    pResultMatrix.Array[i, j] = dNum * pMatrix.Array[i, j];
                }
            }

            return pResultMatrix;
        }

        public static YoonMatrix2X2Double operator /(YoonMatrix2X2Double pMatrix, double dNum)
        {
            YoonMatrix2X2Double pResultMatrix = new YoonMatrix2X2Double();
            for (int i = 0; i < pResultMatrix.Length; i++)
            {
                for (int j = 0; j < pResultMatrix.Length; j++)
                {
                    pResultMatrix.Array[i, j] = pMatrix.Array[i, j] / dNum;
                }
            }

            return pResultMatrix;
        }

        public static YoonMatrix2X2Double operator *(YoonMatrix2X2Double dNum, YoonMatrix2X2Double pMatrix)
        {
            YoonMatrix2X2Double pResultMatrix = new YoonMatrix2X2Double();
            for (int i = 0; i < pResultMatrix.Length; i++)
            {
                for (int j = 0; j < pResultMatrix.Length; j++)
                {
                    pResultMatrix.Array[i, j] = 0;
                    for (int kValue = 0; kValue < pResultMatrix.Length; kValue++)
                        pResultMatrix.Array[i, j] += (dNum.Array[i, kValue] * pMatrix.Array[kValue, j]);
                }
            }

            return pResultMatrix;
        }

        public static bool operator ==(YoonMatrix2X2Double a, YoonMatrix2X2Double b)
        {
            for (int i = 0; i < a?.Length; i++)
            {
                for (int j = 0; j < a?.Length; j++)
                {
                    if (a?.Array[i, j] != b?.Array[i, j])
                        return false;
                }
            }

            return true;
        }

        public static bool operator !=(YoonMatrix2X2Double a, YoonMatrix2X2Double b)
        {
            for (int i = 0; i < a?.Length; i++)
            {
                for (int j = 0; j < a?.Length; j++)
                {
                    if (a?.Array[i, j] != b?.Array[i, j])
                        return true;
                }
            }

            return false;
        }
    }

    public class YoonMatrix3X3Int : IYoonMatrix, IYoonMatrix<int>, IEquatable<YoonMatrix3X3Int>
    {
        public int Length { get; } = 3;

        public int matrix_11
        {
            get => Array[0, 0];

            set => Array[0, 0] = value;
        }

        public int matrix_12
        {
            get => Array[0, 1];

            set => Array[0, 1] = value;
        }

        public int matrix_13
        {
            get => Array[0, 2];

            set => Array[0, 2] = value;
        }

        public int matrix_21
        {
            get => Array[1, 0];

            set => Array[1, 0] = value;
        }

        public int matrix_22
        {
            get => Array[1, 1];

            set => Array[1, 1] = value;
        }

        public int matrix_23
        {
            get => Array[1, 2];

            set => Array[1, 2] = value;
        }

        public int matrix_31
        {
            get => Array[2, 0];

            set => Array[2, 0] = value;
        }

        public int matrix_32
        {
            get => Array[2, 1];

            set => Array[2, 1] = value;
        }

        public int matrix_33
        {
            get => Array[2, 2];

            set => Array[2, 2] = value;
        }

        public YoonMatrix3X3Int()
        {
            matrix_11 = matrix_22 = matrix_33 = 1;
            matrix_12 = matrix_13 = matrix_21 = matrix_23 = matrix_31 = matrix_32 = 0;
        }

        public YoonMatrix3X3Int(IYoonMatrix pMatrix)
        {
            this.CopyFrom(pMatrix);
        }

        public int[,] Array { get; set; } = new int[3, 3];

        public int Determinant
        {
            /*
                   matrix_11 * matrix_22 * matrix_33 + matrix_21 * matrix_32 * matrix_13 + matrix_31 * matrix_12 * matrix_23
                 - matrix_11 * matrix_32 * matrix_23 - matrix_31 * matrix_22 * matrix_13 - matrix_21 * matrix_12 * matrix_33;
            */
            get
            {
                int nSum = 0;
                for (int i = 0; i < Length; i++)
                    nSum += Array[0, i] * Cofactor(0, i);
                return nSum;
            }
        }

        public IYoonMatrix Clone()
        {
            return new YoonMatrix3X3Int
            {
                matrix_11 = matrix_11,
                matrix_12 = matrix_12,
                matrix_13 = matrix_13,
                matrix_21 = matrix_21,
                matrix_22 = matrix_22,
                matrix_23 = matrix_23,
                matrix_31 = matrix_31,
                matrix_32 = matrix_32,
                matrix_33 = matrix_33
            };
        }

        public void CopyFrom(IYoonMatrix pMatrix)
        {
            if (pMatrix is not YoonMatrix3X3Int pMatrixInt) return;
            matrix_11 = pMatrixInt.matrix_11;
            matrix_12 = pMatrixInt.matrix_12;
            matrix_13 = pMatrixInt.matrix_13;
            matrix_21 = pMatrixInt.matrix_21;
            matrix_22 = pMatrixInt.matrix_22;
            matrix_23 = pMatrixInt.matrix_23;
            matrix_31 = pMatrixInt.matrix_31;
            matrix_32 = pMatrixInt.matrix_32;
            matrix_33 = pMatrixInt.matrix_33;
        }

        public int Cofactor(int nRow, int nCol)
        {
            return (int)(Math.Pow(-1, nRow + nCol) * ((YoonMatrix2X2Int) GetMinorMatrix(nRow, nCol)).Determinant);
        }

        public IYoonMatrix GetMinorMatrix(int nRow, int nCol)
        {
            if (nRow < 0 || nRow >= Length || nCol < 0 || nCol >= Length)
                return new YoonMatrix2X2Int();

            int[,] pArray = new int[2, 2];
            int iCount = 0;
            int jCount = 0;
            for (int i = 0; i < Length; i++)
            {
                if (i == nRow) continue;
                for (int j = 0; j < Length; j++)
                {
                    if (j == nCol) continue;
                    pArray[iCount, jCount] = Array[i, j];
                    jCount++;
                }
                iCount++;
            }

            return new YoonMatrix2X2Int {Array = pArray};
        }

        public IYoonMatrix GetAdjointMatrix()
        {
            int[,] pArray = new int[3, 3];
            for (int i = 0; i < Length; i++)
            {
                for (int j = 0; j < Length; j++)
                {
                    pArray[i, j] = Cofactor(i, j);
                }
            }
            YoonMatrix3X3Int pMatrix = new YoonMatrix3X3Int();
            pMatrix.Array = pArray;
            return pMatrix.Transpose();
        }

        public IYoonMatrix Inverse()
        {
            if (Determinant == 0) return Unit();

            YoonMatrix3X3Int pMatrix = new YoonMatrix3X3Int(this);
            CopyFrom((pMatrix.GetAdjointMatrix() as YoonMatrix3X3Int) / pMatrix.Determinant);
            return this;
        }

        public IYoonMatrix Transpose()
        {
            YoonMatrix3X3Int pMatrix = new YoonMatrix3X3Int(this);
            matrix_12 = pMatrix.matrix_21;
            matrix_13 = pMatrix.matrix_31;
            matrix_21 = pMatrix.matrix_12;
            matrix_23 = pMatrix.matrix_32;
            matrix_31 = pMatrix.matrix_13;
            matrix_32 = pMatrix.matrix_23;
            return this;
        }

        public IYoonMatrix Unit()
        {
            matrix_11 = matrix_22 = matrix_33 = 1;
            matrix_12 = matrix_13 = matrix_21 = matrix_23 = matrix_31 = matrix_32 = 0;
            return this;
        }

        public override bool Equals(object obj)
        {
            return Equals(obj as YoonMatrix3X3Int);
        }

        public bool Equals(YoonMatrix3X3Int other)
        {
            return other != null &&
                   Length == other.Length &&
                   matrix_11 == other.matrix_11 &&
                   matrix_12 == other.matrix_12 &&
                   matrix_13 == other.matrix_13 &&
                   matrix_21 == other.matrix_21 &&
                   matrix_22 == other.matrix_22 &&
                   matrix_23 == other.matrix_23 &&
                   matrix_31 == other.matrix_31 &&
                   matrix_32 == other.matrix_32 &&
                   matrix_33 == other.matrix_33 &&
                   EqualityComparer<int[,]>.Default.Equals(Array, other.Array) &&
                   Determinant == other.Determinant;
        }

        public override int GetHashCode()
        {
            int hashCode = -1889127308;
            hashCode = hashCode * -1521134295 + Length.GetHashCode();
            hashCode = hashCode * -1521134295 + matrix_11.GetHashCode();
            hashCode = hashCode * -1521134295 + matrix_12.GetHashCode();
            hashCode = hashCode * -1521134295 + matrix_13.GetHashCode();
            hashCode = hashCode * -1521134295 + matrix_21.GetHashCode();
            hashCode = hashCode * -1521134295 + matrix_22.GetHashCode();
            hashCode = hashCode * -1521134295 + matrix_23.GetHashCode();
            hashCode = hashCode * -1521134295 + matrix_31.GetHashCode();
            hashCode = hashCode * -1521134295 + matrix_32.GetHashCode();
            hashCode = hashCode * -1521134295 + matrix_33.GetHashCode();
            hashCode = hashCode * -1521134295 + EqualityComparer<int[,]>.Default.GetHashCode(Array);
            hashCode = hashCode * -1521134295 + Determinant.GetHashCode();
            return hashCode;
        }

        public static YoonMatrix3X3Int operator *(int nNum, YoonMatrix3X3Int pMatrix)
        {
            YoonMatrix3X3Int pResultMatrix = new YoonMatrix3X3Int();
            for (int i = 0; i < pResultMatrix.Length; i++)
            {
                for (int j = 0; j < pResultMatrix.Length; j++)
                {
                    pResultMatrix.Array[i, j] = nNum * pMatrix.Array[i, j];
                }
            }
            return pResultMatrix;
        }

        public static YoonMatrix3X3Int operator /(YoonMatrix3X3Int pMatrix, int nNum)
        {
            YoonMatrix3X3Int pResultMatrix = new YoonMatrix3X3Int();
            for (int i = 0; i < pResultMatrix.Length; i++)
            {
                for (int j = 0; j < pResultMatrix.Length; j++)
                {
                    pResultMatrix.Array[i, j] = pMatrix.Array[i, j] / nNum;
                }
            }
            return pResultMatrix;
        }

        public static YoonMatrix3X3Int operator *(YoonMatrix3X3Int nNum, YoonMatrix3X3Int pMatrix)
        {
            YoonMatrix3X3Int pResultMatrix = new YoonMatrix3X3Int();
            for (int i = 0; i < pResultMatrix.Length; i++)
            {
                for (int j = 0; j < pResultMatrix.Length; j++)
                {
                    pResultMatrix.Array[i, j] = 0;
                    for (int kValue = 0; kValue < pResultMatrix.Length; kValue++)
                        pResultMatrix.Array[i, j] += (nNum.Array[i, kValue] * pMatrix.Array[kValue, j]);
                }
            }
            return pResultMatrix;
        }

        public static bool operator ==(YoonMatrix3X3Int pMatrixSource, YoonMatrix3X3Int pMatrixObject)
        {
            for (int i = 0; i < pMatrixSource?.Length; i++)
            {
                for (int j = 0; j < pMatrixSource?.Length; j++)
                {
                    if (pMatrixSource?.Array[i, j] != pMatrixObject?.Array[i, j])
                        return false;
                }
            }
            return true;
        }

        public static bool operator !=(YoonMatrix3X3Int pMatrixSource, YoonMatrix3X3Int pMatrixObject)
        {
            for (int i = 0; i < pMatrixSource?.Length; i++)
            {
                for (int j = 0; j < pMatrixSource?.Length; j++)
                {
                    if (pMatrixSource?.Array[i, j] != pMatrixObject?.Array[i, j])
                        return true;
                }
            }
            return false;
        }
    }

    public class YoonMatrix3X3Double : IYoonMatrix, IYoonMatrix<double>, IEquatable<YoonMatrix3X3Double>
    {
        public int Length { get; } = 3;

        public double matrix_11
        {
            get => Array[0, 0];

            set => Array[0, 0] = value;
        }

        public double matrix_12
        {
            get => Array[0, 1];

            set => Array[0, 1] = value;
        }

        public double matrix_13
        {
            get => Array[0, 2];

            set => Array[0, 2] = value;
        }

        public double matrix_21
        {
            get => Array[1, 0];

            set => Array[1, 0] = value;
        }

        public double matrix_22
        {
            get => Array[1, 1];

            set => Array[1, 1] = value;
        }

        public double matrix_23
        {
            get => Array[1, 2];

            set => Array[1, 2] = value;
        }

        public double matrix_31
        {
            get => Array[2, 0];

            set => Array[2, 0] = value;
        }

        public double matrix_32
        {
            get => Array[2, 1];

            set => Array[2, 1] = value;
        }

        public double matrix_33
        {
            get => Array[2, 2];

            set => Array[2, 2] = value;
        }

        public YoonMatrix3X3Double()
        {
            matrix_11 = matrix_22 = matrix_33 = 1;
            matrix_12 = matrix_13 = matrix_21 = matrix_23 = matrix_31 = matrix_32 = 0;
        }

        public YoonMatrix3X3Double(IYoonMatrix pMatrix)
        {
            this.CopyFrom(pMatrix);
        }

        public double[,] Array { get; set; } = new double[3, 3];

        public double Determinant
        {
            /*
                   matrix_11 * matrix_22 * matrix_33 + matrix_21 * matrix_32 * matrix_13 + matrix_31 * matrix_12 * matrix_23
                 - matrix_11 * matrix_32 * matrix_23 - matrix_31 * matrix_22 * matrix_13 - matrix_21 * matrix_12 * matrix_33;
            */
            get
            {
                double dSum = 0;
                for (int i = 0; i < Length; i++)
                    dSum += Array[0, i] * Cofactor(0, i);
                return dSum;
            }
        }

        public IYoonMatrix Clone()
        {
            return new YoonMatrix3X3Double
            {
                matrix_11 = matrix_11,
                matrix_12 = matrix_12,
                matrix_13 = matrix_13,
                matrix_21 = matrix_21,
                matrix_22 = matrix_22,
                matrix_23 = matrix_23,
                matrix_31 = matrix_31,
                matrix_32 = matrix_32,
                matrix_33 = matrix_33
            };
        }

        public void CopyFrom(IYoonMatrix pMatrix)
        {
            if (pMatrix is not YoonMatrix3X3Double pMatrixDouble) return;
            matrix_11 = pMatrixDouble.matrix_11;
            matrix_12 = pMatrixDouble.matrix_12;
            matrix_13 = pMatrixDouble.matrix_13;
            matrix_21 = pMatrixDouble.matrix_21;
            matrix_22 = pMatrixDouble.matrix_22;
            matrix_23 = pMatrixDouble.matrix_23;
            matrix_31 = pMatrixDouble.matrix_31;
            matrix_32 = pMatrixDouble.matrix_32;
            matrix_33 = pMatrixDouble.matrix_33;
        }

        public double Cofactor(int nRow, int nCol)
        {
            return Math.Pow(-1, nRow + nCol) * (GetMinorMatrix(nRow, nCol) as YoonMatrix2X2Double).Determinant;
        }

        public IYoonMatrix GetMinorMatrix(int nRow, int nCol)
        {
            if (nRow < 0 || nRow >= Length || nCol < 0 || nCol >= Length)
                return new YoonMatrix2X2Double();

            double[,] pArray = new double[2, 2];
            int iCount = 0;
            int jCount = 0;
            for (int i = 0; i < Length; i++)
            {
                if (i == nRow) continue;
                for (int j = 0; j < Length; j++)
                {
                    if (j == nCol) continue;
                    pArray[iCount, jCount] = Array[i, j];
                    jCount++;
                }

                iCount++;
            }

            return new YoonMatrix2X2Double {Array = pArray};
        }

        public IYoonMatrix GetAdjointMatrix()
        {
            double[,] pArray = new double[3, 3];
            for (int i = 0; i < Length; i++)
            {
                for (int j = 0; j < Length; j++)
                {
                    pArray[i, j] = Cofactor(i, j);
                }
            }

            YoonMatrix3X3Double pMatrix = new YoonMatrix3X3Double {Array = pArray};
            return pMatrix.Transpose();
        }

        public IYoonMatrix Inverse()
        {
            if (Determinant == 0) return Unit();

            YoonMatrix3X3Double pMatrix = new YoonMatrix3X3Double(this);
            CopyFrom((pMatrix.GetAdjointMatrix() as YoonMatrix3X3Double) / pMatrix.Determinant);
            return this;
        }

        public IYoonMatrix Transpose()
        {
            YoonMatrix3X3Double pMatrix = new YoonMatrix3X3Double(this);
            matrix_12 = pMatrix.matrix_21;
            matrix_13 = pMatrix.matrix_31;
            matrix_21 = pMatrix.matrix_12;
            matrix_23 = pMatrix.matrix_32;
            matrix_31 = pMatrix.matrix_13;
            matrix_32 = pMatrix.matrix_23;
            return this;
        }

        public IYoonMatrix Unit()
        {
            matrix_11 = matrix_22 = matrix_33 = 1;
            matrix_12 = matrix_13 = matrix_21 = matrix_23 = matrix_31 = matrix_32 = 0;
            return this;
        }

        public override bool Equals(object obj)
        {
            return Equals(obj as YoonMatrix3X3Double);
        }

        public bool Equals(YoonMatrix3X3Double other)
        {
            return other != null &&
                   Length == other.Length &&
                   matrix_11 == other.matrix_11 &&
                   matrix_12 == other.matrix_12 &&
                   matrix_13 == other.matrix_13 &&
                   matrix_21 == other.matrix_21 &&
                   matrix_22 == other.matrix_22 &&
                   matrix_23 == other.matrix_23 &&
                   matrix_31 == other.matrix_31 &&
                   matrix_32 == other.matrix_32 &&
                   matrix_33 == other.matrix_33 &&
                   EqualityComparer<double[,]>.Default.Equals(Array, other.Array) &&
                   Determinant == other.Determinant;
        }

        public override int GetHashCode()
        {
            int hashCode = -1889127308;
            hashCode = hashCode * -1521134295 + Length.GetHashCode();
            hashCode = hashCode * -1521134295 + matrix_11.GetHashCode();
            hashCode = hashCode * -1521134295 + matrix_12.GetHashCode();
            hashCode = hashCode * -1521134295 + matrix_13.GetHashCode();
            hashCode = hashCode * -1521134295 + matrix_21.GetHashCode();
            hashCode = hashCode * -1521134295 + matrix_22.GetHashCode();
            hashCode = hashCode * -1521134295 + matrix_23.GetHashCode();
            hashCode = hashCode * -1521134295 + matrix_31.GetHashCode();
            hashCode = hashCode * -1521134295 + matrix_32.GetHashCode();
            hashCode = hashCode * -1521134295 + matrix_33.GetHashCode();
            hashCode = hashCode * -1521134295 + EqualityComparer<double[,]>.Default.GetHashCode(Array);
            hashCode = hashCode * -1521134295 + Determinant.GetHashCode();
            return hashCode;
        }

        public static YoonMatrix3X3Double operator *(double dNum, YoonMatrix3X3Double pMatrix)
        {
            YoonMatrix3X3Double pReesultMatrix = new YoonMatrix3X3Double();
            for (int i = 0; i < pReesultMatrix.Length; i++)
            {
                for (int j = 0; j < pReesultMatrix.Length; j++)
                {
                    pReesultMatrix.Array[i, j] = dNum * pMatrix.Array[i, j];
                }
            }

            return pReesultMatrix;
        }

        public static YoonMatrix3X3Double operator /(YoonMatrix3X3Double pMatrix, double dNum)
        {
            YoonMatrix3X3Double pResultMatrix = new YoonMatrix3X3Double();
            for (int i = 0; i < pResultMatrix.Length; i++)
            {
                for (int j = 0; j < pResultMatrix.Length; j++)
                {
                    pResultMatrix.Array[i, j] = pMatrix.Array[i, j] / dNum;
                }
            }

            return pResultMatrix;
        }

        public static YoonMatrix3X3Double operator *(YoonMatrix3X3Double dNum, YoonMatrix3X3Double pMatrix)
        {
            YoonMatrix3X3Double pResultMatrix = new YoonMatrix3X3Double();
            for (int i = 0; i < pResultMatrix.Length; i++)
            {
                for (int j = 0; j < pResultMatrix.Length; j++)
                {
                    pResultMatrix.Array[i, j] = 0;
                    for (int kValue = 0; kValue < pResultMatrix.Length; kValue++)
                        pResultMatrix.Array[i, j] += (dNum.Array[i, kValue] * pMatrix.Array[kValue, j]);
                }
            }

            return pResultMatrix;
        }

        public static bool operator ==(YoonMatrix3X3Double pMatrixSource, YoonMatrix3X3Double pMatrixObject)
        {
            for (int i = 0; i < pMatrixSource?.Length; i++)
            {
                for (int j = 0; j < pMatrixSource?.Length; j++)
                {
                    if (pMatrixSource?.Array[i, j] != pMatrixObject?.Array[i, j])
                        return false;
                }
            }

            return true;
        }

        public static bool operator !=(YoonMatrix3X3Double pMatrixSource, YoonMatrix3X3Double pMatrixObject)
        {
            for (int i = 0; i < pMatrixSource?.Length; i++)
            {
                for (int j = 0; j < pMatrixSource?.Length; j++)
                {
                    if (pMatrixSource?.Array[i, j] != pMatrixObject?.Array[i, j])
                        return true;
                }
            }

            return false;
        }
    }

    /// <summary>
    /// Equal Transformation Matrix (for integer)
    /// </summary>
    public class YoonMatrix2N : YoonMatrix3X3Int, IYoonMatrix2D<int>
    {
        public IYoonMatrix SetScaleUnit(int scaleX, int scaleY)
        {
            Unit();
            matrix_11 *= scaleX;
            matrix_22 *= scaleY;
            return this;
        }

        public IYoonMatrix SetMovementUnit(int moveX, int moveY)
        {
            Unit();
            matrix_13 += moveX;
            matrix_23 += moveY;
            return this;
        }

        public IYoonMatrix SetMovementUnit(IYoonVector2D<int> pVector)
        {
            Unit();
            matrix_13 += pVector.X;
            matrix_23 += pVector.Y;
            return this;
        }

        public IYoonMatrix SetRotateUnit(double dAngle)
        {
            Unit();
            int cosT = (int)Math.Cos(dAngle);
            int sinT = (int)Math.Sin(dAngle);
            matrix_11 = cosT;
            matrix_12 = -sinT;
            matrix_21 = sinT;
            matrix_22 = cosT;
            return this;
        }
    }

    /// <summary>
    /// Equal Transformation Matrix (for double)
    /// </summary>
    public class YoonMatrix2D : YoonMatrix3X3Double, IYoonMatrix2D<double>
    {

        public IYoonMatrix SetScaleUnit(double scaleX, double scaleY)
        {
            Unit();
            matrix_11 *= scaleX;
            matrix_22 *= scaleY;
            return this;
        }

        public IYoonMatrix SetMovementUnit(double moveX, double moveY)
        {
            Unit();
            matrix_13 += moveX;
            matrix_23 += moveY;
            return this;
        }

        public IYoonMatrix SetMovementUnit(IYoonVector2D<double> pVector)
        {
            Unit();
            matrix_13 += pVector.X;
            matrix_23 += pVector.Y;
            return this;
        }

        public IYoonMatrix SetRotateUnit(double dAngle)
        {
            Unit();
            double cosT = Math.Cos(dAngle);
            double sinT = Math.Sin(dAngle);
            matrix_11 = cosT;
            matrix_12 = -sinT;
            matrix_21 = sinT;
            matrix_22 = cosT;
            return this;
        }

    }

    public class YoonMatrix4X4Double : IYoonMatrix<double>, IEquatable<YoonMatrix4X4Double>
    {
        public int Length { get; set; } = 4;

        public double matrix_11
        {
            get => Array[0, 0];

            set => Array[0, 0] = value;
        }

        public double matrix_12
        {
            get => Array[0, 1];

            set => Array[0, 1] = value;
        }

        public double matrix_13
        {
            get => Array[0, 2];

            set => Array[0, 2] = value;
        }

        public double matrix_14
        {
            get => Array[0, 3];

            set => Array[0, 3] = value;
        }

        public double matrix_21
        {
            get => Array[1, 0];

            set => Array[1, 0] = value;
        }

        public double matrix_22
        {
            get => Array[1, 1];

            set => Array[1, 1] = value;
        }

        public double matrix_23
        {
            get => Array[1, 2];

            set => Array[1, 2] = value;
        }

        public double matrix_24
        {
            get => Array[1, 3];

            set => Array[1, 3] = value;
        }

        public double matrix_31
        {
            get => Array[2, 0];

            set => Array[2, 0] = value;
        }

        public double matrix_32
        {
            get => Array[2, 1];

            set => Array[2, 1] = value;
        }

        public double matrix_33
        {
            get => Array[2, 2];

            set => Array[2, 2] = value;
        }

        public double matrix_34
        {
            get => Array[2, 3];

            set => Array[2, 3] = value;
        }


        public double matrix_41
        {
            get => Array[3, 0];

            set => Array[3, 0] = value;
        }

        public double matrix_42
        {
            get => Array[3, 1];

            set => Array[3, 1] = value;
        }

        public double matrix_43
        {
            get => Array[3, 2];

            set { Array[3, 2] = value; }
        }

        public double matrix_44
        {
            get => Array[3, 3];

            set => Array[3, 3] = value;
        }

        public YoonMatrix4X4Double()
        {
            matrix_11 = matrix_22 = matrix_33 = matrix_44 = 1;
            matrix_12 = matrix_13 = matrix_14 = matrix_21 = matrix_23 =
                matrix_24 = matrix_31 = matrix_32 = matrix_34 = matrix_41 = matrix_42 = matrix_43 = 0;
        }

        public YoonMatrix4X4Double(IYoonMatrix pMatrix)
        {
            this.CopyFrom(pMatrix);
        }

        public double[,] Array { get; set; } = new double[4, 4];

        public double Determinant
        {
            /*
                   matrix_11 * matrix_22 * matrix_33 * matrix_44 + matrix_11 * matrix_23 * matrix_34 * matrix_42 + matrix_11 * matrix_24 * matrix_32 * matrix_43
                 + matrix_12 * matrix_21 * matrix_34 * matrix_43 + matrix_12 * matrix_23 * matrix_31 * matrix_44 + matrix_12 * matrix_24 * matrix_33 * matrix_41
                 + matrix_13 * matrix_21 * matrix_32 * matrix_44 + matrix_13 * matrix_22 * matrix_34 * matrix_41 + matrix_13 * matrix_24 * matrix_31 * matrix_42
                 + matrix_14 * matrix_21 * matrix_33 * matrix_42 + matrix_14 * matrix_22 * matrix_31 * matrix_43 + matrix_14 * matrix_23 * matrix_32 * matrix_41
                 - matrix_11 * matrix_22 * matrix_34 * matrix_43 - matrix_11 * matrix_23 * matrix_32 * matrix_44 - matrix_11 * matrix_24 * matrix_33 * matrix_42
                 - matrix_12 * matrix_21 * matrix_33 * matrix_44 - matrix_12 * matrix_23 * matrix_34 * matrix_41 - matrix_12 * matrix_24 * matrix_31 * matrix_43
                 - matrix_13 * matrix_21 * matrix_34 * matrix_42 - matrix_13 * matrix_22 * matrix_31 * matrix_44 - matrix_13 * matrix_24 * matrix_32 * matrix_41
                 - matrix_14 * matrix_21 * matrix_32 * matrix_43 - matrix_14 * matrix_22 * matrix_33 * matrix_41 - matrix_14 * matrix_23 * matrix_31 * matrix_42;
            */
            get
            {
                double dSum = 0;
                for (int i = 0; i < Length; i++)
                    dSum += Array[0, i] * Cofactor(0, i);
                return dSum;
            }
        }

        public IYoonMatrix Clone()
        {
            return new YoonMatrix4X4Double
            {
                matrix_11 = matrix_11,
                matrix_12 = matrix_12,
                matrix_13 = matrix_13,
                matrix_14 = matrix_14,
                matrix_21 = matrix_21,
                matrix_22 = matrix_22,
                matrix_23 = matrix_23,
                matrix_24 = matrix_24,
                matrix_31 = matrix_31,
                matrix_32 = matrix_32,
                matrix_33 = matrix_33,
                matrix_34 = matrix_34,
                matrix_41 = matrix_41,
                matrix_42 = matrix_42,
                matrix_43 = matrix_43,
                matrix_44 = matrix_44
            };
        }

        public void CopyFrom(IYoonMatrix pMatrix)
        {
            if (pMatrix is not YoonMatrix4X4Double pMatrixDouble) return;
            matrix_11 = pMatrixDouble.matrix_11;
            matrix_12 = pMatrixDouble.matrix_12;
            matrix_13 = pMatrixDouble.matrix_13;
            matrix_14 = pMatrixDouble.matrix_14;
            matrix_21 = pMatrixDouble.matrix_21;
            matrix_22 = pMatrixDouble.matrix_22;
            matrix_23 = pMatrixDouble.matrix_23;
            matrix_24 = pMatrixDouble.matrix_24;
            matrix_31 = pMatrixDouble.matrix_31;
            matrix_32 = pMatrixDouble.matrix_32;
            matrix_33 = pMatrixDouble.matrix_33;
            matrix_34 = pMatrixDouble.matrix_34;
            matrix_41 = pMatrixDouble.matrix_41;
            matrix_42 = pMatrixDouble.matrix_42;
            matrix_43 = pMatrixDouble.matrix_43;
            matrix_44 = pMatrixDouble.matrix_44;
        }

        public double Cofactor(int nRow, int nCol)
        {
            return Math.Pow(-1, nRow + nCol) * ((YoonMatrix3X3Double) GetMinorMatrix(nRow, nCol)).Determinant;
        }

        public IYoonMatrix GetMinorMatrix(int nRow, int nCol)
        {
            if (nRow < 0 || nRow >= Length || nCol < 0 || nCol >= Length)
                return new YoonMatrix3X3Double();

            double[,] pArray = new double[3, 3];
            int iCount = 0;
            int jCount = 0;
            for (int i = 0; i < Length; i++)
            {
                if (i == nRow) continue;
                for (int j = 0; j < Length; j++)
                {
                    if (j == nCol) continue;
                    pArray[iCount, jCount] = Array[i, j];
                    jCount++;
                }

                iCount++;
            }

            return new YoonMatrix3X3Double {Array = pArray};
        }

        public IYoonMatrix GetAdjointMatrix()
        {
            double[,] pArray = new double[3, 3];
            for (int i = 0; i < Length; i++)
            {
                for (int j = 0; j < Length; j++)
                {
                    pArray[i, j] = Cofactor(i, j);
                }
            }

            YoonMatrix4X4Double pMatrix = new YoonMatrix4X4Double {Array = pArray};
            return pMatrix.Transpose();
        }

        public IYoonMatrix Inverse()
        {
            if (Determinant == 0) return Unit();

            YoonMatrix4X4Double pMatrix = new YoonMatrix4X4Double(this);
            CopyFrom((pMatrix.GetAdjointMatrix() as YoonMatrix4X4Double) / pMatrix.Determinant);
            return this;
        }

        public IYoonMatrix Transpose()
        {
            YoonMatrix4X4Double m = new YoonMatrix4X4Double(this);
            matrix_12 = m.matrix_21;
            matrix_13 = m.matrix_31;
            matrix_14 = m.matrix_41;
            matrix_21 = m.matrix_12;
            matrix_23 = m.matrix_32;
            matrix_24 = m.matrix_42;
            matrix_31 = m.matrix_13;
            matrix_32 = m.matrix_23;
            matrix_34 = m.matrix_43;
            matrix_41 = m.matrix_14;
            matrix_42 = m.matrix_24;
            matrix_43 = m.matrix_34;
            return this;
        }

        public IYoonMatrix Unit()
        {
            matrix_11 = matrix_22 = matrix_33 = matrix_44 = 1;
            matrix_12 = matrix_13 = matrix_14 = matrix_21 = matrix_23 =
                matrix_24 = matrix_31 = matrix_32 = matrix_34 = matrix_41 = matrix_42 = matrix_43 = 0;
            return this;
        }

        public override bool Equals(object obj)
        {
            return Equals(obj as YoonMatrix4X4Double);
        }

        public bool Equals(YoonMatrix4X4Double other)
        {
            return other != null &&
                   Length == other.Length &&
                   matrix_11 == other.matrix_11 &&
                   matrix_12 == other.matrix_12 &&
                   matrix_13 == other.matrix_13 &&
                   matrix_14 == other.matrix_14 &&
                   matrix_21 == other.matrix_21 &&
                   matrix_22 == other.matrix_22 &&
                   matrix_23 == other.matrix_23 &&
                   matrix_24 == other.matrix_24 &&
                   matrix_31 == other.matrix_31 &&
                   matrix_32 == other.matrix_32 &&
                   matrix_33 == other.matrix_33 &&
                   matrix_34 == other.matrix_34 &&
                   matrix_41 == other.matrix_41 &&
                   matrix_42 == other.matrix_42 &&
                   matrix_43 == other.matrix_43 &&
                   matrix_44 == other.matrix_44 &&
                   EqualityComparer<double[,]>.Default.Equals(Array, other.Array) &&
                   Determinant == other.Determinant;
        }

        public override int GetHashCode()
        {
            int hashCode = -1235618589;
            hashCode = hashCode * -1521134295 + Length.GetHashCode();
            hashCode = hashCode * -1521134295 + matrix_11.GetHashCode();
            hashCode = hashCode * -1521134295 + matrix_12.GetHashCode();
            hashCode = hashCode * -1521134295 + matrix_13.GetHashCode();
            hashCode = hashCode * -1521134295 + matrix_14.GetHashCode();
            hashCode = hashCode * -1521134295 + matrix_21.GetHashCode();
            hashCode = hashCode * -1521134295 + matrix_22.GetHashCode();
            hashCode = hashCode * -1521134295 + matrix_23.GetHashCode();
            hashCode = hashCode * -1521134295 + matrix_24.GetHashCode();
            hashCode = hashCode * -1521134295 + matrix_31.GetHashCode();
            hashCode = hashCode * -1521134295 + matrix_32.GetHashCode();
            hashCode = hashCode * -1521134295 + matrix_33.GetHashCode();
            hashCode = hashCode * -1521134295 + matrix_34.GetHashCode();
            hashCode = hashCode * -1521134295 + matrix_41.GetHashCode();
            hashCode = hashCode * -1521134295 + matrix_42.GetHashCode();
            hashCode = hashCode * -1521134295 + matrix_43.GetHashCode();
            hashCode = hashCode * -1521134295 + matrix_44.GetHashCode();
            hashCode = hashCode * -1521134295 + EqualityComparer<double[,]>.Default.GetHashCode(Array);
            hashCode = hashCode * -1521134295 + Determinant.GetHashCode();
            return hashCode;
        }

        public static YoonMatrix4X4Double operator *(double dNum, YoonMatrix4X4Double pMatrix)
        {
            YoonMatrix4X4Double pResultMatrix = new YoonMatrix4X4Double();
            for (int i = 0; i < pResultMatrix.Length; i++)
            {
                for (int j = 0; j < pResultMatrix.Length; j++)
                {
                    pResultMatrix.Array[i, j] = dNum * pMatrix.Array[i, j];
                }
            }

            return pResultMatrix;
        }

        public static YoonMatrix4X4Double operator /(YoonMatrix4X4Double pMatrix, double dNum)
        {
            YoonMatrix4X4Double pResultMatrix = new YoonMatrix4X4Double();
            for (int i = 0; i < pResultMatrix.Length; i++)
            {
                for (int j = 0; j < pResultMatrix.Length; j++)
                {
                    pResultMatrix.Array[i, j] = pMatrix.Array[i, j] / dNum;
                }
            }

            return pResultMatrix;
        }

        public static YoonMatrix4X4Double operator *(YoonMatrix4X4Double pMatrixSource,
            YoonMatrix4X4Double pMatrixObject)
        {
            YoonMatrix4X4Double pResultMatrix = new YoonMatrix4X4Double();
            for (int i = 0; i < pResultMatrix.Length; i++)
            {
                for (int j = 0; j < pResultMatrix.Length; j++)
                {
                    pResultMatrix.Array[i, j] = 0;
                    for (int kValue = 0; kValue < pResultMatrix.Length; kValue++)
                        pResultMatrix.Array[i, j] += (pMatrixSource.Array[i, kValue] * pMatrixObject.Array[kValue, j]);
                }
            }

            return pResultMatrix;
        }

        public static bool operator ==(YoonMatrix4X4Double pMatrixSource, YoonMatrix4X4Double pMatrixObject)
        {
            for (int i = 0; i < pMatrixSource?.Length; i++)
            {
                for (int j = 0; j < pMatrixSource?.Length; j++)
                {
                    if (pMatrixSource?.Array[i, j] != pMatrixObject?.Array[i, j])
                        return false;
                }
            }

            return true;
        }

        public static bool operator !=(YoonMatrix4X4Double pMatrixSource, YoonMatrix4X4Double pMatrixObject)
        {
            for (int i = 0; i < pMatrixSource?.Length; i++)
            {
                for (int j = 0; j < pMatrixSource?.Length; j++)
                {
                    if (pMatrixSource?.Array[i, j] != pMatrixObject?.Array[i, j])
                        return true;
                }
            }

            return false;
        }
    }

    /// <summary>
    /// Three-Dimensional Coversive Matrix (to double)
    /// </summary>
    public class YoonMatrix3D : YoonMatrix4X4Double, IYoonMatrix3D<double>
    {
        public IYoonMatrix SetScaleUnit(double scaleX, double scaleY, double scaleZ)
        {
            Unit();
            matrix_11 *= scaleX;
            matrix_22 *= scaleY;
            matrix_33 *= scaleZ;
            return this;
        }

        public IYoonMatrix SetMovementUnit(double moveX, double moveY, double moveZ)
        {
            Unit();
            matrix_14 += moveX;
            matrix_24 += moveY;
            matrix_34 += moveZ;
            return this;
        }

        public IYoonMatrix SetMovementUnit(IYoonVector3D<double> pVector)
        {
            Unit();
            matrix_14 += pVector.X;
            matrix_24 += pVector.Y;
            matrix_34 += pVector.Z;
            return this;
        }

        public IYoonMatrix SetRotateXUnit(double dAngle)
        {
            Unit();
            double cosT = Math.Cos(dAngle);
            double sinT = Math.Sin(dAngle);
            matrix_22 = cosT;
            matrix_23 = -sinT;
            matrix_32 = sinT;
            matrix_33 = cosT;
            return this;
        }

        public IYoonMatrix SetRotateYUnit(double dAngle)
        {
            Unit();
            double cosT = Math.Cos(dAngle);
            double sinT = Math.Sin(dAngle);
            matrix_11 = cosT;
            matrix_13 = sinT;
            matrix_31 = -sinT;
            matrix_33 = cosT;
            return this;
        }

        public IYoonMatrix SetRotateZUnit(double dAngle)
        {
            Unit();
            double cosT = Math.Cos(dAngle);
            double sinT = Math.Sin(dAngle);
            matrix_11 = cosT;
            matrix_12 = -sinT;
            matrix_21 = sinT;
            matrix_22 = cosT;
            return this;
        }

    }

}
