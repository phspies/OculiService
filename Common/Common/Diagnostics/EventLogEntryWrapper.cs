using System;using System.Diagnostics;

namespace OculiService.Common.Diagnostics
{
  internal class EventLogEntryWrapper : EventLogEntryData
  {
    private EventLogEntry entry;

    public override string Category
    {
      get
      {
        return this.entry.Category;
      }
    }

    public override short CategoryNumber
    {
      get
      {
        return this.entry.CategoryNumber;
      }
    }

    public override EventLogEntryType EntryType
    {
      get
      {
        return this.entry.EntryType;
      }
    }

    public override int Index
    {
      get
      {
        return this.entry.Index;
      }
    }

    public override long InstanceId
    {
      get
      {
        return this.entry.InstanceId;
      }
    }

    public override string MachineName
    {
      get
      {
        return this.entry.MachineName;
      }
    }

    public override string Message
    {
      get
      {
        return this.entry.Message;
      }
    }

    public override string[] ReplacementStrings
    {
      get
      {
        return this.entry.ReplacementStrings;
      }
    }

    public override string Source
    {
      get
      {
        return this.entry.Source;
      }
    }

    public override DateTime TimeGenerated
    {
      get
      {
        return this.entry.TimeGenerated;
      }
    }

    public override DateTime TimeWritten
    {
      get
      {
        return this.entry.TimeWritten;
      }
    }

    public override string UserName
    {
      get
      {
        return this.entry.UserName;
      }
    }

    internal EventLogEntryWrapper(EventLogEntry entry)
    {
      this.entry = entry;
    }
  }
}
