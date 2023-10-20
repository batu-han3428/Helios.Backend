using Helios.eCRF.Models;
using Helios.eCRF.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace Helios.eCRF.Controllers
{
    [ApiController]
    [Route("[controller]/[action]")]
    public class ModuleController : Controller
    {
        private IStudyService _studyService;

        /// <summary>
        /// modül ekler
        /// </summary>
        /// <param name="name">modül adı</param>
        /// <returns>başarılı başarısız döner</returns>
        [HttpPost]
        public async Task<bool> AddModule(string name)
        {
            var model = new ModuleModel { Name = name };
            var result = await _studyService.AddModule(model);
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
            var result = await _studyService.UpdateModule(model);
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
            var result = await _studyService.DeleteModule(model);
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
            var result = await _studyService.GetModule(id);

            return result;
        }


        /// <summary>
        /// modülleri döner
        /// </summary>
        /// <returns>modüller</returns>
        [HttpGet]
        public async Task<List<ModuleModel>> GetModuleList()
        {
            var result = await _studyService.GetModuleList();

            return result;
        }

        [HttpPost]
        public async void SaveModuleContent(ElementModel model)
        {
            //var result = await _studyService.DeleteModule(model);
            //return result;
        }
    }
}
