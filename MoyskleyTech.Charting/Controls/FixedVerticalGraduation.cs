using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LMST.Sportif.DLL.Windows.Controls
{
    public class FixedVerticalGraduation : VerticalGraduation
    {
        private Color barColor;
        private Brush barBrush;
        private Pen barPen;
        private double fixedMax=1;
        public double FixedMax
        {
            get { return fixedMax; }
            set { if ( value > 0 ) fixedMax = value; }
        }
        public FixedVerticalGraduation(PlotValues pv)
        {
            this.pv = pv;
            BarColor = Color.Wheat;
            MinHeightBetweenGraduation = 10;
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
        public int Min
        {
            get; set;
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
                return fixedMax;
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
                return pv?.MinY ?? 0;
            }
        }
        public int MinHeightBetweenGraduation
        {
            get; set;
        }
        public double MinHeightBetweenUnit { get; set; }
        public PositionGraduation Position
        {
            get; set;
        }
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

            double echelle = Math.Abs(fixedMax / height);
            if ( echelle < 0.0001 )
                return;
            if ( echelle > 1000 )
                return;
            if ( Position == PositionGraduation.Left )
            {
                Point bottomLeft = new Point(Margin.Left, bottom);
                Point topLeft = new Point(Margin.Left, 0);
                g.DrawLine(barPen , bottomLeft , topLeft);
            }
            else if ( Position == PositionGraduation.Right )
            {
                Point bottomRight = new Point(w-Margin.Right, bottom);
                Point topRight = new Point(w-Margin.Right, 0);
                g.DrawLine(barPen , bottomRight , topRight);
            }
            else if ( Position == PositionGraduation.Center )
            {
                Point bottomCenter = new Point((width)/2+Margin.Left, bottom);
                Point topCenter = new Point((width)/2+Margin.Left, 0);
                g.DrawLine(barPen , bottomCenter , topCenter);
            }

            var unitCount = Min;
            double nextGrad = bottom;
            while ( nextGrad > 0 )
            {
                if ( unitCount != Min || !SkipFirst )
                {
                    var mes = g.MeasureString(unitCount.ToString(),c.Font);
                    if ( Position == PositionGraduation.Left )
                    {
                        g.DrawLine(barPen , left , ( int ) nextGrad , left + 10 , ( int ) nextGrad);
                        if ( nextGrad - mes.Height > 0 )
                            g.DrawString(unitCount.ToString() , c.Font , barBrush , new Point(left , ( int ) nextGrad) , new StringFormat { LineAlignment = StringAlignment.Far });
                    }
                    else if ( Position == PositionGraduation.Right )
                    {
                        g.DrawLine(barPen , w - Margin.Right , ( int ) nextGrad , w - Margin.Right - 10 , ( int ) nextGrad);
                        if ( nextGrad - mes.Height > 0 )
                            g.DrawString(unitCount.ToString() , c.Font , barBrush , new Point(w - Margin.Right , ( int ) nextGrad) , new StringFormat { LineAlignment = StringAlignment.Far , Alignment = StringAlignment.Far });
                    }
                    else if ( Position == PositionGraduation.Center )
                    {
                        g.DrawLine(barPen , ( width ) / 2 + Margin.Left-5 , ( int ) nextGrad , ( width ) / 2 + Margin.Left+5 , ( int ) nextGrad);
                        if ( nextGrad - mes.Height > 0 )
                            g.DrawString(unitCount.ToString() , c.Font , barBrush , new Point(( width ) / 2 + Margin.Left , ( int ) nextGrad) , new StringFormat { LineAlignment = StringAlignment.Far });
                    }
                }
                var befGrad = nextGrad;
                while ( nextGrad > befGrad - MinHeightBetweenGraduation )
                {
                    var currentUnit = nextGrad * echelle;
                    var nextUnit = currentUnit - MinHeightBetweenUnit;
                    nextGrad = nextUnit / echelle;
                    unitCount++;
                }
            }
            g.ResetClip();
        }
    }
}
