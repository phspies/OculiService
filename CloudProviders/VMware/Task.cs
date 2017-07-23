using OculiService.Common.Logging;
using System;
using VimApi;

namespace OculiService.CloudProviders.VMware
{
  internal class Task : VCManagedItem, IVimTask, IVimManagedItem
  {
    private ILogger _logger;
    private string _description;
    private DateTime? _completeTime;
    private TaskInfoState _state;

    public string Description
    {
      get
      {
        return this._description;
      }
      set
      {
        this._description = value;
      }
    }

    public DateTime? CompleteTime
    {
      get
      {
        return this._completeTime;
      }
      set
      {
        this._completeTime = value;
      }
    }

    public TaskInfoState State
    {
      get
      {
        return this._state;
      }
      set
      {
        this._state = value;
      }
    }

    internal Task(IVimService vimService, ManagedObjectReference managedObject)
      : base(vimService, managedObject)
    {
      this._logger = ((VCService) vimService).Logger;
    }

    internal Task(IVimService vimService, ManagedObjectReference managedObject, string description, DateTime? completeTime, TaskInfoState state)
      : base(vimService, managedObject)
    {
      this._logger = ((VCService) vimService).Logger;
      this._description = description;
      this._completeTime = completeTime;
      this._state = state;
    }

    public void WaitForResult(string op, VimClientlContext rstate)
    {
      if (this._logger != null)
        this._logger.Verbose("WaitForResult: start, op: " + op, "Task");
      object[] objArray = this.WaitForValues(rstate, new string[3]{ "info.state", "info.error", "info.progress" }, new string[1]{ "state" }, new object[1][]{ new object[2]{ (object) TaskInfoState.success, (object) TaskInfoState.error } });
      if (objArray[0].Equals((object) TaskInfoState.success))
      {
        if (this._logger == null)
          return;
        this._logger.Verbose("WaitForResult: success, op: " + op, "Task");
      }
      else
      {
        if (objArray.Length > 1 && objArray[1] != null)
        {
          if (this._logger != null)
            this._logger.Verbose("WaitForResult: LocalizedMethodFault: " + ((LocalizedMethodFault) objArray[1]).localizedMessage + ", op: " + op, "Task");
          throw new EsxException(((LocalizedMethodFault) objArray[1]).localizedMessage, true);
        }
        if (this._logger != null)
          this._logger.Verbose("WaitForResult: unknown fault, op: " + op, "Task");
        throw new EsxException("WaitForResult: Unknown error returned by Vim", true);
      }
    }

    public void Cancel()
    {
      try
      {
        this.VcService.Service.CancelTask(this.ManagedObject);
      }
      catch (Exception ex)
      {
      }
    }
  }
}
