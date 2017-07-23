using Newtonsoft.Json;
using OculiService.CloudProviders.Oculi.Contracts.Common.Contracts;
using System.Collections.Generic;

namespace OculiService.CloudProviders.Oculi.Contracts
{
    public class OculiPlatformListType : OculiListType
    {
        public OculiPlatformListType()
        {
            this.platforms = new List<OculiPlatformType>();
        }
        [JsonProperty("platforms")]
        public List<OculiPlatformType> platforms { get; set; }
    }
}
