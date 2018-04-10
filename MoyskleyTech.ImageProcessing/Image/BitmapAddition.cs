using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MoyskleyTech.ImageProcessing.Image
{
    public class BitmapAddition
    {
        public BitmapAdditionMode Mode { get; set; }
        private Bitmap A,B;
        private BitmapAddition()
        {
        }
        public BitmapAddition(Bitmap bitmapA , Bitmap bitmapB):this()
        {
            this.A = bitmapA;
            this.B = bitmapB;
        }

      
        public BitmapAddition this[BitmapAdditionMode m]
        {
            get
            {
                return new BitmapAddition() { A = A , B = B , Mode = m };
            }
        }

        public static implicit operator Bitmap(BitmapAddition b)
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
            if ( Mode == BitmapAdditionMode.BandAdd )
            {
                for ( var i = 0; i < A.Width * A.Height; i++ )
                {
                    var a = A[i];
                    var b= B[i];
                    result[i] = Pixel.FromArgb(Clamp(a.A + b.A) , Clamp(a.R + b.R) , Clamp(a.G + b.G) , Clamp(a.B + b.B));
                }
            }
            else if ( Mode == BitmapAdditionMode.Max )
            {
                for ( var i = 0; i < A.Width * A.Height; i++ )
                {
                    var a = A[i];
                    var b= B[i];
                    Func<byte,byte,byte> max = System.Math.Max;
                    result[i] = Pixel.FromArgb(max(a.A , b.A) , max(a.R , b.R) , max(a.G , b.G) , max(a.B , b.B));
                }
            }
            else if ( Mode == BitmapAdditionMode.Min )
            {
                for ( var i = 0; i < A.Width * A.Height; i++ )
                {
                    var a = A[i];
                    var b= B[i];
                    Func<byte,byte,byte> min = System.Math.Min;
                    result[i] = Pixel.FromArgb(min(a.A , b.A) , min(a.R , b.R) , min(a.G , b.G) , min(a.B , b.B));
                }
            }
            else if ( Mode == BitmapAdditionMode.Mask )
            {
                for ( var i = 0; i < A.Width * A.Height; i++ )
                {
                    var a = A[i];
                    var b= B[i];
                    result[i] = Pixel.FromArgb(System.Math.Min((byte)(b.A),a.A),a.R,a.G,a.B);
                }
            }
            return result;
        }

    }
    public enum BitmapAdditionMode
    {
        BandAdd,Max,Min,Mask
    }
}
