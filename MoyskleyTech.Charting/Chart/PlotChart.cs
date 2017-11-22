using System;
using System.Collections.Generic;
using System.Collections;
using System.ComponentModel;
using System.Diagnostics;
using MoyskleyTech.ImageProcessing.Image;
using System.Windows;
using System.Linq;
using System.Text;

namespace MoyskleyTech.Charting
{
    public class PlotChart
    {
        private Bitmap bmp;
        private double max = 100;
        public List<Plot> Plots;
        public Font Font { get; set; } = BaseFonts.Premia;
        public int FontSize { get; set; } = 1;
        public ChartMode Mode { get; set; } = ChartMode.Intelligent;


        public PlotChart(Bitmap bmp)
        {
            BackPixel = Pixels.LightGray;
            Plots = new List<Plot>();
            this.bmp = bmp;
        }
        public Bitmap Draw()
        {
            Graphics g = Graphics.FromImage(bmp);
            Draw(g , bmp.Width , bmp.Height);
            g.Dispose();
            return bmp;
        }

        public void Draw(Graphics g , int w , int h)
        {
            g.FillRectangle(BackPixel,0,0,w,h);
            if ( Plots.Count > 0 )
            {
                max = MaxY;

                double minX = MinX;
                double minY = MinY;
                double maxX = MaxX;
                double maxY = MaxY;

                foreach ( Plot p in Plots )
                {
                    if ( p.Visible )
                        p.Draw(this , g , w , h);
                    g.ResetTransform();
                    g.ResetClip();
                }
            }
        }


        internal double MinX
        {
            get
            {
                /*  var activePlots = Plots.Where((x) => x.Visible);
                  if(activePlots.Count()>0)
                      return activePlots.Min((x) => x.MinX);*/
                return 0;
            }
        }
        double maxx;
        public double MaxX
        {
            get
            {
                if ( Mode == ChartMode.Intelligent )
                {
                    var activePlots = Plots.Where((x) => x.Visible);
                    if ( activePlots.Count() > 0 )
                        return maxx = activePlots.Max((x) => x.MaxX);
                    return maxx = 0;
                }
                else
                {
                    return maxx;
                }
            }
            set
            {
                maxx = value;
            }
        }

        internal double MinY
        {
            get
            {
                /*  var activePlots = Plots.Where((x) => x.Visible);
                  if (activePlots.Count() > 0)
                      return activePlots.Min((x) => x.MinY);*/
                return 0;
            }
        }
        double maxy;
        public double MaxY
        {
            get
            {
                if ( Mode == ChartMode.Intelligent )
                {
                    var activePlots = Plots.Where((x) => x.Visible);
                    if ( activePlots.Count() > 0 )
                        return maxy = activePlots.Max((x) => x.MaxY);
                    return maxy = 0;
                }
                else
                {
                    return maxy;
                }
            }
            set
            {
                maxy = value;
            }
        }

        public Pixel BackPixel { get; set; }
        public int Height { get { return bmp.Height; } }
        public int Width { get { return bmp.Width; } }
    }


}
