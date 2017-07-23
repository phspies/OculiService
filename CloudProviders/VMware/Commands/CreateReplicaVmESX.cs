using OculiService.Common.Logging;
using OculiService.Core.Contract;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using VimWrapper;
using OculiService.Commands.Interfaces;
using OculiService.Jobs.Commands;
using Oculi.Jobs.Context;

namespace OculiService.CloudProviders.VMware
{
  public class CreateVmESX : TaskCommand, IJobCommandCommon, ITaskCommandBase
  {
    protected int _VerifyDisksRetryCount = 36;
    protected int _VerifyDisksWaitBetweenRetry = 5000;
    protected ScsiControllerInfo[] _HelperSCSIControllers;
    protected IVimDatastore[] _Datastores;
    private IVimHost _ESXHost;
    private IVimVm _HelperVm;

    public CreateVmESX(TaskContext context)
      : base(context)
    {
    }

    public void Invoke()
    {
      this._Logger.Information("Creating and Configuring the replica vm.");
      this._Context.SetHighAndLowLevelState("PROVISIONING", "ConfiguringVm");
      this._CheckVolumesSpecified();
      this._ESXHost = this._GetHelperESXHost();
      this._HelperVm = this._GetHelperVm();
      this._HelperSCSIControllers = this._GetScsiControllersInfo(this._HelperVm);
      this._Datastores = this._GetDatastores();
      this._ValidateHelperForJob();
      this._CheckStopping();
      this._CheckVmDoesntExist();
      this._ReportVmInfo();
      this._CheckStopping();
      VmCreationInfo vmCreationInfo = this._BuildCreationInfo();
      IVimVm vmWithNoDisks = this._CreateVmWithNoDisks(vmCreationInfo);
      this._Logger.Verbose("Setting up replica vm information for " + this._Context.JobInfoWrapper.VmName);
      vmCreationInfo.Disks[0].SizeMB = Math.Max(vmCreationInfo.Disks[0].SizeMB, 2060L);
      this._Logger.Verbose("Finished creating replica vm:" + this._Context.JobInfoWrapper.VmName);
      this._CheckStopping();
      this._Context.JobInfoWrapper.VmUuid = new Guid(vmWithNoDisks.Uuid);
      this._Logger.Verbose("Saving the  VM's GUID: " + vmWithNoDisks.Uuid);
      if (this._Context.ESXHost.VC_Vim != null && !this._Context.ESXHost.VC_Vim.IsVirtualCenter && this._VirtualNicType() == VirtualNicType.Vmxnet3)
        this._PowerOnAndOff();
      this._CreateAnyAdditionalSCSIControllers(vmWithNoDisks, vmCreationInfo.Disks);
      this._AddDisksTo(vmWithNoDisks, vmCreationInfo.Disks);
      this._VerifyDisksAllAdded(vmWithNoDisks, vmCreationInfo.Disks);
      this._Logger.Information("Creation and configuration of the replica vm complete.");
    }

    private void _PowerOnAndOff()
    {
      this._Logger.Verbose("Powering replica VM on and off to ensure MAC addresses are generated.");
      try
      {
        this._Startup();
        this._PowerOff();
      }
      catch (Exception ex)
      {
        this._Logger.Warning(ex, "Exception power cycling replica VM.");
      }
    }

    private void _CheckVolumesSpecified()
    {
      if (this._Context.JobInfoWrapper.SourceVolumes.Length == 0)
        throw new OculiServiceServiceException(0, "There are no source volumes specified");
    }

    private void _VerifyDisksAllAdded(IVimVm replicaVm, VmDiskInfo[] savedDisks)
    {
      if (!CUtils.WaitForResult(this._VerifyDisksRetryCount, this._VerifyDisksWaitBetweenRetry, (Func<bool>) (() => this._GetVMDKInfo(replicaVm).Length == savedDisks.Length)))
        throw new OculiServiceServiceException(0, "Not enough drives are attached to the replica");
    }

    private void _AddDisksTo(IVimVm replicaVm, VmDiskInfo[] disks)
    {
      ScsiControllerInfo[] scsiControllersInfo = this._GetScsiControllersInfo(replicaVm);
      this._Logger.Verbose("Creating and adding drives to the replica vm");
      this._Context.SetLowLevelState("CreatingDisks");
      this._GetDatastore();
      foreach (VmDiskInfo disk in disks)
      {
        this._CheckStopping();
        bool flag = false;
        string file = disk.File;
        disk.CtrlKey = scsiControllersInfo[disk.CtrlKey - 1].CtrlKey;
        if (!string.IsNullOrEmpty(disk.PreExistingDiskPath))
        {
          try
          {
            IVimDatastore location = disk.Location;
            this._Logger.Verbose("Getting Vmdks Info on datastore: " + location.Name);
            string fileName = Path.GetFileName(disk.PreExistingDiskPath.Replace("\\", "/"));
            Dictionary<string, VmdkFileInfo> vmdksFileInfo = location.GetVmdksFileInfo(disk.PreExistingDiskPath, this._Context.ESXHost.ClientCtx);
            if (vmdksFileInfo == null && vmdksFileInfo.Count < 2)
              throw new EsxException("The pre-existing disk " + disk.PreExistingDiskPath + " could not be found.", false);
            VmdkFileInfo fileInfo1 = vmdksFileInfo[fileName];
            VmdkFileInfo fileInfo2 = vmdksFileInfo[fileInfo1.DataFileName];
            if (fileInfo1.Size + fileInfo2.Size <= 0UL)
              throw new EsxException("The pre-existing disk " + disk.PreExistingDiskPath + " does not have a valid configuration.", false);
            this._EnsureVmFolderExists(replicaVm, disk);
            string targetFile1 = this._VimUtils_GetVolumeName(location) + this._Context.JobInfoWrapper.VmName + "/" + fileInfo1.Name;
            this._Logger.Verbose(string.Format("Pre-stage folder vmdks \"{0}\" moving to destination folder \"{1}\"", (object) fileInfo1.FullName, (object) targetFile1));
            this._VimDatastore_MoveFilesByFullName(location, fileInfo1, targetFile1);
            string targetFile2 = this._VimUtils_GetVolumeName(location) + this._Context.JobInfoWrapper.VmName + "/" + fileInfo2.DataFileName;
            this._Logger.Verbose(string.Format("Pre-stage folder vmdks \"{0}\" moving to destination folder \"{1}\"", (object) fileInfo2.FullName, (object) targetFile2));
            this._VimDatastore_MoveFilesByFullName(location, fileInfo2, targetFile2);
            this._Logger.Verbose(string.Format("Pre-stage Disk found and trying to add \"{0}\"", (object) targetFile1), "CreateESXVm");
            disk.File = targetFile1;
            this._AddVirtualDisk(replicaVm, disk);
            flag = true;
          }
          catch (Exception ex)
          {
            this._Logger.Error(ex, "Exception thrown on attaching a disk:");
            throw ex;
          }
        }
        if (!flag)
        {
          this._Logger.Verbose(string.Format("Trying to create disk \"{0}\"", (object) file));
          this._CreateVirtualDisk(replicaVm, disk);
        }
        this._Logger.Verbose(string.Format("disk \"{0}\" created/added.  Actual file used is \"{1}\"", (object) file, (object) disk.File));
      }
    }

    private void _CreateAnyAdditionalSCSIControllers(IVimVm replicaVm, VmDiskInfo[] savedDisks)
    {
      int num = savedDisks.Length / 15 + 1;
      if (num <= 1)
        return;
      this._Logger.Verbose("Creating additional SCSI controllers on replica vm");
      this._Context.SetLowLevelState("CreatingScsiCtrl");
      int[] scsiBusNumbers = new int[num - 1];
      for (int index = 1; index < num; ++index)
        scsiBusNumbers[index - 1] = index;
      this._AddScsiController(replicaVm, scsiBusNumbers);
    }

    private IVimVm _CreateVmWithNoDisks(VmCreationInfo vmCreationInfo)
    {
      this._CheckStopping();
      this._Context.SetLowLevelState("CreatingVm");
      VmDiskInfo[] disks = vmCreationInfo.Disks;
      vmCreationInfo.Disks = (VmDiskInfo[]) null;
      IVimVm vm = this._CreateVm(vmCreationInfo, this._ESXHost);
      vmCreationInfo.Disks = disks;
      return vm;
    }

    private VmCreationInfo _BuildCreationInfo()
    {
      this._Logger.Verbose("Setting up VMDK file information");
      this._AssignControllersToVolumes();
      VmDiskInfo[] disks = this._BuildDiskList(this._Context.JobInfoWrapper.VolumePersistedState);
      if (disks.Length == 0)
        throw new OculiServiceServiceException(0, "There are no disks to create");
      this._Logger.Verbose("Setting up NIC information");
      this._AssignDeviceAndBusNumbers(this._Context.JobInfoWrapper.Nics);
      OculiServiceInternalVirtualNetworkInterfaceInfo[] replicaNics = this._Context.JobInfoWrapper.Nics;
      string[] nicMapping = this._VirtualNetworkList(replicaNics);
      this._Logger.Verbose("Added information for " + replicaNics.Length.ToString() + " nics.");
      VirtualNicType nicType = this._VirtualNicType();
      this._CheckStopping();
      IVimDatastore replicaDatastore = this._GetDatastore();
      return new VmCreationInfo(this._Context.JobInfoWrapper.VmName, this._Context.JobInfoWrapper.GuestOS, this._Context.JobInfoWrapper.NumberOfCPUs, this._Context.JobInfoWrapper.NumberOfCoresPerProcessor, this._Context.JobInfoWrapper.RamSizeMB, this._ScsiControllerType(), nicType, nicMapping, disks, replicaDatastore);
    }

    private void _AssignDeviceAndBusNumbers(OculiServiceInternalVirtualNetworkInterfaceInfo[] nics)
    {
      int num1 = 17;
      foreach (OculiServiceInternalVirtualNetworkInterfaceInfo nic in nics)
      {
        int num2 = 0;
        nic.BusNumber = num2;
        int num3 = num1++;
        nic.DeviceNumber = num3;
      }
    }

    private VirtualNicType _VirtualNicType()
    {
      OculiServiceInternalVirtualNetworkInterfaceInfo networkInterfaceInfo = ((IEnumerable<OculiServiceInternalVirtualNetworkInterfaceInfo>) this._Context.JobInfoWrapper.Nics).FirstOrDefault<OculiServiceInternalVirtualNetworkInterfaceInfo>();
      if (networkInterfaceInfo == null)
        throw new OculiServiceServiceException(0, "There are no network adapters on the vm.");
      return !networkInterfaceInfo.VirtualNicType.Equals("VmxNet3") ? VirtualNicType.E1000 : VirtualNicType.Vmxnet3;
    }

    private string[] _VirtualNetworkList(OculiServiceInternalVirtualNetworkInterfaceInfo[] nics)
    {
      return ((IEnumerable<OculiServiceInternalVirtualNetworkInterfaceInfo>) nics).Select<OculiServiceInternalVirtualNetworkInterfaceInfo, string>((Func<OculiServiceInternalVirtualNetworkInterfaceInfo, string>) (nic => nic.VirtualNetwork)).ToArray<string>();
    }

    private void _CheckVmDoesntExist()
    {
      if (this._DoesExist())
        throw new OculiServiceServiceException(0, string.Format(" virtual machine name {0} already exist on the target esx host", (object) this._Context.JobInfoWrapper.VmName));
    }

    private VmDiskInfo[] _BuildDiskList(OculiServiceVolumePersistedState[] volumes)
    {
      return ((IEnumerable<OculiServiceVolumePersistedState>) volumes).Select<OculiServiceVolumePersistedState, VmDiskInfo>((Func<OculiServiceVolumePersistedState, VmDiskInfo>) (volume =>
      {
        if (string.IsNullOrEmpty(volume.VirtualDiskPath))
        {
          this._Logger.Error("Volume missing Data Storage URL for job " + this._Context.JobInfoWrapper.Name);
          throw new OculiServiceServiceException(0, "Volume missing Data Storage URL");
        }
        string file = ESXHost.BuildDiskName(this._Context.JobInfoWrapper.SourceHostName, volume.Name);
        IVimDatastore datastoreByUrl = this._GetDatastoreByUrl(volume);
        long datastoreMaxVmdkSizeMb = this._ESXHost.GetDatastoreMaxVmdkSizeMB(datastoreByUrl);
        this._CheckDiskProvisioningType(volume);
        VmDiskInfo disk = new VmDiskInfo(false, file, volume.VmSCSIBus + 1, volume.VmSCSIUnitNumber, Math.Min(volume.DesiredSize / 1048576L, datastoreMaxVmdkSizeMb), "persistent", datastoreByUrl, volume.DiskProvisioningType, volume.PreexistingDiskPath);
        this._ReportOnDisk(disk, volume);
        return disk;
      })).ToArray<VmDiskInfo>();
    }

    private void _CheckDiskProvisioningType(OculiServiceVolumePersistedState volume)
    {
      string provisioningType = volume.DiskProvisioningType;
      if (provisioningType != "Dynamic" && provisioningType != "Fixed" && provisioningType != "Flat Disk")
        throw new OculiServiceServiceException(0, "Unknown DiskProvisioningType \"" + provisioningType + "\"");
    }

    private void _AssignControllersToVolumes()
    {
      int num1 = 1;
      int num2 = 1;
      foreach (OculiServiceVolumePersistedState volumePersistedState in this._Context.JobInfoWrapper.VolumePersistedState)
      {
        this._CheckStopping();
        if (volumePersistedState.IsSystemDrive)
        {
          volumePersistedState.VmSCSIBus = 0;
          volumePersistedState.VmSCSIUnitNumber = 0;
        }
        else
        {
          volumePersistedState.VmSCSIBus = num1 - 1;
          volumePersistedState.VmSCSIUnitNumber = num2;
          ++num2;
          if (num2 == 7)
            ++num2;
          else if (num2 >= 16)
          {
            ++num1;
            num2 = 0;
          }
        }
      }
      this._Context.JobInfoWrapper.VolumePersistedState = ((IEnumerable<OculiServiceVolumePersistedState>) this._Context.JobInfoWrapper.VolumePersistedState).OrderBy<OculiServiceVolumePersistedState, int>((Func<OculiServiceVolumePersistedState, int>) (v => v.VmSCSIBus * 256 + v.VmSCSIUnitNumber)).ToArray<OculiServiceVolumePersistedState>();
    }

    private void _ReportOnDisk(VmDiskInfo disk, OculiServiceVolumePersistedState volume)
    {
      this._Logger.FormatInformation("Volume {0}:", (object) volume.Name);
      this._Logger.FormatInformation("Format:         {0}", (object) volume.DriveFormat);
      this._Logger.FormatInformation("Label:          {0}", (object) volume.Label);
      this._Logger.FormatInformation("Disk size:      {0} MB", (object) this.asMB(volume.Size));
      string message = string.Format("Target disk size:      {0} MB", (object) this.asMB(new long?(volume.DesiredSize)));
      long datastoreMaxVmdkSizeMb = this._ESXHost.GetDatastoreMaxVmdkSizeMB(disk.Location);
      if (volume.DesiredSize / 1048576L > datastoreMaxVmdkSizeMb)
        message += string.Format(" which is greater than the maximum size supported, using {0}MB.", (object) datastoreMaxVmdkSizeMb);
      this._Logger.Information(message);
      this._Logger.FormatInformation("Vm SCSIBus:        {0}", (object) volume.VmSCSIBus);
      this._Logger.FormatInformation("Vm SCSIUnitNumber: {0}", (object) volume.VmSCSIUnitNumber);
      this._Logger.FormatInformation("DiskFileName:   {0}", (object) disk.File);
    }

    private long asMB(long? value)
    {
      if (!value.HasValue)
        return 0;
      long num1 = value.Value / 1048576L;
      long? nullable1 = value;
      long? nullable2 = nullable1.HasValue ? new long?(nullable1.GetValueOrDefault() % 1048576L) : new long?();
      long num2 = 0;
      long num3 = (nullable2.GetValueOrDefault() > num2 ? (nullable2.HasValue ? 1 : 0) : 0) != 0 ? 1L : 0L;
      return num1 + num3;
    }

    private void _ValidateHelperForJob()
    {
      string type = this._HelperSCSIControllers[0].Type;
      if (type.ToUpper().StartsWith("BUS"))
      {
        if (this._Context.JobInfoWrapper.OsInfo.Version.Major != 5 || this._Context.JobInfoWrapper.OsInfo.Architecture != OperatingSystemArchitecture.x86 || (this._Context.HelperInfo.HelperOSVersion.Version.Major != 5 || this._Context.HelperInfo.HelperOSVersion.Architecture != OperatingSystemArchitecture.x86))
          throw new OculiServiceServiceException(0, "Controller type not supported with this OS and CPU combination");
      }
      else if (!type.ToUpper().Contains("LSI"))
        throw new OculiServiceServiceException(0, "Helper VM does not have a supported SCSI controller type");
      if (this._Context.JobInfoWrapper.SourceVolumes.Length > this._HelperSCSIControllers.Length * 15 - this._GetVMDKInfo(this._HelperVm).Length)
        throw new OculiServiceServiceException(0, "Not enough available SCSI slots on helper for this source");
    }

    private void _ReportVmInfo()
    {
      this._Logger.Information("System Info");
      this._Logger.Information("===============");
      this._Logger.Information(string.Format(" VM Name:  {0}", (object) this._Context.JobInfoWrapper.VmName));
      this._Logger.Information(string.Format("Number of CPUs:   {0}", (object) this._Context.JobInfoWrapper.NumberOfCPUs));
      this._Logger.Information(string.Format("Cores per CPU:    {0}", (object) this._Context.JobInfoWrapper.NumberOfCoresPerProcessor));
      this._Logger.Information(string.Format("RAM:              {0} (MB)", (object) this._Context.JobInfoWrapper.RamSizeMB));
      this._Logger.Information(string.Format("Guest OS:         {0}", (object) this._Context.JobInfoWrapper.GuestOS));
    }

    private ScsiControllerType _ScsiControllerType()
    {
      if (this._Context.JobInfoWrapper.OsInfo.Version.Major < 6 && !this._HelperSCSIControllers[0].Type.ToUpper().Contains("LSI"))
        return ScsiControllerType.BusLogic;
      return this._Context.JobInfoWrapper.OsInfo.Version.Major == 6 && this._Context.JobInfoWrapper.OsInfo.Version.Minor >= 2 ? ScsiControllerType.LsiLogicSAS : ScsiControllerType.LsiLogicParallel;
    }

    private void _CheckStopping()
    {
      if (this._Context.StoppingProtection)
        throw new OculiServiceServiceException(0, "Job is stopping");
    }

    protected virtual int _ESXHost_ESXVersion_Major()
    {
      return this._Context.ESXHost.ESXVersion_Major;
    }

    protected virtual IVimDatastore _GetDatastoreByUrl(OculiServiceVolumePersistedState volume)
    {
      return this._GetDatastoreByUrl(volume.VirtualDiskPath);
    }

    protected virtual IVimDatastore _GetDatastoreByUrl(string url)
    {
      if (this._Datastores == null)
        this._Datastores = this._GetDatastores();
      return ((IEnumerable<IVimDatastore>) this._Datastores).First<IVimDatastore>((Func<IVimDatastore, bool>) (d => d.DsProperties.Url.Equals(url, StringComparison.OrdinalIgnoreCase)));
    }

    protected virtual bool _DoesExist()
    {
      return this._Context.Invoker.DoesExist();
    }

    protected virtual void _AddScsiController(IVimVm replicaVm, int[] scsiBusNumbers)
    {
      replicaVm.AddScsiCtrl(scsiBusNumbers, this._ScsiControllerType(), this._Context.ESXHost.ClientCtx);
    }

    protected virtual void _VimDatastore_MoveFilesByFullName(IVimDatastore diskDatastore, VmdkFileInfo fileInfo, string targetFile)
    {
      diskDatastore.MoveFilesByFullName(fileInfo.FullName, targetFile, this._Context.JobInfoWrapper.VmName, true, this._Context.ESXHost.ClientCtx);
    }

    protected virtual string _VimUtils_GetVolumeName(IVimDatastore diskDatastore)
    {
      return VimUtils.GetVolumeName(diskDatastore.Name);
    }

    protected virtual IVimHost _GetHelperESXHost()
    {
      return this._Context.ESXHost.HelperESXHost();
    }

    protected virtual IVimVm _GetHelperVm()
    {
      return this._Context.ESXHost.HelperVm();
    }

    protected virtual ScsiControllerInfo[] _GetScsiControllersInfo(IVimVm replicaVm)
    {
      return replicaVm.GetScsiControllersInfo();
    }

    protected virtual IVimDatastore[] _GetDatastores()
    {
      return this._ESXHost.GetDatastoresAndProperties();
    }

    protected virtual IVimDatastore _GetDatastore()
    {
      return this._GetDatastoreByUrl(this._Context.JobInfoWrapper.DataStoreUrl);
    }

    protected virtual VmdkProperties[] _GetVMDKInfo(IVimVm vm)
    {
      return vm.GetVMDKInfo();
    }

    protected virtual IVimVm _CreateVm(VmCreationInfo vmCreationInfo, IVimHost esxHost)
    {
      CreateVmOp createVmOp = new CreateVmOp(this._Context.ESXHost.VC_Vim, esxHost, vmCreationInfo, this._Context.Logger, 120);
      createVmOp.Run();
      return createVmOp.NewVm;
    }

    protected virtual void _CreateVirtualDisk(IVimVm replicaVm, VmDiskInfo disk)
    {
      new CreateVirtualDiskOp(replicaVm, this._Context.ESXHost.VC_Vim, disk, this._Context.Logger, 1).Run();
    }

    protected virtual void _AddVirtualDisk(IVimVm replicaVm, VmDiskInfo disk)
    {
      new AddVirtualDiskOp(replicaVm, this._Context.ESXHost.VC_Vim, disk, this._Context.Logger, 120).Run();
    }

    protected virtual void _EnsureVmFolderExists(IVimVm replicaVm, VmDiskInfo disk)
    {
      if (disk.Location.DirectoryExist(replicaVm.Name, this._Context.ESXHost.ClientCtx))
        return;
      disk.Location.CreateDirectory(replicaVm.Name);
    }

    protected virtual void _PowerOff()
    {
      this._Context.Invoker.PowerOffVm();
    }

    protected virtual void _Startup()
    {
      this._Context.Invoker.StartupVm();
    }
  }
}
