using OculiService.Common.Diagnostics;
using System;
using System.Collections.Generic;

namespace OculiService.Common.Service
{
  public class ServiceBase : System.ServiceProcess.ServiceBase
  {
    private static ServiceBase _instance;

    public static ServiceBase Instance
    {
      get
      {
        return ServiceBase._instance;
      }
    }

    protected ServiceBase()
    {
      ServiceBase._instance = this;
    }

    public virtual void Start(string[] args)
    {
      this.ReadApplicationSettings();
      if (new NamedArgumentCollection((IEnumerable<string>) args).Contains("debug") || ProcessHelper.IsCurrentProcessInteractive())
      {
        this.DebugStart(args);
      }
      else
      {
        this.CanStop = true;
        this.CanPauseAndContinue = false;
        this.CanShutdown = true;
        this.AutoLog = false;
        System.ServiceProcess.ServiceBase.Run((System.ServiceProcess.ServiceBase) this);
      }
    }

    public virtual void DebugStart(string[] args)
    {
      this.OnStart(args);
      Console.WriteLine("{0} service started for debugging purposes. Press any key to stop.", (object) this.ServiceName);
      Console.ReadKey();
      this.OnStop();
    }

    protected virtual void ReadApplicationSettings()
    {
    }

    protected override void Dispose(bool disposing)
    {
      base.Dispose(disposing);
    }
  }
}
