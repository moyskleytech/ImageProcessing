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
using MoyskleyTech.Charting.Charting2D;

namespace WinFormDemo_Multiple_Test_
{
    public partial class GraphicTest : Form
    {
        Bitmap bmp;
        IDrawableChart<Pixel> graph;
        public GraphicTest()
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
            SetPie();
        }

        private void SetPie()
        {
            var pie = new PieChart();
            pie.Mode = PieChartMode.Pie3D;
            pie.BackPixel = new SolidBrush<Pixel>(Pixels.Black);
            pie.Datas = new PieData[] {
                new PieData(){ Name="a",Weight=4},
                new PieData(){ Name="b",Weight=4},
                new PieData(){ Name="c",Weight=12}
            };
            graph = pie;

            var a = (PieChartMode[])Enum.GetValues(typeof(PieChartMode));

            comboBox1.SelectedValueChanged -= ComboBox1_SelectedValueChanged;
            comboBox1.Items.Clear();
            comboBox1.Items.AddRange(a.Cast<object>().ToArray());
            comboBox1.SelectedValueChanged += ComboBox1_SelectedValueChanged;
            comboBox1.SelectedIndex = 0;

            propertyGrid1.SelectedObject = pie;
        }
        private void SetGauge()
        {
            var gauge = new Gauge<Pixel>();
            gauge.Mode = GaugeMode.Needle;
            gauge.NeedleColor = Pixel.FromArgb(Pixels.DeepPink,100);
            gauge.Areas.Add(new GaugeArea<Pixel>()
            {
                Fill = new SolidBrush(Pixels.Red),
                Minimum = 40,
                Maximum = 60,
                Border = Pixels.Red
            });
            gauge.Areas.Add(new GaugeArea<Pixel>()
            {
                Fill = new SolidBrush(Pixels.Yellow),
                Minimum = 60,
                Maximum = 80,
                Border = Pixels.Yellow
            });
            gauge.Areas.Add(new GaugeArea<Pixel>()
            {
                Fill = new SolidBrush(Pixels.LawnGreen),
                Minimum = 80,
                Maximum = 100,
                Border = Pixels.LawnGreen
            });
            graph = gauge;

            var a = (GaugeMode[])Enum.GetValues(typeof(GaugeMode));

            comboBox1.SelectedValueChanged -= ComboBox1_SelectedValueChanged;
            comboBox1.Items.Clear();
            comboBox1.Items.AddRange(a.Cast<object>().ToArray());
            comboBox1.SelectedValueChanged += ComboBox1_SelectedValueChanged;
            comboBox1.SelectedIndex = 0;

            propertyGrid1.SelectedObject = gauge;
        }

        private void ComboBox1_SelectedValueChanged(object sender, EventArgs e)
        {
            if(graph is PieChart<Pixel> pie)
                pie.Mode = (PieChartMode)comboBox1.SelectedItem;
            if (graph is Gauge<Pixel> gauge)
                gauge.Mode = (GaugeMode)comboBox1.SelectedItem;
            Draw();
        }


        private void Draw()
        {
            var g = new NativeGraphicsWrapper(pictureBox1.CreateGraphics());
            g.Clear(Pixels.Black);
            graph.Draw(g, pictureBox1.Width, pictureBox1.Height);
            g.Dispose();
            if(cbLib.Checked)
                graph.Draw(Graphics.FromImage(bmp), pictureBox2.Width, pictureBox2.Height);

            var old = pictureBox2.Image;
            pictureBox2.Image = bmp.ToWinFormBitmap();
            old?.Dispose();
        }

        private void PropertyGrid1_PropertyValueChanged(object s, PropertyValueChangedEventArgs e)
        {
            Draw();
        }

        private void Button1_Click(object sender, EventArgs e)
        {
            SetPie();
            Draw();
        }

        private void Button2_Click(object sender, EventArgs e)
        {
            SetGauge();
            Draw();
        }
    }
}
