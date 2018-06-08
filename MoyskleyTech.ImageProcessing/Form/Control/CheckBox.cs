using MoyskleyTech.ImageProcessing.Image;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MoyskleyTech.ImageProcessing.Form.Control
{
    public class CheckBox: BaseControl,ICheckHoverController
    {
        private CheckBoxBase BB;

        public Bitmap BackgroundImage { get => BB.BackgroundImage; set => BB.BackgroundImage = value; }
        public Brush<Pixel> CheckColor { get => BB.CheckColor; set => BB.CheckColor = value; }
        public Brush<Pixel> BackColor { get => BB.BackColor; set => BB.BackColor = value; }
        public Brush<Pixel> HoverColor { get => BB.HoverColor; set => BB.HoverColor = value; }
        public Brush<Pixel> ClickedColor { get => BB.ClickedColor; set => BB.ClickedColor = value; }
        public bool Round { get => BB.Round; set => BB.Round = value; }
        public int BorderSize { get => BB.BorderSize; set => BB.BorderSize = value; }
        public int CheckSize { get => BB.CheckSize; set => BB.CheckSize = value; }
        public Brush<Pixel> BorderColor { get => BB.BorderColor; set => BB.BorderColor = value; }
        public bool IsHover { get => BB.IsHover; set => BB.IsHover = value; }
        public bool IsPressed { get => BB.IsPressed; set => BB.IsPressed = value; }
        public bool IsPressedRight { get => BB.IsPressedRight; set => BB.IsPressedRight = value; }
        public bool? IsChecked { get => BB.IsChecked; set => BB.IsChecked = value; }
        public bool IsReadOnly { get => BB.IsReadOnly; set => BB.IsReadOnly = value; }
        public bool IsRadio { get => BB.IsRadio; set => BB.IsRadio = value; }

        public event EventHandler<Point> CursorHover;
        public event EventHandler<ClickEvent> CursorClick;
        public event EventHandler<ClickEvent> CursorDown;
        public event EventHandler<ClickEvent> CursorUp;

        public CheckBox()
        {
            Renderer = BB= new CheckBoxBase();
            BB.CursorHover += BB_CursorHover;
            BB.CursorClick += BB_CursorClick;
            BB.CursorDown += BB_CursorDown;
            BB.CursorUp += BB_CursorUp;
        }
        public CheckBox(CheckBoxBase render)
        {
            Renderer = BB = render;
            BB.CursorHover += BB_CursorHover;
            BB.CursorClick += BB_CursorClick;
            BB.CursorDown += BB_CursorDown;
            BB.CursorUp += BB_CursorUp;
        }

        private void BB_CursorClick(object sender , ClickEvent e)
        {
            if ( !IsReadOnly )
            {
                if ( IsRadio )
                {
                    foreach ( var cb in Parent.Childrens.OfType<CheckBox>() )
                    {
                        if ( cb.IsRadio )
                            cb.IsChecked = false;
                    }
                    IsChecked = true;
                }
            }
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
