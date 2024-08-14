using Helios.Caching.Helpers;
using Helios.Caching.Services.Interfaces;
using Helios.Common.Model;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Caching.Memory;
using StackExchange.Redis;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace Helios.Caching.Services
{
    public class RedisCacheService : IRedisCacheService
    {
        private readonly IDistributedCache _cache;
        private readonly IMemoryCache _localCache;
        private IDatabase _db;

        public RedisCacheService(IDistributedCache cache, IMemoryCache localCache)
        {
            _cache = cache;
            _localCache = localCache;
            _db = ConnectionHelper.Connection.GetDatabase();
        }

        public async Task SetAsync<T>(string key, T value, TimeSpan? expiration = null)
        {
            //var options = new DistributedCacheEntryOptions();
            //if (expiration.HasValue)
            //{
            //    options.SetAbsoluteExpiration(expiration.Value);
            //}

            //var jsonValue = JsonSerializer.Serialize(value);
            //await _cache.SetStringAsync(key, jsonValue, options);
            _localCache.Set(key, value, new TimeSpan(100, 0, 0));
            //var isSet = _db.StringSet(key, JsonSerializer.Serialize(value), new TimeSpan(100, 0, 0));
        }

        public async Task<T> GetAsync<T>(string key)
        {
            try
            {
                //var jsonValue = await _cache.GetStringAsync(key);
                //var value = _db.StringGet(key);
                bool v = _localCache.TryGetValue(key, out List<T> value);

                //if (!string.IsNullOrEmpty(value))
                //{
                //    return JsonSerializer.Deserialize<T>(value);
                //}

                if (value == null)
                {
                    return default;
                }

                //return JsonSerializer.Deserialize<T>(jsonValue);
            }
            catch (Exception ex)
            {

            }

            return default;
        }

        public async Task RemoveAsync(string key)
        {
            await _cache.RemoveAsync(key);
        }
    }
}
