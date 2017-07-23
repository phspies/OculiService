using System;
using System.Collections.Generic;
using System.Linq;

namespace OculiService.CloudProviders.VMware
{
  public static class ESXSCSIHelper
  {
    public static List<ScsiSlot> GetAvailableScsiSlots(IVimVm v)
    {
      List<ScsiSlot> scsiSlotList = new List<ScsiSlot>();
      Dictionary<int, List<int>> dictionary = new Dictionary<int, List<int>>();
      foreach (ScsiControllerInfo scsiControllerInfo in v.GetScsiControllersInfo())
      {
        List<int> intList = new List<int>(15);
        for (int index = 0; index < 16; ++index)
        {
          if (index != 7)
            intList.Add(index);
        }
        dictionary[scsiControllerInfo.BusNumber] = intList;
      }
      foreach (VmdkProperties vmdkProperties in v.GetVMDKInfo())
      {
        if (dictionary.ContainsKey(vmdkProperties.BusNumber))
          dictionary[vmdkProperties.BusNumber].Remove(vmdkProperties.UnitNumber);
      }
      foreach (KeyValuePair<int, List<int>> keyValuePair in dictionary)
      {
        foreach (int u in keyValuePair.Value)
          scsiSlotList.Add(new ScsiSlot(keyValuePair.Key, u));
      }
      return scsiSlotList;
    }

    public static int GetSCSIControllerKey(IVimVm v, int busNumber)
    {
      ScsiControllerInfo scsiControllerInfo = ((IEnumerable<ScsiControllerInfo>) v.GetScsiControllersInfo()).FirstOrDefault<ScsiControllerInfo>((Func<ScsiControllerInfo, bool>) (si => si.BusNumber == busNumber));
      if (scsiControllerInfo == null)
        throw new OculiServiceServiceException(0, "Internal Error: missing scsi controller " + busNumber.ToString());
      return scsiControllerInfo.CtrlKey;
    }

    public static int GetSCSIBusFromControllerKey(IVimVm v, int ctrlKey)
    {
      ScsiControllerInfo scsiControllerInfo = ((IEnumerable<ScsiControllerInfo>) v.GetScsiControllersInfo()).FirstOrDefault<ScsiControllerInfo>((Func<ScsiControllerInfo, bool>) (si => si.CtrlKey == ctrlKey));
      if (scsiControllerInfo == null)
        throw new OculiServiceServiceException(0, "Internal Error: missing scsi controller with key" + ctrlKey.ToString());
      return scsiControllerInfo.BusNumber;
    }
  }
}
