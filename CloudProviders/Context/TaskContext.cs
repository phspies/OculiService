using Common_Util.Registry;
using OculiService.Common;
using OculiService.Common.Logging;
using OculiService.Core.Contract;
using OculiService.CloudProviders.Contract;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Oculi.Jobs.Commands;

namespace Oculi.Jobs.Context
{
  public class TaskContext : IDisposable
  {
    public static Mutex DiskSystemLock = new Mutex();
    private object _RemoveJobLock = new object();
    private JobInfo _JobInfo;
    private ILogger _Logger;
    protected JobStateContext _StateContext;
    protected IHelperInformation _HelperInfo;
    public bool _StoppingProtection;
    protected IESXHost _ESXHost;
    protected IHyperVHelper _HyperVHelper;
    protected IHyperV2Helper _HyperV2Helper;
    protected IInformation _Info;
    protected ITaskInfoWrapper _JobInfoWrapper;
    protected RegistryLoader _SystemHive;
    protected RegistryLoader _SoftwareHive;

    public IOculiEngineWrapper Engine { get; set; }

    public IServiceResolver ServiceResolver { get; set; }

    public LocalDiskManagement LocalDiskManagement { get; set; }

    public IWin32API Win32API { get; set; }

    public IOculiCredentialHelper CredentialHelper { get; set; }

    public IVirtualizationEngine RawEngine { get; set; }

    public IContextCleanup ContextCleanup { get; set; }

    public IDiagnosticAggregator StatsAggregator { get; set; }

    public JobInfo JobInfo
    {
      get
      {
        return this._JobInfo;
      }
    }

    public JobStateContext StateContext
    {
      get
      {
        return this._StateContext;
      }
    }

    public ILogger Logger
    {
      get
      {
        return this._Logger;
      }
    }

    public IHelperInformation HelperInfo
    {
      get
      {
        if (this._HelperInfo == null)
          this._HelperInfo = (IHelperInformation) new HelperInformation(this.ServiceResolver);
        return this._HelperInfo;
      }
    }

    public ICommandFactory Factory { get; set; }

    public ICommandInvoker Invoker { get; set; }

    public bool StoppingProtection
    {
      get
      {
        return this._StoppingProtection;
      }
    }

    public IESXHost ESXHost
    {
      get
      {
        if (this._ESXHost == null)
          this._ESXHost = (IESXHost) new Oculi.Jobs.Context.ESXHost(this, this.Logger);
        return this._ESXHost;
      }
    }

    public IHyperVHelper HyperVHelper
    {
      get
      {
        if (this._HyperVHelper == null)
          this._HyperVHelper = (IHyperVHelper) new Oculi.Jobs.Context.HyperVHelper(this);
        return this._HyperVHelper;
      }
    }

    public IHyperV2Helper HyperV2Helper
    {
      get
      {
        if (this._HyperV2Helper == null)
          this._HyperV2Helper = (IHyperV2Helper) new Oculi.Jobs.Context.HyperV2Helper(this);
        return this._HyperV2Helper;
      }
    }

    public IInformation Info
    {
      get
      {
        if (this._Info == null)
          this._Info = (IInformation) new Information(this.JobInfoWrapper);
        return this._Info;
      }
    }

    public ITaskInfoWrapper JobInfoWrapper
    {
      get
      {
        return this._JobInfoWrapper;
      }
      internal set
      {
        this._JobInfoWrapper = value;
      }
    }

    public MachineInfo ReverseHelperMachineInfo { get; set; }

    public RegistryLoader SystemHive
    {
      get
      {
        return this._SystemHive;
      }
      set
      {
        this._SystemHive = value;
      }
    }

    public RegistryLoader SoftwareHive
    {
      get
      {
        return this._SoftwareHive;
      }
      set
      {
        this._SoftwareHive = value;
      }
    }

    public bool IsVraJobClass
    {
      get
      {
        return this._JobInfo.JobType == "Oculi";
      }
    }

    public bool IsV2VJobClass
    {
      get
      {
        if (!(this._JobInfo.JobType == "V2V"))
          return this._JobInfo.JobType == "ClusterAwareHV2V";
        return true;
      }
    }

    public object RemoveJobLock
    {
      get
      {
        return this._RemoveJobLock;
      }
    }

    internal TaskContext()
    {
    }

    public TaskContext(JobInfo jobInfo, ILogger logger, IDiagnosticAggregator statsAggregator, IServiceResolver serviceResolver)
    {
      Invariant.ArgumentNotNull((object) jobInfo, "jobInfo");
      Invariant.ArgumentNotNull((object) logger, "logger");
      this._JobInfo = jobInfo;
      this.StatsAggregator = statsAggregator;
      this.ServiceResolver = serviceResolver;
      this._Logger = (ILogger) new LoggerWrapper(logger);
      this.CredentialHelper = OculiCredentialHelperFactory.Create(this);
      this._JobInfoWrapper = (ITaskInfoWrapper) new Oculi.Jobs.Context.TaskInfoWrapper(this._JobInfo, this.CredentialHelper);
      this._StateContext = new JobStateContext(this);
      this.Factory = (ICommandFactory) new CommandFactory(this);
      this.Invoker = (ICommandInvoker) new CommandInvoker(this.Factory);
      this.Win32API = (IWin32API) new Oculi.Jobs.Win32API(this._Logger);
      this.RawEngine = (IVirtualizationEngine) new VirtualizationEngine(this.ServiceResolver, this.JobInfo);
      this.Engine = (IOculiEngineWrapper) new OculiEngineWrapper(this);
      this.LocalDiskManagement = new LocalDiskManagement(this);
    }

    public TaskContext(PrepareVmInformation bootInfo, ILogger logger, IServiceResolver serviceResolver)
    {
      Invariant.ArgumentNotNull((object) bootInfo, "bootInfo");
      this.ServiceResolver = serviceResolver;
      this._JobInfo = new JobInfo();
      this._JobInfo.Options = new JobOptions();
      this._JobInfo.Options.OculiOptions = new OculiOptions();
      this._JobInfo.Options.ClusterOptions = new ClusterOptions();
      this._JobInfo.JobPersistedState = new JobPersistedState();
      this._JobInfo.JobPersistedState.OculiPersistedState = new OculiPersistedState();
      this._JobInfo.JobPersistedState.OculiPersistedState.SourceMachineInfo = new MachineInfo();
      this._JobInfo.JobPersistedState.OculiPersistedState.SourceMachineInfo.ServerInfo = new ServerInfo();
      this._JobInfo.JobPersistedState.OculiPersistedState.SourceMachineInfo.ServerInfo.OperatingSystem = new OperatingSystemInfo();
      this._JobInfo.JobPersistedState.OculiPersistedState.SourceMachineInfo.ProductInfo = new ProductInfo();
      this._JobInfo.JobPersistedState.OculiPersistedState.State = new JobState()
      {
        HasError = false,
        HighLevel = "CREATED",
        IsInSync = false,
        LowLevel = "Created"
      };
      this._Logger = (ILogger) new LoggerWrapper(logger);
      this._JobInfoWrapper = (ITaskInfoWrapper) new Oculi.Jobs.Context.TaskInfoWrapper(this._JobInfo, (IOculiCredentialHelper) null);
      this._StateContext = new JobStateContext(this);
      this.Factory = (ICommandFactory) new CommandFactory(this);
      this.Invoker = (ICommandInvoker) new CommandInvoker(this.Factory);
      this.LocalDiskManagement = new LocalDiskManagement(this);
      this._JobInfo.JobPersistedState.OculiPersistedState.SourceMachineInfo.ServerInfo.OperatingSystem.Version = bootInfo.OSVersion;
      this._JobInfo.JobPersistedState.OculiPersistedState.SourceMachineInfo.ServerInfo.OperatingSystem.Architecture = bootInfo.CPUArchitecture;
      this._JobInfo.Options.OculiOptions.Hypervisor = bootInfo.Hypervisor;
      this._JobInfo.JobType = "Oculi";
      this._JobInfo.Options.ClusterOptions.TargetIsCluster = false;
      this._JobInfo.Options.OculiOptions.IsSourceHostCluster = false;
      this._JobInfo.JobPersistedState.OculiPersistedState.Nics = ((IEnumerable<VirtualNetworkInterfaceInfo>) bootInfo.NetworkInterfaceInfo).Select<VirtualNetworkInterfaceInfo, OculiInternalVirtualNetworkInterfaceInfo>((Func<VirtualNetworkInterfaceInfo, OculiInternalVirtualNetworkInterfaceInfo>) (nic => OculiInternalVirtualNetworkInterfaceInfo.Create(nic))).ToArray<OculiInternalVirtualNetworkInterfaceInfo>();
      this._JobInfo.JobPersistedState.OculiPersistedState.VmVersion = bootInfo.VmVersion;
      OculiPersistedState vraPersistedState = this._JobInfo.JobPersistedState.OculiPersistedState;
      OculiVolumePersistedState[] volumePersistedStateArray = new OculiVolumePersistedState[1];
      int index = 0;
      OculiVolumePersistedState volumePersistedState = new OculiVolumePersistedState();
      volumePersistedState.MountPoint = bootInfo.SystemVolumeMountPath;
      int num = 1;
      volumePersistedState.IsSystemDrive = num != 0;
      string str = bootInfo.WindowsDir.Substring(0, 1);
      volumePersistedState.Name = str;
      volumePersistedStateArray[index] = volumePersistedState;
      vraPersistedState.VolumePersistedState = volumePersistedStateArray;
      this.LocalDiskManagement.FillVolumeInfoFromMountPath(this.JobInfoWrapper.VolumePersistedState[0]);
      this._JobInfo.JobPersistedState.OculiPersistedState.SourceMachineInfo.ServerInfo.SystemRoot = bootInfo.WindowsDir;
      this._JobInfo.JobPersistedState.OculiPersistedState.SourceMachineInfo.ServerInfo.SystemVolume = bootInfo.WindowsDir.Substring(0, 2);
      this._JobInfo.JobPersistedState.OculiPersistedState.OculiVirtualizationConnectionInfo = new VirtualizationConnectionInfo();
      this._JobInfo.JobPersistedState.OculiPersistedState.OculiVirtualizationConnectionInfo.RepsetName = "";
      this._JobInfo.Options.OculiOptions.VMInfo = new VMInfo();
      this._JobInfoWrapper.TargetOSVersion = OculiService.TargetHostOperatingSystemInfo.Version;
    }

    public void CheckStopping()
    {
      if (this.StoppingProtection)
        throw new OculiServiceException(0, "Job is stopping");
    }

    public void SetHighAndLowLevelState(string highLevelState, string lowLevelState)
    {
      this.StateContext.SetHighAndLowLevelState(highLevelState, lowLevelState);
    }

    public void SetLowLevelState(string lowLevelState)
    {
      this.StateContext.SetLowLevelState(lowLevelState);
    }

    public void SetErrorState(string msg)
    {
      this.StateContext.SetErrorState(msg);
    }

    public void ClearErrorState()
    {
      this.StateContext.ClearErrorState();
    }

    public void Dispose()
    {
      this.Engine.TryDispose();
      this.Engine = (IOculiEngineWrapper) null;
    }
  }
}
