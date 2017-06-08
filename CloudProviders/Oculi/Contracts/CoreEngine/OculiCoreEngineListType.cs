using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OculiService.CloudProviders.Oculi.Contracts
{
    class OculiCoreEngineListType
    {
        [JsonProperty("coreengines")]
        public List<OculiCoreEngineType> coreengines { get; set; }
        [JsonProperty("paging")]
        public OculiPagingType paging { get; set; }
    }
}
