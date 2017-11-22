#if PCL
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ICSharpCode.SharpZipLib.VirtualFileSystem
{
    /// <summary>
    /// Virtual File System Stream
    /// </summary>
    public abstract class VfsStream : Stream
    {
        /// <summary>
        /// Name of file name
        /// </summary>
        public abstract String Name { get; }
    }

    /// <summary>
    /// Stream proxy for VFS Stream
    /// </summary>
    public class VfsProxyStream : VfsStream
    {
        String _Name;
        Stream _Stream;

        /// <summary>
        /// Base stream
        /// </summary>
        protected Stream Stream
        {
            get
            {
                if (_Stream == null) throw new ObjectDisposedException("VfsProxyStream");
                return _Stream;
            }
        }

        /// <summary>
        /// Create a new proxy stream
        /// </summary>
        public VfsProxyStream(Stream stream, String name)
        {
            if (stream == null) throw new ArgumentNullException("stream");
            this._Stream = stream;
            this._Name = name;
        }

        /// <summary>
        /// Dispose resources
        /// </summary>
        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);
            if (disposing && _Stream != null)
            {
                _Stream.Dispose();
                _Stream = null;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public override void Flush()
        {
            Stream.Flush();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="buffer"></param>
        /// <param name="offset"></param>
        /// <param name="count"></param>
        /// <returns></returns>
        public override int Read(byte[] buffer, int offset, int count)
        {
            return Stream.Read(buffer, offset, count);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="offset"></param>
        /// <param name="origin"></param>
        /// <returns></returns>
        public override long Seek(long offset, SeekOrigin origin)
        {
            return Stream.Seek(offset, origin);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="value"></param>
        public override void SetLength(long value)
        {
            Stream.SetLength(value);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="buffer"></param>
        /// <param name="offset"></param>
        /// <param name="count"></param>
        public override void Write(byte[] buffer, int offset, int count)
        {
            Stream.Write(buffer, offset, count);
        }
        
        /// <summary>
        /// Name
        /// </summary>
        public override String Name { get { return _Name; } }

        /// <summary>
        /// 
        /// </summary>
        public override bool CanRead
        {
            get { return Stream.CanRead; }
        }

        /// <summary>
        /// 
        /// </summary>
        public override bool CanSeek
        {
            get { return Stream.CanSeek; }
        }

        /// <summary>
        /// 
        /// </summary>
        public override bool CanWrite
        {
            get { return Stream.CanWrite; }
        }

        /// <summary>
        /// 
        /// </summary>
        public override long Length
        {
            get { return Stream.Length; }
        }

        /// <summary>
        /// 
        /// </summary>
        public override long Position
        {
            get { return Stream.Position; }
            set { Stream.Position = value; }
        }

    }
}
#endif
