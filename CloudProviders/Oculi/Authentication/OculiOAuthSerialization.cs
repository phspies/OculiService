using Newtonsoft.Json;
using OculiService.CloudProviders.Oculi.Contracts;
using RestSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace OculiService.CloudProviders.Oculi.Classes
{
    static class OculiOAuthSerialization
    {
        static public Tuple<bool, OculiOAuth2Token, object> Deserialize(IRestResponse _response)
        {
            try
            {
                object _response_object = JsonConvert.DeserializeObject<OculiOAuth2AuthResponse>(_response.Content, new JsonSerializerSettings
                {
                    MissingMemberHandling = MissingMemberHandling.Ignore,
                    NullValueHandling = NullValueHandling.Ignore,
                    Error = new OculiException().DeserializationExpection
                });
                if (_response.StatusCode == HttpStatusCode.OK)
                {
                    OculiOAuth2Token _token = OculiTokenFactory.SetToken(_response);
                    return Tuple.Create(true, new OculiOAuth2Token(), _response_object);
                }
                else if (_response.StatusCode == HttpStatusCode.Unauthorized)
                {
                    OculiOAuth2Token _token = OculiTokenFactory.SetToken(_response);
                    return Tuple.Create(false, _token, _response_object);
                }
                else
                {
                    throw new Exception("Platform credentials incorrect");
                }

            }
            catch (Exception ex)
            {
                throw new Exception(String.Format("Cannot deserialize object : {0}", ex.GetBaseException().Message));
            }

        }

    }
}
