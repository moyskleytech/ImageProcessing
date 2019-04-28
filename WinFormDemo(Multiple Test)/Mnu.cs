using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using MoyskleyTech.ImageProcessing.Image;

namespace WinFormDemo_Multiple_Test_
{
    public partial class Mnu : Form
    {
        public Mnu()
        {
            InitializeComponent();
        }

        private void Button1_Click(object sender , EventArgs e)
        {
            ( new RPG() ).ShowDialog();
        }

        private void Button2_Click(object sender , EventArgs e)
        {
            ( new Form1() ).ShowDialog();
        }

        private void Button3_Click(object sender , EventArgs e)
        {
            ( new SpeedTest() ).ShowDialog();
        }

        private void button4_Click(object sender , EventArgs e)
        {
            ( new CustomForms() ).ShowDialog();
        }

        private void button5_Click(object sender , EventArgs e)
        {
            ( new Maze() ).ShowDialog();
        }

        private void button6_Click(object sender , EventArgs e)
        {
            ( new Scaling() ).ShowDialog();
        }

        private void btn4_Click(object sender , EventArgs e)
        {
            ( new Charting3() ).ShowDialog();
        }

        private void Mnu_Load(object sender , EventArgs e)
        {
            Pixel p = Pixels.DeepPink;
            var f = ColorConvert.Convert<Pixel , ARGB_Float>(p);
            var b  =ColorConvert.Convert<ARGB_Float, Pixel>(f);
        }

        private void Button7_Click(object sender, EventArgs e)
        {
            (new PieChartTest()).ShowDialog();
        }
    }
}
