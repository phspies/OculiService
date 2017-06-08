using OculiService.Common.Diagnostics;
using OculiService.Common.Logging.EventLog.Interface;
using OculiService.Common.Logging.File.Interface;
using System;

namespace OculiService.Common.Logging.EventLog
{
  public sealed class ReportingEventLogWriter : IReportingEventLogWriter
  {
    private readonly IEventLog _eventLog;
    private readonly IArchivingTextWriter _archivingTextWriter;
    private readonly ILogEntryConverter _logEntryConverter;

    public ReportingEventLogWriter(IEventLog eventLog, IArchivingTextWriter archivingTextWriter, ILogEntryConverter logEntryConverter)
    {
      this._logEntryConverter = logEntryConverter;
      this._archivingTextWriter = archivingTextWriter;
      this._eventLog = eventLog;
    }

    public void Write(LogEntry logEntry)
    {
      Invariant.ArgumentNotNull((object) logEntry, "logEntry");
      try
      {
        this._eventLog.WriteEvent(this._logEntryConverter.Convert(logEntry), logEntry.Values);
        logEntry.Message = string.Format("Event log entry written: '{0}'.", (object) (logEntry.EventId & (int) ushort.MaxValue));
      }
      catch (Exception ex)
      {
        logEntry.Message = string.Format("Unable to write event log entry '{0}': {1}", (object) (logEntry.EventId & (int) ushort.MaxValue), (object) ex.Message);
      }
      this._archivingTextWriter.Write(logEntry);
    }
  }
}
