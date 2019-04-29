using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MoyskleyTech.ImageProcessing.Image;

namespace MoyskleyTech.Charting.Charting2D
{
    public class Gauge<R> : IDrawableChart<R>
        where R : unmanaged
    {
        public double Mininum { get; set; } = 0;
        public double Maximum { get; set; } = 100;
        public double Value { get; set; } = 60;
        private int numberOfGraduations = 5;
        public int NumberOfGraduations { get => numberOfGraduations; set { if (value > 0 && value < Maximum) numberOfGraduations = value; else throw new InvalidOperationException("Cannot set number of graduations lower than 0 or more than Max"); } }
        private int numberOfSubGraduations = 5;
        public int NumberOfSubGraduations { get => numberOfSubGraduations; set { if (value > 0 && value < Maximum) numberOfSubGraduations = value; else throw new InvalidOperationException("Cannot set number of subgraduations lower than 0 or more than Max"); } }

        private double chartSpan = 270;
        public double ChartSpanDegree { get => chartSpan; set { if (value > 0 && value <= 360) chartSpan = value; else throw new InvalidOperationException("Cannot set chart span outside of 360 degree"); } }
        public double ChartSpanRadian { get => chartSpan / 180 * Math.PI; set { if (value > 0 && value <= 2 * Math.PI) chartSpan = value * 180 / Math.PI; else throw new InvalidOperationException("Cannot set chart span outside of 2 PI radian"); } }

        public R NeedleColor { get; set; } = ColorConvert.Convert<Pixel, R>(Pixels.Red);
        public R GraduationColor { get; set; } = ColorConvert.Convert<Pixel, R>(Pixels.DarkGray);
        public R SubGraduationColor { get; set; } = ColorConvert.Convert<Pixel, R>(Pixels.Gray);
        public R BorderColor { get; set; } = ColorConvert.Convert<Pixel, R>(Pixels.Black);

        public int NeedleSize { get; set; } = 1;
        public int BorderSize { get; set; } = 3;
        public Brush<R> Background { get; set; } = new SolidBrush<R>(ColorConvert.Convert<Pixel, R>(Pixels.Black));
        public Brush<R> ChartBackground { get; set; } = new SolidBrush<R>(ColorConvert.Convert<Pixel, R>(Pixels.White));

        public List<GaugeArea<R>> Areas { get; private set; } = new List<GaugeArea<R>>();

        public GaugeMode Mode { get; set; } = GaugeMode.Needle;
        public Padding Padding { get; set; } = new Padding(0);
        public double GraduationProportion { get; set; } = 0.9;
        public double SubGraduationProportion { get; set; } = 0.95;
        public double NeedleProportion { get; set; } = 0.7;
        public double NeedleProportionInside { get; set; } = 0;
        public bool GraduationDisplayed { get; set; } = true;

        public Image<R> Draw(int width, int height)
        {
            Image<R> r = Image<R>.Create(width, height);
            var g = Graphics<R>.FromImage(r);
            Draw(g, width, height);
            g.Dispose();
            return r;
        }

        public void Draw(Graphics<R> g, int width, int height)
        {
            var chartBegin = 1.5 * Math.PI - ChartSpanRadian / 2;

            var needleAngle = Value / Maximum * ChartSpanRadian + chartBegin;
            var needleSpan = needleAngle - chartBegin;

            int w = width - Padding.Horizontal;
            int h = height - Padding.Vertical;
            double cx = Padding.Left + w / 2;
            double cy = Padding.Top + h / 2;
            var center = new PointF(cx, cy);

            g.FillRectangle(Background, Padding.Left, Padding.Top, w, h);

            if (h > w)//Circle
                h = w;
            else if (w > h)
                w = h;

            
            var chartArc = GraphicsHelper.GetEllipseArcPolygon(center, w / 2, h / 2, chartBegin, ChartSpanRadian).Concat(new PointF[] {center });
            var minY = chartArc.Min((x) => x.Y);
            var maxY = chartArc.Max((x) => x.Y);
            var ocy = cy;
            cy = (maxY + minY) / 2;
            var dy = ocy - cy;
            cy = ocy + dy;
            center = new PointF(cx, cy);

            chartArc = GraphicsHelper.GetEllipseArcPolygon(center, w / 2, h / 2, chartBegin, ChartSpanRadian);

            g.DrawPath(BorderColor, BorderSize, chartArc);

            if(ChartSpanDegree>180)
                g.FillPolygon(ChartBackground, chartArc.ToArray());
            else
                g.FillPolygon(ChartBackground, chartArc.Concat(new PointF[] { center }).ToArray());

            double valueToAngle(double value)
            {
                return value / Maximum * ChartSpanRadian + chartBegin;
            }

            foreach (var area in Areas)
            {
                var angleBegin = valueToAngle(area.Minimum);
                var angleEnd = valueToAngle(area.Maximum);
                var angleSpan = angleEnd - angleBegin;

                var arcPolygonIn = GraphicsHelper.GetEllipseArcPolygon(center, w / 2*area.InsideProportion, h / 2 * area.InsideProportion, angleBegin, angleSpan);
                var arcPolygonOut = GraphicsHelper.GetEllipseArcPolygon(center, w / 2 * area.OutsideProportion, h / 2*area.OutsideProportion, angleBegin, angleSpan);

                var areaPolygon = arcPolygonIn.Concat(arcPolygonOut.Reverse()).ToArray();

                g.FillPolygon(area.Fill, areaPolygon);
                g.DrawPolygon(area.Border, areaPolygon);
            }
            if (GraduationDisplayed)
                DisplayGraduation(g, chartBegin, w, h, center);

            if (Mode == GaugeMode.Needle)
            {

                var outsidePoint = GraphicsHelper.FindEllipsePoint(w / 2 * NeedleProportion, h / 2 * NeedleProportion, 0, center.X, center.Y, needleAngle);
                var insidePoint = center;
                g.DrawLine(NeedleColor, outsidePoint, insidePoint, NeedleSize);
            }
            else if (Mode == GaugeMode.FillToNeedle)
            {
                var arcPolygon = GraphicsHelper.GetEllipseArcPolygon(center, w / 2 * NeedleProportion, h / 2 * NeedleProportion, chartBegin, needleSpan);
                var arcPolygonIn = GraphicsHelper.GetEllipseArcPolygon(center, w / 2 * NeedleProportionInside, h / 2 * NeedleProportionInside, chartBegin, needleSpan);
                var areaPolygon = arcPolygon.Concat(arcPolygonIn.Reverse()).ToArray();

                g.FillPolygon(NeedleColor, areaPolygon);
            }

        }

        private void DisplayGraduation(Graphics<R> g, double chartBegin, int w, int h, PointF center)
        {
            int numberOfSections = numberOfGraduations - 1;
            var sectionSpan = ChartSpanRadian / numberOfSections;
            var subSectionSpan = sectionSpan / numberOfSubGraduations;
            double sectionBegin = chartBegin;


            for (var t = 0; t < numberOfGraduations; t++)
            {
                double subSectionBegin = sectionBegin + subSectionSpan;
                var outsidePoint = GraphicsHelper.FindEllipsePoint(w / 2, h / 2, 0, center.X, center.Y, sectionBegin);
                var insidePoint = GraphicsHelper.FindEllipsePoint(w / 2 * GraduationProportion, h / 2 * GraduationProportion, 0, center.X, center.Y, sectionBegin);
                g.DrawLine(GraduationColor, outsidePoint, insidePoint);

                if (t != numberOfGraduations - 1)
                    for (var k = 1; k < numberOfSubGraduations; k++)
                    {
                        var outsidePointSub = GraphicsHelper.FindEllipsePoint(w / 2, h / 2, 0, center.X, center.Y, subSectionBegin);
                        var insidePointSub = GraphicsHelper.FindEllipsePoint(w / 2 * SubGraduationProportion, h / 2 * SubGraduationProportion, 0, center.X, center.Y, subSectionBegin);
                        g.DrawLine(SubGraduationColor, outsidePointSub, insidePointSub);
                        subSectionBegin += subSectionSpan;
                    }
                sectionBegin += sectionSpan;
            }
        }
    }
}
