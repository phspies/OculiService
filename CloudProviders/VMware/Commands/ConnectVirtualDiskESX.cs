using OculiService.Common.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Oculi.Jobs.Context;
using Common_Util;
using OculiService.Common.Interfaces;
using OculiService.CloudProviders.OperatingSystems.Windows;

namespace OculiService.CloudProviders.VMware
{
  public class ConnectVirtualDiskESX : ConnectVirtualDiskESXCommon, IJobCommandVolumeInfo
  {
    protected int _VolumeIdRetryCount = 3;
    protected int _VolumeIdRetryDelay = 3000;

    public ConnectVirtualDiskESX(TaskContext context)
      : base(context)
    {
    }

    public bool Invoke(OculiServiceVolumePersistedState volumeInfo)
    {
      lock (Win32Utils.HelperOSLock)
      {
        this._PrepareToConnectDrive(volumeInfo);
        List<string> volumeIds = CVolume.GetAllVolumeIDs();
        this._ConnectDrive(volumeInfo);
        this._OnlineNewDisk(volumeInfo);
        List<string> local_3 = new List<string>();
        for (int local_4 = 0; local_4 < this._VolumeIdRetryCount; ++local_4)
        {
          local_3 = CVolume.GetAllVolumeIDs().Where<string>((Func<string, bool>) (v => !volumeIds.Contains(v))).ToList<string>();
          this._Logger.FormatVerbose("Volumes after attaching attempt {1}: {0}", (object) string.Join(", ", local_3.ToArray()), (object) local_4);
          if (local_3.Count <= 0)
            Thread.Sleep(this._VolumeIdRetryDelay);
          else
            break;
        }
        if (local_3.Count > 1)
          throw new OculiServiceServiceException(0, "Too many volumes added with disk " + volumeInfo.VirtualDiskFilename);
        if (local_3.Count == 0)
          throw new OculiServiceServiceException(0, "No volumes added with disk " + volumeInfo.VirtualDiskFilename);
        volumeInfo.VolumeName = local_3[0];
      }
      return true;
    }

    private void _PrepareToConnectDrive(OculiServiceVolumePersistedState volumeInfo)
    {
      this._Logger.Verbose("Preparing to connect drive.");
      this._AvailableSCSISlots = this._GetAvailableSCSISlots();
      this._SetSCSISlot(volumeInfo);
      if (string.IsNullOrEmpty(volumeInfo.VirtualDiskFilename))
        volumeInfo.VirtualDiskFilename = ((IEnumerable<VmdkProperties>) this._GetVmVMDKInfo()).Where<VmdkProperties>((Func<VmdkProperties, bool>) (vmdkInfo =>
        {
          if (vmdkInfo.BusNumber == volumeInfo.VmSCSIBus)
            return vmdkInfo.UnitNumber == volumeInfo.VmSCSIUnitNumber;
          return false;
        })).First<VmdkProperties>().FileName;
      this._AssertVolumeNotAttached(volumeInfo);
      this._Logger.FormatVerbose("The disk associated with filename \"{0}\" will be attached to bus {1} and unit {2}", (object) volumeInfo.VirtualDiskFilename, (object) volumeInfo.ApplianceSCSIBus, (object) volumeInfo.ApplianceSCSIUnitNumber);
    }

    private void _ConnectDrive(OculiServiceVolumePersistedState volumeInfo)
    {
      this._Logger.Verbose("Connecting disk.");
      int keyFromHelperBus = this._GetSCSIControllerKeyFromHelperBus(volumeInfo);
      VmDiskInfo[] vmDiskInfos = new VmDiskInfo[1]{ new VmDiskInfo(true, volumeInfo.VirtualDiskFilename, keyFromHelperBus, volumeInfo.ApplianceSCSIUnitNumber, 0L, "persistent", (IVimDatastore) null, volumeInfo.PreexistingDiskPath) };
      Dictionary<uint, DiskInformation> currentDiskInformation = this._GetCurrentDiskInformation();
      ConnectVirtualDiskESXCommon.LogDiskInformation(currentDiskInformation, this._Logger);
      CUtils.Retry(this._ConnectAttempts, this._ConnectDelay, (CUtils.Workload) (() => this._AddVirtualDisksToHelper(vmDiskInfos)));
      this._Logger.Verbose("VMWare has been asked to add these disks:  " + string.Join(",", ((IEnumerable<VmDiskInfo>) vmDiskInfos).Select<VmDiskInfo, string>((Func<VmDiskInfo, string>) (vmDiskInfo => vmDiskInfo.File)).ToArray<string>()));
      try
      {
        this._FillVolumeInfoFromNewDisk(this._GetNewDisks(currentDiskInformation, 1), volumeInfo);
      }
      catch (Exception ex)
      {
        this._ResetESXConnection();
        this._RemoveVirtDiskFromAppliance(volumeInfo);
        throw;
      }
    }

    protected virtual void _RemoveVirtDiskFromAppliance(OculiServiceVolumePersistedState volumeInfo)
    {
      this._Context.Invoker.RemoveDrive(volumeInfo);
    }

    protected virtual void _OnlineNewDisk(OculiServiceVolumePersistedState volumeInfo)
    {
      this._Context.Invoker.OnlineDisk(volumeInfo);
    }
  }
}
