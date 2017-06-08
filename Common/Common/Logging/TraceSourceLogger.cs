using System.Diagnostics;
using System.Globalization;

namespace OculiService.Common.Logging
{
  public class TraceSourceLogger : ILogger
  {
    private readonly CultureInfo culture = CultureInfo.CreateSpecificCulture("en-US");
    private readonly TraceSource source;

    public TraceSource TraceSource
    {
      get
      {
        return this.source;
      }
    }

    public CultureInfo Culture
    {
      get
      {
        return this.culture;
      }
    }

    public TraceSourceLogger(TraceSource source)
    {
      Invariant.ArgumentNotNull((object) source, "source");
      this.source = source;
    }

    public void WriteEntry(LogEntry logEntry)
    {
      this.source.TraceData(logEntry.EventType, logEntry.EventId, (object) logEntry);
    }
  }
}
