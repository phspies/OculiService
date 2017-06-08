using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OculiService.CloudProviders.Oculi.Contracts.Workload
{
    public class OculiWorkloadPackageType
    {
        [JsonProperty("id")]
        public string id { get; set; }
        [JsonProperty("workload_id")]
        public string workload_id { get; set; }
        [JsonProperty("name")]
        public string name { get; set; }
        [JsonProperty("description")]
        public object description { get; set; }
        [JsonProperty("installlocation")]
        public object installlocation { get; set; }
        [JsonProperty("installstate")]
        public object installstate { get; set; }
        [JsonProperty("vendor")]
        public object vendor { get; set; }
        [JsonProperty("version")]
        public object version { get; set; }
        [JsonProperty("deleted")]
        public bool deleted { get; set; }
        [JsonProperty("created_at")]
        public DateTime created_at { get; set; }
        [JsonProperty("updated_at")]
        public DateTime updated_at { get; set; }
    }
}
