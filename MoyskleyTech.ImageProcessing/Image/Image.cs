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
    [NotSerialized]
    public unsafe class Image<Representation>
        where Representation:struct
    {
        private int width,height;
        private readonly byte* data;
        private readonly int lengthOfItem;
        private IntPtr raw;
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
            data = ( byte* ) raw.ToPointer();
            width = w;
            height = h;
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
            Marshal.FreeHGlobal(raw);
            GC.SuppressFinalize(this);
        }
        /// <summary>
        /// Source to edit or copy
        /// </summary>
        public IntPtr Source { get { return raw; } }
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
        public Representation this[int pos]
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

        public void CopyTo(IntPtr dst)
        {
            byte* src = data;
            byte* dest = (byte*)dst.ToPointer();
            for ( var i = 0; i < width * height*lengthOfItem; i++ )
                *dest++ = *src++;
        }
        public void CopyFrom(IntPtr dst)
        {
            byte* src = (byte*)dst.ToPointer();
            byte* dest = data;
            for ( var i = 0; i < width * height * lengthOfItem; i++ )
                *dest++ = *src++;
        }
        public Image<NewRepresentation> ConvertTo<NewRepresentation>()
            where NewRepresentation:struct
        {
            Image<NewRepresentation> image = new Image<NewRepresentation>(width,height);
            MethodInfo methodInfo=null;
            if ( typeof(NewRepresentation) == typeof(Pixel) )
                methodInfo = typeof(ColorConvert).GetRuntimeMethod("ToRGB" , new Type[ ] { typeof(Representation) });
            else
                methodInfo = typeof(ColorConvert).GetRuntimeMethod("To"+typeof(NewRepresentation).Name , new Type[ ] { typeof(Representation) });

            if ( methodInfo == null )
                throw new NotImplementedException("Specified conversion from "+ typeof(Representation).Name +" to "+ typeof(NewRepresentation).Name +" is not implemented");

            for ( var i = 0; i < width * height; i++ )
            {
                image[i] = (NewRepresentation)methodInfo.Invoke(null , new object[ ] { this[i] });
            }
            return image;
        }
        public void ApplyFilter(Func<Representation , Point , Representation> func)
        {
            for ( var y = 0; y < height; y++ )
                for ( var x = 0; x < width; x++ )
                    this[x , y] = func(this[x , y] , new Point(x , y));
        }
        public OneBandImage GetByteBand(int no)
        {
            OneBandImage img = new OneBandImage(width,height);
            byte* src = data;
            byte* dest = img.Source;
            for ( var i = 0; i < width * height; i++ )
            {
                *dest++ = *src;
                src += lengthOfItem;
            }
            return img;
        }
        public void SetByteBand(OneBandImage band,int no)
        {
            byte* src = band.Source;
            byte* dest = data;
            for ( var i = 0; i < width * height; i++ )
            {
                *dest = *src;
                dest += lengthOfItem;
                src ++;
            }
        }

        public Image<float> GetFloatBand(int no)
        {
            if ( no < lengthOfItem )
            {
                Image<float> img = new Image<float>(width,height);
                byte* src = data+no;
                float* dest = (float*)img.Source.ToPointer();
                for ( var i = 0; i < width * height; i++ )
                {
                    *dest++ = *( ( float* ) src );      
                    src += lengthOfItem;
                }
                return img;
            }
            return null;
        }
        public void SetFloatBand(Image<float> band , int no)
        {
            if ( ( no + 1 ) * sizeof(float) <= lengthOfItem )
            {
                float* src = (float*)band.Source.ToPointer();
                byte* dest = data+no*sizeof(float);
                for ( var i = 0; i < width * height; i++ )
                {
                    *((float*)dest) = *src;
                    dest += lengthOfItem;
                    src++;
                }
            }
        }
        public Image<Representation> GetSubImage(Rectangle location)
        {
            Image<Representation> bmp = new Image<Representation>(location.Width,location.Height);
            for ( int x1 = 0, x = location.Left; x <= location.Right && x < width; x++, x1++ )
            {
                for ( int y1 = 0, y = location.Top; y <= location.Bottom && y < height; y++, y1++ )
                {
                    bmp[x1 , y1] = this[x , y];
                }
            }
            return bmp;
        }
        public Image<Representation> GetImage(int x , int y , int w , int h)
        {
            return GetSubImage(new Rectangle(x , y , w , h));
        }
        public Image<Representation> Crop(Rectangle rectangle)
        { return GetSubImage(rectangle); }

        public ImageProxy<Representation> Proxy(Rectangle rectangle)
        {
            return new ImageProxy<Representation>(this , rectangle);
        }
    }
}
