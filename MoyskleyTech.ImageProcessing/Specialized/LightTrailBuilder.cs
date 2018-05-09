using MoyskleyTech.ImageProcessing.Image;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MoyskleyTech.ImageProcessing.Specialized
{
    /// <summary>
    /// Allow creation of LightTrail images
    /// </summary>
    public class LightTrailBuilder
    {
        int width,height,max;
        Bitmap workingObject;
        /// <summary>
        /// Create a lightTrailBuilder using size
        /// </summary>
        /// <param name="width"></param>
        /// <param name="height"></param>
        public LightTrailBuilder(int width , int height)
        {
            this.width = width;
            this.height = height;
            max = width * height;
            workingObject = new Bitmap(width , height);
            Graphics.FromImage(workingObject).Clear(Pixels.Black);
        }
        /// <summary>
        /// Create a lightTrailBuilder using first frame
        /// </summary>
        /// <param name="firstFrame"></param>
        public LightTrailBuilder(Bitmap firstFrame)
        {
            width = workingObject.Width;
            height = workingObject.Height;
            max = width * height;
            workingObject = (Bitmap)firstFrame.Clone();
        }
        /// <summary>
        /// Add a frame to the lighttrail(Max of current and added)
        /// </summary>
        /// <param name="frame"></param>
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
        /// <summary>
        /// Return result
        /// </summary>
        /// <returns></returns>
        public Bitmap Build()
        {
            return ( Bitmap ) workingObject.Clone();
        }
    }
}
