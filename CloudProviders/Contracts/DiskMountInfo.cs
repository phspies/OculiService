﻿// Decompiled with JetBrains decompiler
// Type: OculiService.CloudProviders.Contract.DiskMountInfo
// Assembly: OculiService.Common.Contract.Data, Version=8.0.0.1554, Culture=neutral, PublicKeyToken=null
// MVID: 0E0F9990-C9DC-47DF-965B-3C0317866496
// Assembly location: C:\Downloads\Double-Take\Service\OculiService.Common.Contract.Data.dll

using System.Runtime.Serialization;

namespace OculiService.CloudProviders.Contract
{
  [DataContract]
  public class DiskMountInfo : IExtensibleDataObject
  {
    [DataMember]
    public string DiskIdentifier { get; set; }

    [DataMember]
    public string MountPath { get; set; }

    [DataMember]
    public string FileSystem { get; set; }

    public ExtensionDataObject ExtensionData { get; set; }
  }
}
