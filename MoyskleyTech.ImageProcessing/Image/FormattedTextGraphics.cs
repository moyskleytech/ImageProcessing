using MoyskleyTech.ImageProcessing.Text;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MoyskleyTech.ImageProcessing.Image
{
    public unsafe partial class Graphics
    {
        public struct FormattedTextState
        {
            public Brush<Pixel> color;
            public int size;
            public bool bold;
            public bool italic;
        }
    }
}
