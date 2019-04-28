using MoyskleyTech.ImageProcessing.Image;
using MoyskleyTech.ImageProcessing.Image.Helper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static System.Math;
namespace MoyskleyTech.Charting.Chart
{
    public class PieChart : PieChart<Pixel> { }
    public partial class PieChart<R>
        where R : unmanaged
    {

        public Font Font { get; set; } = BaseFonts.Premia;
        public float FontSize { get; set; } = 1;
        public int LineThickness { get; set; } = 0;
        public float DonutHoleSize { get; set; } = 0.4f;
        public float Thickness3D { get; set; } = 0.1f;
        public Brush<R> BackPixel { get; set; } = new SolidBrush<R>(ColorConvert.Convert<Pixel, R>(Pixels.Transparent));
        public R FontPixel { get; set; } = ColorConvert.Convert<Pixel, R>(Pixels.WhiteSmoke);
        public IEnumerable<PieData> Datas { get; set; } = new PieData[0];
        public IEnumerable<R> Colors { get; set; } = null;
        public Padding Padding { get; set; } = new Padding();
        public StringFormat StringFormat { get; set; }
        public PieChartMode Mode { get; set; } = PieChartMode.Pie2D;
        public PieChart()
        {
            StringFormat = new StringFormat() { Alignment = StringAlignment.Center, LineAlignment = StringAlignment.Center };
        }
        private IEnumerable<R> CreateColorScheme()
        {
            Random r = new Random();
            Pixel[] scheme = new Pixel[17]
            {
                Pixel.FromArgb(0xff5668E2),
                Pixel.FromArgb(0xff56AEE2),
                Pixel.FromArgb(0xff56E2CF),
                Pixel.FromArgb(0xff56E289),
                Pixel.FromArgb(0xff68E256),
                Pixel.FromArgb(0xffAEE256),
                Pixel.FromArgb(0xffE2CF56),
                Pixel.FromArgb(0xffE28956),
                Pixel.FromArgb(0xffE25668),
                Pixel.FromArgb(0xffE256AE),
                Pixel.FromArgb(0xffCF56E2),
                Pixel.FromArgb(0xff8A56E2),

                Pixel.FromArgb(0xff00B8BF),
                Pixel.FromArgb(0xff8DD5E7),
                Pixel.FromArgb(0xffEDFF9F),
                Pixel.FromArgb(0xffFFA928),
                Pixel.FromArgb(0xffC0FFF6)
            };
            return from x in scheme select ColorConvert.Convert<Pixel, R>(x);
        }
        private IEnumerable<R> GetColors()
        {
            if (Colors == null)
            {
                return CreateColorScheme();
            }
            return new CircularArray<R>(Colors.ToArray());
        }
        public Image<R> Draw(int bmpWidth, int bmpHeight)
        {
            var bmp = Image<R>.Create(bmpWidth, bmpHeight);
            Graphics<R> g = Graphics<R>.FromImage(bmp);
            Draw(g, bmp.Width, bmp.Height);
            g.Dispose();
            return bmp;
        }
        public void Draw(Graphics<R> g, int bmpWidth, int bmpHeight)
        {
            var width = bmpWidth - Padding.Horizontal;
            var height = bmpHeight - Padding.Vertical;
            g.FillRectangle(BackPixel, Padding.Left, Padding.Top, width, height);

            double totalWeight = Datas.Sum((x) => x.Weight);

            double angle = 2 * PI;
            double angleSpan = 0;
            var enumColor = GetColors().GetEnumerator();
            var r = Math.Min(width / 2, height / 2);

            if (Mode == PieChartMode.Pie2D)
            {
                DrawPie2D(g, width, height, totalWeight, ref angle, ref angleSpan, enumColor, r);
            }
            else if (Mode == PieChartMode.Donut2D)
            {
                DrawDonut2D(g, width, height, totalWeight, ref angle, ref angleSpan, enumColor, r);
            }
            else if (Mode == PieChartMode.Donut3D)
            {
                DrawDonut3D(g, width, height, totalWeight, ref angle, ref angleSpan, enumColor, r);
            }
            else // Mode == PieChartMode.Pie3D
            {
                DrawPie3D(g, width, height, totalWeight, ref angle, ref angleSpan, enumColor, r);
            }
        }

        private void DrawPie3D(Graphics<R> g, int width, int height, double totalWeight, ref double angle, ref double angleSpan, IEnumerator<R> enumColor, int r)
        {
            int w = r * 2, h = (int)(r * (2-Thickness3D));
            int x = width / 2 - r + Padding.Left;//offsetX + Padding.Left;
            int y = height / 2 - r + Padding.Top;// offsetY + Padding.Top;
            int xCenter = x + r;
            int yCenter = y + r;

            LinkedList<PointF> location = new LinkedList<PointF>();
            foreach (PieData pd in Datas)
            {
                enumColor.MoveNext();
                var color = enumColor.Current;
                var borderColor = ColorConvert.Convert<Pixel, R>(Pixel.FromArgb(50, 0, 0, 0).Over(ColorConvert.Convert<R, Pixel>(color)));
                angleSpan = pd.Weight / totalWeight * 2 * PI;
                if (pd == Datas.Last())
                    angleSpan = angle;
                var poly = g.DrawPie(borderColor, x, y, w, h, angleSpan, angle-angleSpan, LineThickness);
                g.FillPolygon(color, poly.ToArray());
                //g.FillPie(color , x , y , w , h , angleSpan , angle);

                var lowerPoints = poly.Where((pt) => { return pt.Y > y + r; });
                lowerPoints = lowerPoints.Concat(lowerPoints.Reverse().Select((pt) => new PointF(pt.X, pt.Y + r * Thickness3D)));
                if (lowerPoints.Count() > 0)
                    g.FillPolygon(borderColor, lowerPoints.ToArray());

                var avgX = poly.Average((X) => X.X);
                var avgY = poly.Average((X) => X.Y);
                location.AddLast(new PointF(avgX, avgY));
                angle -= angleSpan;
            }
            var locationIter = location.GetEnumerator();
            foreach (PieData pd in Datas)
            {
                locationIter.MoveNext();

                //g.DrawString(pd.Name , new FontSize(Font , FontSize) , Pixels.White , locationIter.Current.Move(1 , 1) , StringFormat);
                g.DrawString(pd.Name, new FontSizeF(Font, FontSize), FontPixel, locationIter.Current, StringFormat);
            }
        }
        private void DrawDonut3D(Graphics<R> g, int width, int height, double totalWeight, ref double angle, ref double angleSpan, IEnumerator<R> enumColor, int r)
        {
            var h = (int)(r * (1-Thickness3D/2));
            var w = r;

            //g.FillCircle(FontPixel , offsetX+ Padding.Left + halfWidth , offsetY+ Padding.Top + halfHeight , r);
            int x = width / 2 - r + Padding.Left;//offsetX + Padding.Left;
            int y = height / 2 - r + Padding.Top;// offsetY + Padding.Top;

            LinkedList<PointF> location = new LinkedList<PointF>();
            foreach (PieData pd in Datas)
            {
                enumColor.MoveNext();

                var color = enumColor.Current;
                var borderColor = ColorConvert.Convert<Pixel, R>(Pixel.FromArgb(50, 0, 0, 0).Over(ColorConvert.Convert<R, Pixel>(color)));

                angleSpan = pd.Weight / totalWeight * 2 * PI;
                if (pd == Datas.Last())
                    angleSpan = angle;

                var arcOuter = GraphicsHelper.GetEllipseArcPolygon(new PointF(x + r, y + h), w, h, angle - angleSpan, angleSpan);
                var arcInner = GraphicsHelper.GetEllipseArcPolygon(new PointF(x + r, y + h), w * DonutHoleSize, h * DonutHoleSize, angle - angleSpan, angleSpan);

                var arcPoly = arcOuter.Concat(arcInner.Reverse()).ToArray();

                g.FillPolygon(color, arcPoly);
                g.DrawPolygon(borderColor, arcPoly);

                double[] a = new double[arcPoly.Length], b = new double[arcPoly.Length];
                Graphics<Pixel>.PrecalculatePnPolyValues(arcPoly, a, b);
                var lowerPointsA = arcOuter.Where((pt) => { return !Graphics<Pixel>.PnPolyUsingPrecalc(arcPoly, a, b, pt.X, pt.Y + 3); });
                var lowerPointsB = arcInner.Where((pt) => { return !Graphics<Pixel>.PnPolyUsingPrecalc(arcPoly, a, b, pt.X, pt.Y + 3); });
                //lowerPointsA = lowerPointsA.Concat(lowerPointsA.Reverse().Select((pt) => new PointF(pt.X, pt.Y + r * 0.1)));
                //lowerPointsB = lowerPointsB.Concat(lowerPointsB.Reverse().Select((pt) => new PointF(pt.X, pt.Y + r * 0.1)));

                //if (lowerPointsA.Count() > 2)
                //    g.FillPolygon(borderColor, lowerPointsA.ToArray());
                //if (lowerPointsB.Count() > 2)
                //    g.FillPolygon(borderColor, lowerPointsB.ToArray());

                SubdivideAndDraw(g, lowerPointsA, r, borderColor);
                SubdivideAndDraw(g, lowerPointsB, r, borderColor);

                var avgX = arcPoly.Average((X) => X.X);
                var avgY = arcPoly.Average((X) => X.Y);
                location.AddLast(new PointF(avgX, avgY));
                angle -= angleSpan;
            }
            var locationIter = location.GetEnumerator();
            foreach (PieData pd in Datas)
            {
                locationIter.MoveNext();

                //g.DrawString(pd.Name , new FontSize(Font , FontSize) , Pixels.White , locationIter.Current.Move(1 , 1) , StringFormat);
                g.DrawString(pd.Name, new FontSizeF(Font, FontSize), FontPixel, locationIter.Current, StringFormat);
            }


        }
        private void SubdivideAndDraw(Graphics<R> g, IEnumerable<PointF> lowerPoints, int r, R borderColor)
        {
            double maxDist = r * 0.2;
            if (lowerPoints.Count() > 2)
            {
                var previous = lowerPoints.ElementAt(0);
                foreach (var pt in lowerPoints.Reverse().Skip(1).Reverse().Skip(1))
                {
                    if (Math.Abs(pt.X - previous.X) + Math.Abs(pt.Y - previous.Y) < maxDist)
                        g.FillPolygon(borderColor,
                            previous,
                            pt,
                            pt.Move(0, r * Thickness3D),
                            previous.Move(0, r * Thickness3D));
                    previous = pt;
                }
            }
        }
        private void DrawPie2D(Graphics<R> g, int width, int height, double totalWeight, ref double angle, ref double angleSpan, IEnumerator<R> enumColor, int r)
        {
            //g.FillCircle(FontPixel , offsetX+ Padding.Left + halfWidth , offsetY+ Padding.Top + halfHeight , r);
            int x = width / 2 - r + Padding.Left;//offsetX + Padding.Left;
            int y = height / 2 - r + Padding.Top;// offsetY + Padding.Top;

            LinkedList<PointF> location = new LinkedList<PointF>();
            foreach (PieData pd in Datas)
            {
                enumColor.MoveNext();

                var color = enumColor.Current;
                var borderColor = ColorConvert.Convert<Pixel, R>(Pixel.FromArgb(50, 0, 0, 0).Over(ColorConvert.Convert<R, Pixel>(color)));

                angleSpan = pd.Weight / totalWeight * 2 * PI;
                if (pd == Datas.Last())
                    angleSpan = angle;


                var poly = g.DrawPie(borderColor, x, y, r * 2, r * 2, angleSpan, angle - angleSpan, LineThickness);
                //g.FillPie(color , offsetX + Padding.Left  , offsetY + Padding.Top  , r * 2, r * 2, angleSpan , angle);
                g.FillPolygon(color, poly.ToArray());

                var avgX = poly.Average((X) => X.X);
                var avgY = poly.Average((X) => X.Y);
                location.AddLast(new PointF(avgX, avgY));
                angle -= angleSpan;
            }
            var locationIter = location.GetEnumerator();
            foreach (PieData pd in Datas)
            {
                locationIter.MoveNext();

                //g.DrawString(pd.Name , new FontSize(Font , FontSize) , Pixels.White , locationIter.Current.Move(1 , 1) , StringFormat);
                g.DrawString(pd.Name, new FontSizeF(Font, FontSize), FontPixel, locationIter.Current, StringFormat);
            }
        }
        private void DrawDonut2D(Graphics<R> g, int width, int height, double totalWeight, ref double angle, ref double angleSpan, IEnumerator<R> enumColor, int r)
        {
            //g.FillCircle(FontPixel , offsetX+ Padding.Left + halfWidth , offsetY+ Padding.Top + halfHeight , r);
            int x = width / 2 - r + Padding.Left;//offsetX + Padding.Left;
            int y = height / 2 - r + Padding.Top;// offsetY + Padding.Top;

            LinkedList<PointF> location = new LinkedList<PointF>();
            foreach (PieData pd in Datas)
            {
                enumColor.MoveNext();

                var color = enumColor.Current;
                var borderColor = ColorConvert.Convert<Pixel, R>(Pixel.FromArgb(50, 0, 0, 0).Over(ColorConvert.Convert<R, Pixel>(color)));

                angleSpan = pd.Weight / totalWeight * 2 * PI;
                if (pd == Datas.Last())
                    angleSpan = angle;

                var arcOuter = GraphicsHelper.GetEllipseArcPolygon(new PointF(x + r, y + r), r, r, angle - angleSpan, angleSpan);
                var arcInner = GraphicsHelper.GetEllipseArcPolygon(new PointF(x + r, y + r), r * DonutHoleSize, r * DonutHoleSize, angle - angleSpan, angleSpan);

                var arcPoly = arcOuter.Concat(arcInner.Reverse());

                g.FillPolygon(color, arcPoly.ToArray());
                g.DrawPolygon(borderColor, arcPoly.ToArray());

                var avgX = arcPoly.Average((X) => X.X);
                var avgY = arcPoly.Average((X) => X.Y);
                location.AddLast(new PointF(avgX, avgY));
                angle -= angleSpan;
            }
            var locationIter = location.GetEnumerator();
            foreach (PieData pd in Datas)
            {
                locationIter.MoveNext();

                //g.DrawString(pd.Name , new FontSize(Font , FontSize) , Pixels.White , locationIter.Current.Move(1 , 1) , StringFormat);
                g.DrawString(pd.Name, new FontSizeF(Font, FontSize), FontPixel, locationIter.Current, StringFormat);
            }
        }

    }
}
