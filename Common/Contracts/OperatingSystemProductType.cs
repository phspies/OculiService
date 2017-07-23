using System.Runtime.Serialization;

namespace OculiService.Core.Contract
{
  [DataContract]
  public enum OperatingSystemProductType
  {
    [EnumMember] None,
    [EnumMember] Workstation,
    [EnumMember] DomainController,
    [EnumMember] Server,
  }
}
