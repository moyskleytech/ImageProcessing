using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using MoyskleyTech.ImageProcessing.WinForm;
using MoyskleyTech.ImageProcessing.Image;
using Bitmap = System.Drawing.Bitmap;
using Graphics = System.Drawing.Graphics;
using Braphics = MoyskleyTech.ImageProcessing.Image.Graphics;
using Rectangle = MoyskleyTech.ImageProcessing.Image.Rectangle;
using Font = MoyskleyTech.ImageProcessing.Image.Font;
using MoyskleyTech.Charting.Charting3D;
using MoyskleyTech.ImageProcessing.Drawing3D;
namespace WinFormDemo_Multiple_Test_
{
    public partial class Charting3 : Form
    {
        private Bitmap bmp;
        private Bitmap bmpTampon;
        private NativeGraphicsWrapper graphicsTampon;
        private Graphics graphics;
        private string txt;
        private bool InButton=false;
        private bool ButtonClicked=false;
        private Viewport3D viewport;
        private Plot3D chart;
        private Plot3D chart2;
        public Charting3()
        {
            InitializeComponent();
            this.SetStyle(ControlStyles.OptimizedDoubleBuffer , true);
        }

        private void RPG_Load(object sender , EventArgs e)
        {
            viewport = new Viewport3D();
            chart = new Plot3D();
            chart2 = new Plot3D();
            viewport.Items.Add(chart);
            viewport.Items.Add(chart2);
            chart.Style = Plot3DStyle.Linked;

            List<ChartData3D>[] lsts = new List<ChartData3D>[3]{ new List<ChartData3D>(),new List<ChartData3D>(),new List<ChartData3D>() };
            for ( double α = 0; α < 2 * Math.PI; α += 0.2 )
            {
                var x = Math.Cos(α);
                var y = Math.Sin(α);
                lsts[0].Add(new ChartData3D(x , y , 0));
                lsts[1].Add(new ChartData3D(x , 0 , y));
            }
            lsts[0].Add(new ChartData3D(1 , 0 , 0));
            lsts[1].Add(new ChartData3D(1 , 0 , 0));
            chart.Data.AddRange(lsts[0]);
            chart.Data.AddRange(lsts[1]);
            chart.Data.AddRange(lsts[2]);
            //for ( double z = 0; z < 1; z += 0.18 )
            //{
            //    //x2+y2=r2
            //    //y2=1-x2
            //    var radiusForZ = Math.Sqrt(1-z*z);
            //    for ( double r = 0; r < Math.PI; r += 0.1 )
            //    {
            //        chart2.Data.Add(new ChartData3D(Math.Cos(r) * radiusForZ , Math.Sin(r) * radiusForZ , z));
            //        chart2.Data.Add(new ChartData3D(Math.Cos(r) * radiusForZ , -Math.Sin(r) * radiusForZ , z));
            //        chart2.Data.Add(new ChartData3D(Math.Cos(r) * radiusForZ , Math.Sin(r) * radiusForZ , -z));
            //        chart2.Data.Add(new ChartData3D(Math.Cos(r) * radiusForZ , -Math.Sin(r) * radiusForZ , -z));
            //    }
            //}
            for ( double β = 0; β < 2 * Math.PI; β += 0.2 )
                for ( double α = 0; α < 2 * Math.PI; α += 0.2 )
                {
                    var x = Math.Cos(β) * Math.Sin(α);
                    var y = Math.Cos(β)*Math.Cos(α);
                    var z = Math.Sin(β);
                    chart2.Data.Add(new ChartData3D(x , y , z));
                }

            chart2.PointColor = Pixels.DeepPink;

            viewport.Font = new Font("Arial");
            viewport.LinePointCount = 0;

            InitGraphics();
            CreateFrame();
            DisplayFrame();
        }

        private void InitGraphics()
        {
            bmp?.Dispose();
            bmpTampon?.Dispose();
            graphics?.Dispose();
            graphicsTampon?.Dispose();

            bmp = new Bitmap(pbRPG.Width , pbRPG.Height);
            bmpTampon = new Bitmap(pbRPG.Width , pbRPG.Height);
            graphics = Graphics.FromImage(bmp);
            graphicsTampon = new NativeGraphicsWrapper(Graphics.FromImage(bmpTampon));
        }


        private void CreateFrame()
        {
            graphicsTampon.Clear(Pixels.White);
            viewport.Render(graphicsTampon , bmpTampon.Width , bmpTampon.Height);
        }



        private void DisplayFrame()
        {
            graphics.DrawImage(bmpTampon , 0 , 0 , pbRPG.Width , pbRPG.Height);
            pbRPG.Image = bmp;
            this.Invalidate();
        }

        private void RPG_KeyPress(object sender , KeyPressEventArgs e)
        {
            var c = e.KeyChar;
            if ( c == '\r' )
                txt += '\n';
            else if ( c == '\b' )
                txt = txt.Substring(0 , txt.Length - 1);
            else txt += c;

            CreateFrame();
            DisplayFrame();
        }

        private void RPG_Resize(object sender , EventArgs e)
        {
            InitGraphics();
            CreateFrame();
            DisplayFrame();
        }

        private void pbRPG_MouseDown(object sender , MouseEventArgs e)
        {
            lx = e.X; ly = e.Y;
            CreateFrame();
            DisplayFrame();
        }

        int lx,ly;
        private void pbRPG_MouseMove(object sender , MouseEventArgs e)
        {
            if ( e.Button == MouseButtons.Left )
            {
                viewport.CursorMove(-e.X + lx , +e.Y - ly);
            }
            CreateFrame();
            DisplayFrame();
            lx = e.X; ly = e.Y;
        }

        private void pbRPG_Click(object sender , EventArgs e)
        {
            OpenFileDialog ofd = new OpenFileDialog();
            if ( ofd.ShowDialog() == DialogResult.OK )
            {
                var fs = ofd.OpenFile();
                var image = new BitmapFactory().Decode(fs).Rescale(20,20, ScalingMode.AverageInterpolate);
                fs.Dispose();
                Image3D<Pixel> image3D = new Image3D<Pixel>(){ Image=image, Corners=new Point3D[]{ new Point3D(-1,-1,0), new Point3D(-1 , 1 , 0), new Point3D(1 , -1 , 0), new Point3D(1 , 1 , 0) } };
                viewport.Items.Add(image3D);
            }
        }

        private void pbRPG_MouseUp(object sender , MouseEventArgs e)
        {
            CreateFrame();
            DisplayFrame();
        }


    }
}
