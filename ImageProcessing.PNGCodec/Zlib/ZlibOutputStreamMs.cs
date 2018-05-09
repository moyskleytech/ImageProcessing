using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.IO.Compression;
// ONLY FOR .NET 4.5
namespace Hjg.Pngcs.Zlib {


   public class ZlibOutputStreamMs : AZlibOutputStream {

        public ZlibOutputStreamMs(Stream st, int compressLevel, EDeflateCompressStrategy strat, bool leaveOpen) : base(st, compressLevel, strat, leaveOpen){ 
        }

       private DeflateStream deflateStream; // lazily created, if real read/write is called
        private Adler32 adler32 = new Adler32();
        private bool initdone = false;
        private bool closed = false;

        public override void WriteByte(byte value) {
            if (!initdone) DoInit();
            if (deflateStream == null) InitStream();
            base.WriteByte(value);
            adler32.Update(value);
        }

        public override void Write(byte[] array, int offset, int count) {
            if (count == 0) return;
            if (!initdone) DoInit();
            if (deflateStream == null) InitStream();
            deflateStream.Write(array, offset, count);
            adler32.Update(array, offset, count);
        }

        public void Close() {
            if (!initdone) DoInit(); // can happen if never called write
            if (closed) return;
            closed = true;
            // sigh ... no only must I close the parent stream to force a flush, but I must save a reference
            // raw stream because (apparently) Close() sets it to null (shame on you, MS developers)
            deflateStream?.Flush();
            deflateStream?.Dispose();
            if (deflateStream == null) {
                rawStream.WriteByte(3);
                rawStream.WriteByte(0);
            }
            // add crc
            uint crcv = adler32.GetValue();
            rawStream.WriteByte((byte)((crcv >> 24) & 0xFF));
            rawStream.WriteByte((byte)((crcv >> 16) & 0xFF));
            rawStream.WriteByte((byte)((crcv >> 8) & 0xFF));
            rawStream.WriteByte((byte)((crcv) & 0xFF));
            if ( !leaveOpen )
                rawStream.Dispose();
        }

        private void InitStream() {
            if (deflateStream != null) return;
            // I must create the DeflateStream only if necessary, because of its bug with empty input (sigh)
            // I must create with leaveopen=true always and do the closing myself, because MS moronic implementation of DeflateStream: I cant force a flush of the underlying stream witouth closing (sigh bis)
           System.IO.Compression. CompressionLevel clevel = CompressionLevel.Optimal;
            // thaks for the granularity, MS!
           if (compressLevel >= 1 && compressLevel <= 5) clevel = CompressionLevel.Fastest;
           else if (compressLevel == 0) clevel = CompressionLevel.NoCompression;
            deflateStream = new DeflateStream(rawStream, clevel, true);
        }

        private void DoInit() {
            if (initdone) return;
            initdone = true;
             // http://stackoverflow.com/a/2331025/277304
                int cmf = 0x78;
                int flg = 218;  // sorry about the following lines
                if (compressLevel >= 5 && compressLevel <= 6) flg = 156;
                else if (compressLevel >= 3 && compressLevel <= 4) flg = 94;
                else if (compressLevel <= 2) flg = 1;
                flg -= ((cmf * 256 + flg) % 31); // just in case
                if (flg < 0) flg += 31;
                rawStream.WriteByte((byte)cmf);
                rawStream.WriteByte((byte)flg);
            
        }

        public override void Flush() {
            deflateStream?.Flush();
        }

        public override String getImplementationId() {
            return "Zlib deflater: .Net CLR 4.5";
        }

    }
}
