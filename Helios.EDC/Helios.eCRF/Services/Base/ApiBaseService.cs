using Helios.eCRF.Helpers;
using Helios.eCRF.Services.Interfaces;

namespace Helios.eCRF.Services.Base
{
    public class ApiBaseService : IApiBaseService
    {
        public RestSharpBaseClient RestClient { get; }
        public ApiBaseService(IConfiguration configuration, string serviceConfig )
        {
            RestClient = new RestSharpBaseClient(serviceHost: configuration[serviceConfig]);
        }

        public string ClientIp { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        public string HttpContextRequestPath { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        public Guid UserId { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        public Guid ResearchId { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        public Guid TenantId { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        public Guid ResearcherId { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
    }
}
