using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.InteropServices;
using System.IO;
using System.Threading.Tasks;

namespace MoyskleyTech.ImageProcessing.Image
{
    /// <summary>
    /// Represent a Bitmap (Array of Pixel)
    /// </summary>
    public unsafe partial class Bitmap : Image<Pixel>, IDisposable
    {
        private Pixel* data;
       
        /// <summary>
        /// Create a bitmap using Width and Height
        /// </summary>
        /// <param name="w">Width</param>
        /// <param name="h">Height</param>
        public Bitmap(int w , int h):base(w,h)
        {
            //Allocate
            data = ( Pixel* ) raw.ToPointer();
            width = w;
            height = h;
        }
        /// <summary>
        /// Create a bitmap using Width and Height
        /// </summary>
        /// <param name="w">Width</param>
        /// <param name="h">Height</param>
        /// <param name="p">Height</param>
        public Bitmap(IntPtr p,int w , int h) : base(p,w , h)
        {
            //Allocate
            data = ( Pixel* ) raw.ToPointer();
            width = w;
            height = h;
        }
        /// <summary>
        /// Create a bitmap using Width and Height and source from RGBA format
        /// </summary>
        /// <param name="w">Width</param>
        /// <param name="h">Height</param>
        /// <param name="raw">Source to copy</param>
        public Bitmap(int w , int h , byte[ ] raw):base(w,h)
        {
            //Allocate
            data = ( Pixel* ) this.raw.ToPointer();
            Marshal.Copy(raw , 0 , this.raw , w * h * sizeof(Pixel));
            width = w;
            height = h;
        }
       
        /// <summary>
        /// Create a bitmap using memory already allocated
        /// </summary>
        /// <param name="ptr"></param>
        /// <param name="w"></param>
        /// <param name="h"></param>
        /// <returns></returns>
        public new static Bitmap UsingExistingMemoryPointer(IntPtr ptr , int w , int h)
        {
            return new Bitmap(ptr , w , h);
        }

        /// <summary>
        /// Destrop bitmap
        /// </summary>
        ~Bitmap()
        {
            Dispose();
        }
      
        /// <summary>
        /// Get pixel from coordinate
        /// </summary>
        /// <param name="pos">As 1 dim array</param>
        /// <returns>Pixel</returns>
        public override Pixel this[int pos]
        {
            get
            {
                if ( pos >= 0 && pos < width * height )
                    return data[pos];
                else
                    return new Pixel();
            }
            set
            {
                if ( pos >= 0 && pos < width * height )
                    data[pos] = value;
            }
        }
        /// <summary>
        /// Copy from pointer using ARGB pattern for bytes
        /// </summary>
        /// <param name="ptr">SOurce</param>
        public void CopyFromARGB(void* ptr)
        {
            var iptr= (Pixel*)ptr;
            var optr = data;
            for ( var i = 0; i < height; i++ )
                for ( var j = 0; j < width; j++ )
                    *optr++ = *iptr++;
        }
        /// <summary>
        /// Copy to pointer using ARGB pattern for bytes
        /// </summary>
        /// <param name="ptr">Destination</param>
        public void CopyToARGB(void* ptr)
        {
            var iptr=data;
            var optr = (Pixel*)ptr;
            for ( var i = 0; i < height; i++ )
                for ( var j = 0; j < width; j++ )
                    *optr++ = *iptr++;
        }
        /// <summary>
        /// Copy from pointer using ARGB pattern for bytes
        /// </summary>
        /// <param name="ptr">SOurce</param>
        public void CopyFromRGB(void* ptr)
        {
            var iptr= (byte*)ptr;
            var optr = data;
            for ( var i = 0; i < height; i++ )
                for ( var j = 0; j < width; j++ )
                {
                    optr->R = *iptr++;
                    optr->G = *iptr++;
                    optr->B = *iptr++;
                    optr->A = 255;
                    optr++;
                }
        }
        /// <summary>
        /// Copy to pointer using ARGB pattern for bytes
        /// </summary>
        /// <param name="ptr">Destination</param>
        public void CopyToRGB(void* ptr)
        {
            var iptr=data;
            var optr = (byte*)ptr;
            for ( var i = 0; i < height; i++ )
                for ( var j = 0; j < width; j++ )
                {
                    *optr++ = iptr->R;
                    *optr++ = iptr->G;
                    *optr++ = iptr->B;
                }
        }
        /// <summary>
        /// Copy from pointer using ARGB pattern for bytes
        /// </summary>
        /// <param name="ptr">SOurce</param>
        public void CopyFromBGR(void* ptr)
        {
            var iptr= (byte*)ptr;
            var optr = data;
            for ( var i = 0; i < height; i++ )
                for ( var j = 0; j < width; j++ )
                {
                    optr->B = *iptr++;
                    optr->G = *iptr++;
                    optr->R = *iptr++;
                    optr->A = 255;
                    optr++;
                }
        }
        /// <summary>
        /// Copy to pointer using ARGB pattern for bytes
        /// </summary>
        /// <param name="ptr">Destination</param>
        public void CopyToBGR(void* ptr)
        {
            var iptr=data;
            var optr = (byte*)ptr;
            for ( var i = 0; i < height; i++ )
                for ( var j = 0; j < width; j++ )
                {
                    *optr++ = iptr->B;
                    *optr++ = iptr->G;
                    *optr++ = iptr->R;
                    iptr++;
                }
        }
        /// <summary>
        /// Copy from pointer using BRGA pattern for bytes
        /// </summary>
        /// <param name="ptr">Source</param>
        public void CopyFromBGRA(void* ptr)
        {
            var iptr= (Pixel*)ptr;
            var optr = data;
            for ( var i = 0; i < height; i++ )
                for ( var j = 0; j < width; j++ )
                {
                    optr->B = iptr->A;
                    optr->G = iptr->R;
                    optr->R = iptr->G;
                    optr->A = iptr->B;
                    optr++;
                    iptr++;
                }
        }
        /// <summary>
        /// Copy to pointer using BGRA pattern for bytes
        /// </summary>
        /// <param name="ptr">Destination</param>
        public void CopyToBGRA(void* ptr)
        {
            Pixel* iptr=data;
            Pixel* optr = (Pixel*)ptr;
            for ( var i = 0; i < height; i++ )
                for ( var j = 0; j < width; j++ )
                {
                    optr->A = iptr->B;
                    optr->R = iptr->G;
                    optr->G = iptr->R;
                    optr->B = iptr->A;
                    optr++;
                    iptr++;
                }
        }
        /// <summary>
        /// Copy from pointer using ABGR pattern for bytes
        /// </summary>
        /// <param name="ptr">Source</param>
        public void CopyFromABGR(void* ptr)
        {
            var iptr= (Pixel*)ptr;
            var optr = data;
            for ( var i = 0; i < height; i++ )
                for ( var j = 0; j < width; j++ )
                {
                    optr->A = iptr->A;
                    optr->B = iptr->R;
                    optr->G = iptr->G;
                    optr->R = iptr->B;
                    optr++;
                    iptr++;
                }
        }
        /// <summary>
        /// Copy to pointer using ABGR pattern for bytes
        /// </summary>
        /// <param name="ptr">Destination</param>
        public void CopyToABGR(void* ptr)
        {
            Pixel* iptr=data;
            Pixel* optr = (Pixel*)ptr;
            for ( var i = 0; i < height; i++ )
                for ( var j = 0; j < width; j++ )
                {
                    optr->A = iptr->A;
                    optr->R = iptr->B;
                    optr->G = iptr->G;
                    optr->B = iptr->R;
                    optr++;
                    iptr++;
                }
        }
        /// <summary>
        /// Copy from pointer using RGBA pattern for bytes
        /// </summary>
        /// <param name="ptr">Source</param>
        public void CopyFromRGBA(void* ptr)
        {
            var iptr= (Pixel*)ptr;
            var optr = data;
            for ( var i = 0; i < height; i++ )
                for ( var j = 0; j < width; j++ )
                {
                    optr->R = iptr->A;
                    optr->G = iptr->R;
                    optr->B = iptr->G;
                    optr->A = iptr->B;
                    optr++;
                    iptr++;
                }
        }
        /// <summary>
        /// Copy to pointer using RGBA pattern for bytes
        /// </summary>
        /// <param name="ptr">Destination</param>
        public void CopyToRGBA(void* ptr)
        {
            Pixel* iptr=data;
            Pixel* optr = (Pixel*)ptr;
            for ( var i = 0; i < height; i++ )
                for ( var j = 0; j < width; j++ )
                {
                    optr->A = iptr->R;
                    optr->R = iptr->G;
                    optr->G = iptr->B;
                    optr->B = iptr->A;
                    optr++;
                    iptr++;
                }
        }
       
        /// <summary>
        /// Copy to pointer using ARGB pattern for bytes
        /// </summary>
        /// <param name="scan0">Destination</param>
        public void CopyToRGB(IntPtr scan0)
        {
            CopyToRGB(scan0.ToPointer());
        }
        /// <summary>
        /// Copy to pointer using ARGB pattern for bytes
        /// </summary>
        /// <param name="scan0">Destination</param>
        public void CopyToBGR(IntPtr scan0)
        {
            CopyToBGR(scan0.ToPointer());
        }
        /// <summary>
        /// Copy to pointer using ARGB pattern for bytes
        /// </summary>
        /// <param name="scan0">Destination</param>
        public void CopyToARGB(IntPtr scan0)
        {
            CopyToARGB(scan0.ToPointer());
        }
        /// <summary>
        /// Copy to pointer using BGRA pattern for bytes
        /// </summary>
        /// <param name="scan0">Destination</param>
        public void CopyToBGRA(IntPtr scan0)
        {
            CopyToBGRA(scan0.ToPointer());
        }
        /// <summary>
        /// Copy to pointer using RGBA pattern for bytes
        /// </summary>
        /// <param name="scan0">Destination</param>
        public void CopyToRGBA(IntPtr scan0)
        {
            CopyToRGBA(scan0.ToPointer());
        }
        /// <summary>
        /// Copy to pointer using ABGR pattern for bytes
        /// </summary>
        /// <param name="scan0">Destination</param>
        public void CopyToABGR(IntPtr scan0)
        {
            CopyToABGR(scan0.ToPointer());
        }
        /// <summary>
        /// Copy from pointer using ARGB pattern for bytes
        /// </summary>
        /// <param name="scan0">Source</param>
        public void CopyFromRGB(IntPtr scan0)
        {
            CopyFromRGB(scan0.ToPointer());
        }
        /// <summary>
        /// Copy from pointer using ARGB pattern for bytes
        /// </summary>
        /// <param name="scan0">Source</param>
        public void CopyFromBGR(IntPtr scan0)
        {
            CopyFromBGR(scan0.ToPointer());
        }
        /// <summary>
        /// Copy from pointer using ARGB pattern for bytes
        /// </summary>
        /// <param name="scan0">Source</param>
        public void CopyFromARGB(IntPtr scan0)
        {
            CopyFromARGB(scan0.ToPointer());
        }
        /// <summary>
        /// Copy from pointer using BGRA pattern for bytes
        /// </summary>
        /// <param name="scan0">Source</param>
        public void CopyFromBGRA(IntPtr scan0)
        {
            CopyFromBGRA(scan0.ToPointer());
        }
        /// <summary>
        /// Copy from pointer using RGBA pattern for bytes
        /// </summary>
        /// <param name="scan0">Source</param>
        public void CopyFromRGBA(IntPtr scan0)
        {
            CopyFromRGBA(scan0.ToPointer());
        }
        /// <summary>
        /// Copy from pointer using ABGR pattern for bytes
        /// </summary>
        /// <param name="scan0">Source</param>
        public void CopyFromABGR(IntPtr scan0)
        {
            CopyFromABGR(scan0.ToPointer());
        }
        /// <summary>
        /// Colorize bitmap using specified color
        /// </summary>
        /// <param name="dest">Color</param>
        /// <returns>this</returns>
        public Image<Pixel> Colorize(Pixel dest)
        {
            Image<Pixel> bmpdest = Image<Pixel>.Create(width,height);
            bmpdest.CopyFromARGB(this.dataPointer);

            int count = 0;
            Pixel* ptr = bmpdest.Source;
            {
                while ( count < width * height )
                {
                    //Pixel p = Pixel.FromArgb(*ptr->A, dest.R,dest.G,dest.B);
                    ptr->R = dest.R;
                    ptr->G = dest.G;
                    ptr->B = dest.B;
                    ptr++;
                    count++;
                }
            }

            return bmpdest;
        }
        /// <summary>
        /// Colorize as Red
        /// </summary>
        /// <returns>this</returns>
        public Image<Pixel> ColorizeRed()
        {
            return this.Colorize(Pixels.Red);
        }
        /// <summary>
        /// Colorize as Green
        /// </summary>
        /// <returns>this</returns>
        public Image<Pixel> ColorizeGreen()
        {
            return this.Colorize(Pixels.Green);
        }
        /// <summary>
        /// Colorize as Blue
        /// </summary>
        /// <returns>this</returns>
        public Image<Pixel> ColorizeBlue()
        {
            return this.Colorize(Pixels.Blue);
        }
        /// <summary>
        /// Colorize as White
        /// </summary>
        /// <returns>this</returns>
        public Image<Pixel> ColorizeWhite()
        {
            return this.Colorize(Pixels.White);
        }
        /// <summary>
        /// Colorize as Black
        /// </summary>
        /// <returns>this</returns>
        public Image<Pixel> ColorizeBlack()
        {
            return this.Colorize(Pixels.Black);
        }
        /// <summary>
        /// Copy image from raw Grayscale
        /// </summary>
        /// <param name="raw">Input data</param>
        /// <param name="palette">Color palette</param>
        public void CopyFromRawGrayscale(byte[ ] raw , BitmapPalette8bpp palette = null)
        {
            palette = palette ?? BitmapPalette8bpp.Grayscale;
            int size = width*height;
            Pixel*ptr = data;
            for ( var i = 0; i < size; i++ )
            {
                *ptr++ = palette[raw[i]];
            }
        }
        /// <summary>
        /// Create raw 8 bit per pixel array for specified image
        /// </summary>
        /// <returns>Monoscale raw bitmap</returns>
        public byte[ ] CreateRaw8bppArray()
        {
            MemoryStream ms = new MemoryStream();
            int size = width*height;
            Pixel*ptr = data;
            for ( var i = 0; i < size; i++ )
            {
                ms.WriteByte(ptr->GetGrayTone());
                ptr++;
            }
            return ms.ToArray();
        }


        /// <summary>
        /// Allow bitmap subtraction
        /// </summary>
        /// <param name="bitmapA"></param>
        /// <param name="bitmapB"></param>
        /// <returns></returns>
        public static BitmapSubstract operator -(Bitmap bitmapA , Bitmap bitmapB)
        {
            return new BitmapSubstract(bitmapA , bitmapB);
        }
        /// <summary>
        /// Allow bitmap subtraction
        /// </summary>
        /// <param name="bitmapA"></param>
        /// <param name="bitmapB"></param>
        /// <returns></returns>
        public static BitmapSubstract operator -(Image<Pixel> bitmapA , Bitmap bitmapB)
        {
            return new BitmapSubstract(bitmapA , bitmapB);
        }
        /// <summary>
        /// Allow bitmap subtraction
        /// </summary>
        /// <param name="bitmapA"></param>
        /// <param name="bitmapB"></param>
        /// <returns></returns>
        public static BitmapSubstract operator -(Bitmap bitmapA , Image<Pixel> bitmapB)
        {
            return new BitmapSubstract(bitmapA , bitmapB);
        }
        /// <summary>
        /// Allow inver of bitmap
        /// </summary>
        /// <param name="bitmapA"></param>
        /// <returns></returns>
        public static BitmapInvert operator !(Bitmap bitmapA)
        {
            return new BitmapInvert(bitmapA);
        }
        /// <summary>
        /// Allow bitmap addition
        /// </summary>
        /// <param name="bitmapA"></param>
        /// <param name="bitmapB"></param>
        /// <returns></returns>
        public static BitmapAddition operator +(Bitmap bitmapA , Bitmap bitmapB)
        {
            return new BitmapAddition(bitmapA , bitmapB);
        }
        /// <summary>
        /// Allow bitmap addition
        /// </summary>
        /// <param name="bitmapA"></param>
        /// <param name="bitmapB"></param>
        /// <returns></returns>
        public static BitmapAddition operator +(Image<Pixel> bitmapA , Bitmap bitmapB)
        {
            return new BitmapAddition(bitmapA , bitmapB);
        }
        /// <summary>
        /// Allow bitmap addition
        /// </summary>
        /// <param name="bitmapA"></param>
        /// <param name="bitmapB"></param>
        /// <returns></returns>
        public static BitmapAddition operator +(Bitmap bitmapA , Image<Pixel> bitmapB)
        {
            return new BitmapAddition(bitmapA , bitmapB);
        }
        public override Pixel Get(double x , double y)
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
            Pixel destinationpx = new Pixel()
            {
                R = ( byte ) ( sr / 3 ),
                G = ( byte ) ( sg / 3 ),
                B = ( byte ) ( sb / 3 ),
                A = ( byte ) ( sa / 3 )
            };
            return destinationpx;
        }
        public override Pixel Average(double x , double y , double w , double h)
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
                    Pixel source = this[i,j];
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
    }
    public static unsafe class BitmapExtention
    {
        /// <summary>
        /// Copy from pointer using ARGB pattern for bytes
        /// </summary>
        /// <param name="ptr">SOurce</param>
        public static void CopyFromARGB(this Image<Pixel> bmp , void* ptr)
        {
            var iptr= (Pixel*)ptr;
            var optr = (Pixel*)bmp.DataPointer.ToPointer();
            for ( var i = 0; i < bmp.Height; i++ )
                for ( var j = 0; j < bmp.Width; j++ )
                    *optr++ = *iptr++;
        }
        /// <summary>
        /// Copy to pointer using ARGB pattern for bytes
        /// </summary>
        /// <param name="ptr">Destination</param>
        public static void CopyToARGB(this Image<Pixel> bmp , void* ptr)
        {
            var iptr=(Pixel*)bmp.DataPointer.ToPointer();
            var optr = (Pixel*)ptr;
            for ( var i = 0; i < bmp.Height; i++ )
                for ( var j = 0; j < bmp.Width; j++ )
                    *optr++ = *iptr++;
        }
        /// <summary>
        /// Copy from pointer using ARGB pattern for bytes
        /// </summary>
        /// <param name="ptr">SOurce</param>
        public static void CopyFromRGB(this Image<Pixel> bmp , void* ptr)
        {
            var iptr= (byte*)ptr;
            var optr = (Pixel*)bmp.DataPointer.ToPointer();
            for ( var i = 0; i < bmp.Height; i++ )
                for ( var j = 0; j < bmp.Width; j++ )
                {
                    optr->R = *iptr++;
                    optr->G = *iptr++;
                    optr->B = *iptr++;
                    optr->A = 255;
                    optr++;
                }
        }
        /// <summary>
        /// Copy to pointer using ARGB pattern for bytes
        /// </summary>
        /// <param name="ptr">Destination</param>
        public static void CopyToRGB(this Image<Pixel> bmp , void* ptr)
        {
            var iptr=(Pixel*)bmp.DataPointer.ToPointer();
            var optr = (byte*)ptr;
            for ( var i = 0; i < bmp.Height; i++ )
                for ( var j = 0; j < bmp.Width; j++ )
                {
                    *optr++ = iptr->R;
                    *optr++ = iptr->G;
                    *optr++ = iptr->B;
                }
        }
        /// <summary>
        /// Copy from pointer using ARGB pattern for bytes
        /// </summary>
        /// <param name="ptr">SOurce</param>
        public static void CopyFromBGR(this Image<Pixel> bmp , void* ptr)
        {
            var iptr= (byte*)ptr;
            var optr = (Pixel*)bmp.DataPointer.ToPointer();
            for ( var i = 0; i < bmp.Height; i++ )
                for ( var j = 0; j < bmp.Width; j++ )
                {
                    optr->B = *iptr++;
                    optr->G = *iptr++;
                    optr->R = *iptr++;
                    optr->A = 255;
                    optr++;
                }
        }
        /// <summary>
        /// Copy to pointer using ARGB pattern for bytes
        /// </summary>
        /// <param name="ptr">Destination</param>
        public static void CopyToBGR(this Image<Pixel> bmp , void* ptr)
        {
            var iptr=(Pixel*)bmp.DataPointer.ToPointer();
            var optr = (byte*)ptr;
            for ( var i = 0; i < bmp.Height; i++ )
                for ( var j = 0; j < bmp.Width; j++ )
                {
                    *optr++ = iptr->B;
                    *optr++ = iptr->G;
                    *optr++ = iptr->R;
                    iptr++;
                }
        }
        /// <summary>
        /// Copy from pointer using BRGA pattern for bytes
        /// </summary>
        /// <param name="ptr">Source</param>
        public static void CopyFromBGRA(this Image<Pixel> bmp , void* ptr)
        {
            var iptr= (Pixel*)ptr;
            var optr = (Pixel*)bmp.DataPointer.ToPointer();
            for ( var i = 0; i < bmp.Height; i++ )
                for ( var j = 0; j < bmp.Width; j++ )
                {
                    optr->B = iptr->A;
                    optr->G = iptr->R;
                    optr->R = iptr->G;
                    optr->A = iptr->B;
                    optr++;
                    iptr++;
                }
        }
        /// <summary>
        /// Copy to pointer using BGRA pattern for bytes
        /// </summary>
        /// <param name="ptr">Destination</param>
        public static void CopyToBGRA(this Image<Pixel> bmp , void* ptr)
        {
            Pixel* iptr=(Pixel*)bmp.DataPointer.ToPointer();
            Pixel* optr = (Pixel*)ptr;
            for ( var i = 0; i < bmp.Height; i++ )
                for ( var j = 0; j < bmp.Width; j++ )
                {
                    optr->A = iptr->B;
                    optr->R = iptr->G;
                    optr->G = iptr->R;
                    optr->B = iptr->A;
                    optr++;
                    iptr++;
                }
        }
        /// <summary>
        /// Copy from pointer using ABGR pattern for bytes
        /// </summary>
        /// <param name="ptr">Source</param>
        public static void CopyFromABGR(this Image<Pixel> bmp , void* ptr)
        {
            var iptr= (Pixel*)ptr;
            var optr = (Pixel*)bmp.DataPointer.ToPointer();
            for ( var i = 0; i < bmp.Height; i++ )
                for ( var j = 0; j < bmp.Width; j++ )
                {
                    optr->A = iptr->A;
                    optr->B = iptr->R;
                    optr->G = iptr->G;
                    optr->R = iptr->B;
                    optr++;
                    iptr++;
                }
        }
        /// <summary>
        /// Copy to pointer using ABGR pattern for bytes
        /// </summary>
        /// <param name="ptr">Destination</param>
        public static void CopyToABGR(this Image<Pixel> bmp , void* ptr)
        {
            Pixel* iptr=(Pixel*)bmp.DataPointer.ToPointer();
            Pixel* optr = (Pixel*)ptr;
            for ( var i = 0; i < bmp.Height; i++ )
                for ( var j = 0; j < bmp.Width; j++ )
                {
                    optr->A = iptr->A;
                    optr->R = iptr->B;
                    optr->G = iptr->G;
                    optr->B = iptr->R;
                    optr++;
                    iptr++;
                }
        }
        /// <summary>
        /// Copy from pointer using RGBA pattern for bytes
        /// </summary>
        /// <param name="ptr">Source</param>
        public static void CopyFromRGBA(this Image<Pixel> bmp , void* ptr)
        {
            var iptr= (Pixel*)ptr;
            var optr = (Pixel*)bmp.DataPointer.ToPointer();
            for ( var i = 0; i < bmp.Height; i++ )
                for ( var j = 0; j < bmp.Width; j++ )
                {
                    optr->R = iptr->A;
                    optr->G = iptr->R;
                    optr->B = iptr->G;
                    optr->A = iptr->B;
                    optr++;
                    iptr++;
                }
        }
        /// <summary>
        /// Copy to pointer using RGBA pattern for bytes
        /// </summary>
        /// <param name="ptr">Destination</param>
        public static void CopyToRGBA(this Image<Pixel> bmp,void* ptr)
        {
            Pixel* iptr=(Pixel*)bmp.DataPointer.ToPointer();
            Pixel* optr = (Pixel*)ptr;
            for ( var i = 0; i < bmp.Height; i++ )
                for ( var j = 0; j < bmp.Width; j++ )
                {
                    optr->A = iptr->R;
                    optr->R = iptr->G;
                    optr->G = iptr->B;
                    optr->B = iptr->A;
                    optr++;
                    iptr++;
                }
        }
        /// <summary>
        /// Copy to pointer using ARGB pattern for bytes
        /// </summary>
        /// <param name="scan0">Destination</param>
        public static void CopyToRGB(this Image<Pixel> bmp , IntPtr scan0)
        {
            CopyToRGB(bmp,scan0.ToPointer());
        }
        /// <summary>
        /// Copy to pointer using ARGB pattern for bytes
        /// </summary>
        /// <param name="scan0">Destination</param>
        public static void CopyToBGR(this Image<Pixel> bmp , IntPtr scan0)
        {
            CopyToBGR(bmp , scan0.ToPointer());
        }
        /// <summary>
        /// Copy to pointer using ARGB pattern for bytes
        /// </summary>
        /// <param name="scan0">Destination</param>
        public static void CopyToARGB(this Image<Pixel> bmp , IntPtr scan0)
        {
            CopyToARGB(bmp , scan0.ToPointer());
        }
        /// <summary>
        /// Copy to pointer using BGRA pattern for bytes
        /// </summary>
        /// <param name="scan0">Destination</param>
        public static void CopyToBGRA(this Image<Pixel> bmp , IntPtr scan0)
        {
            CopyToBGRA(bmp , scan0.ToPointer());
        }
        /// <summary>
        /// Copy to pointer using RGBA pattern for bytes
        /// </summary>
        /// <param name="scan0">Destination</param>
        public static void CopyToRGBA(this Image<Pixel> bmp , IntPtr scan0)
        {
            CopyToRGBA(bmp , scan0.ToPointer());
        }
        /// <summary>
        /// Copy to pointer using ABGR pattern for bytes
        /// </summary>
        /// <param name="scan0">Destination</param>
        public static void CopyToABGR(this Image<Pixel> bmp , IntPtr scan0)
        {
            CopyToABGR(bmp , scan0.ToPointer());
        }
        /// <summary>
        /// Copy from pointer using ARGB pattern for bytes
        /// </summary>
        /// <param name="scan0">Source</param>
        public static void CopyFromRGB(this Image<Pixel> bmp , IntPtr scan0)
        {
            CopyFromRGB(bmp , scan0.ToPointer());
        }
        /// <summary>
        /// Copy from pointer using ARGB pattern for bytes
        /// </summary>
        /// <param name="scan0">Source</param>
        public static void CopyFromBGR(this Image<Pixel> bmp , IntPtr scan0)
        {
            CopyFromBGR(bmp , scan0.ToPointer());
        }
        /// <summary>
        /// Copy from pointer using ARGB pattern for bytes
        /// </summary>
        /// <param name="scan0">Source</param>
        public static void CopyFromARGB(this Image<Pixel> bmp , IntPtr scan0)
        {
            CopyFromARGB(bmp , scan0.ToPointer());
        }
        /// <summary>
        /// Copy from pointer using BGRA pattern for bytes
        /// </summary>
        /// <param name="scan0">Source</param>
        public static void CopyFromBGRA(this Image<Pixel> bmp , IntPtr scan0)
        {
            CopyFromBGRA(bmp , scan0.ToPointer());
        }
        /// <summary>
        /// Copy from pointer using RGBA pattern for bytes
        /// </summary>
        /// <param name="scan0">Source</param>
        public static void CopyFromRGBA(this Image<Pixel> bmp , IntPtr scan0)
        {
            CopyFromRGBA(bmp , scan0.ToPointer());
        }
        /// <summary>
        /// Copy from pointer using ABGR pattern for bytes
        /// </summary>
        /// <param name="scan0">Source</param>
        public static void CopyFromABGR(this Image<Pixel> bmp , IntPtr scan0)
        {
            CopyFromABGR(bmp , scan0.ToPointer());
        }
    }
}
