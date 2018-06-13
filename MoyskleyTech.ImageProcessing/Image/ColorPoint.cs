namespace MoyskleyTech.ImageProcessing.Image
{
    public struct ColorPoint<T> where T : struct
    {
        public ColorPoint(Point pt , T val)
        {
            Point = pt;
            Color = val;
        }
        public ColorPoint(int x,int y , T val)
        {
            Point = new Point(x,y);
            Color = val;
        }
        public Point Point { get; set; }
        public T Color { get; set; }
    }
}