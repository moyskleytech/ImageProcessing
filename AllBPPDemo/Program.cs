using MoyskleyTech.ImageProcessing.Image;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AllBPPDemo
{
    class Program
    {
        static void Main(string[ ] args)
        {
            Pixel[] colors = {
                Pixels.Red, Pixels.DeepPink,Pixels.Black,Pixels.Yellow,
                Pixels.LawnGreen,Pixels.DodgerBlue, Pixels.AliceBlue,Pixels.AntiqueWhite,
                Pixels.Aqua,Pixels.Aquamarine,Pixels.Azure,Pixels.Beige,
                Pixels.Bisque,Pixels.BlanchedAlmond,Pixels.Blue,Pixels.BlueViolet,
                Pixels.Brown};

            Bitmap bmp = new Bitmap(100,100);

            Graphics g = Graphics.FromImage(bmp);
            g.Clear(colors[0]);

            var codec = new BitmapCodec();

            string[] names = {"1bpp.bmp","2bpp.bmp","4bpp.bmp","8bpp.bmp","32bpp.bmp","O1bpp.bmp","O2bpp.bmp","O4bpp.bmp","O8bpp.bmp", "Written.bmp" };

            var files = (from x in names select System.IO.File.Open(x, FileMode.OpenOrCreate, FileAccess.ReadWrite, FileShare.ReadWrite)).ToArray();

            codec.Save(bmp , files[0] , CreatePalette(colors , 2));
            codec.SaveOptimized(bmp , files[5]);

            for ( var i = 1; i < colors.Length; i++ )
            {
                g.FillRectangle(colors[i] , i * 3 , 1 , 3 , 100);
                if ( i == 3 )
                {
                    codec.Save(bmp , files[1] , CreatePalette(colors , 4));
                    codec.SaveOptimized(bmp , files[6]);
                }
                else if ( i == 15 )
                {
                    codec.Save(bmp , files[2] , CreatePalette(colors , 16));
                    codec.SaveOptimized(bmp , files[7]);
                }
                else if ( i == colors.Length-1 )
                {
                    codec.Save(bmp , files[3] , CreatePalette(colors , colors.Length));
                    codec.SaveOptimized(bmp , files[8]);
                }
            }
            codec.Save(bmp , files[4]);

            files[1].Seek(0 , SeekOrigin.Begin);
            var bmp2 = codec.DecodeStream(files[1]);

            bmp2.Save(files[9]);

            //FileImageCryptor.Crypt(names[9],names[0]+".bmp");
        }

        private static dynamic CreatePalette(Pixel[ ] colors , int v)
        {
            dynamic palette = CreatePalette(v);
            for ( var i = 0; i < v; i++ )
            {
                palette[i] = colors[i];
            }
            return palette;
        }

        private static dynamic CreatePalette(int v)
        {
            if ( v == 2 )
                return new BitmapPalette1bpp();
            else if ( v == 4 )
                return new BitmapPalette2bpp();
            else if ( v == 16 )
                return new BitmapPalette4bpp();
            else
                return new BitmapPalette8bpp();
        }
    }
}
