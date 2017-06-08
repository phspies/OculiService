using System.Diagnostics;

namespace OculiService.Common.Diagnostics
{
  public interface IProcessFactory
  {
    IProcess CreateProcess(ProcessStartInfo processStartInfo);
  }
}
