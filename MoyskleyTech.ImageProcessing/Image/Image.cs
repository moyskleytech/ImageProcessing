using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace MoyskleyTech.ImageProcessing.Image
{
    /// <summary>
    /// Represent the base class for all images
    /// </summary>
    /// <typeparam name="Representation"></typeparam>
    [NotSerialized]
    public unsafe partial class Image<Representation>
        where Representation : struct
    {
        private bool disposed=false;
        /// <summary>
        /// size of image
        /// </summary>
        protected int width,height;
        /// <summary>
        /// Pointer for data
        /// </summary>
        protected byte* dataPointer;
        /// <summary>
        /// Lenght of an item(sizeof(Representation))
        /// </summary>
        private readonly int lengthOfItem;
        /// <summary>
        /// Pointer for data
        /// </summary>
        protected IntPtr raw;
        /// <summary>
        /// Flag to delete or not pointer when disposing
        /// </summary>
        protected bool deleteOnDispose=true;
        /// <summary>
        /// Allocate it using width and height
        /// </summary>
        /// <param name="w">Width</param>
        /// <param name="h">Height</param>
        public Image(int w , int h) : this(IntPtr.Zero , w , h)
        {

        }
        /// <summary>
        /// Create an image without allocating new memory
        /// </summary>
        /// <param name="ptr"></param>
        /// <param name="w"></param>
        /// <param name="h"></param>
        protected Image(IntPtr ptr , int w , int h)
        {
            lengthOfItem = Marshal.SizeOf(default(Representation));
            if ( ptr == IntPtr.Zero )
                raw = Marshal.AllocHGlobal(w * h * lengthOfItem);
            else
                raw = ptr;
            dataPointer = ( byte* ) raw.ToPointer();
            width = w;
            height = h;
            deleteOnDispose = ptr == IntPtr.Zero;
        }
        /// <summary>
        /// Create an image without allocating new memory
        /// </summary>
        /// <param name="ptr"></param>
        /// <param name="w"></param>
        /// <param name="h"></param>
        public static Image<Representation> UsingExistingMemoryPointer(IntPtr ptr , int w , int h)
        {
            return new Image<Representation>(ptr , w , h);
        }
        /// <summary>
        /// Width of bitmap
        /// </summary>
        public int Width { get { return width; } }
        /// <summary>
        /// Height of bitmap
        /// </summary>
        public int Height { get { return height; } }
        /// <summary>
        /// Return if the image has been destroyed
        /// </summary>
        public bool Disposed { get { return disposed; } }
        /// <summary>
        /// Destrop bitmap
        /// </summary>
        ~Image()
        {
            Dispose();
        }
        /// <summary>
        /// Get pixel from coordinate
        /// </summary>
        /// <param name="x">X position</param>
        /// <param name="y">Y position</param>
        /// <returns>Pixel</returns>
        public Representation Get(int x , int y)
        {
            return this[x , y];
        }
        /// <summary>
        /// Dispose the bitmap and release memory
        /// </summary>
        public void Dispose()
        {
            if ( deleteOnDispose && !disposed )
            {
                Marshal.FreeHGlobal(raw);
                disposed = true;
            }
            GC.SuppressFinalize(this);
        }
        /// <summary>
        /// Source to edit or copy
        /// </summary>
        public IntPtr DataPointer { get { return raw; } }
        /// <summary>
        /// Get pixel from coordinate
        /// </summary>
        /// <param name="x">X position</param>
        /// <param name="y">Y position</param>
        /// <returns>Pixel</returns>
        public Representation this[int x , int y]
        {
            get
            {
                return this[y * width + x];
            }
            set
            {
                this[y * width + x] = value;
            }
        }
        /// <summary>
        /// Get pixel from coordinate
        /// </summary>
        /// <param name="pos">As 1 dim array</param>
        /// <returns>Pixel</returns>
        public virtual Representation this[int pos]
        {
            get
            {
                //return ( Representation ) Marshal.PtrToStructure(this[y * width + x] , typeof(Representation));

                if ( pos > 0 && pos < width * height )
                    return ( Representation ) Marshal.PtrToStructure(raw + ( pos * lengthOfItem ) , typeof(Representation));
                else
                    return default(Representation);
            }
            set
            {
                if ( pos > 0 && pos < width * height )
                    Marshal.StructureToPtr(value , raw + ( pos * lengthOfItem ) , false);
            }
        }
        /// <summary>
        /// Copy data to another pointer
        /// </summary>
        /// <param name="dst"></param>
        public void CopyTo(IntPtr dst)
        {
            byte* src = dataPointer;
            byte* dest = (byte*)dst.ToPointer();
            for ( var i = 0; i < width * height * lengthOfItem; i++ )
                *dest++ = *src++;
        }
        /// <summary>
        /// Copy data from another pointer
        /// </summary>
        /// <param name="dst"></param>
        public void CopyFrom(IntPtr dst)
        {
            byte* src = (byte*)dst.ToPointer();
            byte* dest = dataPointer;
            for ( var i = 0; i < width * height * lengthOfItem; i++ )
                *dest++ = *src++;
        }
        /// <summary>
        /// Copy data to another pointer
        /// </summary>
        /// <param name="dst"></param>
        public void CopyTo(byte* dst)
        {
            byte* src = dataPointer;
            byte* dest = (byte*)dst;
            for ( var i = 0; i < width * height * lengthOfItem; i++ )
                *dest++ = *src++;
        }
        /// <summary>
        /// Copy data from another pointer
        /// </summary>
        /// <param name="dst"></param>
        public void CopyFrom(byte* dst)
        {
            byte* src = (byte*)dst;
            byte* dest = dataPointer;
            for ( var i = 0; i < width * height * lengthOfItem; i++ )
                *dest++ = *src++;
        }
        /// <summary>
        /// Allow conversion between colorspaces
        /// </summary>
        /// <typeparam name="NewRepresentation"></typeparam>
        /// <returns></returns>
        public Image<NewRepresentation> ConvertTo<NewRepresentation>()
            where NewRepresentation : struct
        {
            Image<NewRepresentation> image = Image<NewRepresentation>.Create(width,height);

            Func<Representation,NewRepresentation> converter = ColorConvert.GetConversionFrom<Representation,NewRepresentation>();

            if ( converter == null )
                throw new NotImplementedException("Specified conversion from " + typeof(Representation).Name + " to " + typeof(NewRepresentation).Name + " is not implemented");

            for ( var i = 0; i < width * height; i++ )
            {
                image[i] = converter(this[i]);
            }
            return image;
        }
        /// <summary>
        /// Allow conversion between colorspaces, transform the image buffer
        /// </summary>
        /// <typeparam name="NewRepresentation"></typeparam>
        /// <returns></returns>
        public Image<NewRepresentation> ConvertBufferTo<NewRepresentation>()
            where NewRepresentation : struct
        {
            Image<NewRepresentation> ret=null;
            var szN = Marshal.SizeOf(typeof(NewRepresentation));
            var szO = Marshal.SizeOf(typeof(Representation));
            if ( szN == szO )
            {
                ret = Image<NewRepresentation>.Create(width , height , raw);
                Func<Representation,NewRepresentation> converter = ColorConvert.GetConversionFrom<Representation,NewRepresentation>();
                ret.deleteOnDispose = deleteOnDispose;
                if ( converter == null )
                    throw new NotImplementedException("Specified conversion from " + typeof(Representation).Name + " to " + typeof(NewRepresentation).Name + " is not implemented");

                for ( var i = 0; i < width * height; i++ )
                {
                    ret[i] = converter(this[i]);
                }

                deleteOnDispose = false;
            }
            else if ( szN < szO )
            {
                ret = Image<NewRepresentation>.Create(width , height , raw);
                Func<Representation,NewRepresentation> converter = ColorConvert.GetConversionFrom<Representation,NewRepresentation>();
                ret.deleteOnDispose = deleteOnDispose;
                if ( converter == null )
                    throw new NotImplementedException("Specified conversion from " + typeof(Representation).Name + " to " + typeof(NewRepresentation).Name + " is not implemented");

                for ( var i = 0; i < width * height; i++ )
                {
                    ret[i] = converter(this[i]);
                }

                deleteOnDispose = false;
                var nPtr = Marshal.ReAllocHGlobal(raw , (IntPtr)(szN * width * height));
                if ( nPtr != raw )
                    ret = Image<NewRepresentation>.Create(width , height , nPtr);
            }
            else //if ( szN > szO )
            {
                var nPtr = Marshal.ReAllocHGlobal(raw , (IntPtr)(szN * width * height));
                var tmp = Create(width,height,nPtr);
                ret = Image<NewRepresentation>.Create(width , height , nPtr);

                Func<Representation,NewRepresentation> converter = ColorConvert.GetConversionFrom<Representation,NewRepresentation>();
                ret.deleteOnDispose = deleteOnDispose;
                if ( converter == null )
                    throw new NotImplementedException("Specified conversion from " + typeof(Representation).Name + " to " + typeof(NewRepresentation).Name + " is not implemented");

                for ( var i = width * height - 1; i >= 0; i-- )
                {
                    ret[i] = converter(tmp[i]);
                }
                deleteOnDispose = false;
            }
            this.dataPointer = ret.dataPointer;
            disposed = true;
            return ret;
        }
        /// <summary>
        /// Convert to another colorspace using a function
        /// </summary>
        /// <typeparam name="NewRepresentation"></typeparam>
        /// <param name="converter"></param>
        /// <returns></returns>
        public Image<NewRepresentation> ConvertUsing<NewRepresentation>(Func<Representation , NewRepresentation> converter)
          where NewRepresentation : struct
        {
            Image<NewRepresentation> image = Image<NewRepresentation>.Create(width,height);

            for ( var i = 0; i < width * height; i++ )
            {
                image[i] = converter(this[i]);
            }
            return image;
        }
        /// <summary>
        /// Allow to keep buffer after disposal, usefull if pointer must stay valid even if object is not used anymore. Must call Marshal.FreeHGlobal on DataPointer when finished
        /// </summary>
        public void PreserveBufferOnDispose()
        {
            deleteOnDispose = false;
        }
        /// <summary>
        /// Create an image prefilled with a data
        /// </summary>
        /// <param name="w"></param>
        /// <param name="h"></param>
        /// <param name="d"></param>
        /// <returns></returns>
        public static Image<Representation> FilledWith(int w , int h , Representation d)
        {
            Image<Representation> img = Image<Representation>.Create(w,h);
            int size = img.width*img.height;
            for ( var i = 0; i < size; i++ )
            {
                img[i] = d;
            }
            return img;
        }
        /// <summary>
        /// Apply a filter on an image
        /// </summary>
        /// <param name="func"></param>
        public void ApplyFilter(Func<Representation , Point , Representation> func)
        {
            for ( var y = 0; y < height; y++ )
                for ( var x = 0; x < width; x++ )
                    this[x , y] = func(this[x , y] , new Point(x , y));
        }
        /// <summary>
        /// Get a byte band in the data
        /// </summary>
        /// <param name="no"></param>
        /// <returns></returns>
        public Image<byte> GetByteBand(int no)
        {
            Image<byte> img = Image<byte>.Create(width,height);
            byte* src = dataPointer;
            byte* dest = (byte*)img.DataPointer.ToPointer();
            for ( var i = 0; i < width * height; i++ )
            {
                *dest++ = *src;
                src += lengthOfItem;
            }
            return img;
        }
        /// <summary>
        /// Set a band
        /// </summary>
        /// <param name="band"></param>
        /// <param name="no"></param>
        public void SetByteBand(Image<byte> band , int no)
        {
            byte* src = (byte*)band.DataPointer.ToPointer();
            byte* dest = dataPointer;
            for ( var i = 0; i < width * height; i++ )
            {
                *dest = *src;
                dest += lengthOfItem;
                src++;
            }
        }
        /// <summary>
        /// Get a float band in the data
        /// </summary>
        /// <param name="no"></param>
        /// <returns></returns>
        public Image<float> GetFloatBand(int no)
        {
            if ( no < lengthOfItem )
            {
                Image<float> img = Image<float>.Create(width,height);
                byte* src = dataPointer+no;
                float* dest = (float*)img.DataPointer.ToPointer();
                for ( var i = 0; i < width * height; i++ )
                {
                    *dest++ = *( ( float* ) src );
                    src += lengthOfItem;
                }
                return img;
            }
            return null;
        }
        /// <summary>
        /// Set a band
        /// </summary>
        /// <param name="band"></param>
        /// <param name="no"></param>
        public void SetFloatBand(Image<float> band , int no)
        {
            if ( ( no + 1 ) * sizeof(float) <= lengthOfItem )
            {
                float* src = (float*)band.DataPointer.ToPointer();
                byte* dest = dataPointer+no*sizeof(float);
                for ( var i = 0; i < width * height; i++ )
                {
                    *( ( float* ) dest ) = *src;
                    dest += lengthOfItem;
                    src++;
                }
            }
        }
        /// <summary>
        /// Crop
        /// </summary>
        /// <param name="location"></param>
        /// <returns></returns>
        public Image<Representation> GetSubImage(Rectangle location)
        {
            if ( location.Bottom > this.height )
                location.Height = this.height - location.Top;
            if ( location.Right > this.width )
                location.Width = this.width - location.Left;

            Image<Representation> bmp = Create(location.Width , location.Height);
            for ( int x1 = 0, x = location.Left; x <= location.Right && x < width; x++, x1++ )
            {
                for ( int y1 = 0, y = location.Top; y <= location.Bottom && y < height; y++, y1++ )
                {
                    bmp[x1 , y1] = this[x , y];
                }
            }
            return bmp;
        }
        /// <summary>
        /// Optimized creator(will select best subtype)
        /// </summary>
        /// <param name="width"></param>
        /// <param name="height"></param>
        /// <param name="data">Pointer to use as storage</param>
        /// <returns></returns>
        public static Image<Representation> Create(int width , int height , IntPtr? data = null)
        {
            if ( data == null )
                data = IntPtr.Zero;
            if ( typeof(Representation) == typeof(Pixel) )
                return ( Image<Representation> ) ( object ) new Bitmap(data.Value , width , height);
            if ( typeof(Representation) == typeof(ARGB_Float) )
                return ( Image<Representation> ) ( object ) new SuperHighRangeImage(data.Value , width , height);
            if ( typeof(Representation) == typeof(ARGB_16bit) )
                return ( Image<Representation> ) ( object ) new HighRangeImage(data.Value , width , height);
            if ( typeof(Representation) == typeof(bool) )
                return ( Image<Representation> ) ( object ) new BoolImage(data.Value , width , height);
            if ( typeof(Representation) == typeof(byte) )
                return ( Image<Representation> ) ( object ) new OneBandImage(data.Value , width , height);
            if ( typeof(Representation) == typeof(float) )
                return ( Image<Representation> ) ( object ) new OneBandFloatImage(data.Value , width , height);
            if ( typeof(Representation) == typeof(HSB) )
                return ( Image<Representation> ) ( object ) new HSBImage(data.Value , width , height);
            if ( typeof(Representation) == typeof(HSBA) )
                return ( Image<Representation> ) ( object ) new HSBAImage(data.Value , width , height);
            if ( typeof(Representation) == typeof(_1555) )
                return ( Image<Representation> ) ( object ) new _1555Image(data.Value , width , height);
            if ( typeof(Representation) == typeof(_555) )
                return ( Image<Representation> ) ( object ) new _555Image(data.Value , width , height);
            if ( typeof(Representation) == typeof(_565) )
                return ( Image<Representation> ) ( object ) new _565Image(data.Value , width , height);
            if ( typeof(Representation) == typeof(CYMK) )
                return ( Image<Representation> ) ( object ) new CYMKImage(data.Value , width , height);
            if ( typeof(Representation) == typeof(_332) )
                return ( Image<Representation> ) ( object ) new _332Image(data.Value , width , height);
            if ( typeof(Representation) == typeof(HSB_Float) )
                return ( Image<Representation> ) ( object ) new HSB_FloatImage(data.Value , width , height);
            if ( typeof(Representation) == typeof(RGB) )
                return ( Image<Representation> ) ( object ) new RGBImage(data.Value , width , height);
            if ( typeof(Representation) == typeof(BGR) )
                return ( Image<Representation> ) ( object ) new BGRImage(data.Value , width , height);

            if ( typeof(Representation) == typeof(RGBA) )
                return ( Image<Representation> ) ( object ) new RGBAImage(data.Value , width , height);
            if ( typeof(Representation) == typeof(BGRA) )
                return ( Image<Representation> ) ( object ) new BGRAImage(data.Value , width , height);
            if ( typeof(Representation) == typeof(ARGB) )
                return ( Image<Representation> ) ( object ) new ARGBImage(data.Value , width , height);
            if ( typeof(Representation) == typeof(ABGR) )
                return ( Image<Representation> ) ( object ) new ABGRImage(data.Value , width , height);

            if ( typeof(Representation) == typeof(HSL) )
                return ( Image<Representation> ) ( object ) new HSLImage(data.Value , width , height);

            return new Image<Representation>(width , height);
        }
        /// <summary>
        /// Crop
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <param name="w"></param>
        /// <param name="h"></param>
        /// <returns></returns>
        public Image<Representation> GetImage(int x , int y , int w , int h)
        {
            return GetSubImage(new Rectangle(x , y , w , h));
        }
        /// <summary>
        /// Get a part of the image
        /// </summary>
        /// <param name="rectangle"></param>
        /// <returns></returns>
        public Image<Representation> Crop(Rectangle rectangle)
        { return GetSubImage(rectangle); }
        /// <summary>
        /// Create a proxy of a region of the image(ROI)
        /// </summary>
        /// <param name="rectangle"></param>
        /// <returns></returns>
        public ImageProxy<Representation> Proxy(Rectangle rectangle)
        {
            return new ImageProxy<Representation>(this , rectangle);
        }
        /// <summary>
        /// Rotate and flip an image
        /// </summary>
        /// <param name="rotateFlipType"></param>
        /// <returns></returns>
        public Image<Representation> RotateFlip(RotateFlipType rotateFlipType)
        {
            Image<Representation> output=null;
            var rotation = (RotateFlipType)((int)rotateFlipType&3);

            if ( rotation == RotateFlipType.Rotate90 )
            {
                output = Image<Representation>.Create(height , width);
                for ( var x = 0; x < width; x++ )
                    for ( var y = 0; y < height; y++ )
                        output[height - y - 1 , x] = this[x , y];
            }
            else if ( rotation == RotateFlipType.Rotate180 )
            {
                output = Clone();
                for ( int x = 0, x2 = output.width - 1; x < x2; x++, x2-- )
                    for ( var y = 0; y < output.height; y++ )
                    {
                        var tmp = output[x2,y];
                        output[x2 , y] = output[x , y];
                        output[x , y] = tmp;
                    }
                for ( int x = 0; x < output.width; x++ )
                    for ( int y = 0, y2 = output.height - 1; y < y2; y++, y2-- )
                    {
                        var tmp = output[x,y2];
                        output[x , y2] = output[x , y];
                        output[x , y] = tmp;
                    }
            }
            else if ( rotation == RotateFlipType.Rotate270 )
            {
                output = Image<Representation>.Create(height , width);
                for ( var x = 0; x < width; x++ )
                    for ( var y = 0; y < height; y++ )
                        output[y , width - x - 1] = this[x , y];
            }
            else if ( rotation == RotateFlipType.RotateNone )
                output = Clone();


            if ( ( rotateFlipType & RotateFlipType.FlipX ) == RotateFlipType.FlipX )
                for ( int x = 0, x2 = output.width - 1; x < x2; x++, x2-- )
                    for ( var y = 0; y < output.height; y++ )
                    {
                        var tmp = output[x2,y];
                        output[x2 , y] = output[x , y];
                        output[x , y] = tmp;
                    }

            if ( ( rotateFlipType & RotateFlipType.FlipY ) == RotateFlipType.FlipY )
                for ( int x = 0; x < output.width; x++ )
                    for ( int y = 0, y2 = output.height - 1; y < y2; y++, y2-- )
                    {
                        var tmp = output[x,y2];
                        output[x , y2] = output[x , y];
                        output[x , y] = tmp;
                    }

            return output;
        }
        /// <summary>
        /// Clone an image(Will choose acording to optimized constructor, custom types will not be considered)
        /// </summary>
        /// <returns></returns>
        public Image<Representation> Clone()
        {
            Image<Representation> dest = Create(width,height);
            dest.CopyFrom(dataPointer);
            return dest;
        }
        public virtual Representation Get(double x , double y)
        {
            return Get(( int ) x , ( int ) y);
        }
        public virtual Representation Average(double x,double y,double w,double h)
        {
            return Get(x + w / 2 , y + w / 2);
        }
        public Representation Get(PointF pt)
        {
            return Get(pt.X , pt.Y);
        }
        public Representation Average(RectangleF r)
        {
            return Average(r.X,r.Y,r.Width,r.Height);
        }
    }
    /// <summary>
    /// Optimized image for ARGB_16bit
    /// </summary>
    [NotSerialized]
    public unsafe partial class HighRangeImage : Image<ARGB_16bit>
    {
        ARGB_16bit* ptr;
        /// <summary>
        /// Optimized image for ARGB_16bit
        /// </summary>
        public HighRangeImage(int w , int h) : base(w , h)
        {
            ptr = ( ARGB_16bit* ) base.dataPointer;
        }
        /// <summary>
        /// Optimized image for ARGB_16bit
        /// </summary>
        public HighRangeImage(IntPtr data , int w , int h) : base(data , w , h)
        {
            ptr = ( ARGB_16bit* ) base.dataPointer;
        }
        /// <summary>
        /// Accessor
        /// </summary>
        /// <param name="pos"></param>
        /// <returns></returns>
        public override ARGB_16bit this[int pos] { get => ptr[pos]; set => ptr[pos] = value; }

        public override ARGB_16bit Get(double x , double y)
        {
            var ix = (int)x;
            var iy = (int)y;
            var dx = x-ix;
            var dy = y-iy;

            if ( dx == 0 && dy == 0 )
                return Get(ix , iy);

            var dx2 = 1-dx;
            var dy2 = 1-dy;

            var ipx = this[ix,iy];

            double sa=ipx.A,sr=ipx.R,sg=ipx.G,sb=ipx.B;
            if ( ix > 0 )
            {
                var px = this[ix , iy];
                sa += px.A * dx2;
                sr += px.R * dx2;
                sg += px.G * dx2;
                sb += px.B * dx2;
            }
            if ( iy > 0 )
            {
                var px=this[ix , iy];
                sa += px.A * dy2;
                sr += px.R * dy2;
                sg += px.G * dy2;
                sb += px.B * dy2;
            }
            if ( ix < width - 1 )
            {
                var px = this[ix + 1 , iy];
                sa += px.A * dx;
                sr += px.R * dx;
                sg += px.G * dx;
                sb += px.B * dx;
            }
            if ( iy < height - 1 )
            {
                var px = this[ix,iy+1];
                sa += px.A * dy;
                sr += px.R * dy;
                sg += px.G * dy;
                sb += px.B * dy;
            }
            ARGB_16bit destinationpx = new ARGB_16bit()
            {
                R = ( ushort ) ( sr / 3 ),
                G = ( ushort ) ( sg / 3 ),
                B = ( ushort ) ( sb / 3 ),
                A = ( ushort ) ( sa / 3 )
            };
            return destinationpx;
        }
        public override ARGB_16bit Average(double x , double y , double w , double h)
        {
            var sex = x+w;
            var sey = y+w;
            var sx = (int)x;
            var sy = (int)y;

            ulong count=0;
            ulong acount=0;
            ulong sa=0,sr=0,sg=0,sb=0;
            for ( var i = sx; i < sex; i++ )
            {
                for ( var j = sy; j < sey; j++ )
                {
                    count++;
                    ARGB_16bit source = this[i,j];
                    acount += source.A;
                    sa += source.A;
                    sr += source.R;
                    sg += source.G;
                    sb += source.B;
                }
            }
            ARGB_16bit destinationpx = new ARGB_16bit
            {
                A = ( ushort ) ( sa / count )
            };
            if ( acount > 0 )
            {
                destinationpx.R = ( ushort ) ( sr * ushort.MaxValue / acount );
                destinationpx.G = ( ushort ) ( sg * ushort.MaxValue / acount );
                destinationpx.B = ( ushort ) ( sb * ushort.MaxValue / acount );
            }
            return destinationpx;
        }
    }
    /// <summary>
    /// Optimized image for ARGB_Float
    /// </summary>
    [NotSerialized]
    public unsafe partial class SuperHighRangeImage : Image<ARGB_Float>
    {
        ARGB_Float* ptr;
        /// <summary>
        /// Optimized image for ARGB_Float
        /// </summary>
        public SuperHighRangeImage(int w , int h) : base(w , h)
        {
            ptr = ( ARGB_Float* ) base.dataPointer;
        }
        /// <summary>
        /// Optimized image for ARGB_Float
        /// </summary>
        public SuperHighRangeImage(IntPtr data , int w , int h) : base(data , w , h)
        {
            ptr = ( ARGB_Float* ) base.dataPointer;
        }
        /// <summary>
        /// Accessor
        /// </summary>
        /// <param name="pos"></param>
        /// <returns></returns>
        public override ARGB_Float this[int pos] { get => ptr[pos]; set => ptr[pos] = value; }

        public override ARGB_Float Get(double x , double y)
        {
            var ix = (int)x;
            var iy = (int)y;
            var dx = x-ix;
            var dy = y-iy;

            if ( dx == 0 && dy == 0 )
                return Get(ix , iy);

            var dx2 = 1-dx;
            var dy2 = 1-dy;

            var ipx = this[ix,iy];

            double sa=ipx.A,sr=ipx.R,sg=ipx.G,sb=ipx.B;
            if ( ix > 0 )
            {
                var px = this[ix , iy];
                sa += px.A * dx2;
                sr += px.R * dx2;
                sg += px.G * dx2;
                sb += px.B * dx2;
            }
            if ( iy > 0 )
            {
                var px=this[ix , iy];
                sa += px.A * dy2;
                sr += px.R * dy2;
                sg += px.G * dy2;
                sb += px.B * dy2;
            }
            if ( ix < width - 1 )
            {
                var px = this[ix + 1 , iy];
                sa += px.A * dx;
                sr += px.R * dx;
                sg += px.G * dx;
                sb += px.B * dx;
            }
            if ( iy < height - 1 )
            {
                var px = this[ix,iy+1];
                sa += px.A * dy;
                sr += px.R * dy;
                sg += px.G * dy;
                sb += px.B * dy;
            }
            ARGB_Float destinationpx = new ARGB_Float()
            {
                R = ( float ) ( sr / 3 ),
                G = ( float ) ( sg / 3 ),
                B = ( float ) ( sb / 3 ),
                A = ( float ) ( sa / 3 )
            };
            return destinationpx;
        }
        public override ARGB_Float Average(double x , double y , double w , double h)
        {
            var sex = x+w;
            var sey = y+w;
            var sx = (int)x;
            var sy = (int)y;

            double count=0;
            double acount=0;
            double sa=0,sr=0,sg=0,sb=0;
            for ( var i = sx; i < sex; i++ )
            {
                for ( var j = sy; j < sey; j++ )
                {
                    count++;
                    ARGB_Float source = this[i,j];
                    acount += source.A;
                    sa += source.A;
                    sr += source.R;
                    sg += source.G;
                    sb += source.B;
                }
            }
            ARGB_Float destinationpx = new ARGB_Float
            {
                A = ( float ) ( sa / count )
            };
            if ( acount > 0 )
            {
                destinationpx.R = ( float ) ( sr  / acount );
                destinationpx.G = ( float ) ( sg  / acount );
                destinationpx.B = ( float ) ( sb  / acount );
            }
            return destinationpx;
        }
    }
    /// <summary>
    /// Optimized image for float
    /// </summary>
    [NotSerialized]
    public unsafe partial class OneBandFloatImage : Image<float>
    {
        float* ptr;
        /// <summary>
        /// Optimized image for float
        /// </summary>
        public OneBandFloatImage(int w , int h) : base(w , h)
        {
            ptr = ( float* ) base.dataPointer;
        }
        /// <summary>
        /// Optimized image for float
        /// </summary>
        public OneBandFloatImage(IntPtr data , int w , int h) : base(data , w , h)
        {
            ptr = ( float* ) base.dataPointer;
        }
        /// <summary>
        /// Accessor
        /// </summary>
        /// <param name="pos"></param>
        /// <returns></returns>
        public override float this[int pos] { get => ptr[pos]; set => ptr[pos] = value; }

        public override float Get(double x , double y)
        {
            var ix = (int)x;
            var iy = (int)y;
            var dx = x-ix;
            var dy = y-iy;

            if ( dx == 0 && dy == 0 )
                return Get(ix , iy);

            var dx2 = 1-dx;
            var dy2 = 1-dy;

            var ipx = this[ix,iy];

            double sa=ipx;
            if ( ix > 0 )
            {
                var px = this[ix , iy];
                sa += px * dx2;
            }
            if ( iy > 0 )
            {
                var px=this[ix , iy];
                sa += px * dy2;
            }
            if ( ix < width - 1 )
            {
                var px = this[ix + 1 , iy];
                sa += px * dx;
            }
            if ( iy < height - 1 )
            {
                var px = this[ix,iy+1];
                sa += px * dy;
            }
            return (float)sa/3;
        }
        public override float Average(double x , double y , double w , double h)
        {
            var sex = x+w;
            var sey = y+w;
            var sx = (int)x;
            var sy = (int)y;

            ulong count=0;
            double acount=0;
            for ( var i = sx; i < sex; i++ )
            {
                for ( var j = sy; j < sey; j++ )
                {
                    count++;
                    float source = this[i,j];
                    acount += source;
                }
            }
            
            return (float)acount/count;
        }
    }
    /// <summary>
    /// Optimized image for _1555
    /// </summary>
    [NotSerialized]
    public unsafe partial class _1555Image : Image<_1555>
    {
        _1555* ptr;
        /// <summary>
        /// Optimized image for _1555
        /// </summary>
        public _1555Image(int w , int h) : base(w , h)
        {
            ptr = ( _1555* ) base.dataPointer;
        }
        /// <summary>
        /// Optimized image for _1555
        /// </summary>
        public _1555Image(IntPtr data , int w , int h) : base(data , w , h)
        {
            ptr = ( _1555* ) base.dataPointer;
        }
        /// <summary>
        /// Accessor
        /// </summary>
        /// <param name="pos"></param>
        /// <returns></returns>
        public override _1555 this[int pos] { get => ptr[pos]; set => ptr[pos] = value; }
    }
    /// <summary>
    /// Optimized image for _555
    /// </summary>
    [NotSerialized]
    public unsafe partial class _555Image : Image<_555>
    {
        _555* ptr;
        /// <summary>
        /// Optimized image for _555
        /// </summary>
        public _555Image(int w , int h) : base(w , h)
        {
            ptr = ( _555* ) base.dataPointer;
        }
        /// <summary>
        /// Optimized image for _555
        /// </summary>
        public _555Image(IntPtr data , int w , int h) : base(data , w , h)
        {
            ptr = ( _555* ) base.dataPointer;
        }
        /// <summary>
        /// Accessor
        /// </summary>
        /// <param name="pos"></param>
        /// <returns></returns>
        public override _555 this[int pos] { get => ptr[pos]; set => ptr[pos] = value; }
    }
    /// <summary>
    /// Optimized image for _565
    /// </summary>
    [NotSerialized]
    public unsafe partial class _565Image : Image<_565>
    {
        _565* ptr;
        /// <summary>
        /// Optimized image for _565
        /// </summary>
        public _565Image(int w , int h) : base(w , h)
        {
            ptr = ( _565* ) base.dataPointer;
        }
        /// <summary>
        /// Optimized image for _565
        /// </summary>
        public _565Image(IntPtr data , int w , int h) : base(data , w , h)
        {
            ptr = ( _565* ) base.dataPointer;
        }
        /// <summary>
        /// Accessor
        /// </summary>
        /// <param name="pos"></param>
        /// <returns></returns>
        public override _565 this[int pos] { get => ptr[pos]; set => ptr[pos] = value; }
    }
    /// <summary>
    /// Optimized image for HSBA
    /// </summary>
    [NotSerialized]
    public unsafe partial class HSBAImage : Image<HSBA>
    {
        HSBA* ptr;
        /// <summary>
        /// Optimized image for HSBA
        /// </summary>
        public HSBAImage(int w , int h) : base(w , h)
        {
            ptr = ( HSBA* ) base.dataPointer;
        }
        /// <summary>
        /// Optimized image for HSBA
        /// </summary>
        public HSBAImage(IntPtr data , int w , int h) : base(data , w , h)
        {
            ptr = ( HSBA* ) base.dataPointer;
        }
        /// <summary>
        /// Accessor
        /// </summary>
        /// <param name="pos"></param>
        /// <returns></returns>
        public override HSBA this[int pos] { get => ptr[pos]; set => ptr[pos] = value; }
    }
    /// <summary>
    /// Optimized image for CYMK
    /// </summary>
    [NotSerialized]
    public unsafe partial class CYMKImage : Image<CYMK>
    {
        CYMK* ptr;
        /// <summary>
        /// Optimized image for CYMK
        /// </summary>
        public CYMKImage(int w , int h) : base(w , h)
        {
            ptr = ( CYMK* ) base.dataPointer;
        }
        /// <summary>
        /// Optimized image for CYMK
        /// </summary>
        public CYMKImage(IntPtr data , int w , int h) : base(data , w , h)
        {
            ptr = ( CYMK* ) base.dataPointer;
        }
        /// <summary>
        /// Accessor
        /// </summary>
        /// <param name="pos"></param>
        /// <returns></returns>
        public override CYMK this[int pos] { get => ptr[pos]; set => ptr[pos] = value; }
    }
    /// <summary>
    /// Optimized image for HSL
    /// </summary>
    [NotSerialized]
    public unsafe partial class HSLImage : Image<HSL>
    {
        HSL* ptr;
        /// <summary>
        /// Optimized image for HSL
        /// </summary>
        public HSLImage(int w , int h) : base(w , h)
        {
            ptr = ( HSL* ) base.dataPointer;
        }
        /// <summary>
        /// Optimized image for HSL
        /// </summary>
        public HSLImage(IntPtr data , int w , int h) : base(data , w , h)
        {
            ptr = ( HSL* ) base.dataPointer;
        }
        /// <summary>
        /// Accessor
        /// </summary>
        /// <param name="pos"></param>
        /// <returns></returns>
        public override HSL this[int pos] { get => ptr[pos]; set => ptr[pos] = value; }
    }
    /// <summary>
    /// Optimized image for HSB_Float
    /// </summary>
    [NotSerialized]
    public unsafe partial class HSB_FloatImage : Image<HSB_Float>
    {
        HSB_Float* ptr;
        /// <summary>
        /// Optimized image for HSB_Float
        /// </summary>
        public HSB_FloatImage(int w , int h) : base(w , h)
        {
            ptr = ( HSB_Float* ) base.dataPointer;
        }
        /// <summary>
        /// Optimized image for HSB_Float
        /// </summary>
        public HSB_FloatImage(IntPtr data , int w , int h) : base(data , w , h)
        {
            ptr = ( HSB_Float* ) base.dataPointer;
        }
        /// <summary>
        /// Accessor
        /// </summary>
        /// <param name="pos"></param>
        /// <returns></returns>
        public override HSB_Float this[int pos] { get => ptr[pos]; set => ptr[pos] = value; }
    }
    /// <summary>
    /// Optimized image for BGR
    /// </summary>
    [NotSerialized]
    public unsafe partial class BGRImage : Image<BGR>
    {
        BGR* ptr;
        /// <summary>
        /// Optimized image for BGR
        /// </summary>
        public BGRImage(int w , int h) : base(w , h)
        {
            ptr = ( BGR* ) base.dataPointer;
        }
        /// <summary>
        /// Optimized image for BGR
        /// </summary>
        public BGRImage(IntPtr data , int w , int h) : base(data , w , h)
        {
            ptr = ( BGR* ) base.dataPointer;
        }
        /// <summary>
        /// Accessor
        /// </summary>
        /// <param name="pos"></param>
        /// <returns></returns>
        public override BGR this[int pos] { get => ptr[pos]; set => ptr[pos] = value; }


        public override BGR Get(double x , double y)
        {
            var ix = (int)x;
            var iy = (int)y;
            var dx = x-ix;
            var dy = y-iy;

            if ( dx == 0 && dy == 0 )
                return Get(ix , iy);

            var dx2 = 1-dx;
            var dy2 = 1-dy;

            var ipx = this[ix,iy];

            double sr=ipx.R,sg=ipx.G,sb=ipx.B;
            if ( ix > 0 )
            {
                var px = this[ix , iy];
                sr += px.R * dx2;
                sg += px.G * dx2;
                sb += px.B * dx2;
            }
            if ( iy > 0 )
            {
                var px=this[ix , iy];
                sr += px.R * dy2;
                sg += px.G * dy2;
                sb += px.B * dy2;
            }
            if ( ix < width - 1 )
            {
                var px = this[ix + 1 , iy];
                sr += px.R * dx;
                sg += px.G * dx;
                sb += px.B * dx;
            }
            if ( iy < height - 1 )
            {
                var px = this[ix,iy+1];
                sr += px.R * dy;
                sg += px.G * dy;
                sb += px.B * dy;
            }
            BGR destinationpx = new BGR()
            {
                R = ( byte ) ( sr / 3 ),
                G = ( byte ) ( sg / 3 ),
                B = ( byte ) ( sb / 3 )
            };
            return destinationpx;
        }
        public override BGR Average(double x , double y , double w , double h)
        {
            var sex = x+w;
            var sey = y+w;
            var sx = (int)x;
            var sy = (int)y;

            ulong count=0;
            ulong sa=0,sr=0,sg=0,sb=0;
            for ( var i = sx; i < sex; i++ )
            {
                for ( var j = sy; j < sey; j++ )
                {
                    count++;
                    BGR source = this[i,j];
                    sr += source.R;
                    sg += source.G;
                    sb += source.B;
                }
            }
            BGR destinationpx = new BGR
            {
                B = ( byte ) ( sr / count ),
                G = ( byte ) ( sr / count ),
                R = ( byte ) ( sr / count )
            };
            return destinationpx;
        }
    }
    /// <summary>
    /// Optimized image for RGB
    /// </summary>
    [NotSerialized]
    public unsafe partial class RGBImage : Image<RGB>
    {
        RGB* ptr;
        /// <summary>
        /// Optimized image for RGB
        /// </summary>
        public RGBImage(int w , int h) : base(w , h)
        {
            ptr = ( RGB* ) base.dataPointer;
        }
        /// <summary>
        /// Optimized image for RGB
        /// </summary>
        public RGBImage(IntPtr data , int w , int h) : base(data , w , h)
        {
            ptr = ( RGB* ) base.dataPointer;
        }
        /// <summary>
        /// Accessor
        /// </summary>
        /// <param name="pos"></param>
        /// <returns></returns>
        public override RGB this[int pos] { get => ptr[pos]; set => ptr[pos] = value; }


        public override RGB Get(double x , double y)
        {
            var ix = (int)x;
            var iy = (int)y;
            var dx = x-ix;
            var dy = y-iy;

            if ( dx == 0 && dy == 0 )
                return Get(ix , iy);

            var dx2 = 1-dx;
            var dy2 = 1-dy;

            var ipx = this[ix,iy];

            double sr=ipx.R,sg=ipx.G,sb=ipx.B;
            if ( ix > 0 )
            {
                var px = this[ix , iy];
                sr += px.R * dx2;
                sg += px.G * dx2;
                sb += px.B * dx2;
            }
            if ( iy > 0 )
            {
                var px=this[ix , iy];
                sr += px.R * dy2;
                sg += px.G * dy2;
                sb += px.B * dy2;
            }
            if ( ix < width - 1 )
            {
                var px = this[ix + 1 , iy];
                sr += px.R * dx;
                sg += px.G * dx;
                sb += px.B * dx;
            }
            if ( iy < height - 1 )
            {
                var px = this[ix,iy+1];
                sr += px.R * dy;
                sg += px.G * dy;
                sb += px.B * dy;
            }
            RGB destinationpx = new RGB()
            {
                R = ( byte ) ( sr / 3 ),
                G = ( byte ) ( sg / 3 ),
                B = ( byte ) ( sb / 3 )
            };
            return destinationpx;
        }
        public override RGB Average(double x , double y , double w , double h)
        {
            var sex = x+w;
            var sey = y+w;
            var sx = (int)x;
            var sy = (int)y;

            ulong count=0;
            ulong sa=0,sr=0,sg=0,sb=0;
            for ( var i = sx; i < sex; i++ )
            {
                for ( var j = sy; j < sey; j++ )
                {
                    count++;
                    RGB source = this[i,j];
                    sr += source.R;
                    sg += source.G;
                    sb += source.B;
                }
            }
            RGB destinationpx = new RGB
            {
                B = ( byte ) ( sr / count ),
                G = ( byte ) ( sr / count ),
                R = ( byte ) ( sr / count )
            };
            return destinationpx;
        }
    }
    /// <summary>
    /// Optimized image for _332
    /// </summary>
    [NotSerialized]
    public unsafe partial class _332Image : Image<_332>
    {
        _332* ptr;
        /// <summary>
        /// Optimized image for _332
        /// </summary>
        public _332Image(int w , int h) : base(w , h)
        {
            ptr = ( _332* ) base.dataPointer;
        }
        /// <summary>
        /// Optimized image for _332
        /// </summary>
        public _332Image(IntPtr data , int w , int h) : base(data , w , h)
        {
            ptr = ( _332* ) base.dataPointer;
        }
        /// <summary>
        /// Accessor
        /// </summary>
        /// <param name="pos"></param>
        /// <returns></returns>
        public override _332 this[int pos] { get => ptr[pos]; set => ptr[pos] = value; }
    }
    /// <summary>
    /// Optimized image for Bool
    /// </summary>
    [NotSerialized]
    public unsafe partial class BoolImage : Image<bool>
    {
        bool* ptr;
        /// <summary>
        /// Optimized image for Bool
        /// </summary>
        public BoolImage(int w , int h) : base(w , h)
        {
            ptr = ( bool* ) base.dataPointer;
        }
        /// <summary>
        /// Optimized image for bool
        /// </summary>
        public BoolImage(IntPtr data , int w , int h) : base(data , w , h)
        {
            ptr = ( bool* ) base.dataPointer;
        }
        /// <summary>
        /// Accessor
        /// </summary>
        /// <param name="pos"></param>
        /// <returns></returns>
        public override bool this[int pos] { get => ptr[pos]; set => ptr[pos] = value; }
    }


    /// <summary>
    /// Optimized image for RGB
    /// </summary>
    [NotSerialized]
    public unsafe partial class RGBAImage : Image<RGBA>
    {
        RGBA* ptr;
        /// <summary>
        /// Optimized image for RGB
        /// </summary>
        public RGBAImage(int w , int h) : base(w , h)
        {
            ptr = ( RGBA* ) base.dataPointer;
        }
        /// <summary>
        /// Optimized image for RGB
        /// </summary>
        public RGBAImage(IntPtr data , int w , int h) : base(data , w , h)
        {
            ptr = ( RGBA* ) base.dataPointer;
        }
        /// <summary>
        /// Accessor
        /// </summary>
        /// <param name="pos"></param>
        /// <returns></returns>
        public override RGBA this[int pos] { get => ptr[pos]; set => ptr[pos] = value; }

        public override RGBA Get(double x , double y)
        {
            var ix = (int)x;
            var iy = (int)y;
            var dx = x-ix;
            var dy = y-iy;

            if ( dx == 0 && dy == 0 )
                return Get(ix , iy);

            var dx2 = 1-dx;
            var dy2 = 1-dy;

            var ipx = this[ix,iy];

            double sa=ipx.A,sr=ipx.R,sg=ipx.G,sb=ipx.B;
            if ( ix > 0 )
            {
                var px = this[ix , iy];
                sa += px.A * dx2;
                sr += px.R * dx2;
                sg += px.G * dx2;
                sb += px.B * dx2;
            }
            if ( iy > 0 )
            {
                var px=this[ix , iy];
                sa += px.A * dy2;
                sr += px.R * dy2;
                sg += px.G * dy2;
                sb += px.B * dy2;
            }
            if ( ix < width - 1 )
            {
                var px = this[ix + 1 , iy];
                sa += px.A * dx;
                sr += px.R * dx;
                sg += px.G * dx;
                sb += px.B * dx;
            }
            if ( iy < height - 1 )
            {
                var px = this[ix,iy+1];
                sa += px.A * dy;
                sr += px.R * dy;
                sg += px.G * dy;
                sb += px.B * dy;
            }
            RGBA destinationpx = new RGBA()
            {
                R = ( byte ) ( sr / 3 ),
                G = ( byte ) ( sg / 3 ),
                B = ( byte ) ( sb / 3 ),
                A = ( byte ) ( sa / 3 )
            };
            return destinationpx;
        }
        public override RGBA Average(double x , double y , double w , double h)
        {
            var sex = x+w;
            var sey = y+w;
            var sx = (int)x;
            var sy = (int)y;

            ulong count=0;
            ulong acount=0;
            ulong sa=0,sr=0,sg=0,sb=0;
            for ( var i = sx; i < sex; i++ )
            {
                for ( var j = sy; j < sey; j++ )
                {
                    count++;
                    RGBA source = this[i,j];
                    acount += source.A;
                    sa += source.A;
                    sr += source.R;
                    sg += source.G;
                    sb += source.B;
                }
            }
            RGBA destinationpx = new RGBA
            {
                A = ( byte ) ( sa / count )
            };
            if ( acount > 0 )
            {
                destinationpx.R = ( byte ) ( sr * 255 / acount );
                destinationpx.G = ( byte ) ( sg * 255 / acount );
                destinationpx.B = ( byte ) ( sb * 255 / acount );
            }
            return destinationpx;
        }
    }
    /// <summary>
    /// Optimized image for RGB
    /// </summary>
    [NotSerialized]
    public unsafe partial class ARGBImage : Image<ARGB>
    {
        ARGB* ptr;
        /// <summary>
        /// Optimized image for RGB
        /// </summary>
        public ARGBImage(int w , int h) : base(w , h)
        {
            ptr = ( ARGB* ) base.dataPointer;
        }
        /// <summary>
        /// Optimized image for RGB
        /// </summary>
        public ARGBImage(IntPtr data , int w , int h) : base(data , w , h)
        {
            ptr = ( ARGB* ) base.dataPointer;
        }
        /// <summary>
        /// Accessor
        /// </summary>
        /// <param name="pos"></param>
        /// <returns></returns>
        public override ARGB this[int pos] { get => ptr[pos]; set => ptr[pos] = value; }

        public override ARGB Get(double x , double y)
        {
            var ix = (int)x;
            var iy = (int)y;
            var dx = x-ix;
            var dy = y-iy;

            if ( dx == 0 && dy == 0 )
                return Get(ix , iy);

            var dx2 = 1-dx;
            var dy2 = 1-dy;

            var ipx = this[ix,iy];

            double sa=ipx.A,sr=ipx.R,sg=ipx.G,sb=ipx.B;
            if ( ix > 0 )
            {
                var px = this[ix , iy];
                sa += px.A * dx2;
                sr += px.R * dx2;
                sg += px.G * dx2;
                sb += px.B * dx2;
            }
            if ( iy > 0 )
            {
                var px=this[ix , iy];
                sa += px.A * dy2;
                sr += px.R * dy2;
                sg += px.G * dy2;
                sb += px.B * dy2;
            }
            if ( ix < width - 1 )
            {
                var px = this[ix + 1 , iy];
                sa += px.A * dx;
                sr += px.R * dx;
                sg += px.G * dx;
                sb += px.B * dx;
            }
            if ( iy < height - 1 )
            {
                var px = this[ix,iy+1];
                sa += px.A * dy;
                sr += px.R * dy;
                sg += px.G * dy;
                sb += px.B * dy;
            }
            ARGB destinationpx = new ARGB()
            {
                R = ( byte ) ( sr / 3 ),
                G = ( byte ) ( sg / 3 ),
                B = ( byte ) ( sb / 3 ),
                A = ( byte ) ( sa / 3 )
            };
            return destinationpx;
        }
        public override ARGB Average(double x , double y , double w , double h)
        {
            var sex = x+w;
            var sey = y+w;
            var sx = (int)x;
            var sy = (int)y;

            ulong count=0;
            ulong acount=0;
            ulong sa=0,sr=0,sg=0,sb=0;
            for ( var i = sx; i < sex; i++ )
            {
                for ( var j = sy; j < sey; j++ )
                {
                    count++;
                    ARGB source = this[i,j];
                    acount += source.A;
                    sa += source.A;
                    sr += source.R;
                    sg += source.G;
                    sb += source.B;
                }
            }
            ARGB destinationpx = new ARGB
            {
                A = ( byte ) ( sa / count )
            };
            if ( acount > 0 )
            {
                destinationpx.R = ( byte ) ( sr * 255 / acount );
                destinationpx.G = ( byte ) ( sg * 255 / acount );
                destinationpx.B = ( byte ) ( sb * 255 / acount );
            }
            return destinationpx;
        }
    }
    /// <summary>
    /// Optimized image for RGB
    /// </summary>
    [NotSerialized]
    public unsafe partial class ABGRImage : Image<ABGR>
    {
        ABGR* ptr;
        /// <summary>
        /// Optimized image for RGB
        /// </summary>
        public ABGRImage(int w , int h) : base(w , h)
        {
            ptr = ( ABGR* ) base.dataPointer;
        }
        /// <summary>
        /// Optimized image for RGB
        /// </summary>
        public ABGRImage(IntPtr data , int w , int h) : base(data , w , h)
        {
            ptr = ( ABGR* ) base.dataPointer;
        }
        /// <summary>
        /// Accessor
        /// </summary>
        /// <param name="pos"></param>
        /// <returns></returns>
        public override ABGR this[int pos] { get => ptr[pos]; set => ptr[pos] = value; }

        public override ABGR Get(double x , double y)
        {
            var ix = (int)x;
            var iy = (int)y;
            var dx = x-ix;
            var dy = y-iy;

            if ( dx == 0 && dy == 0 )
                return Get(ix , iy);

            var dx2 = 1-dx;
            var dy2 = 1-dy;

            var ipx = this[ix,iy];

            double sa=ipx.A,sr=ipx.R,sg=ipx.G,sb=ipx.B;
            if ( ix > 0 )
            {
                var px = this[ix , iy];
                sa += px.A * dx2;
                sr += px.R * dx2;
                sg += px.G * dx2;
                sb += px.B * dx2;
            }
            if ( iy > 0 )
            {
                var px=this[ix , iy];
                sa += px.A * dy2;
                sr += px.R * dy2;
                sg += px.G * dy2;
                sb += px.B * dy2;
            }
            if ( ix < width - 1 )
            {
                var px = this[ix + 1 , iy];
                sa += px.A * dx;
                sr += px.R * dx;
                sg += px.G * dx;
                sb += px.B * dx;
            }
            if ( iy < height - 1 )
            {
                var px = this[ix,iy+1];
                sa += px.A * dy;
                sr += px.R * dy;
                sg += px.G * dy;
                sb += px.B * dy;
            }
            ABGR destinationpx = new ABGR()
            {
                R = ( byte ) ( sr / 3 ),
                G = ( byte ) ( sg / 3 ),
                B = ( byte ) ( sb / 3 ),
                A = ( byte ) ( sa / 3 )
            };
            return destinationpx;
        }
        public override ABGR Average(double x , double y , double w , double h)
        {
            var sex = x+w;
            var sey = y+w;
            var sx = (int)x;
            var sy = (int)y;

            ulong count=0;
            ulong acount=0;
            ulong sa=0,sr=0,sg=0,sb=0;
            for ( var i = sx; i < sex; i++ )
            {
                for ( var j = sy; j < sey; j++ )
                {
                    count++;
                    ABGR source = this[i,j];
                    acount += source.A;
                    sa += source.A;
                    sr += source.R;
                    sg += source.G;
                    sb += source.B;
                }
            }
            ABGR destinationpx = new ABGR
            {
                A = ( byte ) ( sa / count )
            };
            if ( acount > 0 )
            {
                destinationpx.R = ( byte ) ( sr * 255 / acount );
                destinationpx.G = ( byte ) ( sg * 255 / acount );
                destinationpx.B = ( byte ) ( sb * 255 / acount );
            }
            return destinationpx;
        }
    }
    /// <summary>
    /// Optimized image for RGB
    /// </summary>
    [NotSerialized]
    public unsafe partial class BGRAImage : Image<BGRA>
    {
        BGRA* ptr;
        /// <summary>
        /// Optimized image for RGB
        /// </summary>
        public BGRAImage(int w , int h) : base(w , h)
        {
            ptr = ( BGRA* ) base.dataPointer;
        }
        /// <summary>
        /// Optimized image for RGB
        /// </summary>
        public BGRAImage(IntPtr data , int w , int h) : base(data , w , h)
        {
            ptr = ( BGRA* ) base.dataPointer;
        }
        /// <summary>
        /// Accessor
        /// </summary>
        /// <param name="pos"></param>
        /// <returns></returns>
        public override BGRA this[int pos] { get => ptr[pos]; set => ptr[pos] = value; }

        public override BGRA Get(double x , double y)
        {
            var ix = (int)x;
            var iy = (int)y;
            var dx = x-ix;
            var dy = y-iy;

            if ( dx == 0 && dy == 0 )
                return Get(ix , iy);

            var dx2 = 1-dx;
            var dy2 = 1-dy;

            var ipx = this[ix,iy];

            double sa=ipx.A,sr=ipx.R,sg=ipx.G,sb=ipx.B;
            if ( ix > 0 )
            {
                var px = this[ix , iy];
                sa += px.A * dx2;
                sr += px.R * dx2;
                sg += px.G * dx2;
                sb += px.B * dx2;
            }
            if ( iy > 0 )
            {
                var px=this[ix , iy];
                sa += px.A * dy2;
                sr += px.R * dy2;
                sg += px.G * dy2;
                sb += px.B * dy2;
            }
            if ( ix < width - 1 )
            {
                var px = this[ix + 1 , iy];
                sa += px.A * dx;
                sr += px.R * dx;
                sg += px.G * dx;
                sb += px.B * dx;
            }
            if ( iy < height - 1 )
            {
                var px = this[ix,iy+1];
                sa += px.A * dy;
                sr += px.R * dy;
                sg += px.G * dy;
                sb += px.B * dy;
            }
            BGRA destinationpx = new BGRA()
            {
                R = ( byte ) ( sr / 3 ),
                G = ( byte ) ( sg / 3 ),
                B = ( byte ) ( sb / 3 ),
                A = ( byte ) ( sa / 3 )
            };
            return destinationpx;
        }
        public override BGRA Average(double x , double y , double w , double h)
        {
            var sex = x+w;
            var sey = y+w;
            var sx = (int)x;
            var sy = (int)y;

            ulong count=0;
            ulong acount=0;
            ulong sa=0,sr=0,sg=0,sb=0;
            for ( var i = sx; i < sex; i++ )
            {
                for ( var j = sy; j < sey; j++ )
                {
                    count++;
                    BGRA source = this[i,j];
                    acount += source.A;
                    sa += source.A;
                    sr += source.R;
                    sg += source.G;
                    sb += source.B;
                }
            }
            BGRA destinationpx = new BGRA
            {
                A = ( byte ) ( sa / count )
            };
            if ( acount > 0 )
            {
                destinationpx.R = ( byte ) ( sr * 255 / acount );
                destinationpx.G = ( byte ) ( sg * 255 / acount );
                destinationpx.B = ( byte ) ( sb * 255 / acount );
            }
            return destinationpx;
        }
    }
}
