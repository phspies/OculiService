using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OculiService.CloudProviders.Oculi.Contracts
{
    public class OculiOAuth2AuthResponse
    {
        [JsonProperty("success")]
        public bool success { get; set; }
        [JsonProperty("errors")]
        public string[] errors { get; set; }
        [JsonProperty("data")]
        public OculiOAuth2AuthResponseData data { get; set; }
    }
}
