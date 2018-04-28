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
        public static explicit operator Image<HSB>(HSBImage img)
        {
            Image<HSB> image = new Image<HSB>(img.Width,img.Height);
            
            img.CopyTo(image.Source);

            return image;
        }
        public static explicit operator HSBImage(Image<HSB> img)
        {
            HSBImage image = new HSBImage(img.Width,img.Height);

            image.CopyFrom(img.Source);

            return image;
        }

        private void CopyTo(IntPtr dst)
        {
            HSB* src = data;
            HSB* dest = (HSB*)dst.ToPointer();
            for ( var i = 0; i < width * height; i++ )
                *dest++ = *src++;
        }
        private void CopyFrom(IntPtr dst)
        {
            HSB* src = (HSB*)dst.ToPointer();
            HSB* dest = data;
            for ( var i = 0; i < width * height; i++ )
                *dest++ = *src++;
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
