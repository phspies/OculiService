using Newtonsoft.Json;
using OculiService.CloudProviders.Oculi.Contracts.Common.Contracts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OculiService.CloudProviders.Oculi.Contracts
{
    public class OculiPlatformTemplateType : OculiRecordType
    {
        [JsonProperty("id")]
        public string id { get; set; }
        [JsonProperty("platform_id")]
        public string platform_id { get; set; }
        [JsonProperty("moid")]
        public string moid { get; set; }
        [JsonProperty("template")]
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
    }
}
