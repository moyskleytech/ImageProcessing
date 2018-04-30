using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.InteropServices;
namespace MoyskleyTech.ImageProcessing.Image
{
    /// <summary>
    /// Represent a pixel
    /// </summary>
    [StructLayout(LayoutKind.Sequential)]
    public struct Pixel
    {
        /// <summary>
        /// Alpha
        /// </summary>
        public byte A;
        /// <summary>
        /// Red
        /// </summary>
        public byte R;
        /// <summary>
        /// Green
        /// </summary>
        public byte G;
        /// <summary>
        /// Blue
        /// </summary>
        public byte B;
        /// <summary>
        /// ARGB int representation
        /// </summary>
        /// <returns></returns>
        public int ToArgb()
        {
            return unchecked(A << 24 | R << 16 | G << 8 | B);
            //return unchecked(B << 24 | G << 16 | R << 8 | A);
        }
        /// <summary>
        /// Get from int representation
        /// </summary>
        /// <param name="argb">int Representation</param>
        /// <returns></returns>
        public static Pixel FromArgb(int argb)
        {
            return new Pixel()
            {
                A = ( byte ) unchecked(argb >> 24 & 0xFF) ,
                R = ( byte ) unchecked(argb >> 16 & 0xFF) ,
                G = ( byte ) unchecked(argb >> 8 & 0xFF) ,
                B = ( byte ) unchecked(argb >> 0 & 0xFF)
            };
        }
        /// <summary>
        /// Get from Pixel and alpha
        /// </summary>
        /// <param name="p">Color</param>
        /// <param name="a">Alpha</param>
        /// <returns></returns>
        public static Pixel FromArgb(Pixel p , byte a)
        {
            return new Pixel()
            {
                A = a ,
                R = p.R ,
                G = p.G ,
                B = p.B
            };
        }
        /// <summary>
        /// From uint Representation
        /// </summary>
        /// <param name="argb">uint representation</param>
        /// <returns></returns>
        public static Pixel FromArgb(uint argb)
        {
            return new Pixel()
            {
                A = ( byte ) unchecked(argb >> 24 & 0xFF) ,
                R = ( byte ) unchecked(argb >> 16 & 0xFF) ,
                G = ( byte ) unchecked(argb >> 8 & 0xFF) ,
                B = ( byte ) unchecked(argb >> 0 & 0xFF)
            };
        }
        /// <summary>
        /// Create from components
        /// </summary>
        /// <param name="a">Alpha</param>
        /// <param name="r">Red</param>
        /// <param name="g">Green</param>
        /// <param name="b">Blue</param>
        /// <returns></returns>
        public static Pixel FromArgb(byte a , byte r , byte g , byte b)
        {
            return new Image.Pixel() { A = a , R = r , G = g , B = b };
        }
        public static Pixel FromName(string name)
        {
            return Pixels.GetPixel(name);
        }

        public static Pixel FromRGB565(UInt16 rgb565)
        {
            var r = rgb565 >> 11;
            var g = (rgb565 >> 5) & 0b111111;
            var b = rgb565 & 0b11111;

            r = r * 256 / 0b11111;
            g = g * 256 / 0b111111;
            b = b * 256 / 0b11111;
            return FromArgb(255 , ( byte ) r , ( byte ) g , ( byte ) b);
        }
        public static Pixel FromRGB555(UInt16 rgb555)
        {
            var r = (rgb555 >> 10) &0b11111;
            var g = (rgb555 >> 5) & 0b11111;
            var b = rgb555 & 0b11111;

            r = r * 256 / 0b11111;
            g = g * 256 / 0b11111;
            b = b * 256 / 0b11111;
            return FromArgb(255 , ( byte ) r , ( byte ) g , ( byte ) b);
        }

        public ushort ToRGB565()
        {
            int r = R;
            int g = G;
            int b = B;
            r = r * 0b11111 / 256;
            g = g * 0b111111 / 256;
            b = b * 0b11111 / 256;

            return ( ushort ) ( r << 11 | g << 5 | b );
        }
        public ushort ToRGB555()
        {
            int r = R;
            int g = G;
            int b = B;
            r = r * 0b11111 / 256;
            g = g * 0b11111 / 256;
            b = b * 0b11111 / 256;

            return ( ushort ) ( r << 10 | g << 5 | b );
        }
        /// <summary>
        /// Merge pixels
        /// </summary>
        /// <param name="fromImage"></param>
        /// <returns></returns>
        public Pixel Over(Pixel fromImage)
        {
            byte[] result = new byte[4];
            result[0] = ( byte ) System.Math.Max(A , fromImage.A);
            result[1] = ( byte ) ( ( A * R + fromImage.R * ( 255 - A ) ) / 255 );
            result[2] = ( byte ) ( ( A * G + fromImage.G * ( 255 - A ) ) / 255 );
            result[3] = ( byte ) ( ( A * B + fromImage.B * ( 255 - A ) ) / 255 );
            return FromArgb(result[0] , result[1] , result[2] , result[3]);
        }
        /// <summary>
        /// Get the gray tone of the pixel
        /// </summary>
        /// <returns></returns>
        public byte GetGrayTone()
        {
            if ( R == G && G == B )
                return R;
            return ( byte ) ( 0.21 * R + 0.72 * G + 0.07 * B );
        }
        /// <summary>Gets the hue-saturation-brightness (HSB) brightness value for this <see cref="T:System.Drawing.Color" /> structure.</summary>
        /// <returns>The brightness of this <see cref="T:System.Drawing.Color" />. The brightness ranges from 0.0 through 1.0, where 0.0 represents black and 1.0 represents white.</returns>
        /// <filterpriority>1</filterpriority>
        /// <PermissionSet>
        ///   <IPermission class="System.Security.Permissions.FileIOPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true" />
        /// </PermissionSet>
        public float GetBrightness()
        {
            float r = (float)this.R / 255f;
            float g = (float)this.G / 255f;
            float b = (float)this.B / 255f;
            float single = r;
            float single1 = r;
            if ( g > single )
            {
                single = g;
            }
            if ( b > single )
            {
                single = b;
            }
            if ( g < single1 )
            {
                single1 = g;
            }
            if ( b < single1 )
            {
                single1 = b;
            }
            return ( single + single1 ) / 2f;
        }

        /// <summary>Gets the hue-saturation-brightness (HSB) hue value, in degrees, for this <see cref="T:System.Drawing.Color" /> structure.</summary>
        /// <returns>The hue, in degrees, of this <see cref="T:System.Drawing.Color" />. The hue is measured in degrees, ranging from 0.0 through 360.0, in HSB color space.</returns>
        /// <filterpriority>1</filterpriority>
        /// <PermissionSet>
        ///   <IPermission class="System.Security.Permissions.FileIOPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true" />
        /// </PermissionSet>
        public float GetHue()
        {
            if ( this.R == this.G && this.G == this.B )
            {
                return 0f;
            }
            float r = (float)this.R / 255f;
            float g = (float)this.G / 255f;
            float b = (float)this.B / 255f;
            float single = 0f;
            float single1 = r;
            float single2 = r;
            if ( g > single1 )
            {
                single1 = g;
            }
            if ( b > single1 )
            {
                single1 = b;
            }
            if ( g < single2 )
            {
                single2 = g;
            }
            if ( b < single2 )
            {
                single2 = b;
            }
            float single3 = single1 - single2;
            if ( r == single1 )
            {
                single = ( g - b ) / single3;
            }
            else if ( g == single1 )
            {
                single = 2f + ( b - r ) / single3;
            }
            else if ( b == single1 )
            {
                single = 4f + ( r - g ) / single3;
            }
            single = single * 60f;
            if ( single < 0f )
            {
                single = single + 360f;
            }
            return single;
        }

        /// <summary>Gets the hue-saturation-brightness (HSB) saturation value for this <see cref="T:System.Drawing.Color" /> structure.</summary>
        /// <returns>The saturation of this <see cref="T:System.Drawing.Color" />. The saturation ranges from 0.0 through 1.0, where 0.0 is grayscale and 1.0 is the most saturated.</returns>
        /// <filterpriority>1</filterpriority>
        /// <PermissionSet>
        ///   <IPermission class="System.Security.Permissions.FileIOPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true" />
        /// </PermissionSet>
        public float GetSaturation()
        {
            float r = (float)this.R / 255f;
            float g = (float)this.G / 255f;
            float b = (float)this.B / 255f;
            float single = 0f;
            float single1 = r;
            float single2 = r;
            if ( g > single1 )
            {
                single1 = g;
            }
            if ( b > single1 )
            {
                single1 = b;
            }
            if ( g < single2 )
            {
                single2 = g;
            }
            if ( b < single2 )
            {
                single2 = b;
            }
            if ( single1 != single2 )
            {
                single = ( ( double ) ( ( single1 + single2 ) / 2f ) > 0.5 ? ( single1 - single2 ) / ( 2f - single1 - single2 ) : ( single1 - single2 ) / ( single1 + single2 ) );
            }
            return single;
        }
        public HSB ToHSB()
        {
            return HSB.FromHSB(GetHue() , GetSaturation() , GetBrightness());
        }
        public static explicit operator Brush(Pixel p)
        {
            return new SolidBrush(p);
        }
        public static explicit operator Brush<Pixel>(Pixel p)
        {
            return new SolidBrush(p);
        }
        public override string ToString()
        {
            return "Pixel[" + R + "," + G + "," + B + "]";
        }


        public static bool operator ==(Pixel a , Pixel b)
        {
            return a.ToArgb() == b.ToArgb();
        }
        public static bool operator !=(Pixel a , Pixel b)
        {
            return a.ToArgb() != b.ToArgb();
        }
        public override bool Equals(object obj)
        {
            if ( obj is Pixel p )
                return p == this;
            return base.Equals(obj);
        }
        public override int GetHashCode()
        {
            return ToArgb().GetHashCode();
        }
    }
    /// <summary>
    /// Struct to store HSV
    /// </summary>
    [StructLayout(LayoutKind.Sequential)]
    public struct HSB
    {
        /// <summary>
        /// Hue
        /// </summary>
        public byte H;
        /// <summary>
        /// Saturation
        /// </summary>
        public byte S;
        /// <summary>
        /// Value
        /// </summary>
        public byte B;

        /// <summary>
        /// Convert HSB to RGB
        /// </summary>
        /// <returns></returns>
        //public Pixel ToRGB()
        //{
        //    float saturation = S/255f;
        //    float brightness = B/255f;
        //    float hue = S/255f;
        //    int r = 0, g = 0, b = 0;
        //    if ( saturation == 0 )
        //    {
        //        r = g = b = ( int ) ( brightness * 255.0f + 0.5f );
        //    }
        //    else
        //    {
        //        float h = (hue - (float)System.Math.Floor(hue)) * 6.0f;
        //        float f = h - (float)System.Math.Floor(h);
        //        float p = brightness * (1.0f - saturation);
        //        float q = brightness * (1.0f - saturation * f);
        //        float t = brightness * (1.0f - (saturation * (1.0f - f)));
        //        switch ( ( int ) h )
        //        {
        //            case 0:
        //                r = ( int ) ( brightness * 255.0f + 0.5f );
        //                g = ( int ) ( t * 255.0f + 0.5f );
        //                b = ( int ) ( p * 255.0f + 0.5f );
        //                break;
        //            case 1:
        //                r = ( int ) ( q * 255.0f + 0.5f );
        //                g = ( int ) ( brightness * 255.0f + 0.5f );
        //                b = ( int ) ( p * 255.0f + 0.5f );
        //                break;
        //            case 2:
        //                r = ( int ) ( p * 255.0f + 0.5f );
        //                g = ( int ) ( brightness * 255.0f + 0.5f );
        //                b = ( int ) ( t * 255.0f + 0.5f );
        //                break;
        //            case 3:
        //                r = ( int ) ( p * 255.0f + 0.5f );
        //                g = ( int ) ( q * 255.0f + 0.5f );
        //                b = ( int ) ( brightness * 255.0f + 0.5f );
        //                break;
        //            case 4:
        //                r = ( int ) ( t * 255.0f + 0.5f );
        //                g = ( int ) ( p * 255.0f + 0.5f );
        //                b = ( int ) ( brightness * 255.0f + 0.5f );
        //                break;
        //            case 5:
        //                r = ( int ) ( brightness * 255.0f + 0.5f );
        //                g = ( int ) ( p * 255.0f + 0.5f );
        //                b = ( int ) ( q * 255.0f + 0.5f );
        //                break;
        //        }
        //    }
        //    return Pixel.FromArgb(255 , ( byte ) r , ( byte ) g , ( byte ) b);
        //}
        public Pixel ToRGB()
        {
            double hue=H/255d , saturation=S/255d , brightness=B/255d;

            byte r = 0, g = 0, b = 0;
            if ( saturation == 0 )
            {
                r = g = b = ( byte ) ( brightness * 255.0f + 0.5f );
            }
            else
            {
                var q = brightness < 0.5 ? brightness * (1 + saturation) : brightness + saturation - brightness * saturation;
                var p = 2 * brightness - q;
                r = ( byte ) ( 255 * HueToRgb(p , q , hue + ( 1d / 3 )) );
                g = ( byte ) ( 255 * HueToRgb(p , q , hue) );
                b = ( byte ) ( 255 * HueToRgb(p , q , hue - ( 1d / 3 )) );
            }
            return Pixel.FromArgb(255 , r , g , b);
        }
        private static double HueToRgb(double p , double q , double t)
        {
            if ( t < 0 )
                t += 1;
            if ( t > 1 )
                t -= 1;
            if ( t < ( 1d / 6 ) )
                return ( p + ( q - p ) * 6 * t );
            if ( t < ( 1d / 2 ) )
                return ( q );
            if ( t < ( 2d / 3 ) )
                return ( p + ( q - p ) * ( 2d / 3 - t ) * 6 );
            return ( p );
        }
        public override string ToString()
        {
            return "HSB[" + ( H * 360 / 255 ).ToString("0") + "," + ( S / 2.55 ).ToString("0") + "%," + ( B / 2.55 ).ToString("0") + "%]";
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="v1">0-360</param>
        /// <param name="v2">0-1</param>
        /// <param name="v3">0-1</param>
        /// <returns></returns>
        public static HSB FromHSB(float v1 , float v2 , float v3)
        {
            HSB hsb = new HSB();
            var h = v1*255/360;
            var s = v2*255;
            var b = v3*255;
            hsb.H = ( byte ) h;
            hsb.S = ( byte ) s;
            hsb.B = ( byte ) b;
            return hsb;
        }
        public static HSB FromHSB(int v1 , int v2 , int v3)
        {
            HSB hsb = new HSB();
            var h = v1;
            var s = v2;
            var b = v3;
            hsb.H = ( byte ) h;
            hsb.S = ( byte ) s;
            hsb.B = ( byte ) b;
            return hsb;
        }

    }
    [StructLayout(LayoutKind.Sequential)]
    public struct HSBA
    {
        /// <summary>
        /// Hue
        /// </summary>
        public byte H;
        /// <summary>
        /// Saturation
        /// </summary>
        public byte S;
        /// <summary>
        /// Value
        /// </summary>
        public byte B;
        public byte A;

        public Pixel ToRGB()
        {
            double hue=H/255d , saturation=S/255d , brightness=B/255d;

            byte r = 0, g = 0, b = 0;
            if ( saturation == 0 )
            {
                r = g = b = ( byte ) ( brightness * 255.0f + 0.5f );
            }
            else
            {
                var q = brightness < 0.5 ? brightness * (1 + saturation) : brightness + saturation - brightness * saturation;
                var p = 2 * brightness - q;
                r = ( byte ) ( 255 * HueToRgb(p , q , hue + ( 1d / 3 )) );
                g = ( byte ) ( 255 * HueToRgb(p , q , hue) );
                b = ( byte ) ( 255 * HueToRgb(p , q , hue - ( 1d / 3 )) );
            }
            return Pixel.FromArgb(A , r , g , b);
        }
        private static double HueToRgb(double p , double q , double t)
        {
            if ( t < 0 )
                t += 1;
            if ( t > 1 )
                t -= 1;
            if ( t < ( 1d / 6 ) )
                return ( p + ( q - p ) * 6 * t );
            if ( t < ( 1d / 2 ) )
                return ( q );
            if ( t < ( 2d / 3 ) )
                return ( p + ( q - p ) * ( 2d / 3 - t ) * 6 );
            return ( p );
        }
        public override string ToString()
        {
            return "HSBA[" + ( H * 360 / 255 ).ToString("0") + "," + ( S / 2.55 ).ToString("0") + "%," + ( B / 2.55 ).ToString("0") + "%," + A + "/255]";
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="v1">0-360</param>
        /// <param name="v2">0-1</param>
        /// <param name="v3">0-1</param>
        /// <returns></returns>
        public static HSBA FromHSBA(float v1 , float v2 , float v3 , byte a)
        {
            HSBA hsb = new HSBA();
            var h = v1*255/360;
            var s = v2*255;
            var b = v3*255;
            hsb.H = ( byte ) h;
            hsb.S = ( byte ) s;
            hsb.B = ( byte ) b;
            hsb.A = a;
            return hsb;
        }
        public static HSBA FromHSBA(int v1 , int v2 , int v3 , byte a)
        {
            HSBA hsb = new HSBA();
            var h = v1;
            var s = v2;
            var b = v3;
            hsb.H = ( byte ) h;
            hsb.S = ( byte ) s;
            hsb.B = ( byte ) b;
            hsb.A = a;
            return hsb;
        }

    }
    [StructLayout(LayoutKind.Sequential)]
    public struct CYMK
    {
        public float C,Y,M,K;
    }
    [StructLayout(LayoutKind.Sequential)]
    public struct HSL
    {
        public float H,S,L;
    }
    [StructLayout(LayoutKind.Sequential)]
    public struct HSB_Float
    {
        public float H,S,B;
    }
    [StructLayout(LayoutKind.Sequential)]
    public struct _565
    {
        public ushort _565_;
        public byte R { get { return ( byte ) ( _565_ >> 11 ); } set { _565_ &= 0b0000011111111111; _565_ |= unchecked(( ushort ) ( value << 11 )); } }
        public byte G { get { return ( byte ) ( ( _565_ >> 5 ) & 0b111111 ); } set { _565_ &= 0b1111100000011111; _565_ |= unchecked(( ushort ) ( ( value & 0b111111 ) << 6 )); } }
        public byte B { get { return ( byte ) ( _565_ & 0b11111 ); } set { _565_ &= 0b1111111111100000; _565_ |= unchecked(( ushort ) ( value & 0b11111 )); } }
        public static implicit operator ushort(_565 v) => v._565_;
        public static implicit operator _565(ushort v) => new _565 { _565_ = v };
    }
    [StructLayout(LayoutKind.Sequential)]
    public struct _555
    {
        public ushort _555_;
        public byte R { get { return ( byte ) ( ( _555_ & 0b0111110000000000 ) >> 10 ); } set { _555_ &= 0b0000001111111111; _555_ |= unchecked(( ushort ) ( ( value & 0b11111 ) << 10 )); } }
        public byte G { get { return ( byte ) ( ( _555_ & 0b0000001111100000 ) >> 5 ); } set { _555_ &= 0b1111110000011111; _555_ |= unchecked(( ushort ) ( ( value & 0b11111 ) << 5 )); } }
        public byte B { get { return ( byte ) ( _555_ & 0b11111 ); } set { _555_ &= 0b1111111111100000; _555_ |= unchecked(( ushort ) ( value & 0b11111 )); } }
        public static implicit operator ushort(_555 v) => v._555_;
        public static implicit operator _555(ushort v) => new _555 { _555_ = v };
        public static implicit operator _555(_1555 v) => new _555 { _555_ = v };
    }
    [StructLayout(LayoutKind.Sequential)]
    public struct _1555
    {
        public ushort _1555_;
        public byte R { get { return ( byte ) ( ( _1555_ & 0b0111110000000000 ) >> 10 ); } set { _1555_ &= 0b0000001111111111; _1555_ |= unchecked(( ushort ) ( ( value & 0b11111 ) << 10 )); } }
        public byte G { get { return ( byte ) ( ( _1555_ & 0b0000001111100000 ) >> 5 ); } set { _1555_ &= 0b1111110000011111; _1555_ |= unchecked(( ushort ) ( ( value & 0b11111 ) << 5 )); } }
        public byte B { get { return ( byte ) ( _1555_ & 0b11111 ); } set { _1555_ &= 0b1111111111100000; _1555_ |= unchecked(( ushort ) ( value & 0b11111 )); } }
        public bool A { get => _1555_ >= 1 << 15; set { _1555_ &= 0b0111111111111111; _1555_ |= unchecked(( ushort ) ( value ? 1 : 0 )); } }
        public static implicit operator ushort(_1555 v) => v._1555_;
        public static implicit operator _1555(ushort v) => new _1555 { _1555_ = v };
        public static implicit operator _1555(_555 v) => new _1555 { _1555_ = v._555_ , A = true };
    }
    [StructLayout(LayoutKind.Sequential)]
    public struct BGR
    {
        public byte B,G,R;
    }
    [StructLayout(LayoutKind.Sequential)]
    public struct RGB
    {
        public byte R,G,B;
    }
    [StructLayout(LayoutKind.Sequential)]
    public struct _332
    {
        public byte _332_;
        public byte R
        {
            get
            {
                return ( byte ) ( ( _332_ & 0b11100000 ) >> 5 );
            }
            set
            {
                _332_ &= 0b00011111;
                _332_ |= unchecked(( byte ) ( ( value & 0b111 ) << 5 ));
            }
        }
        public byte G
        {
            get
            {
                return ( byte ) ( ( _332_ & 0b00011100 ) >> 2 );
            }
            set
            {
                _332_ &= 0b11100011;
                _332_ |= unchecked(( byte ) ( ( value & 0b111 ) << 2 ));
            }
        }
        public byte B
        {
            get
            {
                return ( byte ) ( _332_ & 0b11 );
            }
            set
            {
                _332_ &= 0b11111100;
                _332_ |= unchecked(( byte ) ( value & 0b11 ));
            }
        }
        public static implicit operator byte(_332 v) => v._332_;
        public static implicit operator _332(byte v) => new _332 { _332_ = v };
    }
    [StructLayout(LayoutKind.Sequential)]
    public struct ARGB_Float
    {
        public float A,R,G,B;
    }
    [StructLayout(LayoutKind.Sequential)]
    public struct ARGB_16bit
    {
        public ushort A,R,G,B;
    }
}
