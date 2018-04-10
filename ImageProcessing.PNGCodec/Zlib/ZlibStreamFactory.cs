using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace Hjg.Pngcs.Zlib
{
    public class ZlibStreamFactory
    {
        public static ZlibInputStreamMs createZlibInputStream(Stream st , bool leaveOpen)
        {
            return new ZlibInputStreamMs(st,leaveOpen);
        }

        public static ZlibInputStreamMs createZlibInputStream(Stream st)
        {
            return createZlibInputStream(st , false);
        }

        public static ZlibOutputStreamMs createZlibOutputStream(Stream st , int compressLevel , EDeflateCompressStrategy strat , bool leaveOpen)
        {
            return new ZlibOutputStreamMs(st , compressLevel , strat , leaveOpen);
        }

        public static ZlibOutputStreamMs createZlibOutputStream(Stream st)
        {
            return createZlibOutputStream(st , false);
        }

        public static ZlibOutputStreamMs createZlibOutputStream(Stream st , bool leaveOpen)
        {
            return createZlibOutputStream(st , DeflateCompressLevel.DEFAULT , EDeflateCompressStrategy.Default , leaveOpen);
        }
    }
}
