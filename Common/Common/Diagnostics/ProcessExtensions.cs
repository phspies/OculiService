using System;
namespace OculiService.Common.Diagnostics
{
  public static class ProcessExtensions
  {
    public static bool WaitForExit(this IProcess process)
    {
      return process.WaitForExit(new TimeSpan((long) int.MaxValue));
    }

    public static bool WaitForInputIdle(this IProcess process)
    {
      return process.WaitForInputIdle(new TimeSpan((long) int.MaxValue));
    }
  }
}
