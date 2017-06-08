using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OculiService.CloudProviders.Oculi.Contracts.Workload
{
    public class OculiWorkloadDiskType
    {
        [JsonProperty("id")]
        public string id { get; set; }
        [JsonProperty("workload_id")]
        public string workload_id { get; set; }
        [JsonProperty("deployed")]
        public bool deployed { get; set; }
        [JsonProperty("platform_moid")]
        public object platform_moid { get; set; }
        [JsonProperty("platform_index")]
        public object platform_index { get; set; }
        [JsonProperty("index")]
        public int index { get; set; }
        [JsonProperty("scsi_target_id")]
        public int scsi_target_id { get; set; }
        [JsonProperty("pnp_device_id")]
        public object pnp_device_id { get; set; }
        [JsonProperty("signature")]
        public object signature { get; set; }
        [JsonProperty("deleted")]
        public bool deleted { get; set; }
        [JsonProperty("created_at")]
        public DateTime created_at { get; set; }
        [JsonProperty("updated_at")]
        public DateTime updated_at { get; set; }
    }
}
