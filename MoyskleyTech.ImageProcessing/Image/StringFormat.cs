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
        public StringAlignment LineAlignment { get; set; } = StringAlignment.Near;
        public StringAlignment Alignment { get; set; } = StringAlignment.Near;
        public EllipsisMode EllipsisMode { get; set; } = EllipsisMode.None;
    }
    /// <summary>
    /// Represent string alignment
    /// </summary>
    public enum StringAlignment
    {
#pragma warning disable CS1591
        Near, Center, Far
#pragma warning restore CS1591
    }
    /// <summary>
    /// Represent string alignment
    /// </summary>
    public enum EllipsisMode
    {
#pragma warning disable CS1591
        Character, Word, None
#pragma warning restore CS1591

    }

}
