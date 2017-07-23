using OculiService.Common.Logging;
using System.Net;

namespace OculiService.CloudProviders.VMware
{
  public class VimServiceFactory
  {
    public static IVimService CreateVimService(ILogger logger, string hostName, string username, string password, int port = 0)
    {
      return (IVimService) new VCService(logger, hostName, port, username, password);
    }

    public static IVimService CreateVimService(ILogger tlLogger, string hostName, string userName, string password, int nRetries, int port = 0)
    {
      return (IVimService) new VCService(tlLogger, hostName, port, userName, password, nRetries);
    }

    public static IVimService CreateVimService(ILogger tlLogger, string hostName, ICredential cred, int nRetries, int port = 0)
    {
      return (IVimService) new VCService(tlLogger, hostName, port, cred, nRetries);
    }

    public static IVimService CreateVimService(ILogger tlLogger, string hostName, NetworkCredential credentials, int port = 0)
    {
      return (IVimService) new VCService(tlLogger, hostName, port, credentials);
    }
  }
}
