using Helios.Common.DTO;
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

        /// <summary>
        /// modül ekler
        /// </summary>
        /// <param name="name">modül adı</param>
        /// <returns>başarılı başarısız döner</returns>
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


        /// <summary>
        /// modül günceller
        /// </summary>
        /// <param name="model">modül bilgileri</param>
        /// <returns>başarılı başarısız döner</returns>
        [HttpPost]
        public async Task<bool> UpdateModule(ModuleModel model)
        {
            var result = await _moduleService.UpdateModule(model);
            return result;
        }


        /// <summary>
        /// modül siler
        /// </summary>
        /// <param name="model">modül bilgileri</param>
        /// <returns>başarılı başarısız döner</returns>
        [HttpPost]
        public async Task<bool> DeleteModule(ModuleModel model)
        {
            var result = await _moduleService.DeleteModule(model);
            return result;
        }


        /// <summary>
        /// seçili modülün bilgilerini getirir
        /// </summary>
        /// <param name="id">modül id</param>
        /// <returns>istenen modülün bilgilerini döner</returns>
        [HttpGet]
        public async Task<ModuleModel> GetModule(Guid id)
        {
            var result = await _moduleService.GetModule(id);

            return result;
        }


        /// <summary>
        /// modülleri döner
        /// </summary>
        /// <returns>modüller</returns>
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
        public async Task<ApiResponse<dynamic>> SaveModuleContent(ElementModel model)
        {
            var result = new ApiResponse<dynamic>();

            if (model.IsDependent
                && model.DependentSourceFieldId == null
                || model.DependentTargetFieldId == null
                || model.DependentCondition == 0
                || model.DependentAction == 0
                || model.DependentFieldValue == "")
            {
                result.IsSuccess = false;
                result.Message = "Dependent Error";
            }
            else
            {
                result = await _moduleService.SaveModuleContent(model);
            }
         
            return result;
        }

        [HttpPost]
        public async Task<ApiResponse<dynamic>> CopyElement(string id)
        {
            var elmId = Guid.Parse(id);
            var result = await _moduleService.CopyElement(elmId, new Guid());
            return result;
        }

        [HttpPost]
        public async Task<ApiResponse<dynamic>> DeleteElement(string id)
        {
            var elmId = Guid.Parse(id);
            var result = await _moduleService.DeleteElement(elmId, new Guid());
            return result;
        }
    }
}
