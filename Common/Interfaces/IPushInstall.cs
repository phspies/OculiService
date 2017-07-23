using DoubleTake.Common.Install;

namespace OculiService.Common.Interfaces
{
  public interface IPushInstall
  {
    void Invoke(IPushInstaller pushInstaller, InstallationOption installationOption, int timeoutInMinutes);
  }
}
