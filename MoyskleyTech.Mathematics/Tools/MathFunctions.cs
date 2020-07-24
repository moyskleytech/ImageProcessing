using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MoyskleyTech.Mathematics.Tools
{
    public static class MathFunctions
    {
        static double RoundToSignificantDigits(this double d , int digits)
        {
            if ( d == 0 )
                return 0;
            if ( d == 0 ) { return 0; }
            if ( d < 0 ) { d = Math.Abs(d); }

            double scale = Math.Pow(10, Math.Floor(Math.Log10(Math.Abs(d))) + 1);
            return scale * Math.Round(d / scale , digits);
        }
        static double TruncateToSignificantDigits(this double d , int digits)
        {
            if ( d == 0 )
                return 0;
            if ( d == 0 ) { return 0; }
            if ( d < 0 ) { d = Math.Abs(d); }

            double scale = Math.Pow(10, Math.Floor(Math.Log10(Math.Abs(d))) + 1 - digits);
            return scale * Math.Truncate(d / scale);
        }
        public static string FindFormulaFor<T>(IEnumerable<T> list , Func<T , double> val1 , Func<T , double> val2 , string name1 = "X" , string name2 = "Y" , int degree = 7)
        {
            var coeffs = FindPolynomialLeastSquaresFit(list.Select((x) => new Coordinate(val1(x) , val2(x))).ToList() , degree);
            int idx=1;
            var coeffs2=coeffs.Skip(1).Select((v)=>(v.ToString("G2"))+" "+name1+"^"+(idx++)).Reverse().ToList();

            StringBuilder sb = new StringBuilder();
            sb.Append(name1);
            sb.Append(" ");
            sb.Append(string.Join(" + " , coeffs2));
            if ( coeffs2.Any() )
                sb.Append(" + ");
            sb.Append(coeffs[0]);

            return sb.ToString();
        }
        public static double FindLinearLeastSquaresFit<T>(IEnumerable<T> list , Func<T , double> val1 , Func<T , double> val2 , out double m , out double b)
        {
            return FindLinearLeastSquaresFit(list.Select((x) => new Coordinate(val1(x) , val2(x))).ToList() , out m , out b);
        }
        public static double FindLinearLeastSquaresFit(List<Coordinate> points , out double m , out double b)
        {
            // Perform the calculation.
            // Find the values S1, Sx, Sy, Sxx, and Sxy.
            double S1 = points.Count;
            double Sx = 0;
            double Sy = 0;
            double Sxx = 0;
            double Sxy = 0;
            foreach ( Coordinate pt in points )
            {
                Sx += pt.X;
                Sy += pt.Y;
                Sxx += pt.X * pt.X;
                Sxy += pt.X * pt.Y;
            }

            // Solve for m and b.
            m = ( Sxy * S1 - Sx * Sy ) / ( Sxx * S1 - Sx * Sx );
            b = ( Sxy * Sx - Sy * Sxx ) / ( Sx * Sx - S1 * Sxx );

            return Math.Sqrt(ErrorSquaredLinear(points , m , b));
        }

        // The function calculates the value of the function F(x) at the point x.
        public static double CalculateValueUsingCoefficients(List<double> coeffs , double x)
        {
            double total = 0;
            double x_factor = 1;
            for ( int i = 0; i < coeffs.Count; i++ )
            {
                total += x_factor * coeffs[i];
                x_factor *= x;
            }
            return total;
        }
        // Return the error squared.
        public static double ErrorSquaredPolynomial(List<Coordinate> points , List<double> coeffs)
        {
            double total = 0;
            foreach ( Coordinate pt in points )
            {
                double dy = pt.Y - CalculateValueUsingCoefficients(coeffs, pt.X);
                total += dy * dy;
            }
            return total;
        }
        public static List<double> FindPolynomialLeastSquaresFit<T>(IEnumerable<T> list , Func<T , double> val1 , Func<T , double> val2 , int degree)
        {
            return FindPolynomialLeastSquaresFit(list.Select((x) => new Coordinate(val1(x) , val2(x))).ToList() , degree);
        }
        // Find the least squares linear fit.
        public static List<double> FindPolynomialLeastSquaresFit(List<Coordinate> points , int degree)
        {
            // Allocate space for (degree + 1) equations with 
            // (degree + 2) terms each (including the constant term).
            double[,] coeffs = new double[degree + 1, degree + 2];

            // Calculate the coefficients for the equations.
            for ( int j = 0; j <= degree; j++ )
            {
                // Calculate the coefficients for the jth equation.

                // Calculate the constant term for this equation.
                coeffs[j , degree + 1] = 0;
                foreach ( Coordinate pt in points )
                {
                    coeffs[j , degree + 1] -= Math.Pow(pt.X , j) * pt.Y;
                }

                // Calculate the other coefficients.
                for ( int a_sub = 0; a_sub <= degree; a_sub++ )
                {
                    // Calculate the dth coefficient.
                    coeffs[j , a_sub] = 0;
                    foreach ( Coordinate pt in points )
                    {
                        coeffs[j , a_sub] -= Math.Pow(pt.X , a_sub + j);
                    }
                }
            }

            // Solve the equations.
            double[] answer = GaussianElimination(coeffs);

            // Return the result converted into a List<double>.
            return answer.ToList<double>();
        }

        // Perform Gaussian elimination on these coefficients.
        // Return the array of values that gives the solution.
        private static double[ ] GaussianElimination(double[ , ] coeffs)
        {
            int max_equation = coeffs.GetUpperBound(0);
            int max_coeff = coeffs.GetUpperBound(1);
            for ( int i = 0; i <= max_equation; i++ )
            {
                // Use equation_coeffs[i, i] to eliminate the ith
                // coefficient in all of the other equations.

                // Find a row with non-zero ith coefficient.
                if ( coeffs[i , i] == 0 )
                {
                    for ( int j = i + 1; j <= max_equation; j++ )
                    {
                        // See if this one works.
                        if ( coeffs[j , i] != 0 )
                        {
                            // This one works. Swap equations i and j.
                            // This starts at k = i because all
                            // coefficients to the left are 0.
                            for ( int k = i; k <= max_coeff; k++ )
                            {
                                double temp = coeffs[i, k];
                                coeffs[i , k] = coeffs[j , k];
                                coeffs[j , k] = temp;
                            }
                            break;
                        }
                    }
                }

                // Make sure we found an equation with
                // a non-zero ith coefficient.
                double coeff_i_i = coeffs[i, i];
                if ( coeff_i_i == 0 )
                {
                    throw new ArithmeticException(String.Format(
                        "There is no unique solution for these points." ,
                        coeffs.GetUpperBound(0) - 1));
                }

                // Normalize the ith equation.
                for ( int j = i; j <= max_coeff; j++ )
                {
                    coeffs[i , j] /= coeff_i_i;
                }

                // Use this equation value to zero out
                // the other equations' ith coefficients.
                for ( int j = 0; j <= max_equation; j++ )
                {
                    // Skip the ith equation.
                    if ( j != i )
                    {
                        // Zero the jth equation's ith coefficient.
                        double coef_j_i = coeffs[j, i];
                        for ( int d = 0; d <= max_coeff; d++ )
                        {
                            coeffs[j , d] -= coeffs[i , d] * coef_j_i;
                        }
                    }
                }
            }

            // At this point, the ith equation contains
            // 2 non-zero entries:
            //      The ith entry which is 1
            //      The last entry coeffs[max_coeff]
            // This means Ai = equation_coef[max_coeff].
            double[] solution = new double[max_equation + 1];
            for ( int i = 0; i <= max_equation; i++ )
            {
                solution[i] = coeffs[i , max_coeff];
            }

            // Return the solution values.
            return solution;
        }

        // Return the error squared.
        public static double ErrorSquaredLinear(List<Coordinate> points , double m , double b)
        {
            double total = 0;
            foreach ( Coordinate pt in points )
            {
                double dy = pt.Y - (m * pt.X + b);
                total += dy * dy;
            }
            return total;
        }
    }
}
