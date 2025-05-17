using Helios.Core.Services.Interfaces;
using RestSharp;

namespace Helios.Core.Services.Base
{
    public class ApiBaseService : IApiBaseService
    {
        protected readonly IConfiguration configuration;
        private readonly IHttpContextAccessor _httpContextAccessor;
        public ApiBaseService(IConfiguration configuration, IHttpContextAccessor httpContextAccessor)
        {
            this.configuration = configuration;
        }

        public Int64 UserId { get; set; }
        public Int64 StudyId { get; set; }
        public Int64 TenantId { get; set; }

        protected RestClient AuthServiceClient {
            get { return new RestClient(new Uri("http://authentication:8080")); }
        }
        
        protected RestClient CoreServiceClient {
            get { return new RestClient(new Uri("http://core:8080")); }
        }

        protected RestClient SharedServiceClient {
            get { return new RestClient(new Uri("http://shared:8080")); }
        }
    }
}
