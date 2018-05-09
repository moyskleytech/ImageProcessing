using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MoyskleyTech.ImageProcessing.Image.SpecializedBrushes
{
    /// <summary>
    /// Brush to lighten another
    /// </summary>
    public class LightenBrush : Brush
    {
        Brush inner;
        /// <summary>
        /// Percent of brithness gain
        /// </summary>
        public double Percent { get; set; }
        /// <summary>
        /// Brush to lighten another
        /// </summary>
        public LightenBrush(Brush inner , double percent)
        {
            this.inner = inner;
            this.Percent = percent;
        }
        /// <summary>
        /// Get Color at specified location
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <returns></returns>
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
