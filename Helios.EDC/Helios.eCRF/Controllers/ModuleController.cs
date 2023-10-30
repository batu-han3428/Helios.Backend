using Helios.eCRF.Models;
using Helios.eCRF.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace Helios.eCRF.Controllers
{
    [ApiController]
    [Route("[controller]/[action]")]
    public class ModuleController : Controller
    {
        private IModuleService _moduleService;

        public ModuleController(IModuleService moduleService)
        {
            _moduleService = moduleService;
        }

        [HttpPost]
        public async Task<bool> SaveModule(ModuleModel model)
        {
            var result = false;

            if (!string.IsNullOrEmpty(model.Id))
            {
                result = await _moduleService.UpdateModule(model);
            }
            else
            {
                result = await _moduleService.AddModule(model);
            }

            return result;
        }

        [HttpPost]
        public async Task<bool> AddModule(string name)
        {
            var model = new ModuleModel { Name = name };
            var result = await _moduleService.AddModule(model);
            return result;
        }

        [HttpPost]
        public async Task<bool> UpdateModule(ModuleModel model)
        {
            var result = await _moduleService.UpdateModule(model);
            return result;
        }

        [HttpPost]
        public async Task<bool> DeleteModule(ModuleModel model)
        {
            var result = await _moduleService.DeleteModule(model);
            return result;
        }

        [HttpGet]
        public async Task<ModuleModel> GetModule(Guid id)
        {
            var result = await _moduleService.GetModule(id);

            return result;
        }

        [HttpGet]
        public async Task<List<ModuleModel>> GetModuleList()
        {
            var result = await _moduleService.GetModuleList();

            return result;
        }

        [HttpGet]
        public async Task<List<ElementModel>> GetModuleElements(string id)
        {
            var moduleId = Guid.Parse(id);
            var result = await _moduleService.GetModuleElements(moduleId);

            return result;
        }

        [HttpGet]
        public async Task<ElementModel> GetElementData(string id)
        {
            var elementId = Guid.Parse(id);
            var result = await _moduleService.GetElementData(elementId);

            return result;
        }

        [HttpPost]
        public async void SaveModuleContent(ElementModel model)
        {
            var result = await _moduleService.SaveModuleContent(model);
            //return result;
        }
    }
}
