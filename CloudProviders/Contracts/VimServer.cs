using System.Runtime.Serialization;
using VimWrapper;

namespace OculiService.CloudProviders.Contract
{
  public class VimServer
  {
    public string NetworkId { get; set; }
    public ICredential Credentials { get; set; }
    public string Version { get; set; }
    public VimServer[] ESXServers { get; set; }
  }
}
