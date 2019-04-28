using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.Graphics;
using Android.OS;
using Android.Runtime;
using Android.Util;
using Android.Views;
using Android.Widget;
using MoyskleyTech.Charting.Chart;
using MoyskleyTech.ImageProcessing.Image;

namespace MoyskleyTech.ImageProcessing.Android.Test
{
    public class DrawView : View
    {
        Charting.Chart.PieChart pc;
        ImageProcessing.Image.Bitmap bmp;
        int count=0;
        public DrawView(Context context) : base(context)
        {
            Initialize();
        }
        public DrawView(Context context , IAttributeSet attrs) :
            base(context , attrs)
        {
            Initialize();
        }

        public DrawView(Context context , IAttributeSet attrs , int defStyle) :
            base(context , attrs , defStyle)
        {
            Initialize();
        }

        private void Initialize()
        {
            bmp = new ImageProcessing.Image.Bitmap(100 , 100);
            Graphics g = Graphics.FromImage(bmp);
            g.Clear(Pixels.LawnGreen);
            g.DrawLine(Pixels.Black , 0 , 0 , 100 , 100);

            pc = new Charting.Chart.PieChart();
            pc.Mode = PieChartMode.Pie3D;
            pc.LineThickness = 1;
            pc.Padding = new Padding(15);
            pc.FontSize = 8;

            pc.Datas = new PieData[] {
                new PieData(){ Name="A", Weight=4},
                new PieData(){ Name="B", Weight=4},
                new PieData(){ Name="C", Weight=9}
            };
        }
        public override void Draw(Canvas canvas)
        {
            base.Draw(canvas);
            CanvasGraphics g = new CanvasGraphics(canvas);
            g.Clear(Pixels.Turquoise);
            g.DrawEllipse(Pixels.DeepPink , 0 , 0 , canvas.Width , canvas.Height);
            g.DrawImage(bmp , 10 , 10);

            count++;

            var polygon = GraphicsHelper.GetRoundedRectanglePolygon(new Rectangle(30,30,canvas.Width-60,canvas.Height-60),60);
            Brush<Pixel> brush = new LinearGradientBrush(new ImageProcessing.Image.Point(30,30), Pixels.LightGray,
                new ImageProcessing.Image.Point(0,canvas.Height-60),Pixels.Silver);
            g.FillPolygon(brush , polygon);
            g.DrawPolygon(Pixels.Yellow,10 , polygon);

            pc.Draw(g, canvas.Width, canvas.Height);
            g.DrawString("Hello", new FontSize(BaseFonts.Premia, 1), Pixels.Black, 10, 10);

        }
    }
}