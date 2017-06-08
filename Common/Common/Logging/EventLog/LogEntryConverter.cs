using OculiService.Common.Logging.EventLog.Interface;
using System.Diagnostics;

namespace OculiService.Common.Logging.EventLog
{
  public sealed class LogEntryConverter : ILogEntryConverter
  {
    private static readonly EventInstance CouldNotGenerateEvent = new EventInstance(LogEntryConverter.GetInstanceId(0), 0, LogEntryConverter.GetLogEntryType(TraceEventType.Error));

    private static long GetInstanceId(int id)
    {
      return (long) uint.MaxValue & (long) id;
    }

    private static EventLogEntryType GetLogEntryType(TraceEventType type)
    {
      switch (type)
      {
        case TraceEventType.Critical:
        case TraceEventType.Error:
          return EventLogEntryType.Error;
        case TraceEventType.Warning:
          return EventLogEntryType.Warning;
        default:
          return EventLogEntryType.Information;
      }
    }

    public EventInstance Convert(LogEntry logEntry)
    {
      try
      {
        return new EventInstance(LogEntryConverter.GetInstanceId(logEntry.EventId), logEntry.EventCategoryId, LogEntryConverter.GetLogEntryType(logEntry.EventType));
      }
      catch
      {
        return LogEntryConverter.CouldNotGenerateEvent;
      }
    }
  }
}
