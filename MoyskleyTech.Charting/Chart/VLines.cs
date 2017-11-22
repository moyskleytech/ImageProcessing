using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MoyskleyTech.ImageProcessing.Image;
using System.ComponentModel;

namespace MoyskleyTech.Charting
{

    public class VLines : Plot
    {
        public VLines()
        {
            BarPixel = Pixels.Blue;
        }
        private Pixel barPixel;

        private Pixel fillPixel;
        public FunctionStyle Style
        {
            get; set;
        } = FunctionStyle.Line;

        public LinkedList<double> XPos
        {
            get; set;
        } = new LinkedList<double>();

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
          
            double x=MathMinX;
            try
            {
                var node = XPos.First;
                while ( node != null )
                {
                    g.DrawLine(barPixel , (float)(node.Value * pixelPerPointX) , top , (float)(node.Value * pixelPerPointX) , bottom,(int) LineThickness);
                    node = node.Next;
                }
            }
            catch ( OverflowException o ) {  }
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
