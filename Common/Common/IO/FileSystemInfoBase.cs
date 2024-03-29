﻿using System;using System.IO;

namespace OculiService.Common.IO
{
  public abstract class FileSystemInfoBase
  {
    public abstract FileAttributes Attributes { get; set; }

    public abstract DateTime CreationTime { get; set; }

    public abstract DateTime CreationTimeUtc { get; set; }

    public abstract bool Exists { get; }

    public abstract string Extension { get; }

    public abstract string FullName { get; }

    public abstract DateTime LastAccessTime { get; set; }

    public abstract DateTime LastAccessTimeUtc { get; set; }

    public abstract DateTime LastWriteTime { get; set; }

    public abstract DateTime LastWriteTimeUtc { get; set; }

    public abstract string Name { get; }

    public abstract void Delete();

    public abstract void Refresh();
  }
}
