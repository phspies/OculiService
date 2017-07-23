using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OculiService.CloudProviders.Oculi.Contracts.Common
{
    public interface IOculiTimestampType
    {
        DateTime created_at { get; set; }
        DateTime updated_at { get; set; }
    }
}
