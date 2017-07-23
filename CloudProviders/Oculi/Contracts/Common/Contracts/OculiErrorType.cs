using Newtonsoft.Json;
using System.Collections.Generic;

namespace OculiService.CloudProviders.Oculi.Contracts.Common
{
    public class OculiErrorType : IOculiErrorType
    {
        [JsonProperty("errors")]
        public List<string> errors { get; set; }
    }
}
