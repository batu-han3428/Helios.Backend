using Helios.Caching.Services.Interfaces;
using Helios.Caching.Services;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;

namespace Helios.Caching.Helpers
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddRedisCache(this IServiceCollection services, IConfiguration configuration)
        {
            var redisConnectionString = configuration.GetConnectionString("Redis");
            services.AddStackExchangeRedisCache(options =>
            {
                options.Configuration = redisConnectionString;
                options.InstanceName = "HeliosCacheInstance";
            });

            services.AddHttpContextAccessor();
            services.AddMemoryCache();
            services.AddScoped<IRedisCacheService, RedisCacheService>();

            return services;
        }
    }
}
