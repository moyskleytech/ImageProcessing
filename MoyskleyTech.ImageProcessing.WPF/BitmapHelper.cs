using MoyskleyTech.ImageProcessing.Image;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace MoyskleyTech.ImageProcessing.WPF
{
    public static class BitmapHelper
    {
        public static BitmapImage ToWPFBitmap(this Image<Pixel> bmp)
        {
            var stream = new MemoryStream();
            if ( bmp is Bitmap b )
                b.Save(stream);
            else
            {
                var bmpS = (Bitmap)bmp.ConvertTo<Pixel>();
                bmpS.Save(stream);
                bmpS.Dispose();
            }
            stream.Position = 0;
            var bitmap = new BitmapImage();
            bitmap.BeginInit();
            bitmap.StreamSource = stream;
            bitmap.CacheOption = BitmapCacheOption.OnLoad;
            bitmap.EndInit();
            bitmap.Freeze();
            return bitmap;
        }
        public static Bitmap ToBitmap(this BitmapSource src)
        {
            if ( src.Format != PixelFormats.Bgra32 )
                src = new FormatConvertedBitmap(src , PixelFormats.Bgra32 , null , 0);

            int stride = (int)src.PixelWidth * 4;

            byte[] pixels = new byte[(int)src.PixelHeight * stride];

            src.CopyPixels(pixels , stride , 0);

            Bitmap bmp = new Bitmap(src.PixelWidth,src.PixelHeight);
            unsafe
            {
                fixed ( byte* ptr = pixels )
                {
                    bmp.CopyFromBGRA(ptr);
                }
            }
            return bmp;
        }
    }
}
