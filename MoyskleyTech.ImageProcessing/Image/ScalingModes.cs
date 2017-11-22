using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MoyskleyTech.ImageProcessing.Image
{
    public enum ScalingMode:int
    {
        LinearFromSource =0,
        LinearFromDestination = 17,
        LinearFromSourceHorizontal = 0,
        LinearFromDestinationHorizontal = 1,
        LinearFromSourceVertical = 0,
        LinearFromDestinationVertical = 16,
        Average=256,
        Auto = 255,
        AverageInterpolate=257
    }
}
