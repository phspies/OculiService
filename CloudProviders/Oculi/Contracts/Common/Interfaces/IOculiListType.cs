using System.Collections.Generic;

namespace OculiService.CloudProviders.Oculi.Contracts.Common
{
    public interface IOculiListType
    {
        OculiPagingType paging { get; set; }
    }
}
