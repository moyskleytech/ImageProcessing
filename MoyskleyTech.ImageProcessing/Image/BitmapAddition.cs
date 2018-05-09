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
        private Image<Pixel> Aa,Bb;
        private BitmapAddition()
        {
        }
       
        public BitmapAddition(Image<Pixel> bitmapA , Image<Pixel> bitmapB) : this()
        {
            this.Aa = bitmapA;
            this.Bb = bitmapB;
        }

        public BitmapAddition this[BitmapAdditionMode m]
        {
            get
            {
                return new BitmapAddition() { Aa = Aa , Bb = Bb , Mode = m };
            }
        }

        public static implicit operator Bitmap(BitmapAddition b)
        {
            return b.ToBitmap();
        }
        public static implicit operator Image<Pixel>(BitmapAddition b)
        {
            return b.ToImage();
        }
        private byte Clamp(int a)
        {
            if ( a < 0 ) return 0;
            if ( a > 255 ) return 255;
            return ( byte ) a;
        }
        private Pixel GetAtA(int i)
        {
            return Aa[i];
        }
        private Pixel GetAtB(int i)
        {
            return Bb[i];
        }
        private void Do(Action<Pixel , int> setter)
        {
            if ( Mode == BitmapAdditionMode.BandAdd )
            {
                for ( var i = 0; i < Aa.Width * Aa.Height; i++ )
                {
                    var a = GetAtA(i);
                    var b= GetAtB(i);
                    setter(Pixel.FromArgb(Clamp(a.A + b.A) , Clamp(a.R + b.R) , Clamp(a.G + b.G) , Clamp(a.B + b.B)),i);
                }
            }
            else if ( Mode == BitmapAdditionMode.Max )
            {
                for ( var i = 0; i < Aa.Width * Aa.Height; i++ )
                {
                    var a = GetAtA(i);
                    var b= GetAtB(i);
                    Func<byte,byte,byte> max = System.Math.Max;
                    setter(Pixel.FromArgb(max(a.A , b.A) , max(a.R , b.R) , max(a.G , b.G) , max(a.B , b.B)),i);
                }
            }
            else if ( Mode == BitmapAdditionMode.Min )
            {
                for ( var i = 0; i < Aa.Width * Aa.Height; i++ )
                {
                    var a = GetAtA(i);
                    var b= GetAtB(i);
                    Func<byte,byte,byte> min = System.Math.Min;
                    setter(Pixel.FromArgb(min(a.A , b.A) , min(a.R , b.R) , min(a.G , b.G) , min(a.B , b.B)),i);
                }
            }
            else if ( Mode == BitmapAdditionMode.Mask )
            {
                for ( var i = 0; i < Aa.Width * Aa.Height; i++ )
                {
                    var a = GetAtA(i);
                    var b= GetAtB(i);
                    setter(Pixel.FromArgb(System.Math.Min(( byte ) ( b.A ) , a.A) , a.R , a.G , a.B),i);
                }
            }
        }
        public Bitmap ToBitmap()
        {
            Bitmap result = new Bitmap(Aa.Width,Aa.Height);
            Do((x , i) => result[i] = x);
            return result;
        }
        public Image<Pixel> ToImage()
        {
            return ToBitmap();
        }

    }
    public enum BitmapAdditionMode
    {
        BandAdd,Max,Min,Mask
    }
}
