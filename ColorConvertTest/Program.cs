using System;
using MoyskleyTech.ImageProcessing.Android;
using MoyskleyTech.ImageProcessing.Image;

namespace ColorConvertTest
{
    class Program
    {
        static int total=0;
        static int success=0;
        static void Main(string[ ] args)
        {
            Check(Pixel.FromArgb(255 , 100 , 200 , 150) , new RGB { R = 100 , G = 200 , B = 150 });
            Check(Pixel.FromArgb(255 , 100 , 200 , 150) , new BGR { R = 100 , G = 200 , B = 150 });
            Check(Pixel.FromArgb(255 , 100 , 200 , 150) , new ARGB {A=255, R = 100 , G = 200 , B = 150 });
            Check(Pixel.FromArgb(255 , 100 , 200 , 150) , new ARGB_Float { A=1, R = 100/255f , G = 200 / 255f , B = 150 / 255f });

            Check(Pixel.FromArgb(255 , 100 , 200 , 150) , new ARGB_Float { A = 1 , R = 100 / 255f , G = 200 / 255f , B = 150 / 255f });
            

            Console.WriteLine($"Success {success}/{total}");
        }

        static void Check<T, V>(T src , V dst)
        {
            total++;
            V dst2=ColorConvert.Convert<T,V>(src);
            if ( dst2.GetHashCode() == dst.GetHashCode() ) 
                success++;
            else 
                Console.WriteLine("Error converting {0} to {1}" , typeof(T).Name , typeof(V).Name); 
        }
    }
}
