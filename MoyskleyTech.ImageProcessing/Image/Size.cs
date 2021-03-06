﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MoyskleyTech.ImageProcessing.Image
{
    /// <summary>
    /// Represent a size
    /// </summary>
    public struct Size
    {
        /// <summary>
        /// Size components
        /// </summary>
        public int Width,Height;
        /// <summary>
        /// Represent the 0 size
        /// </summary>
        public static Size Empty { get => new Size(0 , 0); }
        /// <summary>
        /// Create a size using values
        /// </summary>
        /// <param name="w"></param>
        /// <param name="h"></param>
        public Size(int w , int h)
        {
            Width = w;
            Height = h;
        }
    }
    /// <summary>
    /// Represent a size
    /// </summary>
    public struct SizeF
    {
        /// <summary>
        /// Size components
        /// </summary>
        public double Width,Height;
        /// <summary>
        /// Represent the 0 size
        /// </summary>
        public static SizeF Empty { get => new SizeF(0 , 0); }
        /// <summary>
        /// Create a size using values
        /// </summary>
        /// <param name="w"></param>
        /// <param name="h"></param>
        public SizeF(double w , double h)
        {
            Width = w;
            Height = h;
        }
       
        public static explicit operator Size(SizeF p)
        {
            return new Size((int)p.Width, (int)p.Height);
        }

        public static implicit operator SizeF(Size p)
        {
            return new SizeF((int)p.Width, (int)p.Height);
        }
    }
}
