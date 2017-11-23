using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MoyskleyTech.ImageProcessing.Image
{
    /// <summary>
    /// Main use is ROI
    /// </summary>
    public class ImageProxy
    {
        private Bitmap img;
        private Rectangle rct;
        public ImageProxy(Bitmap bitmap , Rectangle rectangle)
        {
            this.img = bitmap;
            this.rct = rectangle;
        }
        public int Left => rct.Left;
        public int Top => rct.Top;
        public int Width => rct.Width;
        public int Height => rct.Height;
        public Bitmap ToBitmap()
        {
            return img.Crop(rct);
        }
        public Pixel this[int x , int y]
        {
            get
            {
                if ( x <= rct.Width && x >= 0 && y <= rct.Height && y >= 0 )
                    return img[x + rct.X , y + rct.Y];
                return Pixels.Transparent;
            }
            set
            {
                if ( x <= rct.Width && x >= 0 && y <= rct.Height && y >= 0 )
                    img[x + rct.X , y + rct.Y] = value;
            }
        }
        public static implicit operator Bitmap(ImageProxy ip)
        {
            return ip.ToBitmap();
        }
        public void ApplyFilter(Func<Pixel , Point , Pixel> func)
        {
            for ( var y = 0; y < Height; y++ )
                for ( var x = 0; x < Width; x++ )
                    this[x , y] = func(this[x , y] , new Point(x , y));
        }
    }
}
