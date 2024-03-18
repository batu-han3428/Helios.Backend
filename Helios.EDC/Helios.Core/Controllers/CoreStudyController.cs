using AutoMapper;
using Helios.Common.DTO;
using Helios.Common.Enums;
using Helios.Common.Model;
using Helios.Core.Contexts;
using Helios.Core.Domains.Entities;
using Helios.Core.helpers;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using System.Linq.Expressions;
using Helios.Common.Helpers.Api;

namespace Helios.Core.Controllers
{
    [ApiController]
    [Route("[controller]/[action]")]
    public class CoreStudyController : Controller
    {
        private CoreContext _context;
        private readonly IMapper _mapper;

        public CoreStudyController(CoreContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
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
                ExternalMails = x.ExternalMails != "" ? JsonConvert.DeserializeObject<List<string>>(x.ExternalMails) : new List<string>(),
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
                    ExternalMails = emailTemplateDTO.ExternalMails.Count > 0 ? JsonConvert.SerializeObject(emailTemplateDTO.ExternalMails) : "",
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
                data.ExternalMails = emailTemplateDTO.ExternalMails.Count > 0 ? JsonConvert.SerializeObject(emailTemplateDTO.ExternalMails) : "";


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

                        _context.Permissions.RemoveRange(visit.StudyVisitPages.SelectMany(x=>x.Permissions));

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
            }else if (pageKey == PermissionPage.Page)
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
                studyVisitPageModule.studyVisitPageModuleCalculationElementDetails = MapCalculatationElementDetailDTOListToStudyVisitPageModuleCalculationElementDetailsList(moduleDTO.studyVisitPageModuleCalculationElementDetails);
                studyVisitPageModule.StudyVisitPageModuleElementEvent = MapModuleElementEventDTOListToStudyVisitPageModuleElementEventList(moduleDTO.StudyVisitPageModuleElementEvent);

                studyVisitPageModuleList.Add(studyVisitPageModule);

                lastOrder++;
            }

            return studyVisitPageModuleList;
        }

        private StudyVisitPageModuleElementDetail MapElementDTOListToStudyVisitPageModuleElementDetailsList(ElementDetailDTO? elementDetailDTO)
        {
            if (elementDetailDTO == null)
            {
                return null;
            }
            return new StudyVisitPageModuleElementDetail
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
                TargetElementId = elementDetailDTO.TargetElementId,
                LeftText = elementDetailDTO.LeftText,
                RightText = elementDetailDTO.RightText,
                IsInCalculation = elementDetailDTO.IsInCalculation,
                MainJs = elementDetailDTO.MainJs,
                RelationMainJs = elementDetailDTO.RelationMainJs,
                RowCount = elementDetailDTO.RowCount,
                ColumnCount = elementDetailDTO.ColumnCount,
                DatagridAndTableProperties = elementDetailDTO.DatagridAndTableProperties,
            };
        }

        private List<StudyVisitPageModuleElement> MapElementDTOListToStudyVisitPageModuleElementList(List<ElementDTO> elementDTOList)
        {
            var studyVisitPageModuleElementList = new List<StudyVisitPageModuleElement>();

            foreach (var elementDTO in elementDTOList)
            {
                var studyVisitPageModuleElement = new StudyVisitPageModuleElement
                {
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
                    CanMissing = elementDTO.CanMissing
                };

                studyVisitPageModuleElement.StudyVisitPageModuleElementDetails = MapElementDTOListToStudyVisitPageModuleElementDetailsList(elementDTO.StudyVisitPageModuleElementDetails);
                studyVisitPageModuleElementList.Add(studyVisitPageModuleElement);
            };

            return studyVisitPageModuleElementList;
        }

        private List<StudyVisitPageModuleCalculationElementDetails> MapCalculatationElementDetailDTOListToStudyVisitPageModuleCalculationElementDetailsList(List<CalculatationElementDetailDTO> calculationElementDetailDTOList)
        {
            var studyVisitPageModuleCalculationElementDetailsList = new List<StudyVisitPageModuleCalculationElementDetails>();

            foreach (var calculationElementDetailDTO in calculationElementDetailDTOList)
            {
                var studyVisitPageModuleCalculationElementDetail = new StudyVisitPageModuleCalculationElementDetails
                {
                    CalculationElementId = calculationElementDetailDTO.CalculationElementId,
                    TargetElementId = calculationElementDetailDTO.TargetElementId,
                    VariableName = calculationElementDetailDTO.VariableName
                };

                studyVisitPageModuleCalculationElementDetailsList.Add(studyVisitPageModuleCalculationElementDetail);
            }

            return studyVisitPageModuleCalculationElementDetailsList;
        }

        private List<StudyVisitPageModuleElementEvent> MapModuleElementEventDTOListToStudyVisitPageModuleElementEventList(List<ModuleElementEventDTO> moduleElementEventDTOList)
        {
            var studyVisitPageModuleElementEventList = new List<StudyVisitPageModuleElementEvent>();

            foreach (var moduleElementEventDTO in moduleElementEventDTOList)
            {
                var studyVisitPageModuleElementEvent = new StudyVisitPageModuleElementEvent
                {

                    EventType = moduleElementEventDTO.EventType,
                    ActionType = moduleElementEventDTO.ActionType,
                    SourceElementId = moduleElementEventDTO.SourceElementId,
                    TargetElementId = moduleElementEventDTO.TargetElementId,
                    ValueCondition = moduleElementEventDTO.ValueCondition,
                    ActionValue = moduleElementEventDTO.ActionValue,
                    VariableName = moduleElementEventDTO.VariableName,
                };

                studyVisitPageModuleElementEventList.Add(studyVisitPageModuleElementEvent);
            }

            return studyVisitPageModuleElementEventList;
        }

        [HttpPost]
        public async Task<ApiResponse<dynamic>> SetStudyModule(List<ModuleDTO> dto)
        {
            try
            {
                List<StudyVisitPageModule> moduleList = MapModuleDTOListToStudyVisitPageModuleList(dto);

                await _context.StudyVisitPageModules.AddRangeAsync(moduleList);

                BaseDTO baseDTO = Request.Headers.GetBaseInformation();

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
                    Message = "Unsuccessful"
                };
            }
        }

        [HttpGet]
        public async Task<List<ElementModel>> GetStudyModuleElementsWithChildren(Int64 moduleId)
        {
            var finalList = new List<ElementModel>();

            var result = await _context.StudyVisitPageModuleElements.Where(x => x.StudyVisitPageModuleId == moduleId && x.IsActive && !x.IsDeleted)
                .Include(x => x.StudyVisitPageModuleElementDetails)
                .Select(x => new ElementModel()
                {
                    Id = x.Id,
                    ParentId = x.StudyVisitPageModuleElementDetails.ParentId,
                    Title = x.Title,
                    Description = x.Description,
                    ElementName = x.ElementName,
                    ElementType = x.ElementType,
                    Order = x.Order,
                    IsDependent = x.IsDependent,
                    IsRelated = x.IsRelated,
                    IsRequired = x.IsRequired,
                    ElementOptions = x.StudyVisitPageModuleElementDetails.ElementOptions,
                    Width = x.Width,
                    Unit = x.StudyVisitPageModuleElementDetails.Unit,
                    Mask = x.StudyVisitPageModuleElementDetails.Mask,
                    LowerLimit = x.StudyVisitPageModuleElementDetails.LowerLimit,
                    UpperLimit = x.StudyVisitPageModuleElementDetails.UpperLimit,
                    Layout = x.StudyVisitPageModuleElementDetails.Layout,
                    DefaultValue = x.StudyVisitPageModuleElementDetails.DefaultValue,
                    AddTodayDate = x.StudyVisitPageModuleElementDetails.AddTodayDate,
                    MainJs = x.StudyVisitPageModuleElementDetails.MainJs,
                    StartDay = x.StudyVisitPageModuleElementDetails.StartDay,
                    EndDay = x.StudyVisitPageModuleElementDetails.EndDay,
                    StartMonth = x.StudyVisitPageModuleElementDetails.StartMonth,
                    EndMonth = x.StudyVisitPageModuleElementDetails.EndMonth,
                    StartYear = x.StudyVisitPageModuleElementDetails.StartYear,
                    EndYear = x.StudyVisitPageModuleElementDetails.EndYear,
                    LeftText = x.StudyVisitPageModuleElementDetails.LeftText,
                    RightText = x.StudyVisitPageModuleElementDetails.RightText,
                    ColumnCount = x.StudyVisitPageModuleElementDetails.ColumnCount,
                    RowCount = x.StudyVisitPageModuleElementDetails.RowCount,
                    DatagridAndTableProperties = x.StudyVisitPageModuleElementDetails.DatagridAndTableProperties,
                    ColumnIndex = x.StudyVisitPageModuleElementDetails.ColunmIndex,
                    RowIndex = x.StudyVisitPageModuleElementDetails.RowIndex,
                }).OrderBy(x => x.Order).AsNoTracking().ToListAsync();

            foreach (var item in result)
            {
                if (item.ParentId == 0)
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
        public async Task<ApiResponse<dynamic>> CopyStudyElement(ElementShortModel model)
        {
            var result = new ApiResponse<dynamic>();

            var element = await _context.StudyVisitPageModuleElements.Where(x => x.Id == model.Id && x.IsActive && !x.IsDeleted).FirstOrDefaultAsync();

            if (element != null)
            {
                var name = element.ElementName + "_1";

                for (; ; )
                {
                    if (checkStudyElementName(element.StudyVisitPageModuleId, name).Result)
                        break;
                    else
                        name = name + "_1";
                }

                element.Id = 0;
                element.ElementName = name;
                element.Order = element.Order + 1;

                var elementDetail = await _context.ElementDetails.Where(x => x.ElementId == model.Id && x.IsActive && !x.IsDeleted).FirstOrDefaultAsync();
                elementDetail.Id = 0;

                _context.Add(element);
                _context.Add(elementDetail);

                result.IsSuccess = await _context.SaveCoreContextAsync(model.UserId, DateTimeOffset.Now) > 0;

                elementDetail.ElementId = element.Id;
                element.StudyVisitPageModuleElementDetails.Id = elementDetail.Id;

                _context.Update(element);
                _context.Update(elementDetail);

                if (element.ElementType == ElementType.Calculated)
                {
                    var calcdtls = await _context.CalculatationElementDetails.Where(x => x.CalculationElementId == model.Id).ToListAsync();

                    foreach (var cal in calcdtls)
                    {
                        cal.CalculationElementId = element.Id;
                        _context.Add(cal);
                    }

                    result.IsSuccess = await _context.SaveCoreContextAsync(model.UserId, DateTimeOffset.Now) > 0;
                }

                if (element.ElementType == ElementType.DataGrid || element.ElementType == ElementType.Table)
                {
                    var childrenDtils = await _context.ElementDetails.Where(x => x.ParentId == model.Id).ToListAsync();
                    var chldrnIds = childrenDtils.Select(x => x.ElementId).ToList();
                    var children = await _context.Elements.Where(x => chldrnIds.Contains(x.Id)).ToListAsync();

                    foreach (var child in children)
                    {
                        var nm = child.ElementName + "_1";

                        for (; ; )
                        {
                            if (checkStudyElementName(child.ModuleId, nm).Result)
                                break;
                            else
                                nm = nm + "_1";
                        }

                        var chDtl = childrenDtils.FirstOrDefault(x => x.ElementId == child.Id);
                        chDtl.Id = 0;

                        child.Id = 0;
                        child.ElementName = nm;
                        child.Order = child.Order + 1;

                        _context.Add(child);
                        _context.Add(chDtl);

                        result.IsSuccess = await _context.SaveCoreContextAsync(model.UserId, DateTimeOffset.Now) > 0;

                        child.ElementDetailId = chDtl.Id;
                        chDtl.Id = child.Id;
                        chDtl.ParentId = element.Id;

                        result.IsSuccess = await _context.SaveCoreContextAsync(model.UserId, DateTimeOffset.Now) > 0;
                    }
                }

                var moduleElements = await _context.Elements.Where(x => x.ModuleId == element.StudyVisitPageModuleId && x.IsActive && !x.IsDeleted).ToListAsync();

                foreach (var item in moduleElements)
                {
                    if (item.Order >= element.Order && item.Id != element.Id)
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
        public async Task<ApiResponse<dynamic>> DeleteStudyElement(ElementShortModel model)
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

                var moduleEvent = _context.ModuleElementEvents.FirstOrDefault(x => x.SourceElementId == model.Id && x.IsActive && !x.IsDeleted);

                if (moduleEvent != null)
                {
                    result.IsSuccess = false;
                    result.Message = "This element used as relation or dependent in another element. Please remove it first and try again.";

                    return result;
                }

                if (element.ElementType == ElementType.DataGrid || element.ElementType == ElementType.Table)
                {
                    var childrenDtils = await _context.ElementDetails.Where(x => x.ParentId == model.Id).ToListAsync();
                    var chldrnIds = childrenDtils.Select(x => x.ElementId).ToList();

                    foreach (var item in childrenDtils)
                    {
                        item.IsActive = false;
                        item.IsDeleted = true;

                        _context.ElementDetails.Update(item);
                    }

                    var children = await _context.Elements.Where(x => chldrnIds.Contains(x.Id)).ToListAsync();

                    foreach (var item in children)
                    {
                        item.IsActive = false;
                        item.IsDeleted = true;

                        _context.Elements.Update(item);
                    }
                }

                if (element.ElementType == ElementType.Calculated)
                {
                    var childrenDtils = await _context.CalculatationElementDetails.Where(x => x.CalculationElementId == model.Id && x.IsActive && !x.IsDeleted).ToListAsync();
                    var targetElmIds = childrenDtils.Select(x => x.TargetElementId).ToList();

                    var anotherCalcDtils = await _context.CalculatationElementDetails.Where(x => targetElmIds.Contains(x.TargetElementId) && x.IsActive && !x.IsDeleted).GroupBy(x => x.TargetElementId).ToListAsync();

                    var chngIds = new List<Int64>();

                    foreach (var item in anotherCalcDtils)
                    {
                        if (item.ToList().Count == 1)
                            chngIds.Add(item.FirstOrDefault().TargetElementId);
                    }

                    var elmDtils = _context.ElementDetails.Where(x => targetElmIds.Contains(x.ElementId) && x.IsActive && !x.IsDeleted).ToList();

                    foreach (var item in elmDtils)
                    {
                        if (chngIds.Contains(item.ElementId))
                        {
                            item.IsInCalculation = false;

                            _context.ElementDetails.Update(item);
                        }
                    }

                    foreach (var item in childrenDtils)
                    {
                        item.IsActive = false;
                        item.IsDeleted = true;

                        _context.CalculatationElementDetails.Update(item);
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
        public async Task<List<ElementModel>> GetStudyModuleAllElements(Int64 moduleId)
        {
            var result = await _context.StudyVisitPageModuleElements.Where(x => x.StudyVisitPageModuleId == moduleId && x.IsActive && !x.IsDeleted)
                .Include(x => x.StudyVisitPageModuleElementDetails)
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
                    ElementOptions = x.StudyVisitPageModuleElementDetails.ElementOptions,
                    Width = x.Width,
                    Unit = x.StudyVisitPageModuleElementDetails.Unit,
                    Mask = x.StudyVisitPageModuleElementDetails.Mask,
                    LowerLimit = x.StudyVisitPageModuleElementDetails.LowerLimit,
                    UpperLimit = x.StudyVisitPageModuleElementDetails.UpperLimit,
                    Layout = x.StudyVisitPageModuleElementDetails.Layout,
                    DefaultValue = x.StudyVisitPageModuleElementDetails.DefaultValue,
                    AddTodayDate = x.StudyVisitPageModuleElementDetails.AddTodayDate,
                    MainJs = x.StudyVisitPageModuleElementDetails.MainJs,
                    StartDay = x.StudyVisitPageModuleElementDetails.StartDay,
                    EndDay = x.StudyVisitPageModuleElementDetails.EndDay,
                    StartMonth = x.StudyVisitPageModuleElementDetails.StartMonth,
                    EndMonth = x.StudyVisitPageModuleElementDetails.EndMonth,
                    StartYear = x.StudyVisitPageModuleElementDetails.StartYear,
                    EndYear = x.StudyVisitPageModuleElementDetails.EndYear,
                    LeftText = x.StudyVisitPageModuleElementDetails.LeftText,
                    RightText = x.StudyVisitPageModuleElementDetails.RightText
                }).OrderBy(x => x.Order).AsNoTracking().ToListAsync();

            return result;
        }

        [HttpGet]
        public async Task<ElementModel> GetStudyElementData(Int64 id)
        {
            var result = new ElementModel();

            result = await _context.StudyVisitPageModuleElements.Where(x => x.Id == id && x.IsActive && !x.IsDeleted)
            .Include(x => x.StudyVisitPageModuleElementDetails)
            .Select(x => new ElementModel()
            {
                Id = x.Id,
                ParentId = x.StudyVisitPageModuleElementDetails.ParentId,
                Title = x.Title,
                ElementName = x.ElementName,
                ElementType = x.ElementType,
                Description = x.Description,
                IsRequired = x.IsRequired,
                IsHidden = x.IsHidden,
                CanMissing = x.CanMissing,
                Width = x.Width,
                Unit = x.StudyVisitPageModuleElementDetails.Unit,
                Mask = x.StudyVisitPageModuleElementDetails.Mask,
                LowerLimit = x.StudyVisitPageModuleElementDetails.LowerLimit,
                UpperLimit = x.StudyVisitPageModuleElementDetails.UpperLimit,
                Layout = x.StudyVisitPageModuleElementDetails.Layout,
                IsDependent = x.IsDependent,
                IsRelated = x.IsRelated,
                RelationMainJs = x.StudyVisitPageModuleElementDetails.RelationMainJs,
                ElementOptions = x.StudyVisitPageModuleElementDetails.ElementOptions,
                DefaultValue = x.StudyVisitPageModuleElementDetails.DefaultValue,
                AddTodayDate = x.StudyVisitPageModuleElementDetails.AddTodayDate,
                MainJs = x.StudyVisitPageModuleElementDetails.MainJs,
                StartDay = x.StudyVisitPageModuleElementDetails.StartDay,
                EndDay = x.StudyVisitPageModuleElementDetails.EndDay,
                StartMonth = x.StudyVisitPageModuleElementDetails.StartMonth,
                EndMonth = x.StudyVisitPageModuleElementDetails.EndMonth,
                StartYear = x.StudyVisitPageModuleElementDetails.StartYear,
                EndYear = x.StudyVisitPageModuleElementDetails.EndYear,
                LeftText = x.StudyVisitPageModuleElementDetails.LeftText,
                RightText = x.StudyVisitPageModuleElementDetails.RightText,
                ColumnCount = x.StudyVisitPageModuleElementDetails.ColumnCount,
                RowCount = x.StudyVisitPageModuleElementDetails.RowCount,
                DatagridAndTableProperties = x.StudyVisitPageModuleElementDetails.DatagridAndTableProperties,
                ColumnIndex = x.StudyVisitPageModuleElementDetails.ColunmIndex,
                RowIndex = x.StudyVisitPageModuleElementDetails.RowIndex,
            }).AsNoTracking().FirstOrDefaultAsync();

            if (result.IsDependent)
            {
                var dep = await _context.ModuleElementEvents.FirstOrDefaultAsync(x => x.TargetElementId == id && x.IsActive && !x.IsDeleted);

                if (dep != null)
                {
                    result.DependentSourceFieldId = dep.SourceElementId;
                    result.DependentTargetFieldId = dep.TargetElementId;
                    result.DependentCondition = (int)dep.ValueCondition;
                    result.DependentAction = (int)dep.ActionType;
                    result.DependentFieldValue = dep.ActionValue;
                }
            }

            return result;
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
                        if(page.StudyVisitId != newData.ParentId && newData.ParentId != null) page.StudyVisitId = newData.ParentId.Value;
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
        #endregion
    }
}
