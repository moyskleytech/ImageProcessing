using MoyskleyTech.ImageProcessing.Image;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MoyskleyTech.ImageProcessing.Form.Control
{
    public class BlockPanel : BaseControl, ICheckHoverController
    {
        private PanelRenderer BB;

        public event EventHandler<Point> CursorHover;
        public event EventHandler<ClickEvent> CursorClick;
        public event EventHandler<ClickEvent> CursorDown;
        public event EventHandler<ClickEvent> CursorUp;

        public Brush<Pixel> BackColor { get => BB.BackColor; set => BB.BackColor = value; }
        public Brush<Pixel> BorderColor { get => BB.BorderColor; set => BB.BorderColor = value; }
        public int BorderSize { get => BB.BorderSize; set => BB.BorderSize = value; }
        public double BorderRadius { get => BB.BorderRadius; set => BB.BorderRadius = value; }
        public bool IsHover { get; set; }
        public bool IsPressed { get; set; }
        public bool IsPressedRight { get; set; }

        public BlockPanel()
        {
            Renderer = BB = new PanelRenderer();
        }
        public BlockPanel(PanelRenderer render)
        {
            Renderer = BB = render;
        }

        public void RaiseHover(Point location)
        {
            
        }

        public void RaiseClick(Point location , bool left)
        {
            
        }

        public void RaiseCursorDown(Point location , bool left)
        {
            
        }

        public void RaiseCursorUp(Point location , bool left)
        {
            
        }
    }
}
