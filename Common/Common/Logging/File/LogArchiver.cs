using OculiService.Common.IO;
using OculiService.Common.Logging.File.Interface;
using System;
using System.Linq;

namespace OculiService.Common.Logging.File
{
  public sealed class LogArchiver : ILogArchiver
  {
    private static readonly int MaxArchivesToKeep = 10;
    private readonly IFileSystem _fileSystem;
    private readonly ILogArchive _logArchive;

    public LogArchiver(ILogArchive logArchive, IFileSystem fileSystem)
    {
      this._logArchive = logArchive;
      this._fileSystem = fileSystem;
    }

    public void ArchiveOldFormatLog(string path)
    {
      if (!this._logArchive.IsOldLogFormat(path))
        return;
      this.TryArchive(path);
    }

    private void TryRemoveOldArchive(string path)
    {
      try
      {
        this._fileSystem.File.Delete(path);
      }
      catch
      {
      }
    }

    public bool TryArchive(string path)
    {
      try
      {
        string archivePath = this._logArchive.GetArchivePath(path);
        this._fileSystem.File.Copy(path, archivePath);
      }
      catch
      {
        return false;
      }
      try
      {
        this._logArchive.GetArchivesToRemove(path, LogArchiver.MaxArchivesToKeep).ForEach<string>((Action<string>) (p => this.TryRemoveOldArchive(p)));
      }
      catch
      {
      }
      return true;
    }
  }
}
