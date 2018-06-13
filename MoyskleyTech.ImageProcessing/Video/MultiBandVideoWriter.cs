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
    /// Writer for multiband video
    /// </summary>
    public class MultiBandVideoWriter
    {
        private Stream s;
        private Image<byte>[] bands;
        /// <summary>
        /// Minimum difference to write
        /// </summary>
        public byte Quality { get; set; }
        /// <summary>
        /// Create a writer for a multiband video
        /// </summary>
        /// <param name="s"></param>
        /// <param name="quality"></param>
        public MultiBandVideoWriter(Stream s,byte quality=0)
        {
            this.s = s;
            Quality = quality;
        }
        /// <summary>
        /// Write a frame to the stream
        /// </summary>
        /// <param name="img"></param>
        public void WriteFrame(Image<byte>[ ] img)
        {
            if ( bands == null )
            {
                if ( img.Length > 0 )
                {
                    s.Write(BitConverter.GetBytes(img[0].Width) , 0 , 4);
                    s.Write(BitConverter.GetBytes(img[0].Height) , 0 , 4);
                }
                else
                    s.Write(new byte[ ] { 0 , 0 , 0 , 0 , 0 , 0 , 0 , 0 } , 0 , 8);
                s.Write(BitConverter.GetBytes(img.Length) , 0 , 4);
                bands = new Image<byte>[img.Length];
                for ( var i = 0; i < bands.Length; i++ )
                {
                    bands[i] = Image<byte>.FilledWith(img[0].Width , img[0].Height,0);
                }
            }
            for ( var i = 0; i < bands.Length; i++ )
            {
                OneBandVideoWriter.WriteBand(bands[i] , img[i] , Quality , s);
            }
        }
       
    }
}
