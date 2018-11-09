using MoyskleyTech.ImageProcessing.Image;
using MoyskleyTech.Mathematics;
using MoyskleyTech.Mathematics.Statistics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MoyskleyTech.Charting.Charting2D
{
    public class HorizontalAxis : IChartItem
    {
        public double Min { get; set; } = 0;
        public double Max { get; set; } = 1;
        public double MinStep { get; set; } = 1;
        public double MinWidthBetweenGraduation { get; set; } = 50;

        public Func<double , string> Namer { get; set; }
        public Pixel BarColor { get; set; }
        public Pixel ForeColor { get; set; }

        public bool HasWidth => true;
        public bool HasHeight => false;

        public double MinX => Min;
        public double MinY => throw new NotImplementedException();

        public double MaxX => Max;
        public double MaxY => throw new NotImplementedException();

        public HorizontalAxis()
        {
            Namer = (x) => x.ToString();
        }

        public void Draw(Graphics<Pixel> g , Rectangle bounds , DescriptiveStatistics stats)
        {
            var origin = stats.NormalizeTo01(new Coordinate[ ] { new Coordinate(0 , 0) }).First();
            var originY = bounds.Height - bounds.Height * origin.Y;

            g.DrawLine(BarColor , 0 , originY , bounds.Width , originY);
        }

        public void Adjust(DescriptiveStatistics stats)
        {
            Max = Math.Ceiling(stats.MaxX);
            Min = Math.Floor(stats.MinX);
        }
    }
}
