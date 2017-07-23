using System;

namespace OculiService.Common.Interfaces
{
  public interface IDirectoryCopyCommand : ITaskCommandBase
  {
    void Invoke(string sourceDirName, string destDirName, bool copySubDir);

    void Invoke(string sourceDirName, string destDirName, bool copySubDir, Func<string, bool> cond);
  }
}
