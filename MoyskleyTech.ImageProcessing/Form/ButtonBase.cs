using MoyskleyTech.ImageProcessing.Image;
using MoyskleyTech.ImageProcessing.Image.SpecializedBrushes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MoyskleyTech.ImageProcessing.Form
{
    public class ButtonBase:IRenderer, ICheckHoverController
    {
        public string Text { get; set; } = "button";
        public Bitmap BackgroundImage { get; set; } = null;
        public Bitmap Icon { get; set; } = null;
        public FontSizeF Font { get; set; } = null;
        public StringFormat TextAlign { get; set; } = new StringFormat() { Alignment = StringAlignment.Center , LineAlignment = StringAlignment.Center, EllipsisMode= EllipsisMode.Word };
        public Brush<Pixel> ForeColor { get; set; } = ( Brush ) Pixels.Black;
        public Pixel BackColor { get; set; } = Pixels.LightGray;
        public Pixel HoverColor { get; set; } = Pixels.LightSkyBlue;
        public Pixel ClickedColor { get; set; } = Pixels.LightSkyBlue;
        public double BorderRadius { get; set; } = 5;
        public int BorderSize { get; set; } = 1;
        public Brush<Pixel> BorderColor { get; set; } = ( Brush ) Pixels.Black;
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

        public virtual void Render(Graphics<Pixel> g ,int x,int y, int width , int height)
        {
            LightenBrush lighten = new LightenBrush((Brush)BackColor,0.1);
            DarkenBrush darken = new DarkenBrush((Brush)BackColor,0.1);
            var color1 = lighten.GetColor(0,0);
            var color2 = darken.GetColor(0,0);

            if ( IsHover )
                color1 = HoverColor;
            if ( IsPressed )
                color1 = color2 = ClickedColor;

            LinearGradientBrush linearGradient = new LinearGradientBrush(
                new Point(x,y),color1,
                new Point(x,y+height),color2);

            g.FillRoundedRectangle(linearGradient , x , y , width , height , BorderRadius);
            g.DrawRoundedRectangle(BorderColor , BorderSize, x , y , width , height , BorderRadius);

            if ( BackgroundImage != null )
            {
                var backgroundImageBounds = new Rectangle((int)(x+BorderRadius),(int)(y+BorderRadius),(int)(width-2*BorderRadius),(int)(height-2*BorderRadius));
                Image<Pixel> rightSizeBackground= BackgroundImage.Resize(backgroundImageBounds.Width,backgroundImageBounds.Height, ScalingMode.AverageInterpolate);
                g.DrawImage(rightSizeBackground , backgroundImageBounds.X,backgroundImageBounds.Y);
                rightSizeBackground.Dispose();
            }
            var textBound = new Rectangle(x,y,width,height);
            if ( Icon != null )
            {
                var iconBound = new Rectangle((int)(x+BorderRadius),(int)(y+BorderRadius),Icon.Width,Icon.Height);
                textBound.X += iconBound.Width;
                textBound.Width -= iconBound.Width;

                Image<Pixel> rightSizeIfon= Icon.Resize(iconBound.Width,iconBound.Height, ScalingMode.AverageInterpolate);
                g.DrawImage(rightSizeIfon , iconBound.X , iconBound.Y);
                rightSizeIfon.Dispose();
            }
            if ( Font == null )
                Font = new FontSizeF(BaseFonts.Premia , 1);

            g.DrawString(Text , Font , ForeColor , textBound,TextAlign);
        }
    }
}
