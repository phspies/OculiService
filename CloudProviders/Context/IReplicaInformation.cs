using System.Collections.Generic;

namespace Oculi.Jobs.Context
{
  public interface IInformation
  {
    string RegistryDump { get; }

    string BCDTemplate { get; }

    string DTDirectory { get; }

    string RegistryDump { get; }

    string SystemDrive { get; }

    string WindowsDir { get; }

    string ConvertToVmDir(string dir, IEnumerable<OculiVolumePersistedState> volumes);

    void AddRunOnce(string name, string path, string rootKey);

    void DeleteOculiConnectionFileOnVm(string sourceDTDirectory, IEnumerable<OculiVolumePersistedState> volumes);
  }
}
