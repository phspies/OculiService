using OculiService.Common.Logging;
using OculiService.CloudProviders.Oculi.Contracts;
using OculiService.CloudProviders.Oculi.Interfaces;
using System.Net;
using System;
using OculiService.CloudProviders.Oculi.Contracts.Workload;

namespace OculiService.CloudProviders.Oculi
{
    public class OculiOrganization : OculiApiCore, IOculiOrganization
    {
        public OculiOrganizationType _object_organization;
        OculiApi _oculi_api;
        ILogger _logger;

        private OculiPlatform _platform_object;
        private OculiWorkload _workload_object;
        public OculiOrganization(OculiApi oculi_api, ILogger logger, OculiOrganizationType object_organization) : base(oculi_api, logger)
        {
            _oculi_api = oculi_api;
            _logger = logger;
            _object_organization = object_organization;
            if (_object_organization == null)
            {
                Retrieve();
            }
        }
        public OculiOrganizationType Retrieve()
        {
            Resource = String.Format("{0}{1}{2}.json",api_prefix,"/organization/",_coreengine_settings.OrganizationID);
            _object_organization = GetOperation<OculiOrganizationType>(new object());
            return _object_organization;
        }
        public OculiPlatform Platform
        {
            get
            {
                _platform_object = (_platform_object == null) ? new OculiPlatform(_oculi_api, _logger, _object_organization) : _platform_object;
                return _platform_object;
            }
        }
        public OculiWorkload Workload
        {
            get
            {
                _workload_object = (_workload_object == null) ? new OculiWorkload(_oculi_api, _logger, _object_organization) : _workload_object;
                return _workload_object;
            }
        }
    }
}
