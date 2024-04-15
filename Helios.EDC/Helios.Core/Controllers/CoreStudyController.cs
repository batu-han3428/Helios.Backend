using Helios.Common.DTO;
using Helios.Common.Enums;
using Helios.Common.Model;
using Helios.Core.Contexts;
using Helios.Core.Domains.Entities;
using Helios.Core.helpers;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;
using Helios.Common.Helpers.Api;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;

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

            if (study != null)
            {
                study.IsLock = !studyLockDTO.IsLock;
                study.EquivalentStudy.IsLock = !studyLockDTO.IsLock;

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
                TemplateType = x.TemplateType,
                CreatedAt = x.CreatedAt,
                UpdatedAt = x.UpdatedAt
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
            var result = await _context.MailTemplates.Where(x => x.IsActive && !x.IsDeleted && x.Id == templateId).Include(x => x.MailTemplatesRoles).Select(x => new EmailTemplateModel()
            {
                Id = x.Id,
                TenantId = x.TenantId,
                StudyId = x.StudyId,
                TemplateBody = x.TemplateBody,
                //ExternalMails = x.ExternalMails != "" ? JsonConvert.DeserializeObject<List<string>>(x.ExternalMails) : new List<string>(),
                //ExternalMails = x.ExternalMails != "" ? JsonSerializer.Deserialize<List<string>>(x.ExternalMails) : new List<string>(),
                Name = x.Name,
                TemplateType = x.TemplateType,
                Roles = x.MailTemplatesRoles.Where(x => x.IsActive && !x.IsDeleted).Select(a => a.RoleId).ToList(),
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
                if (_context.MailTemplateTags.Any(x => x.IsActive && !x.IsDeleted && x.TenantId == emailTemplateTagDTO.TenantId && x.TemplateType == emailTemplateTagDTO.TemplateType && x.Tag == emailTemplateTagDTO.Tag.Trim()))
                {
                    return new ApiResponse<dynamic>
                    {
                        IsSuccess = false,
                        Message = "This tag is registered for this template type."
                    };
                }

                await _context.MailTemplateTags.AddAsync(new MailTemplateTag
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

                var mailTemplate = new MailTemplate()
                {
                    TenantId = emailTemplateDTO.TenantId,
                    TemplateType = emailTemplateDTO.TemplateType,
                    StudyId = emailTemplateDTO.StudyId,
                    Name = emailTemplateDTO.Name.Trim(),
                    TemplateBody = emailTemplateDTO.Editor,
                    ExternalMails = emailTemplateDTO.ExternalMails.Count > 0 ? JsonSerializer.Serialize(emailTemplateDTO.ExternalMails) : "",
                };

                var roles = emailTemplateDTO.Roles.Select(x => new MailTemplatesRole
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
                var data = await _context.MailTemplates.Where(x => x.IsActive && !x.IsDeleted && x.TenantId == emailTemplateDTO.TenantId && x.StudyId == emailTemplateDTO.StudyId && x.Id == emailTemplateDTO.Id).Include(x => x.MailTemplatesRoles).FirstOrDefaultAsync();

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
                data.ExternalMails = emailTemplateDTO.ExternalMails.Count > 0 ? JsonSerializer.Serialize(emailTemplateDTO.ExternalMails) : "";


                var currentRoleIds = data.MailTemplatesRoles.Where(x => x.IsActive && !x.IsDeleted).Select(s => s.RoleId).ToList();
                var newRoleIds = emailTemplateDTO.Roles.ToList();

                if (!currentRoleIds.SequenceEqual(newRoleIds))
                {
                    _context.MailTemplatesRoles.RemoveRange(data.MailTemplatesRoles);

                    foreach (var roleId in newRoleIds)
                    {
                        var newUserSite = new MailTemplatesRole
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

        #region Visit
        [HttpGet]
        public async Task<List<VisitModel>> GetVisits(Int64 studyId)
        {
            return await _context.StudyVisits.Where(x => x.IsActive && !x.IsDeleted && x.StudyId == studyId).Include(x => x.StudyVisitPages).ThenInclude(x => x.StudyVisitPageModules).Select(x => new VisitModel
            {
                Id = x.Id,
                Name = x.Name,
                VisitType = (VisitType)x.VisitType,
                Order = x.Order,
                CreatedAt = x.CreatedAt,
                UpdatedAt = x.UpdatedAt,
                Children = x.StudyVisitPages.Where(page => page.IsActive && !page.IsDeleted).Select(page => new VisitModel
                {
                    Id = page.Id,
                    Name = page.Name,
                    Order = page.Order,
                    CreatedAt = x.CreatedAt,
                    UpdatedAt = page.UpdatedAt,
                    EPro = page.EPro,
                    Children = page.StudyVisitPageModules.Where(module => module.IsActive && !module.IsDeleted).Select(module => new VisitModel
                    {
                        Id = module.Id,
                        Name = module.Name,
                        Order = module.Order,
                        CreatedAt = x.CreatedAt,
                        UpdatedAt = module.UpdatedAt
                    }).ToList()
                }).ToList()
            }).ToListAsync();
        }

        [HttpPost]
        public async Task<ApiResponse<dynamic>> SetVisits(VisitDTO visitDTO)
        {
            try
            {
                if (visitDTO.Id == null || visitDTO.Id == 0)
                {
                    if (visitDTO.Type == VisitStatu.visit.ToString())
                    {
                        StudyVisit visit = new StudyVisit
                        {
                            StudyId = visitDTO.StudyId,
                            Name = visitDTO.Name,
                            VisitType = visitDTO.VisitType.Value,
                            Order = visitDTO.Order,
                        };

                        List<Permission> permissions = new List<Permission>();
                        foreach (VisitPermission permission in Enum.GetValues(typeof(VisitPermission)))
                        {
                            permissions.Add(new Permission { StudyId = visitDTO.StudyId, PermissionKey = (int)permission });
                        }

                        var roles = await _context.StudyRoles.Where(x => x.IsActive && !x.IsDeleted && x.StudyId == visitDTO.StudyId).ToListAsync();
                        foreach (VisitPermission permission in Enum.GetValues(typeof(VisitPermission)))
                        {
                            var rolPermissions = roles.Select(x => new Permission
                            {
                                StudyRoleId = x.Id,
                                PermissionKey = (int)permission,
                                StudyVisit = visit,
                                StudyId = x.StudyId,
                                TenantId = x.TenantId
                            }).ToList();
                            permissions.AddRange(rolPermissions);
                        }

                        visit.Permissions.AddRange(permissions);

                        await _context.StudyVisits.AddAsync(visit);

                        var result = await _context.SaveCoreContextAsync(visitDTO.UserId, DateTimeOffset.Now) > 0;

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
                    else if (visitDTO.Type == VisitStatu.page.ToString())
                    {
                        StudyVisitPage page = new StudyVisitPage
                        {
                            StudyVisitId = visitDTO.ParentId.Value,
                            Name = visitDTO.Name,
                            Order = visitDTO.Order
                        };

                        List<Permission> permissions = new List<Permission>();
                        foreach (VisitPermission permission in Enum.GetValues(typeof(VisitPermission)))
                        {
                            permissions.Add(new Permission { StudyId = visitDTO.StudyId, PermissionKey = (int)permission });
                        }

                        var roles = await _context.StudyRoles.Where(x => x.IsActive && !x.IsDeleted && x.StudyId == visitDTO.StudyId).ToListAsync();
                        foreach (VisitPermission permission in Enum.GetValues(typeof(VisitPermission)))
                        {
                            var rolPermissions = roles.Select(x => new Permission
                            {
                                StudyRoleId = x.Id,
                                PermissionKey = (int)permission,
                                StudyVisitPage = page,
                                StudyId = x.StudyId,
                                TenantId = x.TenantId
                            }).ToList();
                            permissions.AddRange(rolPermissions);
                        }

                        page.Permissions.AddRange(permissions);

                        await _context.StudyVisitPages.AddAsync(page);

                        var result = await _context.SaveCoreContextAsync(visitDTO.UserId, DateTimeOffset.Now) > 0;

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

                    return new ApiResponse<dynamic>
                    {
                        IsSuccess = false,
                        Message = "Unsuccessful"
                    };
                }
                else
                {
                    if (visitDTO.Type == VisitStatu.visit.ToString())
                    {
                        var visit = await _context.StudyVisits.FirstOrDefaultAsync(x => x.Id == visitDTO.Id && x.IsActive && !x.IsDeleted);

                        if (visit != null)
                        {
                            visit.Name = visitDTO.Name;
                            visit.VisitType = visitDTO.VisitType.Value;

                            var result = await _context.SaveCoreContextAsync(visitDTO.UserId, DateTimeOffset.Now) > 0;

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
                    else if (visitDTO.Type == VisitStatu.page.ToString())
                    {
                        var page = await _context.StudyVisitPages.FirstOrDefaultAsync(x => x.Id == visitDTO.Id && x.IsActive && !x.IsDeleted);

                        if (page != null)
                        {
                            page.Name = visitDTO.Name;

                            var result = await _context.SaveCoreContextAsync(visitDTO.UserId, DateTimeOffset.Now) > 0;

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
                    else if (visitDTO.Type == VisitStatu.module.ToString())
                    {
                        var page = await _context.StudyVisitPageModules.FirstOrDefaultAsync(x => x.Id == visitDTO.Id && x.IsActive && !x.IsDeleted);

                        if (page != null)
                        {
                            page.Name = visitDTO.Name;

                            var result = await _context.SaveCoreContextAsync(visitDTO.UserId, DateTimeOffset.Now) > 0;

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

                    return new ApiResponse<dynamic>
                    {
                        IsSuccess = false,
                        Message = "Unsuccessful"
                    };
                }
            }
            catch (Exception e)
            {
                return new ApiResponse<dynamic>
                {
                    IsSuccess = false,
                    Message = "Unsuccessful"
                };
            }
        }

        [HttpPost]
        public async Task<ApiResponse<dynamic>> DeleteVisits(VisitDTO visitDTO)
        {
            try
            {
                if (visitDTO.Type == VisitStatu.visit.ToString())
                {
                    var visit = await _context.StudyVisits
                        .Where(v => v.Id == visitDTO.Id && v.IsActive && !v.IsDeleted)
                        .Include(v => v.StudyVisitPages)
                            .ThenInclude(p => p.StudyVisitPageModules)
                        .Include(v => v.StudyVisitPages)
                            .ThenInclude(p => p.Permissions)
                        .Include(x => x.Permissions)
                        .FirstOrDefaultAsync();

                    if (visit != null)
                    {
                        _context.Permissions.RemoveRange(visit.Permissions);

                        _context.Permissions.RemoveRange(visit.StudyVisitPages.SelectMany(x => x.Permissions));

                        _context.Permissions.RemoveRange();

                        _context.StudyVisits.Remove(visit);

                        var result = await _context.SaveCoreContextAsync(visitDTO.UserId, DateTimeOffset.Now) > 0;

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
                else if (visitDTO.Type == VisitStatu.page.ToString())
                {
                    var page = await _context.StudyVisitPages
                       .Include(p => p.Permissions)
                       .Include(v => v.StudyVisitPageModules)
                       .FirstOrDefaultAsync(v => v.Id == visitDTO.Id && v.IsActive && !v.IsDeleted);

                    if (page != null)
                    {
                        _context.Permissions.RemoveRange(page.Permissions);

                        _context.StudyVisitPages.Remove(page);

                        var result = await _context.SaveCoreContextAsync(visitDTO.UserId, DateTimeOffset.Now) > 0;

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

                return new ApiResponse<dynamic>
                {
                    IsSuccess = false,
                    Message = "Unsuccessful"
                };

            }
            catch (Exception e)
            {
                return new ApiResponse<dynamic>
                {
                    IsSuccess = false,
                    Message = "Unsuccessful"
                };
            }
        }

        [HttpPost]
        public async Task<ApiResponse<dynamic>> SetVisitPageEPro(VisitDTO visitDTO)
        {
            var page = await _context.StudyVisitPages.FirstOrDefaultAsync(v => v.Id == visitDTO.Id && v.IsActive && !v.IsDeleted);

            if (page != null)
            {
                page.EPro = !page.EPro;

                var result = await _context.SaveCoreContextAsync(visitDTO.UserId, DateTimeOffset.Now) > 0;

                if (result)
                {
                    return new ApiResponse<dynamic>
                    {
                        IsSuccess = true,
                        Message = "Successful",
                        Values = new { value = page.EPro }
                    };
                }
            }

            return new ApiResponse<dynamic>
            {
                IsSuccess = false,
                Message = "Unsuccessful"
            };
        }

        [HttpGet]
        public async Task<List<PermissionModel>> GetVisitPagePermissionList(PermissionPage pageKey, Int64 studyId, Int64 id)
        {
            if (pageKey == PermissionPage.Visit)
            {
                return await _context.Permissions.Where(x => x.StudyId == studyId && x.StudyVisitId == id && x.StudyRoleId == null && x.IsActive && !x.IsDeleted).Select(x => new PermissionModel
                {
                    PermissionName = x.PermissionKey
                }).ToListAsync();
            }
            else if (pageKey == PermissionPage.Page)
            {
                return await _context.Permissions.Where(x => x.StudyId == studyId && x.StudyVisitPageId == id && x.StudyRoleId == null && x.IsActive && !x.IsDeleted).Select(x => new PermissionModel
                {
                    PermissionName = x.PermissionKey
                }).ToListAsync();
            }
            else
            {
                return new List<PermissionModel>();
            }
        }

        /// <summary>
        /// yetki ekler veya siler. isactive true ise yetki seçilidir. false veya hiç kaydı yoksa seçili değildir.
        /// </summary>
        /// <param name="dto"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<ApiResponse<dynamic>> SetVisitPagePermission(VisitPagePermissionDTO dto)
        {
            BaseDTO baseDTO = Request.Headers.GetBaseInformation();

            List<Permission> permissions = null;
            Expression<Func<Permission, bool>> query = null;

            //visit yetkileri için if, page yetkileri için else if
            if (dto.StudyVisitId != null && dto.StudyVisitId != 0)
            {
                query = x => x.StudyId == baseDTO.StudyId && x.StudyVisitId == dto.StudyVisitId && !x.IsDeleted;
            }
            else if (dto.StudyVisitPageId != null && dto.StudyVisitPageId != 0)
            {
                query = x => x.StudyId == baseDTO.StudyId && x.StudyVisitPageId == dto.StudyVisitPageId && !x.IsDeleted;
            }
            else
            {
                return new ApiResponse<dynamic>
                {
                    IsSuccess = false,
                    Message = "Unsuccessful"
                };
            }

            //yetkileri çekiyorum
            permissions = await _context.Permissions.Where(query).ToListAsync();

            //yeni seçtiğim yetkilerin daha önce veri tabanında kayıtlı olup olmadığını anlıyorum
            var expectData = dto.PermissionKeys.Except(permissions.Select(x => x.PermissionKey).ToList()).ToList();
            //kayıtlı olmayanlar varsa if giriyor
            if (expectData.Count > 0)
            {
                var newDatas = expectData.Select(x => new Permission { PermissionKey = x, StudyVisitId = dto.StudyVisitId, StudyVisitPageId = dto.StudyVisitPageId, StudyId = baseDTO.StudyId, TenantId = baseDTO.TenantId }).ToList();
                //seçili yetkileri isactive true şekilde ekliyorum.
                await _context.Permissions.AddRangeAsync(newDatas);
            }
            //daha önceden seçili şimdi ise onayı kaldırılmış olan yetkileri buluyorum
            var activeExpect = permissions.Where(x => x.IsActive && !dto.PermissionKeys.Contains(x.PermissionKey)).ToList();
            //o yetkileri false a çekiyorum
            activeExpect.ForEach(x => x.IsActive = false);
            //önceden onayı kaldırılmış ama şimdi onaylanmış yetkileri buluyorum
            var inactiveExpect = permissions.Where(x => !x.IsActive && dto.PermissionKeys.Contains(x.PermissionKey)).ToList();
            //onları true yapıyorum
            inactiveExpect.ForEach(x => x.IsActive = true);

            var result = await _context.SaveCoreContextAsync(baseDTO.UserId, DateTimeOffset.Now) > 0;

            if (result)
            {
                return new ApiResponse<dynamic>
                {
                    IsSuccess = true,
                    Message = "Successful"
                };
            }

            return new ApiResponse<dynamic>
            {
                IsSuccess = false,
                Message = "Unsuccessful"
            };
        }

        private List<StudyVisitPageModule> MapModuleDTOListToStudyVisitPageModuleList(List<ModuleDTO> moduleDTOList)
        {
            var studyVisitPageModuleList = new List<StudyVisitPageModule>();

            Int64 pageId = moduleDTOList.First().StudyVisitPageId;

            int lastOrder = _context.StudyVisitPageModules.Where(x => x.IsActive && !x.IsDeleted && x.StudyVisitPageId == pageId).Select(x => x.Order).DefaultIfEmpty().Max();

            if (lastOrder == 0)
            {
                lastOrder = 1;
            }
            else
            {
                lastOrder++;
            }

            foreach (var moduleDTO in moduleDTOList)
            {
                var studyVisitPageModule = new StudyVisitPageModule
                {
                    StudyVisitPageId = moduleDTO.StudyVisitPageId,
                    Name = moduleDTO.Name,
                    ReferenceKey = moduleDTO.ReferenceKey,
                    VersionKey = moduleDTO.VersionKey,
                    Order = lastOrder,
                };

                studyVisitPageModule.StudyVisitPageModuleElements = MapElementDTOListToStudyVisitPageModuleElementList(moduleDTO.StudyVisitPageModuleElements);

                studyVisitPageModuleList.Add(studyVisitPageModule);

                lastOrder++;
            }

            return studyVisitPageModuleList;
        }

        private List<StudyVisitPageModuleElement> MapElementDTOListToStudyVisitPageModuleElementList(List<ElementDTO> elementDTOList)
        {
            var studyVisitPageModuleElementList = new List<StudyVisitPageModuleElement>();

            foreach (var elementDTO in elementDTOList)
            {
                var elementDetailDTO = elementDTO.StudyVisitPageModuleElementDetails;

                var studyVisitPageModuleElement = new StudyVisitPageModuleElement
                {
                    ElementId = elementDTO.Id,
                    ElementType = elementDTO.ElementType,
                    ElementName = elementDTO.ElementName,
                    Title = elementDTO.Title,
                    IsTitleHidden = elementDTO.IsTitleHidden,
                    Order = elementDTO.Order,
                    Description = elementDTO.Description,
                    Width = elementDTO.Width,
                    IsHidden = elementDTO.IsHidden,
                    IsRequired = elementDTO.IsRequired,
                    IsDependent = elementDTO.IsDependent,
                    IsRelated = elementDTO.IsRelated,
                    IsReadonly = elementDTO.IsReadonly,
                    CanMissing = elementDTO.CanMissing,
                    ReferenceKey = new Guid(),
                    StudyVisitPageModuleElementDetail = new StudyVisitPageModuleElementDetail
                    {
                        ParentId = elementDetailDTO.ParentId,
                        RowIndex = elementDetailDTO.RowIndex,
                        ColunmIndex = elementDetailDTO.ColunmIndex,
                        CanQuery = elementDetailDTO.CanQuery,
                        CanSdv = elementDetailDTO.CanSdv,
                        CanRemoteSdv = elementDetailDTO.CanRemoteSdv,
                        CanComment = elementDetailDTO.CanComment,
                        CanDataEntry = elementDetailDTO.CanDataEntry,
                        ParentElementEProPageNumber = elementDetailDTO.ParentElementEProPageNumber,
                        MetaDataTags = elementDetailDTO.MetaDataTags,
                        EProPageNumber = elementDetailDTO.EProPageNumber,
                        ButtonText = elementDetailDTO.ButtonText,
                        DefaultValue = elementDetailDTO.DefaultValue,
                        Unit = elementDetailDTO.Unit,
                        LowerLimit = elementDetailDTO.LowerLimit,
                        UpperLimit = elementDetailDTO.UpperLimit,
                        Mask = elementDetailDTO.Mask,
                        Layout = elementDetailDTO.Layout,
                        StartDay = elementDetailDTO.StartDay,
                        EndDay = elementDetailDTO.EndDay,
                        StartMonth = elementDetailDTO.StartMonth,
                        EndMonth = elementDetailDTO.EndMonth,
                        StartYear = elementDetailDTO.StartYear,
                        EndYear = elementDetailDTO.EndYear,
                        AddTodayDate = elementDetailDTO.AddTodayDate,
                        ElementOptions = elementDetailDTO.ElementOptions,
                        LeftText = elementDetailDTO.LeftText,
                        RightText = elementDetailDTO.RightText,
                        IsInCalculation = elementDetailDTO.IsInCalculation,
                        MainJs = elementDetailDTO.MainJs,
                        RelationMainJs = elementDetailDTO.RelationMainJs,
                        RowCount = elementDetailDTO.RowCount,
                        ColumnCount = elementDetailDTO.ColumnCount,
                        DatagridAndTableProperties = elementDetailDTO.DatagridAndTableProperties,
                        ReferenceKey = new Guid(),
                    }
                };

                studyVisitPageModuleElementList.Add(studyVisitPageModuleElement);
            };

            return studyVisitPageModuleElementList;
        }

        private async Task<ApiResponse<dynamic>> SetStudyModule(List<ModuleDTO> dto)
        {
            var result = false;

            try
            {
                List<StudyVisitPageModule> moduleList = MapModuleDTOListToStudyVisitPageModuleList(dto);

                await _context.StudyVisitPageModules.AddRangeAsync(moduleList);

                BaseDTO baseDTO = Request.Headers.GetBaseInformation();

                result = await _context.SaveCoreContextAsync(baseDTO.UserId, DateTimeOffset.Now) > 0;

                if (result)
                {
                    IEnumerable<StudyVisitPageModuleElement> stdVstPgMdlElements = moduleList.SelectMany(x => x.StudyVisitPageModuleElements);

                    //set parentElementId
                    foreach (var item in stdVstPgMdlElements)
                    {
                        Int64? parentId = stdVstPgMdlElements.FirstOrDefault(x => x.ElementId == item.StudyVisitPageModuleElementDetail.ParentId)?.Id;
                        item.StudyVisitPageModuleElementDetail.ParentId = parentId;
                        _context.StudyVisitPageModuleElements.Update(item);
                    }

                    var elementDTOs = dto.SelectMany(x => x.StudyVisitPageModuleElements).ToList();

                    foreach (var item in stdVstPgMdlElements)
                    {
                        var elementDTO = elementDTOs.FirstOrDefault(x => x.Id == item.ElementId);

                        if (elementDTO.StudyVisitPageModuleCalculationElementDetails.Count > 0)
                        {
                            var studyVisitPageModuleCalculationElementDetailsList = new List<StudyVisitPageModuleCalculationElementDetail>();

                            foreach (var calculationElementDetailDTO in elementDTO.StudyVisitPageModuleCalculationElementDetails)
                            {
                                var studyVisitPageModuleCalculationElementDetail = new StudyVisitPageModuleCalculationElementDetail
                                {
                                    ReferenceKey = new Guid(),
                                    StudyVisitPageModuleId = item.StudyVisitPageModuleId,
                                    CalculationElementId = item.Id,
                                    TargetElementId = stdVstPgMdlElements.FirstOrDefault(x => x.ElementId == calculationElementDetailDTO.TargetElementId).Id,
                                    VariableName = calculationElementDetailDTO.VariableName
                                };

                                studyVisitPageModuleCalculationElementDetailsList.Add(studyVisitPageModuleCalculationElementDetail);
                            }

                            item.studyVisitPageModuleCalculationElementDetails = studyVisitPageModuleCalculationElementDetailsList;
                        }

                        if (elementDTO.StudyVisitPageModuleElementEvents.Count > 0)
                        {
                            var studyVisitPageModuleElementEventList = new List<StudyVisitPageModuleElementEvent>();

                            foreach (var moduleElementEventDTO in elementDTO.StudyVisitPageModuleElementEvents)
                            {
                                var studyVisitPageModuleElementEvent = new StudyVisitPageModuleElementEvent
                                {
                                    ReferenceKey = new Guid(),
                                    StudyVisitPageModuleId = item.StudyVisitPageModuleId,
                                    EventType = moduleElementEventDTO.EventType,
                                    ActionType = moduleElementEventDTO.ActionType,
                                    SourceElementId = stdVstPgMdlElements.FirstOrDefault(x => x.ElementId == moduleElementEventDTO.SourceElementId).Id,
                                    TargetElementId = item.Id,
                                    ValueCondition = moduleElementEventDTO.ValueCondition,
                                    ActionValue = moduleElementEventDTO.ActionValue,
                                    VariableName = moduleElementEventDTO.VariableName,
                                };

                                studyVisitPageModuleElementEventList.Add(studyVisitPageModuleElementEvent);
                            }

                            item.StudyVisitPageModuleElementEvents = studyVisitPageModuleElementEventList;
                        }

                        if (elementDTO.StudyVisitPageModuleElementValidationDetails.Count > 0)
                        {
                            var studyVisitPageModuleElementValidationDetailList = new List<StudyVisitPageModuleElementValidationDetail>();

                            foreach (var val in elementDTO.StudyVisitPageModuleElementValidationDetails)
                            {
                                var studyVisitPageModuleElementValidationDetail = new StudyVisitPageModuleElementValidationDetail
                                {
                                    ReferenceKey = new Guid(),
                                    StudyVisitPageModuleElementId = item.Id,
                                    ActionType = val.ValidationActionType,
                                    ValueCondition = val.ValidationCondition,
                                    Value = val.ValidationValue,
                                    Message = val.ValidationMessage
                                };

                                studyVisitPageModuleElementValidationDetailList.Add(studyVisitPageModuleElementValidationDetail);
                            }

                            item.StudyVisitPageModuleElementValidationDetails = studyVisitPageModuleElementValidationDetailList;
                        }
                    }

                    result = await _context.SaveCoreContextAsync(baseDTO.UserId, DateTimeOffset.Now) > 0;
                }

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
                    foreach (var item in moduleList)
                    {
                        //remove all added module, moduleElement and moduleElementDetails
                        foreach (var elm in item.StudyVisitPageModuleElements)
                        {
                            elm.IsActive = false;
                            elm.IsDeleted = true;
                            _context.StudyVisitPageModuleElements.Update(elm);

                            elm.StudyVisitPageModuleElementDetail.IsActive = false;
                            elm.StudyVisitPageModuleElementDetail.IsDeleted = true;
                            _context.StudyVisitPageModuleElementDetails.Update(elm.StudyVisitPageModuleElementDetail);

                            foreach (var dtl in elm.studyVisitPageModuleCalculationElementDetails)
                            {
                                dtl.IsActive = false;
                                dtl.IsDeleted = true;
                                _context.studyVisitPageModuleCalculationElementDetails.Update(dtl);
                            }

                            foreach (var evnt in elm.StudyVisitPageModuleElementEvents)
                            {
                                evnt.IsActive = false;
                                evnt.IsDeleted = true;
                                _context.StudyVisitPageModuleElementEvents.Update(evnt);
                            }
                        }

                        item.IsActive = false;
                        item.IsDeleted = true;
                        _context.StudyVisitPageModules.Update(item);
                    }

                    result = await _context.SaveCoreContextAsync(baseDTO.UserId, DateTimeOffset.Now) > 0;

                    return new ApiResponse<dynamic>
                    {
                        IsSuccess = false,
                        Message = "Unsuccessful"
                    };
                }
            }
            catch (Exception e)
            {
                return new ApiResponse<dynamic>
                {
                    IsSuccess = false,
                    Message = "Unsuccessful"
                };
            }
        }
        #endregion

        #region Module
        [HttpGet]
        public async Task<ModuleModel> GetStudyPageModule(Int64 id)
        {
            var model = new ModuleModel();

            var module = await _context.StudyVisitPageModules.FirstOrDefaultAsync(x => x.Id == id && x.IsActive && !x.IsDeleted);

            if (module != null)
            {
                model.Id = module.Id;
                model.Name = module.Name;
            }

            return model;
        }

        [HttpGet]
        public async Task<ApiResponse<dynamic>> GetModuleCollective(string moduleIds, Int64 pageId)
        {
            var a = new ApiResponse<dynamic>();
            string[] tenantIdsArray = moduleIds.Split(',');
            List<Int64> moduleIdsInt = new List<Int64>();
            foreach (string id in tenantIdsArray)
            {
                if (Int64.TryParse(id, out Int64 guid))
                {
                    moduleIdsInt.Add(guid);
                }
            }

            try
            {
                var result = await _context.Modules
                .Where(x => moduleIdsInt.Contains(x.Id) && x.IsActive && !x.IsDeleted)
                .Select(x => new ModuleDTO
                {
                    Name = x.Name,
                    StudyVisitPageId = pageId,
                    ReferenceKey = new Guid(),
                    VersionKey = 1,
                    Order = 1,
                    StudyVisitPageModuleElements = x.Elements.Where(q => q.IsActive && !q.IsDeleted).Select(e => new ElementDTO
                    {
                        Id = e.Id,
                        ElementType = e.ElementType,
                        ElementName = e.ElementName,
                        Title = e.Title,
                        IsTitleHidden = e.IsTitleHidden,
                        Order = e.Order,
                        Description = e.Description,
                        Width = e.Width,
                        IsHidden = e.IsHidden,
                        IsRequired = e.IsRequired,
                        IsDependent = e.IsDependent,
                        IsRelated = e.IsRelated,
                        IsReadonly = e.IsReadonly,
                        CanMissing = e.CanMissing,
                        StudyVisitPageModuleElementDetails = new ElementDetailDTO
                        {
                            ParentId = e.ElementDetail.ParentId,
                            RowIndex = e.ElementDetail.RowIndex,
                            ColunmIndex = e.ElementDetail.ColunmIndex,
                            MetaDataTags = e.ElementDetail.MetaDataTags,
                            ButtonText = e.ElementDetail.ButtonText,
                            DefaultValue = e.ElementDetail.DefaultValue,
                            Unit = e.ElementDetail.Unit,
                            LowerLimit = e.ElementDetail.LowerLimit,
                            UpperLimit = e.ElementDetail.UpperLimit,
                            Mask = e.ElementDetail.Mask,
                            Layout = e.ElementDetail.Layout,
                            RowCount = e.ElementDetail.RowCount,
                            ColumnCount = e.ElementDetail.ColumnCount,
                            DatagridAndTableProperties = e.ElementDetail.DatagridAndTableProperties,
                            StartDay = e.ElementDetail.StartDay,
                            EndDay = e.ElementDetail.EndDay,
                            StartMonth = e.ElementDetail.StartMonth,
                            EndMonth = e.ElementDetail.EndMonth,
                            EndYear = e.ElementDetail.EndYear,
                            StartYear = e.ElementDetail.StartYear,
                            AddTodayDate = e.ElementDetail.AddTodayDate,
                            ElementOptions = e.ElementDetail.ElementOptions,
                            LeftText = e.ElementDetail.LeftText,
                            RightText = e.ElementDetail.RightText,
                            IsInCalculation = e.ElementDetail.IsInCalculation,
                            MainJs = e.ElementDetail.MainJs,
                            RelationMainJs = e.ElementDetail.RelationMainJs
                        },
                        StudyVisitPageModuleCalculationElementDetails = e.CalculatationElementDetails.Where(cl => cl.IsActive && !cl.IsDeleted).Select(cl => new CalculatationElementDetailDTO
                        {
                            CalculationElementId = cl.CalculationElementId,
                            TargetElementId = cl.TargetElementId,
                            VariableName = cl.VariableName,
                        }).ToList(),
                        StudyVisitPageModuleElementEvents = e.ModuleElementEvents.Where(mee => mee.IsActive && !mee.IsDeleted).Select(mee => new ModuleElementEventDTO
                        {
                            EventType = mee.EventType,
                            ActionType = mee.ActionType,
                            SourceElementId = mee.SourceElementId,
                            TargetElementId = mee.TargetElementId,
                            ValueCondition = mee.ValueCondition,
                            ActionValue = mee.ActionValue,
                            VariableName = mee.VariableName,
                        }).ToList(),
                        StudyVisitPageModuleElementValidationDetails = e.ElementValidationDetails.Where(v => v.IsActive && !v.IsDeleted).Select(v => new ElementValidationDTO
                        {
                            ValidationActionType = v.ActionType,
                            ValidationCondition = v.ValueCondition,
                            ValidationMessage = v.Message,
                            ValidationValue = v.Value
                        }).ToList()
                    }).ToList()
                }).ToListAsync();

                var res = await SetStudyModule(result);

                return res;
            }
            catch (Exception e)
            {

            }

            return a;
        }

        [HttpGet]
        public async Task<List<ElementModel>> GetStudyModuleElementsWithChildren(Int64 studyModuleId)
        {
            var finalList = new List<ElementModel>();

            var result = await _context.StudyVisitPageModuleElements.Where(x => x.StudyVisitPageModuleId == studyModuleId && x.IsActive && !x.IsDeleted)
                .Include(x => x.StudyVisitPageModuleElementDetail)
                .Select(x => new ElementModel()
                {
                    Id = x.Id,
                    ParentId = x.StudyVisitPageModuleElementDetail.ParentId,
                    Title = x.Title,
                    Description = x.Description,
                    ElementName = x.ElementName,
                    ElementType = x.ElementType,
                    Order = x.Order,
                    IsDependent = x.IsDependent,
                    IsRelated = x.IsRelated,
                    IsRequired = x.IsRequired,
                    ElementOptions = x.StudyVisitPageModuleElementDetail.ElementOptions,
                    Width = x.Width,
                    Unit = x.StudyVisitPageModuleElementDetail.Unit,
                    Mask = x.StudyVisitPageModuleElementDetail.Mask,
                    LowerLimit = x.StudyVisitPageModuleElementDetail.LowerLimit,
                    UpperLimit = x.StudyVisitPageModuleElementDetail.UpperLimit,
                    Layout = x.StudyVisitPageModuleElementDetail.Layout,
                    DefaultValue = x.StudyVisitPageModuleElementDetail.DefaultValue,
                    AddTodayDate = x.StudyVisitPageModuleElementDetail.AddTodayDate,
                    MainJs = x.StudyVisitPageModuleElementDetail.MainJs,
                    StartDay = x.StudyVisitPageModuleElementDetail.StartDay,
                    EndDay = x.StudyVisitPageModuleElementDetail.EndDay,
                    StartMonth = x.StudyVisitPageModuleElementDetail.StartMonth,
                    EndMonth = x.StudyVisitPageModuleElementDetail.EndMonth,
                    StartYear = x.StudyVisitPageModuleElementDetail.StartYear,
                    EndYear = x.StudyVisitPageModuleElementDetail.EndYear,
                    LeftText = x.StudyVisitPageModuleElementDetail.LeftText,
                    RightText = x.StudyVisitPageModuleElementDetail.RightText,
                    ColumnCount = x.StudyVisitPageModuleElementDetail.ColumnCount,
                    RowCount = x.StudyVisitPageModuleElementDetail.RowCount,
                    DatagridAndTableProperties = x.StudyVisitPageModuleElementDetail.DatagridAndTableProperties,
                    ColumnIndex = x.StudyVisitPageModuleElementDetail.ColunmIndex,
                    RowIndex = x.StudyVisitPageModuleElementDetail.RowIndex,
                    AdverseEventType = x.StudyVisitPageModuleElementDetail.AdverseEventType,
                    TargetElementId = x.StudyVisitPageModuleElementDetail.TargetElementId,
                    ButtonText = x.StudyVisitPageModuleElementDetail.ButtonText
                }).OrderBy(x => x.Order).AsNoTracking().ToListAsync();

            foreach (var item in result)
            {
                if (item.ParentId == 0 || item.ParentId == null)
                    finalList.Add(item);
                else
                {
                    var parent = result.FirstOrDefault(x => x.Id == item.ParentId);

                    parent.ChildElements.Add(item);
                }
            }

            return finalList;
        }

        [HttpPost]
        public async Task<ApiResponse<dynamic>> CopyElement(ElementShortModel model)
        {
            var result = new ApiResponse<dynamic>();

            try
            {
                var stdVstPgMdlElmnt = await _context.StudyVisitPageModuleElements.Where(x => x.Id == model.Id && x.IsActive && !x.IsDeleted).FirstOrDefaultAsync();

                if (stdVstPgMdlElmnt != null)
                {
                    var name = stdVstPgMdlElmnt.ElementName + "_1";

                    for (; ; )
                    {
                        if (checkStudyElementName(stdVstPgMdlElmnt.StudyVisitPageModuleId, name).Result)
                            break;
                        else
                            name = name + "_1";
                    }

                    stdVstPgMdlElmnt.Id = 0;
                    stdVstPgMdlElmnt.ElementName = name;
                    stdVstPgMdlElmnt.Order = stdVstPgMdlElmnt.Order + 1;

                    var elementDetail = await _context.StudyVisitPageModuleElementDetails.Where(x => x.StudyVisitPageModuleElementId == model.Id && x.IsActive && !x.IsDeleted).FirstOrDefaultAsync();
                    elementDetail.Id = 0;

                    _context.Add(stdVstPgMdlElmnt);
                    _context.Add(elementDetail);

                    result.IsSuccess = await _context.SaveCoreContextAsync(model.UserId, DateTimeOffset.Now) > 0;

                    elementDetail.StudyVisitPageModuleElementId = stdVstPgMdlElmnt.Id;

                    _context.Update(elementDetail);

                    if (stdVstPgMdlElmnt.ElementType == ElementType.Calculated)
                    {
                        var calcdtls = await _context.studyVisitPageModuleCalculationElementDetails.Where(x => x.CalculationElementId == model.Id).ToListAsync();

                        foreach (var cal in calcdtls)
                        {
                            cal.CalculationElementId = stdVstPgMdlElmnt.Id;
                            _context.Add(cal);
                        }

                        result.IsSuccess = await _context.SaveCoreContextAsync(model.UserId, DateTimeOffset.Now) > 0;
                    }

                    if (stdVstPgMdlElmnt.ElementType == ElementType.DataGrid || stdVstPgMdlElmnt.ElementType == ElementType.Table)
                    {
                        var childrenDtils = await _context.StudyVisitPageModuleElementDetails.Where(x => x.ParentId == model.Id).ToListAsync();
                        var chldrnIds = childrenDtils.Select(x => x.StudyVisitPageModuleElementId).ToList();
                        var children = await _context.StudyVisitPageModuleElements.Where(x => chldrnIds.Contains(x.Id)).ToListAsync();

                        foreach (var child in children)
                        {
                            var nm = child.ElementName + "_1";

                            for (; ; )
                            {
                                if (checkStudyElementName(child.StudyVisitPageModuleId, nm).Result)
                                    break;
                                else
                                    nm = nm + "_1";
                            }

                            var chDtl = childrenDtils.FirstOrDefault(x => x.StudyVisitPageModuleElementId == child.Id);
                            chDtl.Id = 0;

                            child.Id = 0;
                            child.ElementName = nm;
                            child.Order = child.Order + 1;

                            _context.Add(child);
                            _context.Add(chDtl);

                            result.IsSuccess = await _context.SaveCoreContextAsync(model.UserId, DateTimeOffset.Now) > 0;

                            chDtl.Id = child.Id;
                            chDtl.ParentId = stdVstPgMdlElmnt.Id;

                            _context.Update(chDtl);

                            result.IsSuccess = await _context.SaveCoreContextAsync(model.UserId, DateTimeOffset.Now) > 0;
                        }
                    }

                    var moduleElements = await _context.StudyVisitPageModuleElements.Where(x => x.StudyVisitPageModuleId == stdVstPgMdlElmnt.StudyVisitPageModuleId && x.IsActive && !x.IsDeleted).ToListAsync();

                    foreach (var item in moduleElements)
                    {
                        if (item.Order >= stdVstPgMdlElmnt.Order && item.Id != stdVstPgMdlElmnt.Id)
                        {
                            item.Order = (item.Order + 1);
                            _context.Update(item);
                        }
                    }

                    result.IsSuccess = await _context.SaveCoreContextAsync(model.UserId, DateTimeOffset.Now) > 0;
                    result.Message = result.IsSuccess ? "Successful" : "Error";
                }
                else
                {
                    result.IsSuccess = false;
                    result.Message = "Error";
                }
            }
            catch (Exception ex)
            {

            }

            return result;
        }

        [HttpPost]
        public async Task<ApiResponse<dynamic>> DeleteElement(ElementShortModel model)
        {
            var result = new ApiResponse<dynamic>();
            var element = await _context.StudyVisitPageModuleElements.Where(x => x.Id == model.Id && x.IsActive && !x.IsDeleted).FirstOrDefaultAsync();
            var elementDetail = await _context.StudyVisitPageModuleElementDetails.Where(x => x.StudyVisitPageModuleElementId == model.Id && x.IsActive && !x.IsDeleted).FirstOrDefaultAsync();

            if (element != null)
            {
                if (elementDetail.IsInCalculation && element.ElementType != ElementType.Calculated)
                {
                    result.IsSuccess = false;
                    result.Message = "This element used in a calculation element formul. Please remove it first from calculation element.";

                    return result;
                }

                var moduleEvent = _context.StudyVisitPageModuleElementEvents.FirstOrDefault(x => x.SourceElementId == model.Id && x.IsActive && !x.IsDeleted);

                if (moduleEvent != null)
                {
                    result.IsSuccess = false;
                    result.Message = "This element used as relation or dependent in another element. Please remove it first and try again.";

                    return result;
                }

                if (element.ElementType == ElementType.DataGrid || element.ElementType == ElementType.Table)
                {
                    var childrenDtils = await _context.StudyVisitPageModuleElementDetails.Where(x => x.ParentId == model.Id).ToListAsync();
                    var chldrnIds = childrenDtils.Select(x => x.StudyVisitPageModuleElementId).ToList();

                    foreach (var item in childrenDtils)
                    {
                        item.IsActive = false;
                        item.IsDeleted = true;

                        _context.StudyVisitPageModuleElementDetails.Update(item);
                    }

                    var children = await _context.StudyVisitPageModuleElements.Where(x => chldrnIds.Contains(x.Id)).ToListAsync();

                    foreach (var item in children)
                    {
                        item.IsActive = false;
                        item.IsDeleted = true;

                        _context.StudyVisitPageModuleElements.Update(item);
                    }
                }

                if (element.ElementType == ElementType.Calculated)
                {
                    var childrenDtils = await _context.studyVisitPageModuleCalculationElementDetails.Where(x => x.CalculationElementId == model.Id && x.IsActive && !x.IsDeleted).ToListAsync();
                    var targetElmIds = childrenDtils.Select(x => x.TargetElementId).ToList();

                    var anotherCalcDtils = await _context.studyVisitPageModuleCalculationElementDetails.Where(x => targetElmIds.Contains(x.TargetElementId) && x.IsActive && !x.IsDeleted).GroupBy(x => x.TargetElementId).ToListAsync();

                    var chngIds = new List<Int64>();

                    foreach (var item in anotherCalcDtils)
                    {
                        if (item.ToList().Count == 1)
                            chngIds.Add(item.FirstOrDefault().TargetElementId);
                    }

                    var elmDtils = _context.StudyVisitPageModuleElementDetails.Where(x => targetElmIds.Contains(x.StudyVisitPageModuleElementId) && x.IsActive && !x.IsDeleted).ToList();

                    foreach (var item in elmDtils)
                    {
                        if (chngIds.Contains(item.StudyVisitPageModuleElementId))
                        {
                            item.IsInCalculation = false;

                            _context.StudyVisitPageModuleElementDetails.Update(item);
                        }
                    }

                    foreach (var item in childrenDtils)
                    {
                        item.IsActive = false;
                        item.IsDeleted = true;

                        _context.studyVisitPageModuleCalculationElementDetails.Update(item);
                    }
                }

                element.IsDeleted = true;
                element.IsActive = false;
                elementDetail.IsDeleted = true;
                elementDetail.IsActive = false;

                _context.Update(element);
                _context.Update(elementDetail);

                result.IsSuccess = await _context.SaveCoreContextAsync(model.UserId, DateTimeOffset.Now) > 0;
                result.Message = result.IsSuccess ? "Successful" : "Error";
            }
            else
            {
                result.IsSuccess = false;
                result.Message = "Error";
            }

            return result;
        }

        [HttpGet]
        public async Task<List<ElementModel>> GetVisitPageModuleAllElements(Int64 visitPageModuleId)
        {
            var result = await _context.StudyVisitPageModuleElements.Where(x => x.StudyVisitPageModuleId == visitPageModuleId && x.IsActive && !x.IsDeleted)
                .Include(x => x.StudyVisitPageModuleElementDetail)
                .Select(x => new ElementModel()
                {
                    Id = x.Id,
                    Title = x.Title,
                    Description = x.Description,
                    ElementName = x.ElementName,
                    ElementType = x.ElementType,
                    Order = x.Order,
                    IsDependent = x.IsDependent,
                    IsRelated = x.IsRelated,
                    IsRequired = x.IsRequired,
                    ElementOptions = x.StudyVisitPageModuleElementDetail.ElementOptions,
                    Width = x.Width,
                    Unit = x.StudyVisitPageModuleElementDetail.Unit,
                    Mask = x.StudyVisitPageModuleElementDetail.Mask,
                    LowerLimit = x.StudyVisitPageModuleElementDetail.LowerLimit,
                    UpperLimit = x.StudyVisitPageModuleElementDetail.UpperLimit,
                    Layout = x.StudyVisitPageModuleElementDetail.Layout,
                    DefaultValue = x.StudyVisitPageModuleElementDetail.DefaultValue,
                    AddTodayDate = x.StudyVisitPageModuleElementDetail.AddTodayDate,
                    MainJs = x.StudyVisitPageModuleElementDetail.MainJs,
                    StartDay = x.StudyVisitPageModuleElementDetail.StartDay,
                    EndDay = x.StudyVisitPageModuleElementDetail.EndDay,
                    StartMonth = x.StudyVisitPageModuleElementDetail.StartMonth,
                    EndMonth = x.StudyVisitPageModuleElementDetail.EndMonth,
                    StartYear = x.StudyVisitPageModuleElementDetail.StartYear,
                    EndYear = x.StudyVisitPageModuleElementDetail.EndYear,
                    LeftText = x.StudyVisitPageModuleElementDetail.LeftText,
                    RightText = x.StudyVisitPageModuleElementDetail.RightText,
                    TargetElementId = x.StudyVisitPageModuleElementDetail.TargetElementId,
                    ButtonText = x.StudyVisitPageModuleElementDetail.ButtonText
                }).OrderBy(x => x.Order).AsNoTracking().ToListAsync();

            return result;
        }

        [HttpGet]
        public async Task<ElementModel> GetVisitPageModuleElementData(Int64 id)
        {
            var result = new ElementModel();

            try
            {
                result = await _context.StudyVisitPageModuleElements.Where(x => x.Id == id && x.IsActive && !x.IsDeleted)
                .Include(x => x.StudyVisitPageModuleElementDetail)
                .Select(x => new ElementModel()
                {
                    Id = x.Id,
                    ParentId = x.StudyVisitPageModuleElementDetail.ParentId,
                    Title = x.Title,
                    ElementName = x.ElementName,
                    ElementType = x.ElementType,
                    Description = x.Description,
                    IsRequired = x.IsRequired,
                    IsHidden = x.IsHidden,
                    CanMissing = x.CanMissing,
                    Width = x.Width,
                    Unit = x.StudyVisitPageModuleElementDetail.Unit,
                    Mask = x.StudyVisitPageModuleElementDetail.Mask,
                    LowerLimit = x.StudyVisitPageModuleElementDetail.LowerLimit,
                    UpperLimit = x.StudyVisitPageModuleElementDetail.UpperLimit,
                    Layout = x.StudyVisitPageModuleElementDetail.Layout,
                    IsDependent = x.IsDependent,
                    IsRelated = x.IsRelated,
                    RelationMainJs = x.StudyVisitPageModuleElementDetail.RelationMainJs,
                    ElementOptions = x.StudyVisitPageModuleElementDetail.ElementOptions,
                    DefaultValue = x.StudyVisitPageModuleElementDetail.DefaultValue,
                    AddTodayDate = x.StudyVisitPageModuleElementDetail.AddTodayDate,
                    MainJs = x.StudyVisitPageModuleElementDetail.MainJs,
                    StartDay = x.StudyVisitPageModuleElementDetail.StartDay,
                    EndDay = x.StudyVisitPageModuleElementDetail.EndDay,
                    StartMonth = x.StudyVisitPageModuleElementDetail.StartMonth,
                    EndMonth = x.StudyVisitPageModuleElementDetail.EndMonth,
                    StartYear = x.StudyVisitPageModuleElementDetail.StartYear,
                    EndYear = x.StudyVisitPageModuleElementDetail.EndYear,
                    LeftText = x.StudyVisitPageModuleElementDetail.LeftText,
                    RightText = x.StudyVisitPageModuleElementDetail.RightText,
                    ColumnCount = x.StudyVisitPageModuleElementDetail.ColumnCount,
                    RowCount = x.StudyVisitPageModuleElementDetail.RowCount,
                    DatagridAndTableProperties = x.StudyVisitPageModuleElementDetail.DatagridAndTableProperties,
                    ColumnIndex = x.StudyVisitPageModuleElementDetail.ColunmIndex,
                    RowIndex = x.StudyVisitPageModuleElementDetail.RowIndex,
                    AdverseEventType = x.StudyVisitPageModuleElementDetail.AdverseEventType,
                    TargetElementId = x.StudyVisitPageModuleElementDetail.TargetElementId,
                    ButtonText = x.StudyVisitPageModuleElementDetail.ButtonText
                }).AsNoTracking().FirstOrDefaultAsync();

                var events = new List<StudyVisitPageModuleElementEvent>();

                if (result.IsRelated || result.IsDependent)
                {
                    events = await _context.StudyVisitPageModuleElementEvents.Where(x => x.TargetElementId == id && x.IsActive && !x.IsDeleted).ToListAsync();
                }

                if (result.IsDependent)
                {
                    var dep = events.FirstOrDefault(x => x.EventType == EventType.Dependency && x.IsActive && !x.IsDeleted);

                    if (dep != null)
                    {
                        result.DependentSourceFieldId = dep.SourceElementId;
                        result.DependentTargetFieldId = dep.TargetElementId;
                        result.DependentCondition = (int)dep.ValueCondition;
                        result.DependentAction = (int)dep.ActionType;
                        result.DependentFieldValue = dep.ActionValue;
                    }
                }

                if (result.IsRelated)
                {
                    var rels = events.Where(x => x.EventType == EventType.Relation && x.IsActive && !x.IsDeleted).ToList();
                    var relSrcIds = rels.Select(x => x.SourceElementId).ToList();
                    var srcs = await _context.StudyVisitPageModuleElements.Where(x => relSrcIds.Contains(x.Id)).ToArrayAsync();
                    var relStr = "[";

                    foreach (var item in rels)
                    {
                        var src = srcs.FirstOrDefault(x => x.Id == item.SourceElementId);

                        relStr += "{\"relationFieldsSelectedGroup\":{\"label\":\"" + src.ElementName + " - " + StringExtensionsHelper.GetEnumDescription(src.ElementType) + "\",\"value\":" + item.SourceElementId + "},\"variableName\":\"" + item.VariableName + "\"}";

                        if (item == rels.LastOrDefault())
                            relStr += "]";
                        else
                            relStr += ",";
                    }

                    result.RelationSourceInputs = relStr;
                }

                if (result.ElementType == ElementType.Calculated)
                {
                    var cals = await _context.studyVisitPageModuleCalculationElementDetails.Where(x => x.CalculationElementId == result.Id && x.IsActive && !x.IsDeleted).ToListAsync();
                    var calSrcIds = cals.Select(x => x.TargetElementId).ToList();
                    var srcs = await _context.StudyVisitPageModuleElements.Where(x => calSrcIds.Contains(x.Id)).ToArrayAsync();

                    var calStr = "[";

                    foreach (var item in cals)
                    {
                        var src = srcs.FirstOrDefault(x => x.Id == item.TargetElementId);

                        calStr += "{\"elementFieldSelectedGroup\":{\"label\":\"" + src.ElementName + " - " + StringExtensionsHelper.GetEnumDescription(src.ElementType) + "\",\"value\":" + item.TargetElementId + "},\"variableName\":\"" + item.VariableName + "\"}";

                        if (item == cals.LastOrDefault())
                            calStr += "]";
                        else
                            calStr += ",";
                    }

                    result.CalculationSourceInputs = calStr;
                }
            }
            catch (Exception ex)
            {

            }

            return result;
        }

        [HttpPost]
        public async Task<ApiResponse<dynamic>> SaveVisitPageModuleContent(ElementModel model)
        {
            var result = new ApiResponse<dynamic>();
            var calcList = new List<CalculationModel>();

            var stdVstPgMdlElement = await _context.StudyVisitPageModuleElements.Where(x => x.Id == model.Id && x.IsActive && !x.IsDeleted).FirstOrDefaultAsync();

            if (model.ElementType == ElementType.Calculated)
            {
                if (model.CalculationSourceInputs == null || model.CalculationSourceInputs == "[]" || string.IsNullOrEmpty(model.CalculationSourceInputs))
                {
                    result.IsSuccess = false;
                    result.Message = "Error in calculation elements selection";

                    return result;
                }

                try
                {
                    calcList = JsonSerializer.Deserialize<List<CalculationModel>>(model.CalculationSourceInputs);
                }
                catch (Exception ex)
                {
                    result.IsSuccess = false;
                    result.Message = "Error in calculation elements selection";

                    return result;
                }
            }

            var moduleElements = _context.StudyVisitPageModuleElements.Where(x => x.StudyVisitPageModuleId == model.ModuleId && x.IsActive && !x.IsDeleted).ToListAsync().Result;

            if (stdVstPgMdlElement == null)
            {
                if (!checkStudyElementName(model.ModuleId, model.ElementName, moduleElements).Result)
                {
                    result.IsSuccess = false;
                    result.Message = "Duplicate element name";

                    return result;
                }

                var stdVstPgMdlElmnt = new StudyVisitPageModuleElement();

                try
                {
                    var moduleElementMaxOrder = moduleElements.Count() > 0 ? moduleElements.Select(x => x.Order).Max() : 1;

                    stdVstPgMdlElmnt = new StudyVisitPageModuleElement()
                    {
                        Title = model.Title,
                        ElementName = model.ElementName.TrimStart().TrimEnd(),
                        Description = model.Description,
                        CanMissing = model.CanMissing,
                        ElementType = model.ElementType,
                        IsDependent = model.IsDependent,
                        IsHidden = model.IsHidden,
                        IsReadonly = model.IsReadonly,
                        IsRequired = model.IsRequired,
                        IsRelated = model.IsRelated,
                        IsTitleHidden = model.IsTitleHidden,
                        Width = model.ElementType != ElementType.Hidden ? model.Width : GridLayout.ColMd3,
                        StudyVisitPageModuleId = model.ModuleId,
                        TenantId = model.TenantId,
                        Order = model.ParentId == 0 ? moduleElementMaxOrder + 1 : 0,
                        //CreatedAt = DateTimeOffset.Now,
                        //AddedById = userId,
                    };

                    _context.StudyVisitPageModuleElements.Add(stdVstPgMdlElmnt);
                    result.IsSuccess = await _context.SaveCoreContextAsync(model.UserId, DateTimeOffset.Now) > 0;

                    if (result.IsSuccess)
                    {
                        var stdVstPgMdlElementDetail = new StudyVisitPageModuleElementDetail()
                        {
                            StudyVisitPageModuleElementId = stdVstPgMdlElmnt.Id,
                            TenantId = model.TenantId,
                            ParentId = model.ParentId,
                            Unit = model.Unit,
                            Mask = model.Mask,
                            LowerLimit = model.LowerLimit,
                            UpperLimit = model.UpperLimit,
                            Layout = model.Layout,
                            ElementOptions = model.ElementOptions,
                            MetaDataTags = model.ElementName,
                            DefaultValue = model.DefaultValue,
                            AddTodayDate = model.AddTodayDate,
                            MainJs = model.MainJs,
                            RelationMainJs = model.RelationMainJs,
                            StartDay = model.ElementType != ElementType.DateOption ? 0 : model.StartDay,
                            EndDay = model.ElementType != ElementType.DateOption ? 0 : model.EndDay,
                            StartMonth = model.ElementType != ElementType.DateOption ? 0 : model.StartMonth,
                            EndMonth = model.ElementType != ElementType.DateOption ? 0 : model.EndMonth,
                            StartYear = model.ElementType != ElementType.DateOption ? 0 : model.StartYear,
                            EndYear = model.ElementType != ElementType.DateOption ? 0 : model.EndYear,
                            IsInCalculation = model.ElementType == ElementType.Calculated,
                            LeftText = model.LeftText,
                            RightText = model.RightText,
                            RowCount = model.RowCount,
                            ColumnCount = model.ColumnCount,
                            DatagridAndTableProperties = model.DatagridAndTableProperties,
                            RowIndex = model.ParentId == 0 ? 0 : model.RowIndex,
                            ColunmIndex = model.ColumnIndex,
                            AdverseEventType = model.AdverseEventType,
                            TargetElementId = model.TargetElementId,
                            ButtonText = model.ButtonText
                        };

                        _context.StudyVisitPageModuleElementDetails.Add(stdVstPgMdlElementDetail);
                        result.IsSuccess = await _context.SaveCoreContextAsync(model.UserId, DateTimeOffset.Now) > 0;

                        if (!result.IsSuccess)
                        {
                            stdVstPgMdlElmnt.IsActive = false;
                            stdVstPgMdlElmnt.IsDeleted = true;
                            _context.StudyVisitPageModuleElements.Update(stdVstPgMdlElmnt);
                            var rslt = await _context.SaveCoreContextAsync(model.UserId, DateTimeOffset.Now) > 0;

                            result.IsSuccess = false;
                            result.Message = "Operation failed. Please try again.";

                            return result;
                        }

                        if (model.IsDependent)
                        {
                            var elementEvent = new StudyVisitPageModuleElementEvent()
                            {
                                SourceElementId = model.DependentTargetFieldId,
                                TargetElementId = stdVstPgMdlElmnt.Id,
                                StudyVisitPageModuleId = model.ModuleId,
                                TenantId = model.TenantId,
                                EventType = EventType.Dependency,
                                ValueCondition = (ActionCondition)model.DependentCondition,
                                ActionType = (ActionType)model.DependentAction,
                                ActionValue = model.DependentFieldValue,
                            };

                            _context.StudyVisitPageModuleElementEvents.Add(elementEvent);
                            result.IsSuccess = await _context.SaveCoreContextAsync(model.UserId, DateTimeOffset.Now) > 0;
                        }

                        if (model.ElementType == ElementType.Calculated)
                        {
                            var calcElmIds = calcList.Select(x => x.elementFieldSelectedGroup.value).ToList();
                            var elementInCalList = await _context.StudyVisitPageModuleElementDetails.Where(x => calcElmIds.Contains(x.StudyVisitPageModuleElementId)).ToListAsync();

                            foreach (var item in calcList)
                            {
                                var calcDtil = new StudyVisitPageModuleCalculationElementDetail()
                                {
                                    TenantId = model.TenantId,
                                    StudyVisitPageModuleId = model.ModuleId,
                                    CalculationElementId = stdVstPgMdlElmnt.Id,
                                    TargetElementId = item.elementFieldSelectedGroup.value,
                                    VariableName = item.variableName
                                };

                                _context.studyVisitPageModuleCalculationElementDetails.Add(calcDtil);
                            }

                            foreach (var item in elementInCalList)
                            {
                                item.IsInCalculation = true;

                                _context.StudyVisitPageModuleElementDetails.Update(item);
                            }

                            result.IsSuccess = await _context.SaveCoreContextAsync(model.UserId, DateTimeOffset.Now) > 0;
                        }

                        if (model.IsRelated)
                        {
                            var relList = JsonSerializer.Deserialize<List<RelationModel>>(model.RelationSourceInputs);

                            foreach (var item in relList)
                            {
                                var elementEvent = new StudyVisitPageModuleElementEvent()
                                {
                                    StudyVisitPageModuleId = model.ModuleId,
                                    SourceElementId = item.relationFieldsSelectedGroup.value,
                                    TargetElementId = stdVstPgMdlElmnt.Id,
                                    TenantId = model.TenantId,
                                    EventType = EventType.Relation,
                                    VariableName = item.variableName
                                };

                                _context.StudyVisitPageModuleElementEvents.Add(elementEvent);
                            }

                            var isSuccess = await _context.SaveCoreContextAsync(model.UserId, DateTimeOffset.Now) > 0;

                            if (!isSuccess)
                            {
                                stdVstPgMdlElement.IsRelated = false;
                                stdVstPgMdlElementDetail.RelationMainJs = "";

                                _context.StudyVisitPageModuleElements.Update(stdVstPgMdlElement);
                                _context.StudyVisitPageModuleElementDetails.Update(stdVstPgMdlElementDetail);
                                result.IsSuccess = await _context.SaveCoreContextAsync(model.UserId, DateTimeOffset.Now) > 0;
                            }
                        }

                        if (!result.IsSuccess)//if dependent or calculation didn't save
                        {
                            stdVstPgMdlElmnt.IsActive = false;
                            stdVstPgMdlElmnt.IsDeleted = true;
                            _context.StudyVisitPageModuleElements.Update(stdVstPgMdlElmnt);

                            stdVstPgMdlElementDetail.IsActive = false;
                            stdVstPgMdlElementDetail.IsDeleted = true;
                            _context.StudyVisitPageModuleElementDetails.Update(stdVstPgMdlElementDetail);

                            var aa = await _context.SaveCoreContextAsync(model.UserId, DateTimeOffset.Now) > 0;

                            result.IsSuccess = false;
                            result.Message = "Operation failed. Please try again to save dependents or calculation part.";
                        }
                    }
                    else
                    {
                        result.Message = "Error";
                    }
                }
                catch (Exception ex)
                {
                    var elmDtl = await _context.StudyVisitPageModuleElementDetails.FirstOrDefaultAsync(x => x.StudyVisitPageModuleElementId == stdVstPgMdlElmnt.Id && x.IsActive && !x.IsDeleted);

                    if (elmDtl == null)
                    {
                        stdVstPgMdlElmnt.IsActive = false;
                        stdVstPgMdlElmnt.IsDeleted = true;
                        _context.StudyVisitPageModuleElements.Update(stdVstPgMdlElmnt);
                        var aa = await _context.SaveCoreContextAsync(model.UserId, DateTimeOffset.Now) > 0;

                        result.IsSuccess = false;
                        result.Message = "Operation failed. Please try again.";
                    }
                }
            }
            else
            {
                if (stdVstPgMdlElement.ElementName.TrimStart().TrimEnd() != model.ElementName.TrimStart().TrimEnd())
                {
                    if (!checkStudyElementName(model.ModuleId, model.ElementName, moduleElements).Result)
                    {
                        result.IsSuccess = false;
                        result.Message = "Duplicate element name";

                        return result;
                    }
                }

                stdVstPgMdlElement.Title = model.Title;
                stdVstPgMdlElement.ElementName = model.ElementName.TrimStart().TrimEnd();
                stdVstPgMdlElement.Description = model.Description;
                stdVstPgMdlElement.CanMissing = model.CanMissing;
                stdVstPgMdlElement.ElementType = model.ElementType;
                stdVstPgMdlElement.IsDependent = model.IsDependent;
                stdVstPgMdlElement.IsHidden = model.IsHidden;
                stdVstPgMdlElement.IsReadonly = model.IsReadonly;
                stdVstPgMdlElement.IsRequired = model.IsRequired;
                stdVstPgMdlElement.IsRelated = model.IsRelated;
                stdVstPgMdlElement.IsTitleHidden = model.IsTitleHidden;
                stdVstPgMdlElement.Width = model.Width;
                stdVstPgMdlElement.StudyVisitPageModuleId = model.ModuleId;
                stdVstPgMdlElement.UpdatedAt = DateTimeOffset.Now;
                stdVstPgMdlElement.UpdatedById = model.UserId;

                _context.Update(stdVstPgMdlElement);

                var stdVstPgMdlElementDetail = await _context.StudyVisitPageModuleElementDetails.FirstOrDefaultAsync(x => x.StudyVisitPageModuleElementId == stdVstPgMdlElement.Id && x.IsActive && !x.IsDeleted);

                stdVstPgMdlElementDetail.Unit = model.Unit;
                stdVstPgMdlElementDetail.Mask = model.Mask;
                stdVstPgMdlElementDetail.LowerLimit = model.LowerLimit;
                stdVstPgMdlElementDetail.UpperLimit = model.UpperLimit;
                stdVstPgMdlElementDetail.Layout = model.Layout;
                stdVstPgMdlElementDetail.ElementOptions = model.ElementOptions;
                stdVstPgMdlElementDetail.DefaultValue = model.DefaultValue;
                stdVstPgMdlElementDetail.AddTodayDate = model.AddTodayDate;
                stdVstPgMdlElementDetail.MainJs = model.MainJs;
                stdVstPgMdlElementDetail.RelationMainJs = model.RelationMainJs;
                stdVstPgMdlElementDetail.StartDay = model.StartDay;
                stdVstPgMdlElementDetail.EndDay = model.EndDay;
                stdVstPgMdlElementDetail.StartMonth = model.StartMonth;
                stdVstPgMdlElementDetail.EndMonth = model.EndMonth;
                stdVstPgMdlElementDetail.StartYear = model.StartYear;
                stdVstPgMdlElementDetail.EndYear = model.EndYear;
                stdVstPgMdlElementDetail.LeftText = model.LeftText;
                stdVstPgMdlElementDetail.RightText = model.RightText;
                stdVstPgMdlElementDetail.RowCount = model.RowCount;
                stdVstPgMdlElementDetail.ColumnCount = model.ColumnCount;
                stdVstPgMdlElementDetail.AdverseEventType = model.AdverseEventType;
                stdVstPgMdlElementDetail.DatagridAndTableProperties = model.DatagridAndTableProperties;
                stdVstPgMdlElementDetail.TargetElementId = model.TargetElementId;
                stdVstPgMdlElementDetail.ButtonText = model.ButtonText;
                stdVstPgMdlElement.UpdatedAt = DateTimeOffset.Now;
                stdVstPgMdlElement.UpdatedById = model.UserId;

                _context.Update(stdVstPgMdlElementDetail);
                result.IsSuccess = await _context.SaveCoreContextAsync(model.UserId, DateTimeOffset.Now) > 0;

                if (model.IsDependent)
                {
                    var dep = await _context.StudyVisitPageModuleElementEvents.FirstOrDefaultAsync(x => x.TargetElementId == model.Id && x.IsActive && !x.IsDeleted);

                    if (dep == null)
                    {
                        var elementEvent = new StudyVisitPageModuleElementEvent()
                        {
                            SourceElementId = model.DependentSourceFieldId,
                            TargetElementId = stdVstPgMdlElement.Id,
                            EventType = EventType.Dependency,
                            ValueCondition = (ActionCondition)model.DependentCondition,
                            ActionType = (ActionType)model.DependentAction,
                            ActionValue = model.DependentFieldValue,
                            StudyVisitPageModuleId = stdVstPgMdlElement.StudyVisitPageModuleId
                        };

                        _context.StudyVisitPageModuleElementEvents.Add(elementEvent);
                    }
                    else
                    {
                        dep.SourceElementId = model.DependentSourceFieldId;
                        dep.TargetElementId = stdVstPgMdlElement.Id;
                        dep.ValueCondition = (ActionCondition)model.DependentCondition;
                        dep.ActionType = (ActionType)model.DependentAction;
                        dep.ActionValue = model.DependentFieldValue;
                        dep.StudyVisitPageModuleId = stdVstPgMdlElement.StudyVisitPageModuleId;

                        _context.StudyVisitPageModuleElementEvents.Update(dep);
                    }

                    result.IsSuccess = await _context.SaveCoreContextAsync(model.UserId, DateTimeOffset.Now) > 0;
                }

                if (model.ElementType == ElementType.Calculated)
                {
                    var existCalDtil = await _context.studyVisitPageModuleCalculationElementDetails.Where(x => x.CalculationElementId == stdVstPgMdlElement.Id && x.IsActive && !x.IsDeleted).ToListAsync();
                    var existCalElmIds = existCalDtil.Select(x => x.TargetElementId).ToList();
                    var elementInExistCalList = await _context.StudyVisitPageModuleElementDetails.Where(x => existCalElmIds.Contains(x.StudyVisitPageModuleElementId) && x.IsActive && !x.IsDeleted).ToListAsync();
                    var calcElmIds = calcList.Select(x => x.elementFieldSelectedGroup.value).ToList();

                    //update updated variable list
                    foreach (var item in existCalDtil)
                    {
                        var c = calcList.FirstOrDefault(x => x.variableName == item.VariableName);

                        if (c != null && c.elementFieldSelectedGroup.value != item.TargetElementId)
                        {
                            item.TargetElementId = c.elementFieldSelectedGroup.value;
                            _context.studyVisitPageModuleCalculationElementDetails.Update(item);
                        }
                    }

                    result.IsSuccess = await _context.SaveCoreContextAsync(model.UserId, DateTimeOffset.Now) > 0;

                    //change elementDetail first
                    foreach (var item in elementInExistCalList)
                    {
                        item.IsInCalculation = false;
                        _context.StudyVisitPageModuleElementDetails.Update(item);
                    }

                    var elementInCalList = await _context.StudyVisitPageModuleElementDetails.Where(x => calcElmIds.Contains(x.StudyVisitPageModuleElementId) && x.IsActive && !x.IsDeleted).ToListAsync();

                    //then change new elementDetail flags
                    foreach (var item in elementInCalList)
                    {
                        item.IsInCalculation = true;
                        _context.StudyVisitPageModuleElementDetails.Update(item);
                    }

                    existCalElmIds = existCalDtil.Select(x => x.TargetElementId).ToList();//ids updated

                    //add new calcElementDetails
                    foreach (var item in calcList)
                    {
                        if (!existCalElmIds.Contains(item.elementFieldSelectedGroup.value))
                        {
                            var calcDtil = new StudyVisitPageModuleCalculationElementDetail()
                            {
                                TenantId = model.TenantId,
                                StudyVisitPageModuleId = model.ModuleId,
                                CalculationElementId = stdVstPgMdlElement.Id,
                                TargetElementId = item.elementFieldSelectedGroup.value,
                            };

                            _context.studyVisitPageModuleCalculationElementDetails.Add(calcDtil);
                        }
                    }

                    //remove deleted items from calc
                    foreach (var item in existCalDtil)
                    {
                        if (!calcElmIds.Contains(item.TargetElementId))
                        {
                            item.IsActive = false;
                            item.IsDeleted = true;

                            _context.Update(item);
                        }
                    }

                    _context.StudyVisitPageModuleElementDetails.Update(stdVstPgMdlElementDetail);

                    result.IsSuccess = await _context.SaveCoreContextAsync(model.UserId, DateTimeOffset.Now) > 0;
                }

                if (model.IsRelated)
                {
                    var rels = await _context.StudyVisitPageModuleElementEvents.Where(x => x.TargetElementId == model.Id && x.IsActive && !x.IsDeleted).ToListAsync();

                    var relList = JsonSerializer.Deserialize<List<RelationModel>>(model.RelationSourceInputs);
                    var relElmIds = relList.Select(x => x.relationFieldsSelectedGroup.value).ToList();

                    foreach (var item in rels)
                    {
                        var r = relList.FirstOrDefault(x => x.variableName == item.VariableName);

                        if (r != null && r.relationFieldsSelectedGroup.value != item.TargetElementId)
                        {
                            item.TargetElementId = r.relationFieldsSelectedGroup.value;
                            _context.StudyVisitPageModuleElementEvents.Update(item);
                        }
                    }

                    var relIds = rels.Select(x => x.SourceElementId).ToList();

                    //first add unadded rows to evet
                    foreach (var item in relList)
                    {
                        if (!relIds.Contains(item.relationFieldsSelectedGroup.value))
                        {
                            var elementEvent = new StudyVisitPageModuleElementEvent()
                            {
                                StudyVisitPageModuleId = model.ModuleId,
                                SourceElementId = item.relationFieldsSelectedGroup.value,
                                TargetElementId = model.Id,
                                TenantId = model.TenantId,
                                EventType = EventType.Relation,
                            };

                            _context.StudyVisitPageModuleElementEvents.Add(elementEvent);
                        }
                    }

                    //then remove deleted rows
                    foreach (var item in rels)
                    {
                        var delRel = relList.FirstOrDefault(x => x.relationFieldsSelectedGroup.value == item.SourceElementId);

                        if (delRel == null)
                        {
                            item.IsActive = false;
                            item.IsDeleted = true;

                            _context.StudyVisitPageModuleElementEvents.Add(item);
                        }
                    }

                    _context.StudyVisitPageModuleElementDetails.Update(stdVstPgMdlElementDetail);

                    var isSuccess = await _context.SaveCoreContextAsync(model.UserId, DateTimeOffset.Now) > 0;

                    if (!isSuccess)
                    {
                        stdVstPgMdlElement.IsRelated = false;
                        stdVstPgMdlElementDetail.RelationMainJs = "";

                        _context.StudyVisitPageModuleElements.Update(stdVstPgMdlElement);
                        _context.StudyVisitPageModuleElementDetails.Update(stdVstPgMdlElementDetail);
                        result.IsSuccess = await _context.SaveCoreContextAsync(model.UserId, DateTimeOffset.Now) > 0;
                    }
                }
            }

            return result;
        }

        private async Task<bool> checkStudyElementName(Int64 moduleId, string elementName, List<StudyVisitPageModuleElement> moduleElements = null)
        {
            if (moduleElements == null)
                moduleElements = _context.StudyVisitPageModuleElements.Where(x => x.StudyVisitPageModuleId == moduleId && x.IsActive && !x.IsDeleted).ToListAsync().Result;

            if (moduleElements.FirstOrDefault(x => x.ElementName == elementName.TrimStart().TrimEnd()) != null)
                return false;
            else
                return true;
        }

        [HttpPost]
        public async Task<ApiResponse<dynamic>> SetVisitRanking(List<VisitDTO> dto)
        {
            try
            {
                BaseDTO baseDTO = Request.Headers.GetBaseInformation();

                var visitList = dto.Where(data => data.Type == VisitStatu.visit.ToString()).ToList();
                var pageList = dto.Where(data => data.Type == VisitStatu.page.ToString()).ToList();
                var moduleList = dto.Where(data => data.Type == VisitStatu.module.ToString()).ToList();

                var visitsToUpdate = await _context.StudyVisits.Where(v => visitList.Select(d => d.Id).Contains(v.Id)).ToListAsync();
                var pagesToUpdate = await _context.StudyVisitPages.Where(p => pageList.Select(d => d.Id).Contains(p.Id)).ToListAsync();
                var modulesToUpdate = await _context.StudyVisitPageModules.Where(m => moduleList.Select(d => d.Id).Contains(m.Id)).ToListAsync();

                visitsToUpdate.ForEach(visit =>
                {
                    var newData = visitList.FirstOrDefault(d => d.Id == visit.Id);
                    if (newData != null)
                    {
                        visit.Order = newData.Order;
                    }
                });

                pagesToUpdate.ForEach(page =>
                {
                    var newData = pageList.FirstOrDefault(d => d.Id == page.Id);
                    if (newData != null)
                    {
                        if (page.StudyVisitId != newData.ParentId && newData.ParentId != null) page.StudyVisitId = newData.ParentId.Value;
                        if (page.Order != newData.Order) page.Order = newData.Order;
                    }
                });

                modulesToUpdate.ForEach(module =>
                {
                    var newData = moduleList.FirstOrDefault(d => d.Id == module.Id);
                    if (newData != null)
                    {
                        if (module.StudyVisitPageId != newData.ParentId && newData.ParentId != null) module.StudyVisitPageId = newData.ParentId.Value;
                        if (module.Order != module.Order) module.Order = newData.Order;
                    }
                });

                var result = await _context.SaveCoreContextAsync(baseDTO.UserId, DateTimeOffset.Now) > 0;

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
            catch (Exception)
            {
                return new ApiResponse<dynamic>
                {
                    IsSuccess = false,
                    Message = "An unexpected error occurred."
                };
            }
        }

        [HttpGet]
        public async Task<List<VisitModel>> GetVisitPageList(Int64 studyId)
        {
            return await _context.StudyVisits.Where(x => x.IsActive && !x.IsDeleted && x.StudyId == studyId).Include(x => x.StudyVisitPages).ThenInclude(x => x.StudyVisitPageModules).Select(x => new VisitModel
            {
                Id = x.Id,
                Name = x.Name,
                VisitType = (VisitType)x.VisitType,
                Order = x.Order,
                CreatedAt = x.CreatedAt,
                UpdatedAt = x.UpdatedAt,
                Children = x.StudyVisitPages.Where(page => page.IsActive && !page.IsDeleted).Select(page => new VisitModel
                {
                    Id = page.Id,
                    Name = page.Name,
                    Order = page.Order,
                    CreatedAt = x.CreatedAt,
                    UpdatedAt = page.UpdatedAt,
                    EPro = page.EPro,
                    Children = page.StudyVisitPageModules.Where(module => module.IsActive && !module.IsDeleted).Select(module => new VisitModel
                    {
                        Id = module.Id,
                        Name = module.Name,
                        Order = module.Order,
                        CreatedAt = x.CreatedAt,
                        UpdatedAt = module.UpdatedAt
                    }).ToList()
                }).ToList()
            }).ToListAsync();
        }

        [HttpGet]
        public async Task<VisitCollectionModel> GetVisitCollectionInfo(Int64 elementId)
        {
            var result = await _context.StudyVisitPageModuleElements.Where(x => x.Id == elementId).Select(x => new VisitCollectionModel
            {
                StudyVisitElementId = x.Id,
                StudyVisitModuleId = x.StudyVisitPageModuleId,
                StudyVisitPageId = x.StudyVisitPageModule.StudyVisitPageId,
                StudyVisitId = x.StudyVisitPageModule.StudyVisitPage.StudyVisitId,
            }).FirstOrDefaultAsync();

            return result;
        }
        #endregion

    }
}
