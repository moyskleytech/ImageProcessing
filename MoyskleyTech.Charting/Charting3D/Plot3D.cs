using MoyskleyTech.ImageProcessing.Drawing3D;
using MoyskleyTech.ImageProcessing.Image;
using MoyskleyTech.Mathematics.Statistics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MoyskleyTech.Charting.Charting3D
{
    public class Plot3D : IObject3D
    {
        public List<ChartData3D> Data { get; set; } = new List<ChartData3D>();
        public Pixel PointColor { get; set; } = Pixels.Black;
        public double PointSize { get; set; } = 2;
        public Plot3DStyle Style { get; set; } = Plot3DStyle.Scattered;

        public void Render(IGraphics3D viewport3D)
        {
            var statsX = DescriptiveStatistics.From1D(from x in Data select x.X);
            var statsY = DescriptiveStatistics.From1D(from x in Data select x.Y);
            var statsZ = DescriptiveStatistics.From1D(from x in Data select x.Z);

            var maxX = Math.Max(Math.Abs(statsX.MinY), Math.Abs(statsX.MaxY));
            var maxY = Math.Max(Math.Abs(statsY.MinY), Math.Abs(statsY.MaxY));
            var maxZ = Math.Max(Math.Abs(statsZ.MinY), Math.Abs(statsZ.MaxY));

            ChartData3D previous=null;
            foreach ( var data in Data )
            {
                if ( Style == Plot3DStyle.Linked && previous != null )
                    viewport3D.RenderLine(new Point3D(previous.X / maxX , previous.Y / maxY , previous.Z / maxZ) , new Point3D(data.X / maxX , data.Y / maxY , data.Z / maxZ) , PointColor , PointSize);
                else if(Style== Plot3DStyle.Scattered)
                    viewport3D.RenderPoint(new Point3D(data.X / maxX , data.Y / maxY , data.Z / maxZ) , PointColor , PointSize);
                previous = data;
            }
        }
    }
    public enum Plot3DStyle
    {
        Scattered,
        Linked
    }
}
