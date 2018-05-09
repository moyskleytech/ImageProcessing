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
    /// Writer for video
    /// </summary>
    public class OneBandVideoWriter
    {
        private Stream s;
        private OneBandImage f;
        int ct;
        /// <summary>
        /// Minimum difference to store in video(higher value = smaller file)
        /// </summary>
        public byte Quality { get; set; }
        /// <summary>
        /// Create a writer on the specified stream
        /// </summary>
        /// <param name="s"></param>
        /// <param name="quality"></param>
        public OneBandVideoWriter(Stream s,byte quality=0)
        {
            this.s = s;
            Quality = quality;
        }
        /// <summary>
        /// Write a frame to the monochrome video
        /// </summary>
        /// <param name="img"></param>
        public void WriteFrame(OneBandImage img)
        {
            if ( f == null )
            {
                s.Write(BitConverter.GetBytes(img.Width),0,4);
                s.Write(BitConverter.GetBytes(img.Height) , 0 , 4);
                f = new OneBandImage(img.Width , img.Height);
                f.Clear(0);
                ct = f.Height * f.Width;
            }
            WriteBand(f , img , Quality , s);
            s.Flush();
        }
        /// <summary>
        /// Write a specified band to the video
        /// </summary>
        /// <param name="previous"></param>
        /// <param name="next"></param>
        /// <param name="Quality"></param>
        /// <param name="s"></param>
        public static void WriteBand(OneBandImage previous , OneBandImage next,int Quality,Stream s)
        {
            var diff = GetDiff(previous,next,Quality);
            s.WriteByte((byte)'F');
            Write(diff,s);

            AlterImage(previous , diff);
        }
        private static void AlterImage(OneBandImage f , byte[ ] diff)
        {
            for ( var i = 0; i < diff.Length; i++ )
            {
                f[i] = ( byte ) ( ( f[i] + diff[i] ) % 256 );
            }
        }

        private static void Write(byte[ ] diff,Stream s)
        {
            byte count=0;
            for ( var i = 0; i < diff.Length; i++ )
            {
                if ( diff[i] == 0 )
                {
                    count++;
                    if ( count == 255 )
                    {
                        s.WriteByte(0);
                        s.WriteByte(255);
                        count = 0;
                    }
                }
                else
                {
                    if(count>0)
                    {
                        s.WriteByte(0);
                        s.WriteByte(count);
                        count = 0;
                    }
                    s.WriteByte(diff[i]);
                }
            }
            if ( count > 0 )
            {
                s.WriteByte(0);
                s.WriteByte(count);
                count = 0;
            }
        }
        /// <summary>
        /// Method to get the difference between 2 OneBandImage
        /// </summary>
        /// <param name="f"></param>
        /// <param name="img"></param>
        /// <param name="Quality"></param>
        /// <returns></returns>
        public static byte[] GetDiff(OneBandImage f , OneBandImage img,int Quality)
        {
            var ct = f.Width*f.Height;
            byte[] b = new byte[ct];
            for ( var i = 0; i < ct; i++ )
            {
                var diff = img[i]-f[i];
                if ( System.Math.Abs(diff) < Quality )
                    diff = 0;
                if ( diff < 0 )
                    diff += 256;
                b[i] = ( byte ) diff;
            }
            return b;
        }

    }
}
