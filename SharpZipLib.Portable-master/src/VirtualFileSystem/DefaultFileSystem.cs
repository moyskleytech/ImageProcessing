#if PCL
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ICSharpCode.SharpZipLib.VirtualFileSystem
{

    /// <summary>
    /// Default file system
    /// </summary>
    public class DefaultFileSystem : IVirtualFileSystem
    {
        const String InvalidOperationMessage = "The default file system is not implemented in the Portable Class Library. Implement IVirtualFileSystem and defined it with VFS.SetCurrent().";

        /// <summary>
        /// List directory files
        /// </summary>
        public virtual IEnumerable<String> GetFiles(string directory)
        {
            throw new InvalidOperationException(InvalidOperationMessage);
        }

        /// <summary>
        /// List directories
        /// </summary>
        public virtual IEnumerable<String> GetDirectories(string directory)
        {
            throw new InvalidOperationException(InvalidOperationMessage);
        }

        /// <summary>
        /// Create a temporary file name
        /// </summary>
        public virtual string GetTempFileName()
        {
            throw new InvalidOperationException(InvalidOperationMessage);
        }

        /// <summary>
        /// Copy a file
        /// </summary>
        public virtual void CopyFile(string fromFileName, string toFileName, bool overwrite)
        {
            throw new InvalidOperationException(InvalidOperationMessage);
        }

        /// <summary>
        /// Move a file
        /// </summary>
        public virtual void MoveFile(string fromFileName, string toFileName)
        {
            throw new InvalidOperationException(InvalidOperationMessage);
        }

        /// <summary>
        /// Delete a file
        /// </summary>
        public virtual void DeleteFile(string fileName)
        {
            throw new InvalidOperationException(InvalidOperationMessage);
        }

        /// <summary>
        /// Get the full path for a file
        /// </summary>
        public virtual string GetFullPath(string path)
        {
            return path;
        }

        /// <summary>
        /// Get the informations of a file
        /// </summary>
        public virtual IFileInfo GetFileInfo(string filename)
        {
            throw new InvalidOperationException(InvalidOperationMessage);
        }

        /// <summary>
        /// Get the informations of a directory
        /// </summary>
        public virtual IDirectoryInfo GetDirectoryInfo(string directoryName)
        {
            throw new InvalidOperationException(InvalidOperationMessage);
        }

        /// <summary>
        /// Define the last write time
        /// </summary>
        public virtual void SetLastWriteTime(string name, DateTime dateTime)
        {
            throw new InvalidOperationException(InvalidOperationMessage);
        }

        /// <summary>
        /// Define the attributes
        /// </summary>
        /// <param name="name"></param>
        /// <param name="attributes"></param>
        public virtual void SetAttributes(string name, FileAttributes attributes)
        {
            throw new InvalidOperationException(InvalidOperationMessage);
        }

        /// <summary>
        /// Create a new directory
        /// </summary>
        public virtual void CreateDirectory(string directory)
        {
            throw new InvalidOperationException(InvalidOperationMessage);
        }

        /// <summary>
        /// Create a new file
        /// </summary>
        public virtual VfsStream CreateFile(string filename)
        {
            throw new InvalidOperationException(InvalidOperationMessage);
        }

        /// <summary>
        /// Open a file for read
        /// </summary>
        public virtual VfsStream OpenReadFile(string filename)
        {
            throw new InvalidOperationException(InvalidOperationMessage);
        }

        /// <summary>
        /// Open an existing file for writing
        /// </summary>
        public virtual VfsStream OpenWriteFile(string filename)
        {
            throw new InvalidOperationException(InvalidOperationMessage);
        }

        /// <summary>
        /// Current directory
        /// </summary>
        public virtual String CurrentDirectory { get { throw new InvalidOperationException(InvalidOperationMessage); } }


        /// <summary>
        /// Directory separator
        /// </summary>
        public virtual Char DirectorySeparatorChar { get { throw new InvalidOperationException(InvalidOperationMessage); } }

    }

}
#endif
