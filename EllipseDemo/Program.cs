using MoyskleyTech.ImageProcessing.Image;
using MoyskleyTech.ImageProcessing.Recognition.Border;
using MoyskleyTech.ImageProcessing.Recognition.Shape;
using System;
using MoyskleyTech.ImageProcessing;
namespace EllipseDetectionDemo
{
    class Program
    {
        static void Main(string[ ] args)
        {
            Bitmap bitmap = new Bitmap(200,200);
            Graphics g = Graphics.FromImage(bitmap);
            int i=300;
            Random r = new Random();
            for ( var p = 0; p < 200; p++ )
            {
                g.Clear(Pixels.White);
                var mj = r.Next(30,50);
                var mn = r.Next(60,80);
                var angle = 5.497;//r.NextDouble() * Math.PI * 2;
                g.DrawRotatedEllipse(Pixels.Black , 100 , 100 ,mj,mn, angle);
                var contours = ContourRecognition.Analyse<Pixel>(bitmap , (x) => x==Pixels.Black);
                g.DrawString(mj + ":" + mn + ":" + angle , Pixels.DodgerBlue , new PointF(0 , 0) , BaseFonts.Premia , 1);
                foreach ( var ctr in contours )
                {
                    foreach ( var pt in ctr.Points )
                        g.SetPixel(Pixels.Purple , pt.X , pt.Y);

                    var ellipse = (EllipseShape)ShapeFitting.Match(ShapeType.Ellipse , ctr.Points.ToArray());
                    var circle = (CircleShape)ShapeFitting.Match(ShapeType.Circle, ctr.Points.ToArray());

                    g.DrawString(ellipse.EllipseMajor + ":" + ellipse.EllipseMinor + ":" + ellipse.EllipseRotation , Pixels.DodgerBlue , new PointF(0 , 10) , BaseFonts.Premia , 1);

                    g.DrawCircle(Pixels.Yellow , ( int ) circle.CircleCentreX , ( int ) circle.CircleCentreY , circle.CircleRadius , 2);
                    g.DrawRotatedEllipse(Pixels.Red , ellipse.EllipseCentreX , ellipse.EllipseCentreY , ellipse.EllipseMajor , ellipse.EllipseMinor , ellipse.EllipseRotation,2);
                    //g.FillRotatedEllipse(Pixel.FromArgb(Pixels.Red , 100) , ellipse.EllipseCentreX , ellipse.EllipseCentreY , ellipse.EllipseMajor , ellipse.EllipseMinor , ellipse.EllipseRotation);
                    var fs = System.IO.File.OpenWrite((i++)+".bmp");
                    bitmap.Save(fs);
                    fs.Dispose();
                }
            }
        }
    }
}
