using MoyskleyTech.ImageProcessing.Image;
using System;

namespace MoyskleyTech.ImageProcessing.Form
{
    public interface IRenderer
    {
        void Render(Graphics<Pixel> g , int x , int y , int w , int h);
    }
   
}