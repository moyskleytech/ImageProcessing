namespace Hjg.Pngcs {

    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.IO;
    using System.Linq;
    using System.Runtime.CompilerServices;

    /// <summary>
    /// stream that outputs to memory and allows to flush fragments every 'size'
    /// bytes to some other destination
    /// </summary>
    ///
    abstract internal class ProgressiveOutputStream:Stream  {
        private readonly int size;
        private long countFlushed = 0;
        private List<byte> buffer=new List<byte>();

        public override bool CanRead
        {
            get
            {
                return false;
            }
        }

        public override bool CanSeek
        {
            get
            {
                return false;
            }
        }

        public override bool CanWrite
        {
            get
            {
                return true;
            }
        }

        public override long Length
        {
            get
            {
                return buffer.Count;
            }
        }

        public override long Position
        {
            get; set;
        } = 0;

        public ProgressiveOutputStream(int size_0) {
            this.size = size_0;
            if (size < 8) throw new PngjException("bad size for ProgressiveOutputStream: " + size);
        }
        
        public virtual void Close() {
            Flush();
        }

        public override void Flush() {
            CheckFlushBuffer(true);
        }

        public override void Write(byte[] b, int off, int len) {
            buffer.AddRange(b.Skip(off).Take(len));
            Position += len;
            CheckFlushBuffer(false);
        }

        public void Write(byte[] b) {
            Write(b, 0, b.Length);
            CheckFlushBuffer(false);
        }


        /// <summary>
        /// if it's time to flush data (or if forced==true) calls abstract method
        /// flushBuffer() and cleans those bytes from own buffer
        /// </summary>
        ///
        private void CheckFlushBuffer(bool forced) {
            int count = (int)Position;
            var buf = buffer;
            while (forced || count >= size) {
                int nb = size;
                if (nb > count)
                    nb = count;
                if (nb == 0)
                    return;
                FlushBuffer(buf.Take(nb).ToArray(), nb);
                countFlushed += nb;
                int bytesleft = count - nb;
                count = bytesleft;
                Position = count;
                buffer.RemoveRange(0 , nb);
            }
        }
        
        protected abstract void FlushBuffer(byte[] b, int n);

        public long GetCountFlushed() {
            return countFlushed;
        }

        public override int Read(byte[ ] buffer , int offset , int count)
        {
            throw new NotImplementedException();
        }

        public override long Seek(long offset , SeekOrigin origin)
        {
            throw new NotImplementedException();
        }

        public override void SetLength(long value)
        {
            if ( value < buffer.Count )
                buffer = buffer.Take(( int ) value).ToList();
        }
    }
}
