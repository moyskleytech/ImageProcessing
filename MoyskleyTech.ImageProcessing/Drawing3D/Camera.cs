using MoyskleyTech.Mathematics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MoyskleyTech.ImageProcessing.Drawing3D
{
    public class Camera
    {
        Point3D camPosition;
        public Point3D TargetPoint { get; set; }

        public Point3D CameraPosition { get => camPosition;set=> camPosition = value; }

        public double Distance { get; set; }

        public double ZoomFactor { get; set; }

        public Vector3D UpVector { get; set; }

        public ProjectionTypes ProjectType { get; set; }

        public Matrix ProjectedMatrix { get; set; }

        public ViewTypes View { get; set; }

        public double Phi { get; set; }

        public double CameraTheta { get; set; }

        public double CameraRadius { get; set; }

        private Point3D m_temp;

        private Matrix mout;

        public Camera()
        {
            CameraPosition = Point3D.Origin;
            camPosition.X = 0;
            camPosition.Y = 0;
            camPosition.Z = 3;
            TargetPoint = new Point3D(0 , 0 , 0);
            UpVector = Vector3D.YAxis;
            //CameraRotation = new Quaternion(45, 30, 0);
            Distance = CameraPosition.DistanceTo(TargetPoint);
            CameraRadius = Distance;
            Vector3D n = new Vector3D(CameraPosition, TargetPoint);
            View = ViewTypes.Iso;
            CameraTheta = 45;
            Phi = 30;
            camPosition.Y = Distance * Math.Sin(Math.PI * Phi / 180);
            camPosition.X = Distance * Math.Cos(Math.PI * CameraTheta / 180) * Math.Cos(Math.PI * Phi / 180);
            camPosition.Z = Distance * Math.Sin(Math.PI * CameraTheta / 180) * Math.Cos(Math.PI * Phi / 180);
            m_temp = Point3D.Origin;

        }


        public void Project(double theta = 0 , double phi = 0 , double tx = 0 , double ty = 0 , double tz = 0)
        {
            if ( UpVector == Vector3D.YAxis )
            {
                m_temp.Y = Distance * Math.Sin(Math.PI * ( Phi + phi ) / 180);
                m_temp.X = Distance * Math.Cos(Math.PI * ( CameraTheta + theta ) / 180) * Math.Cos(Math.PI * ( Phi + phi ) / 180);
                m_temp.Z = Distance * Math.Sin(Math.PI * ( CameraTheta + theta ) / 180) * Math.Cos(Math.PI * ( Phi + phi ) / 180);
            }
            else
            {
                m_temp.Z = Distance * Math.Sin(Math.PI * ( Phi + phi ) / 180);
                m_temp.X = Distance * Math.Cos(Math.PI * ( CameraTheta + theta ) / 180) * Math.Cos(Math.PI * ( Phi + phi ) / 180);
                m_temp.Y = Distance * Math.Sin(Math.PI * ( CameraTheta + theta ) / 180) * Math.Cos(Math.PI * ( Phi + phi ) / 180);
            }

            double _sin1, _cos1, _sin2, _cos2, _sin3, _cos3;
            double _d1,_d2,_d3;

            Matrix _m1 = Matrix.Identity(4);

            _m1[3 , 0] = -( m_temp.X );
            _m1[3 , 1] = -( m_temp.Y );
            _m1[3 , 2] = -( m_temp.Z );

            Point3D _dist = m_temp-TargetPoint;
            _d2 = m_temp.DistanceTo(TargetPoint);
            _d1 = Math.Sqrt(( _dist.X * _dist.X ) + ( _dist.Z * _dist.Z ));

            // X Axis rotation
            Matrix _m2 = Matrix.Identity(4);
            if ( _d1 != 0.0 )
            {
                _sin1 = -_dist.X / _d1;
                _cos1 = _dist.Z / _d1;
                _m2[0 , 0] = _cos1;
                _m2[0 , 2] = -_sin1;
                _m2[2 , 0] = _sin1;
                _m2[2 , 2] = _cos1;

            }
            // Y Axis rotation
            _d2 = Math.Sqrt(( _dist.X * _dist.X ) + ( _dist.Y * _dist.Y ) + ( _dist.Z * _dist.Z ));
            Matrix _m3 = Matrix.Identity(4);
            if ( _d2 != 0.0 )
            {
                _sin2 = _dist.Y / _d2;
                _cos2 = _d1 / _d2;
                _m3[1 , 1] = _cos2;
                _m3[1 , 2] = _sin2;
                _m3[2 , 1] = -_sin2;
                _m3[2 , 2] = _cos2;
            }
            double[] _up2= _m2.ApplyTransform(UpVector.X, UpVector.Y, UpVector.Z, 1);
            double[] _up1=_m3.ApplyTransform(_up2[0], _up2[1], _up2[2], _up2[3]);

            // Z Axis Rotation
            _d3 = Math.Sqrt(( _up1[0] * _up1[0] ) + ( _up1[1] * _up1[1] ));
            Matrix _m4 = Matrix.Identity(4);
            if ( _d3 != 0.0 )
            {
                _sin3 = _up1[0] / _d3;
                _cos3 = _up1[1] / _d3;
                _m4[0 , 0] = _cos3;
                _m4[0 , 1] = _sin3;
                _m4[1 , 0] = -_sin3;
                _m4[1 , 1] = _cos3;
            }
            Matrix _m5=Matrix.Identity(4);
            if ( ProjectType == ProjectionTypes.Perspective && _d2 != 0.0 )
            {
                _m5[2 , 3] = -1 / _d2;
            }
            else
            {
                _m5 = Matrix.Identity(4);
            }
            Matrix _a= (_m1* _m2);
            Matrix _b =(_m3* _m4);
            Matrix _res = (_a* _b);

            // Still in progress code
            if ( View == ViewTypes.Front )
            {
                Matrix m_view = Matrix.Identity(4);

                m_view[2 , 2] = 0;

                ProjectedMatrix = m_view;
            }
            else if ( View == ViewTypes.Top )
            {
                Matrix m_view = Matrix.Identity(4);
                m_view[1 , 1] = 0;
                m_view[2 , 1] = -1;
                m_view[2 , 2] = 0;
                ProjectedMatrix = m_view;
            }
            else if ( View == ViewTypes.Left )
            {
                Matrix m_view = Matrix.Identity(4);
                m_view[0 , 0] = 0;
                m_view[2 , 0] = -1;
                m_view[2 , 2] = 0;
                ProjectedMatrix = m_view;
            }
            else
                ProjectedMatrix = (_res * _m5);
        }



    }

    public enum ProjectionTypes
    {
        Orthographic,
        Perspective
    }
    public enum ViewTypes
    {
        Front,
        Rear,
        Top,
        Bottom,
        Left,
        Right,
        Iso
    }
}
