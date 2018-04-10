﻿using MoyskleyTech.ImageProcessing.Image;
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
        private Canvas ctx;
        public WPFGraphics(Canvas context)
        {
            ctx = context;
        }
        public override void Clear(Brush p)
        {
            ctx.Children.Clear();
            ctx.Background = Convert(p);
        }
        public override void Clear(Pixel p)
        {
            ctx.Children.Clear();
            ctx.Background = new System.Windows.Media.SolidColorBrush(Convert(p));
        }
        public override void DrawCircle(Brush p , int x0 , int y0 , double r)
        {
            DrawCircle(p , x0 , y0 , r , 1);
        }
        public override void DrawCircle(Pixel p , int x0 , int y0 , double r)
        {
            DrawCircle(p , x0 , y0 , r , 1);
        }
        public override void DrawCircle(Brush p , int x0 , int y0 , double r , int thickness)
        {
            System.Windows.Shapes.Ellipse circle = new System.Windows.Shapes.Ellipse();
            circle.Stroke = Convert(p,(int)(x0-r),(int)(y0-r));
            circle.StrokeThickness = thickness;
            circle.Width = r + r;
            circle.Height = r + r;
            Canvas.SetLeft(circle , x0 - r);
            Canvas.SetTop(circle , y0 - r);
            ctx.Children.Add(circle);
        }
        public override void DrawCircle(Pixel p , int x0 , int y0 , double r , int thickness)
        {
            System.Windows.Shapes.Ellipse circle = new System.Windows.Shapes.Ellipse();
            circle.Stroke = ConvertBrush(p);
            circle.StrokeThickness = thickness;
            circle.Width = r + r;
            circle.Height = r + r;
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
            System.Windows.Shapes.Ellipse circle = new System.Windows.Shapes.Ellipse();
            circle.Stroke = ConvertBrush(p);
            circle.StrokeThickness = t;
            circle.Width = w;
            circle.Height = h;
            Canvas.SetLeft(circle , x);
            Canvas.SetTop(circle , y);
            ctx.Children.Add(circle);
        }
        public override void DrawEllipse(Brush p , int x , int y , int w , int h)
        {
            DrawEllipse(p , x , y , w , h , 1);
        }
        public override void DrawEllipse(Brush p , int x , int y , int w , int h , int t)
        {
            System.Windows.Shapes.Ellipse circle = new System.Windows.Shapes.Ellipse();
            circle.Stroke = Convert(p,x,y);
            circle.StrokeThickness = t;
            circle.Width = w;
            circle.Height = h;
            Canvas.SetLeft(circle , x);
            Canvas.SetTop(circle , y);
            ctx.Children.Add(circle);
        }
        public override void FillCircle(Brush p , int x0 , int y0 , double r)
        {
            System.Windows.Shapes.Ellipse circle = new System.Windows.Shapes.Ellipse();
            circle.Fill = Convert(p,(int)(x0-r),(int)(y0-r));
            circle.Width = r + r;
            circle.Height = r + r;
            Canvas.SetLeft(circle , x0 - r);
            Canvas.SetTop(circle , y0 - r);
            ctx.Children.Add(circle);
        }
        public override void FillCircle(Pixel p , int x0 , int y0 , double r)
        {
            System.Windows.Shapes.Ellipse circle = new System.Windows.Shapes.Ellipse();
            circle.Fill = ConvertBrush(p);
            circle.Width = r + r;
            circle.Height = r + r;
            Canvas.SetLeft(circle , x0 - r);
            Canvas.SetTop(circle , y0 - r);
            ctx.Children.Add(circle);
        }
        public override void FillEllipse(Pixel p , int x , int y , int w , int h)
        {
            System.Windows.Shapes.Ellipse circle = new System.Windows.Shapes.Ellipse();
            circle.Fill = ConvertBrush(p);
            circle.Width = w;
            circle.Height = h;
            Canvas.SetLeft(circle , x);
            Canvas.SetTop(circle , y);
            ctx.Children.Add(circle);
        }
        public override void FillEllipse(Brush p , int x , int y , int w , int h)
        {
            System.Windows.Shapes.Ellipse circle = new System.Windows.Shapes.Ellipse();
            circle.Fill = Convert(p,x,y);
            circle.Width = w;
            circle.Height = h;
            Canvas.SetLeft(circle , x);
            Canvas.SetTop(circle , y);
            ctx.Children.Add(circle);
        }
        public override void DrawImage(MoyskleyTech.ImageProcessing.Image.Bitmap source , int x , int y)
        {
            System.Windows.Controls.Image img = new System.Windows.Controls.Image();
            MemoryStream ms = new MemoryStream();
            source.ToStream(ms);

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
        public override void DrawLine(Brush p , double x , double y , double x2 , double y2)
        {
            DrawLine(p , x , y , x2 , y2 , 1);
        }
        public override void DrawLine(Pixel p , double x , double y , double x2 , double y2 , int thickness)
        {
            System.Windows.Shapes.Line line = new System.Windows.Shapes.Line();
            line.X1 = x;
            line.X2 = x2;
            line.Y1 = y;
            line.Y2 = y2;
            line.Stroke = ConvertBrush(p);
            line.StrokeThickness = thickness;
            ctx.Children.Add(line);
        }
        public override void DrawLine(Brush p , double x , double y , double x2 , double y2 , int thickness)
        {
            System.Windows.Shapes.Line line = new System.Windows.Shapes.Line();
            line.X1 = x;
            line.X2 = x2;
            line.Y1 = y;
            line.Y2 = y2;
            line.Stroke = Convert(p,(int)x,(int)y);
            line.StrokeThickness = thickness;
            ctx.Children.Add(line);
        }
        public override void DrawLine(Pixel p , MoyskleyTech.ImageProcessing.Image.PointF p1 , MoyskleyTech.ImageProcessing.Image.PointF p2)
        {
            DrawLine(p , p1.X , p1.Y , p2.X , p2.Y , 1);
        }
        public override void DrawLine(Pixel p , MoyskleyTech.ImageProcessing.Image.PointF p1 , MoyskleyTech.ImageProcessing.Image.PointF p2 , int thickness)
        {
            DrawLine(p , p1.X , p1.Y , p2.X , p2.Y , thickness);
        }
        public override void DrawLine(Brush p , MoyskleyTech.ImageProcessing.Image.PointF p1 , MoyskleyTech.ImageProcessing.Image.PointF p2)
        {
            DrawLine(p , p1.X , p1.Y , p2.X , p2.Y , 1);
        }
        public override void DrawLine(Brush p , MoyskleyTech.ImageProcessing.Image.PointF p1 , MoyskleyTech.ImageProcessing.Image.PointF p2 , int thickness)
        {
            DrawLine(p , p1.X , p1.Y , p2.X , p2.Y , thickness);
        }
        public override void FillPolygon(Brush p , params MoyskleyTech.ImageProcessing.Image.PointF[ ] points)
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

            return XamlReader.Parse("<Path Data='" + CreatePolygon(points) + "'/>",context) as System.Windows.Shapes.Path;
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

      

        public override void DrawPolygon(Brush p , int thickness , params MoyskleyTech.ImageProcessing.Image.PointF[ ] points)
        {
            System.Windows.Shapes.Path path = ParsePath(points);

            path.Stroke = Convert(p);
            path.StrokeThickness = thickness;
            ctx.Children.Add(path);
        }
        public override void DrawPolygon(Brush p , params MoyskleyTech.ImageProcessing.Image.PointF[ ] points)
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
        public override void DrawString(string str , Pixel p , int x , int y , Font f , int size , StringFormat sf = null)
        {
            TextBlock tb = new TextBlock();
            tb.Text = str;
            tb.FontFamily = new System.Windows.Media.FontFamily(f.Name);
            tb.FontSize = size * 12;
            tb.Foreground = ConvertBrush(p);
            Canvas.SetLeft(tb , x);
            Canvas.SetTop(tb , y);
            ctx.Children.Add(tb);

        }
        public override void DrawString(string str , Brush p , int x , int y , Font f , int size , StringFormat sf = null)
        {
            TextBlock tb = new TextBlock();
            tb.Text = str;
            tb.FontFamily = new System.Windows.Media.FontFamily(f.Name);
            tb.FontSize = size * 12;
            tb.Foreground = Convert(p,x,y);
            Canvas.SetLeft(tb , x);
            Canvas.SetTop(tb , y);
            ctx.Children.Add(tb);
        }
        public override void SetPixel(Brush p , double x , double y)
        {
            DrawLine(p ,new PointF(x , y), new PointF(x+1 , y),1);
        }
        public override void SetPixel(Pixel p , double x , double y)
        {
            DrawLine(p ,new PointF(x , y), new PointF(x+1 , y),1);
        }
        protected override void SetPixelInternal(Pixel p , double px , double py,bool alpha)
        {
            FillRectangle(p , px , py , 1 , 1);
        }
        public void DrawImage(System.Windows.Media.ImageSource src , int x , int y)
        {
            System.Windows.Controls.Image tb = new System.Windows.Controls.Image();
            tb.Width = src.Width;
            tb.Height = src.Height;
            tb.Source = src;
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
        private System.Windows.Media.Brush Convert(Brush myBrush,int x=0,int y=0)
        {
            if ( myBrush is ImageBrush )
            {
                MemoryStream ms = new MemoryStream();
                ( myBrush as ImageBrush ).Image.ToStream(ms);

                var imageSource = new System.Windows.Media.Imaging.BitmapImage();
                ms.Position = 0;
                imageSource.BeginInit();
                imageSource.StreamSource = ms;
                imageSource.EndInit();

                return new System.Windows.Media.ImageBrush(imageSource) { TileMode = System.Windows.Media.TileMode.Tile };
            }
            {
                LinearGradientBrush lgb = myBrush as LinearGradientBrush;
                if ( lgb != null )
                {
                    var nlgb = new System.Windows.Media.LinearGradientBrush();
                    nlgb.StartPoint = new System.Windows.Point(lgb.SourceLocation.X-x , lgb.SourceLocation.Y-y);
                    nlgb.EndPoint = new System.Windows.Point(lgb.FinalLocation.X-x , lgb.FinalLocation.Y-y);
                    nlgb.GradientStops.Add(new System.Windows.Media.GradientStop(Convert(lgb.SourceColor) , 0));
                    nlgb.GradientStops.Add(new System.Windows.Media.GradientStop(Convert(lgb.FinalColor) , 1));
                    nlgb.MappingMode = System.Windows.Media.BrushMappingMode.Absolute;
                    
                    nlgb.ColorInterpolationMode = System.Windows.Media.ColorInterpolationMode.SRgbLinearInterpolation;
                    return nlgb;
                }
            }
            {
                RadialGradientBrush rgb = myBrush as RadialGradientBrush;
                if ( rgb != null )
                {
                    var nrgb = new System.Windows.Media.RadialGradientBrush();
                    nrgb.GradientStops.Add(new System.Windows.Media.GradientStop(Convert(rgb.SourceColor) , 0));
                    nrgb.GradientStops.Add(new System.Windows.Media.GradientStop(Convert(rgb.FinalColor) , 1));
                    nrgb.Center = new System.Windows.Point(rgb.SourceLocation.X-x , rgb.SourceLocation.Y-y);
                    nrgb.RadiusX = rgb.Radius;
                    nrgb.RadiusY = rgb.Radius;
                    nrgb.MappingMode = System.Windows.Media.BrushMappingMode.Absolute;
                    return nrgb;
                }
            }
            {
                VisualBrush vb = myBrush as VisualBrush;
                if ( vb != null )
                    return vb.Inner;
            }

            
            return null;
        }
        public override void Dispose()
        {

        }
    }
}
