using OculiService.Common;
using OculiService.CloudProviders.Contract;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Reactive.Subjects;

namespace Oculi.Jobs.Context
{
  public class TaskInfoWrapper : ITaskInfoWrapper
  {
    private JobInfo _jobInfo;
    private BehaviorSubject<JobInfo> _persistenceStream;
    private IOculiCredentialHelper _credentialHelper;
    private int _BatchLevel;
    private OperatingSystemVersion _targetOSVersionCache;

    public IObservable<JobInfo> PersistenceStream
    {
      get
      {
        return (IObservable<JobInfo>) this._persistenceStream;
      }
    }

    public string SourceProductLicense
    {
      get
      {
        return this._jobInfo.Options.OculiOptions.SourceProductLicense;
      }
      set
      {
        this._jobInfo.Options.OculiOptions.SourceProductLicense = value;
        this.Save();
      }
    }

    public string Name
    {
      get
      {
        return this._jobInfo.Options.Name;
      }
    }

    public string DataStoreUrl
    {
      get
      {
        return this._jobInfo.Options.OculiOptions.VMInfo.Path;
      }
      set
      {
        this._jobInfo.Options.OculiOptions.VMInfo.Path = value;
        this.Save();
      }
    }

    public string ESXHostName
    {
      get
      {
        return this._jobInfo.Options.OculiOptions.ESXHostName;
      }
    }

    public DateTime FailoverTime
    {
      get
      {
        return this._jobInfo.JobPersistedState.OculiPersistedState.FailoverTime;
      }
      set
      {
        this._jobInfo.JobPersistedState.OculiPersistedState.FailoverTime = value;
        this.Save();
      }
    }

    public string GuestOS
    {
      get
      {
        return this._jobInfo.Options.OculiOptions.VMInfo.OperatingSystem;
      }
    }

    public string HelperUuid
    {
      get
      {
        return this._jobInfo.Options.OculiOptions.ApplianceInfo.Id.ToString();
      }
    }

    public string Hypervisor
    {
      get
      {
        return this._jobInfo.Options.OculiOptions.Hypervisor;
      }
    }

    public Guid JobGuid
    {
      get
      {
        return this._jobInfo.Id;
      }
    }

    public string JobType
    {
      get
      {
        return this._jobInfo.JobType;
      }
    }

    public OculiVolumePersistedState[] VolumePersistedState
    {
      get
      {
        return this._jobInfo.JobPersistedState.OculiPersistedState.VolumePersistedState;
      }
      set
      {
        this._jobInfo.JobPersistedState.OculiPersistedState.VolumePersistedState = value;
        this.Save();
      }
    }

    public OculiService.Core.Contract.VolumeOptions[] VolumeOptions
    {
      get
      {
        return this._jobInfo.Options.OculiOptions.Volumes;
      }
    }

    public VirtualNetworkInterfaceInfo[] NetworkInterfaceInfo
    {
      get
      {
        return this._jobInfo.Options.OculiOptions.SourceNetworkInterfaceInfo;
      }
      set
      {
        this._jobInfo.Options.OculiOptions.SourceNetworkInterfaceInfo = value;
        this.Save();
      }
    }

    public OculiInternalVirtualNetworkInterfaceInfo[] Nics
    {
      get
      {
        return this._jobInfo.JobPersistedState.OculiPersistedState.Nics;
      }
      set
      {
        this._jobInfo.JobPersistedState.OculiPersistedState.Nics = value;
        this.Save();
      }
    }

    public int NumberOfCPUs
    {
      get
      {
        return this._jobInfo.Options.OculiOptions.VMInfo.Cpus;
      }
    }

    public int NumberOfCoresPerProcessor
    {
      get
      {
        if (this._jobInfo.Options.OculiOptions.VMInfo.CoresPerProcessor <= 0)
          return 1;
        return this._jobInfo.Options.OculiOptions.VMInfo.CoresPerProcessor;
      }
    }

    public OperatingSystemInfo OsInfo
    {
      get
      {
        return this._jobInfo.JobPersistedState.OculiPersistedState.SourceMachineInfo.ServerInfo.OperatingSystem;
      }
    }

    public MachineInfo SourceMachineInfo
    {
      get
      {
        return this._jobInfo.JobPersistedState.OculiPersistedState.SourceMachineInfo;
      }
      set
      {
        this._jobInfo.JobPersistedState.OculiPersistedState.SourceMachineInfo = value;
        this.Save();
      }
    }

    public HostUriBuilder ServerVimHostUriBuilder
    {
      get
      {
        return this._credentialHelper.TargetVimServerHostUriBuilder;
      }
    }

    public string PrestageFolder
    {
      get
      {
        return this._jobInfo.Options.OculiOptions.VMInfo.PrestageFolder;
      }
    }

    public long RamSizeMB
    {
      get
      {
        return this._jobInfo.Options.OculiOptions.VMInfo.Memory / 1048576L;
      }
    }

    public string VmName
    {
      get
      {
        return this._jobInfo.Options.OculiOptions.VMInfo.DisplayName;
      }
      set
      {
        this._jobInfo.Options.OculiOptions.VMInfo.DisplayName = value;
        this.Save();
      }
    }

    public Guid VmUuid
    {
      get
      {
        return this._jobInfo.Options.OculiOptions.VMInfo.Id;
      }
      set
      {
        this._jobInfo.Options.OculiOptions.VMInfo.Id = value;
        this.Save();
      }
    }

    public string RepsetName
    {
      get
      {
        return this._jobInfo.JobPersistedState.OculiPersistedState.OculiVirtualizationConnectionInfo.RepsetName;
      }
    }

    public string RunOnceAtStartup
    {
      get
      {
        return this._jobInfo.Options.OculiOptions.VMInfo.RunOnceAtStartup;
      }
      set
      {
        this._jobInfo.Options.OculiOptions.VMInfo.RunOnceAtStartup = value;
        this.Save();
      }
    }

    public bool ShouldShutdownSource
    {
      get
      {
        return this._jobInfo.Options.OculiOptions.WorkloadCustomizationOptions.ShouldShutdownSource;
      }
    }

    public string SourceDTDirectory
    {
      get
      {
        return this._jobInfo.JobPersistedState.OculiPersistedState.SourceMachineInfo.ProductInfo.InstallationPath;
      }
    }

    public string SourceHostDnsName
    {
      get
      {
        return this._credentialHelper.SourceName;
      }
    }

    public string SourceHostName
    {
      get
      {
        return this._credentialHelper.SourceName;
      }
    }

    public string SourcePassword
    {
      get
      {
        return this._credentialHelper.SourceUri.ToConnectionParameters().Credentials.Password;
      }
    }

    public string SourceUserName
    {
      get
      {
        NetworkCredential credentials = this._credentialHelper.SourceUri.ToConnectionParameters().Credentials;
        if (!string.IsNullOrEmpty(credentials.Domain))
          return credentials.Domain + "\\" + credentials.UserName;
        return credentials.UserName;
      }
    }

    public Uri SourceUri
    {
      get
      {
        return this._credentialHelper.SourceUri;
      }
    }

    public OculiService.Core.Contract.VolumeOptions[] SourceVolumes
    {
      get
      {
        return this._jobInfo.Options.OculiOptions.Volumes;
      }
    }

    public string SourceWindowsDir
    {
      get
      {
        return this._jobInfo.JobPersistedState.OculiPersistedState.SourceMachineInfo.ServerInfo.SystemRoot;
      }
    }

    public DateTime StartTime
    {
      get
      {
        return this._jobInfo.JobPersistedState.OculiPersistedState.StartTime;
      }
      set
      {
        this._jobInfo.JobPersistedState.OculiPersistedState.StartTime = value;
        this.Save();
      }
    }

    public JobState State
    {
      get
      {
        return this._jobInfo.JobPersistedState.OculiPersistedState.State;
      }
      set
      {
        this._jobInfo.JobPersistedState.OculiPersistedState.State = value;
        this.Save();
      }
    }

    public bool PowerupAfterFailover
    {
      get
      {
        return this._jobInfo.Options.OculiOptions.WorkloadCustomizationOptions.PowerupAfterFailover;
      }
    }

    public bool AutomaticFailover
    {
      get
      {
        if (this.CoreMonitorOptions != null && this.CoreMonitorOptions.MonitorConfiguration != null && (this.CoreMonitorOptions.MonitorConfiguration.Addresses != null && this.CoreMonitorOptions.MonitorConfiguration.Addresses.Length != 0))
          return ((IEnumerable<MonitoredAddressConfiguration>) this.CoreMonitorOptions.MonitorConfiguration.Addresses).Any<MonitoredAddressConfiguration>((Func<MonitoredAddressConfiguration, bool>) (a =>
          {
            if (a.PingMethods != PingMethods.Network)
              return a.PingMethods == PingMethods.Service;
            return true;
          }));
        return false;
      }
    }

    public CoreMonitorOptions CoreMonitorOptions
    {
      get
      {
        return this._jobInfo.Options.CoreMonitorOptions;
      }
    }

    public DateTime StopTime
    {
      get
      {
        return this._jobInfo.JobPersistedState.OculiPersistedState.StopTime;
      }
      set
      {
        this._jobInfo.JobPersistedState.OculiPersistedState.StopTime = value;
        this.Save();
      }
    }

    public string SystemDriveLetter
    {
      get
      {
        return this._jobInfo.JobPersistedState.OculiPersistedState.SourceMachineInfo.ServerInfo.SystemVolume;
      }
    }

    public HostUriBuilder SourceServerVimHostUriBuilder
    {
      get
      {
        return this._credentialHelper.ReverseVimServerHostUriBuilder;
      }
    }

    public string SourceVMUUID
    {
      get
      {
        return this._jobInfo.Options.OculiOptions.SourceVmInfo.Id.ToString();
      }
    }

    public OculiService.CloudProviders.Contract.VirtualSwitchMapping[] VirtualSwitchMapping
    {
      get
      {
        return this._jobInfo.Options.OculiOptions.VirtualSwitchMapping;
      }
    }

    public OculiService.CloudProviders.Contract.VirtualSwitchMapping[] VirtualSwitchMappingTestFailover
    {
      get
      {
        return this._jobInfo.Options.OculiOptions.VirtualSwitchMappingTestFailover;
      }
    }

    public VMInfo SourceVmInfo
    {
      get
      {
        return this._jobInfo.Options.OculiOptions.SourceVmInfo;
      }
    }

    public VMInfo VmInfo
    {
      get
      {
        return (VMInfo) this._jobInfo.Options.OculiOptions.VMInfo;
      }
    }

    public string SourceESXHostName
    {
      get
      {
        return this._jobInfo.Options.OculiOptions.SourceESXHostName;
      }
    }

    public int ReverseCount
    {
      get
      {
        return this._jobInfo.Options.OculiOptions.ReverseCount;
      }
    }

    public bool IsWANFailoverEnabled
    {
      get
      {
        return this._jobInfo.Options.OculiOptions.IsWANFailoverEnabled;
      }
    }

    public bool IsSourceHostCluster
    {
      get
      {
        return this._jobInfo.Options.OculiOptions.IsSourceHostCluster;
      }
    }

    public bool TargetIsCluster
    {
      get
      {
        return this._jobInfo.Options.ClusterOptions.TargetIsCluster;
      }
    }

    public PathTransformation[] PathTransformations
    {
      get
      {
        return this._jobInfo.Options.CoreConnectionOptions.PathTransformations;
      }
      set
      {
        this._jobInfo.Options.CoreConnectionOptions.PathTransformations = value;
        this.Save();
      }
    }

    public MirrorParameters MirrorParameters
    {
      get
      {
        return this._jobInfo.Options.CoreConnectionOptions.ConnectionStartParameters.MirrorParameters;
      }
    }

    public bool IsUpgradeProcessingRequired
    {
      get
      {
        if (this._jobInfo.JobPersistedState != null && this._jobInfo.JobPersistedState.OculiPersistedState != null)
          return this._jobInfo.JobPersistedState.OculiPersistedState.UpgradeProcessingRequired;
        return false;
      }
      set
      {
        this._jobInfo.JobPersistedState.OculiPersistedState.UpgradeProcessingRequired = value;
        this.Save();
      }
    }

    public VirtualNetworkInterfaceInfo[] NetworkInterfaceInfo
    {
      get
      {
        return this._jobInfo.Options.OculiOptions.NetworkInterfaceInfo;
      }
    }

    public JobOptions Options
    {
      get
      {
        return this._jobInfo.Options;
      }
      set
      {
        this._jobInfo.Options = value;
        this.Save();
      }
    }

    public IDictionary<string, Uri> OtherHostUris
    {
      get
      {
        return this._jobInfo.OtherHostUris;
      }
    }

    public CoreConnectionOptions CoreConnectionOptions
    {
      get
      {
        return this._jobInfo.Options.CoreConnectionOptions;
      }
    }

    public Uri SourceHostUri
    {
      get
      {
        return this._jobInfo.SourceHostUri;
      }
    }

    public Uri TargetHostUri
    {
      get
      {
        return this._jobInfo.TargetHostUri;
      }
    }

    public string SourceUniqueId
    {
      set
      {
        this._jobInfo.SourceUniqueId = value;
        this.Save();
      }
    }

    public string TargetUniqueId
    {
      set
      {
        this._jobInfo.TargetUniqueId = value;
        this.Save();
      }
    }

    public Workload Workload
    {
      get
      {
        return this._jobInfo.Options.Workload;
      }
      set
      {
        this._jobInfo.Options.Workload = value;
        this.Save();
      }
    }

    public OperatingSystemVersion TargetOSVersion
    {
      get
      {
        if (this._targetOSVersionCache == null)
          this._targetOSVersionCache = OculiService.TargetHostOperatingSystemInfo.Version;
        return this._targetOSVersionCache;
      }
      set
      {
        this._targetOSVersionCache = value;
      }
    }

    public string VmVersion
    {
      get
      {
        return this._jobInfo.JobPersistedState.OculiPersistedState.VmVersion;
      }
      set
      {
        this._jobInfo.JobPersistedState.OculiPersistedState.VmVersion = value;
      }
    }

    protected TaskInfoWrapper()
    {
    }

    public TaskInfoWrapper(JobInfo jobInfo, IOculiCredentialHelper credentialHelper)
    {
      this._jobInfo = jobInfo;
      this._credentialHelper = credentialHelper;
      this._persistenceStream = new BehaviorSubject<JobInfo>(jobInfo);
    }

    public void StartBatch()
    {
      this._BatchLevel = this._BatchLevel + 1;
    }

    public void EndBatch()
    {
      if (this._BatchLevel > 0)
        this._BatchLevel = this._BatchLevel - 1;
      if (this._BatchLevel != 0)
        return;
      this.Save();
    }

    public virtual void Save()
    {
      if (this._BatchLevel != 0)
        return;
      this._persistenceStream.OnNext(DataContractUtility.Clone<JobInfo>(this._jobInfo));
    }

    public virtual void Load()
    {
    }

    public void SetHighAndLowLevelState(string highLevelState, string lowLevelState)
    {
      if (this.State.HighLevel == highLevelState && this.State.LowLevel == lowLevelState)
        return;
      this.State.HighLevel = highLevelState;
      this.State.LowLevel = lowLevelState;
      this.State.HasError = false;
      this.Save();
    }

    public void SetLowLevelState(string lowLevelState)
    {
      if (this.State.LowLevel == lowLevelState)
        return;
      this.State.LowLevel = lowLevelState;
      this.State.HasError = false;
      this.Save();
    }

    public void SetErrorState(ErrorInfo newItem)
    {
      if (this.State.RecentErrors.Any<ErrorInfo>((Func<ErrorInfo, bool>) (item => item.Message == newItem.Message)))
        return;
      this.State.RecentErrors.Add(newItem);
      this.State.HasError = this.State.RecentErrors.Any<ErrorInfo>((Func<ErrorInfo, bool>) (e => e.Health == Health.Error));
      this.Save();
    }

    public void ClearErrorState()
    {
      this.State.RecentErrors.Clear();
      this.State.HasError = false;
      this.Save();
    }
  }
}
