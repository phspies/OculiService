using System.Runtime.Serialization;

namespace OculiService.CloudProviders.Contract
{
    public class ClusterResourceState : IExtensibleDataObject
    {
        public string CurrentOwnerNodeName { get; set; }
        public string GroupName { get; set; }
        public ExtensionDataObject ExtensionData { get; set; }
    }
}
