using Helios.Common.DTO;
using Helios.Common.Enums;
using Helios.Common.Model;
using Helios.Core.Contexts;
using Helios.Core.Domains.Entities;
using Helios.Core.Services.Base;
using Helios.Core.Services.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using Newtonsoft.Json;
using StackExchange.Redis;

namespace Helios.Core.Services
{
    public class CacheService :BaseService, ICacheService
    {
        private readonly IConnectionMultiplexer _redisCache;
        private readonly IMemoryCache _localCache;
        private CoreContext _context;
        //private IDatabase _db;
        public CacheService(IConnectionMultiplexer redisCache, IMemoryCache localCache, CoreContext context, IConfiguration config) : base(context, config)
        {
            _redisCache = redisCache;
            _localCache = localCache;
            _context = context;
        }

        public string redisKeyStudyPage(Int64 studyId, Int64 pageId)
        {
            string preFix = "page";
            return "";

            //return string("{preFix}:{studyId}:{pageId}");
        }

        public async Task<ApiResponse<dynamic>> SetSubjectDetailMenu(Int64 studyId)
        {
            var response = new ApiResponse<dynamic>();

            try
            {
                var result = await _context.StudyVisits.Where(x => x.StudyId == studyId && x.IsActive && !x.IsDeleted)
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

                string prefix = "Study";
                var localCacheKey = prefix + ":" + studyId;

                if (result != null && result.Count > 0)
                {
                    _localCache.Set(localCacheKey, result, new TimeSpan(0, 10, 0));
                }

            }
            catch (Exception ex)
            {
                response = new ApiResponse<dynamic>
                {
                    IsSuccess = false,
                    Message = "Operation failed. Please try again."
                };
            }

            return response;
        }

        public async Task<List<SubjectDetailMenuModel>> GetSubjectDetailMenu(Int64 studyId)
        {
            string prefix = "Study";
            var localCacheKey = prefix + ":" + studyId;

            if (_localCache.TryGetValue(localCacheKey, out List<SubjectDetailMenuModel> SubjectDetailMenu))
            {
                return SubjectDetailMenu;
            }

            return null;
        }

        public T GetData<T>(string key)
        {
            //var value = _db.StringGet(key);
            //if (!string.IsNullOrEmpty(value))
            //{
            //    return JsonConvert.DeserializeObject<T>(value);
            //}
            return default;
        }

        public bool SetData<T>(string key, T value, DateTimeOffset expirationTime)
        {
            TimeSpan expiryTime = expirationTime.DateTime.Subtract(DateTime.Now);
            //var isSet = _db.StringSet(key, JsonConvert.SerializeObject(value), expiryTime);
            //return isSet;
            return false;
        }

        public object RemoveData(string key)
        {
            //bool _isKeyExist = _db.KeyExists(key);
            //if (_isKeyExist == true)
            //{
            //    return _db.KeyDelete(key);
            //}
            return false;
        }

        private void Dispose(bool disposing)
        {
            if (!disposing) return;
            if (_context != null)
            {
                _context.Dispose();
            }

        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
    }
}
