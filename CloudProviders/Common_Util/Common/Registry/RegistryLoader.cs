// Decompiled with JetBrains decompiler
// Type: Common_Util.Registry.RegistryLoader
// Assembly: Oculi.Virtualization.Common_Util, Version=8.0.0.1554, Culture=neutral, PublicKeyToken=null
// MVID: 729775AE-A0F3-4545-B89A-1AD01077DBC4
// Assembly location: C:\Downloads\Double-Take\Service\Oculi.Virtualization.Common_Util.dll

using Common_Util.Win32API;
using Microsoft.Win32;
using System;
using System.Security.AccessControl;

namespace Common_Util.Registry
{
  public class RegistryLoader : IDisposable, IRegistryLoader
  {
    private string _HiveFileName;
    private string _RegistryRootPath;
    private bool _Disposed;

    public bool Disposed
    {
      get
      {
        return this._Disposed;
      }
    }

    public string CurrentControlSet
    {
      get
      {
        return this.GetCurrentControlSet();
      }
    }

    public RegistryLoader(string hiveFileName, string registryRootPath)
    {
      this._HiveFileName = hiveFileName;
      this._RegistryRootPath = registryRootPath;
      this._Disposed = false;
      using (new GrantBackupAndRestorePrivilege())
        Win32Interface.TestRegistryResult(0, Win32Interface.RegLoadKey(HKey.LocalMachine, registryRootPath, hiveFileName), string.Format("RegLoadKey failed to load the hive \"{0}\" into the key \"{1}\"", (object) hiveFileName, (object) registryRootPath));
    }

    ~RegistryLoader()
    {
      this.Dispose(false);
    }

    public void Dispose()
    {
      this.Dispose(true);
      GC.SuppressFinalize((object) this);
    }

    private void Dispose(bool disposing)
    {
      if (this._Disposed)
        return;
      Win32Interface.RegUnloadKey(HKey.LocalMachine, this._RegistryRootPath);
      this._Disposed = true;
    }

    public string GetCurrentControlSet()
    {
      return string.Format("{0}\\ControlSet{1:d3}", (object) this._RegistryRootPath, (object) (int) Microsoft.Win32.Registry.GetValue("HKEY_LOCAL_MACHINE\\" + (this._RegistryRootPath + "\\Select"), "Current", (object) null));
    }

    public static void FixKeySecurity(RegistryKey rootKey, string subKey)
    {
      using (RegistryKey registryKey = rootKey.OpenSubKey(subKey, RegistryKeyPermissionCheck.ReadWriteSubTree))
      {
        if (registryKey == null)
          return;
        RegistrySecurity accessControl = registryKey.GetAccessControl();
        string userName = Environment.UserName;
        accessControl.AddAccessRule(new RegistryAccessRule(userName, RegistryRights.FullControl, InheritanceFlags.None, PropagationFlags.None, AccessControlType.Allow));
        registryKey.SetAccessControl(accessControl);
      }
    }

    public static void CreateSecuredKey(string keyName)
    {
      try
      {
        RegistryLoader.FixKeySecurity(Microsoft.Win32.Registry.LocalMachine, keyName);
      }
      catch (Exception ex)
      {
      }
      using (RegistryKey subKey = Microsoft.Win32.Registry.LocalMachine.CreateSubKey(keyName))
      {
        if (subKey == null)
          throw new Exception(string.Format("CreateSecuredKey failed to create/open key \"{0}\"", (object) keyName));
      }
      try
      {
        RegistryLoader.FixKeySecurity(Microsoft.Win32.Registry.LocalMachine, keyName);
      }
      catch (Exception ex)
      {
      }
    }

    public string GetRootKey()
    {
      return this._RegistryRootPath;
    }
  }
}
