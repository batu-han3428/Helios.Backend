using Helios.eCRF.Services.Helper;
using Helios.eCRF.Services.Interfaces;
using RestSharp;

namespace Helios.eCRF.Services.Base
{
    public class ApiBaseService : IApiBaseService
    {
        protected readonly IConfiguration configuration;

        public ApiBaseService(IConfiguration configuration)
        {
            this.configuration = configuration;
        }

        protected RestClient AuthServiceClient {
            get { return new RestClient(new Uri("http://10.8.0.7:8080")); }
        }
        
        protected RestClient CoreServiceClient {
            get { return new RestClient(new Uri("http://10.8.0.8:3500/")); }
        }

        public string ClientIp { get; set; }
        public string HttpContextRequestPath { get; set; }
        public Int64 UserId { get; set; }
        public Int64 ResearchId { get; set; }
        public Int64 TenantId { get; set; }
        public Int64 ResearcherId { get; set; }
    }
}
