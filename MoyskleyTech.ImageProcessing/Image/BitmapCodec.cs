﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace MoyskleyTech.ImageProcessing.Image
{
    /// <summary>
    /// Represent the codec for bitmap
    /// </summary>
    public class BitmapCodec : IBitmapCodec
    {
        /// <summary>
        /// Return the lenght of bitmap codec(2)
        /// </summary>
        public int SignatureLength
        {
            get
            {
                return 2;
            }
        }
        /// <summary>
        /// Check if signature is bitmap
        /// </summary>
        /// <param name="signature"></param>
        /// <param name="f"></param>
        /// <returns>Decoder if bitmap signature or null</returns>
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

        /// <summary>
        /// Load bitmap from stream
        /// </summary>
        /// <param name="s"></param>
        /// <returns></returns>
        public Image<Pixel> DecodeStream(Stream s)
        {
            BitmapDecoder decoder = new BitmapDecoder();
            decoder.SetStream(new BufferedStream(s , new byte[0]));
            decoder.ReadHeader();
            return decoder.ReadBitmap();
        }

        public IEnumerable<ColorPoint<T>> ReadData<T>(Stream s) where T : unmanaged
        {
            BitmapDecoder decoder = new BitmapDecoder();
            decoder.SetStream(new BufferedStream(s , new byte[0]));
            decoder.ReadHeader();
            return decoder.ReadData<T>();
        }

        public Image<T> ReadImage<T>(Stream s) where T : unmanaged
        {
            BitmapDecoder decoder = new BitmapDecoder();
            decoder.SetStream(new BufferedStream(s , new byte[0]));
            decoder.ReadHeader();
            var img = Image<T>.Create(decoder.Width , decoder.Height);
            foreach ( var px in decoder.ReadData<T>() )
            {
                img[px.Point] = px.Color;
            }
            return img;
        }

        /// <summary>
        /// Save bitmap to stream
        /// </summary>
        /// <param name="bmp"></param>
        /// <param name="s"></param>
        /// /// <param name="palette"></param>
        public void Save<T>(ImageProxy<T> bmp , Stream s , BitmapPalette8bpp palette)
            where T : unmanaged
        {
            SaveStream(bmp , s , palette);
        }
        /// <summary>
        /// Save bitmap to stream
        /// </summary>
        /// <param name="bmp"></param>
        /// <param name="s"></param>
        /// /// <param name="palette"></param>
        public void Save<T>(ImageProxy<T> bmp , Stream s , BitmapPalette4bpp palette)
            where T : unmanaged
        {
            SaveStream(bmp , s , palette);
        }
        /// <summary>
        /// Save bitmap to stream
        /// </summary>
        /// <param name="bmp"></param>
        /// <param name="s"></param>
        /// /// <param name="palette"></param>
        public void Save<T>(ImageProxy<T> bmp , Stream s , BitmapPalette2bpp palette)
            where T : unmanaged
        {
            SaveStream(bmp , s , palette);
        }
        /// <summary>
        /// Save bitmap to stream
        /// </summary>
        /// <param name="bmp"></param>
        /// <param name="s"></param>
        /// /// <param name="palette"></param>
        public void Save<T>(ImageProxy<T> bmp , Stream s , BitmapPalette1bpp palette)
            where T : unmanaged
        {
            SaveStream(bmp , s , palette);
        }
        /// <summary>
        /// Save the bitmap as optimized size
        /// </summary>
        /// <param name="bmp"></param>
        /// <param name="s"></param>
        /// <param name="allow1bpp">Allow 1bpp bitmap</param>
        /// <param name="allow2bpp">Allow 2bpp bitmap</param>
        /// <param name="allow4bpp">Allow 4bpp bitmap</param>
        /// <param name="allow8bpp">Allow 8bpp bitmap</param>
        public void SaveOptimized<T>(ImageProxy<T> bmp , Stream s , bool allow1bpp = true , bool allow2bpp = false , bool allow4bpp = true , bool allow8bpp = true)
            where T : unmanaged
        {
            var stats = new ImageStatistics(bmp.As<Pixel>());
            var dom = stats.ColorStatistics.Dominance;
            if ( dom.Count <= 2 && allow1bpp )
                SaveStream(bmp , s , CreatePalette1(dom));
            else if ( dom.Count <= 4 && allow2bpp )
                SaveStream(bmp , s , CreatePalette2(dom));
            else if ( dom.Count <= 16 && allow4bpp )
                SaveStream(bmp , s , CreatePalette4(dom));
            else if ( dom.Count <= 256 && allow8bpp )
                SaveStream(bmp , s , CreatePalette8(dom));
            else
                Save(bmp , s);

        }
        /// <summary>
        /// Create palette of 8bpp
        /// </summary>
        /// <param name="dom"></param>
        /// <returns></returns>
        private BitmapPalette8bpp CreatePalette8(Dictionary<Pixel , int> dom)
        {
            BitmapPalette8bpp palette = new BitmapPalette8bpp();
            var keys = dom.Keys.ToArray();
            for ( var i = 0; i < dom.Count; i++ )
                palette[i] = keys[i];
            return palette;
        }
        /// <summary>
        /// Create palette of 4bpp
        /// </summary>
        /// <param name="dom"></param>
        /// <returns></returns>
        private BitmapPalette4bpp CreatePalette4(Dictionary<Pixel , int> dom)
        {
            BitmapPalette4bpp palette = new BitmapPalette4bpp();
            var keys = dom.Keys.ToArray();
            for ( var i = 0; i < dom.Count; i++ )
                palette[i] = keys[i];
            return palette;
        }
        /// <summary>
        /// Create palette of 2bpp
        /// </summary>
        /// <param name="dom"></param>
        /// <returns></returns>
        private BitmapPalette2bpp CreatePalette2(Dictionary<Pixel , int> dom)
        {
            BitmapPalette2bpp palette = new BitmapPalette2bpp();
            var keys = dom.Keys.ToArray();
            for ( var i = 0; i < dom.Count; i++ )
                palette[i] = keys[i];
            return palette;
        }
        /// <summary>
        /// Create monochrome palette
        /// </summary>
        /// <param name="dom"></param>
        /// <returns></returns>
        private BitmapPalette1bpp CreatePalette1(Dictionary<Pixel , int> dom)
        {
            BitmapPalette1bpp palette = new BitmapPalette1bpp();
            var keys = dom.Keys.ToArray();
            for ( var i = 0; i < dom.Count; i++ )
                palette[i] = keys[i];
            return palette;
        }
        /// <summary>
        /// Write bitmap to stream
        /// </summary>
        /// <param name="s">Destination</param>
        public unsafe void Save<T>(ImageProxy<T> bmp , Stream s)
            where T : unmanaged
        {
            s.WriteByte(( byte ) 'B');//0
            s.WriteByte(( byte ) 'M');//1


            var size = bmp.Width*bmp.Height*4+54;
            var sizeAsByte = BitConverter.GetBytes(size);

            s.Write(sizeAsByte , 0 , 4);//2-5

            s.WriteByte(0);//6
            s.WriteByte(0);//7
            s.WriteByte(0);//8
            s.WriteByte(0);//9

            s.Write(BitConverter.GetBytes(54) , 0 , 4);//10-13

            s.Write(BitConverter.GetBytes(40) , 0 , 4);//14-17

            s.Write(BitConverter.GetBytes(bmp.Width) , 0 , 4);//18-21
            s.Write(BitConverter.GetBytes(bmp.Height) , 0 , 4);//22-25

            s.Write(BitConverter.GetBytes(( short ) 1) , 0 , 2);//26-27
            s.Write(BitConverter.GetBytes(( short ) 32) , 0 , 2);//28-29

            s.Write(BitConverter.GetBytes(0) , 0 , 4);//30-33

            s.Write(BitConverter.GetBytes(0) , 0 , 4);//imagesize

            s.Write(BitConverter.GetBytes(unchecked(( int ) 0x00000EC4)) , 0 , 4);
            s.Write(BitConverter.GetBytes(unchecked(( int ) 0x00000EC4)) , 0 , 4);

            s.Write(BitConverter.GetBytes(0) , 0 , 4);//numcolorspalette
            s.Write(BitConverter.GetBytes(0) , 0 , 4);//mostimpcolor


            var converter = ColorConvert.GetConversionFrom<T,Pixel>();
            for ( var i = bmp.Height - 1; i >= 0; i-- )
            {
                for ( var j = 0; j < bmp.Width; j++ )
                {
                    var ptr = converter(bmp[j,i]);
                    s.WriteByte(ptr.B);
                    s.WriteByte(ptr.G);
                    s.WriteByte(ptr.R);
                    s.WriteByte(ptr.A);
                }
            }
        }
        private unsafe void SaveStream<T>(ImageProxy<T> bmp , Stream s , BitmapPalette8bpp palette)
            where T : unmanaged
        {
            palette = palette ?? BitmapPalette8bpp.Grayscale;
            s.WriteByte(( byte ) 'B');//0
            s.WriteByte(( byte ) 'M');//1

            const int sizeOfPalette = 256*4;
            var size = (int)(System.Math.Ceiling(bmp.Width/4d)*4*bmp.Height+54+(sizeOfPalette));
            var sizeAsByte = BitConverter.GetBytes(size);

            s.Write(sizeAsByte , 0 , 4);//2-5

            s.WriteByte(0);//6
            s.WriteByte(0);//7
            s.WriteByte(0);//8
            s.WriteByte(0);//9

            s.Write(BitConverter.GetBytes(54) , 0 , 4);//10-13

            s.Write(BitConverter.GetBytes(40) , 0 , 4);//14-17

            s.Write(BitConverter.GetBytes(bmp.Width) , 0 , 4);//18-21
            s.Write(BitConverter.GetBytes(bmp.Height) , 0 , 4);//22-25

            s.Write(BitConverter.GetBytes(( short ) 1) , 0 , 2);//26-27
            s.Write(BitConverter.GetBytes(( short ) 8) , 0 , 2);//28-29

            s.Write(BitConverter.GetBytes(0) , 0 , 4);//30-33

            s.Write(BitConverter.GetBytes(0) , 0 , 4);//imagesize

            s.Write(BitConverter.GetBytes(unchecked(( int ) 0x00000EC4)) , 0 , 4);
            s.Write(BitConverter.GetBytes(unchecked(( int ) 0x00000EC4)) , 0 , 4);

            s.Write(BitConverter.GetBytes(256) , 0 , 4);//numcolorspalette
            s.Write(BitConverter.GetBytes(0) , 0 , 4);//mostimpcolor
            s.Flush();
            for ( int i = 0; i < 256; i++ )
            {
                Pixel pi = palette[i];
                s.WriteByte(pi.B);
                s.WriteByte(pi.G);
                s.WriteByte(pi.R);
                s.WriteByte(pi.A);
            }
            s.Flush();
            var converter = ColorConvert.GetConversionFrom<T,Pixel>();
            for ( var i = bmp.Height - 1; i >= 0; i-- )
            {
                for ( var j = 0; j < bmp.Width; j++ )
                {
                    var idx = palette[converter(bmp[j,i])];
                    if ( idx < 0 )
                        throw new ArgumentException("Bitmap has color not in palette, please reduce first");
                    s.WriteByte(( byte ) idx);
                }
                for ( var j = bmp.Width; j % 4 != 0; j++ )
                {
                    s.WriteByte(0);
                }
            }
        }
        private unsafe void SaveStream<T>(ImageProxy<T> bmp , Stream s , BitmapPalette4bpp palette)
            where T : unmanaged
        {
            s.WriteByte(( byte ) 'B');//0
            s.WriteByte(( byte ) 'M');//1

            const int sizeOfPalette = 16*4;
            var size = (int)(System.Math.Ceiling(System.Math.Ceiling(bmp.Width / 2d)/4d)*4*bmp.Height+54+(sizeOfPalette));
            var sizeAsByte = BitConverter.GetBytes(size);

            s.Write(sizeAsByte , 0 , 4);//2-5

            s.WriteByte(0);//6
            s.WriteByte(0);//7
            s.WriteByte(0);//8
            s.WriteByte(0);//9

            s.Write(BitConverter.GetBytes(54) , 0 , 4);//10-13

            s.Write(BitConverter.GetBytes(40) , 0 , 4);//14-17

            s.Write(BitConverter.GetBytes(bmp.Width) , 0 , 4);//18-21
            s.Write(BitConverter.GetBytes(bmp.Height) , 0 , 4);//22-25

            s.Write(BitConverter.GetBytes(( short ) 1) , 0 , 2);//26-27
            s.Write(BitConverter.GetBytes(( short ) 4) , 0 , 2);//28-29

            s.Write(BitConverter.GetBytes(0) , 0 , 4);//30-33

            s.Write(BitConverter.GetBytes(0) , 0 , 4);//imagesize

            s.Write(BitConverter.GetBytes(unchecked(( int ) 0x00000EC4)) , 0 , 4);
            s.Write(BitConverter.GetBytes(unchecked(( int ) 0x00000EC4)) , 0 , 4);

            s.Write(BitConverter.GetBytes(16) , 0 , 4);//numcolorspalette
            s.Write(BitConverter.GetBytes(0) , 0 , 4);//mostimpcolor
            s.Flush();
            for ( int i = 0; i < 16; i++ )
            {
                Pixel pi = palette[i];
                s.WriteByte(pi.B);
                s.WriteByte(pi.G);
                s.WriteByte(pi.R);
                s.WriteByte(pi.A);
            }
            s.Flush();
            var converter = ColorConvert.GetConversionFrom<T,Pixel>();
            for ( var i = bmp.Height - 1; i >= 0; i-- )
            {

                byte actual=0;
                for ( var j = 0; j < bmp.Width; j++ )
                {
                    var idx1 = palette[converter(bmp[j,i])];
                    if ( idx1 < 0 )
                        throw new ArgumentException("Bitmap has color not in palette, please reduce first");
                    if ( ( j & 1 ) == 0 )
                    {
                        actual = ( byte ) ( idx1 << 4 );
                    }
                    else
                    {
                        actual |= ( byte ) ( idx1 );
                        s.WriteByte(actual);
                    }
                }
                if ( ( bmp.Width & 1 ) == 1 )
                    s.WriteByte(actual);
                for ( var j = System.Math.Ceiling(bmp.Width / 2d); j % 4 != 0; j++ )
                {
                    s.WriteByte(0);
                }
            }
        }
        private unsafe void SaveStream<T>(ImageProxy<T> bmp , Stream s , BitmapPalette2bpp palette)
            where T : unmanaged
        {
            s.WriteByte(( byte ) 'B');//0
            s.WriteByte(( byte ) 'M');//1

            const int sizeOfPalette = 4*4;
            var size = (int)(System.Math.Ceiling(System.Math.Ceiling(bmp.Width / 4d)/4d)*4*bmp.Height+54+(sizeOfPalette));
            var sizeAsByte = BitConverter.GetBytes(size);

            s.Write(sizeAsByte , 0 , 4);//2-5

            s.WriteByte(0);//6
            s.WriteByte(0);//7
            s.WriteByte(0);//8
            s.WriteByte(0);//9

            s.Write(BitConverter.GetBytes(54) , 0 , 4);//10-13

            s.Write(BitConverter.GetBytes(40) , 0 , 4);//14-17

            s.Write(BitConverter.GetBytes(bmp.Width) , 0 , 4);//18-21
            s.Write(BitConverter.GetBytes(bmp.Height) , 0 , 4);//22-25

            s.Write(BitConverter.GetBytes(( short ) 1) , 0 , 2);//26-27
            s.Write(BitConverter.GetBytes(( short ) 2) , 0 , 2);//28-29

            s.Write(BitConverter.GetBytes(0) , 0 , 4);//30-33

            s.Write(BitConverter.GetBytes(0) , 0 , 4);//imagesize

            s.Write(BitConverter.GetBytes(unchecked(( int ) 0x00000EC4)) , 0 , 4);
            s.Write(BitConverter.GetBytes(unchecked(( int ) 0x00000EC4)) , 0 , 4);

            s.Write(BitConverter.GetBytes(4) , 0 , 4);//numcolorspalette
            s.Write(BitConverter.GetBytes(0) , 0 , 4);//mostimpcolor
            s.Flush();
            for ( int i = 0; i < 4; i++ )
            {
                Pixel pi = palette[i];
                s.WriteByte(pi.B);
                s.WriteByte(pi.G);
                s.WriteByte(pi.R);
                s.WriteByte(pi.A);
            }
            s.Flush();
            var converter = ColorConvert.GetConversionFrom<T,Pixel>();
            for ( var i = bmp.Height - 1; i >= 0; i-- )
            {
                byte actual=0;
                for ( var j = 0; j < bmp.Width; j++ )
                {
                    var idx1 = palette[converter(bmp[j,i])];
                    if ( idx1 < 0 )
                        throw new ArgumentException("Bitmap has color not in palette, please reduce first");
                    var mod=j%4;
                    if ( mod == 0 )
                    {
                        actual = ( byte ) ( idx1 << 6 );
                    }
                    else if ( mod == 1 )
                    {
                        actual |= ( byte ) ( idx1 << 4 );
                    }
                    else if ( mod == 2 )
                    {
                        actual |= ( byte ) ( idx1 << 2 );
                    }
                    else if ( mod == 3 )
                    {
                        actual |= ( byte ) ( idx1 << 0 );
                        s.WriteByte(actual);
                    }
                }
                if ( bmp.Width % 4 != 0 )
                    s.WriteByte(actual);
                for ( var j = System.Math.Ceiling(bmp.Width / 4d); j % 4 != 0; j++ )
                {
                    s.WriteByte(0);
                }
            }
        }
        private unsafe void SaveStream<T>(ImageProxy<T> bmp , Stream s , BitmapPalette1bpp palette)
            where T : unmanaged
        {
            s.WriteByte(( byte ) 'B');//0
            s.WriteByte(( byte ) 'M');//1

            const int sizeOfPalette = 2*4;
            var size = (int)(System.Math.Ceiling(System.Math.Ceiling(bmp.Width / 8d)/4d)*4*bmp.Height+54+(sizeOfPalette));
            var sizeAsByte = BitConverter.GetBytes(size);

            s.Write(sizeAsByte , 0 , 4);//2-5

            s.WriteByte(0);//6
            s.WriteByte(0);//7
            s.WriteByte(0);//8
            s.WriteByte(0);//9

            s.Write(BitConverter.GetBytes(54) , 0 , 4);//10-13

            s.Write(BitConverter.GetBytes(40) , 0 , 4);//14-17

            s.Write(BitConverter.GetBytes(bmp.Width) , 0 , 4);//18-21
            s.Write(BitConverter.GetBytes(bmp.Height) , 0 , 4);//22-25

            s.Write(BitConverter.GetBytes(( short ) 1) , 0 , 2);//26-27
            s.Write(BitConverter.GetBytes(( short ) 1) , 0 , 2);//28-29

            s.Write(BitConverter.GetBytes(0) , 0 , 4);//30-33

            s.Write(BitConverter.GetBytes(0) , 0 , 4);//imagesize

            s.Write(BitConverter.GetBytes(unchecked(( int ) 0x00000EC4)) , 0 , 4);
            s.Write(BitConverter.GetBytes(unchecked(( int ) 0x00000EC4)) , 0 , 4);

            s.Write(BitConverter.GetBytes(2) , 0 , 4);//numcolorspalette
            s.Write(BitConverter.GetBytes(0) , 0 , 4);//mostimpcolor
            s.Flush();
            for ( int i = 0; i < 2; i++ )
            {
                Pixel pi = palette[i];
                s.WriteByte(pi.B);
                s.WriteByte(pi.G);
                s.WriteByte(pi.R);
                s.WriteByte(pi.A);
            }
            s.Flush();
            var converter = ColorConvert.GetConversionFrom<T,Pixel>();
            for ( var i = bmp.Height - 1; i >= 0; i-- )
            {

                byte actual=0;
                for ( var j = 0; j < bmp.Width; j++ )
                {
                    var idx1 = palette[converter(bmp[j,i])];
                    if ( idx1 < 0 )
                        throw new ArgumentException("Bitmap has color not in palette, please reduce first");
                    var mod=j%8;
                    if ( mod == 0 )
                    {
                        actual = ( byte ) idx1;
                    }
                    else if ( mod != 7 )
                    {
                        actual |= ( byte ) ( idx1 << mod );
                    }
                    else if ( mod == 7 )
                    {
                        actual |= ( byte ) ( idx1 << 7 );
                        s.WriteByte(actual);
                    }
                }
                if ( bmp.Width % 8 != 0 )
                    s.WriteByte(actual);
                for ( var j = System.Math.Ceiling(bmp.Width / 8d); j % 4 != 0; j++ )
                {
                    s.WriteByte(0);
                }
            }
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

        public int Height => System.Math.Abs(header.height);

        public int Width => header.width;

        private byte Read()
        {
            index++;
            return s.Read();
        }
        public unsafe Image<Pixel> ReadBitmap()
        {
            Image<Pixel> bmp = Image<Pixel>.Create(header.width,System.Math.Abs(header.height));
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
        public IEnumerable<ColorPoint<T>> ReadData<T>()
            where T:unmanaged
        {
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
                    foreach ( var px in ReadRow<T>(header.width,r) )
                        yield return px;
                    SkipPadding(header.width);
                }
            else
                for ( var r = 0; r <= -header.height; r++ )
                {
                    foreach ( var px in ReadRow<T>(header.width , r) )
                        yield return px;
                    SkipPadding(header.width);
                }
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
        private IEnumerable<ColorPoint<T>> ReadRow<T>(int count,int y)
            where T:struct
        {
            var pxToT = ColorConvert.GetConversionFrom<Pixel,T>();
            for ( var i = 0; i < count; i++ )
            {
                byte a,r,g,b;
                switch ( header.numberOfBitPerPixel )
                {
                    case 1:
                        b = Read();
                        for ( var j = 0; j < 8 && i < count; j++ )
                        {
                            yield return new ColorPoint<T>(i , y , pxToT(palette1bpp[( b >> ( 7 - j ) ) & 0x1]));
                            i++;
                        }
                        i--;
                        break;
                    case 2:
                        b = Read();
                        for ( var j = 0; j < 4 && i < count; j++ )
                        {
                            yield return new ColorPoint<T>(i , y , pxToT(palette2bpp[( b >> ( 7 - j * 2 ) ) & 0x3]));
                            i++;
                        }
                        i--;
                        break;
                    case 4:
                        b = Read();
                        var pixel2= palette4bpp[b >> 4];
                        var pixel1 =palette4bpp[b &0xF];
                        yield return new ColorPoint<T>(i , y , pxToT(pixel2));
                        i++;
                        if ( i < count )
                            yield return new ColorPoint<T>(i , y , pxToT(pixel1));
                        break;
                    case 8:
                        b = Read();
                        yield return new ColorPoint<T>(i , y , pxToT(palette8bpp[b]));
                        break;
                    case 24:
                        a = 255;
                        b = Read();
                        g = Read();
                        r = Read();
                        yield return new ColorPoint<T>(i , y , pxToT(Pixel.FromArgb(a , r , g , b)));
                        break;
                    case 32:
                        b = Read();
                        g = Read();
                        r = Read();
                        a = Read();
                        yield return new ColorPoint<T>(i , y , pxToT(Pixel.FromArgb(a , r , g , b)));
                        break;
                }
            }
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
                        for ( var j = 0; j < 8 && i < count; j++ )
                        {
                            array[i] = palette1bpp[( b >> ( 7 - j ) ) & 0x1];
                            i++;
                        }
                        i--;
                        break;
                    case 2:
                        b = Read();
                        for ( var j = 0; j < 4 && i < count; j++ )
                        {
                            array[i] = palette2bpp[( b >> ( 7 - j * 2 ) ) & 0x3];
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
