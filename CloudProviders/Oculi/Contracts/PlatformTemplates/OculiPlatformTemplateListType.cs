using Newtonsoft.Json;
using OculiService.CloudProviders.Oculi.Contracts.Common.Contracts;
using System.Collections.Generic;

namespace OculiService.CloudProviders.Oculi.Contracts
{
    public class OculiPlatformTemplateListType : OculiListType
    {
        public OculiPlatformTemplateListType()
        {
            platformtemplates = new List<OculiPlatformTemplateType>();
        }
        [JsonProperty("platformtemplates")]
        public List<OculiPlatformTemplateType> platformtemplates { get; set; }
    }
}
