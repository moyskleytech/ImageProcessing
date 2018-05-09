using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MoyskleyTech.ImageProcessing.Image
{
    /// <summary>
    /// Allow bitmap inversion
    /// </summary>
    public class BitmapInvert
    {
        /// <summary>
        /// The inversion mode
        /// </summary>
        public BitmapInvertMode Mode { get; set; }
      
        private Image<Pixel> Aa;
    
        private BitmapInvert()
        {
            
        }
        /// <summary>
        /// Create the inversion from image
        /// </summary>
        /// <param name="bitmapA"></param>
        public BitmapInvert(Image<Pixel> bitmapA) : this()
        {
            this.Aa = bitmapA;
        }
        /// <summary>
        /// Allow change of mode
        /// </summary>
        /// <param name="m"></param>
        /// <returns></returns>
        public BitmapInvert this[BitmapInvertMode m]
        {
            get
            {
                return new BitmapInvert() { Aa = Aa ,Mode = m };
            }
        }
        /// <summary>
        /// Get result
        /// </summary>
        /// <param name="b"></param>
        public static implicit operator Bitmap(BitmapInvert b)
        {
            return b.ToBitmap();
        }
        /// <summary>
        /// Get result
        /// </summary>
        /// <param name="b"></param>
        public static implicit operator Image<Pixel>(BitmapInvert b)
        {
            return b.ToImage();
        }
        private byte B(int b)
        {
            return ( byte ) b;
        }
        private Pixel GetAtA(int i)
        {
            return Aa[i];
        }
        /// <summary>
        /// Get result
        /// </summary>
        /// <returns></returns>
        public Bitmap ToBitmap()
        {
            Bitmap result = new Bitmap(Aa.Width,Aa.Height);
            Do((i , x) => result[i] = x);
            return result;
        }
        /// <summary>
        /// Get result
        /// </summary>
        /// <returns></returns>
        public Image<Pixel> ToImage()
        {
            return ToBitmap();
        }
        private void Do(Action<int , Pixel> setter)
        {
            if ( Mode == BitmapInvertMode.Color )
            {
                for ( var i = 0; i < Aa.Width * Aa.Height; i++ )
                {
                    var a = GetAtA(i);
                    setter(i,Pixel.FromArgb(a.A , B(255 - a.R) , B(255 - a.G) , B(255 - a.B)));
                }
            }
            else if ( Mode == BitmapInvertMode.AllBand )
            {
                for ( var i = 0; i < Aa.Width * Aa.Height; i++ )
                {
                    var a = GetAtA(i);
                    setter(i , Pixel.FromArgb(B(255 - a.A) , B(255 - a.R) , B(255 - a.G) , B(255 - a.B)));
                }
            }
            else if ( Mode == BitmapInvertMode.Alpha )
            {
                for ( var i = 0; i < Aa.Width * Aa.Height; i++ )
                {
                    var a = GetAtA(i);
                    setter(i , Pixel.FromArgb(B(255 - a.A) , a.R , a.G , a.B));
                }
            }

        }
    }
    /// <summary>
    /// Define mode for inversion
    /// </summary>
    public enum BitmapInvertMode
    {
        /// <summary>
        /// Invert color only
        /// </summary>
        Color,
        /// <summary>
        /// Invert all band (color+alpha)
        /// </summary>
        AllBand,
        /// <summary>
        /// Invert Alpha only
        /// </summary>
        Alpha
    }
}
