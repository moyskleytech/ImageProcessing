using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MoyskleyTech.ImageProcessing.Drawing3D
{
    public struct Point3D
    {
        public double X { get; set; }
        public double Y { get; set; }
        public double Z { get; set; }


        public double MaxCoodinate
        {
            get { return Math.Max(Math.Max(Math.Abs(X) , Math.Abs(Y)) , Math.Abs(Z)); }

        }

        public static Point3D Origin
        {
            get { return new Point3D(0 , 0 , 0); }
        }

        public Point3D(double x , double y , double z)
        {
            Z = z;
            X = x;
            Y = y;
        }
        public Point3D(Point3D otherPoint)
        {
            X = otherPoint.X;
            Y = otherPoint.Y;
            Z = otherPoint.Z;
        }

        public Point3D(double[ ] coord)
        {
            X = coord[0];
            Y = coord[1];
            Z = coord[2];
        }

        #region Methods
        public static double Distance(Point3D a , Point3D b)
        {
            double x = a.X - b.X;
            double y = a.Y - b.Y;
            double z = a.Z - b.Z;
            return Math.Sqrt(( x * x ) + ( y * y ) + ( z * z ));

        }
        public double DistanceTo(Point3D b)
        {
            return Point3D.Distance(this , b);
        }

        public static bool IsEqual(Point3D a , Point3D b)
        {
            return ( a == b );
        }

        public double[ ] ToArray()
        {
            return new double[ ] { this.X , this.Y , this.Z };
        }


        #endregion

        #region Override Methods
        public object Clone()
        {
            return new Point3D(X , Y , Z);
        }
        public override string ToString()
        {
            return ( X.ToString() + "," + Y.ToString() + "," + Z.ToString() );
        }
        public override bool Equals(object obj)
        {
            if ( object.ReferenceEquals(this , obj) )
                return true;
            else if ( obj is Point3D pt )
                return ( double.Equals(this.X , pt.X) &&
                    double.Equals(this.Y , pt.Y) &&
                    double.Equals(this.Z , pt.Z) );
            return false;
        }
        public override int GetHashCode()
        {
            double z = this.Z;
            return base.GetHashCode() * 397 ^ z.GetHashCode();
        }
        #endregion

        #region Operator Overloading

        public static Point3D operator +(Point3D a , Point3D b)
        {
            return new Point3D(a.X + b.X , a.Y + b.Y , a.Z + b.Z);
        }
        public static Point3D operator -(Point3D a , Point3D b)
        {
            return new Point3D(a.X - b.X , a.Y - b.Y , a.Z - b.Z);
        }
        public static Point3D operator /(Point3D a , double b)
        {
            return new Point3D(a.X / b , a.Y / b , a.Z / b);
        }
        public static Point3D operator *(Point3D a , double b)
        {
            return new Point3D(a.X * b , a.Y * b , a.Z * b);
        }
        public static Point3D operator *(double b , Point3D a)
        {
            return new Point3D(a.X * b , a.Y * b , a.Z * b);
        }
        public static bool operator ==(Point3D a , Point3D b)
        {
            return Point3D.Equals(a , b);
        }
        public static bool operator !=(Point3D a , Point3D b)
        {
            return !Point3D.Equals(a , b);
        }

        #endregion
    }
}
