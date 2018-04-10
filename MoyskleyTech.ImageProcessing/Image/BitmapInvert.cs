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
    
        private BitmapInvert()
        {
            
        }
        public BitmapInvert(Bitmap bitmapA):this()
        {
            this.A = bitmapA;
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
        public byte B(int b)
        {
            return ( byte ) b;
        }
        public Bitmap ToBitmap()
        {
            Bitmap result = new Bitmap(A.Width,A.Height);
            if ( Mode == BitmapInvertMode.Color )
            {
                for ( var i = 0; i < A.Width * A.Height; i++ )
                {
                    var a = A[i];
                    result[i] = Pixel.FromArgb(a.A , B(255 - a.R) , B(255 - a.G) , B(255 - a.B));
                }
            } else if ( Mode == BitmapInvertMode.AllBand )
            {
                for ( var i = 0; i < A.Width * A.Height; i++ )
                {
                    var a = A[i];
                    result[i] = Pixel.FromArgb(B(255-a.A) , B(255 - a.R) , B(255 - a.G) , B(255 - a.B));
                }
            }else if ( Mode == BitmapInvertMode.Alpha )
            {
                for ( var i = 0; i < A.Width * A.Height; i++ )
                {
                    var a = A[i];
                    result[i] = Pixel.FromArgb(B(255 - a.A) , a.R , a.G , a.B);
                }
            }

            return result;
        }

    }
    public enum BitmapInvertMode
    {
        Color,AllBand,Alpha
    }
}
