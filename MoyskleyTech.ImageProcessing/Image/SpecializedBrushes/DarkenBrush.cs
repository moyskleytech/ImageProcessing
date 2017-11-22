using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MoyskleyTech.ImageProcessing.Image.SpecializedBrushes
{
    public class DarkenBrush : Brush
    {
        Brush inner;
        public double Percent { get; set; }
        public DarkenBrush(Brush inner , double percent)
        {
            this.inner = inner;
            this.Percent = percent;
        }
        public override Pixel GetColor(int x , int y)
        {
            Pixel p = inner.GetColor(x , y);
            int dr = p.R;
            int dg=  p.G;
            int db = p.G;

            p.R = ( byte ) ( dr - dr * Percent );
            p.G = ( byte ) ( dg - dg * Percent );
            p.B = ( byte ) ( db - db * Percent );

            return p;
        }
    }
}
