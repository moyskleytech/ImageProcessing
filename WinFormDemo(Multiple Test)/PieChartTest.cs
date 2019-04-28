using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using MoyskleyTech.ImageProcessing.WinForm;
using MoyskleyTech.ImageProcessing.Image;
using MoyskleyTech.Charting.Chart;

namespace WinFormDemo_Multiple_Test_
{
    public partial class PieChartTest : Form
    {
        Bitmap bmp;
        PieChart pie;
        public PieChartTest()
        {
            InitializeComponent();
        }

        private void pictureBox1_Click(object sender , EventArgs e)
        {
            
        }

        private void PieChartTest_Load(object sender, EventArgs e)
        {
            bmp = new Bitmap(pictureBox2.Width, pictureBox2.Height);
            Graphics.FromImage(bmp).Clear(Pixels.Black);
            pie = new PieChart();
            pie.Mode = PieChartMode.Pie3D;
            pie.BackPixel = new SolidBrush<Pixel>(Pixels.Black);
            pie.Datas = new PieData[] {
                new PieData(){ Name="a",Weight=4},
                new PieData(){ Name="b",Weight=4},
                new PieData(){ Name="c",Weight=8}
            };

           var a = (PieChartMode[])Enum.GetValues(typeof(PieChartMode));

            comboBox1.Items.AddRange(a.Cast<object>().ToArray());
            comboBox1.SelectedValueChanged += ComboBox1_SelectedValueChanged;
            comboBox1.SelectedIndex = 0;

            propertyGrid1.SelectedObject = pie;
        }

        private void ComboBox1_SelectedValueChanged(object sender, EventArgs e)
        {
            pie.Mode = (PieChartMode)comboBox1.SelectedItem;
            Draw();
        }


        private void Draw()
        {
            var g = new NativeGraphicsWrapper(pictureBox1.CreateGraphics());
            g.Clear(Pixels.Black);
            pie.Draw(g, pictureBox1.Width, pictureBox1.Height);
            g.Dispose();
            pie.Draw(Graphics.FromImage(bmp), pictureBox2.Width, pictureBox2.Height);

            var old = pictureBox2.Image;
            pictureBox2.Image = bmp.ToWinFormBitmap();
            old?.Dispose();
        }

        private void PropertyGrid1_PropertyValueChanged(object s, PropertyValueChangedEventArgs e)
        {
            Draw();
        }
    }
}
