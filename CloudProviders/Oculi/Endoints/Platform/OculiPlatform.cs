using OculiService.Common.Logging;
using OculiService.CloudProviders.Oculi.Contracts;
using OculiService.CloudProviders.Oculi.Interfaces;
using System;
using OculiService.CloudProviders.Oculi.Contracts.Workload;

namespace OculiService.CloudProviders.Oculi
{
    public class OculiPlatform : OculiApiCore, IOculiPlatform
    {
        private OculiOrganizationType _organization_object;
        private OculiWorkload _workload_object;
        private OculiApi _oculi_api;
        private ILogger _logger;
        
        public OculiPlatform(OculiApi oculi_api, ILogger logger, OculiOrganizationType object_parent) : base(oculi_api, logger)
        {
            _oculi_api = oculi_api;
            _logger = logger;
            _organization_object = object_parent;
        }
        public OculiPlatformType PlatformObject
        {
            get; set;
        }
        public OculiWorkload Workload
        {
            get
            {
                _workload_object = (_workload_object == null) ? new OculiWorkload(_oculi_api, _logger, _organization_object, PlatformObject) : _workload_object;
                return _workload_object;
            }
        }
        public OculiPlatformListType List()
        {
            Resource = SetUrlEndpoint();
            OculiPlatformListType _object_list = GetOperation<OculiPlatformListType>(new object());
            return _object_list;
        }

        public OculiPlatformType Create(OculiPlatformType _platform)
        {
            Resource = SetUrlEndpoint();
            PlatformObject = PostOperation<OculiPlatformType>(_platform);
            return PlatformObject;
        }
        public OculiPlatformType Update(OculiPlatformType _platform)
        {
            Resource = SetUrlEndpoint();
            PlatformObject = PutOperation<OculiPlatformType>(_platform);
            return PlatformObject;
        }
        public void Destroy(OculiPlatformType _platform)
        {
            Resource = SetUrlEndpoint();
            DeleteOperation<OculiStatusType>(new object());
        }

        private string SetUrlEndpoint()
        {
            if (PlatformObject == null)
            {
                return String.Format("{0}/organization/{1}/platforms.json", api_prefix, _organization_object.id);
            }
            else
            {
                return String.Format("{0}/organization/{1}/platforms/{2}.json", api_prefix, _organization_object.id, PlatformObject.id);
            }
        }
    }
}
