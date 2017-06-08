using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OculiService.CloudProviders.Oculi.Contracts
{
    public class OculiOrganizationType
    {
        [JsonProperty("id")]
        public string id { get; set; }
        [JsonProperty("organization")]
        public string organization { get; set; }
        [JsonProperty("users")]
        public OculiUserRootType[] users { get; set; }
    }

}
