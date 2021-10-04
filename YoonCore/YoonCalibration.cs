using NumSharp;

namespace YoonFactory.Image
{
    public class YoonCalibration
    {
        private double _dFx = 1.0;
        private double _dFy = 1.0;
        private double _dCx = 0.0;
        private double _dCy = 0.0;
        private double _dSkew = 0.0;
        private double[,] _pRotArray = new double[3, 3];
        private double[] _pTransArray = new double[3];

        public YoonVector2D FocalLength => new YoonVector2D(_dFx, _dFy);

        public YoonVector2D PrincipalPoint => new YoonVector2D(_dCx, _dCy);

        public double Skew => _dSkew;

        public YoonMatrix3X3Double CameraMatrix => new YoonMatrix3X3Double
        {
            matrix_11 = _dFx,
            matrix_22 = _dFy,
            matrix_12 = _dSkew,
            matrix_13 = _dCx,
            matrix_23 = _dCy
        };

        public YoonMatrix3X3Double RotationMatrix => new YoonMatrix3X3Double(_pRotArray);
        
        public YoonVector3D Transpose => new YoonVector3D(_pTransArray[0], _pTransArray[1], _pTransArray[2]);

        private NDArray CalibrationMatrix()
        {
            NDArray pRotationArray = new NDArray(_pRotArray.ToArray1D(), new Shape(3, 4));
            NDArray pTransArray = new NDArray(_pTransArray, new Shape(3, 1));
            return np.hstack(pRotationArray, pTransArray);
        }
    }
}