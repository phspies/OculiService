using Newtonsoft.Json;
using OculiService.CloudProviders.Oculi.Contracts.Common.Contracts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OculiService.CloudProviders.Oculi.Contracts
{
    public class OculiCredentialListType : OculiListType
    {
        [JsonProperty("platforms")]
        public List<OculiCredentialType> credentials { get; set; }
    }
}
