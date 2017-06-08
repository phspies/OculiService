using OculiService.Common.ExceptionHandling;
using System;
using System.Diagnostics;
using System.Reflection;
using System.Text;

namespace OculiService.Common.Logging
{
  public static class LoggerExtensions
  {
    public static void LogVerbose(this ILogger logger, LoggerCategory category, Exception exception, string formatString, params object[] formatParameters)
    {
      string str = string.Format((IFormatProvider) logger.Culture, formatString, formatParameters);
      if (exception != null)
        str = LoggerExtensions.BuildExceptionText(exception, str);
      logger.Log(TraceEventType.Verbose, str, category == null ? (string) null : category.Name);
    }

    public static void LogVerbose(this ILogger logger, LoggerCategory category, Exception exception)
    {
      logger.LogVerbose(category, exception, (string) null, new object[0]);
    }

    public static void LogVerbose(this ILogger logger, LoggerCategory category, string formatString, params object[] formatParameters)
    {
      logger.LogVerbose(category, (Exception) null, formatString, formatParameters);
    }

    public static void LogVerbose(this ILogger logger, Exception exception, string formatString, params object[] formatParameters)
    {
      logger.LogVerbose((LoggerCategory) null, exception, formatString, formatParameters);
    }

    public static void LogVerbose(this ILogger logger, string formatString, params object[] formatParameters)
    {
      logger.LogVerbose((LoggerCategory) null, (Exception) null, formatString, formatParameters);
    }

    public static void Verbose(this ILogger logger, int id, params object[] values)
    {
      logger.WriteEntry(new LogEntry()
      {
        EventId = id,
        EventType = TraceEventType.Verbose,
        Values = values
      });
    }

    public static void Verbose(this ILogger logger, string messageText, string category)
    {
      logger.Log(TraceEventType.Verbose, messageText, category);
    }

    public static void Verbose(this ILogger logger, string messageText)
    {
      logger.Verbose(messageText, (string) null);
    }

    public static void Verbose(this ILogger logger, Exception exception)
    {
      logger.Verbose(LoggerExtensions.BuildExceptionText(exception));
    }

    public static void Verbose(this ILogger logger, Exception exception, string supplementalMessage)
    {
      logger.Verbose(exception, supplementalMessage, (string) null);
    }

    public static void Verbose(this ILogger logger, Exception exception, string supplementalMessage, string category)
    {
      logger.Verbose(LoggerExtensions.BuildExceptionText(exception, supplementalMessage), category);
    }

    public static void FormatVerbose(this ILogger logger, string messageFormat, params object[] formatParameters)
    {
      logger.Verbose(string.Format((IFormatProvider) logger.Culture, messageFormat, formatParameters));
    }

    public static void FormatVerboseWithException(this ILogger logger, Exception exception, string messageFormat, params object[] formatParameters)
    {
      logger.Verbose(exception, string.Format((IFormatProvider) logger.Culture, messageFormat, formatParameters));
    }

    public static void FormatVerboseWithCategory(this ILogger logger, string category, string messageFormat, params object[] formatParameters)
    {
      logger.Verbose(string.Format((IFormatProvider) logger.Culture, messageFormat, formatParameters), category);
    }

    public static void LogInformation(this ILogger logger, LoggerCategory category, Exception exception, string formatString, params object[] formatParameters)
    {
      string str = string.Format((IFormatProvider) logger.Culture, formatString, formatParameters);
      if (exception != null)
        str = LoggerExtensions.BuildExceptionText(exception, str);
      logger.Log(TraceEventType.Information, str, category == null ? (string) null : category.Name);
    }

    public static void LogInformation(this ILogger logger, LoggerCategory category, Exception exception)
    {
      logger.LogInformation(category, exception, (string) null, new object[0]);
    }

    public static void LogInformation(this ILogger logger, LoggerCategory category, string formatString, params object[] formatParameters)
    {
      logger.LogInformation(category, (Exception) null, formatString, formatParameters);
    }

    public static void LogInformation(this ILogger logger, Exception exception, string formatString, params object[] formatParameters)
    {
      logger.LogInformation((LoggerCategory) null, exception, formatString, formatParameters);
    }

    public static void LogInformation(this ILogger logger, string formatString, params object[] formatParameters)
    {
      logger.LogInformation((LoggerCategory) null, (Exception) null, formatString, formatParameters);
    }

    public static void InformationEvent(this ILogger logger, int id, params object[] values)
    {
      logger.WriteEntry(new LogEntry()
      {
        EventId = id,
        EventType = TraceEventType.Information,
        Values = values
      });
    }

    public static void InformationEventWithCategory(this ILogger logger, int id, int category, params object[] values)
    {
      logger.WriteEntry(new LogEntry()
      {
        EventId = id,
        EventCategoryId = category,
        EventType = TraceEventType.Information,
        Values = values
      });
    }

    public static void Information(this ILogger logger, string message, string category)
    {
      logger.Log(TraceEventType.Information, message, category);
    }

    public static void Information(this ILogger logger, string message)
    {
      logger.Information(message, (string) null);
    }

    public static void Information(this ILogger logger, Exception exception)
    {
      logger.Information(LoggerExtensions.BuildExceptionText(exception));
    }

    public static void Information(this ILogger logger, Exception exception, string supplementalMessage)
    {
      logger.Information(exception, supplementalMessage, (string) null);
    }

    public static void Information(this ILogger logger, Exception exception, string supplementalMessage, string category)
    {
      logger.Information(LoggerExtensions.BuildExceptionText(exception, supplementalMessage), category);
    }

    public static void FormatInformation(this ILogger logger, string messageFormat, params object[] formatParameters)
    {
      logger.Information(string.Format((IFormatProvider) logger.Culture, messageFormat, formatParameters));
    }

    public static void FormatInformationWithException(this ILogger logger, Exception exception, string messageFormat, params object[] formatParameters)
    {
      logger.Information(exception, string.Format((IFormatProvider) logger.Culture, messageFormat, formatParameters));
    }

    public static void FormatInformationWithCategory(this ILogger logger, string category, string messageFormat, params object[] formatParameters)
    {
      logger.Information(string.Format((IFormatProvider) logger.Culture, messageFormat, formatParameters), category);
    }

    public static void LogWarning(this ILogger logger, LoggerCategory category, Exception exception, string formatString, params object[] formatParameters)
    {
      string str = string.Format((IFormatProvider) logger.Culture, formatString, formatParameters);
      if (exception != null)
        str = LoggerExtensions.BuildExceptionText(exception, str);
      logger.Log(TraceEventType.Warning, str, category == null ? (string) null : category.Name);
    }

    public static void LogWarning(this ILogger logger, LoggerCategory category, Exception exception)
    {
      logger.LogWarning(category, exception, (string) null, new object[0]);
    }

    public static void LogWarning(this ILogger logger, LoggerCategory category, string formatString, params object[] formatParameters)
    {
      logger.LogWarning(category, (Exception) null, formatString, formatParameters);
    }

    public static void LogWarning(this ILogger logger, Exception exception, string formatString, params object[] formatParameters)
    {
      logger.LogWarning((LoggerCategory) null, exception, formatString, formatParameters);
    }

    public static void LogWarning(this ILogger logger, string formatString, params object[] formatParameters)
    {
      logger.LogWarning((LoggerCategory) null, (Exception) null, formatString, formatParameters);
    }

    public static void WarningEvent(this ILogger logger, int id, params object[] values)
    {
      logger.WriteEntry(new LogEntry()
      {
        EventId = id,
        EventType = TraceEventType.Warning,
        Values = values
      });
    }

    public static void WarningEventWithCategory(this ILogger logger, int id, int category, params object[] values)
    {
      logger.WriteEntry(new LogEntry()
      {
        EventId = id,
        EventCategoryId = category,
        EventType = TraceEventType.Warning,
        Values = values
      });
    }

    public static void Warning(this ILogger logger, string messageText, string category)
    {
      logger.Log(TraceEventType.Warning, messageText, category);
    }

    public static void Warning(this ILogger logger, string message)
    {
      logger.Warning(message, (string) null);
    }

    public static void Warning(this ILogger logger, Exception exception)
    {
      logger.Warning(LoggerExtensions.BuildExceptionText(exception));
    }

    public static void Warning(this ILogger logger, Exception exception, string supplementalMessage)
    {
      logger.Warning(exception, supplementalMessage, (string) null);
    }

    public static void Warning(this ILogger logger, Exception exception, string supplementalMessage, string category)
    {
      logger.Warning(LoggerExtensions.BuildExceptionText(exception, supplementalMessage), category);
    }

    public static void FormatWarning(this ILogger logger, string messageFormat, params object[] formatParameters)
    {
      logger.Warning(string.Format((IFormatProvider) logger.Culture, messageFormat, formatParameters));
    }

    public static void FormatWarningWithException(this ILogger logger, Exception exception, string messageFormat, params object[] formatParameters)
    {
      logger.Warning(exception, string.Format((IFormatProvider) logger.Culture, messageFormat, formatParameters));
    }

    public static void FormatWarningWithCategory(this ILogger logger, string category, string messageFormat, params object[] formatParameters)
    {
      logger.Warning(string.Format((IFormatProvider) logger.Culture, messageFormat, formatParameters), category);
    }

    public static void LogError(this ILogger logger, LoggerCategory category, Exception exception, string formatString, params object[] formatParameters)
    {
      string str = string.Format((IFormatProvider) logger.Culture, formatString, formatParameters);
      if (exception != null)
        str = LoggerExtensions.BuildExceptionText(exception, str);
      logger.Log(TraceEventType.Error, str, category == null ? (string) null : category.Name);
    }

    public static void LogError(this ILogger logger, LoggerCategory category, Exception exception)
    {
      logger.LogError(category, exception, (string) null, new object[0]);
    }

    public static void LogError(this ILogger logger, LoggerCategory category, string formatString, params object[] formatParameters)
    {
      logger.LogError(category, (Exception) null, formatString, formatParameters);
    }

    public static void LogError(this ILogger logger, Exception exception, string formatString, params object[] formatParameters)
    {
      logger.LogError((LoggerCategory) null, exception, formatString, formatParameters);
    }

    public static void LogError(this ILogger logger, string formatString, params object[] formatParameters)
    {
      logger.LogError((LoggerCategory) null, (Exception) null, formatString, formatParameters);
    }

    public static void ErrorEvent(this ILogger logger, int id, params object[] values)
    {
      logger.WriteEntry(new LogEntry()
      {
        EventId = id,
        EventType = TraceEventType.Error,
        Values = values
      });
    }

    public static void ErrorEventWithCategory(this ILogger logger, int id, int category, params object[] values)
    {
      logger.WriteEntry(new LogEntry()
      {
        EventId = id,
        EventCategoryId = category,
        EventType = TraceEventType.Error,
        Values = values
      });
    }

    public static void Error(this ILogger logger, string message, string category)
    {
      logger.Log(TraceEventType.Error, message, category);
    }

    public static void Error(this ILogger logger, string messageText)
    {
      logger.Error(messageText, (string) null);
    }

    public static void Error(this ILogger logger, Exception exception)
    {
      logger.Error(LoggerExtensions.BuildExceptionText(exception));
    }

    public static void Error(this ILogger logger, Exception exception, string supplementalMessage)
    {
      logger.Error(exception, supplementalMessage, (string) null);
    }

    public static void Error(this ILogger logger, Exception exception, string supplementalMessage, string category)
    {
      logger.Error(LoggerExtensions.BuildExceptionText(exception, supplementalMessage), category);
    }

    public static void FormatError(this ILogger logger, string messageFormat, params object[] formatParameters)
    {
      logger.Error(string.Format((IFormatProvider) logger.Culture, messageFormat, formatParameters));
    }

    public static void FormatErrorWithException(this ILogger logger, Exception exception, string messageFormat, params object[] formatParameters)
    {
      logger.Error(exception, string.Format((IFormatProvider) logger.Culture, messageFormat, formatParameters));
    }

    public static void FormatErrorWithCategory(this ILogger logger, string category, string messageFormat, params object[] formatParameters)
    {
      logger.Error(string.Format((IFormatProvider) logger.Culture, messageFormat, formatParameters), category);
    }

    public static void LogCritical(this ILogger logger, LoggerCategory category, Exception exception, string formatString, params object[] formatParameters)
    {
      string str = string.Format((IFormatProvider) logger.Culture, formatString, formatParameters);
      if (exception != null)
        str = LoggerExtensions.BuildExceptionText(exception, str);
      logger.Log(TraceEventType.Critical, str, category == null ? (string) null : category.Name);
    }

    public static void LogCritical(this ILogger logger, LoggerCategory category, Exception exception)
    {
      logger.LogCritical(category, exception, (string) null, new object[0]);
    }

    public static void LogCritical(this ILogger logger, LoggerCategory category, string formatString, params object[] formatParameters)
    {
      logger.LogCritical(category, (Exception) null, formatString, formatParameters);
    }

    public static void LogCritical(this ILogger logger, Exception exception, string formatString, params object[] formatParameters)
    {
      logger.LogCritical((LoggerCategory) null, exception, formatString, formatParameters);
    }

    public static void LogCritical(this ILogger logger, string formatString, params object[] formatParameters)
    {
      logger.LogCritical((LoggerCategory) null, (Exception) null, formatString, formatParameters);
    }

    public static void CriticalEvent(this ILogger logger, int id, params object[] values)
    {
      logger.WriteEntry(new LogEntry()
      {
        EventId = id,
        EventType = TraceEventType.Critical,
        Values = values
      });
    }

    public static void CriticalEventWithCategory(this ILogger logger, int id, int category, params object[] values)
    {
      logger.WriteEntry(new LogEntry()
      {
        EventId = id,
        EventCategoryId = category,
        EventType = TraceEventType.Critical,
        Values = values
      });
    }

    public static void Critical(this ILogger logger, string message, string category)
    {
      logger.Log(TraceEventType.Critical, message, category);
    }

    public static void Critical(this ILogger logger, string message)
    {
      logger.Critical(message, (string) null);
    }

    public static void Critical(this ILogger logger, Exception exception)
    {
      logger.Critical(LoggerExtensions.BuildExceptionText(exception));
    }

    public static void Critical(this ILogger logger, Exception exception, string supplementalMessage)
    {
      logger.Critical(exception, supplementalMessage, (string) null);
    }

    public static void Critical(this ILogger logger, Exception exception, string supplementalMessage, string category)
    {
      logger.Critical(LoggerExtensions.BuildExceptionText(exception, supplementalMessage), category);
    }

    public static void FormatCritical(this ILogger logger, string messageFormat, params object[] formatParameters)
    {
      logger.Critical(string.Format((IFormatProvider) logger.Culture, messageFormat, formatParameters));
    }

    public static void FormatCriticalWithException(this ILogger logger, Exception exception, string messageFormat, params object[] formatParameters)
    {
      logger.Critical(exception, string.Format((IFormatProvider) logger.Culture, messageFormat, formatParameters));
    }

    public static void FormatCriticalWithCategory(this ILogger logger, string category, string messageFormat, params object[] formatParameters)
    {
      logger.Critical(string.Format((IFormatProvider) logger.Culture, messageFormat, formatParameters), category);
    }

    public static void Log(this ILogger logger, TraceEventType eventType, string message, string category)
    {
      if (category == null)
      {
        logger.WriteEntry(new LogEntry()
        {
          EventType = eventType,
          Message = message
        });
      }
      else
      {
        using (LogicalOperation.Create(category, new object[0]))
          logger.WriteEntry(new LogEntry()
          {
            EventType = eventType,
            Message = message
          });
      }
    }

    private static string BuildExceptionText(Exception ex)
    {
      return LoggerExtensions.BuildExceptionText(ex, (string) null);
    }

    private static string BuildExceptionText(Exception ex, string supplementalMessage)
    {
      StringBuilder messages = new StringBuilder();
      StringBuilder stackTraces = new StringBuilder();
      if (!string.IsNullOrEmpty(supplementalMessage))
      {
        messages.Append(supplementalMessage);
        messages.Append("--");
      }
      messages.Append((object) ex.GetType());
      messages.Append(": ");
      messages.Append(ex.Message);
      if (ex is AggregateException)
      {
        messages.AppendLine();
        AggregateException aggregateException1 = (AggregateException) ex;
        messages.Append("---> (Root Cause Exception) ");
        messages.Append(LoggerExtensions.BuildExceptionText(aggregateException1.GetBaseException()));
        messages.AppendLine("<---");
        messages.AppendLine();
        AggregateException aggregateException2 = aggregateException1.Flatten();
        if (aggregateException2.InnerExceptions != null)
        {
          for (int index = 0; index < aggregateException2.InnerExceptions.Count; ++index)
          {
            messages.Append(string.Format("---> (Inner Exception #{0}) ", (object) index));
            messages.Append(LoggerExtensions.BuildExceptionText(aggregateException2.InnerExceptions[index]));
            messages.AppendLine("<---");
            messages.AppendLine();
          }
        }
      }
      else if (ex is RollbackException)
      {
        messages.AppendLine();
        RollbackException rollbackException = (RollbackException) ex;
        if (rollbackException.Original != null)
        {
          messages.Append("---> (Original Exception) ");
          messages.Append(LoggerExtensions.BuildExceptionText(rollbackException.Original));
          messages.AppendLine("<---");
          messages.AppendLine();
        }
        if (rollbackException.FromRollback != null)
        {
          messages.Append("---> (Rollback Exception) ");
          messages.Append(LoggerExtensions.BuildExceptionText(rollbackException.FromRollback));
          messages.AppendLine("<---");
          messages.AppendLine();
        }
      }
      else
      {
        if (ex is ReflectionTypeLoadException)
        {
          ReflectionTypeLoadException typeLoadException = (ReflectionTypeLoadException) ex;
          if (typeLoadException.LoaderExceptions != null)
          {
            int length = typeLoadException.LoaderExceptions.Length;
            for (int index = 0; index < length; ++index)
            {
              messages.Append("---> (Loader Exception #" + (object) index + ") ");
              messages.Append(LoggerExtensions.BuildExceptionText(typeLoadException.LoaderExceptions[index]));
              messages.AppendLine("<---");
              messages.AppendLine();
            }
          }
        }
        LoggerExtensions.BuildExceptionText(ex.InnerException, messages, stackTraces);
      }
      stackTraces.Append(ex.StackTrace);
      return messages.Append(stackTraces.ToString()).ToString();
    }

    private static void BuildExceptionText(Exception ex, StringBuilder messages, StringBuilder stackTraces)
    {
      if (ex == null)
      {
        messages.AppendLine();
      }
      else
      {
        messages.Append(" ---> ");
        messages.Append((object) ex.GetType());
        messages.Append(": ");
        messages.Append(ex.Message);
        if (ex is AggregateException)
        {
          messages.AppendLine();
          AggregateException aggregateException = (AggregateException) ex;
          if (aggregateException.InnerExceptions != null)
          {
            for (int index = 0; index < aggregateException.InnerExceptions.Count; ++index)
            {
              messages.Append(string.Format("---> (Inner Exception #{0}) ", (object) index));
              messages.Append(LoggerExtensions.BuildExceptionText(aggregateException.InnerExceptions[index]));
              messages.AppendLine("<---");
              messages.AppendLine();
            }
          }
        }
        else if (ex is RollbackException)
        {
          messages.AppendLine();
          RollbackException rollbackException = (RollbackException) ex;
          if (rollbackException.Original != null)
          {
            messages.Append("---> (Original Exception) ");
            messages.Append(LoggerExtensions.BuildExceptionText(rollbackException.Original));
            messages.AppendLine("<---");
            messages.AppendLine();
          }
          if (rollbackException.FromRollback != null)
          {
            messages.Append("---> (Rollback Exception) ");
            messages.Append(LoggerExtensions.BuildExceptionText(rollbackException.FromRollback));
            messages.AppendLine("<---");
            messages.AppendLine();
          }
        }
        else
        {
          if (ex is ReflectionTypeLoadException)
          {
            ReflectionTypeLoadException typeLoadException = (ReflectionTypeLoadException) ex;
            int length = typeLoadException.LoaderExceptions.Length;
            if (typeLoadException.LoaderExceptions != null)
            {
              for (int index = 0; index < length; ++index)
              {
                messages.Append("---> (Loader Exception #" + (object) index + ") ");
                messages.Append(LoggerExtensions.BuildExceptionText(typeLoadException.LoaderExceptions[index]));
                messages.AppendLine("<---");
                messages.AppendLine();
              }
            }
          }
          LoggerExtensions.BuildExceptionText(ex.InnerException, messages, stackTraces);
        }
        stackTraces.AppendLine(ex.StackTrace);
        stackTraces.AppendLine("   --- End of inner exception stack trace ---");
      }
    }
  }
}
