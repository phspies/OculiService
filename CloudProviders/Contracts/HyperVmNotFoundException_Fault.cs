// Decompiled with JetBrains decompiler
// Type: OculiService.CloudProviders.Contract.HyperVmNotFoundException_Fault
// Assembly: OculiService.Common.Contract.Service, Version=8.0.0.1554, Culture=neutral, PublicKeyToken=null
// MVID: 08CF1387-08D0-402E-B017-E44E9C821E62
// Assembly location: C:\Downloads\Double-Take\Service\OculiService.Common.Contract.Service.dll

using OculiService.Common.Contract;
using System.Runtime.Serialization;

namespace OculiService.CloudProviders.Contract
{
  public sealed class HyperVmNotFoundException_Fault : ICommonFault
  {
    [DataMember]
    public string Name { get; set; }

    [DataMember]
    public string Message { get; set; }

    [DataMember]
    public string StackTrace { get; set; }

    [DataMember]
    public ICommonFault InnerFault { get; set; }
  }
}
