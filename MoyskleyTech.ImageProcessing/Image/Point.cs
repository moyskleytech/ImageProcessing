using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MoyskleyTech.ImageProcessing.Image
{
    /// <summary>
    /// Represent an integer point(8 bytes)
    /// </summary>
    public struct Point : IEquatable<Point>
    {
        /// <summary>
        /// X component
        /// </summary>
        public int X;
        /// <summary>
        /// Y component
        /// </summary>
        public int Y;
        /// <summary>
        /// Create point from components
        /// </summary>
        /// <param name="x">X component</param>
        /// <param name="y">Y component</param>
        public Point(int x , int y)
        {
            Y = y;
            X = x;
        }
        /// <summary>
        /// Move point by specified value
        /// </summary>
        /// <param name="x">X offset</param>
        /// <param name="y">Y offset</param>
        /// <returns></returns>
        public Point Move(int x , int y)
        {
            return new Point(X + x , Y + y);
        }
        /// <summary>
        /// Move point by specified value
        /// </summary>
        /// <param name="x">offset</param>
        /// <returns></returns>
        public Point Move(Point x)
        {
            return new Point(X + x.X , Y + x.Y);
        }
        /// <summary>
        /// Write point
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return "{x:" + X + ",y:" + Y + "}";
        }

        public override bool Equals(object obj)
        {
            return obj is Point && Equals(( Point ) obj);
        }

        public bool Equals(Point other)
        {
            return X == other.X &&
                     Y == other.Y;
        }

        public override int GetHashCode()
        {
            var hashCode = 1861411795;
            hashCode = hashCode * -1521134295 + base.GetHashCode();
            hashCode = hashCode * -1521134295 + X.GetHashCode();
            hashCode = hashCode * -1521134295 + Y.GetHashCode();
            return hashCode;
        }

        /// <summary>
        /// Convert Point to PointF
        /// </summary>
        /// <param name="p"></param>
        public static explicit operator Mathematics.Coordinate(Point p)
        {
            return new Mathematics.Coordinate(p.X , p.Y);
        }
        /// <summary>
        /// Convert Point to PointF
        /// </summary>
        /// <param name="p"></param>
        public static implicit operator Mathematics.Matrix(Point p)
        {
            return new Mathematics.Matrix(new double[ , ] { { p.X } , { p.Y } , { 1 } });
        }
        public static bool operator ==(Point p1, Point p2)
        {
            return p1.X == p2.X && p1.Y == p2.Y;
        }
        public static bool operator !=(Point p1 , Point p2)
        {
            return p1.X != p2.X || p1.Y != p2.Y;
        }

        public static Rectangle operator +(Point p, Size s)
        {
            return new Rectangle(p, s);
        }
        public static Rectangle operator +(Size s, Point p)
        {
            return new Rectangle(p, s);
        }
    }
    /// <summary>
    /// Represent an integer point(16 bytes)
    /// </summary>
    public struct PointF : IEquatable<PointF>
    {
        /// <summary>
        /// X component
        /// </summary>
        public double X;
        /// <summary>
        /// Y component
        /// </summary>
        public double Y;
        /// <summary>
        /// Create point from components
        /// </summary>
        /// <param name="x">X component</param>
        /// <param name="y">Y component</param>
        public PointF(double x , double y)
        {
            Y = y;
            X = x;
        }
        /// <summary>
        /// Move point by specified value
        /// </summary>
        /// <param name="x">X offset</param>
        /// <param name="y">Y offset</param>
        /// <returns></returns>
        public PointF Move(double x , double y)
        {
            return new PointF(X + x , Y + y);
        }
        /// <summary>
        /// Move point by specified value
        /// </summary>
        /// <param name="x">offset</param>
        /// <returns></returns>
        public PointF Move(PointF x)
        {
            return new PointF(X + x.X , Y + x.Y);
        }
        /// <summary>
        /// Write point
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return "{x:" + X + ",y:" + Y + "}";
        }

        public override bool Equals(object obj)
        {
            return obj is PointF && Equals(( PointF ) obj);
        }

        public bool Equals(PointF other)
        {
            return X == other.X &&
                     Y == other.Y;
        }

        public override int GetHashCode()
        {
            var hashCode = 1861411795;
            hashCode = hashCode * -1521134295 + base.GetHashCode();
            hashCode = hashCode * -1521134295 + X.GetHashCode();
            hashCode = hashCode * -1521134295 + Y.GetHashCode();
            return hashCode;
        }

        /// <summary>
        /// Convert Point to PointF
        /// </summary>
        /// <param name="p"></param>
        public static implicit operator PointF(Point p)
        {
            return new PointF(p.X , p.Y);
        }
        /// <summary>
        /// Convert Point to PointF
        /// </summary>
        /// <param name="p"></param>
        public static implicit operator PointF(Mathematics.Coordinate p)
        {
            return new PointF(p.X , p.Y);
        }
        /// <summary>
        /// Convert Point to PointF
        /// </summary>
        /// <param name="p"></param>
        public static explicit operator Point(PointF p)
        {
            return new Point((int)p.X , (int)p.Y);
        }
        /// <summary>
        /// Convert Point to PointF
        /// </summary>
        /// <param name="p"></param>
        public static explicit operator Mathematics.Coordinate(PointF p)
        {
            return new Mathematics.Coordinate(p.X , p.Y);
        }
        /// <summary>
        /// Convert Point to PointF
        /// </summary>
        /// <param name="p"></param>
        public static implicit operator Mathematics.Matrix(PointF p)
        {
            return new Mathematics.Matrix(new double[ , ] { { p.X } , { p.Y } , { 1 } });
        }
        public static bool operator ==(PointF p1 , PointF p2)
        {
            return p1.X == p2.X && p1.Y == p2.Y;
        }
        public static bool operator !=(PointF p1 , PointF p2)
        {
            return p1.X != p2.X || p1.Y != p2.Y;
        }

        public static RectangleF operator +(PointF p, SizeF s)
        {
            return new RectangleF(p, s);
        }
        public static RectangleF operator +(SizeF s, PointF p)
        {
            return new RectangleF(p, s);
        }
    }
}
