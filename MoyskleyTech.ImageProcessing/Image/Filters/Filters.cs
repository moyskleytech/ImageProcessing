using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MoyskleyTech.ImageProcessing.Image
{
    public static class Filters
    {
        public static Func<Pixel , Point , Pixel> Threashold(byte b)
        {
            return (x , y) => ( x.R > b ) ? Pixels.White : Pixels.Black;
        }
        public static Func<Pixel , Point , Pixel> ThreasholdCustomColor(byte b , Pixel trueP , Pixel falseP)
        {
            return (x , y) => ( x.R > b ) ? trueP : falseP;
        }
        public static Func<Pixel , Point , Pixel> Max()
        {
            return (x , y) =>
            {
                byte m=(new byte[ ] { x.R , x.G , x.B }).Max();
                return Pixel.FromArgb(255 , m , m , m);
            };
        }
        public static Func<Pixel , Point , Pixel> Invert()
        {
            return (x , y) =>
            {
                return Pixel.FromArgb(255 , ( byte ) ( 255 - x.R ) , ( byte ) ( 255 - x.G ) , ( byte ) ( 255 - x.B ));
            };
        }
    }
}
