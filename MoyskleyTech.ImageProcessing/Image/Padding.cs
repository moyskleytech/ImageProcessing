using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MoyskleyTech.ImageProcessing.Image
{
    /// <summary>
    /// Represent a padding
    /// </summary>
    public struct Padding
    {
        /// <summary>
        /// Left padding
        /// </summary>
        public int Left;
        /// <summary>
        /// Right padding
        /// </summary>
        public int Right;
        /// <summary>
        /// Top padding
        /// </summary>
        public int Top;
        /// <summary>
        /// Bottom padding
        /// </summary>
        public int Bottom;
        /// <summary>
        /// Top+Bottom
        /// </summary>
        public int Vertical { get { return Top + Bottom; } }
        /// <summary>
        /// Left+Right
        /// </summary>
        public int Horizontal { get { return Left + Right; } }

        public Padding(int val)
        {
            Left = Top = Bottom = Right = val;
        }
        public Padding(int l,int t, int b, int r)
        {
            Left = l;
            Top = t;
            Bottom = b;
            Right = r;
        }
    }
}

