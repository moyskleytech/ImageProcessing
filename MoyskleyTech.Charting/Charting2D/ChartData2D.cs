using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MoyskleyTech.Charting.Charting2D
{
    public class ChartData2D
    {
        public object AdditionnalData
        {
            get; set;
        }
        public ChartData2D(double x, double y)
        {
            X = x;
            Y = y;
        }
        public double X
        {
            get;
            set;
        }
        public double Y
        {
            get;
            set;
        }
    }
}
