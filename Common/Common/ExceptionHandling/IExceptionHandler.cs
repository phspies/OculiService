using System;
namespace OculiService.Common.ExceptionHandling
{
  public interface IExceptionHandler
  {
    bool HandleException(Exception exception);
  }
}
