using OculiService.Common.Logging;
using OculiService.CloudProviders.Oculi.Contracts;
using OculiService.CloudProviders.Oculi.Interfaces;
using System.Net;
using RestSharp;

namespace OculiService.CloudProviders.Oculi
{
    public class OculiOAuth2Endpoint : OculiApiCore, IOculiOAuth2Endpoint
    {
        public OculiOAuth2Endpoint(OculiApi oculi_api, ILogger logger) : base(oculi_api, logger)
        {
        }
        public IRestResponse Signin()
        {
            Resource = string.Format("/api/sign_in", _coreengine_id);
            IRestResponse _auth_response = PostOperationSimple<RestResponse>(new object(), "creds");
            return _auth_response;
        }
    }
}
