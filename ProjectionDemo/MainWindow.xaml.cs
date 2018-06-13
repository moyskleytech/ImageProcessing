using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using MoyskleyTech.ImageProcessing.Image;
using MoyskleyTech.ImageProcessing.WPF;
namespace ProjectionDemo
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        int width = 190;
        int height = 300;
        int pointIdx=0;
        System.Windows.Point[] pts = new System.Windows.Point[4];
        BitmapImage bmp;
        public MainWindow()
        {
            InitializeComponent();
        }

        private void SetProjectPoint(object sender , MouseButtonEventArgs e)
        {
            var pos = e.GetPosition(imgDst);
            pts[pointIdx++] = pos;
            if ( pointIdx == 4 )
            {
                pointIdx = 0;
                var mbmp = new Bitmap(width , height);
                Graphics g = Graphics.FromImage(mbmp);
                g.Clear(Pixels.Black);
                g.ProjectTransform(
                    new PointF[ ] {
                    new PointF(0,0),
                    new PointF(0,height),
                    new PointF(width,0),
                    new PointF(width , height)
                    } ,
                    new PointF[ ] {
                    new PointF(pts[0].X,pts[0].Y),
                    new PointF(pts[1].X,pts[1].Y),
                    new PointF(pts[2].X,pts[2].Y),
                    new PointF(pts[3].X,pts[3].Y)
                    });
                
                g.DrawImage(bmp.ToBitmap() , 0 , 0);

                imgDst.Source = mbmp.ToWPFBitmap();
            }
        }

        private void SelectNewImage(object sender , MouseButtonEventArgs e)
        {
            OpenFileDialog ofd=new OpenFileDialog();
            if ( ofd.ShowDialog() == true )
            {
                bmp = new BitmapImage(new Uri(ofd.FileName));
                var mbmp = bmp.ToBitmap();
                mbmp = (Bitmap)mbmp.Rescale(width , height , ScalingMode.AverageInterpolate);
                bmp = mbmp.ToWPFBitmap();
                imgSrc.Source = bmp;
            }
        }
    }
}
