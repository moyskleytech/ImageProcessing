using MoyskleyTech.ImageProcessing.Image;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace MoyskleyTech.ImageProcessing.IO
{
    public class FileImageCryptor
    {
        public static void Crypt(string fileIn , string fileOut)
        {
            var fsIn = System.IO.File.OpenRead(fileIn);
            var fsOut = System.IO.File.OpenWrite(fileOut);
            SaveStream(fsIn , fsOut);
            fsIn.Dispose();
            fsOut.Dispose();
        }
        private static void SaveStream(Stream i , Stream o)
        {
            o.WriteByte(( byte ) 'B');//0
            o.WriteByte(( byte ) 'M');//1

            const int sizeOfPalette = 2*4;
            var size = (int)(System.Math.Ceiling(i.Length/4d)*4+54+(sizeOfPalette));
            var sizeAsByte = BitConverter.GetBytes(size);

            o.Write(sizeAsByte , 0 , 4);//2-5

            o.WriteByte(0);//6
            o.WriteByte(0);//7
            o.WriteByte(0);//8
            o.WriteByte(0);//9

            o.Write(BitConverter.GetBytes(54) , 0 , 4);//10-13

            o.Write(BitConverter.GetBytes(40) , 0 , 4);//14-17

            o.Write(BitConverter.GetBytes(32) , 0 , 4);//18-21
            o.Write(BitConverter.GetBytes(i.Length) , 0 , 4);//22-25

            o.Write(BitConverter.GetBytes(( short ) 1) , 0 , 2);//26-27
            o.Write(BitConverter.GetBytes(( short ) 1) , 0 , 2);//28-29

            o.Write(BitConverter.GetBytes(0) , 0 , 4);//30-33

            o.Write(BitConverter.GetBytes(0) , 0 , 4);//imagesize

            o.Write(BitConverter.GetBytes(unchecked(( int ) 0x00000EC4)) , 0 , 4);
            o.Write(BitConverter.GetBytes(unchecked(( int ) 0x00000EC4)) , 0 , 4);

            o.Write(BitConverter.GetBytes(2) , 0 , 4);//numcolorspalette
            o.Write(BitConverter.GetBytes(0) , 0 , 4);//mostimpcolor
            o.Flush();
            Pixel pi = Pixels.Black;
            for ( int c = 0; c < 2; c++ )
            {
                o.WriteByte(pi.B);
                o.WriteByte(pi.G);
                o.WriteByte(pi.R);
                o.WriteByte(pi.A);
                pi = Pixels.Cyan;
            }
            o.Flush();
            var len = i.Length;
            for ( var c = 0; c <len; c++ )
            {
                o.WriteByte((byte)i.ReadByte());
            }
            while ( len % 4 != 0 )
                o.WriteByte((byte)(( len++ ) * 0));

        }
    }
}
