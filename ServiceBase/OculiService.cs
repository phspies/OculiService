using OculiService.Common;
using OculiService.Common.Diagnostics;
using OculiService.Common.IO;
using OculiService.Common.Logging;
using OculiService.Common.Service;
using System;
using System.ComponentModel;
using System.Configuration;
using System.IO;
using System.Net;
using System.Reactive.Concurrency;
using System.Threading;

namespace OculiService.Internal.Service
{
    public class OculiService : ServiceBase
    {
        private static readonly string PersistencePath = PathHelpers.GetFullPath("", RelativeFolder.Application, RelativeSubfolder.Data);
        private static readonly string MiniDumpFormatString = "OculiServiceDump_{0}.dmp";
        private OculiServiceLogger logger;
        private OculiServiceLoader loader;
        private bool createDumpsAtShutdown;
        private IContainer components;

        public OculiService()
        {
            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls;
            this.CanShutdown = true;
            this.InitializeComponent();
            AppDomain.CurrentDomain.UnhandledException += new UnhandledExceptionEventHandler(this.AppDomainUnhandledExceptionHandler);
        }

        private void Initialize()
        {
            ConfigurationMonitor.Enabled = true;
            this.logger = new OculiServiceLogger();
            LoggerTraceListener.Logger = (ILogger)this.logger;
            this.loader = new OculiServiceLoader(this.logger, (IScheduler)Scheduler.Default);
        }

        protected override void ReadApplicationSettings()
        {
            base.ReadApplicationSettings();
            bool result;
            if (bool.TryParse(ConfigurationManager.AppSettings["clearArchivedDumpFilesOnStartup"], out result) & result)
                MiniDumpProvider.ClearArchivedDumpFiles(OculiService.PersistencePath);
            bool.TryParse(ConfigurationManager.AppSettings["createCrashDumpsOnShutdown"], out this.createDumpsAtShutdown);
            int.TryParse(ConfigurationManager.AppSettings["maxLogFileSizeBytes"], out OculiServiceLogger.MaxLogFileSizeBytes);
        }

        protected override void OnStart(string[] args)
        {
            ThreadPool.QueueUserWorkItem((WaitCallback)(param0 =>
           {
               this.Initialize();
               using (LogicalOperation.Create("Starting Oculi Service", new object[0]))
               {
                   try
                   {
                       this.loader.Load();
                   }
                   catch (Exception ex)
                   {
                       this.logger.Critical(ex, "Unhandled exception while loading the Oculi Service");
                       throw;
                   }
               }
           }));
        }

        protected override void OnShutdown()
        {
            this.logger.Information("Oculi is shutting down.");
            base.OnShutdown();
        }

        protected override void OnStop()
        {
            using (LogicalOperation.Create("Stopping Oculi Service", new object[0]))
            {
                try
                {
                    this.loader.Unload();
                }
                catch (Exception ex)
                {
                    this.logger.Critical(ex, "Unhandled exception while unloading the Oculi Service");
                    throw;
                }
            }
        }

        private void AppDomainUnhandledExceptionHandler(object sender, UnhandledExceptionEventArgs e)
        {
            if (!e.IsTerminating)
                return;
            using (LogicalOperation.Create(string.Format("An unhandled exception occurred in AppDomain: [{0}]", (object)AppDomain.CurrentDomain.FriendlyName), new object[0]))
            {
                try
                {
                    if (e != null && e.ExceptionObject != null)
                    {
                        if (e.ExceptionObject is Exception)
                            this.loader.Logger.Critical((Exception)e.ExceptionObject);
                        else
                            this.loader.Logger.Critical("Unknown error: " + e.ExceptionObject.ToString());
                    }
                    else
                        this.loader.Logger.Critical("Malformed unknown error");
                }
                finally
                {
                    string filepath = Path.Combine(OculiService.PersistencePath, string.Format(OculiService.MiniDumpFormatString, (object)DateTime.UtcNow.ToString("yyyy_MM_dd_HH_mm_ss")));
                    using (LogicalOperation.Create(string.Format("Writing crash dump to: [{0}]", (object)filepath), new object[0]))
                    {
                        int num = MiniDumpProvider.Write(filepath);
                        if (num != 0)
                            this.loader.Logger.Critical(string.Format("Failed to write crash dump with error code: {0}", (object)num));
                        this.loader.Logger.Verbose("Crash dump written successfully");
                    }
                }
            }
        }

        protected override void Dispose(bool disposing)
        {
            this.OnDispose(disposing);
            if (disposing && this.components != null)
                this.components.Dispose();
            base.Dispose(disposing);
        }

        private void OnDispose(bool disposing)
        {
            if (!disposing || this.loader == null)
                return;
            this.loader.Dispose();
        }

        private void InitializeComponent()
        {
            this.components = (IContainer)new Container();
            this.ServiceName = "OculiService";
        }
    }
}
