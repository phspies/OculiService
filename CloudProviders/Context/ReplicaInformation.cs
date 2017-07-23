using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.IO;

namespace Oculi.Jobs.Context
{
  public class Information : IInformation
  {
    private ITaskInfoWrapper _JobInfoWrapper;

    public string SystemDrive
    {
      get
      {
        return this._JobInfoWrapper.SystemDriveLetter + "\\";
      }
    }

    public string WindowsDir
    {
      get
      {
        return Utils.MapDirectoryToDriveMounting(this._JobInfoWrapper.VolumePersistedState, this._JobInfoWrapper.SourceWindowsDir);
      }
    }

    public string RegistryDump
    {
      get
      {
        return this._JobInfoWrapper.SystemDriveLetter + "\\RegistryDump";
      }
    }

    public string RegistryDump
    {
      get
      {
        return Utils.MapDirectoryToDriveMounting(this._JobInfoWrapper.VolumePersistedState, this.RegistryDump);
      }
    }

    public string DTDirectory
    {
      get
      {
        return Utils.MapDirectoryToDriveMounting(this._JobInfoWrapper.VolumePersistedState, this._JobInfoWrapper.SourceDTDirectory);
      }
    }

    public string SystemDrive
    {
      get
      {
        return Utils.MapDirectoryToDriveMounting(this._JobInfoWrapper.VolumePersistedState, this.SystemDrive);
      }
    }

    public string BCDTemplate
    {
      get
      {
        return Path.Combine(this.WindowsDir, "System32\\Config\\BCD-Template");
      }
    }

    public Information(ITaskInfoWrapper jobInfoWrapper)
    {
      this._JobInfoWrapper = jobInfoWrapper;
    }

    public string ConvertToVmDir(string dir, IEnumerable<OculiVolumePersistedState> volumes)
    {
      if ((int) dir[1] == 58 && char.IsLetter(dir[0]))
      {
        string str = char.ToUpper(dir[0]).ToString();
        foreach (OculiVolumePersistedState volume in volumes)
        {
          if (str == volume.Name)
            return volume.MountPoint + dir.Substring(2);
        }
      }
      throw new Exception("Malformed path.  No mapped drive letter.");
    }

    public void AddRunOnce(string name, string path, string rootKey)
    {
      Registry.SetValue("HKEY_LOCAL_MACHINE\\" + rootKey + "\\Microsoft\\Windows\\CurrentVersion\\RunOnce", name, (object) path);
    }

    public void DeleteOculiConnectionFileOnVm(string sourceDTDirectory, IEnumerable<OculiVolumePersistedState> volumes)
    {
      try
      {
        string replicaVmDir = this.ConvertToVmDir(sourceDTDirectory, volumes);
        if (string.IsNullOrEmpty(replicaVmDir))
          return;
        File.Delete(replicaVmDir + "\\connect.sts");
      }
      catch (Exception ex)
      {
      }
    }
  }
}
