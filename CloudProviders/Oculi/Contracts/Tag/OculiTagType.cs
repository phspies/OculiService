using Newtonsoft.Json;
using OculiService.CloudProviders.Oculi.Contracts.Common.Contracts;
using System.ComponentModel.DataAnnotations.Schema;

namespace OculiService.CloudProviders.Oculi.Contracts.Workload
{
    [Table("OculiTags")]
    public class OculiTagType : OculiRecordType
    {
        [JsonProperty("id")]
        public object id { get; set; }
        [JsonProperty("organization_id")]
        public object organization_id { get; set; }
        [JsonProperty("tagkey")]
        public object tagkey { get; set; }
        [JsonProperty("tagvalue")]
        public object tagvalue { get; set; }
        [JsonProperty("displayreport")]
        public bool displayreport { get; set; }
        [JsonProperty("valuerequired")]
        public bool valuerequired { get; set; }
        [JsonProperty("deleted")]
        public bool deleted { get; set; }
    }
}
