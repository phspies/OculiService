using System.Runtime.Serialization;

namespace OculiService.Core.Contract
{
  [DataContract]
  public enum OperatingSystemArchitecture
  {
    [EnumMember] x86 = 0,
    [EnumMember] ia64 = 6,
    [EnumMember] x64 = 9,
  }
}
