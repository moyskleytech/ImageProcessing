using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MoyskleyTech.Mathematics.Tools
{
    public class BilinearInterpolator
    {
        public static double Interpolate(Func<int , int , double> filler , double x , double y)
        {
            double p00,p01,p10,p11;

            int intX = ( int ) x;
            double fractionX = x - intX;

            int intY = ( int ) y;
            double fractionY = y - intY;

            p00 = filler(intX , intY);
            p01 = filler(intX, intY + 1);
            p10 = filler(intX + 1, intY);
            p11 = filler(intX + 1, intY + 1);

            double mult(double a , double p)
            {
                return p*a;
            }
            double add(double a , double p)
            {
                return p+a;
            }

            var p3d= add(mult((1-fractionX) , (add( mult((1-fractionY),p00) , mult(fractionY,p01)))), mult(fractionX,add(mult((1-fractionY),p10) , mult(fractionY,p11))));
            return p3d;
        }
        public static double[ ] Interpolate(Action<int , int , double[ ]> filler , int length , double x , double y)
        {
            double[] p00,p01,p10,p11;
            p00 = new double[length];
            p01 = new double[length];
            p10 = new double[length];
            p11 = new double[length];

            int intX = ( int ) x;
            double fractionX = x - intX;

            int intY = ( int ) y;
            double fractionY = y - intY;

            filler(intX , intY , p00);
            filler(intX , intY + 1 , p01);
            filler(intX + 1 , intY , p10);
            filler(intX + 1 , intY + 1 , p11);

            double[ ] mult(double a , double[ ] p)
            {
                for ( var i = 0; i < p.Length; i++ )
                    p[i] = p[i] * a;
                return p;
            }
            double[ ] add(double[ ] a , double[ ] p)
            {
                for ( var i = 0; i < p.Length; i++ )
                    p[i] = p[i] + a[i];
                return p;
            }

            var p3d= add(mult((1-fractionX) , (add( mult((1-fractionY),p00) , mult(fractionY,p01)))), mult(fractionX,add(mult((1-fractionY),p10) , mult(fractionY,p11))));
            return p3d;
        }
    }
}
