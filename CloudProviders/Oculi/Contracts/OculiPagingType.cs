using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OculiService.CloudProviders.Oculi.Contracts
{
    public class OculiPagingType
    {
        [JsonProperty("total_entries")]
        public int total_entries { get; set; }
        [JsonProperty("page_size")]
        public int page_size { get; set; }
        [JsonProperty("total_pages")]
        public int total_pages { get; set; }
        [JsonProperty("current_page")]
        public int current_page { get; set; }
        [JsonProperty("next_page")]
        public object next_page { get; set; }
    }
}
