﻿using System;using System.IO;
using System.Security.AccessControl;
using System.Text;

namespace OculiService.Common.IO
{
  internal sealed class FileAdapter : FileBase
  {
    private static readonly ITransactions _transactions = (ITransactions) new OculiService.Common.IO.Transactions();

    public override ITransactions Transactions
    {
      get
      {
        return FileAdapter._transactions;
      }
    }

    public override void AppendAllText(string path, string contents)
    {
      File.AppendAllText(path, contents);
    }

    public override void AppendAllText(string path, string contents, Encoding encoding)
    {
      File.AppendAllText(path, contents, encoding);
    }

    public override StreamWriter AppendText(string path)
    {
      return File.AppendText(path);
    }

    public override void Copy(string sourceFileName, string destFileName)
    {
      File.Copy(sourceFileName, destFileName);
    }

    public override void Copy(string sourceFileName, string destFileName, bool overwrite)
    {
      File.Copy(sourceFileName, destFileName, overwrite);
    }

    public override Stream Create(string path)
    {
      return (Stream) File.Create(path);
    }

    public override Stream Create(string path, int bufferSize)
    {
      return (Stream) File.Create(path, bufferSize);
    }

    public override Stream Create(string path, int bufferSize, FileOptions options)
    {
      return (Stream) File.Create(path, bufferSize, options);
    }

    public override Stream Create(string path, int bufferSize, FileOptions options, FileSecurity fileSecurity)
    {
      return (Stream) File.Create(path, bufferSize, options, fileSecurity);
    }

    public override StreamWriter CreateText(string path)
    {
      return File.CreateText(path);
    }

    public override void Decrypt(string path)
    {
      File.Decrypt(path);
    }

    public override void Delete(string path)
    {
      File.Delete(path);
    }

    public override void Encrypt(string path)
    {
      File.Encrypt(path);
    }

    public override bool Exists(string path)
    {
      return File.Exists(path);
    }

    public override FileSecurity GetAccessControl(string path)
    {
      return File.GetAccessControl(path);
    }

    public override FileSecurity GetAccessControl(string path, AccessControlSections includeSections)
    {
      return File.GetAccessControl(path, includeSections);
    }

    public override FileAttributes GetAttributes(string path)
    {
      return File.GetAttributes(path);
    }

    public override DateTime GetCreationTime(string path)
    {
      return File.GetCreationTime(path);
    }

    public override DateTime GetCreationTimeUtc(string path)
    {
      return File.GetCreationTimeUtc(path);
    }

    public override FileInfoBase GetInfo(string fileName)
    {
      return (FileInfoBase) new FileInfoAdapter(new FileInfo(fileName));
    }

    public override DateTime GetLastAccessTime(string path)
    {
      return File.GetLastAccessTime(path);
    }

    public override DateTime GetLastAccessTimeUtc(string path)
    {
      return File.GetLastAccessTimeUtc(path);
    }

    public override DateTime GetLastWriteTime(string path)
    {
      return File.GetLastWriteTime(path);
    }

    public override DateTime GetLastWriteTimeUtc(string path)
    {
      return File.GetLastWriteTimeUtc(path);
    }

    public override void Move(string sourceFileName, string destFileName)
    {
      File.Move(sourceFileName, destFileName);
    }

    public override void Move(string sourceFileName, string destFileName, FileBase.MoveFileFlags flags)
    {
      FileBase.MoveFileEx(sourceFileName, destFileName, flags);
    }

    public override Stream Open(string path, FileMode mode)
    {
      return (Stream) File.Open(path, mode);
    }

    public override Stream Open(string path, FileMode mode, FileAccess access)
    {
      return (Stream) File.Open(path, mode, access);
    }

    public override Stream Open(string path, FileMode mode, FileAccess access, FileShare share)
    {
      return (Stream) File.Open(path, mode, access, share);
    }

    public override Stream Open(string path, FileMode mode, FileAccess access, FileShare share, int bufferSize, FileOptions options)
    {
      return (Stream) new FileStream(path, mode, access, share, bufferSize, options);
    }

    public override Stream OpenRead(string path)
    {
      return (Stream) File.OpenRead(path);
    }

    public override StreamReader OpenText(string path)
    {
      return File.OpenText(path);
    }

    public override Stream OpenWrite(string path)
    {
      return (Stream) File.OpenWrite(path);
    }

    public override byte[] ReadAllBytes(string path)
    {
      return File.ReadAllBytes(path);
    }

    public override string[] ReadAllLines(string path)
    {
      return File.ReadAllLines(path);
    }

    public override string[] ReadAllLines(string path, Encoding encoding)
    {
      return File.ReadAllLines(path, encoding);
    }

    public override string ReadAllText(string path)
    {
      return File.ReadAllText(path);
    }

    public override string ReadAllText(string path, Encoding encoding)
    {
      return File.ReadAllText(path, encoding);
    }

    public override void Replace(string sourceFileName, string destinationFileName, string destinationBackupFileName)
    {
      File.Replace(sourceFileName, destinationFileName, destinationBackupFileName);
    }

    public override void Replace(string sourceFileName, string destinationFileName, string destinationBackupFileName, bool ignoreMetadataErrors)
    {
      File.Replace(sourceFileName, destinationFileName, destinationBackupFileName);
    }

    public override void SetAccessControl(string path, FileSecurity fileSecurity)
    {
      File.SetAccessControl(path, fileSecurity);
    }

    public override void SetAttributes(string path, FileAttributes fileAttributes)
    {
      File.SetAttributes(path, fileAttributes);
    }

    public override void SetCreationTime(string path, DateTime creationTime)
    {
      File.SetCreationTime(path, creationTime);
    }

    public override void SetCreationTimeUtc(string path, DateTime creationTimeUtc)
    {
      File.SetCreationTimeUtc(path, creationTimeUtc);
    }

    public override void SetLastAccessTime(string path, DateTime lastAccessTime)
    {
      File.SetLastAccessTime(path, lastAccessTime);
    }

    public override void SetLastAccessTimeUtc(string path, DateTime lastAccessTimeUtc)
    {
      File.SetLastAccessTimeUtc(path, lastAccessTimeUtc);
    }

    public override void SetLastWriteTime(string path, DateTime lastWriteTime)
    {
      File.SetLastWriteTime(path, lastWriteTime);
    }

    public override void SetLastWriteTimeUtc(string path, DateTime lastWriteTimeUtc)
    {
      File.SetLastWriteTimeUtc(path, lastWriteTimeUtc);
    }

    public override void WriteAllBytes(string path, byte[] bytes)
    {
      File.WriteAllBytes(path, bytes);
    }

    public override void WriteAllLines(string path, string[] contents)
    {
      File.WriteAllLines(path, contents);
    }

    public override void WriteAllLines(string path, string[] contents, Encoding encoding)
    {
      File.WriteAllLines(path, contents, encoding);
    }

    public override void WriteAllText(string path, string contents)
    {
      File.WriteAllText(path, contents);
    }

    public override void WriteAllText(string path, string contents, Encoding encoding)
    {
      File.WriteAllText(path, contents, encoding);
    }
  }
}
