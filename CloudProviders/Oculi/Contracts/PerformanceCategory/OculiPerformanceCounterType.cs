using Newtonsoft.Json;
using OculiService.CloudProviders.Oculi.Contracts.Common.Contracts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OculiService.CloudProviders.Oculi.Contracts
{
    public class OculiPerformanceCounterType : OculiRecordType
    {
        [JsonProperty("id")]
        public object id { get; set; }
        [JsonProperty("performancecategory_id")]
        public object performancecategory_id { get; set; }
        [JsonProperty("timestamp")]
        public object timestamp { get; set; }
        [JsonProperty("value")]
        public float value { get; set; }
    }
}
