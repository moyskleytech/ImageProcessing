using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MoyskleyTech.ImageProcessing.Image.SpecializedBrushes
{
    public class NoiseBrush:Brush
    {
        Random r=new Random();
        public override Pixel GetColor(int x , int y)
        {
            byte[] bytes=new byte[3];
            r.NextBytes(bytes);
            return Pixel.FromArgb(255 , bytes[0] , bytes[1] , bytes[2]);
        }
    }
}
