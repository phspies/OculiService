using OculiService.Core.Contract;

namespace Oculi.Jobs.Context
{
  public interface IHelperInformation
  {
    string HelperBootDir { get; }

    string HelperDnsName { get; }

    OperatingSystemInfo HelperOSVersion { get; }

    string HelperSystemDir { get; }

    string HelperSystemDrive { get; }
  }
}
