using System;
using System.Collections.Generic;
using MoyskleyTech.ImageProcessing.Image;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MoyskleyTech.Charting
{
    public class IntelligentHorizontalGraduation : HorizontalGraduation
    {
        private Pixel barPixel;

        public event EventHandler<TextOutputEventArgs<double>> TextOutput;
        public IntelligentHorizontalGraduation(PlotValues pv)
        {
            this.pv = pv;
            BarPixel = Pixels.Wheat;
            MinWidthBetweenGraduation = 10;
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
                return pv.MaxX;
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
                return pv.MinX;
            }
        }

        public override double MinY
        {
            get
            {
                return 0;
            }
        }
        public int MinWidthBetweenGraduation
        {
            get; set;
        }
        public double MinWidthBetweenUnit { get; set; }

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

            double echelle = Math.Abs(MaxX / width);
            if ( echelle < 0.0001 )
                return;
            if ( echelle > long.MaxValue )
                return;
            Point bottomLeft = new Point(left, bottom-1);
            Point bottomRight = new Point(right, bottom-1);
            g.DrawLine(barPixel , bottomLeft , bottomRight);
            var unitCount =0;
            double nextGrad = 0;
            while ( nextGrad < width )
            {
                if ( unitCount != 0 || !SkipFirst )
                {
                    g.DrawLine(barPixel , left + ( int ) nextGrad , bottom , left + ( int ) nextGrad , bottom - 10);

                    var strOutput =unitCount.ToString();

                    TextOutputEventArgs<double> e= new TextOutputEventArgs<double>();
                    e.Input = nextGrad * echelle;
                    e.Output = strOutput;
                    if ( TextOutput != null )
                        TextOutput(this , e);

                    g.DrawString(e.Output , new FontSize(c.Font,c.FontSize) , barPixel , new Point(left + ( int ) nextGrad , bottom) , new StringFormat { LineAlignment = StringAlignment.Far });
                }
                var befGrad = nextGrad;
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
