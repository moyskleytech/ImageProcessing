using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using MoyskleyTech.ImageProcessing.Image;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace WinFormDemo_Multiple_Test_
{
    public partial class SpeedTest : Form
    {
        public SpeedTest()
        {
            InitializeComponent();
        }

        private void Do_Click(object sender , EventArgs e)
        {
            label1.Text = "";
            Bitmap bitmap = new Bitmap(100,100);
            Image<Pixel> pxlImage = (Image<Pixel>)bitmap;

            DateTime begin=DateTime.Now;
            HSBImage hsbImage = bitmap.ToHSB();
            TimeSpan timeBMPtoHSB = DateTime.Now-begin;
            label1.Text  += "Bitmap to HSBImage "+timeBMPtoHSB+"\r\n";
            begin = DateTime.Now;
            Image<HSB> imgHSB = pxlImage.ConvertTo<HSB>();
            TimeSpan timeIMGPXLtoIMGHSB = DateTime.Now-begin;
            label1.Text += "PixelImage to Image<HSB> " + timeIMGPXLtoIMGHSB + "\r\n";
            begin = DateTime.Now;
            Image<ARGB_Float> img128bpp = pxlImage.ConvertTo<ARGB_Float>();
            TimeSpan timeIMGPXLtoIMG128bpp = DateTime.Now-begin;
            label1.Text += "PixelImage to Image<ARGB_Float> " + timeIMGPXLtoIMG128bpp + "\r\n";

            begin = DateTime.Now;
            var g1 = Graphics.FromImage(bitmap);
            g1.Clear(Pixels.Black);
            TimeSpan timeClearBitmap = DateTime.Now-begin;
            label1.Text += "Time To Clear Bitmap " + timeClearBitmap + "\r\n";

            begin = DateTime.Now;
            var g2 = Graphics<Pixel>.FromImage(pxlImage);
            g2.Clear(Pixels.Black);
            TimeSpan timeClearImagePixel = DateTime.Now-begin;
            label1.Text += "Time To Clear PixelImage " + timeClearImagePixel + "\r\n";

            begin = DateTime.Now;
            var g3 = Graphics<Pixel>.FromImage(pxlImage);
            g3.Clear(ColorConvert.Convert<_332,Pixel>(new SolidBrush<_332>(new _332() {_332_=0b10010010 })));
            TimeSpan timeConvertedBrush = DateTime.Now-begin;
            label1.Text += "Time To Clear PixelImage using ConvertedBrush " + timeConvertedBrush + "\r\n";

        }
    }
}
