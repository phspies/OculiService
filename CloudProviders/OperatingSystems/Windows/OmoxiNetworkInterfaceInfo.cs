using OculiService.Core.Contract;
using System.Runtime.Serialization;

namespace OculiService.CloudProviders.OperatingSystems.Windows
{
    public class OculiServiceNetworkInterfaceInfo : IExtensibleDataObject
  {
    public UnicastIPAddressInfo[] IPAddresses { get; set; }
    public string Name { get; set; }
    public string Guid { get; set; }
    public string Description { get; set; }
    public int InterfaceIndex { get; set; }
    public int Index { get; set; }
    public string PnpInstanceId { get; set; }
    public string ServiceName { get; set; }
    public string MacAddress { get; set; }
    public string[] DnsServers { get; set; }
    public string[] Gateways { get; set; }
    public string DnsDomain { get; set; }
    public ExtensionDataObject ExtensionData { get; set; }
  }
}
