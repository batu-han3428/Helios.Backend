using Helios.Authentication.Services.Interfaces;
using RestSharp;

namespace Helios.Authentication.Services
{
    public class ApiBaseService : IApiBaseService
    {
        protected readonly IConfiguration configuration;

        public ApiBaseService(IConfiguration configuration)
        {
            this.configuration = configuration;
        }

        protected RestClient AuthServiceClient
        {
            get { return new RestClient(new Uri("http://authentication:8080")); }
        }

        protected RestClient CoreServiceClient
        {
            get { return new RestClient(new Uri("http://core:8080")); }
        }

        public Int64 UserId { get; set; }
        public Int64 StudyId { get; set; }
        public Int64 TenantId { get; set; }
    }
}
