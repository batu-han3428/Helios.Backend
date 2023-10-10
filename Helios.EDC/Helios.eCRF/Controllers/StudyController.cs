using Helios.Common.DTO;
using Helios.Common.Model;
using Helios.eCRF.Models;
using Helios.eCRF.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
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
        #region Study

        [HttpGet]
        [Authorize(Roles = "TenantAdmin")]
        public async Task<List<StudyDTO>> GetStudyList()
        {
            var result = await _studyService.GetStudyList();

            return result;
        }

        [HttpGet("{studyId}")]
        [Authorize(Roles = "TenantAdmin")]
        public async Task<StudyDTO> GetStudy(Guid studyId)
        {
            var result = await _studyService.GetStudy(studyId);

            return result;
        }

        [HttpPost]
        [Authorize(Roles = "TenantAdmin")]
        public async Task<IActionResult> StudySave(StudyModel studyModel)
        {
            var result = await _studyService.StudySave(studyModel);

            return Ok(result);
        }
        #endregion

        #region Site
        [HttpGet("{studyId}")]
        [Authorize(Roles = "TenantAdmin")]
        public async Task<List<SiteDTO>> GetSiteList(Guid studyId)
        {
            var result = await _studyService.GetSiteList(studyId);

            return result;
        }

        [HttpGet("{siteId}")]
        [Authorize(Roles = "TenantAdmin")]
        public async Task<SiteDTO> GetSite(Guid siteId)
        {
            var result = await _studyService.GetSite(siteId);

            return result;
        }

        [HttpPost]
        [Authorize(Roles = "TenantAdmin")]
        public async Task<IActionResult> SiteSaveOrUpdate(SiteModel siteModel)
        {
            var result = await _studyService.SiteSaveOrUpdate(siteModel);

            return Ok(result);
        }

        [HttpPost]
        [Authorize(Roles = "TenantAdmin")]
        public async Task<IActionResult> SiteDelete(SiteModel siteModel)
        {
            var result = await _studyService.SiteDelete(siteModel);

            return Ok(result);
        }
        #endregion
    }
}
