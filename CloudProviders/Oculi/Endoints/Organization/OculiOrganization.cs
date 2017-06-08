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
        public OculiOrganization(OculiApi oculi_api, ILogger logger) : base(oculi_api, logger)
        {
            _oculi_api = oculi_api;
            _logger = logger;
        }
        public OculiOrganizationType Retrieve()
        {
            Resource = String.Format("{0}{1}{2}.json",api_prefix,"/organization/",_coreengine_settings.OrganizationID);
            OculiOrganizationType _organization = GetOperation<OculiOrganizationType>(new object());
            _object_organization = _organization;
            return _organization;
        }

        public OculiWorkload Workload { get { return new OculiWorkload(_oculi_api, _logger, _object_organization); } }
    }
}
