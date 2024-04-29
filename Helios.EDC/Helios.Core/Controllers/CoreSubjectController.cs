using Helios.Common.DTO;
using Helios.Core.Contexts;
using Helios.Core.Domains.Entities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Helios.Core.Controllers
{
    public class CoreSubjectController : Controller
    {
        private CoreContext _context;

        public CoreSubjectController(CoreContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<List<SubjectDTO>> GetSubjectList(Int64 studyId)
        {
            var result = await _context.Subjects.Where(p => p.StudyId == studyId && p.IsActive && !p.IsDeleted).AsNoTracking().Select(x => new SubjectDTO()
            {
                Id = x.Id,
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

                if (site.MaxEnrolmentCount > subjectsInSite.Count)
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

                var subjectNo = getSubjectNumber(site.CountryCode, site.Name, subjectsInSite.Count, study.SubjectNumberDigitCount);

                var newSubject = new Subject
                {
                    StudyId = study.Id,
                    SubjectNumber = subjectNo,
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

                foreach (var item in study.StudyVisits)
                {
                    var sv = new SubjectVisit
                    {
                        SubjectId = newSubject.Id,
                        StudyVisitId = item.Id,
                    };

                    _context.SubjectVisits.Add(sv);

                    foreach (var pgs in item.StudyVisitPages)
                    {
                        var pg = new SubjectVisitPage
                        {
                            StudyVisitPageId = pgs.Id,
                            SubjectVisitId = sv.Id,
                        };

                        _context.SubjectVisitPages.Add(pg);
                    }
                }
            }

            return new ApiResponse<dynamic>
            {
                IsSuccess = true,
                Message = "Successful",
            };
        }

        private string getSubjectNumber(string countryCode, string site, int subjectNumberInSite, int subjectNumberDigitCOunt = 4)
        {
            var subjectNumber = "";

            if (subjectNumberDigitCOunt != 4)
            {
                subjectNumber = countryCode + site + subjectNumberInSite.ToString("D" + subjectNumberDigitCOunt);
            }
            else
            {
                subjectNumber = countryCode + site + subjectNumberInSite.ToString("D4");
            }

            return subjectNumber;
        }
    }
}
