using MoyskleyTech.ImageProcessing.Image;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Markup;
using System.Windows;
using System.Windows.Media.Composition;

namespace MoyskleyTech.ImageProcessing.WPF
{
    public class WPFGraphics : Graphics
    {
        public int FontFactor { get; set; }= 12;
        private Canvas ctx;
        public WPFGraphics(Canvas context)
        {
            ctx = context;
        }
        public override void Clear(Brush<Pixel> p)
        {
            ctx.Children.Clear();
            ctx.Background = Convert(p);
        }
        public override void Clear(Pixel p)
        {
            ctx.Children.Clear();
            ctx.Background = new System.Windows.Media.SolidColorBrush(Convert(p));
        }
        public override void DrawCircle(Brush<Pixel> p , int x0 , int y0 , double r)
        {
            DrawCircle(p , x0 , y0 , r , 1);
        }
        public override void DrawCircle(Pixel p , int x0 , int y0 , double r)
        {
            DrawCircle(p , x0 , y0 , r , 1);
        }
        public override void DrawCircle(Brush<Pixel> p , int x0 , int y0 , double r , int thickness)
        {
            System.Windows.Shapes.Ellipse circle = new System.Windows.Shapes.Ellipse
            {
                Stroke = Convert(p , ( int ) ( x0 - r ) , ( int ) ( y0 - r )) ,
                StrokeThickness = thickness ,
                Width = r + r ,
                Height = r + r
            };
            Canvas.SetLeft(circle , x0 - r);
            Canvas.SetTop(circle , y0 - r);
            ctx.Children.Add(circle);
        }
        public override void DrawCircle(Pixel p , int x0 , int y0 , double r , int thickness)
        {
            System.Windows.Shapes.Ellipse circle = new System.Windows.Shapes.Ellipse
            {
                Stroke = ConvertBrush(p) ,
                StrokeThickness = thickness ,
                Width = r + r ,
                Height = r + r
            };
            Canvas.SetLeft(circle , x0 - r);
            Canvas.SetTop(circle , y0 - r);
            ctx.Children.Add(circle);
        }
        public override void DrawEllipse(Pixel p , int x , int y , int w , int h)
        {
            DrawEllipse(p , x , y , w , h , 1);
        }
        public override void DrawEllipse(Pixel p , int x , int y , int w , int h , int t)
        {
            System.Windows.Shapes.Ellipse circle = new System.Windows.Shapes.Ellipse
            {
                Stroke = ConvertBrush(p) ,
                StrokeThickness = t ,
                Width = w ,
                Height = h
            };
            Canvas.SetLeft(circle , x);
            Canvas.SetTop(circle , y);
            ctx.Children.Add(circle);
        }
        public override void DrawEllipse(Brush<Pixel> p , int x , int y , int w , int h)
        {
            DrawEllipse(p , x , y , w , h , 1);
        }
        public override void DrawEllipse(Brush<Pixel> p , int x , int y , int w , int h , int t)
        {
            System.Windows.Shapes.Ellipse circle = new System.Windows.Shapes.Ellipse
            {
                Stroke = Convert(p , x , y) ,
                StrokeThickness = t ,
                Width = w ,
                Height = h
            };
            Canvas.SetLeft(circle , x);
            Canvas.SetTop(circle , y);
            ctx.Children.Add(circle);
        }
        public override void FillCircle(Brush<Pixel> p , int x0 , int y0 , double r)
        {
            System.Windows.Shapes.Ellipse circle = new System.Windows.Shapes.Ellipse
            {
                Fill = Convert(p , ( int ) ( x0 - r ) , ( int ) ( y0 - r )) ,
                Width = r + r ,
                Height = r + r
            };
            Canvas.SetLeft(circle , x0 - r);
            Canvas.SetTop(circle , y0 - r);
            ctx.Children.Add(circle);
        }
        public override void FillCircle(Pixel p , int x0 , int y0 , double r)
        {
            System.Windows.Shapes.Ellipse circle = new System.Windows.Shapes.Ellipse
            {
                Fill = ConvertBrush(p) ,
                Width = r + r ,
                Height = r + r
            };
            Canvas.SetLeft(circle , x0 - r);
            Canvas.SetTop(circle , y0 - r);
            ctx.Children.Add(circle);
        }
        public override void FillEllipse(Pixel p , int x , int y , int w , int h)
        {
            System.Windows.Shapes.Ellipse circle = new System.Windows.Shapes.Ellipse
            {
                Fill = ConvertBrush(p) ,
                Width = w ,
                Height = h
            };
            Canvas.SetLeft(circle , x);
            Canvas.SetTop(circle , y);
            ctx.Children.Add(circle);
        }
        public override void FillEllipse(Brush<Pixel> p , int x , int y , int w , int h)
        {
            System.Windows.Shapes.Ellipse circle = new System.Windows.Shapes.Ellipse
            {
                Fill = Convert(p , x , y) ,
                Width = w ,
                Height = h
            };
            Canvas.SetLeft(circle , x);
            Canvas.SetTop(circle , y);
            ctx.Children.Add(circle);
        }
        public override void DrawImage(ImageProxy<Pixel> source , int x , int y)
        {
            System.Windows.Controls.Image img = new System.Windows.Controls.Image();
            MemoryStream ms = new MemoryStream();
            new BitmapCodec().Save<Pixel>(source , ms);
           
            var imageSource = new System.Windows.Media.Imaging.BitmapImage();
            ms.Position = 0;
            imageSource.BeginInit();
            imageSource.StreamSource = ms;
            imageSource.EndInit();

            img.Source = imageSource;

            Canvas.SetLeft(img , x);
            Canvas.SetTop(img , y);
            ctx.Children.Add(img);
        }
        public override void DrawLine(Pixel p , double x , double y , double x2 , double y2)
        {
            DrawLine(p , x , y , x2 , y2 , 1);
        }
        public override void DrawLine(Brush<Pixel> p , double x , double y , double x2 , double y2)
        {
            DrawLine(p , x , y , x2 , y2 , 1);
        }
        public override void DrawLine(Pixel p , double x , double y , double x2 , double y2 , int thickness)
        {
            System.Windows.Shapes.Line line = new System.Windows.Shapes.Line
            {
                X1 = x ,
                X2 = x2 ,
                Y1 = y ,
                Y2 = y2 ,
                Stroke = ConvertBrush(p) ,
                StrokeThickness = thickness
            };
            ctx.Children.Add(line);
        }
        public override void DrawLine(Brush<Pixel> p , double x , double y , double x2 , double y2 , int thickness)
        {
            System.Windows.Shapes.Line line = new System.Windows.Shapes.Line
            {
                X1 = x ,
                X2 = x2 ,
                Y1 = y ,
                Y2 = y2 ,
                Stroke = Convert(p , ( int ) x , ( int ) y) ,
                StrokeThickness = thickness
            };
            ctx.Children.Add(line);
        }
        public override void FillPolygon(Brush<Pixel> p , params MoyskleyTech.ImageProcessing.Image.PointF[ ] points)
        {
            System.Windows.Shapes.Path path = ParsePath(points);

            path.Fill = Convert(p);
            ctx.Children.Add(path);
        }

        private System.Windows.Shapes.Path ParsePath(PointF[ ] points)
        {
            var context = new ParserContext {XamlTypeMapper = new XamlTypeMapper(new string[0])};

            context.XmlnsDictionary.Add("" , "http://schemas.microsoft.com/winfx/2006/xaml/presentation");
            context.XmlnsDictionary.Add("x" , "http://schemas.microsoft.com/winfx/2006/xaml");

            return XamlReader.Parse("<Path Data='" + CreatePolygon(points) + "'/>" , context) as System.Windows.Shapes.Path;
        }

        public string CreatePolygon(params MoyskleyTech.ImageProcessing.Image.PointF[ ] points)
        {
            StringBuilder sb = new StringBuilder();
            sb.Append('M');
            sb.Append(points[0].X.ToString("").Replace(',' , '.'));
            sb.Append(',');
            sb.Append(points[0].Y.ToString("").Replace(',' , '.'));
            sb.Append(' ');
            foreach ( PointF pt in points.Skip(1) )
            {
                sb.Append('L');
                sb.Append(pt.X.ToString("").Replace(',' , '.'));
                sb.Append(',');
                sb.Append(pt.Y.ToString("").Replace(',' , '.'));
                sb.Append(' ');
            }
            return sb.ToString();
        }
        public override void FillPolygon(Pixel p , params MoyskleyTech.ImageProcessing.Image.PointF[ ] points)
        {
            System.Windows.Shapes.Path path = ParsePath(points);

            path.Fill = ConvertBrush(p);
            ctx.Children.Add(path);
        }



        public override void DrawPolygon(Brush<Pixel> p , int thickness , params MoyskleyTech.ImageProcessing.Image.PointF[ ] points)
        {
            System.Windows.Shapes.Path path = ParsePath(points);

            path.Stroke = Convert(p);
            path.StrokeThickness = thickness;
            ctx.Children.Add(path);
        }
        public override void DrawPolygon(Brush<Pixel> p , params MoyskleyTech.ImageProcessing.Image.PointF[ ] points)
        {
            System.Windows.Shapes.Path path = ParsePath(points);

            path.Stroke = Convert(p);
            path.StrokeThickness = 1;
            ctx.Children.Add(path);
        }

        public override void DrawPolygon(Pixel p , int thickness , params MoyskleyTech.ImageProcessing.Image.PointF[ ] points)
        {
            System.Windows.Shapes.Path path = ParsePath(points);

            path.Stroke = ConvertBrush(p);
            path.StrokeThickness = thickness;
            ctx.Children.Add(path);
        }
        public override void DrawPolygon(Pixel p , params MoyskleyTech.ImageProcessing.Image.PointF[ ] points)
        {
            DrawPolygon(p , 1 , points);
        }
        public override void DrawString(string str , Pixel p , int x , int y , Font f , float size , StringFormat sf = null)
        {
            TextBlock tb = new TextBlock
            {
                Text = str ,
                FontFamily = new System.Windows.Media.FontFamily(f.Name) ,
                FontSize = size * FontFactor,
                Foreground = ConvertBrush(p)
            };
            Canvas.SetLeft(tb , x);
            Canvas.SetTop(tb , y);
            ctx.Children.Add(tb);
        }
        public override void DrawString(string str , Brush<Pixel> p , int x , int y , Font f , float size , StringFormat sf = null)
        {
            TextBlock tb = new TextBlock
            {
                Text = str ,
                FontFamily = new System.Windows.Media.FontFamily(f.Name) ,
                FontSize = size * FontFactor,
                Foreground = Convert(p , x , y)
            };
            Canvas.SetLeft(tb , x);
            Canvas.SetTop(tb , y);
            ctx.Children.Add(tb);
        }
        public override StringMeasurement MeasureString(string str, Font f, float size)
        {
            TextBlock tb = new TextBlock
            {
                Text = str,
                FontFamily = new System.Windows.Media.FontFamily(f.Name),
                FontSize = size * FontFactor
            };
            ctx.Children.Add(tb);
            var tsize = new StringMeasurement() { Width = (float)tb.Width, Height = (float)tb.Height };
            ctx.Children.Remove(tb);
            return tsize;
        }
        public override void SetPixel(Brush<Pixel> p , double x , double y)
        {
            DrawLine(p , new PointF(x , y) , new PointF(x + 1 , y) , 1);
        }
        public override void SetPixel(Pixel p , double x , double y)
        {
            DrawLine(p , new PointF(x , y) , new PointF(x + 1 , y) , 1);
        }
        public override void SetPixelWithoutTransform(Pixel p , double px , double py , bool alpha)
        {
            FillRectangle(p , px , py , 1 , 1);
        }
        public void DrawImage(System.Windows.Media.ImageSource src , int x , int y)
        {
            System.Windows.Controls.Image tb = new System.Windows.Controls.Image
            {
                Width = src.Width ,
                Height = src.Height ,
                Source = src
            };
            Canvas.SetLeft(tb , x);
            Canvas.SetTop(tb , y);
            ctx.Children.Add(tb);
        }
        private System.Windows.Media.Brush ConvertBrush(Pixel myBrush)
        {
            return
                new System.Windows.Media.SolidColorBrush(
                    new System.Windows.Media.Color()
                    {
                        A = myBrush.A ,
                        R = myBrush.R ,
                        G = myBrush.G ,
                        B = myBrush.B
                    });
        }
        private System.Windows.Media.Color Convert(Pixel myBrush)
        {
            return new System.Windows.Media.Color()
            {
                A = myBrush.A ,
                R = myBrush.R ,
                G = myBrush.G ,
                B = myBrush.B
            };
        }
        private System.Windows.Media.Brush Convert(Brush<Pixel> myBrush , int x = 0 , int y = 0)
        {
            if ( myBrush is ImageBrush )
            {
                MemoryStream ms = new MemoryStream();
                new BitmapCodec().Save<Pixel>(( myBrush as ImageBrush ).Image , ms);
               
                var imageSource = new System.Windows.Media.Imaging.BitmapImage();
                ms.Position = 0;
                imageSource.BeginInit();
                imageSource.StreamSource = ms;
                imageSource.EndInit();

                return new System.Windows.Media.ImageBrush(imageSource) { TileMode = System.Windows.Media.TileMode.Tile };
            }
            if ( myBrush is LinearGradientBrush lgb )
            {
                var nlgb = new System.Windows.Media.LinearGradientBrush
                {
                    StartPoint = new System.Windows.Point(lgb.SourceLocation.X - x , lgb.SourceLocation.Y - y) ,
                    EndPoint = new System.Windows.Point(lgb.FinalLocation.X - x , lgb.FinalLocation.Y - y)
                };
                nlgb.GradientStops.Add(new System.Windows.Media.GradientStop(Convert(lgb.SourceColor) , 0));
                nlgb.GradientStops.Add(new System.Windows.Media.GradientStop(Convert(lgb.FinalColor) , 1));
                nlgb.MappingMode = System.Windows.Media.BrushMappingMode.Absolute;

                nlgb.ColorInterpolationMode = System.Windows.Media.ColorInterpolationMode.SRgbLinearInterpolation;
                return nlgb;
            }
            if ( myBrush is RadialGradientBrush rgb )
            {
                var nrgb = new System.Windows.Media.RadialGradientBrush();
                nrgb.GradientStops.Add(new System.Windows.Media.GradientStop(Convert(rgb.SourceColor) , 0));
                nrgb.GradientStops.Add(new System.Windows.Media.GradientStop(Convert(rgb.FinalColor) , 1));
                nrgb.Center = new System.Windows.Point(rgb.SourceLocation.X - x , rgb.SourceLocation.Y - y);
                nrgb.RadiusX = rgb.Radius;
                nrgb.RadiusY = rgb.Radius;
                nrgb.MappingMode = System.Windows.Media.BrushMappingMode.Absolute;
                return nrgb;
            }
            if ( myBrush is VisualBrush vb )
                return vb.Inner;


            return null;
        }
        public override void Dispose()
        {

        }
    }
}
