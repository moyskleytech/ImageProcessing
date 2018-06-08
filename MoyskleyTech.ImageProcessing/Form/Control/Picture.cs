using MoyskleyTech.ImageProcessing.Image;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MoyskleyTech.ImageProcessing.Form.Control
{
    public class Picture : BaseControl
    {
        private PictureRenderer BB;
        public Brush<Pixel> BackColor { get => BB.BackColor; set => BB.BackColor = value; }
        public Brush<Pixel> BorderColor { get => BB.BorderColor; set => BB.BorderColor = value; }
        public int BorderSize { get => BB.BorderSize; set => BB.BorderSize = value; }
        public Image<Pixel> Image { get => BB.Image; set => BB.Image = value; }

        public Picture()
        {
            Renderer = BB = new PictureRenderer();
        }
        public Picture(PictureRenderer render)
        {
            Renderer = BB = render;
        }
    }
}
