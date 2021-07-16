using System;
using System.Collections.Generic;

namespace YoonFactory
{
    public interface IYoonMatrix
    {
        int Length { get; }

        IYoonMatrix Clone();
        IYoonMatrix Inverse();
        IYoonMatrix Transpose();
        IYoonMatrix Unit();
        void CopyFrom(IYoonMatrix pMatrix);
    }

    public interface IYoonMatrix<T> : IYoonMatrix where T : IComparable, IComparable<T>
    {
        T[,] Array { get; set; }
        T Determinant { get; }  // 행렬식
        T Cofactor(int nRow, int nCol); // 여인자
        IYoonMatrix GetMinorMatrix(int nRow, int nCol); // 소행렬
        IYoonMatrix GetAdjointMatrix();    // 수반행렬
    }

    public interface IYoonMatrix2D<T> : IYoonMatrix<T> where T : IComparable, IComparable<T>
    {
        IYoonMatrix SetScaleUnit(T nScaleX, T nScaleY);
        IYoonMatrix SetMovementUnit(T nMoveX, T nMoveY);
        IYoonMatrix SetMovementUnit(IYoonVector2D<T> pVector);
        IYoonMatrix SetRotateUnit(double dAngle);
    }

    public interface IYoonMatrix3D<T> : IYoonMatrix<T> where T : IComparable, IComparable<T>
    {
        IYoonMatrix SetScaleUnit(T dScaleX, T dScaleY, T dScaleZ);
        IYoonMatrix SetMovementUnit(T dMoveX, T dMoveY, T dMoveZ);
        IYoonMatrix SetMovementUnit(IYoonVector3D<T> pVector);
        IYoonMatrix SetRotateXUnit(double dAngle);
        IYoonMatrix SetRotateYUnit(double dAngle);
        IYoonMatrix SetRotateZUnit(double dAngle);
    }

    public interface IYoonFigure
    {
        IYoonFigure Clone();
    }

    public interface IYoonVector : IYoonFigure
    {
        int Count { get; }

        bool Equals(IYoonVector pVector);
        new IYoonVector Clone();
        void CopyFrom(IYoonVector pVector);
        void Zero();
        IYoonVector Unit();
        double Length();
        double Distance(IYoonVector pVector);
    }

    public interface IYoonVector2D<T> : IYoonVector where T : IComparable, IComparable<T>
    {
        T W { get; set; }
        T X { get; set; }
        T Y { get; set; }
        T[] Array { get; set; }
        double Angle2D(IYoonVector pVector);
        eYoonDir2D Direction { get; set; }
        IYoonCartesian<T> Cartesian { get; set; }
        eYoonDir2D DirectionTo(IYoonVector pVector);
        IYoonVector GetScaleVector(T nScaleX, T nScaleY);
        IYoonVector GetNextVector(T nMoveX, T nMoveY);
        IYoonVector GetNextVector(IYoonVector pVector);
        IYoonVector GetNextVector(eYoonDir2D nDir);
        IYoonVector GetRotateVector(double dAngle);
        IYoonVector GetRotateVector(IYoonVector pVectorCenter, double dAngle);
        void Scale(T nScaleX, T nScaleY);
        void Move(T nMoveX, T nMoveY);
        void Move(IYoonVector pVector);
        void Move(eYoonDir2D nDir);
        void Rotate(double dAngle);
        void Rotate(IYoonVector pVectorCenter, double dAngle);
    }

    public interface IYoonVector3D<T> : IYoonVector where T : IComparable, IComparable<T>
    {
        T W { get; set; }
        T X { get; set; }
        T Y { get; set; }
        T Z { get; set; }
        T[] Array { get; set; }
        double AngleX(IYoonVector pVector);
        double AngleY(IYoonVector pVector);
        double AngleZ(IYoonVector pVector);
        IYoonCartesian<T> Cartesian { get; set; }
        IYoonVector GetScaleVector(T dScaleX, T dScaleY, T dScaleZ);
        IYoonVector GetNextVector(T dMoveX, T dMoveY, T dMoveZ);
        IYoonVector GetNextVector(IYoonVector pVector);
        void Scale(T dScaleX, T dScaleY, T dScaleZ);
        void Move(T dX, T dY, T dZ);
        void Move(IYoonVector pVector);
        void RotateX(double dAngle);
        void RotateY(double dAngle);
        void RotateZ(double dAngle);
    }

    public interface IYoonJoint
    {
        IYoonJoint Clone();
        void CopyFrom(IYoonJoint pJoint);
    }

    public interface IYoonJoint<T> : IYoonJoint where T : IComparable, IComparable<T>
    {
        T J1 { get; set; }
        T J2 { get; set; }
        T J3 { get; set; }
        T J4 { get; set; }
        T J5 { get; set; }
        T J6 { get; set; }
        T[] ToArray { get; }

        void Zero();
    }

    public interface IYoonCartesian
    {
        IYoonCartesian Clone();
        void CopyFrom(IYoonCartesian pCart);
    }

    public interface IYoonCartesian<T> : IYoonCartesian where T : IComparable, IComparable<T>
    {
        T X { get; set; }
        T Y { get; set; }
        T Z { get; set; }
        T RX { get; set; }
        T RY { get; set; }
        T RZ { get; set; }
        T[] Array { get; set; }

        void Zero();
        void Unit();
    }

    public interface IYoonLine : IYoonFigure
    {
        double Length { get; }

        new IYoonLine Clone();
        void CopyFrom(IYoonLine pLine);
        bool Equals(IYoonLine pLine);
        bool IsContain(IYoonVector pVector);
        double Distance(IYoonVector pVector);
    }

    public interface IYoonLine2D<T> : IYoonLine where T : IComparable, IComparable<T>
    {
        IYoonVector2D<T> StartPos { get; }
        IYoonVector2D<T> EndPos { get; }
        IYoonVector2D<T> CenterPos { get; }
        IYoonRect2D<T> Area { get; }

        T X(T valueY);
        T Y(T valueX);
    }

    public interface IYoonRect : IYoonFigure
    {
        new IYoonRect Clone();
        void CopyFrom(IYoonRect pRect);
        bool Equals(IYoonRect pRect);
        bool IsContain(IYoonVector pVector);

        double Area();
    }

    public interface IYoonRect2D<T> : IYoonRect where T : IComparable, IComparable<T>
    {
        IYoonVector2D<T> CenterPos { get; set; }

        T Width { get; set; }
        T Height { get; set; }
        T Left { get; }
        T Top { get; }
        T Right { get; }
        T Bottom { get; }
        IYoonVector2D<T> TopLeft { get; }
        IYoonVector2D<T> TopRight { get; }
        IYoonVector2D<T> BottomLeft { get; }
        IYoonVector2D<T> BottomRight { get; }

        IYoonVector2D<T> GetPosition(eYoonDir2D nDir);
    }

    public interface IYoonTriangle : IYoonFigure
    {
        new IYoonTriangle Clone();
        void CopyFrom(IYoonTriangle pTriangle);
    }

    public interface IYoonTriangle2D<T> : IYoonTriangle where T : IComparable, IComparable<T>
    {
        T X { get; set; }
        T Y { get; set; }
        T Height { get; set; }
        T Area();
    }

    public interface IYoonMapping : IDisposable
    {
        void CopyFrom(IYoonMapping pMapping);
        IYoonMapping Clone();

        int Width { get; }
        int Height { get; }
        IYoonVector Offset { get; }
        List<IYoonVector> RealPoints { get; set; }
        List<IYoonVector> PixelPoints { get; set; }

        void SetReferencePosition(IYoonVector vecPixelPos, IYoonVector vecRealPos);
        IYoonVector GetPixelResolution(IYoonVector vecPixelPos);   // mm/pixel

        IYoonVector ToPixel(IYoonVector vecRealPos);
        IYoonVector ToReal(IYoonVector vecPixelPos);
    }

    public interface IYoonSection<TKey, TValue> : IEnumerable<KeyValuePair<TKey, TValue>>, IDictionary<TKey, TValue>
    {
        IEqualityComparer<TKey> Comparer { get; }
        TValue this[int nIndex] { get; set; }

        TKey KeyOf(int nIndex);
        int IndexOf(TKey pKey);
        int IndexOf(TKey pKey, int nStartIndex);
        int IndexOf(TKey pKey, int nStartIndex, int nCount);
        int LastIndexOf(TKey pKey);
        int LastIndexOf(TKey pKey, int nStartIndex);
        int LastIndexOf(TKey pKey, int nStartIndex, int nCount);
        void Insert(int nIndex, TKey pKey, TValue pValue);
        void InsertRange(int nIndex, IEnumerable<KeyValuePair<TKey, TValue>> pCollection);
        void RemoveAt(int nIndex);
        void RemoveRange(int nIndex, int nCount);
        void Reverse();
        void Reverse(int nIndex, int nCount);
        ICollection<TValue> GetOrderedValues();
    }
}
