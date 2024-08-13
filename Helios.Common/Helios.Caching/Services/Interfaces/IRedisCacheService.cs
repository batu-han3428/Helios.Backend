using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Helios.Caching.Services.Interfaces
{
    public interface IRedisCacheService
    {
        Task SetAsync<T>(string key, T value, TimeSpan? expiration = null);
        Task<T> GetAsync<T>(string key);
        Task RemoveAsync(string key);
    }
}
