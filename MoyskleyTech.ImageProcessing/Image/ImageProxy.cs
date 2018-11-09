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
    public class ImageProxy<Representation>
        where Representation : unmanaged
    {
        /// <summary>
        /// The proxying image
        /// </summary>
        protected Image<Representation> img;


        public bool Readonly { get; private set; }
        /// <summary>
        /// Area rectangle
        /// </summary>
        protected Rectangle rct;
        internal virtual Rectangle Rect
        {
            get { return rct; }
            set { rct = value; }
        }
        /// <summary>
        /// Create a empty proxy
        /// </summary>
        protected ImageProxy(bool rdonly)
        {
            Readonly = false;
        }
        /// <summary>
        /// Create a proxy using an image
        /// </summary>
        /// <param name="bitmap"></param>
        /// <param name="rectangle"></param>
        public ImageProxy(Image<Representation> bitmap , Rectangle rectangle , bool rdonly = false) : this(rdonly)
        {
            this.img = bitmap;
            this.Rect = rectangle;
        }
        /// <summary>
        /// Create a proxy using another proxy
        /// </summary>
        /// <param name="prx"></param>
        /// <param name="rectangle"></param>
        public ImageProxy(ImageProxy<Representation> prx , Rectangle rectangle , bool rdonly = false) : this(prx.Readonly || rdonly)
        {
            this.img = prx.img;
            this.Rect = new Rectangle(rectangle.X + prx.Rectangle.X , rectangle.Y + prx.Rectangle.Y , rectangle.Width , rectangle.Height);
        }

        /// <summary>
        /// Left of area
        /// </summary>
        public int Left => Rect.Left;
        /// <summary>
        /// Top of area
        /// </summary>
        public int Top => Rect.Top;
        /// <summary>
        /// Left of area
        /// </summary>
        public int X => Rect.X;
        /// <summary>
        /// Top of area
        /// </summary>
        public int Y => Rect.Y;
        /// <summary>
        /// Right of area
        /// </summary>
        public int Right => Rect.Right;
        /// <summary>
        /// Bottom of area
        /// </summary>
        public int Bottom => Rect.Bottom;
        /// <summary>
        /// Width of area
        /// </summary>
        public int Width => Rect.Width;
        /// <summary>
        /// Height of area
        /// </summary>
        public int Height => Rect.Height;
        /// <summary>
        /// Area rectangle
        /// </summary>
        public Rectangle Rectangle => Rect;
        /// <summary>
        /// Convert to image
        /// </summary>
        /// <returns></returns>
        public virtual Image<Representation> ToImage()
        {
            Image<Representation> image = Image<Representation>.Create(Rect.Width,Rect.Height);
            for ( var x = 0; x < Width; x++ )
                for ( var y = 0; y < Height; y++ )
                    image[x , y] = ( this[x , y] );
            return image;
        }
        /// <summary>
        /// Convert to image using converter
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="converter"></param>
        /// <returns></returns>
        public virtual Image<T> ToImage<T>(Func<Representation , T> converter = null)
            where T : unmanaged
        {
            if ( converter == null )
                converter = ColorConvert.GetConversionFrom<Representation , T>();
            Image<T> image = Image<T>.Create(Rect.Width,Rect.Height);
            for ( var x = 0; x < Width; x++ )
                for ( var y = 0; y < Height; y++ )
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
                if ( x < Rect.Width && x >= 0 && y < Rect.Height && y >= 0 )
                    return img[x + rct.X , y + rct.Y];
                return default(Representation);
            }
            set
            {
                if ( !Readonly && x < Rect.Width && x >= 0 && y < Rect.Height && y >= 0 )
                    img[x + rct.X , y + rct.Y] = value;
            }
        }
        /// <summary>
        /// Convert to image
        /// </summary>
        /// <param name="ip"></param>
        public static explicit operator Image<Representation>(ImageProxy<Representation> ip)
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

        public ImageProxy<B> As<B>()
            where B : unmanaged
        {
            ImageTypeProxy<Representation,B> proxy = new ImageTypeProxy<Representation, B>(this);
            return proxy;
        }
        public ImageProxy<B> As<B>(Func<Representation , B> converter)
            where B : unmanaged
        {
            ImageTypeProxy<Representation,B> proxy = new ImageTypeProxy<Representation, B>(this,converter);
            return proxy;
        }
    }

    /// <summary>
    /// Main use is ROI
    /// </summary>
    public class ImageTypeProxy<RepresentationA, RepresentationB> : ImageProxy<RepresentationB>
        where RepresentationA : unmanaged
        where RepresentationB : unmanaged
    {
        /// <summary>
        /// The proxying image
        /// </summary>
        protected ImageProxy<RepresentationA> imgP;

        Func<RepresentationA,RepresentationB> converter;
        Func<RepresentationB,RepresentationA> converter2;

        internal override Rectangle Rect { get => imgP.Rect; set => imgP.Rect = value; }
        /// <summary>
        /// Create a empty proxy
        /// </summary>
        protected ImageTypeProxy(bool rdonly , Func<RepresentationA , RepresentationB> converter) : base(rdonly || converter!=null)
        {
            if ( converter == null )
                converter = ColorConvert.GetConversionFrom<RepresentationA , RepresentationB>();
            this.converter = converter;
            converter2 = ColorConvert.GetConversionFrom<RepresentationB , RepresentationA>();
        }

        public ImageTypeProxy(ImageProxy<RepresentationA> t) : this(t.Readonly , null)
        {
            imgP = t;
        }
        public ImageTypeProxy(ImageProxy<RepresentationA> t , Func<RepresentationA , RepresentationB> converter) : this(t.Readonly , converter)
        {
            imgP = t;
        }


        /// <summary>
        /// Get a pixel from image
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <returns></returns>
        public override RepresentationB this[int x , int y]
        {
            get
            {
                return converter(imgP[x + rct.X , y + rct.Y]);
            }
            set
            {
                imgP[x + rct.X , y + rct.Y] = converter2(value);
            }
        }

    }
}
