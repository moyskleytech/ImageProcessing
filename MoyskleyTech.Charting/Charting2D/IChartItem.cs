using MoyskleyTech.ImageProcessing.Image;
using MoyskleyTech.Mathematics.Statistics;

namespace MoyskleyTech.Charting.Charting2D
{
    public interface IChartItem
    {
        void Draw(Graphics<Pixel> g , Rectangle bounds , DescriptiveStatistics stats);
        bool HasWidth { get; }
        double MinX { get; }
        double MaxX { get; }
        bool HasHeight { get; }
        double MinY { get; }
        double MaxY { get; }
    }
}