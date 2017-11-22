using MoyskleyTech.ImageProcessing.Image;
using MoyskleyTech.ImageProcessing.WinForm;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MoyskleyTech.ImageProcessing.WinForm
{
    public class NativeGraphicsWrapper : Graphics
    {
        private System.Drawing.Graphics ctx;
        private const float FONT_FACTOR = 12f;
        public NativeGraphicsWrapper(System.Drawing.Graphics context)
        {
            ctx = context;
            ctx.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.None;
            ctx.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.Bilinear;
            ctx.PixelOffsetMode = System.Drawing.Drawing2D.PixelOffsetMode.HighSpeed;
        }
        public override void Clear(Brush p)
        {
            var c=ConvertToColor(p);
            ctx.Clear(c);
        }



        public override void Clear(Pixel p)
        {
            ctx.Clear(ConvertToColor(p));
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
            var pen = ConvertToPen(p , thickness);
            ctx.DrawEllipse(pen , ( float ) ( x0 - r ) , ( float ) ( y0 - r ) , ( float ) ( r * 2 ) , ( float ) ( r * 2 ));
            pen.Dispose();
        }
        public override void DrawCircle(Pixel p , int x0 , int y0 , double r , int thickness)
        {
            var pen=ConvertToPen(p , thickness);
            ctx.DrawEllipse(pen , ( float ) ( x0 - r ) , ( float ) ( y0 - r ) , ( float ) ( r * 2 ) , ( float ) ( r * 2 ));
            pen.Dispose();
        }
        public override void DrawEllipse(Pixel p , int x , int y , int w , int h)
        {
            DrawEllipse(p , x , y , w , h , 1);
        }
        public override void DrawEllipse(Pixel p , int x , int y , int w , int h , int t)
        {
            var pen = ConvertToPen(p , t);
            ctx.DrawEllipse(pen , x , y , w , h);
            pen.Dispose();
        }
        public override void DrawEllipse(Brush p , int x , int y , int w , int h)
        {
            DrawEllipse(p , x , y , w , h , 1);
        }
        public override void DrawEllipse(Brush p , int x , int y , int w , int h , int t)
        {
            var pen = ConvertToPen(p , t);
            ctx.DrawEllipse(pen , x , y , w , h);
            pen.Dispose();
        }
        public override void FillCircle(Brush p , int x0 , int y0 , double r)
        {
            var brush = ConvertToBrush(p);
            ctx.FillEllipse(brush , ( float ) ( x0 - r ) , ( float ) ( y0 - r ) , ( float ) ( r * 2 ) , ( float ) ( r * 2 ));
            brush.Dispose();
        }
        public override void FillCircle(Pixel p , int x0 , int y0 , double r)
        {
            var brush = ConvertToBrush(p);
            ctx.FillEllipse(brush , ( float ) ( x0 - r ) , ( float ) ( y0 - r ) , ( float ) ( r * 2 ) , ( float ) ( r * 2 ));
            brush.Dispose();
        }
        public override void FillEllipse(Pixel p , int x , int y , int w , int h)
        {
            var brush = ConvertToBrush(p);
            ctx.FillEllipse(brush , x , y , w , h);
            brush.Dispose();
        }
        public override void FillEllipse(Brush p , int x , int y , int w , int h)
        {
            var brush = ConvertToBrush(p);
            ctx.FillEllipse(brush , x , y , w , h);
            brush.Dispose();
        }
        public override void DrawImage(MoyskleyTech.ImageProcessing.Image.Bitmap source , int x , int y)
        {
            var img=source.ToWinFormBitmap();
            ctx.DrawImage(img , x , y);
            img.Dispose();
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
            var pen=ConvertToPen(p,thickness);
            ctx.DrawLine(pen , ( float ) x , ( float ) y , ( float ) x2 , ( float ) y2);
            pen.Dispose();
        }
        public override void DrawLine(Brush p , double x , double y , double x2 , double y2 , int thickness)
        {
            var pen=ConvertToPen(p,thickness);
            ctx.DrawLine(pen , ( float ) x , ( float ) y , ( float ) x2 , ( float ) y2);
            pen.Dispose();
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
            var brush = ConvertToBrush(p);
            ctx.FillPolygon(brush , ( from x in points select new System.Drawing.PointF(( float ) x.X , ( float ) x.Y) ).ToArray());
            brush.Dispose();
        }
        public override void FillPolygon(Pixel p , params MoyskleyTech.ImageProcessing.Image.PointF[ ] points)
        {
            var brush = ConvertToBrush(p);
            ctx.FillPolygon(brush , ( from x in points select new System.Drawing.PointF(( float ) x.X , ( float ) x.Y) ).ToArray());
            brush.Dispose();
        }
        public override void DrawPolygon(Brush p , int thickness , params MoyskleyTech.ImageProcessing.Image.PointF[ ] points)
        {
            var pen =ConvertToPen(p,thickness);
            ctx.DrawPolygon(pen , ( from x in points select new System.Drawing.PointF(( float ) x.X , ( float ) x.Y) ).ToArray());
            pen.Dispose();
        }
        public override void DrawPolygon(Brush p , params MoyskleyTech.ImageProcessing.Image.PointF[ ] points)
        {
            var pen=ConvertToPen(p , 1);
            ctx.DrawPolygon(pen , ( from x in points select new System.Drawing.PointF(( float ) x.X , ( float ) x.Y) ).ToArray());
            pen.Dispose();
        }

        public override void DrawPolygon(Pixel p , int thickness , params MoyskleyTech.ImageProcessing.Image.PointF[ ] points)
        {
            var pen = ConvertToPen(p , thickness);
            ctx.DrawPolygon(pen , ( from x in points select new System.Drawing.PointF(( float ) x.X , ( float ) x.Y) ).ToArray());
            pen.Dispose();
        }
        public override void DrawPolygon(Pixel p , params MoyskleyTech.ImageProcessing.Image.PointF[ ] points)
        {
            DrawPolygon(p , 1 , points);
        }
        public override void DrawString(string str , Pixel p , int x , int y , Font f , int size , StringFormat sf = null)
        {
            var brush = ConvertToBrush(p);
            var font=new System.Drawing.Font(f.Name , size*FONT_FACTOR);
            ctx.DrawString(str , font , brush , x , y , Convert(sf));
            font.Dispose();
            brush.Dispose();
        }
        public override void DrawString(string str , Brush p , int x , int y , Font f , int size , StringFormat sf = null)
        {
            var brush = ConvertToBrush(p);
            var font=new System.Drawing.Font(f.Name , size*FONT_FACTOR);
            ctx.DrawString(str , font , brush , x , y , Convert(sf));
            font.Dispose();
            brush.Dispose();
        }
        public override void DrawString(string str , Pixel p , int x , int y , Font f , float size , StringFormat sf = null)
        {
            var brush = ConvertToBrush(p);
            var font=new System.Drawing.Font(f.Name , size*FONT_FACTOR);
            ctx.DrawString(str , font , brush , x , y , Convert(sf));
            font.Dispose();
            brush.Dispose();
        }
        public override void DrawString(string str , Brush p , int x , int y , Font f , float size , StringFormat sf = null)
        {
            var brush = ConvertToBrush(p);
            var font=new System.Drawing.Font(f.Name , size*FONT_FACTOR);
            ctx.DrawString(str , font , brush , x , y , Convert(sf));
            font.Dispose();
            brush.Dispose();
        }


        public override void SetPixel(Brush p , double x , double y)
        {
            FillRectangle(p , x , y , 1 , 1);
        }
        public override void SetPixel(Pixel p , double x , double y)
        {
            FillRectangle(p , x , y , 1 , 1);
        }
        protected override void SetPixelInternal(Pixel p , double px , double py)
        {

        }
        public void DrawImage(System.Drawing.Bitmap src , int x , int y)
        {
            ctx.DrawImage(src , x , y);
        }
        private System.Drawing.Color ConvertToColor(Image.Brush p)
        {
            var clr = p.GetColor(0,0);
            return System.Drawing.Color.FromArgb(clr.A , clr.R , clr.G , clr.B);
        }
        private System.Drawing.Color ConvertToColor(Image.Pixel p)
        {
            var clr = p;
            return System.Drawing.Color.FromArgb(clr.A , clr.R , clr.G , clr.B);
        }
        private System.Drawing.Pen ConvertToPen(Image.Pixel p , float width)
        {
            return new System.Drawing.Pen(ConvertToColor(p) , width);
        }
        private System.Drawing.Pen ConvertToPen(Image.Brush p , float width)
        {
            var brush = ConvertToBrush(p);
            return new System.Drawing.Pen(brush , width);
        }

        private System.Drawing.Brush ConvertToBrush(Image.Brush myBrush)
        {
            if ( myBrush is ImageBrush )
            {
                MemoryStream ms = new MemoryStream();
                var img = ( myBrush as ImageBrush ).Image;
                return new System.Drawing.TextureBrush(img.ToWinFormBitmap() , System.Drawing.Drawing2D.WrapMode.Tile);
            }
            {
                LinearGradientBrush lgb = myBrush as LinearGradientBrush;
                if ( lgb != null )
                {
                    var nlgb = new System.Drawing.Drawing2D.LinearGradientBrush(
                         new System.Drawing.Point(lgb.SourceLocation.X, lgb.SourceLocation.Y),
                         new System.Drawing.Point(lgb.FinalLocation.X, lgb.FinalLocation.Y),
                         ConvertToColor(lgb.SourceColor),
                         ConvertToColor(lgb.FinalColor)
                        );
                    return nlgb;
                }
            }
            return new System.Drawing.SolidBrush(ConvertToColor(myBrush));
        }
        private System.Drawing.Brush ConvertToBrush(Image.Pixel p)
        {
            return new System.Drawing.SolidBrush(ConvertToColor(p));
        }
        private System.Drawing.StringFormat Convert(Image.StringFormat sf)
        {
            return new System.Drawing.StringFormat() { Alignment = Convert(sf.Alignment) , LineAlignment = Convert(sf.LineAlignment) };
        }
        private System.Drawing.StringAlignment Convert(Image.StringAlignment alignment)
        {
            return ( System.Drawing.StringAlignment ) Enum.Parse(typeof(System.Drawing.StringAlignment) , alignment.ToString());
        }
        private System.Drawing.PointF[ ] Convert(PointF[ ] pts)
        {
            return ( from x in pts select new System.Drawing.PointF(( float ) x.X , ( float ) x.Y) ).ToArray();
        }
        private System.Drawing.SizeF Convert(StringMeasurement sm)
        {
            return new System.Drawing.SizeF(sm.Width , sm.Height);
        }
        private StringMeasurement Convert(System.Drawing.SizeF sm)
        {
            return new StringMeasurement() { Width = sm.Width , Height = sm.Height };
        }
        public override void RotateTransform(double angle)
        {
            ctx.RotateTransform(( float ) angle);
        }
        public override void ResetTransform()
        {
            ctx.ResetTransform();
        }
        public override void ResetClip()
        {
            ctx.ResetClip();
        }
        public override void SetClip(Rectangle r)
        {
            ctx.SetClip(new System.Drawing.Rectangle(r.X , r.Y , r.Width , r.Height));
        }
        public override void SetClip(PointF[ ] r)
        {
            var gp = new System.Drawing.Drawing2D.GraphicsPath();
            gp.AddLines(Convert(r));
            ctx.SetClip(gp);
        }
        public override void TranslateTransform(double x , double y)
        {
            ctx.TranslateTransform(( float ) x , ( float ) y);
        }
        public override void ScaleTransform(double x , double y)
        {
            ctx.ScaleTransform(( float ) x , ( float ) y);
        }
        public override StringMeasurement MeasureString(string str , Font f , float size)
        {
            var font=new System.Drawing.Font(f.Name , size*FONT_FACTOR);
            var ret=Convert(ctx.MeasureString(str , font));
            font.Dispose();
            return ret;
        }
        public override void DrawRectangle(Pixel p , Rectangle r)
        {
            var pen = ConvertToPen(p , 1);
            ctx.DrawRectangle(pen , r.X , r.Y , r.Width , r.Height);
            pen.Dispose();
        }
        public override void Dispose()
        {
            ctx.Dispose();
        }
    }
}
