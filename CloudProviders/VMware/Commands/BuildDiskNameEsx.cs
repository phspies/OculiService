using OculiService.Commands.Interfaces;
using OculiService.Jobs.Commands;
using Oculi.Jobs.Context;

namespace OculiService.CloudProviders.VMware
{
  public class BuildDiskNameEsx : TaskCommand, IBuildDiskName, ITaskCommandBase
  {
    public BuildDiskNameEsx(TaskContext context)
      : base(context)
    {
    }

    public string Invoke(string serverName, string diskName)
    {
      return serverName + "_" + diskName + ".vmdk";
    }
  }
}
