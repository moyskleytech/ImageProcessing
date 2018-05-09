using System;
using System.Collections.Generic;
using System.Linq;

namespace MoyskleyTech.ImageProcessing.Image
{
    /// <summary>
    /// Palette for 4bpp Bitmap
    /// </summary>
    public class BitmapPalette4bpp
    {
        Pixel[] inner = new Pixel[16];
       
        /// <summary>
        /// Create palette using a gradient from two colors
        /// </summary>
        /// <param name="first">Start color</param>
        /// <param name="last">End color</param>
        /// <returns></returns>
        public static BitmapPalette4bpp FromGradient(Pixel first , Pixel last)
        {
            BitmapPalette4bpp palette = new BitmapPalette4bpp();
            LinearGradientBrush brush = new LinearGradientBrush(new Point(0,0), first,new Point(16,0),last );
            for ( var i = 0; i < 16; i++ )
            {
                palette[i] = brush.GetColor(15-i , 0);
            }
            return palette;
        }
        /// <summary>
        /// Accesor for color in palette
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        public Pixel this[int index]
        {
            get
            {
                return inner[index];
            }
            set
            {
                inner[index] = value;
            }
        }
        /// <summary>
        /// Accesor for finding Pixel in palette or -1 if not present
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        public int this[Pixel index]
        {
            get
            {
                return Array.IndexOf<Pixel>(inner , index);
            }
        }
        internal void CheckIfAlphaZero()
        {
            if ( inner.All((x) => x.A == 0) )
                for ( var i = 0; i < inner.Length; i++ )
                    inner[i].A = 255;
        }
    }
    /// <summary>
    /// Palette for 2bpp bitmap
    /// </summary>
    public class BitmapPalette2bpp
    {
        Pixel[] inner = new Pixel[4];
        /// <summary>
        /// Create from 2 colors
        /// </summary>
        /// <param name="first"></param>
        /// <param name="last"></param>
        /// <returns></returns>
        public static BitmapPalette2bpp FromGradient(Pixel first , Pixel last)
        {
            BitmapPalette2bpp palette = new BitmapPalette2bpp();
            LinearGradientBrush brush = new LinearGradientBrush(new Point(0,0), first,new Point(4,0),last );
            for ( var i = 0; i < 4; i++ )
            {
                palette[i] = brush.GetColor(3 - i , 0);
            }
            return palette;
        }
        /// <summary>
        /// Get color at specified index
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        public Pixel this[int index]
        {
            get
            {
                return inner[index];
            }
            set
            {
                inner[index] = value;
            }
        }
        /// <summary>
        /// Find index of color
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        public int this[Pixel index]
        {
            get
            {
                return Array.IndexOf<Pixel>(inner , index);
            }
        }
        internal void CheckIfAlphaZero()
        {
            if ( inner.All((x) => x.A == 0) )
                for ( var i = 0; i < inner.Length; i++ )
                    inner[i].A = 255;
        }
    }
    /// <summary>
    /// Palette for 1bpp Bitmap(Monochrome)
    /// </summary>
    public class BitmapPalette1bpp
    {
        Pixel[] inner = new Pixel[2];
        /// <summary>
        /// Access palette at index
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        public Pixel this[int index]
        {
            get
            {
                return inner[index];
            }
            set
            {
                inner[index] = value;
            }
        }
        /// <summary>
        /// Find index of color
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        public int this[Pixel index]
        {
            get
            {
                return Array.IndexOf<Pixel>(inner , index);
            }
        }
        internal void CheckIfAlphaZero()
        {
            if ( inner.All((x) => x.A == 0) )
                for ( var i = 0; i < inner.Length; i++ )
                    inner[i].A = 255;
        }
    }
}