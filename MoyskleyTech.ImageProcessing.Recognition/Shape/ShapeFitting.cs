using MoyskleyTech.ImageProcessing.Image;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MoyskleyTech.ImageProcessing.Recognition.Shape
{
    public static class ShapeFitting
    {
        public static BestFit Match(ShapeType type , Point[ ] array)
        {
            switch ( type )
            {
                case ShapeType.Line:
                    return MatchLine(array);
                case ShapeType.Circle:
                    return MatchCircle(array);
                case ShapeType.Ellipse:
                    return MatchEllipse(array);
            }
            return null;
        }

        private static LineShape MatchLine(Point[ ] array)
        {
            LineShape shape = new LineShape();
            shape.Fit(array);
            return shape;
        }
        private static CircleShape MatchCircle(Point[ ] array)
        {
            CircleShape shape = new CircleShape();
            shape.Fit(array);
            return shape;
        }
        private static EllipseShape MatchEllipse(Point[ ] array)
        {
            EllipseShape shape = new EllipseShape();
            shape.Fit(array);
            return shape;
        }
    }
    public enum ShapeType
    {
        Line, Circle, Ellipse
    }
    public abstract class BestFit
    {
        internal int nbObs=0;
        protected Math.Matrix.Matrix m_residuals;
        protected Math.Matrix.Matrix m_design;
        protected Math.Matrix.Matrix m_l;
        protected Math.Matrix.Matrix m_qweight;
        protected Math.Matrix.Matrix m_observations;
        protected Math.Matrix.Matrix m_provisionals;
        protected Math.Matrix.Matrix m_b;
        protected Math.Matrix.Matrix m_solution;
        protected int m_minx,m_maxx,m_miny,m_maxy;
        protected abstract void GenerateProvisionals();
        protected abstract void FormulateMatrices();
        protected abstract void EvaluateFinalResiduals(int point ,  out double vxi,out double vyi);
        private const double CONVERGENCE_CRITERIA=0.000000001;
        internal abstract double SolveAt(double x , double y);
        internal abstract void FillOutput();
        private const int MAX_ITERATIONS=50;

        protected virtual void NormaliseAdjustedUnknowns() { }

        public void Fit(Point[ ] array)
        {
            nbObs = array.Length;

            m_maxx = array.Max((x) => x.X);
            m_maxy = array.Max((x) => x.Y);
            m_minx = array.Min((x) => x.X);
            m_miny = array.Min((x) => x.Y);

            ResizeMatrices();

            for ( var i = 0; i < array.Length; i++ )
            {
                m_observations[i , 0] = array[i].X;
                m_observations[i , 1] = array[i].Y;
            }

            Compute();
        }
        public void Compute()
        {
            GenerateProvisionals();
            bool successful = true;

            int iteration = 0;

            while ( true )
            {
                FormulateMatrices();

                // evaluate the unknowns - small corrections to be applied to the provisional unknowns
                if ( EvaluateUnknowns() )
                {
                    ++iteration;

                    // add the solution to the provisional unknowns
                    EvaluateAdjustedUnknowns();

                    if ( HasConverged() )
                        break;
                    if ( IsDegenerate(iteration) )
                        break;
                }
                else
                {
                    successful = false;
                    break;
                }
                
            }

            successful = successful && ( iteration > 0 && iteration < MAX_ITERATIONS );
            if ( successful )
            {
                // Give ellipse chance to normalise it's axes. 
                NormaliseAdjustedUnknowns();

                // evaluate the residuals
                EvaluateResiduals();

                // add the residuals to the provisional observations
                EvaluateAdjustedObservations();

                // Check that the adjusted unknowns and adjusted observations satisfy
                // the original line/circle/ellipse equation.
                GlobalCheck();

                // Subsequent error analysis and statistical output
                ErrorAnalysis(iteration);
            }
            FillOutput();
        }

        private void ErrorAnalysis(int iteration)
        {
           
        }

        private void GlobalCheck()
        {
            List<double> global = new List<double>(nbObs);
            bool pass = (nbObs > 0);

            for ( int i = 0; i < nbObs; ++i )
            {
                double x = m_observations[i, 0];
                double y = m_observations[i, 1];

                global[i] = SolveAt(x , y); // should be zero
                pass = pass && System.Math.Abs(global[i]) < 0.01; // TODO: Is this too lax?
            }

           
            // Secondary test is to check that aTPv is zero too, really only neccessary
            // if the above global check fails.
            var atp = m_design.Transposee* m_qweight;
            var atpv = atp* m_residuals;

            pass = true;
            for ( int j = 0; j < m_numUnknowns; ++j )
                pass = pass && (atpv[j , 0]==0);

        }

        

        private void EvaluateAdjustedObservations()
        {
            for ( int i = 0; i < nbObs; ++i )
            {
                double vxi = 0.0;
                double vyi = 0.0;
                EvaluateFinalResiduals(i ,out vxi ,out vyi); // overridden for normal (non-quasi-parametric) BestFitLine

                m_observations[i , 0] += vxi;
                m_observations[i , 1] += vyi;
            }

        }

        private void EvaluateResiduals()
        {
            m_residuals = (m_design * m_solution) + -m_l;
        }

        private bool IsDegenerate(int iteration)
        {
            return (iteration >= MAX_ITERATIONS);
        }

        private bool HasConverged()
        {
            for ( int i = 0; i < m_numUnknowns; ++i )
            {
                if ( System.Math.Abs(m_solution[i , 0]) > CONVERGENCE_CRITERIA )
                    return false;
            }
            return true;
        }

        private void EvaluateAdjustedUnknowns()
        {
            m_provisionals += m_solution;
        }

        private bool EvaluateUnknowns()
        {
            try
            {
                MoyskleyTech.Math.Matrix.Matrix pa = (m_qweight* m_design);
                MoyskleyTech.Math.Matrix.Matrix atpa = ((m_design).Transposee* pa);

                Math.Matrix.Matrix inverse = new Math.Matrix.Matrix(atpa.Lignes, atpa.Colonnes);
                //if (CholeskyInversion(atpa, inverse))

                inverse = atpa.MatriceInverse;
                Math.Matrix.Matrix pl = (m_qweight* m_l);
                Math.Matrix.Matrix atpl = ((m_design.Transposee)* pl);

                m_solution = ( inverse * atpl );

                return true;

            }
            catch
            {
                return false;
            }
        }

        protected abstract int m_numUnknowns { get; }
        private void ResizeMatrices()
        {
            m_provisionals = new Math.Matrix.Matrix(5 , 1);
            m_residuals = new Math.Matrix.Matrix(nbObs , 1);
            m_design = new Math.Matrix.Matrix(nbObs , m_numUnknowns);
            m_l = new Math.Matrix.Matrix(nbObs , 1);
            m_qweight = new Math.Matrix.Matrix(nbObs , nbObs);
            m_observations = new Math.Matrix.Matrix(nbObs , 2);
            m_b = new Math.Matrix.Matrix(nbObs , nbObs * 2);
        }
    }
    public class LineShape : BestFit
    {
        public double LineGradient { get; set; }
        public double LineYIntercept { get; set; }
        protected override int m_numUnknowns => 2;

        protected override void EvaluateFinalResiduals(int point , out double vxi , out double vyi)
        {
            vxi = 0.0;
            vyi = m_residuals[point , 0];
        }

        protected override void FormulateMatrices()
        {
            double slope = m_provisionals[0, 0];
            double intercept = m_provisionals[1, 0];

            for ( int i = 0; i < nbObs; i++ )
            {
                double x = m_observations[i, 0];
                double y = m_observations[i, 1];

                m_design[i , 0] = x;
                m_design[i , 1] = 1.0;

                m_qweight[i , i] = 1.0; // parametric case, not quasi-parametric
                m_l[i , 0] = y - ( ( x * slope ) + intercept );
            }
        }

        protected override void GenerateProvisionals()
        {
            double slope = (m_maxy - m_miny) / (m_maxx - m_minx);
            double intercept = m_miny - (slope * m_minx);

            m_provisionals[0 , 0] = slope;
            m_provisionals[1 , 0] = intercept;
        }

        internal override void FillOutput()
        {
            this.LineGradient = m_provisionals[0 , 0];
            this.LineYIntercept = m_provisionals[1 , 0];
        }

        internal override double SolveAt(double x , double y)
        {
            double slope = m_provisionals[0, 0];
            double intercept = m_provisionals[1, 0];
            return y - ( x * slope + intercept );
        }
    }
    public class CircleShape : BestFit
    {
        public double CircleCentreX { get; set; }
        public double CircleCentreY { get; set; }
        public double CircleRadius { get; set; }
        protected override int m_numUnknowns => 3;

        protected override void EvaluateFinalResiduals(int point , out double vxi , out double vyi)
        {
            vxi = 0;
            vyi = 0;
        }

        protected override void FormulateMatrices()
        {
            double x0 = m_provisionals[0, 0];
            double y0 = m_provisionals[1, 0];
            double radius = m_provisionals[2, 0];

            for ( int i = 0; i < nbObs; i++ )
            {
                double dx = m_observations[i, 0] - x0;
                double dy = m_observations[i, 1] - y0;
                double dxsqr = dx * dx;
                double dysqr = dy * dy;

                m_design[i , 0] = -2.0 * dx;
                m_design[i , 1] = -2.0 * dy;
                m_design[i , 2] = -2.0 * radius;
                m_qweight[i , i] = 1.0 / ( 4.0 * ( dxsqr + dysqr ) );
                m_l[i , 0] = ( radius * radius ) - dxsqr - dysqr;
                m_b[i , i * 2 + 0] = 2.0 * dx;
                m_b[i , i * 2 + 1] = 2.0 * dy;
            }
        }

        protected override void GenerateProvisionals()
        {
            double centrex = 0.5 * (m_maxx + m_minx);
            double centrey = 0.5 * (m_maxy + m_miny);
            double radius =  0.5 * System.Math.Max(m_maxx - m_minx, m_maxy - m_miny);

            m_provisionals[0 , 0] = centrex;
            m_provisionals[1 , 0] = centrey;
            m_provisionals[2 , 0] = radius;
        }

        internal override void FillOutput()
        {
            CircleCentreX = m_provisionals[0 , 0];
            CircleCentreY = m_provisionals[1 , 0];
            CircleRadius = m_provisionals[2 , 0];
        }

        internal override double SolveAt(double x , double y)
        {
            double dx = x - m_provisionals[0, 0];
            double dy = y - m_provisionals[1, 0];
            double rsqr = m_provisionals[2, 0] * m_provisionals[2, 0];

            return rsqr - dx * dx - dy * dy;
        }
    }
    public class EllipseShape : BestFit
    {
        public double EllipseCentreX { get; set; }
        public double EllipseCentreY { get; set; }
        public double EllipseMajor { get; set; }
        public double EllipseMinor { get; set; }
        public double EllipseRotation { get; set; }

        protected override int m_numUnknowns => 5;

        protected override void EvaluateFinalResiduals(int point , out double vxi , out double vyi)
        {
            vxi = vyi = 0;
        }

        protected override void FormulateMatrices()
        {
            double x0 = m_provisionals[0, 0];
            double y0 = m_provisionals[1, 0];
            double a = m_provisionals[2, 0];
            double b = m_provisionals[3, 0];

            double sinr = System.Math.Sin(m_provisionals[4, 0]);
            double cosr = System.Math.Cos(m_provisionals[4, 0]);
            double asqr = a * a;
            double bsqr = b * b;

            for ( int i = 0; i < nbObs; i++ )
            {
                double x = m_observations[i, 0];
                double y = m_observations[i, 1];

                double d1 = ((y - y0) * cosr - (x - x0) * sinr);
                double d2 = ((x - x0) * cosr + (y - y0) * sinr);

                m_design[i , 0] = 2.0 * ( ( asqr * d1 * sinr ) - ( bsqr * d2 * cosr ) );
                m_design[i , 1] = -2.0 * ( ( bsqr * d2 * sinr ) + ( asqr * d1 * cosr ) );
                m_design[i , 2] = 2.0 * a * ( ( d1 * d1 ) - bsqr );
                m_design[i , 3] = 2.0 * b * ( ( d2 * d2 ) - asqr );
                m_design[i , 4] = 2.0 * d1 * d2 * ( bsqr - asqr );

                double p = 2.0 * ((asqr * d1 * sinr) + (bsqr * d2 * cosr));
                double q = -2.0 * ((bsqr * d2 * sinr) + (asqr * d1 * cosr));
                m_qweight[i , i] = 1.0 / ( p * p + q * q );
                m_l[i , 0] = -( ( asqr * d1 * d1 ) + ( bsqr * d2 * d2 ) - ( asqr * bsqr ) );
            }
        }

        protected override void GenerateProvisionals()
        {
            double centrex = 0.5 * (m_maxx + m_minx);
            double centrey = 0.5 * (m_maxy + m_miny);
            double dx = m_maxx - m_minx;
            double dy = m_maxy - m_miny;
            double major = 0.5 * System.Math.Max(dx, dy);
            double minor = 0.5 * System.Math.Min(dx, dy);
            double rotation = System.Math.Atan2(dy , dx);

            m_provisionals[0 , 0] = centrex;
            m_provisionals[1 , 0] = centrey;
            m_provisionals[2 , 0] = major;
            m_provisionals[3 , 0] = minor;
            m_provisionals[4 , 0] = rotation;
        }

        internal override void FillOutput()
        {
            EllipseCentreX = m_provisionals[0 , 0];
            EllipseCentreY = m_provisionals[1 , 0];
            EllipseMajor = m_provisionals[2 , 0];
            EllipseMinor = m_provisionals[3 , 0];
            EllipseRotation = m_provisionals[4 , 0];
        }

        internal override double SolveAt(double x , double y)
        {
            double x0 = m_provisionals[0, 0];
            double y0 = m_provisionals[1, 0];
            double asqr = m_provisionals[2, 0] * m_provisionals[2, 0];
            double bsqr = m_provisionals[3, 0] * m_provisionals[3, 0];
            double sinr = System.Math.Sin(m_provisionals[4, 0]);
            double cosr = System.Math.Cos(m_provisionals[4, 0]);
            double dx = x - x0;
            double dy = y - y0;
            double d1 = ( dx * cosr + dy * sinr);
            double d2 = (-dx * sinr + dy * cosr);

            return ( ( d1 * d1 ) / asqr ) + ( ( d2 * d2 ) / bsqr ) - 1.0;
        }

        public void Draw(Graphics g,Brush b,int t=1)
        {
            var m = g.TransformationMatrix.Clone();
            g.ResetTransform();

            g.TranslateTransform(EllipseCentreX , EllipseCentreY);
            g.RotateTransform(EllipseRotation);
            g.TranslateTransform(-EllipseCentreX , -EllipseCentreY);

            g.DrawEllipse(b , (int)EllipseCentreX , ( int ) EllipseCentreY , ( int ) EllipseMajor , ( int ) EllipseMinor , t);

            g.TransformationMatrix = m;
        }
        public void Fill(Graphics g , Brush b)
        {
            var m = g.TransformationMatrix.Clone();
            g.ResetTransform();

            g.TranslateTransform(EllipseCentreX , EllipseCentreY);
            g.RotateTransform(EllipseRotation);
            g.TranslateTransform(-EllipseCentreX , -EllipseCentreY);

            g.FillEllipse(b , ( int ) EllipseCentreX , ( int ) EllipseCentreY , ( int ) EllipseMajor , ( int ) EllipseMinor);

            g.TransformationMatrix = m;
        }
    }
}
