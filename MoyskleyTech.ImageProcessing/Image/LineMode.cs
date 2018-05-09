namespace MoyskleyTech.ImageProcessing.Image
{
    /// <summary>
    /// Describe how lines should be drawn
    /// </summary>
    public enum LineMode
    {
        /// <summary>
        /// Drawn using For loop
        /// </summary>
        ForLoop,
        /// <summary>
        /// Drawn using 4 connex heuristic
        /// </summary>
        FourConnex,
        /// <summary>
        /// Drawn using 8 connex heuristic
        /// </summary>
        EightConnex
    }
}