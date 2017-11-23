using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MoyskleyTech.Math.Matrix;
using MoyskleyTech.ImageProcessing.Image.Helper;
using System.Reflection;

namespace MoyskleyTech.ImageProcessing.Image
{
    /// <summary>
    /// PCL Portability to System.Drawing.Graphics
    /// </summary>
    public unsafe partial class Graphics : IDisposable
    {
        private static List<Type> subTypes = new List<Type>();
        /// <summary>
        /// The bitmap where to draw
        /// </summary>
        private Bitmap bmp;
        /// <summary>
        /// The transformation matrix
        /// </summary>
        private Matrix transformationMatrix;
        /// <summary>
        /// The clip(application to setpixel)
        /// </summary>
        private Rectangle clip;
        private PointF[] clipPolygon;
        private double[]clipPolygonConstant , clipPolygonMultiple;

        public LineMode LineMode = LineMode.ForLoop;
        /// <summary>
        /// Register Subtype
        /// </summary>
        /// <param name="t"></param>
        public static void RegisterSubType(Type t)
        {
            if ( t.GetTypeInfo().IsSubclassOf(typeof(Graphics)) )
                subTypes.Add(t);
            else
                throw new ArgumentException("t is not typeof(Graphics)" , "t");
        }
        /// <summary>
        /// Only for subclassing
        /// </summary>
        protected Graphics() { }
        /// <summary>
        /// Create a Graphics object from image
        /// </summary>
        /// <param name="bmp">The bitmap where to Draw</param>
        /// <returns>Return null if not applicable</returns>
        public static Graphics FromImage(Bitmap bmp)
        {
            Graphics instance;
            instance = new Graphics(1);
            instance.bmp = bmp;
            instance.ResetClip();
            return instance;
        }
        /// <summary>
        /// Create a graphics object from type
        /// </summary>
        /// <param name="type">Type of graphics</param>
        /// <returns></returns>
        public static Graphics FromType(string type)
        {
            Graphics instance;
            foreach ( var t in subTypes )
            {
                var method = t.GetRuntimeMethod("_FromType" , new Type[ ] { typeof(string) });
                instance = ( Graphics ) method.Invoke(null , new object[ ] { type });
                if ( instance != null )
                    return instance;
            }
            return null;
        }
        /// <summary>
        /// Private constructor to things not related to bitmap
        /// </summary>
        private Graphics(int u)
        {
            transformationMatrix = Matrix.Identity(3);
        }
        /// <summary>
        /// Set the clipping for SetPixel
        /// </summary>
        /// <param name="r">Clip Rectangle</param>
        public virtual void SetClip(Rectangle r)
        {
            ResetClip();
            clip = r;
        }
        /// <summary>
        /// Set the clipping for SetPixel
        /// </summary>
        /// <param name="r">Clip Rectangle</param>
        public virtual void SetClip(PointF[ ] r)
        {
            ResetClip();
            clipPolygon = r;
            clipPolygonConstant = new double[clipPolygon.Length];
            clipPolygonMultiple = new double[clipPolygon.Length];
            PrecalculatePnPolyValues(clipPolygon , clipPolygonConstant , clipPolygonMultiple);
        }
        /// <summary>
        /// Reset the clip to the full bitmap
        /// </summary>
        public virtual void ResetClip()
        {
            clip = new Rectangle() { X = 0 , Y = 0 , Width = bmp.Width , Height = bmp.Height };
            clipPolygon = null;
            clipPolygonConstant = null;
            clipPolygonMultiple = null;
        }
        /// <summary>
        /// Get the transformation matrix for editing purpose
        /// </summary>
        public Matrix TransformationMatrix { get { return transformationMatrix; } }

        /// <summary>
        /// Draw a rectangle of the specified color
        /// </summary>
        /// <param name="p">The color</param>
        /// <param name="x">X position</param>
        /// <param name="y">Y position</param>
        /// <param name="w">Width</param>
        /// <param name="h">Height</param>
        public virtual void DrawRectangle(Pixel p , double x , double y , double w , double h)
        {
            DrawLine(p , x , y , x , y + h); //Left
            DrawLine(p , x , y , x + w , y); //Top
            DrawLine(p , x + w , y , x + w , y + h);//Right
            DrawLine(p , x , y + h , x + w , y + h);//Bottom
        }
        /// <summary>
        /// Draw a rectangle of the specified color
        /// </summary>
        /// <param name="p">The color</param>
        /// <param name="x">X position</param>
        /// <param name="y">Y position</param>
        /// <param name="w">Width</param>
        /// <param name="h">Height</param>
        public virtual void DrawRectangle(Brush p , double x , double y , double w , double h)
        {
            DrawLine(p , x , y , x , y + h); //Left
            DrawLine(p , x , y , x + w , y); //Top
            DrawLine(p , x + w , y , x + w , y + h);//Right
            DrawLine(p , x , y + h , x + w , y + h);//Bottom
        }
        /// <summary>
        /// Draw a rectangle of the specified color
        /// </summary>
        /// <param name="p">The color</param>
        /// <param name="x">X position</param>
        /// <param name="y">Y position</param>
        /// <param name="w">Width</param>
        /// <param name="h">Height</param>
        /// <param name="thick">Line thickness</param>
        public virtual void DrawRectangle(Pixel p , double x , double y , double w , double h , int thick)
        {
            DrawLine(p , x , y , x , y + h , thick); //Left
            DrawLine(p , x , y , x + w , y , thick); //Top
            DrawLine(p , x + w , y , x + w , y + h , thick);//Right
            DrawLine(p , x , y + h , x + w , y + h , thick);//Bottom
        }
        /// <summary>
        /// Draw a rectangle of the specified color
        /// </summary>
        /// <param name="p">The color</param>
        /// <param name="x">X position</param>
        /// <param name="y">Y position</param>
        /// <param name="w">Width</param>
        /// <param name="h">Height</param>
        /// <param name="thick">Line thickness</param>
        public virtual void DrawRectangle(Brush p , double x , double y , double w , double h , int thick)
        {
            DrawLine(p , x , y , x , y + h , thick); //Left
            DrawLine(p , x , y , x + w , y , thick); //Top
            DrawLine(p , x + w , y , x + w , y + h , thick);//Right
            DrawLine(p , x , y + h , x + w , y + h , thick);//Bottom
        }
        /// <summary>
        /// Draw a rectangle of the specified color
        /// </summary>
        /// <param name="p">The color</param>
        /// <param name="pos">Position</param>
        /// <param name="w">Width</param>
        /// <param name="h">Height</param>
        public virtual void DrawRectangle(Pixel p , PointF pos , int w , int h)
        {
            DrawRectangle(p , pos.X , pos.Y , w , h);
        }
        /// <summary>
        /// Draw a rectangle of the specified color
        /// </summary>
        /// <param name="p">The color</param>
        /// <param name="pos">Position</param>
        /// <param name="w">Width</param>
        /// <param name="h">Height</param>
        public virtual void DrawRectangle(Brush p , PointF pos , int w , int h)
        {
            DrawRectangle(p , pos.X , pos.Y , w , h);
        }
        /// <summary>
        /// Draw a rectangle of the specified color
        /// </summary>
        /// <param name="p">The color</param>
        /// <param name="r">The rectangle</param>
        public virtual void DrawRectangle(Pixel p , Rectangle r)
        {
            DrawRectangle(p , r.X , r.Y , r.Width , r.Height);
        }
        /// <summary>
        /// Draw a rectangle of the specified color
        /// </summary>
        /// <param name="p">The color</param>
        /// <param name="r">The rectangle</param>
        public virtual void DrawRectangle(Brush p , Rectangle r)
        {
            DrawRectangle(p , r.X , r.Y , r.Width , r.Height);
        }
        /// <summary>
        /// Draw a polygon outline
        /// </summary>
        /// <param name="p">The color</param>
        /// <param name="points">Points describing the polygon</param>
        public virtual void DrawPolygon(Pixel p , params PointF[ ] points)
        {
            for ( var i = 0; i < points.Length - 1; i++ )
                DrawLine(p , points[i] , points[i + 1]);
            DrawLine(p , points[0] , points[points.Length - 1]);
        }
        /// <summary>
        /// Draw a polygon outline
        /// </summary>
        /// <param name="p">The color</param>
        /// <param name="points">Points describing the polygon</param>
        public virtual void DrawPolygon(Brush p , params PointF[ ] points)
        {
            for ( var i = 0; i < points.Length - 1; i++ )
                DrawLine(p , points[i] , points[i + 1]);
            DrawLine(p , points[0] , points[points.Length - 1]);
        }
        /// <summary>
        /// Draw a polygon outline
        /// </summary>
        /// <param name="p">The color</param>
        /// <param name="thickness">Thickness of outline</param>
        /// <param name="points">Points describing the polygon</param>
        public virtual void DrawPolygon(Pixel p , int thickness , params PointF[ ] points)
        {
            if ( thickness == 0 )
                DrawPolygon(p , points);
            else
            {
                for ( var i = 0; i < points.Length - 1; i++ )
                    DrawLine(p , points[i] , points[i + 1] , thickness);
                DrawLine(p , points[0] , points[points.Length - 1] , thickness);
            }
        }
        /// <summary>
        /// Draw a polygon outline
        /// </summary>
        /// <param name="p">The color</param>
        /// <param name="thickness">Thickness of outline</param>
        /// <param name="points">Points describing the polygon</param>
        public virtual void DrawPolygon(Brush p , int thickness , params PointF[ ] points)
        {
            if ( thickness == 0 )
                DrawPolygon(p , points);
            else
            {
                for ( var i = 0; i < points.Length - 1; i++ )
                    DrawLine(p , points[i] , points[i + 1] , thickness);
                DrawLine(p , points[0] , points[points.Length - 1] , thickness);
            }
        }
        /// <summary>
        /// Draw a polygon outline
        /// </summary>
        /// <param name="p">The color</param>
        /// <param name="points">Points describing the polygon</param>
        public virtual void DrawPolygon(Pixel p , IEnumerable<PointF> points)
        {
            DrawPolygon(p , points.ToArray());
        }
        /// <summary>
        /// Draw a polygon outline
        /// </summary>
        /// <param name="p">The color</param>
        /// <param name="points">Points describing the polygon</param>
        public virtual void DrawPolygon(Brush p , IEnumerable<PointF> points)
        {
            DrawPolygon(p , points.ToArray());
        }
        /// <summary>
        /// Draw a polygon outline
        /// </summary>
        /// <param name="p">The color</param>
        /// <param name="thickness">Thickness of outline</param>
        /// <param name="points">Points describing the polygon</param>
        public virtual void DrawPolygon(Pixel p , int thickness , IEnumerable<PointF> points)
        {
            DrawPolygon(p , thickness , points.ToArray());
        }
        /// <summary>
        /// Draw a polygon outline
        /// </summary>
        /// <param name="p">The color</param>
        /// <param name="thickness">Thickness of outline</param>
        /// <param name="points">Points describing the polygon</param>
        public virtual void DrawPolygon(Brush p , int thickness , IEnumerable<PointF> points)
        {
            DrawPolygon(p , thickness , points.ToArray());
        }
        /// <summary>
        /// Fill the rectangle using the specified color
        /// </summary>
        /// <param name="p">The color</param>
        /// <param name="px">X position</param>
        /// <param name="py">Y position</param>
        /// <param name="pw">Width</param>
        /// <param name="ph">Height</param>
        public virtual void FillRectangle(Pixel p , double px , double py , double pw , double ph)
        {
            int x=(int)px,y=(int)py, w=(int)pw, h=(int)ph;
            FillPolygon(p , new PointF(x , y) , new PointF(x + w , y) , new PointF(x + w , y + h) , new PointF(x , y + h));
        }
        /// <summary>
        /// Fill the rectangle using the specified color
        /// </summary>
        /// <param name="p">The color</param>
        /// <param name="px">X position</param>
        /// <param name="py">Y position</param>
        /// <param name="pw">Width</param>
        /// <param name="ph">Height</param>
        public virtual void FillRectangle(Brush p , double px , double py , double pw , double ph)
        {
            int x=(int)px,y=(int)py, w=(int)pw, h=(int)ph;
            FillPolygon(p , new PointF(x , y) , new PointF(x + w , y) , new PointF(x + w , y + h) , new PointF(x , y + h));
        }
        /// <summary>
        /// Fill the specified polygon using pnpoly and bounds
        /// </summary>
        /// <param name="p">The color</param>
        /// <param name="points">Points describing the polygon</param>
        public virtual void FillPolygon(Pixel p , params PointF[ ] points)
        {
            DrawPolygon(p , points);
            var minX = points.Min((x)=>x.X);
            var minY = points.Min((x)=>x.Y);

            var maxX = points.Max((x)=>x.X);
            var maxY = points.Max((x)=>x.Y);

            double[] a=new double[points.Length],b=new double[points.Length];
            PrecalculatePnPolyValues(points , a , b);
            for ( var x = minX; x < maxX; x++ )
            {
                for ( var y = minY; y < maxY; y++ )
                {
                    if ( PnPolyUsingPrecalc(points , a , b , x , y) )
                        //if ( IsPointFInPolygon(points , x , y) )
                        SetPixel(p , x , y);
                }
            }
        }
        /// <summary>
        /// Fill the specified polygon using pnpoly and bounds
        /// </summary>
        /// <param name="p">The color</param>
        /// <param name="points">Points describing the polygon</param>
        public virtual void FillPolygon(Brush p , params PointF[ ] points)
        {
            DrawPolygon(p , points);
            var minX = points.Min((x)=>x.X);
            var minY = points.Min((x)=>x.Y);

            var maxX = points.Max((x)=>x.X);
            var maxY = points.Max((x)=>x.Y);

            double[] a=new double[points.Length],b=new double[points.Length];
            PrecalculatePnPolyValues(points , a , b);
            for ( var x = minX; x < maxX; x++ )
            {
                for ( var y = minY; y < maxY; y++ )
                {
                    if ( PnPolyUsingPrecalc(points , a , b , x , y) )
                        //if ( IsPointFInPolygon(points , x , y) )
                        SetPixel(p , x , y);
                }
            }
        }

        /// <summary>
        /// Used to accelerate PnPoly
        /// </summary>
        /// <param name="points">Points describing polygon</param>
        /// <param name="constant">used by pnpoly</param>
        /// <param name="multiple">used by pnpoly</param>
        private void PrecalculatePnPolyValues(PointF[ ] points , double[ ] constant , double[ ] multiple)
        {

            int   i, j=points.Length-1 ;

            for ( i = 0; i < points.Length; i++ )
            {
                if ( points[j].Y == points[i].Y )
                {
                    constant[i] = points[i].X;
                    multiple[i] = 0;
                }
                else
                {
                    constant[i] = points[i].X - ( points[i].Y * points[j].X ) / ( points[j].Y - points[i].Y ) + ( points[i].Y * points[i].X ) / ( points[j].Y - points[i].Y );
                    multiple[i] = ( points[j].X - points[i].X ) / ( points[j].Y - points[i].Y );
                }
                j = i;
            }
        }
        /// <summary>
        /// Identify if point is in polygon
        /// </summary>
        /// <param name="points">Points describing polygon</param>
        /// <param name="constant">used by pnpoly</param>
        /// <param name="multiple">used by pnpoly</param>
        /// <param name="x">X to test</param>
        /// <param name="y">Y to test</param>
        /// <returns></returns>
        private bool PnPolyUsingPrecalc(PointF[ ] points , double[ ] constant , double[ ] multiple , double x , double y)
        {

            int   i, j=points.Length-1 ;
            bool  oddNodes=false      ;

            for ( i = 0; i < points.Length; i++ )
            {
                if ( ( points[i].Y < y && points[j].Y >= y
                || points[j].Y < y && points[i].Y >= y ) )
                {
                    oddNodes ^= ( y * multiple[i] + constant[i] < x );
                }
                j = i;
            }

            return oddNodes;
        }

        /// <summary>
        /// pnpoly
        /// </summary>
        /// <param name="points"></param>
        /// <param name="testx"></param>
        /// <param name="testy"></param>
        /// <returns></returns>
        private bool IsPointInPolygon(PointF[ ] points , double testx , double testy)
        {
            int i, j;
            bool c=false;
            //Useless since we check it in outer fonction for bounds
            /*
            var minX = points.Min((x)=>x.X);
            var minY = points.Min((x)=>x.Y);

            var maxX = points.Max((x)=>x.X);
            var maxY = points.Max((x)=>x.Y);*/

            //if ( testx < minX || testx > maxX || testy < minY || testy > maxY )
            //    return false;
            for ( i = 0, j = points.Length - 1; i < points.Length; j = i++ )
            {
                if ( ( ( points[i].Y > testy ) != ( points[j].Y > testy ) ) &&
                  ( testx < ( points[j].X - points[i].X ) * ( testy - points[i].Y ) / ( points[j].Y - points[i].Y ) + points[j].X ) )

                    c = !c;
            }

            return c;
        }
        /// <summary>
        /// Draw image in bitmap
        /// </summary>
        /// <param name="source">Source</param>
        /// <param name="x">X origin</param>
        /// <param name="y">Y origin</param>
        public virtual void DrawImage(Bitmap source , int x , int y)
        {
            int sw=source.Width,sh=source.Height;
            for ( var i = 0; i < sw; i++ )
            {
                for ( var j = 0; j < sh; j++ )
                {
                    SetPixel(source[i , j] , x + i , y + j);
                }
            }
        }
        /// <summary>
        /// Draw image in bitmap
        /// </summary>
        /// <param name="source">Source</param>
        /// <param name="x">X origin</param>
        /// <param name="y">Y origin</param>
        public virtual void DrawImage(Bitmap source , Rectangle dest)
        {
            var tmp = source.Resize(dest.Width , dest.Height, ScalingMode.AverageInterpolate);
            DrawImage(tmp , dest.X , dest.Y);
            tmp.Dispose();
        }
        /// <summary>
        /// Draw image in bitmap
        /// </summary>
        /// <param name="source">Source</param>
        /// <param name="x">X origin</param>
        /// <param name="y">Y origin</param>
        public virtual void DrawImage(Bitmap source , Rectangle dest,Rectangle src)
        {
            var tmp = source.Crop(src);
            DrawImage(tmp , dest);
            tmp.Dispose();
        }
        /// <summary>
        /// Draw a line
        /// </summary>
        /// <param name="p">The color</param>
        /// <param name="x">X origin</param>
        /// <param name="y">Y origin</param>
        /// <param name="x2">X destination</param>
        /// <param name="y2">Y destination</param>
        public virtual void DrawLine(Pixel p , double x , double y , double x2 , double y2)
        {
            Matrix p1,p2;
            p1 = new Matrix(new double[ , ] { { x } , { y } , { 1 } });
            p2 = new Matrix(new double[ , ] { { x2 } , { y2 } , { 1 } });

            p1 = Transform(p1);
            p2 = Transform(p2);

            DrawLineInternal(p , p1 , p2);
        }
        /// <summary>
        /// Draw a line
        /// </summary>
        /// <param name="p">The color</param>
        /// <param name="x">X origin</param>
        /// <param name="y">Y origin</param>
        /// <param name="x2">X destination</param>
        /// <param name="y2">Y destination</param>
        public virtual void DrawLine(Brush p , double x , double y , double x2 , double y2)
        {
            Matrix p1,p2;
            p1 = new Matrix(new double[ , ] { { x } , { y } , { 1 } });
            p2 = new Matrix(new double[ , ] { { x2 } , { y2 } , { 1 } });

            p1 = Transform(p1);
            p2 = Transform(p2);

            DrawLineInternal(p , p1 , p2);
        }
        /// <summary>
        /// Draw a line
        /// </summary>
        /// <param name="p">The color</param>
        /// <param name="x">X origin</param>
        /// <param name="y">Y origin</param>
        /// <param name="x2">X destination</param>
        /// <param name="y2">Y destination</param>
        /// <param name="thickness">Thickness of line</param>
        public virtual void DrawLine(Pixel p , double x , double y , double x2 , double y2 , int thickness)
        {
            if ( thickness == 0 )
                DrawLine(p , x , y , x2 , y2);
            else
                DrawLineBeforeTransform(p , x , y , x2 , y2 , thickness);
        }
        /// <summary>
        /// Draw a line
        /// </summary>
        /// <param name="p">The color</param>
        /// <param name="x">X origin</param>
        /// <param name="y">Y origin</param>
        /// <param name="x2">X destination</param>
        /// <param name="y2">Y destination</param>
        /// <param name="thickness">Thickness of line</param>
        public virtual void DrawLine(Brush p , double x , double y , double x2 , double y2 , int thickness)
        {
            if ( thickness == 0 )
                DrawLine(p , x , y , x2 , y2);
            else
                DrawLineBeforeTransform(p , x , y , x2 , y2 , thickness);
        }
        /// <summary>
        /// Helper to draw line
        /// </summary>
        /// <param name="p">The color</param>
        /// <param name="x1">X origin</param>
        /// <param name="y1">Y origin</param>
        /// <param name="x2">X destination</param>
        /// <param name="y2">Y destination</param>
        /// <param name="thickness">Thickness of line</param>
        private void DrawLineBeforeTransform(Pixel p , double x1 , double y1 , double x2 , double y2 , int thickness)
        {

            // < line x1 = "0" y1 = "0" x2 = "200" y2 = "200" style = "stroke:rgb(255,0,0);stroke-width:2" />

            //bmp.instructions.AddLast(new Instruction("line" , x1 , y1 , x2 , y2 , p));

            double dx = (x2 - x1) ;
            double dy = (y2 - y1) ;
            double dydx = dy/dx;
            double dxdy = dx/dy;

            if ( dx < 0 )
                for ( var i = x2; i < x1; i++ )
                    FillCircle(p , ( int ) i , ( int ) ( y2 + dydx * ( i - x2 ) ) , thickness);
            else
                for ( var i = x1; i < x2; i++ )
                    FillCircle(p , ( int ) i , ( int ) ( y1 + dydx * ( i - x1 ) ) , thickness);

            if ( dy < 0 )
                for ( var i = y2; i < y1; i++ )
                    FillCircle(p , ( int ) ( x2 + dxdy * ( i - y2 ) ) , ( int ) i , thickness);
            else
                for ( var i = y1; i < y2; i++ )
                    FillCircle(p , ( int ) ( x1 + dxdy * ( i - y1 ) ) , ( int ) i , thickness);
        }
        /// <summary>
        /// Helper to draw line
        /// </summary>
        /// <param name="p">The color</param>
        /// <param name="x1">X origin</param>
        /// <param name="y1">Y origin</param>
        /// <param name="x2">X destination</param>
        /// <param name="y2">Y destination</param>
        /// <param name="thickness">Thickness of line</param>
        private void DrawLineBeforeTransform(Brush p , double x1 , double y1 , double x2 , double y2 , int thickness)
        {

            // < line x1 = "0" y1 = "0" x2 = "200" y2 = "200" style = "stroke:rgb(255,0,0);stroke-width:2" />

            //bmp.instructions.AddLast(new Instruction("line" , x1 , y1 , x2 , y2 , p));

            double dx = (x2 - x1) ;
            double dy = (y2 - y1) ;
            double dydx = dy/dx;
            double dxdy = dx/dy;

            if ( dx < 0 )
                for ( var i = x2; i < x1; i++ )
                    FillCircle(p , ( int ) i , ( int ) ( y2 + dydx * ( i - x2 ) ) , thickness);
            else
                for ( var i = x1; i < x2; i++ )
                    FillCircle(p , ( int ) i , ( int ) ( y1 + dydx * ( i - x1 ) ) , thickness);

            if ( dy < 0 )
                for ( var i = y2; i < y1; i++ )
                    FillCircle(p , ( int ) ( x2 + dxdy * ( i - y2 ) ) , ( int ) i , thickness);
            else
                for ( var i = y1; i < y2; i++ )
                    FillCircle(p , ( int ) ( x1 + dxdy * ( i - y1 ) ) , ( int ) i , thickness);
        }
        /// <summary>
        /// Draw line
        /// </summary>
        /// <param name="p">The color</param>
        /// <param name="p1">Origin</param>
        /// <param name="p2">Destination</param>
        public virtual void DrawLine(Pixel p , PointF p1 , PointF p2)
        {
            DrawLine(p , p1.X , p1.Y , p2.X , p2.Y);
        }
        /// <summary>
        /// Draw line
        /// </summary>
        /// <param name="p">The color</param>
        /// <param name="p1">Origin</param>
        /// <param name="p2">Destination</param>
        public virtual void DrawLine(Brush p , PointF p1 , PointF p2)
        {
            DrawLine(p , p1.X , p1.Y , p2.X , p2.Y);
        }
        /// <summary>
        /// Draw line
        /// </summary>
        /// <param name="p">The color</param>
        /// <param name="p1">Origin</param>
        /// <param name="p2">Destination</param>
        /// <param name="thickness">Thickness of line</param>
        public virtual void DrawLine(Pixel p , PointF p1 , PointF p2 , int thickness)
        {
            if ( thickness == 0 )
                DrawLine(p , p1.X , p1.Y , p2.X , p2.Y);
            else
                DrawLine(p , p1.X , p1.Y , p2.X , p2.Y , thickness);
        }
        /// <summary>
        /// Draw line
        /// </summary>
        /// <param name="p">The color</param>
        /// <param name="p1">Origin</param>
        /// <param name="p2">Destination</param>
        /// <param name="thickness">Thickness of line</param>
        public virtual void DrawLine(Brush p , PointF p1 , PointF p2 , int thickness)
        {
            if ( thickness == 0 )
                DrawLine(p , p1.X , p1.Y , p2.X , p2.Y);
            else
                DrawLine(p , p1.X , p1.Y , p2.X , p2.Y , thickness);
        }
        /// <summary>
        /// Clear the Bitmap using specified color
        /// </summary>
        /// <param name="p">The color</param>
        public virtual void Clear(Pixel p)
        {
            //bmp.instructions.Clear();
            Pixel* ptr = bmp.Source;
            for ( var i = 0; i < bmp.Width; i++ )
                for ( var j = 0; j < bmp.Height; j++ )
                    *ptr++ = p;
        }
        /// <summary>
        /// Clear the Bitmap using specified color
        /// </summary>
        /// <param name="p">The color</param>
        public virtual void Clear(Brush p)
        {
            //bmp.instructions.Clear();
            Pixel* ptr = bmp.Source;
            for ( var i = 0; i < bmp.Width; i++ )
                for ( var j = 0; j < bmp.Height; j++ )
                    *ptr++ = p.GetColor(i , j);
        }
        /// <summary>
        /// Helper to set pixel
        /// </summary>
        /// <param name="p">The color</param>
        /// <param name="px">X position</param>
        /// <param name="py">Y position</param>
        protected virtual void SetPixelInternal(Pixel p , double px , double py)
        {
            int x,y;
            x = ( int ) px;
            y = ( int ) py;

            if ( IsInClipRange(x , y) )
            {
                if ( p.A == 255 )
                    bmp[x , y] = p;
                else
                {
                    Pixel fromImage = bmp[x,y];
                    Pixel that = p;
                    bmp[x , y] = that.Over(fromImage);
                }
            }
        }

        private bool IsInClipRange(int x , int y)
        {
            if ( !( x >= clip.X && x < clip.X + clip.Width && y >= clip.Y && y < clip.Y + clip.Height ) )
                return false;
            if ( clipPolygon != null )
            {
                return ( PnPolyUsingPrecalc(clipPolygon , clipPolygonConstant , clipPolygonMultiple , x , y) );
            }
            return true;
        }

        /// <summary>
        /// Helper to set pixel
        /// </summary>
        /// <param name="p">The color</param>
        /// <param name="px">X position</param>
        /// <param name="py">Y position</param>
        private void SetPixelInternal(Brush b , double px , double py)
        {
            int x,y;
            x = ( int ) px;
            y = ( int ) py;

            if ( x >= clip.X && x < clip.X + clip.Width )
                if ( y >= clip.Y && y < clip.Y + clip.Height )
                {
                    Pixel p = b.GetColor(x,y);
                    if ( p.A == 255 )
                        bmp[x , y] = p;
                    else
                    {
                        Pixel fromImage = bmp[x,y];
                        Pixel that = p;
                        bmp[x , y] = that.Over(fromImage);
                    }
                }
        }
        /// <summary>
        /// Helper to draw line
        /// </summary>
        /// <param name="p">The color</param>
        /// <param name="p1">Origin</param>
        /// <param name="p2">Destination</param>
        private void DrawLineInternal(Pixel p , Matrix p1 , Matrix p2)
        {

            // < line x1 = "0" y1 = "0" x2 = "200" y2 = "200" style = "stroke:rgb(255,0,0);stroke-width:2" />
            int x1 = (int)p1[0,0];
            int x2 = (int)p2[0,0];
            int y1 = (int)p1[1,0];
            int y2 = (int)p2[1,0];

            if ( LineMode == LineMode.ForLoop )
            {
                //bmp.instructions.AddLast(new Instruction("line" , x1 , y1 , x2 , y2 , p));

                double dx = (x2 - x1) ;
                double dy = (y2 - y1) ;
                double dydx = dy/dx;
                double dxdy = dx/dy;

                if ( dx < 0 )
                    for ( var i = x2; i < x1; i++ )
                        SetPixelInternal(p , i , y2 + dydx * ( i - x2 ));
                else
                    for ( var i = x1; i < x2; i++ )
                        SetPixelInternal(p , i , y1 + dydx * ( i - x1 ));

                if ( dy < 0 )
                    for ( var i = y2; i < y1; i++ )
                        SetPixelInternal(p , x2 + dxdy * ( i - y2 ) , i);
                else
                    for ( var i = y1; i < y2; i++ )
                        SetPixelInternal(p , x1 + dxdy * ( i - y1 ) , i);
            }
            else if ( LineMode == LineMode.FourConnex )
            {
                SetPixelInternal(p , x1 , y1);
                while ( y1 != y2 || x2 != x1 )
                {
                    var options = new List<Point>(4);
                    options.Add(new Point(x1 - 1 , y1));
                    options.Add(new Point(x1 + 1 , y1));
                    options.Add(new Point(x1 , y1 - 1));
                    options.Add(new Point(x1 , y1 + 1));
                    var pt = FindBestOption(x2,y2, options);
                    x1 = pt.X;
                    y1 = pt.Y;
                    SetPixelInternal(p , x1 , y1);
                }
            }
            else if ( LineMode == LineMode.EightConnex )
            {
                SetPixelInternal(p , x1 , y1);
                while ( y1 != y2 || x2 != x1 )
                {
                    var options = new List<Point>(8);
                    options.Add(new Point(x1 - 1 , y1));
                    options.Add(new Point(x1 + 1 , y1));
                    options.Add(new Point(x1 , y1 - 1));
                    options.Add(new Point(x1 , y1 + 1));

                    options.Add(new Point(x1 - 1 , y1 - 1));
                    options.Add(new Point(x1 - 1 , y1 + 1));
                    options.Add(new Point(x1 + 1 , y1 - 1));
                    options.Add(new Point(x1 + 1 , y1 + 1));
                    var pt = FindBestOption(x2,y2, options);
                    x1 = pt.X;
                    y1 = pt.Y;
                    SetPixelInternal(p , x1 , y1);
                }
            }
        }

        private static Point FindBestOption(int x2 , int y2 , List<Point> options)
        {
            long lowestValue = long.MaxValue;
            int lowestIndex = 0;
            for ( var i = 0; i < options.Count; i++ )
            {
                long dx=options[i].X-x2,dy=options[i].Y-y2;
                long value = dx*dx+dy*dy;
                if ( value <= lowestValue )
                {
                    lowestIndex = i;
                    lowestValue = value;
                }
            }
            return options[lowestIndex];
        }

        /// <summary>
        /// Helper to draw line
        /// </summary>
        /// <param name="p">The color</param>
        /// <param name="p1">Origin</param>
        /// <param name="p2">Destination</param>
        private void DrawLineInternal(Brush p , Matrix p1 , Matrix p2)
        {

            // < line x1 = "0" y1 = "0" x2 = "200" y2 = "200" style = "stroke:rgb(255,0,0);stroke-width:2" />
            int x1 = (int)p1[0,0];
            int x2 = (int)p2[0,0];
            int y1 = (int)p1[1,0];
            int y2 = (int)p2[1,0];

            //bmp.instructions.AddLast(new Instruction("line" , x1 , y1 , x2 , y2 , p));

            double dx = (x2 - x1) ;
            double dy = (y2 - y1) ;
            double dydx = dy/dx;
            double dxdy = dx/dy;

            if ( dx < 0 )
                for ( var i = x2; i < x1; i++ )
                    SetPixelInternal(p , i , y2 + dydx * ( i - x2 ));
            else
                for ( var i = x1; i < x2; i++ )
                    SetPixelInternal(p , i , y1 + dydx * ( i - x1 ));

            if ( dy < 0 )
                for ( var i = y2; i < y1; i++ )
                    SetPixelInternal(p , x2 + dxdy * ( i - y2 ) , i);
            else
                for ( var i = y1; i < y2; i++ )
                    SetPixelInternal(p , x1 + dxdy * ( i - y1 ) , i);
        }
        /// <summary>
        /// Draw circle outline
        /// </summary>
        /// <param name="p">The color</param>
        /// <param name="x0">X center</param>
        /// <param name="y0">Y center</param>
        /// <param name="r">Radius</param>
        public virtual void DrawCircle(Pixel p , int x0 , int y0 , double r)
        {
            Matrix px = Transform(x0,y0);

            //bmp.instructions.AddLast(new Instruction("circle" , px[0 , 0] , px[1 , 0] , r , 1 , p));

            var poly = GetCirclePolygon(x0 , y0 , r);

            DrawPolygon(p , poly);
        }
        /// <summary>
        /// Draw circle outline
        /// </summary>
        /// <param name="p">The color</param>
        /// <param name="x0">X center</param>
        /// <param name="y0">Y center</param>
        /// <param name="r">Radius</param>
        public virtual void DrawCircle(Brush p , int x0 , int y0 , double r)
        {
            Matrix px = Transform(x0,y0);

            //bmp.instructions.AddLast(new Instruction("circle" , px[0 , 0] , px[1 , 0] , r , 1 , p));

            var poly = GetCirclePolygon(x0 , y0 , r);

            DrawPolygon(p , poly);
        }
        /// <summary>
        /// Draw circle outline
        /// </summary>
        /// <param name="p">The color</param>
        /// <param name="x0">X center</param>
        /// <param name="y0">Y center</param>
        /// <param name="r">Radius</param>
        /// <param name="thickness">Thickness of outline</param>
        public virtual void DrawCircle(Pixel p , int x0 , int y0 , double r , int thickness)
        {
            if ( thickness == 0 )
            {
                DrawCircle(p , x0 , y0 , r);
                return;
            }

            var poly = GetCirclePolygon(x0 , y0 , r);

            DrawPolygon(p , thickness , poly);

        }
        /// <summary>
        /// Draw circle outline
        /// </summary>
        /// <param name="p">The color</param>
        /// <param name="x0">X center</param>
        /// <param name="y0">Y center</param>
        /// <param name="r">Radius</param>
        /// <param name="thickness">Thickness of outline</param>
        public virtual void DrawCircle(Brush p , int x0 , int y0 , double r , int thickness)
        {
            if ( thickness == 0 )
            {
                DrawCircle(p , x0 , y0 , r);
                return;
            }

            var poly = GetCirclePolygon(x0 , y0 , r);

            DrawPolygon(p , thickness , poly);

        }
        /// <summary>
        /// Fill circle
        /// </summary>
        /// <param name="p">The color</param>
        /// <param name="x0">X center</param>
        /// <param name="y0">Y center</param>
        /// <param name="r">Radius</param>
        public virtual void FillCircle(Pixel p , int x0 , int y0 , double r)
        {
            Matrix px = Transform(x0,y0);

            //bmp.instructions.AddLast(new Instruction("circleF" , px[0 , 0] , px[1 , 0] , r , p));

            var poly = GetCirclePolygon(x0 , y0 , r);

            FillPolygon(p , poly);
        }
        /// <summary>
        /// Fill circle
        /// </summary>
        /// <param name="p">The color</param>
        /// <param name="x0">X center</param>
        /// <param name="y0">Y center</param>
        /// <param name="r">Radius</param>
        public virtual void FillCircle(Brush p , int x0 , int y0 , double r)
        {
            Matrix px = Transform(x0,y0);

            //bmp.instructions.AddLast(new Instruction("circleF" , px[0 , 0] , px[1 , 0] , r , p));

            var poly = GetCirclePolygon(x0 , y0 , r);

            FillPolygon(p , poly);
        }
        /// <summary>
        /// Fill Ellipse(circle)
        /// </summary>
        /// <param name="p">The color</param>
        /// <param name="x">X center</param>
        /// <param name="y">Y center</param>
        /// <param name="w">Width</param>
        /// <param name="h">Height</param>
        public virtual void FillEllipse(Pixel p , int x , int y , int w , int h)
        {
            var poly = getEllipse(x , y , w,h);
            FillPolygon(p , poly);
        }
        /// <summary>
        /// Fill Ellipse(circle)
        /// </summary>
        /// <param name="p">The color</param>
        /// <param name="x">X center</param>
        /// <param name="y">Y center</param>
        /// <param name="w">Width</param>
        /// <param name="h">Height</param>
        public virtual void FillEllipse(Brush p , int x , int y , int w , int h)
        {
            var poly = getEllipse(x , y , w,h);
            FillPolygon(p , poly);
        }
        /// <summary>
        /// Draw Ellipse(circle)
        /// </summary>
        /// <param name="p">The color</param>
        /// <param name="x">X center</param>
        /// <param name="y">Y center</param>
        /// <param name="w">Width</param>
        /// <param name="h">Height</param>
        public virtual void DrawEllipse(Pixel p , int x , int y , int w , int h)
        {
            var poly = getEllipse(x , y , w,h);
            DrawPolygon(p , poly);
        }
        /// <summary>
        /// Draw Ellipse(circle)
        /// </summary>
        /// <param name="p">The color</param>
        /// <param name="x">X center</param>
        /// <param name="y">Y center</param>
        /// <param name="w">Width</param>
        /// <param name="h">Height</param>
        public virtual void DrawEllipse(Brush p , int x , int y , int w , int h)
        {
            var poly = getEllipse(x , y , w,h);
            DrawPolygon(p , poly);
        }
        /// 
        /// <summary>
        /// Draw Ellipse(circle)
        /// </summary>
        /// <param name="p">The color</param>
        /// <param name="x">X center</param>
        /// <param name="y">Y center</param>
        /// <param name="w">Width</param>
        /// <param name="h">Height</param>
        /// <param name="t">Thickness of outline</param>
        public virtual void DrawEllipse(Pixel p , int x , int y , int w , int h , int t)
        {
            if ( t == 0 )
                DrawEllipse(p , x , y , w , h);
            else
            {
                var poly = getEllipse(x , y , w,h);
                DrawPolygon(p , t , poly);
            }
        }
        /// 
        /// <summary>
        /// Draw Ellipse(circle)
        /// </summary>
        /// <param name="p">The color</param>
        /// <param name="x">X center</param>
        /// <param name="y">Y center</param>
        /// <param name="w">Width</param>
        /// <param name="h">Height</param>
        /// <param name="t">Thickness of outline</param>
        public virtual void DrawEllipse(Brush p , int x , int y , int w , int h , int t)
        {
            if ( t == 0 )
                DrawEllipse(p , x , y , w , h);
            else
            {
                var poly = getEllipse(x , y , w,h);
                DrawPolygon(p , t , poly);
            }
        }
        /// <summary>
        /// Draw pie
        /// </summary>
        /// <param name="p">The color</param>
        /// <param name="x0">X center of circle</param>
        /// <param name="y0">Y center of circle</param>
        /// <param name="r">Radius</param>
        /// <param name="spanAngle">Span of angle</param>
        /// <param name="startAngle">Start angle</param>
        /// <returns>Polygon describing the pie</returns>
        public virtual IEnumerable<PointF> DrawPie(Pixel p , int x0 , int y0 , double r , double spanAngle , double startAngle)
        {
            PointF[ ] poly2 = GetPiePolygon(x0 , y0 , r ,r, spanAngle , startAngle);

            DrawPolygon(p , poly2);

            return poly2;
        }
        /// <summary>
        /// Draw pie
        /// </summary>
        /// <param name="p">The color</param>
        /// <param name="x0">X center of circle</param>
        /// <param name="y0">Y center of circle</param>
        /// <param name="r">Radius</param>
        /// <param name="spanAngle">Span of angle</param>
        /// <param name="startAngle">Start angle</param>
        /// <returns>Polygon describing the pie</returns>
        public virtual IEnumerable<PointF> DrawPie(Brush p , int x0 , int y0 , double r , double spanAngle , double startAngle)
        {
            PointF[ ] poly2 = GetPiePolygon(x0 , y0 , r ,r, spanAngle , startAngle);

            DrawPolygon(p , poly2);

            return poly2;
        }
        /// <summary>
        /// Draw pie
        /// </summary>
        /// <param name="p">The color</param>
        /// <param name="x0">X center of circle</param>
        /// <param name="y0">Y center of circle</param>
        /// <param name="w">Width</param>
        /// <param name="h">Height</param>
        /// <param name="spanAngle">Span of angle</param>
        /// <param name="startAngle">Start angle</param>
        /// <returns>Polygon describing the pie</returns>
        public virtual IEnumerable<PointF> DrawPie(Pixel p , int x0 , int y0 , double w , double h , double spanAngle , double startAngle)
        {
            PointF[ ] poly2 = GetPiePolygon(x0 , y0 , w,h , spanAngle , startAngle);

            DrawPolygon(p , poly2);

            return poly2;
        }
        /// <summary>
        /// Draw pie
        /// </summary>
        /// <param name="p">The color</param>
        /// <param name="x0">X center of circle</param>
        /// <param name="y0">Y center of circle</param>
        /// <param name="w">Width</param>
        /// <param name="h">Height</param>
        /// <param name="spanAngle">Span of angle</param>
        /// <param name="startAngle">Start angle</param>
        /// <returns>Polygon describing the pie</returns>
        public virtual IEnumerable<PointF> DrawPie(Brush p , int x0 , int y0 , double w , double h , double spanAngle , double startAngle)
        {
            PointF[ ] poly2 = GetPiePolygon(x0 , y0 , w,h , spanAngle , startAngle);

            DrawPolygon(p , poly2);

            return poly2;
        }
        /// <summary>
        /// Draw pie
        /// </summary>
        /// <param name="p">The color</param>
        /// <param name="x0">X center of circle</param>
        /// <param name="y0">Y center of circle</param>
        /// <param name="r">Radius</param>
        /// <param name="spanAngle">Span of angle</param>
        /// <param name="startAngle">Start angle</param>
        /// <param name="thickness">Thickness of outline</param>
        /// <returns>Polygon describing the pie</returns>
        public virtual IEnumerable<PointF> DrawPie(Pixel p , int x0 , int y0 , double r , double spanAngle , double startAngle , int thickness)
        {
            if ( thickness == 0 )
                return DrawPie(p , x0 , y0 , r , spanAngle , startAngle);
            else
            {
                PointF[ ] poly2 = GetPiePolygon(x0 , y0 , r,r , spanAngle , startAngle);
                DrawPolygon(p , thickness , poly2);
                return poly2;
            }
        }
        /// <summary>
        /// Draw pie
        /// </summary>
        /// <param name="p">The color</param>
        /// <param name="x0">X center of circle</param>
        /// <param name="y0">Y center of circle</param>
        /// <param name="r">Radius</param>
        /// <param name="spanAngle">Span of angle</param>
        /// <param name="startAngle">Start angle</param>
        /// <param name="thickness">Thickness of outline</param>
        /// <returns>Polygon describing the pie</returns>
        public virtual IEnumerable<PointF> DrawPie(Brush p , int x0 , int y0 , double r , double spanAngle , double startAngle , int thickness)
        {
            if ( thickness == 0 )
                return DrawPie(p , x0 , y0 , r , spanAngle , startAngle);
            else
            {
                PointF[ ] poly2 = GetPiePolygon(x0 , y0 , r,r , spanAngle , startAngle);
                DrawPolygon(p , thickness , poly2);
                return poly2;
            }
        }
        /// <summary>
        /// Draw pie
        /// </summary>
        /// <param name="p">The color</param>
        /// <param name="x0">X center of circle</param>
        /// <param name="y0">Y center of circle</param>
        /// <param name="w">Width</param>
        /// <param name="h">Height</param>
        /// <param name="spanAngle">Span of angle</param>
        /// <param name="startAngle">Start angle</param>
        /// <param name="thickness">Thickness of outline</param>
        /// <returns>Polygon describing the pie</returns>
        public virtual IEnumerable<PointF> DrawPie(Pixel p , int x0 , int y0 , double w , double h , double spanAngle , double startAngle , int thickness)
        {
            if ( thickness == 0 )
                return DrawPie(p , x0 , y0 , w , spanAngle , startAngle);
            else
            {
                PointF[ ] poly2 = GetPiePolygon(x0 , y0 , w,h , spanAngle , startAngle);
                DrawPolygon(p , thickness , poly2);
                return poly2;
            }
        }
        /// <summary>
        /// Draw pie
        /// </summary>
        /// <param name="p">The color</param>
        /// <param name="x0">X center of circle</param>
        /// <param name="y0">Y center of circle</param>
        /// <param name="w">Width</param>
        /// <param name="h">Height</param>
        /// <param name="spanAngle">Span of angle</param>
        /// <param name="startAngle">Start angle</param>
        /// <param name="thickness">Thickness of outline</param>
        /// <returns>Polygon describing the pie</returns>
        public virtual IEnumerable<PointF> DrawPie(Brush p , int x0 , int y0 , double w , double h , double spanAngle , double startAngle , int thickness)
        {
            if ( thickness == 0 )
                return DrawPie(p , x0 , y0 , w , spanAngle , startAngle);
            else
            {
                PointF[ ] poly2 = GetPiePolygon(x0 , y0 , w,h , spanAngle , startAngle);
                DrawPolygon(p , thickness , poly2);
                return poly2;
            }
        }
        /// <summary>
        /// Fill pie
        /// </summary>
        /// <param name="p">The color</param>
        /// <param name="x0">X center of circle</param>
        /// <param name="y0">Y center of circle</param>
        /// <param name="r">Radius</param>
        /// <param name="spanAngle">Span of angle</param>
        /// <param name="startAngle">Start angle</param>
        public virtual void FillPie(Pixel p , int x0 , int y0 , double r , double spanAngle , double startAngle)
        {
            PointF[ ] poly2 = GetPiePolygon(x0 , y0 , r,r , spanAngle , startAngle);

            FillPolygon(p , poly2);
        }
        /// <summary>
        /// Fill pie
        /// </summary>
        /// <param name="p">The color</param>
        /// <param name="x0">X center of circle</param>
        /// <param name="y0">Y center of circle</param>
        /// <param name="r">Radius</param>
        /// <param name="spanAngle">Span of angle</param>
        /// <param name="startAngle">Start angle</param>
        public virtual void FillPie(Brush p , int x0 , int y0 , double r , double spanAngle , double startAngle)
        {
            PointF[ ] poly2 = GetPiePolygon(x0 , y0 , r,r , spanAngle , startAngle);

            FillPolygon(p , poly2);
        }
        /// <summary>
        /// Fill pie
        /// </summary>
        /// <param name="p">The color</param>
        /// <param name="x0">X center of circle</param>
        /// <param name="y0">Y center of circle</param>
        /// <param name="w">Width</param>
        /// <param name="h">Height</param>
        /// <param name="spanAngle">Span of angle</param>
        /// <param name="startAngle">Start angle</param>
        public virtual void FillPie(Pixel p , int x0 , int y0 , double w , double h , double spanAngle , double startAngle)
        {
            PointF[ ] poly2 = GetPiePolygon(x0 , y0 , w,h , spanAngle , startAngle);

            FillPolygon(p , poly2);
        }
        /// <summary>
        /// Fill pie
        /// </summary>
        /// <param name="p">The color</param>
        /// <param name="x0">X center of circle</param>
        /// <param name="y0">Y center of circle</param>
        /// <param name="w">Width</param>
        /// <param name="h">Height</param>
        /// <param name="spanAngle">Span of angle</param>
        /// <param name="startAngle">Start angle</param>
        public virtual void FillPie(Brush p , int x0 , int y0 , double w , double h , double spanAngle , double startAngle)
        {
            PointF[ ] poly2 = GetPiePolygon(x0 , y0 , w,h , spanAngle , startAngle);

            FillPolygon(p , poly2);
        }
        /// <summary>
        /// Helper to get the polygin describing a Pie
        /// </summary>
        /// <param name="x0">X center of circle</param>
        /// <param name="y0">Y center of circle</param>
        /// <param name="w">Width</param>
        /// <param name="h">Height</param>
        /// <param name="spanAngle">Span of angle</param>
        /// <param name="startAngle">Start angle</param>
        /// <returns></returns>
        private PointF[ ] GetPiePolygon(int x0 , int y0 , double w , double h , double spanAngle , double startAngle)
        {
            var poly = getEllipse(x0-(int)w/2 , y0-(int)h/2 , (int)w,(int)h);
            var count = poly.Length;

            int start = (int)System.Math.Ceiling(startAngle/2/System.Math.PI*count);
            int take = (int)System.Math.Ceiling(spanAngle/2/System.Math.PI*count+1);
            var center= new PointF[ ] { new PointF(x0 , y0) };
            var poly2 = center.Concat(Reduce(new CircularArray<PointF>(poly).Skip(start).Take(take),w)).Concat(center).ToArray();

            return poly2;
        }
        /// <summary>
        /// Helper to get circle
        /// </summary>
        /// <param name="x0">X center</param>
        /// <param name="y0">Y center</param>
        /// <param name="r">Radius</param>
        /// <returns></returns>
        public static PointF[ ] GetCirclePolygon(int x0 , int y0 , double r)
        {
            LinkedList<PointF> points = new LinkedList<PointF>();

            double f = 1 - r;
            double ddF_x = 1;
            double ddF_y = -2 * r;
            double x = 0;
            double y = r;

            points.AddLast(new PointF(x0 , y0 + r));
            points.AddLast(new PointF(x0 , y0 - r));
            points.AddLast(new PointF(x0 + r , y0));
            points.AddLast(new PointF(x0 - r , y0));

            while ( x < y )
            {
                if ( f >= 0 )
                {
                    y--;
                    ddF_y += 2;
                    f += ddF_y;
                }
                x++;
                ddF_x += 2;
                f += ddF_x;

                points.AddLast(new PointF(x0 + x , y0 + y));
                points.AddLast(new PointF(x0 - x , y0 + y));
                points.AddLast(new PointF(x0 + x , y0 - y));
                points.AddLast(new PointF(x0 - x , y0 - y));
                points.AddLast(new PointF(x0 + y , y0 + x));
                points.AddLast(new PointF(x0 - y , y0 + x));
                points.AddLast(new PointF(x0 + y , y0 - x));
                points.AddLast(new PointF(x0 - y , y0 - x));
            }

            var part1 = (points.Where((p)=>p.Y>y0).OrderBy((p) => p.X));
            var part2 = (points.Where((p)=>p.Y<=y0).OrderBy((p) => -p.X));
            var ret = part2.Concat(part1).ToArray();


            return ret;
        }
        private PointF[ ] getEllipse(int x , int y , int w , int h)
        {
            var min=System.Math.Min(w , h);
            var points =GetCirclePolygon(w/2 , h/2 , min);
            for ( var i = 0; i < points.Length; i++ )
            {
                points[i].X = points[i].X * w / min;
                points[i].Y = points[i].Y * h / min;
                points[i].X += x;
                points[i].Y += y;
            }
            return points;
        }
        /// <summary>
        /// Reduce the number of item from an array using radius
        /// </summary>
        /// <typeparam name="T">Type</typeparam>
        /// <param name="input">input array</param>
        /// <param name="r">Radius</param>
        /// <returns>Reduced array</returns>
        private IEnumerable<T> Reduce<T>(IEnumerable<T> input , double r)
        {
            int i=0;
            r = ( int ) r / 8;
            int len = input.Count();

            var enumerator = input.GetEnumerator();
            enumerator.MoveNext();
            yield return enumerator.Current;
            i = 1;
            while ( enumerator.MoveNext() )
            {
                var p = enumerator.Current;
                i++;
                if ( i == len || i % r == 0 )
                    yield return p;
            }
        }
        /// <summary>
        /// Set the pixel with specified color
        /// </summary>
        /// <param name="p">The color</param>
        /// <param name="x">X position</param>
        /// <param name="y">Y position</param>
        public virtual void SetPixel(Pixel p , double x , double y)
        {
            var p1 = new Matrix(new double[ , ] { { x } , { y }, { 1 } });
            p1 = Transform(p1);

            SetPixelInternal(p , p1[0 , 0] , p1[1 , 0]);
        }
        /// <summary>
        /// Set the pixel with specified color
        /// </summary>
        /// <param name="p">The color</param>
        /// <param name="x">X position</param>
        /// <param name="y">Y position</param>
        public virtual void SetPixel(Brush p , double x , double y)
        {
            var p1 = new Matrix(new double[ , ] { { x } , { y }, { 1 } });
            p1 = Transform(p1);

            SetPixelInternal(p.GetColor(( int ) x , ( int ) y) , p1[0 , 0] , p1[1 , 0]);
        }
        /// <summary>
        /// Used to transform point using transforamtion matrix
        /// </summary>
        /// <param name="p1">Point</param>
        /// <returns>Transformed point</returns>
        private Matrix Transform(Matrix p1)
        {
            return transformationMatrix * p1;
        }
        /// <summary>
        /// Used to transform point using transforamtion matrix
        /// </summary>
        /// <param name="x">X position</param>
        /// <param name="y">Y position</param>
        /// <returns>Transformed point</returns>
        private Matrix Transform(int x , int y)
        {
            Matrix p1 = new Matrix(new double[,] { {x }, {y }, {0 } });
            return transformationMatrix * p1;
        }
        /// <summary>
        /// Write specifie text in bitmap
        /// </summary>
        /// <param name="str">Text to write</param>
        /// <param name="p">The color</param>
        /// <param name="x">X origin</param>
        /// <param name="y">Y origin</param>
        /// <param name="f">The Font</param>
        /// <param name="size">The Font Size</param>
        /// <param name="sf">[optional]String format(top left if missing)</param>
        public virtual void DrawString(string str , Pixel p , int ox , int oy , Font f , int size , StringFormat sf = null)
        {
            DrawString(str , p , ox , oy , f , ( float ) size , sf);
        }
        /// <summary>
        /// Write specifie text in bitmap
        /// </summary>
        /// <param name="str">Text to write</param>
        /// <param name="p">The color</param>
        /// <param name="x">X origin</param>
        /// <param name="y">Y origin</param>
        /// <param name="f">The Font</param>
        /// <param name="size">The Font Size</param>
        /// <param name="sf">[optional]String format(top left if missing)</param>

        public virtual void DrawString(string str , Pixel p , int ox , int oy , Font f , float size , StringFormat sf = null)
        {
            float x=ox,y=oy;
            if ( sf == null || sf.Alignment == StringAlignment.Near && sf.LineAlignment == StringAlignment.Near )
            {
                //bmp.instructions.AddLast(new Instruction("text" , str , x , y , p , size));
                int mh = 0;

                foreach ( char c in str )
                {
                    bool[,] character = f.GetChar(c);
                    mh = System.Math.Max(mh , character.GetLength(0));
                    if ( c == 10 )
                    {
                        x = ox;
                        y += mh * size + 1;
                        mh = 0;
                    }
                    else
                        x += DrawFormattedCharInternalF(character , x , y , new Text.FormattedChar() { Color = ( Brush ) p , Size = size });
                }
            }
            else
            {
                var measure = MeasureString(str,f,size);

                if ( sf.Alignment == StringAlignment.Center )
                    x -= measure.Width / 2;
                else if ( sf.Alignment == StringAlignment.Far )
                    x -= measure.Width;

                if ( sf.LineAlignment == StringAlignment.Center )
                    y -= measure.Height / 2;
                else if ( sf.LineAlignment == StringAlignment.Far )
                    y -= measure.Height;

                DrawString(str , p , ( int ) x , ( int ) y , f , size);
            }
        }
        /// <summary>
        /// Write specifie text in bitmap
        /// </summary>
        /// <param name="str">Text to write</param>
        /// <param name="p">The color</param>
        /// <param name="x">X origin</param>
        /// <param name="y">Y origin</param>
        /// <param name="f">The Font</param>
        /// <param name="size">The Font Size</param>
        /// <param name="sf">[optional]String format(top left if missing)</param>
        public virtual void DrawString(string str , Brush p , int ox , int oy , Font f , int size , StringFormat sf = null)
        {
            DrawString(str , p , ox , oy , f , ( float ) size , sf);
        }
        /// <summary>
        /// Write specifie text in bitmap
        /// </summary>
        /// <param name="str">Text to write</param>
        /// <param name="p">The color</param>
        /// <param name="x">X origin</param>
        /// <param name="y">Y origin</param>
        /// <param name="f">The Font</param>
        /// <param name="size">The Font Size</param>
        /// <param name="sf">[optional]String format(top left if missing)</param>
        public virtual void DrawString(string str , Brush p , int ox , int oy , Font f , float size , StringFormat sf = null)
        {
            float x=ox,y=oy;
            if ( sf == null || sf.Alignment == StringAlignment.Near && sf.LineAlignment == StringAlignment.Near )
            {
                //bmp.instructions.AddLast(new Instruction("text" , str , x , y , p , size));
                int mh = 0;

                foreach ( char c in str )
                {
                    bool[,] character = f.GetChar(c);
                    mh = System.Math.Max(mh , character.GetLength(0));
                    if ( c == 10 )
                    {
                        x = ox;
                        y += mh * size + 1;
                        mh = 0;
                    }
                    else
                        x += DrawFormattedCharInternalF(character , x , y , new Text.FormattedChar() { Color = ( Brush ) p , Size = size });
                }
            }
            else
            {
                var measure = MeasureString(str,f,size);

                if ( sf.Alignment == StringAlignment.Center )
                    x -= measure.Width / 2;
                else if ( sf.Alignment == StringAlignment.Far )
                    x -= measure.Width;

                if ( sf.LineAlignment == StringAlignment.Center )
                    y -= measure.Height / 2;
                else if ( sf.LineAlignment == StringAlignment.Far )
                    y -= measure.Height;

                DrawString(str , p , ( int ) x , ( int ) y , f , size);
            }
        }
        /// <summary>
        /// Write specifie text in bitmap
        /// </summary>
        /// <param name="str">Text to write</param>
        /// <param name="fs">Font and size</param>
        /// <param name="p">The color</param>
        /// <param name="x">X origin</param>
        /// <param name="y">Y origin</param>
        public virtual void DrawString(string str , FontSize fs , Pixel p , int x , int y)
        {
            Font f = fs.Font;
            var size=fs.Size;
            DrawString(str , p , x , y , f , size);
        }
        /// <summary>
        /// Write specifie text in bitmap
        /// </summary>
        /// <param name="str">Text to write</param>
        /// <param name="fs">Font and size</param>
        /// <param name="p">The color</param>
        /// <param name="x">X origin</param>
        /// <param name="y">Y origin</param>
        public virtual void DrawString(string str , FontSizeF fs , Pixel p , int x , int y)
        {
            Font f = fs.Font;
            var size=fs.Size;
            DrawString(str , p , x , y , f , size);
        }
        /// <summary>
        /// Write specifie text in bitmap
        /// </summary>
        /// <param name="str">Text to write</param>
        /// <param name="fs">Font and size</param>
        /// <param name="p">The color</param>
        /// <param name="x">X origin</param>
        /// <param name="y">Y origin</param>
        public virtual void DrawString(string str , FontSize fs , Brush p , int x , int y)
        {
            Font f = fs.Font;
            var size=fs.Size;
            DrawString(str , p , x , y , f , size);
        }
        /// <summary>
        /// Write specifie text in bitmap
        /// </summary>
        /// <param name="str">Text to write</param>
        /// <param name="fs">Font and size</param>
        /// <param name="p">The color</param>
        /// <param name="x">X origin</param>
        /// <param name="y">Y origin</param>
        public virtual void DrawString(string str , FontSizeF fs , Brush p , int x , int y)
        {
            Font f = fs.Font;
            var size=fs.Size;
            DrawString(str , p , x , y , f , size);
        }
        /// <summary>
        /// Write specifie text in bitmap
        /// </summary>
        /// <param name="str">Text to write</param>
        /// <param name="fs">Font and size</param>
        /// <param name="p">The color</param>
        /// <param name="pt">Origin</param>
        public virtual void DrawString(string str , FontSize fs , Pixel p , PointF pt)
        {
            Font f = fs.Font;
            var size=fs.Size;
            DrawString(str , p , ( int ) pt.X , ( int ) pt.Y , f , size);
        }
        /// <summary>
        /// Write specifie text in bitmap
        /// </summary>
        /// <param name="str">Text to write</param>
        /// <param name="fs">Font and size</param>
        /// <param name="p">The color</param>
        /// <param name="pt">Origin</param>
        public virtual void DrawString(string str , FontSizeF fs , Pixel p , PointF pt)
        {
            Font f = fs.Font;
            var size=fs.Size;
            DrawString(str , p , ( int ) pt.X , ( int ) pt.Y , f , size);
        }
        /// <summary>
        /// Write specifie text in bitmap
        /// </summary>
        /// <param name="str">Text to write</param>
        /// <param name="fs">Font and size</param>
        /// <param name="p">The color</param>
        /// <param name="pt">Origin</param>
        public virtual void DrawString(string str , FontSize fs , Brush p , PointF pt)
        {
            Font f = fs.Font;
            var size=fs.Size;
            DrawString(str , p , ( int ) pt.X , ( int ) pt.Y , f , size);
        }
        /// <summary>
        /// Write specifie text in bitmap
        /// </summary>
        /// <param name="str">Text to write</param>
        /// <param name="fs">Font and size</param>
        /// <param name="p">The color</param>
        /// <param name="pt">Origin</param>
        public virtual void DrawString(string str , FontSizeF fs , Brush p , PointF pt)
        {
            Font f = fs.Font;
            var size=fs.Size;
            DrawString(str , p , ( int ) pt.X , ( int ) pt.Y , f , size);
        }
        /// <summary>
        /// Write specifie text in bitmap
        /// </summary>
        /// <param name="str">Text to write</param>
        /// <param name="fs">Font and size</param>
        /// <param name="p">The color</param>
        /// <param name="pt">Origin</param>
        /// <param name="sf">Stringformat specifing position</param>
        public virtual void DrawString(string str , FontSize fs , Pixel p , PointF pt , StringFormat sf)
        {
            Font f = fs.Font;
            var size=fs.Size;
            DrawString(str , p , ( int ) pt.X , ( int ) pt.Y , f , size , sf);
        }
        /// <summary>
        /// Write specifie text in bitmap
        /// </summary>
        /// <param name="str">Text to write</param>
        /// <param name="fs">Font and size</param>
        /// <param name="p">The color</param>
        /// <param name="pt">Origin</param>
        /// <param name="sf">Stringformat specifing position</param>
        public virtual void DrawString(string str , FontSizeF fs , Pixel p , PointF pt , StringFormat sf)
        {
            Font f = fs.Font;
            var size=fs.Size;
            DrawString(str , p , ( int ) pt.X , ( int ) pt.Y , f , size , sf);
        }
        /// <summary>
        /// Write specifie text in bitmap
        /// </summary>
        /// <param name="str">Text to write</param>
        /// <param name="fs">Font and size</param>
        /// <param name="p">The color</param>
        /// <param name="pt">Origin</param>
        /// <param name="sf">Stringformat specifing position</param>
        public virtual void DrawString(string str , FontSize fs , Brush p , PointF pt , StringFormat sf)
        {
            Font f = fs.Font;
            var size=fs.Size;
            DrawString(str , p , ( int ) pt.X , ( int ) pt.Y , f , size , sf);
        }
        /// <summary>
        /// Write specifie text in bitmap
        /// </summary>
        /// <param name="str">Text to write</param>
        /// <param name="fs">Font and size</param>
        /// <param name="p">The color</param>
        /// <param name="pt">Origin</param>
        /// <param name="sf">Stringformat specifing position</param>
        public virtual void DrawString(string str , FontSizeF fs , Brush p , PointF pt , StringFormat sf)
        {
            Font f = fs.Font;
            var size=fs.Size;
            DrawString(str , p , ( int ) pt.X , ( int ) pt.Y , f , size , sf);
        }
        /// <summary>
        /// Write specifie text in bitmap
        /// </summary>
        /// <param name="str">Text to write</param>
        /// <param name="f">Font</param>
        /// <param name="size">Font size</param>
        /// <param name="p">The color</param>
        /// <param name="pt">Origin</param>
        public virtual void DrawString(string str , Pixel p , PointF pt , Font f , int size)
        {
            DrawString(str , p , ( int ) pt.X , ( int ) pt.Y , f , size);
        }
        /// <summary>
        /// Write specifie text in bitmap
        /// </summary>
        /// <param name="str">Text to write</param>
        /// <param name="f">Font</param>
        /// <param name="size">Font size</param>
        /// <param name="p">The color</param>
        /// <param name="pt">Origin</param>
        public virtual void DrawString(string str , Pixel p , PointF pt , Font f , float size)
        {
            DrawString(str , p , ( int ) pt.X , ( int ) pt.Y , f , size);
        }
        /// <summary>
        /// Write specifie text in bitmap
        /// </summary>
        /// <param name="str">Text to write</param>
        /// <param name="f">Font</param>
        /// <param name="size">Font size</param>
        /// <param name="p">The color</param>
        /// <param name="pt">Origin</param>
        public virtual void DrawString(string str , Brush p , PointF pt , Font f , int size)
        {
            DrawString(str , p , ( int ) pt.X , ( int ) pt.Y , f , size);
        }
        /// <summary>
        /// Write specifie text in bitmap
        /// </summary>
        /// <param name="str">Text to write</param>
        /// <param name="f">Font</param>
        /// <param name="size">Font size</param>
        /// <param name="p">The color</param>
        /// <param name="pt">Origin</param>
        public virtual void DrawString(string str , Brush p , PointF pt , Font f , float size)
        {
            DrawString(str , p , ( int ) pt.X , ( int ) pt.Y , f , size);
        }
        /// <summary>
        /// Helper to draw char
        /// </summary>
        /// <param name="character">Character as bool array</param>
        /// <param name="p">The color</param>
        /// <param name="x">X position</param>
        /// <param name="y">Y position</param>
        /// <param name="size">Font size</param>
        /// <returns></returns>
        private int DrawCharInternal(bool[ , ] character , Pixel p , int x , int y , int size)
        {
            int h = character.GetLength(0);
            int w = character.GetLength(1);
            int ox=x,oy=y;

            for ( var i = 0; i < h; i++ )
            {
                x = ox;
                for ( var j = 0; j < w; j++ )
                {
                    x += size;
                    for ( var k = 0; k < size; k++ )
                    {
                        for ( var l = 0; l < size; l++ )
                        {
                            if ( character[i , j] )
                                SetPixel(p , x + k , y + l);
                        }
                    }
                }
                y += size;
            }
            return character.GetLength(1) * size + size;
        }
        /// <summary>
        /// Helper to draw char
        /// </summary>
        /// <param name="character">Character as bool array</param>
        /// <param name="p">The color</param>
        /// <param name="x">X position</param>
        /// <param name="y">Y position</param>
        /// <param name="size">Font size</param>
        /// <returns></returns>
        private int DrawCharInternal(bool[ , ] character , Brush p , int x , int y , int size)
        {
            int h = character.GetLength(0);
            int w = character.GetLength(1);
            int ox=x,oy=y;

            for ( var i = 0; i < h; i++ )
            {
                x = ox;
                for ( var j = 0; j < w; j++ )
                {
                    x += size;
                    for ( var k = 0; k < size; k++ )
                    {
                        for ( var l = 0; l < size; l++ )
                        {
                            if ( character[i , j] )
                                SetPixel(p , x + k , y + l);
                        }
                    }
                }
                y += size;
            }
            return character.GetLength(1) * size + size;
        }
        /// <summary>
        /// Dispose of Graphics, useless but here for compatibility with System.Drawing.Graphics
        /// </summary>
        public virtual void Dispose()
        {
            bmp = null;
            transformationMatrix = null;
        }
        /// <summary>
        /// Struct to represent measure of text
        /// </summary>
        public struct StringMeasurement
        {
            /// <summary>
            /// Measures
            /// </summary>
            public float Width,Height;
        }
        /// <summary>
        /// Measure space used for specified string
        /// </summary>
        /// <param name="str">Text</param>
        /// <param name="f">Font</param>
        /// <param name="size">Font size</param>
        /// <returns>Width and Height</returns>
        public virtual StringMeasurement MeasureString(String str , Font f , float size)
        {
            float x=0,y=0;
            float mh = 0,mx=0;
            foreach ( char c in str )
            {
                bool[,] character = f.GetChar(c);
                mh = System.Math.Max(mh , character.GetLength(0));
                if ( c == 10 )
                {
                    mx = System.Math.Max(mx , x);
                    x = 0;
                    y += mh * size + 1;
                    mh = 0;
                }
                else
                    x += character.GetLength(1) * size + size;
            }
            mx = System.Math.Max(mx , x);
            return new StringMeasurement() { Height = y + mh * size , Width = mx };
        }
        #region Transform
        /// <summary>
        /// Rotate graphics using specified angle in radian
        /// </summary>
        /// <param name="angle">Angle in radian</param>
        public virtual void RotateTransform(double angle)
        {
            while ( angle < 0 )
            {
                angle += System.Math.PI * 2;
            }
            Matrix mt = Matrix.Identity(3);
            mt[0 , 0] = System.Math.Cos(angle);
            mt[0 , 1] = System.Math.Sin(angle);
            mt[1 , 0] = -System.Math.Sin(angle);
            mt[2 , 1] = System.Math.Cos(angle);
            transformationMatrix = transformationMatrix * mt;
        }
        /// <summary>
        /// Scale graphics
        /// </summary>
        /// <param name="x">X scale</param>
        /// <param name="y">Y scale</param>
        public virtual void ScaleTransform(double x , double y)
        {
            Matrix mt = Matrix.Identity(3);
            mt[0 , 0] = x;
            mt[1 , 1] = y;
            transformationMatrix = transformationMatrix * mt;
        }
        /// <summary>
        /// Translate the graphics
        /// </summary>
        /// <param name="x">X translate</param>
        /// <param name="y">Y translate</param>
        public virtual void TranslateTransform(double x , double y)
        {
            Matrix mt = Matrix.Identity(3);
            mt[0 , 2] = x;
            mt[1 , 2] = y;
            transformationMatrix = transformationMatrix * mt;
        }
        /// <summary>
        /// Perform an X Skew
        /// </summary>
        /// <param name="a"></param>
        public virtual void SkewXTransform(double a)
        {
            Matrix mt = Matrix.Identity(3);
            mt[0 , 1] = System.Math.Tan(a);
            transformationMatrix = transformationMatrix * mt;
        }
        /// <summary>
        /// Perform an Y Skew
        /// </summary>
        /// <param name="a"></param>
        public virtual void SkewYTransform(double a)
        {
            Matrix mt = Matrix.Identity(3);
            mt[1 , 0] = System.Math.Tan(a);
            transformationMatrix = transformationMatrix * mt;
        }
        /// <summary>
        /// Reset transformation
        /// </summary>
        public virtual void ResetTransform()
        {
            transformationMatrix = Matrix.Identity(3);
        }
        #endregion
    }
}


