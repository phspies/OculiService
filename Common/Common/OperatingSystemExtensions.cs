using System;
namespace OculiService.Common
{
  public static class OperatingSystemExtensions
  {
    public static bool IsWindows2008OrLater(this OperatingSystem system)
    {
      return 6 <= system.Version.Major;
    }
  }
}
