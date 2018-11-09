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
    [Obsolete("Use Charting2D namespace" , true)]
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
            Values = new List<Charting2D.ChartData2D>();
            BarPixel = Pixels.Blue;
            MinShown = 0;
            MaxShown = double.MaxValue;
        }
        public List<Charting2D.ChartData2D> Values
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
                        ret = Math.Min(Values.ToList().Max((x) => x.X) , MaxShown);
                    else
                        ret = double.MinValue;
                    SemaphoreRelease();
                    return ret - MinShown;
                }
                return double.MinValue;
            }
        }
        public void ValuesAdd(Charting2D.ChartData2D v)
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
                        ret = Values.ToList().Max((x) => x.Y);
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
                        ret = Values.ToList().Min((x) => x.X);
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
                        ret = Values.ToList().Min((x) => x.Y);
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

            //var height = h - Margin.Vertical;
            //var width = w - Margin.Horizontal;
            //var left = Margin.Left;
            //var top = Margin.Top;
            //var bottom = top+height;
            //var right = left+width;

            //g.SetClip(new Rectangle(left , top , width , height));

            //if ( !Visible )
            //    return;
            //if ( Values.Count > 1 || MaxShown != double.MaxValue )
            //{
            //    if ( c.MaxY < 0 )
            //        return;
            //    double pixelPerPointX=0;
            //    double pixelPerPointY = ( double )height / c.MaxY;
            //    if ( double.IsInfinity(pixelPerPointY) )
            //        return;

            //    try
            //    {
            //        if ( MaxShown != double.MaxValue )
            //        {
            //            pixelPerPointX = ( double ) width / ( MaxShown - MinShown );
            //        }
            //        else
            //            pixelPerPointX = ( double ) width / ( MaxX );
            //        if ( !SemaphoreWait(50) )
            //            return;
            //        Pixel b = barPixel;
            //        if ( Values.Count > 0 )
            //        {
            //            var valPre = Values[0];
            //            for ( var i = 0 ; i < Values.Count ; i++ )
            //            {
            //                if ( i > 1 )
            //                    valPre = Values[i - 1];
            //                var val = Values[i];

            //                if ( double.IsNaN(val.X) || double.IsNaN(val.Y) )
            //                    continue;
            //                if ( double.IsInfinity(val.X) || double.IsInfinity(val.Y) )
            //                    continue;
            //                if ( valPre.X > MaxShown )
            //                    break;
            //                if ( val.X < MinShown )
            //                    continue;
            //                Point p = new Point((int)((val.X - MinShown) * pixelPerPointX )+left,top+ (int)(height - val.Y * pixelPerPointY));
            //                g.FillEllipse(BarPixel , p.X - PointSize , p.Y - PointSize , PointSize * 2 , PointSize * 2);


            //                if ( Style == PlotStyle.Linked )
            //                {
            //                    if ( double.IsNaN(valPre.X) || double.IsNaN(valPre.Y) )
            //                        continue;
            //                    if ( double.IsInfinity(valPre.X) || double.IsInfinity(valPre.Y) )
            //                        continue;

            //                    Point pPre = new Point((int)((valPre.X - MinShown) * pixelPerPointX+left),top+ (int)(height - valPre.Y * pixelPerPointY));
            //                    g.DrawLine(BarPixel , pPre , p,(int)LineThickness);

            //                }
            //                else if ( Style == PlotStyle.FillTop )
            //                {
            //                    if ( double.IsNaN(valPre.X) || double.IsNaN(valPre.Y) )
            //                        continue;
            //                    if ( double.IsInfinity(valPre.X) || double.IsInfinity(valPre.Y) )
            //                        continue;

            //                    Point pPre = new Point((int)((valPre.X - MinShown) * pixelPerPointX+left),top+ (int)(height - valPre.Y * pixelPerPointY));

            //                    g.FillPolygon(fillPixel , new PointF[ ] { pPre , p , new Point(p.X , top) , new Point(pPre.X , top) });
            //                    g.DrawLine(BarPixel , pPre , p, (int)LineThickness);
            //                }
            //                else if ( Style == PlotStyle.FillBottom )
            //                {
            //                    if ( double.IsNaN(valPre.X) || double.IsNaN(valPre.Y) )
            //                        continue;
            //                    if ( double.IsInfinity(valPre.X) || double.IsInfinity(valPre.Y) )
            //                        continue;

            //                    Point pPre = new Point((int)((valPre.X - MinShown) * pixelPerPointX+left),top+ (int)(height - valPre.Y * pixelPerPointY));

            //                    g.FillPolygon(fillPixel , new PointF[ ] { pPre , p , new Point(p.X , ( int ) ( bottom )) , new Point(pPre.X , ( int ) ( bottom )) });
            //                    g.DrawLine(BarPixel , pPre , p, ( int ) LineThickness);
            //                }

            //            }

            //        }

            //    }
            //    catch { }
            //    SemaphoreRelease();
            //    g.ResetClip();
            //}
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
     

        public List<Charting2D.ChartData2D> Maximums(double deltaMin , double sample = 3 , double minVar = 0)
        {
            if ( Values.Count == 0 )
                return new List<Charting2D.ChartData2D>();
            if ( Values.Count == 1 )
                return new List<Charting2D.ChartData2D> { Values[0] };
            {
                List<Charting2D.ChartData2D> potentiels = new List<Charting2D.ChartData2D>();
                SemaphoreWait();
                for ( var i = 1 ; i < Values.Count - 1 ; i++ )
                {
                    if ( Values[i - 1].Y <= Values[i].Y && Values[i].Y >= Values[i + 1].Y )
                        if ( Values.Where((x , p) => p > i - sample && p < i + sample).Average((x) => x.Y) + minVar < Values[i].Y )
                            potentiels.Add(Values[i]);
                }

                List<Charting2D.ChartData2D> maxs = new List<Charting2D.ChartData2D>();
                if ( potentiels.Count > 0 )
                    maxs.Add(potentiels[0]);
                for ( var i = 1 ; i < potentiels.Count ; i++ )
                {
                    if ( potentiels[i - 1].X + deltaMin < potentiels[i].X )
                        maxs.Add(potentiels[i]);
                }
                SemaphoreRelease();
                return maxs;
            }
        }
        public List<Charting2D.ChartData2D> Minimums(double deltaMin , double sample = 3 , double minVar = 0)
        {
            if ( Values.Count == 0 )
                return new List<Charting2D.ChartData2D>();
            if ( Values.Count == 1 )
                return new List<Charting2D.ChartData2D> { Values[0] };
            {
                List<Charting2D.ChartData2D> potentiels = new List<Charting2D.ChartData2D>();
                SemaphoreWait();
                for ( var i = 1 ; i < Values.Count - 1 ; i++ )
                {
                    if ( Values[i - 1].Y >= Values[i].Y && Values[i].Y <= Values[i + 1].Y )
                        if ( Values.Where((x , p) => p > i - sample && p < i + sample).Average((x) => x.Y) - minVar > Values[i].Y )
                            potentiels.Add(Values[i]);
                }

                List<Charting2D.ChartData2D> maxs = new List<Charting2D.ChartData2D>();
                if ( potentiels.Count > 0 )
                    maxs.Add(potentiels[0]);
                for ( var i = 1 ; i < potentiels.Count ; i++ )
                {
                    if ( potentiels[i - 1].X + deltaMin < potentiels[i].X )
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
            double max = this.Values[this.Values.Count-1].X;
            double loc = this.Values[0].X;
            while ( loc < max )
            {
                var avg = this.Values.Where((x)=>x.X > loc-halfStep && x.X < loc +halfStep).Average((x)=>x.Y);
                var addData =this.Values.Where((x)=>x.X > loc-halfStep && x.X < loc +halfStep).First().AdditionnalData;
                ret.ValuesAdd(new Charting2D.ChartData2D(loc+halfStep , avg) { AdditionnalData = addData });
                loc += smooth;
            }
            ret.Values.Add(this.Values[this.Values.Count - 1]);
            SemaphoreRelease();
            return ret;
        }

        public Charting2D.ChartData2D MouseToGraphCoord(PlotChart c , Point mouse)
        {
            throw new NotImplementedException();
            //var height = c.Height - Margin.Vertical;
            //var width = c.Width - Margin.Horizontal;
            //var left = Margin.Left;
            //var top = Margin.Top;
            //var bottom = top+height;
            //var right = left+width;
            //double pixelPerPointX=0;
            //double pixelPerPointY = ( double )height / c.MaxY;

            //if ( Values.Count > 1 || MaxShown != double.MaxValue )
            //{


            //    if ( double.IsInfinity(pixelPerPointY) )
            //        return new Charting2D.ChartData2D(double.NaN , double.NaN);
            //    if ( MaxShown != double.MaxValue )
            //    {
            //        pixelPerPointX = ( double ) width / ( MaxShown - MinShown );
            //    }
            //    else
            //        pixelPerPointX = ( double ) width / ( MaxX );

            //}

            //var ex = mouse.X-Margin.Left;
            //var ey = (c.Height - mouse.Y)-Margin.Bottom;

            //var px = ex/pixelPerPointX;
            //var py = ey/pixelPerPointY;

            //return new Charting2D.ChartData2D(px , py);

        }

        public PlotValues Scale(int v)
        {
            var max= Values.Max((x)=>x.X);
            var factor = v/max;

            var ret = this.Clone();
            foreach ( var val in ret.Values )
            {
                val.X *= factor;
            }
            return ret;
        }

        public Point GraphToMouseCoord(PlotChart c , Charting2D.ChartData2D pg)
        {
            //var height = c.Height - Margin.Vertical;
            //var width = c.Width - Margin.Horizontal;
            //var left = Margin.Left;
            //var top = Margin.Top;
            //var bottom = top+height;
            //var right = left+width;
            //double pixelPerPointX=0;
            //double pixelPerPointY = ( double )height / c.MaxY;

            //if ( Values.Count > 1 || MaxShown != double.MaxValue )
            //{


            //    if ( double.IsInfinity(pixelPerPointY) )
            //        return new Point(int.MinValue , int.MinValue);
            //    if ( MaxShown != double.MaxValue )
            //    {
            //        pixelPerPointX = ( double ) width / ( MaxShown - MinShown );
            //    }
            //    else
            //        pixelPerPointX = ( double ) width / ( MaxX );

            //}
            //var ex = pg.X * pixelPerPointX;
            //var ey = pg.Y * pixelPerPointY;
            //var px = ex + Margin.Left;
            //var py = c.Height - Margin.Bottom - ey;
            //return new Point(( int ) px , ( int ) py);
            throw new NotImplementedException();
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
