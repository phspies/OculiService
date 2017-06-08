using System;
namespace OculiService.Common.ExceptionHandling
{
  public class NotifyingExceptionHandler : IExceptionHandler
  {
    [ThreadStatic]
    public static Action<string> Callback;
    private bool rethrow;
    private readonly string message;
    private readonly Func<Exception, string> messageFactory;

    public static event EventHandler<ExceptionHandlerEventArgs> ExceptionHandled;

    public NotifyingExceptionHandler(string message, bool rethrow)
    {
      this.message = message;
      this.rethrow = rethrow;
    }

    public NotifyingExceptionHandler(Func<Exception, string> messageFactory, bool rethrow)
    {
      this.messageFactory = messageFactory;
      this.rethrow = rethrow;
    }

    public bool HandleException(Exception exception)
    {
      
      EventHandler<ExceptionHandlerEventArgs> exceptionHandled = NotifyingExceptionHandler.ExceptionHandled;
      string message = this.GetMessage(exception);
      if (exceptionHandled != null)
        exceptionHandled((object) this, new ExceptionHandlerEventArgs(exception, message));
      if (NotifyingExceptionHandler.Callback != null)
        NotifyingExceptionHandler.Callback(message);
      return this.rethrow;
    }

    protected string GetMessage(Exception ex)
    {
      if (this.messageFactory != null)
        return this.messageFactory(ex);
      return this.message;
    }
  }
}
