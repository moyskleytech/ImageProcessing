using MoyskleyTech.ImageProcessing.Image;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MoyskleyTech.ImageProcessing.Specialized
{
    public class LightTrailBuilder
    {
        int width,height,max;
        Bitmap workingObject;

        public LightTrailBuilder(int width , int height)
        {
            this.width = width;
            this.height = height;
            max = width * height;
            workingObject = new Bitmap(width , height);
            Graphics.FromImage(workingObject).Clear(Pixels.Black);
        }
        public LightTrailBuilder(Bitmap firstFrame)
        {
            width = workingObject.Width;
            height = workingObject.Height;
            max = width * height;
            workingObject = (Bitmap)firstFrame.Clone();
        }

        public void AddFrame(Bitmap frame)
        {
            if ( frame.Width != width )
                throw new ArgumentException("Invalid width" , "frame");
            if ( frame.Height != height )
                throw new ArgumentException("Invalid height" , "frame");
            unsafe
            {
                Pixel* ptrWork = workingObject.Source;
                Pixel* ptrFrame = frame.Source;
                for ( var i = 0; i < max; i++ )
                {
                    if ( ptrFrame->ToHSB().B > ptrWork->ToHSB().B )
                        *ptrWork = *ptrFrame;
                    ptrWork++;
                    ptrFrame++;
                }
            }
        }
        public Bitmap Build()
        {
            return ( Bitmap ) workingObject.Clone();
        }
    }
}
