using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MoyskleyTech.ImageProcessing.Image
{
    public static class BitmapFactoryExtention
    {
        public static Bitmap Decode(this BitmapFactory fact, string filename)
        {
            var fs = System.IO.File.OpenRead(filename);
            var bmp = fact.Decode(fs);
            fs.Dispose();
            return bmp;
        }
    }
}
