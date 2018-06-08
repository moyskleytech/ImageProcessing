using MoyskleyTech.ImageProcessing.Image;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MoyskleyTech.ImageProcessing.Form.Control
{
    public class ProgressBar : BaseControl
    {
        private ProgressBarRenderer BB;

        public Brush<Pixel> BarColor { get => BB.BarColor; set => BB.BarColor = value; }
        public Brush<Pixel> BackColor { get => BB.BackColor; set => BB.BackColor = value; }
        public int BorderSize { get => BB.BorderSize; set => BB.BorderSize = value; }
        public Brush<Pixel> BorderColor { get => BB.BorderColor; set => BB.BorderColor = value; }
        public double Max { get => BB.Max; set => BB.Max = value; }
        public double Value { get => BB.Value; set => BB.Value = value; }
        public ProgressBarOrientation Orientation { get => BB.Orientation; set => BB.Orientation = value; }

        public ProgressBar()
        {
            Renderer = BB = new ProgressBarRenderer();
        }
        public ProgressBar(ProgressBarRenderer render)
        {
            Renderer = BB = render;
        }
    }
}
