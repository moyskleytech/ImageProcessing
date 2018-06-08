using MoyskleyTech.ImageProcessing.Image;
using MoyskleyTech.ImageProcessing.Image.SpecializedBrushes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MoyskleyTech.ImageProcessing.Form
{
    public class CheckBoxBase : IRenderer, ICheckHoverController
    {
        public Bitmap BackgroundImage { get; set; } = null;
        public Brush<Pixel> CheckColor { get; set; } = ( Brush ) Pixels.Black;
        public Brush<Pixel> BackColor { get; set; } = ( Brush ) Pixels.LightGray;
        public Brush<Pixel> HoverColor { get; set; } = ( Brush ) Pixels.LightSkyBlue;
        public Brush<Pixel> ClickedColor { get; set; } = ( Brush ) Pixels.SkyBlue;
        public bool Round { get; set; } = false;
        public int BorderSize { get; set; } = 1;
        public int CheckSize { get; set; } = 1;
        public Brush<Pixel> BorderColor { get; set; } = ( Brush ) Pixels.Black;
        public bool IsHover { get; set; } = false;
        public bool IsPressed { get; set; } = false;
        public bool IsPressedRight { get; set; } = false;
        public bool? IsChecked { get; set; } = false;
        public bool IsReadOnly { get; set; }
        public bool IsRadio { get; set; } = false;
        public event EventHandler<Point> CursorHover;
        public event EventHandler<ClickEvent> CursorClick;
        public event EventHandler<ClickEvent> CursorDown;
        public event EventHandler<ClickEvent> CursorUp;
      

        public void RaiseClick(Point location , bool left)
        {
            if ( !IsReadOnly )
            {
                if ( !IsRadio )
                {
                    if ( IsChecked == true )
                        IsChecked = false;
                    else
                        IsChecked = true;
                }
            }
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
        public virtual void Render(Graphics<Pixel> g , int x , int y , int width , int height)
        {
            var back = BackColor;
            if ( IsHover )
                back = HoverColor;
            if ( IsPressed )
                back = ClickedColor;

            PointF[] backgroundPolygon = null;
            PointF[] checkPolygon = null;
            if ( Round )
                backgroundPolygon = GraphicsHelper.GetCirclePolygon(x + width / 2 , y + height / 2 , Math.Min(width / 2 , height / 2));
            else
                backgroundPolygon = GraphicsHelper.GetRoundedRectanglePolygon(new RectangleF(x , y , width , height) , 3);
            if ( Round )
                checkPolygon = GraphicsHelper.GetCirclePolygon(x + width / 2 , y + height / 2 , Math.Min(width / 2 , height / 2) - 4);
            else
                checkPolygon = GraphicsHelper.GetRoundedRectanglePolygon(new RectangleF(x + 4 , y + 4 , width - 8 , height - 8) , 3);

            g.FillPolygon(back , backgroundPolygon);
            var state = g.SaveClipState();
            g.ResetClip();
            g.SetClip(backgroundPolygon);
            if ( BackgroundImage != null )
            {
                var backgroundImageBounds = new Rectangle(x,y,width,height);
                Image<Pixel> rightSizeBackground= BackgroundImage.Resize(backgroundImageBounds.Width,backgroundImageBounds.Height, ScalingMode.AverageInterpolate);
                g.DrawImage(rightSizeBackground , x , y);
                rightSizeBackground.Dispose();
            }
            if ( IsChecked == true )
                g.DrawPath(CheckColor , CheckSize , new PointF(x + width / 4 , y + height / 2) , new PointF(x + width / 2 , y + height * .75) , new PointF(x + width * .75 , y));
            if ( IsChecked == null )
                g.FillPolygon(CheckColor , checkPolygon);

            g.RestoreClipState(state);
            g.DrawPolygon(BorderColor , BorderSize , backgroundPolygon);

        }
    }
}
