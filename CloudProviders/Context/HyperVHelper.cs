using OculiService.Common;
using OculiService.Common.ComponentModel;
using OculiService.Common.Logging;
using OculiService.Common.Xml;
using Oculi.Core;
using HvWrapper;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Oculi.Jobs.Context
{
  public class HyperVHelper : IHyperVHelper
  {
    private IHvServiceCallContext _ServiceCallContext = (IHvServiceCallContext) new HOculi_HvServiceCallContext();
    private TaskContext _Context;
    private ILogger _Logger;
    private IHvServiceLogger _ServiceLogger;

    public string TargetHostName
    {
      get
      {
        return this._Context.CredentialHelper.TargetHelperHostUriBuilder.NetworkId;
      }
    }

    public HyperVHelper(TaskContext context)
    {
      this._Context = context;
      this._Logger = this._Context.Logger;
      this._ServiceLogger = (IHvServiceLogger) new HOculi_HvServiceLogger(this._Logger);
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

    public IHvService SourceHost()
    {
      HostUriBuilder helperHostUriBuilder = this._Context.CredentialHelper.ReverseHelperHostUriBuilder;
      string username = CUtils.CombinUsernameAndDomain(helperHostUriBuilder.Credentials.UserName, helperHostUriBuilder.Credentials.Domain);
      string password = helperHostUriBuilder.Credentials.Password;
      IHvService hvServiceByWmi = HvServiceFactory.CreateHvServiceByWmi(this.CurrentSourceHostName(), this._ServiceLogger, this._ServiceCallContext);
      try
      {
        hvServiceByWmi.Logon(username, password);
        return hvServiceByWmi;
      }
      catch (Exception ex)
      {
        this._Logger.Information(ex, "Exception thrown during login:  ");
        hvServiceByWmi.Dispose();
        throw;
      }
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

    public IHvService TargetHost()
    {
      string address = this.CurrentTargetHostName();
      string username = "";
      string password = "";
      string str = "localhost";
      if (address != str)
      {
        HostUriBuilder helperHostUriBuilder = this._Context.CredentialHelper.TargetHelperHostUriBuilder;
        username = CUtils.CombinUsernameAndDomain(helperHostUriBuilder.Credentials.UserName, helperHostUriBuilder.Credentials.Domain);
        password = helperHostUriBuilder.Credentials.Password;
      }
      IHvServiceLogger serviceLogger = this._ServiceLogger;
      IHvServiceCallContext serviceCallContext = this._ServiceCallContext;
      IHvService hvServiceByWmi = HvServiceFactory.CreateHvServiceByWmi(address, serviceLogger, serviceCallContext);
      try
      {
        hvServiceByWmi.Logon(username, password);
        return hvServiceByWmi;
      }
      catch (Exception ex)
      {
        this._Logger.Information(ex, "Exception thrown during login:  ");
        hvServiceByWmi.Dispose();
        throw;
      }
    }

    public IHvService TargetHost_Hv2()
    {
      string address = this.CurrentTargetHostName();
      string username = "";
      string password = "";
      string str = "localhost";
      if (address != str)
      {
        HostUriBuilder helperHostUriBuilder = this._Context.CredentialHelper.TargetHelperHostUriBuilder;
        username = CUtils.CombinUsernameAndDomain(helperHostUriBuilder.Credentials.UserName, helperHostUriBuilder.Credentials.Domain);
        password = helperHostUriBuilder.Credentials.Password;
      }
      IHvServiceLogger serviceLogger = this._ServiceLogger;
      IHvServiceCallContext serviceCallContext = this._ServiceCallContext;
      IHvService service2012ByWmi = HvServiceFactory.CreateHvService2012ByWmi(address, serviceLogger, serviceCallContext);
      try
      {
        service2012ByWmi.Logon(username, password);
        return service2012ByWmi;
      }
      catch (Exception ex)
      {
        this._Logger.Information(ex, "Exception thrown during login:  ");
        service2012ByWmi.Dispose();
        throw;
      }
    }

    public IHvService LocalHost()
    {
      string address = "localhost";
      string username = "";
      string password = "";
      IHvServiceLogger serviceLogger = this._ServiceLogger;
      IHvServiceCallContext serviceCallContext = this._ServiceCallContext;
      IHvService hvServiceByWmi = HvServiceFactory.CreateHvServiceByWmi(address, serviceLogger, serviceCallContext);
      try
      {
        hvServiceByWmi.Logon(username, password);
        return hvServiceByWmi;
      }
      catch (Exception ex)
      {
        this._Logger.Information(ex, "Exception thrown during login:  ");
        hvServiceByWmi.Dispose();
        throw;
      }
    }

    public IHvService LocalHost_Hv2()
    {
      string address = "localhost";
      string username = "";
      string password = "";
      IHvServiceLogger serviceLogger = this._ServiceLogger;
      IHvServiceCallContext serviceCallContext = this._ServiceCallContext;
      IHvService service2012ByWmi = HvServiceFactory.CreateHvService2012ByWmi(address, serviceLogger, serviceCallContext);
      try
      {
        service2012ByWmi.Logon(username, password);
        return service2012ByWmi;
      }
      catch (Exception ex)
      {
        this._Logger.Information(ex, "Exception thrown during login:  ");
        service2012ByWmi.Dispose();
        throw;
      }
    }
  }
}
