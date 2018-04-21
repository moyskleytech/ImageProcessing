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
    public unsafe partial class Bitmap : IDisposable
    {
        private readonly int width,height;
        private readonly Pixel* data;
        private readonly IntPtr raw;
        /// <summary>
        /// Create a bitmap using Width and Height
        /// </summary>
        /// <param name="w">Width</param>
        /// <param name="h">Height</param>
        public Bitmap(int w , int h)
        {
            //Allocate
            raw = Marshal.AllocHGlobal(w * h * sizeof(Pixel));
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
        public Bitmap(int w , int h , byte[ ] raw)
        {
            //Allocate
            this.raw = Marshal.AllocHGlobal(w * h * sizeof(Pixel));
            data = ( Pixel* ) this.raw.ToPointer();
            Marshal.Copy(raw , 0 , this.raw , w * h * sizeof(Pixel));
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
        ~Bitmap()
        {
            Dispose();
        }
        /// <summary>
        /// Get pixel from coordinate
        /// </summary>
        /// <param name="x">X position</param>
        /// <param name="y">Y position</param>
        /// <returns>Pixel</returns>
        public Pixel Get(int x , int y)
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
        public Pixel* Source { get { return data; } }
        /// <summary>
        /// Get pixel from coordinate
        /// </summary>
        /// <param name="x">X position</param>
        /// <param name="y">Y position</param>
        /// <returns>Pixel</returns>
        public Pixel this[int x , int y]
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

        public ImageProxy this[Rectangle rec]
        {
            get
            {
                return this.Proxy(rec);
            }
        }
        /// <summary>
        /// Get pixel from coordinate
        /// </summary>
        /// <param name="pos">As 1 dim array</param>
        /// <returns>Pixel</returns>
        public Pixel this[int pos]
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
        /// Serialize bitmap to stream
        /// </summary>
        /// <param name="s">Destination</param>
        public void ToStream(Stream s)
        {
            Save(s);
        }
        /// <summary>
        /// Serialize bitmap to stream
        /// </summary>
        /// <param name="s">Destination</param>
        public static Bitmap FromStream(Stream s)
        {
            return new BitmapCodec().DecodeStream(s);
            //return new BitmapFactory().Decode(s);
        }

        /// <summary>
        /// Write bitmap to stream
        /// </summary>
        /// <param name="s">Destination</param>
        public void Save(Stream s)
        {
            s.WriteByte(( byte ) 'B');//0
            s.WriteByte(( byte ) 'M');//1


            var size = width*height*4+54;
            var sizeAsByte = BitConverter.GetBytes(size);

            s.Write(sizeAsByte , 0 , 4);//2-5

            s.WriteByte(0);//6
            s.WriteByte(0);//7
            s.WriteByte(0);//8
            s.WriteByte(0);//9

            s.Write(BitConverter.GetBytes(54) , 0 , 4);//10-13

            s.Write(BitConverter.GetBytes(40) , 0 , 4);//14-17

            s.Write(BitConverter.GetBytes(width) , 0 , 4);//18-21
            s.Write(BitConverter.GetBytes(height) , 0 , 4);//22-25

            s.Write(BitConverter.GetBytes(( short ) 1) , 0 , 2);//26-27
            s.Write(BitConverter.GetBytes(( short ) 32) , 0 , 2);//28-29

            s.Write(BitConverter.GetBytes(0) , 0 , 4);//30-33

            s.Write(BitConverter.GetBytes(0) , 0 , 4);//imagesize

            s.Write(BitConverter.GetBytes(unchecked(( int ) 0x00000EC4)) , 0 , 4);
            s.Write(BitConverter.GetBytes(unchecked(( int ) 0x00000EC4)) , 0 , 4);

            s.Write(BitConverter.GetBytes(0) , 0 , 4);//numcolorspalette
            s.Write(BitConverter.GetBytes(0) , 0 , 4);//mostimpcolor


            for ( var i = height - 1; i >= 0; i-- )
            {
                Pixel* ptr = data + i * width;

                for ( var j = 0; j < width; j++ )
                {
                    s.WriteByte(ptr->B);
                    s.WriteByte(ptr->G);
                    s.WriteByte(ptr->R);
                    s.WriteByte(ptr->A);
                    ptr++;
                }
            }
        }
        /// <summary>
        /// Write bitmap to stream
        /// </summary>
        /// <param name="s">Destination</param>
        public void Save8Bpp(Stream s , BitmapPalette8bpp palette = null)
        {
            palette = palette ?? BitmapPalette8bpp.Grayscale;
            s.WriteByte(( byte ) 'B');//0
            s.WriteByte(( byte ) 'M');//1

            const int sizeOfPalette = 256*4;
            var size = (int)(System.Math.Ceiling(width/4d)*4*height+54+(sizeOfPalette));
            var sizeAsByte = BitConverter.GetBytes(size);

            s.Write(sizeAsByte , 0 , 4);//2-5

            s.WriteByte(0);//6
            s.WriteByte(0);//7
            s.WriteByte(0);//8
            s.WriteByte(0);//9

            s.Write(BitConverter.GetBytes(54) , 0 , 4);//10-13

            s.Write(BitConverter.GetBytes(40) , 0 , 4);//14-17

            s.Write(BitConverter.GetBytes(width) , 0 , 4);//18-21
            s.Write(BitConverter.GetBytes(height) , 0 , 4);//22-25

            s.Write(BitConverter.GetBytes(( short ) 1) , 0 , 2);//26-27
            s.Write(BitConverter.GetBytes(( short ) 8) , 0 , 2);//28-29

            s.Write(BitConverter.GetBytes(0) , 0 , 4);//30-33

            s.Write(BitConverter.GetBytes(0) , 0 , 4);//imagesize

            s.Write(BitConverter.GetBytes(unchecked(( int ) 0x00000EC4)) , 0 , 4);
            s.Write(BitConverter.GetBytes(unchecked(( int ) 0x00000EC4)) , 0 , 4);

            s.Write(BitConverter.GetBytes(256) , 0 , 4);//numcolorspalette
            s.Write(BitConverter.GetBytes(0) , 0 , 4);//mostimpcolor
            s.Flush();
            for ( int i = 0; i < 256; i++ )
            {
                Pixel pi = palette[i];
                s.WriteByte(pi.B);
                s.WriteByte(pi.G);
                s.WriteByte(pi.R);
                s.WriteByte(pi.A);
            }
            s.Flush();
            for ( var i = height - 1; i >= 0; i-- )
            {
                Pixel* ptr = data + i * width;

                for ( var j = 0; j < width; j++ )
                {
                    s.WriteByte(ptr->GetGrayTone());
                    ptr++;
                }
                for ( var j = width; j % 4 != 0; j++ )
                {
                    s.WriteByte(0);
                }
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
        public Bitmap Colorize(Pixel dest)
        {
            Bitmap bmpdest = new Bitmap(width,height);
            bmpdest.CopyFromARGB(this.data);

            int count = 0;
            Pixel* ptr = bmpdest.data;
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
        public Bitmap ColorizeRed()
        {
            return this.Colorize(Pixels.Red);
        }
        /// <summary>
        /// Colorize as Green
        /// </summary>
        /// <returns>this</returns>
        public Bitmap ColorizeGreen()
        {
            return this.Colorize(Pixels.Green);
        }
        /// <summary>
        /// Colorize as Blue
        /// </summary>
        /// <returns>this</returns>
        public Bitmap ColorizeBlue()
        {
            return this.Colorize(Pixels.Blue);
        }
        /// <summary>
        /// Colorize as White
        /// </summary>
        /// <returns>this</returns>
        public Bitmap ColorizeWhite()
        {
            return this.Colorize(Pixels.White);
        }
        /// <summary>
        /// Colorize as Black
        /// </summary>
        /// <returns>this</returns>
        public Bitmap ColorizeBlack()
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
        /// Blur the image
        /// </summary>
        /// <param name="blurSize">Size of blur</param>
        /// <returns>Copy of Image</returns>
        public Bitmap GenerateBlurred(byte blurSize)
        {
            Bitmap blurred = new Bitmap(width , height);
            blurred.CopyFromARGB(this.data);

            return blurred.Blur(blurSize);
        }
        /// <summary>
        /// Blur the image
        /// </summary>
        /// <param name="blurSize">Size of blur</param>
        /// <returns>Copy of Image</returns>
        public Task<Bitmap> GenerateBlurredAsync(byte blurSize)
        {
            return Task.Run(() =>
            {
                Bitmap blurred = new Bitmap(width , height);
                blurred.CopyFromARGB(this.data);
                return blurred.Blur(blurSize);
            });
        }
        /// <summary>
        /// Blur the image
        /// </summary>
        /// <param name="blurSize">Size of blur</param>
        /// <returns>this</returns>
        public Bitmap Blur(byte blurSize)
        {
            ValidateBlurSize(blurSize);

            Pixel* debut = data;
            Pixel* ptr=debut;
            Pixel*[] lines  = new Pixel*[height];

            for ( var i = 0; i < height; i++ )
                lines[i] = debut + width * i;

            for ( var i = 0; i < height; i++ )
            {
                BlurLine(lines , i , blurSize);
            }

            return this;
        }

        private static void ValidateBlurSize(byte blurSize)
        {
            if ( blurSize > 32 )
            {
                throw new ArgumentException("Blur size cannot be more than 32");
            }
            else if ( blurSize <= 0 )
            {
                throw new ArgumentException("Blur size cannot be less than 1");
            }
        }

        /// <summary>
        /// Blur the image
        /// </summary>
        /// <param name="blurSize">Size of blur</param>
        /// <returns>this</returns>
        public Task BlurAsync(byte blurSize)
        {
            return Task.Run(() =>
            {
                Blur(blurSize);
            });
        }
        /// <summary>
        /// Create a monoscale copy
        /// </summary>
        /// <returns></returns>
        public Bitmap GenerateMonoscale(BitmapPalette8bpp palette)
        {
            Bitmap blurred = new Bitmap(width , height);
            blurred.CopyFromARGB(this.data);
            blurred.SetMonoscale(palette);
            return blurred;
        }
        /// <summary>
        /// Create a grayscale copy
        /// </summary>
        /// <returns></returns>
        public Bitmap GenerateGrayscale()
        {
            Bitmap blurred = new Bitmap(width , height);
            blurred.CopyFromARGB(this.data);
            blurred.SetGrayscale();
            return blurred;
        }
        /// <summary>
        /// Create a grayscale copy async
        /// </summary>
        /// <returns></returns>
        public Task<Bitmap> GenerateGrayscaleAsync()
        {
            return Task.Run(() =>
            {
                Bitmap blurred = new Bitmap(width , height);
                blurred.CopyFromARGB(this.data);
                blurred.SetGrayscale();
                return blurred;
            });
        }

        public Bitmap RotateFlip(RotateFlipType rotateFlipType)
        {
            Bitmap output=null;
            var rotation = (RotateFlipType)((int)rotateFlipType&3);

            if ( rotation == RotateFlipType.Rotate90 )
            {
                output = new Bitmap(height , width);
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
                output = new Bitmap(height , width);
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
        /// Create a monoscale copy async
        /// </summary>
        /// <returns></returns>
        public Task<Bitmap> GenerateMonoscaleAsync(BitmapPalette8bpp palette)
        {
            return Task.Run(() =>
            {
                Bitmap blurred = new Bitmap(width , height);
                blurred.CopyFromARGB(this.data);
                blurred.SetMonoscale(palette);
                return blurred;
            });
        }
        /// <summary>
        /// Modify the image to grayscale
        /// </summary>
        public void SetGrayscale()
        {
            unsafe
            {
                Pixel* debut = data;
                Pixel* current =debut;
                for ( var l = 0; l < height; l++ )
                {
                    for ( var c = 0; c < width; c++ )
                    {
                        byte d = current->GetGrayTone() ;
                        current->R = d;
                        current->G = d;
                        current->B = d;
                        current++;
                    }
                }
            }
        }
        /// <summary>
        /// Modify the image to grayscale
        /// </summary>
        public void SetMonoscale(BitmapPalette8bpp palette)
        {
            unsafe
            {
                Pixel* debut = data;
                Pixel* current =debut;
                for ( var l = 0; l < height; l++ )
                {
                    for ( var c = 0; c < width; c++ )
                    {
                        byte d = current->GetGrayTone() ;
                        Pixel pi = palette[d];
                        current->R = pi.R;
                        current->G = pi.G;
                        current->B = pi.B;
                        current++;
                    }
                }
            }
        }

        /// <summary>
        /// Modify the image to grayscale
        /// </summary>
        public void SetMonochrome(Pixel? light = null , Pixel? shadow = null , int threshold = 128)
        {
            Pixel lightp = light ?? Pixels.White;
            Pixel shadowp = shadow ?? Pixels.Black;
            unsafe
            {
                Pixel* debut = data;
                Pixel* current =debut;
                for ( var l = 0; l < height; l++ )
                {
                    for ( var c = 0; c < width; c++ )
                    {
                        byte d = current->GetGrayTone() ;
                        if ( d > threshold )
                        {
                            current->R = lightp.R;
                            current->G = lightp.G;
                            current->B = lightp.B;
                            current->A = lightp.A;
                        }
                        else
                        {
                            current->R = shadowp.R;
                            current->G = shadowp.G;
                            current->B = shadowp.B;
                            current->A = shadowp.A;
                        }
                        current++;
                    }
                }
            }
        }
        /// <summary>
        /// Modify the image to monochrome async
        /// </summary>
        public Task SetMonochromeAsync(Pixel? light = null , Pixel? shadow = null , int threshold = 128)
        {
            return Task.Run(() => { SetMonochrome(light , shadow , threshold); });
        }
        /// <summary>
        /// Modify the image to grayscale async
        /// </summary>
        public Task SetGrayscaleAsync()
        {
            return Task.Run(( Action ) SetGrayscale);
        }
        /// <summary>
        /// Modify the image to monoscale async
        /// </summary>
        public Task SetMonoscaleAsync(BitmapPalette8bpp palette)
        {
            return Task.Run(() => { SetMonoscale(palette); });
        }
        private void BlurLine(Pixel*[ ] lines , int line , int blurSize)
        {
            var debVertical =System.Math.Max(line-blurSize,0);
            var endVertical =System.Math.Min(line+blurSize,height);
            uint a,r,g,b,count;
            ulong acount=0;
            for ( var c = 0; c < width; c++ )
            {
                acount = a = r = g = b = count = 0;

                var debHorizontal=System.Math.Max(c - blurSize , 0);
                var endHorizontal=System.Math.Min(c + blurSize , width);

                for ( var x = debHorizontal; x < endHorizontal; x++ )
                    for ( var i = debVertical; i < endVertical; i++ )
                    {
                        Pixel* p = lines[i]+x;
                        a += p->A;
                        r += ( uint ) ( p->R * ( p->A / 255d ) );
                        g += ( uint ) ( p->G * ( p->A / 255d ) );
                        b += ( uint ) ( p->B * ( p->A / 255d ) );
                        acount += p->A;
                        count++;
                    }

                Pixel* current = lines[line]+c;
                current->A = ( byte ) ( a / count );
                if ( acount > 0 )
                {
                    current->R = ( byte ) ( r * 255 / acount );
                    current->G = ( byte ) ( g * 255 / acount );
                    current->B = ( byte ) ( b * 255 / acount );
                }
                else
                {
                    current->R = 0;
                    current->G = 0;
                    current->B = 0;
                }
            }
        }

        public HSBImage ToHSB()
        {
            HSBImage img = new HSBImage(width,height);
            HSB* dest=img.Source;
            Pixel* src = Source;
            int max = width*height;
            for ( var i = 0; i < max; i++ )
            {
                *dest++ = ( src++ )->ToHSB();
            }
            return img;
        }
        public OneBandImage GetAlphaBandImage()
        {
            OneBandImage img = new OneBandImage(width,height);
            Pixel* p = data;
            byte* dest = img.Source;
            int pt = width*height;
            for ( var i = 0; i < pt; i++ )
            {
                *dest++ = p++->A;
            }
            return img;
        }
        public OneBandImage GetRedBandImage()
        {
            OneBandImage img = new OneBandImage(width,height);
            Pixel* p = data;
            byte* dest = img.Source;
            int pt = width*height;
            for ( var i = 0; i < pt; i++ )
            {
                *dest++ = p++->R;
            }
            return img;
        }
        public OneBandImage GetGreenBandImage()
        {
            OneBandImage img = new OneBandImage(width,height);
            Pixel* p = data;
            byte* dest = img.Source;
            int pt = width*height;
            for ( var i = 0; i < pt; i++ )
            {
                *dest++ = p++->G;
            }
            return img;
        }
        public OneBandImage GetBlueBandImage()
        {
            OneBandImage img = new OneBandImage(width,height);
            Pixel* p = data;
            byte* dest = img.Source;
            int pt = width*height;
            for ( var i = 0; i < pt; i++ )
            {
                *dest++ = p++->B;
            }
            return img;
        }
        public OneBandImage GetGrayBandImage()
        {
            OneBandImage img = new OneBandImage(width,height);
            Pixel* p = data;
            byte* dest = img.Source;
            int pt = width*height;
            for ( var i = 0; i < pt; i++ )
            {
                *dest++ = p++->GetGrayTone();
            }
            return img;
        }

        public void ReplaceAlphaBand(OneBandImage img)
        {
            Pixel* o = data;
            byte* inp = img.Source;
            int pt = width*height;
            for ( var i = 0; i < pt; i++ )
            {
                o++->A = *inp++ ;
            }
        }
        public void ReplaceRedBand(OneBandImage img)
        {
            Pixel* o = data;
            byte* inp = img.Source;
            int pt = width*height;
            for ( var i = 0; i < pt; i++ )
            {
                o++->R = *inp++;
            }
        }
        public void ReplaceGreenBand(OneBandImage img)
        {
            Pixel* o = data;
            byte* inp = img.Source;
            int pt = width*height;
            for ( var i = 0; i < pt; i++ )
            {
                o++->G = *inp++;
            }
        }
        public void ReplaceBlueBand(OneBandImage img)
        {
            Pixel* o = data;
            byte* inp = img.Source;
            int pt = width*height;
            for ( var i = 0; i < pt; i++ )
            {
                o++->B = *inp++;
            }
        }
        public void ReplaceGrayBand(OneBandImage img)
        {
            Pixel* o = data;
            byte* inp = img.Source;
            int pt = width*height;
            for ( var i = 0; i < pt; i++ )
            {
                var d = *inp++;
                o++->R = d;
                o++->G = d;
                o++->B = d;
            }
        }
        public ImageStatistics GetStatistics()
        {
            return new ImageStatistics(this);
        }
        public ImageStatistics GetStatistics(Rectangle rectangle)
        {
            var cropped = this.Crop(rectangle);
            var stats= new ImageStatistics(cropped);
            cropped.Dispose();
            return stats;
        }

        public void ApplyFilter(Func<Pixel , Point , Pixel> func)
        {
            for ( var y = 0; y < height; y++ )
                for ( var x = 0; x < width; x++ )
                    this[x , y] = func(this[x , y] , new Point(x , y));
        }
        public ImageProxy Proxy(Rectangle rectangle)
        {
            return new ImageProxy(this , rectangle);
        }

        public static BitmapSubstract operator -(Bitmap bitmapA , Bitmap bitmapB)
        {
            return new BitmapSubstract(bitmapA , bitmapB);
        }
        public static BitmapInvert operator !(Bitmap bitmapA)
        {
            return new BitmapInvert(bitmapA);
        }
        public static BitmapAddition operator +(Bitmap bitmapA , Bitmap bitmapB)
        {
            return new BitmapAddition(bitmapA , bitmapB);
        }
    }

}
