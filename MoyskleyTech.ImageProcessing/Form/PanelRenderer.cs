using MoyskleyTech.ImageProcessing.Image;
using MoyskleyTech.ImageProcessing.Image.SpecializedBrushes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MoyskleyTech.ImageProcessing.Form
{
    public class PanelRenderer : IRenderer
    {
        public Brush<Pixel> BackColor { get; set; } = ( Brush ) Pixels.LightGray;
        public double BorderRadius { get; set; } = 0;
        public int BorderSize { get; set; } = 0;
        public Brush<Pixel> BorderColor { get; set; } = ( Brush ) Pixels.Black;
        public virtual void Render(Graphics<Pixel> g , int x , int y , int width , int height)
        {
            g.FillRoundedRectangle(BackColor , x , y , width , height , BorderRadius);
            if ( BorderSize > 0 )
                g.DrawRoundedRectangle(BorderColor , BorderSize , x , y , width , height , BorderRadius);
        }
    }
}
