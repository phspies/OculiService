using OculiService.Common.Logging;
using System.Diagnostics;
using System.Text;

namespace OculiService.Common.Diagnostics
{
  public class LoggerTraceListener : TraceListener
  {
    private StringBuilder line = new StringBuilder();

    public static ILogger Logger { get; set; }

    public override void Write(string message)
    {
      if (LoggerTraceListener.Logger == null || this.Filter != null && !this.Filter.ShouldTrace((TraceEventCache) null, string.Empty, TraceEventType.Verbose, 0, message, (object[]) null, (object) null, (object[]) null))
        return;
      this.line.Append(message);
    }

    public override void WriteLine(string message)
    {
      if (LoggerTraceListener.Logger == null || this.Filter != null && !this.Filter.ShouldTrace((TraceEventCache) null, string.Empty, TraceEventType.Verbose, 0, message, (object[]) null, (object) null, (object[]) null))
        return;
      if (this.line.Length > 0)
      {
        message = this.line.ToString() + message;
        this.line = new StringBuilder();
      }
      LoggerTraceListener.Logger.Log(TraceEventType.Verbose, this.line.ToString(), "Tracing");
    }

    public override void TraceEvent(TraceEventCache eventCache, string source, TraceEventType eventType, int id, string message)
    {
      if (LoggerTraceListener.Logger == null || this.Filter != null && !this.Filter.ShouldTrace(eventCache, source, eventType, id, message, (object[]) null, (object) null, (object[]) null))
        return;
      LoggerTraceListener.Logger.WriteEntry(new LogEntry()
      {
        EventType = eventType,
        EventId = id,
        Message = message
      });
    }

    public override void TraceEvent(TraceEventCache eventCache, string source, TraceEventType eventType, int id, string format, params object[] args)
    {
      string message = args == null ? format : string.Format(format, args);
      this.TraceEvent(eventCache, source, eventType, id, message);
    }

    public override void TraceData(TraceEventCache eventCache, string source, TraceEventType eventType, int id, object data)
    {
      this.TraceEvent(eventCache, source, eventType, id, data.ToString());
    }

    public override void TraceData(TraceEventCache eventCache, string source, TraceEventType eventType, int id, params object[] data)
    {
      StringBuilder stringBuilder = new StringBuilder();
      if (data != null)
      {
        for (int index = 0; index < data.Length; ++index)
        {
          if (index != 0)
            stringBuilder.Append(", ");
          if (data[index] != null)
            stringBuilder.Append(data[index].ToString());
        }
      }
      this.TraceEvent(eventCache, source, eventType, id, stringBuilder.ToString());
    }
  }
}
