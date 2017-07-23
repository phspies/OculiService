using Newtonsoft.Json;
using OculiService.CloudProviders.Oculi.Contracts.Common.Contracts;

namespace OculiService.CloudProviders.Oculi.Contracts.Common
{
    public interface IOculiPagingType
    {
        OculiPagingDetailType paging { get; set; }
    }
}
