using MoyskleyTech.ImageProcessing.Image;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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


        public static void Match4Connex<Representation>(this ImageProxy<Representation> bmp , int x , int y , Func<Representation , bool> condition , Action<Point , Representation> action)
            where Representation : unmanaged
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
        public static void Match8Connex<Representation>(this ImageProxy<Representation> bmp , int x , int y , Func<Representation , bool> condition , Action<Point , Representation> action)
            where Representation : unmanaged
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
        public static void Match4ConnexLargeImage<Representation>(this ImageProxy<Representation> bmp , Image<bool> visited , int x , int y , Func<Representation , bool> condition , Action<Point , Representation> action)
            where Representation : unmanaged
        {
            Stack<Point> points = new Stack<Point>();
            points.Push(new Point(x , y));
            while ( points.Any() )
            {
                Point p = points.Pop();
                if ( !visited[p] )
                {
                    visited[p] = true;
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
        public static void Match8ConnexLargeImage<Representation>(this ImageProxy<Representation> bmp , Image<bool> visited , int x , int y , Func<Representation , bool> condition , Action<Point , Representation> action)
            where Representation : unmanaged
        {
            Stack<Point> points = new Stack<Point>();
            points.Push(new Point(x , y));
            while ( points.Any() )
            {
                Point p = points.Pop();
                if ( !visited[p] )
                {
                    visited[p] = true;
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
        public static void MatchSelector<Representation>(this ImageProxy<Representation> bmp , int x , int y , Func<Representation , bool> condition , Func<Point , Point[ ]> selector , Action<Point , Representation> action)
             where Representation : unmanaged
        {
            Stack<Point> points = new Stack<Point>();
            points.Push(new Point(x , y));
            HashSet<Point> visited = new HashSet<Point>();
            while ( points.Any() )
            {
                Point p = points.Pop();
                if ( p.X >= 0 && p.Y >= 0 && p.X < bmp.Width && p.Y < bmp.Height )
                    if ( !visited.Contains(p) )
                    {
                        visited.Add(p);
                        var px=bmp[p.X,p.Y];
                        if ( condition(px) )
                        {
                            action(p , px);

                            foreach ( var pt in selector(p) )
                                points.Push(pt);
                        }
                    }
            }
        }
        public static void MatchSelectorLargeImage<Representation>(this ImageProxy<Representation> bmp , Image<bool> visited , int x , int y , Func<Representation , bool> condition , Func<Point , Point[ ]> selector , Action<Point , Representation> action)
             where Representation : unmanaged
        {
            Stack<Point> points = new Stack<Point>();
            points.Push(new Point(x , y));
            while ( points.Any() )
            {
                Point p = points.Pop();
                if ( p.X >= 0 && p.Y >= 0 && p.X < bmp.Width && p.Y < bmp.Height )
                    if ( !visited[p] )
                    {
                        visited[p] = true;
                        var px=bmp[p.X,p.Y];
                        if ( condition(px) )
                        {
                            action(p , px);

                            foreach ( var pt in selector(p) )
                                points.Push(pt);
                        }
                    }
            }
        }
        public static List<Point> Match4ConnexList<Representation>(this ImageProxy<Representation> bmp , int x , int y , Func<Representation , bool> condition)
            where Representation : unmanaged
        {
            List<Point> output = new List<Point>();
            bmp.Match4Connex(x , y , condition , (pt , px) => { output.Add(pt); });
            return output;
        }
        public static List<Point> Match8ConnexList<Representation>(this ImageProxy<Representation> bmp , int x , int y , Func<Representation , bool> condition)
            where Representation : unmanaged
        {
            List<Point> output = new List<Point>();
            bmp.Match8Connex(x , y , condition , (pt , px) => { output.Add(pt); });
            return output;
        }
        public static void Match4ConnexMultiplePoints<Representation>(this ImageProxy<Representation> bmp , Point[ ] startPoints , Func<Representation , bool> condition , Action<Point , Representation> action)
       where Representation : unmanaged
        {
            Queue<Point> points = new Queue<Point>(startPoints);

            HashSet<Point> visited = new HashSet<Point>();
            while ( points.Any() )
            {
                Point p = points.Dequeue();
                if ( !visited.Contains(p) )
                {
                    visited.Add(p);
                    var px=bmp[p.X,p.Y];
                    if ( condition(px) )
                    {
                        action(p , px);
                        if ( p.X > 0 )
                            points.Enqueue(new Point(p.X - 1 , p.Y));
                        if ( p.Y > 0 )
                            points.Enqueue(new Point(p.X , p.Y - 1));
                        if ( p.X < bmp.Width - 1 )
                            points.Enqueue(new Point(p.X + 1 , p.Y));
                        if ( p.Y < bmp.Height - 1 )
                            points.Enqueue(new Point(p.X , p.Y + 1));
                    }
                }
            }
        }
        public static void Match8ConnexMultiplePoints<Representation>(this ImageProxy<Representation> bmp , Point[ ] startPoints , Func<Representation , bool> condition , Action<Point , Representation> action)
            where Representation : unmanaged
        {
            Queue<Point> points = new Queue<Point>(startPoints);
            HashSet<Point> visited = new HashSet<Point>();
            while ( points.Any() )
            {
                Point p = points.Dequeue();
                if ( !visited.Contains(p) )
                {
                    visited.Add(p);
                    var px=bmp[p.X,p.Y];
                    if ( condition(px) )
                    {
                        action(p , px);
                        if ( p.X > 0 )
                            points.Enqueue(new Point(p.X - 1 , p.Y));
                        if ( p.Y > 0 )
                            points.Enqueue(new Point(p.X , p.Y - 1));
                        if ( p.X < bmp.Width - 1 )
                            points.Enqueue(new Point(p.X + 1 , p.Y));
                        if ( p.Y < bmp.Height - 1 )
                            points.Enqueue(new Point(p.X , p.Y + 1));

                        if ( p.X > 0 && p.Y > 0 )
                            points.Enqueue(new Point(p.X - 1 , p.Y - 1));
                        if ( p.X < bmp.Width - 1 && p.Y > 0 )
                            points.Enqueue(new Point(p.X + 1 , p.Y - 1));

                        if ( p.X > 0 && p.Y < bmp.Height - 1 )
                            points.Enqueue(new Point(p.X - 1 , p.Y + 1));
                        if ( p.X < bmp.Width - 1 && p.Y < bmp.Height - 1 )
                            points.Enqueue(new Point(p.X + 1 , p.Y + 1));
                    }
                }
            }
        }

        public static void Match4Connex<Representation>(this Image<Representation> bmp , int x , int y , Func<Representation , bool> condition , Action<Point , Representation> action)
            where Representation : unmanaged
        {
            Match4Connex(( ImageProxy<Representation> ) bmp , x , y , condition , action);
        }
        public static void Match8Connex<Representation>(this Image<Representation> bmp , int x , int y , Func<Representation , bool> condition , Action<Point , Representation> action)
            where Representation : unmanaged
        {
            Match8Connex(( ImageProxy<Representation> ) bmp , x , y , condition , action);
        }
        public static List<Point> Match4ConnexList<Representation>(this Image<Representation> bmp , int x , int y , Func<Representation , bool> condition)
            where Representation : unmanaged
        {
            return Match4ConnexList(( ImageProxy<Representation> ) bmp , x , y , condition);
        }
        public static List<Point> Match8ConnexList<Representation>(this Image<Representation> bmp , int x , int y , Func<Representation , bool> condition)
            where Representation : unmanaged
        {
            return Match4ConnexList(( ImageProxy<Representation> ) bmp , x , y , condition);
        }
        public static void Match4ConnexMultiplePoints<Representation>(this Image<Representation> bmp , Point[ ] startPoints , Func<Representation , bool> condition , Action<Point , Representation> action)
       where Representation : unmanaged
        {
            Match4ConnexMultiplePoints(( ImageProxy<Representation> ) bmp , startPoints , condition , action);
        }
        public static void Match8ConnexMultiplePoints<Representation>(this Image<Representation> bmp , Point[ ] startPoints , Func<Representation , bool> condition , Action<Point , Representation> action)
            where Representation : unmanaged
        {
            Match8ConnexMultiplePoints(( ImageProxy<Representation> ) bmp , startPoints , condition , action);
        }


        public static void Match8Connex(this bool[ , ] bmp , int x , int y , Action<Point> action)
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
        public static List<Point> Match4ConnexList(this bool[ , ] bmp , int x , int y)
        {
            List<Point> output = new List<Point>();
            bmp.Match4Connex(x , y , (pt) => { output.Add(pt); });
            return output;
        }
        public static List<Point> Match8ConnexList(this bool[ , ] bmp , int x , int y)
        {
            List<Point> output = new List<Point>();
            bmp.Match8Connex(x , y , (pt) => { output.Add(pt); });
            return output;
        }

        public static Func<Point , Point[ ]> Match4ConnexSelector
        {
            get
            {
                return (p) =>
                {
                    return new Point[ ] {
                        new Point(p.X-1,p.Y),
                        new Point(p.X+1,p.Y),
                        new Point(p.X,p.Y-1),
                        new Point(p.X,p.Y+1)
                    };
                };
            }
        }
        public static Func<Point , Point[ ]> Match8ConnexSelector
        {
            get
            {
                return (p) =>
                {
                    return new Point[ ] {
                        new Point(p.X-1,p.Y),
                        new Point(p.X+1,p.Y),
                        new Point(p.X,p.Y-1),
                        new Point(p.X,p.Y+1),
                        new Point(p.X-1,p.Y-1),
                        new Point(p.X+1,p.Y+1),
                        new Point(p.X+1,p.Y-1),
                        new Point(p.X-1,p.Y+1)
                    };
                };
            }
        }
    }
}
