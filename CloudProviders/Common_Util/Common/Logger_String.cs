using OculiService.Common.Logging;
using System.Globalization;
using System.IO;

namespace Common_Util
{
  public class Logger_String : ILogger
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

    public string Log
    {
      get
      {
        return this._Writer.ToString();
      }
      set
      {
        this._Writer.Write(value);
      }
    }

    public Logger_String(string initial)
    {
      this._Writer = (TextWriter) new StringWriter();
      this._Writer.Write(initial);
    }

    public Logger_String()
    {
      this._Writer = (TextWriter) new StringWriter();
    }

    public void WriteEntry(LogEntry entry)
    {
      this._Writer.WriteLine(entry.ToString());
    }
  }
}
