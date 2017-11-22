#if PCL
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ICSharpCode.SharpZipLib.VirtualFileSystem
{
    /// <summary>
    /// VFS Element informations
    /// </summary>
    public interface IVfsElement
    {
        /// <summary>
        /// Name
        /// </summary>
        string Name { get; }
        /// <summary>
        /// Element exists
        /// </summary>
        bool Exists { get; }
        /// <summary>
        /// Attributes
        /// </summary>
        FileAttributes Attributes { get; }
        /// <summary>
        /// Creation time
        /// </summary>
        DateTime CreationTime { get; }
        /// <summary>
        /// Last access time
        /// </summary>
        DateTime LastAccessTime { get; }
        /// <summary>
        /// Last write time
        /// </summary>
        DateTime LastWriteTime { get; }
    }
}
#endif
