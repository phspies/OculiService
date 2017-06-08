using System.Collections.Generic;

namespace OculiService.Common.Logging.File.Interface
{
  public interface ILogArchive
  {
    bool IsOldLogFormat(string path);

    string GetArchivePath(string path);

    IEnumerable<string> GetArchivesToRemove(string path, int maxArchivesToKeep);
  }
}
