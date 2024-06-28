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
using System.Text.Json;
using Helios.Core.Services.Interfaces;
using System.Xml.Linq;

namespace Helios.Core.Controllers
{
    [ApiController]
    [Route("[controller]/[action]")]
    public class CoreStudyController : Controller
    {
        private CoreContext _context;
        private IStudyService _studyService;

        public CoreStudyController(CoreContext context, IStudyService studyService)
        {
            _context = context;
            _studyService = studyService;
        }

        #region Study
        [HttpGet]
        public async Task<List<StudyDTO>> GetStudyList(bool isLock, Int64 tenantId)
        {
            var result = await _context.Studies.Where(x =>x.TenantId==tenantId &&  !x.IsDemo && x.IsActive && !x.IsDeleted && x.IsLock == isLock).Select(x => new StudyDTO()
            {
                Id = x.Id,
                EquivalentStudyId = x.EquivalentStudyId,
                StudyName = x.StudyName,
                ProtocolCode = x.ProtocolCode,
                AskSubjectInitial = x.AskSubjectInitial,
                StudyLink = x.StudyLink,
                CreatedAt = x.CreatedAt,
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
            try
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
                    Guid _refKey = Guid.NewGuid();

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

                    var activeResearch = new Study()
                    {
                        TenantId = studyModel.TenantId,
                        IsDemo = false,
                        StudyName = studyModel.StudyName,
                        ProtocolCode = studyModel.ProtocolCode,
                        AskSubjectInitial = studyModel.AskSubjectInitial,
                        StudyLink = studyModel.StudyLink,
                        StudyType = studyModel.DoubleDataEntry ? (int)StudyType.DoubleEntry : (int)StudyType.Normal,
                        SubDescription = studyModel.SubDescription,
                        Description = studyModel.Description,
                        ReasonForChange = studyModel.ReasonForChange,
                        StudyLanguage = studyModel.StudyLanguage,
                        VersionKey = _versionKey,
                        ReferenceKey = _refKey,
                        IsActive = true,
                        IsDeleted = false,
                        SubjectNumberDigitCount = studyModel.SubjectNumberDigist
                    };
                    var demoResearch = new Study
                    {
                        TenantId = studyModel.TenantId,
                        IsDemo = true,
                        StudyName = "DEMO-" + studyModel.StudyName,
                        ProtocolCode = "DEMO-" + studyModel.ProtocolCode,
                        StudyLink = "DEMO-" + studyModel.StudyLink,
                        AskSubjectInitial = studyModel.AskSubjectInitial,
                        StudyType = studyModel.DoubleDataEntry ? (int)StudyType.DoubleEntry : (int)StudyType.Normal,
                        SubDescription = studyModel.SubDescription,
                        Description = studyModel.Description,
                        ReasonForChange = studyModel.ReasonForChange,
                        StudyLanguage = studyModel.StudyLanguage,
                        VersionKey = _versionKey,
                        ReferenceKey = _refKey,
                        IsActive = true,
                        IsDeleted = false,
                        SubjectNumberDigitCount = studyModel.SubjectNumberDigist
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
                    var oldEntity = _context.Studies.Include(x => x.EquivalentStudy).FirstOrDefault(p => p.Id == studyModel.StudyId && p.IsActive && !p.IsDeleted);
                    if (oldEntity != null)
                    {
                        studyModel.StudyLink = StringExtensionsHelper.TurkishCharacterReplace(studyModel.StudyLink);
                        if (oldEntity.ProtocolCode != studyModel.ProtocolCode)
                        {
                            oldEntity.ProtocolCode = studyModel.ProtocolCode;
                            oldEntity.EquivalentStudy.ProtocolCode = "DEMO-" + studyModel.ProtocolCode;
                        }
                        if (oldEntity.StudyName != studyModel.StudyName)
                        {
                            oldEntity.StudyName = studyModel.StudyName;
                            oldEntity.EquivalentStudy.StudyName = "DEMO-" + studyModel.StudyName;
                        }
                        if (oldEntity.AskSubjectInitial != studyModel.AskSubjectInitial)
                        {
                            oldEntity.AskSubjectInitial = studyModel.AskSubjectInitial;
                            oldEntity.EquivalentStudy.AskSubjectInitial = studyModel.AskSubjectInitial;
                        }
                        if (oldEntity.StudyLink != studyModel.StudyLink)
                        {
                            oldEntity.StudyLink = studyModel.StudyLink;
                            oldEntity.EquivalentStudy.StudyLink = "DEMO-" + studyModel.StudyLink;
                        }
                        if (oldEntity.ReasonForChange != studyModel.ReasonForChange)
                        {
                            oldEntity.ReasonForChange = studyModel.ReasonForChange;
                            oldEntity.EquivalentStudy.ReasonForChange = studyModel.ReasonForChange;
                        }
                        if (oldEntity.Description != studyModel.Description)
                        {
                            oldEntity.Description = studyModel.Description;
                            oldEntity.EquivalentStudy.Description = studyModel.Description;
                        }
                        if (oldEntity.SubDescription != studyModel.SubDescription)
                        {
                            oldEntity.SubDescription = studyModel.SubDescription;
                            oldEntity.EquivalentStudy.SubDescription = studyModel.SubDescription;
                        }
                        if (oldEntity.StudyType != (studyModel.DoubleDataEntry ? (int)StudyType.DoubleEntry : (int)StudyType.Normal))
                        {
                            oldEntity.StudyType = studyModel.DoubleDataEntry ? (int)StudyType.DoubleEntry : (int)StudyType.Normal;
                            oldEntity.EquivalentStudy.StudyType = studyModel.DoubleDataEntry ? (int)StudyType.DoubleEntry : (int)StudyType.Normal;
                        }
                        if (oldEntity.StudyLanguage != studyModel.StudyLanguage)
                        {
                            oldEntity.StudyLanguage = studyModel.StudyLanguage;
                            oldEntity.EquivalentStudy.StudyLanguage = studyModel.StudyLanguage;
                        }
                        if (oldEntity.SubjectNumberDigitCount != studyModel.SubjectNumberDigist)
                        {
                            oldEntity.SubjectNumberDigitCount = studyModel.SubjectNumberDigist;
                            oldEntity.EquivalentStudy.SubjectNumberDigitCount = studyModel.SubjectNumberDigist;
                        }

                        _context.Studies.Update(oldEntity);
                        var result = await _context.SaveChangesAsync();

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
                VisitType = x.VisitType,
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
        public async Task<ApiResponse<dynamic>> SetTransferData(List<TransferDataDTO> transferDataDTOs)
        {
            ApiResponse<dynamic> response = null;

            if (transferDataDTOs == null || transferDataDTOs.Count < 1)
            {
                return response = new ApiResponse<dynamic>
                {
                    IsSuccess = false,
                    Message = "Unsuccessful"
                };
            }

            BaseDTO baseDTO = Request.Headers.GetBaseInformation();

            var visitDTOs = transferDataDTOs.Where(x => x.Type == VisitStatu.visit).ToList();
            var pageDTOs = transferDataDTOs.Where(x => x.Type == VisitStatu.page).ToList();
            var moduleDTOs = transferDataDTOs.Where(x => x.Type == VisitStatu.module).ToList();

            List<StudyVisit> visitDatas = null;
            List<StudyVisitPage> pageDatas = null;
            List<StudyVisitPageModule> moduleDatas = null;

            #region Visits
            List<StudyVisit> addedVisits = null;
            if (visitDTOs.Count > 0)
            {
                visitDatas = await _context.StudyVisits.Where(x => visitDTOs.Select(a => a.Id).Contains(x.Id)).Select(visit => new StudyVisit
                {
                    Id = visit.Id,
                    StudyId = visit.Study.EquivalentStudyId.Value,
                    ReferenceKey = visit.ReferenceKey,
                    VersionKey = visit.VersionKey,
                    VisitType = visit.VisitType,
                    Name = visit.Name,
                    Order = visit.Order,
                    TenantId = visit.TenantId,
                    IsDeleted = visit.IsDeleted,
                    IsActive = visit.IsActive,
                    StudyVisitPages = visit.StudyVisitPages.Select(page => new StudyVisitPage
                    {
                        ReferenceKey = page.ReferenceKey,
                        VersionKey = page.VersionKey,
                        Name = page.Name,
                        Order = page.Order,
                        EPro = page.EPro,
                        TenantId = page.TenantId,
                        IsActive = page.IsActive,
                        IsDeleted = page.IsDeleted,
                        StudyVisitPageModules = page.StudyVisitPageModules.Select(module => new StudyVisitPageModule
                        {
                            Name = module.Name,
                            ReferenceKey = module.ReferenceKey,
                            VersionKey = module.VersionKey,
                            Order = module.Order,
                            TenantId = module.TenantId,
                            IsActive = module.IsActive,
                            IsDeleted = module.IsDeleted,
                            StudyVisitPageModuleElements = module.StudyVisitPageModuleElements.Select(element => new StudyVisitPageModuleElement
                            {
                                ElementType = element.ElementType,
                                ElementName = element.ElementName,
                                Title = element.Title,
                                IsTitleHidden = element.IsTitleHidden,
                                Order = element.Order,
                                Description = element.Description,
                                Width = element.Width,
                                IsHidden = element.IsHidden,
                                IsRequired = element.IsRequired,
                                IsReadonly = element.IsReadonly,
                                IsDependent = element.IsDependent,
                                IsRelated = element.IsRelated,
                                CanMissing = element.CanMissing,
                                ReferenceKey = element.ReferenceKey,
                                TenantId = element.TenantId,
                                IsActive = element.IsActive,
                                IsDeleted = element.IsDeleted,
                                StudyVisitPageModuleElementDetail = element.StudyVisitPageModuleElementDetail,
                                StudyVisitPageModuleCalculationElementDetails = element.StudyVisitPageModuleCalculationElementDetails.Select(calcu => new StudyVisitPageModuleCalculationElementDetail
                                {
                                    TargetElementId = calcu.TargetElementId,
                                    VariableName = calcu.VariableName,
                                    ReferenceKey = calcu.ReferenceKey,
                                    TenantId = calcu.TenantId,
                                    IsDeleted = calcu.IsDeleted,
                                    IsActive = calcu.IsActive
                                }).ToList(),
                                StudyVisitPageModuleElementEvents = element.StudyVisitPageModuleElementEvents.Select(events => new StudyVisitPageModuleElementEvent
                                {
                                    EventType = events.EventType,
                                    ActionType = events.ActionType,
                                    SourceElementId = events.SourceElementId,
                                    TargetElementId = events.TargetElementId,
                                    ValueCondition = events.ValueCondition,
                                    ActionValue = events.ActionValue,
                                    VariableName = events.VariableName,
                                    ReferenceKey = events.ReferenceKey,
                                    TenantId = events.TenantId,
                                    IsDeleted = events.IsDeleted,
                                    IsActive = events.IsActive
                                }).ToList(),
                                StudyVisitPageModuleElementValidationDetails = element.StudyVisitPageModuleElementValidationDetails.Select(val => new StudyVisitPageModuleElementValidationDetail
                                {
                                    ActionType = val.ActionType,
                                    ValueCondition = val.ValueCondition,
                                    Value = val.Value,
                                    Message = val.Message,
                                    ReferenceKey = val.ReferenceKey,
                                    TenantId = val.TenantId,
                                    IsDeleted = val.IsDeleted,
                                    IsActive = val.IsActive
                                }).ToList()
                            }).ToList()
                        }).ToList(),
                        Permissions = page.Permissions
                    }).ToList(),
                    Permissions = visit.Permissions
                }).AsSplitQuery().ToListAsync();

                #region Visit insert
                var insertVisits = visitDTOs.Where(x => x.Statu == TransferChangeType.Insert);

                if (insertVisits.Count() > 0)
                {
                    addedVisits = visitDatas.Where(x => x.IsActive && !x.IsDeleted && insertVisits.Select(d => d.Id).Contains(x.Id)).Select(visitData =>
                    {
                        var newVisit = new StudyVisit
                        {
                            StudyId = visitData.StudyId,
                            ReferenceKey = visitData.ReferenceKey,
                            VersionKey = visitData.VersionKey,
                            VisitType = visitData.VisitType,
                            Name = visitData.Name,
                            Order = visitData.Order,
                            TenantId = visitData.TenantId
                        };

                        var pages = visitData.StudyVisitPages.Where(x => x.IsActive && !x.IsDeleted).Select(page => new StudyVisitPage
                        {
                            ReferenceKey = page.ReferenceKey,
                            VersionKey = page.VersionKey,
                            Name = page.Name,
                            Order = page.Order,
                            EPro = page.EPro,
                            TenantId = page.TenantId,
                            StudyVisitPageModules = page.StudyVisitPageModules.Where(x => x.IsActive && !x.IsDeleted).Select(module =>
                            {
                                var newModule = new StudyVisitPageModule
                                {
                                    StudyVisitPageId = page.Id,
                                    Name = module.Name,
                                    ReferenceKey = module.ReferenceKey,
                                    VersionKey = module.VersionKey,
                                    Order = module.Order,
                                    TenantId = module.TenantId
                                };

                                newModule.StudyVisitPageModuleElements = module.StudyVisitPageModuleElements.Where(x => x.IsActive && !x.IsDeleted).Select(element =>
                                {
                                    var newModuleElements = new StudyVisitPageModuleElement
                                    {
                                        ElementType = element.ElementType,
                                        ElementName = element.ElementName,
                                        Title = element.Title,
                                        IsTitleHidden = element.IsTitleHidden,
                                        Order = element.Order,
                                        Description = element.Description,
                                        Width = element.Width,
                                        IsHidden = element.IsHidden,
                                        IsRequired = element.IsRequired,
                                        IsDependent = element.IsDependent,
                                        IsRelated = element.IsRelated,
                                        CanMissing = element.CanMissing,
                                        ReferenceKey = element.ReferenceKey,
                                        TenantId = element.TenantId
                                    };

                                    var calcus = element.StudyVisitPageModuleCalculationElementDetails.Where(x => x.IsActive && !x.IsDeleted).Select(calculation =>
                                    {
                                        var calcu = new StudyVisitPageModuleCalculationElementDetail
                                        {
                                            CalculationElementId = element.Id,
                                            TargetElementId = calculation.TargetElementId,
                                            VariableName = calculation.VariableName,
                                            ReferenceKey = calculation.ReferenceKey,
                                            TenantId = calculation.TenantId
                                        };
                                        newModule.StudyVisitPageModuleCalculationElementDetail.Add(calcu);
                                        return calcu;
                                    }).ToList();

                                    newModuleElements.StudyVisitPageModuleElementDetail = new StudyVisitPageModuleElementDetail
                                    {
                                        ParentId = element.StudyVisitPageModuleElementDetail.ParentId,
                                        RowIndex = element.StudyVisitPageModuleElementDetail.RowIndex,
                                        ColunmIndex = element.StudyVisitPageModuleElementDetail.ColunmIndex,
                                        CanQuery = element.StudyVisitPageModuleElementDetail.CanQuery,
                                        CanSdv = element.StudyVisitPageModuleElementDetail.CanSdv,
                                        CanRemoteSdv = element.StudyVisitPageModuleElementDetail.CanRemoteSdv,
                                        CanComment = element.StudyVisitPageModuleElementDetail.CanComment,
                                        CanDataEntry = element.StudyVisitPageModuleElementDetail.CanDataEntry,
                                        ParentElementEProPageNumber = element.StudyVisitPageModuleElementDetail.ParentElementEProPageNumber,
                                        MetaDataTags = element.StudyVisitPageModuleElementDetail.MetaDataTags,
                                        EProPageNumber = element.StudyVisitPageModuleElementDetail.EProPageNumber,
                                        ButtonText = element.StudyVisitPageModuleElementDetail.ButtonText,
                                        DefaultValue = element.StudyVisitPageModuleElementDetail.DefaultValue,
                                        Unit = element.StudyVisitPageModuleElementDetail.Unit,
                                        LowerLimit = element.StudyVisitPageModuleElementDetail.LowerLimit,
                                        UpperLimit = element.StudyVisitPageModuleElementDetail.UpperLimit,
                                        Mask = element.StudyVisitPageModuleElementDetail.Mask,
                                        Layout = element.StudyVisitPageModuleElementDetail.Layout,
                                        StartDay = element.StudyVisitPageModuleElementDetail.StartDay,
                                        EndDay = element.StudyVisitPageModuleElementDetail.EndDay,
                                        StartMonth = element.StudyVisitPageModuleElementDetail.StartMonth,
                                        EndMonth = element.StudyVisitPageModuleElementDetail.EndMonth,
                                        StartYear = element.StudyVisitPageModuleElementDetail.StartYear,
                                        EndYear = element.StudyVisitPageModuleElementDetail.EndYear,
                                        AddTodayDate = element.StudyVisitPageModuleElementDetail.AddTodayDate,
                                        ElementOptions = element.StudyVisitPageModuleElementDetail.ElementOptions,
                                        TargetElementId = element.StudyVisitPageModuleElementDetail.TargetElementId,
                                        LeftText = element.StudyVisitPageModuleElementDetail.LeftText,
                                        RightText = element.StudyVisitPageModuleElementDetail.RightText,
                                        IsInCalculation = element.StudyVisitPageModuleElementDetail.IsInCalculation,
                                        MainJs = element.StudyVisitPageModuleElementDetail.MainJs,
                                        RelationMainJs = element.StudyVisitPageModuleElementDetail.RelationMainJs,
                                        RowCount = element.StudyVisitPageModuleElementDetail.RowCount,
                                        ColumnCount = element.StudyVisitPageModuleElementDetail.ColumnCount,
                                        DatagridAndTableProperties = element.StudyVisitPageModuleElementDetail.DatagridAndTableProperties,
                                        AdverseEventType = element.StudyVisitPageModuleElementDetail.AdverseEventType,
                                        TenantId = element.TenantId
                                    };

                                    newModuleElements.StudyVisitPageModuleCalculationElementDetails = calcus;

                                    newModuleElements.StudyVisitPageModuleElementEvents = element.StudyVisitPageModuleElementEvents.Where(x => x.IsActive && !x.IsDeleted).Select(events =>
                                    {
                                        var newEvents = new StudyVisitPageModuleElementEvent
                                        {
                                            EventType = events.EventType,
                                            ActionType = events.ActionType,
                                            SourceElementId = events.SourceElementId,
                                            TargetElementId = events.TargetElementId,
                                            ValueCondition = events.ValueCondition,
                                            ActionValue = events.ActionValue,
                                            VariableName = events.VariableName,
                                            ReferenceKey = events.ReferenceKey,
                                            TenantId = events.TenantId
                                        };
                                        newModule.StudyVisitPageModuleElementEvent.Add(newEvents);
                                        return newEvents;
                                    }).ToList();

                                    newModuleElements.StudyVisitPageModuleElementValidationDetails = element.StudyVisitPageModuleElementValidationDetails.Where(x => x.IsActive && !x.IsDeleted).Select(val =>
                                    {
                                        var newVal = new StudyVisitPageModuleElementValidationDetail
                                        {
                                            Message = val.Message,
                                            ActionType = val.ActionType,
                                            Value = val.Value,
                                            ValueCondition = val.ValueCondition,
                                            ReferenceKey = val.ReferenceKey,
                                            TenantId = val.TenantId
                                        };
                                        return newVal;
                                    }).ToList();

                                    return newModuleElements;
                                }).ToList();
                                return newModule;
                            }).ToList(),
                        }).ToList();

                        foreach (var page in pages)
                        {
                            newVisit.StudyVisitPages.Add(page);
                        }

                        return newVisit;
                    }).ToList();

                    await _context.StudyVisits.AddRangeAsync(addedVisits);
                }
                #endregion

                #region Visit update
                var updateVisits = visitDTOs.Where(x => x.Statu == TransferChangeType.Update);

                if (updateVisits.Count() > 0)
                {
                    var updatedVisits = visitDatas.Where(x => x.IsActive && !x.IsDeleted && updateVisits.Select(a => a.Id).Contains(x.Id)).ToList();

                    var newUpVisits = await _context.StudyVisits.Where(x => x.IsActive && !x.IsDeleted && updatedVisits.Select(a => a.ReferenceKey).Contains(x.ReferenceKey) && !updateVisits.Select(a => a.Id).Contains(x.Id)).ToListAsync();

                    foreach (var item in newUpVisits)
                    {
                        var p = updatedVisits.FirstOrDefault(x => x.ReferenceKey == item.ReferenceKey);
                        if (p != null)
                        {
                            item.Name = p.Name;
                            item.Order = p.Order;
                        }
                    }

                    _context.StudyVisits.UpdateRange(newUpVisits);
                }
                #endregion

                #region Visit delete
                var deleteVisits = visitDTOs.Where(x => x.Statu == TransferChangeType.Delete);

                if (deleteVisits.Count() > 0)
                {
                    var deletedVisits = visitDatas.Where(x => !x.IsActive && x.IsDeleted && deleteVisits.Select(a => a.Id).Contains(x.Id)).ToList();

                    var newDelVisits = await _context.StudyVisits.Where(x => x.IsActive && !x.IsDeleted && deletedVisits.Select(a => a.ReferenceKey).Contains(x.ReferenceKey) && !deleteVisits.Select(a => a.Id).Contains(x.Id)).Select(visit => new StudyVisit
                    {
                        Id = visit.Id,
                        CreatedAt = visit.CreatedAt,
                        AddedById = visit.AddedById,
                        UpdatedAt = visit.UpdatedAt,
                        UpdatedById = visit.UpdatedById,
                        IsActive = visit.IsActive,
                        IsDeleted = visit.IsDeleted,
                        TenantId = visit.TenantId,
                        StudyId = visit.StudyId,
                        ReferenceKey = visit.ReferenceKey,
                        VersionKey = visit.VersionKey,
                        VisitType = visit.VisitType,
                        Name = visit.Name,
                        Order = visit.Order,
                        StudyVisitPages = visit.StudyVisitPages.Select(page => new StudyVisitPage
                        {
                            Id = page.Id,
                            AddedById = page.AddedById,
                            CreatedAt = page.CreatedAt,
                            UpdatedAt = page.UpdatedAt,
                            UpdatedById = page.UpdatedById,
                            IsActive = page.IsActive,
                            IsDeleted = page.IsDeleted,
                            TenantId = page.TenantId,
                            StudyVisitId = page.StudyVisitId,
                            ReferenceKey = page.ReferenceKey,
                            VersionKey = page.VersionKey,
                            Name = page.Name,
                            Order = page.Order,
                            EPro = page.EPro,
                            StudyVisitPageModules = page.StudyVisitPageModules.Select(module => new StudyVisitPageModule
                            {
                                Id = module.Id,
                                AddedById = module.AddedById,
                                CreatedAt = module.CreatedAt,
                                UpdatedAt = module.UpdatedAt,
                                UpdatedById = module.UpdatedById,
                                IsActive = module.IsActive,
                                IsDeleted = module.IsDeleted,
                                TenantId = module.TenantId,
                                StudyVisitPageId = module.StudyVisitPageId,
                                Name = module.Name,
                                ReferenceKey = module.ReferenceKey,
                                VersionKey = module.VersionKey,
                                Order = module.Order,
                                StudyVisitPageModuleElements = module.StudyVisitPageModuleElements.Select(element => new StudyVisitPageModuleElement()
                                {
                                    Id = element.Id,
                                    CreatedAt = element.CreatedAt,
                                    AddedById = element.AddedById,
                                    UpdatedAt = element.UpdatedAt,
                                    UpdatedById = element.UpdatedById,
                                    IsActive = element.IsActive,
                                    IsDeleted = element.IsDeleted,
                                    TenantId = element.TenantId,
                                    StudyVisitPageModuleId = element.StudyVisitPageModuleId,
                                    ElementType = element.ElementType,
                                    ElementName = element.ElementName,
                                    Title = element.Title,
                                    IsTitleHidden = element.IsTitleHidden,
                                    Order = element.Order,
                                    Description = element.Description,
                                    Width = element.Width,
                                    IsHidden = element.IsHidden,
                                    IsRequired = element.IsRequired,
                                    IsDependent = element.IsDependent,
                                    IsRelated = element.IsRelated,
                                    IsReadonly = element.IsReadonly,
                                    CanMissing = element.CanMissing,
                                    ReferenceKey = element.ReferenceKey,
                                    StudyVisitPageModuleElementDetail = element.StudyVisitPageModuleElementDetail,
                                    StudyVisitPageModuleElementEvents = element.StudyVisitPageModuleElementEvents,
                                    StudyVisitPageModuleCalculationElementDetails = element.StudyVisitPageModuleCalculationElementDetails,
                                    StudyVisitPageModuleElementValidationDetails = element.StudyVisitPageModuleElementValidationDetails
                                }).ToList(),
                                StudyVisitPageModuleElementEvent = module.StudyVisitPageModuleElementEvent,
                                StudyVisitPageModuleCalculationElementDetail = module.StudyVisitPageModuleCalculationElementDetail
                            }).ToList(),
                            Permissions = page.Permissions
                        }).ToList(),
                        Permissions = visit.Permissions
                    }).AsSplitQuery().ToListAsync();

                    _context.Permissions.RemoveRange(newDelVisits.SelectMany(x => x.Permissions));

                    _context.Permissions.RemoveRange(newDelVisits.SelectMany(x => x.StudyVisitPages).SelectMany(x => x.Permissions));

                    _context.StudyVisits.RemoveRange(newDelVisits);
                }
                #endregion

                #region Visit back
                var backVisits = visitDTOs.Where(x => x.Statu == TransferChangeType.Back);

                if (backVisits.Count() > 0)
                {
                    visitDatas.Where(x => backVisits.Select(a => a.Id).Contains(x.Id)).ToList().ForEach(visit =>
                    {
                        visit.IsActive = true;
                        visit.IsDeleted = false;
                        visit.Permissions.ForEach(vPer =>
                        {
                            vPer.IsActive = true;
                            vPer.IsDeleted = false;
                        });
                        visit.StudyVisitPages.ToList().ForEach(page =>
                        {
                            page.IsActive = true;
                            page.IsDeleted = false;
                            page.StudyVisitPageModules.ToList().ForEach(module =>
                            {
                                module.IsActive = true;
                                module.IsDeleted = false;
                                module.StudyVisitPageModuleElements.ToList().ForEach(element =>
                                {
                                    element.IsActive = true;
                                    element.IsDeleted = false;
                                    if (element.StudyVisitPageModuleElementDetail != null)
                                    {
                                        element.StudyVisitPageModuleElementDetail.IsActive = true;
                                        element.StudyVisitPageModuleElementDetail.IsDeleted = false;
                                    }
                                    element.StudyVisitPageModuleCalculationElementDetails.ToList().ForEach(calcu =>
                                    {
                                        calcu.IsActive = true;
                                        calcu.IsDeleted = false;
                                    });
                                    element.StudyVisitPageModuleElementEvents.ToList().ForEach(eEvent =>
                                    {
                                        eEvent.IsActive = true;
                                        eEvent.IsDeleted = false;
                                    });
                                    element.StudyVisitPageModuleElementValidationDetails.ToList().ForEach(val =>
                                    {
                                        val.IsActive = true;
                                        val.IsDeleted = false;
                                    });
                                });
                            });
                            page.Permissions.ForEach(pPer =>
                            {
                                pPer.IsActive = true;
                                pPer.IsDeleted = false;
                            });
                        });
                    });
                }
                #endregion
            }
            #endregion

            #region Pages
            List<StudyVisitPage> addedPages = null;
            if (pageDTOs.Count > 0)
            {
                pageDatas = await _context.StudyVisitPages.Where(x => pageDTOs.Select(a => a.Id).Contains(x.Id)).Select(page => new StudyVisitPage
                {
                    Id = page.Id,
                    StudyVisit = page.StudyVisit,
                    ReferenceKey = page.ReferenceKey,
                    VersionKey = page.VersionKey,
                    Name = page.Name,
                    Order = page.Order,
                    EPro = page.EPro,
                    TenantId = page.TenantId,
                    IsActive = page.IsActive,
                    IsDeleted = page.IsDeleted,
                    StudyVisitPageModules = page.StudyVisitPageModules.Select(module => new StudyVisitPageModule
                    {
                        Name = module.Name,
                        ReferenceKey = module.ReferenceKey,
                        VersionKey = module.VersionKey,
                        Order = module.Order,
                        TenantId = module.TenantId,
                        IsActive = module.IsActive,
                        IsDeleted = module.IsDeleted,
                        StudyVisitPageModuleElements = module.StudyVisitPageModuleElements.Select(element => new StudyVisitPageModuleElement
                        {
                            ElementType = element.ElementType,
                            ElementName = element.ElementName,
                            Title = element.Title,
                            IsTitleHidden = element.IsTitleHidden,
                            Order = element.Order,
                            Description = element.Description,
                            Width = element.Width,
                            IsHidden = element.IsHidden,
                            IsRequired = element.IsRequired,
                            IsDependent = element.IsDependent,
                            IsReadonly = element.IsReadonly,
                            IsRelated = element.IsRelated,
                            CanMissing = element.CanMissing,
                            ReferenceKey = element.ReferenceKey,
                            TenantId = element.TenantId,
                            IsActive = element.IsActive,
                            IsDeleted = element.IsDeleted,
                            StudyVisitPageModuleElementDetail = element.StudyVisitPageModuleElementDetail,
                            StudyVisitPageModuleCalculationElementDetails = element.StudyVisitPageModuleCalculationElementDetails.Select(calcu => new StudyVisitPageModuleCalculationElementDetail
                            {
                                TargetElementId = calcu.TargetElementId,
                                VariableName = calcu.VariableName,
                                ReferenceKey = calcu.ReferenceKey,
                                TenantId = calcu.TenantId,
                                IsDeleted = calcu.IsDeleted,
                                IsActive = calcu.IsActive
                            }).ToList(),
                            StudyVisitPageModuleElementEvents = element.StudyVisitPageModuleElementEvents.Select(events => new StudyVisitPageModuleElementEvent
                            {
                                EventType = events.EventType,
                                ActionType = events.ActionType,
                                SourceElementId = events.SourceElementId,
                                TargetElementId = events.TargetElementId,
                                ValueCondition = events.ValueCondition,
                                ActionValue = events.ActionValue,
                                VariableName = events.VariableName,
                                ReferenceKey = events.ReferenceKey,
                                TenantId = events.TenantId,
                                IsDeleted = events.IsDeleted,
                                IsActive = events.IsActive
                            }).ToList(),
                            StudyVisitPageModuleElementValidationDetails = element.StudyVisitPageModuleElementValidationDetails.Select(val => new StudyVisitPageModuleElementValidationDetail
                            {
                                ActionType = val.ActionType,
                                ValueCondition = val.ValueCondition,
                                Value = val.Value,
                                Message = val.Message,
                                ReferenceKey = val.ReferenceKey,
                                TenantId = val.TenantId,
                                IsDeleted = val.IsDeleted,
                                IsActive = val.IsActive
                            }).ToList()
                        }).ToList()
                    }).ToList(),
                    Permissions = page.Permissions
                }).AsSplitQuery().ToListAsync();

                #region Page insert
                var insertPages = pageDTOs.Where(x => x.Statu == TransferChangeType.Insert);

                if (insertPages.Count() > 0)
                {
                    var visitRefKeys = pageDatas.Select(x => x.StudyVisit).Select(x => x.ReferenceKey);

                    var activeVisits = await _context.StudyVisits.Where(x => x.IsActive && !x.IsDeleted && visitRefKeys.Contains(x.ReferenceKey) && !pageDatas.Select(a => a.StudyVisit).Select(d => d.Id).Contains(x.Id)).ToListAsync();

                    addedPages = pageDatas.Where(x => x.IsActive && !x.IsDeleted && insertPages.Select(d => d.Id).Contains(x.Id)).Select(pageData =>
                    {
                        var visit = activeVisits.FirstOrDefault(x => x.ReferenceKey == pageData.StudyVisit.ReferenceKey);
                        var newPage = new StudyVisitPage
                        {
                            StudyVisitId = visit != null ? visit.Id : 0,
                            ReferenceKey = pageData.ReferenceKey,
                            VersionKey = pageData.VersionKey,
                            Name = pageData.Name,
                            Order = pageData.Order,
                            EPro = pageData.EPro,
                            TenantId = pageData.TenantId
                        };

                        var modules = pageData.StudyVisitPageModules.Where(x => x.IsActive && !x.IsDeleted).Select(module =>
                        {
                            var newModule = new StudyVisitPageModule
                            {
                                StudyVisitPageId = newPage.Id,
                                Name = module.Name,
                                ReferenceKey = module.ReferenceKey,
                                VersionKey = module.VersionKey,
                                Order = module.Order,
                                TenantId = module.TenantId
                            };

                            newModule.StudyVisitPageModuleElements = module.StudyVisitPageModuleElements.Where(x => x.IsActive && !x.IsDeleted).Select(element =>
                            {
                                var newModuleElements = new StudyVisitPageModuleElement
                                {
                                    ElementType = element.ElementType,
                                    ElementName = element.ElementName,
                                    Title = element.Title,
                                    IsTitleHidden = element.IsTitleHidden,
                                    Order = element.Order,
                                    Description = element.Description,
                                    Width = element.Width,
                                    IsHidden = element.IsHidden,
                                    IsRequired = element.IsRequired,
                                    IsDependent = element.IsDependent,
                                    IsRelated = element.IsRelated,
                                    CanMissing = element.CanMissing,
                                    ReferenceKey = element.ReferenceKey,
                                    TenantId = element.TenantId
                                };

                                var calcus = element.StudyVisitPageModuleCalculationElementDetails.Where(x => x.IsActive && !x.IsDeleted).Select(calculation =>
                                {
                                    var calcu = new StudyVisitPageModuleCalculationElementDetail
                                    {
                                        CalculationElementId = element.Id,
                                        TargetElementId = calculation.TargetElementId,
                                        VariableName = calculation.VariableName,
                                        ReferenceKey = calculation.ReferenceKey,
                                        TenantId = calculation.TenantId
                                    };
                                    newModule.StudyVisitPageModuleCalculationElementDetail.Add(calcu);
                                    return calcu;
                                }).ToList();

                                newModuleElements.StudyVisitPageModuleElementDetail = new StudyVisitPageModuleElementDetail
                                {
                                    ParentId = element.StudyVisitPageModuleElementDetail.ParentId,
                                    RowIndex = element.StudyVisitPageModuleElementDetail.RowIndex,
                                    ColunmIndex = element.StudyVisitPageModuleElementDetail.ColunmIndex,
                                    CanQuery = element.StudyVisitPageModuleElementDetail.CanQuery,
                                    CanSdv = element.StudyVisitPageModuleElementDetail.CanSdv,
                                    CanRemoteSdv = element.StudyVisitPageModuleElementDetail.CanRemoteSdv,
                                    CanComment = element.StudyVisitPageModuleElementDetail.CanComment,
                                    CanDataEntry = element.StudyVisitPageModuleElementDetail.CanDataEntry,
                                    ParentElementEProPageNumber = element.StudyVisitPageModuleElementDetail.ParentElementEProPageNumber,
                                    MetaDataTags = element.StudyVisitPageModuleElementDetail.MetaDataTags,
                                    EProPageNumber = element.StudyVisitPageModuleElementDetail.EProPageNumber,
                                    ButtonText = element.StudyVisitPageModuleElementDetail.ButtonText,
                                    DefaultValue = element.StudyVisitPageModuleElementDetail.DefaultValue,
                                    Unit = element.StudyVisitPageModuleElementDetail.Unit,
                                    LowerLimit = element.StudyVisitPageModuleElementDetail.LowerLimit,
                                    UpperLimit = element.StudyVisitPageModuleElementDetail.UpperLimit,
                                    Mask = element.StudyVisitPageModuleElementDetail.Mask,
                                    Layout = element.StudyVisitPageModuleElementDetail.Layout,
                                    StartDay = element.StudyVisitPageModuleElementDetail.StartDay,
                                    EndDay = element.StudyVisitPageModuleElementDetail.EndDay,
                                    StartMonth = element.StudyVisitPageModuleElementDetail.StartMonth,
                                    EndMonth = element.StudyVisitPageModuleElementDetail.EndMonth,
                                    StartYear = element.StudyVisitPageModuleElementDetail.StartYear,
                                    EndYear = element.StudyVisitPageModuleElementDetail.EndYear,
                                    AddTodayDate = element.StudyVisitPageModuleElementDetail.AddTodayDate,
                                    ElementOptions = element.StudyVisitPageModuleElementDetail.ElementOptions,
                                    TargetElementId = element.StudyVisitPageModuleElementDetail.TargetElementId,
                                    LeftText = element.StudyVisitPageModuleElementDetail.LeftText,
                                    RightText = element.StudyVisitPageModuleElementDetail.RightText,
                                    IsInCalculation = element.StudyVisitPageModuleElementDetail.IsInCalculation,
                                    MainJs = element.StudyVisitPageModuleElementDetail.MainJs,
                                    RelationMainJs = element.StudyVisitPageModuleElementDetail.RelationMainJs,
                                    RowCount = element.StudyVisitPageModuleElementDetail.RowCount,
                                    ColumnCount = element.StudyVisitPageModuleElementDetail.ColumnCount,
                                    DatagridAndTableProperties = element.StudyVisitPageModuleElementDetail.DatagridAndTableProperties,
                                    AdverseEventType = element.StudyVisitPageModuleElementDetail.AdverseEventType,
                                    TenantId = element.TenantId
                                };

                                newModuleElements.StudyVisitPageModuleCalculationElementDetails = calcus;

                                newModuleElements.StudyVisitPageModuleElementEvents = element.StudyVisitPageModuleElementEvents.Where(x => x.IsActive && !x.IsDeleted).Select(events =>
                                {
                                    var newEvents = new StudyVisitPageModuleElementEvent
                                    {
                                        EventType = events.EventType,
                                        ActionType = events.ActionType,
                                        SourceElementId = events.SourceElementId,
                                        TargetElementId = events.TargetElementId,
                                        ValueCondition = events.ValueCondition,
                                        ActionValue = events.ActionValue,
                                        VariableName = events.VariableName,
                                        ReferenceKey = events.ReferenceKey,
                                        TenantId = events.TenantId
                                    };
                                    newModule.StudyVisitPageModuleElementEvent.Add(newEvents);
                                    return newEvents;
                                }).ToList();

                                newModuleElements.StudyVisitPageModuleElementValidationDetails = element.StudyVisitPageModuleElementValidationDetails.Where(x => x.IsActive && !x.IsDeleted).Select(val =>
                                {
                                    var newVal = new StudyVisitPageModuleElementValidationDetail
                                    {
                                        Message = val.Message,
                                        ActionType = val.ActionType,
                                        Value = val.Value,
                                        ValueCondition = val.ValueCondition,
                                        ReferenceKey = val.ReferenceKey,
                                        TenantId = val.TenantId
                                    };
                                    return newVal;
                                }).ToList();

                                return newModuleElements;
                            }).ToList();
                            return newModule;
                        }).ToList();

                        foreach (var module in modules)
                        {
                            newPage.StudyVisitPageModules.Add(module);
                        }

                        return newPage;
                    }).ToList();

                    await _context.StudyVisitPages.AddRangeAsync(addedPages);
                }
                #endregion

                #region Page update
                var updatePages = pageDTOs.Where(x => x.Statu == TransferChangeType.Update);

                if (updatePages.Count() > 0)
                {
                    var updatedPages = pageDatas.Where(x => x.IsActive && !x.IsDeleted && updatePages.Select(a => a.Id).Contains(x.Id)).ToList();

                    var newUpPages = await _context.StudyVisitPages.Where(x => x.IsActive && !x.IsDeleted && updatedPages.Select(a => a.ReferenceKey).Contains(x.ReferenceKey) && !updatePages.Select(a => a.Id).Contains(x.Id)).ToListAsync();

                    foreach (var item in newUpPages)
                    {
                        var p = updatedPages.FirstOrDefault(x => x.ReferenceKey == item.ReferenceKey);
                        if (p != null)
                        {
                            item.Name = p.Name;
                            item.Order = p.Order;
                        }
                    }

                    _context.StudyVisitPages.UpdateRange(newUpPages);
                }
                #endregion

                #region Page delete
                var deletePages = pageDTOs.Where(x => x.Statu == TransferChangeType.Delete);

                if (deletePages.Count() > 0)
                {
                    var deletedPages = pageDatas.Where(x => !x.IsActive && x.IsDeleted && deletePages.Select(a => a.Id).Contains(x.Id)).ToList();

                    var newDelPages = await _context.StudyVisitPages.Where(x => x.IsActive && !x.IsDeleted && deletedPages.Select(a => a.ReferenceKey).Contains(x.ReferenceKey) && !deletePages.Select(a => a.Id).Contains(x.Id)).Select(page => new StudyVisitPage
                    {
                        Id = page.Id,
                        AddedById = page.AddedById,
                        CreatedAt = page.CreatedAt,
                        UpdatedAt = page.UpdatedAt,
                        UpdatedById = page.UpdatedById,
                        IsActive = page.IsActive,
                        IsDeleted = page.IsDeleted,
                        TenantId = page.TenantId,
                        StudyVisitId = page.StudyVisitId,
                        ReferenceKey = page.ReferenceKey,
                        VersionKey = page.VersionKey,
                        Name = page.Name,
                        Order = page.Order,
                        EPro = page.EPro,
                        StudyVisitPageModules = page.StudyVisitPageModules.Select(module => new StudyVisitPageModule
                        {
                            Id = module.Id,
                            AddedById = module.AddedById,
                            CreatedAt = module.CreatedAt,
                            UpdatedAt = module.UpdatedAt,
                            UpdatedById = module.UpdatedById,
                            IsActive = module.IsActive,
                            IsDeleted = module.IsDeleted,
                            TenantId = module.TenantId,
                            StudyVisitPageId = module.StudyVisitPageId,
                            Name = module.Name,
                            ReferenceKey = module.ReferenceKey,
                            VersionKey = module.VersionKey,
                            Order = module.Order,
                            StudyVisitPageModuleElements = module.StudyVisitPageModuleElements.Select(element => new StudyVisitPageModuleElement()
                            {
                                Id = element.Id,
                                CreatedAt = element.CreatedAt,
                                AddedById = element.AddedById,
                                UpdatedAt = element.UpdatedAt,
                                UpdatedById = element.UpdatedById,
                                IsActive = element.IsActive,
                                IsDeleted = element.IsDeleted,
                                TenantId = element.TenantId,
                                StudyVisitPageModuleId = element.StudyVisitPageModuleId,
                                ElementType = element.ElementType,
                                ElementName = element.ElementName,
                                Title = element.Title,
                                IsTitleHidden = element.IsTitleHidden,
                                Order = element.Order,
                                Description = element.Description,
                                Width = element.Width,
                                IsHidden = element.IsHidden,
                                IsRequired = element.IsRequired,
                                IsDependent = element.IsDependent,
                                IsRelated = element.IsRelated,
                                IsReadonly = element.IsReadonly,
                                CanMissing = element.CanMissing,
                                ReferenceKey = element.ReferenceKey,
                                StudyVisitPageModuleElementDetail = element.StudyVisitPageModuleElementDetail,
                                StudyVisitPageModuleElementEvents = element.StudyVisitPageModuleElementEvents,
                                StudyVisitPageModuleCalculationElementDetails = element.StudyVisitPageModuleCalculationElementDetails,
                                StudyVisitPageModuleElementValidationDetails = element.StudyVisitPageModuleElementValidationDetails
                            }).ToList(),
                            StudyVisitPageModuleElementEvent = module.StudyVisitPageModuleElementEvent,
                            StudyVisitPageModuleCalculationElementDetail = module.StudyVisitPageModuleCalculationElementDetail
                        }).ToList(),
                        Permissions = page.Permissions
                    }).AsSplitQuery().ToListAsync();

                    _context.Permissions.RemoveRange(newDelPages.SelectMany(x => x.Permissions));

                    _context.StudyVisitPages.RemoveRange(newDelPages);
                }
                #endregion

                #region Page back
                var backPages = pageDTOs.Where(x => x.Statu == TransferChangeType.Back);

                if (backPages.Count() > 0)
                {
                    pageDatas.Where(x => backPages.Select(a => a.Id).Contains(x.Id)).ToList().ForEach(page =>
                    {
                        page.IsActive = true;
                        page.IsDeleted = false;
                        page.StudyVisitPageModules.ToList().ForEach(module =>
                        {
                            module.IsActive = true;
                            module.IsDeleted = false;
                            module.StudyVisitPageModuleElements.ToList().ForEach(element =>
                            {
                                element.IsActive = true;
                                element.IsDeleted = false;
                                if (element.StudyVisitPageModuleElementDetail != null)
                                {
                                    element.StudyVisitPageModuleElementDetail.IsActive = true;
                                    element.StudyVisitPageModuleElementDetail.IsDeleted = false;
                                }
                                element.StudyVisitPageModuleCalculationElementDetails.ToList().ForEach(calcu =>
                                {
                                    calcu.IsActive = true;
                                    calcu.IsDeleted = false;
                                });
                                element.StudyVisitPageModuleElementEvents.ToList().ForEach(eEvent =>
                                {
                                    eEvent.IsActive = true;
                                    eEvent.IsDeleted = false;
                                });
                                element.StudyVisitPageModuleElementValidationDetails.ToList().ForEach(val =>
                                {
                                    val.IsActive = true;
                                    val.IsDeleted = false;
                                });
                            });
                        });
                        page.Permissions.ForEach(pPer =>
                        {
                            pPer.IsActive = true;
                            pPer.IsDeleted = false;
                        });
                    });
                }
                #endregion
            }
            #endregion

            #region Module
            List<StudyVisitPageModule> addedModules = null;
            if (moduleDTOs.Count > 0)
            {
                moduleDatas = await _context.StudyVisitPageModules.Where(x => moduleDTOs.Select(a => a.Id).Contains(x.Id)).Select(module => new StudyVisitPageModule
                {
                    Id = module.Id,
                    Name = module.Name,
                    ReferenceKey = module.ReferenceKey,
                    VersionKey = module.VersionKey,
                    Order = module.Order,
                    TenantId = module.TenantId,
                    IsActive = module.IsActive,
                    IsDeleted = module.IsDeleted,
                    StudyVisitPage = module.StudyVisitPage,
                    StudyVisitPageModuleElements = module.StudyVisitPageModuleElements.Select(element => new StudyVisitPageModuleElement
                    {
                        ElementType = element.ElementType,
                        ElementName = element.ElementName,
                        Title = element.Title,
                        IsTitleHidden = element.IsTitleHidden,
                        Order = element.Order,
                        Description = element.Description,
                        Width = element.Width,
                        IsHidden = element.IsHidden,
                        IsRequired = element.IsRequired,
                        IsDependent = element.IsDependent,
                        IsReadonly = element.IsReadonly,
                        IsRelated = element.IsRelated,
                        CanMissing = element.CanMissing,
                        ReferenceKey = element.ReferenceKey,
                        TenantId = element.TenantId,
                        IsActive = element.IsActive,
                        IsDeleted = element.IsDeleted,
                        CreatedAt = element.CreatedAt,
                        UpdatedAt = element.UpdatedAt,
                        UpdatedById = element.UpdatedById,
                        AddedById = element.AddedById,
                        StudyVisitPageModuleElementDetail = element.StudyVisitPageModuleElementDetail,
                        StudyVisitPageModuleCalculationElementDetails = element.StudyVisitPageModuleCalculationElementDetails.Select(calcu => new StudyVisitPageModuleCalculationElementDetail
                        {
                            TargetElementId = calcu.TargetElementId,
                            VariableName = calcu.VariableName,
                            ReferenceKey = calcu.ReferenceKey,
                            TenantId = calcu.TenantId,
                            IsDeleted = calcu.IsDeleted,
                            IsActive = calcu.IsActive,
                            CalculationElementId = calcu.CalculationElementId,
                            StudyVisitPageModule = calcu.StudyVisitPageModule,
                            CreatedAt = calcu.CreatedAt,
                            UpdatedAt = calcu.UpdatedAt,
                            UpdatedById = calcu.UpdatedById,
                            AddedById = calcu.AddedById,
                        }).ToList(),
                        StudyVisitPageModuleElementEvents = element.StudyVisitPageModuleElementEvents.Select(events => new StudyVisitPageModuleElementEvent
                        {
                            EventType = events.EventType,
                            ActionType = events.ActionType,
                            SourceElementId = events.SourceElementId,
                            TargetElementId = events.TargetElementId,
                            ValueCondition = events.ValueCondition,
                            ActionValue = events.ActionValue,
                            VariableName = events.VariableName,
                            ReferenceKey = events.ReferenceKey,
                            TenantId = events.TenantId,
                            IsDeleted = events.IsDeleted,
                            IsActive = events.IsActive,
                            CreatedAt = events.CreatedAt,
                            UpdatedAt = events.UpdatedAt,
                            UpdatedById = events.UpdatedById,
                            AddedById = events.AddedById,
                            StudyVisitPageModule = events.StudyVisitPageModule,
                        }).ToList(),
                        StudyVisitPageModuleElementValidationDetails = element.StudyVisitPageModuleElementValidationDetails.Select(val => new StudyVisitPageModuleElementValidationDetail
                        {
                            ActionType = val.ActionType,
                            ValueCondition = val.ValueCondition,
                            Value = val.Value,
                            Message = val.Message,
                            ReferenceKey = val.ReferenceKey,
                            TenantId = val.TenantId,
                            IsDeleted = val.IsDeleted,
                            IsActive = val.IsActive,
                            CreatedAt = val.CreatedAt,
                            UpdatedAt = val.UpdatedAt,
                            UpdatedById = val.UpdatedById,
                            AddedById = val.AddedById,
                        }).ToList()
                    }).ToList()
                }).AsSplitQuery().ToListAsync();

                #region Module insert
                var insertModules = moduleDTOs.Where(x => x.Statu == TransferChangeType.Insert);

                if (insertModules.Count() > 0)
                {
                    var pageRefKeys = moduleDatas.Select(x => x.StudyVisitPage).Select(x => x.ReferenceKey);

                    var activePages = await _context.StudyVisitPages.Where(x => x.IsActive && !x.IsDeleted && pageRefKeys.Contains(x.ReferenceKey) && !moduleDatas.Select(a => a.StudyVisitPage).Select(d => d.Id).Contains(x.Id)).ToListAsync();

                    addedModules = moduleDatas.Where(x => x.IsActive && !x.IsDeleted && insertModules.Select(d => d.Id).Contains(x.Id)).Select(moduleData =>
                    {
                        var page = activePages.FirstOrDefault(x => x.ReferenceKey == moduleData.StudyVisitPage.ReferenceKey);
                        var newModule = new StudyVisitPageModule
                        {
                            StudyVisitPageId = page != null ? page.Id : 0,
                            Name = moduleData.Name,
                            ReferenceKey = moduleData.ReferenceKey,
                            VersionKey = moduleData.VersionKey,
                            Order = moduleData.Order,
                            TenantId = moduleData.TenantId
                        };

                        var elements = moduleData.StudyVisitPageModuleElements.Where(x => x.IsActive && !x.IsDeleted).Select(element =>
                        {
                            var newModuleElements = new StudyVisitPageModuleElement
                            {
                                ElementType = element.ElementType,
                                ElementName = element.ElementName,
                                Title = element.Title,
                                IsTitleHidden = element.IsTitleHidden,
                                Order = element.Order,
                                Description = element.Description,
                                Width = element.Width,
                                IsHidden = element.IsHidden,
                                IsRequired = element.IsRequired,
                                IsDependent = element.IsDependent,
                                IsRelated = element.IsRelated,
                                CanMissing = element.CanMissing,
                                ReferenceKey = element.ReferenceKey,
                                TenantId = element.TenantId
                            };

                            var calcus = element.StudyVisitPageModuleCalculationElementDetails.Where(x => x.IsActive && !x.IsDeleted).Select(calculation =>
                            {
                                var calcu = new StudyVisitPageModuleCalculationElementDetail
                                {
                                    CalculationElementId = element.Id,
                                    TargetElementId = calculation.TargetElementId,
                                    VariableName = calculation.VariableName,
                                    ReferenceKey = calculation.ReferenceKey,
                                    TenantId = calculation.TenantId
                                };
                                newModule.StudyVisitPageModuleCalculationElementDetail.Add(calcu);
                                return calcu;
                            }).ToList();

                            newModuleElements.StudyVisitPageModuleElementDetail = new StudyVisitPageModuleElementDetail
                            {
                                ParentId = element.StudyVisitPageModuleElementDetail.ParentId,
                                RowIndex = element.StudyVisitPageModuleElementDetail.RowIndex,
                                ColunmIndex = element.StudyVisitPageModuleElementDetail.ColunmIndex,
                                CanQuery = element.StudyVisitPageModuleElementDetail.CanQuery,
                                CanSdv = element.StudyVisitPageModuleElementDetail.CanSdv,
                                CanRemoteSdv = element.StudyVisitPageModuleElementDetail.CanRemoteSdv,
                                CanComment = element.StudyVisitPageModuleElementDetail.CanComment,
                                CanDataEntry = element.StudyVisitPageModuleElementDetail.CanDataEntry,
                                ParentElementEProPageNumber = element.StudyVisitPageModuleElementDetail.ParentElementEProPageNumber,
                                MetaDataTags = element.StudyVisitPageModuleElementDetail.MetaDataTags,
                                EProPageNumber = element.StudyVisitPageModuleElementDetail.EProPageNumber,
                                ButtonText = element.StudyVisitPageModuleElementDetail.ButtonText,
                                DefaultValue = element.StudyVisitPageModuleElementDetail.DefaultValue,
                                Unit = element.StudyVisitPageModuleElementDetail.Unit,
                                LowerLimit = element.StudyVisitPageModuleElementDetail.LowerLimit,
                                UpperLimit = element.StudyVisitPageModuleElementDetail.UpperLimit,
                                Mask = element.StudyVisitPageModuleElementDetail.Mask,
                                Layout = element.StudyVisitPageModuleElementDetail.Layout,
                                StartDay = element.StudyVisitPageModuleElementDetail.StartDay,
                                EndDay = element.StudyVisitPageModuleElementDetail.EndDay,
                                StartMonth = element.StudyVisitPageModuleElementDetail.StartMonth,
                                EndMonth = element.StudyVisitPageModuleElementDetail.EndMonth,
                                StartYear = element.StudyVisitPageModuleElementDetail.StartYear,
                                EndYear = element.StudyVisitPageModuleElementDetail.EndYear,
                                AddTodayDate = element.StudyVisitPageModuleElementDetail.AddTodayDate,
                                ElementOptions = element.StudyVisitPageModuleElementDetail.ElementOptions,
                                TargetElementId = element.StudyVisitPageModuleElementDetail.TargetElementId,
                                LeftText = element.StudyVisitPageModuleElementDetail.LeftText,
                                RightText = element.StudyVisitPageModuleElementDetail.RightText,
                                IsInCalculation = element.StudyVisitPageModuleElementDetail.IsInCalculation,
                                MainJs = element.StudyVisitPageModuleElementDetail.MainJs,
                                RelationMainJs = element.StudyVisitPageModuleElementDetail.RelationMainJs,
                                RowCount = element.StudyVisitPageModuleElementDetail.RowCount,
                                ColumnCount = element.StudyVisitPageModuleElementDetail.ColumnCount,
                                DatagridAndTableProperties = element.StudyVisitPageModuleElementDetail.DatagridAndTableProperties,
                                AdverseEventType = element.StudyVisitPageModuleElementDetail.AdverseEventType,
                                TenantId = element.TenantId
                            };

                            newModuleElements.StudyVisitPageModuleCalculationElementDetails = calcus;

                            newModuleElements.StudyVisitPageModuleElementEvents = element.StudyVisitPageModuleElementEvents.Where(x => x.IsActive && !x.IsDeleted).Select(events =>
                            {
                                var newEvents = new StudyVisitPageModuleElementEvent
                                {
                                    EventType = events.EventType,
                                    ActionType = events.ActionType,
                                    SourceElementId = events.SourceElementId,
                                    TargetElementId = events.TargetElementId,
                                    ValueCondition = events.ValueCondition,
                                    ActionValue = events.ActionValue,
                                    VariableName = events.VariableName,
                                    ReferenceKey = events.ReferenceKey,
                                    TenantId = events.TenantId
                                };
                                newModule.StudyVisitPageModuleElementEvent.Add(newEvents);
                                return newEvents;
                            }).ToList();

                            newModuleElements.StudyVisitPageModuleElementValidationDetails = element.StudyVisitPageModuleElementValidationDetails.Where(x => x.IsActive && !x.IsDeleted).Select(val =>
                            {
                                var newVal = new StudyVisitPageModuleElementValidationDetail
                                {
                                    Message = val.Message,
                                    ActionType = val.ActionType,
                                    Value = val.Value,
                                    ValueCondition = val.ValueCondition,
                                    ReferenceKey = val.ReferenceKey,
                                    TenantId = val.TenantId
                                };
                                return newVal;
                            }).ToList();

                            return newModuleElements;
                        }).ToList();

                        foreach (var element in elements)
                        {
                            newModule.StudyVisitPageModuleElements.Add(element);
                        }

                        return newModule;
                    }).ToList();

                    await _context.StudyVisitPageModules.AddRangeAsync(addedModules);
                }
                #endregion

                #region Module update
                var updateModules = moduleDTOs.Where(x => x.Statu == TransferChangeType.Update);

                if (updateModules.Count() > 0)
                {
                    var updatedModules = moduleDatas.Where(x => x.IsActive && !x.IsDeleted && updateModules.Select(a => a.Id).Contains(x.Id)).ToList();

                    var newUpModules = await _context.StudyVisitPageModules.Where(x => x.IsActive && !x.IsDeleted && updatedModules.Select(a => a.ReferenceKey).Contains(x.ReferenceKey) && !updateModules.Select(a => a.Id).Contains(x.Id)).Select(module => new StudyVisitPageModule
                    {
                        Id = module.Id,
                        Name = module.Name,
                        ReferenceKey = module.ReferenceKey,
                        VersionKey = module.VersionKey,
                        Order = module.Order,
                        TenantId = module.TenantId,
                        IsActive = module.IsActive,
                        IsDeleted = module.IsDeleted,
                        StudyVisitPage = module.StudyVisitPage,
                        UpdatedAt = module.UpdatedAt,
                        AddedById = module.AddedById,
                        UpdatedById = module.UpdatedById,
                        CreatedAt = module.CreatedAt,
                        StudyVisitPageModuleElements = module.StudyVisitPageModuleElements.Select(element => new StudyVisitPageModuleElement
                        {
                            Id = element.Id,
                            StudyVisitPageModuleId = element.StudyVisitPageModuleId,
                            ElementType = element.ElementType,
                            ElementName = element.ElementName,
                            Title = element.Title,
                            IsTitleHidden = element.IsTitleHidden,
                            Order = element.Order,
                            Description = element.Description,
                            Width = element.Width,
                            IsHidden = element.IsHidden,
                            IsRequired = element.IsRequired,
                            IsDependent = element.IsDependent,
                            IsRelated = element.IsRelated,
                            CanMissing = element.CanMissing,
                            ReferenceKey = element.ReferenceKey,
                            TenantId = element.TenantId,
                            IsActive = element.IsActive,
                            IsDeleted = element.IsDeleted,
                            IsReadonly = element.IsReadonly,
                            CreatedAt = element.CreatedAt,
                            UpdatedAt = element.UpdatedAt,
                            UpdatedById = element.UpdatedById,
                            AddedById = element.AddedById,
                            StudyVisitPageModuleElementDetail = element.StudyVisitPageModuleElementDetail,
                            StudyVisitPageModuleCalculationElementDetails = element.StudyVisitPageModuleCalculationElementDetails.Select(calcu => new StudyVisitPageModuleCalculationElementDetail
                            {
                                Id = calcu.Id,
                                TargetElementId = calcu.TargetElementId,
                                VariableName = calcu.VariableName,
                                ReferenceKey = calcu.ReferenceKey,
                                TenantId = calcu.TenantId,
                                IsDeleted = calcu.IsDeleted,
                                IsActive = calcu.IsActive,
                                CalculationElementId = calcu.CalculationElementId,
                                StudyVisitPageModuleId = calcu.StudyVisitPageModuleId,
                                CreatedAt = calcu.CreatedAt,
                                UpdatedAt = calcu.UpdatedAt,
                                UpdatedById = calcu.UpdatedById,
                                AddedById = calcu.AddedById,
                            }).ToList(),
                            StudyVisitPageModuleElementEvents = element.StudyVisitPageModuleElementEvents.Select(events => new StudyVisitPageModuleElementEvent
                            {
                                Id = events.Id,
                                EventType = events.EventType,
                                ActionType = events.ActionType,
                                SourceElementId = events.SourceElementId,
                                TargetElementId = events.TargetElementId,
                                ValueCondition = events.ValueCondition,
                                ActionValue = events.ActionValue,
                                VariableName = events.VariableName,
                                ReferenceKey = events.ReferenceKey,
                                TenantId = events.TenantId,
                                IsDeleted = events.IsDeleted,
                                IsActive = events.IsActive,
                                CreatedAt = events.CreatedAt,
                                UpdatedAt = events.UpdatedAt,
                                UpdatedById = events.UpdatedById,
                                AddedById = events.AddedById,
                                StudyVisitPageModuleId = events.StudyVisitPageModuleId
                            }).ToList(),
                            StudyVisitPageModuleElementValidationDetails = element.StudyVisitPageModuleElementValidationDetails.Select(val => new StudyVisitPageModuleElementValidationDetail
                            {
                                Id = val.Id,
                                ActionType = val.ActionType,
                                ValueCondition = val.ValueCondition,
                                Value = val.Value,
                                Message = val.Message,
                                ReferenceKey = val.ReferenceKey,
                                TenantId = val.TenantId,
                                IsDeleted = val.IsDeleted,
                                IsActive = val.IsActive,
                                CreatedAt = val.CreatedAt,
                                UpdatedAt = val.UpdatedAt,
                                UpdatedById = val.UpdatedById,
                                AddedById = val.AddedById,
                            }).ToList()
                        }).ToList()
                    }).AsSplitQuery().ToListAsync();

                    foreach (var item in newUpModules)
                    {
                        var p = updatedModules.FirstOrDefault(x => x.ReferenceKey == item.ReferenceKey);
                        if (p != null)
                        {
                            item.Name = p.Name;
                            item.Order = p.Order;

                            var demoDeletedElements = p.StudyVisitPageModuleElements.Where(e => !e.IsActive && e.IsDeleted).ToList();

                            _context.StudyVisitPageModuleElements.RemoveRange(item.StudyVisitPageModuleElements.Where(x => demoDeletedElements.Select(a => a.ReferenceKey).Contains(x.ReferenceKey)));

                            var addedElements = p.StudyVisitPageModuleElements.Where(e => e.IsActive && !e.IsDeleted && !item.StudyVisitPageModuleElements.Any(n => n.ReferenceKey == e.ReferenceKey)).Select(element =>
                            {
                                var newModuleElements = new StudyVisitPageModuleElement
                                {
                                    ElementType = element.ElementType,
                                    ElementName = element.ElementName,
                                    Title = element.Title,
                                    IsTitleHidden = element.IsTitleHidden,
                                    Order = element.Order,
                                    Description = element.Description,
                                    Width = element.Width,
                                    IsHidden = element.IsHidden,
                                    IsRequired = element.IsRequired,
                                    IsDependent = element.IsDependent,
                                    IsRelated = element.IsRelated,
                                    CanMissing = element.CanMissing,
                                    ReferenceKey = element.ReferenceKey,
                                    TenantId = element.TenantId,
                                    StudyVisitPageModuleId = item.Id
                                };

                                var calcus = element.StudyVisitPageModuleCalculationElementDetails.Where(x => x.IsActive && !x.IsDeleted).Select(calculation =>
                                {
                                    var calcu = new StudyVisitPageModuleCalculationElementDetail
                                    {
                                        CalculationElementId = element.Id,
                                        TargetElementId = calculation.TargetElementId,
                                        VariableName = calculation.VariableName,
                                        ReferenceKey = calculation.ReferenceKey,
                                        TenantId = calculation.TenantId,
                                        StudyVisitPageModuleId = item.Id
                                    };
                                    return calcu;
                                }).ToList();

                                newModuleElements.StudyVisitPageModuleElementDetail = new StudyVisitPageModuleElementDetail
                                {
                                    ParentId = element.StudyVisitPageModuleElementDetail.ParentId,
                                    RowIndex = element.StudyVisitPageModuleElementDetail.RowIndex,
                                    ColunmIndex = element.StudyVisitPageModuleElementDetail.ColunmIndex,
                                    CanQuery = element.StudyVisitPageModuleElementDetail.CanQuery,
                                    CanSdv = element.StudyVisitPageModuleElementDetail.CanSdv,
                                    CanRemoteSdv = element.StudyVisitPageModuleElementDetail.CanRemoteSdv,
                                    CanComment = element.StudyVisitPageModuleElementDetail.CanComment,
                                    CanDataEntry = element.StudyVisitPageModuleElementDetail.CanDataEntry,
                                    ParentElementEProPageNumber = element.StudyVisitPageModuleElementDetail.ParentElementEProPageNumber,
                                    MetaDataTags = element.StudyVisitPageModuleElementDetail.MetaDataTags,
                                    EProPageNumber = element.StudyVisitPageModuleElementDetail.EProPageNumber,
                                    ButtonText = element.StudyVisitPageModuleElementDetail.ButtonText,
                                    DefaultValue = element.StudyVisitPageModuleElementDetail.DefaultValue,
                                    Unit = element.StudyVisitPageModuleElementDetail.Unit,
                                    LowerLimit = element.StudyVisitPageModuleElementDetail.LowerLimit,
                                    UpperLimit = element.StudyVisitPageModuleElementDetail.UpperLimit,
                                    Mask = element.StudyVisitPageModuleElementDetail.Mask,
                                    Layout = element.StudyVisitPageModuleElementDetail.Layout,
                                    StartDay = element.StudyVisitPageModuleElementDetail.StartDay,
                                    EndDay = element.StudyVisitPageModuleElementDetail.EndDay,
                                    StartMonth = element.StudyVisitPageModuleElementDetail.StartMonth,
                                    EndMonth = element.StudyVisitPageModuleElementDetail.EndMonth,
                                    StartYear = element.StudyVisitPageModuleElementDetail.StartYear,
                                    EndYear = element.StudyVisitPageModuleElementDetail.EndYear,
                                    AddTodayDate = element.StudyVisitPageModuleElementDetail.AddTodayDate,
                                    ElementOptions = element.StudyVisitPageModuleElementDetail.ElementOptions,
                                    TargetElementId = element.StudyVisitPageModuleElementDetail.TargetElementId,
                                    LeftText = element.StudyVisitPageModuleElementDetail.LeftText,
                                    RightText = element.StudyVisitPageModuleElementDetail.RightText,
                                    IsInCalculation = element.StudyVisitPageModuleElementDetail.IsInCalculation,
                                    MainJs = element.StudyVisitPageModuleElementDetail.MainJs,
                                    RelationMainJs = element.StudyVisitPageModuleElementDetail.RelationMainJs,
                                    RowCount = element.StudyVisitPageModuleElementDetail.RowCount,
                                    ColumnCount = element.StudyVisitPageModuleElementDetail.ColumnCount,
                                    DatagridAndTableProperties = element.StudyVisitPageModuleElementDetail.DatagridAndTableProperties,
                                    AdverseEventType = element.StudyVisitPageModuleElementDetail.AdverseEventType,
                                    TenantId = element.TenantId
                                };

                                newModuleElements.StudyVisitPageModuleCalculationElementDetails = calcus;

                                item.StudyVisitPageModuleCalculationElementDetail = calcus;

                                var events = element.StudyVisitPageModuleElementEvents.Where(x => x.IsActive && !x.IsDeleted).Select(events =>
                                {
                                    var newEvents = new StudyVisitPageModuleElementEvent
                                    {
                                        EventType = events.EventType,
                                        ActionType = events.ActionType,
                                        SourceElementId = events.SourceElementId,
                                        TargetElementId = events.TargetElementId,
                                        ValueCondition = events.ValueCondition,
                                        ActionValue = events.ActionValue,
                                        VariableName = events.VariableName,
                                        ReferenceKey = events.ReferenceKey,
                                        TenantId = events.TenantId,
                                        StudyVisitPageModuleId = item.Id
                                    };
                                    return newEvents;
                                }).ToList();

                                newModuleElements.StudyVisitPageModuleElementEvents = events;

                                item.StudyVisitPageModuleElementEvent = events;

                                newModuleElements.StudyVisitPageModuleElementValidationDetails = element.StudyVisitPageModuleElementValidationDetails.Where(x => x.IsActive && !x.IsDeleted).Select(val =>
                                {
                                    var newVal = new StudyVisitPageModuleElementValidationDetail
                                    {
                                        Message = val.Message,
                                        ActionType = val.ActionType,
                                        Value = val.Value,
                                        ValueCondition = val.ValueCondition,
                                        ReferenceKey = val.ReferenceKey,
                                        TenantId = val.TenantId
                                    };
                                    return newVal;
                                }).ToList();

                                return newModuleElements;
                            }).ToList();

                            foreach (var aE in addedElements)
                            {
                                item.StudyVisitPageModuleElements.Add(aE);
                            }

                            if (addedElements.Count > 0)
                            {
                                if (addedModules == null) addedModules = new List<StudyVisitPageModule>();
                                addedModules.Add(item);
                            }

                            var upElements = p.StudyVisitPageModuleElements.Where(e => e.IsActive && !e.IsDeleted && !demoDeletedElements.Concat(addedElements).Select(a => a.Id).Contains(e.Id)).ToList();

                            if (upElements.Count > 0)
                            {
                                foreach (var element in item.StudyVisitPageModuleElements)
                                {
                                    var element1 = upElements.FirstOrDefault(x => x.ReferenceKey == element.ReferenceKey);
                                    if (element1 != null)
                                    {
                                        element.ElementType = element1.ElementType;
                                        element.ElementName = element1.ElementName;
                                        element.Title = element1.Title;
                                        element.IsTitleHidden = element1.IsTitleHidden;
                                        element.Order = element1.Order;
                                        element.Description = element1.Description;
                                        element.Width = element1.Width;
                                        element.IsHidden = element1.IsHidden;
                                        element.IsRequired = element1.IsRequired;
                                        element.IsDependent = element1.IsDependent;
                                        element.IsRelated = element1.IsRelated;
                                        element.IsReadonly = element1.IsReadonly;
                                        element.CanMissing = element1.CanMissing;

                                        if (element1.StudyVisitPageModuleElementDetail != null)
                                        {
                                            element.StudyVisitPageModuleElementDetail.RowIndex = element1.StudyVisitPageModuleElementDetail.RowIndex;
                                            element.StudyVisitPageModuleElementDetail.ColunmIndex = element1.StudyVisitPageModuleElementDetail.ColunmIndex;
                                            element.StudyVisitPageModuleElementDetail.CanQuery = element1.StudyVisitPageModuleElementDetail.CanQuery;
                                            element.StudyVisitPageModuleElementDetail.CanSdv = element1.StudyVisitPageModuleElementDetail.CanSdv;
                                            element.StudyVisitPageModuleElementDetail.CanRemoteSdv = element1.StudyVisitPageModuleElementDetail.CanRemoteSdv;
                                            element.StudyVisitPageModuleElementDetail.CanComment = element1.StudyVisitPageModuleElementDetail.CanComment;
                                            element.StudyVisitPageModuleElementDetail.CanDataEntry = element1.StudyVisitPageModuleElementDetail.CanDataEntry;
                                            element.StudyVisitPageModuleElementDetail.ParentElementEProPageNumber = element1.StudyVisitPageModuleElementDetail.ParentElementEProPageNumber;
                                            element.StudyVisitPageModuleElementDetail.MetaDataTags = element1.StudyVisitPageModuleElementDetail.MetaDataTags;
                                            element.StudyVisitPageModuleElementDetail.EProPageNumber = element1.StudyVisitPageModuleElementDetail.EProPageNumber;
                                            element.StudyVisitPageModuleElementDetail.ButtonText = element1.StudyVisitPageModuleElementDetail.ButtonText;
                                            element.StudyVisitPageModuleElementDetail.DefaultValue = element1.StudyVisitPageModuleElementDetail.DefaultValue;
                                            element.StudyVisitPageModuleElementDetail.Unit = element1.StudyVisitPageModuleElementDetail.Unit;
                                            element.StudyVisitPageModuleElementDetail.LowerLimit = element1.StudyVisitPageModuleElementDetail.LowerLimit;
                                            element.StudyVisitPageModuleElementDetail.UpperLimit = element1.StudyVisitPageModuleElementDetail.UpperLimit;
                                            element.StudyVisitPageModuleElementDetail.Mask = element1.StudyVisitPageModuleElementDetail.Mask;
                                            element.StudyVisitPageModuleElementDetail.Layout = element1.StudyVisitPageModuleElementDetail.Layout;
                                            element.StudyVisitPageModuleElementDetail.StartDay = element1.StudyVisitPageModuleElementDetail.StartDay;
                                            element.StudyVisitPageModuleElementDetail.EndDay = element1.StudyVisitPageModuleElementDetail.EndDay;
                                            element.StudyVisitPageModuleElementDetail.StartMonth = element1.StudyVisitPageModuleElementDetail.StartMonth;
                                            element.StudyVisitPageModuleElementDetail.EndMonth = element1.StudyVisitPageModuleElementDetail.EndMonth;
                                            element.StudyVisitPageModuleElementDetail.StartYear = element1.StudyVisitPageModuleElementDetail.StartYear;
                                            element.StudyVisitPageModuleElementDetail.EndYear = element1.StudyVisitPageModuleElementDetail.EndYear;
                                            element.StudyVisitPageModuleElementDetail.AddTodayDate = element1.StudyVisitPageModuleElementDetail.AddTodayDate;
                                            element.StudyVisitPageModuleElementDetail.ElementOptions = element1.StudyVisitPageModuleElementDetail.ElementOptions;
                                            element.StudyVisitPageModuleElementDetail.TargetElementId = element1.StudyVisitPageModuleElementDetail.TargetElementId;
                                            element.StudyVisitPageModuleElementDetail.LeftText = element1.StudyVisitPageModuleElementDetail.LeftText;
                                            element.StudyVisitPageModuleElementDetail.RightText = element1.StudyVisitPageModuleElementDetail.RightText;
                                            element.StudyVisitPageModuleElementDetail.IsInCalculation = element1.StudyVisitPageModuleElementDetail.IsInCalculation;
                                            element.StudyVisitPageModuleElementDetail.MainJs = element1.StudyVisitPageModuleElementDetail.MainJs;
                                            element.StudyVisitPageModuleElementDetail.RelationMainJs = element1.StudyVisitPageModuleElementDetail.RelationMainJs;
                                            element.StudyVisitPageModuleElementDetail.RowCount = element1.StudyVisitPageModuleElementDetail.RowCount;
                                            element.StudyVisitPageModuleElementDetail.ColumnCount = element1.StudyVisitPageModuleElementDetail.ColumnCount;
                                            element.StudyVisitPageModuleElementDetail.DatagridAndTableProperties = element1.StudyVisitPageModuleElementDetail.DatagridAndTableProperties;
                                            element.StudyVisitPageModuleElementDetail.AdverseEventType = element1.StudyVisitPageModuleElementDetail.AdverseEventType;
                                        }

                                        var demoCalDeletedElements = element1.StudyVisitPageModuleCalculationElementDetails.Where(e => !e.IsActive && e.IsDeleted).ToList();

                                        _context.studyVisitPageModuleCalculationElementDetails.RemoveRange(element.StudyVisitPageModuleCalculationElementDetails.Where(x => demoCalDeletedElements.Select(a => a.ReferenceKey).Contains(x.ReferenceKey)));

                                        var demoCalAddedElements = element1.StudyVisitPageModuleCalculationElementDetails.Where(e => e.IsActive && !e.IsDeleted && !element.StudyVisitPageModuleCalculationElementDetails.Any(n => n.ReferenceKey == e.ReferenceKey)).Select(calculation =>
                                        {
                                            var calcu = new StudyVisitPageModuleCalculationElementDetail
                                            {
                                                CalculationElementId = element.Id,
                                                TargetElementId = calculation.TargetElementId,
                                                VariableName = calculation.VariableName,
                                                ReferenceKey = calculation.ReferenceKey,
                                                TenantId = calculation.TenantId,
                                                StudyVisitPageModuleId = item.Id
                                            };
                                            return calcu;
                                        }).ToList();

                                        foreach (var aEC in demoCalAddedElements)
                                        {
                                            element.StudyVisitPageModuleCalculationElementDetails.Add(aEC);
                                        }

                                        var upCalElements = element1.StudyVisitPageModuleCalculationElementDetails.Where(e => e.IsActive && !e.IsDeleted && !demoCalDeletedElements.Concat(demoCalAddedElements).Select(a => a.Id).Contains(e.Id)).ToList();

                                        if (upCalElements.Count > 0)
                                        {
                                            foreach (var calc in element.StudyVisitPageModuleCalculationElementDetails)
                                            {
                                                var calc1 = upCalElements.FirstOrDefault(x => x.ReferenceKey == calc.ReferenceKey);
                                                if (calc1 != null)
                                                {
                                                    calc.VariableName = calc1.VariableName;
                                                }
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    }

                    _context.StudyVisitPageModules.UpdateRange(newUpModules);
                }
                #endregion

                #region Module delete
                var deleteModules = moduleDTOs.Where(x => x.Statu == TransferChangeType.Delete);

                if (deleteModules.Count() > 0)
                {
                    var deletedModules = moduleDatas.Where(x => !x.IsActive && x.IsDeleted && deleteModules.Select(a => a.Id).Contains(x.Id)).ToList();

                    var newDelModules = await _context.StudyVisitPageModules.Where(x => x.IsActive && !x.IsDeleted && deletedModules.Select(a => a.ReferenceKey).Contains(x.ReferenceKey) && !deleteModules.Select(a => a.Id).Contains(x.Id)).Select(module => new StudyVisitPageModule
                    {
                        Id = module.Id,
                        AddedById = module.AddedById,
                        CreatedAt = module.CreatedAt,
                        UpdatedAt = module.UpdatedAt,
                        UpdatedById = module.UpdatedById,
                        IsActive = module.IsActive,
                        IsDeleted = module.IsDeleted,
                        TenantId = module.TenantId,
                        StudyVisitPageId = module.StudyVisitPageId,
                        Name = module.Name,
                        ReferenceKey = module.ReferenceKey,
                        VersionKey = module.VersionKey,
                        Order = module.Order,
                        StudyVisitPageModuleElements = module.StudyVisitPageModuleElements.Select(element => new StudyVisitPageModuleElement()
                        {
                            Id = element.Id,
                            CreatedAt = element.CreatedAt,
                            AddedById = element.AddedById,
                            UpdatedAt = element.UpdatedAt,
                            UpdatedById = element.UpdatedById,
                            IsActive = element.IsActive,
                            IsDeleted = element.IsDeleted,
                            TenantId = element.TenantId,
                            StudyVisitPageModuleId = element.StudyVisitPageModuleId,
                            ElementType = element.ElementType,
                            ElementName = element.ElementName,
                            Title = element.Title,
                            IsTitleHidden = element.IsTitleHidden,
                            Order = element.Order,
                            Description = element.Description,
                            Width = element.Width,
                            IsHidden = element.IsHidden,
                            IsRequired = element.IsRequired,
                            IsDependent = element.IsDependent,
                            IsRelated = element.IsRelated,
                            IsReadonly = element.IsReadonly,
                            CanMissing = element.CanMissing,
                            ReferenceKey = element.ReferenceKey,
                            StudyVisitPageModuleElementDetail = element.StudyVisitPageModuleElementDetail,
                            StudyVisitPageModuleElementEvents = element.StudyVisitPageModuleElementEvents,
                            StudyVisitPageModuleCalculationElementDetails = element.StudyVisitPageModuleCalculationElementDetails,
                            StudyVisitPageModuleElementValidationDetails = element.StudyVisitPageModuleElementValidationDetails
                        }).ToList(),
                        StudyVisitPageModuleElementEvent = module.StudyVisitPageModuleElementEvent,
                        StudyVisitPageModuleCalculationElementDetail = module.StudyVisitPageModuleCalculationElementDetail
                    }).AsSplitQuery().ToListAsync();

                    _context.StudyVisitPageModules.RemoveRange(newDelModules);
                }
                #endregion

                #region Module back
                var backModules = moduleDTOs.Where(x => x.Statu == TransferChangeType.Back);

                if (backModules.Count() > 0)
                {
                    moduleDatas.Where(x => backModules.Select(a => a.Id).Contains(x.Id)).ToList().ForEach(module =>
                    {
                        module.IsActive = true;
                        module.IsDeleted = false;
                        module.StudyVisitPageModuleElements.ToList().ForEach(element =>
                        {
                            element.IsActive = true;
                            element.IsDeleted = false;
                            if (element.StudyVisitPageModuleElementDetail != null)
                            {
                                element.StudyVisitPageModuleElementDetail.IsActive = true;
                                element.StudyVisitPageModuleElementDetail.IsDeleted = false;
                            }
                            element.StudyVisitPageModuleCalculationElementDetails.ToList().ForEach(calcu =>
                            {
                                calcu.IsActive = true;
                                calcu.IsDeleted = false;
                            });
                            element.StudyVisitPageModuleElementEvents.ToList().ForEach(eEvent =>
                            {
                                eEvent.IsActive = true;
                                eEvent.IsDeleted = false;
                            });
                            element.StudyVisitPageModuleElementValidationDetails.ToList().ForEach(val =>
                            {
                                val.IsActive = true;
                                val.IsDeleted = false;
                            });
                        });
                    });
                }
                #endregion
            }
            #endregion

            var result = await _context.SaveCoreContextAsync(baseDTO.UserId, DateTimeOffset.Now);

            if (result > 0)
            {
                if ((addedVisits != null && addedVisits.Count > 0) || (addedPages != null && addedPages.Count > 0) || (addedModules != null && addedModules.Count > 0))
                {
                    if (addedVisits != null && addedVisits.Count > 0)
                    {
                        try
                        {
                            response = await SetCalculationsEventsAndParentIds(addedVisits, visitDatas, VisitStatu.visit);
                        }
                        catch (Exception)
                        {
                            response = await UndoTransactions(addedVisits, VisitStatu.visit);
                        }
                    }

                    if (addedPages != null && addedPages.Count > 0)
                    {
                        try
                        {
                            response = await SetCalculationsEventsAndParentIds(addedPages, pageDatas.Where(x => addedPages.Select(a => a.ReferenceKey).Contains(x.ReferenceKey)).ToList(), VisitStatu.page);
                        }
                        catch (Exception)
                        {
                            response = await UndoTransactions(addedPages, VisitStatu.page);
                        }
                    }

                    if (addedModules != null && addedModules.Count > 0)
                    {
                        try
                        {
                            response = await SetCalculationsEventsAndParentIds(addedModules, moduleDatas.Where(x => addedModules.Select(a => a.ReferenceKey).Contains(x.ReferenceKey)).ToList(), VisitStatu.module);
                        }
                        catch (Exception)
                        {
                            response = await UndoTransactions(addedModules, VisitStatu.module);
                        }
                    }

                    return response;
                }
                else
                {
                    return new ApiResponse<dynamic>
                    {
                        IsSuccess = true,
                        Message = "Successful"
                    };
                }
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

        private async Task<ApiResponse<dynamic>> UndoTransactions<T>(List<T> addedObject, VisitStatu visitStatu)
        {
            BaseDTO baseDTO = Request.Headers.GetBaseInformation();

            if (visitStatu == VisitStatu.visit)
            {
                List<StudyVisit> addedVisits = addedObject.Cast<StudyVisit>().ToList();
                _context.StudyVisits.RemoveRange(addedVisits);
            }
            else if (visitStatu == VisitStatu.page)
            {
                List<StudyVisitPage> addedPages = addedObject.Cast<StudyVisitPage>().ToList();
                _context.StudyVisitPages.RemoveRange(addedPages);
            }
            else if (visitStatu == VisitStatu.module)
            {
                List<StudyVisitPageModule> addedModules = addedObject.Cast<StudyVisitPageModule>().ToList();
                _context.StudyVisitPageModules.RemoveRange(addedModules);
            }

            var result = await _context.SaveCoreContextAsync(baseDTO.UserId, DateTimeOffset.Now);

            if (result > -1)
            {
                return new ApiResponse<dynamic>
                {
                    IsSuccess = false,
                    Message = "Unsuccessful"
                };
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

        private async Task<ApiResponse<dynamic>> SetCalculationsEventsAndParentIds<T>(List<T> addedObject, List<T> data, VisitStatu visitStatu)
        {
            BaseDTO baseDTO = Request.Headers.GetBaseInformation();

            if (visitStatu == VisitStatu.visit)
            {
                List<StudyVisit> addedVisits = addedObject.Cast<StudyVisit>().ToList();
                List<StudyVisit> visitDatas = data.Cast<StudyVisit>().ToList();

                var elementReferenceKeys = visitDatas
                   .SelectMany(visitData => visitData.StudyVisitPages)
                   .SelectMany(studyVisitPage => studyVisitPage.StudyVisitPageModules)
                   .SelectMany(studyVisitPageModule => studyVisitPageModule.StudyVisitPageModuleElements)
                   .Select(element => element.ReferenceKey)
                   .Distinct();

                var elementsData = await _context.StudyVisitPageModuleElements.Where(x => elementReferenceKeys.Contains(x.ReferenceKey)).ToListAsync();

                var addedCalcuTargetElement = addedVisits.SelectMany(x => x.StudyVisitPages).SelectMany(x => x.StudyVisitPageModules).SelectMany(x => x.StudyVisitPageModuleElements).SelectMany(x => x.StudyVisitPageModuleCalculationElementDetails).ToList();

                foreach (var item in addedCalcuTargetElement)
                {
                    var ggg = elementsData.FirstOrDefault(x => item.TargetElementId == x.Id);
                    if (ggg != null)
                    {
                        var nItem = elementsData.FirstOrDefault(x => x.Id != ggg.Id && x.ReferenceKey == ggg.ReferenceKey);
                        if (nItem != null)
                        {
                            item.TargetElementId = nItem.Id;
                        }
                    }
                }

                var addedEventTargetElement = addedVisits.SelectMany(x => x.StudyVisitPages).SelectMany(x => x.StudyVisitPageModules).SelectMany(x => x.StudyVisitPageModuleElements).SelectMany(x => x.StudyVisitPageModuleElementEvents).ToList();

                foreach (var item in addedEventTargetElement)
                {
                    var ggg = elementsData.FirstOrDefault(x => item.SourceElementId == x.Id);
                    if (ggg != null)
                    {
                        var nItem = elementsData.FirstOrDefault(x => x.Id != ggg.Id && x.ReferenceKey == ggg.ReferenceKey);
                        if (nItem != null)
                        {
                            item.SourceElementId = nItem.Id;
                        }
                    }
                }

                var addedParentIds = addedVisits.SelectMany(x => x.StudyVisitPages).SelectMany(x => x.StudyVisitPageModules).SelectMany(x => x.StudyVisitPageModuleElements).Where(x => x.StudyVisitPageModuleElementDetail != null && x.StudyVisitPageModuleElementDetail.ParentId != null).Select(x => x.StudyVisitPageModuleElementDetail).ToList();

                foreach (var item in addedParentIds)
                {
                    var ggg = elementsData.FirstOrDefault(x => item?.ParentId == x.Id);
                    if (ggg != null)
                    {
                        var nItem = elementsData.FirstOrDefault(x => x.Id != ggg.Id && x.ReferenceKey == ggg.ReferenceKey);
                        if (nItem != null)
                        {
                            item.ParentId = nItem.Id;
                        }
                    }
                }

                var result1 = await _context.SaveCoreContextAsync(baseDTO.UserId, DateTimeOffset.Now);

                if (result1 > -1)
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
            else if (visitStatu == VisitStatu.page)
            {
                List<StudyVisitPage> addedPages = addedObject.Cast<StudyVisitPage>().ToList();
                List<StudyVisitPage> pageDatas = data.Cast<StudyVisitPage>().ToList();

                var calcus = addedPages.Where(x => x.StudyVisitPageModules != null && x.StudyVisitPageModules.Any() && x.StudyVisitPageModules.SelectMany(a => a.StudyVisitPageModuleElements).Any(e => e != null && e.StudyVisitPageModuleCalculationElementDetails != null && e.StudyVisitPageModuleCalculationElementDetails.Any())).ToList();

                var events = addedPages.Where(x => x.StudyVisitPageModules != null && x.StudyVisitPageModules.Any() && x.StudyVisitPageModules.SelectMany(a => a.StudyVisitPageModuleElements).Any(e => e != null && e.StudyVisitPageModuleElementEvents != null && e.StudyVisitPageModuleElementEvents.Any())).ToList();

                var elementReferenceKeys = pageDatas
                   .SelectMany(studyVisitPage => studyVisitPage.StudyVisitPageModules)
                   .SelectMany(studyVisitPageModule => studyVisitPageModule.StudyVisitPageModuleElements)
                   .Select(element => element.ReferenceKey)
                   .Distinct();

                var elementsData = await _context.StudyVisitPageModuleElements.Where(x => elementReferenceKeys.Contains(x.ReferenceKey)).ToListAsync();

                var addedCalcuTargetElement = calcus.SelectMany(x => x.StudyVisitPageModules).SelectMany(x => x.StudyVisitPageModuleElements).SelectMany(x => x.StudyVisitPageModuleCalculationElementDetails).ToList();

                foreach (var item in addedCalcuTargetElement)
                {
                    var ggg = elementsData.FirstOrDefault(x => item.TargetElementId == x.Id);
                    if (ggg != null)
                    {
                        var nItem = elementsData.FirstOrDefault(x => x.Id != ggg.Id && x.ReferenceKey == ggg.ReferenceKey);
                        if (nItem != null)
                        {
                            item.TargetElementId = nItem.Id;
                        }
                    }
                }

                var addedEventTargetElement = events.SelectMany(x => x.StudyVisitPageModules).SelectMany(x => x.StudyVisitPageModuleElements).SelectMany(x => x.StudyVisitPageModuleElementEvents).ToList();

                foreach (var item in addedEventTargetElement)
                {
                    var ggg = elementsData.FirstOrDefault(x => item.SourceElementId == x.Id);
                    if (ggg != null)
                    {
                        var nItem = elementsData.FirstOrDefault(x => x.Id != ggg.Id && x.ReferenceKey == ggg.ReferenceKey);
                        if (nItem != null)
                        {
                            item.SourceElementId = nItem.Id;
                        }
                    }
                }

                var addedParentIds = addedPages.SelectMany(x => x.StudyVisitPageModules).SelectMany(x => x.StudyVisitPageModuleElements).Where(x => x.StudyVisitPageModuleElementDetail != null && x.StudyVisitPageModuleElementDetail.ParentId != null).Select(x => x.StudyVisitPageModuleElementDetail).ToList();

                foreach (var item in addedParentIds)
                {
                    var ggg = elementsData.FirstOrDefault(x => item?.ParentId == x.Id);
                    if (ggg != null)
                    {
                        var nItem = elementsData.FirstOrDefault(x => x.Id != ggg.Id && x.ReferenceKey == ggg.ReferenceKey);
                        if (nItem != null)
                        {
                            item.ParentId = nItem.Id;
                        }
                    }
                }

                var result1 = await _context.SaveCoreContextAsync(baseDTO.UserId, DateTimeOffset.Now);

                if (result1 > -1)
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
            else if (visitStatu == VisitStatu.module)
            {
                List<StudyVisitPageModule> addedModules = addedObject.Cast<StudyVisitPageModule>().ToList();
                List<StudyVisitPageModule> moduleDatas = data.Cast<StudyVisitPageModule>().ToList();

                var calcus = addedModules.Where(x => x.StudyVisitPageModuleElements.Any(e => e != null && e.StudyVisitPageModuleCalculationElementDetails != null && e.StudyVisitPageModuleCalculationElementDetails.Any())).ToList();

                var events = addedModules.Where(x => x.StudyVisitPageModuleElements.Any(e => e != null && e.StudyVisitPageModuleElementEvents != null && e.StudyVisitPageModuleElementEvents.Any())).ToList();

                var elementReferenceKeys = moduleDatas
                   .SelectMany(studyVisitPageModule => studyVisitPageModule.StudyVisitPageModuleElements)
                   .Select(element => element.ReferenceKey)
                   .Distinct();

                var elementsData = await _context.StudyVisitPageModuleElements.Where(x => elementReferenceKeys.Contains(x.ReferenceKey)).Include(x => x.StudyVisitPageModuleElementDetail).ToListAsync();

                var addedCalcuTargetElement = calcus.SelectMany(x => x.StudyVisitPageModuleElements).SelectMany(x => x.StudyVisitPageModuleCalculationElementDetails).ToList();
                var oldCalcu = moduleDatas.SelectMany(x => x.StudyVisitPageModuleElements).SelectMany(x => x.StudyVisitPageModuleCalculationElementDetails);
                foreach (var item in addedCalcuTargetElement)
                {
                    if (oldCalcu.Any(x => x.ReferenceKey == item.ReferenceKey && x.Id != item.Id && x.TargetElementId == item.TargetElementId))
                    {
                        var ggg = elementsData.FirstOrDefault(x => item.TargetElementId == x.Id);
                        if (ggg != null)
                        {
                            var nItem = elementsData.FirstOrDefault(x => x.Id != ggg.Id && x.ReferenceKey == ggg.ReferenceKey);
                            if (nItem != null)
                            {
                                item.TargetElementId = nItem.Id;
                            }
                        }
                    }
                }

                var addedEventTargetElement = events.SelectMany(x => x.StudyVisitPageModuleElements).SelectMany(x => x.StudyVisitPageModuleElementEvents).ToList();
                var oldEvent = moduleDatas.SelectMany(x => x.StudyVisitPageModuleElements).SelectMany(x => x.StudyVisitPageModuleElementEvents);
                foreach (var item in addedEventTargetElement)
                {
                    if (oldEvent.Any(x => x.ReferenceKey == item.ReferenceKey && x.Id != item.Id && x.SourceElementId == item.SourceElementId))
                    {
                        var ggg = elementsData.FirstOrDefault(x => item.SourceElementId == x.Id);
                        if (ggg != null)
                        {
                            var nItem = elementsData.FirstOrDefault(x => x.Id != ggg.Id && x.ReferenceKey == ggg.ReferenceKey);
                            if (nItem != null)
                            {
                                item.SourceElementId = nItem.Id;
                            }
                        }
                    }
                }

                var addedParentIds = addedModules.SelectMany(x => x.StudyVisitPageModuleElements).Where(x => x.StudyVisitPageModuleElementDetail != null && x.StudyVisitPageModuleElementDetail.ParentId != null).Select(x => x.StudyVisitPageModuleElementDetail).ToList();
                var oldDetailst = moduleDatas.SelectMany(x => x.StudyVisitPageModuleElements).Select(x => x.StudyVisitPageModuleElementDetail);
                foreach (var item in addedParentIds)
                {
                    var ggg = elementsData.FirstOrDefault(x => item?.ParentId == x.Id);
                    if (ggg != null)
                    {
                        if (oldDetailst.Any(x => ggg.StudyVisitPageModuleElementDetail.Id == x.Id && item.ParentId == x.ParentId))
                        {
                            var nItem = elementsData.FirstOrDefault(x => x.Id != ggg.Id && x.ReferenceKey == ggg.ReferenceKey);
                            if (nItem != null)
                            {
                                item.ParentId = nItem.Id;
                            }
                        }
                    }
                }
                var ddd = await _context.StudyVisitPageModuleElements.Where(x => addedCalcuTargetElement.Select(a => a.TargetElementId).Contains(x.Id)).Include(x => x.StudyVisitPageModuleElementDetail).ToListAsync();

                var addedParentIds1 = addedModules.SelectMany(x => x.StudyVisitPageModuleElements).Where(x => x.StudyVisitPageModuleElementDetail != null).Select(x => x.StudyVisitPageModuleElementDetail).ToList();
                var oldDetailst1 = moduleDatas.SelectMany(x => x.StudyVisitPageModuleElements).Select(x => x.StudyVisitPageModuleElementDetail);
                foreach (var item in ddd)
                {
                    var ggg = elementsData.FirstOrDefault(x => item.ReferenceKey == x.ReferenceKey);
                    if (ggg != null)
                    {
                        item.StudyVisitPageModuleElementDetail.IsInCalculation = ggg.StudyVisitPageModuleElementDetail.IsInCalculation;

                    }
                }

                var result1 = await _context.SaveCoreContextAsync(baseDTO.UserId, DateTimeOffset.Now);

                if (result1 > -1)
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
                    Message = "Unsuccessful"
                };
            }
        }

        [HttpGet]
        public async Task<List<VisitModel>> GetTransferData(Int64 demoStudyId, Int64 activeStudyId)
        {
            var demoVisits = await _context.StudyVisits.Where(x => x.StudyId == demoStudyId).Select(visit => new StudyVisit
            {
                Id = visit.Id,
                UpdatedAt = visit.UpdatedAt,
                ReferenceKey = visit.ReferenceKey,
                VisitType = visit.VisitType,
                Name = visit.Name,
                Order = visit.Order,
                IsActive = visit.IsActive,
                IsDeleted = visit.IsDeleted,
                StudyVisitPages = visit.StudyVisitPages.Select(page => new StudyVisitPage
                {
                    Id = page.Id,
                    UpdatedAt = page.UpdatedAt,
                    ReferenceKey = page.ReferenceKey,
                    Name = page.Name,
                    Order = page.Order,
                    IsActive = page.IsActive,
                    IsDeleted = page.IsDeleted,
                    StudyVisitPageModules = page.StudyVisitPageModules.Select(module => new StudyVisitPageModule
                    {
                        Id = module.Id,
                        UpdatedAt = module.UpdatedAt,
                        Name = module.Name,
                        ReferenceKey = module.ReferenceKey,
                        Order = module.Order,
                        IsActive = module.IsActive,
                        IsDeleted = module.IsDeleted,
                        StudyVisitPageModuleElements = module.StudyVisitPageModuleElements.Select(element => new StudyVisitPageModuleElement
                        {
                            Id = element.Id,
                            TenantId = element.TenantId,
                            AddedById = element.AddedById,
                            CreatedAt = element.CreatedAt,
                            UpdatedAt = element.UpdatedAt,
                            UpdatedById = element.UpdatedById,
                            StudyVisitPageModuleId = element.StudyVisitPageModuleId,
                            ElementType = element.ElementType,
                            ElementName = element.ElementName,
                            Title = element.Title,
                            IsTitleHidden = element.IsTitleHidden,
                            Order = element.Order,
                            Description = element.Description,
                            Width = element.Width,
                            IsHidden = element.IsHidden,
                            IsRequired = element.IsRequired,
                            IsDependent = element.IsDependent,
                            IsRelated = element.IsRelated,
                            IsReadonly = element.IsReadonly,
                            CanMissing = element.CanMissing,
                            ReferenceKey = element.ReferenceKey,
                            StudyVisitPageModuleElementDetail = element.StudyVisitPageModuleElementDetail,
                            IsActive = element.IsActive,
                            IsDeleted = element.IsDeleted,
                            StudyVisitPageModuleCalculationElementDetails = element.StudyVisitPageModuleCalculationElementDetails.Select(calcu => new StudyVisitPageModuleCalculationElementDetail
                            {
                                Id = calcu.Id,
                                TenantId = calcu.TenantId,
                                AddedById = calcu.AddedById,
                                CreatedAt = calcu.CreatedAt,
                                UpdatedAt = calcu.UpdatedAt,
                                UpdatedById = calcu.UpdatedById,
                                StudyVisitPageModuleId = calcu.StudyVisitPageModuleId,
                                CalculationElementId = calcu.CalculationElementId,
                                TargetElementId = calcu.TargetElementId,
                                VariableName = calcu.VariableName,
                                ReferenceKey = calcu.ReferenceKey,
                                IsActive = calcu.IsActive,
                                IsDeleted = calcu.IsDeleted,
                            }).ToList(),
                            StudyVisitPageModuleElementEvents = element.StudyVisitPageModuleElementEvents.Select(events => new StudyVisitPageModuleElementEvent
                            {
                                Id = events.Id,
                                TenantId = events.TenantId,
                                AddedById = events.AddedById,
                                CreatedAt = events.CreatedAt,
                                UpdatedAt = events.UpdatedAt,
                                UpdatedById = events.UpdatedById,
                                StudyVisitPageModuleId = events.StudyVisitPageModuleId,
                                EventType = events.EventType,
                                ActionType = events.ActionType,
                                SourceElementId = events.SourceElementId,
                                TargetElementId = events.TargetElementId,
                                ValueCondition = events.ValueCondition,
                                ActionValue = events.ActionValue,
                                VariableName = events.VariableName,
                                ReferenceKey = events.ReferenceKey,
                                IsActive = events.IsActive,
                                IsDeleted = events.IsDeleted,
                            }).ToList(),
                            StudyVisitPageModuleElementValidationDetails = element.StudyVisitPageModuleElementValidationDetails.Select(val => new StudyVisitPageModuleElementValidationDetail
                            {
                                Id = val.Id,
                                TenantId = val.TenantId,
                                AddedById = val.AddedById,
                                CreatedAt = val.CreatedAt,
                                UpdatedAt = val.UpdatedAt,
                                UpdatedById = val.UpdatedById,
                                IsActive = val.IsActive,
                                IsDeleted = val.IsDeleted,
                                StudyVisitPageModuleElementId = val.StudyVisitPageModuleElementId,
                                ReferenceKey = val.ReferenceKey,
                                ActionType = val.ActionType,
                                ValueCondition = val.ValueCondition,
                                Value = val.Value,
                                Message = val.Message
                            }).ToList()
                        }).ToList()
                    }).ToList()
                }).ToList()
            }).AsSplitQuery().ToListAsync();

            var activeVisits = await _context.StudyVisits.Where(x => x.StudyId == activeStudyId).Select(visit => new StudyVisit
            {
                Id = visit.Id,
                UpdatedAt = visit.UpdatedAt,
                ReferenceKey = visit.ReferenceKey,
                VisitType = visit.VisitType,
                Name = visit.Name,
                Order = visit.Order,
                IsActive = visit.IsActive,
                IsDeleted = visit.IsDeleted,
                StudyVisitPages = visit.StudyVisitPages.Select(page => new StudyVisitPage
                {
                    Id = page.Id,
                    UpdatedAt = page.UpdatedAt,
                    ReferenceKey = page.ReferenceKey,
                    Name = page.Name,
                    Order = page.Order,
                    IsActive = page.IsActive,
                    IsDeleted = page.IsDeleted,
                    StudyVisitPageModules = page.StudyVisitPageModules.Select(module => new StudyVisitPageModule
                    {
                        Id = module.Id,
                        UpdatedAt = module.UpdatedAt,
                        Name = module.Name,
                        ReferenceKey = module.ReferenceKey,
                        Order = module.Order,
                        IsActive = module.IsActive,
                        IsDeleted = module.IsDeleted,
                        StudyVisitPageModuleElements = module.StudyVisitPageModuleElements.Select(element => new StudyVisitPageModuleElement
                        {
                            Id = element.Id,
                            TenantId = element.TenantId,
                            AddedById = element.AddedById,
                            CreatedAt = element.CreatedAt,
                            UpdatedAt = element.UpdatedAt,
                            UpdatedById = element.UpdatedById,
                            StudyVisitPageModuleId = element.StudyVisitPageModuleId,
                            ElementType = element.ElementType,
                            ElementName = element.ElementName,
                            Title = element.Title,
                            IsTitleHidden = element.IsTitleHidden,
                            Order = element.Order,
                            Description = element.Description,
                            Width = element.Width,
                            IsHidden = element.IsHidden,
                            IsRequired = element.IsRequired,
                            IsDependent = element.IsDependent,
                            IsRelated = element.IsRelated,
                            IsReadonly = element.IsReadonly,
                            CanMissing = element.CanMissing,
                            ReferenceKey = element.ReferenceKey,
                            IsActive = element.IsActive,
                            IsDeleted = element.IsDeleted,
                            StudyVisitPageModuleElementDetail = element.StudyVisitPageModuleElementDetail,
                            StudyVisitPageModuleCalculationElementDetails = element.StudyVisitPageModuleCalculationElementDetails.Select(calcu => new StudyVisitPageModuleCalculationElementDetail
                            {
                                Id = calcu.Id,
                                TenantId = calcu.TenantId,
                                AddedById = calcu.AddedById,
                                CreatedAt = calcu.CreatedAt,
                                UpdatedAt = calcu.UpdatedAt,
                                UpdatedById = calcu.UpdatedById,
                                StudyVisitPageModuleId = calcu.StudyVisitPageModuleId,
                                CalculationElementId = calcu.CalculationElementId,
                                TargetElementId = calcu.TargetElementId,
                                VariableName = calcu.VariableName,
                                ReferenceKey = calcu.ReferenceKey,
                                IsActive = calcu.IsActive,
                                IsDeleted = calcu.IsDeleted,
                            }).ToList(),
                            StudyVisitPageModuleElementEvents = element.StudyVisitPageModuleElementEvents.Select(events => new StudyVisitPageModuleElementEvent
                            {
                                Id = events.Id,
                                TenantId = events.TenantId,
                                AddedById = events.AddedById,
                                CreatedAt = events.CreatedAt,
                                UpdatedAt = events.UpdatedAt,
                                UpdatedById = events.UpdatedById,
                                StudyVisitPageModuleId = events.StudyVisitPageModuleId,
                                EventType = events.EventType,
                                ActionType = events.ActionType,
                                SourceElementId = events.SourceElementId,
                                TargetElementId = events.TargetElementId,
                                ValueCondition = events.ValueCondition,
                                ActionValue = events.ActionValue,
                                VariableName = events.VariableName,
                                ReferenceKey = events.ReferenceKey,
                                IsActive = events.IsActive,
                                IsDeleted = events.IsDeleted,
                            }).ToList(),
                            StudyVisitPageModuleElementValidationDetails = element.StudyVisitPageModuleElementValidationDetails.Select(val => new StudyVisitPageModuleElementValidationDetail
                            {
                                Id = val.Id,
                                TenantId = val.TenantId,
                                AddedById = val.AddedById,
                                CreatedAt = val.CreatedAt,
                                UpdatedAt = val.UpdatedAt,
                                UpdatedById = val.UpdatedById,
                                IsActive = val.IsActive,
                                IsDeleted = val.IsDeleted,
                                StudyVisitPageModuleElementId = val.StudyVisitPageModuleElementId,
                                ReferenceKey = val.ReferenceKey,
                                ActionType = val.ActionType,
                                ValueCondition = val.ValueCondition,
                                Value = val.Value,
                                Message = val.Message
                            }).ToList()
                        }).ToList()
                    }).ToList()
                }).ToList()
            }).AsSplitQuery().ToListAsync();

            var result = demoVisits.OrderBy(visit => visit.Order).Select(x =>
            {
                var visitStatus = GetStatusForVisit(x, activeVisits, demoVisits);
                if (string.IsNullOrEmpty(visitStatus))
                {
                    return null;
                }
                var data = new VisitModel
                {
                    Id = x.Id,
                    Name = x.Name,
                    VisitType = x.VisitType,
                    Order = x.Order,
                    UpdatedAt = x.UpdatedAt,
                    Status = visitStatus,
                    Children = x.StudyVisitPages.OrderBy(page => page.Order).Select(page =>
                    {
                        var pageStatus = GetStatusForPage(page, activeVisits.SelectMany(t => t.StudyVisitPages).ToList(), demoVisits.SelectMany(t => t.StudyVisitPages).ToList(), visitStatus);
                        if (pageStatus == null)
                        {
                            return null;
                        }

                        var newPage = new VisitModel
                        {
                            Id = page.Id,
                            Name = page.Name,
                            Order = page.Order,
                            UpdatedAt = page.UpdatedAt,
                            Status = pageStatus,
                            Children = page.StudyVisitPageModules.OrderBy(module => module.Order).Select(module =>
                            {
                                var moduleStatus = GetStatusForModule(module, activeVisits.SelectMany(t => t.StudyVisitPages).SelectMany(t => t.StudyVisitPageModules).ToList(), demoVisits.SelectMany(t => t.StudyVisitPages).SelectMany(t => t.StudyVisitPageModules).ToList(), visitStatus, pageStatus);
                                if (moduleStatus == null)
                                {
                                    return null;
                                }

                                var newModule = new VisitModel
                                {
                                    Id = module.Id,
                                    Name = module.Name,
                                    Order = module.Order,
                                    UpdatedAt = module.UpdatedAt,
                                    Status = moduleStatus
                                };

                                return newModule;
                            }).Where(x => x != null).ToList()
                        };

                        return newPage;

                    }).Where(x => x != null).ToList()
                };
                return data;
            }).Where(x => x != null).ToList();

            return result;
        }

        string GetStatusForVisit(StudyVisit visit, List<StudyVisit> activeVisits, List<StudyVisit> demoVisits)
        {
            var activeVisit = activeVisits.FirstOrDefault(av => av.ReferenceKey == visit.ReferenceKey);
            var demoVisit = demoVisits.FirstOrDefault(dv => dv.ReferenceKey == visit.ReferenceKey);

            if ((activeVisit == null || activeVisit.IsDeleted && !activeVisit.IsActive) && demoVisit.IsDeleted && !demoVisit.IsActive)
            {
                return null;
            }
            if ((activeVisit == null || (activeVisit.IsDeleted && !activeVisit.IsActive)) && (demoVisit == null || (demoVisit.IsDeleted && !demoVisit.IsActive)))
            {
                return "";
            }
            else if ((activeVisit == null || activeVisit.IsDeleted && !activeVisit.IsActive) && (demoVisit != null && demoVisit.IsActive && !demoVisit.IsDeleted))
            {
                return TransferChangeType.Insert.GetDescription();
            }
            else if (activeVisit != null && activeVisit.IsActive && !activeVisit.IsDeleted && (demoVisit == null || (demoVisit.IsDeleted && !demoVisit.IsActive)))
            {
                return TransferChangeType.Delete.GetDescription();
            }
            else if (activeVisit != null && activeVisit.IsActive && !activeVisit.IsDeleted && demoVisit != null && demoVisit.IsActive && !demoVisit.IsDeleted && (demoVisit.Name != activeVisit.Name || demoVisit.Order != activeVisit.Order || demoVisit.VisitType!=activeVisit.VisitType))
            {
                return TransferChangeType.Update.GetDescription();
            }

            return TransferChangeType.None.GetDescription();
        }

        string? GetStatusForPage(StudyVisitPage page, List<StudyVisitPage> activePages, List<StudyVisitPage> demoPages, string visitStatus)
        {
            var activePage = activePages.FirstOrDefault(ap => ap.ReferenceKey == page.ReferenceKey);
            var demoPage = demoPages.FirstOrDefault(dp => dp.ReferenceKey == page.ReferenceKey);

            if (
              (visitStatus == TransferChangeType.Insert.ToString() && (demoPage == null || (demoPage.IsDeleted && !demoPage.IsActive)))
              ||
              (visitStatus == TransferChangeType.Delete.ToString() && activePage == null && demoPage.IsDeleted && !demoPage.IsActive)
              ||
              ((activePage == null || activePage.IsDeleted && !activePage.IsActive) && demoPage.IsDeleted && !demoPage.IsActive)
            )
            {
                return null;
            }
            else if (
                (visitStatus == TransferChangeType.Insert.ToString() && (demoPage != null || (!demoPage.IsDeleted && demoPage.IsActive)))
                ||
                (activePage == null || (activePage.IsDeleted && !activePage.IsActive)) && (demoPage == null || (demoPage.IsDeleted && !demoPage.IsActive))
                )
            {
                return "";
            }
            else if ((activePage == null || activePage.IsDeleted && !activePage.IsActive) && (demoPage != null && demoPage.IsActive && !demoPage.IsDeleted))
            {
                return TransferChangeType.Insert.GetDescription();
            }
            else if (activePage != null && activePage.IsActive && !activePage.IsDeleted && (demoPage == null || (demoPage.IsDeleted && !demoPage.IsActive)))
            {
                return TransferChangeType.Delete.GetDescription();
            }
            else if (activePage != null && activePage.IsActive && !activePage.IsDeleted && demoPage != null && demoPage.IsActive && !demoPage.IsDeleted && (demoPage.Name != activePage.Name || demoPage.Order != activePage.Order))
            {
                return TransferChangeType.Update.GetDescription();
            }

            return TransferChangeType.None.GetDescription();
        }

        string? GetStatusForModule(StudyVisitPageModule module, List<StudyVisitPageModule> activeModules, List<StudyVisitPageModule> demoModules, string visitStatus, string pageStatus)
        {
            var activeModule = activeModules.FirstOrDefault(am => am.ReferenceKey == module.ReferenceKey);
            var demoModule = demoModules.FirstOrDefault(dm => dm.ReferenceKey == module.ReferenceKey);

            if (
                (
                    (visitStatus == TransferChangeType.Insert.ToString() || pageStatus == TransferChangeType.Insert.ToString())
                    &&
                    (demoModule == null || (demoModule.IsDeleted && !demoModule.IsActive))
                )
                ||
                ((activeModule == null || activeModule.IsDeleted && !activeModule.IsActive) && demoModule.IsDeleted && !demoModule.IsActive)
            )
            {
                return null;
            }
            else if (
                ((visitStatus == TransferChangeType.Insert.ToString() || pageStatus == TransferChangeType.Insert.ToString()) && (demoModule != null || (!demoModule.IsDeleted && demoModule.IsActive)))
                ||
                (activeModule == null || (activeModule.IsDeleted && !activeModule.IsActive)) && (demoModule == null || (demoModule.IsDeleted && !demoModule.IsActive))
            )
            {
                return "";
            }
            else if ((activeModule == null || activeModule.IsDeleted && !activeModule.IsActive) && (demoModule != null && demoModule.IsActive && !demoModule.IsDeleted))
            {
                return TransferChangeType.Insert.GetDescription();
            }
            else if (activeModule != null && activeModule.IsActive && !activeModule.IsDeleted && (demoModule == null || (demoModule.IsDeleted && !demoModule.IsActive)))
            {
                return TransferChangeType.Delete.GetDescription();
            }
            else if (activeModule != null && activeModule.IsActive && !activeModule.IsDeleted && demoModule != null && demoModule.IsActive && !demoModule.IsDeleted && AreModulesEqual(activeModule, demoModule))
            {
                return TransferChangeType.Update.GetDescription();
            }

            return TransferChangeType.None.GetDescription();
        }

        bool AreModulesEqual(StudyVisitPageModule aModule, StudyVisitPageModule dModule)
        {
            if ((aModule.Name != dModule.Name || aModule.Order != dModule.Order))
            {
                return true;
            }

            if (AreModuleElementsEqual(aModule.StudyVisitPageModuleElements.Where(x => x.IsActive && !x.IsDeleted).ToList(), dModule.StudyVisitPageModuleElements.Where(x => x.IsActive && !x.IsDeleted).ToList()))
            {
                return true;
            }

            return false;
        }

        bool AreModuleElementsEqual(List<StudyVisitPageModuleElement> aElements, List<StudyVisitPageModuleElement> dElements)
        {
            if ((aElements == null && dElements != null) || (aElements != null && dElements == null) || (aElements != null && dElements != null && aElements.Count != dElements.Count))
            {
                return true;
            }

            foreach (var aElement in aElements)
            {
                var dElement = dElements.FirstOrDefault(e => e.ReferenceKey == aElement.ReferenceKey);
                if (AreModuleElementPropertiesEqual(aElement, dElement))
                {
                    return true;
                }
                if (AreModuleDetailsEqual(aElement.StudyVisitPageModuleElementDetail, dElement.StudyVisitPageModuleElementDetail)
                    ||
                    ((aElement.StudyVisitPageModuleCalculationElementDetails.Count > 0 || dElement.StudyVisitPageModuleCalculationElementDetails.Count > 0) && AreModuleCalculationDetailsEqual(aElement.StudyVisitPageModuleCalculationElementDetails.ToList(), dElement.StudyVisitPageModuleCalculationElementDetails/*.Where(x=>x.IsActive && !x.IsDeleted)*/.ToList(), aElements, dElements))
                    ||
                    AreModuleEventsEqual(aElement.StudyVisitPageModuleElementEvents.ToList(), dElement.StudyVisitPageModuleElementEvents.ToList(), aElements, dElements)
                    ||
                    AreModuleElementValidationEqual(aElement.StudyVisitPageModuleElementValidationDetails.ToList(), dElement.StudyVisitPageModuleElementValidationDetails.ToList())
                   )
                {
                    return true;
                }
            }

            return false;
        }

        bool AreModuleElementPropertiesEqual(StudyVisitPageModuleElement aElement, StudyVisitPageModuleElement dElement)
        {
            if (aElement.ElementType != dElement.ElementType ||
                aElement.ElementName != dElement.ElementName ||
                aElement.Title != dElement.Title ||
                aElement.IsTitleHidden != dElement.IsTitleHidden ||
                aElement.Order != dElement.Order ||
                aElement.Description != dElement.Description ||
                aElement.Width != dElement.Width ||
                aElement.IsHidden != dElement.IsHidden ||
                aElement.IsRequired != dElement.IsRequired ||
                aElement.IsDependent != dElement.IsDependent ||
                aElement.IsRelated != dElement.IsRelated ||
                aElement.IsReadonly != dElement.IsReadonly ||
                aElement.CanMissing != dElement.CanMissing
            )
            {
                return true;
            }

            return false;
        }

        bool AreModuleDetailsEqual(StudyVisitPageModuleElementDetail aDetail, StudyVisitPageModuleElementDetail dDetail)
        {
            if ((aDetail == null && dDetail != null) || (aDetail != null && dDetail == null))
            {
                return true;
            }

            if (
                aDetail.RowIndex != dDetail.RowIndex ||
                aDetail.ColunmIndex != dDetail.ColunmIndex ||
                aDetail.CanQuery != dDetail.CanQuery ||
                aDetail.CanSdv != dDetail.CanSdv ||
                aDetail.CanRemoteSdv != dDetail.CanRemoteSdv ||
                aDetail.CanComment != dDetail.CanComment ||
                aDetail.CanDataEntry != dDetail.CanDataEntry ||
                aDetail.ParentElementEProPageNumber != dDetail.ParentElementEProPageNumber ||
                aDetail.MetaDataTags != dDetail.MetaDataTags ||
                aDetail.EProPageNumber != dDetail.EProPageNumber ||
                aDetail.ButtonText != dDetail.ButtonText ||
                aDetail.DefaultValue != dDetail.DefaultValue ||
                aDetail.Unit != dDetail.Unit ||
                aDetail.LowerLimit != dDetail.LowerLimit ||
                aDetail.UpperLimit != dDetail.UpperLimit ||
                aDetail.Mask != dDetail.Mask ||
                aDetail.Layout != dDetail.Layout ||
                aDetail.StartDay != dDetail.StartDay ||
                aDetail.EndDay != dDetail.EndDay ||
                aDetail.StartMonth != dDetail.StartMonth ||
                aDetail.EndMonth != dDetail.EndMonth ||
                aDetail.StartYear != dDetail.StartYear ||
                aDetail.EndYear != dDetail.EndYear ||
                aDetail.AddTodayDate != dDetail.AddTodayDate ||
                aDetail.ElementOptions != dDetail.ElementOptions ||
                aDetail.TargetElementId != dDetail.TargetElementId ||
                aDetail.LeftText != dDetail.LeftText ||
                aDetail.RightText != dDetail.RightText ||
                aDetail.IsInCalculation != dDetail.IsInCalculation ||
                aDetail.MainJs != dDetail.MainJs ||
                aDetail.RelationMainJs != dDetail.RelationMainJs ||
                aDetail.RowCount != dDetail.RowCount ||
                aDetail.ColumnCount != dDetail.ColumnCount ||
                aDetail.DatagridAndTableProperties != dDetail.DatagridAndTableProperties ||
                aDetail.AdverseEventType != dDetail.AdverseEventType
            )
            {
                return true;
            }

            return false;
        }

        bool AreModuleCalculationDetailsEqual(List<StudyVisitPageModuleCalculationElementDetail> aCalcDetails, List<StudyVisitPageModuleCalculationElementDetail> dCalcDetails, List<StudyVisitPageModuleElement> aElements, List<StudyVisitPageModuleElement> dElements)
        {
            if ((aCalcDetails == null && dCalcDetails != null) || (aCalcDetails != null && dCalcDetails == null) || (aCalcDetails != null && dCalcDetails != null && aCalcDetails.Count != dCalcDetails.Count))
            {
                return true;
            }

            foreach (var aDetail in aCalcDetails)
            {
                var dDetail = dCalcDetails.FirstOrDefault(d => d.ReferenceKey == aDetail.ReferenceKey);

                if (AreModuleCalculationDetailPropertiesEqual(aDetail, dDetail) || AreModuleCalculationDetailTargetElementsEqual(aDetail, dDetail, aElements, dElements))
                {
                    return true;
                }
            }

            return false;
        }
        bool AreModuleCalculationDetailTargetElementsEqual(StudyVisitPageModuleCalculationElementDetail aEvent, StudyVisitPageModuleCalculationElementDetail dEvent, List<StudyVisitPageModuleElement> aElements, List<StudyVisitPageModuleElement> dElements)
        {
            var aElement = aElements.FirstOrDefault(x => x.Id == aEvent.TargetElementId);
            var dElement = dElements.FirstOrDefault(x => x.Id == dEvent.TargetElementId);
            if (
                aElement.ElementName != dElement.ElementName ||
                aElement.ElementType != dElement.ElementType
            )
            {
                return true;
            }

            return false;
        }
        bool AreModuleCalculationDetailPropertiesEqual(StudyVisitPageModuleCalculationElementDetail aCalcDetail, StudyVisitPageModuleCalculationElementDetail dCalcDetail)
        {
            if (
                aCalcDetail.VariableName != dCalcDetail.VariableName
            )
            {
                return true;
            }

            return false;
        }

        bool AreModuleEventsEqual(List<StudyVisitPageModuleElementEvent> aEvents, List<StudyVisitPageModuleElementEvent> dEvents, List<StudyVisitPageModuleElement> aElements, List<StudyVisitPageModuleElement> dElements)
        {
            if ((aEvents == null && dEvents != null) || (aEvents != null && dEvents == null) || (aEvents != null && dEvents != null && aEvents.Count != dEvents.Count))
            {
                return true;
            }

            foreach (var aEvent in aEvents)
            {
                var dEvent = dEvents.FirstOrDefault(d => d.ReferenceKey == aEvent.ReferenceKey);

                if (AreModuleEventPropertiesEqual(aEvent, dEvent) || AreModuleEventSourceElementsEqual(aEvent, dEvent, aElements, dElements))
                {
                    return true;
                }
            }

            return false;
        }

        bool AreModuleEventSourceElementsEqual(StudyVisitPageModuleElementEvent aEvent, StudyVisitPageModuleElementEvent dEvent, List<StudyVisitPageModuleElement> aElements, List<StudyVisitPageModuleElement> dElements)
        {
            var aElement = aElements.FirstOrDefault(x => x.Id == aEvent.SourceElementId);
            var dElement = dElements.FirstOrDefault(x => x.Id == dEvent.SourceElementId);
            if (
                aElement.ElementName != dElement.ElementName ||
                aElement.ElementType != dElement.ElementType
            )
            {
                return true;
            }

            return false;
        }

        bool AreModuleEventPropertiesEqual(StudyVisitPageModuleElementEvent aEvent, StudyVisitPageModuleElementEvent dEvent)
        {
            if (
                aEvent.EventType != dEvent.EventType ||
                aEvent.ActionType != dEvent.ActionType ||
                aEvent.ValueCondition != dEvent.ValueCondition ||
                aEvent.ActionValue != dEvent.ActionValue ||
                aEvent.VariableName != dEvent.VariableName
            )
            {
                return true;
            }

            return false;
        }

        bool AreModuleElementValidationEqual(List<StudyVisitPageModuleElementValidationDetail> aVals, List<StudyVisitPageModuleElementValidationDetail> dVals)
        {
            if ((aVals == null && dVals != null) || (aVals != null && dVals == null) || (aVals != null && dVals != null && aVals.Count != dVals.Count))
            {
                return true;
            }

            foreach (var aVal in aVals)
            {
                var dVal = dVals.FirstOrDefault(d => d.ReferenceKey == aVal.ReferenceKey);
                if (AreModuleElementValidationPropertiesEqual(aVal, dVal))
                {
                    return true;
                }
            }
            return false;
        }

        bool AreModuleElementValidationPropertiesEqual(StudyVisitPageModuleElementValidationDetail aVal, StudyVisitPageModuleElementValidationDetail dVal)
        {
            if (
                aVal.ActionType != dVal.ActionType ||
                aVal.ValueCondition != dVal.ValueCondition ||
                aVal.Value != dVal.Value ||
                aVal.Message != dVal.Value
            )
            {
                return true;
            }

            return false;
        }

        [HttpPost]
        public async Task<ApiResponse<dynamic>> SetVisits(VisitDTO visitDTO)
        {
            try
            {
                BaseDTO baseDTO = Request.Headers.GetBaseInformation();

                if (visitDTO.Id == null || visitDTO.Id == 0)
                {
                    if (visitDTO.Type == VisitStatu.visit.ToString())
                    {
                        Int64 vVer = 1;
                        if (_context.StudyVisits.Any()) vVer = _context.StudyVisits.Max(x => x.VersionKey) + 1;

                        StudyVisit visit = new StudyVisit
                        {
                            StudyId = visitDTO.StudyId,
                            Name = visitDTO.Name,
                            VisitType = visitDTO.VisitType.Value,
                            Order = visitDTO.Order,
                            ReferenceKey = Guid.NewGuid(),
                            VersionKey = vVer,
                            TenantId = baseDTO.TenantId
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
                            _studyService.RemoveSubjectDetailMenu(visitDTO.StudyId);

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
                        Int64 pVer = 1;
                        if (_context.StudyVisitPages.Any()) pVer = _context.StudyVisitPages.Max(x => x.VersionKey) + 1;

                        StudyVisitPage page = new StudyVisitPage
                        {
                            StudyVisitId = visitDTO.ParentId.Value,
                            Name = visitDTO.Name,
                            Order = visitDTO.Order,
                            ReferenceKey = Guid.NewGuid(),
                            VersionKey = pVer,
                            TenantId = baseDTO.TenantId
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
                            _studyService.RemoveSubjectDetailMenu(visitDTO.StudyId);

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
                                _studyService.RemoveSubjectDetailMenu(visitDTO.StudyId);

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
                                _studyService.RemoveSubjectDetailMenu(visitDTO.StudyId);

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
                                _studyService.RemoveSubjectDetailMenu(visitDTO.StudyId);

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
                        .Where(v => v.Id == visitDTO.Id && v.IsActive && !v.IsDeleted).Select(visit => new StudyVisit
                        {
                            Id = visit.Id,
                            CreatedAt = visit.CreatedAt,
                            AddedById = visit.AddedById,
                            UpdatedAt = visit.UpdatedAt,
                            UpdatedById = visit.UpdatedById,
                            IsActive = visit.IsActive,
                            IsDeleted = visit.IsDeleted,
                            TenantId = visit.TenantId,
                            StudyId = visit.StudyId,
                            ReferenceKey = visit.ReferenceKey,
                            VersionKey = visit.VersionKey,
                            VisitType = visit.VisitType,
                            Name = visit.Name,
                            Order = visit.Order,
                            StudyVisitPages = visit.StudyVisitPages.Select(page => new StudyVisitPage
                            {
                                Id = page.Id,
                                AddedById = page.AddedById,
                                CreatedAt = page.CreatedAt,
                                UpdatedAt = page.UpdatedAt,
                                UpdatedById = page.UpdatedById,
                                IsActive = page.IsActive,
                                IsDeleted = page.IsDeleted,
                                TenantId = page.TenantId,
                                StudyVisitId = page.StudyVisitId,
                                ReferenceKey = page.ReferenceKey,
                                VersionKey = page.VersionKey,
                                Name = page.Name,
                                Order = page.Order,
                                EPro = page.EPro,
                                StudyVisitPageModules = page.StudyVisitPageModules.Select(module => new StudyVisitPageModule
                                {
                                    Id = module.Id,
                                    AddedById = module.AddedById,
                                    CreatedAt = module.CreatedAt,
                                    UpdatedAt = module.UpdatedAt,
                                    UpdatedById = module.UpdatedById,
                                    IsActive = module.IsActive,
                                    IsDeleted = module.IsDeleted,
                                    TenantId = module.TenantId,
                                    StudyVisitPageId = module.StudyVisitPageId,
                                    Name = module.Name,
                                    ReferenceKey = module.ReferenceKey,
                                    VersionKey = module.VersionKey,
                                    Order = module.Order,
                                    StudyVisitPageModuleElements = module.StudyVisitPageModuleElements.Select(element => new StudyVisitPageModuleElement()
                                    {
                                        Id = element.Id,
                                        CreatedAt = element.CreatedAt,
                                        AddedById = element.AddedById,
                                        UpdatedAt = element.UpdatedAt,
                                        UpdatedById = element.UpdatedById,
                                        IsActive = element.IsActive,
                                        IsDeleted = element.IsDeleted,
                                        TenantId = element.TenantId,
                                        StudyVisitPageModuleId = element.StudyVisitPageModuleId,
                                        ElementType = element.ElementType,
                                        ElementName = element.ElementName,
                                        Title = element.Title,
                                        IsTitleHidden = element.IsTitleHidden,
                                        Order = element.Order,
                                        Description = element.Description,
                                        Width = element.Width,
                                        IsHidden = element.IsHidden,
                                        IsRequired = element.IsRequired,
                                        IsDependent = element.IsDependent,
                                        IsRelated = element.IsRelated,
                                        IsReadonly = element.IsReadonly,
                                        CanMissing = element.CanMissing,
                                        ReferenceKey = element.ReferenceKey,
                                        StudyVisitPageModuleElementDetail = element.StudyVisitPageModuleElementDetail,
                                        StudyVisitPageModuleElementEvents = element.StudyVisitPageModuleElementEvents,
                                        StudyVisitPageModuleCalculationElementDetails = element.StudyVisitPageModuleCalculationElementDetails,
                                        StudyVisitPageModuleElementValidationDetails = element.StudyVisitPageModuleElementValidationDetails
                                    }).ToList(),
                                    StudyVisitPageModuleElementEvent = module.StudyVisitPageModuleElementEvent,
                                    StudyVisitPageModuleCalculationElementDetail = module.StudyVisitPageModuleCalculationElementDetail
                                }).ToList(),
                                Permissions = page.Permissions
                            }).ToList(),
                            Permissions = visit.Permissions
                        }).AsSplitQuery().FirstOrDefaultAsync();
                    if (visit != null)
                    {
                        _context.Permissions.RemoveRange(visit.Permissions);

                        _context.Permissions.RemoveRange(visit.StudyVisitPages.SelectMany(x => x.Permissions));


                        _context.StudyVisits.Remove(visit);

                        var result = await _context.SaveCoreContextAsync(visitDTO.UserId, DateTimeOffset.Now) > 0;

                        if (result)
                        {
                            _studyService.RemoveSubjectDetailMenu(visitDTO.StudyId);

                            return new ApiResponse<dynamic>
                            {
                                IsSuccess = true,
                                Message = "Successful"
                            };
                        }
                    }
                }
                else if (visitDTO.Type == VisitStatu.page.ToString())
                {
                    var page = await _context.StudyVisitPages
                       .Where(v => v.Id == visitDTO.Id && v.IsActive && !v.IsDeleted).Select(page => new StudyVisitPage
                       {
                           Id = page.Id,
                           AddedById = page.AddedById,
                           CreatedAt = page.CreatedAt,
                           UpdatedAt = page.UpdatedAt,
                           UpdatedById = page.UpdatedById,
                           IsActive = page.IsActive,
                           IsDeleted = page.IsDeleted,
                           TenantId = page.TenantId,
                           StudyVisitId = page.StudyVisitId,
                           ReferenceKey = page.ReferenceKey,
                           VersionKey = page.VersionKey,
                           Name = page.Name,
                           Order = page.Order,
                           EPro = page.EPro,
                           StudyVisitPageModules = page.StudyVisitPageModules.Select(module => new StudyVisitPageModule
                           {
                               Id = module.Id,
                               AddedById = module.AddedById,
                               CreatedAt = module.CreatedAt,
                               UpdatedAt = module.UpdatedAt,
                               UpdatedById = module.UpdatedById,
                               IsActive = module.IsActive,
                               IsDeleted = module.IsDeleted,
                               TenantId = module.TenantId,
                               StudyVisitPageId = module.StudyVisitPageId,
                               Name = module.Name,
                               ReferenceKey = module.ReferenceKey,
                               VersionKey = module.VersionKey,
                               Order = module.Order,
                               StudyVisitPageModuleElements = module.StudyVisitPageModuleElements.Select(element => new StudyVisitPageModuleElement()
                               {
                                   Id = element.Id,
                                   CreatedAt = element.CreatedAt,
                                   AddedById = element.AddedById,
                                   UpdatedAt = element.UpdatedAt,
                                   UpdatedById = element.UpdatedById,
                                   IsActive = element.IsActive,
                                   IsDeleted = element.IsDeleted,
                                   TenantId = element.TenantId,
                                   StudyVisitPageModuleId = element.StudyVisitPageModuleId,
                                   ElementType = element.ElementType,
                                   ElementName = element.ElementName,
                                   Title = element.Title,
                                   IsTitleHidden = element.IsTitleHidden,
                                   Order = element.Order,
                                   Description = element.Description,
                                   Width = element.Width,
                                   IsHidden = element.IsHidden,
                                   IsRequired = element.IsRequired,
                                   IsDependent = element.IsDependent,
                                   IsRelated = element.IsRelated,
                                   IsReadonly = element.IsReadonly,
                                   CanMissing = element.CanMissing,
                                   ReferenceKey = element.ReferenceKey,
                                   StudyVisitPageModuleElementDetail = element.StudyVisitPageModuleElementDetail,
                                   StudyVisitPageModuleElementEvents = element.StudyVisitPageModuleElementEvents,
                                   StudyVisitPageModuleCalculationElementDetails = element.StudyVisitPageModuleCalculationElementDetails,
                                   StudyVisitPageModuleElementValidationDetails = element.StudyVisitPageModuleElementValidationDetails
                               }).ToList(),
                               StudyVisitPageModuleElementEvent = module.StudyVisitPageModuleElementEvent,
                               StudyVisitPageModuleCalculationElementDetail = module.StudyVisitPageModuleCalculationElementDetail
                           }).ToList(),
                           Permissions = page.Permissions
                       }).AsSplitQuery().FirstOrDefaultAsync();

                    if (page != null)
                    {
                        _context.Permissions.RemoveRange(page.Permissions);

                        _context.StudyVisitPages.Remove(page);

                        var result = await _context.SaveCoreContextAsync(visitDTO.UserId, DateTimeOffset.Now) > 0;

                        if (result)
                        {
                            _studyService.RemoveSubjectDetailMenu(visitDTO.StudyId);

                            return new ApiResponse<dynamic>
                            {
                                IsSuccess = true,
                                Message = "Successful"
                            };
                        }
                    }
                }
                else if (visitDTO.Type == VisitStatu.module.ToString())
                {
                    var module = await _context.StudyVisitPageModules
                       .Where(v => v.Id == visitDTO.Id && v.IsActive && !v.IsDeleted).Select(module => new StudyVisitPageModule
                       {
                           Id = module.Id,
                           AddedById = module.AddedById,
                           CreatedAt = module.CreatedAt,
                           UpdatedAt = module.UpdatedAt,
                           UpdatedById = module.UpdatedById,
                           IsActive = module.IsActive,
                           IsDeleted = module.IsDeleted,
                           TenantId = module.TenantId,
                           StudyVisitPageId = module.StudyVisitPageId,
                           Name = module.Name,
                           ReferenceKey = module.ReferenceKey,
                           VersionKey = module.VersionKey,
                           Order = module.Order,
                           StudyVisitPageModuleElements = module.StudyVisitPageModuleElements.Select(element => new StudyVisitPageModuleElement()
                           {
                               Id = element.Id,
                               CreatedAt = element.CreatedAt,
                               AddedById = element.AddedById,
                               UpdatedAt = element.UpdatedAt,
                               UpdatedById = element.UpdatedById,
                               IsActive = element.IsActive,
                               IsDeleted = element.IsDeleted,
                               TenantId = element.TenantId,
                               StudyVisitPageModuleId = element.StudyVisitPageModuleId,
                               ElementType = element.ElementType,
                               ElementName = element.ElementName,
                               Title = element.Title,
                               IsTitleHidden = element.IsTitleHidden,
                               Order = element.Order,
                               Description = element.Description,
                               Width = element.Width,
                               IsHidden = element.IsHidden,
                               IsRequired = element.IsRequired,
                               IsDependent = element.IsDependent,
                               IsRelated = element.IsRelated,
                               IsReadonly = element.IsReadonly,
                               CanMissing = element.CanMissing,
                               ReferenceKey = element.ReferenceKey,
                               StudyVisitPageModuleElementDetail = element.StudyVisitPageModuleElementDetail,
                               StudyVisitPageModuleElementEvents = element.StudyVisitPageModuleElementEvents,
                               StudyVisitPageModuleCalculationElementDetails = element.StudyVisitPageModuleCalculationElementDetails,
                               StudyVisitPageModuleElementValidationDetails = element.StudyVisitPageModuleElementValidationDetails
                           }).ToList(),
                           StudyVisitPageModuleElementEvent = module.StudyVisitPageModuleElementEvent,
                           StudyVisitPageModuleCalculationElementDetail = module.StudyVisitPageModuleCalculationElementDetail
                       }).AsSplitQuery().FirstOrDefaultAsync();

                    if (module != null)
                    {
                        _context.StudyVisitPageModules.Remove(module);

                        var result = await _context.SaveCoreContextAsync(visitDTO.UserId, DateTimeOffset.Now) > 0;

                        if (result)
                        {
                            _studyService.RemoveSubjectDetailMenu(visitDTO.StudyId);

                            return new ApiResponse<dynamic>
                            {
                                IsSuccess = true,
                                Message = "Successful"
                            };
                        }
                    }
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

            int lastOrder = _context.StudyVisitPageModules
                .Where(x => x.StudyVisitPageId == pageId && x.IsActive && !x.IsDeleted)
                .Select(module => module.Order)
                .DefaultIfEmpty()
                .Max();

            lastOrder = lastOrder == 0 ? 1 : lastOrder + 1;

            Int64 lastVer = 1;


            foreach (var moduleDTO in moduleDTOList)
            {
                var studyVisitPageModule = new StudyVisitPageModule
                {
                    StudyVisitPageId = moduleDTO.StudyVisitPageId,
                    Name = moduleDTO.Name,
                    ReferenceKey = Guid.NewGuid(),
                    VersionKey = lastVer,
                    Order = lastOrder,
                };

                studyVisitPageModule.StudyVisitPageModuleElements = MapElementDTOListToStudyVisitPageModuleElementList(moduleDTO.StudyVisitPageModuleElements);

                studyVisitPageModuleList.Add(studyVisitPageModule);

                lastOrder++;
                lastVer++;
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
                    ReferenceKey = Guid.NewGuid(),
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
                        AdverseEventType = elementDetailDTO.AdverseEventType
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
                        item.StudyVisitPageModuleElementDetail.ParentId = parentId == null ? 0 : parentId;
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
                                    ReferenceKey = Guid.NewGuid(),
                                    StudyVisitPageModuleId = item.StudyVisitPageModuleId,
                                    CalculationElementId = item.Id,
                                    TargetElementId = stdVstPgMdlElements.FirstOrDefault(x => x.ElementId == calculationElementDetailDTO.TargetElementId).Id,
                                    VariableName = calculationElementDetailDTO.VariableName
                                };

                                studyVisitPageModuleCalculationElementDetailsList.Add(studyVisitPageModuleCalculationElementDetail);
                            }

                            item.StudyVisitPageModuleCalculationElementDetails = studyVisitPageModuleCalculationElementDetailsList;
                        }

                        if (elementDTO.StudyVisitPageModuleElementEvents.Count > 0)
                        {
                            var studyVisitPageModuleElementEventList = new List<StudyVisitPageModuleElementEvent>();

                            foreach (var moduleElementEventDTO in elementDTO.StudyVisitPageModuleElementEvents)
                            {
                                var studyVisitPageModuleElementEvent = new StudyVisitPageModuleElementEvent
                                {
                                    ReferenceKey = Guid.NewGuid(),
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
                                    ReferenceKey = Guid.NewGuid(),
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

                            foreach (var dtl in elm.StudyVisitPageModuleCalculationElementDetails)
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

        private Int64 GetSourcePage(List<StudyVisitRelationSourcePageModel> data, Int64 elementId)
        {
            var page = data.Where(x => x.options != null).SelectMany(x => x.options)
                .FirstOrDefault(page => page.options != null && page.options.Any(elm => elm.Id == elementId));

            return page?.Id ?? 0;
        }

        [HttpGet]
        public async Task<StudyVisitRelationModel> GetVisitRelation()
        {
            BaseDTO baseDTO = Request.Headers.GetBaseInformation();

            var sourcePageData = await _context.StudyVisits.Where(x => x.IsActive && !x.IsDeleted && x.StudyId == baseDTO.StudyId).Include(x => x.StudyVisitPages).ThenInclude(x => x.StudyVisitPageModules).ThenInclude(x => x.StudyVisitPageModuleElements).Select(visit => new StudyVisitRelationSourcePageModel
            {
                Id = visit.Id,
                Label = visit.Name,
                options = visit.StudyVisitPages.Where(page => page.IsActive && !page.IsDeleted).Select(page => new StudyVisitRelationSourcePageModel
                {
                    Id = page.Id,
                    Label = page.Name,
                    options = page.StudyVisitPageModules.Where(mod => mod.IsActive && !mod.IsDeleted).SelectMany(mod => mod.StudyVisitPageModuleElements.Where(elm => elm.IsActive && !elm.IsDeleted)).Select(elm => new StudyVisitRelationSourcePageModel
                    {
                        Id = elm.Id,
                        Label = elm.ElementName
                    }).ToList()
                }).ToList()
            }).ToListAsync();

            var result = await _context.StudyVisitRelation
            .Where(x => x.IsActive && !x.IsDeleted && x.StudyId == baseDTO.StudyId)
            .Select(x => new
            {
                x.Id,
                x.ElementId,
                x.ActionCondition,
                x.ActionValue,
                x.TargetPage,
                x.ActionType
            }).ToListAsync();

            var relationData = result.Select(x => new VisitRelationModel
            {
                Key = Guid.NewGuid(),
                Id = x.Id,
                SourcePageId = GetSourcePage(sourcePageData, x.ElementId),
                ElementId = x.ElementId,
                ActionCondition = x.ActionCondition,
                ActionValue = JsonSerializer.Deserialize<List<string>>(x.ActionValue),
                TargetPage = JsonSerializer.Deserialize<List<Int64>>(x.TargetPage),
                ActionType = x.ActionType
            }).ToList();

            var fieldOperationData = Enum.GetValues(typeof(ActionCondition))
                          .Cast<ActionCondition>()
                          .Select(e => new { label = e.GetDescription(), value = Convert.ToInt32(e) })
                          .ToList<object>();

            return new StudyVisitRelationModel
            {
                visitRelationModels = relationData,
                studyVisitRelationSourcePageModels = sourcePageData,
                fieldOperationData = fieldOperationData
            };
        }

        [HttpPost]
        public async Task<ApiResponse<dynamic>> SetVisitRelation(List<StudyVisitRelationDTO> dto)
        {
            BaseDTO baseDTO = Request.Headers.GetBaseInformation();

            var data = await _context.StudyVisitRelation.Where(x => x.IsActive && !x.IsDeleted && x.StudyId == baseDTO.StudyId).ToListAsync();

            var add = dto.Where(i => !data.Any(e => e.Id == i.Id)).ToList();

            var update = dto.Where(i => data.Any(e => e.Id == i.Id &&
            (
                e.ElementId != i.ElementId ||
                e.ActionCondition != i.ActionCondition ||
                AreJsonStringsDifferent(e.ActionValue, i.ActionValue) ||
                AreJsonStringsDifferent(e.TargetPage, i.TargetPage) ||
                e.ActionType != i.ActionType
            ))).ToList();

            var delete = data.Where(e => !dto.Any(i => i.Id == e.Id)).ToList();

            if (add.Any())
            {
                await _context.StudyVisitRelation.AddRangeAsync(add.Select(x => new StudyVisitRelation
                {
                    ElementId = x.ElementId,
                    ActionCondition = x.ActionCondition,
                    ActionValue = x.ActionValue,
                    TargetPage = x.TargetPage,
                    ActionType = x.ActionType,
                    StudyId = baseDTO.StudyId,
                    TenantId = baseDTO.TenantId
                }));
            }

            if (update.Any())
            {
                foreach (var updateEntity in update)
                {
                    var entity = data.First(e => e.Id == updateEntity.Id);
                    entity.ElementId = updateEntity.ElementId;
                    entity.ActionCondition = updateEntity.ActionCondition;
                    entity.ActionValue = updateEntity.ActionValue;
                    entity.TargetPage = updateEntity.TargetPage;
                    entity.ActionType = updateEntity.ActionType;
                }
            }

            if (delete.Any())
            {
                _context.StudyVisitRelation.RemoveRange(delete);
            }

            var result = await _context.SaveCoreContextAsync(baseDTO.UserId, DateTimeOffset.Now);

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

        private bool AreJsonStringsDifferent(string json1, string json2)
        {
            if (string.IsNullOrEmpty(json1) && string.IsNullOrEmpty(json2))
                return false;

            if (string.IsNullOrEmpty(json1) || string.IsNullOrEmpty(json2))
                return true;

            var obj1 = JsonSerializer.Deserialize<object>(json1);
            var obj2 = JsonSerializer.Deserialize<object>(json2);

            return !obj1.Equals(obj2);
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
                    ReferenceKey = Guid.NewGuid(),
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
                            StudyVisitPageModuleElementId = e.ElementDetail.Id,
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

                var noElements = result.Where(module => !module.StudyVisitPageModuleElements.Any()).ToList();
                var tAElements = result.Where(module => module.StudyVisitPageModuleElements.Any()).ToList();

                var names = noElements.Select(element => element.Name).ToArray();
                var concatenatedNames = string.Join(", ", names);

                if (tAElements.Count < 1)
                {
                    return new ApiResponse<dynamic> { IsSuccess = false, Message = "", Values = concatenatedNames };
                }

                var res = await SetStudyModule(tAElements);
                if (noElements.Count > 0) res.Values = concatenatedNames;
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
                            var newdtl = new StudyVisitPageModuleCalculationElementDetail()
                            {
                                TenantId = cal.TenantId,
                                StudyVisitPageModuleId = cal.StudyVisitPageModuleId,
                                TargetElementId = cal.TargetElementId,
                                CalculationElementId = stdVstPgMdlElmnt.Id,
                                VariableName = cal.VariableName,
                            };

                            _context.Add(newdtl);
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
                result.IsSuccess = false;
                result.Message = "Error";
            }

            return result;
        }

        [HttpPost]
        public async Task<ApiResponse<dynamic>> DeleteElement(ElementShortModel model)
        {
            var result = new ApiResponse<dynamic>();
            var element = await _context.StudyVisitPageModuleElements.Where(x => x.Id == model.Id && x.IsActive && !x.IsDeleted).Include(x => x.StudyVisitPageModuleElementDetail).FirstOrDefaultAsync();
            var elementDetail = await _context.StudyVisitPageModuleElementDetails.Where(x => x.StudyVisitPageModuleElementId == model.Id && x.IsActive && !x.IsDeleted).FirstOrDefaultAsync();

            if (element != null)
            {
                if (element.StudyVisitPageModuleElementDetail.IsInCalculation && element.ElementType != ElementType.Calculated)
                {
                    result.IsSuccess = false;
                    result.Message = "This element used in a calculation element formul. Please remove it first from calculation element.";

                    return result;
                }

                var moduleEvent = await _context.StudyVisitPageModuleElementEvents.FirstOrDefaultAsync(x => x.SourceElementId == model.Id && x.IsActive && !x.IsDeleted);

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

                //remove events
                var moduleEvents = await _context.StudyVisitPageModuleElementEvents.Where(x => x.SourceElementId == model.Id && x.IsActive && !x.IsDeleted).ToListAsync();

                foreach (var item in moduleEvents)
                {
                    item.IsActive = false;
                    item.IsDeleted = true;

                    _context.StudyVisitPageModuleElementEvents.Update(item);
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
                        var elmCnt = elmDtils.Where(x => x.Id == item.Id).Count();

                        if (chngIds.Contains(item.StudyVisitPageModuleElementId) && elmCnt == 1)
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

                //remove validations
                var validations = await _context.StudyVisitPageModuleElementValidationDetails.Where(x => x.StudyVisitPageModuleElementId == element.Id && x.IsActive && !x.IsDeleted).ToListAsync();

                if (validations != null && validations.Count > 0)
                {
                    foreach (var validation in validations)
                    {
                        validation.IsActive = false;
                        validation.IsDeleted = true;

                        _context.StudyVisitPageModuleElementValidationDetails.Update(validation);
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

                var validations = await _context.StudyVisitPageModuleElementValidationDetails.Where(x => x.StudyVisitPageModuleElementId == result.Id && x.IsActive && !x.IsDeleted).ToListAsync();

                if (validations != null)
                {
                    result.ValidationList = JsonSerializer.Serialize(validations);
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
            var result = new ApiResponse<dynamic>() { Message = "Operation is successfully." };
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
                        ReferenceKey = Guid.NewGuid(),
                        CreatedAt = DateTimeOffset.Now,
                        AddedById = model.UserId,
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
                                SourceElementId = model.DependentSourceFieldId,
                                TargetElementId = stdVstPgMdlElmnt.Id,
                                StudyVisitPageModuleId = model.ModuleId,
                                TenantId = model.TenantId,
                                EventType = EventType.Dependency,
                                ValueCondition = (ActionCondition)model.DependentCondition,
                                ActionType = (ActionType)model.DependentAction,
                                ActionValue = model.DependentFieldValue,
                                ReferenceKey = Guid.NewGuid()
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
                                    TargetElementId = item.elementFieldSelectedGroup.value != 0 ? item.elementFieldSelectedGroup.value : stdVstPgMdlElmnt.Id,//own element is in calculation list
                                    VariableName = item.variableName,
                                    ReferenceKey = Guid.NewGuid()
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
                                    SourceElementId = item.relationFieldsSelectedGroup.value != 0 ? item.relationFieldsSelectedGroup.value : stdVstPgMdlElmnt.Id,//element related to own
                                    TargetElementId = stdVstPgMdlElmnt.Id,
                                    TenantId = model.TenantId,
                                    EventType = EventType.Relation,
                                    VariableName = item.variableName,
                                    ReferenceKey = Guid.NewGuid()
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

                        if (model.HasValidation)
                        {
                            var validations = JsonSerializer.Deserialize<List<ElementValidationModel>>(model.ValidationList);

                            foreach (var item in validations)
                            {
                                var validation = new StudyVisitPageModuleElementValidationDetail
                                {
                                    StudyVisitPageModuleElementId = stdVstPgMdlElmnt.Id,
                                    ActionType = item.ValidationActionType,
                                    ValueCondition = item.ValidationCondition,
                                    Value = item.ValidationValue,
                                    Message = item.ValidationMessage,
                                    ReferenceKey = Guid.NewGuid()
                                };

                                _context.StudyVisitPageModuleElementValidationDetails.Add(validation);
                            }

                            result.IsSuccess = await _context.SaveCoreContextAsync(model.UserId, DateTimeOffset.Now) > 0;
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
                            StudyVisitPageModuleId = stdVstPgMdlElement.StudyVisitPageModuleId,
                            ReferenceKey = Guid.NewGuid()
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
                else
                {
                    var dep = await _context.StudyVisitPageModuleElementEvents.FirstOrDefaultAsync(x => x.TargetElementId == model.Id && x.EventType == EventType.Dependency && x.IsActive && !x.IsDeleted);

                    if (dep != null)
                    {
                        dep.IsActive = false;
                        dep.IsDeleted = true;

                        _context.StudyVisitPageModuleElementEvents.Update(dep);
                    }
                }

                if (model.ElementType == ElementType.Calculated)
                {
                    var existCalDtil = await _context.studyVisitPageModuleCalculationElementDetails.Where(x => x.CalculationElementId == stdVstPgMdlElement.Id && x.IsActive && !x.IsDeleted).ToListAsync();
                    var existCalElmIds = existCalDtil.Select(x => x.TargetElementId).ToList();
                    var elementInExistCalList = await _context.StudyVisitPageModuleElementDetails.Where(x => existCalElmIds.Contains(x.StudyVisitPageModuleElementId) && x.IsActive && !x.IsDeleted).ToListAsync();
                    var calcElmSgs = calcList.Select(x => x.elementFieldSelectedGroup).ToList();
                    var calcElmIds1 = calcElmSgs.Select(x => x.value).ToList();

                    var elementInExistCalListIds = elementInExistCalList.Select(x => x.StudyVisitPageModuleElementId).ToList();
                    var allCalDtil = await _context.studyVisitPageModuleCalculationElementDetails.Where(x => elementInExistCalListIds.Contains(x.TargetElementId) && x.IsActive && !x.IsDeleted).ToListAsync();

                    //update updated variable list
                    foreach (var item in existCalDtil)
                    {
                        var c = calcList.FirstOrDefault(x => x.variableName == item.VariableName);

                        if (c != null && c.elementFieldSelectedGroup.value != item.TargetElementId)
                        {
                            item.TargetElementId = c.elementFieldSelectedGroup.value;
                            _context.studyVisitPageModuleCalculationElementDetails.Update(item);
                        }

                        var cc = calcList.FirstOrDefault(x => x.elementFieldSelectedGroup.value == item.TargetElementId);

                        if (cc != null && cc.variableName != item.VariableName)
                        {
                            item.VariableName = cc.variableName;
                            _context.studyVisitPageModuleCalculationElementDetails.Update(item);
                        }
                    }

                    result.IsSuccess = await _context.SaveCoreContextAsync(model.UserId, DateTimeOffset.Now) > 0;

                    //change elementDetail first
                    foreach (var item in elementInExistCalList)
                    {
                        var a = allCalDtil.Count(x => x.TargetElementId == item.StudyVisitPageModuleElementId);

                        if (a == 1)
                        {
                            item.IsInCalculation = false;
                            _context.StudyVisitPageModuleElementDetails.Update(item);
                        }
                    }

                    var elementInCalList = await _context.StudyVisitPageModuleElementDetails.Where(x => calcElmIds1.Contains(x.StudyVisitPageModuleElementId) && x.IsActive && !x.IsDeleted).ToListAsync();

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
                                VariableName = item.variableName,
                                ReferenceKey = Guid.NewGuid()
                            };

                            _context.studyVisitPageModuleCalculationElementDetails.Add(calcDtil);
                        }
                    }

                    //remove deleted items from calc
                    foreach (var item in existCalDtil)
                    {
                        if (!calcElmIds1.Contains(item.TargetElementId))
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
                    try
                    {
                        var rels = await _context.StudyVisitPageModuleElementEvents.Where(x => x.TargetElementId == model.Id && x.IsActive && !x.IsDeleted).ToListAsync();

                        var relList = JsonSerializer.Deserialize<List<RelationModel>>(model.RelationSourceInputs);
                        var relElmIds = relList.Select(x => x.relationFieldsSelectedGroup.value).ToList();

                        foreach (var item in rels)
                        {
                            var r = relList.FirstOrDefault(x => x.variableName == item.VariableName);

                            if (r != null && r.relationFieldsSelectedGroup.value != item.SourceElementId)
                            {
                                item.SourceElementId = r.relationFieldsSelectedGroup.value;
                                _context.StudyVisitPageModuleElementEvents.Update(item);
                            }
                        }

                        var relIds = rels.Select(x => x.SourceElementId).ToList();

                        //add unadded rows to evet
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
                                    ReferenceKey = Guid.NewGuid()
                                };

                                _context.StudyVisitPageModuleElementEvents.Add(elementEvent);
                            }
                        }

                        //remove deleted rows
                        foreach (var item in rels)
                        {
                            var delRel = relList.FirstOrDefault(x => x.relationFieldsSelectedGroup.value == item.SourceElementId);

                            if (delRel == null)
                            {
                                item.IsActive = false;
                                item.IsDeleted = true;

                                _context.StudyVisitPageModuleElementEvents.Update(item);
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
                    catch (Exception ex)
                    {
                        stdVstPgMdlElement.IsRelated = false;
                        stdVstPgMdlElementDetail.RelationMainJs = "";

                        _context.StudyVisitPageModuleElements.Update(stdVstPgMdlElement);
                        _context.StudyVisitPageModuleElementDetails.Update(stdVstPgMdlElementDetail);
                        result.IsSuccess = await _context.SaveCoreContextAsync(model.UserId, DateTimeOffset.Now) > 0;
                    }
                }
                else
                {
                    var rels = await _context.StudyVisitPageModuleElementEvents.Where(x => x.TargetElementId == model.Id && x.EventType == EventType.Relation && x.IsActive && !x.IsDeleted).ToListAsync();

                    if (rels.Count > 0)
                    {
                        foreach (var item in rels)
                        {
                            item.IsActive = false;
                            item.IsDeleted = true;

                            _context.StudyVisitPageModuleElementEvents.Update(item);
                        }
                    }
                }

                if (model.HasValidation)
                {
                    var dbValidations = await _context.StudyVisitPageModuleElementValidationDetails.Where(x => x.StudyVisitPageModuleElementId == model.Id).ToListAsync();
                    var validations = JsonSerializer.Deserialize<List<ElementValidationModel>>(model.ValidationList);

                    //add or update
                    foreach (var item in validations)
                    {
                        var existVal = dbValidations.FirstOrDefault(x => x.Id == item.Id && x.IsActive && !x.IsDeleted);

                        if (existVal != null)
                        {
                            existVal.Value = item.ValidationValue;
                            existVal.ValueCondition = item.ValidationCondition;
                            existVal.Message = item.ValidationMessage;
                            existVal.ActionType = item.ValidationActionType;

                            _context.StudyVisitPageModuleElementValidationDetails.Update(existVal);
                        }
                        else
                        {
                            var validation = new StudyVisitPageModuleElementValidationDetail
                            {
                                StudyVisitPageModuleElementId = stdVstPgMdlElement.Id,
                                ActionType = item.ValidationActionType,
                                ValueCondition = item.ValidationCondition,
                                Value = item.ValidationValue,
                                Message = item.ValidationMessage,
                                ReferenceKey = Guid.NewGuid()
                            };

                            _context.StudyVisitPageModuleElementValidationDetails.Add(validation);
                        }

                    }

                    //delete
                    foreach (var item in dbValidations)
                    {
                        var existVal = validations.FirstOrDefault(x => x.Id == item.Id);

                        if (existVal == null)
                        {
                            item.IsActive = false;
                            item.IsDeleted = true;

                            _context.StudyVisitPageModuleElementValidationDetails.Update(item);
                        }
                    }

                    result.IsSuccess = await _context.SaveCoreContextAsync(model.UserId, DateTimeOffset.Now) > 0;
                }
                else
                {
                    var dbValidations = await _context.StudyVisitPageModuleElementValidationDetails.Where(x => x.StudyVisitPageModuleElementId == model.Id).ToListAsync();

                    if (dbValidations.Count > 0)
                    {
                        foreach (var item in dbValidations)
                        {
                            item.IsActive = false;
                            item.IsDeleted = true;

                            _context.StudyVisitPageModuleElementValidationDetails.Update(item);
                        }

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
