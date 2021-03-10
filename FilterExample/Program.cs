using MoyskleyTech.ImageProcessing.Image;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using MoyskleyTech.ImageProcessing.Windows.Forms;
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
            OpenFileDialog ofd = new OpenFileDialog
            {
                Filter = "Image Files(*.BMP;*.PNG;*.JPG)|*.BMP;*.PNG;*.JPG"
            };
            if ( ofd.ShowDialog() == DialogResult.OK )
            {
                var fs = ofd.OpenFile();
                Image<Pixel> bmp = new BitmapFactory().Decode(fs);
                Image<Pixel> ori = bmp.Clone();
                fs.Dispose();

                ShowImage(bmp , "original");

                Squarify.Apply(bmp.Proxy(new Rectangle(10 , 10 , bmp.Width - 20 , bmp.Height - 20)) , 20);

                ShowImage(bmp , "squarified");

                bmp = ori.Clone();
                ColorReducer.FromKMeans(bmp , 10);

                ShowImage(bmp , "color reduced to 10");

                //bmp = ori.Clone();
                //ColorReducer.FromKMeans(bmp , 20);

                //ShowImage(bmp , "color reduced to 20");
                //bmp = ori.Clone();
                //ColorReducer.FromKMeans(bmp , 128);

                //ShowImage(bmp , "color reduced to 128");
                bmp = ( Bitmap ) ori.Clone();
                ColorReducer.FromKMeans(bmp , 512);

                ShowImage(bmp , "color reduced to 512");
            }
        }
        public static void ShowImage(ImageProxy<Pixel> bmp , string name = "")
        {
            new Form() {
                Width = bmp.Width,
                Height = bmp.Height,
                Text = name,
                BackgroundImageLayout = ImageLayout.Stretch,
                BackgroundImage = bmp.ToWinFormBitmap(),
            }.ShowDialog();
        }
    }
}
