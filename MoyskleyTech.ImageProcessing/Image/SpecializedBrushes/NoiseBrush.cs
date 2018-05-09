using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MoyskleyTech.ImageProcessing.Image.SpecializedBrushes
{
    /// <summary>
    /// Brush that create random pixels
    /// </summary>
    public class NoiseBrush:Brush
    {
        Random r=new Random();
        /// <summary>
        /// Get a random pixel at the specified location
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <returns></returns>
        public override Pixel GetColor(int x , int y)
        {
            byte[] bytes=new byte[3];
            r.NextBytes(bytes);
            return Pixel.FromArgb(255 , bytes[0] , bytes[1] , bytes[2]);
        }
    }
}
