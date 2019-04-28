using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MoyskleyTech.Mathematics;
using MoyskleyTech.ImageProcessing.Image.Helper;
using System.Reflection;
using static MoyskleyTech.ImageProcessing.Image.GraphicsHelper;
using MoyskleyTech.ImageProcessing;
namespace MoyskleyTech.ImageProcessing.Image
{
    /// <summary>
    /// PCL Portability to System.Drawing.Graphics
    /// </summary>
    public partial class Graphics<Representation> : IDisposable where Representation : unmanaged
    {
        /// <summary>
        /// The bitmap where to draw(null if proxy)
        /// </summary>
        protected Image<Representation> bmp;
        /// <summary>
        /// The image proxy(null if bitmap of graphics proxy)
        /// </summary>
        protected ImageProxy<Representation> proxy;
        /// <summary>
        /// The graphics proxy(null if bitmap or image proxy)
        /// </summary>
        protected Graphics<Representation> gProxy;

        private Matrix transformationMatrix;

        private Rectangle clip;
        private PointF[] clipPolygon;
        private double[]clipPolygonConstant , clipPolygonMultiple;

        /// <summary>
        /// Represent how to draw lines
        /// </summary>
        public LineMode LineMode = LineMode.ForLoop;
        /// <summary>
        /// Transform position using function instead of matrix(before matrix)
        /// </summary>
        public Func<PointF , PointF> PreTransformFunction { get; set; }
        /// <summary>
        /// Transform position using function instead of matrix(after matrix)
        /// </summary>
        public Func<PointF , PointF> PostTransformFunction { get; set; }
        /// <summary>
        /// Function to choose what appen to current pixels
        /// </summary>
        public Func<Representation,Representation,Representation> CompositionFunction;
        private Func<Representation,Pixel> ToPixel;
        /// <summary>
        /// Only for subclassing
        /// </summary>
        protected Graphics() { }
        /// <summary>
        /// Create a Graphics object from image
        /// </summary>
        /// <param name="bmp">The bitmap where to Draw</param>
        /// <returns>Return null if not applicable</returns>
        public static Graphics<Representation> FromImage(Image<Representation> bmp)
        {
            if ( bmp.Disposed )
                throw new ObjectDisposedException(nameof(bmp));
            Graphics<Representation> instance;
            instance = new Graphics<Representation>(1)
            {
                bmp = bmp
            };
            instance.ResetClip();
            return instance;
        }

        /// <summary>
        /// Create a Graphics object from image
        /// </summary>
        /// <param name="bmp">The bitmap where to Draw</param>
        /// <returns>Return null if not applicable</returns>
        public static Graphics<Representation> FromProxy(ImageProxy<Representation> bmp)
        {
            Graphics<Representation> instance;
            instance = new Graphics<Representation>(1)
            {
                proxy = bmp
            };
            instance.ResetClip();
            return instance;
        }
        /// <summary>
        /// Private constructor to things not related to bitmap
        /// </summary>
        protected Graphics(int u)
        {
            transformationMatrix = Matrix.Identity(3);
            PreTransformFunction = null;
            PostTransformFunction = null;

            CompositionFunction = (x , y) => x;

            if ( typeof(Representation) == typeof(Pixel) )
                CompositionFunction = (x , y) => ( Representation ) ( object ) ComposingFunctionPixel(( Pixel ) ( object ) x , ( Pixel ) ( object ) y);
            if ( typeof(Representation) == typeof(ARGB_16bit) )
                CompositionFunction = (x , y) => ( Representation ) ( object ) ComposingFunctionARGB_16bit(( ARGB_16bit ) ( object ) x , ( ARGB_16bit ) ( object ) y);
            if ( typeof(Representation) == typeof(ARGB_Float) )
                CompositionFunction = (x , y) => ( Representation ) ( object ) ComposingFunctionARGB_Float(( ARGB_Float ) ( object ) x , ( ARGB_Float ) ( object ) y);

            ToPixel = ColorConvert.GetConversionFrom<Representation , Pixel>();
        }
        /// <summary>
        /// Optionnal height for proxy
        /// </summary>
        protected int? width,height,x,y;
        /// <summary>
        /// Get width of graphics
        /// </summary>
        public virtual int Width { get => width ?? bmp?.Width ?? proxy?.Width ?? gProxy?.Width ?? 0; set { width = value; } }
        /// <summary>
        /// Get height of graphics
        /// </summary>
        public virtual int Height { get => height ?? bmp?.Height ?? proxy?.Height ?? gProxy?.Height ?? 0; set { height = value; } }

        /// <summary>
        /// Allow proxying graphics as another graphics(high level)
        /// </summary>
        /// <param name="r"></param>
        /// <returns></returns>
        public virtual Graphics<Representation> Proxy(Rectangle r)
        {
            Graphics<Representation> instance;
            instance = new Graphics<Representation>(1)
            {
                gProxy = this ,
                x = r.X ,
                y = r.Y ,
                width = r.Width ,
                height = r.Height
            };
            instance.ResetClip();
            return instance;
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
            clip = new Rectangle() { X = 0 , Y = 0 , Width = Width , Height = Height };
            clipPolygon = null;
            clipPolygonConstant = null;
            clipPolygonMultiple = null;
        }

        public virtual ClipState SaveClipState()
        {
            ClipState state = new ClipState
            {
                clip = clip ,
                clipPolygon = ( PointF[ ] ) clipPolygon?.Clone() ,
                clipPolygonConstant = ( double[ ] ) clipPolygonConstant?.Clone() ,
                clipPolygonMultiple = ( double[ ] ) clipPolygonMultiple?.Clone()
            };
            return state;
        }
        public virtual void RestoreClipState(ClipState state)
        {
            clip = state.clip;
            clipPolygon = ( PointF[ ] ) state.clipPolygon?.Clone();
            clipPolygonConstant = ( double[ ] ) state.clipPolygonConstant?.Clone();
            clipPolygonMultiple = ( double[ ] ) state.clipPolygonMultiple?.Clone();
        }
        /// <summary>
        /// Get the transformation matrix for editing purpose
        /// </summary>
        public Matrix TransformationMatrix
        {
            get { return transformationMatrix; }
            set
            {
                if ( object.ReferenceEquals(value , null) )
                    ResetTransform();
                else
                    for ( var i = 0; i < transformationMatrix.Rows; i++ )
                        for ( var j = 0; j < transformationMatrix.Columns; j++ )
                            transformationMatrix[i , j] = value[i , j];
            }
        }

        /// <summary>
        /// Draw a rectangle of the specified color
        /// </summary>
        /// <param name="p">The color</param>
        /// <param name="x">X position</param>
        /// <param name="y">Y position</param>
        /// <param name="w">Width</param>
        /// <param name="h">Height</param>
        public virtual void DrawRectangle(Representation p , double x , double y , double w , double h)
        {
            DrawLine(p , x , y , x , y + h - 1); //Left
            DrawLine(p , x , y , x + w - 1 , y); //Top
            DrawLine(p , x + w - 1 , y , x + w - 1 , y + h - 1);//Right
            DrawLine(p , x , y + h - 1 , x + w - 1 , y + h - 1);//Bottom
        }
        /// <summary>
        /// Draw a rectangle of the specified color
        /// </summary>
        /// <param name="p">The color</param>
        /// <param name="x">X position</param>
        /// <param name="y">Y position</param>
        /// <param name="w">Width</param>
        /// <param name="h">Height</param>
        public virtual void DrawRectangle(Brush<Representation> p , double x , double y , double w , double h)
        {
            DrawLine(p , x , y , x , y + h - 1); //Left
            DrawLine(p , x , y , x + w - 1 , y); //Top
            DrawLine(p , x + w - 1 , y , x + w - 1 , y + h - 1);//Right
            DrawLine(p , x , y + h - 1 , x + w - 1 , y + h - 1);//Bottom
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
        public virtual void DrawRectangle(Representation p , double x , double y , double w , double h , int thick)
        {
            DrawLine(p , x , y , x , y + h - 1 , thick); //Left
            DrawLine(p , x , y , x + w - 1 , y , thick); //Top
            DrawLine(p , x + w - 1 , y , x + w - 1 , y + h - 1 , thick);//Right
            DrawLine(p , x , y + h - 1 , x + w - 1 , y + h - 1 , thick);//Bottom
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
        public virtual void DrawRectangle(Brush<Representation> p , double x , double y , double w , double h , int thick)
        {
            DrawLine(p , x , y , x , y + h - 1 , thick); //Left
            DrawLine(p , x , y , x + w - 1 , y , thick); //Top
            DrawLine(p , x + w - 1 , y , x + w - 1 , y + h - 1 , thick);//Right
            DrawLine(p , x , y + h - 1 , x + w - 1 , y + h - 1 , thick);//Bottom
        }
        /// <summary>
        /// Draw a rectangle of the specified color
        /// </summary>
        /// <param name="p">The color</param>
        /// <param name="pos">Position</param>
        /// <param name="w">Width</param>
        /// <param name="h">Height</param>
        public void DrawRectangle(Representation p , PointF pos , int w , int h)
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
        public void DrawRectangle(Brush<Representation> p , PointF pos , int w , int h)
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
        public void DrawRectangle(Representation p , PointF pos , SizeF h)
        {
            DrawRectangle(p , pos.X , pos.Y , h.Width , h.Height);
        }
        /// <summary>
        /// Draw a rectangle of the specified color
        /// </summary>
        /// <param name="p">The color</param>
        /// <param name="pos">Position</param>
        /// <param name="w">Width</param>
        /// <param name="h">Height</param>
        public void DrawRectangle(Brush<Representation> p , PointF pos , SizeF h)
        {
            DrawRectangle(p , pos.X , pos.Y , h.Width , h.Height);
        }
        /// <summary>
        /// Draw a rectangle of the specified color
        /// </summary>
        /// <param name="p">The color</param>
        /// <param name="r">The rectangle</param>
        public void DrawRectangle(Representation p , Rectangle r)
        {
            DrawRectangle(p , r.X , r.Y , r.Width , r.Height);
        }
        /// <summary>
        /// Draw a rectangle of the specified color
        /// </summary>
        /// <param name="p">The color</param>
        /// <param name="r">The rectangle</param>
        public void DrawRectangle(Brush<Representation> p , Rectangle r)
        {
            DrawRectangle(p , r.X , r.Y , r.Width , r.Height);
        }
        /// <summary>
        /// Draw a rectangle of the specified color
        /// </summary>
        /// <param name="p">The color</param>
        /// <param name="r">The rectangle</param>
        public void DrawRectangle(Representation p , RectangleF r)
        {
            DrawRectangle(p , r.X , r.Y , r.Width , r.Height);
        }
        /// <summary>
        /// Draw a rectangle of the specified color
        /// </summary>
        /// <param name="p">The color</param>
        /// <param name="r">The rectangle</param>
        public void DrawRectangle(Brush<Representation> p , RectangleF r)
        {
            DrawRectangle(p , r.X , r.Y , r.Width , r.Height);
        }
        /// <summary>
        /// Draw a polygon outline
        /// </summary>
        /// <param name="p">The color</param>
        /// <param name="points">Points describing the polygon</param>
        public virtual void DrawPolygon(Representation p , params PointF[ ] points)
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
        public virtual void DrawPolygon(Brush<Representation> p , params PointF[ ] points)
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
        public virtual void DrawPolygon(Representation p , int thickness , params PointF[ ] points)
        {
            if ( thickness <= 1 )
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
        public virtual void DrawPolygon(Brush<Representation> p , int thickness , params PointF[ ] points)
        {
            if ( thickness <= 1 )
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
        public virtual void DrawPath(Representation p , params PointF[ ] points)
        {
            for ( var i = 0; i < points.Length - 1; i++ )
                DrawLine(p , points[i] , points[i + 1]);
        }
        /// <summary>
        /// Draw a polygon outline
        /// </summary>
        /// <param name="p">The color</param>
        /// <param name="points">Points describing the polygon</param>
        public virtual void DrawPath(Brush<Representation> p , params PointF[ ] points)
        {
            for ( var i = 0; i < points.Length - 1; i++ )
                DrawLine(p , points[i] , points[i + 1]);
        }
        /// <summary>
        /// Draw a polygon outline
        /// </summary>
        /// <param name="p">The color</param>
        /// <param name="thickness">Thickness of outline</param>
        /// <param name="points">Points describing the polygon</param>
        public virtual void DrawPath(Representation p , int thickness , params PointF[ ] points)
        {
            if ( thickness <= 1 )
                DrawPath(p , points);
            else
            {
                for ( var i = 0; i < points.Length - 1; i++ )
                    DrawLine(p , points[i] , points[i + 1] , thickness);
            }
        }
        /// <summary>
        /// Draw a polygon outline
        /// </summary>
        /// <param name="p">The color</param>
        /// <param name="thickness">Thickness of outline</param>
        /// <param name="points">Points describing the polygon</param>
        public virtual void DrawPath(Brush<Representation> p , int thickness , params PointF[ ] points)
        {
            if ( thickness <= 1 )
                DrawPath(p , points);
            else
            {
                for ( var i = 0; i < points.Length - 1; i++ )
                    DrawLine(p , points[i] , points[i + 1] , thickness);
            }
        }

        /// <summary>
        /// Draw a polygon outline
        /// </summary>
        /// <param name="p">The color</param>
        /// <param name="points">Points describing the polygon</param>
        public void DrawPolygon(Representation p , IEnumerable<PointF> points)
        {
            DrawPolygon(p , points.ToArray());
        }
        /// <summary>
        /// Draw a polygon outline
        /// </summary>
        /// <param name="p">The color</param>
        /// <param name="points">Points describing the polygon</param>
        public void DrawPolygon(Brush<Representation> p , IEnumerable<PointF> points)
        {
            DrawPolygon(p , points.ToArray());
        }
        /// <summary>
        /// Draw a polygon outline
        /// </summary>
        /// <param name="p">The color</param>
        /// <param name="thickness">Thickness of outline</param>
        /// <param name="points">Points describing the polygon</param>
        public void DrawPolygon(Representation p , int thickness , IEnumerable<PointF> points)
        {
            DrawPolygon(p , thickness , points.ToArray());
        }
        /// <summary>
        /// Draw a polygon outline
        /// </summary>
        /// <param name="p">The color</param>
        /// <param name="thickness">Thickness of outline</param>
        /// <param name="points">Points describing the polygon</param>
        public void DrawPolygon(Brush<Representation> p , int thickness , IEnumerable<PointF> points)
        {
            DrawPolygon(p , thickness , points.ToArray());
        }

        /// <summary>
        /// Draw a polygon outline
        /// </summary>
        /// <param name="p">The color</param>
        /// <param name="points">Points describing the polygon</param>
        public void DrawPath(Representation p , IEnumerable<PointF> points)
        {
            DrawPath(p , points.ToArray());
        }
        /// <summary>
        /// Draw a polygon outline
        /// </summary>
        /// <param name="p">The color</param>
        /// <param name="points">Points describing the polygon</param>
        public void DrawPath(Brush<Representation> p , IEnumerable<PointF> points)
        {
            DrawPath(p , points.ToArray());
        }
        /// <summary>
        /// Draw a polygon outline
        /// </summary>
        /// <param name="p">The color</param>
        /// <param name="thickness">Thickness of outline</param>
        /// <param name="points">Points describing the polygon</param>
        public void DrawPath(Representation p , int thickness , IEnumerable<PointF> points)
        {
            DrawPath(p , thickness , points.ToArray());
        }
        /// <summary>
        /// Draw a polygon outline
        /// </summary>
        /// <param name="p">The color</param>
        /// <param name="thickness">Thickness of outline</param>
        /// <param name="points">Points describing the polygon</param>
        public void DrawPath(Brush<Representation> p , int thickness , IEnumerable<PointF> points)
        {
            DrawPath(p , thickness , points.ToArray());
        }
        /// <summary>
        /// Fill the rectangle using the specified color
        /// </summary>
        /// <param name="p">The color</param>
        /// <param name="px">X position</param>
        /// <param name="py">Y position</param>
        /// <param name="pw">Width</param>
        /// <param name="ph">Height</param>
        public virtual void FillRectangle(Representation p , double px , double py , double pw , double ph)
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
        public virtual void FillRectangle(Brush<Representation> p , double px , double py , double pw , double ph)
        {
            int x=(int)px,y=(int)py, w=(int)pw, h=(int)ph;
            FillPolygon(p , new PointF(x , y) , new PointF(x + w , y) , new PointF(x + w , y + h) , new PointF(x , y + h));
        }
        /// <summary>
        /// Fill the specified polygon using pnpoly and bounds
        /// </summary>
        /// <param name="p">The color</param>
        /// <param name="points">Points describing the polygon</param>
        public virtual void FillPolygon(Representation p , params PointF[ ] points)
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
        public virtual void FillPolygon(Brush<Representation> p , params PointF[ ] points)
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
        public virtual void DrawImage(ImageProxy<Representation> source , int x , int y)
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
        public virtual void DrawImage(ImageProxy<Representation> source , int x , int y,int width,int height)
        {
            if ( source.Width != width || source.Height != height )
            {
                var d = source.ToImage().Rescale(width,height, ScalingMode.AverageInterpolate);
                DrawImage(d , x , y);
                d.Dispose();
            }
            else
                DrawImage(source , x , y);
        }
        /// <summary>
        /// Draw image in bitmap
        /// </summary>
        /// <param name="source">Source</param>
        /// <param name="x">X origin</param>
        /// <param name="y">Y origin</param>
        public void DrawImage(ImageProxy<Representation> source , Point pt)
        {
            DrawImage(source , pt.X , pt.Y);
        }
        /// <summary>
        /// Draw image in bitmap
        /// </summary>
        /// <param name="source">Source</param>
        /// <param name="x">X origin</param>
        /// <param name="y">Y origin</param>
        public void DrawImage(ImageProxy<Representation> source , Rectangle pt)
        {
            DrawImage(source , pt.X , pt.Y,pt.Width,pt.Height);
        }

        /// <summary>
        /// Draw a line
        /// </summary>
        /// <param name="p">The color</param>
        /// <param name="x">X origin</param>
        /// <param name="y">Y origin</param>
        /// <param name="x2">X destination</param>
        /// <param name="y2">Y destination</param>
        public virtual void DrawLine(Representation p , double x , double y , double x2 , double y2)
        {
            PointF p1,p2;

            p1 = TransformUsingMatrix(new PointF(x , y));
            p2 = TransformUsingMatrix(new PointF(x2 , y2));

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
        public virtual void DrawLine(Brush<Representation> p , double x , double y , double x2 , double y2)
        {
            PointF p1,p2;

            p1 = TransformUsingMatrix(new PointF(x , y));
            p2 = TransformUsingMatrix(new PointF(x2 , y2));

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
        public virtual void DrawLine(Representation p , double x , double y , double x2 , double y2 , int thickness)
        {
            if ( thickness <= 1 )
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
        public virtual void DrawLine(Brush<Representation> p , double x , double y , double x2 , double y2 , int thickness)
        {
            if ( thickness <= 0 )
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
        private void DrawLineBeforeTransform(Representation p , double x1 , double y1 , double x2 , double y2 , int thickness)
        {

            // < line x1 = "0" y1 = "0" x2 = "200" y2 = "200" style = "stroke:rgb(255,0,0);stroke-width:2" />

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
        private void DrawLineBeforeTransform(Brush<Representation> p , double x1 , double y1 , double x2 , double y2 , int thickness)
        {

            // < line x1 = "0" y1 = "0" x2 = "200" y2 = "200" style = "stroke:rgb(255,0,0);stroke-width:2" />

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
        public void DrawLine(Representation p , PointF p1 , PointF p2)
        {
            DrawLine(p , p1.X , p1.Y , p2.X , p2.Y);
        }
        /// <summary>
        /// Draw line
        /// </summary>
        /// <param name="p">The color</param>
        /// <param name="p1">Origin</param>
        /// <param name="p2">Destination</param>
        public void DrawLine(Brush<Representation> p , PointF p1 , PointF p2)
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
        public void DrawLine(Representation p , PointF p1 , PointF p2 , int thickness)
        {
            DrawLine(p , p1.X , p1.Y , p2.X , p2.Y , thickness);
        }
        /// <summary>
        /// Draw line
        /// </summary>
        /// <param name="p">The color</param>
        /// <param name="p1">Origin</param>
        /// <param name="p2">Destination</param>
        /// <param name="thickness">Thickness of line</param>
        public void DrawLine(Brush<Representation> p , PointF p1 , PointF p2 , int thickness)
        {
            DrawLine(p , p1.X , p1.Y , p2.X , p2.Y , thickness);
        }
        /// <summary>
        /// Clear the Bitmap using specified color
        /// </summary>
        /// <param name="p">The color</param>
        public virtual void Clear(Representation p)
        {
            for ( var i = 0; i < Width; i++ )
                for ( var j = 0; j < Height; j++ )
                    this[i , j] = p;
        }
        /// <summary>
        /// Clear the Bitmap using specified color
        /// </summary>
        /// <param name="p">The color</param>
        public virtual void Clear(Brush<Representation> p)
        {
            for ( var i = 0; i < Width; i++ )
                for ( var j = 0; j < Height; j++ )
                    this[i , j] = p.GetColor(i , j);
        }
        /// <summary>
        /// Helper to set pixel
        /// </summary>
        /// <param name="p">The color</param>
        /// <param name="px">X position</param>
        /// <param name="py">Y position</param>
        public virtual void SetPixelWithoutTransform(Representation p , double px , double py)
        {
            SetPixelWithoutTransform(p , px , py , true);
        }
        public void SetPixelWithoutTransform(Representation p , PointF pt)
        {
            SetPixelWithoutTransform(p , pt.X , pt.Y);
        }
        /// <summary>
        /// Helper to set pixel
        /// </summary>
        /// <param name="p">The color</param>
        /// <param name="px">X position</param>
        /// <param name="py">Y position</param>
        /// <param name="alpha">if pixel contains alpha</param>
        public virtual void SetPixelWithoutTransform(Representation p , double px , double py , bool alpha)
        {
            int x,y;
            x = ( int ) px;
            y = ( int ) py;

            if ( IsInClipRange(x , y) )
            {
                if ( gProxy != null )
                {
                    gProxy.SetPixelWithoutTransform(p , px + this.x.Value , py + this.y.Value , alpha);
                }
                else
                {
                    Representation fromImage = this[x,y];
                    Representation that = p;
                    this[x , y] = CompositionFunction(that , fromImage);
                }
            }
        }
        public void SetPixelWithoutTransform(Representation p , PointF pt , bool alpha)
        {
            SetPixelWithoutTransform(p , pt.X , pt.Y , alpha);
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
        /// <param name="b">The color</param>
        /// <param name="px">X position</param>
        /// <param name="py">Y position</param>
        private void SetPixelInternal(Brush<Representation> b , double px , double py)
        {
            SetPixelInternal(b , px , py , true);
        }
        private void SetPixelInternal(Brush<Representation> b , double px , double py , bool alpha)
        {
            int x,y;
            x = ( int ) px;
            y = ( int ) py;

            if ( x >= clip.X && x < clip.X + clip.Width )
                if ( y >= clip.Y && y < clip.Y + clip.Height )
                {

                    if ( gProxy != null )
                    {
                        gProxy.SetPixelWithoutTransform(b.GetColor(x , y) , px + this.x.Value , py + this.y.Value , alpha);
                    }
                    Representation p = b.GetColor(x,y);
                    {
                        Representation fromImage = this[x,y];
                        Representation that = p;
                        this[x , y] = CompositionFunction(that , fromImage);
                    }

                }
        }
        /// <summary>
        /// Base Composing function
        /// </summary>
        /// <param name="current"></param>
        /// <param name="fromImage"></param>
        /// <returns></returns>
        public static Pixel ComposingFunctionPixel(Pixel current , Pixel fromImage)
        {
            byte[] result = new byte[4];
            result[0] = ( byte ) System.Math.Max(current.A , fromImage.A);
            result[1] = ( byte ) ( ( current.A * current.R + fromImage.R * ( 255 - current.A ) ) / 255 );
            result[2] = ( byte ) ( ( current.A * current.G + fromImage.G * ( 255 - current.A ) ) / 255 );
            result[3] = ( byte ) ( ( current.A * current.B + fromImage.B * ( 255 - current.A ) ) / 255 );
            return Pixel.FromArgb(result[0] , result[1] , result[2] , result[3]);
        }
        /// <summary>
        /// Base Composing function
        /// </summary>
        /// <param name="current"></param>
        /// <param name="fromImage"></param>
        /// <returns></returns>
        public static ARGB_Float ComposingFunctionARGB_Float(ARGB_Float current , ARGB_Float fromImage)
        {
            float[] result = new float[4];
            result[0] = ( float ) System.Math.Max(current.A , fromImage.A);
            result[1] = ( float ) ( ( current.A * current.R + fromImage.R * ( 1 - current.A ) ) );
            result[2] = ( float ) ( ( current.A * current.G + fromImage.G * ( 1 - current.A ) ) );
            result[3] = ( float ) ( ( current.A * current.B + fromImage.B * ( 1 - current.A ) ) );
            return new ARGB_Float { A = result[0] , R = result[1] , G = result[2] , B = result[3] };
        }
        /// <summary>
        /// Base Composing function
        /// </summary>
        /// <param name="current"></param>
        /// <param name="fromImage"></param>
        /// <returns></returns>
        public static ARGB_16bit ComposingFunctionARGB_16bit(ARGB_16bit current , ARGB_16bit fromImage)
        {
            ushort[] result = new ushort[4];
            result[0] = ( ushort ) System.Math.Max(current.A , fromImage.A);
            result[1] = ( ushort ) ( ( current.A * current.R + fromImage.R * ( 1 - current.A ) ) / ushort.MaxValue );
            result[2] = ( ushort ) ( ( current.A * current.G + fromImage.G * ( 1 - current.A ) ) / ushort.MaxValue );
            result[3] = ( ushort ) ( ( current.A * current.B + fromImage.B * ( 1 - current.A ) ) / ushort.MaxValue );
            return new ARGB_16bit { A = result[0] , R = result[1] , G = result[2] , B = result[3] };
        }
        /// <summary>
        /// Helper to draw line
        /// </summary>
        /// <param name="p">The color</param>
        /// <param name="p1">Origin</param>
        /// <param name="p2">Destination</param>
        private void DrawLineInternal(Representation p , PointF p1 , PointF p2)
        {

            // < line x1 = "0" y1 = "0" x2 = "200" y2 = "200" style = "stroke:rgb(255,0,0);stroke-width:2" />
            int x1 = (int)p1.X;
            int x2 = (int)p2.X;
            int y1 = (int)p1.Y;
            int y2 = (int)p2.Y;

            if ( LineMode == LineMode.ForLoop )
            {

                double dx = (x2 - x1) ;
                double dy = (y2 - y1) ;
                double dydx = dy/dx;
                double dxdy = dx/dy;

                if ( dx < 0 )
                    for ( var i = x2; i < x1; i++ )
                        SetPixelWithoutTransform(p , i , y2 + dydx * ( i - x2 ));
                else
                    for ( var i = x1; i < x2; i++ )
                        SetPixelWithoutTransform(p , i , y1 + dydx * ( i - x1 ));

                if ( dy < 0 )
                    for ( var i = y2; i < y1; i++ )
                        SetPixelWithoutTransform(p , x2 + dxdy * ( i - y2 ) , i);
                else
                    for ( var i = y1; i < y2; i++ )
                        SetPixelWithoutTransform(p , x1 + dxdy * ( i - y1 ) , i);
            }
            else if ( LineMode == LineMode.FourConnex )
            {
                SetPixelWithoutTransform(p , x1 , y1);
                while ( y1 != y2 || x2 != x1 )
                {
                    var options = new List<Point>(4)
                    {
                        new Point(x1 - 1 , y1) ,
                        new Point(x1 + 1 , y1) ,
                        new Point(x1 , y1 - 1) ,
                        new Point(x1 , y1 + 1)
                    };
                    var pt = FindBestOption(x2,y2, options);
                    x1 = pt.X;
                    y1 = pt.Y;
                    SetPixelWithoutTransform(p , x1 , y1);
                }
            }
            else if ( LineMode == LineMode.EightConnex )
            {
                SetPixelWithoutTransform(p , x1 , y1);
                while ( y1 != y2 || x2 != x1 )
                {
                    var options = new List<Point>(8)
                    {
                        new Point(x1 - 1 , y1) ,
                        new Point(x1 + 1 , y1) ,
                        new Point(x1 , y1 - 1) ,
                        new Point(x1 , y1 + 1) ,

                        new Point(x1 - 1 , y1 - 1) ,
                        new Point(x1 - 1 , y1 + 1) ,
                        new Point(x1 + 1 , y1 - 1) ,
                        new Point(x1 + 1 , y1 + 1)
                    };
                    var pt = FindBestOption(x2,y2, options);
                    x1 = pt.X;
                    y1 = pt.Y;
                    SetPixelWithoutTransform(p , x1 , y1);
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
        private void DrawLineInternal(Brush<Representation> p , PointF p1 , PointF p2)
        {

            // < line x1 = "0" y1 = "0" x2 = "200" y2 = "200" style = "stroke:rgb(255,0,0);stroke-width:2" />
            int x1 = (int)p1.X;
            int x2 = (int)p2.X;
            int y1 = (int)p1.Y;
            int y2 = (int)p2.Y;


            if ( LineMode == LineMode.ForLoop )
            {

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
                    var options = new List<Point>(4)
                    {
                        new Point(x1 - 1 , y1) ,
                        new Point(x1 + 1 , y1) ,
                        new Point(x1 , y1 - 1) ,
                        new Point(x1 , y1 + 1)
                    };
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
                    var options = new List<Point>(8)
                    {
                        new Point(x1 - 1 , y1) ,
                        new Point(x1 + 1 , y1) ,
                        new Point(x1 , y1 - 1) ,
                        new Point(x1 , y1 + 1) ,

                        new Point(x1 - 1 , y1 - 1) ,
                        new Point(x1 - 1 , y1 + 1) ,
                        new Point(x1 + 1 , y1 - 1) ,
                        new Point(x1 + 1 , y1 + 1)
                    };
                    var pt = FindBestOption(x2,y2, options);
                    x1 = pt.X;
                    y1 = pt.Y;
                    SetPixelInternal(p , x1 , y1);
                }
            }
        }
        /// <summary>
        /// Draw circle outline
        /// </summary>
        /// <param name="p">The color</param>
        /// <param name="x0">X center</param>
        /// <param name="y0">Y center</param>
        /// <param name="r">Radius</param>
        public virtual void DrawCircle(Representation p , int x0 , int y0 , double r)
        {
            Point px = TransformUsingMatrix(x0,y0);

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
        public virtual void DrawCircle(Brush<Representation> p , int x0 , int y0 , double r)
        {
            Point px = TransformUsingMatrix(x0,y0);

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
        public virtual void DrawCircle(Representation p , int x0 , int y0 , double r , int thickness)
        {
            if ( thickness <= 1 )
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
        public virtual void DrawCircle(Brush<Representation> p , int x0 , int y0 , double r , int thickness)
        {
            if ( thickness <= 1 )
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
        public void DrawCircle(Representation p , Point pt , double r)
        {
            DrawCircle(p , pt.X , pt.Y , r);
        }
        /// <summary>
        /// Draw circle outline
        /// </summary>
        /// <param name="p">The color</param>
        /// <param name="x0">X center</param>
        /// <param name="y0">Y center</param>
        /// <param name="r">Radius</param>
        public void DrawCircle(Brush<Representation> p , Point pt , double r)
        {
            DrawCircle(p , pt.X , pt.Y , r);
        }
        /// <summary>
        /// Draw circle outline
        /// </summary>
        /// <param name="p">The color</param>
        /// <param name="x0">X center</param>
        /// <param name="y0">Y center</param>
        /// <param name="r">Radius</param>
        /// <param name="thickness">Thickness of outline</param>
        public void DrawCircle(Representation p , Point pt , double r , int thickness)
        {
            DrawCircle(p , pt.X , pt.Y , r , thickness);
        }
        /// <summary>
        /// Draw circle outline
        /// </summary>
        /// <param name="p">The color</param>
        /// <param name="x0">X center</param>
        /// <param name="y0">Y center</param>
        /// <param name="r">Radius</param>
        /// <param name="thickness">Thickness of outline</param>
        public void DrawCircle(Brush<Representation> p , Point pt , double r , int thickness)
        {
            DrawCircle(p , pt.X , pt.Y , r , thickness);
        }
        /// <summary>
        /// Fill circle
        /// </summary>
        /// <param name="p">The color</param>
        /// <param name="x0">X center</param>
        /// <param name="y0">Y center</param>
        /// <param name="r">Radius</param>
        public virtual void FillCircle(Representation p , int x0 , int y0 , double r)
        {
            Point px = TransformUsingMatrix(x0,y0);
            if ( r <= 1 )
                SetPixel(p , x0 , y0);
            else
            {
                var poly = GetCirclePolygon(x0 , y0 , r);

                FillPolygon(p , poly);
            }
        }
        /// <summary>
        /// Fill circle
        /// </summary>
        /// <param name="p">The color</param>
        /// <param name="x0">X center</param>
        /// <param name="y0">Y center</param>
        /// <param name="r">Radius</param>
        public virtual void FillCircle(Brush<Representation> p , int x0 , int y0 , double r)
        {
            Point px = TransformUsingMatrix(x0,y0);
            if ( r <= 1 )
                SetPixel(p , x0 , y0);
            else
            {
                var poly = GetCirclePolygon(x0 , y0 , r);

                FillPolygon(p , poly);
            }
        }

        /// <summary>
        /// Fill circle
        /// </summary>
        /// <param name="p">The color</param>
        /// <param name="x0">X center</param>
        /// <param name="y0">Y center</param>
        /// <param name="r">Radius</param>
        public void FillCircle(Representation p , Point pt , double r)
        {
            FillCircle(p , pt.X , pt.Y , r);
        }
        /// <summary>
        /// Fill circle
        /// </summary>
        /// <param name="p">The color</param>
        /// <param name="x0">X center</param>
        /// <param name="y0">Y center</param>
        /// <param name="r">Radius</param>
        public void FillCircle(Brush<Representation> p , Point pt , double r)
        {
            FillCircle(p , pt.X , pt.Y , r);
        }
        /// <summary>
        /// Fill Ellipse(circle)
        /// </summary>
        /// <param name="p">The color</param>
        /// <param name="x">X center</param>
        /// <param name="y">Y center</param>
        /// <param name="w">Width</param>
        /// <param name="h">Height</param>
        public virtual void FillEllipse(Representation p , int x , int y , int w , int h)
        {
            var poly = GetEllipsePolygon(x , y , w,h);
            FillPolygon(p , poly);
        }
        public void FillEllipse(Representation p , Point pt , int w , int h)
        {
            FillEllipse(p , pt.X , pt.Y , w , h);
        }
        public void FillEllipse(Representation p , Point pt , Size s)
        {
            FillEllipse(p , pt.X , pt.Y , s.Width , s.Height);
        }
        /// <summary>
        /// Fill Ellipse(circle)
        /// </summary>
        /// <param name="p">The color</param>
        /// <param name="x">X center</param>
        /// <param name="y">Y center</param>
        /// <param name="w">Width</param>
        /// <param name="h">Height</param>
        public virtual void FillEllipse(Brush<Representation> p , int x , int y , int w , int h)
        {
            var poly = GetEllipsePolygon(x , y , w,h);
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
        public virtual void DrawEllipse(Representation p , int x , int y , int w , int h)
        {
            var poly = GetEllipsePolygon(x , y , w,h);
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
        public virtual void DrawEllipse(Brush<Representation> p , int x , int y , int w , int h)
        {
            var poly = GetEllipsePolygon(x , y , w,h);
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
        public void DrawEllipse(Brush<Representation> p , Point pt , int w , int h)
        {
            DrawEllipse(p , pt.X , pt.Y , w , h);
        }
        /// <summary>
        /// Draw Ellipse(circle)
        /// </summary>
        /// <param name="p">The color</param>
        /// <param name="x">X center</param>
        /// <param name="y">Y center</param>
        /// <param name="w">Width</param>
        /// <param name="h">Height</param>
        public void DrawEllipse(Brush<Representation> p , Point pt , Size s)
        {
            DrawEllipse(p , pt.X , pt.Y , s.Width , s.Height);
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
        public virtual void DrawEllipse(Representation p , int x , int y , int w , int h , int t)
        {
            if ( t <= 1 )
                DrawEllipse(p , x , y , w , h);
            else
            {
                var poly = GetEllipsePolygon(x , y , w,h);
                DrawPolygon(p , t , poly);
            }
        }
        /// <summary>
        /// Draw Ellipse(circle)
        /// </summary>
        /// <param name="p">The color</param>
        /// <param name="x">X center</param>
        /// <param name="y">Y center</param>
        /// <param name="w">Width</param>
        /// <param name="h">Height</param>
        /// <param name="t">Thickness of outline</param>
        public void DrawEllipse(Representation p , Point pt , int w , int h , int t)
        {
            DrawEllipse(p , pt.X , pt.Y , w , h , t);
        }
        /// <summary>
        /// Draw Ellipse(circle)
        /// </summary>
        /// <param name="p">The color</param>
        /// <param name="x">X center</param>
        /// <param name="y">Y center</param>
        /// <param name="w">Width</param>
        /// <param name="h">Height</param>
        /// <param name="t">Thickness of outline</param>
        public void DrawEllipse(Representation p , Point pt , Size s , int t)
        {
            DrawEllipse(p , pt.X , pt.Y , s.Width , s.Height , t);
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
        public virtual void DrawEllipse(Brush<Representation> p , int x , int y , int w , int h , int t)
        {
            if ( t <= 1 )
                DrawEllipse(p , x , y , w , h);
            else
            {
                var poly = GetEllipsePolygon(x , y , w,h);
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
        public virtual IEnumerable<PointF> DrawPie(Representation p , int x0 , int y0 , double r , double spanAngle , double startAngle)
        {
            PointF[ ] poly2 = GetPiePolygon(x0 , y0 , r ,r, spanAngle , startAngle);

            DrawPolygon(p , poly2);

            return poly2;
        }
        public IEnumerable<PointF> DrawPie(Representation p , Point pt , double r , double spanAngle , double startAngle)
        {
            return DrawPie(p , pt.X , pt.Y , r , spanAngle , startAngle);
        }
        public IEnumerable<PointF> DrawPie(Representation p , Point pt , double r , double spanAngle , double startAngle , int thickness)
        {
            return DrawPie(p , pt.X , pt.Y , r , spanAngle , startAngle , thickness);
        }
        public IEnumerable<PointF> DrawPie(Representation p , Point pt , Size s , double spanAngle , double startAngle)
        {
            return DrawPie(p , pt.X , pt.Y , s.Width , s.Height , spanAngle , startAngle);
        }
        public IEnumerable<PointF> DrawPie(Representation p , Point pt , Size s , double spanAngle , double startAngle , int thickness)
        {
            return DrawPie(p , pt.X , pt.Y , s.Width , s.Height , spanAngle , startAngle , thickness);
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
        public virtual IEnumerable<PointF> DrawPie(Brush<Representation> p , int x0 , int y0 , double r , double spanAngle , double startAngle)
        {
            PointF[ ] poly2 = GetPiePolygon(x0 , y0 , r ,r, spanAngle , startAngle);

            DrawPolygon(p , poly2);

            return poly2;
        }
        public IEnumerable<PointF> DrawPie(Brush<Representation> p , Point pt , double r , double spanAngle , double startAngle)
        {
            return DrawPie(p , pt.X , pt.Y , r , spanAngle , startAngle);
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
        public virtual IEnumerable<PointF> DrawPie(Representation p , int x0 , int y0 , double w , double h , double spanAngle , double startAngle)
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
        public virtual IEnumerable<PointF> DrawPie(Brush<Representation> p , int x0 , int y0 , double w , double h , double spanAngle , double startAngle)
        {
            PointF[ ] poly2 = GetPiePolygon(x0 , y0 , w,h , spanAngle , startAngle);

            DrawPolygon(p , poly2);

            return poly2;
        }
        public IEnumerable<PointF> DrawPie(Brush<Representation> p , Point pt , Size s , double spanAngle , double startAngle)
        {
            return DrawPie(p , pt.X , pt.Y , s.Width , s.Height , spanAngle , startAngle);
        }
        public IEnumerable<PointF> DrawPie(Brush<Representation> p , Point pt , Size s , double spanAngle , double startAngle , int thickness)
        {
            return DrawPie(p , pt.X , pt.Y , s.Width , s.Height , spanAngle , startAngle , thickness);
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
        public virtual IEnumerable<PointF> DrawPie(Representation p , int x0 , int y0 , double r , double spanAngle , double startAngle , int thickness)
        {
            if ( thickness <= 1 )
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
        public virtual IEnumerable<PointF> DrawPie(Brush<Representation> p , int x0 , int y0 , double r , double spanAngle , double startAngle , int thickness)
        {
            if ( thickness <= 1 )
                return DrawPie(p , x0 , y0 , r , spanAngle , startAngle);
            else
            {
                PointF[ ] poly2 = GetPiePolygon(x0 , y0 , r,r , spanAngle , startAngle);
                DrawPolygon(p , thickness , poly2);
                return poly2;
            }
        }
        public IEnumerable<PointF> DrawPie(Brush<Representation> p , Point pt , double r , double spanAngle , double startAngle , int thickness)
        {
            return DrawPie(p , pt.X , pt.Y , r , spanAngle , startAngle , thickness);
        }
        /// <summary>
        /// Draw pie
        /// </summary>
        /// <param name="p">The color</param>
        /// <param name="left">X center of circle</param>
        /// <param name="top">Y center of circle</param>
        /// <param name="w">Width</param>
        /// <param name="h">Height</param>
        /// <param name="spanAngle">Span of angle</param>
        /// <param name="startAngle">Start angle</param>
        /// <param name="thickness">Thickness of outline</param>
        /// <returns>Polygon describing the pie</returns>
        public virtual IEnumerable<PointF> DrawPie(Representation p , int left , int top , double w , double h , double spanAngle , double startAngle , int thickness)
        {
            if ( thickness <= 1 )
                return DrawPie(p , left , top , w, h, spanAngle , startAngle);
            else
            {
                PointF[ ] poly2 = GetPiePolygon(left , top , w,h , spanAngle , startAngle);
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
        public virtual IEnumerable<PointF> DrawPie(Brush<Representation> p , int x0 , int y0 , double w , double h , double spanAngle , double startAngle , int thickness)
        {
            if ( thickness <= 1 )
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
        public virtual void FillPie(Representation p , int x0 , int y0 , double r , double spanAngle , double startAngle)
        {
            PointF[ ] poly2 = GetPiePolygon(x0 , y0 , r,r , spanAngle , startAngle);

            FillPolygon(p , poly2);
        }
        public void FillPie(Representation p , Point pt , double r , double spanAngle , double startAngle)
        {
            FillPie(p , pt.X , pt.Y , r , spanAngle , startAngle);
        }
        public void FillPie(Brush<Representation> p , Point pt , double r , double spanAngle , double startAngle)
        {
            FillPie(p , pt.X , pt.Y , r , spanAngle , startAngle);
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
        public virtual void FillPie(Brush<Representation> p , int x0 , int y0 , double r , double spanAngle , double startAngle)
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
        public virtual void FillPie(Representation p , int x0 , int y0 , double w , double h , double spanAngle , double startAngle)
        {
            PointF[ ] poly2 = GetPiePolygon(x0 , y0 , w,h , spanAngle , startAngle);

            FillPolygon(p , poly2);
        }
        public void FillPie(Representation p , Point pt , double w , double h , double spanAngle , double startAngle)
        {
            FillPie(p , pt.X , pt.Y , w , h , spanAngle , startAngle);
        }
        public void FillPie(Representation p , Point pt , SizeF h , double spanAngle , double startAngle)
        {
            FillPie(p , pt.X , pt.Y , h.Width,h.Height , spanAngle , startAngle);
        }
        public void FillPie(Brush<Representation> p , Point pt , double w , double h , double spanAngle , double startAngle)
        {
            FillPie(p , pt.X , pt.Y , w , h , spanAngle , startAngle);
        }
        public void FillPie(Brush<Representation> p , Point pt , SizeF h , double spanAngle , double startAngle)
        {
            FillPie(p , pt.X , pt.Y , h.Width , h.Height , spanAngle , startAngle);
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
        public virtual void FillPie(Brush<Representation> p , int x0 , int y0 , double w , double h , double spanAngle , double startAngle)
        {
            PointF[ ] poly2 = GetPiePolygon(x0 , y0 , w,h , spanAngle , startAngle);

            FillPolygon(p , poly2);
        }


        /// <summary>
        /// Set the pixel with specified color
        /// </summary>
        /// <param name="p">The color</param>
        /// <param name="x">X position</param>
        /// <param name="y">Y position</param>
        public void SetPixel(Representation p , Point pt)
        {
            SetPixel(p , pt.X , pt.Y);
        }
        public void SetPixel(Brush<Representation> p , Point pt)
        {
            SetPixel(p , pt.X , pt.Y);
        }
        /// <summary>
        /// Set the pixel with specified color
        /// </summary>
        /// <param name="p">The color</param>
        /// <param name="x">X position</param>
        /// <param name="y">Y position</param>
        public virtual void SetPixel(Representation p , double x , double y)
        {
            var p1 = new PointF(x,y);
            p1 = TransformUsingMatrix(p1);

            SetPixelWithoutTransform(p , p1.X , p1.Y);
        }
        /// <summary>
        /// Set the pixel with specified color
        /// </summary>
        /// <param name="p">The color</param>
        /// <param name="x">X position</param>
        /// <param name="y">Y position</param>
        public virtual void SetPixel(Brush<Representation> p , double x , double y)
        {
            var p1 = new PointF(x,y);
            p1 = TransformUsingMatrix(p1);

            SetPixelWithoutTransform(p.GetColor(( int ) x , ( int ) y) , p1.X , p1.Y);
        }
        /// <summary>
        /// Transform a point using the matrix and get the new point
        /// </summary>
        /// <param name="a"></param>
        /// <param name="transformationMatrix"></param>
        /// <returns></returns>
        public PointF TransformUsingMatrix(PointF a , Matrix transformationMatrix)
        {
            if ( PreTransformFunction != null )
                a = PreTransformFunction(a);
            var mt = transformationMatrix * new Matrix(new double[,]{{a.X},{a.Y},{ 1 } });

            a = new PointF(( double ) mt[0 , 0] / mt[2 , 0] , ( double ) mt[1 , 0] / mt[2 , 0]);
            if ( PostTransformFunction != null )
                a = PostTransformFunction(a);
            return a;
        }
        /// <summary>
        /// Transform a point using the matrix and get the new point
        /// </summary>
        /// <param name="a"></param>
        /// <returns></returns>
        public PointF TransformUsingMatrix(PointF a)
        {
            if ( PreTransformFunction != null )
                a = PreTransformFunction(a);
            var mt = transformationMatrix * new Matrix(new double[,]{{a.X},{a.Y},{ 1 } });
            a = new PointF(( double ) mt[0 , 0] / mt[2 , 0] , ( double ) mt[1 , 0] / mt[2 , 0]);
            if ( PostTransformFunction != null )
                a = PostTransformFunction(a);
            return a;
        }
        /// <summary>
        /// Used to transform point using transforamtion matrix
        /// </summary>
        /// <param name="x">X position</param>
        /// <param name="y">Y position</param>
        /// <returns>Transformed point</returns>
        private PointF TransformUsingMatrix(double x , double y)
        {
            return TransformUsingMatrix(new PointF(x , y));
        }
        /// <summary>
        /// Transform a point using the matrix and get the new point
        /// </summary>
        /// <param name="a"></param>
        /// <returns></returns>
        public Point TransformUsingMatrix(Point a)
        {
            if ( PreTransformFunction != null )
                a = ( Point ) PreTransformFunction(a);
            var mt = transformationMatrix * new Matrix(new double[,]{{a.X},{a.Y},{ 1 } });
            a = ( Point ) new PointF(( double ) mt[0 , 0] / mt[2 , 0] , ( double ) mt[1 , 0] / mt[2 , 0]);
            if ( PostTransformFunction != null )
                a = ( Point ) PostTransformFunction(a);
            return a;
        }
        /// <summary>
        /// Used to transform point using transforamtion matrix
        /// </summary>
        /// <param name="x">X position</param>
        /// <param name="y">Y position</param>
        /// <returns>Transformed point</returns>
        private Point TransformUsingMatrix(int x , int y)
        {
            return TransformUsingMatrix(new Point(x , y));
        }
        /// <summary>
        /// Write specifie text in bitmap
        /// </summary>
        /// <param name="str">Text to write</param>
        /// <param name="p">The color</param>
        /// <param name="ox">X origin</param>
        /// <param name="oy">Y origin</param>
        /// <param name="f">The Font</param>
        /// <param name="size">The Font Size</param>
        /// <param name="sf">[optional]String format(top left if missing)</param>
        public void DrawString(string str , Representation p , int ox , int oy , Font f , int size , StringFormat sf = null)
        {
            DrawString(str , p , ox , oy , f , ( float ) size , sf);
        }
        /// <summary>
        /// Write specifie text in bitmap
        /// </summary>
        /// <param name="str">Text to write</param>
        /// <param name="p">The color</param>
        /// <param name="ox">X origin</param>
        /// <param name="oy">Y origin</param>
        /// <param name="f">The Font</param>
        /// <param name="size">The Font Size</param>
        /// <param name="sf">[optional]String format(top left if missing)</param>

        public virtual void DrawString(string str , Representation p , int ox , int oy , Font f , float size , StringFormat sf = null)
        {
            var PixelColor = ColorConvert.Convert<Representation,Pixel>(p);
            float x=ox,y=oy;
            if ( sf == null || sf.Alignment == StringAlignment.Near && sf.LineAlignment == StringAlignment.Near )
            {
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
                        x += DrawFormattedCharInternalF(character , x , y , new Text.FormattedChar() { Color = new SolidBrush(PixelColor) , Size = size });
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
        public virtual void DrawString(string text , FontSizeF font , Representation foreColor , Rectangle textBound , StringFormat textAlign)
        {
            DrawString(text , font , new SolidBrush<Representation>(foreColor) , textBound , textAlign);
        }
        public virtual void DrawString(string text , FontSizeF font , Brush<Representation> color , Rectangle area , StringFormat sf)
        {
            var PixelColor = ColorConvert.Convert<Representation,Pixel>(color);

            text = GetTextToDisplayInRect(text , font , sf , area);
            var textSize = MeasureStringImageProcessing(text,font.Font,font.Size);
            var oclip = clip;
            var oclipPolygon = clipPolygon;
            clip = area;
            clipPolygon = null;
            float x = area.X;
            float y =area.Y;
            var f= font.Font;

            if ( sf.Alignment == StringAlignment.Center )
                x = ( int ) ( ( area.X + area.Width / 2 ) - textSize.Width / 2 );
            else if ( sf.Alignment == StringAlignment.Far )
                x = ( int ) ( area.Right - textSize.Width );

            if ( sf.LineAlignment == StringAlignment.Center )
                y = ( int ) ( ( area.Y + area.Height / 2 ) - textSize.Height / 2 );
            else if ( sf.LineAlignment == StringAlignment.Far )
                y = ( int ) ( area.Bottom - textSize.Height );


            var size = font.Size;
            float ox=x;
            float oy=y;

            var lines = text.Split((char)10);
            foreach ( var line in lines )
            {
                var lineMeasure= MeasureStringImageProcessing(line,f,font.Size);
                x = ox;
                if ( sf.Alignment == StringAlignment.Center )
                    x = ( int ) ( ( area.X + area.Width / 2 ) - lineMeasure.Width / 2 );
                else if ( sf.Alignment == StringAlignment.Far )
                    x = ( int ) ( area.Right - lineMeasure.Width );

                foreach ( char c in line )
                {
                    bool[,] character = f.GetChar(c);
                    x += DrawFormattedCharInternalF(character , x , y , new Text.FormattedChar() { Color = PixelColor , Size = size });
                }
                y += lineMeasure.Height;
            }

            clip = oclip;
            clipPolygon = oclipPolygon;
        }
        private string GetTextToDisplayInRect(string text , FontSizeF font , StringFormat format , Rectangle area)
        {
            if ( text == null )
                return text;
            if ( format.EllipsisMode == EllipsisMode.None )
                return text;
            if ( format.EllipsisMode == EllipsisMode.Word )
            {
                int textPos=0;
                char separator=' ';
                char[] separators = new char[]{' ','.',',',';',':','[',']','-','!','+','*','/','\\','<','>','?','\'','"','{','}','~','(',')'};
                bool endText=false;
                string token=string.Empty;
                string getToken()
                {
                    token = string.Empty;
                    while ( textPos < text.Length && !separators.Contains(text[textPos]) )
                    {
                        token += text[textPos++];
                    }
                    if ( textPos == text.Length )
                        endText = true;
                    else
                        separator = text[textPos++];
                    return token;
                }
                var currentLine="";
                var line=0;
                getToken();
                List<string> lines=new List<string>();
                if ( endText )
                    return token;
                while ( !endText )
                {
                    do
                    {
                        currentLine += token + separator;
                        getToken();
                    } while ( !endText && MeasureStringImageProcessing(currentLine + token + separator , font.Font , font.Size).Width < area.Width );
                    lines.Add(currentLine);
                    currentLine = "";
                    line++;
                }
                return string.Join("\r\n" , lines);
            }
            else if ( format.EllipsisMode == EllipsisMode.Character )
            {
                int textPos=0;
                var currentLine="";
                List<string> lines=new List<string>();
                while ( textPos < text.Length )
                {
                    var token= text[textPos++];
                    if (/*Character fit on line*/MeasureStringImageProcessing(currentLine + token , font.Font , font.Size).Width < area.Width || currentLine.Length <= 1 )
                    {
                        currentLine += token;
                    }
                    else
                    {
                        lines.Add(currentLine);
                        currentLine = token.ToString();
                    }
                }
                if ( currentLine != null )
                    lines.Add(currentLine);
                return string.Join("\r\n" , lines);
            }
            return null;
        }
        /// <summary>
        /// Write specifie text in bitmap
        /// </summary>
        /// <param name="str">Text to write</param>
        /// <param name="p">The color</param>
        /// <param name="ox">X origin</param>
        /// <param name="oy">Y origin</param>
        /// <param name="f">The Font</param>
        /// <param name="size">The Font Size</param>
        /// <param name="sf">[optional]String format(top left if missing)</param>
        public void DrawString(string str , Brush<Representation> p , int ox , int oy , Font f , int size , StringFormat sf = null)
        {
            DrawString(str , p , ox , oy , f , ( float ) size , sf);
        }
        /// <summary>
        /// Write specifie text in bitmap
        /// </summary>
        /// <param name="str">Text to write</param>
        /// <param name="p">The color</param>
        /// <param name="ox">X origin</param>
        /// <param name="oy">Y origin</param>
        /// <param name="f">The Font</param>
        /// <param name="size">The Font Size</param>
        /// <param name="sf">[optional]String format(top left if missing)</param>
        public virtual void DrawString(string str , Brush<Representation> p , int ox , int oy , Font f , float size , StringFormat sf = null)
        {
            var PixelColor = ColorConvert.Convert<Representation,Pixel>(p);
            float x=ox,y=oy;
            if ( sf == null || sf.Alignment == StringAlignment.Near && sf.LineAlignment == StringAlignment.Near )
            {
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
                        x += DrawFormattedCharInternalF(character , x , y , new Text.FormattedChar() { Color = ( PixelColor ) , Size = size });
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
        public void DrawString(string str , FontSize fs , Representation p , int x , int y)
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
        public void DrawString(string str , FontSizeF fs , Representation p , int x , int y)
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
        public void DrawString(string str , FontSize fs , Brush<Representation> p , int x , int y)
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
        public void DrawString(string str , FontSizeF fs , Brush<Representation> p , int x , int y)
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
        public void DrawString(string str , FontSize fs , Representation p , PointF pt)
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
        public void DrawString(string str , FontSizeF fs , Representation p , PointF pt)
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
        public void DrawString(string str , FontSize fs , Brush<Representation> p , PointF pt)
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
        public void DrawString(string str , FontSizeF fs , Brush<Representation> p , PointF pt)
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
        public void DrawString(string str , FontSize fs , Representation p , PointF pt , StringFormat sf)
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
        public void DrawString(string str , FontSizeF fs , Representation p , PointF pt , StringFormat sf)
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
        public void DrawString(string str , FontSize fs , Brush<Representation> p , PointF pt , StringFormat sf)
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
        public void DrawString(string str , FontSizeF fs , Brush<Representation> p , PointF pt , StringFormat sf)
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
        public void DrawString(string str , Representation p , PointF pt , Font f , int size)
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
        public void DrawString(string str , Representation p , PointF pt , Font f , float size)
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
        public void DrawString(string str , Brush<Representation> p , PointF pt , Font f , int size)
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
        public void DrawString(string str , Brush<Representation> p , PointF pt , Font f , float size)
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
        private int DrawCharInternal(bool[ , ] character , Representation p , int x , int y , int size)
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
        private int DrawCharInternal(bool[ , ] character , Brush<Representation> p , int x , int y , int size)
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
        /// Measure space used for specified string
        /// </summary>
        /// <param name="str">Text</param>
        /// <param name="f">Font</param>
        /// <param name="size">Font size</param>
        /// <returns>Width and Height</returns>
        public virtual StringMeasurement MeasureString(String str , Font f , float size)
        {
            return MeasureStringImageProcessing(str , f , size);
        }
        /// <summary>
        /// Measure space used for specified string
        /// </summary>
        /// <param name="str">Text</param>
        /// <param name="f">Font</param>
        /// <param name="size">Font size</param>
        /// <returns>Width and Height</returns>
        private static StringMeasurement MeasureStringImageProcessing(string str , Font f , float size)
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

        /// <summary>
        /// Draw rotated ellipse
        /// </summary>
        /// <param name="b"></param>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <param name="major"></param>
        /// <param name="minor"></param>
        /// <param name="angle"></param>
        /// <param name="thickness"></param>
        /// <param name="angleIncrement"></param>
        public virtual void DrawRotatedEllipse(Representation b , double x , double y , double major , double minor , double angle , int thickness = 0 , double angleIncrement = 0)
        {
            DrawRotatedEllipse(new SolidBrush<Representation>(b) , x , y , major , minor , angle , thickness , angleIncrement);
        }
        /// <summary>
        /// Draw rotated ellipse
        /// </summary>
        /// <param name="b"></param>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <param name="major"></param>
        /// <param name="minor"></param>
        /// <param name="angle"></param>
        /// <param name="thickness"></param>
        /// <param name="angleIncrement"></param>
        public virtual void DrawRotatedEllipse(Brush<Representation> b , double x , double y , double major , double minor , double angle , int thickness = 0 , double angleIncrement = 0)
        {
            IEnumerable<PointF> pts = GetRotatedEllipsePolygon(x , y , major , minor , angle , angleIncrement);
            DrawPolygon(b , thickness , pts);
        }



        /// <summary>
        /// Fill rotated ellipse
        /// </summary>
        /// <param name="b"></param>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <param name="major"></param>
        /// <param name="minor"></param>
        /// <param name="angle"></param>
        /// <param name="angleIncrement"></param>
        public virtual void FillRotatedEllipse(Representation b , double x , double y , double major , double minor , double angle , double angleIncrement = 0)
        {
            FillRotatedEllipse(new SolidBrush<Representation>(b) , x , y , major , minor , angle , angleIncrement);
        }
        /// <summary>
        /// Fill rotated ellipse
        /// </summary>
        /// <param name="b"></param>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <param name="major"></param>
        /// <param name="minor"></param>
        /// <param name="angle"></param>
        /// <param name="angleIncrement"></param>
        public virtual void FillRotatedEllipse(Brush<Representation> b , double x , double y , double major , double minor , double angle , double angleIncrement = 0)
        {
            IEnumerable<PointF> pts = GetRotatedEllipsePolygon(x , y , major , minor , angle , angleIncrement);
            FillPolygon(b , pts.ToArray());
        }
        #region Rounded Rectangle
        public void DrawRoundedRectangle(Representation p , double x , double y , double width , double height , double radius)
        {
            DrawPolygon(p , GetRoundedRectanglePolygon(new RectangleF(x , y , width , height) , radius));
        }
        public void DrawRoundedRectangle(Representation p , int thickness , double x , double y , double width , double height , double radius)
        {
            DrawPolygon(p , thickness , GetRoundedRectanglePolygon(new RectangleF(x , y , width , height) , radius));
        }
        public void DrawRoundedRectangle(Brush<Representation> p , double x , double y , double width , double height , double radius)
        {
            DrawPolygon(p , GetRoundedRectanglePolygon(new RectangleF(x , y , width , height) , radius));
        }
        public void DrawRoundedRectangle(Brush<Representation> p , int thickness , double x , double y , double width , double height , double radius)
        {
            DrawPolygon(p , thickness , GetRoundedRectanglePolygon(new RectangleF(x , y , width , height) , radius));
        }
        public void FillRoundedRectangle(Representation p , double x , double y , double width , double height , double radius)
        {
            FillPolygon(p , GetRoundedRectanglePolygon(new RectangleF(x , y , width , height) , radius));
        }
        public void FillRoundedRectangle(Brush<Representation> p , double x , double y , double width , double height , double radius)
        {
            FillPolygon(p , GetRoundedRectanglePolygon(new RectangleF(x , y , width , height) , radius));
        }
        #endregion
        #region RoundedPoint
        public void FillRoundedRectangle(Representation p , PointF pt , double width , double height , double radius)
        {
            FillRoundedRectangle(p , pt.X , pt.Y , width , height , radius);
        }
        public void FillRoundedRectangle(Brush<Representation> p , PointF pt , double width , double height , double radius)
        {
            FillRoundedRectangle(p , pt.X , pt.Y , width , height , radius);
        }
        public void DrawRoundedRectangle(Representation p , PointF pt , double width , double height , double radius)
        {
            DrawRoundedRectangle(p , pt.X , pt.Y , width , height , radius);
        }
        public void DrawRoundedRectangle(Brush<Representation> p , PointF pt , double width , double height , double radius)
        {
            DrawRoundedRectangle(p , pt.X , pt.Y , width , height , radius);
        }
        public void DrawRoundedRectangle(Representation p , int t , PointF pt , double width , double height , double radius)
        {
            DrawRoundedRectangle(p , t , pt.X , pt.Y , width , height , radius);
        }
        public void DrawRoundedRectangle(Brush<Representation> p , int t , PointF pt , double width , double height , double radius)
        {
            DrawRoundedRectangle(p , t , pt.X , pt.Y , width , height , radius);
        }
        #endregion
        #region RoundedPoint +Size
        public void FillRoundedRectangle(Representation p , PointF pt , SizeF s , double radius)
        {
            FillRoundedRectangle(p , pt.X , pt.Y , s.Width , s.Height , radius);
        }
        public void FillRoundedRectangle(Brush<Representation> p , PointF pt , SizeF s , double radius)
        {
            FillRoundedRectangle(p , pt.X , pt.Y , s.Width , s.Height , radius);
        }
        public void DrawRoundedRectangle(Representation p , PointF pt , SizeF s , double radius)
        {
            DrawRoundedRectangle(p , pt.X , pt.Y , s.Width , s.Height , radius);
        }
        public void DrawRoundedRectangle(Brush<Representation> p , PointF pt , SizeF s , double radius)
        {
            DrawRoundedRectangle(p , pt.X , pt.Y , s.Width , s.Height , radius);
        }
        public void DrawRoundedRectangle(Representation p , int t , PointF pt , SizeF s , double radius)
        {
            DrawRoundedRectangle(p , t , pt.X , pt.Y , s.Width , s.Height , radius);
        }
        public void DrawRoundedRectangle(Brush<Representation> p , int t , PointF pt , SizeF s , double radius)
        {
            DrawRoundedRectangle(p , t , pt.X , pt.Y , s.Width , s.Height , radius);
        }
        #endregion
        #region Rounded Rectangle
        public void FillRoundedRectangle(Representation p , RectangleF r , double radius)
        {
            FillRoundedRectangle(p , r.X , r.Y , r.Width , r.Height , radius);
        }
        public void FillRoundedRectangle(Brush<Representation> p , RectangleF r , double radius)
        {
            FillRoundedRectangle(p , r.X , r.Y , r.Width , r.Height , radius);
        }
        public void DrawRoundedRectangle(Representation p , RectangleF r , double radius)
        {
            DrawRoundedRectangle(p , r.X , r.Y , r.Width , r.Height , radius);
        }
        public void DrawRoundedRectangle(Brush<Representation> p , RectangleF r , double radius)
        {
            DrawRoundedRectangle(p , r.X , r.Y , r.Width , r.Height , radius);
        }
        public void DrawRoundedRectangle(Representation p , int t , RectangleF r , double radius)
        {
            DrawRoundedRectangle(p , t , r.X , r.Y , r.Width , r.Height , radius);
        }
        public void DrawRoundedRectangle(Brush<Representation> p , int t , RectangleF r , double radius)
        {
            DrawRoundedRectangle(p , t , r.X , r.Y , r.Width , r.Height , radius);
        }
        #endregion
        public void DrawArc(
            Representation pen ,
            double x ,
            double y ,
            double width ,
            double height ,
            double startAngle ,
            double sweepAngle
        )
        {
            DrawPath(pen , GetEllipseArcPolygon(new PointF(x + width / 2 , y + height / 2) , width / 2 , height / 2 , startAngle , sweepAngle));
        }
        public void DrawArc(
           Brush<Representation> pen ,
           double x ,
           double y ,
           double width ,
           double height ,
           double startAngle ,
           double sweepAngle
       )
        {
            DrawPath(pen , GetEllipseArcPolygon(new PointF(x + width / 2 , y + height / 2) , width / 2 , height / 2 , startAngle , sweepAngle));
        }

        #region Transform
        /// <summary>
        /// Create matrix to project image from 4 points to 4 points
        /// </summary>
        /// <param name="src">4 points</param>
        /// <param name="dst">4 points</param>
        public virtual void ProjectTransform(PointF[ ] src , PointF[ ] dst)
        {
           
            Matrix A = new Matrix(new double[,]{
                { src[0].X,src[0].Y,1,0,0,0,-src[0].X*dst[0].X,-src[0].X*dst[0].Y },
                { 0,0,0,src[0].X,src[0].Y,1,-src[0].Y*dst[0].X,-src[0].Y*dst[0].Y },
                 { src[1].X,src[1].Y,1,0,0,0,-src[1].X*dst[1].X,-src[1].X*dst[1].Y },
                { 0,0,0,src[1].X,src[1].Y,1,-src[1].Y*dst[1].X,-src[1].Y*dst[1].Y },
                 { src[2].X,src[2].Y,1,0,0,0,-src[2].X*dst[2].X,-src[2].X*dst[2].Y },
                { 0,0,0,src[2].X,src[2].Y,1,-src[2].Y*dst[2].X,-src[2].Y*dst[2].Y },
                 { src[3].X,src[3].Y,1,0,0,0,-src[3].X*dst[3].X,-src[3].X*dst[3].Y },
                { 0,0,0,src[3].X,src[3].Y,1,-src[3].Y*dst[3].X,-src[3].Y*dst[3].Y }
            });
            Matrix B = new Matrix(new double[,]{ { dst[0].X },{ dst[0].Y },{ dst[1].X },{ dst[1].Y },{ dst[2].X },{ dst[2].Y },{ dst[3].X },{ dst[3].Y }  });
            try
            {
                //var h = (A.Inverted * B).GetColumn(0).ToArray();

                var gauss= A.Augment(B).Gauss();

                double findValue(int pos)
                {
                    for ( var l = 0; l < gauss.Rows; l++ )
                    {
                        var c = pos;
                        {
                            if ( gauss[l , c] == 1 )
                                return gauss[l , gauss.Columns - 1];
                        }
                    }
                    return 0;
                }
                double[] h = new double[]{findValue(0),findValue(1),findValue(2),findValue(3),findValue(4),findValue(5),findValue(6),findValue(7)};

              

                var H = new Matrix(3,3).Feed(h);
                /*H[0 , 0] = h[0];
                H[0 , 1] = h[1];
                H[0 , 2] = h[2];
                H[1 , 0] = h[3];
                H[1 , 1] = h[4];
                H[1 , 2] = h[5];
                H[2 , 0] = h[6];
                H[2 , 1] = h[7];*/
                H[2 , 2] = 1;

                var M3D = H.InsertRow(2,0,0,0).InsertColumn(2,0,0,1,0);

                string dbg = ("matrix3d("+string.Join(",",M3D.AllValuesByRow().Select((X)=>X.ToString()))+")");
                string dbg2 = ("matrix3d("+string.Join(",",M3D.AllValuesByColumn().Select((X)=>X.ToString()))+")");

                //try
                {
                    PointF[] r = new PointF[4];
                    for ( var j = 0; j < 4; j++ )
                    {
                        r[j] = TransformUsingMatrix(src[0] , H);

                        if ( System.Math.Abs(dst[j].X - r[j].X) > 2 )
                        {
                            int shouldNotGetHere=2;
                        }
                        if ( System.Math.Abs(dst[j].Y - r[j].Y) > 2 )
                        {
                            int shouldNotGetHere=2;
                        }
                    }
                }
                transformationMatrix = transformationMatrix * H;
                int i=6;
            }
            catch
            {
            }
        }
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
            mt[1 , 1] = System.Math.Cos(angle);
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
            PreTransformFunction = null;
            PostTransformFunction = null;
        }
        /// <summary>
        /// Allow representation as another type
        /// </summary>
        /// <typeparam name="NewRepresentation"></typeparam>
        /// <returns></returns>
        public TypeProxyGraphics<NewRepresentation , Representation> As<NewRepresentation>() where NewRepresentation : unmanaged
        {
            return new TypeProxyGraphics<NewRepresentation , Representation>(this , ColorConvert.GetConversionFrom<NewRepresentation , Representation>());
        }
        /// <summary>
        /// Fill a specified Rectangle
        /// </summary>
        /// <param name="p"></param>
        /// <param name="r"></param>
        public void FillRectangle(Brush<Representation> p , RectangleF r)
        {
            FillRectangle(p , r.X , r.Y , r.Width , r.Height);
        }
        public void FillRectangle(Brush<Representation> p , PointF pt,SizeF s)
        {
            FillRectangle(p , pt.X , pt.Y , s.Width , s.Height);
        }
        /// <summary>
        /// Fill a specified Rectangle
        /// </summary>
        /// <param name="p"></param>
        /// <param name="r"></param>
        public void FillRectangle(Representation p , RectangleF r)
        {
            FillRectangle(p , r.X , r.Y , r.Width , r.Height);
        }
        public void FillRectangle(Representation p , PointF pt,SizeF s)
        {
            FillRectangle(p , pt.X , pt.Y , s.Width , s.Height);
        }
        #endregion
        /// <summary>
        /// Allow to obtain Representation as specified position
        /// </summary>
        /// <param name="m"></param>
        /// <returns></returns>
        protected virtual Representation this[int m]
        {
            get
            {
                if ( bmp != null )
                    return bmp[m];
                else if ( proxy != null )
                    return proxy[m];
                else if ( gProxy != null )
                    return gProxy[m];
                return default(Representation);
            }
            set
            {
                if ( bmp != null )
                    bmp[m] = value;
                else if ( proxy != null )
                    proxy[m] = value;
                else if ( gProxy != null )
                    gProxy.SetPixelWithoutTransform(value , m % width.Value + this.x.Value , m / width.Value + this.y.Value , false);
            }
        }
        /// <summary>
        /// Allow to obtain Representation as specified position
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <returns></returns>
        protected virtual Representation this[int x , int y]
        {
            get
            {
                if ( bmp != null )
                    return bmp[x , y];
                else if ( proxy != null )
                    return proxy[x , y];
                else if ( gProxy != null )
                    return gProxy[x , y];
                return default(Representation);
            }
            set
            {
                if ( bmp != null )
                    bmp[x , y] = value;
                else if ( proxy != null )
                    proxy[x , y] = value;
                else if ( gProxy != null )
                    gProxy.SetPixelWithoutTransform(value , x + this.x.Value , y + this.y.Value , false);
            }
        }
    }
}


