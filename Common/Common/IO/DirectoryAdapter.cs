﻿using System;using System.IO;
using System.Security.AccessControl;

namespace OculiService.Common.IO
{
  internal sealed class DirectoryAdapter : DirectoryBase
  {
    public override DirectoryInfoBase CreateDirectory(string path)
    {
      return (DirectoryInfoBase) new DirectoryInfoAdapter(Directory.CreateDirectory(path));
    }

    public override DirectoryInfoBase CreateDirectory(string path, DirectorySecurity directorySecurity)
    {
      return (DirectoryInfoBase) new DirectoryInfoAdapter(Directory.CreateDirectory(path, directorySecurity));
    }

    public override void Delete(string path)
    {
      Directory.Delete(path);
    }

    public override void Delete(string path, bool recursive)
    {
      Directory.Delete(path, recursive);
    }

    public override bool Exists(string path)
    {
      return Directory.Exists(path);
    }

    public override DirectorySecurity GetAccessControl(string path)
    {
      return Directory.GetAccessControl(path);
    }

    public override DirectorySecurity GetAccessControl(string path, AccessControlSections includeSections)
    {
      return Directory.GetAccessControl(path, includeSections);
    }

    public override DateTime GetCreationTime(string path)
    {
      return Directory.GetCreationTime(path);
    }

    public override DateTime GetCreationTimeUtc(string path)
    {
      return Directory.GetCreationTimeUtc(path);
    }

    public override string GetCurrentDirectory()
    {
      return Directory.GetCurrentDirectory();
    }

    public override string[] GetDirectories(string path)
    {
      return Directory.GetDirectories(path);
    }

    public override string[] GetDirectories(string path, string searchPattern)
    {
      return Directory.GetDirectories(path, searchPattern);
    }

    public override string[] GetDirectories(string path, string searchPattern, SearchOption searchOption)
    {
      return Directory.GetDirectories(path, searchPattern, searchOption);
    }

    public override string GetDirectoryRoot(string path)
    {
      return Directory.GetDirectoryRoot(path);
    }

    public override string[] GetFiles(string path)
    {
      return Directory.GetFiles(path);
    }

    public override string[] GetFiles(string path, string searchPattern)
    {
      return Directory.GetFiles(path, searchPattern);
    }

    public override string[] GetFiles(string path, string searchPattern, SearchOption searchOption)
    {
      return Directory.GetFiles(path, searchPattern, searchOption);
    }

    public override string[] GetFileSystemEntries(string path)
    {
      return Directory.GetFileSystemEntries(path);
    }

    public override string[] GetFileSystemEntries(string path, string searchPattern)
    {
      return Directory.GetFileSystemEntries(path, searchPattern);
    }

    public override DirectoryInfoBase GetInfo(string path)
    {
      return (DirectoryInfoBase) new DirectoryInfoAdapter(new DirectoryInfo(path));
    }

    public override DateTime GetLastAccessTime(string path)
    {
      return Directory.GetLastAccessTime(path);
    }

    public override DateTime GetLastAccessTimeUtc(string path)
    {
      return Directory.GetLastAccessTimeUtc(path);
    }

    public override DateTime GetLastWriteTime(string path)
    {
      return Directory.GetLastWriteTime(path);
    }

    public override DateTime GetLastWriteTimeUtc(string path)
    {
      return Directory.GetLastWriteTimeUtc(path);
    }

    public override string[] GetLogicalDrives()
    {
      return Directory.GetLogicalDrives();
    }

    public override DirectoryInfoBase GetParent(string path)
    {
      return (DirectoryInfoBase) Directory.GetParent(path);
    }

    public override void Move(string sourceDirName, string destDirName)
    {
      Directory.Move(sourceDirName, destDirName);
    }

    public override void SetAccessControl(string path, DirectorySecurity directorySecurity)
    {
      Directory.SetAccessControl(path, directorySecurity);
    }

    public override void SetCreationTime(string path, DateTime creationTime)
    {
      Directory.SetCreationTime(path, creationTime);
    }

    public override void SetCreationTimeUtc(string path, DateTime creationTimeUtc)
    {
      Directory.SetCreationTimeUtc(path, creationTimeUtc);
    }

    public override void SetCurrentDirectory(string path)
    {
      Directory.SetCurrentDirectory(path);
    }

    public override void SetLastAccessTime(string path, DateTime lastAccessTime)
    {
      Directory.SetLastAccessTime(path, lastAccessTime);
    }

    public override void SetLastAccessTimeUtc(string path, DateTime lastAccessTimeUtc)
    {
      Directory.SetLastAccessTimeUtc(path, lastAccessTimeUtc);
    }

    public override void SetLastWriteTime(string path, DateTime lastWriteTime)
    {
      Directory.SetLastWriteTime(path, lastWriteTime);
    }

    public override void SetLastWriteTimeUtc(string path, DateTime lastWriteTimeUtc)
    {
      Directory.SetLastWriteTimeUtc(path, lastWriteTimeUtc);
    }
  }
}
