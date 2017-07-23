using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OculiService.CloudProviders.Oculi.Contracts.Common.Contracts
{
    public class OculiListType : IOculiListType
    {
        public OculiPagingType paging { get; set; }
    }
}
