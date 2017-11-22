using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MoyskleyTech.ImageProcessing.Image;
using System.ComponentModel;
using System.Threading;

namespace MoyskleyTech.Charting
{
    public class PlotValues : Plot
    {
        protected Pixel barPixel;
        protected Pixel fillPixel;
        protected Semaphore semSync = new Semaphore(1, 1);

        
        
        public Pixel FillPixel
        {
            get
            {
                return fillPixel;
            }
            set
            {
                fillPixel = value;
            }
        }
        public Single LineThickness
        {
            get; set;
        } = 0;
       
        public override Pixel BarPixel
        {
            get
            {
                return barPixel;
            }
            set
            {
                barPixel = value;
            }
        }
        public PlotValues()
        {
            Values = new List<PointGraphique>();
            BarPixel = Pixels.Blue;
            MinShown = 0;
            MaxShown = double.MaxValue;
        }
        public List<PointGraphique> Values
        {
            get; set;
        }
        public override double MaxX
        {
            get
            {
                if ( SemaphoreWait(50) )
                {
                    var ret = 0d;
                    if ( Values.Count > 0 )
                        ret = Math.Min(Values.ToList().Max((x) => x.Absisce) , MaxShown);
                    else
                        ret = double.MinValue;
                    SemaphoreRelease();
                    return ret - MinShown;
                }
                return double.MinValue;
            }
        }
        public void ValuesAdd(PointGraphique v)
        {
            SemaphoreWait();
            Values.Add(v);
            SemaphoreRelease();
        }
        public override double MaxY
        {
            get
            {
                if ( SemaphoreWait(50) )
                {
                    var ret = 0d;
                    if ( Values.Count > 0 )
                        ret = Values.ToList().Max((x) => x.Ordonee);
                    else
                        ret = double.MinValue;
                    SemaphoreRelease();
                    return ret;
                }
                return double.MinValue;
            }
        }
        public double MinShown { get; set; }
        public double MaxShown { get; set; }
        public override double MinX
        {
            get
            {
                if ( SemaphoreWait(50) )
                {
                    var ret = 0d;
                    if ( Values.Count > 0 )
                        ret = Values.ToList().Min((x) => x.Absisce);
                    else
                        ret = double.MaxValue;
                    SemaphoreRelease();
                    return ret;
                }
                return double.MaxValue;
            }
        }
        public int PointSize { get; set; } = 2;

        public override double MinY
        {
            get
            {
                if ( SemaphoreWait(50) )
                {
                    var ret = 0d;
                    if ( Values.Count > 0 )
                        ret = Values.ToList().Min((x) => x.Ordonee);
                    else
                        ret = double.MaxValue;
                    SemaphoreRelease();
                    return ret;
                }
                return double.MaxValue;
            }
        }
        public virtual PlotValues Clone()
        {
            PlotValues ret = new PlotValues();
            if ( SemaphoreWait(50) )
            {
                ret.Values = this.Values.ToList();
                SemaphoreRelease();
            }
            ret.BarPixel = this.BarPixel;
            ret.Style = this.Style;
            ret.Visible = this.Visible;
            ret.LineThickness = this.LineThickness;
            ret.PointSize = this.PointSize;

            return ret;
        }
        public override void Draw(PlotChart c , Graphics g , int w , int h)
        {

            var height = h - Margin.Vertical;
            var width = w - Margin.Horizontal;
            var left = Margin.Left;
            var top = Margin.Top;
            var bottom = top+height;
            var right = left+width;

            g.SetClip(new Rectangle(left , top , width , height));

            if ( !Visible )
                return;
            if ( Values.Count > 1 || MaxShown != double.MaxValue )
            {
                if ( c.MaxY < 0 )
                    return;
                double pixelPerPointX=0;
                double pixelPerPointY = ( double )height / c.MaxY;
                if ( double.IsInfinity(pixelPerPointY) )
                    return;

                try
                {
                    if ( MaxShown != double.MaxValue )
                    {
                        pixelPerPointX = ( double ) width / ( MaxShown - MinShown );
                    }
                    else
                        pixelPerPointX = ( double ) width / ( MaxX );
                    if ( !SemaphoreWait(50) )
                        return;
                    Pixel b = barPixel;
                    if ( Values.Count > 0 )
                    {
                        var valPre = Values[0];
                        for ( var i = 0 ; i < Values.Count ; i++ )
                        {
                            if ( i > 1 )
                                valPre = Values[i - 1];
                            var val = Values[i];

                            if ( double.IsNaN(val.Absisce) || double.IsNaN(val.Ordonee) )
                                continue;
                            if ( double.IsInfinity(val.Absisce) || double.IsInfinity(val.Ordonee) )
                                continue;
                            if ( valPre.Absisce > MaxShown )
                                break;
                            if ( val.Absisce < MinShown )
                                continue;
                            Point p = new Point((int)((val.Absisce - MinShown) * pixelPerPointX )+left,top+ (int)(height - val.Ordonee * pixelPerPointY));
                            g.FillEllipse(BarPixel , p.X - PointSize , p.Y - PointSize , PointSize * 2 , PointSize * 2);


                            if ( Style == PlotStyle.Linked )
                            {
                                if ( double.IsNaN(valPre.Absisce) || double.IsNaN(valPre.Ordonee) )
                                    continue;
                                if ( double.IsInfinity(valPre.Absisce) || double.IsInfinity(valPre.Ordonee) )
                                    continue;

                                Point pPre = new Point((int)((valPre.Absisce - MinShown) * pixelPerPointX+left),top+ (int)(height - valPre.Ordonee * pixelPerPointY));
                                g.DrawLine(BarPixel , pPre , p,(int)LineThickness);

                            }
                            else if ( Style == PlotStyle.FillTop )
                            {
                                if ( double.IsNaN(valPre.Absisce) || double.IsNaN(valPre.Ordonee) )
                                    continue;
                                if ( double.IsInfinity(valPre.Absisce) || double.IsInfinity(valPre.Ordonee) )
                                    continue;

                                Point pPre = new Point((int)((valPre.Absisce - MinShown) * pixelPerPointX+left),top+ (int)(height - valPre.Ordonee * pixelPerPointY));

                                g.FillPolygon(fillPixel , new PointF[ ] { pPre , p , new Point(p.X , top) , new Point(pPre.X , top) });
                                g.DrawLine(BarPixel , pPre , p, (int)LineThickness);
                            }
                            else if ( Style == PlotStyle.FillBottom )
                            {
                                if ( double.IsNaN(valPre.Absisce) || double.IsNaN(valPre.Ordonee) )
                                    continue;
                                if ( double.IsInfinity(valPre.Absisce) || double.IsInfinity(valPre.Ordonee) )
                                    continue;

                                Point pPre = new Point((int)((valPre.Absisce - MinShown) * pixelPerPointX+left),top+ (int)(height - valPre.Ordonee * pixelPerPointY));

                                g.FillPolygon(fillPixel , new PointF[ ] { pPre , p , new Point(p.X , ( int ) ( bottom )) , new Point(pPre.X , ( int ) ( bottom )) });
                                g.DrawLine(BarPixel , pPre , p, ( int ) LineThickness);
                            }

                        }

                    }

                }
                catch { }
                SemaphoreRelease();
                g.ResetClip();
            }
        }
        public PlotStyle Style { get; set; } = PlotStyle.Scattered;
        public bool SemaphoreWait(int delay = -1)
        {
            bool res=semSync.WaitOne(delay);
            //System.Diagnostics.Debug.WriteLine("SEMAPHORE ALOWED");
            return res;
        }
        public void SemaphoreRelease()
        {
            semSync.Release();
        }
        public void Semaphore(Action c , int delay = -1)
        {
            if ( SemaphoreWait(delay) )
            {
                c();
                SemaphoreRelease();
            }
        }
        public Line LinearRegression()
        {
            SemaphoreWait();
            if ( Values.Count > 1 )
            {
                Line l = new Line();
                double delta = Values.Count * X2Sum - Math.Pow(XSum, 2.0);
                l.OrdonneeALOrigine = ( 1.0 / delta ) * ( X2Sum * YSum - XSum * XYProductSum );
                l.Pente = ( 1.0 / delta ) * ( ( double ) Values.Count * XYProductSum - XSum * YSum );
                semSync.Release();
                return l;
            }
            else
            {
                SemaphoreRelease();
                return new Line() { Pente = 0 , OrdonneeALOrigine = 0 };
            }
        }
        public Line LinearRegressionOptimized()
        {
            if ( Values.Count <= 1 )
                return new Line() { Pente = 0 , OrdonneeALOrigine = 0 };

            SemaphoreWait();
            var lst = from x in Values select new { XY = x.Absisce*x.Ordonee, X = x.Absisce,Y = x.Ordonee,X2 = x.Absisce*x.Absisce ,Y2 = x.Ordonee*x.Ordonee};
            SemaphoreRelease();

            double XYProductSum = 0;
            double XSum = 0;
            double YSum = 0;
            double X2Sum =0;
            double Y2Sum=0;
            foreach ( var item in lst )
            {
                XYProductSum += item.XY;
                XSum += item.X;
                YSum += item.Y;
                X2Sum += item.X2;
                Y2Sum += item.Y2;
            }
            Line l = new Line();
            double delta = Values.Count * X2Sum - Math.Pow(XSum, 2.0);
            l.OrdonneeALOrigine = ( 1.0 / delta ) * ( X2Sum * YSum - XSum * XYProductSum );
            l.Pente = ( 1.0 / delta ) * ( ( double ) Values.Count * XYProductSum - XSum * YSum );

            return l;
        }

        private double XYProductSum
        {
            get
            {
                return ( from x in Values select ( x.Absisce * x.Ordonee ) ).Sum();
            }
        }
        private double XSum
        {
            get
            {
                return ( from x in Values select ( x.Absisce ) ).Sum();
            }
        }
        private double X2Sum
        {
            get
            {
                return ( from x in Values select ( x.Absisce * x.Absisce ) ).Sum();
            }
        }
        private double Y2Sum
        {
            get
            {
                return ( from x in Values select ( x.Ordonee * x.Ordonee ) ).Sum();
            }
        }
        private double YSum
        {
            get
            {
                return ( from x in Values select ( x.Ordonee ) ).Sum();
            }
        }

        public List<PointGraphique> Maximums(double deltaMin , double sample = 3 , double minVar = 0)
        {
            if ( Values.Count == 0 )
                return new List<PointGraphique>();
            if ( Values.Count == 1 )
                return new List<PointGraphique> { Values[0] };
            {
                List<PointGraphique> potentiels = new List<PointGraphique>();
                SemaphoreWait();
                for ( var i = 1 ; i < Values.Count - 1 ; i++ )
                {
                    if ( Values[i - 1].Ordonee <= Values[i].Ordonee && Values[i].Ordonee >= Values[i + 1].Ordonee )
                        if ( Values.Where((x , p) => p > i - sample && p < i + sample).Average((x) => x.Ordonee) + minVar < Values[i].Ordonee )
                            potentiels.Add(Values[i]);
                }

                List<PointGraphique> maxs = new List<PointGraphique>();
                if ( potentiels.Count > 0 )
                    maxs.Add(potentiels[0]);
                for ( var i = 1 ; i < potentiels.Count ; i++ )
                {
                    if ( potentiels[i - 1].Absisce + deltaMin < potentiels[i].Absisce )
                        maxs.Add(potentiels[i]);
                }
                SemaphoreRelease();
                return maxs;
            }
        }
        public List<PointGraphique> Minimums(double deltaMin , double sample = 3 , double minVar = 0)
        {
            if ( Values.Count == 0 )
                return new List<PointGraphique>();
            if ( Values.Count == 1 )
                return new List<PointGraphique> { Values[0] };
            {
                List<PointGraphique> potentiels = new List<PointGraphique>();
                SemaphoreWait();
                for ( var i = 1 ; i < Values.Count - 1 ; i++ )
                {
                    if ( Values[i - 1].Ordonee >= Values[i].Ordonee && Values[i].Ordonee <= Values[i + 1].Ordonee )
                        if ( Values.Where((x , p) => p > i - sample && p < i + sample).Average((x) => x.Ordonee) - minVar > Values[i].Ordonee )
                            potentiels.Add(Values[i]);
                }

                List<PointGraphique> maxs = new List<PointGraphique>();
                if ( potentiels.Count > 0 )
                    maxs.Add(potentiels[0]);
                for ( var i = 1 ; i < potentiels.Count ; i++ )
                {
                    if ( potentiels[i - 1].Absisce + deltaMin < potentiels[i].Absisce )
                        maxs.Add(potentiels[i]);
                }
                SemaphoreRelease();
                return maxs;
            }
        }
        public PlotValues Unnoize(double smooth = 4)
        {
            double halfStep = smooth/2;

            PlotValues ret = this.Clone();
            SemaphoreWait();
            ret.Values.Clear();
            ret.Values.Add(this.Values[0]);
            double max = this.Values[this.Values.Count-1].Absisce;
            double loc = this.Values[0].Absisce;
            while ( loc < max )
            {
                var avg = this.Values.Where((x)=>x.Absisce > loc-halfStep && x.Absisce < loc +halfStep).Average((x)=>x.Ordonee);
                var addData =this.Values.Where((x)=>x.Absisce > loc-halfStep && x.Absisce < loc +halfStep).First().AdditionnalData;
                ret.ValuesAdd(new PointGraphique(loc+halfStep , avg) { AdditionnalData = addData });
                loc += smooth;
            }
            ret.Values.Add(this.Values[this.Values.Count - 1]);
            SemaphoreRelease();
            return ret;
        }

        public PointGraphique MouseToGraphCoord(PlotChart c , Point mouse)
        {
            var height = c.Height - Margin.Vertical;
            var width = c.Width - Margin.Horizontal;
            var left = Margin.Left;
            var top = Margin.Top;
            var bottom = top+height;
            var right = left+width;
            double pixelPerPointX=0;
            double pixelPerPointY = ( double )height / c.MaxY;

            if ( Values.Count > 1 || MaxShown != double.MaxValue )
            {


                if ( double.IsInfinity(pixelPerPointY) )
                    return new PointGraphique(double.NaN , double.NaN);
                if ( MaxShown != double.MaxValue )
                {
                    pixelPerPointX = ( double ) width / ( MaxShown - MinShown );
                }
                else
                    pixelPerPointX = ( double ) width / ( MaxX );

            }

            var ex = mouse.X-Margin.Left;
            var ey = (c.Height - mouse.Y)-Margin.Bottom;

            var px = ex/pixelPerPointX;
            var py = ey/pixelPerPointY;

            return new PointGraphique(px , py);

        }

        public PlotValues Scale(int v)
        {
            var max= Values.Max((x)=>x.Absisce);
            var factor = v/max;

            var ret = this.Clone();
            foreach ( var val in ret.Values )
            {
                val.Absisce *= factor;
            }
            return ret;
        }

        public Point GraphToMouseCoord(PlotChart c , PointGraphique pg)
        {
            var height = c.Height - Margin.Vertical;
            var width = c.Width - Margin.Horizontal;
            var left = Margin.Left;
            var top = Margin.Top;
            var bottom = top+height;
            var right = left+width;
            double pixelPerPointX=0;
            double pixelPerPointY = ( double )height / c.MaxY;

            if ( Values.Count > 1 || MaxShown != double.MaxValue )
            {


                if ( double.IsInfinity(pixelPerPointY) )
                    return new Point(int.MinValue , int.MinValue);
                if ( MaxShown != double.MaxValue )
                {
                    pixelPerPointX = ( double ) width / ( MaxShown - MinShown );
                }
                else
                    pixelPerPointX = ( double ) width / ( MaxX );

            }
            var ex = pg.Absisce * pixelPerPointX;
            var ey = pg.Ordonee * pixelPerPointY;
            var px = ex + Margin.Left;
            var py = c.Height - Margin.Bottom - ey;
            return new Point(( int ) px , ( int ) py);
        }

        public enum PlotStyle
        {
            Scattered,
            Linked,
            FillBottom,
            FillTop
        }

    }
}
