using OculiService.Common.Logging;
using System;
using System.Globalization;

namespace Common_Util
{
  public class Logger_Console : ILogger
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
      Console.WriteLine(entry.ToString());
    }
  }
}
