using System;using System.Collections.Generic;
using System.Diagnostics;

namespace OculiService.Common.Diagnostics
{
  public interface IEventLog : IDisposable
  {
    IList<EventLogEntryData> Entries { get; }

    string Log { get; set; }

    string LogDisplayName { get; }

    string MachineName { get; set; }

    long MaximumKilobytes { get; set; }

    int MinimumRetentionDays { get; }

    OverflowAction OverflowAction { get; }

    string Source { get; set; }

    IObservable<EventLogEntryData> WhenEntryWritten { get; }

    void Clear();

    void Close();

    void ModifyOverflowPolicy(OverflowAction action, int retentionDays);

    void RegisterDisplayName(string resourceFile, long resourceId);

    void WriteEntry(string message, EventLogEntryType type, int eventId, short category, byte[] data);

    void WriteEvent(EventInstance instance, byte[] data, params object[] values);
  }
}
