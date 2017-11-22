using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace MoyskleyTech.ImageProcessing.Image
{
    public class BitmapCodec : IBitmapCodec
    {
        public int SignatureLength
        {
            get
            {
                return 2;
            }
        }

        public IBitmapDecoder CheckSignature(string signature , Stream f)
        {
            if ( signature.Substring(0 , SignatureLength) == "BM" )
            {
                byte[] buffer = new byte[signature.Length];
                for ( var i = 0; i < signature.Length; i++ )
                    buffer[i] = ( byte ) signature[i];
                BitmapDecoder decoder = new BitmapDecoder();
                decoder.SetStream(new BufferedStream(f , buffer));
                return decoder;
            }
            return null;
        }
        public Bitmap DecodeStream(Stream s)
        {
            BitmapDecoder decoder = new BitmapDecoder();
            decoder.SetStream(new BufferedStream(s , new byte[0]));
            decoder.ReadHeader();
            return decoder.ReadBitmap();
        }
        public void Save(Bitmap bmp , Stream s)
        {
            bmp.Save(s);
        }
    }
    internal class BitmapDecoder : IBitmapDecoder
    {
        [StructLayout(LayoutKind.Sequential , Size = 54)]
        internal unsafe struct BITMAP_HEADER
        {
            public fixed byte signature[2];
            public int size;
            public int unused;
            public int bitmapOffset;
            public int sizeOfDIB;
            public int width;
            public int height;
            public ushort numberOfColorPlane;
            public ushort numberOfBitPerPixel;
            public int compression;
            public int sizeOfBitmapDataInclusingPadding;
            public int printResolutionH;
            public int printResolutionV;
            public int numberOfColorInPalette;
            public int numberOfImportantColor;
        }
        BufferedStream s;
        BITMAP_HEADER header;
        BitmapPalette8bpp palette8bpp;
        BitmapPalette4bpp palette4bpp;
        BitmapPalette2bpp palette2bpp;
        BitmapPalette1bpp palette1bpp;
        int index;
        bool error=false;
        private byte Read()
        {
            index++;
            return s.Read();
        }
        public unsafe Bitmap ReadBitmap()
        {
            Bitmap bmp = new Bitmap(header.width,System.Math.Abs(header.height));
            if ( header.numberOfBitPerPixel == 8 )
                palette8bpp = ReadPalette();
            if ( header.numberOfBitPerPixel == 4 )
                palette4bpp = Read4BppPalette();
            if ( header.numberOfBitPerPixel == 2 )
                palette2bpp = Read2BppPalette();
            if ( header.numberOfBitPerPixel == 1 )
                palette1bpp = Read1BppPalette();
            palette8bpp?.CheckIfAlphaZero();
            palette4bpp?.CheckIfAlphaZero();
            palette2bpp?.CheckIfAlphaZero();
            palette1bpp?.CheckIfAlphaZero();
            while ( index < header.bitmapOffset )
                Read();

            if ( header.height > 0 )
                for ( var r = header.height - 1; r >= 0; r-- )
                {
                    var row = ReadPixels(header.width);
                    for ( var c = 0; c < header.width; c++ )
                        bmp[c , r] = row[c];
                    SkipPadding(header.width);
                }
            else
                for ( var r = 0; r <= -header.height; r++ )
                {
                    var row = ReadPixels(header.width);
                    for ( var c = 0; c < header.width; c++ )
                        bmp[c , r] = row[c];
                    SkipPadding(header.width);
                }
            return bmp;
        }

        private BitmapPalette4bpp Read4BppPalette()
        {
            BitmapPalette4bpp palette = new BitmapPalette4bpp();
            for ( var i = 0; i < 16; i++ )
            {
                var b = Read();
                var g = Read();
                var r = Read();
                var a = Read();
                palette[i] = Pixel.FromArgb(a , r , g , b);
            }
            return palette;
        }
        private BitmapPalette2bpp Read2BppPalette()
        {
            BitmapPalette2bpp palette = new BitmapPalette2bpp();
            for ( var i = 0; i < 4; i++ )
            {
                var b = Read();
                var g = Read();
                var r = Read();
                var a = Read();
                palette[i] = Pixel.FromArgb(a , r , g , b);
            }
            return palette;
        }
        private BitmapPalette1bpp Read1BppPalette()
        {
            BitmapPalette1bpp palette = new BitmapPalette1bpp();
            for ( var i = 0; i < 2; i++ )
            {
                var b = Read();
                var g = Read();
                var r = Read();
                var a = Read();
                palette[i] = Pixel.FromArgb(a , r , g , b);
            }
            return palette;
        }

        private BitmapPalette8bpp ReadPalette()
        {
            BitmapPalette8bpp palette = new BitmapPalette8bpp();
            for ( var i = 0; i < 256; i++ )
            {
                var b = Read();
                var g = Read();
                var r = Read();
                var a = Read();
                palette[i] = Pixel.FromArgb(a , r , g , b);
            }
            return palette;
        }
        private Pixel[ ] ReadPixels(int count)
        {
            Pixel[] array = new Pixel[count];
            for ( var i = 0; i < count; i++ )
            {
                byte a,r,g,b;
                switch ( header.numberOfBitPerPixel )
                {
                    case 1:
                        b = Read();
                        for ( var j = 0; j < 8&& i < count; j++ )
                        {
                            array[i] = palette1bpp[(b >> (7-j)) & 0x1];
                            i++;
                        }
                        i--;
                        break;
                    case 2:
                        b = Read();
                        for ( var j = 0; j < 4 && i < count; j++ )
                        {
                            array[i] = palette2bpp[(b>>(7-j*2)) & 0x3];
                            i++;
                        }
                        i--;
                        break;
                    case 4:
                        b = Read();
                        var pixel2= palette4bpp[b >> 4];
                        var pixel1 =palette4bpp[b &0xF];
                        array[i] = pixel2;
                        i++;
                        if ( i < count )
                            array[i] = pixel1;
                        break;
                    case 8:
                        b = Read();
                        array[i] = palette8bpp[b];
                        break;
                    case 24:
                        a = 255;
                        b = Read();
                        g = Read();
                        r = Read();
                        array[i] = Pixel.FromArgb(a , r , g , b);
                        break;
                    case 32:
                        b = Read();
                        g = Read();
                        r = Read();
                        a = Read();
                        array[i] = Pixel.FromArgb(a , r , g , b);
                        break;
                }
            }
            return array;
        }
        private void SkipPadding(int count)
        {
            var readed = header.numberOfBitPerPixel * count;
            while ( readed % 8 != 0 )
                readed++;
            while ( readed % 32 != 0 )
            {
                Read();
                readed += 8;
            }
        }
        public unsafe bool ReadHeader()
        {
            byte[] b = new byte[54];
            for ( var i = 0; i < b.Length; i++ )
                b[i] = Read();

            fixed ( byte* ptr = b )
            {
                header = *( ( BITMAP_HEADER* ) ptr );
            }
            header.size = BitConverter.ToInt32(b , 2);
            header.bitmapOffset = BitConverter.ToInt32(b , 10);
            header.sizeOfDIB = BitConverter.ToInt32(b , 14);
            header.width = BitConverter.ToInt32(b , 18);
            header.height = BitConverter.ToInt32(b , 22);
            header.numberOfColorPlane = BitConverter.ToUInt16(b , 26);
            header.numberOfBitPerPixel = BitConverter.ToUInt16(b , 28);
            header.compression = BitConverter.ToInt32(b , 30);
            header.sizeOfBitmapDataInclusingPadding = BitConverter.ToInt32(b , 34);
            header.printResolutionH = BitConverter.ToInt32(b , 38);
            header.printResolutionV = BitConverter.ToInt32(b , 42);
            header.numberOfColorInPalette = BitConverter.ToInt32(b , 46);
            header.numberOfImportantColor = BitConverter.ToInt32(b , 50);
            if ( header.numberOfColorInPalette > 256 )
                throw new InvalidDataException("Cannot support more than 256 color in palette");
            if ( header.numberOfColorPlane != 1 )
                throw new InvalidDataException("Cannot support more than 1 color plane");
            if ( header.sizeOfDIB < 40 )
                throw new InvalidDataException("Header is too short");
            if ( header.sizeOfDIB > 40 )
                throw new InvalidDataException("Header is too long");
            if ( header.numberOfBitPerPixel != 1
                && header.numberOfBitPerPixel != 2
                && header.numberOfBitPerPixel != 4
                && header.numberOfBitPerPixel != 8
                && header.numberOfBitPerPixel != 24
                && header.numberOfBitPerPixel != 32 )
                throw new InvalidDataException("Support only 1bpp,2bpp,4bpp,8bpp,24bpp and 32bpp");
            if ( header.width <= 0 )
                throw new InvalidDataException("Width must be more than 0");
            if ( header.height <= 0 )
                throw new InvalidDataException("Height must be more than 0");
            return true;
        }

        public void SetStream(BufferedStream s)
        {
            this.s = s;
        }
    }
}
