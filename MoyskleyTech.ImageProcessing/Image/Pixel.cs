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
        public static Pixel FromArgb(Pixel p, byte a)
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
        /// <summary>
        /// Merge pixels
        /// </summary>
        /// <param name="fromImage"></param>
        /// <returns></returns>
        public Pixel Over(Pixel fromImage)
        {
            byte[] result = new byte[4];
            result[0] = ( byte ) System.Math.Max(A , fromImage.A) ;
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
            if ( R == G && G== B )
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
            return HSB.FromHSB(GetHue(), GetSaturation() , GetBrightness());
        }
        public static explicit operator Brush(Pixel p)
        {
            return new SolidBrush(p);
        }
        public override string ToString()
        {
            return "Pixel["+R+","+G+","+B+"]";
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
}
