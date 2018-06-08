using MoyskleyTech.ImageProcessing.Image;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MoyskleyTech.ImageProcessing.Form.Control
{
    public class Button: BaseControl,ICheckHoverController
    {
        private ButtonBase BB;
        public string Text { get => BB.Text; set => BB.Text = value; }
        public Bitmap BackgroundImage { get => BB.BackgroundImage; set => BB.BackgroundImage = value; }
        public Bitmap Icon { get => BB.Icon; set => BB.Icon = value; }
        public FontSizeF Font { get => BB.Font; set => BB.Font = value; }
        public StringFormat TextAlign { get => BB.TextAlign; set => BB.TextAlign = value; }
        public Brush<Pixel> ForeColor { get => BB.ForeColor; set => BB.ForeColor = value; }
        public Pixel BackColor { get => BB.BackColor; set => BB.BackColor = value; }
        public Pixel HoverColor { get => BB.HoverColor; set => BB.HoverColor = value; }
        public Pixel ClickedColor { get => BB.ClickedColor; set => BB.ClickedColor = value; }
        public double BorderRadius { get => BB.BorderRadius; set => BB.BorderRadius = value; }
        public int BorderSize { get => BB.BorderSize; set => BB.BorderSize = value; }
        public Brush<Pixel> BorderColor { get => BB.BorderColor; set => BB.BorderColor = value; }
        public bool IsHover { get => BB.IsHover; set => BB.IsHover = value; }
        public bool IsPressed { get => BB.IsPressed; set => BB.IsPressed = value; }
        public bool IsPressedRight { get => BB.IsPressed; set => BB.IsPressed = value; }

        public event EventHandler<Point> CursorHover;
        public event EventHandler<ClickEvent> CursorClick;
        public event EventHandler<ClickEvent> CursorDown;
        public event EventHandler<ClickEvent> CursorUp;

        public Button()
        {
            Renderer = BB= new ButtonBase();
            BB.CursorHover += BB_CursorHover;
            BB.CursorClick += BB_CursorClick;
            BB.CursorDown += BB_CursorDown;
            BB.CursorUp += BB_CursorUp;
        }
        public Button(ButtonBase render)
        {
            Renderer = BB = render;
            BB.CursorHover += BB_CursorHover;
            BB.CursorClick += BB_CursorClick;
            BB.CursorDown += BB_CursorDown;
            BB.CursorUp += BB_CursorUp;
        }

        private void BB_CursorClick(object sender , ClickEvent e)
        {
            CursorClick?.Invoke(this , e);
        }
        private void BB_CursorDown(object sender , ClickEvent e)
        {
            CursorDown?.Invoke(this , e);
        }
        private void BB_CursorUp(object sender , ClickEvent e)
        {
            CursorUp?.Invoke(this , e);
        }
        private void BB_CursorHover(object sender , Point e)
        {
            CursorHover?.Invoke(this , e);
        }

        public void RaiseHover(Point location)
        {
            ( ( ICheckHoverController ) BB ).RaiseHover(location);
        }

        public void RaiseClick(Point location , bool left)
        {
            ( ( ICheckHoverController ) BB ).RaiseClick(location , left);
        }

        public void RaiseCursorDown(Point location , bool left)
        {
            ( ( ICheckHoverController ) BB ).RaiseCursorDown(location , left);
        }

        public void RaiseCursorUp(Point location , bool left)
        {
            ( ( ICheckHoverController ) BB ).RaiseCursorUp(location , left);
        }
    }
}
