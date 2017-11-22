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
    public class AdvancedPlotValues : PlotValues
    {
       
        public Func<PointGraphique,Pixel> DataToPixel;
        public Func<PointGraphique,Pixel> DataToFillPixel;

        public AdvancedPlotValues():base()
        {
            DataToPixel = (x) => { return BarPixel; };
            DataToFillPixel = (x) => { return FillPixel; };
        }
       
        public override PlotValues Clone()
        {
            AdvancedPlotValues ret = new AdvancedPlotValues();
            semSync.WaitOne();
            ret.Values = this.Values.ToList();
            ret.BarPixel = this.BarPixel;
            ret.Style = this.Style;
            ret.Visible = this.Visible;
            ret.LineThickness = this.LineThickness;
            ret.DataToPixel = this.DataToPixel;
            semSync.Release();
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
                    if ( !semSync.WaitOne(50) )
                        return;
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
                                continue;
                            if ( val.Absisce < MinShown )
                                continue;

                            barPixel = DataToPixel(val);
                            fillPixel = DataToFillPixel(val);

                            Point p = new Point((int)((val.Absisce - MinShown) * pixelPerPointX )+left,top+ (int)(height - val.Ordonee * pixelPerPointY));
                            if ( PointSize > 0 )
                                g.FillCircle(barPixel , p.X - PointSize , p.Y - PointSize , PointSize);


                            if ( Style == PlotStyle.Linked )
                            {
                                if ( double.IsNaN(valPre.Absisce) || double.IsNaN(valPre.Ordonee) )
                                    continue;
                                if ( double.IsInfinity(valPre.Absisce) || double.IsInfinity(valPre.Ordonee) )
                                    continue;

                                Point pPre = new Point((int)((valPre.Absisce - MinShown) * pixelPerPointX+left),top+ (int)(height - valPre.Ordonee * pixelPerPointY));
                                g.DrawLine(barPixel , pPre , p,(int)LineThickness);

                            }
                            else if ( Style == PlotStyle.FillTop )
                            {
                                if ( double.IsNaN(valPre.Absisce) || double.IsNaN(valPre.Ordonee) )
                                    continue;
                                if ( double.IsInfinity(valPre.Absisce) || double.IsInfinity(valPre.Ordonee) )
                                    continue;

                                Point pPre = new Point((int)((valPre.Absisce - MinShown) * pixelPerPointX+left),top+ (int)(height - valPre.Ordonee * pixelPerPointY));

                                g.FillPolygon(fillPixel , new PointF[ ] { pPre , p , new Point(p.X , top) , new Point(pPre.X , top) });
                                g.DrawLine(barPixel , pPre , p,(int)LineThickness);
                            }
                            else if ( Style == PlotStyle.FillBottom )
                            {
                                if ( double.IsNaN(valPre.Absisce) || double.IsNaN(valPre.Ordonee) )
                                    continue;
                                if ( double.IsInfinity(valPre.Absisce) || double.IsInfinity(valPre.Ordonee) )
                                    continue;

                                Point pPre = new Point((int)((valPre.Absisce - MinShown) * pixelPerPointX+left),top+ (int)(height - valPre.Ordonee * pixelPerPointY));

                                g.FillPolygon(fillPixel , new PointF[ ] { pPre , p , new Point(p.X , ( int ) ( bottom )) , new Point(pPre.X , ( int ) ( bottom )) });
                                g.DrawLine(barPixel , pPre , p,(int)LineThickness);
                            }

                        }


                    }
                }
                catch { }
                semSync.Release();
                g.ResetClip();
            }
        }
    }
}
