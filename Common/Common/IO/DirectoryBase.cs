﻿using System;using System.IO;
using System.Security.AccessControl;

namespace OculiService.Common.IO
{
  public abstract class DirectoryBase
  {
    public abstract DirectoryInfoBase CreateDirectory(string path);

    public abstract DirectoryInfoBase CreateDirectory(string path, DirectorySecurity directorySecurity);

    public abstract void Delete(string path);

    public abstract void Delete(string path, bool recursive);

    public abstract bool Exists(string path);

    public abstract DirectorySecurity GetAccessControl(string path);

    public abstract DirectorySecurity GetAccessControl(string path, AccessControlSections includeSections);

    public abstract DateTime GetCreationTime(string path);

    public abstract DateTime GetCreationTimeUtc(string path);

    public abstract string GetCurrentDirectory();

    public abstract string[] GetDirectories(string path);

    public abstract string[] GetDirectories(string path, string searchPattern);

    public abstract string[] GetDirectories(string path, string searchPattern, SearchOption searchOption);

    public abstract string GetDirectoryRoot(string path);

    public abstract string[] GetFiles(string path);

    public abstract string[] GetFiles(string path, string searchPattern);

    public abstract string[] GetFiles(string path, string searchPattern, SearchOption searchOption);

    public abstract string[] GetFileSystemEntries(string path);

    public abstract string[] GetFileSystemEntries(string path, string searchPattern);

    public abstract DirectoryInfoBase GetInfo(string path);

    public abstract DateTime GetLastAccessTime(string path);

    public abstract DateTime GetLastAccessTimeUtc(string path);

    public abstract DateTime GetLastWriteTime(string path);

    public abstract DateTime GetLastWriteTimeUtc(string path);

    public abstract string[] GetLogicalDrives();

    public abstract DirectoryInfoBase GetParent(string path);

    public abstract void Move(string sourceDirName, string destDirName);

    public abstract void SetAccessControl(string path, DirectorySecurity directorySecurity);

    public abstract void SetCreationTime(string path, DateTime creationTime);

    public abstract void SetCreationTimeUtc(string path, DateTime creationTimeUtc);

    public abstract void SetCurrentDirectory(string path);

    public abstract void SetLastAccessTime(string path, DateTime lastAccessTime);

    public abstract void SetLastAccessTimeUtc(string path, DateTime lastAccessTimeUtc);

    public abstract void SetLastWriteTime(string path, DateTime lastWriteTime);

    public abstract void SetLastWriteTimeUtc(string path, DateTime lastWriteTimeUtc);
  }
}
