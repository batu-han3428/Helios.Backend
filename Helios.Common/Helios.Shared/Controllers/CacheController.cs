using Helios.Common.DTO;
using Helios.Common.Model;
using Helios.Shared.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using StackExchange.Redis;

namespace Helios.Shared.Controllers
{
    [ApiController]
    [Route("[controller]/[action]")]
    public class CacheController : Controller
    {
        //private readonly IConnectionMultiplexer _redisCache;
        private readonly IMemoryCache _localCache;
        private readonly ICacheService _cacheService;

        public CacheController(IMemoryCache localCache, ICacheService cacheService)
        {
            //_redisCache = redisCache;
            _localCache = localCache;
            _cacheService = cacheService;
        }

        [HttpPost]
        public ApiResponse<dynamic> SetSubjectDetailMenu(SubjectMenuModel model)
        {
            var response = new ApiResponse<dynamic>();

            string prefix = "Study:Menu";
            var localCacheKey = prefix + ":" + model.StudyId;

            if (model.DetailMenuModels != null)
            {
                _localCache.Set(localCacheKey, model.DetailMenuModels, new TimeSpan(100, 0, 0));
            }

            //var expireDate = new DateTimeOffset(DateTime.UtcNow.AddDays(90));
            //response.IsSuccess = _cacheService.SetData(localCacheKey, model.DetailMenuModels, expireDate);

            return response;
        }

        [HttpGet]
        public List<SubjectDetailMenuModel> GetSubjectDetailMenu(Int64 studyId)
        {
            string prefix = "Study:Menu";
            var localCacheKey = prefix + ":" + studyId;

            //var result = _cacheService.GetData<List<SubjectDetailMenuModel>>(localCacheKey);

            //return result;
            bool v = _localCache.TryGetValue(localCacheKey, out List<SubjectDetailMenuModel> SubjectDetailMenu);

            if (SubjectDetailMenu != null)
            {
                return SubjectDetailMenu;
            }

            return null;
        }

        [HttpPost]
        public ApiResponse<dynamic> RemoveSubjectDetailMenu(Int64 studyId)
        {
            var response = new ApiResponse<dynamic>();
            string prefix = "Study:Menu";

            var localCacheKey = prefix + ":" + studyId;
            _localCache.Remove(localCacheKey);

            return response;
        }

        [HttpPost]
        public ApiResponse<dynamic> SetUserPermissions(UserPermissionCacheModel model)
        {
            var response = new ApiResponse<dynamic>();

            string prefix = "Study:Permissions";
            var localCacheKey = prefix + ":" + model.StudyId;

            if (model.UserPermissionModel != null)
            {
                _localCache.Set(localCacheKey, model.UserPermissionModel, new TimeSpan(100, 0, 0));
            }

            return response;
        }

        [HttpGet]
        public UserPermissionCacheModel GetUserPermissions(Int64 studyId)
        {
            string prefix = "Study:Permissions";
            var localCacheKey = prefix + ":" + studyId;

            bool v = _localCache.TryGetValue(localCacheKey, out UserPermissionCacheModel userPermissions);

            if (userPermissions != null)
            {
                return userPermissions;
            }

            return null;
        }

        [HttpPost]
        public ApiResponse<dynamic> RemoveUserPermissions(Int64 studyId)
        {
            var response = new ApiResponse<dynamic>();
            string prefix = "Study:Permissions";

            var localCacheKey = prefix + ":" + studyId;
            _localCache.Remove(localCacheKey);

            return response;
        }

    }
}
