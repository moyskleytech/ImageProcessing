using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MoyskleyTech.ImageProcessing.Image
{
    public class BitmapSubstract
    {
        public int Tolerance { get; set; }
        public BitmapSubstractMode Mode { get; set; }
        public Func<Pixel,Pixel,int> DistanceFunction { get; set; }
        private Bitmap A,B;
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
        public BitmapSubstract(Bitmap bitmapA , Bitmap bitmapB):this()
        {
            this.A = bitmapA;
            this.B = bitmapB;
        }
        public BitmapSubstract(Image<Pixel> bitmapA , Bitmap bitmapB) : this()
        {
            this.Aa = bitmapA;
            this.B = bitmapB;
        }
        public BitmapSubstract(Bitmap bitmapA , Image<Pixel> bitmapB) : this()
        {
            this.A = bitmapA;
            this.Bb = bitmapB;
        }
        public BitmapSubstract(Image<Pixel> bitmapA , Image<Pixel> bitmapB) : this()
        {
            this.Aa = bitmapA;
            this.Bb = bitmapB;
        }
        private Pixel GetAtA(int i)
        {
            return A?[i] ?? Aa[i];
        }
        private Pixel GetAtB(int i)
        {
            return B?[i] ?? Bb[i];
        }

        public BitmapSubstract this[int t]
        {
            get {
                return new BitmapSubstract() {A = A , B = B, Aa=Aa,Bb=Bb , Mode = Mode , DistanceFunction = DistanceFunction , Tolerance = t };
            }
        }
        public BitmapSubstract this[Func<Pixel,Pixel,int> f]
        {
            get
            {
                return new BitmapSubstract() { A = A , B = B , Aa = Aa , Bb = Bb , Mode = Mode , DistanceFunction = f , Tolerance = Tolerance };
            }
        }
        public BitmapSubstract this[BitmapSubstractMode m]
        {
            get
            {
                return new BitmapSubstract() { A = A , B = B , Aa = Aa , Bb = Bb , Mode = m , DistanceFunction = DistanceFunction , Tolerance = Tolerance };
            }
        }

        public static implicit operator Bitmap(BitmapSubstract b)
        {
            return b.ToBitmap();
        }
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
        public Bitmap ToBitmap()
        {
            Bitmap result = new Bitmap(A.Width,A.Height);
            Do((i , x) => result[i] = x);
            return result;
        }
        public Image<Pixel> ToImage()
        {
            Image<Pixel> result = new PixelImage(A.Width,A.Height);
            Do((i , x) => result[i] = x);
            return result;
        }
        private void Do(Action<int , Pixel> setter)
        {
            if ( Mode == BitmapSubstractMode.BandDifference )
            {
                for ( var i = 0; i < A.Width * A.Height; i++ )
                {
                    var a = GetAtA(i);
                    var b= GetAtB(i);
                    setter(i,Pixel.FromArgb(Clamp(a.A - b.A) , Clamp(a.R - b.R) , Clamp(a.G - b.G) , Clamp(a.B - b.B)));
                }
            }
            else if ( Mode == BitmapSubstractMode.KeepSame )
            {
                for ( var i = 0; i < A.Width * A.Height; i++ )
                {
                    var a = GetAtA(i);
                    var b= GetAtB(i);
                    var d = DistanceFunction(a,b);
                    if ( d < Tolerance )
                        setter(i,a);
                    else
                        setter(i,Pixels.Transparent);
                }
            }
            else if ( Mode == BitmapSubstractMode.Mask )
            {
                for ( var i = 0; i < A.Width * A.Height; i++ )
                {
                    var a = GetAtA(i);
                    var b= GetAtB(i);
                    setter(i,Pixel.FromArgb(System.Math.Min(( byte ) ( 255 - b.A ) , a.A) , a.R , a.G , a.B));
                }
            }
        }
    }
    public enum BitmapSubstractMode
    {
        BandDifference,KeepSame,Mask
    }
}
