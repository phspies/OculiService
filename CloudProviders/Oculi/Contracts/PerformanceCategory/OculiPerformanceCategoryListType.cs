using Newtonsoft.Json;
using OculiService.CloudProviders.Oculi.Contracts.Common.Contracts;
using System.Collections.Generic;

namespace OculiService.CloudProviders.Oculi.Contracts
{
    public class OculiPerformanceCategoryListType : OculiListType
    {
        public OculiPerformanceCategoryListType()
        {
            performancecategories = new List<OculiPerformanceCategoryType>();
        }
        [JsonProperty("performancecategories")]
        public List<OculiPerformanceCategoryType> performancecategories { get; set; }
    }
}
