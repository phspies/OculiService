using System;using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;

namespace OculiService.Common.Logging
{
  public class LogEntry
  {
    private static readonly Lazy<Tuple<string, int>> _processInfo = new Lazy<Tuple<string, int>>(new Func<Tuple<string, int>>(LogEntry.GetProcessInfo), true);
    private StringBuilder _errorMessages;

    public string LoggerName { get; set; }

    public string ActivityName { get; set; }

    public string Message { get; set; }

    public int Priority { get; set; }

    public int EventId { get; set; }

    public int EventCategoryId { get; set; }

    public TraceEventType EventType { get; set; }

    public string Title { get; set; }

    public DateTime Timestamp { get; set; }

    public string MachineName { get; set; }

    public string AppDomainName { get; set; }

    public int ProcessId { get; set; }

    public string ProcessName { get; set; }

    public string ThreadName { get; set; }

    public int ThreadId { get; set; }

    public object[] Values { get; set; }

    public Guid ActivityId { get; set; }

    public Guid? RelatedActivityId { get; set; }

    public string ErrorMessages
    {
      get
      {
        if (this._errorMessages == null)
          return (string) null;
        return this._errorMessages.ToString();
      }
    }

    public LogEntry()
    {
      if (LogicalOperation.LogicalOperationStack.Any<object>())
        this.ActivityName = LogicalOperation.LogicalOperationStack.First<object>().ToString();
      this.Timestamp = DateTime.Now;
      this.MachineName = Environment.MachineName;
      this.AppDomainName = AppDomain.CurrentDomain.FriendlyName;
      this.ActivityId = Trace.CorrelationManager.ActivityId;
      this.RelatedActivityId = new Guid?();
      this.EventType = TraceEventType.Verbose;
      this.Message = string.Empty;
      this.ProcessName = LogEntry._processInfo.Value.Item1;
      this.ProcessId = LogEntry._processInfo.Value.Item2;
      Thread currentThread = Thread.CurrentThread;
      this.ThreadName = currentThread.Name;
      this.ThreadId = currentThread.ManagedThreadId;
    }

    private static Tuple<string, int> GetProcessInfo()
    {
      using (Process currentProcess = Process.GetCurrentProcess())
        return new Tuple<string, int>(currentProcess.ProcessName, currentProcess.Id);
    }

    public virtual void AddErrorMessage(string errorMessage)
    {
      if (this._errorMessages == null)
        this._errorMessages = new StringBuilder();
      this._errorMessages.Insert(0, Environment.NewLine);
      this._errorMessages.Insert(0, Environment.NewLine);
      this._errorMessages.Insert(0, errorMessage);
    }

    public override string ToString()
    {
      return LogEntry.TextWriterFormatter(this);
    }

    private static string CsvEscapeString(string original)
    {
      return original.Replace("\"", "\"\"");
    }

    public static string TextWriterFormatter(LogEntry entry)
    {
      string str;
      try
      {
        string[] array = LogicalOperation.LogicalOperationStack.OfType<string>().Select<string, string>((Func<string, string>) (s => LogEntry.CsvEscapeString(s))).ToArray<string>();
        str = array.Length == 0 ? string.Empty : string.Format(" - {0}", (object) string.Join("|", array));
      }
      catch
      {
        str = "Logical Operation Stack Error";
      }
      return string.Format("[{0}] {1}: {2} (PID:{3}, TID:{4}, AID:{5}{6})", (object) entry.Timestamp.ToString("MM/dd/yyyy HH:mm:ss.fff"), (object) entry.EventType, (object) new StringBuilder(entry.Message).Replace("\r\n", "\n").Replace("\n", Environment.NewLine + "    "), (object) entry.ProcessId, (object) entry.ThreadId, (object) entry.ActivityId, (object) str);
    }
  }
}
