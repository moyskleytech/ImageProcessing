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
    /// Video writer
    /// </summary>
    public class ColorVideoWriter
    {
        private Stream s;
        private OneBandImage r,g,b;
        int ct;
        /// <summary>
        /// Minimum difference to write, 0 = best quality, 255 = 1 frame only
        /// </summary>
        public byte Quality { get; set; }
        /// <summary>
        /// Create the writer
        /// </summary>
        /// <param name="s"></param>
        /// <param name="quality"></param>
        public ColorVideoWriter(Stream s , byte quality = 0)
        {
            this.s = s;
            Quality = quality;
        }
        /// <summary>
        /// Write a frame to the video
        /// </summary>
        /// <param name="img"></param>
        public void WriteFrame(Bitmap img)
        {
            if ( r == null )
            {
                s.Write(BitConverter.GetBytes(img.Width) , 0 , 4);
                s.Write(BitConverter.GetBytes(img.Height) , 0 , 4);
                r = new OneBandImage(img.Width , img.Height);
                r.Clear(0);
                g = new OneBandImage(img.Width , img.Height);
                g.Clear(0);
                b = new OneBandImage(img.Width , img.Height);
                b.Clear(0);
                ct = r.Height * r.Width;
            }
            WriteBand(r , img.GetRedBandImage() , Quality , s);
            WriteBand(g , img.GetGreenBandImage() , Quality , s);
            WriteBand(b , img.GetBlueBandImage() , Quality , s);
        }

        private void WriteBand(OneBandImage r , OneBandImage img , byte quality , Stream s)
        {
            var band=img;
            OneBandVideoWriter.WriteBand(r , img , Quality , s);
            img.Dispose();
        }

    }
}
