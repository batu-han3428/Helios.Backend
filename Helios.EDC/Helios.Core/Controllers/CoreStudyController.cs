using Helios.Common.DTO;
using Helios.Common.Enums;
using Helios.Common.Model;
using Helios.Core.Contexts;
using Helios.Core.Domains.Entities;
using Helios.Core.helpers;
using Helios.Core.Models;
using MassTransit.Internals.GraphValidation;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using System.ComponentModel.DataAnnotations.Schema;

namespace Helios.Core.Controllers
{
    [ApiController]
    [Route("[controller]/[action]")]
    public class CoreStudyController : Controller
    {
        private CoreContext _context;

        public CoreStudyController(CoreContext context)
        {
            _context = context;
        }

        #region Module
        [HttpPost]
        public async Task<bool> AddModule(ModuleModel model)
        {
            _context.Modules.Add(new Module
            {
                AddedById = model.UserId,
                Name = model.Name,
                CreatedAt = DateTimeOffset.Now,
                IsActive = true
            });

            var result = await _context.SaveAuthenticationContextAsync(new Guid(), DateTimeOffset.Now) > 0;

            return result;
        }

        [HttpPost]
        public async Task<bool> UpdateModule(ModuleModel model)
        {
            var module = await _context.Modules.Where(x => x.Id == model.Id && x.IsActive && !x.IsDeleted).FirstOrDefaultAsync();

            if (module == null)
            {
                return false;
            }

            module.Name = model.Name;
            module.UpdatedAt = DateTimeOffset.Now;
            module.UpdatedById = model.UserId;

            _context.Modules.Update(module);

            return true;
        }
        
        [HttpPost]
        public async Task<bool> DeleteModule(ModuleModel model)
        {
            var module = await _context.Modules.Where(x => x.Id == model.Id && x.IsActive && !x.IsDeleted).FirstOrDefaultAsync();

            if (module == null)
            {
                return false;
            }

            module.UpdatedAt = DateTimeOffset.Now;
            module.UpdatedById = model.UserId;
            module.IsActive = false;
            module.IsDeleted = true;

            _context.Modules.Update(module);

            return true;
        }

        [HttpGet]
        public async Task<ModuleModel> GetModule(Guid id)
        {
            var model = new ModuleModel();

            var module = await _context.Modules.FirstOrDefaultAsync(x => x.Id == id && x.IsActive && !x.IsDeleted);

            if (module != null)
            {
                model.Id = module.Id;
                model.Name = module.Name;
            }

            return model;
        }

        [HttpGet]
        public async Task<List<ModuleModel>> GetModuleList()
        {
            var result = await _context.Modules.Where(x => x.IsActive && !x.IsDeleted).Select(x => new ModuleModel()
            {
                Id = x.Id,
                Name = x.Name
            }).ToListAsync();

            return result;
        }

        #endregion

        #region Study
        [HttpGet]
        public async Task<List<StudyDTO>> GetStudyList()
        {
            var result = await _context.Studies.Where(x => !x.IsDemo && x.IsActive && !x.IsDeleted).Select(x => new StudyDTO()
            {
                Id = x.Id,
                StudyName = x.StudyName,
                ProtocolCode = x.ProtocolCode,
                AskSubjectInitial = x.AskSubjectInitial,
                StudyLink = x.StudyLink,
                UpdatedAt = x.UpdatedAt
            }).ToListAsync();

            return result;
        }

        [HttpGet]
        public async Task<StudyDTO> GetStudy(Guid studyId)
        {
            var study = await _context.Studies.FirstOrDefaultAsync(x => x.Id == studyId && x.IsActive && !x.IsDeleted);
               
            if(study != null)
            {
                return new StudyDTO
                {
                    Id = study.Id,
                    StudyName = study.StudyName,
                    StudyLink = study.StudyLink,
                    ProtocolCode = study.ProtocolCode,
                    StudyLanguage = study.StudyLanguage,
                    Description= study.Description,
                    SubDescription= study.SubDescription,
                    DoubleDataEntry = study.StudyType == (int)StudyType.DoubleEntry ? true : false,
                    AskSubjectInitial = study.AskSubjectInitial,
                    ReasonForChange= study.ReasonForChange,
                    UpdatedAt = study.UpdatedAt
                };
            }

            return new StudyDTO();
        }

        [HttpPost]
        public async Task<ApiResponse<dynamic>> StudySave(StudyModel studyModel)
        {
            _context.SetCurrentUser(studyModel.UserId);
            if (studyModel.StudyId == Guid.Empty)
            {
                Guid _versionKey = Guid.NewGuid();
                Guid _refKey = Guid.NewGuid();
                studyModel.StudyLink = StringExtensionsHelper.TurkishCharacterReplace(studyModel.StudyLink);
                var checkShortName = _context.Studies.Any(c => c.StudyLink == studyModel.StudyLink);
                if (checkShortName)
                {
                    return new ApiResponse<dynamic>
                    {
                        IsSuccess = false,
                        Message = "js_ShortNameWarning"
                    };
                }

                var setting = new ResearchSettingDto();
                setting.SubjectNumberDigits = studyModel.SubjectNumberDigist;

                var activeResearch = new Study()
                {
                    Id = Guid.NewGuid(),
                    TenantId = studyModel.TenantId,
                    IsDemo = false,
                    StudyName = studyModel.StudyName,
                    ProtocolCode = studyModel.ProtocolCode,
                    AskSubjectInitial = studyModel.AskSubjectInitial,
                    StudyLink = studyModel.StudyLink,
                    //RandomizationAlgorithm = (RandomizationAlgorithm)research.RandomizationAlgorithm,
                    StudyType = studyModel.DoubleDataEntry ? (int)StudyType.DoubleEntry : (int)StudyType.Normal,
                    SubDescription = studyModel.SubDescription,
                    Description = studyModel.Description,
                    //DataLock = false,
                    //SignDescription = research.SignDescription,
                    //WillBeSigned = research.WillBeSigned,
                    ReasonForChange = studyModel.ReasonForChange,
                    //PatientState = research.PatientState,
                    StudyLanguage = studyModel.StudyLanguage,
                    VersionKey = _versionKey,
                    ReferenceKey = _refKey,
                    IsActive = true,
                    IsDeleted = false
                    //SettingsJson = stJson,
                    //CompanyLogoPath = !string.IsNullOrEmpty(path) ? path : null,
                };
                var demoResearch = new Study
                {
                    Id = Guid.NewGuid(),
                    TenantId = studyModel.TenantId,
                    IsDemo = true,
                    StudyName = "DEMO-" + studyModel.StudyName,
                    ProtocolCode = "DEMO-" + studyModel.ProtocolCode,
                    StudyLink = "DEMO-" + studyModel.StudyLink,
                    AskSubjectInitial = studyModel.AskSubjectInitial,
                    //RandomizationAlgorithm = (RandomizationAlgorithm)research.RandomizationAlgorithm,
                    StudyType = studyModel.DoubleDataEntry ? (int)StudyType.DoubleEntry : (int)StudyType.Normal,
                    SubDescription = studyModel.SubDescription,
                    Description = studyModel.Description,
                    //DataLock = false,
                    //SignDescription = research.SignDescription,
                    //WillBeSigned = research.WillBeSigned,
                    ReasonForChange = studyModel.ReasonForChange,
                    //PatientState = research.PatientState,
                    StudyLanguage = studyModel.StudyLanguage,
                    VersionKey = _versionKey,
                    ReferenceKey = _refKey,
                    IsActive = true,
                    IsDeleted = false
                    //SettingsJson = stJson,
                    //CompanyLogoPath = !string.IsNullOrEmpty(path) ? path : null,
                };
                _context.Studies.Add(activeResearch);
                _context.Studies.Add(demoResearch);               
                var tenantResult = await _context.SaveChangesAsync() > 0;
                if (tenantResult)
                {
                    demoResearch.EquivalentStudyId = activeResearch.Id;
                    activeResearch.EquivalentStudyId = demoResearch.Id;
                    _context.Studies.Update(activeResearch);
                    _context.Studies.Update(demoResearch);

                    var result = await _context.SaveChangesAsync() > 0;

                    return new ApiResponse<dynamic>
                    {
                        IsSuccess = true,
                        Message = "Kayıt Başarılı",
                        Values = new { studyId = activeResearch.Id, demoStudyId = demoResearch.Id }
                    };
                }
                return new ApiResponse<dynamic>
                {
                    IsSuccess = false,
                    Message = "Kayıt Başarısız"
                };
            }
            else
            {
                try
                {
                    var oldEntity = _context.Studies.Include(x => x.EquivalentStudy).FirstOrDefault(p => p.Id == studyModel.StudyId && p.IsActive && !p.IsDeleted);
                    if (oldEntity != null)
                    {
                        studyModel.StudyLink = StringExtensionsHelper.TurkishCharacterReplace(studyModel.StudyLink);
                        oldEntity.ProtocolCode = studyModel.ProtocolCode;
                        oldEntity.StudyName = studyModel.StudyName;
                        oldEntity.AskSubjectInitial = studyModel.AskSubjectInitial;
                        oldEntity.StudyLink = studyModel.StudyLink;
                        //oldEntity.WillBeSigned = studyModel.WillBeSigned;
                        oldEntity.ReasonForChange = studyModel.ReasonForChange;
                        //oldEntity.SignDescription = studyModel.SignDescription;
                        //oldEntity.RandomizationAlgorithm = (RandomizationAlgorithm)research.RandomizationAlgorithm;
                        oldEntity.Description = studyModel.Description;
                        oldEntity.SubDescription = studyModel.SubDescription;
                        oldEntity.StudyType = studyModel.DoubleDataEntry ? (int)StudyType.DoubleEntry : (int)StudyType.Normal;
                        oldEntity.VersionKey = Guid.NewGuid();
                        //oldEntity.PatientState = research.PatientState;
                        oldEntity.StudyLanguage = studyModel.StudyLanguage;
                        //if (!string.IsNullOrEmpty(path))
                        //{
                        //    oldEntity.CompanyLogoPath = path;
                        //    oldEntity.EquivalentStudy.CompanyLogoPath = path;
                        //}
                        //if (!oldEntity.IsDemo)
                        //    oldEntity.TermsOfUseFilePath = research.TermsOfUseFilePath;
                        //oldEntity.SettingsJson = stJson;

                        _context.Studies.Update(oldEntity);
                        var ss = await _context.SaveChangesAsync() > 0;
                        //Demo
                        oldEntity.EquivalentStudy.ProtocolCode = "DEMO-" + studyModel.ProtocolCode;
                        oldEntity.EquivalentStudy.StudyName = "DEMO-" + studyModel.StudyName;
                        oldEntity.EquivalentStudy.StudyLink = "DEMO-" + studyModel.StudyLink;
                        oldEntity.EquivalentStudy.AskSubjectInitial = studyModel.AskSubjectInitial;
                        //oldEntity.EquivalentStudy.WillBeSigned = research.WillBeSigned;
                        oldEntity.EquivalentStudy.ReasonForChange = studyModel.ReasonForChange;
                        //oldEntity.EquivalentStudy.SignDescription = research.SignDescription;
                        //oldEntity.EquivalentStudy.RandomizationAlgorithm = (RandomizationAlgorithm)research.RandomizationAlgorithm;
                        oldEntity.EquivalentStudy.Description = studyModel.Description;
                        oldEntity.EquivalentStudy.SubDescription = studyModel.SubDescription;
                        oldEntity.EquivalentStudy.StudyType = studyModel.DoubleDataEntry ? (int)StudyType.DoubleEntry : (int)StudyType.Normal;
                        oldEntity.EquivalentStudy.VersionKey = Guid.NewGuid();
                        //oldEntity.EquivalentStudy.PatientState = research.PatientState;
                        oldEntity.EquivalentStudy.StudyLanguage = studyModel.StudyLanguage;
                        //if (oldEntity.IsDemo)
                        //    oldEntity.TermsOfUseFilePath = research.TermsOfUseFilePath;
                        //oldEntity.EquivalentStudy.SettingsJson = stJson;

                        _context.Studies.Update(oldEntity);

                        var result = await _context.SaveChangesAsync() > 0;
                        return new ApiResponse<dynamic>
                        {
                            IsSuccess = result,
                            Message = "Güncelleme Başarılı"
                        };
                    }
                    return new ApiResponse<dynamic>
                    {
                        IsSuccess = false,
                        Message = "Güncelleme Başarısız"
                    };
                }
                catch (Exception e)
                {

                    throw;
                }
               
            }           
        }
        #endregion
    }
}
