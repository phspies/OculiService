using RestSharp;
using System;
using System.Net;
using System.Threading;
using Newtonsoft.Json;
using OculiService.CloudProviders.Oculi.Interfaces;
using OculiService.Common.Logging;
using OculiService.CloudProviders.Oculi.Contracts;
using OculiService.CloudProviders.Oculi.Classes;
using OculiService.Common.Interfaces;
using OculiService.Common;

namespace OculiService.CloudProviders.Oculi
{
    public class OculiApiCore : IOculiApiCore
    {
        public String _resource;
        private OculiApi _oculi_api;
        private ILogger _logger;

        public string api_prefix = "/api/v01";
        public ICoreEngineSettings _coreengine_settings = new CoreEngineSettings();
        public string _coreengine_id;
        public OculiOAuth2Token _auth2_token;

        public OculiApiCore(OculiApi oculi_api, ILogger logger)
        {
            _oculi_api = oculi_api;
            _logger = logger;
            _coreengine_id = _coreengine_settings.OrganizationID;
        }
        public T PostOperation<T>(object _object) where T : new()
        {
            return (T)PerformApiCall<T>(Method.POST, _object);
        }
        public T PutOperation<T>(object _object) where T : new()
        {
            return (T)PerformApiCall<T>(Method.PUT, _object);
        }
        public T GetOperation<T>(object _object) where T : new()
        {
            return (T)PerformApiCall<T>(Method.GET, _object);
        }
        public T DeleteOperation<T>(object _object) where T : new()
        {
            return (T)PerformApiCall<T>(Method.DELETE, _object);
        }
        public T PostOperationSimple<T>(object _object, String _authentication) where T : new()
        {
            return (T)PerformSimpleApiCall<T>(Method.POST, _object, _authentication);
        }
        public T PutOperationSimple<T>(object _object, String _authentication) where T : new()
        {
            return (T)PerformSimpleApiCall<T>(Method.PUT, _object, _authentication);
        }
        public T GetOperationSimple<T>(object _object, String _authentication) where T : new()
        {
            return (T)PerformSimpleApiCall<T>(Method.GET, _object, _authentication);
        }
        public IRestResponse PerformSimpleApiCall<T>(Method _method, Object _object, String _authentication) where T : new()
        {
            RestClient _rest_client = new RestClient();
            RestRequest _request = new RestRequest();
            RestClientFactory.Create(Resource, _method, _object, out _rest_client, out _request);
            switch (_authentication)
            {
                case "creds":
                    _request = OculiTokenFactory.SetCredentialRequest(_logger, _oculi_api, _request);
                    break;
                case "token":
                    _request = OculiTokenFactory.SetTokenRequest(_request);
                    break;
                default:
                    throw new Exception("No authentication method specified");
            }
            return _rest_client.Execute(_request);

        }

        public object PerformApiCall<T>(Method _method, Object _object) where T : new()
        {

            RestClient _rest_client = new RestClient();
            RestRequest _request = new RestRequest();
            RestClientFactory.Create(Resource, _method, _object, out _rest_client, out _request);
            if (!OculiTokenFactory.IsTokenConfigured())
            {
                IOculiOAuth2 _signin = new OculiOAuth2(_oculi_api, _logger);
                _signin.SignIn(_request);
            }
            object responseobject = null;
            while (true)
            {
                _request = OculiTokenFactory.SetTokenRequest(_request);
                var restResponse = _rest_client.Execute(_request);
                if (typeof(T) == typeof(RestResponse))
                {
                    return restResponse;
                }
                if (restResponse.StatusCode == HttpStatusCode.OK)
                {
                    try
                    {
                        responseobject = JsonConvert.DeserializeObject<T>(restResponse.Content, new JsonSerializerSettings
                        {
                            MissingMemberHandling = MissingMemberHandling.Ignore,
                            NullValueHandling = NullValueHandling.Ignore,
                            Error = new OculiException().DeserializationExpection
                        });
                    }
                    catch (Exception ex)
                    {
                        //Logger.log(ex.ToString(), Logger.Severity.Error);
                        //Logger.log(JsonConvert.SerializeObject(_object), Logger.Severity.Error);
                        //Logger.log(restResponse.Content, Logger.Severity.Error);
                        throw new Exception(ex.GetBaseException().Message);
                    }
                    break;
                }
                else if (restResponse.StatusCode == HttpStatusCode.Unauthorized)
                {
                    IOculiOAuth2 _signin = new OculiOAuth2(_oculi_api, _logger);
                    _signin.SignIn(_request);
                    continue;
                }
                else if (restResponse.StatusCode == HttpStatusCode.BadRequest)
                {
                    //ResultType _result = new ResultType();
                    try
                    {
                        //_result = JsonConvert.DeserializeObject<ResultType>(restResponse.Content);
                    }
                    catch (Exception ex)
                    {
                        //Logger.log(ex.ToString(), Logger.Severity.Error);
                        //Logger.log(restResponse.Content, Logger.Severity.Error);
                        //throw new Exception(_result.result.message.ToString());
                    }
                }
                else if (restResponse.StatusCode == HttpStatusCode.NotFound)
                {
                    throw new Exception(String.Format("{0} {1}", restResponse.StatusCode, restResponse.ErrorMessage));
                }
                else if (restResponse.StatusCode == HttpStatusCode.RequestTimeout)
                {
                    //Logger.log(String.Format("Connection timeout to {0}", client.BuildUri(request).ToString()), Logger.Severity.Error);
                    Thread.Sleep(new TimeSpan(0, 0, 30));
                }
                else if (restResponse.StatusCode == 0)
                {
                    //Logger.log(String.Format("Unexpected error connecting to {0} with error ({1})", client.BuildUri(request).ToString(), restResponse.ErrorMessage), Logger.Severity.Error);

                    Thread.Sleep(new TimeSpan(0, 0, 30));
                }
                else
                {
                    //Logger.log(String.Format("Unexpected API error on {0} with error ({1})", client.BuildUri(request).ToString(), restResponse.ErrorMessage), Logger.Severity.Error);
                    throw new Exception(String.Format("{0} {1}", restResponse.StatusCode, restResponse.ErrorMessage));
                }
            }
            return responseobject;

        }


        public String Resource
        {
            get
            {
                return _resource;
            }
            set
            {
                _resource = value;
            }
        }
    }
}
