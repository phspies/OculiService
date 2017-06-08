using OculiService.Common.Logging.EventLog.Interface;
using OculiService.Common.Logging.File.Interface;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace OculiService.Common.Logging.Listener
{
  public sealed class ArchivingTraceListener : TraceListener
  {
    private IReportingEventLogWriter _reportingEventLogWriter;
    private IArchivingTextWriter _archivingTextWriter;

    public ArchivingTraceListener(IArchivingTextWriter archivingTextWriter, IReportingEventLogWriter reportingEventLogWriter)
    {
      this._reportingEventLogWriter = reportingEventLogWriter;
      this._archivingTextWriter = archivingTextWriter;
    }

    public override void TraceData(TraceEventCache eventCache, string source, TraceEventType eventType, int id, object data)
    {
      this.TraceData(eventCache, source, eventType, id, new object[1]{ data });
    }

    public override void TraceData(TraceEventCache eventCache, string source, TraceEventType eventType, int id, params object[] data)
    {
      LogEntry logEntry = data == null ? (LogEntry) null : ((IEnumerable<object>) data).SingleOrDefault<object>() as LogEntry;
      if (logEntry == null)
        return;
      if (logEntry.EventId != 0)
        this._reportingEventLogWriter.Write(logEntry);
      else
        this._archivingTextWriter.Write(logEntry);
    }

    public override void Write(string message)
    {
    }

    public override void WriteLine(string message)
    {
    }

    public void Archive()
    {
      this._archivingTextWriter.Archive();
    }
  }
}
