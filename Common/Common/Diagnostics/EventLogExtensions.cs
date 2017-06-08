using System.Diagnostics;

namespace OculiService.Common.Diagnostics
{
  public static class EventLogExtensions
  {
    public static IEventLog CreateEventLog()
    {
      return (IEventLog) new EventLogWrapper(new EventLog());
    }

    public static IEventLog CreateEventLog(string logName)
    {
      return (IEventLog) new EventLogWrapper(new EventLog(logName));
    }

    public static IEventLog CreateEventLog(string logName, string machineName)
    {
      return (IEventLog) new EventLogWrapper(new EventLog(logName, machineName));
    }

    public static IEventLog CreateEventLog(string logName, string machineName, string source)
    {
      return (IEventLog) new EventLogWrapper(new EventLog(logName, machineName, source));
    }

    public static void WriteEntry(this IEventLog eventLog, string message)
    {
      eventLog.WriteEntry(message, EventLogEntryType.Information, 0, (short) 0, (byte[]) null);
    }

    public static void WriteEntry(this IEventLog eventLog, string message, EventLogEntryType type)
    {
      eventLog.WriteEntry(message, type, 0, (short) 0, (byte[]) null);
    }

    public static void WriteEntry(this IEventLog eventLog, string message, EventLogEntryType type, int eventId)
    {
      eventLog.WriteEntry(message, type, eventId, (short) 0, (byte[]) null);
    }

    public static void WriteEntry(this IEventLog eventLog, string message, EventLogEntryType type, int eventId, short category)
    {
      eventLog.WriteEntry(message, type, eventId, category, (byte[]) null);
    }

    public static void WriteEvent(this IEventLog eventLog, EventInstance instance, params object[] values)
    {
      eventLog.WriteEvent(instance, (byte[]) null, values);
    }
  }
}
