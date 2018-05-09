using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MoyskleyTech.ImageProcessing.Image
{
    public unsafe partial class Bitmap
    {
        /// <summary>
        /// Let resize a bitmap to a new size
        /// </summary>
        /// <param name="width">New width</param>
        /// <param name="height">New height</param>
        /// <param name="mode">Mode</param>
        /// <returns>New bitmap</returns>
        public Bitmap Resize(int width , int height , ScalingMode mode = ScalingMode.Auto)
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
        public Bitmap Rescale(int width , int height , ScalingMode mode = ScalingMode.Auto)
        {
            if ( width <= 0 )
                throw new ArgumentException(nameof(width) + " cannot be less than 1");
            if ( height <= 0 )
                throw new ArgumentException(nameof(height) + " cannot be less than 1");
            Bitmap destination = new Bitmap(width,height);

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
        private void RescaleAverage(Bitmap destination , bool interpolate = false)
        {
            double scaleX = (double)width / destination.width;
            double scaleY = (double)height / destination.height;
            for ( var x = 0; x < destination.width; x++ )
            {
                for ( var y = 0; y < destination.height; y++ )
                {
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

                        var dx = dsx-sx;
                        var dy = dsy-sy;
                        var dx2 = 1-dx;
                        var dy2 = 1-dy;
                        var ipx = this[sx,sy];
                        double sa=ipx.A,sr=ipx.R,sg=ipx.G,sb=ipx.B;
                        if ( sx > 0 )
                        {
                            var px = this[sx - 1 , sy];
                            sa += px.A * dx2;
                            sr += px.R * dx2;
                            sg += px.G * dx2;
                            sb += px.B * dx2;
                        }
                        if ( sy > 0 )
                        {
                            var px=this[sx , sy - 1];
                            sa += px.A * dy2;
                            sr += px.R * dy2;
                            sg += px.G * dy2;
                            sb += px.B * dy2;
                        }
                        if ( sx < width - 1 )
                        {
                            var px = this[sex + 1 , sey];
                            sa += px.A * dx;
                            sr += px.R * dx;
                            sg += px.G * dx;
                            sb += px.B * dx;
                        }
                        if ( sy < height - 1 )
                        {
                            var px = this[sex  , sey+ 1];
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

                        destination[x , y] = destinationpx;
                    }
                    else
                    {
                        if ( sex == sx )
                            sex++;
                        if ( sey == sy )
                            sey++;
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
                        destination[x , y] = destinationpx;
                    }
                }
            }
        }
        /// <summary>
        /// Let resize a bitmap to a new size(ASYNC)
        /// </summary>
        /// <param name="width">New width</param>
        /// <param name="height">New height</param>
        /// <param name="mode">Mode</param>
        /// <returns>New bitmap</returns>
        public Task<Bitmap> RescaleAsync(int width , int height , ScalingMode mode = ScalingMode.Auto)
        {
            return Task.Run(() => Rescale(width , height , mode));
        }
        private void RescaleLinearFromSourceH(Bitmap destination , ScalingMode verticalMode)
        {
            var vmode = (ScalingMode)((int)verticalMode&0xFF00);
            double scaleX = (double)destination.width / width;
            for ( var i = 0; i < width; i++ )
            {
                RescaleV(destination , vmode , i , ( int ) ( i * scaleX ));
            }
        }
        private void RescaleLinearFromDestinationH(Bitmap destination , ScalingMode verticalMode)
        {
            var vmode = (ScalingMode)((int)verticalMode&0xFF00);
            double scaleX = (double)width / destination.width;
            for ( var i = 0; i < destination.width; i++ )
            {
                RescaleV(destination , vmode , ( int ) ( i * scaleX ) , i);
            }
        }
        private void RescaleV(Bitmap destination , ScalingMode verticalMode , int sx , int ex)
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
        private void RescaleLinearFromSourceV(Bitmap destination , int sx , int ex)
        {
            double scaleY = (double)destination.height / height;
            for ( var i = 0; i < height; i++ )
            {
                var sy = i;
                var ey = (int)(i*scaleY);
                destination[ex , ey] = this[sx , sy];
            }
        }
        private void RescaleLinearFromDestinationV(Bitmap destination , int sx , int ex)
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
        public Bitmap GetSubBitmap(Rectangle location)
        {
            Bitmap bmp = new Bitmap(location.Width,location.Height);
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
        public Bitmap GetBitmap(int x , int y , int w , int h)
        {
            return GetSubBitmap(new Rectangle(x , y , w , h));
        }
        /// <summary>
        /// Get a cropped version of image
        /// </summary>
        /// <param name="rectangle"></param>
        /// <returns></returns>
        public new Bitmap Crop(Rectangle rectangle)
        { return GetSubBitmap(rectangle); }
        /// <summary>
        /// Get a cropped version of image
        /// </summary>
        /// <param name="rectangle"></param>
        /// <returns></returns>
        public Bitmap Clone(Rectangle rectangle)
        { return GetSubBitmap(rectangle); }
    }
}
