using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MoyskleyTech.Mathematics;
using MoyskleyTech.ImageProcessing.Image.Helper;
using System.Reflection;

namespace MoyskleyTech.ImageProcessing.Image
{
    /// <summary>
    /// PCL Portability to System.Drawing.Graphics
    /// </summary>
    public partial class Graphics : Graphics<Pixel>, IDisposable
    {
        /// <summary>
        /// Only for subclassing
        /// </summary>
        protected Graphics() { }
        /// <summary>
        /// Create a Graphics object from image
        /// </summary>
        /// <param name="bmp">The bitmap where to Draw</param>
        /// <returns>Return null if not applicable</returns>
        public static Graphics FromImage(Bitmap bmp)
        {
            Graphics instance;
            instance = new Graphics(1)
            {
                bmp = bmp
            };
            instance.ResetClip();
            return instance;
        }
        
        private Graphics(int u):base(u)
        {
        }
       
        /// <summary>
        /// Draw image in bitmap
        /// </summary>
        /// <param name="source">Source</param>
        /// <param name="dest">X origin</param>
        public virtual void DrawImage(Image<Pixel> source , Rectangle dest)
        {
            var tmp = source.Resize(dest.Width , dest.Height, ScalingMode.AverageInterpolate);
            DrawImage(( Image<Pixel> ) tmp , dest.X , dest.Y);
            tmp.Dispose();
        }
        /// <summary>
        /// Draw image in bitmap
        /// </summary>
        /// <param name="source">Source</param>
        /// <param name="dest">X origin</param>
        /// <param name="src">Y origin</param>
        public virtual void DrawImage(Image<Pixel> source , Rectangle dest , Rectangle src)
        {
            var tmp = source.Crop(src);
            DrawImage(tmp , dest);
            tmp.Dispose();
        }

       
    }
}


