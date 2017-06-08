namespace OculiService.Common.Logging.EventLog.Interface
{
  public interface IReportingEventLogWriter
  {
    void Write(LogEntry logEntry);
  }
}
