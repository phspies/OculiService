using OculiService.Common.Logging;
using Microsoft.Win32;
using OculiService.Jobs.Commands;
using OculiService.Commands.Interfaces;
using Oculi.Jobs.Context;

namespace OculiService.CloudProviders.VMware
{
  public class AdjustVideoAccelerationLevelESXx86 : TaskCommand, IJobCommandCommon, ITaskCommandBase
  {
    public AdjustVideoAccelerationLevelESXx86(TaskContext context) : base(context)
    {
    }

    public void Invoke()
    {
      string key = this._GetCurrentControlSet() + "\\Services\\vmx_svga\\Device0";
      using (RegistryKey subKey = this._RegistryLocalMachineCreateSubKey(key, RegistryKeyPermissionCheck.Default))
      {
        if (subKey == null)
        {
          string str = string.Format("Failed to create/open the key \"{0}\"", (object) key);
          this._Logger.Warning(str, "VmFailover");
          throw new OculiServiceServiceException(0, str);
        }
        this._RegistryKeySetValue(subKey, "Acceleration.Level", (object) 0, RegistryValueKind.DWord);
      }
    }

    protected virtual RegistryKey _RegistryLocalMachineCreateSubKey(string key, RegistryKeyPermissionCheck permissionCheck)
    {
      return Registry.LocalMachine.CreateSubKey(key, permissionCheck);
    }

    protected virtual void _RegistryKeySetValue(RegistryKey key, string name, object value, RegistryValueKind valueKind)
    {
      key.SetValue(name, value, valueKind);
    }

    protected virtual string _GetCurrentControlSet()
    {
      return this._Context.SystemHive.CurrentControlSet;
    }
  }
}
