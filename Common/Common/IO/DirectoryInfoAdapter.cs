using System;using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.AccessControl;

namespace OculiService.Common.IO
{
  internal sealed class DirectoryInfoAdapter : DirectoryInfoBase
  {
    private readonly DirectoryInfo instance;

    public override FileAttributes Attributes
    {
      get
      {
        return this.instance.Attributes;
      }
      set
      {
        this.instance.Attributes = value;
      }
    }

    public override DateTime CreationTime
    {
      get
      {
        return this.instance.CreationTime;
      }
      set
      {
        this.instance.CreationTime = value;
      }
    }

    public override DateTime CreationTimeUtc
    {
      get
      {
        return this.instance.CreationTimeUtc;
      }
      set
      {
        this.instance.CreationTimeUtc = value;
      }
    }

    public override bool Exists
    {
      get
      {
        return this.instance.Exists;
      }
    }

    public override string Extension
    {
      get
      {
        return this.instance.Extension;
      }
    }

    public override string FullName
    {
      get
      {
        return this.instance.FullName;
      }
    }

    public override DateTime LastAccessTime
    {
      get
      {
        return this.instance.LastAccessTime;
      }
      set
      {
        this.instance.LastAccessTime = value;
      }
    }

    public override DateTime LastAccessTimeUtc
    {
      get
      {
        return this.instance.LastAccessTimeUtc;
      }
      set
      {
        this.instance.LastAccessTimeUtc = value;
      }
    }

    public override DateTime LastWriteTime
    {
      get
      {
        return this.instance.LastWriteTime;
      }
      set
      {
        this.instance.LastWriteTime = value;
      }
    }

    public override DateTime LastWriteTimeUtc
    {
      get
      {
        return this.instance.LastWriteTimeUtc;
      }
      set
      {
        this.instance.LastWriteTimeUtc = value;
      }
    }

    public override string Name
    {
      get
      {
        return this.instance.Name;
      }
    }

    public override DirectoryInfoBase Parent
    {
      get
      {
        return (DirectoryInfoBase) new DirectoryInfoAdapter(this.instance.Parent);
      }
    }

    public override DirectoryInfoBase Root
    {
      get
      {
        return (DirectoryInfoBase) new DirectoryInfoAdapter(this.instance.Root);
      }
    }

    public DirectoryInfoAdapter(DirectoryInfo instance)
    {
      this.instance = instance;
    }

    public override void Delete()
    {
      this.instance.Delete();
    }

    public override void Refresh()
    {
      this.instance.Refresh();
    }

    public override void Create()
    {
      this.instance.Create();
    }

    public override void Create(DirectorySecurity directorySecurity)
    {
      this.instance.Create(directorySecurity);
    }

    public override DirectoryInfoBase CreateSubdirectory(string path)
    {
      return (DirectoryInfoBase) new DirectoryInfoAdapter(this.instance.CreateSubdirectory(path));
    }

    public override DirectoryInfoBase CreateSubdirectory(string path, DirectorySecurity directorySecurity)
    {
      return (DirectoryInfoBase) new DirectoryInfoAdapter(this.instance.CreateSubdirectory(path, directorySecurity));
    }

    public override void Delete(bool recursive)
    {
      this.instance.Delete(recursive);
    }

    public override DirectorySecurity GetAccessControl()
    {
      return this.instance.GetAccessControl();
    }

    public override DirectorySecurity GetAccessControl(AccessControlSections includeSections)
    {
      return this.instance.GetAccessControl(includeSections);
    }

    public override DirectoryInfoBase[] GetDirectories()
    {
      return (DirectoryInfoBase[]) ((IEnumerable<DirectoryInfo>) this.instance.GetDirectories()).Select<DirectoryInfo, DirectoryInfoAdapter>((Func<DirectoryInfo, DirectoryInfoAdapter>) (di => new DirectoryInfoAdapter(di))).ToArray<DirectoryInfoAdapter>();
    }

    public override DirectoryInfoBase[] GetDirectories(string searchPattern)
    {
      return (DirectoryInfoBase[]) ((IEnumerable<DirectoryInfo>) this.instance.GetDirectories(searchPattern)).Select<DirectoryInfo, DirectoryInfoAdapter>((Func<DirectoryInfo, DirectoryInfoAdapter>) (di => new DirectoryInfoAdapter(di))).ToArray<DirectoryInfoAdapter>();
    }

    public override DirectoryInfoBase[] GetDirectories(string searchPattern, SearchOption searchOption)
    {
      return (DirectoryInfoBase[]) ((IEnumerable<DirectoryInfo>) this.instance.GetDirectories(searchPattern, searchOption)).Select<DirectoryInfo, DirectoryInfoAdapter>((Func<DirectoryInfo, DirectoryInfoAdapter>) (di => new DirectoryInfoAdapter(di))).ToArray<DirectoryInfoAdapter>();
    }

    public override FileInfoBase[] GetFiles()
    {
      return (FileInfoBase[]) ((IEnumerable<FileInfo>) this.instance.GetFiles()).Select<FileInfo, FileInfoAdapter>((Func<FileInfo, FileInfoAdapter>) (fi => new FileInfoAdapter(fi))).ToArray<FileInfoAdapter>();
    }

    public override FileInfoBase[] GetFiles(string searchPattern)
    {
      return (FileInfoBase[]) ((IEnumerable<FileInfo>) this.instance.GetFiles(searchPattern)).Select<FileInfo, FileInfoAdapter>((Func<FileInfo, FileInfoAdapter>) (fi => new FileInfoAdapter(fi))).ToArray<FileInfoAdapter>();
    }

    public override FileInfoBase[] GetFiles(string searchPattern, SearchOption searchOption)
    {
      return (FileInfoBase[]) ((IEnumerable<FileInfo>) this.instance.GetFiles(searchPattern, searchOption)).Select<FileInfo, FileInfoAdapter>((Func<FileInfo, FileInfoAdapter>) (fi => new FileInfoAdapter(fi))).ToArray<FileInfoAdapter>();
    }

    public override FileSystemInfoBase[] GetFileSystemInfos()
    {
      return ((IEnumerable<FileSystemInfo>) this.instance.GetFileSystemInfos()).Select<FileSystemInfo, FileSystemInfoBase>((Func<FileSystemInfo, FileSystemInfoBase>) (fsi =>
      {
        if ((fsi.Attributes & FileAttributes.Directory) != (FileAttributes) 0)
          return (FileSystemInfoBase) new DirectoryInfoAdapter(fsi as DirectoryInfo);
        return (FileSystemInfoBase) new FileInfoAdapter(fsi as FileInfo);
      })).ToArray<FileSystemInfoBase>();
    }

    public override FileSystemInfoBase[] GetFileSystemInfos(string searchPattern)
    {
      return ((IEnumerable<FileSystemInfo>) this.instance.GetFileSystemInfos(searchPattern)).Select<FileSystemInfo, FileSystemInfoBase>((Func<FileSystemInfo, FileSystemInfoBase>) (fsi =>
      {
        if ((fsi.Attributes & FileAttributes.Directory) != (FileAttributes) 0)
          return (FileSystemInfoBase) new DirectoryInfoAdapter(fsi as DirectoryInfo);
        return (FileSystemInfoBase) new FileInfoAdapter(fsi as FileInfo);
      })).ToArray<FileSystemInfoBase>();
    }

    public override void MoveTo(string destDirName)
    {
      this.instance.MoveTo(destDirName);
    }

    public override void SetAccessControl(DirectorySecurity directorySecurity)
    {
      this.instance.SetAccessControl(directorySecurity);
    }
  }
}
