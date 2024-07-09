using Helios.Common.DTO;
using Helios.Common.Model;
using Helios.Core.Services.Base;
using Helios.Core.Services.Interfaces;
using RestSharp;
using System.Text.Json;

namespace Helios.Core.Services
{
    public class StudyService : ApiBaseService, IStudyService
    {
        public StudyService(IConfiguration configuration, IHttpContextAccessor httpContextAccessor) : base(configuration, httpContextAccessor)
        {
        }
        public async Task<ApiResponse<dynamic>> SetSubjectDetailMenu(Int64 studyId, List<SubjectDetailMenuModel> detailMenuModels)
        {
            var model = new SubjectMenuModel()
            {
                StudyId = studyId,
                DetailMenuModels = detailMenuModels
            };

            using (var client = SharedServiceClient)
            {
                var req = new RestRequest("Cache/SetSubjectDetailMenu", Method.Post);
                req.AddJsonBody(model);

                var result = await client.ExecuteAsync<ApiResponse<dynamic>>(req);
                return result.Data;
            }

        }

        public async Task<List<SubjectDetailMenuModel>> GetSubjectDetailMenu(Int64 studyId)
        {
            using (var client = SharedServiceClient)
            {
                var req = new RestRequest("Cache/GetSubjectDetailMenu", Method.Get);
                req.AddParameter("studyId", studyId);
                var result = await client.ExecuteAsync<List<SubjectDetailMenuModel>>(req);
                return result.Data;
            }
        }

        public async Task<ApiResponse<dynamic>> RemoveSubjectDetailMenu(Int64 studyId)
        {
            var model = new SubjectMenuModel()
            {
                StudyId = studyId,
            };

            using (var client = SharedServiceClient)
            {
                var req = new RestRequest("Cache/RemoveSubjectDetailMenu", Method.Post);
                req.AddJsonBody(model);

                var result = await client.ExecuteAsync<ApiResponse<dynamic>>(req);
                return result.Data;
            }

        }

        public async Task<ApiResponse<dynamic>> SetUserPermissions(Int64 studyId, UserPermissionModel userPermissionModel)
        {
            var model = new UserPermissionCacheModel()
            {
                StudyId = studyId,
                UserPermissionModel = userPermissionModel
            };

            using (var client = SharedServiceClient)
            {
                var req = new RestRequest("Cache/SetUserPermissions", Method.Post);
                req.AddJsonBody(model);

                var result = await client.ExecuteAsync<ApiResponse<dynamic>>(req);
                return result.Data;
            }

        }
    }
}
