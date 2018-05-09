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
        /// <summary>
        /// Represent text state
        /// </summary>
        public struct FormattedTextState
        {
            /// <summary>
            /// Current color
            /// </summary>
            public Brush<Pixel> color;
            /// <summary>
            /// Current size
            /// </summary>
            public int size;
            /// <summary>
            /// Current bold
            /// </summary>
            public bool bold;
            /// <summary>
            /// Current italic
            /// </summary>
            public bool italic;
        }
    }
}
