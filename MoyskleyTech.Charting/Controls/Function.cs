using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;
using System.ComponentModel;
using LMST.Sportif.DLL.Portable.Helper;
namespace LMST.Sportif.DLL.Windows.Controls
{

    public class FunctionPlot : Plot
    {
        public FunctionPlot()
        {
            BarColor = Color.Blue;
        }
        private Color barColor;
        private Brush barBrush;
        private Pen barPen;

        private Color fillColor;
        private Brush fillBrush;
        [Category("Line Appearance")]
        public FunctionStyle Style
        {
            get; set;
        } = FunctionStyle.Line;
        [Category("Line Appearance")]
        public Single LineThickness
        {
            get
            {
                return barPen.Width;
            }
            set
            {
                barPen.Width = value;
            }
        }
        [Category("Line Appearance")]
        public override Color BarColor
        {
            get
            {
                return barColor;
            }
            set
            {
                barColor = value;
                if ( barBrush != null )
                    barBrush.Dispose();
                barBrush = new SolidBrush(barColor);
                if ( barPen == null )
                    barPen = new Pen(value);
                barPen.Color = value;

            }
        }
        [Category("Line Appearance")]
        public Color FillColor
        {
            get
            {
                return fillColor;
            }
            set
            {
                fillColor = value;
                if ( fillBrush != null )
                    fillBrush.Dispose();
                fillBrush = new SolidBrush(fillColor);
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
            if ( !Visible )
                return;
            double maxX = Math.Min(c.MaxX,MathMaxX);
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

            double x=MathMinX;
            g.TranslateTransform(0 , height);
            g.ScaleTransform(1 , -1);
            try
            {
                while ( x < maxX && pixelPerPointX > 10e-11 )
                {
                    Point p1 = new Point((int)(x*pixelPerPointX), (int)(MathFunction(x)*pixelPerPointY));
                    Point p2 = new Point((int)((x+DX)*pixelPerPointX), (int)(MathFunction(x+DX)*pixelPerPointY));
                    switch ( Style )
                    {
                        case FunctionStyle.Line:
                            {
                                g.DrawLine(barPen , p1 , p2);
                            }
                            break;
                        case FunctionStyle.FillBottom:
                            {
                                g.FillPolygon(fillBrush , new Point[ ] { p1 , p2 , new Point(p2.X , 0) , new Point(p1.X , 0) });
                                g.DrawLine(barPen , p1 , p2);
                            }
                            break;
                        case FunctionStyle.FillTop:
                            {
                                g.FillPolygon(fillBrush , new Point[ ] { p1 , p2 , new Point(p2.X , ( int ) ( maxY * pixelPerPointY )) , new Point(p1.X , ( int ) ( maxY * pixelPerPointY )) });
                                g.DrawLine(barPen , p1 , p2);
                            }
                            break;
                    }

                    x += DX;
                }
            }
            catch ( OverflowException o ) { o.Nada(); }
            g.ScaleTransform(1 , -1);
            g.TranslateTransform(0 , -height);
            g.ResetClip();
        }
        public enum FunctionStyle
        {
            Line,
            FillBottom,
            FillTop
        }
    }
}
