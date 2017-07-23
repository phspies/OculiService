using OculiService.Common;
using System;
using System.Runtime.Serialization;

namespace OculiService.Core.Contract
{
  [DataContract]
  public class UnicastIPAddressInfo : IEquatable<UnicastIPAddressInfo>, IExtensibleDataObject
  {
    [DataMember]
    public string IPAddress { get; set; }

    [DataMember]
    public string IPv4Mask { get; set; }

    [DataMember]
    public bool IsDHCP { get; set; }

    [DataMember]
    public bool IsNAT { get; set; }

    [DataMember]
    public bool IsOnline { get; set; }

    public ExtensionDataObject ExtensionData { get; set; }

    public bool Equals(UnicastIPAddressInfo other)
    {
      if (other == null)
        return false;
      if (this == other)
        return true;
      if (this.IPAddress.Equals(other.IPAddress))
        return this.IPv4Mask.Equals(other.IPv4Mask);
      return false;
    }

    public override int GetHashCode()
    {
      return (int) HashCode.From<string>(this.IPAddress).And<string>(this.IPv4Mask);
    }
  }
}
