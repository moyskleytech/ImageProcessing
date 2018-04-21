using MoyskleyTech.ImageProcessing.Image;
using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using MoyskleyTech.Mathematics;
namespace MoyskleyTech.ImageProcessing.Filters
{
    public static class ColorReducer
    {
        public static double FromStatistics(ImageProxy bmp , int colorCount)
        {
            var statistics = new ImageStatistics(bmp);
            var colors = statistics.ColorStatistics.Dominance.OrderByDescending((x) => x.Value).Take(colorCount).Select((x)=>x.Key).ToArray() ;
            int getDistance(Pixel a , Pixel b)
            {
                return System.Math.Abs(a.R - b.R) + System.Math.Abs(a.G - b.G) + System.Math.Abs(a.B - b.B);
            }
            Pixel getClosest(Pixel inp)
            {
                Pixel output = Pixels.Black;
                int distance=int.MaxValue;
                foreach ( var c in colors )
                {
                    var d = getDistance(inp,c);
                    if ( d < distance )
                    {
                        distance = d;
                        output = c;
                    }
                    if ( d == 0 )
                        break;
                }
                return output;
            }

            for ( var x = 0; x < bmp.Width; x++ )
                for ( var y = 0; y < bmp.Height; y++ )
                    bmp[x , y] = getClosest(bmp[x , y]);


            double getRepresentation()
            {
                double max = bmp.Width*bmp.Height;
                double representedExactly = (from x in colors select statistics.ColorStatistics.Dominance[x]).Sum();
                return representedExactly / max;
            }
            return getRepresentation();
        }
        public static double PreviewFromStatistics(ImageProxy bmp , int colorCount)
        {
            var statistics = new ImageStatistics(bmp);
            var colors = statistics.ColorStatistics.Dominance.OrderByDescending((x) => x.Value).Take(colorCount).Select((x)=>x.Key).ToArray() ;
            double getRepresentation()
            {
                double max = bmp.Width*bmp.Height;
                double representedExactly = (from x in colors select statistics.ColorStatistics.Dominance[x]).Sum();
                return representedExactly / max;
            }
            return getRepresentation();
        }
        public static IEnumerable<double> PreviewFromStatistics(ImageProxy bmp)
        {
            var statistics = new ImageStatistics(bmp);
            double max = bmp.Width*bmp.Height;
            double actual=0;
            var colors = statistics.ColorStatistics.Dominance.OrderByDescending((x) => x.Value).GetEnumerator();

            while ( colors.MoveNext() )
            {
                actual += colors.Current.Value;
                yield return actual / max;
            }
        }
        private static Func<Pixel[ ]> GetBeginningMean(int count , ImageStatistics stats)
        {
            Random r = new Random();
            return () =>
            {
                var dom = stats.ColorStatistics.Dominance;
                var range=dom.Count;
                return ( from x in Enumerable.Range(0 , count) select dom.ElementAt(range / count * x).Key ).ToArray();

                //return (from x in stats.ColorStatistics.Dominance orderby r.Next() select x.Key).Take(count).ToArray();
            };
        }
        private static Func<HSB[ ]> GetBeginningMeanHSB(int count , ImageStatistics stats)
        {
            Random r = new Random();
            return () =>
            {
                return GetBeginningMean(count , stats)().Select((x) => x.ToHSB()).ToArray();
            };
        }
        public static List<Pixel> FromKMeans(ImageProxy bmp , int colorCount, Func<Pixel , Pixel , double> getDistance=null)
        {
            var statistics = new ImageStatistics(bmp);
            IEnumerable<Pixel> colors=null;
            if ( getDistance == null )
                getDistance = RGBDistanceBasic;
            if ( statistics.ColorStatistics.Dominance.Count > colorCount )
            {
                Random r = new Random();

                Pixel getMean(IEnumerable<Pixel> inp)
                {
                    if ( inp.Any() )
                    {
                        double avgR = inp.Average((x) => x.R);
                        double avgG = inp.Average((x) => x.G);
                        double avgB = inp.Average((x) => x.B);
                        return Pixel.FromArgb(255 , ( byte ) avgR , ( byte ) avgG , ( byte ) avgB);
                    }
                    return Pixels.DeepPink;
                }
                //var colors = statistics.ColorStatistics.Dominance.OrderByDescending((x) => x.Value).Take(colorCount).Select((x)=>x.Key).ToArray() ;
                KMeans<Pixel> p = new KMeans<Pixel>(colorCount,GetBeginningMean(colorCount,statistics),getDistance,getMean,
                statistics.ColorStatistics.Dominance.SelectMany((x)=>
                from n in Enumerable.Range(1,x.Value) select x.Key)
                ,100);
                colors = p.Means;
            }
            else
                colors = from x in statistics.ColorStatistics.Dominance select x.Key;
            Pixel getClosest(Pixel inp)
            {
                Pixel output = Pixels.Black;
                double distance=double.MaxValue;
                foreach ( var c in colors )
                {
                    var d = getDistance(inp,c);
                    if ( d < distance )
                    {
                        distance = d;
                        output = c;
                    }
                    if ( d == 0 )
                        break;
                }
                return output;
            }

            for ( var x = 0; x < bmp.Width; x++ )
                for ( var y = 0; y < bmp.Height; y++ )
                    bmp[x , y] = getClosest(bmp[x , y]);

            return colors.ToList();
        }
        public static List<Pixel> FromKMeansHSB(ImageProxy bmp , int colorCount , Func<HSB , HSB , double> getDistance = null)
        {
            var statistics = new ImageStatistics(bmp);
            IEnumerable<HSB> colors=null;
            if ( getDistance == null )
                getDistance = HSBDistanceBasic;
            if ( statistics.ColorStatistics.Dominance.Count > colorCount )
            {
                Random r = new Random();

                HSB getMean(IEnumerable<HSB> inp)
                {
                    if ( inp.Any() )
                    {
                        double avgR = inp.Average((x) => x.H);
                        double avgG = inp.Average((x) => x.S);
                        double avgB = inp.Average((x) => x.B);
                        return HSB.FromHSB(( byte ) avgR , ( byte ) avgG , ( byte ) avgB);
                    }
                    return HSB.FromHSB(128 , 0 , 0);
                }
                //var colors = statistics.ColorStatistics.Dominance.OrderByDescending((x) => x.Value).Take(colorCount).Select((x)=>x.Key).ToArray() ;
                KMeans<HSB> p = new KMeans<HSB>(colorCount,GetBeginningMeanHSB(colorCount,statistics),getDistance,getMean,
                statistics.ColorStatistics.Dominance.SelectMany((x)=>
                from n in Enumerable.Range(1,x.Value) select x.Key.ToHSB())
                ,100);
                colors = p.Means;
            }
            else
                colors = from x in statistics.ColorStatistics.Dominance select x.Key.ToHSB();
            Pixel getClosest(Pixel inp)
            {
                Pixel output = Pixels.Black;
                double distance=double.MaxValue;
                foreach ( var c in colors )
                {
                    var d = getDistance(inp.ToHSB(),c);
                    if ( d < distance )
                    {
                        distance = d;
                        output = c.ToRGB();
                    }
                    if ( d == 0 )
                        break;
                }
                return output;
            }

            for ( var x = 0; x < bmp.Width; x++ )
                for ( var y = 0; y < bmp.Height; y++ )
                    bmp[x , y] = getClosest(bmp[x , y]);

            return colors.Select((x) => x.ToRGB()).ToList();
        }

        public static void FromXMeans(ImageProxy bmp , int colorCount , int maxDistance = 50 , Func<Pixel , Pixel , double> getDistance = null)
        {
            var statistics = new ImageStatistics(bmp);
            IEnumerable<Pixel> colors=null;
            if ( getDistance == null )
                getDistance = RGBDistanceBasic;
            if ( statistics.ColorStatistics.Dominance.Count > colorCount )
            {
                Random r = new Random();

                Pixel getMean(IEnumerable<Pixel> inp)
                {
                    if ( inp.Any() )
                    {
                        double avgR = inp.Average((x) => x.R);
                        double avgG = inp.Average((x) => x.G);
                        double avgB = inp.Average((x) => x.B);
                        return Pixel.FromArgb(255 , ( byte ) avgR , ( byte ) avgG , ( byte ) avgB);
                    }
                    return Pixels.DeepPink;
                }
                //var colors = statistics.ColorStatistics.Dominance.OrderByDescending((x) => x.Value).Take(colorCount).Select((x)=>x.Key).ToArray() ;
                XMeansDistanceBased<Pixel> p = new XMeansDistanceBased<Pixel>(colorCount,GetBeginningMean(colorCount,statistics),getDistance,getMean,
                statistics.ColorStatistics.Dominance.SelectMany((x)=>
                from n in Enumerable.Range(1,x.Value) select x.Key)
                ,1000,maxDistance);
                colors = p.Means;
            }
            else
                colors = from x in statistics.ColorStatistics.Dominance select x.Key;
            Pixel getClosest(Pixel inp)
            {
                Pixel output = Pixels.Black;
                double distance=double.MaxValue;
                foreach ( var c in colors )
                {
                    var d = getDistance(inp,c);
                    if ( d < distance )
                    {
                        distance = d;
                        output = c;
                    }
                    if ( d == 0 )
                        break;
                }
                return output;
            }

            for ( var x = 0; x < bmp.Width; x++ )
                for ( var y = 0; y < bmp.Height; y++ )
                    bmp[x , y] = getClosest(bmp[x , y]);
        }
        public static void FromPalette(ImageProxy bmp , BitmapPalette8bpp palette, Func<Pixel , Pixel , double> getDistance = null)
        {
            if ( getDistance == null )
                getDistance = RGBDistanceBasic;
            Pixel getClosest(Pixel inp)
            {
                Pixel output = Pixels.Black;
                double distance=double.MaxValue;
                for ( var i = 0; i < 256; i++ )
                {
                    var c = palette[i];
                    var d = getDistance(inp,c);
                    if ( d < distance )
                    {
                        distance = d;
                        output = c;
                    }
                    if ( d == 0 )
                        break;
                }
                return output;
            }
            for ( var x = 0; x < bmp.Width; x++ )
                for ( var y = 0; y < bmp.Height; y++ )
                    bmp[x , y] = getClosest(bmp[x , y]);
        }
        public static void FromPalette(ImageProxy bmp , List<Pixel> palette , Dictionary<Pixel , Pixel> cache = null , Func<Pixel , Pixel , double> getDistance = null)
        {
            if ( cache == null )
                cache = new Dictionary<Pixel , Pixel>();
            if ( getDistance == null )
                getDistance = RGBDistanceBasic;
            Pixel getClosest(Pixel inp)
            {
                Pixel output = Pixels.Black;
                double distance=double.MaxValue;
                for ( var i = 0; i < palette.Count; i++ )
                {
                    var c = palette[i];
                    var d = getDistance(inp,c);
                    if ( d < distance )
                    {
                        distance = d;
                        output = c;
                    }
                    if ( d == 0 )
                        break;
                }
                return output;
            }
            for ( var x = 0; x < bmp.Width; x++ )
                for ( var y = 0; y < bmp.Height; y++ )
                {
                    var px = bmp[x , y];
                    if ( cache.ContainsKey(px) )
                        bmp[x , y] = cache[px];
                    else
                        bmp[x , y] = cache[px] = getClosest(bmp[x , y]);
                }
        }

        public static Func<Pixel , Pixel , double> RGBDistanceBasic
        {
            get
            {
                double getDistance(Pixel a , Pixel b)
                {
                    return System.Math.Abs(a.R - b.R) + System.Math.Abs(a.G - b.G) + System.Math.Abs(a.B - b.B);
                }
                return getDistance;
            }
        }
        public static Func<HSB , HSB , double> HSBDistanceBasic
        {
            get
            {
                double getDistance(HSB a , HSB b)
                {
                    return 5 * System.Math.Abs(a.H - b.H) + 2 * System.Math.Abs(a.S - b.S) + System.Math.Abs(a.B - b.B);
                }
                return getDistance;
            }
        }
        public static Func<HSB , HSB , double> RGBDistanceInHSBImage
        {
            get
            {
                double getDistance(HSB c , HSB d)
                {
                    var a = c.ToRGB();
                    var b = d.ToRGB();

                    return System.Math.Abs(a.R - b.R) + System.Math.Abs(a.G - b.G) + System.Math.Abs(a.B - b.B);
                }
                return getDistance;
            }
        }
        public static Func<Pixel , Pixel , double> HSBDistanceInRGBImage
        {
            get
            {
                double getDistance(Pixel c, Pixel d)
                {
                    var a = c.ToHSB();
                    var b = d.ToHSB();
                    return 5 * System.Math.Abs(a.H - b.H) + 2 * System.Math.Abs(a.S - b.S) + System.Math.Abs(a.B - b.B);
                }
                return getDistance;
            }
        }
    }
}
