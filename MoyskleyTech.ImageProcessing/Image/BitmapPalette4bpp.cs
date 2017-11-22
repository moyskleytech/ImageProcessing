using System;
using System.Collections.Generic;
using System.Linq;

namespace MoyskleyTech.ImageProcessing.Image
{
    public class BitmapPalette4bpp
    {
        Pixel[] inner = new Pixel[16];
       
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
    public class BitmapPalette2bpp
    {
        Pixel[] inner = new Pixel[4];

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
    public class BitmapPalette1bpp
    {
        Pixel[] inner = new Pixel[2];

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