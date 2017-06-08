using System;using System.IO;
using System.Reactive;
using System.Reactive.Linq;

namespace OculiService.Common.IO
{
  public class FileSystemObserver : IFileSystemObserver
  {
    private FileSystemWatcher watcher;
    private IObservable<FileSystemEventArgs> changes;

    public string Filter
    {
      get
      {
        return this.watcher.Filter;
      }
      set
      {
        this.watcher.Filter = value;
      }
    }

    public NotifyFilters NotifyFilter
    {
      get
      {
        return this.watcher.NotifyFilter;
      }
      set
      {
        this.watcher.NotifyFilter = value;
      }
    }

    public bool Observing
    {
      get
      {
        return this.watcher.EnableRaisingEvents;
      }
      set
      {
        this.watcher.EnableRaisingEvents = value;
      }
    }

    public string Path
    {
      get
      {
        return this.watcher.Path;
      }
      set
      {
        this.watcher.Path = value;
      }
    }

    public IObservable<FileSystemEventArgs> Changes
    {
      get
      {
        return this.changes;
      }
    }

    public FileSystemObserver()
    {
      this.watcher = new FileSystemWatcher();
      IObservable<EventPattern<FileSystemEventArgs>> observable1 = Observable.FromEventPattern<FileSystemEventArgs>((object) this.watcher, "Changed");
      IObservable<EventPattern<FileSystemEventArgs>> observable2 = Observable.FromEventPattern<FileSystemEventArgs>((object) this.watcher, "Created");
      IObservable<EventPattern<FileSystemEventArgs>> observable3 = Observable.FromEventPattern<FileSystemEventArgs>((object) this.watcher, "Deleted");
      IObservable<EventPattern<RenamedEventArgs>> source = Observable.FromEventPattern<RenamedEventArgs>((object) this.watcher, "Renamed");
      this.changes = Observable.Merge<EventPattern<FileSystemEventArgs>>(new IObservable<EventPattern<FileSystemEventArgs>>[3]
      {
        observable1,
        observable2,
        observable3
      }).Select<EventPattern<FileSystemEventArgs>, FileSystemEventArgs>((Func<EventPattern<FileSystemEventArgs>, FileSystemEventArgs>) (e => e.EventArgs)).Merge<FileSystemEventArgs>(source.Select<EventPattern<RenamedEventArgs>, FileSystemEventArgs>((Func<EventPattern<RenamedEventArgs>, FileSystemEventArgs>) (e => (FileSystemEventArgs) e.EventArgs)));
    }
  }
}
