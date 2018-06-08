using MoyskleyTech.ImageProcessing.Image;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MoyskleyTech.ImageProcessing.Form.Control
{
    public class Panel : BaseControl
    {
        private PanelRenderer BB;
        public Brush<Pixel> BackColor { get => BB.BackColor; set => BB.BackColor = value; }
        public Brush<Pixel> BorderColor { get => BB.BorderColor; set => BB.BorderColor = value; }
        public int BorderSize { get => BB.BorderSize; set => BB.BorderSize = value; }
        public double BorderRadius { get => BB.BorderRadius; set => BB.BorderRadius = value; }

        public Panel()
        {
            Renderer = BB = new PanelRenderer();
        }
        public Panel(PanelRenderer render)
        {
            Renderer = BB = render;
        }
    }
}
