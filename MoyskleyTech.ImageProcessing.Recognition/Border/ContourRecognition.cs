using MoyskleyTech.ImageProcessing.Image;
using MoyskleyTech.ImageProcessing.Recognition;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MoyskleyTech.ImageProcessing.Recognition.Border
{
    public static class ContourRecognition
    {
        public static List<Contour<Representation>> Analyse<Representation>(ImageProxy<Representation> bmp , Func<Representation , bool> condition , bool keepAllPoints = false , ContourRecognitionMode mode = ContourRecognitionMode.EightConnex)
           where Representation : unmanaged
        {
            return Analyse(bmp , condition , keepAllPoints ? ContourRecognitionPointKeep.All : ContourRecognitionPointKeep.Border,mode);
        }
        public static List<Contour<Representation>> Analyse<Representation>(ImageProxy<Representation> bmp , Func<Representation , bool> condition , ContourRecognitionPointKeep keepAllPoints = ContourRecognitionPointKeep.Border , ContourRecognitionMode mode = ContourRecognitionMode.EightConnex)
            where Representation : unmanaged
        {
            HashSet<Point> visited=new HashSet<Point>();
            List<Contour< Representation >> contours = new List<Contour<Representation>>();
            for ( var y = 0; y < bmp.Height; y++ )
            {
                for ( var x = 0; x < bmp.Width; x++ )
                {
                    Contour< Representation> contour = new Contour<Representation>();
                    contour.PointMode = keepAllPoints;
                    if ( !visited.Contains(new Point(x , y)) )
                    {
                        var px = bmp[x,y];
                        if ( condition(px) )
                        {
                            if ( mode == ContourRecognitionMode.EightConnex )
                                bmp.Match8Connex(x , y , condition , (pt , pxl) =>
                                {
                                    contour.Include(pt);
                                    visited.Add(pt);
                                });
                            else if ( mode == ContourRecognitionMode.FourConnex )
                                bmp.Match4Connex(x , y , condition , (pt , pxl) =>
                                {
                                    contour.Include(pt);
                                    visited.Add(pt);
                                });


                            contour.ImageArea = new ImageProxy<Representation>(bmp , contour.Area);
                            if( keepAllPoints == ContourRecognitionPointKeep.Border)
                                contour.CleanPoints();
                            contours.Add(contour);
                        }
                    }
                }
            }
            return contours;
        }
        public static List<Contour<Representation>> AnalyseFromPoints<Representation>(ImageProxy<Representation> bmp , List<Point> pts , Func<Representation , bool> condition , bool keepAllPoints = false , ContourRecognitionMode mode = ContourRecognitionMode.EightConnex)
            where Representation : unmanaged
        {
            return AnalyseFromPoints(bmp , pts , condition , keepAllPoints ? ContourRecognitionPointKeep.All : ContourRecognitionPointKeep.Border , mode);
        }
            public static List<Contour<Representation>> AnalyseFromPoints<Representation>(ImageProxy<Representation> bmp , List<Point> pts , Func<Representation , bool> condition , ContourRecognitionPointKeep keepAllPoints = ContourRecognitionPointKeep.Border , ContourRecognitionMode mode = ContourRecognitionMode.EightConnex)
             where Representation : unmanaged
        {
            HashSet<Point> visited=new HashSet<Point>();
            List<Contour<Representation>> contours = new List<Contour<Representation>>();
            
            foreach ( var sp in pts )
            {
                var x = sp.X;
                var y = sp.Y;
                Contour<Representation> contour = new Contour<Representation>();
                if ( !visited.Contains(sp) )
                {
                    var px = bmp[x,y];
                    if ( condition(px) )
                    {
                        if ( mode == ContourRecognitionMode.EightConnex )
                            bmp.Match8Connex(x , y , condition , (pt , pxl) =>
                            {
                                contour.Include(pt);
                                visited.Add(pt);
                            });
                        else if ( mode == ContourRecognitionMode.FourConnex )
                            bmp.Match4Connex(x , y , condition , (pt , pxl) =>
                            {
                                contour.Include(pt);
                                visited.Add(pt);
                            });


                        contour.ImageArea = new ImageProxy<Representation>(bmp , contour.Area);
                        if ( keepAllPoints == ContourRecognitionPointKeep.Border )
                            contour.CleanPoints();
                        contours.Add(contour);
                    }
                }
            }
            return contours;
        }
     
        public static double[ ] GetHuMoments<Representation>(ImageProxy<Representation> proxy)
            where Representation : unmanaged
        {
            var converter = ColorConvert.GetConversionFrom<Representation,Pixel>();
            var grayImage = proxy.ToImage((r)=>converter(r).GetGrayTone());
            //Called 3 times
            double getRawMoment(int p , int q)
            {
                int width = grayImage.Width;
                int height = grayImage.Height;

                double m = 0;
                for ( int i = 0, k = height; i < k; i++ )
                {
                    for ( int j = 0, l = width; j < l; j++ )
                    {
                        m += Math.Pow(i , p) * Math.Pow(j , q) * grayImage.Get(i , j);
                    }
                }
                return m;
            }
            //Called 1 time
            PointF getCentroid()
            {
                double m00 = getRawMoment(0, 0);
                double m10 = getRawMoment(1, 0);
                double m01 = getRawMoment(0, 1);
                double x0 = m10 / m00;
                double y0 = m01 / m00;
                return new PointF(x0 , y0);
            }
            PointF centroid = getCentroid();
            //Called 8 times
            double getCentralMoment(int p , int q)
            {
                int width = grayImage.Width;
                int height = grayImage.Height;

                double mc = 0;
                for ( int i = 0, k = height; i < k; i++ )
                {
                    for ( int j = 0, l = width; j < l; j++ )
                    {
                        mc += Math.Pow(( i - centroid.X ) , p) * Math.Pow(( j - centroid.Y ) , q) * grayImage.Get(i , j);
                    }
                }
                return mc;
            }
            //Called 8 times
            double getNormalizedCentralMoment(int p , int q)
            {
                double gama = ((p + q) / 2) + 1;
                double mpq = getCentralMoment(p, q);
                double m00gama = Math.Pow(mpq, gama);
                return mpq / m00gama;
            }

            double[] moments = new double[8];

            double
            n20 = getNormalizedCentralMoment( 2, 0),
            n02 = getNormalizedCentralMoment( 0, 2),
            n30 = getNormalizedCentralMoment(3, 0),
            n12 = getNormalizedCentralMoment(1, 2),
            n21 = getNormalizedCentralMoment( 2, 1),
            n03 = getNormalizedCentralMoment( 0, 3),
            n11 = getNormalizedCentralMoment( 1, 1);

            //First moment
            moments[0] = n20 + n02;

            //Second moment
            moments[1] = Math.Pow(( n20 - 02 ) , 2) + Math.Pow(2 * n11 , 2);

            //Third moment
            moments[2] = Math.Pow(n30 - ( 3 * ( n12 ) ) , 2)
                       + Math.Pow(( 3 * n21 - n03 ) , 2);

            //Fourth moment
            moments[3] = Math.Pow(( n30 + n12 ) , 2) + Math.Pow(( n12 + n03 ) , 2);

            //Fifth moment
            moments[4] = ( n30 - 3 * n12 ) * ( n30 + n12 )
                         * ( Math.Pow(( n30 + n12 ) , 2) - 3 * Math.Pow(( n21 + n03 ) , 2) )
                         + ( 3 * n21 - n03 ) * ( n21 + n03 )
                         * ( 3 * Math.Pow(( n30 + n12 ) , 2) - Math.Pow(( n21 + n03 ) , 2) );

            //Sixth moment
            moments[5] = ( n20 - n02 )
                         * ( Math.Pow(( n30 + n12 ) , 2) - Math.Pow(( n21 + n03 ) , 2) )
                         + 4 * n11 * ( n30 + n12 ) * ( n21 + n03 );

            //Seventh moment
            moments[6] = ( 3 * n21 - n03 ) * ( n30 + n12 )
                         * ( Math.Pow(( n30 + n12 ) , 2) - 3 * Math.Pow(( n21 + n03 ) , 2) )
                         + ( n30 - 3 * n12 ) * ( n21 + n03 )
                         * ( 3 * Math.Pow(( n30 + n12 ) , 2) - Math.Pow(( n21 + n03 ) , 2) );

            //Eighth moment
            moments[7] = n11 * ( Math.Pow(( n30 + n12 ) , 2) - Math.Pow(( n03 + n21 ) , 2) )
                             - ( n20 - n02 ) * ( n30 + n12 ) * ( n03 + n21 );

            grayImage.Dispose();
            return moments;
        }
        public static double HuDistance<Representation1,Representation2>(ImageProxy<Representation1> A, ImageProxy<Representation2> B,int noDistance)
             where Representation1 : unmanaged
             where Representation2 : unmanaged
        {
            var hu1 = GetHuMoments(A);
            var hu2 = GetHuMoments(B);

            var ma1 = (from x in hu1 select Math.Sign(x) * Math.Log10(x)).ToArray();
            var ma2 = (from x in hu2 select Math.Sign(x) * Math.Log10(x)).ToArray();
            if ( noDistance == 1 )
            {
                return ( from i in Enumerable.Range(0 , hu1.Length) select 1d / ma1[i] - 1d / ma2[i] ).Sum();
            }
            else if ( noDistance == 2 )
            {
                return ( from i in Enumerable.Range(0 , hu1.Length) select ma1[i] - ma2[i] ).Sum();
            }
            else if ( noDistance == 3 )
            {
                return ( from i in Enumerable.Range(0 , hu1.Length) select (ma1[i] - ma2[i])/ ma1[i] ).Sum();
            }
            return double.NaN;
        }
    }
    public enum ContourRecognitionMode
    {
        FourConnex, EightConnex
    }
    public enum ContourRecognitionPointKeep
    {
        All, None,Border
    }
    public class Contour<Representation>
        where Representation : unmanaged
    {
        public Contour()
        {
            Points = new List<Point>();
        }
        public ImageProxy<Representation> ImageArea { get; internal set; }
        public List<Point> Points { get; internal set; }
        private Rectangle? area;
        public void CleanPoints()
        {
            var rct= Area;
            bool[,] tmp = new bool[rct.Width+2,rct.Height+2];
            bool[,] tmp2 = new bool[rct.Width+2,rct.Height+2];
            foreach ( var pt in Points )
            {
                tmp[pt.X + 1 - rct.Left , pt.Y + 1 - rct.Top] = true;
            }
            Points.Clear();
            for ( var x = 0; x < rct.Width; x++ )
            {
                for ( var y = 0; y < rct.Height; y++ )
                {
                    var tx=x+1;
                    var ty=y+1;
                    if ( tmp[tx , ty] )
                    {
                        tmp2[tx , ty] = ( !( tmp[tx - 1 , ty] && tmp[tx + 1 , ty] && tmp[tx , ty - 1] && tmp[tx , ty + 1] ) );
                    }
                }
            }
            bool found=false;
            for ( var x = 0; x < rct.Width && !found; x++ )
            {
                for ( var y = 0; y < rct.Height && !found; y++ )
                {
                    var tx=x+1;
                    var ty=y+1;
                    if ( tmp2[tx , ty] )
                    {
                        found = true;
                        Points.AddRange(tmp2.Match8ConnexList(tx , ty).Select((pt) => new Point(pt.X + rct.Left , pt.Y + rct.Top)));
                    }
                }
            }
            //Points.Add(new Point(x + rct.Left , y + rct.Top));

        }
        public void Include(Point point)
        {
            if ( area == null )
                area = new Rectangle(point.X , point.Y , 1 , 1);
            if ( PointMode != ContourRecognitionPointKeep.None )
                Points.Add(point);

            var areaRect = area.Value;
            int dty = areaRect.Y - point.Y;
            if ( dty > 0 )
            {
                areaRect.Top -= dty;
                areaRect.Height += dty;
            }
            int dtx = areaRect.Left - point.X;
            if ( dtx > 0 )
            {
                areaRect.Left -= dtx;
                areaRect.Width += dtx;
            }
            if ( point.X > areaRect.Right )
                areaRect.Width += point.X - areaRect.Right;
            if ( point.Y > areaRect.Bottom )
                areaRect.Height += point.Y - areaRect.Bottom;
        }
        public Rectangle Area
        {
            get
            {
                if ( area != null )
                    return area.Value;
                int x = Points.Min((p)=>p.X);
                int y = Points.Min((p)=>p.Y);
                int mx = Points.Max((p)=>p.X);
                int my = Points.Max((p)=>p.Y);
                return new Rectangle(x , y , mx - x + 1 , my - y + 1);
            }
            set => area = value;
        }

        public ContourRecognitionPointKeep PointMode { get; internal set; }
    }
}
