#if PCL
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ICSharpCode.SharpZipLib.VirtualFileSystem
{
    /// <summary>
    /// File informations
    /// </summary>
    public interface IFileInfo : IVfsElement
    {
        /// <summary>
        /// Size of file
        /// </summary>
        long Length { get; }
    }
}
#endif