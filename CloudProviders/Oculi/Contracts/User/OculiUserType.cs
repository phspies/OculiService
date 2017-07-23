using Newtonsoft.Json;
using OculiService.CloudProviders.Oculi.Contracts.Common;
using OculiService.CloudProviders.Oculi.Contracts.Common.Contracts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OculiService.CloudProviders.Oculi.Contracts
{
    public class OculiUserType : OculiRecordType
    {
        [JsonProperty("id")]
        public string id { get; set; }
        [JsonProperty("email")]
        public string email { get; set; }
        [JsonProperty("name")]
        public string name { get; set; }
        [JsonProperty("sign_in_count")]
        public int sign_in_count { get; set; }
        [JsonProperty("last_sign_in_at")]
        public DateTime last_sign_in_at { get; set; }
    }

}
