using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OculiService.CloudProviders.Oculi.Contracts.Platforms
{
    public class OculiDatamoverPolicyListType
    {
        [JsonProperty("datamoverpolicytypes")]
        public List<OculiDatamoverDeploymentPolicyType> datamoverdeploymenttypes { get; set; }
        [JsonProperty("paging")]
        public OculiPagingType paging { get; set; }
    }
}
