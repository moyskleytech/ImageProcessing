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
        public ImageProxy(ImageProxy prx , Rectangle rectangle)
        {
            this.img = prx.img;
            this.rct = new Rectangle(rectangle.X + prx.Rectangle.X, rectangle.Y + prx.Rectangle.Y, rectangle.Width,rectangle.Height);
        }
        public int Left => rct.Left;
        public int Top => rct.Top;
        public int X => rct.X;
        public int Y => rct.Y;
        public int Right => rct.Right;
        public int Bottom => rct.Bottom;
        public int Width => rct.Width;
        public int Height => rct.Height;
        public Rectangle Rectangle => rct;
        public Bitmap ToBitmap()
        {
            return img.Crop(rct);
        }
        public Pixel this[int t]
        {
            get {
                int y = t/Width;
                int x = t-y*Width;
                return this[x , y];
            }
            set {
                int y = t/Width;
                int x = t-y*Width;
                this[x , y]=value;
            }
        }
        public Pixel this[int x , int y]
        {
            get
            {
                if ( x < rct.Width && x >= 0 && y < rct.Height && y >= 0 )
                    return img[x + rct.X , y + rct.Y];
                return Pixels.Transparent;
            }
            set
            {
                if ( x < rct.Width && x >= 0 && y < rct.Height && y >= 0 )
                    img[x + rct.X , y + rct.Y] = value;
            }
        }
        public static implicit operator Bitmap(ImageProxy ip)
        {
            return ip.ToBitmap();
        }
        public static implicit operator ImageProxy(Bitmap ip)
        {
            return new ImageProxy(ip,new Rectangle(0,0,ip.Width,ip.Height));
        }
        public void ApplyFilter(Func<Pixel , Point , Pixel> func)
        {
            for ( var y = 0; y < Height; y++ )
                for ( var x = 0; x < Width; x++ )
                    this[x , y] = func(this[x , y] , new Point(x , y));
        }
    }
}
