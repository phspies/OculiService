// Decompiled with JetBrains decompiler
// Type: OculiService.CloudProviders.Contract.ServerQualificationResults
// Assembly: OculiService.Common.Contract.Data, Version=8.0.0.1554, Culture=neutral, PublicKeyToken=null
// MVID: 0E0F9990-C9DC-47DF-965B-3C0317866496
// Assembly location: C:\Downloads\Double-Take\Service\OculiService.Common.Contract.Data.dll

using System.Runtime.Serialization;

namespace OculiService.CloudProviders.Contract
{
  [DataContract]
  public class ServerQualificationResults : IExtensibleDataObject
  {
    [DataMember]
    public int Cpus { get; set; }

    [DataMember]
    public int CoresPerProcessor { get; set; }

    [DataMember]
    public int LogicalProcessors { get; set; }

    [DataMember]
    public long Memory { get; set; }

    [DataMember]
    public VirtualSwitchInfo[] VirtualSwitches { get; set; }

    [DataMember]
    public VolumeQualificationResults[] Volumes { get; set; }

    [DataMember]
    public string Version { get; set; }

    public ExtensionDataObject ExtensionData { get; set; }
  }
}
