using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MoyskleyTech.Mathematics
{
    public struct Coordinate
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
        public Coordinate(double x , double y)
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
        public Coordinate Move(double x , double y)
        {
            return new Coordinate(X + x , Y + y);
        }
        /// <summary>
        /// Move point by specified value
        /// </summary>
        /// <param name="x">offset</param>
        /// <returns></returns>
        public Coordinate Move(Coordinate x)
        {
            return new Coordinate(X + x.X , Y + x.Y);
        }
        /// <summary>
        /// Write point
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return "{x:" + X + ",y:" + Y + "}";
        }
       
        public static implicit operator Matrix(Coordinate p)
        {
            return new Matrix(new double[ , ] { { p.X } , { p.Y } , { 1 } });
        }
        public static implicit operator Matrix<double>(Coordinate p)
        {
            return new Matrix<double>(new double[ , ] { { p.X } , { p.Y } , { 1 } });
        }
        public static implicit operator Coordinate<double>(Coordinate p)
        {
            return new Coordinate<double>(p.X,p.Y);
        }
    }
    public struct Coordinate<Number>
        where Number:struct
    {
        /// <summary>
        /// X component
        /// </summary>
        public Number X;
        /// <summary>
        /// Y component
        /// </summary>
        public Number Y;
        /// <summary>
        /// Create point from components
        /// </summary>
        /// <param name="x">X component</param>
        /// <param name="y">Y component</param>
        public Coordinate(Number x , Number y)
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
        public Coordinate<Number> Move(Number x , Number y)
        {
            return new Coordinate<Number>(( dynamic ) X + x , ( dynamic ) Y + y);
        }
        /// <summary>
        /// Move point by specified value
        /// </summary>
        /// <param name="x">offset</param>
        /// <returns></returns>
        public Coordinate<Number> Move(Coordinate<Number> x)
        {
            return new Coordinate<Number>(( dynamic ) X + x.X , ( dynamic ) Y + x.Y);
        }
        /// <summary>
        /// Write point
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return "{x:" + X + ",y:" + Y + "}";
        }

        /// <summary>
        /// Convert Point to PointF
        /// </summary>
        /// <param name="p"></param>
        public static implicit operator Matrix<Number>(Coordinate<Number> p)
        {
            return new Matrix<Number>(new Number[ , ] { { p.X } , { p.Y } , { (dynamic)1 } });
        }
    }
}
