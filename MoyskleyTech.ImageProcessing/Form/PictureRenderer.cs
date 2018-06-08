using MoyskleyTech.ImageProcessing.Image;
using MoyskleyTech.ImageProcessing.Image.SpecializedBrushes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MoyskleyTech.ImageProcessing.Form
{
    public class PictureRenderer : IRenderer
    {
        public Brush<Pixel> BackColor { get; set; } = ( Brush ) Pixels.Black;
        public int BorderSize { get; set; } = 0;
        public Brush<Pixel> BorderColor { get; set; } = ( Brush ) Pixels.Silver;
        public Image<Pixel> Image { get; set; } = null;
        private Image<Pixel> cache;
        public virtual void Render(Graphics<Pixel> g , int x , int y , int width , int height)
        {
            g.FillRectangle(BackColor , x , y , width , height);
            if ( Image != null )
            {
                if ( cache==null|| cache.Width != width && cache.Height != height )
                {
                    cache?.Dispose();
                    cache = Ajust(width, height);
                }
                g.DrawImage(cache , x , y);
            }

            g.DrawRectangle(BorderColor , x , y , width , height , BorderSize);

        }
        private Image<Pixel> Ajust(int width , int height)
        {
            return Image.Resize(width , height , ScalingMode.AverageInterpolate);
        }
    }
}
