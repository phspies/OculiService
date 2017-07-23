using OculiService.CloudProviders.Oculi.Contracts;
using OculiService.CloudProviders.Oculi.Contracts.Workload;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace OculiService.CloudProviders.Oculi.Interfaces
{
    public interface IOculiWorkload
    {
        void Set(OculiWorkloadType set_object);
        OculiWorkloadType Get();
        OculiWorkloadType Create(OculiWorkloadType _workload);
        OculiWorkloadType Update(OculiWorkloadType _workload);
        void Destroy(OculiWorkloadType _workload);
    }
}
