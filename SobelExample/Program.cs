using MoyskleyTech.ImageProcessing.Image;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using MoyskleyTech.ImageProcessing.Recognition.Border;
using MoyskleyTech.ImageProcessing.Recognition.Shape;
using MoyskleyTech.ImageProcessing.WinForm;
namespace SobelExample
{
    public class Program
    {
        [STAThread]
        public static void Main(string[ ] args)
        {
            ImageProcessing.PNGCodec.PngCodec.Register();
            ImageProcessing.JPEGCodec.JPEGCodec.Register();
            OpenFileDialog ofd = new OpenFileDialog
            {
                Filter = "Image Files(*.BMP;*.PNG;*.JPG)|*.BMP;*.PNG;*.JPG"
            };
            if ( ofd.ShowDialog() == DialogResult.OK )
            {
                var fs = ofd.OpenFile();
                Bitmap bmp = new BitmapFactory().Decode(fs);
                fs.Dispose();
                Bitmap bmp2=bmp;
                if ( bmp.Width > 800 || bmp.Height > 800 )
                {
                    double ratio = (double)bmp.Width/bmp.Height;
                    var new_w = 800*ratio;
                    if ( new_w <= 800 )
                        bmp2 = bmp.Resize((int)new_w , 800 , ScalingMode.AverageInterpolate);
                    else
                    {
                        ratio = ( double ) bmp.Height / bmp.Width;
                        var new_h = 800*ratio;
                        bmp2 = bmp.Resize(800 , ( int ) new_h , ScalingMode.AverageInterpolate);
                    }
                }
                if ( bmp2 != bmp )
                {
                    bmp.Dispose();
                    bmp = bmp2;
                }
                //bmp.SetGrayscale();
                //bmp.Blur(2);

                var sobelled = Sobel.ConvolutionFilter(bmp, Sobel.xSobel,Sobel.ySobel,2,0);
                //sobelled.Blur(2);
                byte threashold=96;
                //sobelled.ApplyFilter(Filters.Invert());
                sobelled.ApplyFilter(Filters.Max());
                sobelled.ApplyFilter(Filters.Threashold(threashold));
                ShowImage(bmp , "Original");
                ShowImage(sobelled , "Sobel");
            }
        }
        public static void ShowImage(Bitmap bmp , string name = "")
        {
            new Form()
            {
                Width = bmp.Width ,
                Height = bmp.Height ,
                Text = name ,
                BackgroundImage = bmp.ToWinFormBitmap()
            }.ShowDialog();
        }
    }
}
