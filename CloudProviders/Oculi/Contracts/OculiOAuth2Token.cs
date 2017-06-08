using Newtonsoft.Json;

namespace OculiService.CloudProviders.Oculi.Contracts
{ 
    public class OculiOAuth2Token
    {
        [JsonProperty("uid")]
        public string uid { get; set; }
        [JsonProperty("access-token")]
        public string accesstoken { get; set; }
        [JsonProperty("expiry")]
        public long expiry { get; set; }
        [JsonProperty("coreengine_id")]
        public string coreengine_id { get; set; }

    }
}
