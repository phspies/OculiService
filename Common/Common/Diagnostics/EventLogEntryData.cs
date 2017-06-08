using System;using System.Diagnostics;

namespace OculiService.Common.Diagnostics
{
  public abstract class EventLogEntryData
  {
    public abstract string Category { get; }

    public abstract short CategoryNumber { get; }

    public abstract EventLogEntryType EntryType { get; }

    public abstract int Index { get; }

    public abstract long InstanceId { get; }

    public abstract string MachineName { get; }

    public abstract string Message { get; }

    public abstract string[] ReplacementStrings { get; }

    public abstract string Source { get; }

    public abstract DateTime TimeGenerated { get; }

    public abstract DateTime TimeWritten { get; }

    public abstract string UserName { get; }
  }
}
