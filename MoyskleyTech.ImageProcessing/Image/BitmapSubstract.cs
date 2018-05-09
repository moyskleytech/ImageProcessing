using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MoyskleyTech.ImageProcessing.Image
{
    /// <summary>
    /// Allow bitmap substraction
    /// </summary>
    public class BitmapSubstract
    {
        /// <summary>
        /// Tolerence if mode is BandDifference
        /// </summary>
        public int Tolerance { get; set; }
        /// <summary>
        /// Define the mode of substract
        /// </summary>
        public BitmapSubstractMode Mode { get; set; }
        /// <summary>
        /// Distance function between pixels
        /// </summary>
        public Func<Pixel , Pixel , int> DistanceFunction { get; set; }
        private Image<Pixel> Aa,Bb;
        private int GetDistancePixel(Pixel a , Pixel b)
        {
            Func<int,int> abs = System.Math.Abs;
            return abs(a.A - b.A) + abs(a.R - b.R) + abs(a.G - b.G) + abs(a.B - b.B);
        }
        private BitmapSubstract()
        {
            DistanceFunction = GetDistancePixel;
            Tolerance = 4;
        }
        /// <summary>
        /// Create from 2 images
        /// </summary>
        /// <param name="bitmapA"></param>
        /// <param name="bitmapB"></param>
        public BitmapSubstract(Image<Pixel> bitmapA , Image<Pixel> bitmapB) : this()
        {
            this.Aa = bitmapA;
            this.Bb = bitmapB;
        }
        private Pixel GetAtA(int i)
        {
            return Aa[i];
        }
        private Pixel GetAtB(int i)
        {
            return Bb[i];
        }
        /// <summary>
        /// Allow change of tolerence
        /// </summary>
        /// <param name="t"></param>
        /// <returns></returns>
        public BitmapSubstract this[int t]
        {
            get
            {
                return new BitmapSubstract() { Aa = Aa , Bb = Bb , Mode = Mode , DistanceFunction = DistanceFunction , Tolerance = t };
            }
        }
        /// <summary>
        /// Allow change of distance function
        /// </summary>
        /// <param name="f"></param>
        /// <returns></returns>
        public BitmapSubstract this[Func<Pixel , Pixel , int> f]
        {
            get
            {
                return new BitmapSubstract() { Aa = Aa , Bb = Bb , Mode = Mode , DistanceFunction = f , Tolerance = Tolerance };
            }
        }
        /// <summary>
        /// Allow change of mode
        /// </summary>
        /// <param name="m"></param>
        /// <returns></returns>
        public BitmapSubstract this[BitmapSubstractMode m]
        {
            get
            {
                return new BitmapSubstract() { Aa = Aa , Bb = Bb , Mode = m , DistanceFunction = DistanceFunction , Tolerance = Tolerance };
            }
        }
        /// <summary>
        /// Get result
        /// </summary>
        /// <param name="b"></param>
        public static implicit operator Bitmap(BitmapSubstract b)
        {
            return b.ToBitmap();
        }
        /// <summary>
        /// Get result
        /// </summary>
        /// <param name="b"></param>
        public static implicit operator Image<Pixel>(BitmapSubstract b)
        {
            return b.ToImage();
        }
        private byte Clamp(int a)
        {
            if ( a < 0 ) return 0;
            if ( a > 255 ) return 255;
            return ( byte ) a;
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
            if ( Mode == BitmapSubstractMode.BandDifference )
            {
                for ( var i = 0; i < Aa.Width * Aa.Height; i++ )
                {
                    var a = GetAtA(i);
                    var b= GetAtB(i);
                    setter(i , Pixel.FromArgb(Clamp(a.A - b.A) , Clamp(a.R - b.R) , Clamp(a.G - b.G) , Clamp(a.B - b.B)));
                }
            }
            else if ( Mode == BitmapSubstractMode.KeepSame )
            {
                for ( var i = 0; i < Aa.Width * Aa.Height; i++ )
                {
                    var a = GetAtA(i);
                    var b= GetAtB(i);
                    var d = DistanceFunction(a,b);
                    if ( d < Tolerance )
                        setter(i , a);
                    else
                        setter(i , Pixels.Transparent);
                }
            }
            else if ( Mode == BitmapSubstractMode.Mask )
            {
                for ( var i = 0; i < Aa.Width * Aa.Height; i++ )
                {
                    var a = GetAtA(i);
                    var b= GetAtB(i);
                    setter(i , Pixel.FromArgb(System.Math.Min(( byte ) ( 255 - b.A ) , a.A) , a.R , a.G , a.B));
                }
            }
        }
    }
    /// <summary>
    /// Define Substraction mode
    /// </summary>
    public enum BitmapSubstractMode
    {
        /// <summary>
        /// Difference
        /// </summary>
        BandDifference,
        /// <summary>
        /// Keep if distance function is inferior to tolerente
        /// </summary>
        KeepSame,
        /// <summary>
        /// Use the second image as mask
        /// </summary>
        Mask
    }
}
