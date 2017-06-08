using OculiService.CloudProviders.Oculi;
using OculiService.CloudProviders.Oculi.Contracts;
using OculiService.CloudProviders.Oculi.Contracts.Workload;
using OculiService.CloudProviders.Oculi.Interfaces;
using OculiService.Common;
using OculiService.Common.Interfaces;
using OculiService.Common.Logging;
using System;
using System.Net;
using System.Reactive.Concurrency;
using System.Threading;

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
            this.logger.Information("This is a test message");

            IOculiApi _api = new OculiApi(this.logger);
            OculiOrganizationType _organization = _api.Organization.Retrieve();
            _api.Organization.Workload.Create(new OculiWorkloadType() { hostname = "test" });
        }

        public void Unload()
        {
        }

    }
}
