using OculiService.CloudProviders.Oculi;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace OculiService.CloudProviders.VMware
{
  public class AddDisksToExistingVmESX : TaskCommand, IJobCommandESXVm
  {
    private IVimVm _Vm;
    protected ScsiControllerInfo[] _HelperSCSIControllers;
    private IVimHost _ESXHost;
    private IVimVm _HelperVm;

    public AddDisksToExistingVmESX(TaskContext context) : base(context)
    {
    }

    public void Invoke(IVimVm vm)
    {
      this._Vm = vm;
      this._Logger.FormatInformation("Adding disk to the vm {0}", (object) this._Vm.Name);
      this._Context.SetLowLevelState("ConfiguringVm");
      this._ESXHost = this._GetESXHostVim();
      this._HelperVm = this._GetHelperVm();
      this._HelperSCSIControllers = this._HelperVm.GetScsiControllersInfo();
      this._Logger.Information("Setting up VMDK file information");
      this._AssignControllersToVolumes();
      VmDiskInfo[] vmDiskInfoArray = this._BuildDiskList(this._Context.JobInfoWrapper.VolumePersistedState);
      if (vmDiskInfoArray.Length == 0)
        throw new OculiServiceException(0, "There are no disks to create");
      vmDiskInfoArray[0].SizeMB = Math.Max(vmDiskInfoArray[0].SizeMB, 2060L);
      this._CheckStopping();
      this._CreateAnyAdditionalSCSIControllers(this._Vm, vmDiskInfoArray);
      this._AddDisksTo(this._Vm, vmDiskInfoArray);
      AddDisksToExistingVmESX._VerifyDisksAllAdded(this._Vm, vmDiskInfoArray);
      this._Logger.Information("All disks added");
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

    private void _ReportOnDisk(VmDiskInfo disk, OculiServiceVolumePersistedState volume)
    {
      this._Logger.Information("Volume " + volume.VolumeName + ":");
      this._Logger.Information("Format:         " + volume.DriveFormat);
      this._Logger.Information("Label:          " + volume.Label);
      this._Logger.Information("Disk size:      " + (object) this.asMB(volume.Size) + " MB");
      string message = string.Format("Target disk size:      {0}MB", (object) this.asMB(new long?(volume.DesiredSize)));
      long datastoreMaxVmdkSizeMb = this._ESXHost.GetDatastoreMaxVmdkSizeMB(disk.Location);
      if (this.asMB(new long?(volume.DesiredSize)) > datastoreMaxVmdkSizeMb)
        message += string.Format(" which is greater than the maximum size supported, using {0} MB.", (object) datastoreMaxVmdkSizeMb);
      this._Logger.Information(message);
      this._Logger.Information("Vm SCSIBus:        " + (object) volume.VmSCSIBus);
      this._Logger.Information("Vm SCSIUnitNumber: " + (object) volume.VmSCSIUnitNumber);
      this._Logger.Information("DiskFileName:   " + disk.File);
    }

    private string _DiskProvisioningType(string diskProvisioningType)
    {
      if (diskProvisioningType == "Dynamic" || diskProvisioningType == "Fixed" || diskProvisioningType == "Flat Disk")
        return diskProvisioningType;
      throw new OculiServiceServiceException(0, "Unknown DiskProvisioningType \"" + diskProvisioningType + "\"");
    }

    private VmDiskInfo[] _BuildDiskList(OculiServiceVolumePersistedState[] internalVolumeInfos)
    {
      return ((IEnumerable<OculiServiceVolumePersistedState>) internalVolumeInfos).Select<OculiServiceVolumePersistedState, VmDiskInfo>((Func<OculiServiceVolumePersistedState, VmDiskInfo>) (vi =>
      {
        if (string.IsNullOrEmpty(vi.VirtualDiskPath))
        {
          this._Logger.FormatWarning("Volume missing Data Storage URL for vm {0} will use {1}", (object) this._Context.JobInfoWrapper.VmName, (object) this._Context.JobInfoWrapper.DataStoreUrl);
          vi.VirtualDiskPath = this._Context.JobInfoWrapper.DataStoreUrl;
        }
        string file = this._BuildESXDiskName(vi);
        IVimDatastore datastoreByUrl = this._ESXHost.GetDatastoreByUrl(vi.VirtualDiskPath);
        long datastoreMaxVmdkSizeMb = this._ESXHost.GetDatastoreMaxVmdkSizeMB(datastoreByUrl);
        VmDiskInfo disk = new VmDiskInfo(false, file, vi.VmSCSIBus + 1, vi.VmSCSIUnitNumber, Math.Min(vi.DesiredSize / 1048576L, datastoreMaxVmdkSizeMb), "persistent", datastoreByUrl, this._DiskProvisioningType(vi.DiskProvisioningType), vi.PreexistingDiskPath);
        this._ReportOnDisk(disk, vi);
        return disk;
      })).ToArray<VmDiskInfo>();
    }

    private void _CheckStopping()
    {
      if (this._Context.StoppingProtection)
        throw new OculiServiceServiceException(0, "Job is stopping");
    }

    private void _CreateAnyAdditionalSCSIControllers(IVimVm replicaVm, VmDiskInfo[] savedDisks)
    {
      ScsiControllerInfo[] scsiControllersInfo = replicaVm.GetScsiControllersInfo();
      int num = savedDisks.Length / 15 + 1;
      if (num <= scsiControllersInfo.Length)
        return;
      this._Logger.Information("Creating additional SCSI controllers on replica vm");
      this._Context.SetLowLevelState("CreatingScsiCtrl");
      int[] busNumbers = new int[num - scsiControllersInfo.Length];
      for (int length = scsiControllersInfo.Length; length < num; ++length)
        busNumbers[length - scsiControllersInfo.Length] = length;
      replicaVm.AddScsiCtrl(busNumbers, this._ScsiControllerType(), this._Context.ESXHost.ClientCtx);
    }

    private ScsiControllerType _ScsiControllerType()
    {
      if (this._Context.JobInfoWrapper.OsInfo.Version.Major < 6 && !this._HelperVm.GetScsiControllersInfo()[0].Type.ToUpper().Contains("LSI"))
        return ScsiControllerType.BusLogic;
      return this._Context.JobInfoWrapper.OsInfo.Version.Major == 6 && this._Context.JobInfoWrapper.OsInfo.Version.Minor >= 2 ? ScsiControllerType.LsiLogicSAS : ScsiControllerType.LsiLogicParallel;
    }

    private void _AddDisksTo(IVimVm replicaVm, VmDiskInfo[] disks)
    {
      ScsiControllerInfo[] scsiControllersInfo = replicaVm.GetScsiControllersInfo();
      this._Logger.Information("Creating and adding drives to the replica vm", "CreateESXVm");
      this._Context.SetLowLevelState("CreatingDisks");
      List<VmDiskInfo> source = new List<VmDiskInfo>();
      foreach (VmDiskInfo disk in disks)
      {
        this._CheckStopping();
        disk.CtrlKey = scsiControllersInfo[disk.CtrlKey - 1].CtrlKey;
        string file = disk.File;
        if (!string.IsNullOrEmpty(disk.PreExistingDiskPath))
          this._ProcessPrestagedDisk(replicaVm, disk);
        else
          source.Add(disk);
        this._Logger.Information(string.Format("disk \"{0}\" created/added.  Actual file used is \"{1}\"", (object) file, (object) disk.File));
      }
      if (source.Count <= 0)
        return;
      source.Select<VmDiskInfo, long>((Func<VmDiskInfo, long>) (d => d.SizeMB)).Aggregate<long>((Func<long, long, long>) ((acc, next) => acc += next));
      CreateVirtualDiskOp.CreateVirtualDiskCallContext virtualDiskCallContext = new CreateVirtualDiskOp.CreateVirtualDiskCallContext();
      this._CreateVirtualDisks(replicaVm, source.ToArray(), (VimClientlContext) virtualDiskCallContext);
    }

    private void _ProcessPrestagedDisk(IVimVm replicaVm, VmDiskInfo disk)
    {
      try
      {
        IVimDatastore location = disk.Location;
        this._Logger.Information("Getting Vmdks Info on datastore: " + location.Name);
        string fileName = Path.GetFileName(disk.PreExistingDiskPath.Replace("\\", "/"));
        Dictionary<string, VmdkFileInfo> vmdksFileInfo = location.GetVmdksFileInfo(disk.PreExistingDiskPath, this._Context.ESXHost.ClientCtx);
        if (vmdksFileInfo == null && vmdksFileInfo.Count < 2)
          throw new EsxException("The pre-existing disk " + disk.PreExistingDiskPath + " could not be found.", false);
        VmdkFileInfo vmdkFileInfo1 = vmdksFileInfo[fileName];
        VmdkFileInfo vmdkFileInfo2 = vmdksFileInfo[vmdkFileInfo1.DataFileName];
        if (vmdkFileInfo1.Size + vmdkFileInfo2.Size <= 0UL)
          throw new EsxException("The pre-existing disk " + disk.PreExistingDiskPath + " does not have a valid configuration.", false);
        string str = this._VimUtilsGetVolumeName(location) + this._Context.JobInfoWrapper.VmName + "/";
        string name = vmdkFileInfo1.Name;
        string target1 = str + name;
        this._Logger.Information(string.Format("Pre-stage folder vmdk-s \"{0}\" moving to destination folder \"{1}\"", (object) vmdkFileInfo1.FullName, (object) target1));
        location.MoveFilesByFullName(vmdkFileInfo1.FullName, target1, this._Context.JobInfoWrapper.VmName, true, this._Context.ESXHost.ClientCtx);
        string dataFileName = vmdkFileInfo1.DataFileName;
        string target2 = str + dataFileName;
        this._Logger.Information(string.Format("Pre-stage folder vmdk-s \"{0}\" moving to destination folder \"{1}\"", (object) vmdkFileInfo2.FullName, (object) target2));
        location.MoveFilesByFullName(vmdkFileInfo2.FullName, target2, this._Context.JobInfoWrapper.VmName, true, this._Context.ESXHost.ClientCtx);
        this._Logger.Information(string.Format("Pre-stage Disk found and trying to add \"{0}\"", (object) target1));
        disk.File = target1;
        this._AddVirtualDisk(replicaVm, disk);
      }
      catch (Exception ex)
      {
        this._Logger.Error("Exception thrown on attaching a disk" + ex.ToString());
        throw ex;
      }
    }

    private static void _VerifyDisksAllAdded(IVimVm replicaVm, VmDiskInfo[] savedDisks)
    {
      if (!CUtils.WaitForResult(36, 5000, (Func<bool>) (() => replicaVm.GetVMDKInfo().Length == savedDisks.Length)))
        throw new OculiServiceServiceException(0, "Not enough drives are attached to the replica");
    }

    protected virtual void _CreateVirtualDisk(IVimVm replicaVm, VmDiskInfo disk)
    {
      new CreateVirtualDiskOp(replicaVm, this._Context.ESXHost.VC_Vim, disk, this._Context.Logger, 120).Run();
    }

    protected virtual void _CreateVirtualDisks(IVimVm replicaVm, VmDiskInfo[] vmDiskInfo, VimClientlContext ctx)
    {
      replicaVm.CreateVirtualDisks(vmDiskInfo, ctx);
    }

    protected virtual void _AddVirtualDisk(IVimVm replicaVm, VmDiskInfo disk)
    {
      new AddVirtualDiskOp(replicaVm, this._Context.ESXHost.VC_Vim, disk, this._Context.Logger, 120).Run();
    }

    protected virtual IVimVm _GetHelperVm()
    {
      return this._Context.ESXHost.HelperVm();
    }

    protected virtual IVimHost _GetESXHostVim()
    {
      return this._Context.ESXHost.HelperESXHost();
    }

    protected virtual string _BuildESXDiskName(OculiServiceVolumePersistedState vi)
    {
      return ESXHost.BuildDiskName(this._Context.JobInfoWrapper.SourceHostName, vi.Name);
    }

    protected virtual string _VimUtilsGetVolumeName(IVimDatastore diskDatastore)
    {
      return VimUtils.GetVolumeName(diskDatastore.Name);
    }
  }
}
