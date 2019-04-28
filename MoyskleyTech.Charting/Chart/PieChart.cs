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
        where R: unmanaged
    {
      
        public Font Font { get; set; } = BaseFonts.Premia;
        public float FontSize { get; set; } = 1;
        public int LineThickness { get; set; } = 0;
        public Brush<R> BackPixel { get; set; } = new SolidBrush<R>(ColorConvert.Convert<Pixel,R>(Pixels.Transparent));
        public R FontPixel { get; set; } = ColorConvert.Convert <Pixel,R>(Pixels.WhiteSmoke);
        public IEnumerable<PieData> Datas { get; set; } = new PieData[0];
        public IEnumerable<R> Colors { get; set; } = null;
        public Padding Padding { get; set; } = new Padding();
        public StringFormat StringFormat { get; set; }
        public PieChartMode Mode { get; set; } = PieChartMode.Pie2D;
        public PieChart()
        {
            StringFormat = new StringFormat() { Alignment = StringAlignment.Center , LineAlignment = StringAlignment.Center };
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
            if ( Colors == null )
            {
                return CreateColorScheme();
            }
            return new CircularArray<R>(Colors.ToArray());
        }
        public Image<R> Draw(int bmpWidth, int bmpHeight)
        {
            var bmp = Image<R>.Create(bmpWidth, bmpHeight);
            Graphics<R> g = Graphics<R>.FromImage(bmp);
            Draw(g , bmp.Width , bmp.Height);
            g.Dispose();
            return bmp;
        }
        public void Draw(Graphics<R> g , int bmpWidth , int bmpHeight)
        {
            var width = bmpWidth - Padding.Horizontal;
            var height = bmpHeight - Padding.Vertical;
            g.FillRectangle(BackPixel , Padding.Left , Padding.Top , width , height);
            
            double totalWeight = Datas.Sum((x)=>x.Weight);

            double angle=0;
            double angleSpan=0;
            var enumColor = GetColors().GetEnumerator();
            var r = Math.Min(width/2,height/2);

            if ( Mode == PieChartMode.Pie2D )
            {
                //g.FillCircle(FontPixel , offsetX+ Padding.Left + halfWidth , offsetY+ Padding.Top + halfHeight , r);
                int x = width / 2 - r +Padding.Left;//offsetX + Padding.Left;
                int y = height / 2 - r + Padding.Top;// offsetY + Padding.Top;

                LinkedList<PointF> location = new LinkedList<PointF>();
                foreach ( PieData pd in Datas )
                {
                    enumColor.MoveNext();

                    var color = enumColor.Current;
                    var borderColor = ColorConvert.Convert < Pixel, R> (Pixel.FromArgb(50,0,0,0).Over(ColorConvert.Convert < R,Pixel > (color)));

                    angleSpan = pd.Weight / totalWeight * 2 * PI;
                    if ( pd == Datas.Last() )
                        angleSpan = 2 * PI - angle;

                    
                    var poly = g.DrawPie(borderColor, x, y, r * 2, r * 2, angleSpan, angle, LineThickness);
                    //g.FillPie(color , offsetX + Padding.Left  , offsetY + Padding.Top  , r * 2, r * 2, angleSpan , angle);
                    g.FillPolygon(color, poly.ToArray());

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
            else // Mode == PieChartMode.Pie3D
            {
                int w=r*2,h=(int)(r*1.9);
                int x = width / 2 - r + Padding.Left;//offsetX + Padding.Left;
                int y = height / 2 - r + Padding.Top;// offsetY + Padding.Top;
                int xCenter = x+r;
                int yCenter = y+r;

                LinkedList<PointF> location = new LinkedList<PointF>();
                foreach ( PieData pd in Datas )
                {
                    enumColor.MoveNext();
                    var color = enumColor.Current;
                    var borderColor = ColorConvert.Convert < Pixel, R> (Pixel.FromArgb(50,0,0,0).Over(ColorConvert.Convert < R,Pixel >( color)));
                    angleSpan = pd.Weight / totalWeight * 2 * PI;
                    if ( pd == Datas.Last() )
                        angleSpan = 2 * PI - angle;
                    var poly = g.DrawPie(borderColor , x , y , w,h, angleSpan , angle, LineThickness);
                    g.FillPolygon(color, poly.ToArray());
                    //g.FillPie(color , x , y , w , h , angleSpan , angle);

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
