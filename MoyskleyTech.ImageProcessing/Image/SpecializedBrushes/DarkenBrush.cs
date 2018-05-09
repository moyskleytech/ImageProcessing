using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MoyskleyTech.ImageProcessing.Image.SpecializedBrushes
{
    /// <summary>
    /// Brush to darken another brush
    /// </summary>
    public class DarkenBrush : Brush
    {
        Brush inner;
        /// <summary>
        /// Percent of loss of brithness
        /// </summary>
        public double Percent { get; set; }
        /// <summary>
        /// Create the brush using another one
        /// </summary>
        /// <param name="inner">Another brush</param>
        /// <param name="percent"></param>
        public DarkenBrush(Brush inner , double percent)
        {
            this.inner = inner;
            this.Percent = percent;
        }
        /// <summary>
        /// Get the color at location
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <returns></returns>
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
