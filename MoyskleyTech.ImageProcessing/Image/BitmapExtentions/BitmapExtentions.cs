using MoyskleyTech.ImageProcessing.Image.SpecializedBrushes;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace MoyskleyTech.ImageProcessing.Image.BitmapExtentions
{
    public static class BitmapExtentions
    {
        public static void Noise(this Bitmap bmp)
        {
            Graphics g = Graphics.FromImage(bmp);
            g.Clear(new NoiseBrush());
            g.Dispose();
        }

        public unsafe static BitmapPalette8bpp ReduceColor(this Bitmap bmp)
        {
            BitmapPalette8bpp palette = new BitmapPalette8bpp();
            int pos=0;
            Dictionary<Pixel,byte> positions = new Dictionary<Pixel, byte>();

            int w = bmp.Width;
            int h = bmp.Height;
            int size = w*h;
            Pixel* ptr = bmp.Source;

            for ( var i = 0; i < size; i++ )
            {
                byte index=0;
                if ( positions.ContainsKey(*ptr) )
                    index = positions[*ptr];
                else if(pos<256)
                {
                    index = (byte)pos;
                    palette[index] = *ptr;
                    positions[*ptr] = ( byte ) pos++;
                }
                *ptr++ = Pixel.FromArgb(255 , index , index , index);
            }
            return palette;
        }
        public unsafe static void SaveReducedColor(this Bitmap bmp , Stream s)
        {
            Bitmap tmp = new Bitmap(bmp.Width,bmp.Height);
            tmp.CopyFromARGB(bmp.Source);
            BitmapPalette8bpp palette = tmp.ReduceColor();
            tmp.Save8Bpp(s , palette);
            tmp.Dispose();
        }
    }
}
