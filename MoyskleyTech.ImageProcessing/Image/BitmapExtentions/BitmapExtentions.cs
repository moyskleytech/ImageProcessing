using MoyskleyTech.ImageProcessing.Image.SpecializedBrushes;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using MoyskleyTech.ImageProcessing.Image;
namespace MoyskleyTech.ImageProcessing
{
    /// <summary>
    /// Allow basic operation of bitmap that are added
    /// </summary>
    public static class BitmapExtentions
    {
        /// <summary>
        /// Noise value of bitmap(usefull to hide bitmap content after use)
        /// </summary>
        /// <param name="bmp"></param>
        public static void Noise(this Image<Pixel> bmp)
        {
            Graphics<Pixel> g = Graphics<Pixel>.FromImage(bmp);
            g.Clear(new NoiseBrush());
            g.Dispose();
        }
        /// <summary>
        /// Reduce color to 8bpp
        /// </summary>
        /// <param name="bmp"></param>
        /// <returns></returns>
        public unsafe static BitmapPalette8bpp ReduceColor(this Image<Pixel> bmp)
        {
            BitmapPalette8bpp palette = new BitmapPalette8bpp();
            int pos=0;
            Dictionary<Pixel,byte> positions = new Dictionary<Pixel, byte>();

            int w = bmp.Width;
            int h = bmp.Height;
            int size = w*h;

            for ( var i = 0; i < size; i++ )
            {
                ref Pixel px = ref bmp.GetRef(i);
                byte index=0;
                if ( positions.ContainsKey(px) )
                    index = positions[px];
                else if ( pos < 256 )
                {
                    index = ( byte ) pos;
                    palette[index] = px;
                    positions[px] = ( byte ) pos++;
                }
                px = Pixel.FromArgb(255 , index , index , index);
            }
            return palette;
        }
        public static void Replace<T>(this Image<T> bmp , Rectangle destination , ImageProxy<T> newImage) where T:unmanaged
        {
            Replace(( ImageProxy<T> ) bmp , destination , newImage);
        }
        public static void Replace<T>(this ImageProxy<T> bmp , Rectangle destination , ImageProxy<T> newImage) where T : unmanaged
        {
            var g= Graphics<T>.FromProxy(bmp);
            g.DrawImage(bmp , destination);
        }
        /// <summary>
        /// Allow saving in 8bpp mode
        /// </summary>
        /// <param name="bmp"></param>
        /// <param name="s"></param>
        public unsafe static void SaveReducedColor(this Image<Pixel> bmp , Stream s)
        {
            Image<Pixel> tmp = bmp.Clone();
            BitmapPalette8bpp palette = tmp.ReduceColor();
            new BitmapCodec().Save<Pixel>(tmp , s , palette);
            tmp.Dispose();
        }

        public static Image<Pixel> GenerateBlurred(this Image<Pixel> bmp , int size , double weight)
        {
            return GenerateBlurred(( ImageProxy<Pixel> ) bmp , size , weight);
        }
        public static Image<Pixel> GenerateBlurred(this ImageProxy<Pixel> bmp , int size, double weight)
        {
            var kernel = GraphicsHelper.GetGaussianKernel(size , weight);
            return Convolve(bmp , kernel);
        }
        public static Task<Image<Pixel>> GenerateBlurredAsync(this Image<Pixel> bmp , int size , double weight)
        {
            return GenerateBlurredAsync(( ImageProxy<Pixel> ) bmp , size , weight);
        }
        public static Task<Image<Pixel>> GenerateBlurredAsync(this ImageProxy<Pixel> bmp , int size , double weight)
        {
            return Task.Run(() =>
            {
                var kernel = GraphicsHelper.GetGaussianKernel(size , weight);
                return Convolve(bmp , kernel);
            });
        }
        public static ImageProxy<Pixel> Blur(this Image<Pixel> bmp , int size , double weight)
        {
            return Blur(( ImageProxy<Pixel> ) bmp , size , weight);
        }
        public static ImageProxy<Pixel> Blur(this ImageProxy<Pixel> bmp , int size , double weight)
        {
            Graphics<Pixel> graphics = Graphics<Pixel>.FromProxy(bmp);
            var img = GenerateBlurred(bmp,size,weight);
            graphics.DrawImage(img , 0 , 0);
            img.Dispose();
            return bmp;
        }

        public static Image<T> GenerateBlurred<T>(this Image<T> bmp , int size , double weight)
            where T: unmanaged
        {
            return GenerateBlurred(( ImageProxy<T> ) bmp , size , weight);
        }
        public static Image<T> GenerateBlurred<T>(this ImageProxy<T> bmp , int size , double weight) where T : unmanaged
        {
            var kernel = GraphicsHelper.GetGaussianKernel(size , weight);
            return Convolve(bmp , kernel);
        }
        public static Task<Image<T>> GenerateBlurredAsync<T>(this Image<T> bmp , int size , double weight) where T : unmanaged
        {
            return GenerateBlurredAsync(( ImageProxy<T> ) bmp , size , weight);
        }
        public static Task<Image<T>> GenerateBlurredAsync<T>(this ImageProxy<T> bmp , int size , double weight) where T : unmanaged
        {
            return Task.Run(() =>
            {
                var kernel = GraphicsHelper.GetGaussianKernel(size , weight);
                return Convolve(bmp , kernel);
            });
        }
        public static ImageProxy<T> Blur<T>(this Image<T> bmp , int size , double weight) where T : unmanaged
        {
            return Blur(( ImageProxy<T> ) bmp , size , weight);
        }
        public static ImageProxy<T> Blur<T>(this ImageProxy<T> bmp , int size , double weight) where T : unmanaged
        {
            Graphics<T> graphics = Graphics<T>.FromProxy(bmp);
            var img = GenerateBlurred(bmp,size,weight);
            graphics.DrawImage(img , 0 , 0);
            img.Dispose();
            return bmp;
        }
        public static Task BlurAsync(this Image<Pixel> bmp , int size , double weight)
        {
            return BlurAsync(( ImageProxy<Pixel> ) bmp , size , weight);
        }
        public static Task BlurAsync(this ImageProxy<Pixel> bmp , int size , double weight)
        {
            return Task.Run(() =>
            {
                Blur(bmp , size,weight);
            });
        }
        public static Task BlurAsync<T>(this Image<T> bmp , int size , double weight) where T : unmanaged
        {
            return BlurAsync(( ImageProxy<T> ) bmp , size , weight);
        }
        public static Task BlurAsync<T>(this ImageProxy<T> bmp , int size , double weight) where T : unmanaged
        {
            return Task.Run(() =>
            {
                Blur(bmp , size , weight);
            });
        }

        /// <summary>
        /// Create a monoscale copy
        /// </summary>
        /// <returns></returns>
        public static Image<Pixel> GenerateMonoscale(this Image<Pixel> bmp , BitmapPalette8bpp palette)
        {
            Image<Pixel> blurred = bmp.Clone();
            blurred.SetMonoscale(palette);
            return blurred;
        }
        /// <summary>
        /// Create a grayscale copy
        /// </summary>
        /// <returns></returns>
        public static Image<Pixel> GenerateGrayscale(this Image<Pixel> bmp)
        {
            Image<Pixel> blurred = bmp.Clone();
            blurred.SetGrayscale();
            return blurred;
        }
        /// <summary>
        /// Create a grayscale copy async
        /// </summary>
        /// <returns></returns>
        public static Task<Image<Pixel>> GenerateGrayscaleAsync(this Image<Pixel> bmp)
        {
            return Task.Run(() =>
            {
                Image<Pixel> blurred = bmp.Clone();
                blurred.SetGrayscale();
                return blurred;
            });
        }

        /// <summary>
        /// Create a monoscale copy async
        /// </summary>
        /// <returns></returns>
        public static Task<Image<Pixel>> GenerateMonoscaleAsync(this Image<Pixel> bmp , BitmapPalette8bpp palette)
        {
            return Task.Run(() =>
            {
                Image<Pixel> blurred = bmp.Clone();
                blurred.SetMonoscale(palette);
                return blurred;
            });
        }
        /// <summary>
        /// Modify the image to grayscale
        /// </summary>
        public static void SetGrayscale(this Image<Pixel> bmp)
        {

            var ct = bmp.Width*bmp.Height;
            for ( var i = 0; i < ct; i++ )
            {
                var current = bmp[i];
                byte d = current.GetGrayTone() ;
                current.R = d;
                current.G = d;
                current.B = d;
                bmp[i] = current;
            }
        }
        /// <summary>
        /// Modify the image to grayscale
        /// </summary>
        public static void SetMonoscale(this Image<Pixel> bmp , BitmapPalette8bpp palette)
        {
            var ct = bmp.Width*bmp.Height;
            for ( var i = 0; i < ct; i++ )
            {
                var current = bmp[i];
                Pixel d = palette[current.GetGrayTone()];
                current.R = d.R;
                current.G = d.G;
                current.B = d.B;
                bmp[i] = current;
            }
        }

        /// <summary>
        /// Modify the image to grayscale
        /// </summary>
        public static void SetMonochrome(this Image<Pixel> bmp , Pixel? light = null , Pixel? shadow = null , byte threshold = 128)
        {
            Pixel lightp = light ?? Pixels.White;
            Pixel shadowp = shadow ?? Pixels.Black;
            bmp.ApplyFilter(Filters.ThreasholdCustomColor(threshold , lightp , shadowp));
        }
        /// <summary>
        /// Modify the image to monochrome async
        /// </summary>
        public static Task SetMonochromeAsync(this Image<Pixel> bmp , Pixel? light = null , Pixel? shadow = null , byte threshold = 128)
        {
            return Task.Run(() => { SetMonochrome(bmp , light , shadow , threshold); });
        }
        /// <summary>
        /// Modify the image to grayscale async
        /// </summary>
        public static Task SetGrayscaleAsync(this Image<Pixel> bmp)
        {
            return Task.Run(bmp.SetGrayscale);
        }
        /// <summary>
        /// Modify the image to monoscale async
        /// </summary>
        public static Task SetMonoscaleAsync(this Image<Pixel> bmp , BitmapPalette8bpp palette)
        {
            return Task.Run(() => { SetMonoscale(bmp , palette); });
        }
        private unsafe static void BlurLine(this Image<Pixel> bmp , Pixel*[ ] lines , int line , int blurSize)
        {
            var debVertical =System.Math.Max(line-blurSize,0);
            var endVertical =System.Math.Min(line+blurSize,bmp.Height);
            uint a,r,g,b,count;
            ulong acount=0;
            for ( var c = 0; c < bmp.Width; c++ )
            {
                acount = a = r = g = b = count = 0;

                var debHorizontal=System.Math.Max(c - blurSize , 0);
                var endHorizontal=System.Math.Min(c + blurSize , bmp.Width);

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
        /// <summary>
        /// Conversion to HSB colorspace
        /// </summary>
        /// <returns></returns>
        public static Image<HSB> ToHSB(this Image<Pixel> bmp)
        {
            Image<HSB> img = Image<HSB>.Create(bmp.Width,bmp.Height);
            var ct = bmp.Width*bmp.Height;
            for ( var i = 0; i < ct; i++ )
            {
                img[i] = bmp[i].ToHSB();
            }
            return img;
        }
        /// <summary>
        /// Get a band image for alpha
        /// </summary>
        /// <returns></returns>
        public static Image<byte> GetAlphaBandImage(this Image<Pixel> bmp)
        {
            Image<byte> img = Image<byte>.Create(bmp.Width,bmp.Height);
            var ct = bmp.Width*bmp.Height;
            for ( var i = 0; i < ct; i++ )
            {
                img[i] = bmp[i].A;
            }
            return img;
        }
        /// <summary>
        /// Get a band image for red
        /// </summary>
        /// <returns></returns>
        public static Image<byte> GetRedBandImage(this Image<Pixel> bmp)
        {
            Image<byte> img = Image<byte>.Create(bmp.Width,bmp.Height);
            var ct = bmp.Width*bmp.Height;
            for ( var i = 0; i < ct; i++ )
            {
                img[i] = bmp[i].R;
            }
            return img;
        }
        /// <summary>
        /// Get a band image for green
        /// </summary>
        /// <returns></returns>
        public static Image<byte> GetGreenBandImage(this Image<Pixel> bmp)
        {
            Image<byte> img = Image<byte>.Create(bmp.Width,bmp.Height);
            var ct = bmp.Width*bmp.Height;
            for ( var i = 0; i < ct; i++ )
            {
                img[i] = bmp[i].G;
            }
            return img;
        }
        /// <summary>
        /// Get a band image for blue
        /// </summary>
        /// <returns></returns>
        public static Image<byte> GetBlueBandImage(this Image<Pixel> bmp)
        {
            Image<byte> img = Image<byte>.Create(bmp.Width,bmp.Height);
            var ct = bmp.Width*bmp.Height;
            for ( var i = 0; i < ct; i++ )
            {
                img[i] = bmp[i].B;
            }
            return img;
        }
        /// <summary>
        /// Get a grayscale version(compacted)
        /// </summary>
        /// <returns></returns>
        public static Image<byte> GetGrayBandImage(this Image<Pixel> bmp)
        {
            Image<byte> img = Image<byte>.Create(bmp.Width,bmp.Height);
            var ct = bmp.Width*bmp.Height;
            for ( var i = 0; i < ct; i++ )
            {
                img[i] = bmp[i].GetGrayTone();
            }
            return img;
        }
      
        /// <summary>
        /// Allow replace of a color band
        /// </summary>
        /// <param name="img"></param>
        public static void ReplaceAlphaBand(this Image<Pixel> bmp , Image<byte> img)
        {
            var ct = bmp.Width*bmp.Height;
            for ( var i = 0; i < ct; i++ )
            {
                var c = bmp[i];
                var im = img[i];
                c.A = im;
                bmp[i] = c;
            }
        }
        /// <summary>
        /// Allow replace of a color band
        /// </summary>
        /// <param name="img"></param>
        public static void ReplaceRedBand(this Image<Pixel> bmp , Image<byte> img)
        {
            var ct = bmp.Width*bmp.Height;
            for ( var i = 0; i < ct; i++ )
            {
                var c = bmp[i];
                var im = img[i];
                c.R = im;
                bmp[i] = c;
            }
        }
        /// <summary>
        /// Allow replace of a color band
        /// </summary>
        /// <param name="img"></param>
        public static void ReplaceGreenBand(this Image<Pixel> bmp , Image<byte> img)
        {
            var ct = bmp.Width*bmp.Height;
            for ( var i = 0; i < ct; i++ )
            {
                var c = bmp[i];
                var im = img[i];
                c.G = im;
                bmp[i] = c;
            }
        }
        /// <summary>
        /// Allow replace of a color band
        /// </summary>
        /// <param name="img"></param>
        public static void ReplaceBlueBand(this Image<Pixel> bmp , Image<byte> img)
        {
            var ct = bmp.Width*bmp.Height;
            for ( var i = 0; i < ct; i++ )
            {
                var c = bmp[i];
                var im = img[i];
                c.B = im;
                bmp[i] = c;
            }
        }
        /// <summary>
        /// Allow replace of a color band
        /// </summary>
        /// <param name="img"></param>
        public static void ReplaceGrayBand(this Image<Pixel> bmp , Image<byte> img)
        {
            var ct = bmp.Width*bmp.Height;
            for ( var i = 0; i < ct; i++ )
            {
                var c = bmp[i];
                var im = img[i];
                c.R = im;
                c.G = im;
                c.B = im;
                bmp[i] = c;
            }
        }
        /// <summary>
        /// Allow extraction of statistics from bitmap
        /// </summary>
        /// <returns></returns>
        public static ImageStatistics GetStatistics(this Image<Pixel> bmp)
        {
            return new ImageStatistics(bmp);
        }

        /// <summary>
        /// Allow extraction of statistics from Bitmap using ROI
        /// </summary>
        /// <returns></returns>
        /// 

        public static ImageStatistics GetStatistics(this Image<Pixel> bmp , Rectangle rectangle)
        {
            var cropped = bmp.Proxy(rectangle);
            var stats= new ImageStatistics(cropped);
            return stats;
        }
        public static Image<T> Convolve<T>(this Image<T> sourceBitmap , double[ , ] filterMatrix , double factor = 1 ,
                                                  int bias = 0)
            where T : unmanaged
        {
            var imgpx=Convolve((ImageProxy<T>)sourceBitmap , filterMatrix , factor , bias);
            return imgpx.ConvertBufferTo<T>();
        }
        public static Image<Pixel> Convolve(this Image<Pixel> sourceBitmap , double[ , ] filterMatrix , double factor = 1 ,
                                                   int bias = 0)
        {
            var imgpx=Convolve((ImageProxy<Pixel>)sourceBitmap , filterMatrix , factor , bias);
            return imgpx;
        }
        public static Image<T> Convolve<T>(this ImageProxy<T> sourceBitmap , double[ , ] filterMatrix , double factor = 1 ,
                                                  int bias = 0)
            where T : unmanaged
        {
            var imgpx=Convolve(sourceBitmap.As<T>() , filterMatrix , factor , bias);
            return imgpx.ConvertBufferTo<T>();
        }
        public static Image<Pixel> Convolve(this ImageProxy<Pixel> sourceBitmap , double[ , ] filterMatrix , double factor = 1 ,
                                                   int bias = 0)
        {

            Image<Pixel> dest = Image<Pixel>.Create(sourceBitmap.Width,sourceBitmap.Height);

            double blue = 0.0;
            double green = 0.0;
            double red = 0.0;


            int filterWidth = filterMatrix.GetLength(1);
            int filterHeight = filterMatrix.GetLength(0);


            int filterOffset = (filterWidth-1) / 2;

            int locationX = 0;
            int locationY = 0;

            for ( int offsetY = 0; offsetY <
                sourceBitmap.Height; offsetY++ )
            {
                for ( int offsetX = 0; offsetX <
                    sourceBitmap.Width; offsetX++ )
                {
                    blue = 0;
                    green = 0;
                    red = 0;


                    locationX = offsetX;
                    locationY = offsetY;

                    var src=sourceBitmap[locationX,locationY];

                    for ( int filterY = -filterOffset;
                        filterY <= filterOffset; filterY++ )
                    {
                        for ( int filterX = -filterOffset;
                            filterX <= filterOffset; filterX++ )
                        {

                            var px=sourceBitmap[locationX + filterX,locationY+filterY];

                            blue += ( double ) ( px.B ) *
                                    filterMatrix[filterY + filterOffset ,
                                                        filterX + filterOffset];


                            green += ( double ) ( px.G ) *
                                     filterMatrix[filterY + filterOffset ,
                                                        filterX + filterOffset];


                            red += ( double ) ( px.R ) *
                                   filterMatrix[filterY + filterOffset ,
                                                      filterX + filterOffset];
                        }
                    }


                    blue = factor * blue + bias;
                    green = factor * green + bias;
                    red = factor * red + bias;


                    blue = ( blue > 255 ? 255 : ( blue < 0 ? 0 : blue ) );
                    green = ( green > 255 ? 255 : ( green < 0 ? 0 : green ) );
                    red = ( red > 255 ? 255 : ( red < 0 ? 0 : blue ) );

                    dest[locationX , locationY] = Pixel.FromArgb(src.A , ( byte ) ( red ) , ( byte ) ( green ) , ( byte ) ( blue ));
                }
            }
            return dest;
        }


        #region BGR
        /// <summary>
        /// Get a band image for red
        /// </summary>
        /// <returns></returns>
        public static Image<byte> GetRedBandImage(this Image<BGR> bmp)
        {
            Image<byte> img = Image<byte>.Create(bmp.Width,bmp.Height);
            var ct = bmp.Width*bmp.Height;
            for ( var i = 0; i < ct; i++ )
            {
                img[i] = bmp[i].R;
            }
            return img;
        }
        /// <summary>
        /// Get a band image for green
        /// </summary>
        /// <returns></returns>
        public static Image<byte> GetGreenBandImage(this Image<BGR> bmp)
        {
            Image<byte> img = Image<byte>.Create(bmp.Width,bmp.Height);
            var ct = bmp.Width*bmp.Height;
            for ( var i = 0; i < ct; i++ )
            {
                img[i] = bmp[i].G;
            }
            return img;
        }
        /// <summary>
        /// Get a band image for blue
        /// </summary>
        /// <returns></returns>
        public static Image<byte> GetBlueBandImage(this Image<BGR> bmp)
        {
            Image<byte> img = Image<byte>.Create(bmp.Width,bmp.Height);
            var ct = bmp.Width*bmp.Height;
            for ( var i = 0; i < ct; i++ )
            {
                img[i] = bmp[i].B;
            }
            return img;
        }
        /// <summary>
        /// Get a grayscale version(compacted)
        /// </summary>
        /// <returns></returns>
        public static Image<byte> GetGrayBandImage(this Image<BGR> bmp)
        {
            Image<byte> img = Image<byte>.Create(bmp.Width,bmp.Height);
            var ct = bmp.Width*bmp.Height;
            for ( var i = 0; i < ct; i++ )
            {
                img[i] = bmp[i].ToPixel().GetGrayTone();
            }
            return img;
        }
        /// <summary>
        /// Allow replace of a color band
        /// </summary>
        /// <param name="img"></param>
        public static void ReplaceRedBand(this Image<BGR> bmp , Image<byte> img)
        {
            var ct = bmp.Width*bmp.Height;
            for ( var i = 0; i < ct; i++ )
            {
                var c = bmp[i];
                var im = img[i];
                c.R = im;
                bmp[i] = c;
            }
        }
        /// <summary>
        /// Allow replace of a color band
        /// </summary>
        /// <param name="img"></param>
        public static void ReplaceGreenBand(this Image<BGR> bmp , Image<byte> img)
        {
            var ct = bmp.Width*bmp.Height;
            for ( var i = 0; i < ct; i++ )
            {
                var c = bmp[i];
                var im = img[i];
                c.G = im;
                bmp[i] = c;
            }
        }
        /// <summary>
        /// Allow replace of a color band
        /// </summary>
        /// <param name="img"></param>
        public static void ReplaceBlueBand(this Image<BGR> bmp , Image<byte> img)
        {
            var ct = bmp.Width*bmp.Height;
            for ( var i = 0; i < ct; i++ )
            {
                var c = bmp[i];
                var im = img[i];
                c.B = im;
                bmp[i] = c;
            }
        }
        /// <summary>
        /// Allow replace of a color band
        /// </summary>
        /// <param name="img"></param>
        public static void ReplaceGrayBand(this Image<BGR> bmp , Image<byte> img)
        {
            var ct = bmp.Width*bmp.Height;
            for ( var i = 0; i < ct; i++ )
            {
                var c = bmp[i];
                var im = img[i];
                c.R = im;
                c.G = im;
                c.B = im;
                bmp[i] = c;
            }
        }
        #endregion
        #region RGB
        /// <summary>
        /// Get a band image for red
        /// </summary>
        /// <returns></returns>
        public static Image<byte> GetRedBandImage(this Image<RGB> bmp)
        {
            Image<byte> img = Image<byte>.Create(bmp.Width,bmp.Height);
            var ct = bmp.Width*bmp.Height;
            for ( var i = 0; i < ct; i++ )
            {
                img[i] = bmp[i].R;
            }
            return img;
        }
        /// <summary>
        /// Get a band image for green
        /// </summary>
        /// <returns></returns>
        public static Image<byte> GetGreenBandImage(this Image<RGB> bmp)
        {
            Image<byte> img = Image<byte>.Create(bmp.Width,bmp.Height);
            var ct = bmp.Width*bmp.Height;
            for ( var i = 0; i < ct; i++ )
            {
                img[i] = bmp[i].G;
            }
            return img;
        }
        /// <summary>
        /// Get a band image for blue
        /// </summary>
        /// <returns></returns>
        public static Image<byte> GetBlueBandImage(this Image<RGB> bmp)
        {
            Image<byte> img = Image<byte>.Create(bmp.Width,bmp.Height);
            var ct = bmp.Width*bmp.Height;
            for ( var i = 0; i < ct; i++ )
            {
                img[i] = bmp[i].B;
            }
            return img;
        }
        /// <summary>
        /// Get a grayscale version(compacted)
        /// </summary>
        /// <returns></returns>
        public static Image<byte> GetGrayBandImage(this Image<RGB> bmp)
        {
            Image<byte> img = Image<byte>.Create(bmp.Width,bmp.Height);
            var ct = bmp.Width*bmp.Height;
            for ( var i = 0; i < ct; i++ )
            {
                img[i] = bmp[i].ToPixel().GetGrayTone();
            }
            return img;
        }
        /// <summary>
        /// Allow replace of a color band
        /// </summary>
        /// <param name="img"></param>
        public static void ReplaceRedBand(this Image<RGB> bmp , Image<byte> img)
        {
            var ct = bmp.Width*bmp.Height;
            for ( var i = 0; i < ct; i++ )
            {
                var c = bmp[i];
                var im = img[i];
                c.R = im;
                bmp[i] = c;
            }
        }
        /// <summary>
        /// Allow replace of a color band
        /// </summary>
        /// <param name="img"></param>
        public static void ReplaceGreenBand(this Image<RGB> bmp , Image<byte> img)
        {
            var ct = bmp.Width*bmp.Height;
            for ( var i = 0; i < ct; i++ )
            {
                var c = bmp[i];
                var im = img[i];
                c.G = im;
                bmp[i] = c;
            }
        }
        /// <summary>
        /// Allow replace of a color band
        /// </summary>
        /// <param name="img"></param>
        public static void ReplaceBlueBand(this Image<RGB> bmp , Image<byte> img)
        {
            var ct = bmp.Width*bmp.Height;
            for ( var i = 0; i < ct; i++ )
            {
                var c = bmp[i];
                var im = img[i];
                c.B = im;
                bmp[i] = c;
            }
        }
        /// <summary>
        /// Allow replace of a color band
        /// </summary>
        /// <param name="img"></param>
        public static void ReplaceGrayBand(this Image<RGB> bmp , Image<byte> img)
        {
            var ct = bmp.Width*bmp.Height;
            for ( var i = 0; i < ct; i++ )
            {
                var c = bmp[i];
                var im = img[i];
                c.R = im;
                c.G = im;
                c.B = im;
                bmp[i] = c;
            }
        }
        #endregion

        #region GET_BAND_T
        /// <summary>
        /// Get a band image for red
        /// </summary>
        /// <returns></returns>
        public static Image<byte> GetRedBandImage<T>(this Image<T> bmp)
            where T:unmanaged
        {
            var converter = ColorConvert.GetConversionFrom<T,Pixel>();
            Image<byte> img = Image<byte>.Create(bmp.Width,bmp.Height);
            var ct = bmp.Width*bmp.Height;
            for ( var i = 0; i < ct; i++ )
            {
                img[i] = converter(bmp[i]).R;
            }
            return img;
        }
        /// <summary>
        /// Get a band image for green
        /// </summary>
        /// <returns></returns>
        public static Image<byte> GetGreenBandImage<T>(this Image<T> bmp) where T:unmanaged
        {
            var converter = ColorConvert.GetConversionFrom<T,Pixel>();
            Image<byte> img = Image<byte>.Create(bmp.Width,bmp.Height);
            var ct = bmp.Width*bmp.Height;
            for ( var i = 0; i < ct; i++ )
            {
                img[i] = converter(bmp[i]).G;
            }
            return img;
        }
        /// <summary>
        /// Get a band image for blue
        /// </summary>
        /// <returns></returns>
        public static Image<byte> GetBlueBandImage<T>(this Image<T> bmp) where T:unmanaged
        {
            var converter = ColorConvert.GetConversionFrom<T,Pixel>();
            Image<byte> img = Image<byte>.Create(bmp.Width,bmp.Height);
            var ct = bmp.Width*bmp.Height;
            for ( var i = 0; i < ct; i++ )
            {
                img[i] = converter(bmp[i]).B;
            }
            return img;
        }
        /// <summary>
        /// Get a grayscale version(compacted)
        /// </summary>
        /// <returns></returns>
        public static Image<byte> GetGrayBandImage<T>(this Image<T> bmp) where T:unmanaged
        {
            var converter = ColorConvert.GetConversionFrom<T,Pixel>();
            Image<byte> img = Image<byte>.Create(bmp.Width,bmp.Height);
            var ct = bmp.Width*bmp.Height;
            for ( var i = 0; i < ct; i++ )
            {
                img[i] = converter(bmp[i]).GetGrayTone();
            }
            return img;
        }
        /// <summary>
        /// Allow replace of a color band
        /// </summary>
        /// <param name="img"></param>
        public static void ReplaceRedBand<T>(this Image<T> bmp , Image<byte> img) where T:unmanaged
        {
            var converter = ColorConvert.GetConversionFrom<T,Pixel>();
            var converter2 = ColorConvert.GetConversionFrom<Pixel,T>();
            var ct = bmp.Width*bmp.Height;
            for ( var i = 0; i < ct; i++ )
            { 
                var c = converter(bmp[i]);
                var im = img[i];
                c.R = im;
                bmp[i] = converter2(c);
            }
        }
        /// <summary>
        /// Allow replace of a color band
        /// </summary>
        /// <param name="img"></param>
        public static void ReplaceGreenBand<T>(this Image<T> bmp , Image<byte> img) where T : unmanaged
        {
            var converter = ColorConvert.GetConversionFrom<T,Pixel>();
            var converter2 = ColorConvert.GetConversionFrom<Pixel,T>();
            var ct = bmp.Width*bmp.Height;
            for ( var i = 0; i < ct; i++ )
            {
                var c = converter(bmp[i]);
                var im = img[i];
                c.G = im;
                bmp[i] = converter2(c);
            }
        }
        /// <summary>
        /// Allow replace of a color band
        /// </summary>
        /// <param name="img"></param>
        public static void ReplaceBlueBand<T>(this Image<T> bmp , Image<byte> img) where T : unmanaged
        {
            var converter = ColorConvert.GetConversionFrom<T,Pixel>();
            var converter2 = ColorConvert.GetConversionFrom<Pixel,T>();
            var ct = bmp.Width*bmp.Height;
            for ( var i = 0; i < ct; i++ )
            {
                var c = converter(bmp[i]);
                var im = img[i];
                c.B = im;
                bmp[i] = converter2(c);
            }
        }
        /// <summary>
        /// Allow replace of a color band
        /// </summary>
        /// <param name="img"></param>
        public static void ReplaceGrayBand<T>(this Image<T> bmp , Image<byte> img) where T : unmanaged
        {
            var converter = ColorConvert.GetConversionFrom<T,Pixel>();
            var converter2 = ColorConvert.GetConversionFrom<Pixel,T>();
            var ct = bmp.Width*bmp.Height;
            for ( var i = 0; i < ct; i++ )
            {
                var c = converter(bmp[i]);
                var im = img[i];
                c.R = im;c.G = im;c.B = im;
                bmp[i] = converter2(c);
            }
        }
        #endregion

        #region ARGB_FLOAT
        /// <summary>
        /// Get a band image for red
        /// </summary>
        /// <returns></returns>
        public static Image<float> GetRedBandImage(this Image<ARGB_Float> bmp)
        {
            Image<float> img = Image<float>.Create(bmp.Width,bmp.Height);
            var ct = bmp.Width*bmp.Height;
            for ( var i = 0; i < ct; i++ )
            {
                img[i] = bmp[i].R;
            }
            return img;
        }
        /// <summary>
        /// Get a band image for green
        /// </summary>
        /// <returns></returns>
        public static Image<float> GetGreenBandImage(this Image<ARGB_Float> bmp)
        {
            Image<float> img = Image<float>.Create(bmp.Width,bmp.Height);
            var ct = bmp.Width*bmp.Height;
            for ( var i = 0; i < ct; i++ )
            {
                img[i] = bmp[i].G;
            }
            return img;
        }
        /// <summary>
        /// Get a band image for blue
        /// </summary>
        /// <returns></returns>
        public static Image<float> GetBlueBandImage(this Image<ARGB_Float> bmp)
        {
            Image<float> img = Image<float>.Create(bmp.Width,bmp.Height);
            var ct = bmp.Width*bmp.Height;
            for ( var i = 0; i < ct; i++ )
            {
                img[i] = bmp[i].B;
            }
            return img;
        }
        /// <summary>
        /// Get a band image for blue
        /// </summary>
        /// <returns></returns>
        public static Image<float> GetAlphaBandImage(this Image<ARGB_Float> bmp)
        {
            Image<float> img = Image<float>.Create(bmp.Width,bmp.Height);
            var ct = bmp.Width*bmp.Height;
            for ( var i = 0; i < ct; i++ )
            {
                img[i] = bmp[i].A;
            }
            return img;
        }
        /// <summary>
        /// Get a grayscale version(compacted)
        /// </summary>
        /// <returns></returns>
        public static Image<float> GetGrayBandImage(this Image<ARGB_Float> bmp)
        {
            Image<float> img = Image<float>.Create(bmp.Width,bmp.Height);
            var ct = bmp.Width*bmp.Height;
            for ( var i = 0; i < ct; i++ )
            {
                img[i] = bmp[i].ToPixel().GetGrayTone();
            }
            return img;
        }
        /// <summary>
        /// Allow replace of a color band
        /// </summary>
        /// <param name="img"></param>
        public static void ReplaceRedBand(this Image<ARGB_Float> bmp , Image<float> img)
        {
            var ct = bmp.Width*bmp.Height;
            for ( var i = 0; i < ct; i++ )
            {
                var c = bmp[i];
                var im = img[i];
                c.R = im;
                bmp[i] = c;
            }
        }
        /// <summary>
        /// Allow replace of a color band
        /// </summary>
        /// <param name="img"></param>
        public static void ReplaceGreenBand(this Image<ARGB_Float> bmp , Image<float> img)
        {
            var ct = bmp.Width*bmp.Height;
            for ( var i = 0; i < ct; i++ )
            {
                var c = bmp[i];
                var im = img[i];
                c.G = im;
                bmp[i] = c;
            }
        }
        /// <summary>
        /// Allow replace of a color band
        /// </summary>
        /// <param name="img"></param>
        public static void ReplaceBlueBand(this Image<ARGB_Float> bmp , Image<float> img)
        {
            var ct = bmp.Width*bmp.Height;
            for ( var i = 0; i < ct; i++ )
            {
                var c = bmp[i];
                var im = img[i];
                c.B = im;
                bmp[i] = c;
            }
        }
        /// <summary>
        /// Allow replace of a color band
        /// </summary>
        /// <param name="img"></param>
        public static void ReplaceAlphaBand(this Image<ARGB_Float> bmp , Image<float> img)
        {
            var ct = bmp.Width*bmp.Height;
            for ( var i = 0; i < ct; i++ )
            {
                var c = bmp[i];
                var im = img[i];
                c.A = im;
                bmp[i] = c;
            }
        }
        /// <summary>
        /// Allow replace of a color band
        /// </summary>
        /// <param name="img"></param>
        public static void ReplaceGrayBand(this Image<ARGB_Float> bmp , Image<float> img)
        {
            var ct = bmp.Width*bmp.Height;
            for ( var i = 0; i < ct; i++ )
            {
                var c = bmp[i];
                var im = img[i];
                c.R = im;
                c.G = im;
                c.B = im;
                bmp[i] = c;
            }
        }
        #endregion
        #region ARGB_16bit
        /// <summary>
        /// Get a band image for red
        /// </summary>
        /// <returns></returns>
        public static Image<ushort> GetRedBandImage(this Image<ARGB_16bit> bmp)
        {
            Image<ushort> img = Image<ushort>.Create(bmp.Width,bmp.Height);
            var ct = bmp.Width*bmp.Height;
            for ( var i = 0; i < ct; i++ )
            {
                img[i] = bmp[i].R;
            }
            return img;
        }
        /// <summary>
        /// Get a band image for green
        /// </summary>
        /// <returns></returns>
        public static Image<ushort> GetGreenBandImage(this Image<ARGB_16bit> bmp)
        {
            Image<ushort> img = Image<ushort>.Create(bmp.Width,bmp.Height);
            var ct = bmp.Width*bmp.Height;
            for ( var i = 0; i < ct; i++ )
            {
                img[i] = bmp[i].G;
            }
            return img;
        }
        /// <summary>
        /// Get a band image for blue
        /// </summary>
        /// <returns></returns>
        public static Image<ushort> GetBlueBandImage(this Image<ARGB_16bit> bmp)
        {
            Image<ushort> img = Image<ushort>.Create(bmp.Width,bmp.Height);
            var ct = bmp.Width*bmp.Height;
            for ( var i = 0; i < ct; i++ )
            {
                img[i] = bmp[i].B;
            }
            return img;
        }
        /// <summary>
        /// Get a band image for blue
        /// </summary>
        /// <returns></returns>
        public static Image<ushort> GetAlphaBandImage(this Image<ARGB_16bit> bmp)
        {
            Image<ushort> img = Image<ushort>.Create(bmp.Width,bmp.Height);
            var ct = bmp.Width*bmp.Height;
            for ( var i = 0; i < ct; i++ )
            {
                img[i] = bmp[i].A;
            }
            return img;
        }
        /// <summary>
        /// Get a grayscale version(compacted)
        /// </summary>
        /// <returns></returns>
        public static Image<ushort> GetGrayBandImage(this Image<ARGB_16bit> bmp)
        {
            Image<ushort> img = Image<ushort>.Create(bmp.Width,bmp.Height);
            var ct = bmp.Width*bmp.Height;
            for ( var i = 0; i < ct; i++ )
            {
                img[i] = bmp[i].ToPixel().GetGrayTone();
            }
            return img;
        }
        /// <summary>
        /// Allow replace of a color band
        /// </summary>
        /// <param name="img"></param>
        public static void ReplaceRedBand(this Image<ARGB_16bit> bmp , Image<ushort> img)
        {
            var ct = bmp.Width*bmp.Height;
            for ( var i = 0; i < ct; i++ )
            {
                var c = bmp[i];
                var im = img[i];
                c.R = im;
                bmp[i] = c;
            }
        }
        /// <summary>
        /// Allow replace of a color band
        /// </summary>
        /// <param name="img"></param>
        public static void ReplaceGreenBand(this Image<ARGB_16bit> bmp , Image<ushort> img)
        {
            var ct = bmp.Width*bmp.Height;
            for ( var i = 0; i < ct; i++ )
            {
                var c = bmp[i];
                var im = img[i];
                c.G = im;
                bmp[i] = c;
            }
        }
        /// <summary>
        /// Allow replace of a color band
        /// </summary>
        /// <param name="img"></param>
        public static void ReplaceBlueBand(this Image<ARGB_16bit> bmp , Image<ushort> img)
        {
            var ct = bmp.Width*bmp.Height;
            for ( var i = 0; i < ct; i++ )
            {
                var c = bmp[i];
                var im = img[i];
                c.B = im;
                bmp[i] = c;
            }
        }
        /// <summary>
        /// Allow replace of a color band
        /// </summary>
        /// <param name="img"></param>
        public static void ReplaceAlphaBand(this Image<ARGB_16bit> bmp , Image<ushort> img)
        {
            var ct = bmp.Width*bmp.Height;
            for ( var i = 0; i < ct; i++ )
            {
                var c = bmp[i];
                var im = img[i];
                c.A = im;
                bmp[i] = c;
            }
        }
        /// <summary>
        /// Allow replace of a color band
        /// </summary>
        /// <param name="img"></param>
        public static void ReplaceGrayBand(this Image<ARGB_16bit> bmp , Image<ushort> img)
        {
            var ct = bmp.Width*bmp.Height;
            for ( var i = 0; i < ct; i++ )
            {
                var c = bmp[i];
                var im = img[i];
                c.R = im;
                c.G = im;
                c.B = im;
                bmp[i] = c;
            }
        }
        #endregion
    }
}
