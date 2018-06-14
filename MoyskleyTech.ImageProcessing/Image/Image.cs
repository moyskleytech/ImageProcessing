﻿using System;
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
    public unsafe partial class Image<Representation> : IDisposable
        where Representation : unmanaged
    {
        /// <summary>
        /// size of image
        /// </summary>
        protected int width,height;
        /// <summary>
        /// Pointer for data
        /// </summary>
        protected Representation* dataPointer;
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

        private Func<Func<int,int,Representation>,Size,double,double,Representation> getter;
        private Func<Func<int,int,Representation>,double,double,double,double,Representation> averager;

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
            lengthOfItem = sizeof(Representation);
            if ( ptr == IntPtr.Zero )
                raw = Marshal.AllocHGlobal(w * h * lengthOfItem);
            else
                raw = ptr;
            dataPointer = ( Representation* ) raw.ToPointer();
            width = w;
            height = h;
            deleteOnDispose = ptr == IntPtr.Zero;
            getter = Image.GetGetterFunction<Representation>();
            averager = Image.GetAverageFunction<Representation>();
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
        public int Width => width;
        /// <summary>
        /// Height of bitmap
        /// </summary>
        public int Height => height;
        /// <summary>
        /// Return if the image has been destroyed
        /// </summary>
        public bool Disposed { get; private set; } = false;

        public Size Size => new Size(width , height);
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


        public virtual ref Representation GetRef(int x , int y)
        {
            return ref dataPointer[y * width + x];
        }
        public virtual ref Representation GetRef(int i)
        {
            return ref dataPointer[i];
        }
        /// <summary>
        /// Dispose the bitmap and release memory
        /// </summary>
        public void Dispose()
        {
            if ( deleteOnDispose && !Disposed )
            {
                Marshal.FreeHGlobal(raw);
                Disposed = true;
            }
            GC.SuppressFinalize(this);
        }
        /// <summary>
        /// Source to edit or copy
        /// </summary>
        public IntPtr DataPointer { get { return raw; } }
        /// <summary>
        /// Source to edit or copy
        /// </summary>
        public Representation* Source { get { return dataPointer; } }
        public Representation this[Point pt]
        {
            get => this[pt.X , pt.Y];
            set => this[pt.X , pt.Y] = value;
        }
        /// <summary>
        /// Proxy an image easy
        /// </summary>
        /// <param name="rec"></param>
        /// <returns></returns>
        public ImageProxy<Representation> this[Rectangle rec]
        {
            get
            {
                return this.Proxy(rec);
            }
        }
        /// <summary>
        /// Serialize bitmap to stream
        /// </summary>
        /// <param name="s">Destination</param>
        public static Image<Pixel> FromStream(Stream s)
        {
            return new BitmapCodec().DecodeStream(s);
            //return new BitmapFactory().Decode(s);
        }
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
                    return dataPointer[pos];
                else
                    return default;
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
            Representation* src = dataPointer;
            Representation* dest = (Representation*)dst.ToPointer();
            for ( var i = 0; i < width * height; i++ )
                *dest++ = *src++;
        }
        /// <summary>
        /// Copy data from another pointer
        /// </summary>
        /// <param name="dst"></param>
        public void CopyFrom(IntPtr dst)
        {
            Representation* src = (Representation*)dst.ToPointer();
            Representation* dest = dataPointer;
            for ( var i = 0; i < width * height; i++ )
                *dest++ = *src++;
        }
        /// <summary>
        /// Copy data to another pointer
        /// </summary>
        /// <param name="dst"></param>
        public void CopyTo(void* dst)
        {
            Representation* src = dataPointer;
            Representation* dest = (Representation*)dst;
            for ( var i = 0; i < width * height; i++ )
                *dest++ = *src++;
        }
        /// <summary>
        /// Copy data from another pointer
        /// </summary>
        /// <param name="dst"></param>
        public void CopyFrom(void* dst)
        {
            Representation* src = (Representation*)dst;
            Representation* dest = dataPointer;
            for ( var i = 0; i < width * height; i++ )
                *dest++ = *src++;
        }
        /// <summary>
        /// Allow conversion between colorspaces
        /// </summary>
        /// <typeparam name="NewRepresentation"></typeparam>
        /// <returns></returns>
        public Image<NewRepresentation> ConvertTo<NewRepresentation>()
            where NewRepresentation : unmanaged
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
            where NewRepresentation : unmanaged
        {
            if ( Disposed )
                return null;
            if ( !deleteOnDispose )
                return ConvertTo<NewRepresentation>();
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
            this.dataPointer = ( Representation* ) ret.dataPointer;
            Disposed = true;
            return ret;
        }
        /// <summary>
        /// Convert to another colorspace using a function
        /// </summary>
        /// <typeparam name="NewRepresentation"></typeparam>
        /// <param name="converter"></param>
        /// <returns></returns>
        public Image<NewRepresentation> ConvertUsing<NewRepresentation>(Func<Representation , NewRepresentation> converter)
          where NewRepresentation : unmanaged
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
            byte* src = (byte*)dataPointer;
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
            byte* dest = (byte*)dataPointer;
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
                byte* src = (byte*)dataPointer+no;
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
                byte* dest = (byte*)dataPointer+no*sizeof(float);
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

            return Image<Representation>.UsingExistingMemoryPointer(data.Value,width , height);
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
        public virtual Image<Representation> Clone()
        {
            Image<Representation> dest = Create(width,height);
            dest.CopyFrom(dataPointer);
            return dest;
        }
        public virtual Representation Get(double x , double y)
        {
            if ( getter != null )
                return getter(Get , Size , x , y);
            return Get(( int ) x , ( int ) y);
        }
        public virtual Representation Average(double x , double y , double w , double h)
        {
            if ( averager != null )
                return averager(Get , x , y , w , h);
            return Get(x + w / 2 , y + w / 2);
        }
        public Representation Get(PointF pt)
        {
            return Get(pt.X , pt.Y);
        }
        public Representation Average(RectangleF r)
        {
            return Average(r.X , r.Y , r.Width , r.Height);
        }

    }

    public static class Image
    {
        public static Dictionary<Type , Func<Func<int,int,object>,Size,double , double , object>> GetFunctions = new Dictionary<Type, Func<Func<int,int,object>,Size,double, double, object>>();
        public static Dictionary<Type , Func<Func<int,int,object>,double,double, double , double , object>> AverageFunctions = new Dictionary<Type, Func<Func<int,int,object>,double, double,double, double, object>>();

        public static Func<Func<int , int , T> , Size , double , double , T> GetGetterFunction<T>()
            where T : unmanaged
        {
            var t = typeof(T);
            if ( !GetFunctions.ContainsKey(t) )
                RegisterGetFunction<T>(Get);
            return (g , s , x , y) => ( T ) GetFunctions[t]((x2 , y2) => g(x2 , y2) , s , x , y);
        }
        public static Func<Func<int , int , T> , double , double , double , double , T> GetAverageFunction<T>()
            where T : unmanaged
        {
            var t = typeof(T);
            if ( !AverageFunctions.ContainsKey(t) )
                RegisterAverageFunction<T>(Average);
            return (g , x , y , z , w) => ( T ) AverageFunctions[t]((x2 , y2) => g(x2 , y2) , x , y , z , w);
        }
        public static void RegisterGetFunction<T>(Func<Func<int , int , T> , Size , double , double , T> func)
            where T : unmanaged
        {
            var t = typeof(T);
            GetFunctions[t] = (g , s , x , y) => func((x2 , y2) => ( T ) g(x2 , y2) , s , x , y);
        }
        public static void RegisterAverageFunction<T>(Func<Func<int , int , T> , double , double , double , double , T> func)
            where T : unmanaged
        {
            var t = typeof(T);
            AverageFunctions[t] = (g , x , y , z , w) => func((x2 , y2) => ( T ) g(x2 , y2) , x , y , z , w);
        }

        static Image()
        {
            RegisterGetFunction<ARGB_16bit>(Get);
            RegisterGetFunction<ARGB_Float>(Get);
            RegisterGetFunction<float>(Get);
            RegisterAverageFunction<ARGB_16bit>(Average);
            RegisterAverageFunction<ARGB_Float>(Average);
            RegisterAverageFunction<float>(Average);

            //Autogenerated
            RegisterGetFunction<BGR>(Get);
            RegisterGetFunction<BGRA>(Get);
            RegisterGetFunction<RGB>(Get);
            RegisterGetFunction<RGBA>(Get);
            RegisterGetFunction<ABGR>(Get);
            RegisterGetFunction<ARGB>(Get);

            RegisterGetFunction<_1555>(Get);
            RegisterGetFunction<_332>(Get);
            RegisterGetFunction<_555>(Get);
            RegisterGetFunction<_565>(Get);

            RegisterGetFunction<HSB>(Get);
            RegisterGetFunction<HSL>(Get);
            RegisterGetFunction<HSBA>(Get);
            RegisterGetFunction<HSB_Float>(Get);
            RegisterGetFunction<CYMK>(Get);

            RegisterAverageFunction<BGR>(Average);
            RegisterAverageFunction<BGRA>(Average);
            RegisterAverageFunction<RGB>(Average);
            RegisterAverageFunction<RGBA>(Average);
            RegisterAverageFunction<ABGR>(Average);
            RegisterAverageFunction<ARGB>(Average);

            RegisterAverageFunction<_1555>(Average);
            RegisterAverageFunction<_332>(Average);
            RegisterAverageFunction<_555>(Average);
            RegisterAverageFunction<_565>(Average);

            RegisterAverageFunction<HSB>(Average);
            RegisterAverageFunction<HSL>(Average);
            RegisterAverageFunction<HSBA>(Average);
            RegisterAverageFunction<HSB_Float>(Average);
            RegisterAverageFunction<CYMK>(Average);
        }
        public static ARGB_16bit Get(Func<int , int , ARGB_16bit> getter , Size size , double x , double y)
        {
            var ix = (int)x;
            var iy = (int)y;
            var dx = x-ix;
            var dy = y-iy;

            if ( dx == 0 && dy == 0 )
                return getter(ix , iy);

            var dx2 = 1-dx;
            var dy2 = 1-dy;

            var ipx = getter(ix,iy);

            double sa=ipx.A,sr=ipx.R,sg=ipx.G,sb=ipx.B;
            if ( ix > 0 )
            {
                var px = getter(ix , iy);
                sa += px.A * dx2;
                sr += px.R * dx2;
                sg += px.G * dx2;
                sb += px.B * dx2;
            }
            if ( iy > 0 )
            {
                var px=getter(ix , iy);
                sa += px.A * dy2;
                sr += px.R * dy2;
                sg += px.G * dy2;
                sb += px.B * dy2;
            }
            if ( ix < size.Width - 1 )
            {
                var px = getter(ix + 1 , iy);
                sa += px.A * dx;
                sr += px.R * dx;
                sg += px.G * dx;
                sb += px.B * dx;
            }
            if ( iy < size.Height - 1 )
            {
                var px = getter(ix,iy+1);
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
        public static ARGB_16bit Average(Func<int , int , ARGB_16bit> getter , double x , double y , double w , double h)
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
                    ARGB_16bit source = getter(i,j);
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
        public static ARGB_Float Get(Func<int , int , ARGB_Float> getter , Size size , double x , double y)
        {
            var ix = (int)x;
            var iy = (int)y;
            var dx = x-ix;
            var dy = y-iy;

            if ( dx == 0 && dy == 0 )
                return getter(ix , iy);

            var dx2 = 1-dx;
            var dy2 = 1-dy;

            var ipx = getter(ix,iy);

            double sa=ipx.A,sr=ipx.R,sg=ipx.G,sb=ipx.B;
            if ( ix > 0 )
            {
                var px = getter(ix , iy);
                sa += px.A * dx2;
                sr += px.R * dx2;
                sg += px.G * dx2;
                sb += px.B * dx2;
            }
            if ( iy > 0 )
            {
                var px=getter(ix , iy);
                sa += px.A * dy2;
                sr += px.R * dy2;
                sg += px.G * dy2;
                sb += px.B * dy2;
            }
            if ( ix < size.Width - 1 )
            {
                var px = getter(ix + 1 , iy);
                sa += px.A * dx;
                sr += px.R * dx;
                sg += px.G * dx;
                sb += px.B * dx;
            }
            if ( iy < size.Height - 1 )
            {
                var px = getter(ix,iy+1);
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
        public static ARGB_Float Average(Func<int , int , ARGB_Float> getter , double x , double y , double w , double h)
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
                    ARGB_Float source = getter(i,j);
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
                destinationpx.R = ( float ) ( sr / acount );
                destinationpx.G = ( float ) ( sg / acount );
                destinationpx.B = ( float ) ( sb / acount );
            }
            return destinationpx;
        }
        public static float Get(Func<int , int , float> getter , Size size , double x , double y)
        {
            var ix = (int)x;
            var iy = (int)y;
            var dx = x-ix;
            var dy = y-iy;

            if ( dx == 0 && dy == 0 )
                return getter(ix , iy);

            var dx2 = 1-dx;
            var dy2 = 1-dy;

            var ipx = getter(ix,iy);

            double sa=ipx;
            if ( ix > 0 )
            {
                var px = getter(ix , iy);
                sa += px * dx2;
            }
            if ( iy > 0 )
            {
                var px=getter(ix , iy);
                sa += px * dy2;
            }
            if ( ix < size.Width - 1 )
            {
                var px = getter(ix + 1 , iy);
                sa += px * dx;
            }
            if ( iy < size.Height - 1 )
            {
                var px = getter(ix,iy+1);
                sa += px * dy;
            }
            return ( float ) sa / 3;
        }
        public static float Average(Func<int , int , float> getter , double x , double y , double w , double h)
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
                    float source = getter(i,j);
                    acount += source;
                }
            }

            return ( float ) acount / count;
        }
        public static Pixel Get(Func<int , int , Pixel> getter , Size size , double x , double y)
        {
            var ix = (int)x;
            var iy = (int)y;
            var dx = x-ix;
            var dy = y-iy;

            if ( dx == 0 && dy == 0 )
                return getter(ix , iy);

            var dx2 = 1-dx;
            var dy2 = 1-dy;

            var ipx = getter(ix,iy);

            double sa=ipx.A,sr=ipx.R,sg=ipx.G,sb=ipx.B;
            if ( ix > 0 )
            {
                var px = getter(ix , iy);
                sa += px.A * dx2;
                sr += px.R * dx2;
                sg += px.G * dx2;
                sb += px.B * dx2;
            }
            if ( iy > 0 )
            {
                var px=getter(ix , iy);
                sa += px.A * dy2;
                sr += px.R * dy2;
                sg += px.G * dy2;
                sb += px.B * dy2;
            }
            if ( ix < size.Width - 1 )
            {
                var px = getter(ix + 1 , iy);
                sa += px.A * dx;
                sr += px.R * dx;
                sg += px.G * dx;
                sb += px.B * dx;
            }
            if ( iy < size.Height - 1 )
            {
                var px = getter(ix,iy+1);
                sa += px.A * dy;
                sr += px.R * dy;
                sg += px.G * dy;
                sb += px.B * dy;
            }
            Pixel destinationpx = new Pixel()
            {
                R = ( byte ) ( sr / 3 ),
                G = ( byte ) ( sg / 3 ),
                B = ( byte ) ( sb / 3 ),
                A = ( byte ) ( sa / 3 )
            };
            return destinationpx;
        }
        public static Pixel Average(Func<int , int , Pixel> getter , double x , double y , double w , double h)
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
                    Pixel source = getter(i,j);
                    acount += source.A;
                    sa += source.A;
                    sr += source.R;
                    sg += source.G;
                    sb += source.B;
                }
            }
            Pixel destinationpx = new Pixel
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
        public static T Get<T>(Func<int , int , T> getter , Size size , double x , double y)
            where T : unmanaged
        {
            var converter1 = ColorConvert.GetConversionFrom<T,Pixel>();
            var converter2 = ColorConvert.GetConversionFrom<Pixel,T>();
            return converter2(Get((x2 , y2) => converter1(getter(x2 , y2)) , size , x , y));
        }
        public static T Average<T>(Func<int , int , T> getter , double x , double y , double w , double h)
            where T : unmanaged
        {
            var converter1 = ColorConvert.GetConversionFrom<T,Pixel>();
            var converter2 = ColorConvert.GetConversionFrom<Pixel,T>();

            return converter2(Average((x2 , y2) => converter1(getter(x2 , y2)) , x , y , w , h));
        }
    }
}
