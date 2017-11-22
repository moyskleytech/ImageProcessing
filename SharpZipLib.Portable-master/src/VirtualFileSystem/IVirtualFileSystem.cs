#if PCL
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ICSharpCode.SharpZipLib.VirtualFileSystem
{

    /// <summary>
    /// Interface providing Virtual File System access
    /// </summary>
    public interface IVirtualFileSystem
    {

        /// <summary>
        /// List directory files
        /// </summary>
        IEnumerable<String> GetFiles(string directory);

        /// <summary>
        /// List directories
        /// </summary>
        IEnumerable<String> GetDirectories(string directory);

        /// <summary>
        /// Get the full path for a file
        /// </summary>
        string GetFullPath(string path);

        /// <summary>
        /// Get the informations of a directory
        /// </summary>
        IDirectoryInfo GetDirectoryInfo(string directoryName);

        /// <summary>
        /// Get the informations of a file
        /// </summary>
        IFileInfo GetFileInfo(string filename);

        /// <summary>
        /// Define the last write time
        /// </summary>
        void SetLastWriteTime(string name, DateTime dateTime);

        /// <summary>
        /// Define the attributes
        /// </summary>
        /// <param name="name"></param>
        /// <param name="attributes"></param>
        void SetAttributes(string name, FileAttributes attributes);

        /// <summary>
        /// Create a new directory
        /// </summary>
        void CreateDirectory(string directory);

        /// <summary>
        /// Create a temporary file name
        /// </summary>
        string GetTempFileName();

        /// <summary>
        /// Copy a file
        /// </summary>
        void CopyFile(string fromFileName, string toFileName, bool overwrite);

        /// <summary>
        /// Move a file
        /// </summary>
        void MoveFile(string fromFileName, string toFileName);

        /// <summary>
        /// Delete a file
        /// </summary>
        void DeleteFile(string fileName);

        /// <summary>
        /// Create an new file
        /// </summary>
        VfsStream CreateFile(String filename);

        /// <summary>
        /// Open file for read
        /// </summary>
        VfsStream OpenReadFile(String filename);

        /// <summary>
        /// Open existing file for write
        /// </summary>
        VfsStream OpenWriteFile(String filename);

        /// <summary>
        /// Current directory
        /// </summary>
        String CurrentDirectory { get; }

        /// <summary>
        /// Directory separator
        /// </summary>
        Char DirectorySeparatorChar { get; }

    }

}
#endif