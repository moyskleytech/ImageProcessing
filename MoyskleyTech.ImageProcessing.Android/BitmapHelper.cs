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
        public static G.Bitmap ToAndroidBitmap(this Bitmap bmp)
        {
            G.Bitmap bitmap = G.Bitmap.CreateBitmap(bmp.Width,bmp.Height, G.Bitmap.Config.Argb8888);
            bitmap.CopyFrom(bmp);
            return bitmap;
        }
        public static Bitmap ToBitmap(this G.Bitmap src)
        {
            Bitmap bmp= new Bitmap(src.Width,src.Height);
            bmp.CopyFrom(src);
            return bmp;
        }
        public static void CopyFrom(this G.Bitmap bitmap , Bitmap bmp)
        {
            var ptr = bitmap.LockPixels();
            bmp.CopyToRGBA(ptr);
            bitmap.UnlockPixels();
        }
        public static void CopyFrom(this Bitmap bmp,G.Bitmap src)
        {
            var ptr = src.LockPixels();
            bmp.CopyFromRGBA(ptr);
            src.UnlockPixels();
        }
    }
}
