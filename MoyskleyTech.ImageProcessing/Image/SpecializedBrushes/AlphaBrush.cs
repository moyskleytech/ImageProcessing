using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MoyskleyTech.ImageProcessing.Image.SpecializedBrushes
{
    public class AlphaBrush:Brush
    {
        Brush inner;
        public byte Alpha { get; set; }
        public AlphaBrush(Brush inner,byte alpha)
        {
            this.inner = inner;
            this.Alpha = alpha;
        }
        public override Pixel GetColor(int x , int y)
        {
            return Pixel.FromArgb(inner.GetColor(x , y) , Alpha);
        }
    }
}
