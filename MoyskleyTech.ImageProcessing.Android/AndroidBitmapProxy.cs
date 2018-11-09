using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using MoyskleyTech.ImageProcessing.Image;
using G = Android.Graphics;
namespace MoyskleyTech.ImageProcessing.Android
{
    public class AndroidBitmapPixelProxy : ImageProxy<Pixel>
    {
        static AndroidBitmapPixelProxy()
        {
            ColorConvertAndroid.RegisterIfNot();
        }
        G.Bitmap image;
        public AndroidBitmapPixelProxy(G.Bitmap bmp):base(false)
        {
            image = bmp;
            rct = new Rectangle(0 , 0 , bmp.Width , bmp.Height);
        }
        public AndroidBitmapPixelProxy(G.Bitmap bmp , Rectangle src) : base(false)
        {
            image = bmp;
            rct = src;
        }
        public override Pixel this[int x , int y]
        {
            get
            {
                if ( x < rct.Width && x >= 0 && y < rct.Height && y >= 0 )
                    return Pixel.FromArgb(image.GetPixel(x , y));
                return Pixels.Transparent;
            }
            set
            {
                if ( x < rct.Width && x >= 0 && y < rct.Height && y >= 0 )
                    image.SetPixel(x , y , G.Color.Argb(value.A , value.R , value.G , value.B));
            }
        }
        public override Image<Pixel> ToImage()
        {
            Image<Pixel> image = Image<Pixel>.Create(rct.Width,rct.Height);
            for ( var x = 0; x < Width; x++ )
                for ( var y = 0; y < Width; y++ )
                    image[x , y] = this[x , y];
            return image;
        }
        public override Image<T> ToImage<T>(Func<Pixel , T> converter)
        {
            Image<T> image = Image<T>.Create(rct.Width,rct.Height);
            for ( var x = 0; x < Width; x++ )
                for ( var y = 0; y < Width; y++ )
                    image[x , y] = converter(this[x , y]);
            return image;
        }
    }
    public class AndroidBitmapProxy : ImageProxy<G.Color>
    {
        static AndroidBitmapProxy()
        {
            ColorConvertAndroid.RegisterIfNot();
        }
        G.Bitmap image;
        public AndroidBitmapProxy(G.Bitmap bmp) : base(false)
        {
            image = bmp;
            rct = new Rectangle(0 , 0 , bmp.Width , bmp.Height);
        }
        public AndroidBitmapProxy(G.Bitmap bmp , Rectangle src) : base(false)
        {
            image = bmp;
            rct = src;
        }
        public override G.Color this[int x , int y]
        {
            get
            {
                if ( x < rct.Width && x >= 0 && y < rct.Height && y >= 0 )
                {
                    var argb = image.GetPixel(x,y);
                    return new G.Color(argb);
                }
                return G.Color.Transparent;
            }
            set
            {
                if ( x < rct.Width && x >= 0 && y < rct.Height && y >= 0 )
                    image.SetPixel(x , y , value);
            }
        }
        public override Image<G.Color> ToImage()
        {
            Image<G.Color> image = Image<G.Color>.Create(rct.Width,rct.Height);
            for ( var x = 0; x < Width; x++ )
                for ( var y = 0; y < Width; y++ )
                    image[x , y] = this[x , y];
            return image;
        }
        public override Image<T> ToImage<T>(Func<G.Color , T> converter)
        {
            Image<T> image = Image<T>.Create(rct.Width,rct.Height);
            for ( var x = 0; x < Width; x++ )
                for ( var y = 0; y < Width; y++ )
                    image[x , y] = converter(this[x , y]);
            return image;
        }
    }
}