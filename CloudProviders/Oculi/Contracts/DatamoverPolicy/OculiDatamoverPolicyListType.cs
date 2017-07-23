using Newtonsoft.Json;
using OculiService.CloudProviders.Oculi.Contracts.Common.Contracts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OculiService.CloudProviders.Oculi.Contracts.Platforms
{
    public class OculiDatamoverPolicyListType : OculiListType
    {
        [JsonProperty("datamoverpolicytypes")]
        public List<OculiDatamoverDeploymentPolicyType> datamoverdeploymenttypes { get; set; }
    }
}
