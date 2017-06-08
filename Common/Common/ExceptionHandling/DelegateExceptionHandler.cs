using System;
namespace OculiService.Common.ExceptionHandling
{
  public class DelegateExceptionHandler : IExceptionHandler
  {
    private readonly Func<Exception, bool> handler;

    public DelegateExceptionHandler(Func<Exception, bool> handler)
    {
      this.handler = handler;
    }

    public bool HandleException(Exception exception)
    {
      return this.handler(exception);
    }
  }
}
