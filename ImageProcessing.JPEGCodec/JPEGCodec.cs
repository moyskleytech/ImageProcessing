using MoyskleyTech.ImageProcessing.Image;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using BitMiracle;
using BitMiracle.LibJpeg;
namespace ImageProcessing.JPEGCodec
{
    internal struct COMP3 { public byte A,B,C; };
    public class JPEGCodec : IBitmapCodec
    {

        public static void Register()
        {
            BitmapFactory.RegisterCodec(new JPEGCodec());
        }
        public int SignatureLength
        {
            get
            {
                return 3;
            }
        }

        public IBitmapDecoder CheckSignature(string signature , Stream f)
        {
            if ( signature.Length == 0 )
            {
                JPEGDecoder decoder = new JPEGDecoder();
                decoder.SetStream(new BufferedStream(f , new byte[0]));
                return decoder;
            }
            if ( signature[0] == 0xFF )
                if ( signature[1] == 0xD8 )
                    if ( signature[2] == 0xFF )
                    {
                        byte[] buffer = new byte[signature.Length];
                        for ( var i = 0; i < signature.Length; i++ )
                            buffer[i] = ( byte ) signature[i];
                        JPEGDecoder decoder = new JPEGDecoder();
                        decoder.SetStream(new BufferedStream(f , buffer));
                        return decoder;
                    }
            return null;
        }

        public unsafe void Save(Bitmap bmp , Stream s)
        {
            SampleRow[] rows = new SampleRow[bmp.Height];
            Pixel black = Pixels.Black;
            for ( var r = 0; r < bmp.Height; r++ )
            {
                byte[] rowB = new byte[bmp.Width*3];
                fixed ( byte* tab = rowB )
                {
                    COMP3* ptr = (COMP3*)tab;
                    for ( var c = 0; c < bmp.Width; c++ )
                    {
                        var pixel = bmp[c,r];
                        pixel = pixel.Over(black);
                        ptr->A = pixel.R;
                        ptr->B = pixel.G;
                        ptr->C = pixel.B;
                        ptr++;
                    }
                }
                rows[r] = new SampleRow(rowB , bmp.Width , 8 , 3);
            }
            JpegImage img = new JpegImage(rows, Colorspace.RGB);

            img.WriteJpeg(s);
        }

        public Bitmap DecodeStream(Stream s)
        {
            JPEGDecoder decoder = new JPEGDecoder();
            decoder.SetStream(new BufferedStream(s , new byte[0]));
            decoder.ReadHeader();
            return decoder.ReadBitmap();
        }
    }
    internal class JPEGDecoder : IBitmapDecoder
    {
        BufferedStream s;

        public Bitmap ReadBitmap()
        {
            JpegImage img = new JpegImage(s);
            if ( img.Colorspace == Colorspace.RGB )
            {
                var bmp = new Bitmap(img.Width,img.Height);
                for ( var r = 0; r < img.Height; r++ )
                {
                    var row = img.GetRow(r);
                    for ( var c = 0; c < img.Width; c++ )
                    {
                        var sample = row.GetAt(c);
                        var components = new byte[3];
                        components[0] = ( byte ) sample.GetComponent(0);
                        components[1] = ( byte ) sample.GetComponent(1);
                        components[2] = ( byte ) sample.GetComponent(2);
                        bmp[c , r] = Pixel.FromArgb(255 , components[0] , components[1] , components[2]);
                    }
                }
                return bmp;
            }
            else
            {
                MemoryStream ms = new MemoryStream();
                img.WriteBitmap(ms);
                ms.Position = 0;
                var bmp = new BitmapCodec().DecodeStream(ms);
                ms.Dispose();
                return bmp;
            }
        }

        public bool ReadHeader()
        {
            return true;
        }

        public void SetStream(BufferedStream s)
        {
            this.s = s;
        }
    }
}
