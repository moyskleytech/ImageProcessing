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
            return ToWPFBitmap(( ImageProxy<Pixel> ) bmp);
        }
        public static BitmapImage ToWPFBitmap(this ImageProxy<Pixel> bmp)
        {
            var stream = new MemoryStream();
            new BitmapCodec().Save<Pixel>(bmp , stream);

            stream.Position = 0;
            var bitmap = new BitmapImage();
            bitmap.BeginInit();
            bitmap.StreamSource = stream;
            bitmap.CacheOption = BitmapCacheOption.OnLoad;
            bitmap.EndInit();
            bitmap.Freeze();
            return bitmap;
        }
        public static Image<Pixel> ToBitmap(this BitmapSource src)
        {
            if ( src.Format != PixelFormats.Bgra32 )
                src = new FormatConvertedBitmap(src , PixelFormats.Bgra32 , null , 0);

            int stride = (int)src.PixelWidth * 4;

            byte[] pixels = new byte[(int)src.PixelHeight * stride];

            src.CopyPixels(pixels , stride , 0);

            Image<Pixel> bmp = new Image<Pixel>(src.PixelWidth,src.PixelHeight);
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
