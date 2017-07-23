using System;
using System.Diagnostics;
using System.Net;
using System.Threading;
using System.Web.Services.Protocols;
using VimWrapper;

public abstract class RetryOpBase
{
  private string _opName;
  protected IVimService _vimService;
  protected VimClientlContext _callCtx;
  protected ILogger _logger;
  private int _maxRetryTimeSec;

  public RetryOpBase(IVimService vimService, string opName, ILogger logger, int maxRetryTimeSec)
  {
    this._vimService = vimService;
    this._opName = opName;
    this._logger = logger;
    this._maxRetryTimeSec = maxRetryTimeSec;
  }

  protected abstract void TryRun();

  protected virtual void Preinvoke()
  {
  }

  protected virtual void Postinvoke()
  {
  }

  protected virtual void OnNonRetriableException()
  {
  }

  protected virtual void LogError(string msg, Exception ex)
  {
  }

  protected virtual bool IsJobStopping()
  {
    return false;
  }

  public void Run()
  {
    bool flag1 = false;
    string str = "";
    this._logger.Verbose(this._opName + ": Preinvoke begin", "Retry");
    this.Preinvoke();
    this._logger.Verbose(this._opName + ": Preinvoke end", "Retry");
    Stopwatch stopwatch = new Stopwatch();
    stopwatch.Start();
    int num1 = 0;
    int num2 = 3;
    do
    {
      bool flag2 = false;
      if (this.IsJobStopping())
      {
        this._logger.Verbose(this._opName + " not initialized, aborting ", "Retry");
        throw new EsxAbortException();
      }
      try
      {
        this._logger.Verbose(this._opName + ": begin", "Retry");
        this.TryRun();
        this._logger.Verbose(this._opName + ": end", "Retry");
        flag1 = true;
      }
      catch (ThreadAbortException ex)
      {
        this._logger.Information("RetriableOps: thread is being aborted", "Retry");
      }
      catch (SoapException ex1)
      {
        str = ex1.Message;
        this.LogError(this._opName + " failed (Soap exception)", (Exception) ex1);
        if (ex1.Detail.InnerXml.Contains("xsi:type=\"NoPermission\""))
        {
          try
          {
            this._vimService.LogOff();
            this._logger.Information(this._opName + ": reconnecting to WebService", "Retry");
            this._vimService.Logon();
            this._logger.Information("Reconnected successfully", "Retry");
            flag2 = true;
          }
          catch (Exception ex2)
          {
          }
        }
      }
      catch (WebException ex)
      {
        str = ex.Message;
        this.LogError(this._opName + " failed (Net.WebException)", (Exception) ex);
      }
      catch (NoSuchVmException ex)
      {
        if (this._maxRetryTimeSec == 0)
          throw;
      }
      catch (Exception ex)
      {
        bool flag3 = false;
        if (ex is EsxException)
          flag3 = ((EsxException) ex).IsErrorRecoverable;
        if (!flag3)
        {
          throw;
        }
        else
        {
          str = ex.Message;
          this.LogError(this._opName + " failed", ex);
        }
      }
      if (!flag1 && !this._callCtx.IsRetriableCall)
      {
        this._logger.Information("NonRetriableException", "Retry");
        try
        {
          this.OnNonRetriableException();
          this._logger.Information(this._opName + ": OnNonRetriableException() succeeded", "Retry");
          flag1 = true;
        }
        catch (Exception ex)
        {
          this.LogError(this._opName + ": OnNonRetriableException failed", ex);
          break;
        }
      }
      if (!flag1)
      {
        if (this.IsJobStopping())
        {
          this._logger.Verbose(this._opName + " not initialized, aborting ", "Retry");
          throw new EsxAbortException();
        }
        if (flag2 || this._maxRetryTimeSec != 0)
        {
          Thread.Sleep(5000);
          this._logger.Information("Retrying " + this._opName + " (" + str + ")", "Retry");
        }
        else
          break;
      }
      else
        break;
    }
    while (num1++ < num2 || stopwatch.ElapsedMilliseconds < (long) (this._maxRetryTimeSec * 1000));
    if (!flag1)
    {
      if (this.IsJobStopping())
        throw new EsxAbortException();
      throw new EsxException(this._opName + " failed: " + str, true);
    }
    this.Postinvoke();
  }
}
