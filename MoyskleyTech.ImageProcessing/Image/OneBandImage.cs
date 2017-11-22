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
    [NotSerialized]
    public unsafe partial class OneBandImage : IDisposable
    {
        private readonly int width,height;
        private readonly byte* data;
        private readonly IntPtr raw;
        /// <summary>
        /// Create a bitmap using Width and Height
        /// </summary>
        /// <param name="w">Width</param>
        /// <param name="h">Height</param>
        public OneBandImage(int w , int h)
        {
            //Allocate
            raw = Marshal.AllocHGlobal(w * h * sizeof(byte));
            data = ( byte* ) raw.ToPointer();
            width = w;
            height = h;
        }
        /// <summary>
        /// Create a bitmap using Width and Height and source from RGBA format
        /// </summary>
        /// <param name="w">Width</param>
        /// <param name="h">Height</param>
        /// <param name="raw">Source to copy</param>
        public OneBandImage(int w , int h , byte[ ] raw)
        {
            //Allocate
            this.raw = Marshal.AllocHGlobal(w * h * sizeof(byte));
            data = ( byte* ) this.raw.ToPointer();
            Marshal.Copy(raw , 0 , this.raw , w * h * sizeof(byte));
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
        ~OneBandImage()
        {
            Dispose();
        }
        /// <summary>
        /// Get pixel from coordinate
        /// </summary>
        /// <param name="x">X position</param>
        /// <param name="y">Y position</param>
        /// <returns>Pixel</returns>
        public byte Get(int x , int y)
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
        public byte* Source { get { return data; } }
        /// <summary>
        /// Get pixel from coordinate
        /// </summary>
        /// <param name="x">X position</param>
        /// <param name="y">Y position</param>
        /// <returns>Pixel</returns>
        public byte this[int x , int y]
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
        public byte this[int pos]
        {
            get
            {
                if ( pos >= 0 && pos < width * height )
                    return data[pos];
                else
                    return new byte();
            }
            set
            {
                if ( pos >= 0 && pos < width * height )
                    data[pos] = value;
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
        /// Write bitmap to stream
        /// </summary>
        /// <param name="s">Destination</param>
        public void Save(Stream s , BitmapPalette8bpp palette = null)
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
                byte* ptr = data + i * width;

                for ( var j = 0; j < width; j++ )
                {
                    s.WriteByte(*ptr);
                    ptr++;
                }
                for ( var j = width; j % 4 != 0; j++ )
                {
                    s.WriteByte(0);
                }
            }
        }
      
        /// <summary>
        /// Copy image from raw Grayscale
        /// </summary>
        /// <param name="raw">Input data</param>
        /// <param name="palette">Color palette</param>
        public void CopyFromRawGrayscale(byte[ ] raw)
        {
            int size = width*height;
            byte*ptr = data;
            for ( var i = 0; i < size; i++ )
            {
                *ptr++ = raw[i];
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
            byte*ptr = data;
            for ( var i = 0; i < size; i++ )
            {
                ms.WriteByte(*ptr);
                ptr++;
            }
            return ms.ToArray();
        }


        /// <summary>
        /// Blur the image
        /// </summary>
        /// <param name="blurSize">Size of blur</param>
        /// <returns>this</returns>
        public OneBandImage Blur(byte blurSize)
        {
            ValidateBlurSize(blurSize);

            byte* debut = data;
            byte* ptr=debut;
            byte*[] lines  = new byte*[height];

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
     

        public OneBandImage RotateFlip(RotateFlipType rotateFlipType)
        {
            OneBandImage output=null;
            var rotation = (RotateFlipType)((int)rotateFlipType&3);

            if ( rotation == RotateFlipType.Rotate90 )
            {
                output = new OneBandImage(height , width);
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
                output = new OneBandImage(height , width);
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

        private void BlurLine(byte*[ ] lines , int line , int blurSize)
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
                        byte* p = lines[i]+x;
                        a += *p;
                        acount += 255;
                        count++;
                    }

                byte* current = lines[line]+c;
                if ( acount > 0 )
                {
                    *current =  (byte)( a * 255 / acount );
                }
                else
                {
                    *current = 0;
                }
            }
        }
        public static Bitmap MixBands(OneBandImage A, OneBandImage R, OneBandImage G, OneBandImage B)
        {
            Bitmap bmp = new Bitmap(A.width,A.height);
            byte* a=A.Source;
            byte* r=R.Source;
            byte* g=G.Source;
            byte* b=B.Source;
            Pixel* dest = bmp.Source;
            var pt = A.width*A.height;
            for ( var i = 0; i < pt; i++ )
            {
                dest->A = *a++;
                dest->R = *r++;
                dest->G = *g++;
                dest->B = *b++;
                dest++;
            }
            return bmp;
        }
        public static Bitmap MixBands(OneBandImage R , OneBandImage G , OneBandImage B)
        {
            Bitmap bmp = new Bitmap(R.width,R.height);
            byte* r=R.Source;
            byte* g=G.Source;
            byte* b=B.Source;
            Pixel* dest = bmp.Source;
            var pt = R.width*R.height;
            for ( var i = 0; i < pt; i++ )
            {
                dest->A = 255;
                dest->R = *r++;
                dest->G = *g++;
                dest->B = *b++;
                dest++;
            }
            return bmp;
        }
        public void CopyTo(OneBandImage other)
        {
            other.CopyFromRawGrayscale(this.CreateRaw8bppArray());
        }
        public void Clear(byte b)
        {
            byte* r=data;
            var pt = width*height;
            for ( var i = 0; i < pt; i++ )
                *r++ = b;
        }
    }
}
