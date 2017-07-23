using Oculi.Jobs.Context;
using OculiService.Common.Interfaces;
using OculiService.Jobs.Commands;
using System;
using System.Collections.Generic;
using System.Linq;

namespace OculiService.Commands
{
  public class CompositCommandCommon : TaskCommand, IJobCommandCommon, ITaskCommandBase
  {
    private IEnumerable<IJobCommandCommon> _Commands;

    public CompositCommandCommon(TaskContext context, IEnumerable<IJobCommandCommon> commands) : base(context)
    {
      this._Commands = commands;
    }

    public void Invoke()
    {
      this._Commands.ForEach<IJobCommandCommon>((Action<IJobCommandCommon>) (c => c.Invoke()));
    }
  }
}
