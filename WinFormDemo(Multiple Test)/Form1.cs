using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using MoyskleyTech.ImageProcessing.Windows.Forms;

namespace WinFormDemo_Multiple_Test_
{
    public partial class Form1 : Form
    {
        Bitmap bmp;
        int pointIdx=0;
        Point[] pts = new Point[4];
        public Form1()
        {
            InitializeComponent();
        }

        private void ImgSrc_Click(object sender , EventArgs e)
        {
            OpenFileDialog ofd=new OpenFileDialog();
            if ( ofd.ShowDialog() ==  DialogResult.OK )
            {
                bmp = new Bitmap((ofd.FileName));
                var bmpt = new Bitmap(bmp , imgSrc.Width , imgSrc.Height);
                bmp.Dispose();
            }
        }

        private void ImgDest_MouseUp(object sender , MouseEventArgs e)
        {
            var pos = e.Location;
            pts[pointIdx++] = pos;
            if ( pointIdx == 4 )
            {
                pointIdx = 0;
                var mbmp = new MoyskleyTech.ImageProcessing.Image.Bitmap(imgDest.Width , imgDest.Height);

                MoyskleyTech.ImageProcessing.Image.Graphics g = MoyskleyTech.ImageProcessing.Image.Graphics.FromImage(mbmp);
                g.Clear(MoyskleyTech.ImageProcessing.Image.Pixels.Black);
                g.ProjectTransform(
                    new MoyskleyTech.ImageProcessing.Image.PointF[ ] {
                    new MoyskleyTech.ImageProcessing.Image.PointF(0,0),
                    new MoyskleyTech.ImageProcessing.Image.PointF(0,imgDest.Height),
                    new MoyskleyTech.ImageProcessing.Image.PointF(imgDest.Width,0),
                    new MoyskleyTech.ImageProcessing.Image.PointF(imgDest.Width , imgDest.Height)
                    } ,
                    new MoyskleyTech.ImageProcessing.Image.PointF[ ] {
                    new MoyskleyTech.ImageProcessing.Image.PointF(pts[0].X,pts[0].Y),
                    new MoyskleyTech.ImageProcessing.Image.PointF(pts[1].X,pts[1].Y),
                    new MoyskleyTech.ImageProcessing.Image.PointF(pts[2].X,pts[2].Y),
                    new MoyskleyTech.ImageProcessing.Image.PointF(pts[3].X,pts[3].Y)
                    });

                var mat = g.TransformationMatrix;
               // g.TransformationMatrix = mat.Transposee;
                
                g.DrawImage(bmp.ToBitmap() , 0 , 0);

                imgDest.Image = mbmp.ToWinFormBitmap();
            }
        }
    }
}
