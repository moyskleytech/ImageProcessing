using MoyskleyTech.ImageProcessing.Image;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MoyskleyTech.ImageProcessing.Drawing3D
{
    public interface IGraphics3D
    {
        void RenderPoint(Point3D point , Pixel color , double size);
        void RenderLine(Point3D point1, Point3D point2 , Pixel color , double size);
        void RenderText(Point3D point , string text , Pixel color , double size);
        void RenderPolygon(Point3D[] points ,  Pixel color);
    }
}
