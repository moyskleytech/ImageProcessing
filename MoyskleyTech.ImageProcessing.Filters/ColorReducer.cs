using MoyskleyTech.ImageProcessing.Image;
using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using MoyskleyTech.Math;
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
        private static Func<Pixel[ ]> GetBeginningMean(int count,ImageStatistics stats)
        {
            Random r = new Random();
            return () => {
                return (from x in stats.ColorStatistics.Dominance orderby r.Next() select x.Key).Take(count).ToArray();
            };
        }
        public static void FromKMeans(ImageProxy bmp , int colorCount)
        {
            var statistics = new ImageStatistics(bmp);
            IEnumerable<Pixel> colors=null;
            double getDistance(Pixel a , Pixel b)
            {
                return System.Math.Abs(a.R - b.R) + System.Math.Abs(a.G - b.G) + System.Math.Abs(a.B - b.B);
            }
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
        }

        public static void FromXMeans(ImageProxy bmp , int colorCount,int maxDistance=50)
        {
            var statistics = new ImageStatistics(bmp);
            IEnumerable<Pixel> colors=null;
            double getDistance(Pixel a , Pixel b)
            {
                return System.Math.Abs(a.R - b.R) + System.Math.Abs(a.G - b.G) + System.Math.Abs(a.B - b.B);
            }
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
    }
}
