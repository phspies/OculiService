using OculiService.Common.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using VimWrapper;
using Oculi.Commands.Interfaces;
using Oculi.Jobs.Commands;
using Oculi.Jobs.Context;
using Oculi.Contract;

namespace OculiService.CloudProviders.VMware
{
  public class RemoveDriveESX : TaskCommand, ITaskCommand, VolumeInfo
  {
    protected int _RemoveVirtualDisk_Retry = 10;
    protected int _RemoveVirtualDisk_Delay = 5000;
    protected int _RemoveSCSIDevice_Retry = 10;
    protected int _RemoveSCSIDevice_Delay = 5000;

    public RemoveDriveESX(TaskContext context)
      : base(context)
    {
    }

    public bool Invoke(OculiVolumePersistedState volumeInfo)
    {
      this._Logger.Verbose("Locking down job for disk removal for job " + this._Context.JobInfoWrapper.Name);
      try
      {
        lock (Win32Utils.HelperOSLock)
        {
          VmdkProperties local_2 = this._FindDriveVmdkProperties(volumeInfo);
          if (local_2 != null)
          {
            string local_3 = volumeInfo.PNPDeviceID;
            if (local_3 != null)
              this._RemoveSCSIDevice(local_3);
            this._RemoveVirtualDisk(volumeInfo, local_2);
            this._Logger.Verbose("Removed Drive for " + local_3);
          }
          else
            this._Logger.Warning("Could not find hard disk: " + volumeInfo.VirtualDiskFilename);
          return true;
        }
      }
      finally
      {
        this._Logger.Verbose("Releasing lock for disk removal for job " + this._Context.JobInfoWrapper.Name);
      }
    }

    private void _RemoveVirtualDisk(OculiVolumePersistedState volumeInfo, VmdkProperties driveProperties)
    {
      VmdkProperties[] vmdkProps = new VmdkProperties[1]{ driveProperties };
      try
      {
        CUtils.Retry(this._RemoveVirtualDisk_Retry, this._RemoveVirtualDisk_Delay, (CUtils.Workload) (() =>
        {
          this._HelperVm_RemoveVirtualDisk(vmdkProps);
          if (!this._CheckDriveGone(volumeInfo))
            throw new OculiServiceException(0, string.Format("Failed to detach a volume from the Helper VM ({0:d})", (object) volumeInfo.DriveName));
        }));
      }
      catch (Exception ex)
      {
        this._Logger.FormatWarningWithException(ex, "Failed to detach volume {0} from helper VM due to exception:  ", (object) volumeInfo.VolumeName);
        throw;
      }
    }

    private void _RemoveSCSIDevice(string pnpDeviceID)
    {
      try
      {
        CUtils.Retry(this._RemoveSCSIDevice_Retry, this._RemoveSCSIDevice_Delay, (CUtils.Workload) (() =>
        {
          if (!this._Win32Api_RemoveSCSIDevice(pnpDeviceID))
            throw new OculiServiceException(0, string.Format("Failed to remove SCSI device ({0})", (object) pnpDeviceID));
        }));
      }
      catch (Exception ex)
      {
        this._Logger.FormatWarningWithException(ex, "Failed to remove SCSI device ({0}) due to exception:  ", (object) pnpDeviceID);
        throw;
      }
    }

    private bool _CheckDriveGone(OculiVolumePersistedState volumeInfo)
    {
      return this._FindDriveVmdkProperties(volumeInfo) == null;
    }

    private VmdkProperties _FindDriveVmdkProperties(OculiVolumePersistedState volumeInfo)
    {
      return ((IEnumerable<VmdkProperties>) this._HelperVm_GetVMDKInfo()).FirstOrDefault<VmdkProperties>((Func<VmdkProperties, bool>) (v => v.FileName == volumeInfo.VirtualDiskFilename));
    }

    protected virtual VmdkProperties[] _HelperVm_GetVMDKInfo()
    {
      return this._Context.ESXHost.HelperVm().GetVMDKInfo();
    }

    protected virtual bool _Win32Api_RemoveSCSIDevice(string pnpDeviceID)
    {
      return new Win32API(this._Context.Logger).RemoveSCSIDevice(pnpDeviceID);
    }

    protected virtual void _HelperVm_RemoveVirtualDisk(VmdkProperties[] vmdkProps)
    {
      new RemoveVirtualDiskOp(this._Context.ESXHost.HelperVm(), this._Context.ESXHost.VC_Vim, vmdkProps, this._Context.Logger, 120).Run();
    }
  }
}
