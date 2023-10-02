using Helios.Common.DTO;
using Helios.Common.Model;
using Helios.eCRF.Models;
using Helios.eCRF.Services.Base;
using Helios.eCRF.Services.Interfaces;
using MassTransit.Internals.GraphValidation;
using Newtonsoft.Json;
using RestSharp;

namespace Helios.eCRF.Services
{
    public class StudyService : ApiBaseService, IStudyService
    {
        public StudyService(IConfiguration configuration) : base(configuration)
        {
        }

        public async Task<bool> AddModule(ModuleModel model)
        {
            using (var client = CoreServiceClient)
            {
                var req = new RestRequest("CoreStudy/AddModule", Method.Post);
                req.AddJsonBody(model);
                var result = await client.ExecuteAsync(req);
                return result.IsSuccessful;
            }
        }

        public async Task<bool> UpdateModule(ModuleModel model)
        {
            using (var client = CoreServiceClient)
            {
                var req = new RestRequest("CoreStudy/UpdateModule", Method.Post);
                req.AddJsonBody(model);
                var result = await client.ExecuteAsync(req);
                return result.IsSuccessful;
            }
        }
        
        public async Task<bool> DeleteModule(ModuleModel model)
        {
            using (var client = CoreServiceClient)
            {
                var req = new RestRequest("CoreStudy/DeleteModule", Method.Post);
                req.AddJsonBody(model);
                var result = await client.ExecuteAsync(req);
                return result.IsSuccessful;
            }
        }

        public async Task<ModuleModel> GetModule(Guid id)
        {
            var module = new ModuleModel();

            using (var client = CoreServiceClient)
            {
                var req = new RestRequest("CoreStudy/GetModule", Method.Get);
                req.AddParameter("id", id);
                var result = await client.ExecuteAsync(req);
                module = JsonConvert.DeserializeObject<ModuleModel>(result.Content);
            }

            return module;
        }

        public async Task<List<ModuleModel>> GetModuleList()
        {
            var moduleList = new List<ModuleModel>();

            using (var client = CoreServiceClient)
            {
                var req = new RestRequest("CoreStudy/GetModuleList", Method.Get);
                var result = await client.ExecuteAsync(req);
                moduleList = JsonConvert.DeserializeObject<List<ModuleModel>>(result.Content);
            }

            return moduleList;
        }

        public async Task<List<StudyDTO>> GetStudyList()
        {
            using (var client = CoreServiceClient)
            {
                var req = new RestRequest("CoreStudy/GetStudyList", Method.Get);
                var result = await client.ExecuteAsync<List<StudyDTO>>(req);
                return result.Data;
            }
        }

        public async Task<StudyDTO> GetStudy(Guid studyId)
        {
            using (var client = CoreServiceClient)
            {
                var req = new RestRequest("CoreStudy/GetStudy", Method.Get);
                req.AddParameter("studyId", studyId);
                var result = await client.ExecuteAsync<StudyDTO>(req);
                return result.Data;
            }
        }

        public async Task<ApiResponse<dynamic>> StudySave(StudyModel studyModel)
        {
            using (var client = CoreServiceClient)
            {
                var req = new RestRequest("CoreStudy/StudySave", Method.Post);
                req.AddJsonBody(studyModel);
                var result = await client.ExecuteAsync<ApiResponse<dynamic>>(req);
                return result.Data;
            }
        }
    }
}
