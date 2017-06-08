using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OculiService.CloudProviders.Oculi.Contracts
{
    class OculiCoreEngineType
    {
        [JsonProperty("id")]
        public string id { get; set; }
        [JsonProperty("description")]
        public string description { get; set; }
        [JsonProperty("hostname")]
        public object hostname { get; set; }
        [JsonProperty("version")]
        public object version { get; set; }
        [JsonProperty("ipaddress")]
        public object ipaddress { get; set; }
        [JsonProperty("enabled")]
        public bool enabled { get; set; }
        [JsonProperty("last_contact")]
        public object last_contact { get; set; }
        [JsonProperty("created_at")]
        public DateTime created_at { get; set; }
        [JsonProperty("updated_at")]
        public DateTime updated_at { get; set; }
        [JsonProperty("organization_id")]
        public string organization_id { get; set; }
        [JsonProperty("configured")]
        public bool configured { get; set; }
    }
}
