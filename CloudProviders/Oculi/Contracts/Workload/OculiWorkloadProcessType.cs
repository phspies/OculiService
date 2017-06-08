using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OculiService.CloudProviders.Oculi.Contracts.Workload
{
    public class OculiWorkloadProcessType
    {
        [JsonProperty("id")]
        public string id { get; set; }
        [JsonProperty("workload_id")]
        public string workload_id { get; set; }
        [JsonProperty("name")]
        public string name { get; set; }
        [JsonProperty("commandline")]
        public object commandline { get; set; }
        [JsonProperty("process_id")]
        public object process_id { get; set; }
        [JsonProperty("virtual_size")]
        public object virtual_size { get; set; }
        [JsonProperty("deleted")]
        public bool deleted { get; set; }
        [JsonProperty("created_at")]
        public DateTime created_at { get; set; }
        [JsonProperty("updated_at")]
        public DateTime updated_at { get; set; }
    }
}
