using System;
using VimWrapper;

namespace OculiService.Jobs.Commands
{
  public interface ICommandInvoker
  {
    void CreateJobCmd();

    void CreateJob();
  }
}
