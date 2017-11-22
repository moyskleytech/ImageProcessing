using System;
using System.Collections.Generic;
using System.Collections;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.Windows;
using System.Windows.Forms;
using System.Linq;
using System.Text;

namespace LMST.Sportif.DLL.Windows.Controls
{
    public class PlotChart : System.Windows.Forms.Control
    {
        
        private double max = 100;
        public List<Plot> Plots;
        public ChartMode Mode { get; set; } = ChartMode.Intelligent;
        public BorderStyle BorderStyle
        {
            get; set;
        }
       
        public PlotChart()
        {
           BackColor = Color.LightGray;
           
            Plots = new List<Plot>();
            DoubleBuffered = true;
            SetStyle(ControlStyles.OptimizedDoubleBuffer, true);

        }
        void ValueChanged()
        {

            Invalidate();
        }
        public PlotChart(IContainer container)
        {
            container.Add(this);
            BackColor = Color.LightGray;
            
            Plots = new List<Plot>();
            DoubleBuffered = true;
            SetStyle(ControlStyles.OptimizedDoubleBuffer, true);
        }
        protected override void OnResize(EventArgs e)
        {
            base.OnResize(e);
            this.Refresh();
        }
        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);
            // System.Drawing.Graphics g = Graphics.FromImage(bmp); ;
            Draw(e.Graphics,this.Width,this.Height);
            //e.Graphics.DrawImage(bmp, 0, 0, this.Width, this.Height);
        }

        public void Draw(Graphics g,int w,int h)
        {
            g.Clear(BackColor);
            if ( BorderStyle != System.Windows.Forms.BorderStyle.None )
                g.DrawRectangle(Pens.Black , 0 , 0 , Width - 1 , Height - 1);
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
                        p.Draw(this , g,w,h);
                    g.ResetTransform();
                    g.ResetClip();
                }
            }
        }

        protected override void OnPrint(PaintEventArgs e)
        {
            OnPaint(e);
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
            get {
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

        protected override void OnSizeChanged(EventArgs e)
        {
            base.OnSizeChanged(e);
            Invalidate();
        }
        protected override void OnBackColorChanged(EventArgs e)
        {
            base.OnBackColorChanged(e);
            Invalidate();
        }
        protected override void OnFontChanged(EventArgs e)
        {
            base.OnFontChanged(e);
            Invalidate();
        }

      

    }


}
