
using System.Runtime.Serialization;

namespace OculiService.CloudProviders.Contract
{
    [DataContract]
    public class DiskInitializationResult : IExtensibleDataObject
    {
        [DataMember]
        public bool Completed { get; set; }

        [DataMember]
        public bool Success { get; set; }

        [DataMember]
        public string Message { get; set; }

        public ExtensionDataObject ExtensionData { get; set; }
    }
}
