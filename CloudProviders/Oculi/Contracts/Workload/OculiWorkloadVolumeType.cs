using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OculiService.CloudProviders.Oculi.Contracts.Workload
{
    public class OculiWorkloadInterfaceType
    {
        [JsonProperty("id")]
        public string id { get; set; }
        [JsonProperty("workload_id")]
        public string workload_id { get; set; }
        [JsonProperty("platformnetwork_id")]
        public object platformnetwork_id { get; set; }
        [JsonProperty("deployed")]
        public bool deployed { get; set; }
        [JsonProperty("platform_moid")]
        public object platform_moid { get; set; }
        [JsonProperty("platform_index")]
        public object platform_index { get; set; }
        [JsonProperty("ip_addresses")]
        public string ip_addresses { get; set; }
        [JsonProperty("name")]
        public object name { get; set; }
        [JsonProperty("description")]
        public object description { get; set; }
        [JsonProperty("interface_index")]
        public int interface_index { get; set; }
        [JsonProperty("index")]
        public int index { get; set; }
        [JsonProperty("pnp_instance_id")]
        public object pnp_instance_id { get; set; }
        [JsonProperty("service_name")]
        public object service_name { get; set; }
        [JsonProperty("mac_address")]
        public object mac_address { get; set; }
        [JsonProperty("dns_servers")]
        public object dns_servers { get; set; }
        [JsonProperty("gateways")]
        public object gateways { get; set; }
        [JsonProperty("dns_domain")]
        public object dns_domain { get; set; }
        [JsonProperty("deleted")]
        public bool deleted { get; set; }
        [JsonProperty("created_at")]
        public DateTime created_at { get; set; }
        [JsonProperty("updated_at")]
        public DateTime updated_at { get; set; }
    }
}
