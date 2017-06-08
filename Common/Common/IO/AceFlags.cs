using System;
namespace OculiService.Common.IO
{
  [Flags]
  public enum AceFlags : byte
  {
    ObjectInheritAce = 1,
    ContainerInheritAce = 2,
    NoPropogateInheritAce = 4,
    InheritOnlyAce = 8,
    InheritedAce = 16,
  }
}
