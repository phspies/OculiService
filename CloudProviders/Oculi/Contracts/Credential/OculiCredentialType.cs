using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OculiService.CloudProviders.Oculi.Contracts
{
    public class OculiCredentialType
    {
        [JsonProperty("id")]
        public string id { get; set; }
        [JsonProperty("description")]
        public string description { get; set; }
        [JsonProperty("organization_id")]
        public string organization_id { get; set; }
        [JsonProperty("username")]
        public string username { get; set; }
        [JsonProperty("password")]
        public string password { get; set; }
        [JsonProperty("domain")]
        public object domain { get; set; }
        [JsonProperty("enabled")]
        public bool enabled { get; set; }
        [JsonProperty("credential_type")]
        public int credential_type { get; set; }
        [JsonProperty("ostype")]
        public string ostype { get; set; }
        [JsonProperty("seckey")]
        public string seckey { get; set; }
        [JsonProperty("created_at")]
        public DateTime created_at { get; set; }
        [JsonProperty("updated_at")]
        public DateTime updated_at { get; set; }
    }

}
