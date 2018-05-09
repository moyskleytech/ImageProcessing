using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MoyskleyTech.ImageProcessing.Image.SpecializedBrushes
{
    /// <summary>
    /// Brush that alter alpha from another
    /// </summary>
    public class AlphaBrush:Brush
    {
        Brush inner;
        /// <summary>
        /// Desired alpha
        /// </summary>
        public byte Alpha { get; set; }
        /// <summary>
        /// Create the brush
        /// </summary>
        /// <param name="inner"></param>
        /// <param name="alpha"></param>
        public AlphaBrush(Brush inner,byte alpha)
        {
            this.inner = inner;
            this.Alpha = alpha;
        }
        /// <summary>
        /// Get the color at location
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <returns></returns>
        public override Pixel GetColor(int x , int y)
        {
            return Pixel.FromArgb(inner.GetColor(x , y) , Alpha);
        }
    }
}
