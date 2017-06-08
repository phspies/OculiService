using OculiService.Common.Logging;
using OculiService.CloudProviders.Oculi.Contracts;
using OculiService.CloudProviders.Oculi.Interfaces;
using System.Net;
using System;
using OculiService.CloudProviders.Oculi.Contracts.Workload;

namespace OculiService.CloudProviders.Oculi
{
    public class OculiWorkload : OculiApiCore, IOculiWorkload
    {
        public OculiWorkloadType _object;
        public OculiOrganizationType _object_parent;
        private string _base_endpoint_url, _object_endpoint_url;
        public OculiWorkload(OculiApi oculi_api, ILogger logger, OculiOrganizationType object_parent) : base(oculi_api, logger)
        {
            _object_parent = object_parent;
            if (_object == null)
            {
                _base_endpoint_url = String.Format("{0}/organization/{1}/workloads.json", api_prefix, _object_parent.id);
            }
            else
            {
                _object_endpoint_url = String.Format("{0}/organization/{1}/workloads/{2}.json", api_prefix, _object_parent.id, _object.id);
            }
        }

        public OculiWorkloadType Get()
        {
            Resource = _object_endpoint_url;
            _object = GetOperation<OculiWorkloadType>(new object());
            return _object;
        }

        public OculiWorkloadType Create(OculiWorkloadType _workload)
        {
            Resource = _base_endpoint_url;
            _object = PutOperation<OculiWorkloadType>(_workload);
            return _object;
        }
        public OculiWorkloadType Update(OculiWorkloadType _workload)
        {
            Resource = _object_endpoint_url;
            _object = PostOperation<OculiWorkloadType>(_workload);
            return _object;
        }
        public void Destroy(OculiWorkloadType _workload)
        {
            Resource = _object_endpoint_url;
            DeleteOperation<OculiStatusType>(new object());
        }
    }
}
