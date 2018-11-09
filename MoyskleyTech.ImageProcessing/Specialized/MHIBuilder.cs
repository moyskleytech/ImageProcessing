using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MoyskleyTech.ImageProcessing.Image;
namespace MoyskleyTech.ImageProcessing.Specialized
{
    public class MHIBuilderBGR
    {
        private Image<BGR> build;
        private Image<BGR> previous;
        public double TimeFactor { get; set; } = 0.8;
        public MHIBuilderBGR(int w , int h)
        {
            build = Image<BGR>.FilledWith(w , h , Pixels.Black.ToBGR());
        }

        public ImageProxy<BGR> GetMHI()
        {
            return new ImageProxy<BGR>(build , build.Bounds , true);
        }

        public void AddFrame(ImageProxy<BGR> image)
        {
            if ( previous != null )
            {
                var w = build.Width;
                var h = build.Height;
                for ( var x = 0; x < w; x++ )
                    for ( var y = 0; y < h; y++ )
                    {
                        var b=build[x,y];
                        b.R = ( byte ) ( b.R * TimeFactor );
                        b.G = ( byte ) ( b.G * TimeFactor );
                        b.B = ( byte ) ( b.B * TimeFactor );
                        build[x , y] = b;
                    }
                for ( var x = 0; x < w; x++ )
                    for ( var y = 0; y < h; y++ )
                    {
                        var n = image[x,y];
                        var p = previous[x,y];
                        var b = build[x,y];
                        b.R = ( byte ) Math.Max(b.R , Math.Abs(n.R - p.R));
                        b.G = ( byte ) Math.Max(b.G , Math.Abs(n.G - p.G));
                        b.B = ( byte ) Math.Max(b.B , Math.Abs(n.B - p.B));
                        build[x , y] = b;
                    }

                previous.Dispose();
            }
            previous = image.ToImage();
        }
        public void AddFrame<T>(ImageProxy<T> image)
            where T:unmanaged
        {
            AddFrame(image.As<BGR>());
        }
    }
    public class MHIBuilderByte
    {
        private Image<byte> build;
        private Image<byte> previous;
        public double TimeFactor { get; set; } = 0.8;
        public MHIBuilderByte(int w , int h)
        {
            build = Image<byte>.FilledWith(w , h , 0);
        }

        public ImageProxy<byte> GetMHI()
        {
            return new ImageProxy<byte>(build , build.Bounds , true);
        }

        public void AddFrame(ImageProxy<byte> image)
        {
            if ( previous != null )
            {
                var w = build.Width;
                var h = build.Height;
                for ( var x = 0; x < w; x++ )
                    for ( var y = 0; y < h; y++ )
                    {
                        var b=build[x,y];
                        b = ( byte ) ( b * TimeFactor );
                        build[x , y] = b;
                    }
                for ( var x = 0; x < w; x++ )
                    for ( var y = 0; y < h; y++ )
                    {
                        var n = image[x,y];
                        var p = previous[x,y];
                        var b = build[x,y];
                        b = ( byte ) Math.Max(b , Math.Abs(n - p));
                        build[x , y] = b;
                    }

                previous.Dispose();
            }
            previous = image.ToImage();
        }
        public void AddFrame<T>(ImageProxy<T> image)
            where T : unmanaged
        {
            AddFrame(image.As<byte>());
        }
    }
}
