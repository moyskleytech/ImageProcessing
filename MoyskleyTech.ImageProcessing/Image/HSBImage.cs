using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace MoyskleyTech.ImageProcessing.Image
{
    /// <summary>
    /// Represent a Bitmap (Array of Pixel)
    /// </summary>
    [NotSerialized]
    public unsafe class HSBImage
    {
        private int width,height;
        private readonly HSB* data;
        private IntPtr raw;
        private const uint VERSION=1;
        /// <summary>
        /// Allocate it using width and height
        /// </summary>
        /// <param name="w">Width</param>
        /// <param name="h">Height</param>
        public HSBImage(int w , int h)
        {
            //Allocate
            raw = Marshal.AllocHGlobal(w * h * sizeof(HSB));
            data = ( HSB* ) raw.ToPointer();
            width = w;
            height = h;
        }
        /// <summary>
        /// Width of bitmap
        /// </summary>
        public int Width { get { return width; } }
        /// <summary>
        /// Height of bitmap
        /// </summary>
        public int Height { get { return height; } }

        /// <summary>
        /// Destrop bitmap
        /// </summary>
        ~HSBImage()
        {
            Dispose();
        }
        /// <summary>
        /// Get pixel from coordinate
        /// </summary>
        /// <param name="x">X position</param>
        /// <param name="y">Y position</param>
        /// <returns>Pixel</returns>
        public HSB Get(int x , int y)
        {
            return this[x , y];
        }
        /// <summary>
        /// Dispose the bitmap and release memory
        /// </summary>
        public void Dispose()
        {
            Marshal.FreeHGlobal(raw);
            GC.SuppressFinalize(this);
        }
        /// <summary>
        /// Source to edit or copy
        /// </summary>
        public HSB* Source { get { return data; } }
        /// <summary>
        /// Get pixel from coordinate
        /// </summary>
        /// <param name="x">X position</param>
        /// <param name="y">Y position</param>
        /// <returns>Pixel</returns>
        public HSB this[int x , int y]
        {
            get
            {
                return this[y * width + x];
            }
            set
            {
                this[y * width + x] = value;
            }
        }
        /// <summary>
        /// Get pixel from coordinate
        /// </summary>
        /// <param name="pos">As 1 dim array</param>
        /// <returns>Pixel</returns>
        public HSB this[int pos]
        {
            get
            {
                if ( pos > 0 && pos < width * height )
                    return data[pos];
                else
                    return new HSB();
            }
            set
            {
                if ( pos > 0 && pos < width * height )
                    data[pos] = value;
            }
        }
        /// <summary>
        /// Convert to Bitmap
        /// </summary>
        /// <returns></returns>
        public Bitmap ToRGB()
        {
            Bitmap img = new Bitmap(width,height);
            Pixel* dest=img.Source;
            HSB* src = Source;
            int max = width*height;
            for ( var i = 0; i < max; i++ )
            {
                *dest++ = ( src++ )->ToRGB();
            }
            return img;
        }
        /// <summary>
        /// Allow saving to disk
        /// </summary>
        /// <param name="s"></param>
        /// <param name="format"></param>
        public void Save(Stream s,HSBSaveFormat format = HSBSaveFormat.HSB888)
        {
            s.WriteByte(( byte ) 'H');
            s.WriteByte(( byte ) 'S');
            s.WriteByte(( byte ) 'B');
            s.WriteByte((byte)format);
            s.Write(BitConverter.GetBytes(VERSION) , 0 , 4);
            s.Write(BitConverter.GetBytes(width) , 0 , 4);
            s.Write(BitConverter.GetBytes(height) , 0 , 4);
            HSB* ptr = data;
            unchecked {
                for ( var x = 0; x < width; x++ )
                {
                    for ( var y = 0; y < height; y++ )
                    {
                        HSB* pixel = ptr++;
                        s.WriteByte(pixel->H);
                        switch ( format )
                        {
                            case HSBSaveFormat.HSB844:
                                s.WriteByte(( byte ) ( ( pixel->S &0xF0 ) | ( pixel->B >> 4 ) ));
                                break;
                            case HSBSaveFormat.HSB888:
                                s.WriteByte(pixel->S);
                                s.WriteByte(pixel->B);
                                break;
                        }
                    }
                }
            }
        }
        /// <summary>
        /// Load image from file
        /// </summary>
        /// <param name="s"></param>
        /// <returns></returns>
        public static HSBImage Load(Stream s)
        {
            byte read=0;
            read=(byte)s.ReadByte();
            if ( read != 'H' )
                return null;
            read = ( byte ) s.ReadByte();
            if ( read != 'S' )
                return null;
            read = ( byte ) s.ReadByte();
            if ( read != 'B' )
                return null;
            read = ( byte ) s.ReadByte();
            var format = (HSBSaveFormat)read;
            byte[] array = new byte[4];
            s.Read(array , 0 , 4);
            uint version = BitConverter.ToUInt32(array,0);

            if ( version > VERSION )
                return null;
            s.Read(array , 0 , 4);
            int width = BitConverter.ToInt32(array,0);
            s.Read(array , 0 , 4);
            int height = BitConverter.ToInt32(array,0);
            HSBImage img = new HSBImage(width,height);
            HSB* ptr = img.data;
            unchecked
            {
                for ( var x = 0; x < width; x++ )
                {
                    for ( var y = 0; y < height; y++ )
                    {
                        HSB* pixel = ptr++;
                        read = ( byte ) s.ReadByte();
                        pixel->H = read;
                        switch ( format )
                        {
                            case HSBSaveFormat.HSB844:
                                read = ( byte ) s.ReadByte();
                                pixel->S = (byte)((read&0xF0));
                                pixel->B = ( byte ) ( ( read & 0x0F )<<4 );
                                break;
                            case HSBSaveFormat.HSB888:
                                read = ( byte ) s.ReadByte();
                                pixel->S = read;
                                read = ( byte ) s.ReadByte();
                                pixel->B = read;
                                break;
                        }
                    }
                }
            }
            return img;
        }
    }
    /// <summary>
    /// Struct to store HSV
    /// </summary>
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
    /// <summary>
    /// 
    /// </summary>
    public enum HSBSaveFormat:byte
    {
#pragma warning disable CS1591
        HSB844 = 0,
        HSB888=1
    }
}
