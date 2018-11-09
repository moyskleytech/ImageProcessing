using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MoyskleyTech.Mathematics;

namespace MoyskleyTech.ImageProcessing.Drawing3D
{
    public static class Matrix3DExtention
    {

        public static void Normalize3D(this Matrix m_maxtrix)
        {
            for ( int i = 0; i < 4; i++ )
            {
                for ( int j = 0; j < 4; j++ )
                {
                    m_maxtrix[i , j] = m_maxtrix[i , j] / m_maxtrix[3 , 3];
                }
            }
        }

        public static double[ ] ApplyTransform(this Matrix m_maxtrix , double x , double y , double z , double w)
        {
            double[] _res = new double[4];
            double[] _point = new double[] {x,y,z,w};
            double _value = 0;
            for ( int i = 0; i < 4; i++ )
            {
                _value = 0;
                for ( int j = 0; j < 4; j++ )
                {
                    _value = _value + _point[j] * m_maxtrix[j , i];
                }
                _res[i] = _value;
            }
            if ( _value != 0.0 )
            {
                _res[0] = _res[0] / _value;
                _res[1] = _res[1] / _value;
            }
            else
            {
                _res[3] = double.PositiveInfinity;
            }
            return _res;
        }
        public static double[ ] ApplyTransform(this Matrix m_maxtrix , Point3D? point)
        {
            if ( !point.HasValue )
                return null;
            double x = point.Value.X, y= point.Value.Y, z = point.Value.Z,w=1;
            double[] _res = new double[4];
            double[] _point = new double[] {x,y,z,w};
            double _value = 0;
            for ( int i = 0; i < 4; i++ )
            {
                _value = 0;
                for ( int j = 0; j < 4; j++ )
                {
                    _value = _value + _point[j] * m_maxtrix[j , i];
                }
                _res[i] = _value;
            }
            if ( _value != 0.0 )
            {
                _res[0] = _res[0] / _value;
                _res[1] = _res[1] / _value;
            }
            else
            {
                _res[3] = double.PositiveInfinity;
            }
            return _res;
        }
    }
}
