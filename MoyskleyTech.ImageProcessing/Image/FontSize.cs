namespace MoyskleyTech.ImageProcessing.Image
{
    /// <summary>
    /// Represent Font+Size
    /// </summary>
    public class FontSize
    {
        /// <summary>
        /// Default constructor for serialization
        /// </summary>
        public FontSize()
        { }
        /// <summary>
        /// Create Font+Size using Font and Size
        /// </summary>
        /// <param name="f"></param>
        /// <param name="size"></param>
        public FontSize(Font f , int size)
        {
            Font = f;
            Size = size;
        }
        /// <summary>
        /// Font
        /// </summary>
        public Font Font { get; set; }
        /// <summary>
        /// Size
        /// </summary>
        public int Size { get; set; }
    }
    /// <summary>
    /// Represent Font+Size
    /// </summary>
    public class FontSizeF
    {
        /// <summary>
        /// Default constructor for serialization
        /// </summary>
        public FontSizeF()
        { }
        /// <summary>
        /// Create Font+Size using Font and Size
        /// </summary>
        /// <param name="f"></param>
        /// <param name="size"></param>
        public FontSizeF(Font f , float size)
        {
            Font = f;
            Size = size;
        }
        /// <summary>
        /// Font
        /// </summary>
        public Font Font { get; set; }
        /// <summary>
        /// Size
        /// </summary>
        public float Size { get; set; }
    }
}