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

        #region Study


        /// <summary>
        /// çalışma listesini döner
        /// </summary>
        /// <returns>çalışmalar</returns>
        [HttpGet("{isLock}")]
        [Authorize(Roles = "TenantAdmin")]
        public async Task<List<StudyDTO>> GetStudyList(bool isLock)
        {
            var result = await _studyService.GetStudyList(isLock);

            return result;
        }


        /// <summary>
        /// seçili çalışmanın bilgilerini döner
        /// </summary>
        /// <param name="studyId">çalışma id</param>
        /// <returns>çalışma bilgileri</returns>
        [HttpGet("{studyId}")]
        [Authorize(Roles = "TenantAdmin")]
        public async Task<StudyDTO> GetStudy(Guid studyId)
        {
            var result = await _studyService.GetStudy(studyId);

            return result;
        }


        /// <summary>
        /// çalışma kaydeder
        /// </summary>
        /// <param name="studyModel">çalışma bilgileri</param>
        /// <returns>başarılı başarısız</returns>
        [HttpPost]
        [Authorize(Roles = "TenantAdmin")]
        public async Task<IActionResult> StudySave(StudyModel studyModel)
        {
            var result = await _studyService.StudySave(studyModel);

            return Ok(result);
        }


        /// <summary>
        /// çalışmayı kitler ya da kilidini açar
        /// </summary>
        /// <param name="studyLockDTO">çalışma bilgileri</param>
        /// <returns>başarılı başarısız</returns>
        [HttpPost]
        [Authorize(Roles = "TenantAdmin")]
        public async Task<IActionResult> StudyLockOrUnlock(StudyLockDTO studyLockDTO)
        {
            var result = await _studyService.StudyLockOrUnlock(studyLockDTO);

            return Ok(result);
        }
        #endregion

        #region Site


        /// <summary>
        /// çalışmadaki site listesini döner
        /// </summary>
        /// <param name="studyId">çalışma id</param>
        /// <returns>site listesi</returns>
        [HttpGet("{studyId}")]
        [Authorize(Roles = "TenantAdmin")]
        public async Task<List<SiteDTO>> GetSiteList(Guid studyId)
        {
            var result = await _studyService.GetSiteList(studyId);

            return result;
        }


        /// <summary>
        /// seçili site ı döner
        /// </summary>
        /// <param name="siteId">site id</param>
        /// <returns>site bilgisi</returns>
        [HttpGet("{siteId}")]
        [Authorize(Roles = "TenantAdmin")]
        public async Task<SiteDTO> GetSite(Guid siteId)
        {
            var result = await _studyService.GetSite(siteId);

            return result;
        }


        /// <summary>
        /// yeni bir site kaydeder veya seçili site ı günceller
        /// </summary>
        /// <param name="siteModel">site bilgisi</param>
        /// <returns>başarılı başarısız</returns>
        [HttpPost]
        [Authorize(Roles = "TenantAdmin")]
        public async Task<IActionResult> SiteSaveOrUpdate(SiteModel siteModel)
        {
            var result = await _studyService.SiteSaveOrUpdate(siteModel);

            return Ok(result);
        }


        /// <summary>
        /// seçili site ı siler
        /// </summary>
        /// <param name="siteModel">site bilgileri</param>
        /// <returns>başarılı başarısız</returns>
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
