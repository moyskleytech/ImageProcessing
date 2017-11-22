using System;
using System.Collections.Generic;
using MoyskleyTech.ImageProcessing.Image;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MoyskleyTech.Charting
{
    public class IntelligentVerticalGraduation : VerticalGraduation
    {

        private Pixel barPixel;
        public double HeightMultiplier
        {
            get; set;
        } = 1.5;
        public IntelligentVerticalGraduation(PlotValues pv)
        {
            this.pv = pv;
            BarPixel = Pixels.Wheat;
            MinHeightBetweenGraduation = 10;
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
                return pv.MaxY * HeightMultiplier;
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
                return pv.MinY;
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

            double echelle = Math.Abs(MaxY / height);
            if ( echelle < 0.0001 )
                return;
            if ( echelle > 1000 )
                return;
            if ( Position == PositionGraduation.Left )
            {
                Point bottomLeft = new Point(Margin.Left, bottom);
                Point topLeft = new Point(Margin.Left, 0);
                g.DrawLine(barPixel , bottomLeft , topLeft);
            }
            else if ( Position == PositionGraduation.Right )
            {
                Point bottomRight = new Point(w-1, bottom);
                Point topRight = new Point(w-1, 0);
                g.DrawLine(barPixel , bottomRight , topRight);
            }

            var unitCount = 0;
            double nextGrad = bottom;
            while ( nextGrad > 0 )
            {
                if ( unitCount != 0 || !SkipFirst )
                {
                    var mes = g.MeasureString(unitCount.ToString(),c.Font,c.FontSize);
                    if ( Position == PositionGraduation.Left )
                    {
                        g.DrawLine(barPixel , left , ( int ) nextGrad , left + 10 , ( int ) nextGrad);
                        if ( nextGrad - mes.Height > 0 )
                            g.DrawString(unitCount.ToString() , new FontSize(c.Font,c.FontSize) , barPixel , new Point(left , ( int ) nextGrad) , new StringFormat { LineAlignment = StringAlignment.Far });
                    }
                    else if ( Position == PositionGraduation.Right )
                    {
                        g.DrawLine(barPixel , w - Margin.Right , ( int ) nextGrad , w - Margin.Right - 10 , ( int ) nextGrad);
                        if ( nextGrad - mes.Height > 0 )
                            g.DrawString(unitCount.ToString() , new FontSize(c.Font,c.FontSize) , barPixel , new Point(w - Margin.Right , ( int ) nextGrad) , new StringFormat { LineAlignment = StringAlignment.Far , Alignment = StringAlignment.Far });
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
