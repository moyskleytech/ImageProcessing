using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LMST.Sportif.DLL.Windows.Controls
{
    public class FixedHorizontalGraduation : HorizontalGraduation
    {
        private Color barColor;
        private Brush barBrush;
        private Pen barPen;
        public event EventHandler<TextOutputEventArgs<double>> TextOutput;
        public FixedHorizontalGraduation(PlotValues pv)
        {
            this.pv = pv;
            BarColor = Color.Wheat;
            MinWidthBetweenGraduation = 10;
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
        double mx;
        public override double MaxX
        {
            get
            {
                return mx;
            }
        }
        public int Min
        {
            get; set;
        }
        public double FixedMax
        {
            get
            { return mx / MinWidthBetweenUnit; }
            set
            {
                mx = value * MinWidthBetweenUnit;
            }
        }

        public override double MaxY
        {
            get
            {
                return pv?.MaxY ?? 0;
            }
        }

        public override double MinX
        {
            get
            {
                return pv?.MinX ?? 0;
            }
        }

        public override double MinY
        {
            get
            {
                return pv?.MinY ?? 0;
            }
        }
        public int MinWidthBetweenGraduation
        {
            get; set;
        }
        public double MinWidthBetweenUnit { get; set; }
        public PositionGraduation Position
        {
            get; set;
        } = PositionGraduation.Left;
        public override void Draw(PlotChart c , Graphics g , int w , int h)
        {
            if ( !Visible )
                return;
            var height = h - Margin.Vertical;
            var width = w - Margin.Horizontal;
            var left = Margin.Left;
            var top = Margin.Top;
            var bottom = top+height;
            var right = left+width;
            g.SetClip(new Rectangle(left , top , width , height));

            double echelle = Math.Abs(mx /width);
            if ( echelle < 0.0001 )
                return;
            if ( echelle > long.MaxValue )
                return;

            if ( Position == PositionGraduation.Left )
            {
                Point bottomLeft = new Point(left, bottom-1);
                Point bottomRight = new Point(right, bottom-1);
                g.DrawLine(barPen , bottomLeft , bottomRight);
            }
            else if ( Position == PositionGraduation.Right )
            {
                Point bottomLeft = new Point(left, top+1);
                Point bottomRight = new Point(right, top+1);
                g.DrawLine(barPen , bottomLeft , bottomRight);
            }
            else if ( Position == PositionGraduation.Center )
            {
                Point bottomLeft = new Point(left, top+height/2);
                Point bottomRight = new Point(right, top+height/2);
                g.DrawLine(barPen , bottomLeft , bottomRight);
            }




            var unitCount =Min;
            double nextGrad = 0;
            while ( nextGrad < width )
            {


                var strOutput =unitCount.ToString();

                TextOutputEventArgs<double> e= new TextOutputEventArgs<double>();
                e.Input = nextGrad * echelle;
                e.Output = strOutput;
                TextOutput?.Invoke(this , e);
                var mes = g.MeasureString(unitCount.ToString(),c.Font);
                if ( Position == PositionGraduation.Left )
                {
                    g.DrawLine(barPen , left + ( int ) nextGrad , bottom , left + ( int ) nextGrad , bottom - 10);
                    g.DrawString(e.Output , c.Font , barBrush , new Point(left + ( int ) nextGrad , bottom) , new StringFormat { LineAlignment = StringAlignment.Far });
                }
                else if ( Position == PositionGraduation.Right )
                {
                    g.DrawLine(barPen , left + ( int ) nextGrad , top , left + ( int ) nextGrad , top + 10);
                    g.DrawString(e.Output , c.Font , barBrush , new Point(left + ( int ) nextGrad , (int)(top+mes.Height)) , new StringFormat { LineAlignment = StringAlignment.Far });
                }
                else if ( Position == PositionGraduation.Center )
                {
                    int center=top+height/2;

                    g.DrawLine(barPen , left + ( int ) nextGrad , center+5 , left + ( int ) nextGrad , center - 5);
                    g.DrawString(e.Output , c.Font , barBrush , new Point(left + ( int ) nextGrad , center) , new StringFormat { LineAlignment = StringAlignment.Far });

                }
                var befGrad = nextGrad;
                if ( echelle > int.MaxValue ) return;
                while ( nextGrad < befGrad + MinWidthBetweenGraduation )
                {
                    var currentUnit = nextGrad * echelle;
                    var nextUnit = currentUnit + MinWidthBetweenUnit;
                    nextGrad = nextUnit / echelle;
                    unitCount++;
                }
            }
            g.ResetClip();
        }
    }
}
