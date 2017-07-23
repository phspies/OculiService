using OculiService.CloudProviders.VMware;
using OculiService.Common;
using OculiService.Common.Logging;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Threading;

namespace Oculi.Jobs.Context
{
    public class ESXHost : IESXHost, IDisposable
    {
        private TaskContext _Context;
        private ILogger _Logger;
        private VimClientlContext _clientCtx;
        private const int VmMaxRetries = 3;
        private const int VmRetryWait = 5000;
        private IVimService _VC_Vim;
        private IVimService _Source_VC_Vim;
        private const string _FailedToLogon = "Failed to log on";

        public VimClientlContext ClientCtx
        {
            get
            {
                lock (this)
                {
                    if (this._clientCtx == null)
                        this._clientCtx = (VimClientlContext)new OculiServiceVimCallContext();
                    return this._clientCtx;
                }
            }
            set
            {
                lock (this)
                    this._clientCtx = value;
            }
        }

        public IVimService VC_Vim
        {
            get
            {
                lock (this)
                {
                    if (this._VC_Vim == null)
                    {
                        HostUriBuilder local_2 = this._Context.JobInfoWrapper.ServerVimHostUriBuilder;
                        this.VC_Vim = this._LoginVimService(local_2.NetworkId, CUtils.GetFullUserName(local_2.Credentials), local_2.Credentials.Password, local_2.Uri.Port);
                    }
                    return this._VC_Vim;
                }
            }
            set
            {
                lock (this)
                {
                    this._VC_Vim = value;
                    if (this._VC_Vim == null)
                        this._Context.ContextCleanup.Remove("VC_Vim");
                    else
                        this._Context.ContextCleanup.Add("VC_Vim", (Action)(() =>
                       {
                           if (this._VC_Vim == null)
                               return;
                           this._VC_Vim.LogOff();
                           this._VC_Vim = (IVimService)null;
                       }));
                }
            }
        }

        public IVimService Source_VC_Vim
        {
            get
            {
                lock (this)
                {
                    if (this._Source_VC_Vim == null)
                    {
                        HostUriBuilder local_2 = this._Context.JobInfoWrapper.SourceServerVimHostUriBuilder;
                        this.Source_VC_Vim = this._LoginVimService(local_2.NetworkId, CUtils.GetFullUserName(local_2.Credentials), local_2.Credentials.Password, local_2.Uri.Port);
                    }
                    return this._Source_VC_Vim;
                }
            }
            set
            {
                lock (this)
                {
                    this._Source_VC_Vim = value;
                    if (this._Source_VC_Vim == null)
                        this._Context.ContextCleanup.Remove("Source_VC_Vim");
                    else
                        this._Context.ContextCleanup.Add("Source_VC_Vim", (Action)(() =>
                       {
                           this._Source_VC_Vim.LogOff();
                           this._Source_VC_Vim = (IVimService)null;
                       }));
                }
            }
        }

        public int ESXVersion_Major
        {
            get
            {
                return Convert.ToInt32(this.HelperESXHost().Properties.Version.Split('.')[0]);
            }
        }

        public int ESXVersion_Minor
        {
            get
            {
                return Convert.ToInt32(this.HelperESXHost().Properties.Version.Split('.')[1]);
            }
        }

        public ESXHost(TaskContext info, ILogger logger)
        {
            this._Context = info;
            this._Logger = logger;
        }

        ~ESXHost()
        {
            this.Dispose(false);
        }

        private T VmRetry<T>(Func<T> wl)
        {
            Exception exception = (Exception)null;
            for (int index = 0; index < 3; ++index)
            {
                try
                {
                    return wl();
                }
                catch (Exception ex)
                {
                    if (ex.Message.Contains("Failed to log on") || ex is OculiServiceServiceException)
                    {
                        throw;
                    }
                    else
                    {
                        if (exception == null)
                            exception = ex;
                        if (this._VC_Vim != null)
                        {
                            this.VC_Vim.LogOff();
                            this.VC_Vim = (IVimService)null;
                        }
                    }
                }
                Thread.Sleep(5000);
            }
            throw exception;
        }

        public IVimVm Vm()
        {
            try
            {
                return this.VmRetry<IVimVm>((Func<IVimVm>)(() =>
               {
                   try
                   {
                       return this.VC_Vim.SearchVmByUuid(this._Context.JobInfoWrapper.VmUuid.ToString());
                   }
                   catch (Exception ex)
                   {
                       this._Logger.Verbose(ex, "Failed to find the  Vm using SearchVmByUuid(), instead got exception:");
                       if (ex.Message.Contains("Failed to log on"))
                           throw;
                   }
                   Dictionary<string, IVimVm> allVmsDictWithUuid = this.VC_Vim.GetAllVMsDictWithUuid();
                   IVimVm vimVm;
                   if (allVmsDictWithUuid.TryGetValue(this._Context.JobInfoWrapper.VmUuid.ToString(), out vimVm))
                   {
                       this._Logger.Verbose("Found the  Vm using an alternate method.");
                       return vimVm;
                   }
                   this._Logger.FormatInformation("Failed to locate  Vm ({0}) The existing UUIDs are: {1}", (object)this._Context.JobInfoWrapper.VmUuid, (object)string.Join(", ", allVmsDictWithUuid.Select<KeyValuePair<string, IVimVm>, string>((Func<KeyValuePair<string, IVimVm>, string>)(v => v.Value.Name + "=> " + v.Key)).ToArray<string>()));
                   throw new OculiServiceServiceException(0, " Vm not found");
               }));
            }
            catch (NoSuchVmException ex)
            {
                return (IVimVm)null;
            }
            catch (Exception ex)
            {
                this._Logger.Information(ex, "Failed to obtain Vm due to exception:");
                throw;
            }
        }

        public IVimVm SourceVm()
        {
            try
            {
                return this.VmRetry<IVimVm>((Func<IVimVm>)(() =>
               {
                   try
                   {
                       return this.Source_VC_Vim.SearchVmByUuid(this._Context.JobInfoWrapper.SourceVMUUID);
                   }
                   catch (Exception ex)
                   {
                       this._Logger.Verbose(ex, "Failed to find the Source Vm using SearchVmByUuid(), instead got exception:");
                       if (ex.Message.Contains("Failed to log on"))
                           throw;
                   }
                   Dictionary<string, IVimVm> allVmsDictWithUuid = this.Source_VC_Vim.GetAllVMsDictWithUuid();
                   IVimVm vimVm;
                   if (allVmsDictWithUuid.TryGetValue(this._Context.JobInfoWrapper.SourceVMUUID, out vimVm))
                   {
                       this._Logger.Verbose("Found the Source Vm using an alternate method.");
                       return vimVm;
                   }
                   this._Logger.FormatInformation("Failed to locate Source Vm ({0}) The existing UUIDs are: {1}", (object)this._Context.JobInfoWrapper.SourceVMUUID, (object)string.Join(", ", allVmsDictWithUuid.Select<KeyValuePair<string, IVimVm>, string>((Func<KeyValuePair<string, IVimVm>, string>)(v => v.Value.Name + "=> " + v.Key)).ToArray<string>()));
                   throw new OculiServiceServiceException(0, "Source Vm not found");
               }));
            }
            catch (NoSuchVmException ex)
            {
                return (IVimVm)null;
            }
            catch (Exception ex)
            {
                this._Logger.Information(ex, "Failed to obtain SourceVm due to exception:");
                throw;
            }
        }

        public IVimVm HelperVm()
        {
            try
            {
                return this.VmRetry<IVimVm>((Func<IVimVm>)(() =>
               {
                   try
                   {
                       return this.VC_Vim.SearchVmByUuid(this._Context.JobInfoWrapper.HelperUuid);
                   }
                   catch (Exception ex)
                   {
                       this._Logger.Verbose(ex, "Failed to find the HelperVm using SearchVmByUuid(), instead got exception:");
                       if (ex.Message.Contains("Failed to log on"))
                           throw;
                   }
                   Dictionary<string, IVimVm> allVmsDictWithUuid = this.VC_Vim.GetAllVMsDictWithUuid();
                   IVimVm vimVm;
                   if (allVmsDictWithUuid.TryGetValue(this._Context.JobInfoWrapper.HelperUuid, out vimVm))
                   {
                       this._Logger.Verbose("Found the Helper Vm using an alternate method.");
                       return vimVm;
                   }
                   this._Logger.FormatInformation("Failed to locate Helper Vm ({0}) The existing UUIDs are: {1}", (object)this._Context.JobInfoWrapper.HelperUuid, (object)string.Join(", ", allVmsDictWithUuid.Select<KeyValuePair<string, IVimVm>, string>((Func<KeyValuePair<string, IVimVm>, string>)(v => v.Value.Name + "=> " + v.Key)).ToArray<string>()));
                   throw new OculiServiceServiceException(0, "Helper Vm not found");
               }));
            }
            catch (Exception ex)
            {
                this._Logger.Information(ex, "Failed to obtain HelperVm due to exception:");
                throw;
            }
        }

        public IVimHost HelperESXHost()
        {
            try
            {
                return this.VmRetry<IVimHost>((Func<IVimHost>)(() => this.VC_Vim.GetHost(this._Context.JobInfoWrapper.ESXHostName)));
            }
            catch (NoSuchVmException ex)
            {
                return (IVimHost)null;
            }
            catch (Exception ex)
            {
                this._Logger.Information(ex, "Failed to obtain ESXHost due to exception:");
                throw;
            }
        }

        public IVimHost SourceESXHost()
        {
            try
            {
                return this.VmRetry<IVimHost>((Func<IVimHost>)(() => this.VC_Vim.GetHost(this._Context.JobInfoWrapper.SourceHostName)));
            }
            catch (NoSuchVmException ex)
            {
                return (IVimHost)null;
            }
            catch (Exception ex)
            {
                this._Logger.Warning(ex, "Failed to obtain ESXHost due to exception:");
                throw;
            }
        }

        private IVimService _LoginVimService(string server, string user, string pass, int port)
        {
            this._Logger.Information("Logging on to ESX server: : " + server);
            Trace.Assert(!string.IsNullOrEmpty(server), "Server cannot be null or empty");
            Trace.Assert(!string.IsNullOrEmpty(user), "User cannot be null or empty");
            lock (this)
            {
                try
                {
                    IVimService vs = VimServiceFactory.CreateVimService(this._Logger, server, user, pass, 5, port);
                    CUtils.RetryIf(5, 5000, (CUtils.Workload)(() => vs.Logon()), (Func<Exception, bool>)(ex => ex is WebException));
                    return vs;
                }
                catch (Exception exception_0)
                {
                    this._Logger.Error(exception_0, "Failed to log on to the ESX server due to the exception:  ");
                    throw new OculiServiceServiceException(0, "Failed to log on to the ESX server due to the exception:  " + exception_0.Message, exception_0);
                }
            }
        }

        public string GetTargetDsName()
        {
            string str = string.Empty;
            try
            {
                str = this.HelperESXHost().GetDatastoreByUrl(this._Context.JobInfoWrapper.DataStoreUrl).Name;
            }
            catch (Exception ex)
            {
                this._Logger.Error(ex, "Failed to get the datastore name due to the exception:  ");
            }
            return str;
        }

        public static string BuildDiskName(string serverName, string diskName)
        {
            return serverName + "_" + diskName + ".vmdk";
        }

        public void Dispose()
        {
            this.Dispose(true);
            GC.SuppressFinalize((object)this);
        }

        private void Dispose(bool disposing)
        {
            if (this._VC_Vim != null)
            {
                this._VC_Vim.LogOff();
                this._VC_Vim = (IVimService)null;
            }
            if (this._Source_VC_Vim == null)
                return;
            this._Source_VC_Vim.LogOff();
            this._Source_VC_Vim = (IVimService)null;
        }
    }
}
