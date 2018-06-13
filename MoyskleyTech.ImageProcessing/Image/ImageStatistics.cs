using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MoyskleyTech.ImageProcessing.Image
{
    /// <summary>
    /// Represent all statistics relative to an image
    /// </summary>
    public class ImageStatistics
    {
        /// <summary>
        /// Red band
        /// </summary>
        public BandStatistics Red { get; set; }
        /// <summary>
        /// Green band
        /// </summary>
        public BandStatistics Green { get; set; }
        /// <summary>
        /// Blue band
        /// </summary>
        public BandStatistics Blue { get; set; }
        /// <summary>
        /// Alpha band
        /// </summary>
        public BandStatistics Alpha { get; set; }
        /// <summary>
        /// Gray band
        /// </summary>
        public BandStatistics GrayTone { get; set; }
        /// <summary>
        /// Hue band
        /// </summary>
        public BandStatistics Hue { get; set; }
        /// <summary>
        /// Saturation band
        /// </summary>
        public BandStatistics Saturation { get; set; }
        /// <summary>
        /// Brightness band
        /// </summary>
        public BandStatistics Brightness { get; set; }
        /// <summary>
        /// Statistics on color
        /// </summary>
        public ColorStatistics ColorStatistics { get; set; }
        /// <summary>
        /// Create the statistics from imageproxy
        /// </summary>
        /// <param name="bmp"></param>
        public ImageStatistics(ImageProxy<Pixel> bmp)
        {
            int ct = bmp.Width*bmp.Height;
            var numberSelector = from x in Enumerable.Range(0 , ct) select x;
            Alpha = new BandStatistics(from x in numberSelector select bmp[x].A , Band.Alpha);
            Red = new BandStatistics(from x in numberSelector select bmp[x].R , Band.Red);
            Green = new BandStatistics(from x in numberSelector select bmp[x].G , Band.Green);
            Blue = new BandStatistics(from x in numberSelector select bmp[x].B , Band.Blue);
            GrayTone = new BandStatistics(from x in numberSelector select bmp[x].GetGrayTone() , Band.Gray);

            ColorStatistics = new ColorStatistics(from x in numberSelector select bmp[x]);

            var tbmp = bmp.ToImage();
            var hsb = tbmp.ConvertTo<HSB>();
            tbmp.Dispose();
            Hue = new BandStatistics(from x in numberSelector select hsb[x].H , Band.Hue);
            Saturation = new BandStatistics(from x in numberSelector select hsb[x].S , Band.Saturation);
            Brightness = new BandStatistics(from x in numberSelector select hsb[x].B , Band.Brightness);
            hsb.Dispose();
        }
        /// <summary>
        /// Create the statistics from imageproxy
        /// </summary>
        /// <param name="bmp"></param>
        public ImageStatistics(ImageProxy<HSB> hsb)
        {
            int ct = hsb.Width*hsb.Height;

            var bmp = hsb.As<Pixel>();

            var numberSelector = from x in Enumerable.Range(0 , ct) select x;
            Alpha = new BandStatistics(from x in numberSelector select bmp[x].A , Band.Alpha);
            Red = new BandStatistics(from x in numberSelector select bmp[x].R , Band.Red);
            Green = new BandStatistics(from x in numberSelector select bmp[x].G , Band.Green);
            Blue = new BandStatistics(from x in numberSelector select bmp[x].B , Band.Blue);
            GrayTone = new BandStatistics(from x in numberSelector select bmp[x].GetGrayTone() , Band.Gray);

            ColorStatistics = new ColorStatistics(from x in numberSelector select bmp[x]);

            Hue = new BandStatistics(from x in numberSelector select hsb[x].H , Band.Hue);
            Saturation = new BandStatistics(from x in numberSelector select hsb[x].S , Band.Saturation);
            Brightness = new BandStatistics(from x in numberSelector select hsb[x].B , Band.Brightness);
        }
    }
    /// <summary>
    /// Represent the color statistics
    /// </summary>
    public class ColorStatistics
    {
        /// <summary>
        /// Dictionnary of each pixel and their quantity
        /// </summary>
        public Dictionary<Pixel , int> Dominance { get; set; } = new Dictionary<Pixel , int>();
        /// <summary>
        /// Create statistics from enumerable
        /// </summary>
        /// <param name="enumerable"></param>
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
    /// <summary>
    /// Image statistic over one band
    /// </summary>
    public class BandStatistics
    {
        /// <summary>
        /// Name of band
        /// </summary>
        public Band Band { get; set; }
        /// <summary>
        /// Create statistics from enumerator of values in band
        /// </summary>
        /// <param name="selector"></param>
        /// <param name="band"></param>
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
        /// <summary>
        /// Minimum of band
        /// </summary>
        public byte Min { get; set; }
        /// <summary>
        /// Maximum of band
        /// </summary>
        public byte Max { get; set; }
        /// <summary>
        /// Average of band
        /// </summary>
        public byte Average { get; set; }
        /// <summary>
        /// Histogram of band
        /// </summary>
        public HistogramStatistic Histogram { get; set; }
    }
    /// <summary>
    /// Represent an histogram of a band
    /// </summary>
    public class HistogramStatistic
    {
        /// <summary>
        /// Band name
        /// </summary>
        public Band Band { get; set; }
        /// <summary>
        /// Create an histograms
        /// </summary>
        /// <param name="b"></param>
        public HistogramStatistic(Band b)
        {
            Band = b;
        }
        int count=0;
        private int[] vals = new int[256];
        /// <summary>
        /// Append a value in the histogram
        /// </summary>
        /// <param name="b"></param>
        public void Append(byte b)
        {
            vals[b]++;
            count++;
        }
        /// <summary>
        /// Get the max count in 0-1 value
        /// </summary>
        public float MaxNormalizedCount { get { return NormalizedCount.Max(); } }
        /// <summary>
        /// Get the count in 0-1 value
        /// </summary>
        public float[ ] NormalizedCount { get { return ( from x in vals select ( float ) x / count ).ToArray(); } }
        /// <summary>
        /// Get the count of each value
        /// </summary>
        public int[ ] Count { get { return ( from x in vals select x ).ToArray(); } }
        /// <summary>
        /// Get the max value
        /// </summary>
        public int DominantValue
        {
            get
            {
                int i=0;
                return ( from x in vals select new { val = x , pos = i++ } ).ToArray().OrderByDescending((x) => x.val).First().pos;
            }
        }
        /// <summary>
        /// Draw the histogram in a graphics
        /// </summary>
        /// <param name="g"></param>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <param name="w"></param>
        /// <param name="h"></param>
        /// <param name="background"></param>
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
        /// <summary>
        /// Render the histogram in a bitmap
        /// </summary>
        /// <param name="w"></param>
        /// <param name="h"></param>
        /// <returns></returns>
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
    /// <summary>
    /// Enumeration of all possible Band in statistics
    /// </summary>
    public enum Band
    {
#pragma warning disable CS1591
        Red, Green, Blue, Hue, Saturation, Brightness, Alpha, Gray
#pragma warning restore CS1591
    }
}
