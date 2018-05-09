using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MoyskleyTech.ImageProcessing.Image
{
#pragma warning disable CS1591
    public unsafe partial class HighRangeImage
    {
        public HighRangeImage Resize(int width , int height , ScalingMode mode = ScalingMode.Auto)
        {
            return Rescale(width , height , mode);
        }
        public HighRangeImage Rescale(int width , int height , ScalingMode mode = ScalingMode.Auto)
        {
            if ( width <= 0 )
                throw new ArgumentException(nameof(width) + " cannot be less than 1");
            if ( height <= 0 )
                throw new ArgumentException(nameof(height) + " cannot be less than 1");
            HighRangeImage destination = new HighRangeImage(width,height);

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
        private void RescaleAverage(HighRangeImage destination , bool interpolate = false)
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
                        ARGB_16bit destinationpx = new ARGB_16bit()
                        {
                            R = ( ushort ) ( sr / 3 ),
                            G = ( ushort ) ( sg / 3 ),
                            B = ( ushort ) ( sb / 3 ),
                            A = ( ushort ) ( sa / 3 )
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
                                ARGB_16bit source = this[i,j];
                                acount += source.A;
                                sa += source.A;
                                sr += source.R;
                                sg += source.G;
                                sb += source.B;
                            }
                        }
                        ARGB_16bit destinationpx = new ARGB_16bit
                        {
                            A = ( ushort ) ( sa / count )
                        };
                        if ( acount > 0 )
                        {
                            destinationpx.R = ( ushort ) ( sr * ushort.MaxValue / acount );
                            destinationpx.G = ( ushort ) ( sg * ushort.MaxValue / acount );
                            destinationpx.B = ( ushort ) ( sb * ushort.MaxValue / acount );
                        }
                        destination[x , y] = destinationpx;
                    }
                }
            }
        }
        public Task<HighRangeImage> RescaleAsync(int width , int height , ScalingMode mode = ScalingMode.Auto)
        {
            return Task.Run(() => Rescale(width , height , mode));
        }
        private void RescaleLinearFromSourceH(HighRangeImage destination , ScalingMode verticalMode)
        {
            var vmode = (ScalingMode)((int)verticalMode&0xFF00);
            double scaleX = (double)destination.width / width;
            for ( var i = 0; i < width; i++ )
            {
                RescaleV(destination , vmode , i , ( int ) ( i * scaleX ));
            }
        }
        private void RescaleLinearFromDestinationH(HighRangeImage destination , ScalingMode verticalMode)
        {
            var vmode = (ScalingMode)((int)verticalMode&0xFF00);
            double scaleX = (double)width / destination.width;
            for ( var i = 0; i < destination.width; i++ )
            {
                RescaleV(destination , vmode , ( int ) ( i * scaleX ) , i);
            }
        }
        private void RescaleV(HighRangeImage destination , ScalingMode verticalMode , int sx , int ex)
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
        private void RescaleLinearFromSourceV(HighRangeImage destination , int sx , int ex)
        {
            double scaleY = (double)destination.height / height;
            for ( var i = 0; i < height; i++ )
            {
                var sy = i;
                var ey = (int)(i*scaleY);
                destination[ex , ey] = this[sx , sy];
            }
        }
        private void RescaleLinearFromDestinationV(HighRangeImage destination , int sx , int ex)
        {
            double scaleY = (double)height / destination.height;
            for ( var i = 0; i < destination.height; i++ )
            {
                var sy = (int)(i*scaleY);
                var ey = i;
                destination[ex , ey] = this[sx , sy];
            }
        }
        public HighRangeImage GetSubBitmap(Rectangle location)
        {
            HighRangeImage bmp = new HighRangeImage(location.Width,location.Height);
            for ( int x1 = 0, x = location.Left; x <= location.Right && x < width; x++, x1++ )
            {
                for ( int y1 = 0, y = location.Top; y <= location.Bottom && y < height; y++, y1++ )
                {
                    bmp[x1 , y1] = this[x , y];
                }
            }
            return bmp;
        }
        public HighRangeImage GetBitmap(int x , int y , int w , int h)
        {
            return GetSubBitmap(new Rectangle(x , y , w , h));
        }
        public HighRangeImage Clone(Rectangle rectangle)
        { return GetSubBitmap(rectangle); }
    }
    public unsafe partial class SuperHighRangeImage
    {
        public SuperHighRangeImage Resize(int width , int height , ScalingMode mode = ScalingMode.Auto)
        {
            return Rescale(width , height , mode);
        }
        public SuperHighRangeImage Rescale(int width , int height , ScalingMode mode = ScalingMode.Auto)
        {
            if ( width <= 0 )
                throw new ArgumentException(nameof(width) + " cannot be less than 1");
            if ( height <= 0 )
                throw new ArgumentException(nameof(height) + " cannot be less than 1");
            SuperHighRangeImage destination = new SuperHighRangeImage(width,height);

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
        private void RescaleAverage(SuperHighRangeImage destination , bool interpolate = false)
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
                        ARGB_Float destinationpx = new ARGB_Float()
                        {
                            R = ( ushort ) ( sr / 3 ),
                            G = ( ushort ) ( sg / 3 ),
                            B = ( ushort ) ( sb / 3 ),
                            A = ( ushort ) ( sa / 3 )
                        };

                        destination[x , y] = destinationpx;
                    }
                    else
                    {
                        if ( sex == sx )
                            sex++;
                        if ( sey == sy )
                            sey++;
                        double count=0;
                        double acount=0;
                        double sa=0,sr=0,sg=0,sb=0;
                        for ( var i = sx; i < sex; i++ )
                        {
                            for ( var j = sy; j < sey; j++ )
                            {
                                count++;
                                ARGB_Float source = this[i,j];
                                acount += source.A;
                                sa += source.A;
                                sr += source.R;
                                sg += source.G;
                                sb += source.B;
                            }
                        }
                        ARGB_Float destinationpx = new ARGB_Float
                        {
                            A = ( float ) ( sa / count )
                        };
                        if ( acount > 0 )
                        {
                            destinationpx.R = ( float ) ( sr / acount );
                            destinationpx.G = ( float ) ( sg / acount );
                            destinationpx.B = ( float ) ( sb / acount );
                        }
                        destination[x , y] = destinationpx;
                    }
                }
            }
        }
        public Task<SuperHighRangeImage> RescaleAsync(int width , int height , ScalingMode mode = ScalingMode.Auto)
        {
            return Task.Run(() => Rescale(width , height , mode));
        }
        private void RescaleLinearFromSourceH(SuperHighRangeImage destination , ScalingMode verticalMode)
        {
            var vmode = (ScalingMode)((int)verticalMode&0xFF00);
            double scaleX = (double)destination.width / width;
            for ( var i = 0; i < width; i++ )
            {
                RescaleV(destination , vmode , i , ( int ) ( i * scaleX ));
            }
        }
        private void RescaleLinearFromDestinationH(SuperHighRangeImage destination , ScalingMode verticalMode)
        {
            var vmode = (ScalingMode)((int)verticalMode&0xFF00);
            double scaleX = (double)width / destination.width;
            for ( var i = 0; i < destination.width; i++ )
            {
                RescaleV(destination , vmode , ( int ) ( i * scaleX ) , i);
            }
        }
        private void RescaleV(SuperHighRangeImage destination , ScalingMode verticalMode , int sx , int ex)
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
        private void RescaleLinearFromSourceV(SuperHighRangeImage destination , int sx , int ex)
        {
            double scaleY = (double)destination.height / height;
            for ( var i = 0; i < height; i++ )
            {
                var sy = i;
                var ey = (int)(i*scaleY);
                destination[ex , ey] = this[sx , sy];
            }
        }
        private void RescaleLinearFromDestinationV(SuperHighRangeImage destination , int sx , int ex)
        {
            double scaleY = (double)height / destination.height;
            for ( var i = 0; i < destination.height; i++ )
            {
                var sy = (int)(i*scaleY);
                var ey = i;
                destination[ex , ey] = this[sx , sy];
            }
        }
        public SuperHighRangeImage GetSubBitmap(Rectangle location)
        {
            SuperHighRangeImage bmp = new SuperHighRangeImage(location.Width,location.Height);
            for ( int x1 = 0, x = location.Left; x <= location.Right && x < width; x++, x1++ )
            {
                for ( int y1 = 0, y = location.Top; y <= location.Bottom && y < height; y++, y1++ )
                {
                    bmp[x1 , y1] = this[x , y];
                }
            }
            return bmp;
        }
        public SuperHighRangeImage GetBitmap(int x , int y , int w , int h)
        {
            return GetSubBitmap(new Rectangle(x , y , w , h));
        }
        public SuperHighRangeImage Clone(Rectangle rectangle)
        { return GetSubBitmap(rectangle); }
    }
    public unsafe partial class OneBandFloatImage
    {
        public OneBandFloatImage Resize(int width , int height , ScalingMode mode = ScalingMode.Auto)
        {
            return Rescale(width , height , mode);
        }
        public OneBandFloatImage Rescale(int width , int height , ScalingMode mode = ScalingMode.Auto)
        {
            if ( width <= 0 )
                throw new ArgumentException(nameof(width) + " cannot be less than 1");
            if ( height <= 0 )
                throw new ArgumentException(nameof(height) + " cannot be less than 1");
            OneBandFloatImage destination = new OneBandFloatImage(width,height);

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
        private void RescaleAverage(OneBandFloatImage destination , bool interpolate = false)
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
                        double sr=ipx;
                        if ( sx > 0 )
                        {
                            var px = this[sx - 1 , sy];
                            sr += px * dx2;
                        }
                        if ( sy > 0 )
                        {
                            var px=this[sx , sy - 1];
                            sr += px * dy2;
                        }
                        if ( sx < width - 1 )
                        {
                            var px = this[sex + 1 , sey];
                            sr += px * dx;
                        }
                        if ( sy < height - 1 )
                        {
                            var px = this[sex  , sey+ 1];
                            sr += px * dy;
                        }
                        float destinationpx = ( float ) ( sr / 3 );

                        destination[x , y] = destinationpx;
                    }
                    else
                    {
                        if ( sex == sx )
                            sex++;
                        if ( sey == sy )
                            sey++;
                        double count=0;
                        double sa=0;
                        for ( var i = sx; i < sex; i++ )
                        {
                            for ( var j = sy; j < sey; j++ )
                            {
                                count++;
                                float source = this[i,j];
                                sa += source;
                            }
                        }
                        float destinationpx = ( float ) ( sa / count );

                        destination[x , y] = destinationpx;
                    }
                }
            }
        }
        public Task<OneBandFloatImage> RescaleAsync(int width , int height , ScalingMode mode = ScalingMode.Auto)
        {
            return Task.Run(() => Rescale(width , height , mode));
        }
        private void RescaleLinearFromSourceH(OneBandFloatImage destination , ScalingMode verticalMode)
        {
            var vmode = (ScalingMode)((int)verticalMode&0xFF00);
            double scaleX = (double)destination.width / width;
            for ( var i = 0; i < width; i++ )
            {
                RescaleV(destination , vmode , i , ( int ) ( i * scaleX ));
            }
        }
        private void RescaleLinearFromDestinationH(OneBandFloatImage destination , ScalingMode verticalMode)
        {
            var vmode = (ScalingMode)((int)verticalMode&0xFF00);
            double scaleX = (double)width / destination.width;
            for ( var i = 0; i < destination.width; i++ )
            {
                RescaleV(destination , vmode , ( int ) ( i * scaleX ) , i);
            }
        }
        private void RescaleV(OneBandFloatImage destination , ScalingMode verticalMode , int sx , int ex)
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
        private void RescaleLinearFromSourceV(OneBandFloatImage destination , int sx , int ex)
        {
            double scaleY = (double)destination.height / height;
            for ( var i = 0; i < height; i++ )
            {
                var sy = i;
                var ey = (int)(i*scaleY);
                destination[ex , ey] = this[sx , sy];
            }
        }
        private void RescaleLinearFromDestinationV(OneBandFloatImage destination , int sx , int ex)
        {
            double scaleY = (double)height / destination.height;
            for ( var i = 0; i < destination.height; i++ )
            {
                var sy = (int)(i*scaleY);
                var ey = i;
                destination[ex , ey] = this[sx , sy];
            }
        }
        public OneBandFloatImage GetSubBitmap(Rectangle location)
        {
            OneBandFloatImage bmp = new OneBandFloatImage(location.Width,location.Height);
            for ( int x1 = 0, x = location.Left; x <= location.Right && x < width; x++, x1++ )
            {
                for ( int y1 = 0, y = location.Top; y <= location.Bottom && y < height; y++, y1++ )
                {
                    bmp[x1 , y1] = this[x , y];
                }
            }
            return bmp;
        }
        public OneBandFloatImage GetBitmap(int x , int y , int w , int h)
        {
            return GetSubBitmap(new Rectangle(x , y , w , h));
        }
        public OneBandFloatImage Clone(Rectangle rectangle)
        { return GetSubBitmap(rectangle); }
    }
#pragma warning restore CS1591
}
