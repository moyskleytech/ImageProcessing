using MoyskleyTech.ImageProcessing.Image;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using A = global::Android;
using G = global::Android.Graphics;
namespace MoyskleyTech.ImageProcessing.Android
{
    public static class BitmapHelper
    {
        static BitmapHelper()
        {
            ColorConvertAndroid.RegisterIfNot();
        }
        public static G.Bitmap ToAndroidBitmap(this Image<Pixel> bmp)
        {
            return ToAndroidBitmap(( ImageProxy<Pixel> ) bmp);
        }
        public static G.Bitmap ToAndroidBitmap(this ImageProxy<Pixel> bmp)
        {
            G.Bitmap bitmap = G.Bitmap.CreateBitmap(bmp.Width,bmp.Height, G.Bitmap.Config.Argb8888);
            var tmp = bmp.ToImage();
            bitmap.CopyFrom(tmp);
            tmp.Dispose();
            return bitmap;
        }
        public static Image<Pixel> ToBitmap(this G.Bitmap src)
        {
            Image<Pixel> bmp= Image<Pixel>.Create(src.Width,src.Height);
            bmp.CopyFrom(src);
            return bmp;
        }
        public static void CopyFrom(this G.Bitmap bitmap , Image<Pixel> bmp)
        {
            var ptr = bitmap.LockPixels();
            bmp.CopyToRGBA(ptr);
            bitmap.UnlockPixels();
        }
        public static void CopyFrom(this Image<Pixel> bmp,G.Bitmap src)
        {
            var ptr = src.LockPixels();
            bmp.CopyFromRGBA(ptr);
            src.UnlockPixels();
        }
    }
}
