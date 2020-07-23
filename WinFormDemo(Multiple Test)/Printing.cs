using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using MoyskleyTech.ImageProcessing.Image;
using MoyskleyTech.ImageProcessing.WinForm;

namespace WinFormDemo_Multiple_Test_
{
    public partial class Printing : Form
    {
        string[] words;
        public Printing()
        {
            InitializeComponent();
            words = Properties.Resources.words.Split(new char[] { '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries);
        }

        private void Button1_Click(object sender, EventArgs e)
        {
            var doc = new System.Drawing.Printing.PrintDocument();
            doc.PrintPage += Doc_PrintPage;
            printPreviewControl1.Document = doc;
        }

        private void Doc_PrintPage(object sender, System.Drawing.Printing.PrintPageEventArgs e)
        {
            var g = new NativeGraphicsWrapper(e.Graphics);

            var data = FillData();

            float[] widths = new float[data[0].Length];
            for (int i = 0; i < widths.Length; i++)
                widths[i] = 0;

            var font = FontLibrary.WindowsFonts.Get("Arial");
            var fontSize = 1.1f;

            var elementSizes = data.Select((l) =>
            {
                return l.Select((c) =>
                {
                    return g.MeasureString(c, font, fontSize);
                }).ToArray();
            }).ToArray();


            for (var c = 0; c < widths.Length; c++)
            {
                for (var l = 0; l < data.Length; l++)
                {
                    var line = data[l];
                    var elementSize = elementSizes[l][c];
                    if (elementSize.Width > widths[c])
                        widths[c] = elementSize.Width;
                }
            }

            var totalSize = widths.Sum();
            var wFactor = 1f;
            if (totalSize > e.MarginBounds.Width)
            {
                wFactor = e.MarginBounds.Width / totalSize;
            }

            int y = e.MarginBounds.Top;

            for (int l = 0; l < data.Length; l++)
            {
                int x = e.MarginBounds.Left;
                var line = data[l];
                for (var c = 0; c < data[l].Length; c++)
                {
                    var size = elementSizes[l][c];
                    var rect = new Rectangle(x, y, (int)(widths[c] * wFactor), (int)(size.Height));
                    g.DrawRectangle(Pixels.Black, rect);
                    g.DrawString(data[l][c], new FontSizeF(font, fontSize), Pixels.Black, rect.Location, new StringFormat());

                    x += rect.Width;
                }
                y += (int)elementSizes[l].Max((el) => el.Height);
            }

            e.HasMorePages = false;
        }

        private string[][] FillData()
        {
            Random r = new Random();
            var data = new string[(int)numericUpDown1.Value][];
            for (int l = 0; l < data.Length; l++)
            {
                data[l] = new string[(int)numericUpDown2.Value];
                for (var c = 0; c < data[l].Length; c++)
                {
                    data[l][c] = words[r.Next(words.Length)];
                }
            }

            return data;
        }
    }
}
