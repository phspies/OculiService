// Decompiled with JetBrains decompiler
// Type: OculiService.CloudProviders.Contract.LinuxBootProcessingOptions
// Assembly: OculiService.Common.Contract.Data, Version=8.0.0.1554, Culture=neutral, PublicKeyToken=null
// MVID: 0E0F9990-C9DC-47DF-965B-3C0317866496
// Assembly location: C:\Downloads\Double-Take\Service\OculiService.Common.Contract.Data.dll

using System.Runtime.Serialization;

namespace OculiService.CloudProviders.Contract
{
  [DataContract]
  public class LinuxBootProcessingOptions : IExtensibleDataObject
  {
    [DataMember]
    public BootProcessingLinuxVolumeInfo[] VolumeInformation { get; set; }

    [DataMember]
    public BootProcessingLinuxVolumeInfo[] SwapVolumes { get; set; }

    [DataMember]
    public VirtualNetworkInterfaceInfo[] NetworkInterfaceInfo { get; set; }

    [DataMember]
    public string BootDiskPath { get; set; }

    [DataMember]
    public bool IsWanFailover { get; set; }

    [DataMember]
    public string MountRoot { get; set; }

    public ExtensionDataObject ExtensionData { get; set; }
  }
}
