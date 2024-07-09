using Helios.Common.DTO;
using Helios.Common.Model;
using Helios.Core.Services.Base;
using Helios.Core.Services.Interfaces;
using RestSharp;
using System.Text.Json;

namespace Helios.Core.Services
{
    public class UserService : ApiBaseService, IUserService
    {
        public UserService(IConfiguration configuration, IHttpContextAccessor httpContextAccessor) : base(configuration, httpContextAccessor)
        {
        }

        public async Task<ApiResponse<dynamic>> RemoveUserPermissions(Int64 studyId)
        {
            var model = new SubjectMenuModel()
            {
                StudyId = studyId,
            };

            using (var client = SharedServiceClient)
            {
                var req = new RestRequest("Cache/RemoveUserPermissions", Method.Post);
                req.AddJsonBody(model);

                var result = await client.ExecuteAsync<ApiResponse<dynamic>>(req);
                return result.Data;
            }

        }
    }
}
