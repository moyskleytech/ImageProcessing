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

        public BitmapSubstract this[int t]
        {
            get {
                return new BitmapSubstract() {A = A , B = B , Mode = Mode , DistanceFunction = DistanceFunction , Tolerance = t };
            }
        }
        public BitmapSubstract this[Func<Pixel,Pixel,int> f]
        {
            get
            {
                return new BitmapSubstract() { A = A , B = B , Mode = Mode , DistanceFunction = f , Tolerance = Tolerance };
            }
        }
        public BitmapSubstract this[BitmapSubstractMode m]
        {
            get
            {
                return new BitmapSubstract() { A = A , B = B , Mode = m , DistanceFunction = DistanceFunction , Tolerance = Tolerance };
            }
        }

        public static implicit operator Bitmap(BitmapSubstract b)
        {
            return b.ToBitmap();
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
            if ( Mode == BitmapSubstractMode.BandDifference )
            {
                for ( var i = 0; i < A.Width * A.Height; i++ )
                {
                    var a = A[i];
                    var b= B[i];
                    result[i] = Pixel.FromArgb(Clamp(a.A - b.A) , Clamp(a.R - b.R) , Clamp(a.G - b.G) , Clamp(a.B - b.B));
                }
            }
            else if ( Mode == BitmapSubstractMode.KeepSame )
            {
                for ( var i = 0; i < A.Width * A.Height; i++ )
                {
                    var a = A[i];
                    var b= B[i];
                    var d = DistanceFunction(a,b);
                    if ( d < Tolerance )
                        result[i] = a;
                    else
                        result[i]= Pixels.Transparent;
                }
            }else if ( Mode == BitmapSubstractMode.Mask )
            {
                for ( var i = 0; i < A.Width * A.Height; i++ )
                {
                    var a = A[i];
                    var b= B[i];
                    result[i] = Pixel.FromArgb(System.Math.Min((byte)(255-b.A),a.A),a.R,a.G,a.B);
                }
            }
            return result;
        }

    }
    public enum BitmapSubstractMode
    {
        BandDifference,KeepSame,Mask
    }
}
