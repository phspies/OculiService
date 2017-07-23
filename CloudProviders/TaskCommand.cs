using Oculi.Jobs.Context;
using OculiService.Common.Logging;
using OculiService.Jobs.Context;

namespace OculiService.Jobs.Commands
{
  public abstract class TaskCommand : ITaskCommandBase
  {
    protected TaskContext _Context;

    protected ILogger _Logger
    {
      get
      {
        return this._Context.Logger;
      }
    }

    public virtual string Name
    {
      get
      {
        return this.GetType().Name + "{" + (object) this._Context.JobInfoWrapper.JobGuid + "}";
      }
    }

    public virtual TaskContext Context
    {
      get
      {
        return this._Context;
      }
    }

    public TaskCommand(TaskContext context)
    {
      this._Context = context;
    }
  }
}
