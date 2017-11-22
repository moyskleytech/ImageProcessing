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
    public class PieChart
    {
        public enum PieChartMode
        {
            Pie2D, Pie3D
        }
        private Bitmap bmp;

        public Font Font { get; set; } = BaseFonts.Premia;
        public float FontSize { get; set; } = 1;
        public int LineThickness { get; set; } = 0;
        public Brush BackPixel { get; set; } = new SolidBrush(Pixels.Transparent);
        public Pixel FontPixel { get; set; } = Pixels.WhiteSmoke;
        public IEnumerable<PieData> Datas { get; set; } = new PieData[0];
        public IEnumerable<Pixel> Colors { get; set; } = null;
        public Padding Padding { get; set; } = new Padding();
        public StringFormat StringFormat { get; set; }
        public PieChartMode Mode { get; set; } = PieChartMode.Pie2D;
        public PieChart(Bitmap bmp)
        {
            this.bmp = bmp;
            StringFormat = new StringFormat() { Alignment = StringAlignment.Center , LineAlignment = StringAlignment.Center };
        }
        private IEnumerable<Pixel> CreateColorScheme()
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

            return scheme;
        }
        private IEnumerable<Pixel> GetColors()
        {
            if ( Colors == null )
            {
                return CreateColorScheme();
            }
            return new CircularArray<Pixel>(Colors.ToArray());
        }
        public Bitmap Draw()
        {
            Graphics g = Graphics.FromImage(bmp);
            Draw(g , 0 , 0 , bmp.Width , bmp.Height);
            g.Dispose();
            return bmp;
        }
        public void Draw(Graphics g , int bmpWidth , int bmpHeight)
        {
            Draw(g , 0 , 0 , bmpWidth , bmpHeight);
        }
        public void Draw(Graphics g , int offsetX , int offsetY , int bmpWidth , int bmpHeight)
        {
            g.FillRectangle(BackPixel , offsetX , offsetY , bmpWidth , bmpHeight);

            var width = bmpWidth-Padding.Horizontal;
            var height = bmpHeight-Padding.Vertical;
            var halfWidth = width/2;
            var halfHeight = height/2;

            double totalWeight = Datas.Sum((x)=>x.Weight);

            double angle=0;
            double angleSpan=0;
            var enumColor = GetColors().GetEnumerator();
            var r = Math.Min(halfHeight,halfWidth);

            if ( Mode == PieChartMode.Pie2D )
            {
                //g.FillCircle(FontPixel , offsetX+ Padding.Left + halfWidth , offsetY+ Padding.Top + halfHeight , r);
                LinkedList<PointF> location = new LinkedList<PointF>();
                foreach ( PieData pd in Datas )
                {
                    enumColor.MoveNext();

                    var color = enumColor.Current;
                    var borderColor = Pixel.FromArgb(50,0,0,0).Over(color);

                    angleSpan = pd.Weight / totalWeight * 2 * PI;
                    if ( pd == Datas.Last() )
                        angleSpan = 2 * PI - angle;
                    var poly = g.DrawPie(borderColor , offsetX+ Padding.Left+halfWidth , offsetY+ Padding.Top+halfHeight , r, angleSpan , angle, LineThickness);
                    g.FillPie(color , offsetX + Padding.Left + halfWidth , offsetY + Padding.Top + halfHeight , r , angleSpan , angle);

                    var avgX = poly.Average((X) => X.X);
                    var avgY = poly.Average((X) => X.Y);
                    location.AddLast(new PointF(avgX , avgY));
                    angle += angleSpan;
                }
                var locationIter=  location.GetEnumerator();
                foreach ( PieData pd in Datas )
                {
                    locationIter.MoveNext();

                    //g.DrawString(pd.Name , new FontSize(Font , FontSize) , Pixels.White , locationIter.Current.Move(1 , 1) , StringFormat);
                    g.DrawString(pd.Name , new FontSizeF(Font , FontSize) , FontPixel , locationIter.Current , StringFormat);
                }
            }
            else
            {
                int x=offsetX+ Padding.Left+halfWidth-r, y=offsetY+ Padding.Top + halfHeight-r, w=r,h=(int)(r*0.9);
                int xCenter = x+r;
                int yCenter = y+r;

                LinkedList<PointF> location = new LinkedList<PointF>();
                foreach ( PieData pd in Datas )
                {
                    enumColor.MoveNext();
                    var color = enumColor.Current;
                    var borderColor = Pixel.FromArgb(50,0,0,0).Over(color);
                    angleSpan = pd.Weight / totalWeight * 2 * PI;
                    if ( pd == Datas.Last() )
                        angleSpan = 2 * PI - angle;
                    var poly = g.DrawPie(borderColor , x+r , y+r , w,h, angleSpan , angle, LineThickness);
                    g.FillPie(color , x + r , y + r , w , h , angleSpan , angle);

                    var lowerPoints = poly.Where((pt) => { return pt.Y > y + r; });
                    lowerPoints = lowerPoints.Concat(lowerPoints.Reverse().Select((pt) => new PointF(pt.X , pt.Y + r * 0.1)));
                    if ( lowerPoints.Count() > 0 )
                        g.FillPolygon(borderColor , lowerPoints.ToArray());

                    var avgX = poly.Average((X) => X.X);
                    var avgY = poly.Average((X) => X.Y);
                    location.AddLast(new PointF(avgX , avgY));
                    angle += angleSpan;
                }
                var locationIter=  location.GetEnumerator();
                foreach ( PieData pd in Datas )
                {
                    locationIter.MoveNext();

                    //g.DrawString(pd.Name , new FontSize(Font , FontSize) , Pixels.White , locationIter.Current.Move(1 , 1) , StringFormat);
                    g.DrawString(pd.Name , new FontSizeF(Font , FontSize) , FontPixel , locationIter.Current , StringFormat);
                }
            }
        }
    }
}
