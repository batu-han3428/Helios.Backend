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
        public async Task<SubjectListModel> GetSubjectList(Int64 studyId, Int64 userId)
        {
            var subjectListModel = new SubjectListModel();
            var menu = await GetSubjectDetailMenuLocal(studyId);
            var csRes = await _studyService.SetSubjectDetailMenu(studyId, menu);
            var result = await _context.Subjects.Where(p => p.StudyId == studyId && p.IsActive && !p.IsDeleted)
                .Include(x => x.Site)
                .Include(x => x.SubjectVisits)
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
                    InitialName = x.InitialName
                }).ToListAsync();

            var role = await _context.StudyUsers.Where(x => x.IsActive && !x.IsDeleted && x.StudyId == studyId && x.AuthUserId == userId && x.StudyRole != null).Include(x => x.StudyRole).Select(x => new StudyUsersRolesDTO
            {
                RoleId = x.StudyRole.Id,
                RoleName = x.StudyRole.Name
            }).ToListAsync();
            var permissions = await _context.Permissions.Where(x => x.StudyRoleId == role.FirstOrDefault().RoleId && x.StudyId == studyId).ToListAsync();
            subjectListModel.SubjectList = result;
            subjectListModel.HasQuery = permissions.Any(x => x.PermissionKey == (int)StudyRolePermission.Monitoring_QueryView);
            subjectListModel.HasRandomizasyon = permissions.Any(x => x.PermissionKey == (int)StudyRolePermission.Subject_Randomize && x.PermissionKey == (int)StudyRolePermission.Subject_ViewRandomization);
            subjectListModel.HasSdv = permissions.Any(x => x.PermissionKey == (int)StudyRolePermission.Monitoring_Sdv && x.PermissionKey == (int)StudyRolePermission.Monitoring_Verification && x.PermissionKey == (int)StudyRolePermission.Monitoring_RemoteSdv);
            return subjectListModel;
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
                    .Include(x => x.Sites)
                    .Include(x => x.StudyVisits)
                    .ThenInclude(x => x.StudyVisitPages)
                    .ThenInclude(x => x.StudyVisitPageModules)
                    .ThenInclude(x => x.StudyVisitPageModuleElements)
                    .ThenInclude(x => x.StudyVisitPageModuleElementDetail)
                    .FirstOrDefaultAsync(x => x.Id == model.StudyId);

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

            }

            return new ApiResponse<dynamic>
            {
                IsSuccess = true,
                Message = "Successful",
            };
        }
        [HttpPost]
        public async Task<List<SiteModel>> GetSites(SubjectDTO model)
        {
            var aa = await _context.Sites.Where(x => x.StudyId == model.StudyId && x.IsActive && !x.IsDeleted)
              .Select(site => new SiteModel
              {
                  Id = site.Id,
                  Name = site.Name,
              }).ToListAsync();
            return aa;
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

        [HttpGet]
        public async Task<List<SubjectDetailMenuModel>> GetSubjectDetailMenu(Int64 subjectId)
        {
            return await _context.SubjectVisits.Include(sv => sv.SubjectVisitPages.Where(page => page.IsActive && !page.IsDeleted))
                .Where(x => x.IsActive && !x.IsDeleted && x.SubjectId == subjectId)
                .Select(visit => new SubjectDetailMenuModel
                {
                    Id = visit.Id,
                    Title = visit.StudyVisit.Name,
                    Children = visit.SubjectVisitPages
                        .Where(page => page.IsActive && !page.IsDeleted)
                        .Select(page => new SubjectDetailMenuModel
                        {
                            Id = page.Id,
                            Title = page.StudyVisitPage.Name
                        })
                        .ToList()
                }).ToListAsync();
        }

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
                .SelectMany(x => x.SubjectVisitPageModules)
                .SelectMany(x => x.SubjectVisitPageModuleElements)
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

                var hh = await SetHidden(model.Id);
                var aa = await SetCalculation(model.Id);

                if (result.IsSuccess)
                    result.Message = "Successfully.";
            }
            else
            {
                result.IsSuccess = false;
                result.Message = "Operation failed!";
            }

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

            var targets = await _context.StudyVisitPageModuleElementDetails.Where(x => x.TargetElementId == element.StudyVisitPageModuleElementId).ToListAsync();

            if (targets != null && targets.Count > 0)
            {
                var targetIds = targets.Select(x => x.Id).ToList();
                var targetSbjcts = await _context.SubjectVisitPageModuleElements.Where(x => targetIds.Contains(x.StudyVisitPageModuleElementId) && x.SubjectVisitModule.SubjectVisitPage.SubjectVisit.SubjectId == element.SubjectVisitModule.SubjectVisitPage.SubjectVisit.SubjectId && x.IsActive && !x.IsDeleted).ToListAsync();

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

            var element = await _context.SubjectVisitPageModuleElements
                .Include(x => x.SubjectVisitModule)
                .ThenInclude(x => x.SubjectVisitPage)
                .ThenInclude(x => x.SubjectVisit)
                .ThenInclude(x => x.Subject)
                .Include(x => x.StudyVisitPageModuleElement)
                .ThenInclude(x => x.StudyVisitPageModuleElementDetail)
                .FirstOrDefaultAsync(x => x.Id == elementId && x.IsActive && !x.IsDeleted);

            if (element.StudyVisitPageModuleElement.StudyVisitPageModuleElementDetail.IsInCalculation)
            {
                var finalCalcVal = "";
                var subjectId = element.SubjectVisitModule.SubjectVisitPage.SubjectVisit.SubjectId;

                var hiddens = await _context.StudyVisitPageModuleElements.Where(x => x.StudyVisitPageModuleCalculationElementDetails.Any(y => y.TargetElementId == element.StudyVisitPageModuleElementId) && x.ElementType == Common.Enums.ElementType.Hidden).ToListAsync();
                var hiddenIds = hiddens.Select(x => x.Id).ToList();

                var allUsedCalcDetails = await _context.studyVisitPageModuleCalculationElementDetails.Where(x => (x.TargetElementId == element.StudyVisitPageModuleElementId || hiddenIds.Contains(x.TargetElementId)) && x.IsActive && !x.IsDeleted).ToListAsync();// bu element hangi calculationlerde kullanilmis.

                var calcIdsInCalcDtils = allUsedCalcDetails.Select(x => x.CalculationElementId).ToList();
                var allTargetIdsInCalcDtils = allUsedCalcDetails.Select(x => x.TargetElementId).ToList();

                var allStdCalcElements = await _context.StudyVisitPageModuleElements
                    .Include(x => x.StudyVisitPageModuleElementDetail)
                    .Include(x => x.StudyVisitPageModuleCalculationElementDetails)
                    .Where(x => calcIdsInCalcDtils.Contains(x.Id) && x.IsActive && !x.IsDeleted).ToListAsync();

                //var allStdTarElements = await _context.StudyVisitPageModuleElements
                //    .Include(x => x.StudyVisitPageModuleElementDetail)
                //    .Include(x => x.StudyVisitPageModuleCalculationElementDetails)
                //    .Where(x => allTargetIdsInCalcDtils.Contains(x.Id) && x.IsActive && !x.IsDeleted).ToListAsync();

                //var allStdTarElementsIds = allStdTarElements.Select(x => x.Id).ToList();

                //allStdCalcElements.AddRange(hiddens);
                var allCalcElementsIds = allStdCalcElements.Select(x => x.Id).ToList();
                var allOtherStdTarElements = await _context.studyVisitPageModuleCalculationElementDetails
                    .Where(x => allCalcElementsIds.Contains(x.CalculationElementId) && x.IsActive && !x.IsDeleted).ToListAsync();
                var allStdTarElementsIds = allOtherStdTarElements.Select(x => x.TargetElementId).ToList();

                //var allUsedSbjElements = await _context.SubjectVisitPageModuleElements
                //        .Include(x => x.StudyVisitPageModuleElement)
                //        .ThenInclude(x => x.StudyVisitPageModuleCalculationElementDetails.Where(x => allCalcElementsIds.Contains(x.CalculationElementId) && x.IsActive && !x.IsDeleted))
                //        .Include(x => x.SubjectVisitModule)
                //        .ThenInclude(x => x.SubjectVisitPage)
                //        .ThenInclude(x => x.SubjectVisit)
                //        .ThenInclude(x => x.Subject)
                //        .Where(x=>x.SubjectVisitModule.SubjectVisitPage.SubjectVisit.SubjectId == subjectId && x.IsActive && !x.IsDeleted)
                //        .ToListAsync();

                var allUsedSbjElements = await _context.SubjectVisitPageModuleElements
                        .Include(x => x.StudyVisitPageModuleElement)
                        .Include(x => x.SubjectVisitModule)
                        .ThenInclude(x => x.SubjectVisitPage)
                        .ThenInclude(x => x.SubjectVisit)
                        .ThenInclude(x => x.Subject)
                        .Where(x => allStdTarElementsIds.Contains(x.StudyVisitPageModuleElementId)
                        && x.SubjectVisitModule.SubjectVisitPage.SubjectVisit.SubjectId == subjectId
                        && x.IsActive && !x.IsDeleted)
                        .ToListAsync();

                //var allStdUsedElement = await _context.studyVisitPageModuleCalculationElementDetails.Where(x => allCalcElementsIds.Contains(x.CalculationElementId) && x.IsActive && !x.IsDeleted).ToListAsync();

                foreach (var cal in allStdCalcElements)
                {
                    var thisCalElms = allOtherStdTarElements.Where(y => y.CalculationElementId == cal.Id).ToList();
                    var thisCalElmIds = thisCalElms.Select(x => x.TargetElementId).ToList();
                    var thisCalTarElms = allUsedSbjElements.Where(x => thisCalElmIds.Contains(x.StudyVisitPageModuleElementId)
                    && x.StudyVisitPageModuleElement.ElementType != Common.Enums.ElementType.Calculated).ToList();

                    var thisCalSbjElm = allUsedSbjElements.FirstOrDefault(x => x.StudyVisitPageModuleElementId == cal.Id);

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
                                var sbjctElm = allUsedSbjElements.FirstOrDefault(x => x.StudyVisitPageModuleElementId == dtl.TargetElementId && x.StudyVisitPageModuleElement.ElementType != Common.Enums.ElementType.Calculated);

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
                        result = await _context.SaveCoreContextAsync(34, DateTimeOffset.Now) > 0;
                    }
                }
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
    }
}
