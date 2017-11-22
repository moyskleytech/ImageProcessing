using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MoyskleyTech.ImageProcessing.Image.SpecializedBrushes
{
    public class LightenBrush : Brush
    {
        Brush inner;
        public double Percent { get; set; }
        public LightenBrush(Brush inner , double percent)
        {
            this.inner = inner;
            this.Percent = percent;
        }
        public override Pixel GetColor(int x , int y)
        {
            Pixel p = inner.GetColor(x , y);
            int dr = 255-p.R;
            int dg=  255-p.G;
            int db = 255-p.G;

            p.R = ( byte ) ( p.R + dr * Percent );
            p.G = ( byte ) ( p.G + dg * Percent );
            p.B = ( byte ) ( p.B + db * Percent );

            return p;
        }
    }
}
