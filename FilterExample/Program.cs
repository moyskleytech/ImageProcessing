using MoyskleyTech.ImageProcessing.Image;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using MoyskleyTech.ImageProcessing.WinForm;
using MoyskleyTech.ImageProcessing.Filters;

namespace FilterExample
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
                Bitmap ori = bmp.Clone();
                fs.Dispose();

                ShowImage(bmp , "original");

                Squarify.Apply(bmp.Proxy(new Rectangle(10 , 10 , bmp.Width - 20 , bmp.Height - 20)) , 20);

                ShowImage(bmp , "squarified");

                bmp = ori.Clone();
                ColorReducer.FromKMeans(bmp , 10);

                ShowImage(bmp , "color reduced to 10");

                bmp = ori.Clone();
                ColorReducer.FromKMeans(bmp , 20);

                ShowImage(bmp , "color reduced to 20");
                bmp = ori.Clone();
                ColorReducer.FromKMeans(bmp , 128);

                ShowImage(bmp , "color reduced to 128");
                bmp = ori.Clone();
                ColorReducer.FromKMeans(bmp , 512);

                ShowImage(bmp , "color reduced to 512");
            }
        }
        public static void ShowImage(Bitmap bmp , string name = "")
        {
            Form f = new Form();
            f.Width = bmp.Width;
            f.Height = bmp.Height;
            f.Text = name;
            f.BackgroundImageLayout = ImageLayout.Stretch;
            f.BackgroundImage = bmp.ToWinFormBitmap();
            f.ShowDialog();
        }
    }
}
