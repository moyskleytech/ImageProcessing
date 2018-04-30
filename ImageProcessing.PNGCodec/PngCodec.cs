using MoyskleyTech.ImageProcessing.Image;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using Hjg.Pngcs;

namespace ImageProcessing.PNGCodec
{
#pragma warning disable CS0649
    public struct COMP3 { public byte A,B,C; };
    public struct COMP4 { public byte A,B,C,D; };
    public class PngCodec : IBitmapCodec
    {
        private byte[] signature = new byte[] { 137, 80, 78, 71, 13, 10, 26, 10};
        public int SignatureLength
        {
            get
            {
                return signature.Length;
            }
        }

        public static void Register()
        {
            BitmapFactory.RegisterCodec(new PngCodec());
        }

        public IBitmapDecoder CheckSignature(string signature , Stream f)
        {
            for ( var i = 0; i < this.signature.Length; i++ )
                if ( ( byte ) signature[i] != this.signature[i] )
                    return null;
            byte[] buffer = new byte[signature.Length];
            for ( var i = 0; i < signature.Length; i++ )
                buffer[i] = ( byte ) signature[i];
            PngDecoder decoder = new PngDecoder();
            decoder.SetStream(new BufferedStream(f , buffer));
            return decoder;
        }

        public Bitmap DecodeStream(Stream s)
        {
            PngDecoder decoder = new PngDecoder();
            decoder.SetStream(new BufferedStream(s , new byte[0]));
            decoder.ReadHeader();
            return decoder.ReadBitmap(); 
        }

        public unsafe void Save(Bitmap bmp , Stream s)
        {
            PngWriter pngw = new PngWriter(s, new ImageInfo(bmp.Width,bmp.Height,8,true,false,false));
            pngw.SetUseUnPackedMode(true);
            for ( var r = 0; r < bmp.Height; r++ )
            {
                byte[] rowB = new byte[bmp.Width*4];
                fixed ( byte* tab = rowB )
                {
                    COMP4* ptr = (COMP4*)tab;
                    for ( var c = 0; c < bmp.Width; c++ )
                    {
                        var pixel = bmp[c,r];
                        ptr->D = pixel.A;
                        ptr->A = pixel.R;
                        ptr->B = pixel.G;
                        ptr->C = pixel.B;
                        ptr++;
                    }
                }
                pngw.WriteRowByte(rowB , r);
            }
            pngw.End();
        }
    }

    internal class PngDecoder : IBitmapDecoder
    {
        BufferedStream s;
        public PngDecoder()
        {
        }
      
        public unsafe Bitmap ReadBitmap()
        {
            PngReader pngr = new PngReader(s);
            pngr.SetUnpackedMode(true);
            
            Bitmap bmp = new Bitmap(pngr.ImgInfo.Cols,pngr.ImgInfo.Rows);
            for ( int row = 0; row < pngr.ImgInfo.Rows; row++ )
            {
                ImageLine l1 = pngr.ReadRowByte(row);
                fixed ( byte* line = l1.ScanlineB )
                {
                    if ( l1.channels == 4 )
                    {
                        COMP4* ptr = (COMP4*)line;

                        for ( var c = 0; c < bmp.Width; c++ )
                        {
                            Pixel dest = Pixel.FromArgb(ptr->D,ptr->A,ptr->B,ptr->C);
                            bmp[c , row] = dest;
                            ptr++;
                        }
                    }
                    else if ( l1.channels == 3 )
                    {
                        COMP3* ptr = (COMP3*)line;
                        for ( var c = 0; c < bmp.Width; c++ )
                        {
                            Pixel dest = Pixel.FromArgb(255,ptr->A,ptr->B,ptr->C);
                            bmp[c , row] = dest;
                            ptr++;
                        }
                    }
                }
            }
            return bmp;
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
