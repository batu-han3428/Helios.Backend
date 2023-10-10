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
        
        [HttpGet]
        [Authorize(Roles = "TenantAdmin")]
        public async Task<List<StudyDTO>> GetStudyList()
        {
            var result = await _studyService.GetStudyList();

            return result;
        }

        [HttpGet("{studyId}")]
        public async Task<StudyDTO> GetStudy(Guid studyId)
        {
            var result = await _studyService.GetStudy(studyId);

            return result;
        }

        [HttpPost]
        public async Task<IActionResult> StudySave(StudyModel studyModel)
        {
            var result = await _studyService.StudySave(studyModel);

            return Ok(result);
        }
    }
}
