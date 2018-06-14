using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MoyskleyTech.ImageProcessing.Image
{
    /// <summary>
    /// Represent a pattern
    /// </summary>
    public abstract class Brush : Brush<Pixel>
    {
    }
    /// <summary>
    /// Represent a pattern
    /// </summary>
    public abstract class Brush<Representation>
    where Representation : struct
    {
        /// <summary>
        /// Get the color from the specified position in the pattern
        /// </summary>
        /// <param name="x">X position</param>
        /// <param name="y">Y position</param>
        /// <returns></returns>
        public abstract Representation GetColor(int x , int y);
    }
    /// <summary>
    /// Represent a brush using an image
    /// </summary>
    [NotSerialized]
    public class ImageBrush : Brush
    {
        /// <summary>
        /// Create the brush using an image
        /// </summary>
        /// <param name="img"></param>
        public ImageBrush(Image<Pixel> img)
        {
            Image = img;
        }

        /// <summary>
        /// Return the image
        /// </summary>
        public Image<Pixel> Image { get; }

        /// <summary>
        /// Get the color from the specified position in the pattern
        /// </summary>
        /// <param name="x">X position</param>
        /// <param name="y">Y position</param>
        /// <returns></returns>
        public override Pixel GetColor(int x , int y)
        {
            return Image[x % Image.Width , y % Image.Height];
        }
    }
    /// <summary>
    /// Represent a brush using a linear pattern
    /// </summary>
    public class LinearGradientBrush : Brush
    {
        Point sourcePt,finalPt;
        Pixel source,final;

        /// <summary>
        /// Source location of gradient
        /// </summary>
        public Point SourceLocation { get { return sourcePt; } }
        /// <summary>
        /// Color at begenning
        /// </summary>
        public Pixel SourceColor { get { return source; } }
        /// <summary>
        /// Location of end
        /// </summary>
        public Point FinalLocation { get { return finalPt; } }
        /// <summary>
        /// Color at end
        /// </summary>
        public Pixel FinalColor { get { return final; } }
        /// <summary>
        /// Create a pattern
        /// </summary>
        /// <param name="srcp">Source point</param>
        /// <param name="src">Source color</param>
        /// <param name="destp">End point</param>
        /// <param name="dest">End color</param>
        public LinearGradientBrush(Point srcp , Pixel src , Point destp , Pixel dest)
        {
            this.sourcePt = srcp;
            this.source = src;
            this.finalPt = destp;
            this.final = dest;
        }
        /// <summary>
        /// Get the color from the specified position in the pattern
        /// </summary>
        /// <param name="x">X position</param>
        /// <param name="y">Y position</param>
        /// <returns></returns>
        public override Pixel GetColor(int x , int y)
        {
            var A = (finalPt.X - sourcePt.X);
            var B = (finalPt.Y - sourcePt.Y);
            var C1 = A * sourcePt.X + B * sourcePt.Y;
            var C2 = A * finalPt.X + B * finalPt.Y;
            var C = A * x + B * y;
            if ( C <= C1 )
                return source;
            if ( C >= C2 )
                return final;

            //( start_color * ( C2 - C ) + end_color * ( C - C1 ) ) / ( C2 - C1 )
            var a = (byte)((source.A*(C2-C)+final.A*(C-C1))/(C2-C1));
            var r = (byte)((source.R*(C2-C)+final.R*(C-C1))/(C2-C1));
            var g = (byte)((source.G*(C2-C)+final.G*(C-C1))/(C2-C1));
            var b = (byte)((source.B*(C2-C)+final.B*(C-C1))/(C2-C1));
            return Pixel.FromArgb(a , r , g , b);
        }

    }
    /// <summary>
    /// Represent a brush using a linear pattern
    /// </summary>
    public class LinearMultiGradientBrush : Brush
    {
        /// <summary>
        /// Represent a stop
        /// </summary>
        public class GradientStop
        {
            /// <summary>
            /// Color at position
            /// </summary>
            public Pixel Color { get; set; }
            /// <summary>
            /// Weight of the color(max 1 min 0)
            /// </summary>
            public double Weigth { get; set; }
        }
        Point sourcePt,finalPt;
        /// <summary>
        /// All colors
        /// </summary>
        public List<GradientStop> Stops { get; set; } = new List<GradientStop>();
        /// <summary>
        /// Source of gradient
        /// </summary>
        public Point SourceLocation { get { return sourcePt; } }

        /// <summary>
        /// End of gradient
        /// </summary>
        public Point FinalLocation { get { return finalPt; } }
        /// <summary>
        /// Create a pattern
        /// </summary>
        /// <param name="srcp">Source point</param>
        /// <param name="destp">End point</param>
        /// <param name="stops">End color</param>
        public LinearMultiGradientBrush(Point srcp , Point destp , IEnumerable<GradientStop> stops)
        {
            this.sourcePt = srcp;
            this.finalPt = destp;
            this.Stops = stops.ToList();
        }
        /// <summary>
        /// Get the color from the specified position in the pattern
        /// </summary>
        /// <param name="x">X position</param>
        /// <param name="y">Y position</param>
        /// <returns></returns>
        public override Pixel GetColor(int x , int y)
        {
            var dx = (finalPt.X - sourcePt.X);
            var dy = (finalPt.Y - sourcePt.Y);
            var C1 = dx * sourcePt.X + dy * sourcePt.Y;
            var C2 = dx * finalPt.X + dy * finalPt.Y;
            var C = dx * x + dy * y;
            if ( C <= C1 )
                return Stops[0].Color;
            if ( C >= C2 )
                return Stops.Last().Color;

            //( start_color * ( C2 - C ) + end_color * ( C - C1 ) ) / ( C2 - C1 )

            var weigth =(C-C1) / (double)(C2-C1);
            var before = Stops.Last((p)=>p.Weigth<=weigth);
            var after = Stops.First((p)=>p.Weigth>=weigth);
            if ( before == after )
                return before.Color;
            var bpx = sourcePt.X + dx*before.Weigth;
            var bpy = sourcePt.Y + dy*before.Weigth;

            var epx = sourcePt.X + dx*after.Weigth;
            var epy = sourcePt.Y + dy*after.Weigth;

            dx = ( int ) ( epx - bpx );
            dy = ( int ) ( epy - bpy );
            C1 = ( int ) ( dx * bpx + dy * bpy );
            C2 = ( int ) ( dx * epx + dy * epy );
            C = dx * x + dy * y;

            var a = (byte)((before.Color.A*(C2-C)+after.Color.A*(C-C1))/(C2-C1));
            var r = (byte)((before.Color.R*(C2-C)+after.Color.R*(C-C1))/(C2-C1));
            var g = (byte)((before.Color.G*(C2-C)+after.Color.G*(C-C1))/(C2-C1));
            var b = (byte)((before.Color.B*(C2-C)+after.Color.B*(C-C1))/(C2-C1));
            return Pixel.FromArgb(a , r , g , b);
        }

    }
    /// <summary>
    /// Represent a radial gradient
    /// </summary>
    public class RadialGradientBrush : Brush
    {
        Point sourcePt;
        Pixel source, final;
        double radius;
        double da,dr,dg,db;
        /// <summary>
        /// Center of radialbrush
        /// </summary>
        public Point SourceLocation { get { return sourcePt; } }
        /// <summary>
        /// Color at center
        /// </summary>
        public Pixel SourceColor { get { return source; } }
        /// <summary>
        /// Radius of brush
        /// </summary>
        public double Radius { get { return radius; } }
        /// <summary>
        /// Color at the outside of radial
        /// </summary>
        public Pixel FinalColor { get { return final; } }
        /// <summary>
        /// Create a pattern
        /// </summary>
        /// <param name="srcp">Source point</param>
        /// <param name="src">Source color</param>
        /// <param name="destp">End point</param>
        /// <param name="dest">End color</param>
        public RadialGradientBrush(Point srcp , Pixel src , Point destp , Pixel dest)
        {
            this.sourcePt = srcp;
            this.source = src;
            this.radius = System.Math.Sqrt(System.Math.Pow(destp.X - srcp.X , 2) + System.Math.Pow(destp.Y - srcp.Y , 2));
            this.final = dest;
            da = ( final.A - source.A ) / radius;
            dr = ( final.R - source.R ) / radius;
            dg = ( final.G - source.G ) / radius;
            db = ( final.B - source.B ) / radius;
        }
        /// <summary>
        /// Get the color from the specified position in the pattern
        /// </summary>
        /// <param name="x">X position</param>
        /// <param name="y">Y position</param>
        /// <returns></returns>
        public override Pixel GetColor(int x , int y)
        {
            if ( x == sourcePt.X && y == sourcePt.Y )
                return source;
            var r = System.Math.Sqrt(System.Math.Pow(x - sourcePt.X , 2) + System.Math.Pow(y - sourcePt.Y , 2));
            if ( r > radius )
                return final;

            return Pixel.FromArgb(
                ( byte ) ( source.A + da * r ) ,
                ( byte ) ( source.R + dr * r ) ,
                ( byte ) ( source.G + dg * r ) ,
                ( byte ) ( source.B + db * r ));
        }
    }
    /// <summary>
    /// Create a solidbrush to use a color on a method that uses a brush
    /// </summary>
    public class SolidBrush : Brush
    {
        private Pixel p;
        /// <summary>
        /// Create a solidbrush to use a color on a method that uses a brush
        /// </summary>
        public SolidBrush(Pixel p)
        {
            this.p = p;
        }
        /// <summary>
        /// Get color at specified location
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <returns></returns>
        public override Pixel GetColor(int x , int y)
        {
            return p;
        }
    }
    /// <summary>
    /// Create a solidbrush to use a color on a method that uses a brush
    /// </summary>
    public class SolidBrush<Representation> : Brush<Representation>
        where Representation : struct
    {
        private Representation p;
        /// <summary>
        /// Create a solidbrush to use a color on a method that uses a brush
        /// </summary>
        /// <param name="p">The color</param>
        public SolidBrush(Representation p)
        {
            this.p = p;
        }
        /// <summary>
        /// Get Color at specified location
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <returns></returns>
        public override Representation GetColor(int x , int y)
        {
            return p;
        }
        /// <summary>
        /// Cast the brush to another colorspace
        /// </summary>
        /// <typeparam name="NewColorspace">New colorspace</typeparam>
        /// <returns></returns>
        public SolidBrush<NewColorspace> As<NewColorspace>()
            where NewColorspace : struct
        {
            return new SolidBrush<NewColorspace>(ColorConvert.Convert<Representation , NewColorspace>(p));
        }
    }
}
