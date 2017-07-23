// Decompiled with JetBrains decompiler
// Type: OculiService.CloudProviders.Contract.PartitionInitializationInfo
// Assembly: OculiService.Common.Contract.Data, Version=8.0.0.1554, Culture=neutral, PublicKeyToken=null
// MVID: 0E0F9990-C9DC-47DF-965B-3C0317866496
// Assembly location: C:\Downloads\Double-Take\Service\OculiService.Common.Contract.Data.dll

using System.Runtime.Serialization;

namespace OculiService.CloudProviders.Contract
{
  [DataContract]
  public class PartitionInitializationInfo : IExtensibleDataObject
  {
    [DataMember]
    public string PartitionType { get; set; }

    [DataMember]
    public int Size { get; set; }

    [DataMember]
    public int Offset { get; set; }

    [DataMember]
    public int Align { get; set; }

    [DataMember]
    public bool Active { get; set; }

    [DataMember]
    public bool ShouldFormatPartition { get; set; }

    [DataMember]
    public string FileSystemType { get; set; }

    [DataMember]
    public string Label { get; set; }

    [DataMember]
    public bool QuickFormat { get; set; }

    [DataMember]
    public bool ShouldCreateMountPointDirectory { get; set; }

    [DataMember]
    public string MountPoint { get; set; }

    [DataMember]
    public bool IsBoot { get; set; }

    [DataMember]
    public int INodeSize { get; set; }

    public ExtensionDataObject ExtensionData { get; set; }
  }
}
