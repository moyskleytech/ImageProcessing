using System;
using System.Collections.Generic;
using MoyskleyTech.ImageProcessing.Text;

namespace MoyskleyTech.ImageProcessing.Image
{
#pragma warning disable CS1591
    /// <summary>
    /// Allow proxying of graphics to another colorspace
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <typeparam name="V"></typeparam>
    public class TypeProxyGraphics<T,V> : Graphics<T>
        where T: unmanaged
        where V: unmanaged
    {
        private Graphics<V> graphics;
        private Func<T , V> func;
        private Func<Brush<T>,Brush<V>> funcB;

        /// <summary>
        /// Create the proxy
        /// </summary>
        /// <param name="graphics"></param>
        /// <param name="func"></param>
        public TypeProxyGraphics(Graphics<V> graphics , Func<T , V> func)
        {
            this.graphics = graphics;
            this.func = func;
            funcB = (x) => new ConvertedBrush<T,V>() { Brush = x , Converter = func };
        }
        /// <summary>
        /// Proxy method
        /// </summary>
        /// <param name="p"></param>
        /// <param name="px"></param>
        /// <param name="py"></param>
        public override void SetPixelWithoutTransform(T p , double px , double py)
        {
            graphics.SetPixelWithoutTransform(func(p) , px , py);
        }
        /// <summary>
        /// Proxy method
        /// </summary>
        /// <param name="p"></param>
        /// <param name="px"></param>
        /// <param name="py"></param>
        /// <param name="alpha"></param>
        public override void SetPixelWithoutTransform(T p , double px , double py , bool alpha)
        {
            graphics.SetPixelWithoutTransform(func(p) , px , py , alpha);
        }
        /// <summary>
        /// Clear the graphics using a brush
        /// </summary>
        /// <param name="p"></param>
        public override void Clear(Brush<T> p)
        {
            graphics.Clear(funcB(p));
        }
        public override void Clear(T p)
        {
            graphics.Clear(func(p));
        }
        public override void Dispose()
        {
            graphics.Dispose();
        }
        public override void DrawCircle(Brush<T> p , int x0 , int y0 , double r)
        {
            graphics.DrawCircle(funcB(p) , x0 , y0 , r);
        }
        public override void DrawCircle(Brush<T> p , int x0 , int y0 , double r , int thickness)
        {
            graphics.DrawCircle(funcB(p) , x0 , y0 , r , thickness);
        }
        public override void DrawCircle(T p , int x0 , int y0 , double r)
        {
            graphics.DrawCircle(func(p) , x0 , y0 , r);
        }
        public override void DrawCircle(T p , int x0 , int y0 , double r , int thickness)
        {
            graphics.DrawCircle(func(p) , x0 , y0 , r , thickness);
        }
        public override void DrawEllipse(Brush<T> p , int x , int y , int w , int h)
        {
            graphics.DrawEllipse(funcB(p) , x , y , w , h);
        }
        public override void DrawEllipse(Brush<T> p , int x , int y , int w , int h , int t)
        {
            graphics.DrawEllipse(funcB(p) , x , y , w , h , t);
        }
        public override void DrawEllipse(T p , int x , int y , int w , int h)
        {
            graphics.DrawEllipse(func(p) , x , y , w , h);
        }
        public override void DrawEllipse(T p , int x , int y , int w , int h , int t)
        {
            graphics.DrawEllipse(func(p) , x , y , w , h , t);
        }
      
        public override void DrawFormattedString(string str , Brush<T> p , float x , float y , Font f , float size)
        {
            graphics.DrawFormattedString(str , funcB(p) , x , y , f , size);
        }
        public override void DrawFormattedString(string str , T p , float x , float y , Font f , float size)
        {
            graphics.DrawFormattedString(str , func(p) , x , y , f , size);
        }
        public override void DrawImage(ImageProxy<T> source , int x , int y)
        {
            var tmp=source.ToImage<V>();
            graphics.DrawImage(tmp , x , y);
            tmp.Dispose();
        }
        public override void DrawImage(ImageProxy<T> source , int x , int y , int width , int height)
        {
            var tmp=source.ToImage<V>();
            graphics.DrawImage(tmp , x , y,width,height);
            tmp.Dispose();
        }
        public override void DrawLine(Brush<T> p , double x , double y , double x2 , double y2)
        {
            graphics.DrawLine(funcB(p) , x , y , x2 , y2);
        }
        public override void DrawLine(Brush<T> p , double x , double y , double x2 , double y2 , int thickness)
        {
            graphics.DrawLine(funcB(p) , x , y , x2 , y2 , thickness);
        }
        public override void DrawLine(T p , double x , double y , double x2 , double y2)
        {
            graphics.DrawLine(func(p) , x , y , x2 , y2);
        }
        public override void DrawLine(T p , double x , double y , double x2 , double y2 , int thickness)
        {
            graphics.DrawLine(func(p) , x , y , x2 , y2 , thickness);
        }
        public override IEnumerable<PointF> DrawPie(Brush<T> p , int x0 , int y0 , double r , double spanAngle , double startAngle)
        {
            return graphics.DrawPie(funcB(p) , x0 , y0 , r , spanAngle , startAngle);
        }
        public override IEnumerable<PointF> DrawPie(Brush<T> p , int x0 , int y0 , double r , double spanAngle , double startAngle , int thickness)
        {
            return graphics.DrawPie(funcB(p) , x0 , y0 , r , spanAngle , startAngle , thickness);
        }
        public override IEnumerable<PointF> DrawPie(Brush<T> p , int x0 , int y0 , double w , double h , double spanAngle , double startAngle)
        {
            return graphics.DrawPie(funcB(p) , x0 , y0 , w , h , spanAngle , startAngle);
        }
        public override IEnumerable<PointF> DrawPie(Brush<T> p , int x0 , int y0 , double w , double h , double spanAngle , double startAngle , int thickness)
        {
            return graphics.DrawPie(funcB(p) , x0 , y0 , w , h , spanAngle , startAngle , thickness);
        }
        public override IEnumerable<PointF> DrawPie(T p , int x0 , int y0 , double r , double spanAngle , double startAngle)
        {
            return graphics.DrawPie(func(p) , x0 , y0 , r , spanAngle , startAngle);
        }
        public override IEnumerable<PointF> DrawPie(T p , int x0 , int y0 , double r , double spanAngle , double startAngle , int thickness)
        {
            return graphics.DrawPie(func(p) , x0 , y0 , r , spanAngle , startAngle , thickness);
        }
        public override IEnumerable<PointF> DrawPie(T p , int x0 , int y0 , double w , double h , double spanAngle , double startAngle)
        {
            return graphics.DrawPie(func(p) , x0 , y0 , w , h , spanAngle , startAngle);
        }
        public override IEnumerable<PointF> DrawPie(T p , int x0 , int y0 , double w , double h , double spanAngle , double startAngle , int thickness)
        {
            return graphics.DrawPie(func(p) , x0 , y0 , w , h , spanAngle , startAngle , thickness);
        }
       
        public override void DrawPolygon(Brush<T> p , int thickness , params PointF[ ] points)
        {
            graphics.DrawPolygon(funcB(p) , thickness , points);
        }
        public override void DrawPolygon(Brush<T> p , params PointF[ ] points)
        {
            graphics.DrawPolygon(funcB(p) , points);
        }
       
        public override void DrawPolygon(T p , int thickness , params PointF[ ] points)
        {
            graphics.DrawPolygon(func(p) , thickness , points);
        }
        public override void DrawPolygon(T p , params PointF[ ] points)
        {
            graphics.DrawPolygon(func(p) , points);
        }
        public override void DrawRectangle(Brush<T> p , double x , double y , double w , double h)
        {
            graphics.DrawRectangle(funcB(p) , x , y , w , h);
        }
        public override void DrawRectangle(Brush<T> p , double x , double y , double w , double h , int thick)
        {
            graphics.DrawRectangle(funcB(p) , x , y , w , h , thick);
        }
       
        public override void DrawRectangle(T p , double x , double y , double w , double h)
        {
            graphics.DrawRectangle(func(p) , x , y , w , h);
        }
        public override void DrawRectangle(T p , double x , double y , double w , double h , int thick)
        {
            graphics.DrawRectangle(func(p) , x , y , w , h , thick);
        }
     
        public override void DrawRotatedEllipse(Brush<T> b , double x , double y , double major , double minor , double angle , int thickness = 0 , double angleIncrement = 0)
        {
            graphics.DrawRotatedEllipse(funcB(b) , x , y , major , minor , angle , thickness , angleIncrement);
        }
        public override void DrawRotatedEllipse(T b , double x , double y , double major , double minor , double angle , int thickness = 0 , double angleIncrement = 0)
        {
            graphics.DrawRotatedEllipse(func(b) , x , y , major , minor , angle , thickness , angleIncrement);
        }
        public override void DrawString(string str , Brush<T> p , int ox , int oy , Font f , float size , StringFormat sf = null)
        {
            graphics.DrawString(str , funcB(p) , ox , oy , f , size , sf);
        }
        public override void DrawString(string str , T p , int ox , int oy , Font f , float size , StringFormat sf = null)
        {
            graphics.DrawString(str , func(p) , ox , oy , f , size , sf);
        }
        public override bool Equals(object obj)
        {
            return graphics.Equals(obj);
        }
        public override void FillCircle(Brush<T> p , int x0 , int y0 , double r)
        {
            graphics.FillCircle(funcB(p) , x0 , y0 , r);
        }
        public override void FillCircle(T p , int x0 , int y0 , double r)
        {
            graphics.FillCircle(func(p) , x0 , y0 , r);
        }
        public override void FillEllipse(Brush<T> p , int x , int y , int w , int h)
        {
            graphics.FillEllipse(funcB(p) , x , y , w , h);
        }
        public override void FillEllipse(T p , int x , int y , int w , int h)
        {
            graphics.FillEllipse(func(p) , x , y , w , h);
        }
        public override void FillPie(Brush<T> p , int x0 , int y0 , double r , double spanAngle , double startAngle)
        {
            graphics.FillPie(funcB(p) , x0 , y0 , r , spanAngle , startAngle);
        }
        public override void FillPie(Brush<T> p , int x0 , int y0 , double w , double h , double spanAngle , double startAngle)
        {
            graphics.FillPie(funcB(p) , x0 , y0 , w , h , spanAngle , startAngle);
        }
        public override void FillPie(T p , int x0 , int y0 , double r , double spanAngle , double startAngle)
        {
            graphics.FillPie(func(p) , x0 , y0 , r , spanAngle , startAngle);
        }
        public override void FillPie(T p , int x0 , int y0 , double w , double h , double spanAngle , double startAngle)
        {
            graphics.FillPie(func(p) , x0 , y0 , w , h , spanAngle , startAngle);
        }
        public override void FillPolygon(Brush<T> p , params PointF[ ] points)
        {
            graphics.FillPolygon(funcB(p) , points);
        }
        public override void FillPolygon(T p , params PointF[ ] points)
        {
            graphics.FillPolygon(func(p) , points);
        }
        public override void FillRectangle(Brush<T> p , double px , double py , double pw , double ph)
        {
            graphics.FillRectangle(funcB(p) , px , py , pw , ph);
        }
        public override void FillRectangle(T p , double px , double py , double pw , double ph)
        {
            graphics.FillRectangle(func(p) , px , py , pw , ph);
        }
        public override void FillRotatedEllipse(Brush<T> b , double x , double y , double major , double minor , double angle , double angleIncrement = 0)
        {
            graphics.FillRotatedEllipse(funcB(b) , x , y , major , minor , angle , angleIncrement);
        }
        public override void FillRotatedEllipse(T b , double x , double y , double major , double minor , double angle , double angleIncrement = 0)
        {
            graphics.FillRotatedEllipse(func(b) , x , y , major , minor , angle , angleIncrement);
        }
        public override int GetHashCode()
        {
            return graphics.GetHashCode();
        }
        public override int Height { get => graphics.Height; set => graphics.Height = value; }
        public override StringMeasurement MeasureString(string str , Font f , float size)
        {
            return graphics.MeasureString(str , f , size);
        }
        public override void ProjectTransform(PointF[ ] src , PointF[ ] dst)
        {
            graphics.ProjectTransform(src , dst);
        }
        public override void ResetClip()
        {
            graphics.ResetClip();
        }
        public override void ResetTransform()
        {
            graphics.ResetTransform();
        }
        public override void RotateTransform(double angle)
        {
            graphics.RotateTransform(angle);
        }
        public override void ScaleTransform(double x , double y)
        {
            graphics.ScaleTransform(x , y);
        }
        public override void SetClip(PointF[ ] r)
        {
            graphics.SetClip(r);
        }
        public override void SetClip(Rectangle r)
        {
            graphics.SetClip(r);
        }
        public override void SetPixel(Brush<T> p , double x , double y)
        {
            graphics.SetPixel(funcB(p) , x , y);
        }
        public override void SetPixel(T p , double x , double y)
        {
            graphics.SetPixel(func(p) , x , y);
        }
        public override void SkewXTransform(double a)
        {
            graphics.SkewXTransform(a);
        }
        public override void SkewYTransform(double a)
        {
            graphics.SkewYTransform(a);
        }
        public override void TranslateTransform(double x , double y)
        {
            graphics.TranslateTransform(x , y);
        }
        public override int Width { get => base.Width; set => base.Width = value; }
        public override void DrawPath(Brush<T> p , int thickness , params PointF[ ] points)
        {
            graphics.DrawPath(funcB(p) , thickness , points);
        }
        public override void DrawPath(Brush<T> p , params PointF[ ] points)
        {
            graphics.DrawPath(funcB(p) , points);
        }
        public override void DrawPath(T p , int thickness , params PointF[ ] points)
        {
            graphics.DrawPath(func(p) , thickness , points);
        }
        public override void DrawPath(T p , params PointF[ ] points)
        {
            graphics.DrawPath(func(p) , points);
        }
        public override void DrawString(string text , FontSizeF font , T foreColor , Rectangle textBound , StringFormat textAlign)
        {
            graphics.DrawString(text , font , func(foreColor) , textBound , textAlign);
        }
        public override void DrawString(string text , FontSizeF font , Brush<T> color , Rectangle area , StringFormat sf)
        {
            graphics.DrawString(text , font , funcB(color) , area , sf);
        }
    }
}