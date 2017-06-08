using RestSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace OculiService.CloudProviders.Oculi
{
    public class RestClientFactory
    {
        public static void Create(String _resource, Method _method, object _object, out RestClient _rest_client, out RestRequest _request)
        {
            ServicePointManager.Expect100Continue = false;
            ServicePointManager.UseNagleAlgorithm = false;
            ServicePointManager.DefaultConnectionLimit = 12 * Environment.ProcessorCount;
            ServicePointManager.ServerCertificateValidationCallback += (sender, certificate, chain, errors) => true;
            _rest_client = new RestClient();
            _rest_client.FollowRedirects = false;
            _rest_client.BaseUrl = new Uri("http://192.168.0.100:3000");
            _rest_client.Timeout = (int)TimeSpan.FromSeconds(240).TotalMilliseconds;

            _rest_client.RemoveDefaultParameter("Accept");
            _rest_client.AddDefaultParameter("Accept", "application/json", ParameterType.HttpHeader);
            _rest_client.Proxy = null;

            _request = new RestRequest();
            _request.Resource = _resource;
            _request.Method = _method;
            _request.RequestFormat = DataFormat.Json;
            _request.Timeout = (int)TimeSpan.FromSeconds(240).TotalMilliseconds;
            _request.JsonSerializer.ContentType = "application/json; charset=utf-8";
            _request.AddHeader("Accept-Encoding", "gzip");
            _request.JsonSerializer = new OculiApiJsonSerializer();
            if (_method == Method.GET)
            {
                _request.AddObject(_object);
            }
            else
            {
                _request.AddJsonBody(_object);
            }

        }
    }
}
