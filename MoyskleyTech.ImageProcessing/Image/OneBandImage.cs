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
    public unsafe partial class OneBandImage : Image<byte>, IDisposable
    {
        public byte* Source { get => data; }
        /// <summary>
        /// Create a bitmap using Width and Height
        /// </summary>
        /// <param name="w">Width</param>
        /// <param name="h">Height</param>
        public OneBandImage(int w , int h):base(w,h)
        {
        }
        /// <summary>
        /// Create a bitmap using Width and Height and source from RGBA format
        /// </summary>
        /// <param name="w">Width</param>
        /// <param name="h">Height</param>
        /// <param name="raw">Source to copy</param>
        public OneBandImage(int w , int h , byte[ ] raw):base(w,h)
        {
            //Allocate
            Marshal.Copy(raw , 0 , this.raw , w * h * sizeof(byte));
        }
               
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
        /// <param name="pos">As 1 dim array</param>
        /// <returns>Pixel</returns>
        public override byte this[int pos]
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
        public static Bitmap MixBands(Image<byte> A , Image<byte> R , Image<byte> G , Image<byte> B)
        {
            Bitmap bmp = new Bitmap(A.Width,A.Height);
            byte* a=(byte*)A.DataPointer.ToPointer();
            byte* r=(byte*)R.DataPointer.ToPointer();
            byte* g=(byte*)G.DataPointer.ToPointer();
            byte* b=(byte*)B.DataPointer.ToPointer();
            Pixel* dest = bmp.Source;
            var pt = A.Width*A.Height;
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
        public static Bitmap MixBands(Image<byte> R , Image<byte> G , Image<byte> B)
        {
            Bitmap bmp = new Bitmap(R.Width,R.Height);
            byte* r=(byte*)R.DataPointer.ToPointer();
            byte* g=(byte*)G.DataPointer.ToPointer();
            byte* b=(byte*)B.DataPointer.ToPointer();
            Pixel* dest = bmp.Source;
            var pt = R.Width*R.Height;
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
