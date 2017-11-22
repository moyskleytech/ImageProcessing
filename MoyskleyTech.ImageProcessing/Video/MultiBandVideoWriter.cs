using MoyskleyTech.ImageProcessing.Image;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MoyskleyTech.ImageProcessing.Video
{
    public class MultiBandVideoWriter
    {
        private Stream s;
        private OneBandImage[] bands;
        public byte Quality { get; set; }
        public MultiBandVideoWriter(Stream s,byte quality=0)
        {
            this.s = s;
            Quality = quality;
        }
        public void WriteFrame(OneBandImage[] img)
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
                bands = new OneBandImage[img.Length];
                for ( var i = 0; i < bands.Length; i++ )
                {
                    bands[i] = new OneBandImage(img[0].Width , img[0].Height);
                    bands[i].Clear(0);
                }
            }
            for ( var i = 0; i < bands.Length; i++ )
            {
                OneBandVideoWriter.WriteBand(bands[i] , img[i] , Quality , s);
            }
        }
       
    }
}
