using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MoyskleyTech.ImageProcessing.Image;
namespace MoyskleyTech.ImageProcessing.Recognition
{
    public static class ConnexMatching
    {
        public static void Match4Connex(this Bitmap bmp , int x , int y , Func<Pixel , bool> condition , Action<Point , Pixel> action)
        {
            Stack<Point> points = new Stack<Point>();
            points.Push(new Point(x , y));
            HashSet<Point> visited = new HashSet<Point>();
            while ( points.Any() )
            {
                Point p = points.Pop();
                if ( !visited.Contains(p) )
                {
                    visited.Add(p);
                    var px=bmp[p.X,p.Y];
                    if ( condition(px) )
                    {
                        action(p , px);
                        if ( p.X > 0 )
                            points.Push(new Point(p.X - 1 , p.Y));
                        if ( p.Y > 0 )
                            points.Push(new Point(p.X , p.Y - 1));
                        if ( p.X < bmp.Width - 1 )
                            points.Push(new Point(p.X + 1 , p.Y));
                        if ( p.Y < bmp.Height - 1 )
                            points.Push(new Point(p.X , p.Y + 1));
                    }
                }
            }
        }
        public static void Match8Connex(this Bitmap bmp , int x , int y , Func<Pixel , bool> condition , Action<Point , Pixel> action)
        {
            Stack<Point> points = new Stack<Point>();
            points.Push(new Point(x , y));
            HashSet<Point> visited = new HashSet<Point>();
            while ( points.Any() )
            {
                Point p = points.Pop();
                if ( !visited.Contains(p) )
                {
                    visited.Add(p);
                    var px=bmp[p.X,p.Y];
                    if ( condition(px) )
                    {
                        action(p , px);
                        if ( p.X > 0 )
                            points.Push(new Point(p.X - 1 , p.Y));
                        if ( p.Y > 0 )
                            points.Push(new Point(p.X , p.Y - 1));
                        if ( p.X < bmp.Width - 1 )
                            points.Push(new Point(p.X + 1 , p.Y));
                        if ( p.Y < bmp.Height - 1 )
                            points.Push(new Point(p.X , p.Y + 1));

                        if ( p.X > 0 && p.Y > 0 )
                            points.Push(new Point(p.X - 1 , p.Y-1));
                        if ( p.X < bmp.Width - 1 && p.Y > 0 )
                            points.Push(new Point(p.X+1 , p.Y - 1));

                        if ( p.X > 0 && p.Y < bmp.Height - 1 )
                            points.Push(new Point(p.X - 1 , p.Y + 1));
                        if ( p.X < bmp.Width - 1 && p.Y < bmp.Height - 1 )
                            points.Push(new Point(p.X + 1 , p.Y + 1));
                    }
                }
            }
        }
        public static List<Point> Match4ConnexList(this Bitmap bmp , int x , int y , Func<Pixel , bool> condition)
        {
            List<Point> output = new List<Point>();
            bmp.Match4Connex(x , y , condition , (pt , px) => { output.Add(pt); });
            return output;
        }
        public static List<Point> Match8ConnexList(this Bitmap bmp , int x , int y , Func<Pixel , bool> condition)
        {
            List<Point> output = new List<Point>();
            bmp.Match8Connex(x , y , condition , (pt , px) => { output.Add(pt); });
            return output;
        }


        public static void Match4Connex<Representation>(this Image<Representation> bmp , int x , int y , Func<Representation , bool> condition , Action<Point , Representation> action)
            where Representation:struct
        {
            Stack<Point> points = new Stack<Point>();
            points.Push(new Point(x , y));
            HashSet<Point> visited = new HashSet<Point>();
            while ( points.Any() )
            {
                Point p = points.Pop();
                if ( !visited.Contains(p) )
                {
                    visited.Add(p);
                    var px=bmp[p.X,p.Y];
                    if ( condition(px) )
                    {
                        action(p , px);
                        if ( p.X > 0 )
                            points.Push(new Point(p.X - 1 , p.Y));
                        if ( p.Y > 0 )
                            points.Push(new Point(p.X , p.Y - 1));
                        if ( p.X < bmp.Width - 1 )
                            points.Push(new Point(p.X + 1 , p.Y));
                        if ( p.Y < bmp.Height - 1 )
                            points.Push(new Point(p.X , p.Y + 1));
                    }
                }
            }
        }
        public static void Match8Connex<Representation>(this Image<Representation> bmp , int x , int y , Func<Representation , bool> condition , Action<Point , Representation> action)
            where Representation:struct
        {
            Stack<Point> points = new Stack<Point>();
            points.Push(new Point(x , y));
            HashSet<Point> visited = new HashSet<Point>();
            while ( points.Any() )
            {
                Point p = points.Pop();
                if ( !visited.Contains(p) )
                {
                    visited.Add(p);
                    var px=bmp[p.X,p.Y];
                    if ( condition(px) )
                    {
                        action(p , px);
                        if ( p.X > 0 )
                            points.Push(new Point(p.X - 1 , p.Y));
                        if ( p.Y > 0 )
                            points.Push(new Point(p.X , p.Y - 1));
                        if ( p.X < bmp.Width - 1 )
                            points.Push(new Point(p.X + 1 , p.Y));
                        if ( p.Y < bmp.Height - 1 )
                            points.Push(new Point(p.X , p.Y + 1));

                        if ( p.X > 0 && p.Y > 0 )
                            points.Push(new Point(p.X - 1 , p.Y - 1));
                        if ( p.X < bmp.Width - 1 && p.Y > 0 )
                            points.Push(new Point(p.X + 1 , p.Y - 1));

                        if ( p.X > 0 && p.Y < bmp.Height - 1 )
                            points.Push(new Point(p.X - 1 , p.Y + 1));
                        if ( p.X < bmp.Width - 1 && p.Y < bmp.Height - 1 )
                            points.Push(new Point(p.X + 1 , p.Y + 1));
                    }
                }
            }
        }
        public static List<Point> Match4ConnexList<Representation>(this Image<Representation> bmp , int x , int y , Func<Representation , bool> condition)
            where Representation:struct
        {
            List<Point> output = new List<Point>();
            bmp.Match4Connex(x , y , condition , (pt , px) => { output.Add(pt); });
            return output;
        }
        public static List<Point> Match8ConnexList<Representation>(this Image<Representation> bmp , int x , int y , Func<Representation , bool> condition)
            where Representation:struct
        {
            List<Point> output = new List<Point>();
            bmp.Match8Connex(x , y , condition , (pt , px) => { output.Add(pt); });
            return output;
        }

        public static void Match8Connex(this bool[,] bmp , int x , int y , Action<Point> action)
        {
            Stack<Point> points = new Stack<Point>();
            points.Push(new Point(x , y));
            HashSet<Point> visited = new HashSet<Point>();
            int width = bmp.GetLength(0);
            int height = bmp.GetLength(1);
            while ( points.Any() )
            {
                Point p = points.Pop();
                if ( !visited.Contains(p) )
                {
                    visited.Add(p);
                    var px=bmp[p.X,p.Y];
                    if ( px )
                    {
                        action(p);
                        if ( p.X > 0 )
                            points.Push(new Point(p.X - 1 , p.Y));
                        if ( p.Y > 0 )
                            points.Push(new Point(p.X , p.Y - 1));
                        if ( p.X < width - 1 )
                            points.Push(new Point(p.X + 1 , p.Y));
                        if ( p.Y < height - 1 )
                            points.Push(new Point(p.X , p.Y + 1));

                        if ( p.X > 0 && p.Y > 0 )
                            points.Push(new Point(p.X - 1 , p.Y - 1));
                        if ( p.X < width - 1 && p.Y > 0 )
                            points.Push(new Point(p.X + 1 , p.Y - 1));

                        if ( p.X > 0 && p.Y < height - 1 )
                            points.Push(new Point(p.X - 1 , p.Y + 1));
                        if ( p.X < width - 1 && p.Y < height - 1 )
                            points.Push(new Point(p.X + 1 , p.Y + 1));
                    }
                }
            }
        }
        public static void Match4Connex(this bool[ , ] bmp , int x , int y , Action<Point> action)
        {
            Stack<Point> points = new Stack<Point>();
            points.Push(new Point(x , y));
            HashSet<Point> visited = new HashSet<Point>();
            int width = bmp.GetLength(0);
            int height = bmp.GetLength(1);
            while ( points.Any() )
            {
                Point p = points.Pop();
                if ( !visited.Contains(p) )
                {
                    visited.Add(p);
                    var px=bmp[p.X,p.Y];
                    if ( px )
                    {
                        action(p);
                        if ( p.X > 0 )
                            points.Push(new Point(p.X - 1 , p.Y));
                        if ( p.Y > 0 )
                            points.Push(new Point(p.X , p.Y - 1));
                        if ( p.X < width - 1 )
                            points.Push(new Point(p.X + 1 , p.Y));
                        if ( p.Y < height - 1 )
                            points.Push(new Point(p.X , p.Y + 1));
                    }
                }
            }
        }
        public static List<Point> Match4ConnexList(this bool[ , ] bmp , int x , int y )
        {
            List<Point> output = new List<Point>();
            bmp.Match4Connex(x , y , (pt) => { output.Add(pt); });
            return output;
        }
        public static List<Point> Match8ConnexList(this bool[ , ] bmp , int x , int y )
        {
            List<Point> output = new List<Point>();
            bmp.Match8Connex(x , y , (pt) => { output.Add(pt); });
            return output;
        }
    }
}
