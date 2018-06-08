using MoyskleyTech.ImageProcessing.Image;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MoyskleyTech.ImageProcessing.Form.Control
{
    public class TrackBar : BaseControl, ICheckHoverController
    {
        private TrackBarBase BB;

        public Brush<Pixel> BarColor { get => BB.BarColor; set => BB.BarColor = value; }
        public Brush<Pixel> BackColor { get => BB.BackColor; set => BB.BackColor = value; }
        public int BorderSize { get => BB.BorderSize; set => BB.BorderSize = value; }
        public Brush<Pixel> BorderColor { get => BB.BorderColor; set => BB.BorderColor = value; }
        public double Max { get => BB.Max; set => BB.Max = value; }
        public double Value { get => BB.Value; set => BB.Value = value; }
        public ProgressBarOrientation Orientation { get => BB.Orientation; set => BB.Orientation = value; }
        public bool IsHover { get => ( ( ICheckHoverController ) BB ).IsHover; set => ( ( ICheckHoverController ) BB ).IsHover = value; }
        public bool IsPressed { get => ( ( ICheckHoverController ) BB ).IsPressed; set => ( ( ICheckHoverController ) BB ).IsPressed = value; }
        public bool IsPressedRight { get => ( ( ICheckHoverController ) BB ).IsPressedRight; set => ( ( ICheckHoverController ) BB ).IsPressedRight = value; }

        public event EventHandler<Point> CursorHover;
        public event EventHandler<ClickEvent> CursorClick;
        public event EventHandler<ClickEvent> CursorDown;
        public event EventHandler<ClickEvent> CursorUp;

        public TrackBar()
        {
            Renderer = BB = new TrackBarBase();
            BB.CursorHover += BB_CursorHover;
            BB.CursorClick += BB_CursorClick;
            BB.CursorDown += BB_CursorDown;
            BB.CursorUp += BB_CursorUp;
        }
        public TrackBar(TrackBarBase render)
        {
            Renderer = BB = render;
            BB.CursorHover += BB_CursorHover;
            BB.CursorClick += BB_CursorClick;
            BB.CursorDown += BB_CursorDown;
            BB.CursorUp += BB_CursorUp;
        }
        private void AjustFromClick(Point location)
        {
            double ratio=0;
            switch ( Orientation )
            {
                case ProgressBarOrientation.LeftToRight:
                    ratio = location.X / (double)Width;
                    break;
                case ProgressBarOrientation.RightToLeft:
                    ratio =1-( location.X / ( double ) Width);
                    break;
                case ProgressBarOrientation.TopToBottom:
                    ratio = location.Y / ( double ) Height;
                    break;
                case ProgressBarOrientation.BottomToTop:
                    ratio =1-( location.Y / ( double ) Height);
                    break;
            }
            Value = ratio * Max;
        }
        private void BB_CursorClick(object sender , ClickEvent e)
        {
            AjustFromClick(e.Location);
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
