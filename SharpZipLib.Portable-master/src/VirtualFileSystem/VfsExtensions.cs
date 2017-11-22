#if PCL
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ICSharpCode.SharpZipLib.VirtualFileSystem
{
    /// <summary>
    /// Extensions for VFS
    /// </summary>
    public static class VfsExtensions
    {
        /// <summary>
        /// Check if a directory exists
        /// </summary>
        public static bool DirectoryExists(this IVirtualFileSystem vfs, String directoryName)
        {
            try
            {
                return vfs.GetDirectoryInfo(directoryName).Exists;
            }
            catch
            {
                return false;
            }
        }
        /// <summary>
        /// Check if a file exists
        /// </summary>
        public static bool FileExists(this IVirtualFileSystem vfs, String fileName)
        {
            try
            {
                return vfs.GetFileInfo(fileName).Exists;
            }
            catch
            {
                return false;
            }
        }
        /// <summary>
        /// List directorires and files 
        /// </summary>
        public static IEnumerable<String> GetDirectoriesAndFiles(this IVirtualFileSystem vfs, String directoryName)
        {
            return vfs.GetDirectories(directoryName).Concat(vfs.GetFiles(directoryName));
        }
    }
}
#endif
