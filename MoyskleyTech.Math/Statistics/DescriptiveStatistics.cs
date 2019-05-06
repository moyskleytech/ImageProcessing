using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace MoyskleyTech.Mathematics.Statistics
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


        public double VarianceX { get; private set; }
        public double VarianceY { get; private set; }
        public double StandardDeviationX { get; private set; }
        public double StandardDeviationY { get; private set; }
        
        public static DescriptiveStatistics From1D(IEnumerable<double> x)
        {
            int i=0;
            return From2D(from t in x select new Coordinate(i++ , t));
        }
        public static DescriptiveStatistics From2D(IEnumerable<Coordinate> points)
        {
            DescriptiveStatistics statistics = new DescriptiveStatistics();
            foreach ( var pt in points )
            {
                double x=pt.X,y=pt.Y;
                statistics.Count++;
                statistics.SumX += x;
                statistics.SumY += y;
                statistics.SumXY += x * y;
                statistics.SumX2 += x * x;
                statistics.SumY2 += y * y;

                if ( x < statistics.MinX )
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
                var reg = new LinearRegression
                {
                    B1 = ( statistics.SumXY - ( statistics.SumX * statistics.SumY / statistics.Count ) ) / ( statistics.SumX2 - ( statistics.SumX * statistics.SumX / statistics.Count ) )
                };
                reg.B1Ori = reg.B1;
                double varExplique;
                double varTot;

                if ( System.Math.Abs(reg.B1) < 1 )
                {
                    varExplique = reg.B1 * reg.B1 * ( statistics.SumX2 - ( statistics.SumX * statistics.SumX / statistics.Count ) );
                    varTot = statistics.SumY2 - ( statistics.SumY * statistics.SumY / statistics.Count );
                }
                else
                {
                    reg.B1 = ( statistics.SumXY - ( statistics.SumY * statistics.SumX / statistics.Count ) ) / ( statistics.SumY2 - ( statistics.SumY * statistics.SumY / statistics.Count ) );
                    varExplique = reg.B1 * reg.B1 * ( statistics.SumY2 - ( statistics.SumY * statistics.SumY / statistics.Count ) );
                    varTot = statistics.SumX2 - ( statistics.SumX * statistics.SumX / statistics.Count );
                }

                reg.B0 = statistics.AverageY - reg.B1 * statistics.AverageX;
                reg.B0Ori = statistics.AverageY - reg.B1Ori * statistics.AverageX;

                reg.R2 = varExplique / varTot;
                reg.Distort = ( System.Math.Sqrt(( varTot - varExplique ) / statistics.Count) );
                if ( double.IsNaN(reg.Distort) )
                    reg.Distort = 0;

                statistics.LinearRegression = reg;

                statistics.VarianceX = statistics.SumX2 / statistics.Count - statistics.AverageX * statistics.AverageX;
                statistics.VarianceY = statistics.SumY2 / statistics.Count - statistics.AverageY * statistics.AverageY;
                statistics.StandardDeviationX = Math.Sqrt(statistics.VarianceX);
                statistics.StandardDeviationY = Math.Sqrt(statistics.VarianceY);

            }
            return statistics;
        }

        public IEnumerable<Coordinate> NormalizeTo01(IEnumerable<Coordinate> items)
        {
            foreach ( var item in items )
            {
                double x=item.X,y=item.Y;

                y = ( ( y - MinY ) / RangeY );
                if ( y < 0 )
                    y = 0;
                if ( y > 1 )
                    y = 1;
                yield return new Coordinate(x , y);
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

    public class DescriptiveStatistics<Number>
        where Number : struct
    {
        public Number SumX { get; private set; }
        public Number SumY { get; private set; }
        public Number SumXY { get; private set; }
        public Number SumX2 { get; private set; }
        public Number SumY2 { get; private set; }
        public int Count { get; private set; }
        public Number AverageY { get; private set; }
        public Number AverageX { get; private set; }

        public Number MaxX { get; private set; }
        public Number MaxY { get; private set; }
        public Number MinX { get; private set; }
        public Number MinY { get; private set; }
        public Number RangeX { get { return ( dynamic ) MaxX - MinX; } }
        public Number RangeY { get { return ( dynamic ) MaxY - MinY; } }
        public LinearRegression<Number> LinearRegression { get; private set; }

        public DescriptiveStatistics()
        {
            var maxValue = (Number)typeof(Number).GetRuntimeField("MaxValue").GetValue(null);
            var minValue = (Number)typeof(Number).GetRuntimeField("MinValue").GetValue(null);
            MaxX = minValue;
            MaxY = minValue;
            MinX = maxValue;
            MinY = maxValue;
        }
        public static DescriptiveStatistics<Number> From1D(IEnumerable<Number> x)
        {
            int i=0;
            return From2D(from t in x select new Coordinate<Number>(( dynamic ) i , t));
        }
        public static DescriptiveStatistics<Number> From2D(IEnumerable<Coordinate<Number>> points)
        {
            DescriptiveStatistics<Number> statistics = new DescriptiveStatistics<Number>();
            foreach ( var pt in points )
            {
                Number x=pt.X,y=pt.Y;
                statistics.Count++;
                statistics.SumX += ( dynamic ) x;
                statistics.SumY += ( dynamic ) y;
                statistics.SumXY += ( dynamic ) x * y;
                statistics.SumX2 += ( dynamic ) x * x;
                statistics.SumY2 += ( dynamic ) y * y;

                if ( ( dynamic ) x < statistics.MinX )
                    statistics.MinX = x;
                if ( ( dynamic ) y < statistics.MinY )
                    statistics.MinY = y;
                if ( ( dynamic ) x > statistics.MaxX )
                    statistics.MaxX = x;
                if ( ( dynamic ) y > statistics.MaxY )
                    statistics.MaxY = y;
            }
            if ( statistics.Count > 0 )
            {
                statistics.AverageX = ( dynamic ) statistics.SumX / statistics.Count;
                statistics.AverageY = ( dynamic ) statistics.SumY / statistics.Count;
                var reg = new LinearRegression<Number>
                {
                    B1 = ( statistics.SumXY - ( ( dynamic ) statistics.SumX * statistics.SumY / statistics.Count ) ) / ( statistics.SumX2 - ( ( dynamic ) statistics.SumX * statistics.SumX / statistics.Count ) )
                };
                reg.B1Ori = reg.B1;
                Number varExplique;
                Number varTot;

                if ( System.Math.Abs(( dynamic ) reg.B1) < 1 )
                {
                    varExplique = ( dynamic ) reg.B1 * reg.B1 * ( statistics.SumX2 - ( ( dynamic ) statistics.SumX * statistics.SumX / statistics.Count ) );
                    varTot = statistics.SumY2 - ( ( dynamic ) statistics.SumY * statistics.SumY / statistics.Count );
                }
                else
                {
                    reg.B1 = ( statistics.SumXY - ( ( dynamic ) statistics.SumY * statistics.SumX / statistics.Count ) ) / ( statistics.SumY2 - ( ( dynamic ) statistics.SumY * statistics.SumY / statistics.Count ) );
                    varExplique = ( dynamic ) reg.B1 * reg.B1 * ( statistics.SumY2 - ( ( dynamic ) statistics.SumY * statistics.SumY / statistics.Count ) );
                    varTot = statistics.SumX2 - ( ( dynamic ) statistics.SumX * statistics.SumX / statistics.Count );
                }

                reg.B0 = statistics.AverageY - ( dynamic ) reg.B1 * statistics.AverageX;
                reg.B0Ori = statistics.AverageY - ( dynamic ) reg.B1Ori * statistics.AverageX;

                reg.R2 = ( dynamic ) varExplique / varTot;
                reg.Distort = ( System.Math.Sqrt(( ( dynamic ) varTot - varExplique ) / statistics.Count) );

                if ( typeof(Number) == typeof(double) )
                    if ( double.IsNaN(( double ) ( dynamic ) reg.Distort) )
                        reg.Distort = ( dynamic ) 0;

                statistics.LinearRegression = reg;

            }
            return statistics;
        }

        public IEnumerable<Coordinate<Number>> NormalizeTo01(IEnumerable<Coordinate<Number>> items)
        {
            foreach ( var item in items )
            {
                Number x=item.X,y=item.Y;

                y = ( ( ( dynamic ) y - MinY ) / RangeY );
                if ( ( dynamic ) y < 0 )
                    y = ( dynamic ) 0;
                if ( ( dynamic ) y > 1 )
                    y = ( dynamic ) 1;
                yield return new Coordinate<Number>(x , y);
            }
        }
    }
    public struct LinearRegression<Number>
        where Number : struct
    {
        public Number B0;
        public Number B0Ori;
        public Number B1;
        public Number B1Ori;
        public Number R2;

        public Number Distort;
    }
}
