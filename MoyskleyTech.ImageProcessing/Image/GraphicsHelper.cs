using MoyskleyTech.ImageProcessing.Image.Helper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MoyskleyTech.ImageProcessing.Image
{
    /// <summary>
    /// Helper functions
    /// </summary>
    public static class GraphicsHelper
    {
        /// <summary>
        /// Get the polygon associated with the rotated ellipse
        /// </summary>
        /// <param name="x">Center X</param>
        /// <param name="y">Center Y</param>
        /// <param name="major">Ellipse Major</param>
        /// <param name="minor">Ellipse Minor</param>
        /// <param name="angle">Angle of ellipse</param>
        /// <param name="angleIncrement">Angle increment</param>
        /// <returns></returns>
        public static IEnumerable<PointF> GetRotatedEllipsePolygon(double x , double y , double major , double minor , double angle , double angleIncrement = 0)
        {
            const double D_PI = System.Math.PI*2;
            if ( angleIncrement == 0 )
                angleIncrement = D_PI / major / minor;
            List<PointF> pts = new List<PointF>();
            for ( var i = 0d; i < D_PI; i += angleIncrement )
                pts.Add(FindEllipsePoint(major , minor , angle , x , y , i));
            return pts;
        }
        private static PointF FindEllipsePoint(double a , double b , double theta , double x , double y , double t)
        {
            Func<double,double> cos = System.Math.Cos;
            Func<double,double> sin = System.Math.Sin;
            double xt = x+ (a*cos(t)*cos(theta)-b*sin(t)*sin(theta));
            double yt = y+(a*cos(t)*sin(theta)+b*sin(t)*cos(theta));

            return new PointF(xt , yt);
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
        public static PointF[ ] GetPiePolygon(double x0 , double y0 , double w , double h , double spanAngle , double startAngle)
        {
            return GetEllipsePiePolygon(new PointF(x0 + w / 2 , y0 + h / 2) , w / 2 , h / 2 , startAngle , spanAngle);
        }
        /// <summary>
        /// Helper to get circle
        /// </summary>
        /// <param name="x0">X center</param>
        /// <param name="y0">Y center</param>
        /// <param name="r">Radius</param>
        /// <returns></returns>
        public static PointF[ ] GetCirclePolygon(double x0 , double y0 , double r)
        {
            return GetArcPolygon(new PointF(x0 , y0) , r , 0 , 2 * Math.PI);
            //LinkedList<PointF> points = new LinkedList<PointF>();

            //double f = 1 - r;
            //double ddF_x = 1;
            //double ddF_y = -2 * r;
            //double x = 0;
            //double y = r;

            //points.AddLast(new PointF(x0 , y0 + r));
            //points.AddLast(new PointF(x0 , y0 - r));
            //points.AddLast(new PointF(x0 + r , y0));
            //points.AddLast(new PointF(x0 - r , y0));

            //while ( x < y )
            //{
            //    if ( f >= 0 )
            //    {
            //        y--;
            //        ddF_y += 2;
            //        f += ddF_y;
            //    }
            //    x++;
            //    ddF_x += 2;
            //    f += ddF_x;

            //    points.AddLast(new PointF(x0 + x , y0 + y));
            //    points.AddLast(new PointF(x0 - x , y0 + y));
            //    points.AddLast(new PointF(x0 + x , y0 - y));
            //    points.AddLast(new PointF(x0 - x , y0 - y));
            //    points.AddLast(new PointF(x0 + y , y0 + x));
            //    points.AddLast(new PointF(x0 - y , y0 + x));
            //    points.AddLast(new PointF(x0 + y , y0 - x));
            //    points.AddLast(new PointF(x0 - y , y0 - x));
            //}

            //var part1 = (points.Where((p)=>p.Y>y0).OrderBy((p) => p.X));
            //var part2 = (points.Where((p)=>p.Y<=y0).OrderBy((p) => -p.X));
            //var ret = part2.Concat(part1).ToArray();


            //return ret;
        }
        /// <summary>
        /// Helper to get an ellipse polygon
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <param name="w"></param>
        /// <param name="h"></param>
        /// <returns></returns>
        public static PointF[ ] GetEllipsePolygon(double x , double y , double w , double h)
        {
            return GetEllipseArcPolygon(new PointF(x + w / 2 , y + y / 2) , w / 2 , y / 2 , 0 , 2 * Math.PI);
        }
        public static PointF[ ] GetRoundedRectanglePolygon(RectangleF bounds , double radius)
        {
            if ( radius == 0 )
                return new PointF[ ] { new PointF(bounds.X,bounds.Y), new PointF(bounds.Right , bounds.Y), new PointF(bounds.Right , bounds.Bottom), new PointF(bounds.X , bounds.Bottom) };
            const double HALF_PI = Math.PI/2;
            double INCREMENT = HALF_PI*4/radius;
            var ptTopLeft = new PointF(bounds.X+radius, bounds.Y+radius);
            var ptTopRight = new PointF(bounds.Right-radius,bounds.Y+radius);
            var ptBottomLeft = new PointF(bounds.X+radius, bounds.Bottom-radius);
            var ptBottomRight = new PointF(bounds.Right-radius,bounds.Bottom-radius);

            var points =new List<PointF>();

            points.AddRange(GetArcPolygon(ptTopRight , radius , 3 * HALF_PI , HALF_PI));
            points.AddRange(GetArcPolygon(ptBottomRight , radius , 0 , HALF_PI));
            points.AddRange(GetArcPolygon(ptBottomLeft , radius , HALF_PI , HALF_PI));
            points.AddRange(GetArcPolygon(ptTopLeft , radius , 2*HALF_PI , HALF_PI));

            return points.ToArray();
        }

        public static PointF[] GetArcPolygon(PointF center , double radius , double beginAngle , double span)
        {
            beginAngle %= 2 * Math.PI;
            var points =new List<PointF>();
            var INCREMENT = (span)/(Math.Max(radius,10));
            for ( var i = beginAngle; i < beginAngle+span; i += INCREMENT )
            {
                points.Add(FindEllipsePoint(radius , radius , 0 , center.X , center.Y , i));
            }
            points.Add(FindEllipsePoint(radius , radius , 0 , center.X , center.Y , beginAngle + span));
            return points.ToArray();
        }
        public static PointF[ ] GetEllipseArcPolygon(PointF center , double radius1,double radius2 , double beginAngle , double span)
        {
            beginAngle %= 2 * Math.PI;
            var points =new List<PointF>();
            var INCREMENT = (span)/(Math.Max(Math.Max(radius1,10),radius2));
            for ( var i = beginAngle; i < beginAngle + span; i += INCREMENT )
            {
                points.Add(FindEllipsePoint(radius1 , radius2 , 0 , center.X , center.Y , i));
            }
            points.Add(FindEllipsePoint(radius1 , radius2 , 0 , center.X , center.Y , beginAngle + span));
            return points.ToArray();
        }
        public static PointF[ ] GetPiePolygon(PointF center , double radius , double beginAngle , double span)
        {
            var points = GetArcPolygon(center,radius,beginAngle,span).ToList();
            points.Add(center);
            return points.ToArray();
        }
        public static PointF[ ] GetEllipsePiePolygon(PointF center , double radius1 , double radius2 , double beginAngle , double span)
        {
            var points = GetEllipseArcPolygon(center,radius1,radius2,beginAngle,span).ToList();
            points.Add(center);
            return points.ToArray();
        }

    }
}
