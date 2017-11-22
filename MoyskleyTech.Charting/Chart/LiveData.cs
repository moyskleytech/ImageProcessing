using System;
using System.Collections.Generic;
using MoyskleyTech.ImageProcessing.Image;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MoyskleyTech.Charting
{
    public class LiveData : Plot
    {
        Pixel barcol;
        public ContentAlignment TextAlign
        {
            get; set;
        }
        public override Pixel BarPixel
        {
            get
            {
                return barcol;
            }

            set
            {
                barcol = value;
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
       
        public string Value { get; set; }
        public override void Draw(PlotChart c, Graphics g , int w , int h)
        {
            StringFormat sf = new StringFormat ();
            Int32 lNum =  (Int32)Math.Log((Double)this.TextAlign, 2);

         
            sf.LineAlignment = ( StringAlignment ) ( lNum / 4 );
            sf.Alignment = ( StringAlignment ) ( lNum % 4 );


            if (Visible) 
                g.DrawString(Value, new FontSize(c.Font,c.FontSize), BarPixel,new Point(0,0), sf);
        }
    }
}
