using System;
namespace OculiService.Common
{
  public interface IPollingService
  {
    IDisposable Subscribe(Action<long> action);

    IDisposable Subscribe(Action<long> action, TimeSpan interval);
  }
}
