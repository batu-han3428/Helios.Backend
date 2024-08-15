using Helios.Caching.Services.Interfaces;
using Helios.Caching.Services;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Configuration;
using StackExchange.Redis;

namespace Helios.Caching.Helpers
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddRedisCache(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddStackExchangeRedisCache(options =>
            {
                options.Configuration = configuration.GetConnectionString("Redis");
                options.InstanceName = "Helios:";
            });

            services.AddScoped<IRedisCacheService, RedisCacheService>();

            return services;
        }
    }
}
