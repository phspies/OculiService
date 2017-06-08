using System.Diagnostics;

namespace OculiService.Common.Logging.EventLog.Interface
{
  public interface ILogEntryConverter
  {
    EventInstance Convert(LogEntry logEntry);
  }
}
