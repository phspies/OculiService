using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OculiService.CloudProviders.Oculi.Contracts.Workload
{
    public class OculiWorkloadVolumeType
    {
        [JsonProperty("id")]
        public string id { get; set; }
        [JsonProperty("workload_id")]
        public string workload_id { get; set; }
        [JsonProperty("deployed")]
        public bool deployed { get; set; }
        [JsonProperty("mountpoint")]
        public string mountpoint { get; set; }
        [JsonProperty("filesystem_type")]
        public string filesystem_type { get; set; }
        [JsonProperty("name")]
        public string name { get; set; }
        [JsonProperty("disk_index")]
        public int disk_index { get; set; }
        [JsonProperty("deleted")]
        public bool deleted { get; set; }
        [JsonProperty("created_at")]
        public DateTime created_at { get; set; }
        [JsonProperty("updated_at")]
        public DateTime updated_at { get; set; }
    }
}
