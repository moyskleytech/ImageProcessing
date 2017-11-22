﻿using System;
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
        /// Get position of Rectangle
        /// </summary>
        public Point Location
        {
            get { return new Point() { X = X , Y = Y }; }
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
}
