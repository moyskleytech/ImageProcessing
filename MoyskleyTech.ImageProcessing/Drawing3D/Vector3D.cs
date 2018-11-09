using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MoyskleyTech.ImageProcessing.Drawing3D
{
    public class Vector3D
    {
        public double X { get; set; }
        public double Y { get; set; }
        public double Z { get; set; }
        public static Vector3D XAxis
        {
            get
            {
                return new Vector3D(1 , 0 , 0);
            }
        }

        public static Vector3D YAxis
        {
            get
            {
                return new Vector3D(0 , 1 , 0);
            }
        }

        public static Vector3D ZAxis
        {
            get
            {
                return new Vector3D(0 , 0 , 1);
            }
        }
        public double Length
        {
            get
            {
                return Math.Sqrt(X * X + Y * Y + Z * Z);
            }
        }
        public static Vector3D Add(Vector3D a , Vector3D b)
        {
            return new Vector3D(a.X + b.X , a.Y + b.Y , a.Z + b.Z);
        }

        public static Vector3D Subtract(Vector3D a , Vector3D b)
        {
            return new Vector3D(a.X - b.X , a.Y - b.Y , a.Z - b.Z);
        }
        public static Vector3D Subtract(Point3D a , Point3D b)
        {
            return new Vector3D(a.X - b.X , a.Y - b.Y , a.Z - b.Z);
        }
        public static Vector3D Add(Point3D a , Point3D b)
        {
            return new Vector3D(a.X + b.X , a.Y + b.Y , a.Z + b.Z);
        }
        public static Vector3D Add(Point3D a , Vector3D b)
        {
            return new Vector3D(a.X + b.X , a.Y + b.Y , a.Z + b.Z);
        }
        public static Vector3D CrossProduct(Vector3D a , Vector3D b)
        {
            return new Vector3D(a.Y * b.Z - a.Z * b.Y , a.Z * b.X - a.X * b.Z , a.X * b.Y - a.Y * b.X);
        }

        public static Vector3D CrossProduct(Point3D a , Point3D b)
        {
            return new Vector3D(a.Y * b.Z - a.Z * b.Y , a.Z * b.X - a.X * b.Z , a.X * b.Y - a.Y * b.X);
        }

        public static double DotProduct(Vector3D a , Vector3D b)
        {
            return a.X * b.X + a.Y * b.Y + a.Z * b.Z;
        }
        public static double DotProduct(Point3D a , Vector3D b)
        {
            return a.X * b.X + a.Y * b.Y + a.Z * b.Z;
        }
        public Vector3D GetNormalVector()
        {
            double val = this.Length;
            return new Vector3D(X / val , Y / val , Z / val);
        }
        public void Normalize()
        {
            double val = this.Length;
            X = X / val;
            Y = Y / val;
            Z = Z / val;
        }

        public override string ToString()
        {
            return ToString();
        }

        public double AngleToR(Vector3D b)
        {
            double angle = (this * b) / (this.Length * b.Length);
            return Math.Acos(angle);
        }
        public double AngleToD(Vector3D b)
        {
            return AngleBetWeenR(this , b) * 180 / Math.PI;
        }


        public static double AngleBetWeenR(Vector3D a , Vector3D b)
        {
            double angle = (a * b) / (a.Length * b.Length);
            return Math.Acos(angle);
        }
        public static double AngleBetWeenD(Vector3D a , Vector3D b)
        {
            return AngleBetWeenR(a , b) * 180 / Math.PI;
        }
        public Vector3D NormalVector(Point3D origin , Point3D a , Point3D b)
        {
            Vector3D u= new Vector3D(origin , a);
            Vector3D v =new Vector3D(origin , b);
            return Vector3D.CrossProduct(u , v);
        }
        public Vector3D CrossProduct(Vector3D v)
        {
            return CrossProduct(this , v);
        }

        public double DotProduct(Vector3D v)
        {
            return DotProduct(this , v);
        }
        public static bool VisibleNormal(Point3D pt1 , Point3D pt2 , Point3D pt3 , Vector3D v4)
        {
            Vector3D v1 = new Vector3D(pt2, pt1);
            Vector3D v2 = new Vector3D(pt2, pt3);
            Vector3D v3 = v1.CrossProduct(v2);
            return v3.DotProduct(v4) < 0;
        }


        public static Vector3D operator +(Vector3D a , Vector3D b)
        {
            return new Vector3D(a.X + b.X , a.Y + b.Y , a.Z + b.Z);
        }

        public static Vector3D operator /(Vector3D v , double s)
        {
            return new Vector3D(v.X / s , v.Y / s , v.Z / s);
        }

        public static Vector3D operator *(double s , Vector3D v)
        {
            return new Vector3D(s * v.X , s * v.Y , s * v.Z);
        }

        public static Vector3D operator *(Vector3D v , double s)
        {
            return new Vector3D(s * v.X , s * v.Y , s * v.Z);
        }

        public static double operator *(Vector3D u , Vector3D v)
        {
            return u.X * v.X + u.Y * v.Y + u.Z * v.Z;
        }

        public static Vector3D operator -(Vector3D v1 , Vector3D v2)
        {
            return new Vector3D(v1.X - v2.X , v1.Y - v2.Y , v1.Z - v2.Z);
        }

        public Vector3D()
        {
        }

        public Vector3D(double x , double y , double z)
        {
            X = x;
            Y = y;
            Z = z;
        }
        public Vector3D(Point3D a , Point3D b)
        {
            X = a.X - b.X;
            Y = a.Y - b.Y;
            Z = a.Z - b.Z;
        }

    }
}
