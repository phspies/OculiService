using System;using System.IO;
using System.Security.AccessControl;

namespace OculiService.Common.IO
{
  internal sealed class FileInfoAdapter : FileInfoBase
  {
    private readonly FileInfo instance;

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

    public override DirectoryInfoBase Directory
    {
      get
      {
        return (DirectoryInfoBase) new DirectoryInfoAdapter(this.instance.Directory);
      }
    }

    public override string DirectoryName
    {
      get
      {
        return this.instance.DirectoryName;
      }
    }

    public override bool IsReadOnly
    {
      get
      {
        return this.instance.IsReadOnly;
      }
      set
      {
        this.instance.IsReadOnly = value;
      }
    }

    public override long Length
    {
      get
      {
        return this.instance.Length;
      }
    }

    public FileInfoAdapter(FileInfo instance)
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

    public override StreamWriter AppendText()
    {
      return this.instance.AppendText();
    }

    public override FileInfoBase CopyTo(string destFileName)
    {
      return (FileInfoBase) new FileInfoAdapter(this.instance.CopyTo(destFileName));
    }

    public override FileInfoBase CopyTo(string destFileName, bool overwrite)
    {
      return (FileInfoBase) new FileInfoAdapter(this.instance.CopyTo(destFileName, overwrite));
    }

    public override Stream Create()
    {
      return (Stream) this.instance.Create();
    }

    public override StreamWriter CreateText()
    {
      return this.instance.CreateText();
    }

    public override void Decrypt()
    {
      this.instance.Decrypt();
    }

    public override void Encrypt()
    {
      this.instance.Encrypt();
    }

    public override FileSecurity GetAccessControl()
    {
      return this.instance.GetAccessControl();
    }

    public override FileSecurity GetAccessControl(AccessControlSections includeSections)
    {
      return this.instance.GetAccessControl(includeSections);
    }

    public override void MoveTo(string destFileName)
    {
      this.instance.MoveTo(destFileName);
    }

    public override Stream Open(FileMode mode)
    {
      return (Stream) this.instance.Open(mode);
    }

    public override Stream Open(FileMode mode, FileAccess access)
    {
      return (Stream) this.instance.Open(mode, access);
    }

    public override Stream Open(FileMode mode, FileAccess access, FileShare share)
    {
      return (Stream) this.instance.Open(mode, access, share);
    }

    public override Stream OpenRead()
    {
      return (Stream) this.instance.OpenRead();
    }

    public override StreamReader OpenText()
    {
      return this.instance.OpenText();
    }

    public override Stream OpenWrite()
    {
      return (Stream) this.instance.OpenWrite();
    }

    public override FileInfoBase Replace(string destinationFileName, string destinationBackupFileName)
    {
      return (FileInfoBase) new FileInfoAdapter(this.instance.Replace(destinationFileName, destinationBackupFileName));
    }

    public override FileInfoBase Replace(string destinationFileName, string destinationBackupFileName, bool ignoreMetadataErrors)
    {
      return (FileInfoBase) new FileInfoAdapter(this.instance.Replace(destinationFileName, destinationBackupFileName, ignoreMetadataErrors));
    }

    public override void SetAccessControl(FileSecurity fileSecurity)
    {
      this.instance.SetAccessControl(fileSecurity);
    }
  }
}
