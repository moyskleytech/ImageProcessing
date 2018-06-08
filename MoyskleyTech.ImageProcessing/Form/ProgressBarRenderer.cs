using MoyskleyTech.ImageProcessing.Image;
using MoyskleyTech.ImageProcessing.Image.SpecializedBrushes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MoyskleyTech.ImageProcessing.Form
{
    public class ProgressBarRenderer : IRenderer
    {
        public Brush<Pixel> BarColor { get; set; } = ( Brush ) Pixels.Green;
        public Brush<Pixel> BackColor { get; set; } = ( Brush ) Pixels.LightGray;
        public int BorderSize { get; set; } = 1;
        public Brush<Pixel> BorderColor { get; set; } = ( Brush ) Pixels.Black;
        public double Max { get; set; } = 100;
        public double Value { get; set; } = 10;
        public ProgressBarOrientation Orientation { get; set; } = ProgressBarOrientation.Horizontal;
        public virtual void Render(Graphics<Pixel> g , int x , int y , int width , int height)
        {
            g.FillRectangle(BackColor , x , y , width , height);
            var rectWidth = Value/Max * width;
            var rectHeight = Value/Max * height;
            switch ( Orientation )
            {
                case ProgressBarOrientation.BottomToTop:
                    g.FillRectangle(BarColor , x , y + height - rectHeight , width , rectHeight);
                    break;
                case ProgressBarOrientation.LeftToRight:
                    g.FillRectangle(BarColor , x , y , rectWidth , height);
                    break;
                case ProgressBarOrientation.TopToBottom:
                    g.FillRectangle(BarColor , x , y , width , rectHeight);
                    break;
                case ProgressBarOrientation.RightToLeft:
                    g.FillRectangle(BarColor , x + width - rectWidth , y , rectWidth , height);
                    break;
            }

            g.DrawRectangle(BorderColor , x , y , width , height , BorderSize);
        }
    }

    public enum ProgressBarOrientation
    {
        LeftToRight = 0,
        RightToLeft = 1,
        TopToBottom = 2,
        BottomToTop = 3,
        Vertical = 2,
        Horizontal = 0,
        VerticalReverse = 1,
        HorizontalReverse = 3
    }

    public class TrackBarBase : ProgressBarRenderer, ICheckHoverController
    {
        public bool IsHover { get; set; } = false;
        public bool IsPressed { get; set; } = false;
        public bool IsPressedRight { get; set; } = false;

        public event EventHandler<Point> CursorHover;
        public event EventHandler<ClickEvent> CursorClick;
        public event EventHandler<ClickEvent> CursorDown;
        public event EventHandler<ClickEvent> CursorUp;

        public void RaiseClick(Point location , bool left)
        {
            CursorClick?.Invoke(this , new ClickEvent() { IsLeftButton = left , Location = location });
        }

        public void RaiseCursorDown(Point location , bool left)
        {
            CursorDown?.Invoke(this , new ClickEvent() { IsLeftButton = left , Location = location });
        }

        public void RaiseCursorUp(Point location , bool left)
        {
            CursorUp?.Invoke(this , new ClickEvent() { IsLeftButton = left , Location = location });
        }

        public void RaiseHover(Point location)
        {
            CursorHover?.Invoke(this , location);
        }
    }
}
