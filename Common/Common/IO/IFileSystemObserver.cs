using System;using System.IO;

namespace OculiService.Common.IO
{
  public interface IFileSystemObserver
  {
    string Filter { get; set; }

    NotifyFilters NotifyFilter { get; set; }

    bool Observing { get; set; }

    string Path { get; set; }

    IObservable<FileSystemEventArgs> Changes { get; }
  }
}
