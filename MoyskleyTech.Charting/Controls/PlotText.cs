using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LMST.Sportif.DLL.Windows.Controls
{
    public class PlotText : Plot
    {
        Color barcol;
        Brush barBrush;
        Color backcol= Color.Transparent;
        Brush backBrush;
        public ContentAlignment TextAlign
        {
            get; set;
        }
        public override Color BarColor
        {
            get
            {
                return barcol;
            }

            set
            {
                barcol = value;
                if ( barBrush != null )
                    barBrush.Dispose();
                barBrush = new SolidBrush(barcol);
            }
        } 
        public Color BackColor
        {
            get
            {
                return backcol;
            }

            set
            {
                backcol = value;
                if ( backBrush != null )
                    backBrush.Dispose();
                backBrush = new SolidBrush(backcol);
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
        public double Rotation
        {
            get; set;
        }
        public Font Font { get; set; } = new Font("Arial" , 12);
        public string Value { get; set; }
        public override void Draw(PlotChart c , Graphics g,int w,int h)
        {

            var height = h - Margin.Vertical;
            var width = w - Margin.Horizontal;
            var left = Margin.Left;
            var top = Margin.Top;
            var bottom = top+height;
            var right = left+width;

            if ( Visible )
            {
                var measured = g.MeasureString(Value,Font);
                int x=0,y=0;
                //Y
                switch ( TextAlign )
                {
                    case ContentAlignment.BottomCenter:
                    case ContentAlignment.BottomLeft:
                    case ContentAlignment.BottomRight:
                        y = ( int ) ( top + height - measured.Height );
                        break;
                    case ContentAlignment.MiddleCenter:
                    case ContentAlignment.MiddleLeft:
                    case ContentAlignment.MiddleRight:
                        y = ( int ) ( top + ( height - measured.Height ) / 2 );
                        break;
                    case ContentAlignment.TopCenter:
                    case ContentAlignment.TopLeft:
                    case ContentAlignment.TopRight:
                        y = top;
                        break;
                }
                //X
                switch ( TextAlign )
                {
                    case ContentAlignment.BottomLeft:
                    case ContentAlignment.MiddleLeft:
                    case ContentAlignment.TopLeft:
                        x = left;
                        break;
                    case ContentAlignment.BottomCenter:
                    case ContentAlignment.MiddleCenter:
                    case ContentAlignment.TopCenter:
                        x = ( int ) ( left + ( width - measured.Width ) / 2 );
                        break;
                    case ContentAlignment.BottomRight:
                    case ContentAlignment.MiddleRight:
                    case ContentAlignment.TopRight:
                        x = ( int ) ( left + width - measured.Width );
                        break;
                }
                g.TranslateTransform(x , y);
                g.RotateTransform(( float ) Rotation);

               
                if(backBrush!=null)
                    g.FillRectangle(backBrush , -2 , -2 , measured.Width+4 , measured.Height+4);
                g.DrawString(Value , Font , barBrush , 0 , 0);

                g.RotateTransform(( float ) -Rotation);
                g.TranslateTransform(-x , -y);
            }

        }
    }
}
