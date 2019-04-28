using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MoyskleyTech.ImageProcessing.Image
{
    /// <summary>
    /// Represent a Rectangle
    /// </summary>
    public struct Rectangle
    {
        /// <summary>
        /// X position
        /// </summary>
        public int X;
        /// <summary>
        /// Y position
        /// </summary>
        public int Y;
        /// <summary>
        /// Width
        /// </summary>
        public int Width;
        /// <summary>
        /// Height
        /// </summary>
        public int Height;
        /// <summary>
        /// Create from components
        /// </summary>
        /// <param name="x">X component</param>
        /// <param name="y">Y component</param>
        /// <param name="w">Width</param>
        /// <param name="h">Height</param>
        public Rectangle(int x , int y , int w , int h)
        {
            X = x;
            Y = y;
            Width = w;
            Height = h;
        }
        /// <summary>
        /// Create from components
        /// </summary>
        /// <param name="x">X component</param>
        /// <param name="y">Y component</param>
        /// <param name="w">Width</param>
        /// <param name="h">Height</param>
        public Rectangle(Point pt, Size s)
        {
            X = pt.X;
            Y = pt.Y;
            Width = s.Width;
            Height = s.Height;
        }
        /// <summary>
        /// Get position of Rectangle
        /// </summary>
        public Point Location
        {
            get { return new Point() { X = X , Y = Y }; }
            set { X = value.X;Y = value.Y; }
        }
        /// <summary>
        /// Get position of Rectangle
        /// </summary>
        public Size Size
        {
            get { return new Size() {  Height=Height, Width=Width }; }
            set { Height = value.Height; Width = value.Width; }
        }
        /// <summary>
        /// Left of rectangle
        /// </summary>
        public int Left { get { return X; }set { X = value; } }
        /// <summary>
        /// Top of rectangle
        /// </summary>
        public int Top { get { return Y ; } set { Y = value; } }
        /// <summary>
        /// Right of rectangle
        /// </summary>
        public int Right { get { return X + Width-1; } }
        /// <summary>
        /// Bottom of rectangle
        /// </summary>
        public int Bottom { get { return Y + Height-1; } }
    }
    /// <summary>
    /// Represent a Rectangle
    /// </summary>
    public struct RectangleF
    {
        /// <summary>
        /// X position
        /// </summary>
        public double X;
        /// <summary>
        /// Y position
        /// </summary>
        public double Y;
        /// <summary>
        /// Width
        /// </summary>
        public double Width;
        /// <summary>
        /// Height
        /// </summary>
        public double Height;
        /// <summary>
        /// Create from components
        /// </summary>
        /// <param name="x">X component</param>
        /// <param name="y">Y component</param>
        /// <param name="w">Width</param>
        /// <param name="h">Height</param>
        public RectangleF(double x , double y , double w , double h)
        {
            X = x;
            Y = y;
            Width = w;
            Height = h;
        }
        /// <summary>
        /// Create from components
        /// </summary>
        /// <param name="x">X component</param>
        /// <param name="y">Y component</param>
        /// <param name="w">Width</param>
        /// <param name="h">Height</param>
        public RectangleF(PointF pt , SizeF s)
        {
            X = pt.X;
            Y = pt.Y;
            Width = s.Width;
            Height = s.Height;
        }
        /// <summary>
        /// Get position of Rectangle
        /// </summary>
        public PointF Location
        {
            get { return new PointF() { X = X , Y = Y }; }
            set { X = value.X; Y = value.Y; }
        }
        /// <summary>
        /// Get position of Rectangle
        /// </summary>
        public SizeF Size
        {
            get { return new SizeF() { Height = Height , Width = Width }; }
            set { Height = value.Height; Width = value.Width; }
        }
        /// <summary>
        /// Left of rectangle
        /// </summary>
        public double Left { get { return X; } set { X = value; } }
        /// <summary>
        /// Top of rectangle
        /// </summary>
        public double Top { get { return Y; } set { Y = value; } }
        /// <summary>
        /// Right of rectangle
        /// </summary>
        public double Right { get { return X + Width - 1; } }
        /// <summary>
        /// Bottom of rectangle
        /// </summary>
        public double Bottom { get { return Y + Height - 1; } }

        public static implicit operator RectangleF(Rectangle rect)
        {
            return new RectangleF(rect.X , rect.Y , rect.Width , rect.Height);
        }
        public static explicit operator Rectangle(RectangleF rect)
        {
            return new Rectangle((int)rect.X, (int)rect.Y, (int)rect.Width, (int)rect.Height);
        }
    }
}
