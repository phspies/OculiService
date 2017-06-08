using Newtonsoft.Json;

namespace OculiService.CloudProviders.Oculi.Contracts
{

    public class OculiOAuth2AuthResponseData
    {
        [JsonProperty("id")]
        public string id { get; set; }
        [JsonProperty("email")]
        public string email { get; set; }
        [JsonProperty("provider")]
        public string provider { get; set; }
        [JsonProperty("name")]
        public string name { get; set; }
        [JsonProperty("uid")]
        public string uid { get; set; }
        [JsonProperty("nickname")]
        public object nickname { get; set; }
        [JsonProperty("image")]
        public object image { get; set; }
    }



}
