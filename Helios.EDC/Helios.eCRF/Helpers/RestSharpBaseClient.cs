using RestSharp;
using RestSharp.Serializers;
using System.Diagnostics;

namespace Helios.eCRF.Helpers
{
    public class RestSharpBaseClient : RestClient
    {
        private readonly string serviceUrl;
        public RestSharpBaseClient(string baseUrl)
        {
            serviceUrl = baseUrl;
        }
        public async Task<RestResponse> ExecuteAsync(RestRequest request)
        {
            var response = await base.ExecuteAsync(request);
            //TimeoutCheck(request, response);
            return response;
        }
    }
}
