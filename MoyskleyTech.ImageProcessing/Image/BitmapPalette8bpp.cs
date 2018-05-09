using System;
using System.Collections.Generic;
using System.Linq;

namespace MoyskleyTech.ImageProcessing.Image
{
    /// <summary>
    /// Palette for 8bpp Bitmap(256 color)
    /// </summary>
    public class BitmapPalette8bpp
    {
        Pixel[] inner = new Pixel[256];
        /// <summary>
        /// The Gray default Palette
        /// </summary>
        public static BitmapPalette8bpp Grayscale
        {
            get
            {
                BitmapPalette8bpp palette = new BitmapPalette8bpp();
                for ( var i = 0; i < 256; i++ )
                {
                    palette[i] = Pixel.FromArgb(255 , ( byte ) i , ( byte ) i , ( byte ) i);
                }
                return palette;
            }
        }
        /// <summary>
        /// Create the palette using 2 colors(usefull for temperatures)
        /// </summary>
        /// <param name="first"></param>
        /// <param name="last"></param>
        /// <returns></returns>
        public static BitmapPalette8bpp FromGradient(Pixel first , Pixel last)
        {
            BitmapPalette8bpp palette = new BitmapPalette8bpp();
            LinearGradientBrush brush = new LinearGradientBrush(new Point(0,0), first,new Point(255,0),last );
            for ( var i = 0; i < 256; i++ )
            {
                palette[i] = brush.GetColor(255-i , 0);
            }
            return palette;
        }
        /// <summary>
        /// Access color by index
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
        /// Index of color
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        public int this[Pixel index]
        {
            get
            {
               return Array.IndexOf<Pixel>(inner,index);
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