using OculiService.Core.Contract;
using HV_Wrapper_v2;
using System;

namespace Oculi.Jobs.Context
{
  public interface IHyperV2Helper
  {
    string TargetHostName { get; }

    OperatingSystemVersion TargetHostVersion { get; }

    bool CanUseGen2Vm { get; }

    IHyperVService SourceHost();

    IHyperVService TargetHost();

    IHyperVService LocalHost();

    string CurrentSourceHostName();

    string CurrentTargetHostName();

    Uri CurrentSourceUri();

    Uri CurrentTargetUri();
  }
}
