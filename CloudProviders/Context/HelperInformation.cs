using OculiService.Common.ComponentModel;
using Oculi.Core;
using OculiService.Core.Contract;
using Microsoft.Win32;
using System.Net;

namespace Oculi.Jobs.Context
{
  public class HelperInformation : IHelperInformation
  {
    private MachineInfo _HelperInfo;
    private string _helperDnsName;

    public OperatingSystemInfo HelperOSVersion
    {
      get
      {
        return this._HelperInfo.ServerInfo.OperatingSystem;
      }
    }

    public string HelperSystemDir
    {
      get
      {
        return this._HelperInfo.ServerInfo.SystemPath;
      }
    }

    public string HelperSystemDrive
    {
      get
      {
        return this._HelperInfo.ServerInfo.SystemRoot;
      }
    }

    public string HelperBootDir
    {
      get
      {
        string str = (string) null;
        string name = "SOFTWARE\\Microsoft\\Windows\\CurrentVersion\\Setup";
        using (RegistryKey registryKey = Registry.LocalMachine.OpenSubKey(name))
        {
          if (registryKey != null)
            str = (string) registryKey.GetValue("BootDir");
        }
        return str;
      }
    }

    public string HelperDnsName
    {
      get
      {
        if (string.IsNullOrEmpty(this._helperDnsName))
          this._helperDnsName = Dns.GetHostName();
        return this._helperDnsName;
      }
    }

    public HelperInformation(IServiceResolver childContainer)
    {
      this._HelperInfo = childContainer.Resolve<IEngineProvider>().GetEngine().GetMachineInfo();
    }
  }
}
