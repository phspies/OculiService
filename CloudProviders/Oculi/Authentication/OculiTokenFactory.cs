using OculiService.CloudProviders.Oculi.Contracts;
using OculiService.CloudProviders.Oculi.Interfaces;
using OculiService.Common;
using OculiService.Common.Collections.Generic;
using OculiService.Common.Encryption;
using OculiService.Common.Interfaces;
using OculiService.Common.Logging;
using RestSharp;
using System;
using System.Collections.Generic;
using System.Linq;

namespace OculiService.CloudProviders.Oculi.Classes
{
    class OculiTokenFactory : OculiApiCore
    {
        public OculiTokenFactory(OculiApi oculi_api, ILogger logger) : base(oculi_api, logger)
        {

        }
        static public bool IsTokenConfigured()
        {
            ICoreEngineSettings _coreengine_settings = new CoreEngineSettings();
            return (!String.IsNullOrWhiteSpace(_coreengine_settings.TokenAccessCode)) ? true : false;

        }
        static public RestRequest SetTokenRequest(RestRequest _request)
        {
            ICoreEngineSettings _coreengine_settings = new CoreEngineSettings();
            OculiOAuth2Token _decrypted_token = new OculiOAuth2Token();
            _decrypted_token = RetrieveToken();
            Dictionary<string, string> _tokens = _request.Parameters.ToDictionary(x => x.Name, x => x.Value.ToString());
            Util.AddOrReplace(_tokens, "username", _coreengine_settings.AccessKey);
            Util.AddOrReplace(_tokens, "access_token", _decrypted_token.accesstoken);
            Util.AddOrReplace(_tokens, "coreengine_id", _coreengine_settings.CoreEngineID);
            _tokens.ForEach(x =>{
                if (_request.Parameters.Any(z => z.Name == x.Key))
                {
                    _request.Parameters.FirstOrDefault(z => z.Name == x.Key).Value = x.Value;
                }
                else
                {
                    _request.AddHeader(x.Key, x.Value);
                }
            });
            return _request;
        }
        static public void StoreToken(OculiOAuth2Token _token)
        {
            TripleDESOAuth2TokenCryptor _token_cryptor = new TripleDESOAuth2TokenCryptor();
            ICoreEngineSettings _coreengine_settings = new CoreEngineSettings();
            OculiOAuth2Token _encrypted_token = _token_cryptor.Encrypt(_token);
            _coreengine_settings.TokenAccessCode = _encrypted_token.accesstoken;
            _coreengine_settings.TokenExpiry = _token.expiry;
        }
        static public OculiOAuth2Token RetrieveToken()
        {
            ICoreEngineSettings _coreengine_settings = new CoreEngineSettings();

            OculiOAuth2Token _encrypted_token = new OculiOAuth2Token();
            _encrypted_token.accesstoken = _coreengine_settings.TokenAccessCode;
            _encrypted_token.expiry = _coreengine_settings.TokenExpiry;
            TripleDESOAuth2TokenCryptor _token_cryptor = new TripleDESOAuth2TokenCryptor();
            OculiOAuth2Token _decrypted_token = _token_cryptor.Decrypt(_encrypted_token);
            return _decrypted_token;
        }
        static public RestRequest SetCredentialRequest(ILogger _logger, OculiApi _api, RestRequest _request)
        {
            ICoreEngineSettings _coreengine_settings = new CoreEngineSettings();
            _request.AddHeader("username", _coreengine_settings.AccessKey);
            _request.AddHeader("password", _coreengine_settings.SecretKey);
            return _request;
        }

        static public OculiOAuth2Token SetToken(IRestResponse _response)
        {
            OculiOAuth2Token _token = new OculiOAuth2Token();
            _token.accesstoken = _response.Headers.FirstOrDefault(x => x.Name == "access_token")?.Value?.ToString();
            _token.expiry = Convert.ToInt64(_response.Headers.FirstOrDefault(x => x.Name == "expiry")?.Value?.ToString());
            return _token;
        }

    }
}
