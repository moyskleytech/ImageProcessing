using MoyskleyTech.ImageProcessing.Image;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MoyskleyTech.ImageProcessing.Video
{
    /// <summary>
    /// Video reader for gray video
    /// </summary>
    public class OneBandVideoReader
    {
        private Stream s;
        private Image<byte> f;
        /// <summary>
        /// Current frame
        /// </summary>
        public Image<byte> Frame { get; set; }
        /// <summary>
        /// Create a reader on the stream
        /// </summary>
        /// <param name="s"></param>
        public OneBandVideoReader(Stream s)
        {
            this.s = s;
        }
        /// <summary>
        /// Read a frame from the video
        /// </summary>
        /// <returns></returns>
        public bool ReadFrame()
        {
            if ( f == null )
            {
                int b1=s.ReadByte();
                int b2=s.ReadByte();
                int b3=s.ReadByte();
                int b4=s.ReadByte();

                if ( b1 == -1 || b2 == -1 || b3 == -1 || b4 == -1 )
                    return false;
                int w = BitConverter.ToInt32(new byte[ ] { (byte)b1 , ( byte ) b2 , ( byte ) b3 , ( byte ) b4 } , 0);
                b1 = s.ReadByte();
                b2 = s.ReadByte();
                b3 = s.ReadByte();
                b4 = s.ReadByte();

                if ( b1 == -1 || b2 == -1 || b3 == -1 || b4 == -1 )
                    return false;
                int h = BitConverter.ToInt32(new byte[ ] { (byte)b1 , ( byte ) b2 , ( byte ) b3 , ( byte ) b4 } , 0);

                Frame = new Image<byte>(w , h);
                f = Image<byte>.FilledWith(w , h,0);
            }
            if ( ReadBand(f , s) )
            {
                f.CopyTo(Frame.DataPointer);
                return true;
            }
            return false;
        }
        /// <summary>
        /// Method to read one band in the stream
        /// </summary>
        /// <param name="f">Band to read</param>
        /// <param name="s">SourceStream</param>
        /// <returns>Complete frame(false = error)</returns>
        public static bool ReadBand(Image<byte> f , Stream s)
        {
            var pt = f.Width*f.Height;
            var octet = s.ReadByte();
            if ( octet != ( byte ) 'F' )
                return false;
            for ( var i = 0; i < pt; i++ )
            {
                octet = s.ReadByte();
                if ( octet == -1 )
                    return false;
                if ( octet == 0 )
                {
                    octet = s.ReadByte();
                    if ( octet == -1 )
                        return false;
                    i += octet - 1;
                }
                else
                {
                    var nv = (f[i] + octet)%256;

                    f[i] = ( byte ) nv;
                }

            }
            return true;
        }
    }
}
