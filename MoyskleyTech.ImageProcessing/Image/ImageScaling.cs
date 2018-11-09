using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MoyskleyTech.ImageProcessing.Image
{
    public unsafe partial class Image<Representation>
    {
        public Rectangle Bounds { get { return new Rectangle(0 , 0 , width , height); } }

        /// <summary>
        /// Let resize a bitmap to a new size
        /// </summary>
        /// <param name="width">New width</param>
        /// <param name="height">New height</param>
        /// <param name="mode">Mode</param>
        /// <returns>New bitmap</returns>
        public Image<Representation> Resize(int width , int height , ScalingMode mode = ScalingMode.Auto)
        {
            return Rescale(width , height , mode);
        }
        /// <summary>
        /// Let resize a bitmap to a new size
        /// </summary>
        /// <param name="width">New width</param>
        /// <param name="height">New height</param>
        /// <param name="mode">Mode</param>
        /// <returns>New bitmap</returns>
        public Image<Representation> Rescale(int width , int height , ScalingMode mode = ScalingMode.Auto)
        {
            if ( width <= 0 )
                throw new ArgumentException(nameof(width) + " cannot be less than 1");
            if ( height <= 0 )
                throw new ArgumentException(nameof(height) + " cannot be less than 1");
            Image<Representation> destination = Image<Representation>.Create(width,height);

            if ( mode == ScalingMode.Auto )
            {
                if ( width > this.width )
                    mode = ScalingMode.LinearFromDestinationHorizontal;
                else
                    mode = ScalingMode.LinearFromSourceHorizontal;

                if ( height > this.height )
                    mode |= ScalingMode.LinearFromDestinationVertical;
                else
                    mode |= ScalingMode.LinearFromSourceVertical;
            }
            else if ( mode == ScalingMode.Average )
            {
                RescaleAverage(destination);
                return destination;
            }
            else if ( mode == ScalingMode.AverageInterpolate )
            {
                RescaleAverage(destination , true);
                return destination;
            }
            var hmode = (ScalingMode)((int)mode&0xFF);

            switch ( hmode )
            {
                case ScalingMode.LinearFromSourceHorizontal:
                    {
                        RescaleLinearFromSourceH(destination , mode);
                    }
                    break;
                case ScalingMode.LinearFromDestinationHorizontal:
                    {
                        RescaleLinearFromDestinationH(destination , mode);
                    }
                    break;
            }
            return destination;
        }
        private void RescaleAverage(Image<Representation> destination , bool interpolate = false)
        {
            double scaleX = (double)width / destination.width;
            double scaleY = (double)height / destination.height;

            Parallel.For(0 , destination.width , (x) => {
                Parallel.For(0 , destination.height , (y) => {
                    int sx = (int)(x*scaleX);
                    int sy = (int)(y*scaleY);
                    var dsx = x*scaleX;
                    var dsy = y*scaleY;
                    int sex = (int)((x+1)*scaleX);
                    int sey = (int)((y+1)*scaleY);
                    var dsex = ((x+1)*scaleX);
                    var dsey = ((y+1)*scaleY);
                    if ( interpolate && ( dsex - dsx < 1 || dsey - dsy < 1 ) )
                    {
                        var destinationpx = this.Get(dsx , dsy);

                        destination[x , y] = destinationpx;
                    }
                    else
                    {
                        if ( sx == sex )
                            sex++;
                        if ( sy == sey )
                            sey++;
                        var destinationpx = Average(sx,sy,sex-sx,sey-sy);
                        destination[x , y] = destinationpx;
                    }
                });
            });
        }
        /// <summary>
        /// Let resize a bitmap to a new size(ASYNC)
        /// </summary>
        /// <param name="width">New width</param>
        /// <param name="height">New height</param>
        /// <param name="mode">Mode</param>
        /// <returns>New bitmap</returns>
        public Task<Image<Representation>> RescaleAsync(int width , int height , ScalingMode mode = ScalingMode.Auto)
        {
            return Task.Run(() => Rescale(width , height , mode));
        }
        private void RescaleLinearFromSourceH(Image<Representation> destination , ScalingMode verticalMode)
        {
            var vmode = (ScalingMode)((int)verticalMode&0xFF00);
            double scaleX = (double)destination.width / width;
            for ( var i = 0; i < width; i++ )
            {
                RescaleV(destination , vmode , i , ( int ) ( i * scaleX ));
            }
        }
        private void RescaleLinearFromDestinationH(Image<Representation> destination , ScalingMode verticalMode)
        {
            var vmode = (ScalingMode)((int)verticalMode&0xFF00);
            double scaleX = (double)width / destination.width;
            for ( var i = 0; i < destination.width; i++ )
            {
                RescaleV(destination , vmode , ( int ) ( i * scaleX ) , i);
            }
        }
        private void RescaleV(Image<Representation> destination , ScalingMode verticalMode , int sx , int ex)
        {
            switch ( verticalMode )
            {
                case ScalingMode.LinearFromSourceVertical:
                    {
                        RescaleLinearFromSourceV(destination , sx , ex);
                    }
                    break;
                case ScalingMode.LinearFromDestinationVertical:
                    {
                        RescaleLinearFromDestinationV(destination , sx , ex);
                    }
                    break;
            }
        }
        private void RescaleLinearFromSourceV(Image<Representation> destination , int sx , int ex)
        {
            double scaleY = (double)destination.height / height;
            for ( var i = 0; i < height; i++ )
            {
                var sy = i;
                var ey = (int)(i*scaleY);
                destination[ex , ey] = this[sx , sy];
            }
        }
        private void RescaleLinearFromDestinationV(Image<Representation> destination , int sx , int ex)
        {
            double scaleY = (double)height / destination.height;
            for ( var i = 0; i < destination.height; i++ )
            {
                var sy = (int)(i*scaleY);
                var ey = i;
                destination[ex , ey] = this[sx , sy];
            }
        }
        /// <summary>
        /// Get a cropped version of image
        /// </summary>
        /// <param name="location"></param>
        /// <returns></returns>
        public Image<Representation> GetSubBitmap(Rectangle location)
        {
            Image<Representation> bmp = Image<Representation>.Create(location.Width,location.Height);
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
        /// Get a cropped version of image
        /// </summary>
        /// <param name="x">Left</param>
        /// <param name="y">Top</param>
        /// <param name="w">Width</param>
        /// <param name="h">Height</param>
        /// <returns></returns>
        public Image<Representation> GetBitmap(int x , int y , int w , int h)
        {
            return GetSubBitmap(new Rectangle(x , y , w , h));
        }
       
        /// <summary>
        /// Get a cropped version of image
        /// </summary>
        /// <param name="rectangle"></param>
        /// <returns></returns>
        public Image<Representation> Clone(Rectangle rectangle)
        { return GetSubBitmap(rectangle); }
    }
}
