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
    /// Video reader for color video
    /// </summary>
    public class ColorVideoReader
    {
        private Stream s;
        private Image<byte> r,g,b;
        /// <summary>
        /// Current frame
        /// </summary>
        public Image<BGR> Frame { get; set; }
        /// <summary>
        /// Create a reader on stream
        /// </summary>
        /// <param name="s"></param>
        public ColorVideoReader(Stream s)
        {
            this.s = s;
        }
        /// <summary>
        /// Read a frame from stream
        /// </summary>
        /// <returns></returns>
        public bool ReadFrame()
        {
            if ( r == null )
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

                Frame = Image<BGR>.Create(w , h);
                Graphics<BGR>.FromImage(Frame).Clear(Pixels.Black.ToBGR());
                r = Image<byte>.FilledWith(w,h,0);
                g = Image<byte>.FilledWith(w , h , 0);
                b = Image<byte>.FilledWith(w , h , 0);
            }
            if ( OneBandVideoReader.ReadBand(r , s) && OneBandVideoReader.ReadBand(g , s) && OneBandVideoReader.ReadBand(b , s) )
            {
                Frame.ReplaceRedBand(r);
                Frame.ReplaceGreenBand(g);
                Frame.ReplaceBlueBand(b);
                return true;
            }
            return false;
        }

    }
}
