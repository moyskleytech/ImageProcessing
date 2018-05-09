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
    public unsafe class Image<Representation>
        where Representation : struct
    {
        /// <summary>
        /// size of image
        /// </summary>
        protected int width,height;
        /// <summary>
        /// Pointer for data
        /// </summary>
        protected readonly byte* dataPointer;
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
        public Image(int w , int h)
        {
            //Allocate
            lengthOfItem = Marshal.SizeOf(default(Representation));
            raw = Marshal.AllocHGlobal(w * h * lengthOfItem);
            dataPointer = ( byte* ) raw.ToPointer();
            width = w;
            height = h;
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
            raw = ptr;
            dataPointer = ( byte* ) raw.ToPointer();
            width = w;
            height = h;
            deleteOnDispose = false;
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
            if ( deleteOnDispose )
                Marshal.FreeHGlobal(raw);
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
        /// <returns></returns>
        public static Image<Representation> Create(int width , int height)
        {
            if ( typeof(Representation) == typeof(Pixel) )
                return ( Image<Representation> ) ( object ) new Bitmap(width , height);
            if ( typeof(Representation) == typeof(ARGB_Float) )
                return ( Image<Representation> ) ( object ) new SuperHighRangeImage(width , height);
            if ( typeof(Representation) == typeof(ARGB_16bit) )
                return ( Image<Representation> ) ( object ) new HighRangeImage(width , height);
            if ( typeof(Representation) == typeof(bool) )
                return ( Image<Representation> ) ( object ) new BoolImage(width , height);
            if ( typeof(Representation) == typeof(byte) )
                return ( Image<Representation> ) ( object ) new OneBandImage(width , height);
            if ( typeof(Representation) == typeof(float) )
                return ( Image<Representation> ) ( object ) new OneBandFloatImage(width , height);
            if ( typeof(Representation) == typeof(HSB) )
                return ( Image<Representation> ) ( object ) new HSBImage(width , height);
            if ( typeof(Representation) == typeof(HSBA) )
                return ( Image<Representation> ) ( object ) new HSBAImage(width , height);
            if ( typeof(Representation) == typeof(_1555) )
                return ( Image<Representation> ) ( object ) new _1555Image(width , height);
            if ( typeof(Representation) == typeof(_555) )
                return ( Image<Representation> ) ( object ) new _555Image(width , height);
            if ( typeof(Representation) == typeof(_565) )
                return ( Image<Representation> ) ( object ) new _565Image(width , height);
            if ( typeof(Representation) == typeof(CYMK) )
                return ( Image<Representation> ) ( object ) new CYMKImage(width , height);
            if ( typeof(Representation) == typeof(_332) )
                return ( Image<Representation> ) ( object ) new _332Image(width , height);
            if ( typeof(Representation) == typeof(HSB_Float) )
                return ( Image<Representation> ) ( object ) new HSB_FloatImage(width , height);
            if ( typeof(Representation) == typeof(RGB) )
                return ( Image<Representation> ) ( object ) new RGBImage(width , height);
            if ( typeof(Representation) == typeof(BGR) )
                return ( Image<Representation> ) ( object ) new BGRImage(width , height);
            if ( typeof(Representation) == typeof(HSL) )
                return ( Image<Representation> ) ( object ) new HSLImage(width , height);

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
        /// Accessor
        /// </summary>
        /// <param name="pos"></param>
        /// <returns></returns>
        public override ARGB_16bit this[int pos] { get => ptr[pos]; set => ptr[pos] = value; }
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
        /// Accessor
        /// </summary>
        /// <param name="pos"></param>
        /// <returns></returns>
        public override ARGB_Float this[int pos] { get => ptr[pos]; set => ptr[pos] = value; }
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
        /// Accessor
        /// </summary>
        /// <param name="pos"></param>
        /// <returns></returns>
        public override float this[int pos] { get => ptr[pos]; set => ptr[pos] = value; }
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
        /// Accessor
        /// </summary>
        /// <param name="pos"></param>
        /// <returns></returns>
        public override BGR this[int pos] { get => ptr[pos]; set => ptr[pos] = value; }
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
        /// Accessor
        /// </summary>
        /// <param name="pos"></param>
        /// <returns></returns>
        public override RGB this[int pos] { get => ptr[pos]; set => ptr[pos] = value; }
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
        /// Accessor
        /// </summary>
        /// <param name="pos"></param>
        /// <returns></returns>
        public override bool this[int pos] { get => ptr[pos]; set => ptr[pos] = value; }
    }
}
