using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using MoyskleyTech.ImageProcessing.Image;

namespace MoyskleyTech.ImageProcessing
{
    /// <summary>
    /// Represent a Bitmap (Array of Pixel)
    /// </summary>
    [NotSerialized]
    public static unsafe class HSBImage 
    {
        private const uint VERSION=1;
       
        /// <summary>
        /// Convert to Bitmap
        /// </summary>
        /// <returns></returns>
        public static Image<Pixel> ToRGB(this Image<HSB> image)
        {
            return image.ConvertTo<Pixel>();
        }
        /// <summary>
        /// Allow saving to disk
        /// </summary>
        /// <param name="s"></param>
        /// <param name="format"></param>
        public static void Save(this Image<HSB> image,Stream s , HSBSaveFormat format = HSBSaveFormat.HSB888)
        {
            s.WriteByte(( byte ) 'H');
            s.WriteByte(( byte ) 'S');
            s.WriteByte(( byte ) 'B');
            s.WriteByte(( byte ) format);
            s.Write(BitConverter.GetBytes(VERSION) , 0 , 4);
            s.Write(BitConverter.GetBytes(image.Width) , 0 , 4);
            s.Write(BitConverter.GetBytes(image.Height) , 0 , 4);
            HSB* ptr = image.Source;
            unchecked
            {
                for ( var x = 0; x < image.Width; x++ )
                {
                    for ( var y = 0; y < image.Height; y++ )
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
        public static Image<HSB> Load(Stream s)
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
            Image<HSB> img = Image<HSB>.Create(width,height);
            HSB* ptr = img.Source;
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
