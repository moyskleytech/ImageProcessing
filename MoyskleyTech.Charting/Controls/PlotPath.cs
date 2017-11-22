using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;

namespace LMST.Sportif.DLL.Windows.Controls
{
    public class PlotPath : Plot
    {
        private Brush barBrush;
        private Color barColor;
        private Pen barPen;
        public PlotPath()
        {
            Path = new Path();
        }
        public Brush BarBrush
        {
            get
            {
                return barBrush;
            }
            set
            {
                barBrush = value;
            }
        }
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

        public double MaxShown { get; set; } = double.MaxValue;

        public override double MaxX
        {
            get
            {
                return double.MinValue;
            }
        }

        public override double MaxY
        {
            get
            {
                return double.MinValue;
            }
        }

        public double MinShown { get; set; }

        public override double MinX
        {
            get
            {
                return 0;
            }
        }
        public bool Border { get; set; } = false;
        public override double MinY
        {
            get
            {
                return 0;
            }
        }
        public Path Path
        {
            get; set;
        }
        public override void Draw(PlotChart c , Graphics g , int w , int h)
        {
            var height = h - Margin.Vertical;
            var width = w - Margin.Horizontal;
            var left = Margin.Left;
            var top = Margin.Top;
            var bottom = top+height;
            var right = left+width;

            g.SetClip(new Rectangle(left , top , width , height));

            if ( !Visible )
                return;

            {
                if ( c.MaxY < 0 )
                    return;
                double pixelPerPointX=0;
                double pixelPerPointY = ( double )height / c.MaxY;
                if ( double.IsInfinity(pixelPerPointY) )
                    return;

                if ( MaxShown != double.MaxValue )
                {
                    pixelPerPointX = ( double ) width / ( MaxShown - MinShown );
                }
                else
                    pixelPerPointX = ( double ) width / ( c.MaxX );


                Brush b = barBrush;

                int count = Path.Values.Count();
                PointF[] tab = new PointF[count];
                for ( var i = 0 ; i < count ; i++ )
                {
                    var val = Path.values[i];
                    if ( double.IsNaN(val.Absisce) || double.IsNaN(val.Ordonee) )
                        continue;
                    if ( double.IsInfinity(val.Absisce) || double.IsInfinity(val.Ordonee) )
                        continue;



                    PointF p = new Point((int)((val.Absisce - MinShown) * pixelPerPointX )+left,top+ (int)(height - val.Ordonee * pixelPerPointY));
                    tab[i] = p;
                }

                g.FillPolygon(barBrush , tab);
                if ( Border )
                {
                    g.DrawPolygon(barPen , tab);
                    g.DrawPolygon(barPen , tab);
                    g.DrawPolygon(barPen , tab);
                    g.DrawPolygon(barPen , tab);
                }
                g.ResetClip();
            }
        }

    }
    public class Path
    {
        internal List<PointGraphique> values;
        public IEnumerable<PointGraphique> Values
        {
            get
            {
                return values;
            }
        }
        public void AddPoint(PointGraphique pg)
        {
            values.Add(pg);
        }
        public void RemovePoint(PointGraphique pg)
        {
            values.Remove(pg);
        }

        public Path() { values = new List<PointGraphique>(); }
    }
}
