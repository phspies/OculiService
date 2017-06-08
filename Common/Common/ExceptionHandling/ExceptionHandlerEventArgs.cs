using System;
namespace OculiService.Common.ExceptionHandling
{
  public class ExceptionHandlerEventArgs : EventArgs
  {
    private readonly Exception exception;
    private readonly string message;

    public Exception Exception
    {
      get
      {
        return this.exception;
      }
    }

    public string Message
    {
      get
      {
        return this.message;
      }
    }

    internal ExceptionHandlerEventArgs(Exception exception, string message)
    {
      this.message = message;
      this.exception = exception;
    }
  }
}
