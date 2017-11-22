using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MoyskleyTech.ImageProcessing.Image;
using System.ComponentModel;

namespace MoyskleyTech.Charting
{

    public class Line : Plot
    {
        public Line()
        {
            BarPixel = Pixels.Blue;
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
        private Pixel fillPixel;
        private Pixel barPixel;
        public LineStyle Style { get; set; } = LineStyle.Line;
        public Single LineThickness
        {
            get; set;
        } = 0;
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
        public double Pente { get; set; }
        public double OrdonneeALOrigine { get; set; }
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
        public double ValueAt(double x)
        {
            return x * Pente + OrdonneeALOrigine;
        }
        public override void Draw(PlotChart c , Graphics g , int w , int h)
        {
            if ( !Visible )
                return;
            double maxX = c.MaxX;
            double maxY = c.MaxY;

            var height = h - Margin.Vertical;
            var width = w - Margin.Horizontal;
            var left = Margin.Left;
            var top = Margin.Top;
            var bottom = top+height;
            var right = left+width;

            g.SetClip(new Rectangle(left , top , width , height));

            double pixelPerPointX = (double)width / maxX;
            if ( double.IsInfinity(pixelPerPointX) || double.IsNaN(pixelPerPointX) )
                pixelPerPointX = 0;
            double pixelPerPointY = (double)height / maxY;
            if ( double.IsInfinity(pixelPerPointY) || double.IsNaN(pixelPerPointY) )
                pixelPerPointY = 0;
            if ( double.IsNaN(OrdonneeALOrigine) || double.IsInfinity(OrdonneeALOrigine) )
                return;

            try
            {
                Point p1 = new Point(Margin.Left, (int)(height - (pixelPerPointY * OrdonneeALOrigine)));
                Point p2 = new Point(w - Margin.Right, (int)(height - (pixelPerPointY * (Pente * maxX + OrdonneeALOrigine))));

                g.DrawLine(barPixel , p1 , p2,(int)LineThickness);

                if ( Style == LineStyle.FillBottom )
                {
                    g.FillPolygon(fillPixel , new PointF[ ] { p1 , p2 , new Point(p2.X , 0) , new Point(p1.X , 0) });
                    g.DrawLine(barPixel , p1 , p2 , ( int ) LineThickness);
                }
                else if ( Style == LineStyle.FillTop )
                {
                    g.FillPolygon(fillPixel , new PointF[ ] { p1 , p2 , new Point(p2.X , ( int ) ( maxY * pixelPerPointY )) , new Point(p1.X , ( int ) ( maxY * pixelPerPointY )) });
                    g.DrawLine(barPixel , p1 , p2 , ( int ) LineThickness);
                }

            }
            catch ( OverflowException o ) { }
            g.ResetClip();
        }
        public enum LineStyle
        {
            Line,
            FillBottom,
            FillTop
        }
    }
}
