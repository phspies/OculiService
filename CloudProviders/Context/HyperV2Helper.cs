using OculiService.Common;
using OculiService.Common.ComponentModel;
using OculiService.Common.Logging;
using OculiService.Common.Xml;
using Oculi.Core;
using OculiService.Core.Contract;
using HV_Wrapper_v2;
using HvWrapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;

namespace Oculi.Jobs.Context
{
  public class HyperV2Helper : IHyperV2Helper
  {
    private IHvServiceCallContext _ServiceCallContext = (IHvServiceCallContext) new HOculi_HvServiceCallContext();
    private TaskContext _Context;
    private ILogger _Logger;
    private ILogger _ServiceLogger;

    public string TargetHostName
    {
      get
      {
        return this._Context.CredentialHelper.TargetHelperHostUriBuilder.NetworkId;
      }
    }

    public bool CanUseGen2Vm
    {
      get
      {
        OperatingSystemVersion version = this._Context.JobInfoWrapper.SourceMachineInfo.ServerInfo.OperatingSystem.Version;
        OperatingSystemVersion targetHostVersion = this.TargetHostVersion;
        if (version.Major * 100 + version.Minor >= 602)
          return targetHostVersion.Major * 100 + targetHostVersion.Minor >= 603;
        return false;
      }
    }

    public OperatingSystemVersion TargetHostVersion
    {
      get
      {
        return OculiService.TargetHostOperatingSystemInfo.Version;
      }
    }

    public HyperV2Helper(TaskContext context)
    {
      this._Context = context;
      this._Logger = this._Context.Logger;
      this._ServiceLogger = this._Logger;
    }

    public string CurrentSourceHostName()
    {
      HostUriBuilder helperHostUriBuilder = this._Context.CredentialHelper.ReverseHelperHostUriBuilder;
      string str = helperHostUriBuilder.NetworkId;
      CUtils.CombinUsernameAndDomain(helperHostUriBuilder.Credentials.UserName, helperHostUriBuilder.Credentials.Domain);
      string password = helperHostUriBuilder.Credentials.Password;
      if (this._Context.JobInfoWrapper.IsSourceHostCluster)
      {
        this._Logger.Information("Finding Owning node for clustered Vm.");
        ClusteredVirtualMachinesInfo[] clusteredVirtualMachines = this._Context.ServiceResolver.Resolve<IClusterProvider>().GetCluster(helperHostUriBuilder.Uri).GetClusteredVirtualMachines();
        ClusteredVirtualMachinesInfo virtualMachinesInfo = ((IEnumerable<ClusteredVirtualMachinesInfo>) clusteredVirtualMachines).Where<ClusteredVirtualMachinesInfo>((Func<ClusteredVirtualMachinesInfo, bool>) (n => ((IEnumerable<VirtualMachine>) n.VirtualMachine).Any<VirtualMachine>((Func<VirtualMachine, bool>) (v => string.Compare(v.Id, this._Context.JobInfoWrapper.SourceVmInfo.Id.ToString("D"), true) == 0)))).FirstOrDefault<ClusteredVirtualMachinesInfo>();
        if (virtualMachinesInfo == null)
        {
          this._Logger.FormatError("Couldn't find the owning node of {0}", (object) this._Context.JobInfoWrapper.SourceVmInfo.DisplayName);
          this._Logger.Verbose("The considered nodes were :");
          foreach (ClusteredVirtualMachinesInfo data in clusteredVirtualMachines)
            this._Logger.Verbose(DataContractUtility.ToXmlString<ClusteredVirtualMachinesInfo>(data));
          throw new OculiServiceException(0, "Couldn't find the owning node for the Vm resource " + this._Context.JobInfoWrapper.SourceVmInfo.DisplayName);
        }
        str = virtualMachinesInfo.OwningNode;
      }
      return str;
    }

    public Uri CurrentSourceUri()
    {
      HostUriBuilder helperHostUriBuilder = this._Context.CredentialHelper.ReverseHelperHostUriBuilder;
      HostUriBuilder hostUriBuilder = new HostUriBuilder(helperHostUriBuilder.Uri);
      CUtils.CombinUsernameAndDomain(helperHostUriBuilder.Credentials.UserName, helperHostUriBuilder.Credentials.Domain);
      string password = helperHostUriBuilder.Credentials.Password;
      if (this._Context.JobInfoWrapper.IsSourceHostCluster)
      {
        this._Logger.Information("Finding Owning node for clustered Vm.");
        ClusteredVirtualMachinesInfo[] clusteredVirtualMachines = this._Context.ServiceResolver.Resolve<IClusterProvider>().GetCluster(helperHostUriBuilder.Uri).GetClusteredVirtualMachines();
        ClusteredVirtualMachinesInfo virtualMachinesInfo = ((IEnumerable<ClusteredVirtualMachinesInfo>) clusteredVirtualMachines).Where<ClusteredVirtualMachinesInfo>((Func<ClusteredVirtualMachinesInfo, bool>) (n => ((IEnumerable<VirtualMachine>) n.VirtualMachine).Any<VirtualMachine>((Func<VirtualMachine, bool>) (v => string.Compare(v.Id, this._Context.JobInfoWrapper.SourceVmInfo.Id.ToString("D"), true) == 0)))).FirstOrDefault<ClusteredVirtualMachinesInfo>();
        if (virtualMachinesInfo == null)
        {
          this._Logger.FormatError("Couldn't find the owning node of {0}", (object) this._Context.JobInfoWrapper.SourceVmInfo.DisplayName);
          this._Logger.Verbose("The considered nodes were :");
          foreach (ClusteredVirtualMachinesInfo data in clusteredVirtualMachines)
            this._Logger.Verbose(DataContractUtility.ToXmlString<ClusteredVirtualMachinesInfo>(data));
          throw new OculiServiceException(0, "Couldn't find the owning node for the Vm resource " + this._Context.JobInfoWrapper.SourceVmInfo.DisplayName);
        }
        hostUriBuilder.NetworkId = virtualMachinesInfo.OwningNode;
      }
      return hostUriBuilder.Uri;
    }

    public IHyperVService SourceHost()
    {
      return (IHyperVService) new HyperVService(this.CurrentSourceHostName(), this._Context.CredentialHelper.ReverseHelperHostUriBuilder.Credentials, this._ServiceLogger, this._ServiceCallContext);
    }

    public string CurrentTargetHostName()
    {
      string str = "localhost";
      if (this._Context.JobInfoWrapper.TargetIsCluster && this._Context.JobInfoWrapper.VmUuid != Guid.Empty)
      {
        this._Logger.Information("Finding Owning node for clustered Vm.");
        ClusteredVirtualMachinesInfo[] clusteredVirtualMachines = this._Context.ServiceResolver.Resolve<IClusterProvider>().GetCluster().GetClusteredVirtualMachines();
        ClusteredVirtualMachinesInfo virtualMachinesInfo = ((IEnumerable<ClusteredVirtualMachinesInfo>) clusteredVirtualMachines).Where<ClusteredVirtualMachinesInfo>((Func<ClusteredVirtualMachinesInfo, bool>) (n => ((IEnumerable<VirtualMachine>) n.VirtualMachine).Any<VirtualMachine>((Func<VirtualMachine, bool>) (v => string.Compare(v.Id, this._Context.JobInfoWrapper.VmUuid.ToString("D"), true) == 0)))).FirstOrDefault<ClusteredVirtualMachinesInfo>();
        if (virtualMachinesInfo == null)
        {
          this._Logger.FormatError("Couldn't find the owning node of vm with uuid {0}", (object) this._Context.JobInfoWrapper.VmUuid);
          this._Logger.Verbose("The considered nodes were :");
          foreach (ClusteredVirtualMachinesInfo data in clusteredVirtualMachines)
            this._Logger.Verbose(DataContractUtility.ToXmlString<ClusteredVirtualMachinesInfo>(data));
          throw new OculiServiceException(0, "Couldn't find the owning node for the Vm with uuid " + (object) this._Context.JobInfoWrapper.VmUuid);
        }
        str = virtualMachinesInfo.OwningNode;
      }
      return str;
    }

    public Uri CurrentTargetUri()
    {
      HostUriBuilder hostUriBuilder = new HostUriBuilder(this._Context.CredentialHelper.TargetHelperUri);
      if (this._Context.JobInfoWrapper.TargetIsCluster)
      {
        this._Logger.Information("Finding Owning node for clustered Vm.");
        ClusteredVirtualMachinesInfo[] clusteredVirtualMachines = this._Context.ServiceResolver.Resolve<IClusterProvider>().GetCluster().GetClusteredVirtualMachines();
        ClusteredVirtualMachinesInfo virtualMachinesInfo = ((IEnumerable<ClusteredVirtualMachinesInfo>) clusteredVirtualMachines).Where<ClusteredVirtualMachinesInfo>((Func<ClusteredVirtualMachinesInfo, bool>) (n => ((IEnumerable<VirtualMachine>) n.VirtualMachine).Any<VirtualMachine>((Func<VirtualMachine, bool>) (v => string.Compare(v.Id, this._Context.JobInfoWrapper.VmUuid.ToString("D"), true) == 0)))).FirstOrDefault<ClusteredVirtualMachinesInfo>();
        if (virtualMachinesInfo == null)
        {
          this._Logger.FormatError("Couldn't find the owning node of vm with uuid {0}", (object) this._Context.JobInfoWrapper.VmUuid);
          this._Logger.Verbose("The considered nodes were :");
          foreach (ClusteredVirtualMachinesInfo data in clusteredVirtualMachines)
            this._Logger.Verbose(DataContractUtility.ToXmlString<ClusteredVirtualMachinesInfo>(data));
          throw new OculiServiceException(0, "Couldn't find the owning node for the Vm with uuid " + (object) this._Context.JobInfoWrapper.VmUuid);
        }
        hostUriBuilder.NetworkId = virtualMachinesInfo.OwningNode;
      }
      return hostUriBuilder.Uri;
    }

    public IHyperVService TargetHost()
    {
      return (IHyperVService) new HyperVService(this.CurrentTargetHostName(), this._Context.CredentialHelper.TargetHelperHostUriBuilder.Credentials, this._ServiceLogger, this._ServiceCallContext);
    }

    public IHyperVService LocalHost()
    {
      return (IHyperVService) new HyperVService("localhost", new NetworkCredential("", ""), this._ServiceLogger, this._ServiceCallContext);
    }
  }
}
