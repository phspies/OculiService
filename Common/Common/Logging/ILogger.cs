using System.Globalization;

namespace OculiService.Common.Logging
{
  public interface ILogger
  {
    CultureInfo Culture { get; }

    void WriteEntry(LogEntry entry);
  }
}
