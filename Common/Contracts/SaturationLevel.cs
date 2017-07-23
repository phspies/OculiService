using System.Runtime.Serialization;

namespace OculiService.Common.Contract
{
  public enum SaturationLevel
  {
    [EnumMember] Unknown,
    [EnumMember] None,
    [EnumMember] Partial,
    [EnumMember] Full,
    [EnumMember] Error,
  }
}
