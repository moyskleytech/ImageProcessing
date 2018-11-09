using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MoyskleyTech.Charting.Charting2D
{
    public class Chart
    {
        public HorizontalAxis HorizontalAxis { get;private set; }

        public Chart()
        {
            HorizontalAxis = new HorizontalAxis();
        }


        public void SetHorizontalAxis(HorizontalAxis horizontalAxis)
        {
            HorizontalAxis = horizontalAxis;
        }
    }
}
