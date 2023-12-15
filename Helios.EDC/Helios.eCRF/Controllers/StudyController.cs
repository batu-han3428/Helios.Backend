using Helios.Common.DTO;
using Helios.Common.Model;
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
        public async Task<IActionResult> GetStudyList(bool isLock)
        {
            try
            {
                var result = await _studyService.GetStudyList(isLock);

                return Ok(result);
            }
            catch (Exception e)
            {
                return StatusCode(500);
            }
        }


        /// <summary>
        /// seçili çalışmanın bilgilerini döner
        /// </summary>
        /// <param name="studyId">çalışma id</param>
        /// <returns>çalışma bilgileri</returns>
        [HttpGet("{studyId}")]
        [Authorize(Roles = "TenantAdmin")]
        public async Task<IActionResult> GetStudy(Int64 studyId)
        {
            try
            {
                var result = await _studyService.GetStudy(studyId);

                return Ok(result);
            }
            catch (Exception e)
            {
                return StatusCode(500);
            }
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
        public async Task<IActionResult> GetSiteList(Int64 studyId)
        {
            try
            {
                var result = await _studyService.GetSiteList(studyId);

                return Ok(result);
            }
            catch (Exception e)
            {
                return StatusCode(500);
            }
        }


        /// <summary>
        /// seçili site ı döner
        /// </summary>
        /// <param name="siteId">site id</param>
        /// <returns>site bilgisi</returns>
        [HttpGet("{siteId}")]
        [Authorize(Roles = "TenantAdmin")]
        public async Task<IActionResult> GetSite(Int64 siteId)
        {
            try
            {
                var result = await _studyService.GetSite(siteId);

                return Ok(result);
            }
            catch (Exception e)
            {
                return StatusCode(500);
            }
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

        #region Mail Template

        /// <summary>
        /// Çalışmanın mail templatelerini listeler
        /// </summary>
        /// <param name="studyId">çalışma id</param>
        /// <returns>mail templateler</returns>
        [HttpGet("{studyId}")]
        [Authorize(Roles = "TenantAdmin")]
        public async Task<IActionResult> GetEmailTemplateList(Int64 studyId)
        {
            try
            {
                var result = await _studyService.GetEmailTemplateList(studyId);

                return Ok(result);
            }
            catch (Exception e)
            {
                return StatusCode(500);
            }
        }


        /// <summary>
        /// email template siler
        /// </summary>
        /// <param name="emailTemplateDTO">template bilgileri</param>
        /// <returns>başarılı başarısız</returns>
        [HttpPost]
        [Authorize(Roles = "TenantAdmin")]
        public async Task<IActionResult> DeleteEmailTemplate(BaseDTO emailTemplateDTO)
        {
            var result = await _studyService.DeleteEmailTemplate(emailTemplateDTO);

            return Ok(result);
        }


        /// <summary>
        /// seçili mail template getirir
        /// </summary>
        /// <param name="templateId">template id</param>
        /// <returns>mail template</returns>
        [HttpGet("{templateId}")]
        [Authorize(Roles = "TenantAdmin")]
        public async Task<IActionResult> GetEmailTemplate(Int64 templateId)
        {
            try
            {
                var result = await _studyService.GetEmailTemplate(templateId);

                return Ok(result);
            }
            catch (Exception e)
            {
                return StatusCode(500);
            }
        }


        /// <summary>
        /// email template tagları listeler
        /// </summary>
        /// <param name="tenantId">tenant id</param>
        /// <param name="templateType">template type</param>
        /// <returns>tag listesi</returns>
        [HttpGet("{tenantId}/{templateType}")]
        [Authorize(Roles = "TenantAdmin")]
        public async Task<IActionResult> GetEmailTemplateTagList(Int64 tenantId, int templateType)
        {
            try
            {
                var result = await _studyService.GetEmailTemplateTagList(tenantId, templateType);

                return Ok(result);
            }
            catch (Exception e)
            {
                return StatusCode(500);
            }
        }


        /// <summary>
        /// email template tag ekler
        /// </summary>
        /// <param name="emailTemplateTagDTO">tag bilgileri</param>
        /// <returns>başarılı başarısız</returns>
        [HttpPost]
        [Authorize(Roles = "TenantAdmin")]
        public async Task<IActionResult> AddEmailTemplateTag(EmailTemplateTagDTO emailTemplateTagDTO)
        {
            var result = await _studyService.AddEmailTemplateTag(emailTemplateTagDTO);

            return Ok(result);
        }


        /// <summary>
        /// email template tag siler
        /// </summary>
        /// <param name="emailTemplateTagDTO">tag bilgileri</param>
        /// <returns>başarılı başarısız</returns>
        [HttpPost]
        [Authorize(Roles = "TenantAdmin")]
        public async Task<IActionResult> DeleteEmailTemplateTag(EmailTemplateTagDTO emailTemplateTagDTO)
        {
            var result = await _studyService.DeleteEmailTemplateTag(emailTemplateTagDTO);

            return Ok(result);
        }


        /// <summary>
        /// email template ekler ya da günceller
        /// </summary>
        /// <param name="emailTemplateDTO">email template bilgileri</param>
        /// <returns>başarılı başarısız</returns>
        [HttpPost]
        [Authorize(Roles = "TenantAdmin")]
        public async Task<IActionResult> SetEmailTemplate(EmailTemplateDTO emailTemplateDTO)
        {
            var result = await _studyService.SetEmailTemplate(emailTemplateDTO);

            return Ok(result);
        }
        #endregion
    }
}
