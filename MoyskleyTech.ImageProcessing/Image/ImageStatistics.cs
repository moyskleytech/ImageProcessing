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
        public ColorStatistics ColorStatistics { get; set; }
        public ImageStatistics(ImageProxy bmp)
        {
            int ct = bmp.Width*bmp.Height;
            var numberSelector = from x in Enumerable.Range(0 , ct) select x;
            Alpha = new BandStatistics(from x in numberSelector select bmp[x].A , Band.Alpha);
            Red = new BandStatistics(from x in numberSelector select bmp[x].R , Band.Red);
            Green = new BandStatistics(from x in numberSelector select bmp[x].G , Band.Green);
            Blue = new BandStatistics(from x in numberSelector select bmp[x].B , Band.Blue);

            ColorStatistics = new ColorStatistics(from x in numberSelector select bmp[x]);

            var tbmp = bmp.ToBitmap();
            var hsb = tbmp.ToHSB();
            tbmp.Dispose();
            Hue = new BandStatistics(from x in numberSelector select hsb[x].H , Band.Hue);
            Saturation = new BandStatistics(from x in numberSelector select hsb[x].S , Band.Saturation);
            Brightness = new BandStatistics(from x in numberSelector select hsb[x].B , Band.Brightness);
            hsb.Dispose();
           
        }
    }
    public class ColorStatistics
    {
        public Dictionary<Pixel , int> Dominance { get; set; } = new Dictionary<Pixel , int>();
        public ColorStatistics(IEnumerable<Pixel> enumerable)
        {
            foreach ( var px in enumerable )
            {
                if ( Dominance.ContainsKey(px) )
                    Dominance[px]++;
                else
                    Dominance[px] = 1;
            }
        }
    }
    public class BandStatistics
    {
        public Band Band { get; set; }
        public BandStatistics(IEnumerable<byte> selector , Band band)
        {
            this.Band = band;
            Min = byte.MaxValue;
            Max = byte.MinValue;
            int count=0;
            double sum=0;
            Histogram = new HistogramStatistic(band);
            foreach ( var b in selector )
            {
                if ( b < Max )
                    Max = b;
                if ( b > Min )
                    Min = b;
                sum += b;
                count++;
                Histogram.Append(b);
            }
            Average = ( byte ) ( sum / count );
        }
        public byte Min { get; set; }
        public byte Max { get; set; }
        public byte Average { get; set; }
        public HistogramStatistic Histogram { get; set; }
    }
    public class HistogramStatistic
    {
        public Band Band { get; set; }
        public HistogramStatistic(Band b)
        {
            Band = b;
        }
        int count=0;
        private int[] vals = new int[256];
        public void Append(byte b)
        {
            vals[b]++;
            count++;
        }
        public float MaxNormalizedCount { get { return NormalizedCount.Max(); } }
        public float[ ] NormalizedCount { get { return ( from x in vals select ( float ) x / count ).ToArray(); } }
        public int[ ] Count { get { return ( from x in vals select x ).ToArray(); } }
        public int DominantValue
        {
            get
            {
                int i=0;
                return ( from x in vals select new { val = x , pos = i } ).ToArray().OrderByDescending((x) => x.val).First().pos;
            }
        }
        public void Render(Graphics g , int x = 0 , int y = 0 , int w = 256 , int h = 256 , Pixel? background = null)
        {
            g.FillRectangle(background ?? Pixels.Transparent , x , y , w , h);
            Brush b=null;
            if ( Band == Band.Hue )
            {
                var stops = new LinearMultiGradientBrush.GradientStop[256];

                for ( var i = 0; i < 256; i++ )
                {
                    stops[i] = new LinearMultiGradientBrush.GradientStop()
                    {
                        Color = HSB.FromHSB(i , 255 , 128).ToRGB() ,
                        Weigth = i / 256d
                    };
                }

                LinearMultiGradientBrush hueBrush = new LinearMultiGradientBrush(new Point(x,y),new Point(x+w,y),stops);
                b = hueBrush;
            }
            else if ( Band == Band.Red )
            {
                b = new SolidBrush(Pixels.Red);
            }
            else if ( Band == Band.Green )
            {
                b = new SolidBrush(Pixels.LawnGreen);
            }
            else if ( Band == Band.Blue )
            {
                b = new SolidBrush(Pixels.DodgerBlue);
            }
            else if ( Band == Band.Saturation )
            {
                b = new SolidBrush(Pixels.DeepPink);
            }
            else
            {
                b = new SolidBrush(Pixels.Black);
            }
            var values = NormalizedCount;
            for ( var i = x; i < x + w; i++ )
            {
                var v = (byte)((i-x)*256/w);
                var lineHeight = values[v]*h/values.Max();
                var top = y+h-lineHeight;
                var bottom = y+h;

                g.DrawLine(b , i , top , i , bottom);
            }
        }
        public Bitmap Render(int w = 256 , int h = 256)
        {
            Bitmap bmp = new Bitmap(w,h);
            var g=Graphics.FromImage(bmp);
            g.Clear(Pixels.Transparent);
            Render(g , 0 , 0 , w , h);
            g.Dispose();
            return bmp;
        }
    }
    public enum Band
    {
        Red, Green, Blue, Hue, Saturation, Brightness, Alpha
    }
}
