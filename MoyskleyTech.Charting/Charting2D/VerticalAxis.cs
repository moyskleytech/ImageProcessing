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
    public class VerticalAxis : IChartItem
    {
        public double Min { get; set; } = 0;
        public double Max { get; set; } = 1;
        public double MinStep { get; set; } = 1;
        public double MinWidthBetweenGraduation { get; set; } = 50;

        public Func<double , string> Namer { get; set; }
        public Pixel BarColor { get; set; }
        public Pixel ForeColor { get; set; }

        public bool HasWidth => false;
        public bool HasHeight => true;

        public double MinX => Min;
        public double MinY => Min;

        public double MaxX => Max;
        public double MaxY => Max;

        public VerticalAxis()
        {
            Namer = (x) => x.ToString();
        }

        public void Draw(Graphics<Pixel> g , Rectangle bounds , DescriptiveStatistics stats)
        {
            var origin = stats.NormalizeTo01(new Coordinate[ ] { new Coordinate(0 , 0) }).First();
            var originX = bounds.Width * origin.X;

            g.DrawLine(BarColor , originX , 0 , originX , 0);
        }

        public void Adjust(DescriptiveStatistics stats)
        {
            Max = Math.Ceiling(stats.MaxY);
            Min = Math.Floor(stats.MinY);
        }
    }
}
