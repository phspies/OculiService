using RestSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OculiService.CloudProviders.Oculi.Interfaces
{
    public interface IOculiOAuth2Endpoint
    {
        IRestResponse Signin();
    }
}
