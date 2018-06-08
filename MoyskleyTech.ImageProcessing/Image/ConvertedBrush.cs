using System;

namespace MoyskleyTech.ImageProcessing.Image
{
    /// <summary>
    /// Brush that convert from one colorspace to another
    /// </summary>
    /// <typeparam name="T">Colorspace source</typeparam>
    /// <typeparam name="V">Colorspace destination</typeparam>
    public class ConvertedBrush<T, V> : Brush<V>
        where T : struct
        where V : struct
    {
        /// <summary>
        /// InnerBrush
        /// </summary>
        public Brush<T> Brush { get; set; }
        /// <summary>
        /// Inner Converter
        /// </summary>
        public Func<T , V> Converter { get; set; }
        /// <summary>
        /// Get color
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <returns></returns>
        public override V GetColor(int x , int y)
        {
            return Converter(Brush.GetColor(x , y));
        }
    }

}
