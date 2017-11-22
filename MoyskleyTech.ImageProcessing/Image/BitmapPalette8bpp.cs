using System;
using System.Collections.Generic;
using System.Linq;

namespace MoyskleyTech.ImageProcessing.Image
{
    public class BitmapPalette8bpp
    {
        Pixel[] inner = new Pixel[256];
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

        internal void CheckIfAlphaZero()
        {
            if ( inner.All((x) => x.A == 0) )
                for ( var i = 0; i < inner.Length; i++ )
                    inner[i].A = 255;
        }
    }
}