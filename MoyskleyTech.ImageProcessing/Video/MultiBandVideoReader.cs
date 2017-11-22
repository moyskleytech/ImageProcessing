using MoyskleyTech.ImageProcessing.Image;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MoyskleyTech.ImageProcessing.Video
{
    public class MultiBandVideoReader
    {
        private Stream s;
        private OneBandImage[] bands;
        public OneBandImage[] Frame { get; set; }
        public MultiBandVideoReader(Stream s)
        {
            this.s = s;
        }
        public bool ReadFrame()
        {
            if ( bands == null )
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

                b1 = s.ReadByte();
                b2 = s.ReadByte();
                b3 = s.ReadByte();
                b4 = s.ReadByte();

                if ( b1 == -1 || b2 == -1 || b3 == -1 || b4 == -1 )
                    return false;
                int b = BitConverter.ToInt32(new byte[ ] { (byte)b1 , ( byte ) b2 , ( byte ) b3 , ( byte ) b4 } , 0);

                Frame = new OneBandImage[b];
                bands = new OneBandImage[b];
                for ( var i = 0; i < b; i++ )
                {
                    var band=new OneBandImage(w , h);
                    band.Clear(0);
                    bands[i] = band;
                    Frame[i] = new OneBandImage(w , h);
                }
            }
            for ( var i = 0; i < bands.Length; i++ )
            {
                if ( !OneBandVideoReader.ReadBand(bands[i] , s) )
                    return false;
                bands[i].CopyTo(Frame[i]);
            }
            return true;
        }

    }
}
