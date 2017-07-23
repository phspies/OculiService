using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace OculiService.CloudProviders.Contract
{
  public class SnapshotInfo : IExtensibleDataObject
  {
    public List<VHDInfo> Vhds = new List<VHDInfo>();

    public string Name { get; set; }

    public string InstanceId { get; set; }

    public string Parent { get; set; }

    public string Self { get; set; }

    public DateTime CreationTime { get; set; }

    public ExtensionDataObject ExtensionData { get; set; }
  }
}
