using MoyskleyTech.ImageProcessing.Image;
using MoyskleyTech.Mathematics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MoyskleyTech.ImageProcessing.Drawing3D
{
    public class Viewport3D
    {
        double m_theta;
        double m_phi;
        int m_x;
        int m_y;
        private List<IObject3D> m_list;
        double m_tx = 0;
        double m_ty = 0;
        double m_tz = 0;
        public Font Font { get; set; }
        public ViewTypes View { get => Camera.View; set => Camera.View = value; }
        public Camera Camera { get; set; }
        public bool DrawAxes { get; set; } = true;
        public List<IObject3D> Items => m_list;
        public int LinePointCount { get; set; } = 10;
        public Viewport3D()
        {
            Camera = new Camera();
            m_list = new List<IObject3D>();
        }

        public void Render(Graphics<Pixel> graphics , int width , int height)
        {
            var m_origin = new Point();
            m_origin.X = width / 2;
            m_origin.Y = height / 2;

            var state = graphics.SaveClipState();
            graphics.TranslateTransform(m_origin.X , m_origin.Y);
            Camera.Project(m_theta , m_phi , m_tx , m_ty , m_tz);
            DrawData(graphics , width , height);
            graphics.RestoreClipState(state);

        }
        private void DrawData(Graphics<Pixel> graphics , int width , int height)
        {
            var scale = Math.Min(width,height)/3;
            //DrawAxes(e.Graphics);
            var renderer = new Renderer3D(){ LinePointCount=LinePointCount };
            if ( DrawAxes )
            {
                renderer.RenderLine(new Point3D(0 , 0 , 0) , new Point3D(1 , 0 , 0) , Pixels.Red , 2);
                renderer.RenderLine(new Point3D(0 , 0 , 0) , new Point3D(0 , 1 , 0) , Pixels.Green , 2);
                renderer.RenderLine(new Point3D(0 , 0 , 0) , new Point3D(0 , 0 , 1) , Pixels.Blue , 2);
                renderer.RenderText(new Point3D(1 , 0 , 0) , "X" , Pixels.Red , 1);
                renderer.RenderText(new Point3D(0 , 1 , 0) , "Y" , Pixels.Green , 1);
                renderer.RenderText(new Point3D(0 , 0 , 1) , "Z" , Pixels.Blue , 1);
            }

            foreach ( IObject3D obj in m_list )
            {
                obj.Render(renderer);
            }
            renderer.Output(graphics , Camera.ProjectedMatrix , scale , Font);
        }
        public void CursorMove(int x , int y)
        {
            m_phi -= x;
            m_theta -= y;
        }

        public class Renderer3D : IGraphics3D
        {
            public int LinePointCount { get; set; }
            private class Instruction
            {
                public Pixel color;
                public Point3D[] p3ds;
                public double size;
                public string text;
            }
            private class PreRenderedInstruction
            {
                public Pixel color;
                public double[][] p3ds;
                public double Z => p3ds.Average((x) => x[2]);
                public double size;
                public string text;
            }
            private List<Instruction> tmp = new List<Instruction>();
            public void Output(Graphics<Pixel> graphics , Matrix tMatrix , double scale , Font font)
            {
                var preRenderer = (from x in tmp select ToPreRendered(tMatrix,x,scale)).ToList();
                tmp.Clear();

                var ordered= preRenderer.OrderBy((x)=>x.Z);

                foreach ( var element in ordered )
                {
                    if ( element.text != null )//Is text
                    {
                        graphics.DrawString(element.text , element.color , ( int ) element.p3ds[0][0] , ( int ) -element.p3ds[0][1] , font , ( float ) element.size);
                    }
                    else
                    {
                        if ( element.p3ds.Length > 1 )//is line or polygon
                        {
                            if ( element.p3ds.Length > 2 )//polygon
                                graphics.FillPolygon(element.color , ( from x in element.p3ds select new PointF(x[0] , x[1]) ).ToArray());
                            else
                                graphics.DrawLine(element.color , ( int ) element.p3ds[0][0] , ( int ) -element.p3ds[0][1] , ( int ) element.p3ds[1][0] , ( int ) -element.p3ds[1][1] , ( int ) element.size);
                        }
                        else//is point
                        {
                            if ( element.size > 1 )
                                graphics.FillCircle(element.color , ( int ) element.p3ds[0][0] , ( int ) -element.p3ds[0][1] , element.size);
                            else
                                graphics.FillRectangle(element.color , ( int ) element.p3ds[0][0] , ( int ) -element.p3ds[0][1] , 1 , 1);
                        }
                    }
                }
            }

            private static PreRenderedInstruction ToPreRendered(Matrix tMatrix , Instruction x , double scale)
            {
                var pts=PreRender(x.p3ds , tMatrix , scale);
                var ret= new PreRenderedInstruction() { color = x.color , size = x.size , text = x.text, p3ds = pts };
                return ret;
            }

            private static double[ ][ ] PreRender(Point3D[ ] p3ds , Matrix tMatrix , double scale)
            {
                double[][] ret = new double[p3ds.Length][];
                for ( var i = 0; i < ret.Length; i++ )
                {
                    ret[i] = tMatrix.ApplyTransform(p3ds[i] * scale);
                }
                return ret;
            }

            private static Point3D? MultiplyIfValue(Point3D? p1 , double scale)
            {
                if ( !p1.HasValue )
                    return null;
                else
                {
                    Point3D point3 = p1.Value;
                    point3.X *= scale;
                    point3.Y *= scale;
                    point3.Z *= scale;
                    p1 = point3;
                }
                return p1;
            }

            public void RenderPoint(Point3D point , Pixel color , double size)
            {
                tmp.Add(new Instruction() { color = color , size = size , p3ds = new Point3D[ ] { point } });
            }

            public void RenderLine(Point3D point1 , Point3D point2 , Pixel color , double size)
            {
                tmp.Add(new Instruction() { color = color , size = size , p3ds = new Point3D[ ] { point1 , point2 } });
                AddLinePoints(point1 , point2 , color , size);
            }

            public void RenderText(Point3D point , string text , Pixel color , double size)
            {
                tmp.Add(new Instruction() { color = color , size = size , p3ds = new Point3D[ ] { point } , text = text });
            }

            private void AddLinePoints(Point3D m_1 , Point3D m_2 , Pixel m_xColor , double size)
            {
                if ( LinePointCount > 0 )
                {
                    var dx = m_1.X-m_2.X;
                    var dy = m_1.Y-m_2.Y;
                    var dz = m_1.Z-m_2.Z;

                    //double x=-m_x.X*scaleFactor,y=-m_x.Y*scaleFactor,z=-m_x.Z*scaleFactor;
                    double x=m_2.X,y=m_2.Y,z=m_2.Z;
                    int count=LinePointCount;
                    for ( var i = 0; i < count; i++ )
                    {
                        RenderPoint(new Point3D(x , y , z) , m_xColor , size);
                        x += dx / count;
                        y += dy / count;
                        z += dz / count;
                    }
                }
            }

            public void RenderPolygon(Point3D[ ] points , Pixel color)
            {
                tmp.Add(new Instruction() { color = color , p3ds = points });
            }
        }
    }
}
