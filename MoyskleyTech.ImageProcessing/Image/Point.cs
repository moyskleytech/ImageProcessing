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
    public struct Point
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
        /// Write point
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return "{x:" + X + ",y:" + Y + "}";
        }
    }
    /// <summary>
    /// Represent an integer point(16 bytes)
    /// </summary>
    public struct PointF
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
        public static implicit operator PointF(Point p)
        {
            return new PointF(p.X , p.Y);
        }
    }
}
