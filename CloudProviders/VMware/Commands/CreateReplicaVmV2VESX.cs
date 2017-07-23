using OculiService.Common.Logging;
using OculiService.Core.Contract;
using OculiService.CloudProviders.Contract;
using System;
using System.Collections.Generic;
using System.Linq;
using VimApi;
using VimWrapper;
using OculiService.Commands.Interfaces;
using OculiService.Jobs.Commands;
using Oculi.Jobs.Context;

namespace OculiService.CloudProviders.VMware
{
  public class CreateVmV2VESX : TaskCommand, IJobCommandCommon, ITaskCommandBase
  {
    protected ScsiControllerInfo[] _HelperSCSIControllers;
    private IVimHost _ESXHost;
    private IVimVm _HelperVm;

    public CreateVmV2VESX(TaskContext context)
      : base(context)
    {
    }

    public void Invoke()
    {
      this._Context.Logger.Information("Creating and configuring the replica vm.");
      this._Context.SetHighAndLowLevelState("PROVISIONING", "ConfiguringVm");
      if (this._Context.JobInfoWrapper.SourceVolumes.Length == 0)
        throw new OculiServiceServiceException(0, "There are no source volumes specified");
      this._ESXHost = this._Context.ESXHost.HelperESXHost();
      this._HelperVm = this._Context.ESXHost.HelperVm();
      this._HelperSCSIControllers = this._HelperVm.GetScsiControllersInfo();
      this._ValidateHelperForJob();
      this._CheckStopping();
      this._CreateVmName();
      this._CheckStopping();
      IVimVm vm = this._CreateVm();
      this._Context.Logger.Verbose(" vm \"" + this._Context.JobInfoWrapper.VmName + "\" created");
      this._Context.JobInfoWrapper.VmUuid = new Guid(vm.Uuid);
      this._Context.Logger.Verbose("Saving the  VM's GUID: " + vm.Uuid);
      this._Context.SetLowLevelState("ConfiguringVm");
      this._CreateNewDisks(vm);
      this._Context.Logger.Information("Creation and configuration of replica vm is complete.");
    }

    private void _CreateVmName()
    {
      this._Context.JobInfoWrapper.VmName = this._GetDisplayNameToCreate();
    }

    private void _ReportOnDisk(VmDiskInfo disk, OculiServiceVolumePersistedState volume)
    {
      this._Context.Logger.Information("Volume " + volume.Name + ":");
      this._Context.Logger.Information("Format:         " + volume.DriveFormat);
      this._Context.Logger.Information("Label:          " + volume.Label);
      ILogger logger = this._Context.Logger;
      string str1 = "Disk size:      ";
      long? size = volume.Size;
      long num = 1048576;
      // ISSUE: variable of a boxed type
      object local = (ValueType) (size.HasValue ? new long?(size.GetValueOrDefault() / num) : new long?());
      string str2 = " MB";
      string message = str1 + (object) local + str2;
      logger.Information(message);
      this._Context.Logger.Information("Target disk size:      " + (object) (volume.DesiredSize / 1048576L) + " MB");
      this._Context.Logger.Information("Vm SCSIBus:        " + (object) volume.VmSCSIBus);
      this._Context.Logger.Information("Vm SCSIUnitNumber: " + (object) volume.VmSCSIUnitNumber);
      this._Context.Logger.Information("DiskFileName:   " + disk.File);
    }

    private void _ValidateHelperForJob()
    {
      ScsiControllerInfo[] scsiControllersInfo = this._HelperVm.GetScsiControllersInfo();
      int index = 0;
      string type = scsiControllersInfo[index].Type;
      if (type.ToUpper().StartsWith("BUS"))
      {
        if (this._Context.JobInfoWrapper.OsInfo.Version.Major != 5 || this._Context.JobInfoWrapper.OsInfo.Architecture != OperatingSystemArchitecture.x86 || (this._Context.HelperInfo.HelperOSVersion.Version.Major != 5 || this._Context.HelperInfo.HelperOSVersion.Architecture != OperatingSystemArchitecture.x86))
          throw new OculiServiceServiceException(0, "Controller type not supported with this OS and CPU combination");
      }
      else if (!type.ToUpper().Contains("LSI"))
        throw new OculiServiceServiceException(0, "Helper VM does not have a supported SCSI controller type");
      if (this._Context.JobInfoWrapper.SourceVolumes.Length > scsiControllersInfo.Length * 15 - this._HelperVm.GetVMDKInfo().Length)
        throw new OculiServiceServiceException(0, "Not enough available SCSI slots on helper for this source");
    }

    private void _CheckStopping()
    {
      if (this._Context.StoppingProtection)
        throw new OculiServiceServiceException(0, "Job is stopping");
    }

    protected virtual IVimVm _CreateVm()
    {
      this._Context.SetLowLevelState("CreatingVm");
      IVimDatastore datastoreByUrl = this._ESXHost.GetDatastoreByUrl(this._Context.JobInfoWrapper.DataStoreUrl);
      string replicaVmName = this._Context.JobInfoWrapper.VmName;
      if (datastoreByUrl.IsFolderOnRootExist(replicaVmName, this._Context.ESXHost.ClientCtx))
        throw new OculiServiceServiceException(0, string.Format("The folder for the vm {0} already exists on the datastore {1}", (object) replicaVmName, (object) datastoreByUrl.BracketedName));
      VirtualMachineConfigSpec compatibleConfigSpec = this._Context.ESXHost.SourceVm().GetCompatibleConfigSpec(datastoreByUrl.Name, this._Context.JobInfoWrapper.VmName);
      compatibleConfigSpec.numCPUs = Math.Min((int) this._ESXHost.GetNumberCPU(), compatibleConfigSpec.numCPUs);
      long num = Math.Min(this._ESXHost.GetMemory() / 1048576L, compatibleConfigSpec.memoryMB);
      compatibleConfigSpec.memoryMB = num - num % 4L;
      this._Logger.FormatInformation(" VM will have {0} CPUs and {1}MB memory.", (object) compatibleConfigSpec.numCPUs, (object) compatibleConfigSpec.memoryMB);
      Dictionary<string, string> networkMap = new Dictionary<string, string>();
      ((IEnumerable<VirtualSwitchMapping>) this._Context.JobInfoWrapper.VirtualSwitchMapping).ForEach<VirtualSwitchMapping>((System.Action<VirtualSwitchMapping>) (vsm => networkMap.Add(vsm.SourceVirtualSwitch.Label, vsm.TargetVirtualSwitch.Label)));
      Dictionary<string, string> distributedPortGroupMap = this._Context.ESXHost.SourceVm().GetHostAndProperties().GetDistributedVirtualPortgroups();
      ((IEnumerable<VirtualDeviceConfigSpec>) compatibleConfigSpec.deviceChange).Where<VirtualDeviceConfigSpec>((Func<VirtualDeviceConfigSpec, bool>) (vdcs =>
      {
        if (vdcs.device is VirtualEthernetCard)
          return vdcs.device.backing is VirtualEthernetCardDistributedVirtualPortBackingInfo;
        return false;
      })).ForEach<VirtualDeviceConfigSpec>((System.Action<VirtualDeviceConfigSpec>) (vdcs =>
      {
        string str1 = distributedPortGroupMap[((VirtualEthernetCardDistributedVirtualPortBackingInfo) vdcs.device.backing).port.portgroupKey];
        int length = str1.IndexOf(" (", 0);
        string str2 = str1.Substring(0, length);
        vdcs.device.deviceInfo.summary = str2;
      }));
      return this._ESXHost.CreateVmWithNetworkMapping(compatibleConfigSpec, networkMap, this._Context.ESXHost.ClientCtx);
    }

    private string _FindValueStartsWith(Dictionary<string, string> map, string label)
    {
      foreach (KeyValuePair<string, string> keyValuePair in map)
      {
        if (keyValuePair.Value.StartsWith(label))
          return keyValuePair.Key;
      }
      return (string) null;
    }

    private string _GetDisplayNameToCreate()
    {
      string key = this._Context.ESXHost.SourceVm().VMProperties.Name;
      Dictionary<string, IVimVm> allVmsDictWithName = this._Context.ESXHost.VC_Vim.GetAllVMsDictWithName();
      if (allVmsDictWithName.ContainsKey(key))
      {
        string str = key;
        key = "Copy of " + str;
        int num = 0;
        for (; allVmsDictWithName.ContainsKey(key); key = string.Format("Copy of {0} ({1})", (object) str, (object) num))
          ++num;
      }
      return key;
    }

    protected virtual void _CreateVirtualDisk(IVimVm replicaVm, VmDiskInfo disk)
    {
      new CreateVirtualDiskOp(replicaVm, this._Context.ESXHost.VC_Vim, disk, this._Context.Logger, 120).Run();
    }

    protected virtual void _AddVirtualDisk(IVimVm replicaVm, VmDiskInfo disk)
    {
      new AddVirtualDiskOp(replicaVm, this._Context.ESXHost.VC_Vim, disk, this._Context.Logger, 120).Run();
    }

    protected virtual void _CreateNewDisks(IVimVm replica)
    {
      this._Context.Invoker.AddDisksToExistingVmESX(replica);
    }
  }
}
