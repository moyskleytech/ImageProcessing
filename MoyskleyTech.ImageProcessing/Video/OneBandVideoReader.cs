using MoyskleyTech.ImageProcessing.Image;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MoyskleyTech.ImageProcessing.Video
{
    public class OneBandVideoReader
    {
        private Stream s;
        private OneBandImage f;
        public OneBandImage Frame { get; set; }
        public OneBandVideoReader(Stream s)
        {
            this.s = s;
        }
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

                Frame = new OneBandImage(w , h);
                f = new OneBandImage(w , h);
                f.Clear(0);
            }
            if ( ReadBand(f , s) )
            {
                f.CopyTo(Frame);
                return true;
            }
            return false;
        }

        public static bool ReadBand(OneBandImage f , Stream s)
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
