using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OculiService.CloudProviders.Oculi.Contracts.Workload
{
    public class OculiWorkloadListType
    {
        [JsonProperty("workloads")]
        public List<OculiWorkloadType> workloads { get; set; }
        [JsonProperty("paging")]
        public OculiPagingType paging { get; set; }
    }
}
