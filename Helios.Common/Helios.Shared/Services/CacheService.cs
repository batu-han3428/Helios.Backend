using Helios.Shared.Helpers;
using Helios.Shared.Services.Interfaces;
using StackExchange.Redis;
using System.Text.Json;

namespace Helios.Shared.Services
{
    public class CacheService : ApiBaseService, ICacheService
    {
        private IDatabase _db;

        public CacheService(IConfiguration configuration, IHttpContextAccessor httpContextAccessor) : base(configuration, httpContextAccessor)
        {
            _db = ConnectionHelper.Connection.GetDatabase();
        }

        public bool SetData<T>(string key, T value, DateTimeOffset expirationTime)
        {
            TimeSpan expiryTime = expirationTime.DateTime.Subtract(DateTime.Now);
            var isSet = _db.StringSet(key, JsonSerializer.Serialize(value), expiryTime);
            return isSet;
        }

        public T GetData<T>(string key)
        {
            var value = _db.StringGet(key);

            if (!string.IsNullOrEmpty(value))
            {
                return JsonSerializer.Deserialize<T>(value);
            }

            return default;
        }

        public object RemoveData(string key)
        {
            bool _isKeyExist = _db.KeyExists(key);

            if (_isKeyExist == true)
            {
                return _db.KeyDelete(key);
            }
            return false;
        }

    }
}
