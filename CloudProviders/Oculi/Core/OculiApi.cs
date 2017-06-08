using OculiService.CloudProviders.Oculi.Contracts;
using OculiService.CloudProviders.Oculi.Interfaces;
using OculiService.Common.Logging;
using System.Net;

namespace OculiService.CloudProviders.Oculi
{
    public class OculiApi : IOculiApi
    {
        public ILogger _logger;
        public OculiOAuth2Token _token = new OculiOAuth2Token();
        public OculiOAuth2 _oculiOauth;
        public OculiApi(ILogger logger)
        {
            _logger = logger;
        }
        public IOculiOAuth2Endpoint Authorization
        {
            get
            {
                return new OculiOAuth2Endpoint(this, _logger);
            }
        }
        public IOculiOrganization Organization
        {
            get
            {
                return new OculiOrganization(this, _logger);
            }
        }


    }
}
