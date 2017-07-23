using OculiService.CloudProviders.Oculi.Contracts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace OculiService.CloudProviders.Oculi.Interfaces
{
    public interface IOculiPlatform
    {
        OculiPlatformListType List();
        OculiPlatformType Create(OculiPlatformType _workload);
        OculiPlatformType Update(OculiPlatformType _workload);
        void Destroy(OculiPlatformType _workload);
    }
}
