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

        public async Task<bool> StudyUserActiveControl(Int64 authUserId)
        {
            using (var client = CoreServiceClient)
            {
                var req = new RestRequest("CoreUser/StudyUserActiveControl", Method.Get);
                req.AddParameter("authUserId", authUserId);
                var result = await client.ExecuteAsync<bool>(req);
                return result.Data;
            }
        }
    }
}
