using Helios.Common.DTO;
using Helios.Common.Enums;
using Helios.Common.Model;
using Helios.Core.Contexts;
using Helios.Core.Domains.Entities;
using Helios.Core.helpers;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;

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

        #region Study
        [HttpGet]
        public async Task<List<StudyDTO>> GetStudyList(bool isLock)
        {
            var result = await _context.Studies.Where(x => !x.IsDemo && x.IsActive && !x.IsDeleted && x.IsLock == isLock).Select(x => new StudyDTO()
            {
                Id = x.Id,
                EquivalentStudyId = x.EquivalentStudyId,
                StudyName = x.StudyName,
                ProtocolCode = x.ProtocolCode,
                AskSubjectInitial = x.AskSubjectInitial,
                StudyLink = x.StudyLink,
                UpdatedAt = x.UpdatedAt,
                IsLock = x.IsLock
            }).ToListAsync();

            return result;
        }

        [HttpGet]
        public async Task<StudyDTO> GetStudy(Int64 studyId)
        {
            var study = await _context.Studies.FirstOrDefaultAsync(x => x.Id == studyId && x.IsActive && !x.IsDeleted);

            if (study != null)
            {
                return new StudyDTO
                {
                    Id = study.Id,
                    EquivalentStudyId = study.EquivalentStudyId,
                    StudyName = study.StudyName,
                    StudyLink = study.StudyLink,
                    IsDemo = study.IsDemo,
                    ProtocolCode = study.ProtocolCode,
                    StudyLanguage = study.StudyLanguage,
                    Description = study.Description,
                    SubDescription = study.SubDescription,
                    DoubleDataEntry = study.StudyType == (int)StudyType.DoubleEntry ? true : false,
                    AskSubjectInitial = study.AskSubjectInitial,
                    ReasonForChange = study.ReasonForChange,
                    UpdatedAt = study.UpdatedAt
                };
            }

            return new StudyDTO();
        }

        [HttpPost]
        public async Task<ApiResponse<dynamic>> StudySave(StudyModel studyModel)
        {
            if (studyModel.StudyId == 0)
            {
                int studiesCount = await _context.Studies.Where(x => x.TenantId == studyModel.TenantId && !x.IsDemo).CountAsync();

                if ((studyModel.StudyLimit != null ? Convert.ToInt32(studyModel.StudyLimit) : 0) <= studiesCount)
                {
                    return new ApiResponse<dynamic>
                    {
                        IsSuccess = false,
                        Message = "Your limit for adding study has been reached. Please contact the system administrator."
                    };
                }

                Int64 _versionKey = 0;
                Int64 _refKey = 0;

                studyModel.StudyLink = StringExtensionsHelper.TurkishCharacterReplace(studyModel.StudyLink);
                var checkShortName = _context.Studies.Any(c => c.StudyLink == studyModel.StudyLink);
                if (checkShortName)
                {
                    return new ApiResponse<dynamic>
                    {
                        IsSuccess = false,
                        Message = "The research short name cannot be the same as another research."
                    };
                }

                var setting = new ResearchSettingDto();
                setting.SubjectNumberDigits = studyModel.SubjectNumberDigist;

                var activeResearch = new Study()
                {
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

                var tenantResult = await _context.SaveCoreContextAsync(studyModel.UserId, DateTimeOffset.Now) > 0;

                if (tenantResult)
                {
                    demoResearch.EquivalentStudyId = activeResearch.Id;
                    activeResearch.EquivalentStudyId = demoResearch.Id;
                    _context.Studies.Update(activeResearch);
                    _context.Studies.Update(demoResearch);

                    var result = await _context.SaveCoreContextAsync(studyModel.UserId, DateTimeOffset.Now) > 0;

                    return new ApiResponse<dynamic>
                    {
                        IsSuccess = true,
                        Message = "Successful",
                        Values = new { studyId = activeResearch.Id, demoStudyId = demoResearch.Id }
                    };
                }
                return new ApiResponse<dynamic>
                {
                    IsSuccess = false,
                    Message = "Unsuccessful"
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
                        oldEntity.VersionKey = 0;
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
                        oldEntity.EquivalentStudy.VersionKey = 0;
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
                            Message = "Successful"
                        };
                    }
                    return new ApiResponse<dynamic>
                    {
                        IsSuccess = false,
                        Message = "Unsuccessful"
                    };
                }
                catch (Exception e)
                {

                    throw;
                }

            }
        }

        [HttpPost]
        public async Task<ApiResponse<dynamic>> StudyLockOrUnlock(StudyLockDTO studyLockDTO)
        {
            var study = await _context.Studies.Where(x => x.Id == studyLockDTO.Id).Include(x => x.EquivalentStudy).FirstOrDefaultAsync();
            
            if(study != null)
            {
                study.IsLock = !studyLockDTO.IsLock;
                study.EquivalentStudy.IsLock= !studyLockDTO.IsLock;

                _context.Studies.Update(study);

                var result = await _context.SaveCoreContextAsync(studyLockDTO.UserId, DateTimeOffset.Now) > 0;

                if (result)
                {
                    return new ApiResponse<dynamic>
                    {
                        IsSuccess = true,
                        Message = "Successful"
                    };
                }
                else
                {
                    return new ApiResponse<dynamic>
                    {
                        IsSuccess = false,
                        Message = "Unsuccessful"
                    };
                }
            }
            else
            {
                return new ApiResponse<dynamic>
                {
                    IsSuccess = false,
                    Message = "An unexpected error occurred."
                };
            }
        }
        #endregion

        #region Site
        [HttpGet]
        public async Task<List<SiteDTO>> GetSiteList(Int64 studyId)
        {
            var result = await _context.Sites.Where(p => p.StudyId == studyId && p.IsActive && !p.IsDeleted).AsNoTracking().Select(x => new SiteDTO()
            {
                Id = x.Id,
                SiteFullName = x.FullName,
                Code = x.Code,
                CountryCode = x.CountryCode,
                CountryName = x.Country,
                MaxEnrolmentCount = x.MaxEnrolmentCount,
                UpdatedAt = x.UpdatedAt
            }).ToListAsync();

            return result;
        }

        [HttpGet]
        public async Task<SiteDTO> GetSite(Int64 siteId)
        {
            var site = await _context.Sites.FirstOrDefaultAsync(x => x.Id == siteId && x.IsActive && !x.IsDeleted);

            if (site != null)
            {
                return new SiteDTO
                {
                    Id = site.Id,
                    Code = site.Code,
                    Name = site.Name,
                    CountryCode = site.CountryCode,
                    CountryName = site.Country,
                    MaxEnrolmentCount = site.MaxEnrolmentCount,
                };
            }

            return new SiteDTO();
        }

        [HttpPost]
        public async Task<ApiResponse<dynamic>> SiteSaveOrUpdate(SiteModel siteModel)
        {
            if (siteModel.Id == 0)
            {
                var hasSite = GetSiteList(siteModel.StudyId).Result.Any(a => a.Code == siteModel.Code && a.CountryCode == siteModel.CountryCode);

                if (hasSite)
                {
                    return new ApiResponse<dynamic>
                    {
                        IsSuccess = false,
                        Message = "The same site number cannot be added again for the same country."
                    };
                }

                await _context.Sites.AddAsync(new Site
                {
                    StudyId = siteModel.StudyId,
                    Name = siteModel.Name,
                    Code = siteModel.Code,
                    Country = siteModel.CountryName,
                    CountryCode = siteModel.CountryCode,
                    MaxEnrolmentCount = siteModel.MaxEnrolmentCount,
                    TenantId = siteModel.TenantId,
                });

                var result = await _context.SaveCoreContextAsync(siteModel.UserId, DateTimeOffset.Now) > 0;
                if (result)
                {
                    //to do addNewSiteToUsersThatSelectedAllSitesBefore

                    return new ApiResponse<dynamic>
                    {
                        IsSuccess = true,
                        Message = "Successful"
                    };
                }
                else
                {
                    return new ApiResponse<dynamic>
                    {
                        IsSuccess = false,
                        Message = "Unsuccessful"
                    };
                }
            }
            else
            {
                var hasSite = GetSiteList(siteModel.StudyId).Result.Any(a => a.Code == siteModel.Code && a.Id != siteModel.Id && a.CountryCode == siteModel.CountryCode);

                if (hasSite)
                {
                    return new ApiResponse<dynamic>
                    {
                        IsSuccess = false,
                        Message = "The same site number cannot be added again for the same country."
                    };
                }

                var oldEntity = _context.Sites.SingleOrDefault(a => a.Id == siteModel.Id);
                if (oldEntity != null)
                {
                    oldEntity.Name = siteModel.Name;
                    oldEntity.Code = siteModel.Code;
                    oldEntity.MaxEnrolmentCount = siteModel.MaxEnrolmentCount;
                    oldEntity.Country = siteModel.CountryName;
                    oldEntity.CountryCode = siteModel.CountryCode;

                    _context.Sites.Update(oldEntity);

                    var result = await _context.SaveCoreContextAsync(siteModel.UserId, DateTimeOffset.Now) > 0;

                    if (result)
                    {
                        return new ApiResponse<dynamic>
                        {
                            IsSuccess = true,
                            Message = "Successful"
                        };
                    }
                }
                return new ApiResponse<dynamic>
                {
                    IsSuccess = false,
                    Message = "Unsuccessful"
                };
            }
        }

        [HttpPost]
        public async Task<ApiResponse<dynamic>> SiteDelete(SiteModel siteModel)
        {
            //to do getUserCount

            var oldEntity = _context.Sites.FirstOrDefault(p => p.Id == siteModel.Id);
            if (oldEntity == null)
            {
                return new ApiResponse<dynamic>
                {
                    IsSuccess = false,
                    Message = "No record to delete was found."
                };
            }
                
            _context.Sites.Remove(oldEntity);

            var result = await _context.SaveCoreContextAsync(siteModel.UserId, DateTimeOffset.Now) > 0;

            if (result)
            {
                return new ApiResponse<dynamic>
                {
                    IsSuccess = true,
                    Message = "Successful"
                };
            }
            else
            {
                return new ApiResponse<dynamic>
                {
                    IsSuccess = false,
                    Message = "Unsuccessful"
                };
            }
        }
        #endregion

        #region Mail Template
        [HttpGet]
        public async Task<List<EmailTemplateModel>> GetEmailTemplateList(Int64 studyId)
        {
            var result = await _context.MailTemplates.Where(x => x.IsActive && !x.IsDeleted && x.StudyId == studyId).Select(x => new EmailTemplateModel()
            {
                Id = x.Id,
                Name = x.Name,
                TemplateType= x.TemplateType,
                CreatedAt= x.CreatedAt,
                UpdatedAt= x.UpdatedAt
            }).ToListAsync();

            return result;
        }

        [HttpPost]
        public async Task<ApiResponse<dynamic>> DeleteEmailTemplate(BaseDTO emailTemplateDTO)
        {
            if (emailTemplateDTO.UserId != 0 && emailTemplateDTO.Id != 0)
            {
                var data = await _context.MailTemplates.FirstOrDefaultAsync(x => x.IsActive && !x.IsDeleted && x.TenantId == emailTemplateDTO.TenantId && x.Id == emailTemplateDTO.Id);

                if (data != null)
                {
                    _context.MailTemplates.Remove(data);

                    var result = await _context.SaveCoreContextAsync(emailTemplateDTO.UserId, DateTimeOffset.Now) > 0;

                    if (result)
                    {
                        return new ApiResponse<dynamic>
                        {
                            IsSuccess = true,
                            Message = "Successful"
                        };
                    }
                    else
                    {
                        return new ApiResponse<dynamic>
                        {
                            IsSuccess = false,
                            Message = "Unsuccessful"
                        };
                    }
                }
            }

            return new ApiResponse<dynamic>
            {
                IsSuccess = false,
                Message = "An unexpected error occurred."
            };

        }

        [HttpGet]
        public async Task<EmailTemplateModel> GetEmailTemplate(Int64 templateId)
        {
            var result = await _context.MailTemplates.Where(x => x.IsActive && !x.IsDeleted && x.Id == templateId).Include(x=>x.MailTemplatesRoles).Select(x => new EmailTemplateModel()
            {
                Id = x.Id,
                TenantId = x.TenantId,
                StudyId = x.StudyId,
                TemplateBody = x.TemplateBody,
                ExternalMails = x.ExternalMails != "" ? JsonConvert.DeserializeObject<List<string>>(x.ExternalMails) : new List<string>(),
                Name = x.Name,
                TemplateType = x.TemplateType,
                Roles = x.MailTemplatesRoles.Where(x=>x.IsActive && !x.IsDeleted).Select(a=>a.RoleId).ToList(),
                CreatedAt = x.CreatedAt,
                UpdatedAt = x.UpdatedAt
            }).FirstOrDefaultAsync();

            return result;
        }

        [HttpGet]
        public async Task<List<EmailTemplateTagModel>> GetEmailTemplateTagList(Int64 tenantId, int templateType)
        {
            var result = await _context.MailTemplateTags.Where(x => x.IsActive && !x.IsDeleted && x.TenantId == tenantId && x.TemplateType == templateType).Select(x => new EmailTemplateTagModel()
            {
                Id = x.Id,
                Tag = x.Tag
            }).ToListAsync();

            return result;
        }

        [HttpPost]
        public async Task<ApiResponse<dynamic>> AddEmailTemplateTag(EmailTemplateTagDTO emailTemplateTagDTO)
        {
            if (emailTemplateTagDTO.UserId != 0 && emailTemplateTagDTO.TenantId != 0)
            {
                if(_context.MailTemplateTags.Any(x=>x.IsActive && !x.IsDeleted && x.TenantId == emailTemplateTagDTO.TenantId && x.TemplateType == emailTemplateTagDTO.TemplateType && x.Tag == emailTemplateTagDTO.Tag.Trim())){
                    return new ApiResponse<dynamic>
                    {
                        IsSuccess = false,
                        Message = "This tag is registered for this template type."
                    };
                }

                await _context.MailTemplateTags.AddAsync(new MailTemplateTags
                {
                    TenantId = emailTemplateTagDTO.TenantId,
                    Tag = emailTemplateTagDTO.Tag.Trim(),
                    TemplateType = emailTemplateTagDTO.TemplateType.Value
                });

                var result = await _context.SaveCoreContextAsync(emailTemplateTagDTO.UserId, DateTimeOffset.Now) > 0;

                if (result)
                {
                    return new ApiResponse<dynamic>
                    {
                        IsSuccess = true,
                        Message = "Successful"
                    };
                }
                else
                {
                    return new ApiResponse<dynamic>
                    {
                        IsSuccess = false,
                        Message = "Unsuccessful"
                    };
                }
            }
            else
            {
                return new ApiResponse<dynamic>
                {
                    IsSuccess = false,
                    Message = "An unexpected error occurred."
                };
            }
        }

        [HttpPost]
        public async Task<ApiResponse<dynamic>> DeleteEmailTemplateTag(EmailTemplateTagDTO emailTemplateTagDTO)
        {
            if (emailTemplateTagDTO.UserId != 0 && emailTemplateTagDTO.Id != 0)
            {
                var data = await _context.MailTemplateTags.FirstOrDefaultAsync(x => x.IsActive && !x.IsDeleted && x.TenantId == emailTemplateTagDTO.TenantId && x.Id == emailTemplateTagDTO.Id);

                if (data != null)
                {
                    _context.MailTemplateTags.Remove(data);

                    var result = await _context.SaveCoreContextAsync(emailTemplateTagDTO.UserId, DateTimeOffset.Now) > 0;

                    if (result)
                    {
                        return new ApiResponse<dynamic>
                        {
                            IsSuccess = true,
                            Message = "Successful"
                        };
                    }
                    else
                    {
                        return new ApiResponse<dynamic>
                        {
                            IsSuccess = false,
                            Message = "Unsuccessful"
                        };
                    }
                }
            }
            
            return new ApiResponse<dynamic>
            {
                IsSuccess = false,
                Message = "An unexpected error occurred."
            };
          
        }

        [HttpPost]
        public async Task<ApiResponse<dynamic>> SetEmailTemplate(EmailTemplateDTO emailTemplateDTO)
        {
            if (emailTemplateDTO.Id == 0)
            {
                if (_context.MailTemplates.Any(x => x.IsActive && !x.IsDeleted && x.TenantId == emailTemplateDTO.TenantId && x.StudyId == emailTemplateDTO.StudyId && x.Name == emailTemplateDTO.Name.Trim()))
                {
                    return new ApiResponse<dynamic>
                    {
                        IsSuccess = false,
                        Message = "This template name already exists."
                    };
                }

                var mailTemplate = new MailTemplates()
                {
                    TenantId = emailTemplateDTO.TenantId,
                    TemplateType = emailTemplateDTO.TemplateType,
                    StudyId = emailTemplateDTO.StudyId,
                    Name = emailTemplateDTO.Name.Trim(),
                    TemplateBody = emailTemplateDTO.Editor,
                    ExternalMails = emailTemplateDTO.ExternalMails.Count > 0 ? JsonConvert.SerializeObject(emailTemplateDTO.ExternalMails) : "",
                };

                var roles = emailTemplateDTO.Roles.Select(x => new MailTemplatesRoles
                {
                    RoleId = x,
                    TenantId = emailTemplateDTO.TenantId
                }).ToList();

                mailTemplate.MailTemplatesRoles.AddRange(roles);
     
                await _context.MailTemplates.AddAsync(mailTemplate);

                var result = await _context.SaveCoreContextAsync(emailTemplateDTO.UserId, DateTimeOffset.Now);

                if (result > 0)
                {
                    return new ApiResponse<dynamic>
                    {
                        IsSuccess = true,
                        Message = "Successful"
                    };
                }
                else if (result == 0)
                {
                    return new ApiResponse<dynamic>
                    {
                        IsSuccess = false,
                        Message = "No changes were made. Please make changes to save."
                    };
                }
                else
                {
                    return new ApiResponse<dynamic>
                    {
                        IsSuccess = false,
                        Message = "Unsuccessful"
                    };
                }
            }
            else
            {
                var data = await _context.MailTemplates.Where(x => x.IsActive && !x.IsDeleted && x.TenantId == emailTemplateDTO.TenantId && x.StudyId == emailTemplateDTO.StudyId && x.Id == emailTemplateDTO.Id).Include(x=>x.MailTemplatesRoles).FirstOrDefaultAsync();

                if (data == null)
                {
                    return new ApiResponse<dynamic>
                    {
                        IsSuccess = false,
                        Message = "An unexpected error occurred."
                    };
                }
               
                if (_context.MailTemplates.Any(x => x.IsActive && !x.IsDeleted && x.TenantId == emailTemplateDTO.TenantId && x.StudyId == emailTemplateDTO.StudyId && x.Id != emailTemplateDTO.Id && x.Name == emailTemplateDTO.Name.Trim()))
                {
                    return new ApiResponse<dynamic>
                    {
                        IsSuccess = false,
                        Message = "This template name already exists."
                    };
                }


                data.TemplateType = emailTemplateDTO.TemplateType;
                data.Name = emailTemplateDTO.Name;
                data.TemplateBody = emailTemplateDTO.Editor;
                data.ExternalMails = emailTemplateDTO.ExternalMails.Count > 0 ? JsonConvert.SerializeObject(emailTemplateDTO.ExternalMails) : "";


                var currentRoleIds = data.MailTemplatesRoles.Where(x => x.IsActive && !x.IsDeleted).Select(s => s.RoleId).ToList();
                var newRoleIds = emailTemplateDTO.Roles.ToList();

                if (!currentRoleIds.SequenceEqual(newRoleIds))
                {
                    _context.MailTemplatesRoles.RemoveRange(data.MailTemplatesRoles);

                    foreach (var roleId in newRoleIds)
                    {
                        var newUserSite = new MailTemplatesRoles
                        {
                            TenantId = data.TenantId,
                            RoleId = roleId,
                            MailTemplateId = data.Id
                        };
                        await _context.MailTemplatesRoles.AddAsync(newUserSite);
                    }
                }

                var result = await _context.SaveCoreContextAsync(emailTemplateDTO.UserId, DateTimeOffset.Now);

                if (result > 0)
                {
                    return new ApiResponse<dynamic>
                    {
                        IsSuccess = true,
                        Message = "Successful"
                    };
                }
                else if (result == 0)
                {
                    return new ApiResponse<dynamic>
                    {
                        IsSuccess = false,
                        Message = "No changes were made. Please make changes to save."
                    };
                }
                else
                {
                    return new ApiResponse<dynamic>
                    {
                        IsSuccess = false,
                        Message = "Unsuccessful"
                    };
                }
            }
        }
        #endregion
    }
}
