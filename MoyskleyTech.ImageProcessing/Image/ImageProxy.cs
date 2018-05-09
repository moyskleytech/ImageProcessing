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
    public class ImageProxy:ImageProxy<Pixel>
    {
        public ImageProxy(Bitmap bitmap , Rectangle rectangle):base((Image<Pixel>)bitmap,rectangle)
        {
          
        }
        public ImageProxy(ImageProxy prx , Rectangle rectangle) : base((ImageProxy<Pixel>)prx , rectangle)
        {
        }
        
        public Bitmap ToBitmap()
        {
            Bitmap image = new Bitmap(rct.Width,rct.Height);
            for ( var x = 0; x < Width; x++ )
                for ( var y = 0; y < Width; y++ )
                    image[x , y] = (this[x , y]);
            return image;
        }
        public Image<T> ToImage<T>()
           where T : struct
        {
            var converter = ColorConvert.GetConversionFrom<Pixel,T>();
            Image<T> image = Image<T>.Create(rct.Width,rct.Height);
            for ( var x = 0; x < Width; x++ )
                for ( var y = 0; y < Width; y++ )
                    image[x , y] = converter(this[x , y]);
            return image;
        }
        public static implicit operator Bitmap(ImageProxy ip)
        {
            return ip.ToBitmap();
        }
        public static implicit operator ImageProxy(Bitmap ip)
        {
            return new ImageProxy(ip,new Rectangle(0,0,ip.Width,ip.Height));
        }
    }

    /// <summary>
    /// Main use is ROI
    /// </summary>
    public class ImageProxy<Representation>
        where Representation:struct
    {
        protected Image<Representation> img;
        protected Rectangle rct;
        protected ImageProxy()
        {

        }
        public ImageProxy(Image<Representation> bitmap , Rectangle rectangle)
        {
            this.img = bitmap;
            this.rct = rectangle;
        }
        public ImageProxy(ImageProxy<Representation> prx , Rectangle rectangle)
        {
            this.img = prx.img;
            this.rct = new Rectangle(rectangle.X + prx.Rectangle.X , rectangle.Y + prx.Rectangle.Y , rectangle.Width , rectangle.Height);
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
        public virtual Image<Representation> ToImage()
        {
            Image<Representation> image = Image<Representation>.Create(rct.Width,rct.Height);
            for ( var x = 0; x < Width; x++ )
                for ( var y = 0; y < Width; y++ )
                    image[x , y] = (this[x , y]);
            return image;
        }
        public virtual Image<T> ToImage<T>(Func<Representation,T> converter)
            where T:struct
        {
            Image<T> image = Image<T>.Create(rct.Width,rct.Height);
            for ( var x = 0; x < Width; x++ )
                for ( var y = 0; y < Width; y++ )
                    image[x , y] = converter(this[x , y]);
            return image;
        }
        public Representation this[int t]
        {
            get
            {
                int y = t/Width;
                int x = t-y*Width;
                return this[x , y];
            }
            set
            {
                int y = t/Width;
                int x = t-y*Width;
                this[x , y] = value;
            }
        }
        public virtual Representation this[int x , int y]
        {
            get
            {
                if ( x < rct.Width && x >= 0 && y < rct.Height && y >= 0 )
                    return img[x + rct.X , y + rct.Y];
                return default(Representation);
            }
            set
            {
                if ( x < rct.Width && x >= 0 && y < rct.Height && y >= 0 )
                    img[x + rct.X , y + rct.Y] = value;
            }
        }
        public static implicit operator Image<Representation>(ImageProxy<Representation> ip)
        {
            return ip.ToImage();
        }
        public static implicit operator ImageProxy<Representation>(Image<Representation> ip)
        {
            return new ImageProxy<Representation>(ip , new Rectangle(0 , 0 , ip.Width , ip.Height));
        }
        public void ApplyFilter(Func<Representation , Point , Representation> func)
        {
            for ( var y = 0; y < Height; y++ )
                for ( var x = 0; x < Width; x++ )
                    this[x , y] = func(this[x , y] , new Point(x , y));
        }
    }
}
