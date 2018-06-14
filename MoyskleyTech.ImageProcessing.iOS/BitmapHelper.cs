using CoreGraphics;
using MoyskleyTech.ImageProcessing.Image;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
namespace MoyskleyTech.ImageProcessing.iOS
{
    public static class BitmapHelper
    {

        public static CGBitmapContext ToCGBitmap(this ImageProxy<Pixel> bmp)
        {
            using ( var img = bmp.ToImage() )
            {
                return img.ToCGBitmap();
            }
        }
        public static CGBitmapContext ToCGBitmap(this Image<Pixel> bmp)
        {
            CGBitmapContext ctx = new CGBitmapContext( null,bmp.Width,bmp.Height,8,bmp.Width*4, CGColorSpace.CreateDeviceRGB(), CGImageAlphaInfo.None);
            bmp.CopyTo(ctx.Data);
            return ctx;
        }
        public static CGImage ToCGImage(this CGBitmapContext bmp)
        {
            return bmp.ToImage();
        }
        public static Image<Pixel> ToBitmap(this CGBitmapContext src)
        {
            Image<Pixel> bmp= Image<Pixel>.Create((int)src.Width,(int)src.Height);
            bmp.CopyFrom(src.Data);
            return bmp;
        }
    }
}
