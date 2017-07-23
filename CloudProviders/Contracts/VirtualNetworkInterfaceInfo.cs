// Decompiled with JetBrains decompiler
// Type: OculiService.CloudProviders.Contract.VirtualNetworkInterfaceInfo
// Assembly: OculiService.Common.Contract.Data, Version=8.0.0.1554, Culture=neutral, PublicKeyToken=null
// MVID: 0E0F9990-C9DC-47DF-965B-3C0317866496
// Assembly location: C:\Downloads\Double-Take\Service\OculiService.Common.Contract.Data.dll

using OculiService.Core.Contract;
using System.Runtime.Serialization;

namespace OculiService.CloudProviders.Contract
{
  public class VirtualNetworkInterfaceInfo : NetworkInterfaceInfo
  {
    public string VirtualNetwork { get; set; }
    public string VirtualNicType { get; set; }
    public int VLAN_ID { get; set; }
    public int VLAN_ID_TestFailover { get; set; }
  }
}
