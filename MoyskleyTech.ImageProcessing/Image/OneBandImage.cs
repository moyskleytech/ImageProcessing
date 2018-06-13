using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.InteropServices;
using System.IO;
using System.Threading.Tasks;
using MoyskleyTech.ImageProcessing.Image;
namespace MoyskleyTech.ImageProcessing
{
    /// <summary>
    /// Represent a Bitmap (Array of Pixel)
    /// </summary>
    [NotSerialized]
    public static unsafe class OneBandImage 
    {
        /// <summary>
        /// Serialize bitmap to stream
        /// </summary>
        /// <param name="s">Destination</param>
        public static void ToStream(this Image<byte> image,Stream s)
        {
            image.Save(s);
        }
        /// <summary>
        /// Write bitmap to stream
        /// </summary>
        /// <param name="s">Destination</param>
        ///  /// <param name="palette">Color</param>
        public static void Save(this Image<byte> image , Stream s , BitmapPalette8bpp palette = null)
        {
            palette = palette ?? BitmapPalette8bpp.Grayscale;
            s.WriteByte(( byte ) 'B');//0
            s.WriteByte(( byte ) 'M');//1

            const int sizeOfPalette = 256*4;
            var size = (int)(System.Math.Ceiling(image.Width/4d)*4*image.Height+54+(sizeOfPalette));
            var sizeAsByte = BitConverter.GetBytes(size);

            s.Write(sizeAsByte , 0 , 4);//2-5

            s.WriteByte(0);//6
            s.WriteByte(0);//7
            s.WriteByte(0);//8
            s.WriteByte(0);//9

            s.Write(BitConverter.GetBytes(54) , 0 , 4);//10-13

            s.Write(BitConverter.GetBytes(40) , 0 , 4);//14-17

            s.Write(BitConverter.GetBytes(image.Width) , 0 , 4);//18-21
            s.Write(BitConverter.GetBytes(image.Height) , 0 , 4);//22-25

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
            for ( var i = image.Height - 1; i >= 0; i-- )
            {
                byte* ptr = image.Source + i * image.Width;

                for ( var j = 0; j < image.Width; j++ )
                {
                    s.WriteByte(*ptr);
                    ptr++;
                }
                for ( var j = image.Width; j % 4 != 0; j++ )
                {
                    s.WriteByte(0);
                }
            }
        }
      
        /// <summary>
        /// Copy image from raw Grayscale
        /// </summary>
        /// <param name="raw">Input data</param>
        public static void CopyFromRawGrayscale(this Image<byte> image,byte[ ] raw)
        {
            int size = image.Width*image.Height;
            byte*ptr = image.Source;
            for ( var i = 0; i < size; i++ )
            {
                *ptr++ = raw[i];
            }
        }
        /// <summary>
        /// Create raw 8 bit per pixel array for specified image
        /// </summary>
        /// <returns>Monoscale raw bitmap</returns>
        public static byte[ ] CreateRaw8bppArray(this Image<byte> image)
        {
            MemoryStream ms = new MemoryStream();
            int size = image.Width*image.Height;
            byte*ptr = image.Source;
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
        public static Image<byte> Blur(this Image<byte> image,byte blurSize)
        {
            ValidateBlurSize(blurSize);

            byte* debut = image.Source;
            byte* ptr=debut;
            byte*[] lines  = new byte*[image.Height];

            for ( var i = 0; i < image.Height; i++ )
                lines[i] = debut + image.Width * i;

            for ( var i = 0; i < image.Height; i++ )
            {
                BlurLine(image,lines , i , blurSize);
            }

            return image;
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
        public static Task BlurAsync(this Image<byte> image,byte blurSize)
        {
            return Task.Run(() =>
            {
                Blur(image,blurSize);
            });
        }
     
        private static void BlurLine(this Image<byte> image,byte*[ ] lines , int line , int blurSize)
        {
            var debVertical =System.Math.Max(line-blurSize,0);
            var endVertical =System.Math.Min(line+blurSize,image.Height);
            uint a,r,g,b,count;
            ulong acount=0;
            for ( var c = 0; c < image.Width; c++ )
            {
                acount = a = r = g = b = count = 0;

                var debHorizontal=System.Math.Max(c - blurSize , 0);
                var endHorizontal=System.Math.Min(c + blurSize , image.Width);

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
        /// <summary>
        /// Mix multiple bands into a bitmap
        /// </summary>
        /// <param name="A"></param>
        /// <param name="R"></param>
        /// <param name="G"></param>
        /// <param name="B"></param>
        /// <returns></returns>
        public static Image<Pixel> MixBands(Image<byte> A , Image<byte> R , Image<byte> G , Image<byte> B)
        {
            Image<Pixel> bmp = Image<Pixel>.Create(A.Width,A.Height);
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
        /// <summary>
        /// Mix multiple bands into a bitmap
        /// </summary>
        /// <param name="R"></param>
        /// <param name="G"></param>
        /// <param name="B"></param>
        /// <returns></returns>
        public static Image<Pixel> MixBands(Image<byte> R , Image<byte> G , Image<byte> B)
        {
            Image<Pixel> bmp = Image<Pixel>.Create(R.Width,R.Height);
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
    }
}
