using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OculiService.CloudProviders.Oculi.Contracts
{
    public class OculiPlatformTemplateType
    {
        [JsonProperty("id")]
        public string id { get; set; }
        [JsonProperty("platform_id")]
        public string platform_id { get; set; }
        [JsonProperty("moid")]
        public string moid { get; set; }
        [JsonProperty("id")]
        public string template { get; set; }
        [JsonProperty("description")]
        public string description { get; set; }
        [JsonProperty("type")]
        public string type { get; set; }
        [JsonProperty("osid")]
        public string osid { get; set; }
        [JsonProperty("ostype")]
        public string ostype { get; set; }
        [JsonProperty("osdisplayname")]
        public string osdisplayname { get; set; }
        [JsonProperty("deleted")]
        public bool deleted { get; set; }
        [JsonProperty("enabled")]
        public bool enabled { get; set; }
        [JsonProperty("created_at")]
        public DateTime created_at { get; set; }
        [JsonProperty("updated_at")]
        public DateTime updated_at { get; set; }
    }
}
