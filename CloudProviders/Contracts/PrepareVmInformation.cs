// Decompiled with JetBrains decompiler
// Type: OculiService.CloudProviders.Contract.PrepareVmInformation
// Assembly: OculiService.Common.Contract.Data, Version=8.0.0.1554, Culture=neutral, PublicKeyToken=null
// MVID: 0E0F9990-C9DC-47DF-965B-3C0317866496
// Assembly location: C:\Downloads\Double-Take\Service\OculiService.Common.Contract.Data.dll

using OculiService.Core.Contract;
using System.Runtime.Serialization;

namespace OculiService.CloudProviders.Contract
{
  [DataContract]
  public class PrepareVmInformation : IExtensibleDataObject
  {
    [DataMember]
    public string Hypervisor { get; set; }

    [DataMember]
    public OperatingSystemVersion OSVersion { get; set; }

    [DataMember]
    public OperatingSystemArchitecture CPUArchitecture { get; set; }

    [DataMember]
    public string WindowsDir { get; set; }

    [DataMember]
    public VirtualNetworkInterfaceInfo[] NetworkInterfaceInfo { get; set; }

    [DataMember]
    public string SystemVolumeMountPath { get; set; }

    [DataMember]
    public string VmVersion { get; set; }

    public ExtensionDataObject ExtensionData { get; set; }
  }
}
