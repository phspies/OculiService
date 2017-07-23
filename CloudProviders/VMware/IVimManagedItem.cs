using System;
using System.Collections.Generic;
using VimApi;

namespace OculiService.CloudProviders.VMware
{
  public interface IVimManagedItem
  {
    DateTime CurrentTime { get; }

    string Name { get; set; }

    ManagedObjectReference ManagedObject { get; set; }

    IVimManagedItem[] GetChildren();

    string GetName();

    Dictionary<string, object> GetProperties(string[] properties);

    object GetProperty(string property);

    void InitializeManagedObject();

    object[] WaitForValues(VimClientlContext rstate, string[] filterProps, string[] endWaitProps, object[][] expectedVals);
  }
}
