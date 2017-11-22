using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;
using System.Windows.Forms;

namespace LMST.Sportif.DLL.Windows.Controls
{
    public abstract class Plot
    {
        public Plot()
        {
            Visible = true;
        }
        public Padding Margin;
        public abstract double MaxX
        {
            get;
        }
        public abstract double MaxY
        {
            get;
        }
        public abstract double MinX
        {
            get;
        }
        public abstract double MinY
        {
            get;
        }
        public abstract Color BarColor
        {
            get; set;
        }
        public bool Visible { get; set; }


        public abstract void Draw(PlotChart plotChart , Graphics g , int w , int h);


    }
}
