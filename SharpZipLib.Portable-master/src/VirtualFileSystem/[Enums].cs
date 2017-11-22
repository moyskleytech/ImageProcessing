#if PCL
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ICSharpCode.SharpZipLib.VirtualFileSystem
{

    /// <summary>
    /// File attributes
    /// </summary>
    [Flags]
    public enum FileAttributes : int
    {
        /// <summary>
        /// File is read only
        /// </summary>
        ReadOnly = 1,
        /// <summary>
        /// File is hidden
        /// </summary>
        Hidden = 2,
        /// <summary>
        /// File is directory
        /// </summary>
        Directory = 16,
        /// <summary>
        /// File is for archive
        /// </summary>
        Archive = 32,
        /// <summary>
        /// File is normal
        /// </summary>
        Normal = 128
    }

}
#endif