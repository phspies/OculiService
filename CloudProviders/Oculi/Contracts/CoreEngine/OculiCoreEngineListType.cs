using Newtonsoft.Json;
using OculiService.CloudProviders.Oculi.Contracts.Common;
using OculiService.CloudProviders.Oculi.Contracts.Common.Contracts;
using System.Collections.Generic;

namespace OculiService.CloudProviders.Oculi.Contracts
{
    class OculiCoreEngineListType : OculiListType
    {
        [JsonProperty("coreengines")]
        public List<OculiCoreEngineType> coreengines { get; set; }
    }
}
