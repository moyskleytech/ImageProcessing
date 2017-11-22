using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using MoyskleyTech.ImageProcessing.Image;

namespace ImageProcessing.TGACodec
{
    public class TGACodec : MoyskleyTech.ImageProcessing.Image.IBitmapCodec
    {
        public int SignatureLength
        {
            get
            {
                return 0;
            }
        }

        public IBitmapDecoder CheckSignature(string signature , Stream f)
        {
            return null;
        }

        public Bitmap DecodeStream(Stream s)
        {
            return TgaReader.Load(s);
        }

        public void Save(Bitmap bmp , Stream s)
        {
            //idFieldLenght
            s.WriteByte(0);
            //ColorMap
            s.WriteByte(0);
            //ImageType
            s.WriteByte(2);
            //ColorMapOffset
            s.WriteByte(0);
            s.WriteByte(0);
            //ColorsUsed
            s.WriteByte(0);
            s.WriteByte(0);
            //bitsPerColorMap
            s.WriteByte(0);
            //x/y coord
            s.WriteByte(0);
            s.WriteByte(0);
            s.WriteByte(0);
            s.WriteByte(0);
            //width
            s.Write(LittleEndian(( ushort ) bmp.Width) , 0 , 2);
            //height
            s.Write(LittleEndian(( ushort ) bmp.Height) , 0 , 2);
            //bitPerPixel
            s.WriteByte(32);
            //imgFlag
            s.WriteByte(0);

            for ( var y = bmp.Height-1; y >=0; y-- )
                for ( var x = 0; x < bmp.Width; x++ )
                {
                    var px = bmp[x,y];
                    s.WriteByte(px.B);
                    s.WriteByte(px.G);
                    s.WriteByte(px.R);
                    s.WriteByte(px.A);
                }
        }

        private byte[ ] LittleEndian(ushort width)
        {
            byte[] a = BitConverter.GetBytes(width);
            if ( !BitConverter.IsLittleEndian )
                a = a.Reverse().ToArray();
            return a;
        }
    }
}
