using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace Hjg.Pngcs.Zlib
{
    public class ZlibStreamFactory
    {
        public static ZlibInputStreamMs CreateZlibInputStream(Stream st , bool leaveOpen)
        {
            return new ZlibInputStreamMs(st,leaveOpen);
        }

        public static ZlibInputStreamMs CreateZlibInputStream(Stream st)
        {
            return CreateZlibInputStream(st , false);
        }

        public static ZlibOutputStreamMs CreateZlibOutputStream(Stream st , int compressLevel , EDeflateCompressStrategy strat , bool leaveOpen)
        {
            return new ZlibOutputStreamMs(st , compressLevel , strat , leaveOpen);
        }

        public static ZlibOutputStreamMs CreateZlibOutputStream(Stream st)
        {
            return CreateZlibOutputStream(st , false);
        }

        public static ZlibOutputStreamMs CreateZlibOutputStream(Stream st , bool leaveOpen)
        {
            return CreateZlibOutputStream(st , DeflateCompressLevel.DEFAULT , EDeflateCompressStrategy.Default , leaveOpen);
        }
    }
}
