using MoyskleyTech.ImageProcessing.Image;
using MoyskleyTech.Mathematics.Tools;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MoyskleyTech.ImageProcessing.Drawing3D
{
    public class Image3D<T> : IObject3D
        where T : unmanaged
    {
        public ImageProxy<T> Image { get; set; }
        public Point3D[ ] Corners { get; set; }
        public void Render(IGraphics3D viewport3D)
        {
            var aspx = Image.As<Pixel>();
            double width = Image.Width, height= Image.Height;
            for ( var x = 0; x < width; x++ )
            {
                for ( var y = 0; y < height; y++ )
                {
                    var pts = new Point3D[]{
                        GetP3D(x-0.5,y-0.5,width,height),
                        GetP3D(x-0.5,y+0.5,width,height),
                        GetP3D(x+0.5,y+0.5,width,height),
                        GetP3D(x+0.5,y-0.5,width,height)
                    };
                    //var width01 = x/width;
                    //var height01 = y/height;
                    //var xn=BilinearInterpolator.Interpolate((x2 , y2) => this[x2, y2].X , width01 , height01);
                    //var yn=BilinearInterpolator.Interpolate((x2 , y2) => this[x2 , y2].Y , width01 , height01);
                    //var zn=BilinearInterpolator.Interpolate((x2 , y2) => this[x2 , y2].Z , width01 , height01);
                    viewport3D.RenderPolygon(pts , aspx[x , y]);
                    //viewport3D.RenderPoint(new Point3D(xn , yn , zn) , aspx[x , y] , 1);
                }
            }

            
        }

        private Point3D GetP3D(double x , double y , double width , double height)
        {
            var width01 = x/width;
            var height01 = y/height;
            var xn=BilinearInterpolator.Interpolate((x2 , y2) => this[x2, y2].X , width01 , height01);
            var yn=BilinearInterpolator.Interpolate((x2 , y2) => this[x2 , y2].Y , width01 , height01);
            var zn=BilinearInterpolator.Interpolate((x2 , y2) => this[x2 , y2].Z , width01 , height01);
            return new Point3D(xn , yn , zn);
        }

        public Point3D this[int x , int y]
        {
            get=>Corners[y * 2 + x];
        }
    }
}
