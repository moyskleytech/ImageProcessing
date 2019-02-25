using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace MoyskleyTech.ImageProcessing.Image
{
    public unsafe class PackedBoolImage : Image<bool>
    {
        byte* bytePtr;
        int lengthPacked;
        /// <summary>
        /// Create a bitmap using Width and Height
        /// </summary>
        /// <param name="w">Width</param>
        /// <param name="h">Height</param>
        public PackedBoolImage(int w , int h) : base(IntPtr.Zero,w , h,true)
        {
            var length = w*h;
            lengthPacked = (int)Math.Ceiling(length/8d);
            raw = Marshal.AllocHGlobal(lengthPacked);
            bytePtr = (byte*)raw.ToPointer();
            width = w;
            height = h;
        }
        /// <summary>
        /// Create a bitmap using Width and Height
        /// </summary>
        /// <param name="w">Width</param>
        /// <param name="h">Height</param>
        public PackedBoolImage(IntPtr source,int w , int h) : base(source , w , h )
        {
            var length = w*h;
            lengthPacked = ( int ) Math.Ceiling(length / 8d);
            bytePtr = ( byte* ) raw.ToPointer();
            width = w;
            height = h;
            if(source!=IntPtr.Zero)
                CopyFrom(source);
        }
        public override bool this[int pos]
        {
            get
            {
                var loc = pos/8;
                var mod = pos-loc*8;
                var b = bytePtr[loc];
                return (( b >> mod ) & 0b1)==1?true:false;
            }
            set
            {
                var loc = pos/8;
                var mod = pos-loc*8;
                var mask = 1<< mod;
                var b = bytePtr[loc];
                if ( value )
                    b = (byte) (b | mask);
                else
                    b = ( byte ) (b & ~mask);
                bytePtr[loc] = b;
            }
        }
        public override Image<bool> Clone()
        {
            PackedBoolImage pbi = new PackedBoolImage(width,height);
            pbi.CopyFrom(bytePtr);
            return pbi;
        }
        public override ref bool GetRef(int i)
        {
            throw new InvalidOperationException();
        }
        public override ref bool GetRef(int x , int y)
        {
            throw new InvalidOperationException();
        }
        public override unsafe void CopyFrom(void* dst)
        {
            byte* s=(byte*)dst;
            byte* d = bytePtr;
            for ( var i = 0; i < lengthPacked; i++ )
                *d++ = *s++;
        }
        public override unsafe void CopyTo(void* dst)
        {
            byte* s=bytePtr;
            byte* d = (byte*)dst;
            for ( var i = 0; i < lengthPacked; i++ )
                *d++ = *s++;
        }
        public override bool Average(double x , double y , double w , double h)
        {
            var sum=0;
            for ( var xo = 0; xo < w; xo++ )
                for ( var yo = 0; yo < h; yo++ )
                    sum += this[xo + (int)x , yo + ( int ) y] ? 1 : 0;
            return sum > w * h / 2;
        }
        public override bool Get(double x , double y)
        {
            return this[(int)x,(int)y];
        }
    }
}
