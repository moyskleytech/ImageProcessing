using ImageProcessing.PNGCodec;
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
using System.IO;
using Hjg.Pngcs;

namespace PDFCodecTest
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender , EventArgs e)
        {
            BitmapFactory.RegisterCodec(new PngCodec());
        }

        private void Button1_Click(object sender , EventArgs e)
        {
            OpenFileDialog ofd = new OpenFileDialog();
            PngCodec codec = new PngCodec();
            ofd.Filter = "*.png|*.png";
            if ( ofd.ShowDialog() == DialogResult.OK )
            {
                var fs = ofd.OpenFile();
                //var fs = new MemoryStream(File.ReadAllBytes(ofd.FileName));
                var bmp1 = new BitmapFactory( ).Decode(fs);
                //var bmp1 = codec.DecodeStream(fs);
                var fs2 = System.IO.File.Open(ofd.FileName+"2.png", System.IO.FileMode.OpenOrCreate, System.IO.FileAccess.ReadWrite);
                codec.Save<Pixel>(bmp1 , fs2);

                //TestRawPng(fs2);

                fs2.Flush();
                fs2.Close();

                pictureBox1.Image = bmp1.ToWinFormBitmap();

                //var bmp2 = codec.DecodeStream(fs2);
                //pictureBox2.Image = bmp2.ToWinFormBitmap();
            }
        }

        private void TestRawPng(FileStream fs2)
        {
            PngWriter writer = new PngWriter(fs2, new ImageInfo(10,10,8,true));

            for ( var i = 0; i < 10; i++ )
                writer.WriteRowByte(new byte[ ] {
                    255,0,0,0,
                    255 ,10,10,10,
                    255 ,30,30,30,
                    255 ,40,40,40,
                    255 ,50,50,50,
                    255 ,70,70,70,
                    255 ,90,90,90,
                    255 ,100,100,100,
                    255 ,150,150,150,
                    255 ,200,200,200
                } , i);
            writer.End();
        }
    }
}
