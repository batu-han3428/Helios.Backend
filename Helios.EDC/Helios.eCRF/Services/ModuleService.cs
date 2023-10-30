using Helios.Common.DTO;
using Helios.Common.Model;
using Helios.Core.Domains.Entities;
using Helios.eCRF.Models;
using Helios.eCRF.Services.Base;
using Helios.eCRF.Services.Interfaces;
using MassTransit.Internals.GraphValidation;
using Newtonsoft.Json;
using RestSharp;

namespace Helios.eCRF.Services
{
    public class ModuleService : ApiBaseService, IModuleService
    {
        public ModuleService(IConfiguration configuration) : base(configuration)
        {
        }

        public async Task<bool> AddModule(ModuleModel model)
        {
            using (var client = CoreServiceClient)
            {
                var req = new RestRequest("CoreModule/AddModule", Method.Post);
                req.AddJsonBody(model);
                var result = await client.ExecuteAsync(req);
                return result.IsSuccessful;
            }
        }

        public async Task<bool> UpdateModule(ModuleModel model)
        {
            using (var client = CoreServiceClient)
            {
                var req = new RestRequest("CoreModule/UpdateModule", Method.Post);
                req.AddJsonBody(model);
                var result = await client.ExecuteAsync(req);
                return result.IsSuccessful;
            }
        }

        public async Task<bool> DeleteModule(ModuleModel model)
        {
            using (var client = CoreServiceClient)
            {
                var req = new RestRequest("CoreModule/DeleteModule", Method.Post);
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
                var req = new RestRequest("CoreModule/GetModule", Method.Get);
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
                var req = new RestRequest("CoreModule/GetModuleList", Method.Get);
                var result = await client.ExecuteAsync(req);
                moduleList = JsonConvert.DeserializeObject<List<ModuleModel>>(result.Content);
            }

            return moduleList;
        }

        public async Task<List<ElementModel>> GetModuleElements(Guid id)
        {
            var elements = new List<ElementModel>();

            using (var client = CoreServiceClient)
            {
                var req = new RestRequest("CoreModule/GetModuleElements", Method.Get);
                req.AddParameter("moduleId", id);
                var result = await client.ExecuteAsync(req);
                elements = JsonConvert.DeserializeObject<List<ElementModel>>(result.Content);
            }

            return elements;
        }

        public async Task<ElementModel> GetElementData(Guid id)
        {
            var element = new ElementModel();

            using (var client = CoreServiceClient)
            {
                var req = new RestRequest("CoreModule/GetElementData", Method.Get);
                req.AddParameter("id", id);
                var result = await client.ExecuteAsync(req);
                element = JsonConvert.DeserializeObject<ElementModel>(result.Content);
            }

            return element;
        }

        public async Task<bool> SaveModuleContent(ElementModel model)
        {
            using (var client = CoreServiceClient)
            {
                var req = new RestRequest("CoreModule/SaveModuleContent", Method.Post);
                req.AddJsonBody(model);
                var result = await client.ExecuteAsync(req);
                return result.IsSuccessful;
            }

            return false;
        }
    }
}
