using MoyskleyTech.ImageProcessing.Image;
using MoyskleyTech.ImageProcessing.Recognition;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MoyskleyTech.ImageProcessing.Recognition.Border
{
    public static class ContourRecognition
    {
        public static List<Contour> Analyse(ImageProxy bmp , Func<Pixel , bool> condition , bool keepAllPoints = false , ContourRecognitionMode mode = ContourRecognitionMode.EightConnex)
        {
            HashSet<Point> visited=new HashSet<Point>();
            List<Contour> contours = new List<Contour>();
            var work = bmp.ToBitmap();
            for ( var y = 0; y < bmp.Height; y++ )
            {
                for ( var x = 0; x < bmp.Width; x++ )
                {
                    Contour contour = new Contour();
                    if ( !visited.Contains(new Point(x , y)) )
                    {
                        var px = work[x,y];
                        if ( condition(px) )
                        {
                            if ( mode == ContourRecognitionMode.EightConnex )
                                work.Match8Connex(x , y , condition , (pt , pxl) =>
                                {
                                    contour.Points.Add(pt);
                                    visited.Add(pt);
                                });
                            else if ( mode == ContourRecognitionMode.FourConnex )
                                work.Match4Connex(x , y , condition , (pt , pxl) =>
                                {
                                    contour.Points.Add(pt);
                                    visited.Add(pt);
                                });


                            contour.ImageArea = new ImageProxy(bmp , contour.Area);
                            if ( !keepAllPoints )
                                contour.CleanPoints();
                            contours.Add(contour);
                        }
                    }
                }
            }
            return contours;
        }

        public static List<Contour> AnalyseFromPoints(ImageProxy bmp , List<Point> pts , Func<Pixel , bool> condition , bool keepAllPoints = false , ContourRecognitionMode mode = ContourRecognitionMode.EightConnex)
        {
            HashSet<Point> visited=new HashSet<Point>();
            List<Contour> contours = new List<Contour>();
            var work = bmp.ToBitmap();
            foreach ( var sp in pts )
            {
                var x = sp.X;
                var y = sp.Y;
                Contour contour = new Contour();
                if ( !visited.Contains(sp) )
                {
                    var px = work[x,y];
                    if ( condition(px) )
                    {
                        if ( mode == ContourRecognitionMode.EightConnex )
                            work.Match8Connex(x , y , condition , (pt , pxl) =>
                            {
                                contour.Points.Add(pt);
                                visited.Add(pt);
                            });
                        else if ( mode == ContourRecognitionMode.FourConnex )
                            work.Match4Connex(x , y , condition , (pt , pxl) =>
                            {
                                contour.Points.Add(pt);
                                visited.Add(pt);
                            });


                        contour.ImageArea = new ImageProxy(bmp , contour.Area);
                        if ( !keepAllPoints )
                            contour.CleanPoints();
                        contours.Add(contour);
                    }
                }
            }
            return contours;
        }
    }
    public enum ContourRecognitionMode
    {
        FourConnex, EightConnex
    }
    public class Contour
    {
        public Contour()
        {
            Points = new List<Point>();
        }
        public ImageProxy ImageArea { get; internal set; }
        public List<Point> Points { get; internal set; }
        public void CleanPoints()
        {
            var rct= Area;
            bool[,] tmp = new bool[rct.Width+2,rct.Height+2];
            bool[,] tmp2 = new bool[rct.Width+2,rct.Height+2];
            foreach ( var pt in Points )
            {
                tmp[pt.X + 1 - rct.Left , pt.Y + 1 - rct.Top] = true;
            }
            Points.Clear();
            for ( var x = 0; x < rct.Width; x++ )
            {
                for ( var y = 0; y < rct.Height; y++ )
                {
                    var tx=x+1;
                    var ty=y+1;
                    if ( tmp[tx , ty] )
                    {
                        tmp2[tx , ty] = ( !( tmp[tx - 1 , ty] && tmp[tx + 1 , ty] && tmp[tx , ty - 1] && tmp[tx , ty + 1] ) );
                    }
                }
            }
            bool found=false;
            for ( var x = 0; x < rct.Width && !found; x++ )
            {
                for ( var y = 0; y < rct.Height && !found; y++ )
                {
                    var tx=x+1;
                    var ty=y+1;
                    if ( tmp2[tx , ty] )
                    {
                        found = true;
                        Points.AddRange(tmp2.Match8ConnexList(tx , ty).Select((pt) => new Point(pt.X+rct.Left,pt.Y+rct.Top)));
                    }
                }
            }
            //Points.Add(new Point(x + rct.Left , y + rct.Top));

        }
        public Rectangle Area
        {
            get
            {
                int x = Points.Min((p)=>p.X);
                int y = Points.Min((p)=>p.Y);
                int mx = Points.Max((p)=>p.X);
                int my = Points.Max((p)=>p.Y);
                return new Rectangle(x , y , mx - x + 1 , my - y + 1);
            }
        }
    }
}
