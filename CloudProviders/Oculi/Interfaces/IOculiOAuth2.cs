using OculiService.CloudProviders.Oculi.Contracts;
using RestSharp;

namespace OculiService.CloudProviders.Oculi.Interfaces
{
    public interface IOculiOAuth2
    {
        void SignIn(RestRequest _request);
    }
}
