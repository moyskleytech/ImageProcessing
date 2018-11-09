using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MoyskleyTech.Charting.Charting3D
{
    public class ChartData3D
    {
        public object AdditionnalData
        {
            get; set;
        }
        public ChartData3D(double x, double y,double z)
        {
            X = x;
            Y = y;
            Z = z;
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
        public double Z
        {
            get;
            set;
        }
    }
}
