using MoyskleyTech.Serialization.XML;
using MoyskleyTech.ImageProcessing.Image;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace MoyskleyTech.ImageProcessing.SVG
{
    public class SVGGraphics:Graphics
    {
        private XMLNode root;
        public SVGGraphics(XMLNode root)
        {
            this.root = root;
            root.Name = "svg";
            root.Attributes["xmlns"] = "http://www.w3.org/2000/svg";
            root.Attributes["version"] = "1.1";
        }
        private string Convert(Brush<Pixel> p)
        {
            Pixel color=p.GetColor(0,0);
            return "rgba(" + color.R + "," + color.G + "," + color.B + "," + ((double)color.A/255) + ")";
        }

        public override void Clear(Brush<Pixel> p)
        {
            root.Children.Clear();
            XMLNode rectangle = new XMLNode() { Name="rect" };
            rectangle.Attributes["x"] = 0;
            rectangle.Attributes["y"] = 0;
            rectangle.Attributes["width"] = root.Attributes["width"];
            rectangle.Attributes["height"] = root.Attributes["height"];
            rectangle.Attributes["fill"] = Convert(p);
            root.Children.Add(rectangle);
        }
        public override void Clear(Pixel p)
        {
            Clear(new SolidBrush(p));
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
            XMLNode circle = new XMLNode() { Name="circle" };
            circle.Attributes["cx"] = x0;
            circle.Attributes["cy"] = y0;
            circle.Attributes["r"] = r;
            circle.Attributes["fill"] = "none";
            circle.Attributes["stroke-width"] = thickness;
            circle.Attributes["stroke"] = Convert(p);
            root.Children.Add(circle);
        }
        public override void DrawCircle(Pixel p , int x0 , int y0 , double r , int thickness)
        {
            DrawCircle(new SolidBrush(p) , x0 , y0 , r , thickness);
        }
        public override void DrawEllipse(Pixel p , int x , int y , int w , int h)
        {
            DrawEllipse(p , x , y , w , h , 1);
        }
        
        public override void FillCircle(Brush<Pixel> p , int x0 , int y0 , double r)
        {
            XMLNode circle = new XMLNode() { Name="circle" };
            circle.Attributes["cx"] = x0;
            circle.Attributes["cy"] = y0;
            circle.Attributes["r"] = r;
            circle.Attributes["fill"] = Convert(p);
            circle.Attributes["stroke"] = Convert(p);
            root.Children.Add(circle);
        }
        public override void FillCircle(Pixel p , int x0 , int y0 , double r)
        {
            FillCircle(new SolidBrush(p) , x0 , y0 , r);
        }
       
        public override void DrawImage(MoyskleyTech.ImageProcessing.Image.Bitmap source , int x , int y)
        {
            root.Attributes["xmlns:xlink"] = "http://www.w3.org/1999/xlink";
            XMLNode image = new XMLNode() { Name="image" };
            image.Attributes["xlink:href"] = Convert(source);
            image.Attributes["x"] = x;
            image.Attributes["y"] = y;
            image.Attributes["width"] = source.Width;
            image.Attributes["height"] = source.Height;
            root.Children.Add(image);
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
            DrawLine(new SolidBrush(p) , x , y , x2 , y2 , thickness);
        }
        public override void DrawLine(Brush<Pixel> p , double x , double y , double x2 , double y2 , int thickness)
        {
            //<line x1="20" y1="100" x2="100" y2="20" stroke - width = "2" stroke = "black" />
            XMLNode line = new XMLNode() { Name="line" };
            line.Attributes["stroke"] = Convert(p);
            line.Attributes["x1"] = x;
            line.Attributes["y1"] = y;
            line.Attributes["x2"] = x2;
            line.Attributes["y2"] = y2;
            line.Attributes["stroke-width"] = thickness;
            root.Children.Add(line);
        }
        public override void DrawLine(Pixel p , MoyskleyTech.ImageProcessing.Image.PointF p1 , MoyskleyTech.ImageProcessing.Image.PointF p2)
        {
            DrawLine(p , p1.X , p1.Y , p2.X , p2.Y , 1);
        }
        public override void DrawLine(Pixel p , MoyskleyTech.ImageProcessing.Image.PointF p1 , MoyskleyTech.ImageProcessing.Image.PointF p2 , int thickness)
        {
            DrawLine(p , p1.X , p1.Y , p2.X , p2.Y , thickness);
        }
        public override void DrawLine(Brush<Pixel> p , MoyskleyTech.ImageProcessing.Image.PointF p1 , MoyskleyTech.ImageProcessing.Image.PointF p2)
        {
            DrawLine(p , p1.X , p1.Y , p2.X , p2.Y , 1);
        }
        public override void DrawLine(Brush<Pixel> p , MoyskleyTech.ImageProcessing.Image.PointF p1 , MoyskleyTech.ImageProcessing.Image.PointF p2 , int thickness)
        {
            DrawLine(p , p1.X , p1.Y , p2.X , p2.Y , thickness);
        }
        public override void FillPolygon(Brush<Pixel> p , params MoyskleyTech.ImageProcessing.Image.PointF[ ] points)
        {
            XMLNode polygon = new XMLNode() { Name="polygon" };
            polygon.Attributes["fill"] = Convert(p);
            polygon.Attributes["points"] = CreatePolygon(points);
            root.Children.Add(polygon);
        }


        public string CreatePath(params MoyskleyTech.ImageProcessing.Image.PointF[ ] points)
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
        public string CreatePolygon(params MoyskleyTech.ImageProcessing.Image.PointF[ ] points)
        {
            StringBuilder sb = new StringBuilder();
            sb.Append(points[0].X.ToString("").Replace(',' , '.'));
            sb.Append(',');
            sb.Append(points[0].Y.ToString("").Replace(',' , '.'));
            sb.Append(' ');
            foreach ( PointF pt in points.Skip(1) )
            {
                sb.Append(pt.X.ToString("").Replace(',' , '.'));
                sb.Append(',');
                sb.Append(pt.Y.ToString("").Replace(',' , '.'));
                sb.Append(' ');
            }
            return sb.ToString();
        }
        public override void FillPolygon(Pixel p , params MoyskleyTech.ImageProcessing.Image.PointF[ ] points)
        {
            FillPolygon(new SolidBrush(p) , points);
        }



        public override void DrawPolygon(Brush<Pixel> p , int thickness , params MoyskleyTech.ImageProcessing.Image.PointF[ ] points)
        {
            XMLNode path = new XMLNode() { Name="path" };
            path.Attributes["stroke"] = Convert(p);
            path.Attributes["stroke-width"] = thickness;
            path.Attributes["points"] = CreatePath(points);
            root.Children.Add(path);
        }
        public override void DrawPolygon(Brush<Pixel> p , params MoyskleyTech.ImageProcessing.Image.PointF[ ] points)
        {
            XMLNode path = new XMLNode() { Name="path" };
            path.Attributes["stroke"] = Convert(p);
            path.Attributes["points"] = CreatePath(points);
            root.Children.Add(path);
        }

        public override void DrawPolygon(Pixel p , int thickness , params MoyskleyTech.ImageProcessing.Image.PointF[ ] points)
        {
            DrawPolygon(new SolidBrush(p) , thickness , points);
        }
        public override void DrawPolygon(Pixel p , params MoyskleyTech.ImageProcessing.Image.PointF[ ] points)
        {
            DrawPolygon(p , 1 , points);
        }
        public override void DrawString(string str , Pixel p , int x , int y , Font f , int size , StringFormat sf = null)
        {
            DrawString(str , new SolidBrush(p) , x , y , f , size , sf);
        }
        public override void DrawString(string str , Brush<Pixel> p , int x , int y , Font f , int size , StringFormat sf = null)
        {
            if ( sf == null )
                sf = new StringFormat() { Alignment = StringAlignment.Near , LineAlignment = StringAlignment.Near };
            XMLNode text = new XMLNode() { Name="text" };
            text.Attributes["fill"] = Convert(p);
            text.Attributes["x"] = x;
            text.Attributes["y"] = y;
            text.Attributes["font-family"] = f.Name;
            text.Attributes["alignment-baseline"] = GetBaseline(sf); //hanging middle baseline
            text.Attributes["text-anchor"] = GetAnchor(sf); //start middle end
            text.Children.Add(new XMLNode(XMLNode.XMLNodeType.Text) { Name = str });
            root.Children.Add(text);
        }

        private object GetAnchor(StringFormat sf)
        {
            switch ( sf.Alignment )
            {
                case StringAlignment.Near:
                    return "start";
                case StringAlignment.Center:
                    return "middle";
                case StringAlignment.Far:
                default:
                    return "end";
            }
        }

        private object GetBaseline(StringFormat sf)
        {
            switch ( sf.LineAlignment )
            {
                case StringAlignment.Near:
                    return "hanging";
                case StringAlignment.Center:
                    return "middle";
                case StringAlignment.Far:
                default:
                    return "baseline";
            }
        }

        public override void SetPixel(Brush<Pixel> p , double x , double y)
        {
            DrawLine(p , new PointF(x , y) , new PointF(x + 1 , y) , 1);
        }
        public override void SetPixel(Pixel p , double x , double y)
        {
            DrawLine(p , new PointF(x , y) , new PointF(x + 1 , y) , 1);
        }
        protected override void SetPixelInternal(Pixel p , double px , double py,bool alpha)
        {
            FillRectangle(p , px , py , 1 , 1);
        }
        public string Convert(Bitmap bmp)
        {
            MemoryStream ms = new MemoryStream();
            bmp.Save(ms);
            ms.Position = 0;
            byte[] array = ms.ToArray();
            ms.Dispose();
            return "data:image/bmp;base64,"+System.Convert.ToBase64String(array);
        }

        private class SolidBrush : Brush
        {
            Pixel color;
            public SolidBrush(Pixel p)
            {
                color = p;
            }
            public override Pixel GetColor(int x , int y)
            {
                return color;
            }
        }
    }
}
