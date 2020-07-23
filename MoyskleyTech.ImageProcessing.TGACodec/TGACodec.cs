using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using MoyskleyTech.ImageProcessing.Image;

namespace ImageProcessing.TGACodec
{
    public class TGACodec : IBitmapCodec
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

        public Image<Pixel> DecodeStream(Stream s)
        {
            return TgaReader.Load(s);
        }

        public IEnumerable<ColorPoint<T>> ReadData<T>(Stream s) where T : unmanaged
        {
            var converter = ColorConvert.GetConversionFrom<Pixel,T>();
            var bmp=DecodeStream(s);
            return from x in Enumerable.Range(0 , bmp.Width)
                   from y in Enumerable.Range(0 , bmp.Height)
                   select new ColorPoint<T>(x , y , converter(bmp[x , y]));
        }

        public Image<T> ReadImage<T>(Stream s) where T : unmanaged
        {
            return DecodeStream(s).ConvertBufferTo<T>();
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

        public void Save<T>(ImageProxy<T> bmp , Stream s) where T : unmanaged
        {
            throw new NotImplementedException();
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
