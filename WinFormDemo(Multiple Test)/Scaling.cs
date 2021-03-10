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
    public partial class Scaling : Form
    {
        Bitmap bmp;
        public Scaling()
        {
            InitializeComponent();
        }

        private void pictureBox1_Click(object sender , EventArgs e)
        {
            OpenFileDialog ofd=new OpenFileDialog();
            if ( ofd.ShowDialog() == DialogResult.OK )
            {
                bmp = new Bitmap(( ofd.FileName ));
                var bmpt = new Bitmap(bmp);
                bmp.Dispose();

                MoyskleyTech.ImageProcessing.Image.Image<MoyskleyTech.ImageProcessing.Image.BGR> ubmp = bmpt.ToBGR();
                ubmp=ubmp.Rescale(8 , 6 , MoyskleyTech.ImageProcessing.Image.ScalingMode.AverageInterpolate);
                pictureBox1.Image = ubmp.ToWinFormBitmap();

                pictureBox1.Image.Save(ofd.FileName + ".png", System.Drawing.Imaging.ImageFormat.Png);

            }
        }
    }
}
