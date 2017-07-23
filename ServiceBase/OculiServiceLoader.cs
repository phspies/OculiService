using OculiService.CloudProviders.Oculi;
using OculiService.CloudProviders.Oculi.Contracts;
using OculiService.CloudProviders.Oculi.Contracts.Workload;
using OculiService.CloudProviders.Oculi.Interfaces;
using OculiService.Common.Database;
using OculiService.Common.Logging;
using System;
using System.Linq;
using System.Reactive.Concurrency;

namespace OculiService.Internal.Service
{
    internal class OculiServiceLoader : IDisposable
    {
        private readonly IScheduler defaultScheduler;
        private readonly OculiServiceLogger logger;

        public ILogger Logger
        {
            get
            {
                return (ILogger)this.logger;
            }
        }

        public OculiServiceLoader(OculiServiceLogger logger, IScheduler defaultScheduler)
        {
            this.logger = logger;
            this.defaultScheduler = defaultScheduler;
        }

        public void Dispose()
        {

        }

        public void Load()
        {
            OculiCoreEngineListType _test = new OculiCoreEngineListType();
            
            this.logger.Information("This is a test message");

            IOculiApi _api = new OculiApi(this.logger);

            OculiWorkloadListType _workload_list = _api.Organization.Workload.List();
            //try
            //{
            //    //_api.Organization.Platform.Workload.Create(new OculiWorkloadType() { hostname = "test" });
            //    using (OculiServiceDatabaseContext _database = new OculiServiceDatabaseContext())
            //    {
            //        ;
            //        _database.Workloads.Add(new OculiWorkloadType() { hostname = "test hostname" });
            //        _database.SaveChanges();
            //    }
            //}
            //catch (Exception ex)
            //{
            //    logger.Critical(ex);
            //}
        }

        public void Unload()
        {
        }

    }
}
