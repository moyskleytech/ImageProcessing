using System;
using System.Collections.Generic;
using MoyskleyTech.ImageProcessing.Image;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MoyskleyTech.Charting
{
    public class PlotText : Plot
    {
        Pixel barPixel;
        Pixel backPixel= Pixels.Transparent;
        public ContentAlignment TextAlign
        {
            get; set;
        }
        public override Pixel BarPixel
        {
            get
            {
                return barPixel;
            }

            set
            {
                barPixel = value;
            }
        } 
        public Pixel BackPixel
        {
            get
            {
                return backPixel;
            }

            set
            {
                backPixel = value;
            }
        } 

        public override double MaxX
        {
            get
            {
                return 0;
            }
        }

        public override double MaxY
        {
            get
            {
                return 0;
            }
        }

        public override double MinX
        {
            get
            {
                return 0;
            }
        }

        public override double MinY
        {
            get
            {
                return 0;
            }
        }
        public double Rotation
        {
            get; set;
        }
        public Font Font { get; set; } = BaseFonts.Premia;
        public int FontSize { get; set; } = 1;
        public string Value { get; set; }
        public override void Draw(PlotChart c , Graphics g,int w,int h)
        {

            var height = h - Margin.Vertical;
            var width = w - Margin.Horizontal;
            var left = Margin.Left;
            var top = Margin.Top;
            var bottom = top+height;
            var right = left+width;

            if ( Visible )
            {
                var measured = g.MeasureString(Value,Font,FontSize);
                int x=0,y=0;
                //Y
                switch ( TextAlign )
                {
                    case ContentAlignment.BottomCenter:
                    case ContentAlignment.BottomLeft:
                    case ContentAlignment.BottomRight:
                        y = ( int ) ( top + height - measured.Height );
                        break;
                    case ContentAlignment.MiddleCenter:
                    case ContentAlignment.MiddleLeft:
                    case ContentAlignment.MiddleRight:
                        y = ( int ) ( top + ( height - measured.Height ) / 2 );
                        break;
                    case ContentAlignment.TopCenter:
                    case ContentAlignment.TopLeft:
                    case ContentAlignment.TopRight:
                        y = top;
                        break;
                }
                //X
                switch ( TextAlign )
                {
                    case ContentAlignment.BottomLeft:
                    case ContentAlignment.MiddleLeft:
                    case ContentAlignment.TopLeft:
                        x = left;
                        break;
                    case ContentAlignment.BottomCenter:
                    case ContentAlignment.MiddleCenter:
                    case ContentAlignment.TopCenter:
                        x = ( int ) ( left + ( width - measured.Width ) / 2 );
                        break;
                    case ContentAlignment.BottomRight:
                    case ContentAlignment.MiddleRight:
                    case ContentAlignment.TopRight:
                        x = ( int ) ( left + width - measured.Width );
                        break;
                }
                g.TranslateTransform(x , y);
                g.RotateTransform(( float ) Rotation);

                g.FillRectangle(backPixel , -2 , -2 , measured.Width+4 , measured.Height+4);

                g.DrawString(Value , barPixel , 0 , 0 , Font , FontSize);

                g.RotateTransform(( float ) -Rotation);
                g.TranslateTransform(-x , -y);
            }

        }
    }
}
