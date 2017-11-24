using MoyskleyTech.ImageProcessing.Image;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MoyskleyTech.ImageProcessing.Recognition.Border
{
    public class Sobel
    {
        //Thanks to Epoch abuse https://epochabuse.com/csharp-sobel/
        public static Bitmap ConvolutionFilter(Bitmap sourceImage , double[ , ] xkernel , double[ , ] ykernel , double factor = 1 , int bias = 0)
        {
            Bitmap result = sourceImage.Clone();
            //Image dimensions stored in variables for convenience
            int width = sourceImage.Width;
            int height = sourceImage.Height;
            
            //Get the total number of bytes in your image - 32 bytes per pixel x image width x image height -> for 32bpp images
            int bytes = sourceImage.Width*sourceImage.Height;

            //Create variable for pixel data for each kernel
            double xr = 0.0;
            double xg = 0.0;
            double xb = 0.0;
            double xa = 0.0;
            double yr = 0.0;
            double yg = 0.0;
            double yb = 0.0;
            double ya = 0.0;
            double rt = 0.0;
            double gt = 0.0;
            double bt = 0.0;
            double at = 0.0;

            //This is how much your center pixel is offset from the border of your kernel
            //Sobel is 3x3, so center is 1 pixel from the kernel border
            int filterOffset = 1;
            int calcOffset = 0;
            int byteOffset = 0;

            //Start with the pixel that is offset 1 from top and 1 from the left side
            //this is so entire kernel is on your image
            for ( int OffsetY = filterOffset; OffsetY < height - filterOffset; OffsetY++ )
            {
                for ( int OffsetX = filterOffset; OffsetX < width - filterOffset; OffsetX++ )
                {
                    //reset rgb values to 0
                    xr = xg = xb = yr = yg = yb = ya = xa = 0;
                    rt = gt = bt = at = 0.0;

                    //position of the kernel center pixel
                    byteOffset = OffsetY * sourceImage.Width + OffsetX;

                    //kernel calculations
                    for ( int filterY = -filterOffset; filterY <= filterOffset; filterY++ )
                    {
                        for ( int filterX = -filterOffset; filterX <= filterOffset; filterX++ )
                        {
                            calcOffset = byteOffset + filterX * 4 + filterY * sourceImage.Width;
                            xb += ( double ) ( sourceImage[calcOffset].B ) * xkernel[filterY + filterOffset , filterX + filterOffset];
                            xg += ( double ) ( sourceImage[calcOffset].G ) * xkernel[filterY + filterOffset , filterX + filterOffset];
                            xr += ( double ) ( sourceImage[calcOffset].R ) * xkernel[filterY + filterOffset , filterX + filterOffset];
                            xa += ( double ) ( sourceImage[calcOffset].A ) * xkernel[filterY + filterOffset , filterX + filterOffset];
                            yb += ( double ) ( sourceImage[calcOffset].B ) * ykernel[filterY + filterOffset , filterX + filterOffset];
                            yg += ( double ) ( sourceImage[calcOffset].G ) * ykernel[filterY + filterOffset , filterX + filterOffset];
                            yr += ( double ) ( sourceImage[calcOffset].G ) * ykernel[filterY + filterOffset , filterX + filterOffset];
                            ya += ( double ) ( sourceImage[calcOffset].A ) * ykernel[filterY + filterOffset , filterX + filterOffset];
                        }
                    }

                    //total rgba values for this pixel
                    bt = System.Math.Sqrt(( xb * xb ) + ( yb * yb ));
                    gt = System.Math.Sqrt(( xg * xg ) + ( yg * yg ));
                    rt = System.Math.Sqrt(( xr * xr ) + ( yr * yr ));
                    at = System.Math.Sqrt(( xa * xa ) + ( ya * ya ));
                    //set limits, bytes can hold values from 0 up to 255;
                    if ( bt > 255 ) bt = 255;
                    else if ( bt < 0 ) bt = 0;
                    if ( gt > 255 ) gt = 255;
                    else if ( gt < 0 ) gt = 0;
                    if ( rt > 255 ) rt = 255;
                    else if ( rt < 0 ) rt = 0;
                    if ( at > 255 ) at = 255;
                    else if ( at < 0 ) at = 0;

                    var px = new Pixel();
                    px.B = ( byte ) ( bt );
                    px.G = ( byte ) ( gt );
                    px.R = ( byte ) ( rt );
                    px.A = 255;// ( byte ) ( at );
                    result[byteOffset] = px;
                }
            }

            return result;
        }
        public static double[ , ] xSobel
        {
            get
            {
                return new double[ , ]
                {
                    { -1, 0, 1 },
                    { -2, 0, 2 },
                    { -1, 0, 1 }
                };
            }
        }

        //Sobel operator kernel for vertical pixel changes
        public static double[ , ] ySobel
        {
            get
            {
                return new double[ , ]
                {
                    {  1,  2,  1 },
                    {  0,  0,  0 },
                    { -1, -2, -1 }
                };
            }
        }
    }
}
