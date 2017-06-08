using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OculiService.CloudProviders.Oculi.Contracts
{
    public class OculiCredentialListType
    {
        [JsonProperty("platforms")]
        public List<OculiCredentialType> credentials { get; set; }
        [JsonProperty("paging")]
        public OculiPagingType paging { get; set; }
    }
}
