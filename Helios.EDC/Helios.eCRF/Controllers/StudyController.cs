using Helios.eCRF.Models;
using Helios.eCRF.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace Helios.eCRF.Controllers
{
    [ApiController]
    [Route("[controller]/[action]")]
    public class StudyController : Controller
    {
        private IStudyService _studyService;

        public StudyController(IStudyService studyService)
        {
            _studyService = studyService;
        }

        [HttpPost]
        public async Task<bool> AddModule(string name)
        {
            var model = new ModuleModel { Name = name };
            var result = await _studyService.AddModule(model);
            return result;
        }

        [HttpPost]
        public async Task<bool> UpdateModule(ModuleModel model)
        {
            var result = await _studyService.UpdateModule(model);
            return result;
        }
        
        [HttpPost]
        public async Task<bool> DeleteModule(ModuleModel model)
        {
            var result = await _studyService.DeleteModule(model);
            return result;
        }

        [HttpGet]
        public async Task<ModuleModel> GetModule(Guid id)
        {
            var result = await _studyService.GetModule(id);

            return result;
        }

        [HttpGet]
        public async Task<List<ModuleModel>> GetModuleList()
        {
            var result = await _studyService.GetModuleList();

            return result;
        }
    }
}
