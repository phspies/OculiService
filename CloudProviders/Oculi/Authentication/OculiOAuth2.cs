using OculiService.Common.Logging;
using OculiService.CloudProviders.Oculi.Contracts;
using OculiService.CloudProviders.Oculi.Interfaces;
using System.Net;
using OculiService.Common;
using OculiService.Common.Interfaces;
using System.Collections.Generic;
using System.Linq;
using System;
using System.Threading;
using RestSharp;
using OculiService.Common.Encryption;
using OculiService.CloudProviders.Oculi.Classes;

namespace OculiService.CloudProviders.Oculi
{
    public class OculiOAuth2 : OculiApiCore, IOculiOAuth2
    {
        ILogger _logger;
        OculiApi _oculi_api;
        public OculiOAuth2(OculiApi oculi_api, ILogger logger) : base(oculi_api, logger)
        {
            _logger = logger;
            _oculi_api = oculi_api;
        }

        public void SignIn(RestRequest _request)
        {
            string _coreengine_id, _organization_id, _access_key, _secret_key;
            while (true)
            {
                TripleDESOAuth2TokenCryptor _token_cryptor = new TripleDESOAuth2TokenCryptor();
                ICoreEngineSettings _core_settings = new CoreEngineSettings();
                _coreengine_id = _core_settings.CoreEngineID;
                _organization_id = _core_settings.OrganizationID;
                _access_key = _core_settings.AccessKey;
                _secret_key = _core_settings.SecretKey;

                List<String> _auth_variables = new List<String>() { "CoreEngineID", "OrganizationID", "AccessKey", "SecretKey" };
                var authorName = AttributeHelpers.GetPropValue(_core_settings, "CoreEngineID");
                if (_auth_variables.Any(x => AttributeHelpers.IsPropValueEmpty(_core_settings, x)))
                {
                    _logger.Error(string.Format("Missing authentication attributes {0}", string.Join(", ", _auth_variables.Where(x => AttributeHelpers.IsPropValueEmpty(_core_settings, x)))));
                    Thread.Sleep(TimeSpan.FromSeconds(10));
                }
                else
                {

                    break;
                }
            }
            IRestResponse _auth_response = _oculi_api.Authorization.Signin();
            OculiTokenFactory.StoreToken(OculiTokenFactory.SetToken(_auth_response));

        }
    }
}
