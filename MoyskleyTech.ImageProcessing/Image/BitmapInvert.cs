using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MoyskleyTech.ImageProcessing.Image
{
    public class BitmapInvert
    {
        public BitmapInvertMode Mode { get; set; }
      
        private Bitmap A;
        private Image<Pixel> Aa;
    
        private BitmapInvert()
        {
            
        }
        public BitmapInvert(Bitmap bitmapA):this()
        {
            this.A = bitmapA;
        }
        public BitmapInvert(Image<Pixel> bitmapA) : this()
        {
            this.Aa = bitmapA;
        }


        public BitmapInvert this[BitmapInvertMode m]
        {
            get
            {
                return new BitmapInvert() { A = A ,Mode = m };
            }
        }

        public static implicit operator Bitmap(BitmapInvert b)
        {
            return b.ToBitmap();
        }
        public static implicit operator Image<Pixel>(BitmapInvert b)
        {
            return b.ToImage();
        }
        public byte B(int b)
        {
            return ( byte ) b;
        }
        private Pixel GetAtA(int i)
        {
            return A?[i] ?? Aa[i];
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
            if ( Mode == BitmapInvertMode.Color )
            {
                for ( var i = 0; i < A.Width * A.Height; i++ )
                {
                    var a = GetAtA(i);
                    setter(i,Pixel.FromArgb(a.A , B(255 - a.R) , B(255 - a.G) , B(255 - a.B)));
                }
            }
            else if ( Mode == BitmapInvertMode.AllBand )
            {
                for ( var i = 0; i < A.Width * A.Height; i++ )
                {
                    var a = GetAtA(i);
                    setter(i , Pixel.FromArgb(B(255 - a.A) , B(255 - a.R) , B(255 - a.G) , B(255 - a.B)));
                }
            }
            else if ( Mode == BitmapInvertMode.Alpha )
            {
                for ( var i = 0; i < A.Width * A.Height; i++ )
                {
                    var a = GetAtA(i);
                    setter(i , Pixel.FromArgb(B(255 - a.A) , a.R , a.G , a.B));
                }
            }

        }
    }
    public enum BitmapInvertMode
    {
        Color,AllBand,Alpha
    }
}
