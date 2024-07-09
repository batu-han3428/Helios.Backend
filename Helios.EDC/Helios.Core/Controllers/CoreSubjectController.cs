using Helios.Common.DTO;
using Helios.Common.Enums;
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
        private IStudyService _studyService;

        public CoreSubjectController(CoreContext context, IStudyService studyService)
        {
            _context = context;
            _studyService = studyService;
        }

        [HttpGet]
        public async Task<List<SubjectDTO>> GetSubjectList(SubjectListFilterDTO dto)
        {
            var menu = await GetSubjectDetailMenuLocal(dto.StudyId);
            var csRes = await _studyService.SetSubjectDetailMenu(dto.StudyId, menu);

            var result = await _context.Subjects.Where(p => p.StudyId == dto.StudyId && p.IsActive == !dto.ShowArchivedSubjects && !p.IsDeleted)
                .Include(x => x.Site)
                .Include(x => x.SubjectVisits.Where(p => p.IsActive && !p.IsDeleted))
                .ThenInclude(x => x.SubjectVisitPages)
                .AsNoTracking().Select(x => new SubjectDTO()
                {
                    Id = x.Id,
                    FirstPageId = x.SubjectVisits.FirstOrDefault().SubjectVisitPages.FirstOrDefault().StudyVisitPageId,
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

            var role = await _context.StudyUsers.Where(x => x.IsActive && !x.IsDeleted && x.StudyId == dto.StudyId && x.AuthUserId == dto.UserId && x.StudyRole != null).Include(x => x.StudyRole).Select(x => new StudyUsersRolesDTO
            {
                RoleId = x.StudyRole.Id,
                RoleName = x.StudyRole.Name
            }).ToListAsync();

            var userPermissions = await getUserPermission(role.FirstOrDefault().RoleId, dto.StudyId);
            var permRes = _studyService.SetUserPermissions(dto.StudyId, userPermissions);

            return result;
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
                CanMonitoringPageUnFreeze = permissions.Any(perm => perm.PermissionKey == (int)StudyRolePermission.Monitoring_HasPageUnFreeze),
                CanMonitoringPageUnLock = permissions.Any(perm => perm.PermissionKey == (int)StudyRolePermission.Monitoring_HasPageUnLock),
                CanMonitoringQueryView = permissions.Any(perm => perm.PermissionKey == (int)StudyRolePermission.Monitoring_QueryView),
                CanMonitoringRemoteSdv = permissions.Any(perm => perm.PermissionKey == (int)StudyRolePermission.Monitoring_RemoteSdv),
                CanMonitoringSdv = permissions.Any(perm => perm.PermissionKey == (int)StudyRolePermission.Monitoring_Sdv),
                CanMonitoringSeePageActionAudit = permissions.Any(perm => perm.PermissionKey == (int)StudyRolePermission.Monitoring_SeePageActionAudit),
                CanMonitoringVerification = permissions.Any(perm => perm.PermissionKey == (int)StudyRolePermission.Monitoring_Verification),
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

        [HttpGet]
        public async Task<UserPermissionModel> SetUserPermissions(Int64 studyId, Int64 userId)
        {
            var role = await _context.StudyUsers.Where(x => x.IsActive && !x.IsDeleted && x.StudyId == studyId && x.AuthUserId == userId && x.StudyRole != null).Include(x => x.StudyRole).Select(x => new StudyUsersRolesDTO
            {
                RoleId = x.StudyRole.Id,
                RoleName = x.StudyRole.Name
            }).ToListAsync();

            if (role != null && role.Count > 0)
            {
                var userPermissions = await getUserPermission(role.FirstOrDefault().RoleId, studyId);
                var cRes = await _studyService.SetUserPermissions(studyId, userPermissions);

                return userPermissions;
            }

            return new UserPermissionModel();
        }

        [HttpGet]
        public async Task<List<SubjectDetailMenuModel>> SetSubjectDetailMenu(Int64 studyId)
        {
            var menu = await GetSubjectDetailMenuLocal(studyId);
            var csRes = await _studyService.SetSubjectDetailMenu(studyId, menu);

            return menu;
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
                var study = await _context.Studies
                    .Include(x => x.Sites.Where(y => y.IsActive && !y.IsDeleted))
                    .Include(x => x.StudyVisits.Where(y => y.IsActive && !y.IsDeleted))
                    .ThenInclude(x => x.StudyVisitPages.Where(y => y.IsActive && !y.IsDeleted))
                    .ThenInclude(x => x.StudyVisitPageModules.Where(y => y.IsActive && !y.IsDeleted))
                    .ThenInclude(x => x.StudyVisitPageModuleElements.Where(y => y.IsActive && !y.IsDeleted))
                    .ThenInclude(x => x.StudyVisitPageModuleElementDetail)
                    .FirstOrDefaultAsync(x => x.Id == model.StudyId && x.IsActive && !x.IsDeleted);

                var site = study.Sites.FirstOrDefault(x => x.Id == model.SiteId);
                var subjectsInSite = await _context.Subjects.Where(x => x.SiteId == model.SiteId && x.IsActive && !x.IsDeleted).ToListAsync();

                if (site.MaxEnrolmentCount > 0 && site.MaxEnrolmentCount <= subjectsInSite.Count)
                {
                    return new ApiResponse<dynamic>
                    {
                        IsSuccess = false,
                        Message = "Looks up a localized string similar to The patient limit that you can add to the " + site.FullName + " center has been exhausted. Please contact the study admin.."
                    };
                }

                //var sCode = site.CountryCode + "" + site.Code;
                //var dd = subjectsInSite.Where(p => p.SubjectNumber.StartsWith(sCode));
                //var spl = dd.Select(x=>x.SubjectNumber);
                //var maxNumber = dd.Any() ? dd.Max(p => p.Number) : 0;
                //var userResearchCountInSite = maxNumber + 1;
                //var userResearchCountInSite = subjectsInSiteCount + 1;

                var subjectNo = getSubjectNumber(site.CountryCode, site.Code, subjectsInSite.Count + 1, study.SubjectNumberDigitCount);

                var newSubject = new Subject
                {
                    StudyId = study.Id,
                    SiteId = model.SiteId,
                    SubjectNumber = subjectNo,
                    InitialName = model.InitialName,
                    SubjectVisits = study.StudyVisits.Select(studyVisit => new SubjectVisit
                    {
                        StudyVisitId = studyVisit.Id,
                        SubjectVisitPages = studyVisit.StudyVisitPages.Select(stdVstPg => new SubjectVisitPage
                        {
                            StudyVisitPageId = stdVstPg.Id,
                            SubjectVisitPageModules = stdVstPg.StudyVisitPageModules.Select(stdVstPgMdl => new SubjectVisitPageModule
                            {
                                StudyVisitPageModuleId = stdVstPgMdl.Id,
                                SubjectVisitPageModuleElements = stdVstPgMdl.StudyVisitPageModuleElements.Select(stdVstPgMdlElm => new SubjectVisitPageModuleElement
                                {
                                    StudyVisitPageModuleElementId = stdVstPgMdlElm.Id,
                                    DataGridRowId = stdVstPgMdlElm.StudyVisitPageModuleElementDetail.RowIndex
                                }).ToList(),
                            }).ToList(),
                        }).ToList(),
                    }).ToList(),
                };

                _context.Subjects.Add(newSubject);
                var result = await _context.SaveCoreContextAsync(34, DateTimeOffset.Now) > 0;
                return new ApiResponse<dynamic>
                {
                    IsSuccess = true,
                    Message = "Successful",
                    Values = new SubjectDTO()
                    {
                        StudyId = study.Id,
                        Id = newSubject.Id,
                        FirstPageId = newSubject.SubjectVisits.FirstOrDefault().SubjectVisitPages.FirstOrDefault().StudyVisitPageId
                    }
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
                .Include(x => x.StudyVisitPageModuleElement)
                .ThenInclude(x => x.StudyVisitPageModuleElementDetail)
                .Select(e => new SubjectElementModel
                {
                    SubjectId = subjectId,
                    SubjectVisitPageId = pageId,
                    SubjectVisitPageModuleElementId = e.Id,
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
                    AdverseEventType = e.StudyVisitPageModuleElement.StudyVisitPageModuleElementDetail.AdverseEventType,
                    TargetElementId = e.StudyVisitPageModuleElement.StudyVisitPageModuleElementDetail.TargetElementId,
                    UserValue = e.UserValue,
                    ShowOnScreen = e.ShowOnScreen,
                    MissingData = e.MissingData,
                    Sdv = e.Sdv,
                    Query = e.Query
                }).OrderBy(x => x.Order).ToListAsync();

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

            return finalList; 
        }

        [HttpPost]
        public async Task<ApiResponse<dynamic>> AutoSaveSubjectData(SubjectElementShortModel model)
        {
            var result = new ApiResponse<dynamic>();

            var element = await _context.SubjectVisitPageModuleElements.FirstOrDefaultAsync(x => x.Id == model.Id && x.IsActive && !x.IsDeleted);

            if (element != null && model.Value != "" && model.Value != element.UserValue)
            {
                element.UserValue = model.Value;

                _context.SubjectVisitPageModuleElements.Update(element);
                result.IsSuccess = await _context.SaveCoreContextAsync(34, DateTimeOffset.Now) > 0;

                if (result.IsSuccess)
                {
                    var hdnResult = await SetHidden(model.Id);
                    var calcResult = await SetCalculation(model.Id);

                    var subject = await _context.SubjectVisitPageModuleElements.FirstOrDefaultAsync(x => x.Id == model.Id && x.IsActive && !x.IsDeleted).Select(x => x.SubjectVisitModule).Select(x => x.SubjectVisitPage).Select(x => x.SubjectVisit).Select(x => x.Subject);

                    subject.UpdatedAt = DateTimeOffset.UtcNow;
                    _context.Subjects.Update(subject);
                    result.IsSuccess = await _context.SaveCoreContextAsync(34, DateTimeOffset.Now) > 0;
                }

                if (result.IsSuccess)
                    result.Message = "Successfully.";
            }
            //else
            //{
            //    result.IsSuccess = false;
            //    result.Message = "Operation failed!";
            //}

            return result;
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
                var calcDtilTarElmnts = await _context.studyVisitPageModuleCalculationElementDetails.Where(x => stdElmntIds.Contains(x.TargetElementId)).ToListAsync();

                //2-khode calc elementha ke tooshun targetha ro daran
                var calcIds = calcDtilTarElmnts.Select(x => x.CalculationElementId).GroupBy(x => x).Select(x => x.Key).ToList();
                var allCalcDtilCalElmnts = await _context.studyVisitPageModuleCalculationElementDetails.Where(x => calcIds.Contains(x.CalculationElementId)).ToListAsync();
                var allCalcDtilCalElmntIds = allCalcDtilCalElmnts.Select(x => x.TargetElementId).ToList();

                //get all subject elements that used in calculation. i need userValue of these elements. 
                var allSbjElmnts = await _context.SubjectVisitPageModuleElements
                    .Include(x => x.StudyVisitPageModuleElement)
                    .Where(x => (allCalcDtilCalElmntIds.Contains(x.StudyVisitPageModuleElementId) || calcIds.Contains(x.StudyVisitPageModuleElementId))
                && x.SubjectVisitModule.SubjectVisitPage.SubjectVisit.SubjectId == subjectId
                && x.IsActive && !x.IsDeleted).ToListAsync();

                var allCalcStdElmnts = await _context.StudyVisitPageModuleElements
                    .Include(x => x.StudyVisitPageModuleElementDetail)
                    .Where(x => calcIds.Contains(x.Id)).ToListAsync();

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

                    var thisCalSbjElm = allSbjElmnts.FirstOrDefault(x => x.StudyVisitPageModuleElementId == cal.Id);

                    if (thisCalSbjElm != null && thisCalTarElms.Any(x => x.UserValue == "" || x.UserValue == null))
                    {
                        thisCalSbjElm.UserValue = "NaN";
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
                                    finalJs += "var " + dtl.VariableName + "=" + sbjctElm.UserValue + ";";
                            }

                            finalJs += javascriptCode + "}";

                            using (var engine = new V8ScriptEngine())
                            {
                                var mathfnCall = " executeScript();";
                                var mathResult = engine.Evaluate(finalJs + mathfnCall);
                                thisCalSbjElm.UserValue = mathResult.ToString();
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

        private async Task<List<SubjectDetailMenuModel>> GetSubjectDetailMenuLocal(Int64 studyId)
        {
            return await _context.StudyVisits.Where(x => x.StudyId == studyId && x.IsActive && !x.IsDeleted)
                .Include(x => x.StudyVisitPages)
                .Select(visit => new SubjectDetailMenuModel
                {
                    Id = visit.Id,
                    Title = visit.Name,
                    Children = visit.StudyVisitPages
                        .Where(page => page.IsActive && !page.IsDeleted)
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
    }
}
