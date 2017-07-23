using Newtonsoft.Json;
using OculiService.CloudProviders.Oculi.Contracts.Common.Contracts;
using System.Collections.Generic;

namespace OculiService.CloudProviders.Oculi.Contracts.Workload
{
    public class OculiTagListType : OculiListType
    {
        [JsonProperty("tags")]
        public List<OculiTagType> tags { get; set; }
    }
}
