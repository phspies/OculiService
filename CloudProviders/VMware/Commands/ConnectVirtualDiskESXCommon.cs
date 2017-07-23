using Common_Util;
using OculiService.Common.Logging;
using OculiService.Jobs.Commands;
using Oculi.Jobs.Context;
using System;
using System.Collections.Generic;
using System.Linq;
using OculiService.CloudProviders.OperatingSystems.Windows;

namespace OculiService.CloudProviders.VMware
{
  public abstract class ConnectVirtualDiskESXCommon : TaskCommand
  {
    protected int _ConnectDelay = 15000;
    protected int _ConnectAttempts = 3;
    protected int _RescanHardwareRetryDelay = 3000;
    protected int _RescanHardwareRetryAttempts = 5;
    protected List<ScsiSlot> _AvailableSCSISlots;

    public ConnectVirtualDiskESXCommon(TaskContext context) : base(context)
    {
    }

    protected void _SetSCSISlot(OculiServiceVolumePersistedState volume)
    {
      if (this._AvailableSCSISlots.Count < 1)
        throw new OculiServiceServiceException(0, "No slots available for drive");
      ScsiSlot availableScsiSlot = this._AvailableSCSISlots[0];
      this._AvailableSCSISlots.RemoveAt(0);
      volume.ApplianceSCSIBus = availableScsiSlot.bus;
      volume.ApplianceSCSIUnitNumber = availableScsiSlot.unit;
    }

    protected int SCSIKey(int bus, int unit)
    {
      return bus * 16 + unit;
    }

    protected void _RescanHardwareWithRetry()
    {
      if (!CUtils.WaitForResult(this._RescanHardwareRetryAttempts, this._RescanHardwareRetryDelay, new Func<bool>(this._RescanHardware)))
      {
        this._Logger.Error("Hardware rescan failed");
        throw new OculiServiceServiceException(0, "Hardware Rescan failed");
      }
    }

    protected void _AssertVolumeNotAttached(OculiServiceVolumePersistedState volume)
    {
      if (!((IEnumerable<VmdkProperties>) this._GetHelperVmVMDKInfo()).Any<VmdkProperties>((Func<VmdkProperties, bool>) (vmdkInfo => vmdkInfo.FileName == volume.VirtualDiskFilename)))
        return;
      this._Logger.FormatError("The volume associated with the filename {0} is already attached.  Something is wrong.", (object) volume.VirtualDiskFilename);
      this._RemoveDrive(volume);
    }

    protected void _FillVolumeInfoFromNewDisk(Dictionary<uint, DiskInformation> newDisks, OculiServiceVolumePersistedState volume)
    {
      foreach (KeyValuePair<uint, DiskInformation> newDisk in newDisks)
      {
        if ((int) newDisk.Value.ScsiTargetId == volume.ApplianceSCSIUnitNumber)
        {
          this._Logger.FormatVerbose("Adding information to volume with filename \"{0}\"", (object) volume.VirtualDiskFilename);
          volume.DriveName = newDisk.Value.DeviceID;
          volume.DriveIndex = (int) newDisk.Value.Index;
          volume.PNPDeviceID = newDisk.Value.PNPDeviceID;
          if (volume.VolumeSignature != null)
            return;
          if ((int) newDisk.Value.Signature == -1)
          {
            volume.VolumeSignature = this._GetPartitionMountInfo(volume).ToArray();
            return;
          }
          uint signature = newDisk.Value.Signature;
          volume.VolumeSignature = new byte[4]
          {
            (byte) (signature & (uint) byte.MaxValue),
            (byte) ((signature & 65280U) >> 8),
            (byte) ((signature & 16711680U) >> 16),
            (byte) ((signature & 4278190080U) >> 24)
          };
          return;
        }
      }
      throw new OculiServiceServiceException(0, "Disk not found for volume " + volume.Name);
    }

    protected Dictionary<uint, DiskInformation> _GetNewDisks(Dictionary<uint, DiskInformation> existingDisks, int expected)
    {
      Dictionary<uint, DiskInformation> dictionary = CUtils.Retry<Dictionary<uint, DiskInformation>>(10, this._ConnectDelay, (Func<Dictionary<uint, DiskInformation>>) (() =>
      {
        this._Logger.Verbose("Rescanning hardware");
        this._RescanHardwareWithRetry();
        this._Logger.Verbose("Checking for new disks");
        Dictionary<uint, DiskInformation> currentDiskInformation = this._GetCurrentDiskInformation();
        ConnectVirtualDiskESXCommon.LogDiskInformation(currentDiskInformation, this._Logger);
        currentDiskInformation.ExceptWith<uint, DiskInformation>((IDictionary<uint, DiskInformation>) existingDisks);
        this._Logger.FormatVerbose("Check for new disks returns {0}, expecting {1}", (object) currentDiskInformation.Count, (object) expected);
        if (currentDiskInformation.Count < expected)
        {
          this._Logger.Verbose("Not enough disks");
          throw new OculiServiceServiceException(0, "Not enough disks added");
        }
        return currentDiskInformation;
      }));
      if (dictionary.Count <= expected)
        return dictionary;
      this._Logger.Verbose("Too many disks");
      throw new OculiServiceServiceException(0, "Too many disks added");
    }

    protected virtual List<byte> _GetPartitionMountInfo(OculiServiceVolumePersistedState volumeInfo)
    {
      return OculiServiceHelper.Instance.GetPartitionMountInfo((SSMLoggingEventHandler) new OculiService.SSMLogger(this._Context.Logger), volumeInfo.DriveName);
    }

    protected virtual void _RemoveDrive(OculiServiceVolumePersistedState volume)
    {
      this._Context.Invoker.RemoveDrive(volume);
    }

    protected virtual void _ResetESXConnection()
    {
      this._Context.ESXHost.VC_Vim.LogOff();
      this._Context.ESXHost.VC_Vim = (IVimService) null;
    }

    protected virtual void _AddVirtualDisksToHelper(VmDiskInfo[] diskBatch)
    {
      this._Context.ESXHost.HelperVm().AddVirtualDisks(diskBatch, this._Context.ESXHost.ClientCtx);
    }

    protected virtual Dictionary<uint, DiskInformation> _GetCurrentDiskInformation()
    {
      return DiskInformation.GetCurrentDiskInformation();
    }

    protected virtual VmdkProperties[] _GetHelperVmVMDKInfo()
    {
      return this._Context.ESXHost.HelperVm().GetVMDKInfo();
    }

    protected virtual List<ScsiSlot> _GetAvailableSCSISlots()
    {
      return ESXSCSIHelper.GetAvailableScsiSlots(this._Context.ESXHost.HelperVm());
    }

    protected virtual bool _RescanHardware()
    {
      return OculiServiceHelper.Instance.RescanHardware();
    }

    protected virtual VmdkProperties[] _GetVmVMDKInfo()
    {
      return this._Context.ESXHost.Vm().GetVMDKInfo();
    }

    protected virtual int _GetSCSIControllerKeyFromHelperBus(OculiServiceVolumePersistedState info)
    {
      return ESXSCSIHelper.GetSCSIControllerKey(this._Context.ESXHost.HelperVm(), info.ApplianceSCSIBus);
    }

    public static void LogDiskInformation(Dictionary<uint, DiskInformation> diskInformation, ILogger logger)
    {
      logger.Information("Logging Disks found.");
      foreach (DiskInformation diskInformation1 in diskInformation.Values)
      {
        logger.Information(string.Format("ScsiTargetId for disk {0} - {1}", (object) diskInformation1.Index, (object) diskInformation1.ScsiTargetId));
        logger.Information(string.Format("DeviceID for disk {0} - {1}", (object) diskInformation1.Index, (object) diskInformation1.DeviceID));
        logger.Information(string.Format("PNPDeviceID for disk {0} - {1}", (object) diskInformation1.Index, (object) diskInformation1.PNPDeviceID));
        logger.Information(string.Format("Signature for disk {0} - {1}", (object) diskInformation1.Index, (object) diskInformation1.Signature));
      }
    }
  }
}
