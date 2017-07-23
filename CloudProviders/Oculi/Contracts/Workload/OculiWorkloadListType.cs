using Newtonsoft.Json;
using OculiService.CloudProviders.Oculi.Contracts.Common;
using OculiService.CloudProviders.Oculi.Contracts.Common.Contracts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OculiService.CloudProviders.Oculi.Contracts.Workload
{
    public class OculiWorkloadListType : OculiListType
    {
        [JsonProperty("workloads")]
        public List<OculiWorkloadType> workloads { get; set; }
    }
}
