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

    public class Line : Plot
    {
        public Line()
        {
            BarColor = Color.Blue;
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
        private Color fillColor;
        private Brush fillBrush;
        private Color barColor;
        private Brush barBrush;
        private Pen barPen;
        public LineStyle Style { get; set; } = LineStyle.Line;
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

                g.DrawLine(barPen , p1 , p2);

                if ( Style == LineStyle.FillBottom )
                {
                    g.FillPolygon(fillBrush , new Point[ ] { p1 , p2 , new Point(p2.X , 0) , new Point(p1.X , 0) });
                    g.DrawLine(barPen , p1 , p2);
                }
                else if ( Style == LineStyle.FillTop )
                {
                    g.FillPolygon(fillBrush , new Point[ ] { p1 , p2 , new Point(p2.X , ( int ) ( maxY * pixelPerPointY )) , new Point(p1.X , ( int ) ( maxY * pixelPerPointY )) });
                    g.DrawLine(barPen , p1 , p2);
                }

            }
            catch ( OverflowException o ) { o.Nada(); }
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
