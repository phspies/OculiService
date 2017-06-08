using OculiService.Common.Logging;
using System;

namespace OculiService.Common.ExceptionHandling
{
  public class LoggingExceptionHandler : IExceptionHandler
  {
    private ILogger logger;

    public LoggingExceptionHandler(ILogger logger)
    {
      this.logger = logger;
    }

    public bool HandleException(Exception exception)
    {
      this.logger.Error(exception.Message);
      return true;
    }
  }
}
