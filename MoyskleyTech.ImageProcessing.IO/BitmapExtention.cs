using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MoyskleyTech.ImageProcessing.Image
{
    public static class BitmapExtention
    {
        public static void Save(this Bitmap fact, string filename)
        {
            var fs = System.IO.File.OpenWrite(filename);
            fact.Save(fs);
            fs.Dispose();
        }
    }
}
