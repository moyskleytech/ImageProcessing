using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MoyskleyTech.ImageProcessing.Image
{
    /// <summary>
    /// Represent how to scale an image
    /// </summary>
    public enum ScalingMode:int
    {
        /// <summary>
        /// Using a forloop on source(should be used only if destination is smaller)
        /// </summary>
        LinearFromSource =0,
        /// <summary>
        /// Using a forloop on destination(should be used only if destination is larger)
        /// </summary>
        LinearFromDestination = 17,
        /// <summary>
        /// Using a forloop on source(should be used only if destination is smaller)
        /// </summary>
        LinearFromSourceHorizontal = 0,
        /// <summary>
        /// Using a forloop on destination(should be used only if destination is larger)
        /// </summary>
        LinearFromDestinationHorizontal = 1,
        /// <summary>
        /// Using a forloop on source(should be used only if destination is smaller)
        /// </summary>
        LinearFromSourceVertical = 0,
        /// <summary>
        /// Using a forloop on destination(should be used only if destination is larger)
        /// </summary>
        LinearFromDestinationVertical = 16,
        /// <summary>
        /// Using an average(should only be used if desination is smaller)
        /// </summary>
        Average=256,
        /// <summary>
        /// Autodetect from linear
        /// </summary>
        Auto = 255,
        /// <summary>
        /// Interpolate color between pixels, the only one non-linear for larger destination
        /// </summary>
        AverageInterpolate=257
    }
}
