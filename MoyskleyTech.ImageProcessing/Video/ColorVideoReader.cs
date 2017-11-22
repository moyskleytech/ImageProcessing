using MoyskleyTech.ImageProcessing.Image;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MoyskleyTech.ImageProcessing.Video
{
    public class ColorVideoReader
    {
        private Stream s;
        private OneBandImage r,g,b;
        public Bitmap Frame { get; set; }
        public ColorVideoReader(Stream s)
        {
            this.s = s;
        }

        public ColorVideoReader()
        {
        }

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

                Frame = new Bitmap(w , h);
                Graphics.FromImage(Frame).Clear(Pixels.Black);
                r = new OneBandImage(w , h);
                r.Clear(0);
                g = new OneBandImage(w , h);
                g.Clear(0);
                b = new OneBandImage(w , h);
                b.Clear(0);
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
