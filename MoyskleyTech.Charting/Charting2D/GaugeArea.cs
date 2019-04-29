using MoyskleyTech.ImageProcessing.Image;

namespace MoyskleyTech.Charting.Charting2D
{
    public class GaugeArea<R> where R : unmanaged
    {
        public double Minimum { get; set; }
        public double Maximum { get; set; }
        public Brush<R> Fill { get; set; }
        public R Border { get; set; }

        public double InsideProportion { get; set; } = 0.8;
        public double OutsideProportion { get; set; } = 1;
    }
}