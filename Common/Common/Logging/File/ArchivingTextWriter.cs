using OculiService.Common.IO;
using OculiService.Common.Logging.File.Interface;
using System.IO;
using System.Text;

namespace OculiService.Common.Logging.File
{
  public sealed class ArchivingTextWriter : IArchivingTextWriter
  {
    private readonly object _syncObj = new object();
    private readonly int _maxLogFileSizeBytes;
    private readonly ILogArchiver _archiver;
    private readonly string _path;
    private readonly IFileSystem _fileSystem;
    private StreamWriter _textWriter;

    private TextWriter TextWriter
    {
      get
      {
        lock (this._syncObj)
        {
          if (this._textWriter == null)
          {
            try
            {
              string local_2 = Path.GetDirectoryName(this._path);
              if (!this._fileSystem.Directory.Exists(local_2))
                this._fileSystem.Directory.CreateDirectory(local_2);
              this._textWriter = new StreamWriter(this._fileSystem.File.Open(this._path, FileMode.OpenOrCreate, FileAccess.Write, FileShare.Read, 4096, FileOptions.WriteThrough | FileOptions.SequentialScan), (Encoding) new UTF8Encoding(false), 4096);
              this._textWriter.BaseStream.Position = this._textWriter.BaseStream.Length;
              this._textWriter.AutoFlush = true;
            }
            catch
            {
              return (TextWriter) new NullTextWriter();
            }
          }
          if (this.ShouldArchive() && this._archiver.TryArchive(this._path))
            this.ResetWriterPosition();
          return (TextWriter) this._textWriter;
        }
      }
    }

    public ArchivingTextWriter(IFileSystem fileSystem, ILogArchiver archiver, string path, int maxLogFileSizeBytes)
    {
      this._path = path;
      this._fileSystem = fileSystem;
      this._archiver = archiver;
      this._maxLogFileSizeBytes = maxLogFileSizeBytes;
      this._archiver.ArchiveOldFormatLog(this._path);
    }

    public void Archive()
    {
      lock (this._syncObj)
      {
        if (!this._archiver.TryArchive(this._path))
          return;
        this.ResetWriterPosition();
      }
    }

    private bool ShouldArchive()
    {
      try
      {
        return this._textWriter.BaseStream.Position >= (long) this._maxLogFileSizeBytes;
      }
      catch
      {
        return false;
      }
    }

    private void ResetWriterPosition()
    {
      try
      {
        if (this._textWriter == null)
          return;
        this._textWriter.BaseStream.SetLength(0L);
        this._textWriter.Flush();
      }
      catch
      {
      }
    }

    public void Write(LogEntry logEntry)
    {
      Invariant.ArgumentNotNull((object) logEntry, "logEntry");
      try
      {
        this.TextWriter.WriteLine(logEntry.ToString());
      }
      catch
      {
      }
    }
  }
}
