using MoyskleyTech.ImageProcessing.Image;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using A = global::Android;
using G = global::Android.Graphics;

namespace MoyskleyTech.ImageProcessing.Android
{
    public class CanvasGraphics : Graphics
    {
        static CanvasGraphics()
        {
            ColorConvertAndroid.RegisterIfNot();
        }
        private G.Canvas ctx;
        private Dictionary<Pixel,G.Paint> cache;
        private Dictionary<Pixel,G.Paint> cacheFill;
        public CanvasGraphics(G.Canvas context)
        {
            ctx = context;
            cache = new Dictionary<Pixel , G.Paint>();
            cacheFill = new Dictionary<Pixel , G.Paint>();
        }
        private G.Paint Convert(Pixel p)
        {
            if ( cache.ContainsKey(p) )
                return cache[p];
            G.Paint paint = new G.Paint();
            paint.SetStyle(G.Paint.Style.Stroke);
            paint.Color = G.Color.Argb(p.A , p.R , p.G , p.B);

            return cache[p] = paint;
        }
        private G.Paint ConvertFill(Pixel p)
        {
            if ( cacheFill.ContainsKey(p) )
                return cacheFill[p];
            G.Paint paint = new G.Paint();
            paint.SetStyle(G.Paint.Style.Fill);
            paint.Color = G.Color.Argb(p.A , p.R , p.G , p.B);
            return cacheFill[p] = paint;
        }
        private G.Paint Convert(Brush<Pixel> myBrush , double x , double y)
        {
            G.Paint paint = new G.Paint();
            if ( myBrush is LinearGradientBrush lgb )
            {
                paint.SetShader(new G.LinearGradient(lgb.SourceLocation.X , lgb.SourceLocation.Y , lgb.FinalLocation.X , lgb.FinalLocation.Y , G.Color.Argb(lgb.SourceColor.A , lgb.SourceColor.R , lgb.SourceColor.G , lgb.SourceColor.B) , G.Color.Argb(lgb.FinalColor.A , lgb.FinalColor.R , lgb.FinalColor.G , lgb.FinalColor.B) , G.Shader.TileMode.Clamp));
                return paint;
            }
            if ( myBrush is ImageBrush imgBrush )
            {
                var aBitmap = imgBrush.Image.ToAndroidBitmap();
                paint.SetShader(new G.BitmapShader(aBitmap , G.Shader.TileMode.Repeat , G.Shader.TileMode.Repeat));
                return paint;
            }
            if ( myBrush is RadialGradientBrush rgb )
            {
                paint.SetShader(new G.RadialGradient(rgb.SourceLocation.X , rgb.SourceLocation.Y , ( float ) rgb.Radius , G.Color.Argb(rgb.SourceColor.A , rgb.SourceColor.R , rgb.SourceColor.G , rgb.SourceColor.B) , G.Color.Argb(rgb.FinalColor.A , rgb.FinalColor.R , rgb.FinalColor.G , rgb.FinalColor.B) , G.Shader.TileMode.Clamp));
                return paint;
            }
            paint.SetARGB(0 , 0 , 0 , 0);
            return paint;
        }

        private G.Paint ConvertFill(Brush<Pixel> p , double x , double y)
        {
            var paint = Convert(p,x,y);
            paint.SetStyle(G.Paint.Style.Fill);
            return paint;
        }
        private G.Paint Stroke(G.Paint src , float thickness)
        {
            G.Paint p = new G.Paint
            {
                Color = src.Color
            };
            if ( src.Shader != null )
                p.SetShader(src.Shader);
            p.StrokeWidth = thickness;
            p.SetStyle(src.GetStyle());
            return p;
        }



        public override void Clear(Brush<Pixel> p)
        {
            //for ( var x = 0; x < ctx.Width; x++ )
            //{
            //    for ( var y = 0; y < ctx.Height; y++ )
            //    {
            //        ctx.DrawRect(x , y , x + 1 , y + 1 , Convert(p.GetColor(x , y)));
            //    }
            //}
            ctx.DrawPaint(ConvertFill(p , 0 , 0));
        }
        public override void Clear(Pixel p)
        {
            ctx.DrawPaint(ConvertFill(p));
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
            var paint = Stroke(Convert(p,x0-r,y0-r),thickness);
            ctx.DrawCircle(x0 , y0 , ( float ) r , paint);
        }
        public override void DrawCircle(Pixel p , int x0 , int y0 , double r , int thickness)
        {
            var paint = Stroke(Convert(p),thickness);
            ctx.DrawCircle(x0 , y0 , ( float ) r , paint);
        }
        public override void DrawEllipse(Pixel p , int x , int y , int w , int h)
        {
            DrawEllipse(p , x , y , w , h , 1);
        }
        public override void DrawEllipse(Pixel p , int x , int y , int w , int h , int t)
        {
            G.RectF rect = new G.RectF(x,y,w,h);
            ctx.DrawOval(rect , Stroke(Convert(p) , t));
        }
        public override void DrawEllipse(Brush<Pixel> p , int x , int y , int w , int h)
        {
            G.RectF rect = new G.RectF(x,y,w,h);
            ctx.DrawOval(rect , Convert(p , x , y));
        }
        public override void DrawEllipse(Brush<Pixel> p , int x , int y , int w , int h , int t)
        {
            G.RectF rect = new G.RectF(x,y,w,h);
            ctx.DrawOval(rect , Stroke(Convert(p , x , y) , t));
        }
        public override void FillCircle(Brush<Pixel> p , int x0 , int y0 , double r)
        {
            ctx.DrawCircle(x0 , y0 , ( float ) r , ConvertFill(p , x0 - r , y0 - r));
        }
        public override void FillCircle(Pixel p , int x0 , int y0 , double r)
        {
            ctx.DrawCircle(x0 , y0 , ( float ) r , ConvertFill(p));
        }
        public override void FillEllipse(Pixel p , int x , int y , int w , int h)
        {
            G.RectF rect = new G.RectF(x,y,w,h);
            ctx.DrawOval(rect , ConvertFill(p));
        }
        public override void FillEllipse(Brush<Pixel> p , int x , int y , int w , int h)
        {
            G.RectF rect = new G.RectF(x,y,w,h);
            ctx.DrawOval(rect , ConvertFill(p , x , y));
        }
        public override void DrawImage(ImageProxy<Pixel> source , int x , int y)
        {
            Image<Pixel> bitmap = source.ToImage();
            ctx.DrawBitmap(bitmap.ToAndroidBitmap() , x , y , null);
            bitmap.Dispose();
        }
        public void DrawImage(G.Bitmap source , int x , int y)
        {
            ctx.DrawBitmap(source , x , y , null);
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
            ctx.DrawLine(( float ) x , ( float ) y , ( float ) x2 , ( float ) y2 , Stroke(Convert(p) , thickness));
        }
        public override void DrawLine(Brush<Pixel> p , double x , double y , double x2 , double y2 , int thickness)
        {
            ctx.DrawLine(( float ) x , ( float ) y , ( float ) x2 , ( float ) y2 , Stroke(Convert(p , x , y) , thickness));
        }
        public override void FillPolygon(Brush<Pixel> p , params MoyskleyTech.ImageProcessing.Image.PointF[ ] points)
        {
            G.Path wallpath = GetPath(points);
            ctx.DrawPath(wallpath , ConvertFill(p , 0 , 0));
        }
        private static G.Path GetPath(PointF[ ] points)
        {
            G.Path wallpath = new G.Path();
            wallpath.Reset();
            wallpath.MoveTo(( float ) points[0].X , ( float ) points[0].Y);
            foreach ( var pt in points )
            {
                wallpath.LineTo(( float ) pt.X , ( float ) pt.Y);
            }
            wallpath.LineTo(( float ) points[0].X , ( float ) points[0].Y);
            return wallpath;
        }

        public override void FillPolygon(Pixel p , params MoyskleyTech.ImageProcessing.Image.PointF[ ] points)
        {
            G.Path wallpath = GetPath(points);
            ctx.DrawPath(wallpath , ConvertFill(p));
        }



        public override void DrawPolygon(Brush<Pixel> p , int thickness , params MoyskleyTech.ImageProcessing.Image.PointF[ ] points)
        {
            G.Path wallpath = GetPath(points);
            ctx.DrawPath(wallpath , Stroke(Convert(p , 0 , 0) , thickness));
        }
        public override void DrawPolygon(Brush<Pixel> p , params MoyskleyTech.ImageProcessing.Image.PointF[ ] points)
        {
            G.Path wallpath = GetPath(points);
            ctx.DrawPath(wallpath , Convert(p , 0 , 0));
        }

        public override void DrawPolygon(Pixel p , int thickness , params MoyskleyTech.ImageProcessing.Image.PointF[ ] points)
        {
            G.Path wallpath = GetPath(points);
            ctx.DrawPath(wallpath , Stroke(Convert(p) , thickness));
        }
        public override void DrawPolygon(Pixel p , params MoyskleyTech.ImageProcessing.Image.PointF[ ] points)
        {
            DrawPolygon(p , 1 , points);
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
        public override void Dispose()
        {

        }
        protected override Pixel this[int m]
        {
            get
            {
                return Pixels.Black;
            }
            set
            {
                SetPixelWithoutTransform(value , m % width.Value , m / width.Value);
            }
        }
        protected override Pixel this[int x , int y]
        {
            get
            {
                return Pixels.Black;
            }
            set
            {
                SetPixelWithoutTransform(value , x , y);
            }
        }
        public override int Height { get => ctx.Height; set { } }
        public override int Width { get => ctx.Width; set { } }
    }
}
