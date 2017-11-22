# SharpZipLib.Portable

SharpZipLib.Portable is a Portable Class Library version of the [\#ziplib](https://github.com/icsharpcode/SharpZipLib) a Zip, GZip, Tar and BZip2 library written entirely in C# for the .NET platform.

As PCL don't suppport cryptography nor System.IO, this library can't crypt/decrypt an archive, and all access to the file system are removed.

It works on .Net 4.5, Windows Phone 8.0/8.1, Windows Store 8.1, Xamarin.Android (MonoDroid), Xamarion.iOS (MonoTouch). 

## Use it

Install the library with Nuget : `Install-Package SharpZipLib.Portable`.

If you need use the library with file names, defined your FileSystem :
```csharp
VFS.SetCurrent(new MyFileSystem());
```

See below for more information about VirtualFileSystem and implementing it.

## Build

[![Build status](https://ci.appveyor.com/api/projects/status/ifiq9bjs50h8nxxg/branch/master)](https://ci.appveyor.com/project/ygrenier/sharpziplib-portable/branch/master)

This library is build with [AppVeyor](https://ci.appveyor.com/project/ygrenier/sharpziplib-portable).

## Virtual File System

With the version 0.86.0.0001, an VirtualFileSystem structure is available. 

This structure permit to use SharpZipLib with file name like original library. 
For this the `VFS.Current` singleton provide the `IVirtualFileSystem` used by the library.

As PCL are not supporting System.IO, the default behavior (`DefaultVirtualFileSystem`) throws `InvalidOperationException`. 
So before using this library with file name you need to implement `IVirtualFileSystem` (or override `DefaultFileSystemSystem`) and define the singleton with `VFS.SetCurrent()`.

## Implementing IVirtualFileSystem

There is nothing special to implement this interface, except for the file attributes. 
Just a small set of attributes (the more classics) are supported, but don't forget to specify `FileAttributes.Normal` for simple file without another attributes, and `FileAttributes.Directory` for the directories.

For example the implementation for the unit tests :

```csharp

    class TestFileSystem : ICSharpCode.SharpZipLib.VirtualFileSystem.IVirtualFileSystem
    {
        class ElementInfo : IVfsElement
        {
            protected FileSystemInfo Info;
            public ElementInfo(FileSystemInfo info)
            {
                Info = info;
            }

            public string Name
            {
                get { return Info.Name; }
            }

            public bool Exists
            {
                get { return Info.Exists; }
            }

            public VirtualFileSystem.FileAttributes Attributes
            {
                get {
                    VirtualFileSystem.FileAttributes attrs = 0;
                    if (Info.Attributes.HasFlag(System.IO.FileAttributes.Normal)) attrs |= VirtualFileSystem.FileAttributes.Normal;
                    if (Info.Attributes.HasFlag(System.IO.FileAttributes.ReadOnly)) attrs |= VirtualFileSystem.FileAttributes.ReadOnly;
                    if (Info.Attributes.HasFlag(System.IO.FileAttributes.Hidden)) attrs |= VirtualFileSystem.FileAttributes.Hidden;
                    if (Info.Attributes.HasFlag(System.IO.FileAttributes.Directory)) attrs |= VirtualFileSystem.FileAttributes.Directory;
                    if (Info.Attributes.HasFlag(System.IO.FileAttributes.Archive)) attrs |= VirtualFileSystem.FileAttributes.Archive;

                    return attrs; 
                }
            }

            public DateTime CreationTime
            {
                get { return Info.CreationTime; }
            }

            public DateTime LastAccessTime
            {
                get { return Info.LastAccessTime; }
            }

            public DateTime LastWriteTime
            {
                get { return Info.LastWriteTime; }
            }
        }
        class DirInfo : ElementInfo, IDirectoryInfo
        {
            public DirInfo(DirectoryInfo dInfo)
                : base(dInfo)
            {
            }
        }
        class FilInfo : ElementInfo, IFileInfo
        {
            protected FileInfo FInfo { get { return (FileInfo)Info; } }
            public FilInfo(FileInfo fInfo)
                : base(fInfo)
            {
            }
            public long Length
            {
                get { return FInfo.Length; }
            }
        }

        public System.Collections.Generic.IEnumerable<string> GetFiles(string directory)
        {
            return Directory.GetFiles(directory);
        }

        public System.Collections.Generic.IEnumerable<string> GetDirectories(string directory)
        {
            return Directory.GetDirectories(directory);
        }

        public string GetFullPath(string path)
        {
            return Path.GetFullPath(path);
        }

        public IDirectoryInfo GetDirectoryInfo(string directoryName)
        {
            return new DirInfo(new DirectoryInfo(directoryName));
        }

        public IFileInfo GetFileInfo(string filename)
        {
            return new FilInfo(new FileInfo(filename));
        }

        public void SetLastWriteTime(string name, DateTime dateTime)
        {
            File.SetLastWriteTime(name, dateTime);
        }

        public void SetAttributes(string name, VirtualFileSystem.FileAttributes attributes)
        {
            System.IO.FileAttributes attrs = 0;
            if (attributes.HasFlag(VirtualFileSystem.FileAttributes.Normal)) attrs |= System.IO.FileAttributes.Normal;
            if (attributes.HasFlag(VirtualFileSystem.FileAttributes.ReadOnly)) attrs |= System.IO.FileAttributes.ReadOnly;
            if (attributes.HasFlag(VirtualFileSystem.FileAttributes.Hidden)) attrs |= System.IO.FileAttributes.Hidden;
            if (attributes.HasFlag(VirtualFileSystem.FileAttributes.Directory)) attrs |= System.IO.FileAttributes.Directory;
            if (attributes.HasFlag(VirtualFileSystem.FileAttributes.Archive)) attrs |= System.IO.FileAttributes.Archive;
            File.SetAttributes(name, attrs);
        }

        public void CreateDirectory(string directory)
        {
            Directory.CreateDirectory(directory);
        }

        public string GetTempFileName()
        {
            return Path.GetTempFileName();
        }

        public void CopyFile(string fromFileName, string toFileName, bool overwrite)
        {
            File.Copy(fromFileName, toFileName, overwrite);
        }

        public void MoveFile(string fromFileName, string toFileName)
        {
            File.Move(fromFileName, toFileName);
        }

        public void DeleteFile(string fileName)
        {
            File.Delete(fileName);
        }

        public VfsStream CreateFile(string filename)
        {
            return new VfsProxyStream(new FileStream(filename, FileMode.Create, FileAccess.ReadWrite, FileShare.Read), filename);
        }

        public VfsStream OpenReadFile(string filename)
        {
            return new VfsProxyStream(new FileStream(filename, FileMode.Open, FileAccess.ReadWrite, FileShare.Read), filename);
        }

        public VfsStream OpenWriteFile(string filename)
        {
            return new VfsProxyStream(File.OpenWrite(filename), filename);
        }

        public string CurrentDirectory
        {
            get { return Environment.CurrentDirectory; }
        }

        public char DirectorySeparatorChar
        {
            get { return Path.DirectorySeparatorChar; }
        }
    }
```
