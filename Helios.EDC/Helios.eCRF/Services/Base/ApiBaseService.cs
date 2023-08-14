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
            var aa = configuration["Services:Core"];
        }

        //public ApiBaseService(IConfiguration configuration, string serviceConfig )
        //{
        //    RestClient = new RestSharpBaseClient(serviceHost: configuration["Services:Core"]);
        //}
        protected RestClient AuthServiceClient {
            get { return new RestClient(new Uri("https://localhost:5201/")); }
            //get { return new RestClient(new Uri("https://localhost:4201/")); }
            //get { return new RestClient(configuration["Services:Core"]); }
        }

        public string ClientIp { get; set; }
        public string HttpContextRequestPath { get; set; }
        public Guid UserId { get; set; }
        public Guid ResearchId { get; set; }
        public Guid TenantId { get; set; }
        public Guid ResearcherId { get; set; }
    }
}
