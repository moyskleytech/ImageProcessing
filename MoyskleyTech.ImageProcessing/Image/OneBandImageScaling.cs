using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MoyskleyTech.ImageProcessing.Image
{
    public unsafe partial class OneBandImage
    {
        public OneBandImage Rescale(int width , int height , ScalingMode mode = ScalingMode.Auto)
        {
            if ( width <= 0 )
                throw new ArgumentException(nameof(width) + " cannot be less than 1");
            if ( height <= 0 )
                throw new ArgumentException(nameof(height) + " cannot be less than 1");
            OneBandImage destination = new OneBandImage(width,height);
           
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
        private void RescaleAverage(OneBandImage destination , bool interpolate = false)
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
                    if ( interpolate && ( dsex-dsx<1 || dsey -dsy<1 ) )
                    {
                       
                        var dx = dsx-sx;
                        var dy = dsy-sy;
                        var dx2 = 1-dx;
                        var dy2 = 1-dy;
                        var ipx = this[sx,sy];
                        double sa=ipx;
                        if ( sx > 0 )
                        {
                            var px = this[sx - 1 , sy];
                            sa += px * dx2;
                        }
                        if ( sy > 0 )
                        {
                            var px=this[sx , sy - 1];
                            sa += px * dy2;
                        }
                        if ( sx < width - 1 )
                        {
                            var px = this[sex + 1 , sey];
                            sa += px * dx;
                        }
                        if ( sy < height - 1 )
                        {
                            var px = this[sex  , sey+ 1];
                            sa += px * dy;
                        }
                        System.Byte destinationpx = new System.Byte();
                        
                        destinationpx = ( byte ) ( sa / 3 );

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
                        ulong sa=0;
                        for ( var i = sx; i < sex; i++ )
                        {
                            for ( var j = sy; j < sey; j++ )
                            {
                                count++;
                                System.Byte source = this[i,j];
                                acount += 255;
                                sa += source;
                            }
                        }
                        System.Byte destinationpx = new System.Byte();
                        if ( acount > 0 )
                        {
                            destinationpx = ( byte ) ( sa * 255 / acount );
                        }
                        destination[x , y] = destinationpx;
                    }
                }
            }
        }
        public Task<OneBandImage> RescaleAsync(int width , int height , ScalingMode mode = ScalingMode.Auto)
        {
            return Task.Run(() => Rescale(width , height , mode));
        }
        private void RescaleLinearFromSourceH(OneBandImage destination , ScalingMode verticalMode)
        {
            var vmode = (ScalingMode)((int)verticalMode&0xFF00);
            double scaleX = (double)destination.width / width;
            for ( var i = 0; i < width; i++ )
            {
                RescaleV(destination , vmode , i , ( int ) ( i * scaleX ));
            }
        }
        private void RescaleLinearFromDestinationH(OneBandImage destination , ScalingMode verticalMode)
        {
            var vmode = (ScalingMode)((int)verticalMode&0xFF00);
            double scaleX = (double)width / destination.width;
            for ( var i = 0; i < destination.width; i++ )
            {
                RescaleV(destination , vmode , ( int ) ( i * scaleX ) , i);
            }
        }
        private void RescaleV(OneBandImage destination , ScalingMode verticalMode , int sx , int ex)
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
        private void RescaleLinearFromSourceV(OneBandImage destination , int sx , int ex)
        {
            double scaleY = (double)destination.height / height;
            for ( var i = 0; i < height; i++ )
            {
                var sy = i;
                var ey = (int)(i*scaleY);
                destination[ex , ey] = this[sx , sy];
            }
        }
        private void RescaleLinearFromDestinationV(OneBandImage destination , int sx , int ex)
        {
            double scaleY = (double)height / destination.height;
            for ( var i = 0; i < destination.height; i++ )
            {
                var sy = (int)(i*scaleY);
                var ey = i;
                destination[ex , ey] = this[sx , sy];
            }
        }
    }
}
