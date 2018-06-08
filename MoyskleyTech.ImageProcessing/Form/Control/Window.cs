using MoyskleyTech.ImageProcessing.Image;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MoyskleyTech.ImageProcessing.Form.Control
{
    public class Window : BaseControl
    {
        private WindowRenderer BB;
        public Brush<Pixel> BackColor { get => BB.BackColor; set => BB.BackColor = value; }
        public Brush<Pixel> BorderColor { get => BB.BorderColor; set => BB.BorderColor = value; }
        public int BorderSize { get => BB.BorderSize; set => BB.BorderSize = value; }
        public int TopBorderSize { get => BB.TopBorderSize; set => BB.TopBorderSize = value; }
        public string Title { get => BB.Title; set => BB.Title = value; }
        public FontSizeF Font { get => BB.Font; set => BB.Font = value; }
        public StringFormat TextAlign { get => BB.TextAlign; set => BB.TextAlign = value; }
        public Brush<Pixel> ForeColor { get => BB.ForeColor; set => BB.ForeColor = value; }
        public override int ChildrenOffsetX { get => BB.BorderSize; set => BB.BorderSize = value; }
        public override int ChildrenOffsetY { get => BB.TopBorderSize; set => BB.TopBorderSize = value; }
        public int InnerWidth { get => Width - BorderSize * 2;  }
        public int InnerHeight { get => Height - BorderSize -TopBorderSize; }
        public Window() : this(new WindowRenderer())
        {

        }
        public Window(WindowRenderer render)
        {
            Renderer = BB = render;
        }
    }
}
