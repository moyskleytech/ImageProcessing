using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MoyskleyTech.Charting
{
    [Obsolete("Use Charting2D namespace" , true)]
    public abstract class Graduation:Plot
    {
        protected PlotValues pv;
        public bool SkipFirst
        {
            get; set;
        }
    }
    [Obsolete("Use Charting2D namespace" , true)]
    public abstract class HorizontalGraduation:Graduation { }
    [Obsolete("Use Charting2D namespace" , true)]
    public abstract class VerticalGraduation : Graduation { }
}
