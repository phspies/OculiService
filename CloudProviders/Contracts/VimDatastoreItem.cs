using System.Runtime.Serialization;

namespace OculiService.CloudProviders.Contract
{
  public class VimDatastoreItem : IExtensibleDataObject
  {
    public string Name { get; set; }
    public string Type { get; set; }
    public long Size { get; set; }
    public ExtensionDataObject ExtensionData { get; set; }
  }
}
