using System.Xml.Serialization;

namespace YoonFactory
{
    public class YoonJointD : IYoonJoint<double>
    {
        public IYoonJoint Clone()
        {
            return new YoonJointD
            {
                J1 = J1,
                J2 = J2,
                J3 = J3,
                J4 = J4,
                J5 = J5,
                J6 = J6
            };
        }

        public void CopyFrom(IYoonJoint pJoint)
        {
            if (pJoint is not YoonJointD pJointDouble) return;
            J1 = pJointDouble.J1;
            J2 = pJointDouble.J2;
            J3 = pJointDouble.J3;
            J4 = pJointDouble.J4;
            J5 = pJointDouble.J5;
            J6 = pJointDouble.J6;
        }

        [XmlIgnore] public double J1 { get; set; }
        [XmlAttribute] public double J2 { get; set; }
        [XmlAttribute] public double J3 { get; set; }
        [XmlIgnore] public double J4 { get; set; }
        [XmlAttribute] public double J5 { get; set; }
        [XmlAttribute] public double J6 { get; set; }

        public double[] ToArray => new double[6] {J1, J2, J3, J4, J5, J6};

        public YoonJointD()
        {
            Zero();
        }

        public YoonJointD(IYoonJoint j)
        {
            CopyFrom(j);
        }

        public YoonJointD(double dJ1, double dJ2, double dJ3, double dJ4, double dJ5, double dJ6)
        {
            J1 = dJ1;
            J2 = dJ2;
            J3 = dJ3;
            J4 = dJ4;
            J5 = dJ5;
            J6 = dJ6;
        }

        public void Zero()
        {
            J1 = J2 = J3 = J4 = J5 = J6 = 0;
        }

        public static YoonJointD operator *(YoonJointD pJoint, double dNum)
        {
            return new YoonJointD(pJoint.J1 * dNum, pJoint.J2 * dNum, pJoint.J3 * dNum, pJoint.J4 * dNum,
                pJoint.J5 * dNum, pJoint.J6 * dNum);
        }

        public static YoonJointD operator +(YoonJointD pJointSource, YoonJointD pJointObject)
        {
            return new YoonJointD(pJointSource.J1 + pJointObject.J1, pJointSource.J2 + pJointObject.J2,
                pJointSource.J3 + pJointObject.J3, pJointSource.J4 + pJointObject.J4, pJointSource.J5 + pJointObject.J5,
                pJointSource.J6 + pJointObject.J6);
        }

        public static YoonJointD operator -(YoonJointD pJointSource, YoonJointD pJointObject)
        {
            return new YoonJointD(pJointSource.J1 - pJointObject.J1, pJointSource.J2 - pJointObject.J2,
                pJointSource.J3 - pJointObject.J3, pJointSource.J4 - pJointObject.J4, pJointSource.J5 - pJointObject.J5,
                pJointSource.J6 - pJointObject.J6);
        }

        public static YoonJointD operator /(YoonJointD pJoint, double dNum)
        {
            return new YoonJointD(pJoint.J1 / dNum, pJoint.J2 / dNum, pJoint.J3 / dNum, pJoint.J4 / dNum,
                pJoint.J5 / dNum, pJoint.J6 / dNum);
        }
    }
}
