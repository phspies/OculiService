using System;
using System.Runtime.Serialization;

namespace OculiService.CloudProviders.Contract
{
  public class VMInfo : IExtensibleDataObject
  {
    public Guid BiosGuid;
    public Guid Id { get; set; }
    public string DisplayName { get; set; }
    public string GuestOS { get; set; }
    public string Address { get; set; }
    public string Path { get; set; }
    public string SnapshotDataPath { get; set; }
    public byte[] BootVolumeSignature { get; set; }
    public string SystemDirectory { get; set; }
    public string[] SnapshotFileNames { get; set; }
    public string[] VirtualHardDiskPath { get; set; }
    public Uri GuestUri { get; set; }
    public ExtensionDataObject ExtensionData { get; set; }
  }
}
