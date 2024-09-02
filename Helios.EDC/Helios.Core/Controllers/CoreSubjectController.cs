using Helios.Caching.Services.Interfaces;
using Helios.Common.DTO;
using Helios.Common.Enums;
using Helios.Common.Helpers;
using Helios.Common.Helpers.Api;
using Helios.Common.Model;
using Helios.Core.Contexts;
using Helios.Core.Domains.Entities;
using Helios.Core.Services.Interfaces;
using MassTransit.Initializers;
using Microsoft.AspNetCore.Mvc;
using Microsoft.ClearScript.V8;
using Microsoft.EntityFrameworkCore;
using System.Data;

namespace Helios.Core.Controllers
{
    [ApiController]
    [Route("[controller]/[action]")]
    public class CoreSubjectController : Controller
    {
        private CoreContext _context;
        private IUserService _userService;
        private IRedisCacheService _cacheService;

        public CoreSubjectController(CoreContext context, IUserService userService, IRedisCacheService cacheService)
        {
            _context = context;
            _userService = userService;
            _cacheService = cacheService;
        }

        [HttpGet]
        public async Task<List<SubjectDTO>> GetSubjectList(SubjectListFilterDTO dto)
        {
            await SetSubjectDetailMenu(dto.StudyId);

            var roleSite = await _context.StudyUsers.Where(x => x.IsActive && !x.IsDeleted && x.StudyId == dto.StudyId && x.AuthUserId == dto.UserId && x.StudyRole != null).Include(x => x.StudyRole).Include(x => x.StudyUserSites).Select(x => new
            {
                RoleId = x.StudyRole.Id,
                RoleName = x.StudyRole.Name,
                Sites = x.StudyUserSites.Where(a => a.IsActive && !a.IsDeleted).Select(x => x.SiteId).ToList()
            }).ToListAsync();

            var sIds = roleSite.SelectMany(a => a.Sites).ToList();

            var result = await _context.Subjects.Where(p => p.StudyId == dto.StudyId && p.IsActive == !dto.ShowArchivedSubjects && !p.IsDeleted && sIds.Contains(p.SiteId))
                .Include(x => x.Site)
                .Include(x => x.SubjectVisits.Where(p => p.IsActive && !p.IsDeleted))
                .ThenInclude(x => x.SubjectVisitPages.Where(p=>p.IsActive && !p.IsDeleted))
                .AsNoTracking().Select(x => new SubjectDTO()
                {
                    Id = x.Id,
                    FirstPageId = x.SubjectVisits.Where(sv => sv.IsActive && !sv.IsDeleted).OrderBy(sv => sv.StudyVisit.Order).FirstOrDefault().StudyVisit.StudyVisitPages.OrderBy(a => a.Order).Select(a => a.Id).FirstOrDefault(),
                    SubjectNumber = x.SubjectNumber,
                    CreatedAt = x.CreatedAt,
                    UpdatedAt = x.UpdatedAt,
                    Country = x.Site.Country,
                    SiteName = x.Site.Name,
                    RandomData = x.RandomData,
                    AddedById = x.AddedById,
                    InitialName = x.InitialName,
                    IsActive = x.IsActive,
                }).ToListAsync();

            var role = roleSite.Select(x => new StudyUsersRolesDTO
            {
                RoleId = x.RoleId,
                RoleName = x.RoleName
            }).ToList();

            await SetUserPermissions(dto.StudyId, dto.UserId);

            return result;
        }

        [HttpGet]
        public async Task<UserPermissionModel> GetUserPermissions(Int64 studyId, Int64 userId)
        {
            if (studyId != 0)
            {
                string prefix = "Study:Permissions";
                var localCacheKey = prefix + ":" + studyId;

                for (; ; )
                {
                    var value = await _cacheService.GetAsync<UserPermissionModel>(localCacheKey);

                    if (value != null)
                    {
                        return value;
                    }
                    else
                    {
                        await SetUserPermissions(studyId, userId);
                    }
                }
            }
            else
                return null;
        }

        private async Task SetUserPermissions(Int64 studyId, Int64 userId)
        {
            var role = await _context.StudyUsers.Where(x => x.IsActive && !x.IsDeleted && x.StudyId == studyId && x.AuthUserId == userId && x.StudyRole != null).Include(x => x.StudyRole).FirstOrDefaultAsync().Select(y => y.StudyRole);

            var userPermissions = await getUserPermission(role.Id, studyId);
            string prefix = "Study:Permissions";
            var localCacheKey = prefix + ":" + studyId;

            await _cacheService.SetAsync(localCacheKey, userPermissions, new TimeSpan(100, 0, 0));
        }

        private async Task<UserPermissionModel> getUserPermission(Int64 roleId, Int64 studyId)
        {
            var permissions = await _context.Permissions.Where(x => x.StudyRoleId == roleId && x.StudyId == studyId && x.IsActive && !x.IsDeleted).ToListAsync();

            var retVal = new UserPermissionModel
            {
                CanSubjectAdd = permissions.Any(perm => perm.PermissionKey == (int)StudyRolePermission.Subject_Add),
                CanSubjectArchive = permissions.Any(perm => perm.PermissionKey == (int)StudyRolePermission.Subject_Archive),
                CanSubjectChangeState = permissions.Any(perm => perm.PermissionKey == (int)StudyRolePermission.Subject_ChangeState),
                CanSubjectDelete = permissions.Any(perm => perm.PermissionKey == (int)StudyRolePermission.Subject_Delete),
                CanSubjectEdit = permissions.Any(perm => perm.PermissionKey == (int)StudyRolePermission.Subject_Edit),
                CanSubjectExportForm = permissions.Any(perm => perm.PermissionKey == (int)StudyRolePermission.Subject_ExportForm),
                CanSubjectRandomize = permissions.Any(perm => perm.PermissionKey == (int)StudyRolePermission.Subject_Randomize),
                CanSubjectSign = permissions.Any(perm => perm.PermissionKey == (int)StudyRolePermission.Subject_Sign),
                CanSubjectView = permissions.Any(perm => perm.PermissionKey == (int)StudyRolePermission.Subject_View),
                CanSubjectViewEConsent = permissions.Any(perm => perm.PermissionKey == (int)StudyRolePermission.Subject_EConsentView),
                CanSubjectViewRandomization = permissions.Any(perm => perm.PermissionKey == (int)StudyRolePermission.Subject_ViewRandomization),
                CanMonitoringAutoQueryClosed = permissions.Any(perm => perm.PermissionKey == (int)StudyRolePermission.Monitoring_AutoQueryClosed),
                CanMonitoringInputAuditTrail = permissions.Any(perm => perm.PermissionKey == (int)StudyRolePermission.Monitoring_InputAuditTrail),
                CanMonitoringMarkAsNull = permissions.Any(perm => perm.PermissionKey == (int)StudyRolePermission.Monitoring_MarkAsNull),
                CanMonitoringPageFreeze = permissions.Any(perm => perm.PermissionKey == (int)StudyRolePermission.Monitoring_HasPageFreeze),
                CanMonitoringPageLock = permissions.Any(perm => perm.PermissionKey == (int)StudyRolePermission.Monitoring_HasPageLock),
                CanMonitoringQueryView = permissions.Any(perm => perm.PermissionKey == (int)StudyRolePermission.Monitoring_QueryView),
                CanMonitoringSdv = permissions.Any(perm => perm.PermissionKey == (int)StudyRolePermission.Monitoring_Sdv),
                CanMonitoringSeePageActionAudit = permissions.Any(perm => perm.PermissionKey == (int)StudyRolePermission.Monitoring_SeePageActionAudit),
                CanFormAddAdverseEvent = permissions.Any(perm => perm.PermissionKey == (int)StudyRolePermission.Form_AddAdverseEvent),
                CanFormAddMultiVisit = permissions.Any(perm => perm.PermissionKey == (int)StudyRolePermission.Form_AddMultiVisit),
                CanFormAEArchive = permissions.Any(perm => perm.PermissionKey == (int)StudyRolePermission.Form_AEArchive),
                CanFormAERemove = permissions.Any(perm => perm.PermissionKey == (int)StudyRolePermission.Form_AERemove),
                CanFormArchiveMultiVisit = permissions.Any(perm => perm.PermissionKey == (int)StudyRolePermission.Form_ArchiveMultiVisit),
                CanFormRemoveMultiVisit = permissions.Any(perm => perm.PermissionKey == (int)StudyRolePermission.Form_RemoveMultiVisit),
                CanFileDelete = permissions.Any(perm => perm.PermissionKey == (int)StudyRolePermission.FileUpload_CanFileDelete),
                CanFileDownload = permissions.Any(perm => perm.PermissionKey == (int)StudyRolePermission.FileUpload_CanFileDownload),
                CanFileUpload = permissions.Any(perm => perm.PermissionKey == (int)StudyRolePermission.FileUpload_CanFileUpload),
                CanFileView = permissions.Any(perm => perm.PermissionKey == (int)StudyRolePermission.FileUpload_CanFileView),
                CanViewAdverseEventDetailReport = permissions.Any(perm => perm.PermissionKey == (int)StudyRolePermission.DataExport_AdverseEventDetailReport),
                CanViewCommentReport = permissions.Any(perm => perm.PermissionKey == (int)StudyRolePermission.DataExport_CommentReport),
                CanViewCustomCodingReport = permissions.Any(perm => perm.PermissionKey == (int)StudyRolePermission.DataExport_CustomCodingReport),
                CanViewFileAttachmentDetailReport = permissions.Any(perm => perm.PermissionKey == (int)StudyRolePermission.DataExport_FileAttachmentDetailReport),
                CanViewFormDataReport = permissions.Any(perm => perm.PermissionKey == (int)StudyRolePermission.DataExport_FormDataReport),
                CanViewFormDetailReport = permissions.Any(perm => perm.PermissionKey == (int)StudyRolePermission.DataExport_FormDetailReport),
                CanViewFullStudyReport = permissions.Any(perm => perm.PermissionKey == (int)StudyRolePermission.DataExport_FullStudyReport),
                CanViewInputAuditTrailReport = permissions.Any(perm => perm.PermissionKey == (int)StudyRolePermission.DataExport_InputAuditTrailReport),
                CanViewLocalLabReport = permissions.Any(perm => perm.PermissionKey == (int)StudyRolePermission.DataExport_LocalLabReport),
                CanViewLockFreezeStatusReport = permissions.Any(perm => perm.PermissionKey == (int)StudyRolePermission.DataExport_LockFreezeStatusReport),
                CanViewMetadataReport = permissions.Any(perm => perm.PermissionKey == (int)StudyRolePermission.DataExport_MetadataReport),
                CanViewMissingDataReport = permissions.Any(perm => perm.PermissionKey == (int)StudyRolePermission.DataExport_MissingDataReport),
                CanViewMissingDataSummary = permissions.Any(perm => perm.PermissionKey == (int)StudyRolePermission.DataExport_MissingDataSummary),
                CanViewMissingSdvDataReport = permissions.Any(perm => perm.PermissionKey == (int)StudyRolePermission.DataExport_MissingSdvDataReport),
                CanViewMriFileReport = permissions.Any(perm => perm.PermissionKey == (int)StudyRolePermission.DataExport_MriFileReport),
                CanViewQueryReport = permissions.Any(perm => perm.PermissionKey == (int)StudyRolePermission.DataExport_QueryReport),
                CanViewRandomizationAuditTrailReport = permissions.Any(perm => perm.PermissionKey == (int)StudyRolePermission.DataExport_RandomizationAuditTrailReport),
                CanViewRandomizationTreatmentGroupReport = permissions.Any(perm => perm.PermissionKey == (int)StudyRolePermission.DataExport_RandomizationTreatmentGroupReport),
                CanViewSeriousAdverseEventDetailReport = permissions.Any(perm => perm.PermissionKey == (int)StudyRolePermission.DataExport_SeriousAdverseEventDetailReport),
                CanViewStudyReports = permissions.Any(perm => perm.PermissionKey == (int)StudyRolePermission.DataExport_StudyReports),
                CanViewSubjectStateWithRandomization = permissions.Any(perm => perm.PermissionKey == (int)StudyRolePermission.DataExport_SubjectStateWithRandomization),
                CanStudyFoldersView = permissions.Any(perm => perm.PermissionKey == (int)StudyRolePermission.StudyDocument_StudyFoldersView),
                CanDashboardBuilderAdmin = permissions.Any(perm => perm.PermissionKey == (int)StudyRolePermission.Dashboard_DashboardBuilderAdmin),
                CanDashboardBuilderPivotExport = permissions.Any(perm => perm.PermissionKey == (int)StudyRolePermission.Dashboard_DashboardBuilderPivotExport),
                CanDashboardBuilderSourceExport = permissions.Any(perm => perm.PermissionKey == (int)StudyRolePermission.Dashboard_DashboardBuilderSourceExport),
                CanIwrsMarkAsRecieved = permissions.Any(perm => perm.PermissionKey == (int)StudyRolePermission.IWRS_IwrsMarkAsRecieved),
                CanIwrsTransfer = permissions.Any(perm => perm.PermissionKey == (int)StudyRolePermission.IWRS_IwrsTransfer),
                CanMedicalCodingCanCode = permissions.Any(perm => perm.PermissionKey == (int)StudyRolePermission.MedicalCoding_CanCode),
                CanBeTMFAdmin = permissions.Any(perm => perm.PermissionKey == (int)StudyRolePermission.TMF_Admin),
                CanTMFAddPlaceholder = permissions.Any(perm => perm.PermissionKey == (int)StudyRolePermission.TMF_AddPlaceholder),
                CanTMFAddUpload = permissions.Any(perm => perm.PermissionKey == (int)StudyRolePermission.TMF_AddUpload),
                CanTMFApproveRejectFile = permissions.Any(perm => perm.PermissionKey == (int)StudyRolePermission.TMF_ApproveRejectFile),
                CanTMFComment = permissions.Any(perm => perm.PermissionKey == (int)StudyRolePermission.TMF_Comment),
                CanTMFDelete = permissions.Any(perm => perm.PermissionKey == (int)StudyRolePermission.TMF_Delete),
                CanTMFHistory = permissions.Any(perm => perm.PermissionKey == (int)StudyRolePermission.TMF_History),
                CanTMFPreview = permissions.Any(perm => perm.PermissionKey == (int)StudyRolePermission.TMF_Preview),
                CanTMFQualityApproval = permissions.Any(perm => perm.PermissionKey == (int)StudyRolePermission.TMF_QualityApproval),
                CanTMFRequest = permissions.Any(perm => perm.PermissionKey == (int)StudyRolePermission.TMF_Request),
                CanTMFShare = permissions.Any(perm => perm.PermissionKey == (int)StudyRolePermission.TMF_Share),
                CanTMFUnblinded = permissions.Any(perm => perm.PermissionKey == (int)StudyRolePermission.TMF_Unblinded),
                CanTMFUpdate = permissions.Any(perm => perm.PermissionKey == (int)StudyRolePermission.TMF_Update),
                CanTMFViewAuditTrail = permissions.Any(perm => perm.PermissionKey == (int)StudyRolePermission.TMF_ViewAuditTrail),
                CanTMFViewDownload = permissions.Any(perm => perm.PermissionKey == (int)StudyRolePermission.TMF_ViewDownload),
                CanTMFViewFileStatus = permissions.Any(perm => perm.PermissionKey == (int)StudyRolePermission.TMF_ViewFileStatus),
            };

            return retVal;
        }

        private async Task SetSubjectDetailMenu(Int64 studyId)
        {
            var menu = await GetSubjectDetailMenuLocal(studyId);
            string prefix = "Study:Menu";
            var localCacheKey = prefix + ":" + studyId;

            await _cacheService.SetAsync(localCacheKey, menu, new TimeSpan(100, 0, 0));
        }

        [HttpGet]
        public async Task<List<SubjectDetailMenuModel>> GetSubjectDetailMenu(Int64 studyId)
        {
            string prefix = "Study:Menu";
            var localCacheKey = prefix + ":" + studyId;

            for (; ; )
            {
                var value = await _cacheService.GetAsync<List<SubjectDetailMenuModel>>(localCacheKey);

                if (value != null)
                {
                    return value;
                }
                else
                {
                    await SetSubjectDetailMenu(studyId);
                }
            }
        }

        [HttpPost]
        public async Task<ApiResponse<dynamic>> AddSubject(SubjectDTO model)
        {
            if (model.StudyId == 0)
            {
                return new ApiResponse<dynamic>
                {
                    IsSuccess = false,
                    Message = "Operation failed. Please try again."
                };
            }
            else
            {
                BaseDTO baseDTO = Request.Headers.GetBaseInformation();

                var study = await _context.Studies
                    .Include(x => x.Sites.Where(y => y.IsActive && !y.IsDeleted))
                    .Include(x => x.StudyVisits.Where(y => y.IsActive && !y.IsDeleted))
                    .ThenInclude(x => x.StudyVisitPages.Where(y => y.IsActive && !y.IsDeleted))
                    .ThenInclude(x => x.StudyVisitPageModules.Where(y => y.IsActive && !y.IsDeleted))
                    .ThenInclude(x => x.StudyVisitPageModuleElements.Where(y => y.IsActive && !y.IsDeleted))
                    .ThenInclude(x => x.StudyVisitPageModuleElementDetail)
                    .FirstOrDefaultAsync(x => x.Id == model.StudyId && x.IsActive && !x.IsDeleted);

                var site = study.Sites.FirstOrDefault(x => x.Id == model.SiteId);
                var subjectsInSite = await _context.Subjects.Where(x => x.SiteId == model.SiteId && !x.IsDeleted).ToListAsync();

                if (site.MaxEnrolmentCount > 0 && site.MaxEnrolmentCount <= subjectsInSite.Count)
                {
                    return new ApiResponse<dynamic>
                    {
                        IsSuccess = false,
                        Message = "Looks up a localized string similar to The patient limit that you can add to the " + site.FullName + " center has been exhausted. Please contact the study admin.."
                    };
                }

                var sCode = site.CountryCode + "" + site.Code;
                var dd = subjectsInSite.Where(p => p.SubjectNumber.StartsWith(sCode));
                var spl = dd.Select(x => x.SubjectNumber);
                var maxNumber = 0;

                if (dd.Any())
                {
                    var nm = spl.Select(x => x.Substring(sCode.Length)).ToList();
                    maxNumber = nm.Max(x => int.Parse(x));
                }

                var subjectCountInSite = maxNumber + 1;

                var subjectNo = getSubjectNumber(site.CountryCode, site.Code, subjectCountInSite, study.SubjectNumberDigitCount);

                for (; ; )
                {
                    var s = await _context.Subjects.Where(x => x.SubjectNumber == subjectNo && x.SiteId == model.SiteId && x.IsActive && !x.IsDeleted).FirstOrDefaultAsync();

                    if (s == null)
                        break;
                    else
                    {
                        subjectCountInSite++;
                        subjectNo = getSubjectNumber(site.CountryCode, site.Code, subjectCountInSite, study.SubjectNumberDigitCount);
                    }
                }

                var newSubject = new Subject
                {
                    StudyId = study.Id,
                    SiteId = model.SiteId,
                    SubjectNumber = subjectNo,
                    InitialName = model.InitialName,
                    TenantId = baseDTO.TenantId,
                    SubjectVisits = study.StudyVisits.Select(studyVisit => new SubjectVisit
                    {
                        StudyVisitId = studyVisit.Id,
                        TenantId = baseDTO.TenantId,
                        SubjectVisitPages = studyVisit.StudyVisitPages.Select(stdVstPg => new SubjectVisitPage
                        {
                            StudyVisitPageId = stdVstPg.Id,
                            TenantId = baseDTO.TenantId,
                            SubjectVisitPageModules = stdVstPg.StudyVisitPageModules.Select(stdVstPgMdl => new SubjectVisitPageModule
                            {
                                StudyVisitPageModuleId = stdVstPgMdl.Id,
                                TenantId = baseDTO.TenantId,
                                SubjectVisitPageModuleElements = stdVstPgMdl.StudyVisitPageModuleElements.Select(stdVstPgMdlElm => new SubjectVisitPageModuleElement
                                {
                                    StudyVisitPageModuleElementId = stdVstPgMdlElm.Id,
                                    DataGridRowId = stdVstPgMdlElm.StudyVisitPageModuleElementDetail.RowIndex,
                                    TenantId = baseDTO.TenantId,
                                }).ToList(),
                            }).ToList(),
                        }).ToList(),
                    }).ToList(),
                };

                _context.Subjects.Add(newSubject);
                var result = await _context.SaveCoreContextAsync(baseDTO.UserId, DateTimeOffset.Now) > 0;
                return new ApiResponse<dynamic>
                {
                    IsSuccess = true,
                    Message = site.MaxEnrolmentCount == 0 ? "This subject created with subject number {SubjectNo}." : "This subject created with subject number {SubjectNo}. Your remaining subject addition limit for this site : {n}",
                    Values = new SubjectDTO()
                    {
                        StudyId = study.Id,
                        Id = newSubject.Id,
                        FirstPageId = newSubject.SubjectVisits.FirstOrDefault().SubjectVisitPages.FirstOrDefault().StudyVisitPageId,
                        SubjectNumber = newSubject.SubjectNumber,
                        AddedById = site.MaxEnrolmentCount - subjectsInSite.Count
                    }
                };
            }
        }

        [HttpPost]
        public async Task<ApiResponse<dynamic>> AddDatagridSubjectElements(Int64 datagridId)
        {
            try
            {
                var datagrid = await _context.SubjectVisitPageModuleElements.FirstOrDefaultAsync(x => x.Id == datagridId);

                if (datagrid != null)
                {
                    var childElements = await _context.SubjectVisitPageModuleElements.Where(x => x.IsActive && !x.IsDeleted && datagrid.SubjectVisitModuleId == x.SubjectVisitModuleId && x.StudyVisitPageModuleElement.StudyVisitPageModuleElementDetail.ParentId == datagrid.StudyVisitPageModuleElementId && x.DataGridRowId == _context.SubjectVisitPageModuleElements
                        .Where(y => y.IsActive && !y.IsDeleted &&
                                    datagrid.SubjectVisitModuleId == y.SubjectVisitModuleId &&
                                    y.StudyVisitPageModuleElement.StudyVisitPageModuleElementDetail.ParentId == datagrid.StudyVisitPageModuleElementId)
                        .Max(y => y.DataGridRowId)).Include(x => x.StudyVisitPageModuleElement).ToListAsync();

                    await _context.SubjectVisitPageModuleElements.AddRangeAsync(childElements.Select(x => new SubjectVisitPageModuleElement
                    {
                        SubjectVisitModuleId = datagrid.SubjectVisitModuleId,
                        StudyVisitPageModuleElementId = x.StudyVisitPageModuleElementId,
                        DataGridRowId = x.DataGridRowId + 1
                    }));

                    BaseDTO baseDTO = Request.Headers.GetBaseInformation();

                    var result = await _context.SaveCoreContextAsync(baseDTO.UserId, DateTimeOffset.Now) > 0;

                    return new ApiResponse<dynamic>
                    {
                        IsSuccess = result,
                        Message = result ? "Successful" : "Unsuccessful"
                    };
                }

                return new ApiResponse<dynamic>
                {
                    IsSuccess = false,
                    Message = "An unexpected error occurred."
                };
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

        [HttpPost]
        public async Task<ApiResponse<dynamic>> RemoveDatagridSubjectElements(DatagridRemoveDTO dto)
        {
            try
            {
                var datagrid = await _context.SubjectVisitPageModuleElements.FirstOrDefaultAsync(x => x.Id == dto.datagridId);

                var subjectElements = await _context.SubjectVisitPageModuleElements.Where(x => x.IsActive && !x.IsDeleted && datagrid.SubjectVisitModuleId == x.SubjectVisitModuleId && x.StudyVisitPageModuleElement.StudyVisitPageModuleElementDetail.ParentId == datagrid.StudyVisitPageModuleElementId && x.DataGridRowId == dto.datagridRowId).ToListAsync();

                if (subjectElements.Count < 1) return new ApiResponse<dynamic>
                {
                    IsSuccess = false,
                    Message = "An unexpected error occurred."
                };

                foreach (var item in subjectElements)
                {
                    if (dto.singleLine) item.UserValue = null;
                    else
                    {
                        item.IsActive = false;
                        item.IsDeleted = true;
                    }
                }

                _context.SubjectVisitPageModuleElements.UpdateRange(subjectElements);

                BaseDTO baseDTO = Request.Headers.GetBaseInformation();

                var result = await _context.SaveCoreContextAsync(baseDTO.UserId, DateTimeOffset.Now) > 0;

                return new ApiResponse<dynamic>
                {
                    IsSuccess = result,
                    Message = result ? "Successful" : "Unsuccessful"
                };
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

        [HttpPost]
        public async Task<List<SiteModel>> GetSites(SubjectDTO model)
        {
            var sites = await _context.Sites.Where(x => x.StudyId == model.StudyId && x.IsActive && !x.IsDeleted)
              .Select(site => new SiteModel
              {
                  Id = site.Id,
                  Name = site.Name,
              }).ToListAsync();

            return sites;
        }

        private string getSubjectNumber(string countryCode, string site, int subjectNumberInSite, int? subjectNumberDigitCount = 4)
        {
            var subjectNumber = "";

            if (subjectNumberDigitCount != 4 && subjectNumberDigitCount != null)
            {
                subjectNumber = countryCode + site + subjectNumberInSite.ToString("D" + subjectNumberDigitCount);
            }
            else
            {
                subjectNumber = countryCode + site + subjectNumberInSite.ToString("D4");
            }

            return subjectNumber;
        }

        //[HttpGet]
        //public async Task<List<SubjectDetailMenuModel>> GetSubjectDetailMenu(Int64 subjectId)
        //{
        //    return await _context.SubjectVisits.Include(sv => sv.SubjectVisitPages.Where(page => page.IsActive && !page.IsDeleted))
        //        .Where(x => x.IsActive && !x.IsDeleted && x.SubjectId == subjectId)
        //        .Select(visit => new SubjectDetailMenuModel
        //        {
        //            Id = visit.Id,
        //            Title = visit.StudyVisit.Name,
        //            Children = visit.SubjectVisitPages
        //                .Where(page => page.IsActive && !page.IsDeleted)
        //                .Select(page => new SubjectDetailMenuModel
        //                {
        //                    Id = page.Id,
        //                    Title = page.StudyVisitPage.Name
        //                })
        //                .ToList()
        //        }).ToListAsync();
        //}


        [HttpGet]
        public async Task<bool> GetStudyAskSubjectInitial(Int64 studyId)
        {
            var study = await _context.Studies.FirstOrDefaultAsync(x => x.Id == studyId && x.IsActive && !x.IsDeleted);
            return study.AskSubjectInitial;
        }

        [HttpGet]
        public async Task<List<SubjectElementModel>> GetSubjectElementList(Int64 subjectId, Int64 pageId)
        {
            var finalList = new List<SubjectElementModel>();

            var studyVisit = await _context.StudyVisitPages.Where(x => x.Id == pageId && x.IsActive && !x.IsDeleted).FirstOrDefaultAsync();

            var result = await _context.SubjectVisitPages.Where(x => x.StudyVisitPageId == studyVisit.Id && x.SubjectVisit.SubjectId == subjectId && x.IsActive && !x.IsDeleted)
                .SelectMany(x => x.SubjectVisitPageModules.Where(y => y.IsActive && !y.IsDeleted))
                .SelectMany(x => x.SubjectVisitPageModuleElements.Where(y => y.IsActive && !y.IsDeleted))
                .Include(x => x.SubjectVisitPageModuleElementComments)
                .Include(x => x.StudyVisitPageModuleElement)
                .ThenInclude(x => x.StudyVisitPageModuleElementDetail)
                .OrderBy(a => a.StudyVisitPageModuleElement.Order)
                .Select(e => new SubjectElementModel
                {
                    ModuleOrder = e.StudyVisitPageModuleElement.StudyVisitPageModule.Order,
                    SubjectId = subjectId,
                    SubjectVisitPageId = pageId,
                    SubjectVisitPageModuleElementId = e.Id,
                    SubjectVisitPageModuleId = e.SubjectVisitModuleId,
                    StudyVisitPageModuleElementId = e.StudyVisitPageModuleElementId,
                    StudyVisitPageModuleElementDetailId = e.StudyVisitPageModuleElement.StudyVisitPageModuleElementDetail.Id,
                    ParentId = e.StudyVisitPageModuleElement.StudyVisitPageModuleElementDetail.ParentId,
                    ElementType = e.StudyVisitPageModuleElement.ElementType,
                    ElementName = e.StudyVisitPageModuleElement.ElementName,
                    Title = e.StudyVisitPageModuleElement.Title,
                    IsTitleHidden = e.StudyVisitPageModuleElement.IsTitleHidden,
                    Order = e.StudyVisitPageModuleElement.Order,
                    Description = e.StudyVisitPageModuleElement.Description,
                    Width = e.StudyVisitPageModuleElement.Width,
                    IsHidden = e.StudyVisitPageModuleElement.IsHidden,
                    IsRequired = e.StudyVisitPageModuleElement.IsRequired,
                    IsDependent = e.StudyVisitPageModuleElement.IsDependent,
                    IsRelated = e.StudyVisitPageModuleElement.IsRelated,
                    IsReadonly = e.StudyVisitPageModuleElement.IsReadonly,
                    CanMissing = e.StudyVisitPageModuleElement.CanMissing,
                    ButtonText = e.StudyVisitPageModuleElement.StudyVisitPageModuleElementDetail.ButtonText,
                    DefaultValue = e.StudyVisitPageModuleElement.StudyVisitPageModuleElementDetail.DefaultValue,
                    Unit = e.StudyVisitPageModuleElement.StudyVisitPageModuleElementDetail.Unit,
                    Mask = e.StudyVisitPageModuleElement.StudyVisitPageModuleElementDetail.Mask,
                    LowerLimit = e.StudyVisitPageModuleElement.StudyVisitPageModuleElementDetail.LowerLimit,
                    UpperLimit = e.StudyVisitPageModuleElement.StudyVisitPageModuleElementDetail.UpperLimit,
                    Layout = e.StudyVisitPageModuleElement.StudyVisitPageModuleElementDetail.Layout,
                    StartDay = e.StudyVisitPageModuleElement.StudyVisitPageModuleElementDetail.StartDay,
                    EndDay = e.StudyVisitPageModuleElement.StudyVisitPageModuleElementDetail.EndDay,
                    StartMonth = e.StudyVisitPageModuleElement.StudyVisitPageModuleElementDetail.StartMonth,
                    EndMonth = e.StudyVisitPageModuleElement.StudyVisitPageModuleElementDetail.EndMonth,
                    StartYear = e.StudyVisitPageModuleElement.StudyVisitPageModuleElementDetail.StartYear,
                    EndYear = e.StudyVisitPageModuleElement.StudyVisitPageModuleElementDetail.EndYear,
                    AddTodayDate = e.StudyVisitPageModuleElement.StudyVisitPageModuleElementDetail.AddTodayDate,
                    ElementOptions = e.StudyVisitPageModuleElement.StudyVisitPageModuleElementDetail.ElementOptions,
                    LeftText = e.StudyVisitPageModuleElement.StudyVisitPageModuleElementDetail.LeftText,
                    RightText = e.StudyVisitPageModuleElement.StudyVisitPageModuleElementDetail.RightText,
                    MainJs = e.StudyVisitPageModuleElement.StudyVisitPageModuleElementDetail.MainJs,
                    RowCount = e.StudyVisitPageModuleElement.StudyVisitPageModuleElementDetail.RowCount,
                    ColumnCount = e.StudyVisitPageModuleElement.StudyVisitPageModuleElementDetail.ColumnCount,
                    DatagridAndTableProperties = e.StudyVisitPageModuleElement.StudyVisitPageModuleElementDetail.DatagridAndTableProperties,
                    RowIndex = e.StudyVisitPageModuleElement.StudyVisitPageModuleElementDetail.RowIndex,
                    ColumnIndex = e.StudyVisitPageModuleElement.StudyVisitPageModuleElementDetail.ColunmIndex,
                    DataGridRowId = e.DataGridRowId,
                    AdverseEventType = e.StudyVisitPageModuleElement.StudyVisitPageModuleElementDetail.AdverseEventType,
                    TargetElementId = e.StudyVisitPageModuleElement.StudyVisitPageModuleElementDetail.TargetElementId,
                    UserValue = e.UserValue,
                    ShowOnScreen = e.ShowOnScreen,
                    MissingData = e.MissingData,
                    Sdv = e.Sdv,
                    Query = e.Query,
                    IsComment = e.SubjectVisitPageModuleElementComments.Any(comment => comment.IsActive && !comment.IsDeleted)
                }).ToListAsync();

            foreach (var item in result)
            {
                if (item.ParentId == 0 || item.ParentId == null)
                    finalList.Add(item);
                else
                {
                    var parent = result.FirstOrDefault(x => x.StudyVisitPageModuleElementId == item.ParentId);

                    parent.ChildElements.Add(item);
                }
            }

            foreach (var item in finalList)
            {
                if (item.ElementType == Common.Enums.ElementType.DataGrid)
                {
                    if (item.ChildElements.Count > 0)
                    {
                        var cnt = item.ChildElements.GroupBy(x => x.StudyVisitPageModuleElementId).FirstOrDefault().Count();

                        item.RowCount = cnt;
                    }
                    else
                    {
                        item.RowCount = 0;
                    }
                }
            }

            return finalList.OrderBy(x=>x.ModuleOrder).ToList();
        }

        [HttpPost]
        public async Task<ApiResponse<dynamic>> AutoSaveSubjectData(SubjectElementShortModel model)
        {
            try
            {
                var element = await _context.SubjectVisitPageModuleElements.FirstOrDefaultAsync(x => x.Id == model.Id && x.IsActive && !x.IsDeleted);

                if (element != null && model.Value != element.UserValue)
                {
                    element.UserValue = model.Value;
                    element.MissingData = false;
                    element.Sdv = false;
                    _context.SubjectVisitPageModuleElements.Update(element);
                    var result = await _context.SaveCoreContextAsync(34, DateTimeOffset.Now) > 0;

                    if (result)
                    {
                        var hdnResult = await SetHidden(model.Id);
                        var calcResult = await SetCalculation(model.Id);

                        var subject = await _context.SubjectVisitPageModuleElements.FirstOrDefaultAsync(x => x.Id == model.Id && x.IsActive && !x.IsDeleted).Select(x => x.SubjectVisitModule).Select(x => x.SubjectVisitPage).Select(x => x.SubjectVisit).Select(x => x.Subject);

                        subject.UpdatedAt = DateTimeOffset.UtcNow;
                        _context.Subjects.Update(subject);
                        var result1 = await _context.SaveCoreContextAsync(34, DateTimeOffset.Now) > 0;

                        return new ApiResponse<dynamic>
                        {
                            IsSuccess = result1,
                            Message = result1 ? "Successful" : "Unsuccessful"
                        };
                    }
                }
                else if (element != null && model.Value == element.UserValue)
                {
                    return new ApiResponse<dynamic>
                    {
                        IsSuccess = false,
                        Message = "No changes were made. Please make changes to save."
                    };
                }

                return new ApiResponse<dynamic>
                {
                    IsSuccess = false,
                    Message = "An unexpected error occurred."
                };
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

        private async Task<bool> SetHidden(Int64 elementId)
        {
            var result = false;

            var element = await _context.SubjectVisitPageModuleElements
                .Include(x => x.StudyVisitPageModuleElement)
                .ThenInclude(x => x.StudyVisitPageModuleElementDetail)
                .Include(x => x.SubjectVisitModule)
                .ThenInclude(x => x.SubjectVisitPage)
                .ThenInclude(x => x.SubjectVisit)
                .FirstOrDefaultAsync(x => x.Id == elementId && x.IsActive && !x.IsDeleted);

            var targets = await _context.StudyVisitPageModuleElementDetails.Where(x => x.TargetElementId == element.StudyVisitPageModuleElementId && x.IsActive && !x.IsDeleted).ToListAsync();

            if (targets != null && targets.Count > 0)
            {
                var targetIds = targets.Select(x => x.StudyVisitPageModuleElementId).ToList();
                var targetSbjcts = await _context.SubjectVisitPageModuleElements
                    .Include(x => x.StudyVisitPageModuleElement)
                    .Where(x => targetIds.Contains(x.StudyVisitPageModuleElementId) && x.SubjectVisitModule.SubjectVisitPage.SubjectVisit.SubjectId == element.SubjectVisitModule.SubjectVisitPage.SubjectVisit.SubjectId && x.IsActive && !x.IsDeleted).ToListAsync();

                foreach (var item in targetSbjcts)
                {
                    item.UserValue = element.UserValue;
                    _context.SubjectVisitPageModuleElements.Update(item);
                }

                result = await _context.SaveCoreContextAsync(34, DateTimeOffset.Now) > 0;
            }

            return result;
        }

        private async Task<bool> SetCalculation(Int64 elementId)
        {
            var result = false;

            var sbjElement = await _context.SubjectVisitPageModuleElements
                .Include(x => x.SubjectVisitModule)
                .ThenInclude(x => x.SubjectVisitPage)
                .ThenInclude(x => x.SubjectVisit)
                .ThenInclude(x => x.Subject)
                .Include(x => x.StudyVisitPageModuleElement)
                .ThenInclude(x => x.StudyVisitPageModuleElementDetail)
                .FirstOrDefaultAsync(x => x.Id == elementId && x.IsActive && !x.IsDeleted);

            //hidden elements that related to this element
            var relatadHdnStdElmnts = await _context.StudyVisitPageModuleElementDetails.Where(x => x.TargetElementId == sbjElement.StudyVisitPageModuleElementId && x.IsActive && !x.IsDeleted).ToListAsync();

            if (sbjElement.StudyVisitPageModuleElement.StudyVisitPageModuleElementDetail.IsInCalculation || (relatadHdnStdElmnts.Count > 0 && relatadHdnStdElmnts.Any(x => x.IsInCalculation)))
            {
                var finalCalcVal = "";
                var subjectId = sbjElement.SubjectVisitModule.SubjectVisitPage.SubjectVisit.SubjectId;
                var relatadHdnStdElmntsIds = relatadHdnStdElmnts.Where(x => x.IsInCalculation).Select(x => x.StudyVisitPageModuleElementId).ToList();

                var stdElmntIds = new List<Int64>
                {
                    sbjElement.StudyVisitPageModuleElementId
                };

                stdElmntIds.AddRange(relatadHdnStdElmntsIds);
                //tu calcDetil bayad 2 ta query bezanim:
                //1-cal ba targetId inayi ke darim(element va hiddenash)
                var calcDtilTarElmnts = await _context.studyVisitPageModuleCalculationElementDetails.Where(x => stdElmntIds.Contains(x.TargetElementId) && x.IsActive && !x.IsDeleted).ToListAsync();

                //2-khode calc elementha ke tooshun targetha ro daran
                var calcIds = calcDtilTarElmnts.Select(x => x.CalculationElementId).GroupBy(x => x).Select(x => x.Key).ToList();
                var allCalcDtilCalElmnts = await _context.studyVisitPageModuleCalculationElementDetails.Where(x => calcIds.Contains(x.CalculationElementId) && x.IsActive && !x.IsDeleted).ToListAsync();
                var allCalcDtilCalElmntIds = allCalcDtilCalElmnts.Select(x => x.TargetElementId).ToList();

                //get all subject elements that used in calculation. i need userValue of these elements. 
                var allSbjElmnts = await _context.SubjectVisitPageModuleElements
                    .Include(x => x.StudyVisitPageModuleElement)
                    .Where(x => (allCalcDtilCalElmntIds.Contains(x.StudyVisitPageModuleElementId) || calcIds.Contains(x.StudyVisitPageModuleElementId))
                && x.SubjectVisitModule.SubjectVisitPage.SubjectVisit.SubjectId == subjectId
                && x.IsActive && !x.IsDeleted).ToListAsync();

                var allCalcStdElmnts = await _context.StudyVisitPageModuleElements
                    .Include(x => x.StudyVisitPageModuleElementDetail)
                    .Where(x => calcIds.Contains(x.Id) && x.IsActive && !x.IsDeleted).ToListAsync();

                //var allTarCalcStdElmnts = await _context.StudyVisitPageModuleElements
                //    .Include(x => x.StudyVisitPageModuleElementDetail)
                //    .Where(x => stdElmntIds.Contains(x.Id)).ToListAsync();

                foreach (var cal in allCalcStdElmnts)
                {
                    var thisCalElms = allCalcDtilCalElmnts.Where(y => y.CalculationElementId == cal.Id).ToList();
                    //var thisCalElmIds = thisCalElms.Select(x => x.TargetElementId).ToList();
                    var thisCalElmIds = allCalcDtilCalElmnts.Where(x => x.CalculationElementId == cal.Id).Select(x => x.TargetElementId).ToList();
                    var thisCalTarElms = allSbjElmnts.Where(x => thisCalElmIds.Contains(x.StudyVisitPageModuleElementId)
                    //var thisCalTarElms = allSbjElmnts.Where(x => x.StudyVisitPageModuleElementId
                    && x.StudyVisitPageModuleElement.ElementType != Common.Enums.ElementType.Calculated).ToList();

                    var thisCalSbjElm = allSbjElmnts.FirstOrDefault(x => x.StudyVisitPageModuleElementId == cal.Id && x.Id != cal.Id);

                    if (thisCalSbjElm != null && thisCalTarElms.Any(x => x.UserValue == "" || x.UserValue == null))
                    {
                        thisCalSbjElm.UserValue = "";
                        thisCalSbjElm.Sdv = false;
                    }
                    else
                    {
                        var finalJs = "function executeScript(){";
                        var javascriptCode = cal.StudyVisitPageModuleElementDetail.MainJs;

                        try
                        {
                            foreach (var dtl in thisCalElms)
                            {
                                var sbjctElm = allSbjElmnts.FirstOrDefault(x => x.StudyVisitPageModuleElementId == dtl.TargetElementId && x.StudyVisitPageModuleElement.ElementType != Common.Enums.ElementType.Calculated);

                                if (sbjctElm != null && sbjctElm.UserValue != null && sbjctElm.UserValue != "")
                                    finalJs += "var " + dtl.VariableName + "='" + sbjctElm.UserValue + "';";
                            }

                            finalJs += javascriptCode + "}";

                            using (var engine = new V8ScriptEngine())
                            {
                                var mathfnCall = " executeScript();";
                                var mathResult = engine.Evaluate(finalJs + mathfnCall);
                                thisCalSbjElm.UserValue = mathResult.ToString();
                                if("[undefined]" != mathResult.ToString()) thisCalSbjElm.Sdv = true;
                            }
                        }
                        catch (Exception ex)
                        {
                            return false;
                        }

                        _context.SubjectVisitPageModuleElements.Update(thisCalSbjElm);
                    }
                }

                result = await _context.SaveCoreContextAsync(34, DateTimeOffset.Now) > 0;
            }

            return result;
        }

        [HttpPost]
        public async Task<bool> SetDependentElementValue(string elementIdString, Int64 pageId, Int64 subjectId)
        {
            try
            {
                string[] elementIdsArray = elementIdString.Split(',');
                if (elementIdsArray.Length < 1 || pageId == 0) return false;
                List<Int64> elementIds = new List<Int64>();
                foreach (string id in elementIdsArray)
                {
                    if (Int64.TryParse(id, out Int64 l))
                    {
                        elementIds.Add(l);
                    }
                }

                BaseDTO baseDTO = Request.Headers.GetBaseInformation();

                var visits = await _context.SubjectVisits.Where(x => x.IsActive && !x.IsDeleted && x.SubjectId == subjectId)
                    .Include(x => x.SubjectVisitPages.Where(x => x.IsActive && !x.IsDeleted && x.StudyVisitPageId == pageId))
                    .ThenInclude(x => x.SubjectVisitPageModules.Where(x => x.IsActive && !x.IsDeleted))
                    .ThenInclude(x => x.SubjectVisitPageModuleElements.Where(x => elementIds.Contains(x.Id))).ToListAsync();

                var subjectElements = visits.SelectMany(x => x.SubjectVisitPages).SelectMany(x => x.SubjectVisitPageModules).SelectMany(x => x.SubjectVisitPageModuleElements).ToList();

                if (subjectElements.Count() < 1) return false;

                subjectElements.ForEach(element =>
                {
                    if (element.UserValue != null && element.UserValue != "") element.UserValue = null;
                });

                _context.SubjectVisitPageModuleElements.UpdateRange(subjectElements);

                return await _context.SaveCoreContextAsync(baseDTO.UserId, DateTimeOffset.Now) > 0;
            }
            catch (Exception)
            {
                return false;
            }
        }

        [HttpPost]
        public async Task<bool> SetDependentPageElementValue(string pageIdString, Int64 subjectId)
        {
            try
            {
                string[] pageIdsArray = pageIdString.Split(',');
                if (pageIdsArray.Length < 1 || subjectId == 0) return false;
                List<Int64> pageIds = new List<Int64>();
                foreach (string id in pageIdsArray)
                {
                    if (Int64.TryParse(id, out Int64 l))
                    {
                        pageIds.Add(l);
                    }
                }

                BaseDTO baseDTO = Request.Headers.GetBaseInformation();

                var visits = await _context.SubjectVisits.Where(x => x.IsActive && !x.IsDeleted && x.SubjectId == subjectId)
                    .Include(x => x.SubjectVisitPages.Where(x => x.IsActive && !x.IsDeleted && pageIds.Contains(x.StudyVisitPageId)))
                    .ThenInclude(x => x.SubjectVisitPageModules.Where(x => x.IsActive && !x.IsDeleted))
                    .ThenInclude(x => x.SubjectVisitPageModuleElements.Where(x => x.IsActive && !x.IsDeleted)).ToListAsync();

                var subjectElements = visits.SelectMany(x => x.SubjectVisitPages).SelectMany(x => x.SubjectVisitPageModules).SelectMany(x => x.SubjectVisitPageModuleElements).ToList();

                if (subjectElements.Count() < 1) return false;

                subjectElements.ForEach(element =>
                {
                    if (element.UserValue != null && element.UserValue != "") element.UserValue = null;
                });

                _context.SubjectVisitPageModuleElements.UpdateRange(subjectElements);

                return await _context.SaveCoreContextAsync(baseDTO.UserId, DateTimeOffset.Now) > 0;
            }
            catch (Exception)
            {
                return false;
            }
        }

        private async Task<List<SubjectDetailMenuModel>> GetSubjectDetailMenuLocal(Int64 studyId)
        {
            return await _context.StudyVisits.Where(x => x.StudyId == studyId && x.IsActive && !x.IsDeleted).OrderBy(x=>x.Order)
                .Include(x => x.StudyVisitPages)
                .Select(visit => new SubjectDetailMenuModel
                {
                    Id = visit.Id,
                    Title = visit.Name,
                    Children = visit.StudyVisitPages
                        .Where(page => page.IsActive && !page.IsDeleted)
                        .OrderBy(page=>page.Order)
                        .Select(page => new SubjectDetailMenuModel
                        {
                            Id = page.Id,
                            Title = page.Name
                        })
                        .ToList()
                }).ToListAsync();
        }

        [HttpPost]
        public async Task<ApiResponse<dynamic>> DeleteOrArchiveSubject(SubjectArchiveOrDeleteModel model)
        {
            var result = new ApiResponse<dynamic>();

            var subject = await _context.Subjects
                .Include(x => x.SubjectVisits)
                .ThenInclude(x => x.SubjectVisitPages)
                .ThenInclude(x => x.SubjectVisitPageModules)
                .ThenInclude(x => x.SubjectVisitPageModuleElements)
                .Where(x => x.Id == model.SubjectId && x.IsActive && !x.IsDeleted).FirstOrDefaultAsync();

            if (subject != null)
            {
                if (model.IsDelete)
                {
                    subject.IsActive = false;
                    subject.IsDeleted = true;

                    foreach (var sv in subject.SubjectVisits)
                    {
                        sv.IsActive = false;
                        sv.IsDeleted = true;

                        _context.SubjectVisits.Update(sv);

                        foreach (var svp in sv.SubjectVisitPages)
                        {
                            svp.IsActive = false;
                            svp.IsDeleted = true;

                            _context.SubjectVisitPages.Update(svp);

                            foreach (var svpm in svp.SubjectVisitPageModules)
                            {
                                svpm.IsActive = false;
                                svpm.IsDeleted = true;

                                _context.SubjectVisitPageModules.Update(svpm);

                                foreach (var svpme in svpm.SubjectVisitPageModuleElements)
                                {
                                    svpme.IsActive = false;
                                    svpme.IsDeleted = true;

                                    _context.SubjectVisitPageModuleElements.Update(svpme);
                                }
                            }
                        }
                    }
                }
                else
                {
                    subject.IsActive = false;

                    foreach (var sv in subject.SubjectVisits)
                    {
                        sv.IsActive = false;

                        _context.SubjectVisits.Update(sv);

                        foreach (var svp in sv.SubjectVisitPages)
                        {
                            svp.IsActive = false;

                            _context.SubjectVisitPages.Update(svp);

                            foreach (var svpm in svp.SubjectVisitPageModules)
                            {
                                svpm.IsActive = false;

                                _context.SubjectVisitPageModules.Update(svpm);

                                foreach (var svpme in svpm.SubjectVisitPageModuleElements)
                                {
                                    svpme.IsActive = false;

                                    _context.SubjectVisitPageModuleElements.Update(svpme);
                                }
                            }
                        }
                    }
                }

                subject.Comment = model.Comment;

                _context.Subjects.Update(subject);
                result.IsSuccess = await _context.SaveCoreContextAsync(34, DateTimeOffset.Now) > 0;

                if (result.IsSuccess)
                {
                    result.Message = "Successful";
                }
                else
                {
                    result.Message = "Error";
                }
            }
            else
            {
                result.IsSuccess = false;
                result.Message = "Try again please!";
            }

            return result;
        }

        [HttpPost]
        public async Task<ApiResponse<dynamic>> UnArchiveSubject(SubjectArchiveOrDeleteModel model)
        {
            var result = new ApiResponse<dynamic>();

            var subject = await _context.Subjects
                .Include(x => x.SubjectVisits)
                .ThenInclude(x => x.SubjectVisitPages)
                .ThenInclude(x => x.SubjectVisitPageModules)
                .ThenInclude(x => x.SubjectVisitPageModuleElements)
                .Where(x => x.Id == model.SubjectId && !x.IsDeleted).FirstOrDefaultAsync();

            if (subject != null)
            {
                subject.IsActive = true;

                foreach (var sv in subject.SubjectVisits)
                {
                    sv.IsActive = true;

                    _context.SubjectVisits.Update(sv);

                    foreach (var svp in sv.SubjectVisitPages)
                    {
                        svp.IsActive = true;

                        _context.SubjectVisitPages.Update(svp);

                        foreach (var svpm in svp.SubjectVisitPageModules)
                        {
                            svpm.IsActive = true;

                            _context.SubjectVisitPageModules.Update(svpm);

                            foreach (var svpme in svpm.SubjectVisitPageModuleElements)
                            {
                                svpme.IsActive = true;

                                _context.SubjectVisitPageModuleElements.Update(svpme);
                            }
                        }
                    }
                }

                subject.Comment = model.Comment;

                _context.Subjects.Update(subject);
                result.IsSuccess = await _context.SaveCoreContextAsync(34, DateTimeOffset.Now) > 0;

                if (result.IsSuccess)
                {
                    result.Message = "Successfully";
                }
                else
                {
                    result.Message = "Error";
                }
            }
            else
            {
                result.IsSuccess = false;
                result.Message = "Try again please!";
            }

            return result;
        }

        [HttpGet]
        public async Task<StudyVisitAnnotatedCrfModel> GetSubjectVisitAnnotatedCrf(Int64 subjectId)
        {
            try
            {
                BaseDTO baseDTO = Request.Headers.GetBaseInformation();

                StudyVisitAnnotatedCrfModel model = new StudyVisitAnnotatedCrfModel();

                var subjectData = await _context.Subjects.Where(x => x.IsActive && !x.IsDeleted && x.Id == subjectId && x.StudyId == 8)
                    .Include(x => x.SubjectVisits.Where(a => a.IsActive && !a.IsDeleted))
                    .ThenInclude(x => x.SubjectVisitPages.Where(a => a.IsActive && !a.IsDeleted))
                    .ThenInclude(x => x.SubjectVisitPageModules.Where(a => a.IsActive && !a.IsDeleted))
                    .ThenInclude(x => x.SubjectVisitPageModuleElements.Where(a => a.IsActive && !a.IsDeleted /*&& (a.UserValue != null && a.UserValue != "")*/))
                    .ThenInclude(x => x.StudyVisitPageModuleElement)
                    .ThenInclude(x => x.StudyVisitPageModuleElementDetail).ToListAsync();

                if (subjectData.Count < 1)
                {
                    return model;
                }

                var subjectPages = subjectData.SelectMany(x => x.SubjectVisits).SelectMany(x => x.SubjectVisitPages).ToList();

                var emptyModuleIds = subjectPages
                .Where(page => page.SubjectVisitPageModules.All(module => !module.SubjectVisitPageModuleElements.Where(x => x.UserValue != null && x.UserValue != "").Any()))
                .SelectMany(page => page.SubjectVisitPageModules.Select(module => module.StudyVisitPageModuleId))
                .ToList();

                var study = await _context.Studies.Where(x => x.IsActive && !x.IsDeleted && x.Id == 8).Select(study => new Study
                {
                    ProtocolCode = study.ProtocolCode,
                    StudyVisits = study.StudyVisits.Where(visit => visit.IsActive && !visit.IsDeleted).OrderBy(visit => visit.Order).Select(visit => new StudyVisit
                    {
                        Id = visit.Id,
                        Name = visit.Name,
                        StudyVisitPages = visit.StudyVisitPages.Where(page => page.IsActive && !page.IsDeleted).OrderBy(page => page.Order).Select(page => new StudyVisitPage
                        {
                            Id = page.Id,
                            Name = page.Name,
                            StudyVisitPageModules = page.StudyVisitPageModules.Where(module => module.IsActive && !module.IsDeleted && !emptyModuleIds.Contains(module.Id)).OrderBy(module => module.Order).Select(module => new StudyVisitPageModule
                            {
                                Id = module.Id,
                                Name = module.Name,
                                StudyVisitPageModuleElements = module.StudyVisitPageModuleElements.Where(elm => elm.IsActive && !elm.IsDeleted && elm.ElementType != Common.Enums.ElementType.Hidden).OrderBy(elm => elm.Order).Select(elm => new StudyVisitPageModuleElement
                                {
                                    Id = elm.Id,
                                    Title = elm.Title,
                                    ElementName = elm.ElementName,
                                    Description = elm.Description,
                                    IsRequired = elm.IsRequired,
                                    ElementType = elm.ElementType,
                                    StudyVisitPageModuleElementDetail = elm.StudyVisitPageModuleElementDetail != null && elm.StudyVisitPageModuleElementDetail.IsActive && !elm.StudyVisitPageModuleElementDetail.IsDeleted
                                                ? new StudyVisitPageModuleElementDetail
                                                {
                                                    LowerLimit = elm.StudyVisitPageModuleElementDetail.LowerLimit,
                                                    UpperLimit = elm.StudyVisitPageModuleElementDetail.UpperLimit,
                                                    ParentId = elm.StudyVisitPageModuleElementDetail.ParentId,
                                                    ElementOptions = elm.StudyVisitPageModuleElementDetail.ElementOptions,
                                                    ColunmIndex = elm.StudyVisitPageModuleElementDetail.ColunmIndex,
                                                    DatagridAndTableProperties = elm.StudyVisitPageModuleElementDetail.DatagridAndTableProperties,
                                                    RowCount = elm.StudyVisitPageModuleElementDetail.RowCount,
                                                }
                                                : null,
                                }).ToList()
                            }).ToList()
                        }).ToList()
                    }).ToList()
                }).AsNoTracking().AsSplitQuery().FirstOrDefaultAsync();

                if (study != null)
                {
                    StudyAnnotatedCrfModel studyModel = new StudyAnnotatedCrfModel
                    {
                        ProtocolCode = study.ProtocolCode,
                        SubjectNumber = subjectData.FirstOrDefault().SubjectNumber
                    };

                    List<VisitAnnotatedCrfModel> visitModel = new List<VisitAnnotatedCrfModel>();

                    var elements = study.StudyVisits.SelectMany(x => x.StudyVisitPages).SelectMany(x => x.StudyVisitPageModules).SelectMany(x => x.StudyVisitPageModuleElements);

                    var chls = elements.Where(x => x?.StudyVisitPageModuleElementDetail?.ParentId != 0).ToList();

                    List<Int64> ids = new List<Int64>();

                    chls.ForEach(chl =>
                    {
                        var parentElm = elements.FirstOrDefault(x => chl?.StudyVisitPageModuleElementDetail?.ParentId == x.Id);
                        if (parentElm != null && parentElm.ElementType == Common.Enums.ElementType.DataGrid)
                        {
                            ids.Add(chl.Id);
                        }
                    });

                    visitModel.AddRange(study.StudyVisits.Select(visit => new VisitAnnotatedCrfModel
                    {
                        Title = visit.Name,
                        Children = visit.StudyVisitPages.Select(page => new VisitAnnotatedCrfModel
                        {
                            Title = page.Name,
                            Children = page.StudyVisitPageModules.Select(module => new VisitAnnotatedCrfModel
                            {
                                Title = module.Name,
                                Children = module.StudyVisitPageModuleElements.Where(x => !ids.Contains(x.Id)).Select(elm =>
                                {
                                    VisitAnnotatedCrfModel newElm = new VisitAnnotatedCrfModel();
                                    newElm.Title = elm.Title != "" ? elm.Title : elm.ElementName;
                                    newElm.Input = elm.ElementType;
                                    newElm.Description = elm.Description;
                                    newElm.IsRequired = elm.IsRequired;
                                    newElm.ElementOptions = elm.StudyVisitPageModuleElementDetail != null ? elm.StudyVisitPageModuleElementDetail.ElementOptions : "";
                                    newElm.LowerLimit = elm.StudyVisitPageModuleElementDetail != null ? elm.StudyVisitPageModuleElementDetail.LowerLimit : "";
                                    newElm.UpperLimit = elm.StudyVisitPageModuleElementDetail != null ? elm.StudyVisitPageModuleElementDetail.UpperLimit : "";

                                    var gh = subjectData.SelectMany(x => x.SubjectVisits).Where(x => x.StudyVisitId == visit.Id).SelectMany(x => x.SubjectVisitPages).Where(x => x.StudyVisitPageId == page.Id).SelectMany(x => x.SubjectVisitPageModules).Where(x => x.StudyVisitPageModuleId == module.Id).SelectMany(x => x.SubjectVisitPageModuleElements).FirstOrDefault(x => x.UserValue != null && x.UserValue != "" && x.StudyVisitPageModuleElementId == elm.Id);

                                    newElm.UserValue = gh?.UserValue;

                                    if (elm.ElementType == Common.Enums.ElementType.DataGrid)
                                    {
                                        var children = elements.Where(x => x?.StudyVisitPageModuleElementDetail?.ParentId == elm.Id).OrderBy(x => x?.StudyVisitPageModuleElementDetail?.ColunmIndex != null ? int.Parse(x?.StudyVisitPageModuleElementDetail?.ColunmIndex.ToString()) : int.MaxValue).ToList();

                                        var datp = System.Text.Json.JsonSerializer.Deserialize<List<DatagridAndTableProperties>>(elm?.StudyVisitPageModuleElementDetail?.DatagridAndTableProperties);

                                        var rowCount = subjectData.SelectMany(x => x.SubjectVisits).Where(x => x.StudyVisitId == visit.Id).SelectMany(x => x.SubjectVisitPages).Where(x => x.StudyVisitPageId == page.Id).SelectMany(x => x.SubjectVisitPageModules).Where(x => x.StudyVisitPageModuleId == module.Id).SelectMany(x => x.SubjectVisitPageModuleElements).Where(x => x.StudyVisitPageModuleElement.StudyVisitPageModuleElementDetail.ParentId == elm.Id).Max(x => x.DataGridRowId);

                                        Dictionary<string, DatagridAndTableDicVal> datagridAndTableValue = new Dictionary<string, DatagridAndTableDicVal>();
                                        for (int i = 0; i < rowCount; i++)
                                        {
                                            int index = 1;
                                            datp?.ForEach(d =>
                                            {
                                                var chl = children.FirstOrDefault(x => x?.StudyVisitPageModuleElementDetail?.ColunmIndex == index);
                                                string elmType = "";
                                                if (chl != null)
                                                {
                                                    var subElm = subjectData.SelectMany(x => x.SubjectVisits).Where(x => x.StudyVisitId == visit.Id).SelectMany(x => x.SubjectVisitPages).Where(x => x.StudyVisitPageId == page.Id).SelectMany(x => x.SubjectVisitPageModules).Where(x => x.StudyVisitPageModuleId == module.Id).SelectMany(x => x.SubjectVisitPageModuleElements).FirstOrDefault(x => x.StudyVisitPageModuleElementId == chl.Id && x.DataGridRowId == (i + 1));
                                                    if (chl.ElementType == Common.Enums.ElementType.RadioList || chl.ElementType == Common.Enums.ElementType.DropDown || chl.ElementType == Common.Enums.ElementType.CheckList || chl.ElementType == Common.Enums.ElementType.DropDownMulti)
                                                    {
                                                        elmType = chl?.StudyVisitPageModuleElementDetail?.ElementOptions;
                                                    }
                                                    else
                                                    {
                                                        elmType = chl.ElementType.ToString();
                                                    }
                                                    datagridAndTableValue.Add(Guid.NewGuid().ToString(), new DatagridAndTableDicVal { ColonName = d.title + "/" + (i + 1).ToString(), ElementType = elmType, Value = subElm != null ? subElm.UserValue : "" });
                                                }
                                                else
                                                {
                                                    datagridAndTableValue.Add(Guid.NewGuid().ToString(), new DatagridAndTableDicVal { ColonName = d.title + "/" + (i + 1).ToString(), ElementType = elmType, Value = "" });
                                                }
                                                index++;
                                            });
                                        }
                                        newElm.DatagridAndTableValue = datagridAndTableValue;
                                    }
                                    return newElm;
                                }).ToList()
                            }).ToList()
                        }).ToList()
                    }).ToList());
                    model.StudyModel = studyModel;
                    model.VisitModel = visitModel;
                }

                return model;
            }
            catch (Exception e)
            {

                throw;
            }
        }

        [HttpGet]
        public async Task<List<SubjectCommentModel>> GetSubjectComments(Int64 subjectElementId)
        {
            var comments = await _context.SubjectVisitPageModuleElementComments.Where(comment => comment.IsActive && !comment.IsDeleted && comment.SubjectVisitPageModuleElementId == subjectElementId).Select(comment => new SubjectCommentModel
            {
                Id = comment.Id,
                Comment = comment.Comment,
                CommentTime = comment.UpdatedAt != new DateTimeOffset(new DateTime(1, 1, 1), TimeSpan.Zero) ? comment.UpdatedAt : comment.CreatedAt,
                SenderId = comment.AddedById
            }).ToListAsync();

            if (comments.Count > 0)
            {
                var users = await _userService.GetUserList(comments.Select(c => c.SenderId).ToList());
                if (users.IsSuccessful)
                {
                    comments.ForEach(c =>
                    {
                        var user = users.Data.FirstOrDefault(u => u.Id == c.SenderId);
                        if (user != null)
                        {
                            c.SenderName = user.Name + " " + user.LastName;
                        }
                    });
                }
            }

            return comments;
        }

        [HttpPost]
        public async Task<ApiResponse<dynamic>> SetSubjectComment(SubjectCommentDTO dto)
        {
            try
            {
                BaseDTO baseDTO = Request.Headers.GetBaseInformation();

                if (dto.Id == 0)
                {
                    await _context.SubjectVisitPageModuleElementComments.AddAsync(new SubjectVisitPageModuleElementComments
                    {
                        SubjectVisitPageModuleElementId = dto.ElementId,
                        TenantId = baseDTO.TenantId,
                        Comment = dto.Comment.Trim()
                    });
                }
                else
                {
                    var comment = await _context.SubjectVisitPageModuleElementComments.FirstOrDefaultAsync(comment => comment.IsActive && !comment.IsDeleted && comment.Id == dto.Id);

                    if (comment != null)
                    {
                        comment.Comment = dto.Comment.Trim();

                        _context.SubjectVisitPageModuleElementComments.Update(comment);
                    }

                }

                var result = await _context.SaveCoreContextAsync(baseDTO.UserId, DateTimeOffset.Now) > 0;

                return new ApiResponse<dynamic>
                {
                    IsSuccess = result,
                    Message = result ? "Successful" : "Unsuccessful"
                };
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

        [HttpPost]
        public async Task<ApiResponse<dynamic>> RemoveSubjectComment(Int64 id)
        {
            try
            {
                if (id == 0)
                {
                    return new ApiResponse<dynamic>
                    {
                        IsSuccess = false,
                        Message = "An unexpected error occurred."
                    };
                }

                var comment = await _context.SubjectVisitPageModuleElementComments.FirstOrDefaultAsync(comment => comment.IsActive && !comment.IsDeleted && comment.Id == id);

                if (comment == null)
                {
                    return new ApiResponse<dynamic>
                    {
                        IsSuccess = false,
                        Message = "An unexpected error occurred."
                    };
                }

                BaseDTO baseDTO = Request.Headers.GetBaseInformation();

                _context.SubjectVisitPageModuleElementComments.Remove(comment);

                var result = await _context.SaveCoreContextAsync(baseDTO.UserId, DateTimeOffset.Now) > 0;

                return new ApiResponse<dynamic>
                {
                    IsSuccess = result,
                    Message = result ? "Successful" : "Unsuccessful"
                };
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

        [HttpPost]
        public async Task<ApiResponse<dynamic>> SetSubjectMissingData(SubjectMissingDataDTO dto)
        {
            try
            {
                if (dto.IsPage)
                {
                    var elements = await (from elm in _context.SubjectVisitPageModuleElements
                                          join detail in _context.StudyVisitPageModuleElementDetails on elm.StudyVisitPageModuleElementId equals detail.StudyVisitPageModuleElementId into details
                                          from detail in details.DefaultIfEmpty()
                                          join parentElm in _context.StudyVisitPageModuleElements on detail.ParentId equals parentElm.Id into parentElms
                                          from parentElm in parentElms.DefaultIfEmpty()
                                          where elm.IsActive
                                            && !elm.IsDeleted
                                            && (elm.UserValue == null || elm.UserValue == "")
                                            && !elm.MissingData
                                            && elm.StudyVisitPageModuleElement.CanMissing
                                            && elm.StudyVisitPageModuleElement.StudyVisitPageModule.StudyVisitPageId == dto.Id
                                            && elm.SubjectVisitModule.SubjectVisitPage.SubjectVisit.SubjectId == dto.SubjectId
                                            && elm.StudyVisitPageModuleElement.ElementType != ElementType.Hidden
                                            && elm.StudyVisitPageModuleElement.ElementType != ElementType.Calculated
                                            && elm.StudyVisitPageModuleElement.ElementType != ElementType.File
                                            && elm.StudyVisitPageModuleElement.ElementType != ElementType.ConcomitantMedication
                                            && elm.StudyVisitPageModuleElement.ElementType != ElementType.AdversEventElement
                                            && elm.StudyVisitPageModuleElement.ElementType != ElementType.Randomization
                                            && !elm.StudyVisitPageModuleElement.IsDependent
                                            && (parentElm == null || !parentElm.IsDependent)
                                          select elm).ToListAsync();

                    if (elements.Count < 1) return new ApiResponse<dynamic> { IsSuccess = false, Message = "There is no blank space to mark as missing data." };

                    elements.ForEach(elm =>
                    {
                        elm.UserValue = dto.Value;
                        elm.MissingData = true;
                        elm.Sdv = false;
                    });

                    _context.SubjectVisitPageModuleElements.UpdateRange(elements);
                }
                else
                {
                    var elm = await _context.SubjectVisitPageModuleElements.FirstOrDefaultAsync(elm => elm.IsActive && !elm.IsDeleted && elm.Id == dto.Id);

                    if (elm == null) return new ApiResponse<dynamic> { IsSuccess = false, Message = "An unexpected error occurred." };

                    elm.UserValue = dto.Value;
                    elm.MissingData = true;
                    elm.Sdv = false;

                    _context.SubjectVisitPageModuleElements.Update(elm);
                }

                BaseDTO baseDTO = Request.Headers.GetBaseInformation();

                var result = await _context.SaveCoreContextAsync(baseDTO.UserId, DateTimeOffset.Now) > 0;

                return new ApiResponse<dynamic>
                {
                    IsSuccess = result,
                    Message = result ? "Successful" : "Unsuccessful"
                };
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

        [HttpPost]
        public async Task<ApiResponse<dynamic>> SetSubjectSdv(List<Int64> ids)
        {
            try
            {
                if (ids.Count < 1)
                {
                    return new ApiResponse<dynamic>
                    {
                        IsSuccess = false,
                        Message = "An unexpected error occurred."
                    };
                }

                var elms = await _context.SubjectVisitPageModuleElements.Where(elm => elm.IsActive && !elm.IsDeleted && ids.Contains(elm.Id)).ToListAsync();

                if (elms.Count < 1) return new ApiResponse<dynamic> { IsSuccess = false, Message = "An unexpected error occurred." };

                bool isSingleElement = elms.Count == 1;

                elms.ForEach(elm =>
                {
                    elm.Sdv = isSingleElement ? !elm.Sdv : true;
                });

                _context.SubjectVisitPageModuleElements.UpdateRange(elms);

                BaseDTO baseDTO = Request.Headers.GetBaseInformation();

                var result = await _context.SaveCoreContextAsync(baseDTO.UserId, DateTimeOffset.Now) > 0;

                return new ApiResponse<dynamic>
                {
                    IsSuccess = result,
                    Message = result ? "Successful" : "Unsuccessful"
                };
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
        public async Task<List<SdvModel>> GetSubjectSdvList()
        {
            try
            {
                BaseDTO baseDTO = Request.Headers.GetBaseInformation();

                var sites = await _context.StudyUsers.Where(x => x.IsActive && !x.IsDeleted && x.StudyId == baseDTO.StudyId /*8*/ && x.AuthUserId == baseDTO.UserId /*34*/).SelectMany(x => x.StudyUserSites.Where(a => a.IsActive && !a.IsDeleted)).AsNoTracking().Select(x => x.SiteId).ToListAsync();

                var excludedElementTypes = new List<int> { 1, 17, 14, 15, 16, 3, 18 };

                return await _context.SubjectVisitPageModuleElements
                    .Where(elm => elm.IsActive && !elm.IsDeleted && sites.Contains(elm.SubjectVisitModule.SubjectVisitPage.SubjectVisit.Subject.SiteId) && !excludedElementTypes.Contains((int)elm.StudyVisitPageModuleElement.ElementType) && elm.SubjectVisitModule.IsActive && !elm.SubjectVisitModule.IsDeleted && elm.SubjectVisitModule.SubjectVisitPage.IsActive && !elm.SubjectVisitModule.SubjectVisitPage.IsDeleted && elm.SubjectVisitModule.SubjectVisitPage.SubjectVisit.IsActive && !elm.SubjectVisitModule.SubjectVisitPage.SubjectVisit.IsDeleted && elm.SubjectVisitModule.SubjectVisitPage.SubjectVisit.Subject.IsActive && !elm.SubjectVisitModule.SubjectVisitPage.SubjectVisit.Subject.IsDeleted)
                    .GroupBy(elm => new
                    {
                        SubjectNo = elm.SubjectVisitModule.SubjectVisitPage.SubjectVisit.Subject.SubjectNumber,
                        SiteName = elm.SubjectVisitModule.SubjectVisitPage.SubjectVisit.Subject.Site.Name,
                        Visit = elm.SubjectVisitModule.SubjectVisitPage.SubjectVisit.StudyVisit.Name,
                        Page = elm.SubjectVisitModule.SubjectVisitPage.StudyVisitPage.Name,
                        PageId = elm.SubjectVisitModule.SubjectVisitPage.StudyVisitPageId,
                        SubjectId = elm.SubjectVisitModule.SubjectVisitPage.SubjectVisit.SubjectId
                    })
                    .Select(g => new SdvModel
                    {
                        SubjectNo = g.Key.SubjectNo,
                        SiteName = g.Key.SiteName,
                        VisitName = g.Key.Visit,
                        PageName = g.Key.Page,
                        SubjectId = g.Key.SubjectId,
                        PageId = g.Key.PageId,
                        SdvStatus = g.All(e => e.Sdv == true) ? SdvStatus.SdvDone : g.Any(e => e.Sdv == true) ? SdvStatus.SdvReady : SdvStatus.SdvPartial
                    }).AsNoTracking().ToListAsync();
            }
            catch (Exception)
            {
                return new List<SdvModel>();
            }
        }
    }
}
