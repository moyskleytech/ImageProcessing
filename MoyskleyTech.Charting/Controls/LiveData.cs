using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LMST.Sportif.DLL.Windows.Controls
{
    public class LiveData : Plot
    {
        Color barcol;
        Brush barBrush;
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
                if (barBrush != null)
                    barBrush.Dispose();
                barBrush = new SolidBrush(barcol);
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
                g.DrawString(Value, c.Font, barBrush,new Rectangle(0,0,w,h), sf);
        }
    }
}
