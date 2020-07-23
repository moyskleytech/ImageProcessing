using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MoyskleyTech.ImageProcessing.Image
{
    public static class BitmapCodecExtention
    {
        public static void Save(this BitmapCodec fact,ImageProxy<Pixel> bmp, string filename)
        {
            var fs = System.IO.File.OpenWrite(filename);
            fact.Save(bmp,fs);
            fs.Dispose();
        }
        public static void Save(this BitmapCodec fact , ImageProxy<Pixel> bmp , string filename, BitmapPalette1bpp palette)
        {
            var fs = System.IO.File.OpenWrite(filename);
            fact.Save(bmp , fs,palette);
            fs.Dispose();
        }
        public static void Save(this BitmapCodec fact , ImageProxy<Pixel> bmp , string filename , BitmapPalette2bpp palette)
        {
            var fs = System.IO.File.OpenWrite(filename);
            fact.Save(bmp , fs , palette);
            fs.Dispose();
        }
        public static void Save(this BitmapCodec fact , ImageProxy<Pixel> bmp , string filename , BitmapPalette4bpp palette)
        {
            var fs = System.IO.File.OpenWrite(filename);
            fact.Save(bmp , fs , palette);
            fs.Dispose();
        }
        public static void Save(this BitmapCodec fact, ImageProxy<Pixel> bmp , string filename , BitmapPalette8bpp palette)
        {
            var fs = System.IO.File.OpenWrite(filename);
            fact.Save(bmp , fs , palette);
            fs.Dispose();
        }
    }
}
