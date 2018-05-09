using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MoyskleyTech.ImageProcessing.Image
{
    public struct Size
    {
        public int Width,Height;
        public static Size Empty { get => new Size(0 , 0); }
        public Size(int w , int h)
        {
            Width = w;
            Height = h;
        }
    }
    public struct SizeF
    {
        public double Width,Height;
        public static SizeF Empty { get => new SizeF(0 , 0); }
        public SizeF(int w , int h)
        {
            Width = w;
            Height = h;
        }
    }
}
