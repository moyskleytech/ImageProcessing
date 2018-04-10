using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MoyskleyTech.Math.Statistics
{
    public class DescriptiveStatistics
    {
        public double SumX { get; private set; }
        public double SumY { get; private set; }
        public double SumXY { get; private set; }
        public double SumX2 { get; private set; }
        public double SumY2 { get; private set; }
        public int Count { get; private set; }
        public double AverageY { get; private set; }
        public double AverageX { get; private set; }

        public double MaxX { get; private set; } = double.MinValue;
        public double MaxY { get; private set; } = double.MinValue;
        public double MinX { get; private set; } = double.MaxValue;
        public double MinY { get; private set; } = double.MaxValue;
        public double RangeX { get { return MaxX - MinX; } }
        public double RangeY { get { return MaxY - MinY; } }
        public LinearRegression LinearRegression { get; private set; }
        public static DescriptiveStatistics From1D(IEnumerable<double> x)
        {
            int i=0;
            return From2D(from t in x select new Tuple<double , double>(i , t));
        }
        public static DescriptiveStatistics From2D(IEnumerable<Tuple<double,double>> points)
        {
            DescriptiveStatistics statistics = new DescriptiveStatistics();
            foreach ( var pt in points )
            {
                double x=pt.Item1,y=pt.Item2;
                statistics.Count++;
                statistics.SumX += x;
                statistics.SumY += y;
                statistics.SumXY += x*y;
                statistics.SumX2 += x*x;
                statistics.SumY2 += y*y;

                if (x< statistics.MinX )
                    statistics.MinX = x;
                if ( y < statistics.MinY )
                    statistics.MinY = y;
                if ( x > statistics.MaxX )
                    statistics.MaxX = x;
                if ( y > statistics.MaxY )
                    statistics.MaxY = y;
            }
            if ( statistics.Count > 0 )
            {
                statistics.AverageX = statistics.SumX / statistics.Count;
                statistics.AverageY = statistics.SumY / statistics.Count;
                var reg = new LinearRegression();

                reg.B1 = ( statistics.SumXY - ( statistics.SumX *  statistics.SumY / statistics.Count ) ) / (  statistics.SumX2 - (  statistics.SumX * statistics.SumX / statistics.Count ) );
                reg.B1Ori = reg.B1;
                double varExplique;
                double varTot;

                if ( System.Math.Abs(reg.B1) < 1 )
                {
                    varExplique = reg.B1 * reg.B1 * (  statistics.SumX2 - (  statistics.SumX *  statistics.SumX /  statistics.Count ) );
                    varTot = statistics.SumY2 - ( statistics.SumY * statistics.SumY / statistics.Count );
                }
                else
                {
                    reg.B1 = (  statistics.SumXY - (  statistics.SumY *  statistics.SumX /  statistics.Count ) ) / (  statistics.SumY2 - (  statistics.SumY *  statistics.SumY /  statistics.Count ) );
                    varExplique = reg.B1 * reg.B1 * (  statistics.SumY2 - (  statistics.SumY *  statistics.SumY /  statistics.Count ) );
                    varTot = statistics.SumX2 - ( statistics.SumX * statistics.SumX / statistics.Count );
                }

                reg.B0 = statistics.AverageY - reg.B1 * statistics.AverageX;
                reg.B0Ori = statistics.AverageY - reg.B1Ori * statistics.AverageX;

                reg.R2 = varExplique / varTot;
                reg.Distort = ( System.Math.Sqrt(( varTot - varExplique ) / statistics.Count) );
                if ( double.IsNaN(reg.Distort) )
                    reg.Distort = 0;

                statistics.LinearRegression = reg;

            }
            return statistics;
        }

        public IEnumerable<Tuple<double , double>> NormalizeTo01(IEnumerable<Tuple<double , double>> items)
        {
            foreach ( var item in items )
            {
                double x=item.Item1,y=item.Item2;

                y = (( y - MinY ) / RangeY);
                if ( y < 0 )
                    y = 0;
                if ( y > 1 )
                    y = 1;
                yield return new Tuple<double , double>(x , y);
            }
        }
    }
    public struct LinearRegression
    {
        public double B0;
        public double B0Ori;
        public double B1;
        public double B1Ori;
        public double R2;

        public double Distort;
    }
}
