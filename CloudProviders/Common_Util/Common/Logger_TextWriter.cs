using OculiService.Common.Logging;
using System.Globalization;
using System.IO;

namespace Common_Util
{
  public class Logger_TextWriter : ILogger
  {
    private readonly CultureInfo culture = CultureInfo.CreateSpecificCulture("en-US");
    protected readonly TextWriter _Writer;

    public CultureInfo Culture
    {
      get
      {
        return this.culture;
      }
    }

    public Logger_TextWriter(TextWriter writer)
    {
      this._Writer = writer;
    }

    public void WriteEntry(LogEntry entry)
    {
      this._Writer.WriteLine(entry.ToString());
    }
  }
}
