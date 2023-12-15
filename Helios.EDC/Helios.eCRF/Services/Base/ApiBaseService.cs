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
            get { return new RestClient(new Uri("https://localhost:5200/")); }
        }
        
        protected RestClient CoreServiceClient {
            get { return new RestClient(new Uri("https://localhost:4200/")); }
        }

        public string ClientIp { get; set; }
        public string HttpContextRequestPath { get; set; }
        public Int64 UserId { get; set; }
        public Int64 ResearchId { get; set; }
        public Int64 TenantId { get; set; }
        public Int64 ResearcherId { get; set; }
    }
}
