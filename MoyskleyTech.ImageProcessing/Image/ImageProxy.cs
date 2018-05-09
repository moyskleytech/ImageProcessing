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
        /// <summary>
        /// Compatibility
        /// </summary>
        /// <param name="bitmap"></param>
        /// <param name="rectangle"></param>
        public ImageProxy(Bitmap bitmap , Rectangle rectangle):base((Image<Pixel>)bitmap,rectangle)
        {
          
        }
        /// <summary>
        /// Compatibility
        /// </summary>
        /// <param name="prx"></param>
        /// <param name="rectangle"></param>
        public ImageProxy(ImageProxy prx , Rectangle rectangle) : base((ImageProxy<Pixel>)prx , rectangle)
        {
        }
        
        /// <summary>
        /// Create bitmap
        /// </summary>
        /// <returns></returns>
        public Bitmap ToBitmap()
        {
            Bitmap image = new Bitmap(rct.Width,rct.Height);
            for ( var x = 0; x < Width; x++ )
                for ( var y = 0; y < Width; y++ )
                    image[x , y] = (this[x , y]);
            return image;
        }
        /// <summary>
        /// Create image converting to new representation
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
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
        /// <summary>
        /// Convert proxy to bitmap
        /// </summary>
        /// <param name="ip"></param>
        public static implicit operator Bitmap(ImageProxy ip)
        {
            return ip.ToBitmap();
        }
        /// <summary>
        /// Convert bitmap to proxy
        /// </summary>
        /// <param name="ip"></param>
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
        /// <summary>
        /// The proxying image
        /// </summary>
        protected Image<Representation> img;
        /// <summary>
        /// Area rectangle
        /// </summary>
        protected Rectangle rct;
        /// <summary>
        /// Create a empty proxy
        /// </summary>
        protected ImageProxy()
        {

        }
        /// <summary>
        /// Create a proxy using an image
        /// </summary>
        /// <param name="bitmap"></param>
        /// <param name="rectangle"></param>
        public ImageProxy(Image<Representation> bitmap , Rectangle rectangle)
        {
            this.img = bitmap;
            this.rct = rectangle;
        }
        /// <summary>
        /// Create a proxy using another proxy
        /// </summary>
        /// <param name="prx"></param>
        /// <param name="rectangle"></param>
        public ImageProxy(ImageProxy<Representation> prx , Rectangle rectangle)
        {
            this.img = prx.img;
            this.rct = new Rectangle(rectangle.X + prx.Rectangle.X , rectangle.Y + prx.Rectangle.Y , rectangle.Width , rectangle.Height);
        }
        /// <summary>
        /// Left of area
        /// </summary>
        public int Left => rct.Left;
        /// <summary>
        /// Top of area
        /// </summary>
        public int Top => rct.Top;
        /// <summary>
        /// Left of area
        /// </summary>
        public int X => rct.X;
        /// <summary>
        /// Top of area
        /// </summary>
        public int Y => rct.Y;
        /// <summary>
        /// Right of area
        /// </summary>
        public int Right => rct.Right;
        /// <summary>
        /// Bottom of area
        /// </summary>
        public int Bottom => rct.Bottom;
        /// <summary>
        /// Width of area
        /// </summary>
        public int Width => rct.Width;
        /// <summary>
        /// Height of area
        /// </summary>
        public int Height => rct.Height;
        /// <summary>
        /// Area rectangle
        /// </summary>
        public Rectangle Rectangle => rct;
        /// <summary>
        /// Convert to image
        /// </summary>
        /// <returns></returns>
        public virtual Image<Representation> ToImage()
        {
            Image<Representation> image = Image<Representation>.Create(rct.Width,rct.Height);
            for ( var x = 0; x < Width; x++ )
                for ( var y = 0; y < Width; y++ )
                    image[x , y] = (this[x , y]);
            return image;
        }
        /// <summary>
        /// Convert to image using converter
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="converter"></param>
        /// <returns></returns>
        public virtual Image<T> ToImage<T>(Func<Representation,T> converter)
            where T:struct
        {
            Image<T> image = Image<T>.Create(rct.Width,rct.Height);
            for ( var x = 0; x < Width; x++ )
                for ( var y = 0; y < Width; y++ )
                    image[x , y] = converter(this[x , y]);
            return image;
        }
        /// <summary>
        /// Get a pixel from image
        /// </summary>
        /// <param name="t"></param>
        /// <returns></returns>
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
        /// <summary>
        /// Get a pixel from image
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <returns></returns>
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
        /// <summary>
        /// Convert to image
        /// </summary>
        /// <param name="ip"></param>
        public static implicit operator Image<Representation>(ImageProxy<Representation> ip)
        {
            return ip.ToImage();
        }
        /// <summary>
        /// Convert to proxy
        /// </summary>
        /// <param name="ip"></param>
        public static implicit operator ImageProxy<Representation>(Image<Representation> ip)
        {
            return new ImageProxy<Representation>(ip , new Rectangle(0 , 0 , ip.Width , ip.Height));
        }
        /// <summary>
        /// Apply a filter on the proxy
        /// </summary>
        /// <param name="func"></param>
        public void ApplyFilter(Func<Representation , Point , Representation> func)
        {
            for ( var y = 0; y < Height; y++ )
                for ( var x = 0; x < Width; x++ )
                    this[x , y] = func(this[x , y] , new Point(x , y));
        }
    }
}
