using Newtonsoft.Json;
using OculiService.CloudProviders.Oculi.Contracts.Common.Contracts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OculiService.CloudProviders.Oculi.Contracts
{
    public class OculiPlatformType : OculiRecordType
    {
        [JsonProperty("id")]
        public string id { get; set; }
        [JsonProperty("platform")]
        public string platform { get; set; }
        [JsonProperty("organization_id")]
        public string organization_id { get; set; }
        [JsonProperty("credential_id")]
        public string credential_id { get; set; }
        [JsonProperty("coreengine_id")]
        public string coreengine_id { get; set; }
        [JsonProperty("enabled")]
        public bool enabled { get; set; }
        [JsonProperty("platformtype")]
        public int platformtype { get; set; }
        [JsonProperty("moid1")]
        public object moid1 { get; set; }
        [JsonProperty("moid2")]
        public object moid2 { get; set; }
        [JsonProperty("moid3")]
        public object moid3 { get; set; }
        [JsonProperty("deleted")]
        public bool deleted { get; set; }
        [JsonProperty("vmware_url")]
        public object vmware_url { get; set; }
        [JsonProperty("hyperv_url")]
        public object hyperv_url { get; set; }
        [JsonProperty("mcp_url")]
        public object mcp_url { get; set; }
        [JsonProperty("amazon_url")]
        public object amazon_url { get; set; }
        [JsonProperty("azure_url")]
        public object azure_url { get; set; }
        [JsonProperty("parent_platform_id")]
        public object parent_platform_id { get; set; }
        [JsonProperty("platformdatacenter_id")]
        public object platformdatacenter_id { get; set; }
        public List<OculiPlatformTemplateType> platformtemplates { get; set; }
    }
}
