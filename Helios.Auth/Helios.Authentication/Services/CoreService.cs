using Helios.Authentication.Services.Interfaces;
using RestSharp;

namespace Helios.Authentication.Services
{
    public class CoreService: ApiBaseService, ICoreService
    {
        public CoreService(IConfiguration configuration) : base(configuration)
        {
        }

        public async Task<List<Int64>> GetUserStudyIds(Int64 userId)
        {
            using (var client = CoreServiceClient)
            {
                var req = new RestRequest("CoreUser/GetUserStudyIds", Method.Get);
                req.AddParameter("userId", userId);
                var result = await client.ExecuteAsync<List<Int64>>(req);
                return result.Data;
            }
        }
    }
}
