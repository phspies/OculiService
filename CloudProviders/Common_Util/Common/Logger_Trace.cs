using OculiService.Common.Logging;
using System.Diagnostics;
using System.Globalization;

namespace Common_Util
{
  public class Logger_Trace : ILogger
  {
    private readonly CultureInfo culture = CultureInfo.CreateSpecificCulture("en-US");

    public CultureInfo Culture
    {
      get
      {
        return this.culture;
      }
    }

    public void WriteEntry(LogEntry entry)
    {
      Trace.WriteLine(entry.ToString());
    }
  }
}
