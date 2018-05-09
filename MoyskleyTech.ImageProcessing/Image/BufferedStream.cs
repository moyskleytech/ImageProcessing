using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MoyskleyTech.ImageProcessing.Image
{
    /// <summary>
    /// Buffered stream to read bitmap from streams
    /// </summary>
    public class BufferedStream : Stream
    {
        Stream s;
        List<byte> buffer = new List<byte>();
        List<byte> readedBuffer = new List<byte>();
        /// <summary>
        /// Base class
        /// </summary>
        public override bool CanRead
        {
            get
            {
                return true;
            }
        }
        /// <summary>
        /// Base class
        /// </summary>
        public override bool CanSeek
        {
            get
            {
                return false;
            }
        }
        /// <summary>
        /// Base class
        /// </summary>
        public override bool CanWrite
        {
            get
            {
                return false;
            }
        }
        /// <summary>
        /// Base class
        /// </summary>
        public override long Length
        {
            get
            {
                return s.Length;
            }
        }
        /// <summary>
        /// Base class
        /// </summary>
        public override long Position
        {
            get
            {
                return 0;
            }

            set
            {
                throw new NotImplementedException();
            }
        }
        /// <summary>
        /// Create from stream and current buffer
        /// </summary>
        /// <param name="s"></param>
        /// <param name="buffer"></param>
        public BufferedStream(Stream s , IEnumerable<byte> buffer)
        {
            this.s = s;
            if ( buffer != null )
                this.buffer.AddRange(buffer);
        }
        /// <summary>
        /// Read a byte from the stream
        /// </summary>
        /// <returns></returns>
        public byte Read()
        {
            if ( buffer.Count > 0 )
            {
                byte b = buffer[0];
                buffer.RemoveAt(0);
                readedBuffer.Add(b);
                return b;
            }
            var by = ( byte ) s.ReadByte();
            readedBuffer.Add(by);
            return by;
        }
        /// <summary>
        /// Rollback stream (N) bytes
        /// </summary>
        /// <param name="count"></param>
        public void Rollback(int count = 1)
        {
            if ( count == 0 )
                count = readedBuffer.Count;
            for ( var i = 0; i < count; i++ )
            {
                buffer.Insert(0 , readedBuffer[readedBuffer.Count - 1]);
                readedBuffer.RemoveAt(readedBuffer.Count - 1);
            }
        }
        /// <summary>
        /// Base class
        /// </summary>
        public override void Flush()
        {
            throw new NotImplementedException();
        }
        /// <summary>
        /// Read a lot of bytes
        /// </summary>
        /// <param name="buffer"></param>
        /// <param name="offset"></param>
        /// <param name="count"></param>
        /// <returns></returns>
        public override int Read(byte[ ] buffer , int offset , int count)
        {
            int oriCount=count,readed=0;
            bool foundEnd=false;
            while ( !foundEnd && count > 0 )
            {
                int by=0;
                if ( this.buffer.Count > 0 )
                {
                    byte b = this.buffer[0];
                    this.buffer.RemoveAt(0);
                    by = b;
                }
                else by = s.ReadByte();
                if ( by == -1 )
                    foundEnd = true;
                else
                {
                    count--;
                    buffer[offset + readed] = ( byte ) by;
                    readed++;
                    readedBuffer.Add(( byte ) by);
                }
            }
            return readed;
        }
        /// <summary>
        /// Seek position in stream
        /// </summary>
        /// <param name="offset"></param>
        /// <param name="origin"></param>
        /// <returns></returns>
        public override long Seek(long offset , SeekOrigin origin)
        {
            if ( origin == SeekOrigin.Begin )
            {
                Rollback(0);
                if ( offset > 0 )
                    while ( offset-- > 0 )
                        Read();
            }
            else if ( origin == SeekOrigin.End )
            {
                bool foundEnd=false;
                while ( !foundEnd )
                {
                    int by=0;
                    if ( this.buffer.Count > 0 )
                    {
                        byte b = this.buffer[0];
                        this.buffer.RemoveAt(0);
                        readedBuffer.Add(b);
                        by = b;
                    }
                    else by = s.ReadByte();
                    if ( by == -1 )
                        foundEnd = true;
                    else
                        readedBuffer.Add(( byte ) by);
                }
                if ( offset < 0 )
                    Rollback(( int ) -offset);
            }
            else if ( origin == SeekOrigin.Current )
            {
                if ( offset < 0 )
                    Rollback(( int ) -offset);
                else
                    while ( offset-- > 0 )
                        Read();
            }

            return Position;
        }
        /// <summary>
        /// Base class
        /// </summary>
        /// <param name="value"></param>
        public override void SetLength(long value)
        {

        }
        /// <summary>
        /// Base class
        /// </summary>
        /// <param name="buffer"></param>
        /// <param name="offset"></param>
        /// <param name="count"></param>
        public override void Write(byte[ ] buffer , int offset , int count)
        {
            throw new NotImplementedException();
        }
    }
}
