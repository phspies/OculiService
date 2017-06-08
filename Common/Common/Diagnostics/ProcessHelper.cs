using System.Security.Principal;
using System.Threading;

namespace OculiService.Common.Diagnostics
{
  public static class ProcessHelper
  {
    public static bool IsCurrentProcessInteractive()
    {
      Thread.GetDomain().SetPrincipalPolicy(PrincipalPolicy.WindowsPrincipal);
      return (Thread.CurrentPrincipal as WindowsPrincipal).IsInRole(new SecurityIdentifier(WellKnownSidType.InteractiveSid, (SecurityIdentifier) null));
    }
  }
}
