using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MoyskleyTech.ImageProcessing.Image;
using System.ComponentModel;

namespace MoyskleyTech.Charting
{
    [Obsolete("Use Charting2D namespace",true)]
    public class FunctionPlot : Plot
    {
        public FunctionPlot()
        {
            BarPixel = Pixels.Blue;
        }
        private Pixel barPixel;

        private Pixel fillPixel;
       
        public FunctionStyle Style
        {
            get; set;
        } = FunctionStyle.Line;
        public Single LineThickness
        {
            get;set;
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
        public Pixel FillPixel
        {
            get
            {
                return fillPixel;
            }
            set
            {
                fillPixel = value;
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
        public Func<double , double> MathFunction
        {
            get; set;
        }
        public double DX
        {
            get; set;
        }
        public double ValueAt(double x)
        {
            return MathFunction(x);
        }
        public double MathMaxX
        {
            get; set;
        } = double.MaxValue;
        public double MathMinX
        {
            get; set;
        } = 0;
        public override void Draw(PlotChart c , Graphics g , int w , int h)
        {
            //if ( !Visible )
            //    return;
            //double maxX = Math.Min(c.MaxX,MathMaxX);
            //double maxY = c.MaxY;

            //var height = h - Margin.Vertical;
            //var width = w - Margin.Horizontal;
            //var left = Margin.Left;
            //var top = Margin.Top;
            //var bottom = top+height;
            //var right = left+width;

            //g.SetClip(new Rectangle(left , top , width , height));

            //double pixelPerPointX = (double)width / maxX;
            //if ( double.IsInfinity(pixelPerPointX) || double.IsNaN(pixelPerPointX) )
            //    pixelPerPointX = 0;
            //double pixelPerPointY = (double)height / maxY;
            //if ( double.IsInfinity(pixelPerPointY) || double.IsNaN(pixelPerPointY) )
            //    pixelPerPointY = 0;

            //double x=MathMinX;
            //g.TranslateTransform(0 , height);
            //g.ScaleTransform(1 , -1);
            //try
            //{
            //    while ( x < maxX && pixelPerPointX > 10e-11 )
            //    {
            //        Point p1 = new Point((int)(x*pixelPerPointX), (int)(MathFunction(x)*pixelPerPointY));
            //        Point p2 = new Point((int)((x+DX)*pixelPerPointX), (int)(MathFunction(x+DX)*pixelPerPointY));
            //        switch ( Style )
            //        {
            //            case FunctionStyle.Line:
            //                {
            //                    g.DrawLine(barPixel , p1 , p2 , ( int ) LineThickness);
            //                }
            //                break;
            //            case FunctionStyle.FillBottom:
            //                {
            //                    g.FillPolygon(fillPixel , p1 , p2 , new Point(p2.X , 0) , new Point(p1.X , 0));
            //                    g.DrawLine(barPixel , p1 , p2 , ( int ) LineThickness);
            //                }
            //                break;
            //            case FunctionStyle.FillTop:
            //                {
            //                    g.FillPolygon(fillPixel , p1 , p2 , new Point(p2.X , ( int ) ( maxY * pixelPerPointY )) , new Point(p1.X , ( int ) ( maxY * pixelPerPointY )));
            //                    g.DrawLine(barPixel , p1 , p2 , ( int ) LineThickness);
            //                }
            //                break;
            //        }

            //        x += DX;
            //    }
            //}
            //catch ( OverflowException ) { }
            //g.ScaleTransform(1 , -1);
            //g.TranslateTransform(0 , -height);
            //g.ResetClip();
        }
        public enum FunctionStyle
        {
            Line,
            FillBottom,
            FillTop
        }
    }
}
