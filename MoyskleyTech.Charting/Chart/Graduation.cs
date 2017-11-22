using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MoyskleyTech.Charting
{
    public abstract class Graduation:Plot
    {
        protected PlotValues pv;
        public bool SkipFirst
        {
            get; set;
        }
    }
   
    public abstract class HorizontalGraduation:Graduation { }
    public abstract class VerticalGraduation : Graduation { }
}
