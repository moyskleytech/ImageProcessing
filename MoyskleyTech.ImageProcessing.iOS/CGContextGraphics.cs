using CoreGraphics;
using MoyskleyTech.ImageProcessing.Image;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UIKit;

namespace MoyskleyTech.ImageProcessing.iOS
{
    public class CGContextGraphics : Graphics
    {
        private CGContext ctx;
        private CGRect area;
        private static Func<Pixel,CGColor> converter;
        static CGContextGraphics()
        {
            if ( converter == null )
            {
                ColorConvert.RegisterTransition<ARGB_Float , CGColor>((px) => new CGColor(px.R , px.G , px.B , px.A) , 1);
                ColorConvert.CompleteTransitions();
                converter = ColorConvert.GetConversionFrom<Pixel , CGColor>();
            }
        }
        public void ReorientCoordinates()
        {
            ctx.ScaleCTM(1 , -1);
            ctx.TranslateCTM(0 , -area.Height);
        }
        private void ResetContext()
        {
            ctx.SetFillColor(0 , 0 , 0 , 0);
            ctx.SetStrokeColor(0 , 0 , 0 , 0);
            ctx.SetLineWidth(1);
        }
        public CGContextGraphics(CGContext context , CGRect a)
        {
            ctx = context;
            area = a;
        }
        private static CGColor Convert(Pixel p)
        {
            return converter(p);
        }
        private static CGColor Convert(Brush<Pixel> b)
        {
            var p=b.GetColor(0,0);
            return converter(p);
        }

        public override void Clear(Brush<Pixel> p)
        {
            ctx.SetFillColor(Convert(p));
            ctx.FillRect(area);
        }
        public override void Clear(Pixel p)
        {
            ctx.SetFillColor(Convert(p));
            ctx.FillRect(area);
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
            ResetContext();
            ctx.SetStrokeColor(Convert(p));
            ctx.SetLineWidth(thickness);
            ctx.AddArc(x0 , y0 , ( nfloat ) r , 0 , ( nfloat ) Math.PI * 2 , true);
            ctx.StrokePath();
        }
        public override void DrawCircle(Pixel p , int x0 , int y0 , double r , int thickness)
        {
            ResetContext();
            ctx.SetStrokeColor(Convert(p));
            ctx.SetLineWidth(thickness);
            ctx.AddArc(x0 , y0 , ( nfloat ) r , 0 , ( nfloat ) Math.PI * 2 , true);
            ctx.StrokePath();
        }
        public override void DrawEllipse(Pixel p , int x , int y , int w , int h)
        {
            DrawEllipse(p , x , y , w , h , 1);
        }
        public override void DrawEllipse(Pixel p , int x , int y , int w , int h , int t)
        {
            ResetContext();
            ctx.SetStrokeColor(Convert(p));
            ctx.SetLineWidth(t);

            ctx.StrokeEllipseInRect(new CGRect(x , y , w , h));
        }
        public override void DrawEllipse(Brush<Pixel> p , int x , int y , int w , int h)
        {
            ResetContext();
            ctx.SetStrokeColor(Convert(p));

            ctx.StrokeEllipseInRect(new CGRect(x , y , w , h));
        }
        public override void DrawEllipse(Brush<Pixel> p , int x , int y , int w , int h , int t)
        {
            ResetContext();
            ctx.SetStrokeColor(Convert(p));
            ctx.SetLineWidth(t);

            ctx.StrokeEllipseInRect(new CGRect(x , y , w , h));
        }
        public override void FillCircle(Brush<Pixel> p , int x0 , int y0 , double r)
        {
            ResetContext();
            ctx.SetStrokeColor(Convert(p));
            ctx.AddArc(x0 , y0 , ( nfloat ) r , 0 , ( nfloat ) Math.PI * 2 , true);
            ctx.FillPath();
        }
        public override void FillCircle(Pixel p , int x0 , int y0 , double r)
        {
            ResetContext();
            ctx.SetStrokeColor(Convert(p));
            ctx.AddArc(x0 , y0 , ( nfloat ) r , 0 , ( nfloat ) Math.PI * 2 , true);
            ctx.FillPath();
        }
        public override void FillEllipse(Pixel p , int x , int y , int w , int h)
        {
            ResetContext();
            ctx.SetStrokeColor(Convert(p));

            ctx.FillEllipseInRect(new CGRect(x , y , w , h));
        }
        public override void FillEllipse(Brush<Pixel> p , int x , int y , int w , int h)
        {
            ResetContext();
            ctx.SetStrokeColor(Convert(p));

            ctx.FillEllipseInRect(new CGRect(x , y , w , h));
        }
        public override void DrawImage(Image<Pixel> source , int x , int y)
        {
            ctx.DrawImage(new CGRect(x , y , source.Width , source.Height) , source.ToCGBitmap().ToImage());
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
            ResetContext();
            ctx.SetStrokeColor(Convert(p));
            ctx.SetLineWidth(thickness);
            ctx.BeginPath();
            ctx.MoveTo(( nfloat ) x , ( nfloat ) y);
            ctx.AddLineToPoint(( nfloat ) x2 , ( nfloat ) y2);
            ctx.StrokePath();
        }
        public override void DrawLine(Brush<Pixel> p , double x , double y , double x2 , double y2 , int thickness)
        {
            ResetContext();
            ctx.SetStrokeColor(Convert(p));
            ctx.SetLineWidth(thickness);
            ctx.BeginPath();
            ctx.MoveTo(( nfloat ) x , ( nfloat ) y);
            ctx.AddLineToPoint(( nfloat ) x2 , ( nfloat ) y2);
            ctx.StrokePath();
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
            ResetContext();
            ctx.SetFillColor(Convert(p));
            GetPath(points);
            ctx.FillPath();
        }

        private void GetPath(PointF[ ] points)
        {
            ctx.BeginPath();
            ctx.MoveTo((nfloat)points[0].X , ( nfloat ) points[0].Y);
            foreach ( var pt in points.Skip(1) )
            {
                ctx.AddLineToPoint(( nfloat ) pt.X , ( nfloat ) pt.Y);
            }
            ctx.AddLineToPoint(( nfloat ) points[0].X , ( nfloat ) points[0].Y);
        }

        public override void FillPolygon(Pixel p , params MoyskleyTech.ImageProcessing.Image.PointF[ ] points)
        {
            ResetContext();
            ctx.SetFillColor(Convert(p));
            GetPath(points);
            ctx.FillPath();
        }



        public override void DrawPolygon(Brush<Pixel> p , int thickness , params MoyskleyTech.ImageProcessing.Image.PointF[ ] points)
        {
            ResetContext();
            ctx.SetStrokeColor(Convert(p));
            ctx.SetLineWidth(thickness);
            GetPath(points);
            ctx.StrokePath();
        }
        public override void DrawPolygon(Brush<Pixel> p , params MoyskleyTech.ImageProcessing.Image.PointF[ ] points)
        {
            ResetContext();
            ctx.SetStrokeColor(Convert(p));
            GetPath(points);
            ctx.StrokePath();
        }

        public override void DrawPolygon(Pixel p , int thickness , params MoyskleyTech.ImageProcessing.Image.PointF[ ] points)
        {
            ResetContext();
            ctx.SetStrokeColor(Convert(p));
            ctx.SetLineWidth(thickness);
            GetPath(points);
            ctx.StrokePath();
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
        protected override void SetPixelInternal(Pixel p , double px , double py , bool alpha)
        {
            FillRectangle(p , px , py , 1 , 1);
        }
        public override void Dispose()
        {

        }

    }
}
