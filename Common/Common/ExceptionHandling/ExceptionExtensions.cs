using System;
using System.Reflection;

namespace OculiService.Common.ExceptionHandling
{
  public static class ExceptionExtensions
  {
    private static Action<Exception> _preserveInternalException = (Action<Exception>) Delegate.CreateDelegate(typeof (Action<Exception>), typeof (Exception).GetMethod("InternalPreserveStackTrace", BindingFlags.Instance | BindingFlags.NonPublic));
    private static FieldInfo _setRemoteStackTrace = typeof (Exception).GetField("_remoteStackTraceString", BindingFlags.Instance | BindingFlags.NonPublic);
    private static readonly MethodInfo prepForRemoting = typeof (Exception).GetMethod("PrepForRemoting", BindingFlags.Instance | BindingFlags.NonPublic);

    public static Exception PrepareForRethrow(this Exception exception)
    {
      Invariant.ArgumentNotNull((object) exception, "exception");
      ExceptionExtensions.prepForRemoting.Invoke((object) exception, new object[0]);
      return exception;
    }

    public static void PreserveStackTrace(this Exception ex)
    {
      if (ex == null)
        throw new ArgumentNullException("ex");
      ExceptionExtensions._preserveInternalException(ex);
    }

    public static void SetRemoteStackTrace(this Exception ex, string stackTraceString)
    {
      if (ex == null)
        throw new ArgumentNullException("ex");
      ExceptionExtensions._setRemoteStackTrace.SetValue((object) ex, (object) stackTraceString);
    }

    public static string ToStringNoStackTrace(this Exception ex)
    {
      string message = ex.Message;
      string str = !string.IsNullOrEmpty(message) ? ex.GetType().FullName + ": " + message : ex.GetType().FullName;
      if (ex.InnerException != null)
        str = str + " ---> " + ex.InnerException.ToStringNoStackTrace();
      return str;
    }
  }
}
