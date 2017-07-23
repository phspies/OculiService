using HvWrapper;
using System;

namespace Oculi.Jobs.Context
{
  public interface IHyperVHelper
  {
    string TargetHostName { get; }

    IHvService SourceHost();

    IHvService TargetHost();

    IHvService LocalHost();

    IHvService TargetHost_Hv2();

    IHvService LocalHost_Hv2();

    string CurrentSourceHostName();

    string CurrentTargetHostName();

    Uri CurrentSourceUri();

    Uri CurrentTargetUri();
  }
}
