using OculiService.Common.Logging;
using OculiService.CloudProviders.Oculi.Contracts;
using OculiService.CloudProviders.Oculi.Interfaces;
using System;
using OculiService.CloudProviders.Oculi.Contracts.Workload;

namespace OculiService.CloudProviders.Oculi
{
    public class OculiWorkload : OculiApiCore, IOculiWorkload
    {
        public OculiWorkloadType _workload_object;
        public OculiOrganizationType _organization_object;
        public OculiPlatformType _platform_object;

        public OculiWorkload(OculiApi oculi_api, ILogger logger, OculiOrganizationType organization_object, OculiPlatformType platform_object = null) : base(oculi_api, logger)
        {
            _organization_object = organization_object;
            _platform_object = platform_object;
        }
        public void Set(OculiWorkloadType set_object)
        {
            _workload_object = set_object;
        }
        public OculiWorkloadType Get()
        {
            Resource = SetUrlEndpoint(_workload_object);
            return GetOperation<OculiWorkloadType>(new object());
        }
        public OculiWorkloadListType List()
        {
            Resource = SetUrlEndpoint();
            return GetOperation<OculiWorkloadListType>(new object());
        }

        public OculiWorkloadType Create(OculiWorkloadType _workload)
        {
            Resource = SetUrlEndpoint(_workload);
            return PostOperation<OculiWorkloadType>(_workload);
        }
        public OculiWorkloadType Update(OculiWorkloadType _workload)
        {
            Resource = SetUrlEndpoint(_workload);
            return PutOperation<OculiWorkloadType>(_workload);
        }
        public void Destroy(OculiWorkloadType _workload)
        {
            Resource = SetUrlEndpoint(_workload);
            DeleteOperation<OculiStatusType>(new object());
        }

        private string SetUrlEndpoint(OculiWorkloadType _workload = null)
        {
            if (_platform_object != null)
            {
                if (_workload?.id == null)
                {
                    return String.Format("{0}/organization/{1}/platforms/{2}/workloads.json", api_prefix, _organization_object.id, _platform_object.id);
                }
                else
                {
                    return String.Format("{0}/organization/{1}/platforms/{2}/workloads/{3}.json", api_prefix, _organization_object.id, _platform_object.id, _workload.id);
                }
            }
            else
            {
                if (_workload?.id == null)
                {
                    return String.Format("{0}/organization/{1}/workloads.json", api_prefix, _organization_object.id);
                }
                else
                {
                    return String.Format("{0}/organization/{1}/workloads/{3}.json", api_prefix, _organization_object.id, _workload.id);
                }
            }
        }
    }
}
