using System;
using VimApi;

namespace OculiService.CloudProviders.VMware
{
  public interface IVimTask : IVimManagedItem
  {
    DateTime? CompleteTime { get; set; }

    string Description { get; set; }

    TaskInfoState State { get; set; }

    void Cancel();

    void WaitForResult(string op, VimClientlContext rstate);
  }
}
