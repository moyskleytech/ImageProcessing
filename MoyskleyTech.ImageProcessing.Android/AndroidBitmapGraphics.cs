using MoyskleyTech.ImageProcessing.Image;
using MoyskleyTech.ImageProcessing.Android;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using A = global::Android;
using G = global::Android.Graphics;
namespace MoyskleyTech.ImageProcessing.Android
{
    public class AndroidBitmapGraphics : Graphics
    {
        static AndroidBitmapGraphics()
        {
            ColorConvertAndroid.RegisterIfNot();
        }
        G.Bitmap bitmap;
        public static AndroidBitmapGraphics FromBitmap(G.Bitmap bmp)
        {
            return new AndroidBitmapGraphics() { bitmap = bmp };
        }
        protected override void SetPixelInternal(Pixel p , double px , double py)
        {
            bitmap.SetPixel((int)px , ( int )py , G.Color.Argb(p.A , p.R , p.G , p.B));
        }
        protected override void SetPixelInternal(Pixel p , double px , double py,bool alpha)
        {
            if ( alpha )
            {
                var pxl = Pixel.FromArgb(bitmap.GetPixel(( int ) px , ( int ) py));
                p = p.Over(pxl);
            }
            bitmap.SetPixel((int)px , ( int )py , G.Color.Argb(p.A , p.R , p.G , p.B));
        }
    }
}
