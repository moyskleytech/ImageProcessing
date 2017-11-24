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
            OpenFileDialog ofd = new OpenFileDialog();
            ofd.Filter = "Image Files(*.BMP;*.PNG;*.JPG)|*.BMP;*.PNG;*.JPG";
            if ( ofd.ShowDialog() == DialogResult.OK )
            {
                var fs = ofd.OpenFile();
                Bitmap bmp = new BitmapFactory().Decode(fs);
                //bmp.SetGrayscale();
                //bmp.Blur(2);
                fs.Dispose();
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
            Form f = new Form();
            f.Width = bmp.Width;
            f.Height = bmp.Height;
            f.Text = name;
            f.BackgroundImage = bmp.ToWinFormBitmap();
            f.ShowDialog();
        }
    }
}
