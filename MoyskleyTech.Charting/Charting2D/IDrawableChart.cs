using MoyskleyTech.ImageProcessing.Image;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MoyskleyTech.Charting.Charting2D
{
    public interface IDrawableChart<R>
        where R:unmanaged
    {
        Image<R> Draw(int width, int height);
        void Draw(Graphics<R> g, int width, int height);
    }
    interface IDrawableChart : IDrawableChart<Pixel> { }
}
