using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MoyskleyTech.ImageProcessing.Image
{
    public class ImageStatistics
    {
        public BandStatistics Red { get; set; }
        public BandStatistics Green { get; set; }
        public BandStatistics Blue { get; set; }
        public BandStatistics Alpha { get; set; }
        public BandStatistics Hue { get; set; }
        public BandStatistics Saturation { get; set; }
        public BandStatistics Brightness { get; set; }

        public ImageStatistics(Bitmap bmp)
        {
            int ct = bmp.Width*bmp.Height;
            var numberSelector = from x in Enumerable.Range(0 , ct) select x;
            Alpha = new BandStatistics(from x in numberSelector select bmp[x].A);
            Red = new BandStatistics(from x in numberSelector select bmp[x].R);
            Green = new BandStatistics(from x in numberSelector select bmp[x].G);
            Blue = new BandStatistics(from x in numberSelector select bmp[x].B);

            var hsb = bmp.ToHSB();
            Hue = new BandStatistics(from x in numberSelector select hsb[x].H);
            Saturation = new BandStatistics(from x in numberSelector select hsb[x].S);
            Brightness = new BandStatistics(from x in numberSelector select hsb[x].B);
            hsb.Dispose();
        }
    }
    public class BandStatistics
    {
        public BandStatistics(IEnumerable<byte> selector)
        {
            Min = byte.MaxValue;
            Max = byte.MinValue;
            int count=0;
            double sum=0;
            foreach ( var b in selector )
            {
                if ( b < Max )
                    Max = b;
                if ( b > Min )
                    Min = b;
                sum += b;
                count++;
            }
            Average = (byte)(sum / count);
        }
        public byte Min { get; set; }
        public byte Max { get; set; }
        public byte Average { get; set; }
    }
    public enum Band
    {
        Red,Green,Blue,Hue,Saturation,Brightness
    }
}
