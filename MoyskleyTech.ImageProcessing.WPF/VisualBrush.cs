
using MoyskleyTech.ImageProcessing.Image;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MoyskleyTech.ImageProcessing.WPF
{
    public class VisualBrush : Brush
    {
        public System.Windows.Media.VisualBrush Inner { get; set; }
        public VisualBrush(System.Windows.Media.VisualBrush inner)
        {
            Inner = inner;
        }
        public override Pixel GetColor(int x , int y)
        { 
            return Pixels.Transparent;
        }
    }
}
