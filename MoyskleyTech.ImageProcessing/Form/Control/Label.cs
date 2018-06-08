using MoyskleyTech.ImageProcessing.Image;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MoyskleyTech.ImageProcessing.Form.Control
{
    public class Label : BaseControl
    {
        private LabelRenderer BB;

        public string Text { get => BB.Text; set => BB.Text = value; }
        public FontSizeF Font { get => BB.Font; set => BB.Font = value; }
        public bool AutoSize { get => BB.AutoSize; set => BB.AutoSize = value; }
        public StringFormat TextAlign { get => BB.TextAlign; set => BB.TextAlign = value; }
        public Brush<Pixel> ForeColor { get => BB.ForeColor; set => BB.ForeColor = value; }

        public Label()
        {
            Renderer = BB = new LabelRenderer();
        }
        public Label(LabelRenderer render)
        {
            Renderer = BB = render;
        }
    }
}
