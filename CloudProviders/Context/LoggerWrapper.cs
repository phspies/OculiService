using OculiService.Common;
using OculiService.Common.Logging;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Reflection;

namespace Oculi.Jobs.Context
{
  public class LoggerWrapper : ILogger
  {
    private static string[] _LoggingMethods = new string[23]{ "WriteLine", "WriteEntry", "Verbose", "FormatVerbose", "FormatVerboseWithCategory", "FormatVerboseWithException", "Information", "FormatInformation", "FormatInformationWithCategory", "FormatInformationWithException", "Warning", "FormatWarning", "FormatWarningWithCategory", "FormatWarningWithException", "Error", "FormatError", "FormatErrorWithCategory", "FormatErrorWithException", "Critical", "FormatCritical", "FormatCriticalWithCategory", "FormatCriticalWithException", "Log" };
    private ILogger _Logger;

    public CultureInfo Culture
    {
      get
      {
        return this._Logger.Culture;
      }
    }

    public LoggerWrapper(ILogger logger)
    {
      Invariant.ArgumentNotNull((object) logger, "logger");
      this._Logger = logger;
    }

    public void WriteEntry(LogEntry entry)
    {
      entry.Message = LoggerWrapper.DetermineActivityName() + ":  " + entry.Message;
      this._Logger.WriteEntry(entry);
    }

    private static string DetermineActivityName()
    {
      MethodBase methodBase = ((IEnumerable<StackFrame>) new StackTrace(1, true).GetFrames()).Select<StackFrame, MethodBase>((Func<StackFrame, MethodBase>) (f => f.GetMethod())).SkipWhile<MethodBase>((Func<MethodBase, bool>) (f => ((IEnumerable<string>) LoggerWrapper._LoggingMethods).Any<string>((Func<string, bool>) (lm => string.Compare(f.Name, lm, true) == 0)))).First<MethodBase>();
      string name = methodBase.Name;
      return string.Format("{0}.{1}", methodBase.ReflectedType == (Type) null ? (object) "C++ Function" : (object) methodBase.ReflectedType.FullName, (object) name);
    }
  }
}
