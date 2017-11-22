using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MoyskleyTech.ImageProcessing.Image
{
    /// <summary>
    /// Represent string alignment
    /// </summary>
    public class StringFormat
    {
        /// <summary>
        /// Position
        /// </summary>
        public StringAlignment LineAlignment,Alignment;
    }
    /// <summary>
    /// Represent string alignment
    /// </summary>
    public enum StringAlignment
    {
#pragma warning disable CS1591
        Near,Center,Far
    }
}
