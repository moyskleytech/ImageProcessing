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
    public unsafe class HSBImage : Image<HSB>
    {
        private readonly HSB* pxls;
        private const uint VERSION=1;
        /// <summary>
        /// Allocate it using width and height
        /// </summary>
        /// <param name="w">Width</param>
        /// <param name="h">Height</param>
        public HSBImage(int w , int h) : base(w , h)
        {
            pxls = ( HSB* ) data;
            width = w;
            height = h;
        }

        /// <summary>
        /// Destrop bitmap
        /// </summary>
        ~HSBImage()
        {
            Dispose();
        }

        /// <summary>
        /// Source to edit or copy
        /// </summary>
        public HSB* Source { get { return pxls; } }
       
        /// <summary>
        /// Get pixel from coordinate
        /// </summary>
        /// <param name="pos">As 1 dim array</param>
        /// <returns>Pixel</returns>
        public override HSB this[int pos]
        {
            get
            {
                if ( pos > 0 && pos < width * height )
                    return pxls[pos];
                else
                    return new HSB();
            }
            set
            {
                if ( pos > 0 && pos < width * height )
                    pxls[pos] = value;
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
        public void Save(Stream s , HSBSaveFormat format = HSBSaveFormat.HSB888)
        {
            s.WriteByte(( byte ) 'H');
            s.WriteByte(( byte ) 'S');
            s.WriteByte(( byte ) 'B');
            s.WriteByte(( byte ) format);
            s.Write(BitConverter.GetBytes(VERSION) , 0 , 4);
            s.Write(BitConverter.GetBytes(width) , 0 , 4);
            s.Write(BitConverter.GetBytes(height) , 0 , 4);
            HSB* ptr = pxls;
            unchecked
            {
                for ( var x = 0; x < width; x++ )
                {
                    for ( var y = 0; y < height; y++ )
                    {
                        HSB* pixel = ptr++;
                        s.WriteByte(pixel->H);
                        switch ( format )
                        {
                            case HSBSaveFormat.HSB844:
                                s.WriteByte(( byte ) ( ( pixel->S & 0xF0 ) | ( pixel->B >> 4 ) ));
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
            read = ( byte ) s.ReadByte();
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
            HSB* ptr = img.pxls;
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
                                pixel->S = ( byte ) ( ( read & 0xF0 ) );
                                pixel->B = ( byte ) ( ( read & 0x0F ) << 4 );
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
    /// 
    /// </summary>
    public enum HSBSaveFormat : byte
    {
#pragma warning disable CS1591
        HSB844 = 0,
        HSB888 = 1
    }
}
