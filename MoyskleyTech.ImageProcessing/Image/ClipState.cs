namespace MoyskleyTech.ImageProcessing.Image
{
    public class ClipState
    {
        internal Rectangle clip;
        internal PointF[] clipPolygon;
        internal double[]clipPolygonConstant , clipPolygonMultiple;
        public object OtherState { get; set; }
    }
}