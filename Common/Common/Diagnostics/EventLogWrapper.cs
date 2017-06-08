using System;using System.Collections.Generic;
using System.Diagnostics;
using System.Reactive;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using System.Threading;

namespace OculiService.Common.Diagnostics
{
  internal class EventLogWrapper : IEventLog, IDisposable
  {
    private EventLog eventLog;
    private EventLogEntryDataCollection entries;
    private IObservable<EventLogEntryData> entrySource;
    private int subscriptions;

    public IList<EventLogEntryData> Entries
    {
      get
      {
        return (IList<EventLogEntryData>) this.entries;
      }
    }

    public string Log
    {
      get
      {
        return this.eventLog.Log;
      }
      set
      {
        this.eventLog.Log = value;
      }
    }

    public string LogDisplayName
    {
      get
      {
        return this.eventLog.LogDisplayName;
      }
    }

    public string MachineName
    {
      get
      {
        return this.eventLog.MachineName;
      }
      set
      {
        this.eventLog.MachineName = value;
      }
    }

    public long MaximumKilobytes
    {
      get
      {
        return this.eventLog.MaximumKilobytes;
      }
      set
      {
        this.eventLog.MaximumKilobytes = value;
      }
    }

    public int MinimumRetentionDays
    {
      get
      {
        return this.eventLog.MinimumRetentionDays;
      }
    }

    public OverflowAction OverflowAction
    {
      get
      {
        return this.eventLog.OverflowAction;
      }
    }

    public string Source
    {
      get
      {
        return this.eventLog.Source;
      }
      set
      {
        this.eventLog.Source = value;
      }
    }

    public IObservable<EventLogEntryData> WhenEntryWritten
    {
      get
      {
        return Observable.Create<EventLogEntryData>((Func<IObserver<EventLogEntryData>, IDisposable>) (observer =>
        {
          if (Interlocked.Increment(ref this.subscriptions) == 1)
            this.eventLog.EnableRaisingEvents = true;
          IDisposable disposable = Disposable.Create((Action) (() =>
          {
            if (Interlocked.Decrement(ref this.subscriptions) != 0)
              return;
            this.eventLog.EnableRaisingEvents = false;
          }));
          return (IDisposable) new CompositeDisposable(new IDisposable[2]{ this.entrySource.Subscribe(observer), disposable });
        }));
      }
    }

    public EventLogWrapper(EventLog eventLog)
    {
      this.eventLog = eventLog;
      this.entries = new EventLogEntryDataCollection(this.eventLog.Entries);
      this.entrySource = Observable.FromEventPattern<EntryWrittenEventArgs>((object) this.eventLog, "EntryWritten").Where<EventPattern<EntryWrittenEventArgs>>((Func<EventPattern<EntryWrittenEventArgs>, bool>) (pattern => pattern.EventArgs.Entry != null)).Select<EventPattern<EntryWrittenEventArgs>, EventLogEntryData>((Func<EventPattern<EntryWrittenEventArgs>, EventLogEntryData>) (pattern => (EventLogEntryData) new EventLogEntryWrapper(pattern.EventArgs.Entry)));
    }

    public void Clear()
    {
      this.eventLog.Clear();
    }

    public void Close()
    {
      this.eventLog.Close();
    }

    public void ModifyOverflowPolicy(OverflowAction action, int retentionDays)
    {
      this.eventLog.ModifyOverflowPolicy(action, retentionDays);
    }

    public void RegisterDisplayName(string resourceFile, long resourceId)
    {
      this.eventLog.RegisterDisplayName(resourceFile, resourceId);
    }

    public void WriteEntry(string message, EventLogEntryType type, int eventId, short category, byte[] data)
    {
      this.eventLog.WriteEntry(message, type, eventId, category, data);
    }

    public void WriteEvent(EventInstance instance, byte[] data, params object[] values)
    {
      this.eventLog.WriteEvent(instance, data, values);
    }

    public void Dispose()
    {
      this.eventLog.Dispose();
    }
  }
}
