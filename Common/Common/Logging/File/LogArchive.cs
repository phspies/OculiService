using OculiService.Common.IO;
using OculiService.Common.Logging.File.Interface;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;

namespace OculiService.Common.Logging.File
{
  public sealed class LogArchive : ILogArchive
  {
    private static readonly string ArchiveDateTimeFormat = "yyyyMMdd";
    private static readonly string ArchiveRegexPattern = "{0}\\.(?<timeStamp>\\d{{8}})(\\.(?<sequence>\\d+))?{1}";
    private static readonly string SequenceGroupName = "sequence";
    private IFileSystem _fileSystem;

    public LogArchive(IFileSystem fileSystem)
    {
      this._fileSystem = fileSystem;
    }

    public string GetArchivePath(string path)
    {
      Invariant.ArgumentNotNull((object) path, "path");
      string str = DateTime.Now.ToString(LogArchive.ArchiveDateTimeFormat, (IFormatProvider) CultureInfo.InvariantCulture);
      int maxSequenceNumber = this.GetMaxSequenceNumber(path);
      string extension = string.Format("{0}{1}{2}", (object) str, maxSequenceNumber == -1 ? (object) "" : (object) ("." + (object) (maxSequenceNumber + 1)), (object) Path.GetExtension(path));
      return Path.ChangeExtension(path, extension);
    }

    public IEnumerable<string> GetArchivesToRemove(string path, int maxArchivesToKeep)
    {
      Invariant.ArgumentNotNull((object) path, "path");
      try
      {
        return this.GetAllArchives(path).OrderByDescending<string, int>((Func<string, int>) (p => this.GetArchiveSequenceNumber(p, Path.GetFileNameWithoutExtension(path)))).Skip<string>(maxArchivesToKeep);
      }
      catch
      {
        return Enumerable.Empty<string>();
      }
    }

    public bool IsOldLogFormat(string path)
    {
      Invariant.ArgumentNotNull((object) path, "path");
      try
      {
        using (StreamReader streamReader = new StreamReader(this._fileSystem.File.OpenRead(path)))
        {
          string str = streamReader.ReadLine();
          if (str == null)
            return false;
          DateTime result;
          return DateTime.TryParse(str.Substring(0, 19), out result);
        }
      }
      catch
      {
        return false;
      }
    }

    private IEnumerable<string> GetAllArchives(string path)
    {
      return ((IEnumerable<string>) this._fileSystem.Directory.GetFiles(Path.GetDirectoryName(path), Path.GetFileNameWithoutExtension(path) + ".*", SearchOption.TopDirectoryOnly)).Where<string>((Func<string, bool>) (p => p != null)).Where<string>((Func<string, bool>) (p => this.IsArchivePath(p, Path.GetFileNameWithoutExtension(path))));
    }

    private bool IsArchivePath(string path, string baseName)
    {
      return this.GetRegexFromBaseName(baseName).IsMatch(path);
    }

    private Regex GetRegexFromBaseName(string baseName)
    {
      return new Regex(string.Format((IFormatProvider) CultureInfo.InvariantCulture, LogArchive.ArchiveRegexPattern, new object[2]{ (object) Regex.Escape(baseName), (object) Regex.Escape(Path.GetExtension(baseName)) }));
    }

    private int GetArchiveSequenceNumber(string path, string baseName)
    {
      try
      {
        Group group = this.GetRegexFromBaseName(baseName).Match(path).Groups[LogArchive.SequenceGroupName];
        int result = int.MaxValue;
        int.TryParse(group.Value, out result);
        return result;
      }
      catch
      {
        return int.MaxValue;
      }
    }

    private int GetMaxSequenceNumber(string path)
    {
      try
      {
        return this.GetAllArchives(path).Select<string, int>((Func<string, int>) (p => this.GetArchiveSequenceNumber(p, Path.GetFileNameWithoutExtension(path)))).Max();
      }
      catch
      {
        return -1;
      }
    }
  }
}
