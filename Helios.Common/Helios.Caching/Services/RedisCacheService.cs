using Helios.Caching.Services.Interfaces;
using Microsoft.Extensions.Caching.Distributed;
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

        public RedisCacheService(IDistributedCache cache)
        {
            _cache = cache;
        }

        public async Task SetAsync<T>(string key, T value, TimeSpan? expiration = null)
        {
            var options = new DistributedCacheEntryOptions();
            if (expiration.HasValue)
            {
                options.SetAbsoluteExpiration(expiration.Value);
            }

            var jsonValue = JsonSerializer.Serialize(value);
            await _cache.SetStringAsync(key, jsonValue, options);
        }

        public async Task<T> GetAsync<T>(string key)
        {
            var jsonValue = await _cache.GetStringAsync(key);
            if (jsonValue == null)
            {
                return default;
            }

            return JsonSerializer.Deserialize<T>(jsonValue);
        }

        public async Task RemoveAsync(string key)
        {
            await _cache.RemoveAsync(key);
        }
    }
}
