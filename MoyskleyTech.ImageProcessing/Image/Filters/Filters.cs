using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MoyskleyTech.ImageProcessing.Image
{
    /// <summary>
    /// Base filters
    /// </summary>
    public static class Filters
    {
        /// <summary>
        /// Apply a threashold using Red band
        /// </summary>
        /// <param name="b"></param>
        /// <returns></returns>
        public static Func<Pixel , Point , Pixel> Threashold(byte b)
        {
            return (x , y) => ( x.R > b ) ? Pixels.White : Pixels.Black;
        }
        /// <summary>
        /// Apply a threashold using Red band
        /// </summary>
        /// <param name="b"></param>
        /// <param name="trueP"></param>
        /// <param name="falseP"></param>
        /// <returns></returns>
        public static Func<Pixel , Point , Pixel> ThreasholdCustomColor(byte b , Pixel trueP , Pixel falseP)
        {
            return (x , y) => ( x.R > b ) ? trueP : falseP;
        }
        /// <summary>
        /// Get the max color and apply it as gray
        /// </summary>
        /// <returns></returns>
        public static Func<Pixel , Point , Pixel> Max()
        {
            return (x , y) =>
            {
                byte m=(new byte[ ] { x.R , x.G , x.B }).Max();
                return Pixel.FromArgb(255 , m , m , m);
            };
        }
        /// <summary>
        /// Invert color
        /// </summary>
        /// <returns></returns>
        public static Func<Pixel , Point , Pixel> Invert()
        {
            return (x , y) =>
            {
                return Pixel.FromArgb(255 , ( byte ) ( 255 - x.R ) , ( byte ) ( 255 - x.G ) , ( byte ) ( 255 - x.B ));
            };
        }
    }
}
