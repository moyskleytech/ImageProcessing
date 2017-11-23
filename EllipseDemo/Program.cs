using MoyskleyTech.ImageProcessing.Image;
using MoyskleyTech.ImageProcessing.Recognition.Border;
using MoyskleyTech.ImageProcessing.Recognition.Shape;
using System;

namespace EllipseDetectionDemo
{
    class Program
    {
        static void Main(string[ ] args)
        {
            Bitmap bitmap = new Bitmap(200,200);
            Graphics g = Graphics.FromImage(bitmap);
            g.Clear(Pixels.White);

            g.FillEllipse(Pixels.Black , 50 , 50 , 50 , 50);
            var contours = ContourRecognition.Analyse(bitmap , (x) => x==Pixels.Black);

            int i=0;
            foreach ( var ctr in contours )
            {
                foreach ( var pt in ctr.Points )
                    g.SetPixel(Pixels.Purple , pt.X,pt.Y);

                var ellipse = ShapeFitting.Match(ShapeType.Ellipse , ctr.Points.ToArray());

                var fs = System.IO.File.OpenWrite((i++)+".bmp");
                ctr.ImageArea.ToBitmap().Save(fs);
                fs.Dispose();
            }

        }
    }
}
