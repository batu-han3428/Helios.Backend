using Helios.Common.DTO;
using Helios.Common.Model;
using Helios.Core.Services.Base;
using Helios.Core.Services.Interfaces;
using RestSharp;

namespace Helios.Core.Services
{
    public class UserService : ApiBaseService, IUserService
    {
        public UserService(IConfiguration configuration, IHttpContextAccessor httpContextAccessor) : base(configuration, httpContextAccessor)
        {
        }

        public async Task<RestResponse<List<AspNetUserDTO>>> GetUserList(List<Int64> AuthUserIds)
        {
            using (var client = AuthServiceClient)
            {
                string authUserIdsString = string.Join(",", AuthUserIds);
                var req = new RestRequest("AdminUser/GetUserList", Method.Get);
                req.AddParameter("AuthUserIds", authUserIdsString);
                var users = await client.ExecuteAsync<List<AspNetUserDTO>>(req);
                return users;
            }
        }
    }
}
