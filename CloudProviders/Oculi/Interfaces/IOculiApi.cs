using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace OculiService.CloudProviders.Oculi.Interfaces
{
    public interface IOculiApi
    {
        IOculiOAuth2Endpoint Authorization { get; }
        IOculiOrganization Organization { get; }
    }
}
