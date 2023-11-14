using Helios.Common.DTO;
using Helios.Common.Model;
using Helios.Core.Domains.Entities;
using Helios.eCRF.Models;
using Helios.eCRF.Services.Base;
using Helios.eCRF.Services.Interfaces;
using MassTransit.Internals.GraphValidation;
using Newtonsoft.Json;
using RestSharp;

namespace Helios.eCRF.Services
{
    public class StudyService : ApiBaseService, IStudyService
    {
        public StudyService(IConfiguration configuration) : base(configuration)
        {
        }

        #region Study
        public async Task<List<StudyDTO>> GetStudyList(bool isLock)
        {
            using (var client = CoreServiceClient)
            {
                var req = new RestRequest("CoreStudy/GetStudyList", Method.Get);
                req.AddParameter("isLock", isLock);
                var result = await client.ExecuteAsync<List<StudyDTO>>(req);
                return result.Data;
            }
        }

        public async Task<StudyDTO> GetStudy(Guid studyId)
        {
            using (var client = CoreServiceClient)
            {
                var req = new RestRequest("CoreStudy/GetStudy", Method.Get);
                req.AddParameter("studyId", studyId);
                var result = await client.ExecuteAsync<StudyDTO>(req);
                return result.Data;
            }
        }

        public async Task<ApiResponse<dynamic>> StudySave(StudyModel studyModel)
        {
            using (var client = CoreServiceClient)
            {
                var req = new RestRequest("CoreStudy/StudySave", Method.Post);
                req.AddJsonBody(studyModel);
                var result = await client.ExecuteAsync<ApiResponse<dynamic>>(req);
                return result.Data;
            }
        }

        public async Task<ApiResponse<dynamic>> StudyLockOrUnlock(StudyLockDTO studyLockDTO)
        {
            using (var client = CoreServiceClient)
            {
                var req = new RestRequest("CoreStudy/StudyLockOrUnlock", Method.Post);
                req.AddJsonBody(studyLockDTO);
                var result = await client.ExecuteAsync<ApiResponse<dynamic>>(req);
                return result.Data;
            }
        }
        #endregion

        #region Site
        public async Task<List<SiteDTO>> GetSiteList(Guid studyId)
        {
            using (var client = CoreServiceClient)
            {
                var req = new RestRequest("CoreStudy/GetSiteList", Method.Get);
                req.AddParameter("studyId", studyId);
                var result = await client.ExecuteAsync<List<SiteDTO>>(req);
                return result.Data;
            }
        }

        public async Task<SiteDTO> GetSite(Guid siteId)
        {
            using (var client = CoreServiceClient)
            {
                var req = new RestRequest("CoreStudy/GetSite", Method.Get);
                req.AddParameter("siteId", siteId);
                var result = await client.ExecuteAsync<SiteDTO>(req);
                return result.Data;
            }
        }

        public async Task<ApiResponse<dynamic>> SiteSaveOrUpdate(SiteModel siteModel)
        {
            using (var client = CoreServiceClient)
            {
                var req = new RestRequest("CoreStudy/SiteSaveOrUpdate", Method.Post);
                req.AddJsonBody(siteModel);
                var result = await client.ExecuteAsync<ApiResponse<dynamic>>(req);
                return result.Data;
            }
        }

        public async Task<ApiResponse<dynamic>> SiteDelete(SiteModel siteModel)
        {
            using (var client = CoreServiceClient)
            {
                var req = new RestRequest("CoreStudy/SiteDelete", Method.Post);
                req.AddJsonBody(siteModel);
                var result = await client.ExecuteAsync<ApiResponse<dynamic>>(req);
                return result.Data;
            }
        }
        #endregion
    }
}
