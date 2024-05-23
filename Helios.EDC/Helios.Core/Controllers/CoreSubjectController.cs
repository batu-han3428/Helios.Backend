using Helios.Common.DTO;
using Helios.Common.Model;
using Helios.Core.Contexts;
using Helios.Core.Domains.Entities;
using MassTransit.Initializers;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Helios.Core.Controllers
{
    [ApiController]
    [Route("[controller]/[action]")]
    public class CoreSubjectController : Controller
    {
        private CoreContext _context;

        public CoreSubjectController(CoreContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<List<SubjectDTO>> GetSubjectList(Int64 studyId, Int64 userId)
        {
            var result = await _context.Subjects.Where(p => p.StudyId == studyId && p.IsActive && !p.IsDeleted)
                .Include(x => x.SubjectVisits)
                .ThenInclude(x => x.SubjectVisitPages)
                .AsNoTracking().Select(x => new SubjectDTO()
                {
                    Id = x.Id,
                    FirstPageId = x.SubjectVisits.FirstOrDefault().SubjectVisitPages.FirstOrDefault().Id,
                    SubjectNumber = x.SubjectNumber,
                    CreatedAt = x.CreatedAt,
                    UpdatedAt = x.UpdatedAt
                }).ToListAsync();

            return result;
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
            var aa= await _context.Sites.Where(x => x.StudyId == model.StudyId && x.IsActive && !x.IsDeleted)
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

            var result = await _context.SubjectVisitPages.Where(x => x.Id == pageId && x.IsActive && !x.IsDeleted)
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
            return result;
        }
    }
}
